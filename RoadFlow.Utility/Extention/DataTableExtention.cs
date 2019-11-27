using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RoadFlow.Utility
{
    public static class DataTableExtention
    {
        /// <summary>
        /// DataTable转List
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="dt">数据源</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt)
        {
            List<T> list = new List<T>();

            //确认参数有效,若无效则返回Null
            if (dt == null)
                return list;
            else if (dt.Rows.Count == 0)
                return list;

            Dictionary<string, string> dicField = new Dictionary<string, string>();
            Dictionary<string, string> dicProperty = new Dictionary<string, string>();
            Type type = typeof(T);

            //创建字段字典，方便查找字段名
            type.GetFields().ForEach(aFiled =>
            {
                dicField.Add(aFiled.Name.ToLower(), aFiled.Name);
            });

            //创建属性字典，方便查找属性名
            type.GetProperties().ForEach(aProperty =>
            {
                dicProperty.Add(aProperty.Name.ToLower(), aProperty.Name);
            });

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //创建泛型对象
                T _t = Activator.CreateInstance<T>();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string memberKey = dt.Columns[j].ColumnName.ToLower();

                    //字段赋值
                    if (dicField.ContainsKey(memberKey))
                    {
                        FieldInfo theField = type.GetField(dicField[memberKey]);
                        var dbValue = dt.Rows[i][j];
                        if (dbValue.GetType() == typeof(DBNull))
                            dbValue = null;
                        if (dbValue != null)
                        {
                            Type memberType = theField.FieldType;
                            dbValue = dbValue.ChangeType(memberType);
                        }
                        theField.SetValue(_t, dbValue);
                    }
                    //属性赋值
                    if (dicProperty.ContainsKey(memberKey))
                    {
                        PropertyInfo theProperty = type.GetProperty(dicProperty[memberKey]);
                        var dbValue = dt.Rows[i][j];
                        if (dbValue.GetType() == typeof(DBNull))
                            dbValue = null;
                        if (dbValue != null)
                        {
                            Type memberType = theProperty.PropertyType;
                            if (memberType.IsGenericType && memberType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                NullableConverter newNullableConverter = new NullableConverter(memberType);
                                dbValue = newNullableConverter.ConvertFrom(dbValue);
                            }
                            else
                            {
                                dbValue = Convert.ChangeType(dbValue, memberType);
                            }
                        }
                        theProperty.SetValue(_t, dbValue);
                    }
                }
                list.Add(_t);
            }
            return list;
        }

        /// <summary>
        /// DataTable转List
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="dt">数据源</param>
        /// <returns></returns>
        public static List<T> EmitToList<T>(this DataTable dt)
        {
            //确认参数有效
            if (dt == null)
                return null;

            List<T> list = new List<T>();
            var objBuilder = EmitHelper.CreateBuilder(typeof(T));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //创建泛型对象
                T _t = (T)objBuilder();
                //获取对象所有属性
                PropertyInfo[] propertyInfo = _t.GetType().GetProperties();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    foreach (PropertyInfo info in propertyInfo)
                    {
                        //属性名称和列名相同时赋值
                        if (dt.Columns[j].ColumnName.ToUpper().Equals(info.Name.ToUpper()))
                        {
                            if (dt.Rows[i][j] != DBNull.Value)
                            {
                                info.SetValue(_t, dt.Rows[i][j], null);
                            }
                            else
                            {
                                info.SetValue(_t, null, null);
                            }
                            break;
                        }
                    }
                }
                list.Add(_t);
            }
            return list;
        }

        /// <summary>
        ///将DataTable转换为标准的CSV字符串
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>返回标准的CSV</returns>
        public static string ToCsvStr(this DataTable dt)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    colum = dt.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }


        /// <summary>
        /// 数据表转化为前端table
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTableToHtml(DataTable dt)
        {
            if (dt.Columns.Count == 0)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("<table class=\"listtable\" style=\"WORD-BREAK:break-all;WORD-WRAP:break-word\">");
            builder.Append("<thead>");
            builder.Append("<tr>");
            foreach (DataColumn column in dt.Columns)
            {
                builder.Append("<th>" + column.ColumnName + "</th>");
            }
            builder.Append("</tr>");
            builder.Append("</thead>");
            builder.Append("<tbody>");
            foreach (DataRow row in dt.Rows)
            {
                builder.Append("<tr>");
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    builder.Append("<td>" + row[i].ToString().HtmlEncode() + "</td>");
                }
                builder.Append("</tr>");
            }
            builder.Append("</tbody>");
            builder.Append("</table>");
            return builder.ToString();
        }


        /// <summary>
        /// 要求两个表结构都要一致，才把表2数据添加到表1
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        public static void AddTableData(DataTable dt1, DataTable dt2)
        {
            if (dt2 == null || dt2.Rows.Count == 0) return;
            // 改进后的方法
            DataRow drcalc;
            foreach (DataRow dr in dt2.Rows)
            {
                drcalc = dt1.NewRow();
                drcalc.ItemArray = dr.ItemArray;
                dt1.Rows.Add(drcalc);
            }
        }


        /// <summary>
        ///     给DataTable增加一个自增列
        ///     如果DataTable 存在 identityid 字段  则 直接返回DataTable 不做任何处理
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>返回Datatable 增加字段 identityid </returns>
        public static DataTable AddIdentityColumn(DataTable dt)
        {
            if (!dt.Columns.Contains("identityid"))
            {
                dt.Columns.Add("identityid");
                for (var i = 0; i < dt.Rows.Count; i++) dt.Rows[i]["identityid"] = (i + 1).ToString();
            }

            return dt;
        }



        /// <summary>
        ///     实体列表转换成DataTable
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="list"> 实体列表</param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(IList<T> list)
            where T : class
        {
            if (list == null || list.Count <= 0) return null;
            var dt = new DataTable(typeof(T).Name);
            DataColumn column;
            DataRow row;

            var myPropertyInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var length = myPropertyInfo.Length;
            var createColumn = true;

            foreach (var t in list)
            {
                if (t == null) continue;

                row = dt.NewRow();
                for (var i = 0; i < length; i++)
                {
                    var pi = myPropertyInfo[i];
                    var name = pi.Name;
                    if (createColumn)
                    {
                        column = new DataColumn(name, pi.PropertyType);
                        dt.Columns.Add(column);
                    }

                    row[name] = pi.GetValue(t, null);
                }

                if (createColumn) createColumn = false;

                dt.Rows.Add(row);
            }

            return dt;
        }


        /// <summary>
        ///     将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToDataTable<T>(IList<T> list)
        {
            return ToDataTable(list, null);
        }

        /// <summary>
        ///     将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            var propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);

            var result = new DataTable();
            if (list.Count > 0)
            {
                var propertys = list[0].GetType().GetProperties();
                foreach (var pi in propertys)
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name)) result.Columns.Add(pi.Name, pi.PropertyType);
                    }

                for (var i = 0; i < list.Count; i++)
                {
                    var tempList = new ArrayList();
                    foreach (var pi in propertys)
                        if (propertyNameList.Count == 0)
                        {
                            var obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                var obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }

                    var array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }

            return result;
        }



        /// <summary>
        ///     排序表的视图
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sorts"></param>
        /// <returns></returns>
        public static DataTable SortedTable(DataTable dt, params string[] sorts)
        {
            if (dt.Rows.Count > 0)
            {
                var tmp = "";
                for (var i = 0; i < sorts.Length; i++) tmp += sorts[i] + ",";
                dt.DefaultView.Sort = tmp.TrimEnd(',');
            }

            return dt;
        }

        /// <summary>
        ///     根据条件过滤表的内容
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static DataTable FilterDataTable(DataTable dt, string condition)
        {
            if (condition.Trim() == "") return dt;
            var newdt = new DataTable();
            newdt = dt.Clone();
            var dr = dt.Select(condition);
            for (var i = 0; i < dr.Length; i++) newdt.ImportRow(dr[i]);
            return newdt;
        }




        /// <summary>
        ///     使用分隔符串联表格字段的内容,如：a,b,c
        /// </summary>
        /// <param name="dt">表格</param>
        /// <param name="columnName">字段名称</param>
        /// <param name="append">增加的字符串，无则为空</param>
        /// <param name="splitChar">分隔符，如逗号(,)</param>
        /// <returns></returns>
        public static string ConcatColumnValue(DataTable dt, string columnName, string append, char splitChar)
        {
            var result = append;
            if (dt != null && dt.Rows.Count > 0)
                foreach (DataRow row in dt.Rows)
                    result += string.Format("{0}{1}", splitChar, row[columnName]);
            return result.Trim(splitChar);
        }



        /// <summary>
        /// 通过表单获取字符串
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldname"></param>
        /// <returns></returns>
        public static string GetTableJoinstr(DataTable table, string fieldname)
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (DataRow row in table.Rows)
            {
                if (table.Rows.Count == i)
                {
                    sb.Append(row[fieldname]);
                }
                else
                {
                    sb.Append(row[fieldname] + ",");
                }
                i++;
            }
            return sb.ToString();
        }


        /// <summary>
        /// DataTable分页并取出指定页码的数据
        /// </summary>
        /// <param name="dtAll">DataTable</param>
        /// <param name="pageNo">页码,注意：从1开始</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>指定页码的DataTable数据</returns>
        public static DataTable GetPagedTable(DataTable dtAll, int pageNo, int pageSize)
        {
            var totalCount = dtAll.Rows.Count;
            var totalPage = getTotalPage(totalCount, pageSize);
            var currentPage = pageNo;
            currentPage = (currentPage > totalPage ? totalPage : currentPage);//如果PageNo过大，则较正PageNo=PageCount
            currentPage = (currentPage <= 0 ? 1 : currentPage);//如果PageNo<=0，则改为首页
            //----克隆表结构到新表
            var onePageTable = dtAll.Clone();
            //----取出1页数据到新表
            var rowBegin = (currentPage - 1) * pageSize;
            var rowEnd = currentPage * pageSize;
            rowEnd = (rowEnd > totalCount ? totalCount : rowEnd);
            for (var i = rowBegin; i <= rowEnd - 1; i++)
            {
                var newRow = onePageTable.NewRow();
                var oldRow = dtAll.Rows[i];
                foreach (DataColumn column in dtAll.Columns)
                {
                    newRow[column.ColumnName] = oldRow[column.ColumnName];
                }
                onePageTable.Rows.Add(newRow);
            }
            return onePageTable;
        }

        /// <summary>
        /// 返回分页后的总页数
        /// </summary>
        /// <param name="totalCount">总记录条数</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <returns>总页数</returns>
        public static int getTotalPage(int totalCount, int pageSize)
        {
            var totalPage = (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0);
            return totalPage;
        }





        public static DataTable GetPagedTable1(DataTable dtAllEas, int PageIndex, int PageSize)
        {
            var rows = dtAllEas.Rows.Cast<DataRow>();
            return DataRowToDataTable(rows.Skip(PageIndex).Take(PageSize).ToArray());
        }


        /// <summary>
        /// DataRow转成TataTable表
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static DataTable DataRowToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone(); // 复制DataRow的表结构
            foreach (DataRow row in rows)
            {
                tmp.ImportRow(row); // 将DataRow添加到DataTable中
            }
            return tmp;
        }



    }
}
