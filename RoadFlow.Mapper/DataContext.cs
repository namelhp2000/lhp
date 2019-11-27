using LogUtility;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using RoadFlow.Utility;
using RoadFlow.Utility.Logs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using LogUtility.Extensions;
using Npgsql;
using System.Data.SQLite;


namespace RoadFlow.Mapper
{

    #region  执行资料注释
    /* *************************执行语句的方法*********************************
     ----ExecuteNonQuery()：执行命令对象的SQL语句，返回一个int类型变量，如果SQL语句是对数据库的记录进行操作（如记录的增加、删除和更新），
     那么方法将返回操作所影响的记录条数。

     ----ExecuteScalar()：执行命令对象的SQL语句，如果SQL语句是SELECT查询，则仅仅返回查询结果集中的第1行第1列，而忽略其他的行 和列。
    该方法所返回的结果为object类型，在使用之前必须强制转换为所需的类型。如果SQL语句不是SELECT查询，则返回结果没有任何作用。

    ----ExecuteReader()：执行命令对象的SQL语句，在ADO.NET中，就是DataReader 对象的ExecuteReader()方法来进行数据的列出，
  并且我们用这个ExecuteReader()方法来显示数据是最快的一种方法，因为当我们在用ExecuteReader()方法中的DataReader 
  对象来进行数据的在网站建设中显示时，他只可以一条一条向前读，不能返回，也就是像ASP中的ADO方法中的Recordset 对象的Movenext一样，
  它没有move -1这样的返回方法。


     */
    #endregion

    /// <summary>
    /// 映射数据上下文
    /// </summary>
    public  partial class DataContext : IDisposable
    {

        #region    字段属性定义
        /// <summary>
        /// 影响的行
        /// </summary>
        private int InfluenceRows;
        /// <summary>
        /// 事务
        /// </summary>
        private DbTransaction Transaction;

