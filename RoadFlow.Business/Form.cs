using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Model.FlowRunModel;
using RoadFlow.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace RoadFlow.Business
{


  
    public class Form
    {
        // Fields
        private readonly RoadFlow.Data.Form formData = new RoadFlow.Data.Form();

        // Methods
        public int Add(RoadFlow.Model.Form form)
        {
            return this.formData.Add(form);
        }


        /// <summary>
        /// 删除和应用程序库
        /// </summary>
        /// <param name="form"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public int DeleteAndApplibrary(RoadFlow.Model.Form form, int delete = 0)
        {
            return this.formData.Delete(form, new AppLibrary().GetByCode(form.Id.ToString()), delete);
        }


        /// <summary>
        /// 删除表单数据
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public string DeleteFormData(Guid formId, string instanceId)
        {
            List<ValueTuple<Guid, string, int, string, string>> formTables = this.GetFormTables(formId);
            if ((formTables == null) || (formTables.Count == 0))
            {
                return "表单未绑定表!";
            }
            List<ValueTuple<string, IEnumerable<object>>> ps = new List<ValueTuple<string, IEnumerable<object>>>();
            foreach (ValueTuple<Guid, string, int, string, string> local1 in formTables)
            {
                string str2 = local1.Item2;
                int num = local1.Item3;
                string str3 = local1.Item4;
                string str4 = local1.Item5;
                if (1 == num)
                {
                    List<object> list1=new List<object>();
                    string[] textArray1 = new string[] { "DELETE FROM ", str2, " WHERE ", str3, "={0}" };
                    string str5 = string.Concat((string[])textArray1);
                    list1 = new List<object> {
                    instanceId,
                    new ValueTuple<string, IEnumerable<object>>(str5, (IEnumerable<object>)list1)
                };
                }
                else
                {
                    List<object> list3 =new List<object>();
                    string[] textArray2 = new string[] { "DELETE FROM ", str2, " WHERE ", str4, "={0}" };
                    string str6 = string.Concat((string[])textArray2);
                    list3 = new List<object> {
                    instanceId,
                    new ValueTuple<string, IEnumerable<object>>(str6, (IEnumerable<object>) list3)
                };
                }
            }
            Guid id = Enumerable.First<ValueTuple<Guid, string, int, string, string>>((IEnumerable<ValueTuple<Guid, string, int, string, string>>)formTables).Item1;
            string str = new DbConnection().ExecuteSQL(id, ps);
            if (!str.IsInt())
            {
                return str;
            }
            return "1";
        }

        public RoadFlow.Model.Form Get(Guid id)
        {
            return this.formData.Get(id);
        }

        public List<RoadFlow.Model.Form> GetAll()
        {
            return this.formData.GetAll();
        }


        /// <summary>
        /// 获取下一级选项
        /// </summary>
        /// <param name="source"></param>
        /// <param name="connId"></param>
        /// <param name="text"></param>
        /// <param name="parentValue"></param>
        /// <param name="dictValueField"></param>
        /// <param name="dictId"></param>
        /// <param name="defaultValue"></param>
        /// <param name="dictIschild"></param>
        /// <returns></returns>
        public string GetChildOptions(string source, string connId, string text, string parentValue, string dictValueField, string dictId, string defaultValue, bool dictIschild)
        {
            if (parentValue.IsNullOrWhiteSpace())
            {
                return "";
            }
            if (source != "dict")
            {
                if (source == "sql")
                {
                    return this.GetOptionsBySQL(connId, Wildcard.Filter(text.FilterSelectSql(), null, null), defaultValue);
                }
                if (source == "url")
                {
                    return this.GetOptionsByUrl(Wildcard.Filter(text, null, null));
                }
                return "";
            }
            Dictionary dictionary = new Dictionary();
            Guid id = parentValue.ToGuid();
            ValueField valueField = ValueField.Id;
            if (dictValueField != "code")
            {
                if (dictValueField == "value")
                {
                    valueField = ValueField.Value;
                    if (!dictValueField.EqualsIgnoreCase("id"))
                    {
                        RoadFlow.Model.Dictionary dictionary3 = dictionary.GetChilds(dictId.ToGuid()).Find(delegate (RoadFlow.Model.Dictionary p) {
                            return p.Value.EqualsIgnoreCase(parentValue);
                        });
                        if (dictionary3 != null)
                        {
                            id = dictionary3.Id;
                        }
                    }
                }
                else if (dictValueField == "title")
                {
                    valueField = ValueField.Title;
                    if (!dictValueField.EqualsIgnoreCase("id"))
                    {
                        RoadFlow.Model.Dictionary dictionary4 = dictionary.GetChilds(dictId.ToGuid()).Find(delegate (RoadFlow.Model.Dictionary p) {
                            return p.Title.EqualsIgnoreCase(parentValue);
                        });
                        if (dictionary4 != null)
                        {
                            id = dictionary4.Id;
                        }
                    }
                }
                else if (dictValueField == "note")
                {
                    valueField = ValueField.Note;
                    if (!dictValueField.EqualsIgnoreCase("id"))
                    {
                        RoadFlow.Model.Dictionary dictionary5 = dictionary.GetChilds(dictId.ToGuid()).Find(delegate (RoadFlow.Model.Dictionary p) {
                            return p.Note.EqualsIgnoreCase(parentValue);
                        });
                        if (dictionary5 != null)
                        {
                            id = dictionary5.Id;
                        }
                    }
                }
                else if (dictValueField == "other")
                {
                    valueField = ValueField.Other;
                    if (!dictValueField.EqualsIgnoreCase("id"))
                    {
                        RoadFlow.Model.Dictionary dictionary6 = dictionary.GetChilds(dictId.ToGuid()).Find(delegate (RoadFlow.Model.Dictionary p) {
                            return p.Other.EqualsIgnoreCase(parentValue);
                        });
                        if (dictionary6 != null)
                        {
                            id = dictionary6.Id;
                        }
                    }
                }
            }
            else
            {
                valueField = ValueField.Code;
                if (!dictValueField.EqualsIgnoreCase("id"))
                {
                    RoadFlow.Model.Dictionary dictionary2 = dictionary.GetChilds(dictId.ToGuid()).Find(delegate (RoadFlow.Model.Dictionary p) {
                        return p.Code.EqualsIgnoreCase(parentValue);
                    });
                    if (dictionary2 != null)
                    {
                        id = dictionary2.Id;
                    }
                }
            }
            return dictionary.GetOptionsByID(id, valueField, defaultValue, dictIschild, true);
        }


        /// <summary>
        /// 获取表生成Html
        /// </summary>
        /// <param name="widht"></param>
        /// <param name="height"></param>
        /// <param name="dataSource"></param>
        /// <param name="dataSourceText"></param>
        /// <param name="connId"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        public string GetDataTableHtml(string widht, string height, string dataSource, string dataSourceText, string connId, string formData)
        {
            int num;
            if (dataSource.IsInt(out num))
            {
                int[] digits = new int[3];
                digits[1] = 1;
                digits[2] = 2;
                if (!num.NotIn(digits) && !dataSourceText.IsNullOrWhiteSpace())
                {
                    Guid guid;
                    DataTable table;
                    JArray array = null;
                    try
                    {
                        array = JArray.Parse(formData);
                    }
                    catch
                    {
                        array = null;
                    }
                    dataSourceText = Wildcard.Filter(dataSourceText, null, array);
                    switch (num)
                    {
                        case 1:
                            {
                                string url = string.Empty;
                                if (!dataSourceText.Trim1().StartsWith("http"))
                                {
                                    url = Tools.GetHttpHost(null) + dataSourceText;
                                }
                                else
                                {
                                    url = dataSourceText;
                                }
                                try
                                {
                                    return HttpHelper.HttpPost(url, "", null, null, 0, null);
                                }
                                catch
                                {
                                    return string.Empty;
                                }
                                break;
                            }
                        case 2:
                            try
                            {
                                object[] args = new object[] { array };
                                ValueTuple<object, Exception> tuple = Tools.ExecuteMethod(dataSourceText, args);
                                if ((tuple.Item2 == null) && (tuple.Item1 != null))
                                {
                                    return tuple.Item1.ToString();
                                }
                                return string.Empty;
                            }
                            catch
                            {
                                return string.Empty;
                            }
                            break;
                    }
                    if ((num != 0) || !connId.IsGuid(out guid))
                    {
                        return string.Empty;
                    }
                    StringBuilder builder = new StringBuilder();
                    try
                    {
                        table = new DbConnection().GetDataTable(guid, dataSourceText, (DbParameter[])null);
                    }
                    catch
                    {
                        return string.Empty;
                    }
                    builder.Append("<table cellpadding=\"0\" cellspacing=\"1\" border=\"0\" style=\"" + (widht.IsNullOrWhiteSpace() ? "" : ("width:" + widht + ";")) + (height.IsNullOrWhiteSpace() ? "" : ("height:" + height + ";")) + "\" class=\"formlisttable\">");
                    builder.Append("<thead><tr>");
                    foreach (DataColumn column in table.Columns)
                    {
                        builder.Append("<th>" + column.ColumnName + "</th>");
                    }
                    builder.Append("</tr></thead>");
                    builder.Append("<tbody>");
                    foreach (DataRow row in table.Rows)
                    {
                        builder.Append("<tr>");
                        foreach (DataColumn column2 in table.Columns)
                        {
                            builder.Append("<td>" + row[column2.ColumnName].ToString() + "</td>");
                        }
                        builder.Append("</tr>");
                    }
                    builder.Append("</tbody>");
                    builder.Append("</table>");
                    return builder.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取默认值json
        /// </summary>
        /// <param name="defaultJSON"></param>
        /// <param name="fieldStatusJSON"></param>
        /// <returns></returns>
        public string GetDefaultValuesJSON(string defaultJSON, string fieldStatusJSON)
        {
            if (defaultJSON.IsNullOrWhiteSpace())
            {
                return "[]";
            }
            JArray array = null;
            try
            {
                array = JArray.Parse(defaultJSON);
            }
            catch
            {
                return "[]";
            }
            JArray array2 = null;
            try
            {
                array2 = JArray.Parse(fieldStatusJSON);
            }
            catch
            {
            }
            JArray array3 = new JArray();
            foreach (JObject obj2 in array)
            {
                string str2 = obj2.Value<string>("id");
                int num = 0;
                if (array2 != null)
                {
                    foreach (JObject obj4 in array2)
                    {
                        if (str2.Equals(obj4.Value<string>("name")))
                        {
                            num = obj4.Value<string>("status").ToInt(0);
                        }
                    }
                }
                if (2 != num)
                {
                    JObject obj6 = new JObject();
                    obj6.Add("id", (JToken)str2);
                    obj6.Add("value", (JToken)Wildcard.Filter(obj2.Value<string>("value"), null, null));
                    JObject obj3 = obj6;
                    array3.Add(obj3);
                }
            }
            return array3.ToString(0, Array.Empty<JsonConverter>());
        }
        /// <summary>
        /// 获取导出表单字符串
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public string GetExportFormString(string ids)
        {
            if (ids.IsNullOrWhiteSpace())
            {
                return "";
            }
            char[] separator = new char[] { ',' };
            JObject obj2 = new JObject();
            JArray array = new JArray();
            JArray array2 = new JArray();
            string[] strArray = ids.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                Guid guid;
                if (strArray[i].IsGuid(out guid))
                {
                    RoadFlow.Model.Form form = this.Get(guid);
                    if (form != null)
                    {
                        array.Add(JObject.FromObject(form));
                        RoadFlow.Model.AppLibrary byCode = new AppLibrary().GetByCode(form.Id.ToString());
                        if (byCode != null)
                        {
                            array2.Add(JObject.FromObject(byCode));
                        }
                    }
                }
            }
            obj2.Add("forms", array);
            obj2.Add("applibrarys", array2);
            return obj2.ToString();
        }

        public string GetFormData(string connId, string tableName, string primaryKey, string instanceId, string stepId, string flowId, string formatJSON, out string fieldStatusJSON)
        {
            Guid guid;
            Guid guid2;
            fieldStatusJSON = "[]";
            if (!connId.IsGuid(out guid))
            {
                return "[]";
            }
            RoadFlow.Model.DbConnection dbconnectionModel = new DbConnection().Get(guid);
            if (dbconnectionModel == null)
            {
                return "[]";
            }
            List<StepFieldStatus> stepFieldStatuses = new List<StepFieldStatus>();
            if (flowId.IsGuid(out guid2))
            {
                RoadFlow.Model.FlowRun flowRunModel = new Flow().GetFlowRunModel(guid2, true);
                if (flowRunModel != null)
                {
                    Guid guid3;
                    Guid stepGuid = stepId.IsGuid(out guid3) ? guid3 : flowRunModel.FirstStepId;
                    Step step = flowRunModel.Steps.Find(delegate (Step p) {
                        return p.Id == stepGuid;
                    });
                    if (step != null)
                    {
                        JArray array3 = new JArray();
                        stepFieldStatuses = step.StepFieldStatuses;
                        foreach (StepFieldStatus status in stepFieldStatuses)
                        {
                            char[] separator = new char[] { '.' };
                            string[] strArray = status.Field.Split(separator);
                            if (strArray.Length == 3)
                            {
                                JObject obj1 = new JObject();
                                obj1.Add("name", (JToken)(strArray[1] + "-" + strArray[2]).ToUpper());
                                obj1.Add("status", (JToken)status.Status);
                                obj1.Add("check", (JToken)status.Check);
                                JObject obj2 = obj1;
                                array3.Add(obj2);
                            }
                        }
                        fieldStatusJSON = array3.ToString(0, Array.Empty<JsonConverter>());
                    }
                }
            }
            else
            {
                Guid guid4;
                if (Tools.HttpContext.Request.Querys("programid").IsGuid(out guid4))
                {
                    RoadFlow.Model.ProgramRun runModel = new Program().GetRunModel(guid4);
                    if (runModel != null)
                    {
                        JArray array4 = new JArray();
                        foreach (RoadFlow.Model.ProgramValidate validate in runModel.ProgramValidates)
                        {
                            JObject obj6 = new JObject();
                            obj6.Add("name", (JToken)(validate.TableName + "-" + validate.FieldName).ToUpper());
                            obj6.Add("status", (JToken)validate.Status);
                            obj6.Add("check", (JToken)validate.Validate);
                            JObject obj3 = obj6;
                            array4.Add(obj3);
                        }
                        fieldStatusJSON = array4.ToString(0, Array.Empty<JsonConverter>());
                    }
                }
            }
            JArray array = null;
            try
            {
                array = JArray.Parse(formatJSON);
            }
            catch
            {
            }
            DataTable table = this.GetFormDataTable(dbconnectionModel, tableName, primaryKey, instanceId);
            if (table.Rows.Count == 0)
            {
                return "[]";
            }
            JArray array2 = new JArray();
            IEnumerator enumerator3 = table.Columns.GetEnumerator();
            {
                while (enumerator3.MoveNext())
                {
                    DataColumn column = (DataColumn)enumerator3.Current;
                    StepFieldStatus status2 = stepFieldStatuses.Find(delegate (StepFieldStatus p) {
                        object[] objArray1 = new object[] { dbconnectionModel.Id, ".", tableName, ".", column.ColumnName };
                        return p.Field.EqualsIgnoreCase(string.Concat((object[])objArray1));
                    });
                    if ((status2 == null) || (status2.Status != 2))
                    {
                        string str = (tableName + "-" + column.ColumnName).ToUpper();
                        string str2 = table.Rows[0][column.ColumnName].ToString();
                        if (array != null)
                        {
                            foreach (JObject obj5 in array)
                            {
                                if (str.Equals(obj5.Value<string>("id")))
                                {
                                    string str3 = obj5.Value<string>("type");
                                    string str4 = obj5.Value<string>("format");
                                    if (!str4.IsNullOrWhiteSpace() && (str3 == "datetime"))
                                    {
                                        DateTime time;
                                        str2 = str2.IsDateTime(out time) ? time.ToString(str4) : "";
                                    }
                                }
                            }
                        }
                        JObject obj8 = new JObject();
                        obj8.Add("name", (JToken)str);
                        obj8.Add("value", (JToken)str2);
                        JObject obj4 = obj8;
                        array2.Add(obj4);
                    }
                }
            }
            return array2.ToString(0, Array.Empty<JsonConverter>());
        }

        public DataTable GetFormDataTable(RoadFlow.Model.DbConnection dbConnectionModel, string tableName, string primaryKey, string instanceId)
        {
            return new DbConnection().GetDataTable(dbConnectionModel, tableName, primaryKey, instanceId, "");
        }

        public List<ValueTuple<Guid, string, int, string, string>> GetFormTables(Guid id)
        {
            List<ValueTuple<Guid, string, int, string, string>> list = new List<ValueTuple<Guid, string, int, string, string>>();
            RoadFlow.Model.Form form = this.Get(id);
            if (form != null)
            {
                JObject obj2 = null;
                try
                {
                    obj2 = JObject.Parse(form.attribute);
                }
                catch
                {
                }
                Guid empty = Guid.Empty;
                if (obj2 != null)
                {
                    empty = obj2.Value<string>("dbConn").ToGuid();
                    string str = obj2.Value<string>("dbTable");
                    string str2 = obj2.Value<string>("dbTablePrimaryKey");
                    list.Add(new ValueTuple<Guid, string, int, string, string>(empty, str, 1, str2, string.Empty));
                }
                JArray array = null;
                try
                {
                    array = JArray.Parse(form.SubtableJSON);
                }
                catch
                {
                }
                if (array == null)
                {
                    return list;
                }
                foreach (JObject obj4 in array)
                {
                    string str3 = obj4.Value<string>("secondtable");
                    string str4 = obj4.Value<string>("secondtableprimarykey");
                    string str5 = obj4.Value<string>("secondtablerelationfield");
                    list.Add(new ValueTuple<Guid, string, int, string, string>(empty, str3, 0, str4, str5));
                }
            }
            return list;
        }

        public string GetOptionsBySQL(string connId, string sql, string value)
        {
            Guid guid;
            DataTable table;
            if (!connId.IsGuid(out guid) || sql.IsNullOrWhiteSpace())
            {
                return "";
            }
            DbConnection connection = new DbConnection();
            try
            {
                table = connection.GetDataTable(guid, Wildcard.Filter(sql.FilterSelectSql(), null, null), (DbParameter[])null);

             //   table = connection.GetDataTable(guid, Wildcard.Filter(sql.FilterSelectSql()));
            }
            catch
            {
                table = new DataTable();
            }
            StringBuilder builder = new StringBuilder();
            foreach (DataRow row in table.Rows)
            {
                string str = string.Empty;
                string str2 = string.Empty;
                if (table.Columns.Count > 0)
                {
                    str = row[0].ToString();
                }
                if (table.Columns.Count > 1)
                {
                    str2 = row[1].ToString();
                }
                else
                {
                    str2 = str;
                }
                if (!str.IsNullOrWhiteSpace() && !str2.IsNullOrWhiteSpace())
                {
                    string[] textArray1 = new string[] { "<option value=\"", str, "\"", str.EqualsIgnoreCase(value) ? " select=\"select\"" : "", ">", str2, "</option>" };
                    builder.Append(string.Concat((string[])textArray1));
                }
            }
            return builder.ToString();
        }

        public string GetOptionsByStringExpression(string expression, string defaultValue = "")
        {
            if (expression.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            char[] separator = new char[] { ';' };
            StringBuilder builder = new StringBuilder();
            foreach (string str in expression.Split(separator))
            {
                if (!str.IsNullOrWhiteSpace())
                {
                    char[] chArray2 = new char[] { ',' };
                    string[] strArray2 = str.Split(chArray2);
                    string str2 = strArray2[0];
                    string str3 = (strArray2.Length > 1) ? strArray2[1] : str2;
                    string[] textArray1 = new string[] { "<option value=\"", str3, "\"", str3.Equals(defaultValue) ? " selected='selected'" : "", ">", str2, "</option>" };
                    builder.Append(string.Concat((string[])textArray1));
                }
            }
            return builder.ToString();
        }

        public string GetOptionsByUrl(string url)
        {
            url = Wildcard.Filter(url, null, null);
            if (!url.ContainsIgnoreCase("http") && !url.ContainsIgnoreCase("https"))
            {
                url = Tools.GetHttpHost(null) + url;
            }
            return HttpHelper.HttpGet(url, null, 0);
        }

        public DataTable GetPagerList(out int count, int size, int number, string name, string type, string order, int status = -1)
        {
            return this.formData.GetPagerList(out count, size, number, name, type, order, status);
        }

        public DataTable GetPagerList(out int count, int size, int number, Guid userId, string name, string type, string order, int status = -1)
        {
            return this.formData.GetPagerList(out count, size, number, userId, name, type, order, status);
        }




        public string GetRadioOrCheckboxHtml(int source, string connId, string dictId, string dictValueField, string text, string defaultValue, string type, string name, string otherAttr)
        {
            if (3 == source)
            {
                return this.GetOptionsByUrl(Wildcard.Filter(text, null, null));
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            switch (source)
            {
                case 0:
                    foreach (RoadFlow.Model.Dictionary dictionary2 in new Dictionary().GetChilds(dictId.ToGuid()))
                    {
                        string title = dictionary2.Title;
                        string str2 = string.Empty;
                        if (dictValueField != "id")
                        {
                            if (dictValueField == "code")
                            {
                                goto Label_00D6;
                            }
                            if (dictValueField == "value")
                            {
                                goto Label_00E1;
                            }
                            if (dictValueField == "title")
                            {
                                goto Label_00EC;
                            }
                            if (dictValueField == "note")
                            {
                                goto Label_00F7;
                            }
                            if (dictValueField == "other")
                            {
                                goto Label_0102;
                            }
                        }
                        else
                        {
                            str2 = dictionary2.Id.ToString();
                        }
                        goto Label_010B;
                    Label_00D6:
                        str2 = dictionary2.Code;
                        goto Label_010B;
                    Label_00E1:
                        str2 = dictionary2.Value;
                        goto Label_010B;
                    Label_00EC:
                        str2 = dictionary2.Title;
                        goto Label_010B;
                    Label_00F7:
                        str2 = dictionary2.Note;
                        goto Label_010B;
                    Label_0102:
                        str2 = dictionary2.Other;
                    Label_010B:
                        dictionary.Add(str2, title);
                    }
                    break;

                case 1:
                    {
                        char[] separator = new char[] { ';' };
                        foreach (string str3 in text.Split(separator))
                        {
                            if (!str3.IsNullOrWhiteSpace())
                            {
                                char[] chArray2 = new char[] { ',' };
                                string[] strArray2 = str3.Split(chArray2);
                                string str = string.Empty;
                                string str5 = string.Empty;
                                if (strArray2.Length != 0)
                                {
                                    str5 = strArray2[0];
                                }
                                if (strArray2.Length > 1)
                                {
                                    str = strArray2[1];
                                }
                                else
                                {
                                    str = str5;
                                }
                                if (!str.IsNullOrWhiteSpace() && !str5.IsNullOrWhiteSpace())
                                {
                                    dictionary.Add(str, str5);
                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        Guid guid;
                        DbConnection connection = new DbConnection();
                        if (connId.IsGuid(out guid))
                        {
                            DataTable table = connection.GetDataTable(guid, Wildcard.Filter(text, null, null), (DbParameter[])null);
                            foreach (DataRow row in table.Rows)
                            {
                                string str6 = string.Empty;
                                string str7 = string.Empty;
                                if (table.Columns.Count > 0)
                                {
                                    str6 = row[0].ToString();
                                }
                                if (table.Columns.Count > 1)
                                {
                                    str7 = row[1].ToString();
                                }
                                else
                                {
                                    str7 = str6;
                                }
                                if (!str6.IsNullOrWhiteSpace() && !str7.IsNullOrWhiteSpace())
                                {
                                    dictionary.Add(str6, str7);
                                }
                            }
                        }
                        break;
                    }
            }
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                string[] textArray1 = new string[] { "<input type=\"", type, "\" style=\"vertical-align:middle;\" value=\"", pair.Key, "\" name=\"", name, "\" id=\"", name, "_", pair.Key, "\"" };
                builder.Append(string.Concat((string[])textArray1));
                if (!defaultValue.IsNullOrWhiteSpace() && ("," + defaultValue + ",").ContainsIgnoreCase(("," + pair.Key + ",")))
                {
                    builder.Append(" checked=\"checked\"");
                }
                builder.Append(" " + otherAttr);
                builder.Append("/>");
                string[] textArray2 = new string[] { "<label style=\"vertical-align:middle;\" for=\"", name, "_", pair.Key, "\">", pair.Value, "</label>" };
                builder.Append(string.Concat((string[])textArray2));
            }
            return builder.ToString();
        }

       // [return: TupleElementNames(new string[] { "searialNumber", "maxField", "maxValue" })]
        public ValueTuple<string, string, int> GetSerialNumber(RoadFlow.Model.DbConnection dbConnectionModel, string tableName, string serialnumberConfig)
        {
            JObject obj1 = JObject.Parse(serialnumberConfig);
            string str = obj1.Value<string>("maxfiled");
            string str2 = obj1.Value<string>("length");
            string str3 = obj1.Value<string>("formatstring");
            string str4 = obj1.Value<string>("sqlwhere");
            StringBuilder builder = new StringBuilder();
            string[] textArray1 = new string[] { "SELECT MAX(", str, ") FROM ", tableName, " " };
            builder.Append(string.Concat((string[])textArray1));
            if (!str4.IsNullOrWhiteSpace())
            {
                string str6 = Wildcard.Filter(str4.UrlDecode().Trim(), null, null);
                if (str6.StartsWith("where", (StringComparison)StringComparison.CurrentCultureIgnoreCase))
                {
                    builder.Append(str6);
                }
                else
                {
                    builder.Append("WHERE " + str6);
                }
            }
            int num = 1;
            DataTable table = new DbConnection().GetDataTable(dbConnectionModel, builder.ToString(), (DbParameter[])null);
            if (table.Rows.Count > 0)
            {
                num = table.Rows[0][0].ToString().ToInt(0) + 1;
            }
            string newStr = ((int)num).ToString().PadLeft(str2.ToInt(1), '0');
            return new ValueTuple<string, string, int>(Wildcard.Filter(str3, null, null).ReplaceIgnoreCase("{serialnumber}", newStr), str, num);
        }

        public string GetShowString(object value, string showModel, string format, string sql)
        {
            Guid guid2;
            if (value == null)
            {
                return string.Empty;
            }
            string str = value.ToString();
            uint num = PrivateImplementationDetails.ComputeStringHash(showModel);
            if (num <= 0x6d74fa96)
            {
                if (num <= 0x307f74ba)
                {
                    switch (num)
                    {
                        case 0x1821232a:
                            if (showModel == "dict_value_title")
                            {
                                goto Label_01DF;
                            }
                            return str;

                        case 0x2057306e:
                            if (showModel == "custom")
                            {
                                return Wildcard.Filter(str, null, value);
                            }
                            return str;

                        case 0x307f74ba:
                            if (showModel == "number_format")
                            {
                                decimal num2;
                                if (!str.IsDecimal(out num2))
                                {
                                    return str;
                                }
                                return num2.ToString(format);
                            }
                            return str;
                    }
                    return str;
                }
                switch (num)
                {
                    case 0x4c5cc0f6:
                        if (showModel == "dict_id_title")
                        {
                            Guid guid;
                            if (!str.IsGuid(out guid))
                            {
                                return string.Empty;
                            }
                            return new Dictionary().GetTitle(guid);
                        }
                        return str;

                    case 0x6ce89a22:
                        if (showModel == "organize_id_name")
                        {
                            return new Organize().GetNames(str);
                        }
                        return str;

                    case 0x6d74fa96:
                        if (showModel == "dict_code_title")
                        {
                            return new Dictionary().GetTitle(str);
                        }
                        return str;
                }
                return str;
            }
            if (num <= 0x95852c0c)
            {
                switch (num)
                {
                    case 0x8a597c03:
                        if (showModel == "dict_note_title")
                        {
                            goto Label_01DF;
                        }
                        return str;

                    case 0x91a2eac1:
                        if (showModel == "files_link")
                        {
                            return str.ToFilesShowString(false);
                        }
                        return str;

                    case 0x95852c0c:
                        if (showModel == "files_img")
                        {
                            return str.ToFilesImgString(0, 0);
                        }
                        return str;
                }
                return str;
            }
            switch (num)
            {
                case 0x9edfed31:
                    if (showModel == "dict_other_title")
                    {
                        break;
                    }
                    return str;

                case 0xccde5aaa:
                    if (showModel == "datetime_format")
                    {
                        DateTime time;
                        if (!str.IsDateTime(out time))
                        {
                            return str;
                        }
                        return time.ToString(format);
                    }
                    return str;

                default:
                    if ((num != 0xe68b9c52) || (showModel != "normal"))
                    {
                    }
                    return str;
            }
        Label_01DF:
            if (!format.IsGuid(out guid2))
            {
                return new Dictionary().GetTitle(format, str);
            }
            return new Dictionary().GetTitle(guid2, str);
        }


        /// <summary>
        /// 导入表单
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string ImportForm(string json)
        {
            IEnumerator<JToken> enumerator;
            if (json.IsNullOrWhiteSpace())
            {
                return "要导入的JSON为空!";
            }
            JObject obj2 = null;
            try
            {
                obj2 = JObject.Parse(json);
            }
            catch
            {
                return "json解析错误!";
            }
            JArray array = obj2.Value<JArray>("forms");
            if (array != null)
            {
                using (enumerator = array.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        RoadFlow.Model.Form form = ((JObject)enumerator.Current).ToObject<RoadFlow.Model.Form>();
                        if (form != null)
                        {
                            if (this.Get(form.Id) != null)
                            {
                                this.Update(form);
                            }
                            else
                            {
                                this.Add(form);
                            }
                            if (form.Status == 1)
                            {
                                //导入表单位置
                                string path = Tools.GetContentRootPath() + "/Areas/RoadFlowCore/Views/FormDesigner/form/";
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                FileStream stream1 = File.Open(path + form.Id + ".cshtml", (FileMode)FileMode.OpenOrCreate, (FileAccess)FileAccess.ReadWrite, (FileShare)FileShare.None);
                                stream1.SetLength(0L);
                                StreamWriter writer1 = new StreamWriter((Stream)stream1, Encoding.UTF8);
                                writer1.Write(form.RunHtml);
                                writer1.Close();
                                stream1.Close();
                            }
                        }
                    }
                }
            }
            JArray array2 = obj2.Value<JArray>("applibrarys");
            AppLibrary library = new AppLibrary();
            if (array2 != null)
            {
                using (enumerator = array2.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        RoadFlow.Model.AppLibrary appLibrary = ((JObject)enumerator.Current).ToObject<RoadFlow.Model.AppLibrary>();
                        if (appLibrary != null)
                        {
                            if (library.Get(appLibrary.Id) != null)
                            {
                                library.Update(appLibrary);
                            }
                            else
                            {
                                library.Add(appLibrary);
                            }
                        }
                    }
                }
            }
            return "1";
        }

        public string ReplaceTitleExpression(string titleExpression, string tableName, string instanceId, HttpRequest request)
        {
            titleExpression = Wildcard.Filter(titleExpression, null, null);
            List<string> list = new List<string>();
            char[] separator = new char[] { '{' };
            foreach (string str in titleExpression.Split(separator))
            {
                if (str.Contains("}"))
                {
                    list.Add(str.Substring(0, str.IndexOf("}")));
                }
            }
            foreach (string str2 in list)
            {
                string key = (tableName + "-" + str2).ToUpper();
                string newStr = request.Forms(key);
                if ((!request.Form["rf_serialnumber_config_" + key].ToString().IsNullOrWhiteSpace() || newStr.IsNullOrEmpty()) && (!instanceId.IsNullOrWhiteSpace() && !tableName.IsNullOrWhiteSpace()))

                   // if ((!request.Form["rf_serialnumber_config_" + key].ToString().IsNullOrWhiteSpace() && !instanceId.IsNullOrWhiteSpace()) && !tableName.IsNullOrWhiteSpace())
                {
                    string str5 = request.Form["form_dbconnid"];
                    string primaryKey = request.Form["form_dbtableprimarykey"];
                    DataTable table = this.GetFormDataTable(new DbConnection().Get(str5.ToGuid()), tableName, primaryKey, instanceId);
                    if (table.Rows.Count > 0)
                    {
                        newStr = table.Rows[0][str2].ToString();
                    }
                }
                titleExpression = titleExpression.ReplaceIgnoreCase("{" + str2 + "}", newStr);
            }
            return titleExpression;
        }

        public ValueTuple<string, string> SaveData(HttpRequest request)
        {
            Guid guid;
            int num2;
            string str = request.Forms("form_dbconnid");
            string str2 = request.Forms("form_dbtable");
            string form_dbtableprimarykey = request.Forms("form_dbtableprimarykey");
            string str3 = request.Forms("form_instanceid");
            string str4 = request.Forms("form_fieldstatus");
            if ("1".Equals(request.Forms("form_iscustomeform")))
            {
                return new ValueTuple<string, string>(str3, string.Empty);
            }
            if (str3.IsNullOrWhiteSpace())
            {
                str3 = request.Querys("instanceid");
            }
            if (!str.IsGuid(out guid))
            {
                return new ValueTuple<string, string>(string.Empty, "连接ID为空");
            }
            if (str2.IsNullOrWhiteSpace() || form_dbtableprimarykey.IsNullOrWhiteSpace())
            {
                return new ValueTuple<string, string>(string.Empty, "表名为空");
            }
            DbConnection connection = new DbConnection();
            RoadFlow.Model.DbConnection conn = connection.Get(guid);
            if (conn == null)
            {
                return new ValueTuple<string, string>(string.Empty, "未找到连接实体");
            }
            List<RoadFlow.Model.TableField> tableFields = connection.GetTableFields(conn, str2);
            if (tableFields.Count == 0)
            {
                return new ValueTuple<string, string>(string.Empty, "表没有字段");
            }
            JArray array = null;
            try
            {
                array = JArray.Parse(str4);
            }
            catch
            {
            }
            object obj2 = str3;
            bool isIdentity = false;
            string seqName = string.Empty;
            bool flag2 = str3.IsNullOrWhiteSpace();
            DataTable table = null;
            if (!flag2)
            {
                table = this.GetFormDataTable(conn, str2, form_dbtableprimarykey, str3);
            }
            List<ValueTuple<Dictionary<string, object>, string, string, int>> tuples = new List<ValueTuple<Dictionary<string, object>, string, string, int>>();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (RoadFlow.Model.TableField field in tableFields)
            {
                string str9 = (str2 + "-" + field.FieldName).ToUpper();
                string str10 = request.Form[str9];
                bool flag3 = form_dbtableprimarykey.EqualsIgnoreCase(field.FieldName);
                int num3 = 0;
                if (array != null)
                {
                    foreach (JObject obj3 in array)
                    {
                        if (obj3.Value<string>("name").Equals(str9))
                        {
                            num3 = obj3.Value<int>("status");
                            break;
                        }
                    }
                }
                if (flag3 && (str10 == null))
                {
                    if (!flag2)
                    {
                        dictionary.Add(field.FieldName, obj2);
                    }
                    else if (field.Type.EqualsIgnoreCase("uniqueidentifier") || field.Type.ContainsIgnoreCase("char"))
                    {
                        obj2 = Guid.NewGuid();
                        dictionary.Add(field.FieldName, obj2);
                    }
                    else if ((field.IsIdentity || field.Type.ContainsIgnoreCase("number")) || (field.Type.ContainsIgnoreCase("int") || field.Type.ContainsIgnoreCase("long")))
                    {
                        isIdentity = true;
                        seqName = str2 + "_SEQ";
                    }
                }
                else
                {
                    if ((str10.IsNullOrEmpty() && (num3 == 0)) && ("," + request.Forms("rf_serialnumber") + ",").Contains("," + str9 + ","))
                    {
                        string str11 = request.Forms("rf_serialnumber_config_" + str9);
                        if (!str11.IsNullOrWhiteSpace())
                        {
                            ValueTuple<string, string, int> tuple1 = this.GetSerialNumber(conn, str2, str11);
                            string str12 = tuple1.Item1;
                            string str13 = tuple1.Item2;
                            int num4 = tuple1.Item3;
                            dictionary.Add(field.FieldName, str12);
                            if (!str13.IsNullOrWhiteSpace())
                            {
                                dictionary.Add(str13, (int)num4);
                            }
                            continue;
                        }
                    }
                    if ((num3 == 0) && !dictionary.ContainsKey(field.FieldName))
                    {
                        if (field.Type.ContainsIgnoreCase("date"))
                        {
                            DateTime time;
                            if (str10.IsDateTime(out time))
                            {
                                dictionary.Add(field.FieldName, time);
                            }
                            else if (field.IsNull && !field.IsDefault)
                            {
                                dictionary.Add(field.FieldName, DBNull.Value);
                            }
                        }
                        else if (str10 == null)
                        {
                            if ((table != null) && (table.Rows.Count > 0))
                            {
                                dictionary.Add(field.FieldName, table.Rows[0][field.FieldName]);
                            }
                            else if (field.IsNull && !field.IsDefault)
                            {
                                dictionary.Add(field.FieldName, DBNull.Value);
                            }
                        }
                        else
                        {
                            dictionary.Add(field.FieldName, str10);
                        }
                    }
                }
            }
            if (Enumerable.Any<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>)dictionary, delegate (KeyValuePair<string, object> p) {
                return !p.Key.EqualsIgnoreCase(form_dbtableprimarykey);
            }))
            {
                tuples.Add(new ValueTuple<Dictionary<string, object>, string, string, int>(dictionary, str2, form_dbtableprimarykey, flag2 ? 1 : 2));
            }
            string str6 = request.Forms("SUBTABLE_id");
            if (!str6.IsNullOrEmpty())
            {
                if (flag2 & isIdentity)
                {
                    string str14 = connection.SaveData(conn, tuples, true, seqName);
                    if (str14.IsInt())
                    {
                        obj2 = str14;
                    }
                    else
                    {
                        return new ValueTuple<string, string>(string.Empty, str14);
                    }
                    tuples.Clear();
                }
                char[] separator = new char[] { ',' };
                foreach (string str15 in str6.Split(separator))
                {
                    Func<KeyValuePair<string, object>, bool> s_9__1=null ;
                    string str16 = request.Forms("SUBTABLE_" + str15 + "_secondtable");
                    string str17 = request.Forms("SUBTABLE_" + str15 + "_primarytablefiled");
                    string secondtableprimarykey = request.Forms("SUBTABLE_" + str15 + "_secondtableprimarykey");
                    string str18 = request.Forms("SUBTABLE_" + str15 + "_secondtablerelationfield");
                    if ((!str16.IsNullOrWhiteSpace() && !str17.IsNullOrWhiteSpace()) && (!secondtableprimarykey.IsNullOrWhiteSpace() && !str18.IsNullOrWhiteSpace()))
                    {
                        DataTable table2 = connection.GetDataTable(conn, str16, str18, obj2.ToString(), "");
                        List<RoadFlow.Model.TableField> list3 = connection.GetTableFields(conn, str16);
                        char[] chArray2 = new char[] { ',' };
                        string[] strArray2 = request.Forms(("SUBTABLE_" + str15 + "_rowindex")).Split(chArray2);
                        foreach (string str19 in strArray2)
                        {
                            if (!str19.IsNullOrWhiteSpace())
                            {
                                bool flag4 = table2.Rows.Count == 0;
                                if (!flag4)
                                {
                                    try
                                    {
                                        flag4 = table2.Select(secondtableprimarykey + "='" + str19 + "'").Length == 0;
                                    }
                                    catch
                                    {
                                        flag4 = true;
                                    }
                                }
                                Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
                                foreach (RoadFlow.Model.TableField field2 in list3)
                                {
                                    bool flag5 = secondtableprimarykey.EqualsIgnoreCase(field2.FieldName);
                                    string[] textArray1 = new string[] { "SUBTABLE_", str15, "_", field2.FieldName, "_", str19 };
                                    string str20 = request.Form[string.Concat((string[])textArray1).ToUpper()];
                                    int num7 = 0;
                                    if (array != null)
                                    {
                                        foreach (JObject obj4 in array)
                                        {
                                            if (obj4.Value<string>("name").Equals((str16 + "-" + field2.FieldName).ToUpper()))
                                            {
                                                num7 = obj4.Value<int>("status");
                                                break;
                                            }
                                        }
                                    }
                                    if ((str20 == null) && str18.EqualsIgnoreCase(field2.FieldName))
                                    {
                                        dictionary2.Add(field2.FieldName, obj2);
                                    }
                                    else if ((str20 == null) & flag5)
                                    {
                                        if (!flag4)
                                        {
                                            dictionary2.Add(field2.FieldName, str19);
                                        }
                                        else if (field2.Type.EqualsIgnoreCase("uniqueidentifier") || field2.Type.ContainsIgnoreCase("char"))
                                        {
                                            dictionary2.Add(field2.FieldName, Guid.NewGuid());
                                        }
                                    }
                                    else if ((num7 == 0) && !dictionary2.ContainsKey(field2.FieldName))
                                    {
                                        if (field2.Type.ContainsIgnoreCase("date"))
                                        {
                                            DateTime time2;
                                            if (str20.IsDateTime(out time2))
                                            {
                                                dictionary2.Add(field2.FieldName, time2);
                                            }
                                            else if (field2.IsNull && !field2.IsDefault)
                                            {
                                                dictionary2.Add(field2.FieldName, DBNull.Value);
                                            }
                                        }
                                        else if (str20 == null)
                                        {
                                            if (field2.IsNull && !field2.IsDefault)
                                            {
                                                dictionary2.Add(field2.FieldName, DBNull.Value);
                                            }
                                        }
                                        else
                                        {
                                            dictionary2.Add(field2.FieldName, str20);
                                        }
                                    }
                                }
                                if (Enumerable.Any<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>)dictionary2, s_9__1 ?? (s_9__1 = delegate (KeyValuePair<string, object> p) {
                                    return !p.Key.EqualsIgnoreCase(secondtableprimarykey);
                                })))
                                {
                                    tuples.Add(new ValueTuple<Dictionary<string, object>, string, string, int>(dictionary2, str16, secondtableprimarykey, flag4 ? 1 : 2));
                                }
                            }
                        }
                        foreach (DataRow row in table2.Rows)
                        {
                            if (!Enumerable.Contains<string>(strArray2, row[secondtableprimarykey].ToString(), (IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase))
                            {
                                Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
                                dictionary1.Add(secondtableprimarykey, row[secondtableprimarykey].ToString());
                                Dictionary<string, object> dictionary3 = dictionary1;
                                tuples.Add(new ValueTuple<Dictionary<string, object>, string, string, int>(dictionary3, str16, secondtableprimarykey, 0));
                            }
                        }
                    }
                }
            }
            string str7 = connection.SaveData(conn, tuples, isIdentity, seqName);
            int num = str7.IsInt(out num2) ? num2 : -1;
            if (((obj2 == null) || obj2.ToString().IsNullOrWhiteSpace()) && (num != -1))
            {
                obj2 = ((int)num).ToString();
            }
            return new ValueTuple<string, string>(obj2.ToString().ToUpper(), (num == -1) ? str7 : string.Empty);
        }

        public int Update(RoadFlow.Model.Form form)
        {
            return this.formData.Update(form);
        }
    }


    


   


   


}
