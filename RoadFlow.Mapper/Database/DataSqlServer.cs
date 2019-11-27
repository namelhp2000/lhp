using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using RoadFlow.Utility;

namespace RoadFlow.Mapper
{


    #region SqlServer数据转换


    public class DataSqlServer : IData
    {
        // Methods
        public void BulkCopy<T>(IEnumerable<T> ts, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            DataTable table = Common.ListToDataTable<T>(ts);
            SqlBulkCopy copy1 = new SqlBulkCopy((SqlConnection)dbConnection, 0, (SqlTransaction)dbTransaction);
            copy1.DestinationTableName=table.TableName;
            copy1.WriteToServer(table);
            copy1.Close();
        }

        public ValueTuple<string, DbParameter[]> GetFindSql<T>(params object[] objects) where T : class, new()
        {

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string tableName = Common.GetTableName(type);
            List<DbParameter> list = new List<DbParameter>();
            StringBuilder builder = new StringBuilder();
            builder.Append(" WHERE 1=1");
            int index = 0;
            foreach (string str2 in Common.GetPrimaryKeys(properties))
            {
                builder.Append(" AND " + str2 + "=@" + str2);
                if (objects.Length > index)
                {
                    object obj2 = objects[index];
                    list.Add(new SqlParameter("@" + str2, obj2));
                }
                index++;
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM " + tableName + builder.ToString(), list.ToArray());
        }

        public string GetIdentitySql(string seqName = "")
        {
            return "SELECT @@IDENTITY";
        }

    
        public ValueTuple<string, List<DbParameter[]>> GetInsertSql<T>(IEnumerable<T> ts) where T : class, new()
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string tableName = Common.GetTableName(type);
            List<DbParameter[]> list = new List<DbParameter[]>();
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return new ValueTuple<string, List<DbParameter[]>>(string.Empty, list);
            }
            StringBuilder builder = new StringBuilder();
            foreach (PropertyInfo info in properties)
            {
                builder.Append("@" + info.Name + ",");
            }
            foreach (T local in ts)
            {
                List<DbParameter> list2 = new List<DbParameter>();
                foreach (PropertyInfo info2 in properties)
                {
                    object obj2 = info2.GetValue(local);
                    list2.Add(new SqlParameter("@" + info2.Name, obj2 ?? DBNull.Value));
                }
                list.Add(list2.ToArray());
            }
            string[] textArray1 = new string[5];
            textArray1[0] = "INSERT INTO ";
            textArray1[1] = (string)tableName;
            textArray1[2] = " VALUES(";
            char[] trimChars = new char[] { ',' };
            textArray1[3] = (string)builder.ToString().TrimEnd(trimChars);
            textArray1[4] = ")";
            return new ValueTuple<string, List<DbParameter[]>>(string.Concat((string[])textArray1), list);
        }





