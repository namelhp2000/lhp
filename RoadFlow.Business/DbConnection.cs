
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{



   
    public class DbConnection
    {
        // Fields
        private readonly RoadFlow.Data.DbConnection dbConnectionData = new RoadFlow.Data.DbConnection();

        // Methods
        public int Add(RoadFlow.Model.DbConnection dbConnection)
        {
            return this.dbConnectionData.Add(dbConnection);
        }

        public int Delete(RoadFlow.Model.DbConnection[] dbConnections)
        {
            return this.dbConnectionData.Delete(dbConnections);
        }

        public string ExecuteSQL(Guid id, List<ValueTuple<string, IEnumerable<object>>> ps)
        {
            RoadFlow.Model.DbConnection dbConnection = this.Get(id);
            if (dbConnection == null)
            {
                return "未找到连接实体";
            }
            return this.dbConnectionData.ExecuteSQL(dbConnection, ps);
        }


        // [Return: TupleElementNames(new string[] { "sql", "parameters" })]
        public string ExecuteSQL(Guid id, List<ValueTuple<string, DbParameter[]>> tuples)
        {
            RoadFlow.Model.DbConnection dbConnection = this.Get(id);
            if (dbConnection == null)
            {
                return "未找到连接实体";
            }
            return this.dbConnectionData.ExecuteSQL(dbConnection, tuples);
        }

        public string ExecuteSQL(Guid id, string sql, IEnumerable<object> paramObjs)
        {
            RoadFlow.Model.DbConnection dbConnection = this.Get(id);
            if (dbConnection == null)
            {
                return "未找到连接实体";
            }
            return this.dbConnectionData.ExecuteSQL(dbConnection, sql, paramObjs);
        }

        public string ExecuteSQL(Guid id, string sql, DbParameter[] parameters = null)
        {
            List<ValueTuple<string, DbParameter[]>> tuples = new List<ValueTuple<string, DbParameter[]>> {
            new ValueTuple<string, DbParameter[]>(sql, parameters)
        };
            return this.ExecuteSQL(id, tuples);
        }

        public string ExecuteSQL(Guid id, string sql, params object[] paramsObj)
        {
            RoadFlow.Model.DbConnection dbConnection = this.Get(id);
            if (dbConnection == null)
            {
                return "未找到连接实体";
            }
            return this.dbConnectionData.ExecuteSQL(dbConnection, sql, paramsObj);
        }

        public RoadFlow.Model.DbConnection Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.DbConnection p) {
                return p.Id == id;
            });
        }

        public List<RoadFlow.Model.DbConnection> GetAll()
        {
            return this.dbConnectionData.GetAll();
        }

        public string GetConnTypeOptions(string value = "")
        {
            StringBuilder builder = new StringBuilder();
            foreach (object obj2 in Enum.GetValues((Type)typeof(DatabaseType)))
            {
                builder.AppendFormat("<option value=\"{0}\" {1}>{0}</option>", obj2, obj2.ToString().EqualsIgnoreCase(value) ? "selected=\"selected\"" : "");
            }
            return builder.ToString();
        }

        public DataTable GetDataTable(RoadFlow.Model.DbConnection dbConnectionModel, string sql, DbParameter[] parameters = null)
        {
            if (dbConnectionModel == null)
            {
                return new DataTable();
            }
            return this.dbConnectionData.GetDataTable(dbConnectionModel, sql, parameters);
        }


      




        public DataTable GetDataTable(RoadFlow.Model.DbConnection dbConnectionModel, string sql, params object[] objects)
        {
            return this.dbConnectionData.GetDataTable(dbConnectionModel, sql, objects);
        }

        public DataTable GetDataTable(Guid id, string sql, DbParameter[] parameters = null)
        {
            return this.GetDataTable(this.Get(id), sql, parameters);
        }

        public DataTable GetDataTable(Guid id, string sql, params object[] objects)
        {
            return this.GetDataTable(this.Get(id), sql, objects);
        }

        public DataTable GetDataTable(RoadFlow.Model.DbConnection dbConnectionModel, string tableName, string primaryKey, string primaryKeyValue, string order = "")
        {
            if (((dbConnectionModel == null) || primaryKeyValue.IsNullOrWhiteSpace()) || (tableName.IsNullOrWhiteSpace() || primaryKey.IsNullOrWhiteSpace()))
            {
                return new DataTable();
            }
            DataTable table = this.dbConnectionData.GetDataTable(dbConnectionModel, tableName, primaryKey, primaryKeyValue);
            if (!order.IsNullOrWhiteSpace())
            {
                table.DefaultView.Sort = order;
                return table.DefaultView.ToTable();
            }
            return table;
        }

        public List<string> GetFieldsBySql(Guid connId, string sql, DbParameter[] parameters = null)
        {
            sql = Wildcard.Filter(sql, null, null).FilterSelectSql();
            List<string> list = new List<string>();
            foreach (DataColumn column in this.dbConnectionData.GetDataTableSchema(this.Get(connId), sql, parameters).Columns)
            {
                list.Add(column.ColumnName);
            }
            return list;
        }

        public string GetFieldValue(Guid id, string sql)
        {
            RoadFlow.Model.DbConnection dbConnection = this.Get(id);
            if (dbConnection == null)
            {
                return string.Empty;
            }
            return this.dbConnectionData.GetFieldValue(dbConnection, sql);
        }

        public string GetFieldValue(Guid id, string sql, params object[] objs)
        {
            RoadFlow.Model.DbConnection dbConnection = this.Get(id);
            if (dbConnection == null)
            {
                return string.Empty;
            }
            return this.dbConnectionData.GetFieldValue(dbConnection, sql, objs);
        }

        public string GetFieldValue(Guid id, string tableName, string fieldName, string primaryKey, string primaryKeyValue)
        {
            RoadFlow.Model.DbConnection dbConnection = this.Get(id);
            if (dbConnection == null)
            {
                return string.Empty;
            }
            return this.dbConnectionData.GetFieldValue(dbConnection, tableName, fieldName, primaryKey, primaryKeyValue);
        }

        public int GetMaxSort()
        {
            List<RoadFlow.Model.DbConnection> all = this.GetAll();
            if (all.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.DbConnection>((IEnumerable<RoadFlow.Model.DbConnection>)all, 
                key=>key.Sort) + 5);
        }

        public string GetOptions(string value = "")
        {
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.DbConnection connection in this.GetAll())
            {
                builder.AppendFormat("<option value=\"{0}\" {1}>{2}</option>", connection.Id, connection.Id.ToString().EqualsIgnoreCase(value) ? "selected=\"selected\"" : "", connection.Name);
            }
            return builder.ToString();
        }

        public string GetTableFieldOptions(Guid id, string tableName, string fieldName = "")
        {
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.TableField field in this.GetTableFields(id, tableName))
            {
                builder.AppendFormat("<option value=\"{0}\" {1}>{2}</option>", field.FieldName, field.FieldName.EqualsIgnoreCase(fieldName) ? "selected=\"selected\"" : "", field.FieldName + (field.Comment.IsNullOrWhiteSpace() ? "" : ("(" + field.Comment + ")")));
            }
            return builder.ToString();
        }

        public List<RoadFlow.Model.TableField> GetTableFields(RoadFlow.Model.DbConnection conn, string tableName)
        {
            List<RoadFlow.Model.TableField> list = new List<RoadFlow.Model.TableField>();
            foreach (DataRow row in this.dbConnectionData.GetTableFields(conn, tableName).Rows)
            {
                RoadFlow.Model.TableField field = new RoadFlow.Model.TableField
                {
                    FieldName = row["f_name"].ToString(),
                    Type = row["t_name"].ToString(),
                    Size = row["length"].ToString().ToInt(-2147483648),
                    IsNull = "1".Equals(row["is_null"].ToString()),
                    IsDefault = row["cdefault"].ToString().ToInt(-2147483648) > 0,
                    IsIdentity = row["isidentity"].ToString().ToInt(-2147483648) == 1,
                    DefaultValue = row["defaultvalue"].ToString(),
                    Comment = row["comments"].ToString()
                };
                list.Add(field);
            }
            return list;
        }

        public List<RoadFlow.Model.TableField> GetTableFields(Guid id, string tableName)
        {
            RoadFlow.Model.DbConnection conn = this.Get(id);
            if (conn == null)
            {
                return new List<RoadFlow.Model.TableField>();
            }
            return this.GetTableFields(conn, tableName);
        }

        public string GetTableOptions(Guid id, string value = "")
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in this.GetTables(id))
            {
                string introduced3 = pair.Key;
                string introduced4 = pair.Key;
                builder.AppendFormat("<option value=\"{0}\" {1}>{2}</option>", introduced3, pair.Key.EqualsIgnoreCase(value) ? "selected=\"selected\"" : "", introduced4 + (pair.Value.IsNullOrWhiteSpace() ? "" : ("(" + pair.Value + ")")));
            }
            return builder.ToString();
        }

        public Dictionary<string, string> GetTables(Guid id)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            RoadFlow.Model.DbConnection dbConnection = this.Get(id);
            if (dbConnection != null)
            {
                foreach (DataRow row in this.dbConnectionData.GetTables(dbConnection).Rows)
                {
                    dictionary.Add(row[0].ToString(), row[1].ToString());
                }
            }
            return dictionary;
        }

        public DatabaseType ParseConnType(string connType)
        {
            return (DatabaseType)Enum.Parse((Type)typeof(DatabaseType), connType, true);
        }


        //[return:TupleElementNames(new string[] { "dicts", "tableName", "primaryKey", "flag" })]
        public string SaveData(RoadFlow.Model.DbConnection dbConnection,  List<ValueTuple<Dictionary<string, object>, string, string, int>> tuples, bool isIdentity = false, string seqName = "")
        {
            if (tuples.Count != 0)
            {
                return this.dbConnectionData.SaveData(dbConnection, tuples, isIdentity, seqName);
            }
            return "0";
        }

        public string TestConnection(Guid id)
        {
            RoadFlow.Model.DbConnection dbConnection = this.Get(id);
            if (dbConnection == null)
            {
                return "未找到连接";
            }
            return this.dbConnectionData.TesetConnection(dbConnection);
        }

        public string TestSQL(Guid id, string sql, DbParameter[] parameters = null)
        {
            RoadFlow.Model.DbConnection dbConnection = this.Get(id);
            if (dbConnection == null)
            {
                return "未找到连接实体";
            }
            return this.dbConnectionData.TestSQL(dbConnection, sql, parameters);
        }

        public int Update(RoadFlow.Model.DbConnection dbConnection)
        {
            return this.dbConnectionData.Update(dbConnection);
        }




        /// <summary>
        /// 获取系统表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">0代表表和视图，1代表表，2代表视图</param>
        /// <returns></returns>
        public List<string> GetTables(Guid id, int type = 0)
        {
            RoadFlow.Model.DbConnection connection = this.Get(id);
            if (connection == null)
            {
                return new List<string>();
            }
            List<string> list2 = new List<string>();
            string str = connection.ConnType;
            if (str == null)
            {
                return list2;
            }
            if (str == "SqlServer")
            {
                return this.method_5(connection, type);
            }
            if (!(str == "Oracle"))
            {
                if (str == "MySql")
                {
                    list2 = this.method_7(connection, type);
                }
                return list2;
            }
            return this.method_6(connection, type);
        }


        /// <summary>
        /// 获取SQLserver系统表方法
        /// </summary>
        /// <param name="dbconnection_0"></param>
        /// <param name="int_0"></param>
        /// <returns></returns>
        private List<string> method_5(RoadFlow.Model.DbConnection dbconnection_0, int int_0)
        {
            List<string> list2;
            using (SqlConnection connection = new SqlConnection(dbconnection_0.ConnString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Log.Add("SqlServer异常报错", "", LogType.系统管理, exception.ToString(), "", "", "", "", "", "", "");
                    // Log.Add(exception);
                    return new List<string>();
                }
                List<string> list = new List<string>();
                string str = string.Empty;
                switch (int_0)
                {
                    case 0:
                        str = "xtype='U' or xtype='V'";
                        break;

                    case 1:
                        str = "xtype='U'";
                        break;

                    case 2:
                        str = "xtype='V'";
                        break;
                }
                using (SqlCommand command = new SqlCommand("SELECT name FROM sysobjects WHERE " + str + " ORDER BY xtype, name", connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(reader.GetString(0));
                    }
                    reader.Close();
                    list2 = list;
                }
            }
            return list2;
        }

        /// <summary>
        /// 获取MySQL系统表方法
        /// </summary>
        /// <param name="dbconnection_0"></param>
        /// <param name="int_0"></param>
        /// <returns></returns>
        private List<string> method_7(RoadFlow.Model.DbConnection dbconnection_0, int int_0)
        {
            List<string> list2;
            using (MySqlConnection connection = new MySqlConnection(dbconnection_0.ConnString))
            {
                try
                {
                    connection.Open();
                }
                catch (MySqlException exception)
                {
                    Log.Add("MySql异常报错", "", LogType.系统管理, exception.ToString(), "", "", "", "", "", "", "");
                   
                    return new List<string>();
                }
                List<string> list = new List<string>();
                string str = string.Empty;
                switch (int_0)
                {
                    case 0:
                        str = "table_type='BASE TABLE' or table_type='VIEW'";
                        break;

                    case 1:
                        str = "table_type='BASE TABLE'";
                        break;

                    case 2:
                        str = "table_type='VIEW'";
                        break;
                }
                using (MySqlCommand command = new MySqlCommand(string.Format("show full tables from {0} where " + str, connection.Database), connection))
                {
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(reader.GetString(0));
                    }
                    reader.Close();
                    list2 = list;
                }
            }
            return list2;
        }

        /// <summary>
        /// 获取Oracle系统表方法
        /// </summary>
        /// <param name="dbconnection_0"></param>
        /// <param name="int_0"></param>
        /// <returns></returns>
        private List<string> method_6(RoadFlow.Model.DbConnection dbconnection_0, int int_0)
        {
            List<string> list2;
            using (OracleConnection connection = new OracleConnection(dbconnection_0.ConnString))
            {
                try
                {
                    connection.Open();
                }
                catch (OracleException exception)
                {
                  
                      Log.Add("Oracle异常报错","", LogType.系统管理, exception.ToString(),"","","","","","","");
                    return new List<string>();
                }
                List<string> list = new List<string>();
                string str = string.Empty;
                switch (int_0)
                {
                    case 0:
                        str = "and TABTYPE='TABLE' or TABTYPE='VIEW'";
                        break;

                    case 1:
                        str = "and TABTYPE='TABLE'";
                        break;

                    case 2:
                        str = "and TABTYPE='VIEW'";
                        break;
                }
                using (OracleCommand command = new OracleCommand("select * from tab where instr(tname,'$',1,1)=0 " + str, connection))
                {
                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(reader.GetString(0));
                    }
                    reader.Close();
                    list2 = list;
                }
            }
            return list2;
        }


        /// <summary>
        /// 获取默认查询语句Sql
        /// </summary>
        /// <param name="conn">连接语句</param>
        /// <param name="tableName">表</param>
        /// <returns></returns>
        public string GetDefaultQuerySql(RoadFlow.Model.DbConnection conn, string tableName)
        {
            string str = string.Empty;
            string str2 = conn.ConnType.ToLower();
            if (str2 == null)
            {
                return str;
            }
            if (str2 == "sqlserver")
            {
                return ("SELECT TOP 50 * FROM " + tableName);
            }
            if (!(str2 == "mysql"))
            {
                if (str2 == "oracle")
                {
                    str = "SELECT * FROM " + tableName + " WHERE ROWNUM BETWEEN 0 AND 50";
                }
                return str;
            }
            return ("SELECT * FROM " + tableName + " LIMIT 0,50");
        }


        /// <summary>
        /// 检验查询语句 系统表不允许删除操作处理
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool CheckSql(string sql)
        {
            if ((sql.Contains("delete", StringComparison.CurrentCultureIgnoreCase) || sql.Contains("drop", StringComparison.CurrentCultureIgnoreCase)) || (sql.Contains("alter", StringComparison.CurrentCultureIgnoreCase) || sql.Contains("truncate", StringComparison.CurrentCultureIgnoreCase)))
            {
                foreach (string str in Config.systemDataTables)
                {
                    foreach (string str2 in sql.Split(new char[] { ' ' }))
                    {
                        if (str2.Equals(str, StringComparison.CurrentCultureIgnoreCase) || ("[" + str2 + "]").Equals("[" + str + "]", StringComparison.CurrentCultureIgnoreCase))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// 根据连接实体得到连接
        /// </summary>
        /// <param name="linkID"></param>
        /// <returns></returns>
        public IDbConnection GetConnection(RoadFlow.Model.DbConnection dbconn)
        {
            if (((dbconn == null) || dbconn.ConnType.IsNullOrEmpty()) || dbconn.ConnString.IsNullOrEmpty())
            {
                return null;
            }
            IDbConnection connection = null;
            try
            {
                string type = dbconn.ConnType;
                if (type == null)
                {
                    return connection;
                }
                if (type != "SqlServer")
                {
                    if (!(type == "Oracle"))
                    {
                        if (type == "MySql")
                        {
                            connection = new MySqlConnection(dbconn.ConnString);
                        }
                        return connection;
                    }
                    return new OracleConnection(dbconn.ConnString);
                }
                connection = new SqlConnection(dbconn.ConnString);
            }
            catch (Exception exception)
            {
                Log.Add("实体得到连接异常报错", "", LogType.数据连接, exception.ToString(), "", "", "", "", "", "", "");
               // Log.Add(exception);
            }
            return connection;
        }


        /// <summary>
        /// 得到一个表的结构
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public DataTable GetTableSchema(IDbConnection conn, string tableName, string dbType)
        {
            DataTable dataTable = new DataTable();
            string str7 = dbType;
            if (str7 != null)
            {
                if (str7 != "SqlServer")
                {
                    if (str7 == "Oracle")
                    {
                        new OracleDataAdapter(string.Format("SELECT COLUMN_NAME as f_name,\r\n                    DATA_TYPE as t_name,\r\n                    CHAR_LENGTH AS length,\r\n                    (DATA_PRECISION||','||DATA_SCALE) AS scale,\r\n                    CASE NULLABLE WHEN 'Y' THEN 1 WHEN 'N' THEN 0 END AS is_null,\r\n                    DATA_DEFAULT AS cdefault,\r\n                    0 as isidentity,DATA_DEFAULT AS defaultvalue FROM user_tab_columns WHERE UPPER(TABLE_NAME)=UPPER('{0}') ORDER BY COLUMN_ID", tableName), (OracleConnection)conn).Fill(dataTable);
                        return dataTable;
                    }
                    if (str7 == "MySql")
                    {
                        DataTable table2 = new DataTable();
                        new MySqlDataAdapter("show full fields from `" + tableName + "`", (MySqlConnection)conn).Fill(table2);
                        dataTable.Columns.Add("f_name", "".GetType());
                        dataTable.Columns.Add("t_name", "".GetType());
                        dataTable.Columns.Add("length", "".GetType());
                        dataTable.Columns.Add("scale", 1.GetType());
                        dataTable.Columns.Add("is_null", 1.GetType());
                        dataTable.Columns.Add("cdefault", 1.GetType());
                        dataTable.Columns.Add("isidentity", 1.GetType());
                        dataTable.Columns.Add("defaultvalue", "".GetType());
                        foreach (DataRow row in table2.Rows)
                        {
                            string str4 = row["Type"].ToString();
                            string str5 = row["Type"].ToString();
                            string str6 = "";
                            if (str4.IndexOf("(") > 0)
                            {
                                str4 = str4.Substring(0, str4.IndexOf("("));
                                try
                                {
                                    str6 = str5.Substring(str5.IndexOf("(") + 1, (str5.IndexOf(")") - str5.IndexOf("(")) - 1);
                                }
                                catch
                                {
                                    str6 = "";
                                }
                            }
                            DataRow row2 = dataTable.NewRow();
                            row2["f_name"] = row["Field"].ToString();
                            row2["t_name"] = str4;
                            row2["length"] = str6;
                            row2["scale"] = 0;
                            row2["is_null"] = "YES" == row["Null"].ToString();
                            row2["cdefault"] = row["Default"].ToString().IsNullOrEmpty() ? 1 : 0;
                            row2["isidentity"] = "auto_increment" == row["Extra"].ToString();
                            row2["defaultvalue"] = row["Default"].ToString();
                            dataTable.Rows.Add(row2);
                        }
                    }
                    return dataTable;
                }
                new SqlDataAdapter(string.Format("select a.name as f_name,b.name as t_name,a.prec as [length],a.scale,a.isnullable as is_null, a.cdefault as cdefault,COLUMNPROPERTY( OBJECT_ID('{0}'),a.name,'IsIdentity') as isidentity, \r\n(select top 1 text from sysobjects d inner join syscolumns e on e.id=d.id inner join syscomments f on f.id=e.cdefault \r\nwhere d.name='{0}' and e.name=a.name) as defaultvalue  ,( SELECT cast(ep.[value]as varchar(100)) FROM  sys.tables AS t INNER JOIN sys.columns  AS n ON t.object_id = n.object_id LEFT JOIN sys.extended_properties AS ep   ON ep.major_id = n.object_id AND ep.minor_id = n.column_id WHERE ep.class =1 and n.name=a.name  AND t.name= '{0}') f_describe \r\n  from \r\n                    sys.syscolumns a inner join sys.types b on b.user_type_id=a.xtype \r\n                    where object_id('{0}')=id order by a.colid", tableName), (SqlConnection)conn).Fill(dataTable);
            }
            return dataTable;
        }

        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public List<string> GetPrimaryKey(RoadFlow.Model.DbConnection conn, string table)
        {
            switch (conn.ConnType.ToLower())
            {
                case "sqlserver":
                    return this.method_16(conn, table);

                case "oracle":
                    return this.method_17(conn, table);

                case "mysql":
                    return this.method_18(conn, table);
            }
            return new List<string>();
        }


        /// <summary>
        /// sqlserver获取主键
        /// </summary>
        /// <param name="dbconnection_0"></param>
        /// <param name="string_0"></param>
        /// <returns></returns>
        private List<string> method_16(RoadFlow.Model.DbConnection dbconnection_0, string string_0)
        {
            string sql = string.Format("select b.column_name\r\nfrom information_schema.table_constraints a\r\ninner join information_schema.constraint_column_usage b\r\non a.constraint_name = b.constraint_name\r\nwhere a.constraint_type = 'PRIMARY KEY' and a.table_name = '{0}'", string_0);
            DataTable table = this.GetDataTable(dbconnection_0, sql, null);
            List<string> list = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(row[0].ToString());
            }
            return list;
        }

        /// <summary>
        /// oracle获取主键
        /// </summary>
        /// <param name="dbconnection_0"></param>
        /// <param name="string_0"></param>
        /// <returns></returns>
        private List<string> method_17(RoadFlow.Model.DbConnection dbconnection_0, string string_0)
        {
            string sql = string.Format("select b.column_name from user_constraints a, user_cons_columns b where a.constraint_name = b.constraint_name and a.constraint_type = 'P' and a.table_name = UPPER('{0}')", string_0);
            DataTable table = this.GetDataTable(dbconnection_0, sql, null);
            List<string> list = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(row[0].ToString());
            }
            return list;
        }


        /// <summary>
        /// mysql获取主键
        /// </summary>
        /// <param name="dbconnection_0"></param>
        /// <param name="string_0"></param>
        /// <returns></returns>
        private List<string> method_18(RoadFlow.Model.DbConnection dbconnection_0, string string_0)
        {
            string sql = string.Format("show full fields from `{0}`", string_0);
            DataTable table = this.GetDataTable(dbconnection_0, sql, null);
            List<string> list = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                if (row["key"].ToString().ToUpper() == "PRI")
                {
                    list.Add(row[0].ToString());
                }
            }
            return list;
        }


        /// <summary>
        /// 获取约束
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<string> GetConstraints(RoadFlow.Model.DbConnection dbConn, string tableName)
        {
            List<string> list = new List<string>();
            string str2 = dbConn.ConnType.ToLower();
            if (str2 != null)
            {
                if (!(str2 == "sqlserver"))
                {
                    if ((str2 == "oracle") || (str2 == "mysql"))
                    {
                    }
                    return list;
                }
                string sql = "select name from sysobjects where parent_obj=(select id from sysobjects where name='" + tableName + "' and type='U')";
                foreach (DataRow row in this.GetDataTable(dbConn, sql, null).Rows)
                {
                    list.Add(row[0].ToString());
                }
            }
            return list;
        }


        /// <summary>
        /// 获取字段数据类型选项
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public string GetFieldDataTypeOptions(string value, string dbType)
        {
            string str = string.Empty;
            string str2 = dbType.ToLower();
            if (str2 == null)
            {
                return str;
            }
            if (str2 == "sqlserver")
            {
                return this.method_19(value);
            }
            if (!(str2 == "oracle"))
            {
                if (str2 == "mysql")
                {
                    str = this.method_21(value);
                }
                return str;
            }
            return this.method_20(value);
        }


        /// <summary>
        /// 获取sqlserver字段类型
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        private string method_19(string string_0)
        {
            List<Tuple<string, string, string>> list = new List<Tuple<string, string, string>> {
        new Tuple<string, string, string>("varchar", "英文字符串", "50"),
        new Tuple<string, string, string>("nvarchar", "中文字符串", "50"),
        new Tuple<string, string, string>("char", "字符", "10"),
        new Tuple<string, string, string>("datetime", "日期时间", ""),
        new Tuple<string, string, string>("text", "长文本", ""),
        new Tuple<string, string, string>("uniqueidentifier", "全局唯一ID", ""),
        new Tuple<string, string, string>("int", "整数", ""),
        new Tuple<string, string, string>("decimal", "小数", ""),
        new Tuple<string, string, string>("money", "货币", ""),
        new Tuple<string, string, string>("float", "浮点数", "")
    };
            StringBuilder builder = new StringBuilder();
            foreach (Tuple<string, string, string> tuple in list)
            {
                builder.Append("<option data-length=\"" + tuple.Item3 + "\" value=\"" + tuple.Item1 + "\"" + (tuple.Item1.Equals(string_0, StringComparison.CurrentCultureIgnoreCase) ? " selected=\"selected\"" : "") + ">" + tuple.Item2 + "</option>");
            }
            return builder.ToString();
        }


        /// <summary>
        ///  获取oracle字段类型
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        private string method_20(string string_0)
        {
            List<Tuple<string, string, string>> list = new List<Tuple<string, string, string>> {
        new Tuple<string, string, string>("VARCHAR2", "英文字符串", "50"),
        new Tuple<string, string, string>("NVARCHAR2", "中文字符串", "50"),
        new Tuple<string, string, string>("CHAR", "字符", "10"),
        new Tuple<string, string, string>("DATE", "日期时间", ""),
        new Tuple<string, string, string>("CLOB", "长文本", ""),
        new Tuple<string, string, string>("NCLOB", "中文长文本", ""),
        new Tuple<string, string, string>("NUMBER", "数字", ""),
        new Tuple<string, string, string>("FLOAT", "浮点数", "")
    };
            StringBuilder builder = new StringBuilder();
            foreach (Tuple<string, string, string> tuple in list)
            {
                builder.Append("<option data-length=\"" + tuple.Item3 + "\" value=\"" + tuple.Item1 + "\"" + (tuple.Item1.Equals(string_0, StringComparison.CurrentCultureIgnoreCase) ? " selected=\"selected\"" : "") + ">" + tuple.Item2 + "</option>");
            }
            return builder.ToString();
        }


        /// <summary>
        /// 获取MySql字段类型
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        private string method_21(string string_0)
        {
            List<Tuple<string, string, string>> list = new List<Tuple<string, string, string>> {
        new Tuple<string, string, string>("varchar", "字符串", "255"),
        new Tuple<string, string, string>("char", "字符", "255"),
        new Tuple<string, string, string>("datetime", "日期时间", ""),
        new Tuple<string, string, string>("timestamp", "时间戳", ""),
        new Tuple<string, string, string>("text", "文本", ""),
        new Tuple<string, string, string>("longtext", "长文本", ""),
        new Tuple<string, string, string>("int", "整数", ""),
        new Tuple<string, string, string>("decimal", "小数", ""),
        new Tuple<string, string, string>("float", "浮点数", "")
    };
            StringBuilder builder = new StringBuilder();
            foreach (Tuple<string, string, string> tuple in list)
            {
                builder.Append("<option data-length=\"" + tuple.Item3 + "\" value=\"" + tuple.Item1 + "\"" + (tuple.Item1.Equals(string_0, StringComparison.CurrentCultureIgnoreCase) ? " selected=\"selected\"" : "") + ">" + tuple.Item2 + "</option>");
            }
            return builder.ToString();
        }



        public bool TestSql(RoadFlow.Model.DbConnection dbconn, string sql, bool replaceSql = true)
        {
            if (dbconn == null)
            {
                return false;
            }
            if (replaceSql)
            {
                sql = Wildcard.Filter(sql.ReplaceSelectSql(), RoadFlow.Business.User.CurrentUser, null);
              //  sql = sql.ReplaceSelectSql().FilterWildcard(Users.CurrentUserID.ToString());
            }
            string str = dbconn.ConnType.ToLower();
            if (str != null)
            {
                bool flag;
                if (str != "sqlserver")
                {
                    if (str != "oracle")
                    {
                        if (!(str == "mysql"))
                        {
                            goto Label_01F6;
                        }
                        using (MySqlConnection connection3 = new MySqlConnection(dbconn.ConnString))
                        {
                            try
                            {
                                connection3.Open();
                            }
                            catch (MySqlException exception5)
                            {
                                Log.Add("mysql异常报错", "", LogType.系统管理, exception5.ToString(), "", "", "", "", "", "", "");
                              //  Log.Add(exception5);
                                return false;
                            }
                            MySqlCommand command3 = new MySqlCommand(sql, connection3);
                            try
                            {
                                command3.ExecuteNonQuery();
                                return true;
                            }
                            catch (MySqlException exception6)
                            {
                                Log.Add("执行MySql语句发生了错误", exception6.Message + exception6.StackTrace, LogType.数据连接, sql, "", null);
                                return false;
                            }
                            finally
                            {
                                if (command3 != null)
                                {
                                    command3.Dispose();
                                }
                            }
                        }
                    }
                    using (OracleConnection connection2 = new OracleConnection(dbconn.ConnString))
                    {
                        try
                        {
                            connection2.Open();
                        }
                        catch (OracleException exception3)
                        {
                            Log.Add("Oracle异常报错", "", LogType.系统管理, exception3.ToString(), "", "", "", "", "", "", "");
                          //  Log.Add(exception3);
                            return false;
                        }
                        OracleCommand command2 = new OracleCommand(sql, connection2);
                        try
                        {
                            command2.ExecuteNonQuery();
                            return true;
                        }
                        catch (OracleException exception4)
                        {
                            Log.Add("执行Oracle语句发生了错误", exception4.Message + exception4.StackTrace, LogType.数据连接, sql, "", null);
                            return false;
                        }
                        finally
                        {
                            if (command2 != null)
                            {
                                command2.Dispose();
                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(dbconn.ConnString))
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (SqlException exception)
                    {
                        Log.Add("sqlserver异常报错", "", LogType.系统管理, exception.ToString(), "", "", "", "", "", "", "");
                       // Log.Add(exception);
                        return false;
                    }
                    SqlCommand command = new SqlCommand(sql, connection);
                    try
                    {
                        command.ExecuteNonQuery();
                        flag = true;
                    }
                    catch (SqlException exception2)
                    {
                        Log.Add("执行SqlServer语句发生了错误", exception2.Message + exception2.StackTrace, LogType.数据连接, sql, "", null);
                        flag = false;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }
                    }
                }
                return flag;
            }
        Label_01F6:
            return false;
        }


        public bool TestSql(RoadFlow.Model.DbConnection dbconn, string sql, out string msg, bool replaceSql = true)
        {
            msg = "";
            if (dbconn == null)
            {
                return false;
            }
            if (replaceSql)
            {
                sql = Wildcard.Filter(sql.ReplaceSelectSql(),RoadFlow.Business.User.CurrentUser,null);
            }
            string str = dbconn.ConnType.ToLower();
            if (str != null)
            {
                bool flag;
                if (str != "sqlserver")
                {
                    if (str != "oracle")
                    {
                        if (!(str == "mysql"))
                        {
                            goto Label_0218;
                        }
                        using (MySqlConnection connection3 = new MySqlConnection(dbconn.ConnString))
                        {
                            try
                            {
                                connection3.Open();
                            }
                            catch (MySqlException exception5)
                            {
                                Log.Add("mysql异常报错", "", LogType.系统管理, exception5.ToString(), "", "", "", "", "", "", "");
                               // Log.Add(exception5);
                                return false;
                            }
                            MySqlCommand command3 = new MySqlCommand(sql, connection3);
                            try
                            {
                                command3.ExecuteNonQuery();
                                return true;
                            }
                            catch (MySqlException exception6)
                            {
                                msg = exception6.Message;
                                Log.Add("执行MySql语句发生了错误", exception6.Message + exception6.StackTrace, LogType.数据连接, sql, "", null);
                                return false;
                            }
                            finally
                            {
                                if (command3 != null)
                                {
                                    command3.Dispose();
                                }
                            }
                        }
                    }
                    using (OracleConnection connection2 = new OracleConnection(dbconn.ConnString))
                    {
                        try
                        {
                            connection2.Open();
                        }
                        catch (OracleException exception3)
                        {
                            Log.Add("Oracle异常报错", "", LogType.系统管理, exception3.ToString(), "", "", "", "", "", "", "");
                           // Log.Add(exception3);
                            return false;
                        }
                        OracleCommand command2 = new OracleCommand(sql, connection2);
                        try
                        {
                            command2.ExecuteNonQuery();
                            return true;
                        }
                        catch (OracleException exception4)
                        {
                            msg = exception4.Message;
                            Log.Add("执行Oracle语句发生了错误", exception4.Message + exception4.StackTrace, LogType.数据连接, sql, "", null);
                            return false;
                        }
                        finally
                        {
                            if (command2 != null)
                            {
                                command2.Dispose();
                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(dbconn.ConnString))
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (SqlException exception)
                    {
                        Log.Add("sqlserver异常报错", "", LogType.系统管理, exception.ToString(), "", "", "", "", "", "", "");
                      //  Log.Add(exception);
                        return false;
                    }
                    SqlCommand command = new SqlCommand(sql, connection);
                    try
                    {
                        command.ExecuteNonQuery();
                        flag = true;
                    }
                    catch (SqlException exception2)
                    {
                        msg = exception2.Message;
                        Log.Add("执行SqlServer语句发生了错误", exception2.Message + exception2.StackTrace, LogType.数据连接, sql, "", null);
                        flag = false;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }
                    }
                }
                return flag;
            }
        Label_0218:
            return false;



        }


      


      

   
}


 


  

}