        // Properties
        /// <summary>
        /// 数据连接
        /// </summary>
        public DbConnection Connection { get; private set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public DatabaseType DbType { get; set; }

        /// <summary>
        /// 是否事务
        /// </summary>
        private bool IsTransaction { get; set; }


        #endregion


        #region 日志设置方法

        /// <summary>
        /// 跟踪日志名称
        /// </summary>
        public const string TraceLogName = "SqlQueryLog";


        /// <summary>
        /// 单行参数写日志
        /// </summary>
        /// <param name="sql">Sql语句</param> 
        /// <param name="parameters">参数</param>
        /// <param name="debugSql">调试Sql语句</param>
        protected  void WriteTraceLog(string sql, string name, DbParameter[] parameters= null)
        {
            IDictionary<string, object> parametersDic = new Dictionary<string, object>();
        
            var log = GetLog();
            if (log.IsTraceEnabled == false)
                return;
           
            if (parameters==null)
            {
                log.Class(GetType().FullName)
                    .Caption(name)
                    .Sql("原始Sql:")
                    .Sql($"{sql}{StringExtensions.Line}")
                    .Trace();
            }
            else
            {
                foreach (var parasmeter in parameters)
                {
                    parametersDic[parasmeter.ParameterName] = parasmeter.Value;
                }
                log.Class(GetType().FullName)
                    .Caption(name)
                    .Sql("原始Sql:")
                    .Sql($"{sql}{StringExtensions.Line}")
                    .SqlParams(parametersDic)
                    .Trace();
            }
            
        }


        /// <summary>
        /// 多行参数写日志
        /// </summary>
        /// <param name="sql">Sql语句</param> 
        /// <param name="parameters">参数</param>
        /// <param name="debugSql">调试Sql语句</param>
        protected void WriteTraceLog(string sql, string name, List<DbParameter[]>  parameters = null)
        {
            IDictionary<string, object> parametersDic = new Dictionary<string, object>();

            var log = GetLog();
            if (log.IsTraceEnabled == false)
                return;

            if (parameters == null)
            {
                log.Class(GetType().FullName)
                    .Caption(name)
                    .Sql("原始Sql:")
                    .Sql($"{sql}{StringExtensions.Line}")
                    .Trace();
            }
            else
            {
                int i = 1;
                foreach (var parasmeter in parameters)
                {
                    foreach (var para in parasmeter)
                    {
                        parametersDic[i+"、"+para.ParameterName] = para.Value;
                    }
                    i++;   
                }
                log.Class(GetType().FullName)
                    .Caption(name)
                    .Sql("原始Sql:")
                    .Sql($"{sql}{StringExtensions.Line}")
                    .SqlParams(parametersDic)
                    .Trace();
            }

        }


        /// <summary>
        /// 获取日志操作
        /// </summary>
        private ILog GetLog()
        {
            try
            {
                return Log.GetLog(TraceLogName);
            }
            catch
            {
                return Log.Null;
            }
        }

        #endregion


        #region 构造函数

        /// <summary>
        /// 构造数据上下文    
        /// TODO当数据类型新增，需要设置的数据库连接类型  
        /// </summary>
        public DataContext()
        {
            this.InfluenceRows = 0;
            this.IsTransaction = true;
            switch (Config.DatabaseType)//使用小写
            {
                case "sqlserver":
                    this.DbType = DatabaseType.SqlServer;
                    this.ConnectionString = Config.ConnectionString_SqlServer;
                    break;

                case "mysql":
                    this.DbType = DatabaseType.MySql;
                    this.ConnectionString = Config.ConnectionString_MySql;
                    break;

                case "oracle":
                    this.DbType = DatabaseType.Oracle;
                    this.ConnectionString = Config.ConnectionString_Oracle;
                    break;
                case "postgresql":
                    this.DbType = DatabaseType.PostgreSql;
                    this.ConnectionString = Config.ConnectionString_PostgreSql;
                    break;
                case "sqlite":
                    this.DbType = DatabaseType.Sqlite;
                    this.ConnectionString = Config.ConnectionString_Sqlite;
                    break;
            }
            this.CreateConnection();
        }

        #endregion


        #region 针对数据库操作方法

        /// <summary>
        /// 构建数据上下文  
        /// TODO 构建数据库
        /// </summary>
        /// <param name="databaseType">数据类型</param>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="isTransaction">是否事务，默认是</param>
        public DataContext(string databaseType, string connectionString, bool isTransaction = true)
        {
            switch (databaseType.ToLower())
            {
                case "sqlserver":
                    this.DbType = DatabaseType.SqlServer;
                    break;

                case "mysql":
                    this.DbType = DatabaseType.MySql;
                    break;

                case "oracle":
                    this.DbType = DatabaseType.Oracle;
                    break;
                case "postgresql":
                    this.DbType = DatabaseType.PostgreSql;
                    break;
                case "sqlite":
                    this.DbType = DatabaseType.Sqlite;
                    break;
            }
            this.ConnectionString = connectionString;
            this.IsTransaction = isTransaction;
            this.CreateConnection();
        }

        /// <summary>
        /// 创建适配器
        /// TODO
        /// </summary>
        /// <returns></returns>
        public DbDataAdapter CreateAdapter()
        {
            switch (this.DbType)
            {
                case DatabaseType.SqlServer:
                    return new SqlDataAdapter();

                case DatabaseType.MySql:
                    return (DbDataAdapter)new MySqlDataAdapter();

                case DatabaseType.Oracle:
                    return new OracleDataAdapter();

                case DatabaseType.PostgreSql:
                    return new NpgsqlDataAdapter();
                case DatabaseType.Sqlite:
                    return new  SQLiteDataAdapter();
            }
            throw new Exception("不支持的数据库类型");
        }

        /// <summary>
        /// 创建连接 goto的妙用
        /// TODO
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateConnection()
        {
            if (this.Connection != null)//判断连接是否开启
            {
                if (this.Connection.State == ((ConnectionState)((int)ConnectionState.Closed)))
                {
                    this.Connection.Open();
                }
                if (this.Transaction == null)
                {
                    this.Transaction = this.Connection.BeginTransaction();
                }
                return this.Connection;
            }
            switch (this.DbType)//数据类型
            {
                case DatabaseType.SqlServer:
                    goto Label_SqlServer;
                case DatabaseType.MySql:
                    goto Label_MySql;

                case DatabaseType.Oracle:
                    goto Label_Oracle;
                case DatabaseType.PostgreSql:
                    goto Label_PostgreSql;
                case DatabaseType.Sqlite:
                    goto Label_Sqlite;

                default:
                    goto Label_Other;
            }


            Label_SqlServer: 
            this.Connection = new SqlConnection(this.ConnectionString);
            try
            {
                this.Connection.Open();
                this.Transaction = this.IsTransaction ? this.Connection.BeginTransaction() : null;
                return this.Connection;
            }
            catch (SqlException exception)
            {
                try
                {
                    this.Transaction.Dispose();
                    this.Connection.Dispose();
                }
                catch
                {
                }
                throw exception;
            }

            Label_MySql:
            this.Connection = (DbConnection)new MySqlConnection(this.ConnectionString);
            try
            {
                this.Connection.Open();
                this.Transaction = this.IsTransaction ? this.Connection.BeginTransaction() : null;
                return this.Connection;
            }
            catch (MySqlException exception2)
            {
                try
                {
                    this.Transaction.Dispose();
                    this.Connection.Dispose();
                }
                catch
                {
                }
                throw exception2;
            }


            Label_Oracle:// Oracle连接
            this.Connection = new OracleConnection(this.ConnectionString);
            try
            {
                this.Connection.Open();
                this.Transaction = this.IsTransaction ? this.Connection.BeginTransaction() : null;
                return this.Connection;
            }
            catch (OracleException exception3)
            {
                try
                {
                    this.Transaction.Dispose();
                    this.Connection.Dispose();
                }
                catch
                {
                }
                throw exception3;
            }


           Label_PostgreSql://PostgreSql连接
            this.Connection = new NpgsqlConnection(this.ConnectionString);
            try
            {
                this.Connection.Open();
                this.Transaction = this.IsTransaction ? this.Connection.BeginTransaction() : null;
                return this.Connection;
            }
            catch (OracleException exception4)
            {
                try
                {
                    this.Transaction.Dispose();
                    this.Connection.Dispose();
                }
                catch
                {
                }
                throw exception4;
            }
           Label_Sqlite://SQLite连接
            this.Connection = new SQLiteConnection(this.ConnectionString);
            try
            {
                this.Connection.Open();
                this.Transaction = this.IsTransaction ? this.Connection.BeginTransaction() : null;
                return this.Connection;
            }
            catch (OracleException exception5)
            {
                try
                {
                    this.Transaction.Dispose();
                    this.Connection.Dispose();
                }
                catch
                {
                }
                throw exception5;
            }
            Label_Other:
            throw new Exception("不支持的数据库类型");
        }


        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Dispose();
            }
            try
            {
                this.Connection.Close();
                this.Connection.Dispose();
            }
            catch
            {
            }
        }