        public ValueTuple<string, DbParameter[]> GetInsertSql<T>(T t) where T : class, new()
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string tableName = Common.GetTableName(type);
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return new ValueTuple<string, DbParameter[]>(string.Empty, new SqlParameter[0]);
            }
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            foreach (PropertyInfo info in properties)
            {
                builder.Append("@" + info.Name + ",");
                object obj2 = info.GetValue(t);
                list.Add(new SqlParameter("@" + info.Name, obj2 ?? DBNull.Value));
            }
            string[] textArray1 = new string[5];
            textArray1[0] = "INSERT INTO ";
            textArray1[1] = (string)tableName;
            textArray1[2] = " VALUES(";
            char[] trimChars = new char[] { ',' };
            textArray1[3] = (string)builder.ToString().TrimEnd(trimChars);
            textArray1[4] = ")";
            return new ValueTuple<string, DbParameter[]>(string.Concat((string[])textArray1), list.ToArray());
        }


        public ValueTuple<string, DbParameter[]> GetInsertIntSql<T>(T t, string ConnectionString) where T : class, new()
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string tableName = Common.GetTableName(type);

            if (string.IsNullOrWhiteSpace(tableName))
            {
                return new ValueTuple<string, DbParameter[]>(string.Empty, new SqlParameter[0]);
            }
            string KeyValue = string.Empty;
            //using (DataContext context = new DataContext("SqlServer", ConnectionString))
            //{
            //    KeyValue= context.SqlserverKey(tableName);
            //}
            KeyValue = GetTableKey(tableName, ConnectionString);
           StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            foreach (PropertyInfo info in properties)
            {
                if(!KeyValue.ToLower().Contains(info.Name.ToString().ToLower()))
                {
                    builder.Append("@" + info.Name + ",");
                    object obj2 = info.GetValue(t);
                    list.Add(new SqlParameter("@" + info.Name, obj2 ?? DBNull.Value));

                }
            }
            string[] textArray1 = new string[5];
            textArray1[0] = "INSERT INTO ";
            textArray1[1] = (string)tableName;
            textArray1[2] = " VALUES(";
            char[] trimChars = new char[] { ',' };
            textArray1[3] = (string)builder.ToString().TrimEnd(trimChars);
            textArray1[4] = ")";
            return new ValueTuple<string, DbParameter[]>(string.Concat((string[])textArray1), list.ToArray());
        }



        public string GetQueryOneSql(string sql)
        {
            return ("SELECT TOP 1 * FROM(" + sql + ") t");
        }

        public ValueTuple<string, List<DbParameter[]>> GetRemoveSql<T>(IEnumerable<T> ts) where T : class, new()
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string tableName = Common.GetTableName(type);
            List<string> primaryKeys = Common.GetPrimaryKeys(properties);
            List<DbParameter[]> list2 = new List<DbParameter[]>();
            if (string.IsNullOrWhiteSpace(tableName) || !Enumerable.Any<string>((IEnumerable<string>)primaryKeys))
            {
                return new ValueTuple<string, List<DbParameter[]>>(string.Empty, list2);
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(" WHERE 1=1");
            foreach (string str2 in primaryKeys)
            {
                builder.Append(" AND " + str2 + "=@" + str2);
            }
            foreach (T local in ts)
            {
                List<DbParameter> list3 = new List<DbParameter>();
                foreach (string str3 in primaryKeys)
                {
                    object obj2 = null;
                    foreach (PropertyInfo info in properties)
                    {
                        if (info.Name.Equals(str3))
                        {
                            obj2 = info.GetValue(local);
                            break;
                        }
                    }
                    list3.Add(new SqlParameter("@" + str3, obj2));
                }
                list2.Add(list3.ToArray());
            }
            return new ValueTuple<string, List<DbParameter[]>>("DELETE FROM " + tableName + builder.ToString(), list2);
        }

        public ValueTuple<string, DbParameter[]> GetRemoveSql<T>(T t) where T : class, new()
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string tableName = Common.GetTableName(type);
            List<string> primaryKeys = Common.GetPrimaryKeys(properties);
            if (string.IsNullOrWhiteSpace(tableName) || !Enumerable.Any<string>((IEnumerable<string>)primaryKeys))
            {
                return new ValueTuple<string, DbParameter[]>(string.Empty, new SqlParameter[0]);
            }
            List<DbParameter> list2 = new List<DbParameter>();
            StringBuilder builder = new StringBuilder();
            builder.Append(" WHERE 1=1");
            foreach (string str2 in primaryKeys)
            {
                object obj2 = null;
                foreach (PropertyInfo info in properties)
                {
                    if (info.Name.Equals(str2))
                    {
                        obj2 = info.GetValue(t);
                        break;
                    }
                }
                if (obj2 != null)
                {
                    builder.Append(" AND " + str2 + "=@" + str2);
                    list2.Add(new SqlParameter("@" + str2, obj2));
                }
            }
            return new ValueTuple<string, DbParameter[]>("DELETE FROM " + tableName + builder.ToString(), list2.ToArray());
        }

        public ValueTuple<string, DbParameter[]> GetSqlAndParameter(string sql, params object[] objects)
        {
            if ((objects == null) || (objects.Length == 0))
            {
                return new ValueTuple<string, DbParameter[]>(sql, new SqlParameter[0]);
            }
            if (objects != null)
            {
                objects = Enumerable.ToArray<object>(objects);
            }
            List<string> list = new List<string>();
            int num = 0;
            List<DbParameter> list2 = new List<DbParameter>();
            foreach (object obj2 in objects)
            {
                int num3 = num++;
                string str = "@p" + ((int)num3).ToString();
                list.Add(str);
                list2.Add(new SqlParameter(str, obj2 ?? DBNull.Value));
            }
            sql = string.Format(sql, (object[])list.ToArray());
            return new ValueTuple<string, DbParameter[]>(sql, list2.ToArray());
        }

        public ValueTuple<string, List<DbParameter[]>> GetUpdateSql<T>(IEnumerable<T> ts) where T : class, new()
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string tableName = Common.GetTableName(type);
            StringBuilder builder = new StringBuilder();
            List<DbParameter[]> list = new List<DbParameter[]>();
            List<string> primaryKeys = Common.GetPrimaryKeys(properties);
            if (string.IsNullOrWhiteSpace(tableName) || !Enumerable.Any<string>((IEnumerable<string>)primaryKeys))
            {
                return new ValueTuple<string, List<DbParameter[]>>(string.Empty, list);
            }
            foreach (PropertyInfo info in properties)
            {
                if (!primaryKeys.Contains(info.Name))
                {
                    builder.Append(info.Name + "=@" + info.Name + ",");
                }
            }
            StringBuilder builder2 = new StringBuilder();
            builder2.Append(" WHERE 1=1");
            foreach (string str2 in primaryKeys)
            {
                builder2.Append(" AND " + str2 + "=@" + str2);
            }
            foreach (T local in ts)
            {
                List<DbParameter> list3 = new List<DbParameter>();
                foreach (PropertyInfo info2 in properties)
                {
                    object obj2 = info2.GetValue(local);
                    list3.Add(new SqlParameter("@" + info2.Name, obj2 ?? DBNull.Value));
                }
                list.Add(list3.ToArray());
            }
            string[] textArray1 = new string[5];
            textArray1[0] = "UPDATE ";
            textArray1[1] = (string)tableName;
            textArray1[2] = " SET ";
            char[] trimChars = new char[] { ',' };
            textArray1[3] = (string)builder.ToString().TrimEnd(trimChars);
            textArray1[4] = (string)builder2.ToString();
            return new ValueTuple<string, List<DbParameter[]>>(string.Concat((string[])textArray1), list);
        }

        public ValueTuple<string, DbParameter[]> GetUpdateSql<T>(T t) where T : class, new()
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string tableName = Common.GetTableName(type);
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            List<string> primaryKeys = Common.GetPrimaryKeys(properties);
            if (string.IsNullOrWhiteSpace(tableName) || !Enumerable.Any<string>((IEnumerable<string>)primaryKeys))
            {
                return new ValueTuple<string, DbParameter[]>(string.Empty, new SqlParameter[0]);
            }
            foreach (PropertyInfo info in properties)
            {
                if (!primaryKeys.Contains(info.Name))
                {
                    builder.Append(info.Name + "=@" + info.Name + ",");
                }
            }
            StringBuilder builder2 = new StringBuilder();
            builder2.Append(" WHERE 1=1");
            foreach (string str2 in primaryKeys)
            {
                builder2.Append(" AND " + str2 + "=@" + str2);
            }
            foreach (PropertyInfo info2 in properties)
            {
                object obj2 = info2.GetValue(t);
                list.Add(new SqlParameter("@" + info2.Name, obj2 ?? DBNull.Value));
            }
            string[] textArray1 = new string[5];
            textArray1[0] = "UPDATE ";
            textArray1[1] = (string)tableName;
            textArray1[2] = " SET ";
            char[] trimChars = new char[] { ',' };
            textArray1[3] = (string)builder.ToString().TrimEnd(trimChars);
            textArray1[4] = (string)builder2.ToString();
            return new ValueTuple<string, DbParameter[]>(string.Concat((string[])textArray1), list.ToArray());
        }

        public List<T> ReaderToList<T>(DbDataReader reader) where T : class, new()
        {
            List<T> list = new List<T>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            while (reader.Read())
            {
                T local = Activator.CreateInstance<T>();
                object[] values = new object[properties.Length];
                reader.GetValues(values);
                int num = 0;
                foreach (PropertyInfo info in properties)
                {
                    object obj2 = values[num++];
                    if (obj2 != DBNull.Value)
                    {
                        info.SetValue(local, obj2);
                    }
                }
                list.Add(local);
            }
            reader.Close();
            reader.Dispose();
            return list;
        }

        public string GetTableKey(string tablename, string ConnectionString)
        {
            StringBuilder builder = new StringBuilder();
            string sql = string.Format("select b.column_name\r\nfrom information_schema.table_constraints a\r\ninner join information_schema.constraint_column_usage b\r\non a.constraint_name = b.constraint_name\r\nwhere a.constraint_type = 'PRIMARY KEY' and a.table_name = '{0}'", tablename);
            DataTable table = new DataTable();
            using (DataContext context = new DataContext("SqlServer", ConnectionString))
            {
                table = context.GetDataTable(sql, null);
            }
            foreach (DataRow row in table.Rows)
            {
                builder.Append(row[0].ToString());
                builder.Append(",");
            }
            return builder.ToString();
        }

        public string GetPaerSql(string sql, int size, int number, out int count, string ConnType, string ConnString, DbParameter[] param = null, string order = "")
        {
            int num;
            string str = string.Empty;
            using (DataContext context = (ConnString.IsNullOrEmpty()) ? new DataContext() : new DataContext(ConnType, ConnString, true))
            {
                str = context.ExecuteScalarString(string.Format("SELECT COUNT(*) FROM ({0}) AS PagerCountTemp", sql), param);
            }
            count = str.IsInt(out num) ? num : 0;
            if (count < (((number * size) - size) + 1))
            {
                number = 1;
            }
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("SELECT * FROM (");
            if (!sql.ContainsIgnoreCase("ROW_NUMBER()"))
            {
                sql = sql.Insert(sql.IndexOfIgnoreCase("from") - 1, ",ROW_NUMBER() OVER(ORDER BY " + order + ") AS PagerAutoRowNumber");
            }
            builder1.Append(sql);
            builder1.AppendFormat(") AS PagerTempTable", Array.Empty<object>());
            builder1.AppendFormat(" WHERE PagerAutoRowNumber BETWEEN {0} AND {1}", (int)(((number * size) - size) + 1), (int)(number * size));
            return builder1.ToString();
        }

        public (string, DbParameter) GetConditions(string typename, string typevalue, datatype type)
        {
            StringBuilder builder = new StringBuilder();
            DbParameter dbp = null;
            DateTime time;
            DateTime time2;
            Guid guid;
            int num;
            string typename1 = typename.IndexOf('.') > 0 ? typename.Substring(typename.IndexOf('.') + 1) : typename;
            switch (type)
            {
                case datatype.stringType:
                    if (!typevalue.IsNullOrWhiteSpace()) //字符串
                    {
                        builder.Append(" AND CHARINDEX(@" + typename1 + "," + typename + ") >0");
                        dbp = new SqlParameter("@" + typename1, typevalue.Trim());
                    }
                    break;
                case datatype.stringTypeIn:
                    if (!typevalue.IsNullOrWhiteSpace()) //字符串
                    {
                        builder.Append(" AND " + typename + " in (" + typevalue.Trim() + ")");

                    }
                    break;
                case datatype.dataStartType:

                    if (typevalue.IsDateTime(out time))  //开始日期
                    {
                        builder.Append(" AND " + typename + " >= @" + typename1);
                        dbp = new SqlParameter("@" + typename1, time.GetDate());
                    }
                    break;
                case datatype.dataEndType:
                    if (typevalue.IsDateTime(out time2))  //结束日期
                    {
                        builder.Append(" AND " + typename + " <@" + typename1 + "1");
                        dbp = new SqlParameter("@" + typename1 + "1", time2.AddDays(1.0).GetDate());
                    }
                    break;
                case datatype.guidType:
                    if (typevalue.IsGuid(out guid)) //guid格式
                    {
                        builder.Append(" AND " + typename + " = @" + typename1);
                        dbp = new SqlParameter("@" + typename1, guid);
                    }
                    break;
                //case datatype.typeType:
                //    if (!typename.IsNullOrWhiteSpace()) //类型
                //    {
                //        builder.Append(" AND "+ typename+" IN(" + typename + ")");
                //    }
                //    break;
                case datatype.typenumType:
                    if (typevalue.IsInt(out num))   //数值
                    {
                        builder.Append(" AND " + typename + " = @" + typename1);
                        dbp = new SqlParameter("@" + typename1, (int)num);
                    }
                    break;
                //case datatype.intTypeIn:
                //    if (!typevalue.IsInt(out num)) //数值
                //    {
                //        builder.Append(" AND " + typename + " in (" + typename + ")");
                //        dbp = new SqlParameter("@" + typename, (int)num);
                //    }
                //    break;
                //case 目标值n:
                //    执行语句n;
                //    break;

                default:
                    builder.Append(" ");
                    dbp = null;
                    break;
            }

            return (builder.ToString(), dbp);
        }
    }

  
    #endregion



}
