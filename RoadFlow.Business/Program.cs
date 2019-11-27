using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility.Cache;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Localization;

namespace RoadFlow.Business
{


    #region 新的方法2.8.3
    public class Program
    {
        // Fields
        private readonly RoadFlow.Data.Program ProgramData = new RoadFlow.Data.Program();

        // Methods
        public int Add(RoadFlow.Model.Program program)
        {
            return this.ProgramData.Add(program);
        }

        /// <summary>
        /// 通过id获取设计项目数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RoadFlow.Model.Program Get(Guid id)
        {
            return this.ProgramData.Get(id);
        }

        public List<string> GetButtonHtml(RoadFlow.Model.ProgramRun programRunModel, Guid userId, Guid menuId, IStringLocalizer localizer = null)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            StringBuilder builder3 = new StringBuilder();
            if (programRunModel == null)
            {
                return new List<string> { "", "", "" };
            }
            if (Enumerable.Any<RoadFlow.Model.ProgramQuery>((IEnumerable<RoadFlow.Model.ProgramQuery>)programRunModel.ProgramQueries))
            {
                Guid queryButtonId = "A5678AAB-69D8-40C5-9523-B4882A234975".ToGuid();
                if (!programRunModel.ProgramButtons.Exists(delegate (RoadFlow.Model.ProgramButton p) {
                    return p.Id == queryButtonId;
                }))
                {
                    RoadFlow.Model.ProgramButton button1 = new RoadFlow.Model.ProgramButton
                    {
                        Id = queryButtonId,
                        ButtonName = (localizer == null) ? "查&nbsp;询&nbsp;" : localizer["Query"],
                        ProgramId = programRunModel.Id,
                        ClientScript = "query();",
                        IsValidateShow = 0,
                        ShowType = 1,
                        Sort = 0,
                        Ico = "fa-search"
                    };
                    programRunModel.ProgramButtons.Add(button1);
                }
            }
            List<RoadFlow.Model.MenuUser> all = new MenuUser().GetAll();
            Menu menu = new Menu();
            foreach (RoadFlow.Model.ProgramButton button in Enumerable.OrderBy<RoadFlow.Model.ProgramButton, int>((IEnumerable<RoadFlow.Model.ProgramButton>)programRunModel.ProgramButtons, key=>key.Sort))
            {
                if ((1 != button.IsValidateShow) || menu.HasUseButton(menuId, button.Id, userId, all))
                {
                    string str = "fun_" + Guid.NewGuid().ToString("N");
                    string buttonName = button.ButtonName;
                    string ico = button.Ico;
                    if (button.ShowType == 0)
                    {
                        if (ico.IsFontIco())
                        {
                            string[] textArray1 = new string[] { "<a href=\"javascript:void(0);\" onclick=\"", str, "();\"><i class='fa ", ico, "'></i><label>", buttonName, "</label></a>" };
                            builder.Append(string.Concat((string[])textArray1));
                        }
                        else if (!ico.IsNullOrWhiteSpace())
                        {
                            string[] textArray2 = new string[] { "<a href=\"javascript:void(0);\" onclick=\"", str, "();\"><span style=\"background-image:url(", ico, ");\">", buttonName, "</span></a>" };
                            builder.Append(string.Concat((string[])textArray2));
                        }
                        else
                        {
                            string[] textArray3 = new string[] { "<a href=\"javascript:void(0);\" onclick=\"", str, "();\"><label>", buttonName, "</label></a>" };
                            builder.Append(string.Concat((string[])textArray3));
                        }
                        string[] textArray4 = new string[] { "<script type='text/javascript'>function ", str, "(){", Wildcard.Filter(button.ClientScript, null, null), "}</script>" };
                        builder.Append(string.Concat((string[])textArray4));
                    }
                    else if (button.ShowType == 1)
                    {
                        builder2.Append("<button type='button' class='mybutton' style='margin-right:8px;'");
                        builder2.Append(" onclick=\"" + str + "();\"");
                        builder2.Append(">");
                        if (!ico.IsNullOrWhiteSpace())
                        {
                            if (ico.IsFontIco())
                            {
                                builder2.Append("<i class='fa " + ico + "' style='margin-right:3px;'></i>" + buttonName);
                            }
                            else
                            {
                                builder2.Append("<img src=\"" + ico + "\" style='margin-right:3px;vertical-align:middle;'/>" + buttonName);
                            }
                        }
                        else
                        {
                            builder2.Append(buttonName);
                        }
                        builder2.Append("</button>");
                        string[] textArray5 = new string[] { "<script type='text/javascript'>function ", str, "(){", Wildcard.Filter(button.ClientScript, null, null), "}</script>" };
                        builder2.Append(string.Concat((string[])textArray5));
                    }
                    else if (button.ShowType == 2)
                    {
                        if (ico.IsFontIco())
                        {
                            string[] textArray6 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"", button.ClientScript, "\"><i class='fa ", ico, "'></i>", buttonName, "</a>" };
                            builder3.Append(string.Concat((string[])textArray6));
                        }
                        else if (!ico.IsNullOrWhiteSpace())
                        {
                            string[] textArray7 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"", button.ClientScript, "\"><span style=\"background-image:url(", ico, ");\">", buttonName, "</span></a>" };
                            builder3.Append(string.Concat((string[])textArray7));
                        }
                        else
                        {
                            string[] textArray8 = new string[] { "<a href=\"javascript:void(0);\" onclick=\"", button.ClientScript, "\"><label>", buttonName, "</label></a>" };
                            builder3.Append(string.Concat((string[])textArray8));
                        }
                    }
                }
            }
            return new List<string> { builder.ToString(), builder2.ToString(), builder3.ToString() };
        }



        /// <summary>
        /// 项目字段设置，需要优化
        /// </summary>
        /// <param name="programFields"></param>
        /// <returns></returns>
        private string GetColModels(List<RoadFlow.Model.ProgramField> programFields)
        {
            if (programFields == null)
            {
                return string.Empty;
            }
            JArray array = new JArray();
            foreach (RoadFlow.Model.ProgramField field in programFields)
            {
                JObject obj2 = new JObject();
                string str = field.Field;
                if (str.IsNullOrWhiteSpace())
                {
                    if (100 == field.ShowType)
                    {
                        str = "opation";
                    }
                    else if (1 == field.ShowType)
                    {
                        str = "rowserialnumber";
                    }
                }

                obj2.Add("name", (JToken)str);

                //排序是否为空
                if (!field.IsSort.IsNullOrWhiteSpace())
                {
                    obj2.Add("index", (JToken)field.IsSort.Trim());
                }
                else
                {
                    if(field.IsDefaultSort == 1)
                    {
                        obj2.Add("index", (JToken)field.Field.Trim());
                    }
                    else
                    {
                        obj2.Add("sortable", false);
                    }
                   
                }

                

                if (field.ShowType == 100)
                {
                    obj2.Add("title", 0);
                }
                if (!field.Width.IsNullOrWhiteSpace())
                {
                    obj2.Add("width", (JToken)field.Width.GetNumber());
                }
                obj2.Add("align", (JToken)field.Align);

                //冻结设置
                if (field.IsFreeze == 1)
                {
                    obj2.Add("frozen", true);
                }
                //运算类型设置
                if(!field.SummaryType.IsNullOrEmpty())
                {
                    obj2.Add("summaryType", (JToken)field.SummaryType);
                    if(!field.SummaryTpl.IsNullOrEmpty())
                    {
                        obj2.Add("summaryTpl", (JToken)field.SummaryTpl);
                    }

                }

                //设置格式化
                if(!field.Formatter.IsNullOrEmpty())
                {
                    obj2.Add("formatter", (JToken)field.Formatter);
                    if(!field.FormatOptions.IsNullOrEmpty())
                    {
                        if (field.Formatter != "select")
                        {
                            obj2.Add("formatoptions", (JToken)field.FormatOptions);
                        }
                        else
                        {
                            obj2.Add("editoptions", (JToken)field.FormatOptions);
                        }
                    }
                   
                }

                array.Add(obj2);
            }
            return array.ToString(0, Array.Empty<JsonConverter>());
        }



        private string GetColNames(List<RoadFlow.Model.ProgramField> programFields)
        {
            if (programFields == null)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            foreach (RoadFlow.Model.ProgramField field in programFields)
            {
                builder.Append("\"" + (field.ShowTitle.IsNullOrWhiteSpace() ? field.Field : field.ShowTitle) + "\",");
            }
            char[] trimChars = new char[] { ',' };
            return (builder.ToString().TrimEnd(trimChars) + "]");
        }

        public DataTable GetData(RoadFlow.Model.ProgramRun programRunModel, HttpRequest request, int size, int number, out int count)
        {
            DbConnection connection = new DbConnection();
            RoadFlow.Model.DbConnection dbConnection = connection.Get(programRunModel.ConnId);
            if (dbConnection == null)
            {
                count = 0;
                return new DataTable();
            }
            RoadFlow.Data.DbconnnectionSql sql = new RoadFlow.Data.DbconnnectionSql(dbConnection);

            string str = Wildcard.Filter(programRunModel.SqlString, null, null).Trim1();
            bool flag = str.Trim().StartsWith("exec", (StringComparison)StringComparison.CurrentCultureIgnoreCase);


            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!flag)
            {


                if (!programRunModel.SqlString.ContainsIgnoreCase("where "))
            {
                builder.Append(" WHERE 1=1");
            }
                foreach (RoadFlow.Model.ProgramQuery query in programRunModel.ProgramQueries)
                {
                    string[] textArray8;
                    string[] textArray7;
                    string[] textArray6;
                    string[] textArray5;
                    string[] textArray4;
                    string[] textArray2;
                    DateTime time;
                    DateTime time2;
                    string field = query.Field;
                    string operators = query.Operators;
                    string str9 = query.ControlName.IsNullOrWhiteSpace() ? ("ctl_" + query.Id.ToString("N")) : query.ControlName;
                    string str10 = request.Form[str9];
                    if (str10.IsNullOrEmpty())
                    {
                        continue;
                    }
                    string str11 = dbConnection.ConnType.EqualsIgnoreCase("oracle") ? ":" : "@";
                    if (!query.InputType.In(new int[] { 1, 2, 3, 4 }))
                    {
                        goto Label_02EA;
                    }
                    if (str10.IsDateTime(out time))
                    {
                        string[] textArray1 = new string[] { " AND ", field, operators, str11, field };
                        builder.Append(string.Concat((string[])textArray1));
                        int[] numArray1 = new int[] { 1, 2 };
                        list.Add(sql.SqlInstance.GetDbParameter(str11 + field, query.InputType.In(numArray1) ? time.Date : time));
                    }
                    int[] digits = new int[] { 2, 4 };
                    if (!query.InputType.In(digits) || !StringExtensions.IsDateTime(request.Form[(str9 + "1")], out time2))
                    {
                        continue;
                    }
                    string str12 = string.Empty;
                    if (operators != ">")
                    {
                        if (operators == "<")
                        {
                            goto Label_022A;
                        }
                        if (operators == ">=")
                        {
                            goto Label_0233;
                        }
                        if (operators == "<=")
                        {
                            goto Label_023C;
                        }
                    }
                    else
                    {
                        str12 = "<";
                    }
                    goto Label_0243;
                    Label_022A:
                    str12 = ">";
                    goto Label_0243;
                    Label_0233:
                    str12 = "<";
                    goto Label_0243;
                    Label_023C:
                    str12 = ">";
                    Label_0243:
                    textArray2 = new string[] { " AND ", field, str12, str11, field, "1" };
                    builder.Append(string.Concat((string[])textArray2));
                    int[] numArray3 = new int[] { 1, 2 };
                    list.Add(sql.SqlInstance.GetDbParameter(str11 + field + "1", query.InputType.In(numArray3) ? time2.AddDays(1.0).Date : time2.AddDays(1.0)));
                    continue;
                    Label_02EA:
                    if (operators != "IN")
                    {
                        if (operators == "NOT IN")
                        {
                            goto Label_037C;
                        }
                        if (operators == "%LIKE")
                        {
                            goto Label_03BA;
                        }
                        if (operators == "LIKE%")
                        {
                            goto Label_03F8;
                        }
                        if (operators == "%LIKE%")
                        {
                            goto Label_0436;
                        }
                        goto Label_0471;
                    }
                    string[] textArray3 = new string[] { " AND ", field, " IN(", str10.FilterSelectSql(), ")" };
                    builder.Append(string.Concat((string[])textArray3));
                    continue;
                    Label_037C:
                    textArray4 = new string[] { " AND ", field, " NOT IN(", str10.FilterSelectSql(), ")" };
                    builder.Append(string.Concat((string[])textArray4));
                    continue;
                    Label_03BA:
                    textArray5 = new string[] { " AND ", field, " LIKE '%", str10.FilterSelectSql(), "'" };
                    builder.Append(string.Concat((string[])textArray5));
                    continue;
                    Label_03F8:
                    textArray6 = new string[] { " AND ", field, " LIKE '", str10.FilterSelectSql(), "%'" };
                    builder.Append(string.Concat((string[])textArray6));
                    continue;
                    Label_0436:
                    textArray7 = new string[] { " AND ", field, " LIKE '%", str10.FilterSelectSql(), "%'" };
                    builder.Append(string.Concat((string[])textArray7));
                    continue;
                    Label_0471:
                    textArray8 = new string[] { " AND ", field, operators, str11, field };
                    builder.Append(string.Concat((string[])textArray8));
                    list.Add(sql.SqlInstance.GetDbParameter(str11 + field, str10));
                }
            }
            else
            {
                foreach (RoadFlow.Model.ProgramQuery query2 in programRunModel.ProgramQueries)
                {
                    string str13 = query2.ControlName.IsNullOrWhiteSpace() ? ("ctl_" + query2.Id.ToString("N")) : query2.ControlName;
                    builder.Append("'" + request.Form[str13] + "',");
                }

            }
            string str2 =request.Forms("sidx");  
            string str3 = request.Forms("sord");
            string str4 = (str2.IsNullOrWhiteSpace() ? programRunModel.DefaultSort : str2) + (str3.IsNullOrEmpty() ? "" : (" " + str3));

            StringBuilder builder2 = new StringBuilder();

            if (!flag)
            {


                //  string str4 = Wildcard.Filter(programRunModel.SqlString, null, null);
                if (dbConnection.ConnType.EqualsIgnoreCase("sqlserver") || dbConnection.ConnType.EqualsIgnoreCase("oracle"))
                {
                    str = str.Insert(str.IndexOfIgnoreCase("from") - 1, ",ROW_NUMBER() OVER(ORDER BY " + str4 + ") AS PagerAutoRowNumber ");
                }
                builder2.Append(str);
                builder2.Append(builder.ToString());
                if (dbConnection.ConnType.EqualsIgnoreCase("mysql") && !str4.IsNullOrWhiteSpace())
                {
                    builder2.Append(" ORDER BY " + str4);
                }
            }
            else
            {
                builder2.Append(str);
                builder2.Append(str.EndsWith("'") ? ((string)",") : ((string)" "));
                char[] trimChars = new char[] { ',' };
                builder2.Append(builder.ToString().TrimEnd(trimChars));

            }
            string str5 = builder2.ToString();
            SessionExtensions.SetString(request.HttpContext.Session, "program_querysql_" + programRunModel.Id.ToString("N"), str5);
            if (list.Count > 0)
            {
                IO.Insert("program_queryparameter_" + programRunModel.Id.ToString("N") + "_" + request.HttpContext.Session.Id, list, DateTimeExtensions.Now.AddHours(2.0));
            }

            count = 0;
            string str6 = string.Empty;
            if ((programRunModel.IsPager == 1) && !flag)
            {
                str6 = sql.SqlInstance.GetPaerSql(str5, size, number, out count, list.ToArray(), "");
            }
            else if ((programRunModel.IsPager == 1) & flag)
            {
                str6 = str5;
            }

            return connection.GetDataTable(dbConnection, (programRunModel.IsPager == 1) ? str6 : str5, list.ToArray());
           
        }






        /// <summary>
        /// 默认值与排序方法
        /// </summary>
        /// <param name="programFields"></param>
        /// <returns></returns>
        private ValueTuple<string,string> GetDefaultSort(List<RoadFlow.Model.ProgramField> programFields)
        {
            if (programFields != null)
            {
                RoadFlow.Model.ProgramField field = programFields.Find(key=> !key.IsSort.IsNullOrWhiteSpace());
                if (field != null)
                {
                    return new ValueTuple<string,string>(field.IsSort,field.SortWay);
                }
                RoadFlow.Model.ProgramField field2 = programFields.Find(key=> !key.Field.IsNullOrWhiteSpace());
                if (field2 != null)
                {
                    return  new  ValueTuple<string, string>(field2.Field, field2.SortWay); 
                }
            }
            return  new ValueTuple<string, string>(string.Empty, string.Empty);
        }

        /// <summary>
        /// 导出包含根据标题显示
        /// </summary>
        /// <param name="programRunModel"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public DataTable GetExportData(RoadFlow.Model.ProgramRun programRunModel, HttpRequest request)
        {
            DataTable table = new DataTable();
            if ((programRunModel != null) && (programRunModel.ProgramExports.Count != 0))
            {
                string str = SessionExtensions.GetString(request.HttpContext.Session, "program_querysql_" + programRunModel.Id.ToString("N"));
                if (!str.IsNullOrWhiteSpace())
                {
                    object obj2 = IO.Get("program_queryparameter_" + programRunModel.Id.ToString("N") + "_" + request.HttpContext.Session.Id);
                    DbParameter[] parameters = null;
                    if (obj2 != null)
                    {
                        parameters = ((List<DbParameter>)obj2).ToArray();
                    }
                    DataTable table2 = new DbConnection().GetDataTable(programRunModel.ConnId, str, parameters);
                    if (table2.Rows.Count == 0)
                    {
                        return table;
                    }
                    foreach (RoadFlow.Model.ProgramExport export in programRunModel.ProgramExports)
                    {
                        table.Columns.Add(export.ShowTitle.IsNullOrEmpty() ? export.Field : export.ShowTitle, Type.GetType("System.String"));
                    }
                    foreach (DataRow row in table2.Rows)
                    {
                        DataRow row2 = table.NewRow();
                        foreach (RoadFlow.Model.ProgramExport export2 in programRunModel.ProgramExports)
                        {
                            string titles = table2.Columns.Contains(export2.Field) ? row[export2.Field].ToString() : string.Empty;
                            if (!titles.IsNullOrWhiteSpace())
                            {
                                int? showType = export2.ShowType;
                                if (showType.HasValue)
                                {
                                    switch (showType.GetValueOrDefault())
                                    {
                                        case 2:
                                            DateTime time;
                                            if (titles.IsDateTime(out time))
                                            {
                                                titles = time.ToString(export2.ShowFormat.IsNullOrWhiteSpace() ? "yyyy-MM-dd HH:mm:ss" : export2.ShowFormat);
                                            }
                                            break;

                                        case 3:
                                            if (!export2.ShowFormat.IsNullOrWhiteSpace())
                                            {
                                                titles = titles.ToDecimal(-79228162514264337593543950335M).ToString(export2.ShowFormat);
                                            }
                                            break;

                                        case 4:
                                            titles = new Dictionary().GetTitles(titles);
                                            break;

                                        case 5:
                                            titles = new Organize().GetNames(titles);
                                            break;

                                        case 6:
                                            titles = Wildcard.Filter(export2.CustomString, null, row);
                                            break;

                                        case 7:
                                            titles = new User().GetNames(titles);
                                            break;
                                    }
                                }
                            }
                            row2[export2.ShowTitle.IsNullOrEmpty() ? export2.Field : export2.ShowTitle] = titles;
                        }
                        table.Rows.Add(row2);
                    }
                    //导出表单与显示表名标注
                    //foreach (RoadFlow.Model.ProgramExport export in programRunModel.ProgramExports)
                    //{
                    //    if(!export.ShowTitle.IsNullOrWhiteSpace())
                    //    {
                    //        table.Columns[export.Field].ColumnName = export.ShowTitle;
                    //    }
                    //}
                }


            }
            return table;
        }

        /// <summary>
        /// 原本获取导出数据表
        /// </summary>
        /// <param name="programRunModel"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public DataTable GetExportData1(RoadFlow.Model.ProgramRun programRunModel, HttpRequest request)
        {
            DataTable table = new DataTable();
            if ((programRunModel != null) && (programRunModel.ProgramExports.Count != 0))
            {
                string str = SessionExtensions.GetString(request.HttpContext.Session, "program_querysql_" + programRunModel.Id.ToString("N"));
                if (!str.IsNullOrWhiteSpace())
                {
                    object obj2 = IO.Get("program_queryparameter_" + programRunModel.Id.ToString("N") + "_" + request.HttpContext.Session.Id);
                    DbParameter[] parameters = null;
                    if (obj2 != null)
                    {
                        parameters = ((List<DbParameter>)obj2).ToArray();
                    }
                    DataTable table2 = new DbConnection().GetDataTable(programRunModel.ConnId, str, parameters);
                    if (table2.Rows.Count == 0)
                    {
                        return table;
                    }
                    foreach (RoadFlow.Model.ProgramExport export in programRunModel.ProgramExports)
                    {
                        table.Columns.Add(export.Field, Type.GetType("System.String"));
                    }
                    foreach (DataRow row in table2.Rows)
                    {
                        DataRow row2 = table.NewRow();
                        foreach (RoadFlow.Model.ProgramExport export2 in programRunModel.ProgramExports)
                        {
                            string titles = table2.Columns.Contains(export2.Field) ? row[export2.Field].ToString() : string.Empty;
                            if (!titles.IsNullOrWhiteSpace())
                            {
                                int? showType = export2.ShowType;
                                if (showType.HasValue)
                                {
                                    switch (showType.GetValueOrDefault())
                                    {
                                        case 2:
                                            DateTime time;
                                            if (titles.IsDateTime(out time))
                                            {
                                                titles = time.ToString(export2.ShowFormat.IsNullOrWhiteSpace() ? "yyyy-MM-dd HH:mm:ss" : export2.ShowFormat);
                                            }
                                            break;

                                        case 3:
                                            if (!export2.ShowFormat.IsNullOrWhiteSpace())
                                            {
                                                titles = titles.ToDecimal(-79228162514264337593543950335M).ToString(export2.ShowFormat);
                                            }
                                            break;

                                        case 4:
                                            titles = new Dictionary().GetTitles(titles);
                                            break;

                                        case 5:
                                            titles = new Organize().GetNames(titles);
                                            break;

                                        case 6:
                                            titles = Wildcard.Filter(export2.CustomString, null, row);
                                            break;

                                        case 7:
                                            titles = new User().GetNames(titles);
                                            break;
                                    }
                                }
                            }
                            row2[export2.Field] = titles;
                        }
                        table.Rows.Add(row2);
                    }
                }
            }
            return table;
        }

        public DataTable GetPagerData(out int count, int size, int number, string name, string types, string order)
        {
            return this.ProgramData.GetPagerData(out count, size, number, name, types, order);
        }

        private string GetQueryData(List<RoadFlow.Model.ProgramQuery> programQueries)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\"pagesize\": size || curPageSize, \"pagenumber\": number || curPageNumber,");
            foreach (RoadFlow.Model.ProgramQuery query in programQueries)
            {
                string str = query.ControlName.IsNullOrWhiteSpace() ? ("ctl_" + query.Id.ToString("N")) : query.ControlName;
                string[] textArray1 = new string[] { "\"", str, "\":$(\"#", str, "\").val()," };
                builder.Append(string.Concat((string[])textArray1));
                int[] digits = new int[] { 2, 4 };
                if (query.InputType.In(digits))
                {
                    string[] textArray2 = new string[] { "\"", str, "1\":$(\"#", str, "1\").val()," };
                    builder.Append(string.Concat((string[])textArray2));
                }
            }
            char[] trimChars = new char[] { ',' };
            return ("{" + builder.ToString().TrimEnd(trimChars) + "}");
        }

        /// <summary>
        /// 获取查询Html页面方式
        /// </summary>
        /// <param name="programQueries"></param>
        /// <returns></returns>
        //private string GetQueryHtml(List<RoadFlow.Model.ProgramQuery> programQueries, IStringLocalizer localizer = null)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    if (programQueries == null)
        //    {
        //        return "";
        //    }
        //    foreach (RoadFlow.Model.ProgramQuery query in programQueries)
        //    {
        //        string str = query.ShowTitle.IsNullOrWhiteSpace() ? query.Field : query.ShowTitle;
        //        string str2 = query.ControlName.IsNullOrWhiteSpace() ? ("ctl_" + query.Id.ToString("N")) : query.ControlName;
        //        builder.Append("<span style=\"margin-right:8px;\">");
        //        builder.Append("<span>" + str + "：</span>");
        //        switch (query.InputType)
        //        {
        //            case 0:
        //                builder.Append("<input type='text' class='mytext'");
        //                builder.Append(" id='" + str2 + "'");
        //                builder.Append(" name='" + str2 + "'");
        //                if (!query.ShowStyle.IsNullOrWhiteSpace())
        //                {
        //                    builder.Append(" style=\"" + query.ShowStyle + "\"");
        //                }
        //                builder.Append("/>");
        //                goto Label_064A;

        //            case 1:
        //                builder.Append("<input type='text' class='mycalendar'");
        //                builder.Append(" id='" + str2 + "'");
        //                builder.Append(" name='" + str2 + "'");
        //                if (!query.ShowStyle.IsNullOrWhiteSpace())
        //                {
        //                    builder.Append(" style=\"" + query.ShowStyle + "\"");
        //                }
        //                builder.Append("/>");
        //                goto Label_064A;

        //            case 2:
        //                builder.Append("<input type='text' class='mycalendar'");
        //                builder.Append(" id='" + str2 + "'");
        //                builder.Append(" name='" + str2 + "'");
        //                if (!query.ShowStyle.IsNullOrWhiteSpace())
        //                {
        //                    builder.Append(" style=\"" + query.ShowStyle + "\"");
        //                }
        //                builder.Append("/>");
        //                builder.Append((localizer == null) ? ((localizer == null) ? ((string)" 至 ") : ((string)localizer["To"])) : ((string)localizer["To"]));

        //                builder.Append("<input type='text' class='mycalendar'");
        //                builder.Append(" id='" + str2 + "1'");
        //                builder.Append(" name='" + str2 + "1'");
        //                if (!query.ShowStyle.IsNullOrWhiteSpace())
        //                {
        //                    builder.Append(" style=\"" + query.ShowStyle + "\"");
        //                }
        //                builder.Append("/>");
        //                goto Label_064A;

        //            case 3:
        //                builder.Append("<input type='text' class='mycalendar' istime='1'");
        //                builder.Append(" id='" + str2 + "'");
        //                builder.Append(" name='" + str2 + "'");
        //                if (!query.ShowStyle.IsNullOrWhiteSpace())
        //                {
        //                    builder.Append(" style=\"" + query.ShowStyle + "\"");
        //                }
        //                builder.Append("/>");
        //                goto Label_064A;

        //            case 4:
        //                builder.Append("<input type='text' class='mycalendar' istime='1'");
        //                builder.Append(" id='" + str2 + "'");
        //                builder.Append(" name='" + str2 + "'");
        //                if (!query.ShowStyle.IsNullOrWhiteSpace())
        //                {
        //                    builder.Append(" style=\"" + query.ShowStyle + "\"");
        //                }
        //                builder.Append("/>");
        //                builder.Append((localizer == null) ? ((localizer == null) ? ((string)" 至 ") : ((string)localizer["To"])) : ((string)localizer["To"]));

        //                builder.Append("<input type='text' class='mycalendar' istime='1'");
        //                builder.Append(" id='" + str2 + "1'");
        //                builder.Append(" name='" + str2 + "1'");
        //                if (!query.ShowStyle.IsNullOrWhiteSpace())
        //                {
        //                    builder.Append(" style=\"" + query.ShowStyle + "\"");
        //                }
        //                builder.Append("/>");
        //                goto Label_064A;

        //            case 5:
        //                break;

        //            case 6:
        //                {
        //                    builder.Append("<input type='text' class='mymember'");
        //                    builder.Append(" id='" + str2 + "'");
        //                    builder.Append(" name='" + str2 + "'");
        //                    if (!query.ShowStyle.IsNullOrWhiteSpace())
        //                    {
        //                        builder.Append(" style=\"" + query.ShowStyle + "\"");
        //                    }
        //                    string organizeAttrString = new Organize().GetOrganizeAttrString(query.OrgAttribute);
        //                    builder.Append(organizeAttrString);
        //                    builder.Append("/>");
        //                    goto Label_064A;
        //                }
        //            case 7:
        //                builder.Append("<input type='text' class='mydict'");
        //                builder.Append(" id='" + str2 + "'");
        //                builder.Append(" name='" + str2 + "'");
        //                builder.Append(" rootid='" + query.DictValue + "'");
        //                if (!query.ShowStyle.IsNullOrWhiteSpace())
        //                {
        //                    builder.Append(" style=\"" + query.ShowStyle + "\"");
        //                }
        //                builder.Append("/>");
        //                goto Label_064A;

        //            default:
        //                goto Label_064A;
        //        }
        //        builder.Append("<select class='myselect'");
        //        builder.Append(" id='" + str2 + "'");
        //        builder.Append(" name='" + str2 + "'");
        //        if (!query.ShowStyle.IsNullOrWhiteSpace())
        //        {
        //            builder.Append(" style=\"" + query.ShowStyle + "\"");
        //        }
        //        builder.Append(">");
        //        builder.Append("<option value=\"\"></option>");
        //        switch (query.DataSource.Value)
        //        {
        //            case 0:
        //                builder.Append(new Form().GetOptionsByStringExpression(query.DataSourceString, ""));
        //                break;

        //            case 1:
        //                builder.Append(new Dictionary().GetOptionsByID(query.DictValue.ToGuid(), ValueField.Id, "", true));
        //                break;

        //            case 2:
        //                builder.Append(new Form().GetOptionsBySQL(query.ConnId, query.DataSourceString, ""));
        //                break;
        //        }
        //        builder.Append("</select>");
        //        Label_064A:
        //        builder.Append("</span>");
        //    }
        //    return builder.ToString();
        //}


        private string GetQueryHtml(List<RoadFlow.Model.ProgramQuery> programQueries, IStringLocalizer localizer = null)
        {
            StringBuilder builder = new StringBuilder();
            if (programQueries == null)
            {
                return "";
            }
            foreach (RoadFlow.Model.ProgramQuery query in programQueries)
            {
                RoadFlow.Utility.ValueField id;
                string str = query.ShowTitle.IsNullOrWhiteSpace() ? query.Field : query.ShowTitle;
                string str2 = query.ControlName.IsNullOrWhiteSpace() ? ("ctl_" + query.Id.ToString("N")) : query.ControlName;
                builder.Append("<span style=\"margin-right:8px;display:inline-block;word-wrap:break-word;white-space:normal;\">");
                builder.Append("<label>" + str + "：</label>");
                switch (query.InputType)
                {
                    case 0:
                        builder.Append("<input type='text' class='mytext'");
                        builder.Append(" id='" + str2 + "'");
                        builder.Append(" name='" + str2 + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            builder.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        builder.Append("/>");
                        goto Label_06F2;

                    case 1:
                        builder.Append("<input type='text' class='mycalendar'");
                        builder.Append(" id='" + str2 + "'");
                        builder.Append(" name='" + str2 + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            builder.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        builder.Append("/>");
                        goto Label_06F2;

                    case 2:
                        builder.Append("<input type='text' class='mycalendar'");
                        builder.Append(" id='" + str2 + "'");
                        builder.Append(" name='" + str2 + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            builder.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        builder.Append("/>");
                        builder.Append((localizer == null) ? ((localizer == null) ? ((string)" 至 ") : ((string)localizer["To"])) : ((string)localizer["To"]));
                        builder.Append("<input type='text' class='mycalendar'");
                        builder.Append(" id='" + str2 + "1'");
                        builder.Append(" name='" + str2 + "1'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            builder.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        builder.Append("/>");
                        goto Label_06F2;

                    case 3:
                        builder.Append("<input type='text' class='mycalendar' istime='1'");
                        builder.Append(" id='" + str2 + "'");
                        builder.Append(" name='" + str2 + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            builder.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        builder.Append("/>");
                        goto Label_06F2;

                    case 4:
                        builder.Append("<input type='text' class='mycalendar' istime='1'");
                        builder.Append(" id='" + str2 + "'");
                        builder.Append(" name='" + str2 + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            builder.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        builder.Append("/>");
                        builder.Append((localizer == null) ? ((localizer == null) ? ((string)" 至 ") : ((string)localizer["To"])) : ((string)localizer["To"]));
                        builder.Append("<input type='text' class='mycalendar' istime='1'");
                        builder.Append(" id='" + str2 + "1'");
                        builder.Append(" name='" + str2 + "1'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            builder.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        builder.Append("/>");
                        goto Label_06F2;

                    case 5:
                        builder.Append("<select class='myselect'");
                        builder.Append(" id='" + str2 + "'");
                        builder.Append(" name='" + str2 + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            builder.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        builder.Append(">");
                        builder.Append("<option value=\"\"></option>");
                        switch (query.DataSource.Value)
                        {
                            case 0:
                                builder.Append(new Form().GetOptionsByStringExpression(query.DataSourceString, ""));
                                break;

                            case 1:
                                id = RoadFlow.Utility.ValueField.Id;
                                switch (query.OrgAttribute.ToInt(0))
                                {
                                    case 1:
                                        goto Label_055E;

                                    case 2:
                                        goto Label_0563;

                                    case 3:
                                        goto Label_0568;

                                    case 4:
                                        goto Label_056D;

                                    case 5:
                                        goto Label_0572;
                                }
                                goto Label_0577;

                            case 2:
                                builder.Append(new Form().GetOptionsBySQL(query.ConnId, query.DataSourceString, ""));
                                break;
                        }
                        goto Label_05C3;

                    case 6:
                        {
                            builder.Append("<input type='text' class='mymember'");
                            builder.Append(" id='" + str2 + "'");
                            builder.Append(" name='" + str2 + "'");
                            if (!query.ShowStyle.IsNullOrWhiteSpace())
                            {
                                builder.Append(" style=\"" + query.ShowStyle + "\"");
                            }
                            string organizeAttrString = new Organize().GetOrganizeAttrString(query.OrgAttribute);
                            builder.Append(organizeAttrString);
                            builder.Append("/>");
                            goto Label_06F2;
                        }
                    case 7:
                        builder.Append("<input type='text' class='mydict'");
                        builder.Append(" id='" + str2 + "'");
                        builder.Append(" name='" + str2 + "'");
                        builder.Append(" rootid='" + query.DictValue + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            builder.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        builder.Append("/>");
                        goto Label_06F2;

                    default:
                        goto Label_06F2;
                }
                Label_055E:
                id = RoadFlow.Utility.ValueField.Title;
                goto Label_057A;
                Label_0563:
                id = RoadFlow.Utility.ValueField.Code;
                goto Label_057A;
                Label_0568:
                id = RoadFlow.Utility.ValueField.Value;
                goto Label_057A;
                Label_056D:
                id = RoadFlow.Utility.ValueField.Note;
                goto Label_057A;
                Label_0572:
                id = RoadFlow.Utility.ValueField.Other;
                goto Label_057A;
                Label_0577:
                id = RoadFlow.Utility.ValueField.Id;
                Label_057A:
                builder.Append(new Dictionary().GetOptionsByID(query.DictValue.ToGuid(), id, "", true, true));
                Label_05C3:
                builder.Append("</select>");
                Label_06F2:
                builder.Append("</span>");
            }
            return builder.ToString();
        }






        /// <summary>
        /// 获取设计项目运行数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RoadFlow.Model.ProgramRun GetRunModel(Guid id, IStringLocalizer localizer = null)
        {
            string str = "ProgramRun_" + id.ToString("N");
            object obj2 = IO.Get(str);
            if (obj2 != null)
            {
                return (RoadFlow.Model.ProgramRun)obj2;
            }
            RoadFlow.Model.Program program = this.Get(id);
            if (program == null)
            {
                return null;
            }
            RoadFlow.Model.ProgramRun run = new RoadFlow.Model.ProgramRun
            {
                ButtonLocation = program.ButtonLocation,
                ClientScript = program.ClientScript,
                ConnId = program.ConnId,
                CreateTime = program.CreateTime,
                CreateUserId = program.CreateUserId,
                EditModel = program.EditModel,
                ExportFileName = program.ExportFileName,
                ExportHeaderText = program.ExportHeaderText,
                ExportTemplate = program.ExportTemplate,
                FormId = program.FormId,
                Height = program.Height,
                Id = program.Id,
                InDataNumberFiledName = program.InDataNumberFiledName,
                IsAdd = program.IsAdd,
                IsPager = program.IsPager,
                RowNumber = program.RowNumber,
                Name = program.Name,
                ProgramButtons = new ProgramButton().GetAll(program.Id),
                ProgramExports = new ProgramExport().GetAll(program.Id),
                ProgramFields = new ProgramField().GetAll(program.Id),
                ProgramQueries = new ProgramQuery().GetAll(program.Id),
                ProgramValidates = new ProgramValidate().GetAll(program.Id),
                ProgramGroups=new ProgramGroup().GetAll(program.Id),
                PublishTime = program.PublishTime,
                SelectColumn = program.SelectColumn,
                SqlString = program.SqlString,
                Status = program.Status,
                TableHead = program.TableHead,
                TableStyle = program.TableStyle,
                Type = program.Type,
                Width = program.Width,
                GroupHeaders = program.GroupHeaders,
                IsGroup=program.IsGroup,
                IsFooterrow=program.IsFooterrow
            };
            run.QueryHtml = this.GetQueryHtml(run.ProgramQueries, localizer);
            run.QueryData = this.GetQueryData(run.ProgramQueries);
            run.GridColNames = this.GetColNames(run.ProgramFields);
            run.GridColModels = this.GetColModels(run.ProgramFields);

            var sortstr=    this.GetDefaultSort(run.ProgramFields);
            run.DefaultSort = program.DefaultSort.IsNullOrWhiteSpace() ? sortstr.Item1: program.DefaultSort;

            run.SortWay = program.DefaultSort.IsNullOrWhiteSpace() ? (sortstr.Item2.IsNullOrEmpty() ? "  ": sortstr.Item2) : " ";
            if (run.ProgramGroups!=null)
            {
                var getGroups = this.GetGroups(run.ProgramGroups);
                run.GridGroups_String = getGroups.Item1;
                run.GridGroups_Bool = getGroups.Item2;
            }
            IO.Insert(str, run);
            return run;
        }


        /// <summary>
        /// 项目字段设置，需要优化
        /// </summary>
        /// <param name="programFields"></param>
        /// <returns></returns>
        private ValueTuple<Dictionary<string, List<string>>, Dictionary<string, List<bool>>> GetGroups(List<RoadFlow.Model.ProgramGroup> programGroups)
        {
            Dictionary<string, List<string>> dic_string = new Dictionary<string, List<string>>();
            Dictionary<string, List<bool>> dic_bool = new Dictionary<string, List<bool>>();
            
            List<string> GroupField = new List<string>();
            List<bool> GroupSummary = new List<bool>();
            List<bool> GroupColumnShow = new List<bool>();
            List<string> GroupText = new List<string>();
            List<bool> GroupCollapse = new List<bool>();
            List<string> GroupOrder = new List<string>();
            List<bool> GroupDataSorted = new List<bool>();
            List<bool> ShowSummaryOnHide = new List<bool>();     

            foreach (RoadFlow.Model.ProgramGroup field in programGroups)
            {
                GroupField.Add(field.GroupField);

                //分组汇总
                if (field.IsGroupSummary == 0)
                {
                    GroupSummary.Add(false);
                }
                else
                {
                    GroupSummary.Add(true);
                }

                //是否显示分组
                if (field.IsGroupColumnShow == 0)
                {
                    GroupColumnShow.Add(false);
                }
                else
                {
                    GroupColumnShow.Add(true);
                }


                //是否折叠
                if (field.IsGroupCollapse == 0)
                {
                    GroupCollapse.Add(false);
                }
                else
                {
                    GroupCollapse.Add(true);
                }
                //是否数据排序
                if (field.IsGroupDataSorted == 0)
                {
                    GroupDataSorted.Add(false);
                }
                else
                {
                    GroupDataSorted.Add(true);
                }

                //是否隐藏汇总
                if (field.IsShowSummaryOnHide == 0)
                {
                    ShowSummaryOnHide.Add(false);
                }
                else
                {
                    ShowSummaryOnHide.Add(true);
                }

                GroupText.Add(field.GroupText);
                GroupOrder.Add(field.GroupOrder);

            }
            dic_string.Add("GroupField", GroupField);
            dic_string.Add("GroupText", GroupText);
            dic_string.Add("GroupOrder", GroupOrder);

            dic_bool.Add("GroupSummary", GroupSummary);
            dic_bool.Add("GroupColumnShow", GroupColumnShow);
            dic_bool.Add("GroupCollapse", GroupCollapse);
            dic_bool.Add("GroupDataSorted", GroupDataSorted);
            dic_bool.Add("ShowSummaryOnHide", ShowSummaryOnHide);




            return  new ValueTuple<Dictionary<string, List<string>>, Dictionary<string, List<bool>>>(dic_string, dic_bool);
        }





        public string GetTitles(string values, string pkField, string titleField, Guid programId)
        {
            RoadFlow.Model.Program program = this.Get(programId);
            if (program == null)
            {
                return "";
            }
            char[] separator = new char[] { ',' };
            string[] strArray = values.Split(separator);
            if (strArray.Length == 0)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            DbConnection connection = new DbConnection();
            foreach (string str in strArray)
            {
                string[] textArray1 = new string[] { "SELECT ", titleField, " FROM (", program.SqlString, ") TEMPTABLE_", titleField, " WHERE ", pkField, "={0}" };
                string sql = string.Concat((string[])textArray1);
                object[] objs = new object[] { str };
                string str3 = connection.GetFieldValue(program.ConnId, sql, objs);
                if (!str3.IsNullOrEmpty())
                {
                    builder.Append(str3);
                    builder.Append("、");
                }
            }
            char[] trimChars = new char[] { '、' };
            return builder.ToString().TrimEnd(trimChars);
        }

        public string Publish(Guid id, IStringLocalizer localizer = null)

        {
            IO.Remove("ProgramRun_" + id.ToString("N"));
            RoadFlow.Model.Program program = this.Get(id);
            if (program == null)
            {
                if (localizer != null)
                {
                    return localizer["NotFoundModel"];
                }
                return "没有找到要发布的程序!";
            }
            program.Status = 1;
            this.Update(program);

            RoadFlow.Model.ProgramRun runModel = this.GetRunModel(id,null);
            if (runModel == null)
            {
                if (localizer != null)
                {
                    return localizer["NotFoundModel"];
                }
                return "没有找到要发布的程序!";

            }

            AppLibrary library = new AppLibrary();
            RoadFlow.Model.AppLibrary byCode = library.GetByCode(id.ToString());
            bool flag = false;
            if (byCode == null)
            {
                flag = true;
                byCode = new RoadFlow.Model.AppLibrary
                {
                    Id = Guid.NewGuid(),
                    Code = id.ToString()
                };
            }
            byCode.Address = "/RoadFlowCore/ProgramDesigner/Run?programid=" + id.ToString();
            byCode.OpenMode = 0;
            byCode.Title = runModel.Name;
            byCode.Type = runModel.Type;
            if (flag)
            {
                library.Add(byCode);
            }
            else
            {
                library.Update(byCode);
            }
            AppLibraryButton button = new AppLibraryButton();
            List<RoadFlow.Model.AppLibraryButton> listByApplibraryId = button.GetListByApplibraryId(byCode.Id);
            List<RoadFlow.Model.ProgramButton> all = new ProgramButton().GetAll(id);
            using (List<RoadFlow.Model.ProgramButton>.Enumerator enumerator = all.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    RoadFlow.Model.ProgramButton button1 = enumerator.Current;
                    RoadFlow.Model.AppLibraryButton appLibraryButton = listByApplibraryId.Find(delegate (RoadFlow.Model.AppLibraryButton p) {
                        return p.Id == button1.Id;
                    });
                    bool flag2 = false;
                    if (appLibraryButton == null)
                    {
                        flag2 = true;
                        appLibraryButton = new RoadFlow.Model.AppLibraryButton
                        {
                            Id = button1.Id
                        };
                    }
                    appLibraryButton.AppLibraryId = byCode.Id;
                    appLibraryButton.ButtonId = button1.ButtonId;
                    appLibraryButton.Events = button1.ClientScript;
                    appLibraryButton.Ico = button1.Ico;
                    appLibraryButton.IsValidateShow = button1.IsValidateShow;
                    appLibraryButton.Name = button1.ButtonName;
                    appLibraryButton.ShowType = button1.ShowType;
                    appLibraryButton.Sort = button1.Sort;
                    if (flag2)
                    {
                        button.Add(appLibraryButton);
                    }
                    else
                    {
                        button.Update(appLibraryButton);
                    }
                }
            }
            using (List<RoadFlow.Model.AppLibraryButton>.Enumerator enumerator2 = listByApplibraryId.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    RoadFlow.Model.AppLibraryButton button2 = enumerator2.Current;
                    if (!all.Exists(delegate (RoadFlow.Model.ProgramButton p) {
                        return p.Id == button2.Id;
                    }))
                    {
                        button.Delete(button2.Id);
                    }
                }
            }
            return "1";
        }

       





        public int Update(RoadFlow.Model.Program program)
        {
            return this.ProgramData.Update(program);
        }

       
}



 


    #endregion 

}