        /// <summary>
        /// 释放上下文数据
        /// </summary>
        ~DataContext()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Dispose();
            }
            try
            {
                this.Connection.Close();
                this.Connection.Dispose();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 数据实例返回对应数据类型
        /// TODO
        /// </summary>
        private IData DataInstance
        {
            get
            {
                switch (this.DbType)
                {
                    case DatabaseType.SqlServer:
                        return new DataSqlServer();

                    case DatabaseType.MySql:
                        return new DataMySql();

                    case DatabaseType.Oracle:
                        return new DataOracle();
                    case DatabaseType.PostgreSql:
                        return new DataPostgreSql();
                    case DatabaseType.Sqlite:
                        return new DataSqlite();
                }
                throw new Exception("不支持的数据库类型");
            }
        }

        #endregion

        #region 测试方法

        /// <summary>
        /// 测试数据连接方法
        /// TODO 数据
        /// </summary>
        /// <returns></returns>
        public string TestConnection()
        {
            switch (this.DbType)
            {
                case DatabaseType.SqlServer:
                    {
                        SqlConnection connection = new SqlConnection(this.ConnectionString);
                        try
                        {
                            connection.Open();
                        }
                        catch (SqlException exception1)
                        {
                            return exception1.Message;
                        }
                        finally
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                        return "1";
                    }
                case DatabaseType.MySql:
                    {
                        MySqlConnection connection2 = new MySqlConnection(this.ConnectionString);
                        try
                        {
                            connection2.Open();
                        }
                        catch (MySqlException exception2)
                        {
                            return exception2.Message;
                        }
                        finally
                        {
                            connection2.Close();
                            connection2.Dispose();
                        }
                        return "1";
                    }
                case DatabaseType.Oracle:
                    {
                        OracleConnection connection3 = new OracleConnection(this.ConnectionString);
                        try
                        {
                            connection3.Open();
                        }
                        catch (OracleException exception3)
                        {
                            return exception3.Message;
                        }
                        finally
                        {
                            connection3.Close();
                            connection3.Dispose();
                        }
                        return "1";
                    }
                case DatabaseType.PostgreSql:
                    {
                        NpgsqlConnection connection4 = new NpgsqlConnection(this.ConnectionString);
                        try
                        {
                            connection4.Open();
                        }
                        catch (OracleException exception4)
                        {
                            return exception4.Message;
                        }
                        finally
                        {
                            connection4.Close();
                            connection4.Dispose();
                        }
                        return "1";
                    }
                case DatabaseType.Sqlite:
                    {
                        SQLiteConnection connection5 = new SQLiteConnection(this.ConnectionString);
                        try
                        {
                            connection5.Open();
                        }
                        catch (OracleException exception5)
                        {
                            return exception5.Message;
                        }
                        finally
                        {
                            connection5.Close();
                            connection5.Dispose();
                        }
                        return "1";
                    }
            }
            return "不支持的数据库类型";
        }

        /// <summary>
        /// 测试Sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string TestSQL(string sql, DbParameter[] parameters = null)
        {
            string message;
            if (!this.IsTransaction)
            {
                return "事务未开启!";
            }
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = this.Transaction;
                if ((parameters != null) && (parameters.Length != 0))
                {
                    command.Parameters.AddRange(parameters);
                }
                try
                {
                    message = ((int)command.ExecuteNonQuery()).ToString();
                }
                catch (SqlException exception1)
                {
                    message = exception1.Message;
                }
                finally
                {
                    //日志添加方法
                    WriteTraceLog(sql, "测试Sql语句", parameters);

                    command.Parameters.Clear();
                    this.Transaction.Rollback();
                }
            }
            return message;
        }


        #endregion


        #region  执行存储方法  目前只支持SqlServer

        /// <summary>
        /// 执行存储过程，针对 执行语句增删改
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="outParamsDic">输出参数</param>
        /// <param name="inParamslist">输入参数</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="IsTransaction">事务连接对象</param>
        /// <returns></returns>
        public int RunProcedureNoQuery(out string errorMsg, out Dictionary<string, object> outParamsDic, List<ValueTuple<string, object, ParameterDirection, DbType?>> inParamslist, string procedureName, bool IsTransaction = true)
        {
            errorMsg = string.Empty;
            Dictionary<DbParameter, string> parameterDic = new Dictionary<DbParameter, string>();
            outParamsDic = new Dictionary<string, object>();
            try
            {
                using (DbCommand command = this.Connection.CreateCommand())
                {
                    if (this.IsTransaction)
                    {
                        command.Transaction = this.Transaction;
                    }
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    if (inParamslist != null && inParamslist.Count > 0)
                    {
                        foreach (var key in inParamslist)
                        {
                            if (key.Item3 == ParameterDirection.ReturnValue || key.Item3 == ParameterDirection.Output)
                            {
                                DbParameter db = this.CreateDbParameter(key.Item1, key.Item4);                       
                                db.Direction = key.Item3;
                                command.Parameters.Add(db);
                                parameterDic[db] = key.Item1;
                            }
                            else if (key.Item3 == ParameterDirection.InputOutput)
                            {
                                DbParameter db = this.CreateDbParameter(key.Item1, key.Item4);
                                db.Value = key.Item2;
                                db.Direction = key.Item3;
                                command.Parameters.Add(db);
                                parameterDic[db] = key.Item1;
                            }
                            else
                            {
                                DbParameter db =  this.CreateDbParameter(key.Item1, key.Item4);
                                db.Value = key.Item2;
                                db.Direction = key.Item3;
                                command.Parameters.Add(db);
                            }

                        }

                    }

                    int influenceNum = command.ExecuteNonQuery();

                    if (parameterDic != null)
                    {
                        foreach (var parameter in parameterDic)
                        {
                            outParamsDic.Add(parameter.Value, parameter.Key.Value.ToString());
                        }
                    }
                    return influenceNum;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return 0;
        }

        /// <summary>
        /// 执行存储过程，针对查询列表
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="outParamsDic">输出参数</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="inParamslist">输入参数 eg:new{Age=30}</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns>返回查询记录</returns>
        public DataTable RunProcedure(out string errorMsg, out Dictionary<string, object> outParamsDic, List<ValueTuple<string, object, ParameterDirection, DbType?>> inParamslist, string procedureName, bool IsTransaction = true)
        {
            outParamsDic = new Dictionary<string, object>();
            errorMsg = string.Empty;
            Dictionary<DbParameter, string> parameterDics = new Dictionary<DbParameter, string>();
            DataTable table = new DataTable();
            try
            {
                using (DbCommand command = this.Connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procedureName;
                    if (this.IsTransaction)
                    {
                        command.Transaction = this.Transaction;
                    }
                   
                    if (inParamslist != null && inParamslist.Count > 0)
                    {
                        foreach (var key in inParamslist)
                        {

                            if (key.Item3 == ParameterDirection.ReturnValue || key.Item3 == ParameterDirection.Output)
                            {
                                DbParameter db = this.CreateDbParameter(key.Item1, key.Item4);
                                db.Direction = key.Item3;
                                command.Parameters.Add(db);
                                parameterDics[db] = key.Item1;
                            }
                            else if (key.Item3 == ParameterDirection.InputOutput)
                            {
                                DbParameter db = this.CreateDbParameter(key.Item1, key.Item4);
                                db.Value = key.Item2;
                                db.Direction = key.Item3;
                                command.Parameters.Add(db);
                                parameterDics[db] = key.Item1;
                            }
                            else
                            {
                                DbParameter db = this.CreateDbParameter(key.Item1, key.Item4);
                                db.Value = key.Item2;
                                db.Direction = key.Item3;
                                command.Parameters.Add(db);
                            }

                        }

                    }
                    using (DbDataAdapter adapter = this.CreateAdapter())
                    {
                        adapter.SelectCommand = command;
                     
                        adapter.Fill(table);
                        if (parameterDics != null)
                        {
                            foreach (var parameter in parameterDics)
                            {
                                outParamsDic.Add(parameter.Value, parameter.Key.Value.ToString());
                            }
                        }
                        command.Parameters.Clear();
                        return table;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return new DataTable();
        }


        /// <summary>
        /// 创建参数 TODO
        /// </summary>
        /// <returns></returns>
        public DbParameter CreateDbParameter(string name,object value)
        {
            switch (this.DbType)
            {
                case DatabaseType.SqlServer:
                    return   (DbParameter)new  SqlParameter("@" + name, value);

                case DatabaseType.MySql:
                    return (DbParameter)new MySqlParameter("@" + name, value);
                case DatabaseType.Sqlite:
                    return (DbParameter)new SQLiteParameter("@" + name, value);
                case DatabaseType.PostgreSql:
                    return (DbParameter)new NpgsqlParameter("@" + name, value);

                case DatabaseType.Oracle:
                    return new OracleParameter(":" + name, Common.GetParameterValue(value));
            }
            throw new Exception("不支持的数据库类型");
        }

        #endregion


        #region 判断数据库类型属性 TODO
        /// <summary>
        /// 是否MySql数据类型
        /// </summary>
        public bool IsMySql
        {
            get
            {
                return (this.DbType == DatabaseType.MySql);
            }
        }

        /// <summary>
        /// 是否Oracle数据类型
        /// </summary>
        public bool IsOracle
        {
            get
            {
                return (this.DbType == DatabaseType.Oracle);
            }
        }

        /// <summary>
        /// 是否SqlServer数据类型
        /// </summary>
        public bool IsSqlServer
        {
            get
            {
                return (this.DbType == DatabaseType.SqlServer);
            }
        }


        /// <summary>
        /// 是否PostgreSql数据类型
        /// </summary>
        public bool IsPostgreSql
        {
            get
            {
                return (this.DbType == DatabaseType.PostgreSql);
            }
        }


        /// <summary>
        /// 是否Sqlite数据类型
        /// </summary>
        public bool IsSqlite
        {
            get
            {
                return (this.DbType == DatabaseType.Sqlite);
            }
        }


        #endregion

    }

}
