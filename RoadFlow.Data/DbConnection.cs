using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RoadFlow.Data
{







    public class DbConnection
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_dbconnection";

        // Methods
        public int Add(RoadFlow.Model.DbConnection dbConnection)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.DbConnection>(dbConnection);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_dbconnection");
        }

        public int Delete(RoadFlow.Model.DbConnection[] dbConnections)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.DbConnection>(dbConnections);
                return context.SaveChanges();
            }
        }

        //[return:TupleElementNames(new string[] { null, "paramObjs" })]
        public string ExecuteSQL(RoadFlow.Model.DbConnection dbConnection, List<ValueTuple<string, IEnumerable<object>>> ts)
        {
            string message;
            DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true);
            try
            {
                int num = 0;
                foreach (ValueTuple<string, IEnumerable<object>> local1 in ts)
                {
                    string sql = local1.Item1;
                    IEnumerable<object> enumerable = local1.Item2;
                    num += context.Execute(sql, Enumerable.ToArray<object>(enumerable));
                }
                context.SaveChanges();
                message = ((int)num).ToString();
            }
            catch (Exception exception1)
            {
                message = exception1.Message;
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }
            return message;
        }


        //[return:TupleElementNames(new string[] { "sql", "parameters" })]
        public string ExecuteSQL(RoadFlow.Model.DbConnection dbConnection, List<ValueTuple<string, DbParameter[]>> tuples)
        {
            string message;
            DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true);
            try
            {
                foreach (ValueTuple<string, DbParameter[]> local1 in tuples)
                {
                    string sql = local1.Item1;
                    DbParameter[] parameters = local1.Item2;
                    context.Execute(sql, parameters);
                }
                message = ((int)context.SaveChanges()).ToString();
            }
            catch (Exception exception1)
            {
                message = exception1.Message;
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }
            return message;
        }

        public string ExecuteSQL(RoadFlow.Model.DbConnection dbConnection, string sql, IEnumerable<object> paramObjs = null)
        {
            string message;
            DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true);
            try
            {
                object[] objects = new object[] { paramObjs };
                int num = context.Execute(sql, objects);
                context.SaveChanges();
                message = ((int)num).ToString();
            }
            catch (Exception exception1)
            {
                message = exception1.Message;
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }
            return message;
        }

        public string ExecuteSQL(RoadFlow.Model.DbConnection dbConnection, string sql, params object[] paramObjs)
        {
            string message;
            DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true);
            try
            {
                int num = context.Execute(sql, paramObjs);
                context.SaveChanges();
                message = ((int)num).ToString();
            }
            catch (Exception exception1)
            {
                message = exception1.Message;
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }
            return message;
        }

        public List<RoadFlow.Model.DbConnection> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_dbconnection");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.DbConnection> list = Enumerable.ToList<RoadFlow.Model.DbConnection>((IEnumerable<RoadFlow.Model.DbConnection>)Enumerable.OrderBy<RoadFlow.Model.DbConnection, int>((IEnumerable<RoadFlow.Model.DbConnection>)context.QueryAll<RoadFlow.Model.DbConnection>(), key => key.Sort));
                    IO.Insert("roadflow_cache_dbconnection", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.DbConnection>)obj2;
        }

        public DataTable GetDataTable(RoadFlow.Model.DbConnection dbConnection, string sql, DbParameter[] parameters = null)
        {
            using (DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true))
            {
                return context.GetDataTable(sql, parameters);
            }
        }





        public DataTable GetDataTable(RoadFlow.Model.DbConnection dbConnection, string sql, params object[] objects)
        {
            using (DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true))
            {
                return context.GetDataTable(sql, objects);
            }
        }

        public DataTable GetDataTable(RoadFlow.Model.DbConnection dbConnection, string tableName, string primaryKey, string primaryKeyValue)
        {
            ValueTuple<string, DbParameter> tuple = new DbconnnectionSql(dbConnection).SqlInstance.GetFieldValueSql(tableName, "*", primaryKey, primaryKeyValue);
            DbParameter[] parameters = new DbParameter[] { tuple.Item2 };
            return this.GetDataTable(dbConnection, tuple.Item1, parameters);
        }

        public DataTable GetDataTableSchema(RoadFlow.Model.DbConnection dbConnection, string sql, DbParameter[] parameters)
        {
            using (DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true))
            {
                return context.GetDataTableSchema(sql, parameters);
            }
        }

        public string GetFieldValue(RoadFlow.Model.DbConnection dbConnection, string sql)
        {
            DataTable table = this.GetDataTable(dbConnection, sql.FilterSelectSql(), (DbParameter[])null);
            if (table.Rows.Count <= 0)
            {
                return string.Empty;
            }
            return table.Rows[0][0].ToString();
        }

        public string GetFieldValue(RoadFlow.Model.DbConnection dbConnection, string sql, params object[] objs)
        {
            DataTable table = this.GetDataTable(dbConnection, sql.FilterSelectSql(), objs);
            if (table.Rows.Count <= 0)
            {
                return string.Empty;
            }
            return table.Rows[0][0].ToString();
        }

        public string GetFieldValue(RoadFlow.Model.DbConnection dbConnection, string tableName, string fieldName, string primaryKey, string primaryKeyValue)
        {
            ValueTuple<string, DbParameter> tuple = new DbconnnectionSql(dbConnection).SqlInstance.GetFieldValueSql(tableName, fieldName, primaryKey, primaryKeyValue);
            DbParameter[] parameters = new DbParameter[] { tuple.Item2 };
            DataTable table = this.GetDataTable(dbConnection, tuple.Item1, parameters);
            if (table.Rows.Count <= 0)
            {
                return string.Empty;
            }
            return table.Rows[0][0].ToString();
        }

        public DataTable GetTableFields(RoadFlow.Model.DbConnection dbConnection, string table)
        {
            using (DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true))
            {
                return context.GetDataTable(new DbconnnectionSql(dbConnection).SqlInstance.GetTableFieldsSql(table), (DbParameter[])null);
            }
        }

        public DataTable GetTables(RoadFlow.Model.DbConnection dbConnection)
        {
            using (DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true))
            {
                return context.GetDataTable(new DbconnnectionSql(dbConnection).SqlInstance.GetDbTablesSql(), (DbParameter[])null);
            }
        }


        //[return:TupleElementNames(new string[] { "dicts", "tableName", "primaryKey", "flag" })]
        public string SaveData(RoadFlow.Model.DbConnection dbConnection, List<ValueTuple<Dictionary<string, object>, string, string, int>> tuples, bool isIdentity = false, string seqName = "")
        {
            DbconnnectionSql sql = new DbconnnectionSql(dbConnection);
            List<ValueTuple<string, DbParameter[]>> list = new List<ValueTuple<string, DbParameter[]>>();
            foreach (ValueTuple<Dictionary<string, object>, string, string, int> local1 in tuples)
            {
                Dictionary<string, object> dicts = local1.Item1;
                string tableName = local1.Item2;
                string primaryKey = local1.Item3;
                int flag = local1.Item4;
                ValueTuple<string, DbParameter[]> tuple1 = sql.SqlInstance.GetSaveDataSql(dicts, tableName, primaryKey, flag);
                string str3 = tuple1.Item1;
                DbParameter[] parameterArray = tuple1.Item2;
                list.Add(new ValueTuple<string, DbParameter[]>(str3, parameterArray));
            }
            using (DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true))
            {
                int identity = 0;
                foreach (ValueTuple<string, DbParameter[]> local2 in list)
                {
                    string str4 = local2.Item1;
                    DbParameter[] parameters = local2.Item2;
                    identity += context.Execute(str4, parameters);
                }
                if (isIdentity)
                {
                    identity = context.GetIdentity(seqName);
                }
                context.SaveChanges();
                return ((int)identity).ToString();
            }
        }

        public string TesetConnection(RoadFlow.Model.DbConnection dbConnection)
        {
            using (DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true))
            {
                return context.TestConnection();
            }
        }

        public string TestSQL(RoadFlow.Model.DbConnection dbConnection, string sql, DbParameter[] parameters)
        {
            using (DataContext context = new DataContext(dbConnection.ConnType, dbConnection.ConnString, true))
            {
                return context.TestSQL(sql, parameters);
            }
        }

        public int Update(RoadFlow.Model.DbConnection dbConnection)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.DbConnection>(dbConnection);
                return context.SaveChanges();
            }
        }

    }
  
}
