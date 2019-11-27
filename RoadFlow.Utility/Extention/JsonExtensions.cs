using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace RoadFlow.Utility
{
    public static class JsonExtensions
    {

        #region  Datatable转换成Json数据
        /// <summary>
        /// 方法1：使用StringBuilder 
        /// using System.Text; 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableToJson(DataTable table)
        {
            var JsonString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
            }
            return JsonString.ToString();
        }

      
        /// <summary>
        ///  方法3：使用Json.Net DLL (Newtonsoft)。
        ///  这个方法中要添加Json.Net DLL引用
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableToJsonWithJsonNet(DataTable table)
        {
            string JsonString = string.Empty;
            JsonString = JsonConvert.SerializeObject(table);
            return JsonString;
        }

        #endregion

        #region  Json数据转换成DataTable 

       


      

     


        /// <summary>
        /// 将json转换为DataTable
        /// 根ToDataTable一样简单转换
        /// 
        /// </summary>
        /// <param name="strJson">得到的json</param>
        /// <returns></returns>
        public static DataTable JsonToDataTable(string strJson)
        {
            //转换json格式
            strJson = strJson.Replace(",\"", "*\"").Replace("\":", "\"#").ToString();
            //取出表名  
            var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;
            //去除表名  
            strJson = strJson.Substring(strJson.IndexOf("[") + 1);
            strJson = strJson.Substring(0, strJson.IndexOf("]"));
            //获取数据  
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split('*');
                //创建表  
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (string str in strRows)
                    {
                        var dc = new DataColumn();
                        string[] strCell = str.Split('#');
                        if (strCell[0].Substring(0, 1) == "\"")
                        {
                            int a = strCell[0].Length;
                            dc.ColumnName = strCell[0].Substring(1, a - 2);
                        }
                        else
                        {
                            dc.ColumnName = strCell[0];
                        }
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }
                //增加内容  
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    dr[r] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }
            return tb;
        }



        #endregion

        #region dataTable转换成网页

        /// <summary>
        /// 把DataTable转换成打印的网页
        /// </summary>
        /// <param name="ExportFileName">标题名称</param>
        /// <param name="isPrint">是否打印</param>
        /// <param name="tbl">DataTable</param>
        /// <returns></returns>
        public static string GetHtmlString(string ExportFileName, bool isPrint, DataTable tbl)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<HTML><HEAD>");
            sb.Append("<title>" + ExportFileName + "</title>");
            sb.Append("<META HTTP-EQUIV='content-type' CONTENT='text/html; charset=GB2312'> ");
            sb.Append("<script language=javascript>");
            sb.Append("self.resizeBy(0,0);");
            sb.Append("self.resizeTo(screen.availWidth,screen.availHeight);");
            sb.Append("</script>");
            sb.Append("<style type=text/css>");
            sb.Append("td{font-size: 9pt;border:solid 1 #000000;}");
            sb.Append("table{padding:3 0 3 0;border:solid 1 #000000;margin:0 0 0 0;BORDER-COLLAPSE: collapse;}");
            sb.Append("</style>");
            sb.Append("</HEAD>");
            if (!isPrint)
                sb.Append("<BODY  >");
            else
                sb.Append("<BODY   onload = 'window.print()'>");
            sb.Append("<table cellSpacing='0' cellPadding='0' width ='100%' border='1'");
            sb.Append(">");
            sb.Append("<tr valign='middle'>");
            sb.Append("<td><b>" + "</b></td>");
            // sb.Append("<td><b>" + CommonUI.Translate("RowSequences") + "</b></td>");
            foreach (DataColumn column in tbl.Columns)
            {
                sb.Append("<td><b><span>" + column.ColumnName + "</span></b></td>");
            }
            sb.Append("</tr>");
            int iColsCount = tbl.Columns.Count;
            int rowsCount = tbl.Rows.Count - 1;
            for (int j = 0; j <= rowsCount; j++)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + ((int)(j + 1)).ToString() + "</td>");
                for (int k = 0; k <= iColsCount - 1; k++)
                {
                    sb.Append("<td");
                    sb.Append(">");
                    object obj = tbl.Rows[j][k];
                    if (obj == DBNull.Value)
                    {
                        // 如果是NULL则在HTML里面使用一个空格替换之  
                        obj = "&nbsp;";
                    }
                    if (obj.ToString() == "")
                    {
                        obj = "&nbsp;";
                    }
                    string strCellContent = obj.ToString().Trim();
                    sb.Append("<span>" + strCellContent + "</span>");
                    sb.Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</TABLE></BODY></HTML>");
            return sb.ToString();
        }



        /// <summary>
        /// DataTable转换成HTML网页
        /// </summary>
        /// <param name="dt">DataTable表</param>
        /// <returns></returns>
        public static string ExportDatatableToHtml(DataTable dt)
        {
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<html >");
            strHTMLBuilder.Append("<head>");
            strHTMLBuilder.Append("</head>");
            strHTMLBuilder.Append("<body>");
            strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='1' bgcolor='lightyellow' style='font-family:Garamond; font-size:smaller'>");

            strHTMLBuilder.Append("<tr >");
            foreach (DataColumn myColumn in dt.Columns)
            {
                strHTMLBuilder.Append("<td >");
                strHTMLBuilder.Append(myColumn.ColumnName);
                strHTMLBuilder.Append("</td>");

            }
            strHTMLBuilder.Append("</tr>");


            foreach (DataRow myRow in dt.Rows)
            {

                strHTMLBuilder.Append("<tr >");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");

                }
                strHTMLBuilder.Append("</tr>");
            }

            //Close tags. 
            strHTMLBuilder.Append("</table>");
            strHTMLBuilder.Append("</body>");
            strHTMLBuilder.Append("</html>");

            string Htmltext = strHTMLBuilder.ToString();

            return Htmltext;

        }



        #endregion


        #region  后台先把前台传过来的字符串转换成HtmlTable实体，然后再转换成DataTable实体

        //变量参数
        /// <summary>
        /// 行分隔
        /// </summary>
        public static readonly string rowSeparater = "|||||";
        /// <summary>
        /// 列分隔
        /// </summary>
        public static readonly string columnSeparater = "@@@@@";
        /// <summary>
        /// 值分隔
        /// </summary>
        public static readonly string valueSeparater = "$$$$$";
        /// <summary>
        /// 空值标识
        /// </summary>
        public static readonly string nullFlag = "HtmlTableUtil_NULL_FLAG";



        ///// <summary>
        ///// 字符串转换成HtmlTable
        ///// </summary>
        //public static HtmlTable String2HtmlTable(string data)
        //{
        //    HtmlTable htmlTable = new HtmlTable();
        //    string[] rowArray = data.Split(new string[] { rowSeparater }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (string row in rowArray)//遍历行
        //    {
        //        HtmlTableRow htmlTableRow = new HtmlTableRow();
        //        string[] colArray = row.Split(new string[] { columnSeparater }, StringSplitOptions.RemoveEmptyEntries);

        //        foreach (string col in colArray)//遍历列
        //        {
        //            HtmlTableCell htmlTableCell = new HtmlTableCell();
        //            string[] valArr = col.Split(new string[] { valueSeparater }, StringSplitOptions.None);
        //            string val = valArr[0];
        //            int rowspan = int.Parse(valArr[1]);
        //            int colspan = int.Parse(valArr[2]);
        //            htmlTableCell.InnerText = val;
        //            htmlTableCell.RowSpan = rowspan;
        //            htmlTableCell.ColSpan = colspan;
        //            htmlTableRow.Cells.Add(htmlTableCell);
        //        }
        //        htmlTable.Rows.Add(htmlTableRow);
        //    }
        //    return htmlTable;
        //}




        ///// <summary>
        ///// HtmlTable转换成DataTable
        ///// </summary>
        //public static DataTable HtmlTable2DataTable(HtmlTable htmlTable)
        //{
        //    DataTable dataTable = new DataTable();
        //    //  DataTable列数
        //    int colCount = 0;
        //    if (htmlTable.Rows.Count > 0)
        //    {
        //        foreach (HtmlTableCell htmlTableCell in htmlTable.Rows[0].Cells)
        //        {
        //            colCount += htmlTableCell.ColSpan;
        //        }
        //    }

        //    //DataTable行数
        //    int rowCount = htmlTable.Rows.Count;

        //    // 给DataTable添加列
        //    for (int i = 0; i < colCount; i++)
        //    {
        //        dataTable.Columns.Add();
        //    }

        //    // 给DataTable添加行
        //    for (int i = 0; i < rowCount; i++)//遍历行
        //    {
        //        DataRow dataRow = dataTable.NewRow();
        //        for (int j = 0; j < colCount; j++)//遍历列
        //        {
        //            dataRow[j] = null;
        //        }
        //        dataTable.Rows.Add(dataRow);
        //    }

        //    //     转换
        //    for (int i = 0; i < htmlTable.Rows.Count; i++)//遍历HtmlTable行
        //    {
        //        HtmlTableRow htmlTableRow = htmlTable.Rows[i];
        //        int delta = 0;//列增量
        //        for (int j = 0; j < htmlTableRow.Cells.Count; j++)//遍历HtmlTable列
        //        {
        //            HtmlTableCell htmlTableCell = htmlTableRow.Cells[j];
        //            // 计算delta
        //            for (int k = j + delta; k < colCount; k++)
        //            {
        //                string cellValue = dataTable.Rows[i][k].ToString();
        //                if (cellValue != null)
        //                {
        //                    if (cellValue.IndexOf(nullFlag) == 0)
        //                    {
        //                        delta++;
        //                        continue;
        //                    }
        //                }
        //                break;
        //            }

        //            //   填充DataTable
        //            dataTable.Rows[i][j + delta] = htmlTableCell.InnerText + valueSeparater + htmlTableCell.RowSpan + valueSeparater + htmlTableCell.ColSpan;
        //            if (htmlTableCell.RowSpan > 1 || htmlTableCell.ColSpan > 1)
        //            {
        //                for (int m = 0; m < htmlTableCell.RowSpan; m++)
        //                {
        //                    for (int n = 0; n < htmlTableCell.ColSpan; n++)
        //                    {
        //                        if (!(m == 0 && n == 0))
        //                        {
        //                            int ii = i + m;
        //                            int jj = j + delta + n;
        //                            dataTable.Rows[ii][jj] = nullFlag + valueSeparater + "1" + valueSeparater + "1";
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //    }

        //    return dataTable;
        //}

        #endregion


        #region 字符串与Json互转
        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        public static string String2Json(String s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 格式化字符型、日期型、布尔型
        /// </summary>
        public static string StringFormat(string str, Type type)
        {
            if (type == typeof(string))
            {
                str = String2Json(str);
                str = "\"" + str + "\"";
            }
            else if (type == typeof(DateTime))
            {
                str = "\"" + str + "\"";
            }
            else if (type == typeof(bool))
            {
                str = str.ToLower();
            }
            else if (type != typeof(string) && string.IsNullOrEmpty(str))
            {
                str = "\"" + str + "\"";
            }
            return str;
        }
        #endregion

        #region List转换成Json
        /// <summary>
        /// List转换成Json
        /// </summary>
        public static string ListToJson<T>(IList<T> list)
        {
            object obj = list[0];
            return ListToJson<T>(list, obj.GetType().Name);
        }

        /// <summary>
        /// List转换成Json 
        /// </summary>
        public static string ListToJson<T>(IList<T> list, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName)) jsonName = list[0].GetType().Name;
            Json.Append("{\"" + jsonName + "\":[");
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    T obj = Activator.CreateInstance<T>();
                    PropertyInfo[] pi = obj.GetType().GetProperties();
                    Json.Append("{");
                    for (int j = 0; j < pi.Length; j++)
                    {
                        Type type = pi[j].GetValue(list[i], null).GetType();
                        Json.Append("\"" + pi[j].Name.ToString() + "\":" + StringFormat(pi[j].GetValue(list[i], null).ToString(), type));

                        if (j < pi.Length - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < list.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }
        #endregion

        #region 对象转换为Json
        /// <summary> 
        /// 对象转换为Json 
        /// </summary> 
        /// <param name="jsonObject">对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(object jsonObject)
        {
            string jsonString = "{";
            PropertyInfo[] propertyInfo = jsonObject.GetType().GetProperties();
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                object objectValue = propertyInfo[i].GetGetMethod().Invoke(jsonObject, null);
                string value = string.Empty;
                if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
                {
                    value = "'" + objectValue.ToString() + "'";
                }
                else if (objectValue is string)
                {
                    value = "'" + ToJson(objectValue.ToString()) + "'";
                }
                else if (objectValue is IEnumerable)
                {
                    value = ToJson((IEnumerable)objectValue);
                }
                else
                {
                    value = ToJson(objectValue.ToString());
                }
                jsonString += "\"" + ToJson(propertyInfo[i].Name) + "\":" + value + ",";
            }
            jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            return jsonString + "}";
        }
        #endregion

        #region 对象集合转换Json
        /// <summary> 
        /// 对象集合转换Json 
        /// </summary> 
        /// <param name="array">集合对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                jsonString += ToJson(item) + ",";
            }
            jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            return jsonString + "]";
        }
        #endregion

        #region 普通集合转换Json
        /// <summary> 
        /// 普通集合转换Json 
        /// </summary> 
        /// <param name="array">集合对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToArrayString(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                jsonString = ToJson(item.ToString()) + ",";
            }
            jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            return jsonString + "]";
        }
        #endregion

        #region  DataSet转换为Json
        /// <summary> 
        /// DataSet转换为Json 
        /// </summary> 
        /// <param name="dataSet">DataSet对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DataSet dataSet)
        {
            string jsonString = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                jsonString += "\"" + table.TableName + "\":" + ToJson(table) + ",";
            }
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }
        #endregion

        #region Datatable转换为Json
        /// <summary> 
        /// Datatable转换为Json 
        /// </summary> 
        /// <param name="table">Datatable对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName;
                    string strValue = drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }

        /// <summary>
        /// DataTable转换为Json 
        /// </summary>
        public static string ToJson(DataTable dt, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName)) jsonName = dt.TableName;
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + StringFormat(dt.Rows[i][j].ToString(), type));
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }
        #endregion

        #region DataReader转换为Json
        /// <summary> 
        /// DataReader转换为Json 
        /// </summary> 
        /// <param name="dataReader">DataReader对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DbDataReader dataReader)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            while (dataReader.Read())
            {
                jsonString.Append("{");
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    Type type = dataReader.GetFieldType(i);
                    string strKey = dataReader.GetName(i);
                    string strValue = dataReader[i].ToString();
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (i < dataReader.FieldCount - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            dataReader.Close();
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }
        #endregion




        ///// <summary> 
        ///// 返回对象序列化 
        ///// </summary> 
        ///// <param name="obj">源对象</param> 
        ///// <returns>json数据</returns> 
        //public static string ToJson(object obj)
        //{
        //    JavaScriptSerializer serialize = new JavaScriptSerializer();
        //    return serialize.Serialize(obj);
        //}

      
        ///// <summary> 
        ///// DataTable转为json 
        ///// </summary> 
        ///// <param name="dt">DataTable</param> 
        ///// <returns>json数据</returns> 
        //public static string ToJson(this DataTable dt)
        //{
        //    List<object> dic = new List<object>();

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        Dictionary<string, object> result = new Dictionary<string, object>();

        //        foreach (DataColumn dc in dt.Columns)
        //        {
        //            result.Add(dc.ColumnName, dr[dc].ToString());
        //        }
        //        dic.Add(result);
        //    }
        //    return ToJson(dic);
        //}


        /// <summary>
        /// 对象Json转换成字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public static string ToJsonString(this object obj)
        {
            if (!(obj is DataTable))
            {
                return ToJson(obj);
            }
            DataTable table = obj as DataTable;
            return ToJson(table);
        }

        /// <summary>
        /// 将对象转换为Json字符串
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="isConvertToSingleQuotes">是否将双引号转成单引号</param>
        public static string ToJson(object target, bool isConvertToSingleQuotes = false)
        {
            if (target == null)
                return "{}";
            var result = JsonConvert.SerializeObject(target);
            if (isConvertToSingleQuotes)
                result = result.Replace("\"", "'");
            return result;
        }


    }
}
