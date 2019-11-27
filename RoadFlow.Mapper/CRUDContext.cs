using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RoadFlow.Mapper
{
    public partial class DataContext : IDisposable
    {
        #region ***************************************增删改查 同步与异步设置*******************************************

        #region  新增数据  新增实体字段顺序必须与数据库字段顺序保持一致。

        /// <summary>
        /// 映射添加
        /// </summary>
        /// <typeparam name="T">实体数据模块</typeparam>
        /// <param name="t"></param>
        /// <returns>影响行数</returns>
        public int Add<T>(T t) where T : class, new()
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {
                ValueTuple<string, DbParameter[]> insertSql = this.DataInstance.GetInsertSql<T>(t);
                string sqlstr = insertSql.Item1;
                DbParameter[] parameterArray = insertSql.Item2;
                command.CommandText = sqlstr;
                if (this.IsTransaction)//是否事务
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0) //参数数组
                {
                    command.Parameters.AddRange(parameterArray);
                }
                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "添加数据", parameterArray);
                int num = command.ExecuteNonQuery();
                command.Parameters.Clear();
                this.InfluenceRows += num;
                return num;
            }
        }


        /// <summary>
        /// 映射添加Int  
        /// </summary>
        /// <typeparam name="T">实体数据模块</typeparam>
        /// <param name="t"></param>
        /// <returns>影响行数</returns>
        public int AddInt<T>(T t) where T : class, new()
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {
                ValueTuple<string, DbParameter[]> insertSql = this.DataInstance.GetInsertIntSql<T>(t, this.ConnectionString);
                string sqlstr = insertSql.Item1;
                DbParameter[] parameterArray = insertSql.Item2;
                command.CommandText = sqlstr;
                if (this.IsTransaction)//是否事务
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0) //参数数组
                {
                    command.Parameters.AddRange(parameterArray);
                }

                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "Int添加数据", parameterArray);

                int num = command.ExecuteNonQuery();
                command.Parameters.Clear();
                this.InfluenceRows += num;
                return num;
            }
        }






        /// <summary>
        /// 批量添加  SqlServer特殊批量添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public int AddRange<T>(IEnumerable<T> ts) where T : class, new()
        {
            int InfluenceNum = Enumerable.Count<T>(ts);
            if ((InfluenceNum > 10) && (this.DbType == DatabaseType.SqlServer))
            {
                this.DataInstance.BulkCopy<T>(ts, this.Connection, this.Transaction);
                this.InfluenceRows += InfluenceNum;
                return InfluenceNum;
            }
            int InfluenceNum1 = 0;
            ValueTuple<string, List<DbParameter[]>> insertSql = this.DataInstance.GetInsertSql<T>(ts);
            string sqlstr = insertSql.Item1;
            List<DbParameter[]> list = insertSql.Item2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                command.CommandText = sqlstr;
                command.Prepare();

                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "批量添加数据", list);
                foreach (DbParameter[] parameterArray in list)
                {
                    if (parameterArray.Length != 0)
                    {
                        command.Parameters.AddRange(parameterArray);
                    }
                    InfluenceNum1 += command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
                this.InfluenceRows += InfluenceNum1;
                return InfluenceNum1;
            }
        }


        #region 异步添加数据

        /// <summary>
        /// 异步映射添加
        /// </summary>
        /// <typeparam name="T">实体数据模块</typeparam>
        /// <param name="t"></param>
        /// <returns>影响行数</returns>
        public async Task<int> AddAsync<T>(T t) where T : class, new()
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {
                ValueTuple<string, DbParameter[]> insertSql = this.DataInstance.GetInsertSql<T>(t);
                string sqlstr = insertSql.Item1;
                DbParameter[] parameterArray = insertSql.Item2;
                command.CommandTimeout = 36000;
                command.CommandText = sqlstr;
                if (this.IsTransaction)//是否事务
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0) //参数数组
                {
                    command.Parameters.AddRange(parameterArray);
                }
                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "添加异步数据", parameterArray);
                int num = await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();
                this.InfluenceRows += num;
                return num;
            }
        }


        /// <summary>
        /// 异步映射添加Int  
        /// </summary>
        /// <typeparam name="T">实体数据模块</typeparam>
        /// <param name="t"></param>
        /// <returns>影响行数</returns>
        public async Task<int> AddIntAsync<T>(T t) where T : class, new()
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {
                ValueTuple<string, DbParameter[]> insertSql = this.DataInstance.GetInsertIntSql<T>(t, this.ConnectionString);
                string sqlstr = insertSql.Item1;
                DbParameter[] parameterArray = insertSql.Item2;
                command.CommandTimeout = 36000;
                command.CommandText = sqlstr;
                if (this.IsTransaction)//是否事务
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0) //参数数组
                {
                    command.Parameters.AddRange(parameterArray);
                }

                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "Int添加数据", parameterArray);

                int num = await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();
                this.InfluenceRows += num;
                return num;
            }
        }

        #endregion







        #endregion


        #region 执行Sql新增删除更新语句

        /// <summary>
        /// 映射执行语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>影响行数</returns>
        public int Execute(string sql, DbParameter[] parameters = null)
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {

                command.CommandText = sql;
                // command.CommandTimeout = 500;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if ((parameters != null) && (parameters.Length != 0))
                {
                    command.Parameters.AddRange(parameters);
                }

                //日志添加方法
                WriteTraceLog(sql, "Excute执行语句", parameters);

                int num = command.ExecuteNonQuery();
                command.Parameters.Clear();
                this.InfluenceRows += num;
                return num;
            }
        }




        /// <summary>
        /// 映射执行语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>影响行数</returns>
        public int Execute(string sql, params object[] objects)
        {
            ValueTuple<string, DbParameter[]> sqlAndParameter = this.DataInstance.GetSqlAndParameter(sql, objects);
            string sqlstr = sqlAndParameter.Item1;
            DbParameter[] parameterArray = sqlAndParameter.Item2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = sqlstr;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0)
                {
                    command.Parameters.AddRange(parameterArray);
                }

                //日志添加方法
                WriteTraceLog(sqlstr, "Excute执行语句", parameterArray);

                int num = command.ExecuteNonQuery();
                command.Parameters.Clear();
                this.InfluenceRows += num;
                return num;
            }
        }


        #region 异步执行语句
        /// <summary>
        /// 异步映射执行语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>影响行数</returns>
        public async Task<int> ExecuteAsync(string sql, DbParameter[] parameters = null)
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandTimeout = 36000;
                command.CommandText = sql;
                // command.CommandTimeout = 500;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if ((parameters != null) && (parameters.Length != 0))
                {
                    command.Parameters.AddRange(parameters);
                }

                //日志添加方法
                WriteTraceLog(sql, "Excute执行语句", parameters);

                int num = await command.ExecuteNonQueryAsync();

                command.Parameters.Clear();
                this.InfluenceRows += num;
                return num;
            }
        }




        /// <summary>
        /// 异步映射执行语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>影响行数</returns>
        public async Task<int> ExecuteAsync(string sql, params object[] objects)
        {
            ValueTuple<string, DbParameter[]> sqlAndParameter = this.DataInstance.GetSqlAndParameter(sql, objects);
            string sqlstr = sqlAndParameter.Item1;
            DbParameter[] parameterArray = sqlAndParameter.Item2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandTimeout = 36000;
                command.CommandText = sqlstr;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0)
                {
                    command.Parameters.AddRange(parameterArray);
                }

                //日志添加方法
                WriteTraceLog(sqlstr, "Excute执行语句", parameterArray);

                int num = await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();
                this.InfluenceRows += num;
                return num;
            }
        }

        #endregion

        #endregion


        #region  ExecuteScalar运行查询，并返回查询所返回的结果集中第一行的第一列


        /// <summary>
        /// 执行标量语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>返回对象第一行</returns>
        public object ExecuteScalar(string sql, DbParameter[] parameters = null)
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = sql;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if ((parameters != null) && (parameters.Length != 0))
                {
                    command.Parameters.AddRange(parameters);
                }

                //日志添加方法
                WriteTraceLog(sql, "ExecuteScalar执行语句", parameters);


                object b = command.ExecuteScalar();

                command.Parameters.Clear();
                return b;
                //command.Parameters.Clear();
                //return command.ExecuteScalar();
            }
        }

        /// <summary>
        /// 执行标量语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>对象</returns>
        public object ExecuteScalar(string sql, params object[] objects)
        {
            ValueTuple<string, DbParameter[]> sqlAndParameter = this.DataInstance.GetSqlAndParameter(sql, objects);
            string sqlstr = sqlAndParameter.Item1;
            DbParameter[] parameters = sqlAndParameter.Item2;
            return this.ExecuteScalar(sqlstr, parameters);
        }

        /// <summary>
        /// 执行标量字符串
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数组</param>
        /// <returns>字符串</returns>
        public string ExecuteScalarString(string sql, DbParameter[] parameters = null)
        {
            object obj2 = this.ExecuteScalar(sql, parameters);
            if (obj2 != null)
            {
                return obj2.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        ///  执行标量字符串
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>字符串</returns>
        public string ExecuteScalarString(string sql, params object[] objects)
        {
            ValueTuple<string, DbParameter[]> sqlAndParameter = this.DataInstance.GetSqlAndParameter(sql, objects);
            string sqlstr = sqlAndParameter.Item1;
            DbParameter[] parameters = sqlAndParameter.Item2;
            return this.ExecuteScalarString(sqlstr, parameters);
        }




        /// <summary>
        /// 获得身份
        /// </summary>
        /// <param name="seqName"></param>
        /// <returns></returns>
        public int GetIdentity(string seqName = "")
        {
            return this.ExecuteScalarString(this.DataInstance.GetIdentitySql(seqName), (DbParameter[])null).ToInt(-1);
        }



        #region 异步ExecuteScalar运行

        /// <summary>
        /// 异步执行标量语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>返回对象第一行</returns>
        public async Task<object> ExecuteScalarAsync(string sql, DbParameter[] parameters = null)
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandTimeout = 36000;
                command.CommandText = sql;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if ((parameters != null) && (parameters.Length != 0))
                {
                    command.Parameters.AddRange(parameters);
                }

                //日志添加方法
                WriteTraceLog(sql, "ExecuteScalar执行语句", parameters);

                object b = await command.ExecuteScalarAsync();

                command.Parameters.Clear();
                return b;
            }
        }



        /// <summary>
        /// 异步执行标量语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>对象</returns>
        public async Task<object> ExecuteScalarAsync(string sql, params object[] objects)
        {
            ValueTuple<string, DbParameter[]> sqlAndParameter = this.DataInstance.GetSqlAndParameter(sql, objects);
            string sqlstr = sqlAndParameter.Item1;
            DbParameter[] parameters = sqlAndParameter.Item2;
            return await this.ExecuteScalarAsync(sqlstr, parameters);
        }

        /// <summary>
        /// 异步执行标量字符串
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数组</param>
        /// <returns>字符串</returns>
        public async Task<string> ExecuteScalarStringAsync(string sql, DbParameter[] parameters = null)
        {
            object obj2 = await this.ExecuteScalarAsync(sql, parameters);
            if (obj2 != null)
            {
                return obj2.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        ///  异步执行标量字符串
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>字符串</returns>
        public async Task<string> ExecuteScalarStringAsync(string sql, params object[] objects)
        {
            ValueTuple<string, DbParameter[]> sqlAndParameter = this.DataInstance.GetSqlAndParameter(sql, objects);
            string sqlstr = sqlAndParameter.Item1;
            DbParameter[] parameters = sqlAndParameter.Item2;
            return await this.ExecuteScalarStringAsync(sqlstr, parameters);
        }



        #endregion





        #endregion


        #region  查询返回DataTable表

        /// <summary>
        /// 通过参数SQL查询获取数据表
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>返回数据表</returns>
        public DataTable GetDataTable(string sql, DbParameter[] parameters = null)
        {
            DataTable table2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = sql;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if ((parameters != null) && (parameters.Length != 0))
                {
                    command.Parameters.AddRange(parameters);
                }

                //日志添加方法
                WriteTraceLog(sql, "GetDataTable方法查询数据", parameters);

                using (DbDataAdapter adapter = this.CreateAdapter())//通过适配器封装表
                {
                    adapter.SelectCommand = command;
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    command.Parameters.Clear();
                    table2 = table;
                }
            }
            return table2;
        }

        /// <summary>
        /// 通过对象SQL语句获取数据表
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(string sql, params object[] objects)
        {
            DataTable table2;
            ValueTuple<string, DbParameter[]> sqlAndParameter = this.DataInstance.GetSqlAndParameter(sql, objects);
            string sqlstr = sqlAndParameter.Item1;
            DbParameter[] parameterArray = sqlAndParameter.Item2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = sqlstr;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0)
                {
                    command.Parameters.AddRange(parameterArray);
                }


                //日志添加方法
                WriteTraceLog(sqlstr, "GetDataTable方法查询数据", parameterArray);

                using (DbDataAdapter adapter = this.CreateAdapter())
                {
                    adapter.SelectCommand = command;
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    command.Parameters.Clear();
                    table2 = table;
                }
            }
            return table2;
        }


        /// <summary>
        /// 获取数据表结构
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableSchema(string sql, DbParameter[] parameters = null)
        {
            DataTable table2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = sql;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if ((parameters != null) && (parameters.Length != 0))
                {
                    command.Parameters.AddRange(parameters);
                }

                //日志添加方法
                WriteTraceLog(sql, "SQL获取数据表结构", parameters);


                using (DbDataAdapter adapter = this.CreateAdapter())
                {
                    adapter.SelectCommand = command;
                    DataTable table = new DataTable();
                    SaveChanges();
                    //填写详细的元数据信息
                    adapter.FillSchema(table, (SchemaType)SchemaType.Mapped);
                    command.Parameters.Clear();
                    table2 = table;
                }
            }
            return table2;
        }


        #region 异步查询返回DataTable表

        /// <summary>
        /// 通过参数SQL查询获取数据表
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>返回数据表</returns>
        public async Task<DataTable> GetDataTableAsync(string sql, DbParameter[] parameters = null)
        {
            DataTable table2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandTimeout = 36000;
                command.CommandText = sql;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if ((parameters != null) && (parameters.Length != 0))
                {
                    command.Parameters.AddRange(parameters);
                }

                //日志添加方法
                WriteTraceLog(sql, "GetDataTableAsync方法查询数据", parameters);

                using (var dr = await command.ExecuteReaderAsync())//通过适配器封装表
                {
                    table2 = new DataTable();
                    await Task.Run(() =>
                    {
                        table2.Load(dr);
                    });
                }
            }
            return table2;
        }

        /// <summary>
        /// 通过对象SQL语句获取数据表
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>数据表</returns>
        public async Task<DataTable> GetDataTableAsync(string sql, params object[] objects)
        {
            DataTable table2;
            ValueTuple<string, DbParameter[]> sqlAndParameter = this.DataInstance.GetSqlAndParameter(sql, objects);
            string sqlstr = sqlAndParameter.Item1;
            DbParameter[] parameterArray = sqlAndParameter.Item2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandTimeout = 36000;
                command.CommandText = sqlstr;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0)
                {
                    command.Parameters.AddRange(parameterArray);
                }


                //日志添加方法
                WriteTraceLog(sqlstr, "GetDataTableAsync方法查询数据", parameterArray);

                using (var dr = await command.ExecuteReaderAsync())//通过适配器封装表
                {
                    table2 = new DataTable();
                    //异步读取数据方式
                    await Task.Run(() =>
                    {
                        table2.Load(dr);
                    });
                }
            }
            return table2;
        }

        #endregion


        #endregion


        #region  查询方法

        /// <summary>
        /// 映射查找单条数据返回第一条方法 只是针对主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <returns></returns>
        public T Find<T>(params object[] objects) where T : class, new()
        {
            ValueTuple<string, DbParameter[]> findSql = this.DataInstance.GetFindSql<T>(objects);
            string sqlstr = findSql.Item1;
            DbParameter[] parameters = findSql.Item2;
            if (((parameters != null) && (parameters.Length != 0)) && !string.IsNullOrWhiteSpace(sqlstr))
            {
                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "查找方法", parameters);

                //查询一条记录
                return this.QueryOne<T>(sqlstr, parameters);
            }
            return default(T);
        }


        /// <summary>
        /// 获取查询列表
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>列表数据</returns>
        public List<T> Query<T>(string sql, DbParameter[] parameters = null) where T : class, new()
        {
            new List<T>();
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = sql;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if ((parameters != null) && (parameters.Length != 0))
                {
                    command.Parameters.AddRange(parameters);
                }

                //日志添加方法
                WriteTraceLog(sql, typeof(T).Name + "查询列表", parameters);

                DbDataReader reader = command.ExecuteReader();
                command.Parameters.Clear();
                //读取列表的方式
                return this.DataInstance.ReaderToList<T>(reader);
            }
        }

        /// <summary>
        /// 通过对象数组获取查询列表
        /// </summary>
        /// <typeparam name="T">实体数据</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>列表</returns>
        public List<T> Query<T>(string sql, params object[] objects) where T : class, new()
        {
            new List<T>();
            ValueTuple<string, DbParameter[]> sqlAndParameter = this.DataInstance.GetSqlAndParameter(sql, objects);
            string sqlstr = sqlAndParameter.Item1;
            DbParameter[] parameterArray = sqlAndParameter.Item2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = sqlstr;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0)
                {
                    command.Parameters.AddRange(parameterArray);
                }

                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "对象数组获取查询列表", parameterArray);

                DbDataReader reader = command.ExecuteReader();
                command.Parameters.Clear();
                return this.DataInstance.ReaderToList<T>(reader);
            }
        }


        /// <summary>
        /// 查询表所有的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>列表</returns>
        public List<T> QueryAll<T>() where T : class, new()
        {
            string tableName = Common.GetTableName(typeof(T));
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return new List<T>();
            }
            return this.Query<T>("SELECT * FROM " + tableName, (DbParameter[])null);
        }


        /// <summary>
        /// 通过参数数组查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>数据单条记录</returns>
        public T QueryOne<T>(string sql, DbParameter[] parameters) where T : class, new()
        {
            List<T> list = this.Query<T>(sql, parameters);
            if (!Enumerable.Any<T>((IEnumerable<T>)list))
            {
                return default(T);
            }
            return Enumerable.First<T>((IEnumerable<T>)list);//获取第一条记录
        }


        /// <summary>
        /// 通过对象数组查询单条数据
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>实体单条数据</returns>
        public T QueryOne<T>(string sql, params object[] objects) where T : class, new()
        {
            List<T> list = this.Query<T>(this.DataInstance.GetQueryOneSql(sql), objects);
            if (!Enumerable.Any<T>((IEnumerable<T>)list))
            {
                return default(T);
            }
            return Enumerable.First<T>((IEnumerable<T>)list);//多条返回第一条
        }


        #region  **********************异步查询*******************************

        /// <summary>
        /// 异步映射查找单条数据返回第一条方法 只是针对主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <returns></returns>
        public async Task<T> FindAsync<T>(params object[] objects) where T : class, new()
        {
            ValueTuple<string, DbParameter[]> findSql = this.DataInstance.GetFindSql<T>(objects);
            string sqlstr = findSql.Item1;
            DbParameter[] parameters = findSql.Item2;
            if (((parameters != null) && (parameters.Length != 0)) && !string.IsNullOrWhiteSpace(sqlstr))
            {
                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "查找方法", parameters);

                //查询一条记录
                return await this.QueryOneAsync<T>(sqlstr, parameters);
            }
            return default(T);
        }


        /// <summary>
        /// 异步获取查询列表
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>列表数据</returns>
        public async Task<List<T>> QueryAsync<T>(string sql, DbParameter[] parameters = null) where T : class, new()
        {
            new List<T>();
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandTimeout = 36000;
                command.CommandText = sql;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if ((parameters != null) && (parameters.Length != 0))
                {
                    command.Parameters.AddRange(parameters);
                }

                //日志添加方法
                WriteTraceLog(sql, typeof(T).Name + "查询列表", parameters);

                DbDataReader reader = await command.ExecuteReaderAsync();
                command.Parameters.Clear();
                //读取列表的方式
                return this.DataInstance.ReaderToList<T>(reader);
            }
        }

        /// <summary>
        /// 异步通过对象数组获取查询列表
        /// </summary>
        /// <typeparam name="T">实体数据</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>列表</returns>
        public async Task<List<T>> QueryAsync<T>(string sql, params object[] objects) where T : class, new()
        {
            new List<T>();
            ValueTuple<string, DbParameter[]> sqlAndParameter = this.DataInstance.GetSqlAndParameter(sql, objects);
            string sqlstr = sqlAndParameter.Item1;
            DbParameter[] parameterArray = sqlAndParameter.Item2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                command.CommandTimeout = 36000;
                command.CommandText = sqlstr;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0)
                {
                    command.Parameters.AddRange(parameterArray);
                }

                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "对象数组获取查询列表", parameterArray);

                DbDataReader reader = await command.ExecuteReaderAsync();
                command.Parameters.Clear();
                return this.DataInstance.ReaderToList<T>(reader);
            }
        }


        /// <summary>
        /// 异步查询表所有的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>列表</returns>
        public async Task<List<T>> QueryAllAsync<T>() where T : class, new()
        {
            string tableName = Common.GetTableName(typeof(T));
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return new List<T>();
            }
            return await this.QueryAsync<T>("SELECT * FROM " + tableName, (DbParameter[])null);
        }


        /// <summary>
        /// 异步通过参数数组查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>数据单条记录</returns>
        public async Task<T> QueryOneAsync<T>(string sql, DbParameter[] parameters) where T : class, new()
        {
            List<T> list = await this.QueryAsync<T>(sql, parameters);
            if (!Enumerable.Any<T>((IEnumerable<T>)list))
            {
                return default(T);
            }
            return Enumerable.First<T>((IEnumerable<T>)list);//获取第一条记录
        }


        /// <summary>
        /// 异步通过对象数组查询单条数据
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="objects">对象数组</param>
        /// <returns>实体单条数据</returns>
        public async Task<T> QueryOneAsync<T>(string sql, params object[] objects) where T : class, new()
        {
            List<T> list = await this.QueryAsync<T>(this.DataInstance.GetQueryOneSql(sql), objects);
            if (!Enumerable.Any<T>((IEnumerable<T>)list))
            {
                return default(T);
            }
            return Enumerable.First<T>((IEnumerable<T>)list);//多条返回第一条
        }


        #endregion

        #endregion


        #region  删除数据
        /// <summary>
        /// 执行移除单条数据
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="t"></param>
        /// <returns>影响条数</returns>
        public int Remove<T>(T t) where T : class, new()
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {
                ValueTuple<string, DbParameter[]> removeSql = this.DataInstance.GetRemoveSql<T>(t);
                string sqlstr = removeSql.Item1;
                DbParameter[] parameterArray = removeSql.Item2;
                command.CommandText = sqlstr;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0)
                {
                    command.Parameters.AddRange(parameterArray);
                }


                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "删除单条数据", parameterArray);


                int InfluenceNum = command.ExecuteNonQuery();
                command.Parameters.Clear();
                this.InfluenceRows += InfluenceNum;
                return InfluenceNum;
            }
        }

        /// <summary>
        /// 移除多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public int RemoveRange<T>(IEnumerable<T> ts) where T : class, new()
        {
            int InfluenceNum = 0;
            ValueTuple<string, List<DbParameter[]>> removeSql = this.DataInstance.GetRemoveSql<T>(ts);
            string sqlstr = removeSql.Item1;
            List<DbParameter[]> list = removeSql.Item2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                command.CommandText = sqlstr;
                command.Prepare();
                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "批量删除多条数据", list);
                foreach (DbParameter[] parameterArray in list)
                {
                    if (parameterArray.Length != 0)
                    {
                        command.Parameters.AddRange(parameterArray);
                    }

                    InfluenceNum += command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
            this.InfluenceRows += InfluenceNum;
            return InfluenceNum;
        }


        #region *************************异步删除*********************************


        /// <summary>
        /// 异步执行移除单条数据
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="t"></param>
        /// <returns>影响条数</returns>
        public async Task<int> RemoveAsync<T>(T t) where T : class, new()
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {
                ValueTuple<string, DbParameter[]> removeSql = this.DataInstance.GetRemoveSql<T>(t);
                string sqlstr = removeSql.Item1;
                DbParameter[] parameterArray = removeSql.Item2;
                command.CommandTimeout = 36000;
                command.CommandText = sqlstr;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0)
                {
                    command.Parameters.AddRange(parameterArray);
                }


                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "删除单条数据", parameterArray);
                int InfluenceNum = await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();
                this.InfluenceRows += InfluenceNum;
                return InfluenceNum;
            }
        }

        /// <summary>
        /// 异步移除多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public async Task<int> RemoveRangeAsync<T>(IEnumerable<T> ts) where T : class, new()
        {
            int InfluenceNum = 0;
            ValueTuple<string, List<DbParameter[]>> removeSql = this.DataInstance.GetRemoveSql<T>(ts);
            string sqlstr = removeSql.Item1;
            List<DbParameter[]> list = removeSql.Item2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                command.CommandTimeout = 36000;
                command.CommandText = sqlstr;
                command.Prepare();
                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "批量删除多条数据", list);
                foreach (DbParameter[] parameterArray in list)
                {
                    if (parameterArray.Length != 0)
                    {
                        command.Parameters.AddRange(parameterArray);
                    }
                    InfluenceNum += await command.ExecuteNonQueryAsync();
                    command.Parameters.Clear();
                }
            }
            this.InfluenceRows += InfluenceNum;
            return InfluenceNum;
        }

        #endregion


        #endregion



        #region  新增修改删除与执行语句最终需要使用保存更改

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            int influenceRows = this.InfluenceRows;
            this.InfluenceRows = 0;
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
                return influenceRows;
            }
            return 0;
        }


        #endregion


        #region  更新数据
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public int Update<T>(T t) where T : class, new()
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {
                ValueTuple<string, DbParameter[]> updateSql = this.DataInstance.GetUpdateSql<T>(t);
                string sqlstr = updateSql.Item1;
                DbParameter[] parameterArray = updateSql.Item2;
                command.CommandText = sqlstr;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0)
                {
                    command.Parameters.AddRange(parameterArray);
                }
                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "更新数据", parameterArray);
                int InfluenceNum = command.ExecuteNonQuery();
                command.Parameters.Clear();
                this.InfluenceRows += InfluenceNum;
                return InfluenceNum;
            }
        }


        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public int UpdateRange<T>(IEnumerable<T> ts) where T : class, new()
        {
            int InfluenceNum = 0;
            ValueTuple<string, List<DbParameter[]>> updateSql = this.DataInstance.GetUpdateSql<T>(ts);
            string sqlstr = updateSql.Item1;
            List<DbParameter[]> list = updateSql.Item2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                command.CommandText = sqlstr;
                command.Prepare();
                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "批量更新多条数据", list);

                foreach (DbParameter[] parameterArray in list)
                {
                    if (parameterArray.Length != 0)
                    {
                        command.Parameters.AddRange(parameterArray);
                    }
                    InfluenceNum += command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
            this.InfluenceRows += InfluenceNum;
            return InfluenceNum;
        }



        #region *******************************异步更新******************************
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync<T>(T t) where T : class, new()
        {
            using (DbCommand command = this.Connection.CreateCommand())
            {
                ValueTuple<string, DbParameter[]> updateSql = this.DataInstance.GetUpdateSql<T>(t);
                string sqlstr = updateSql.Item1;
                DbParameter[] parameterArray = updateSql.Item2;
                command.CommandTimeout = 36000;
                command.CommandText = sqlstr;
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                if (parameterArray.Length != 0)
                {
                    command.Parameters.AddRange(parameterArray);
                }

                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "更新数据", parameterArray);


                int InfluenceNum = await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();
                this.InfluenceRows += InfluenceNum;
                return InfluenceNum;
            }
        }


        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public async Task<int> UpdateRangeAsync<T>(IEnumerable<T> ts) where T : class, new()
        {
            int InfluenceNum = 0;
            ValueTuple<string, List<DbParameter[]>> updateSql = this.DataInstance.GetUpdateSql<T>(ts);
            string sqlstr = updateSql.Item1;
            List<DbParameter[]> list = updateSql.Item2;
            using (DbCommand command = this.Connection.CreateCommand())
            {
                if (this.IsTransaction)
                {
                    command.Transaction = this.Transaction;
                }
                command.CommandTimeout = 36000;
                command.CommandText = sqlstr;
                command.Prepare();

                //日志添加方法
                WriteTraceLog(sqlstr, typeof(T).Name + "批量更新多条数据", list);

                foreach (DbParameter[] parameterArray in list)
                {
                    if (parameterArray.Length != 0)
                    {
                        command.Parameters.AddRange(parameterArray);
                    }



                    InfluenceNum += await command.ExecuteNonQueryAsync();
                    command.Parameters.Clear();
                }
            }
            this.InfluenceRows += InfluenceNum;
            return InfluenceNum;
        }



        #endregion



        #endregion



        #endregion

    }
}
