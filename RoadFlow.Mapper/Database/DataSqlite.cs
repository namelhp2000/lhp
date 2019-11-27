using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using RoadFlow.Utility;

namespace RoadFlow.Mapper
{
    /// <summary>
    /// Sqlite数据转换
    /// </summary>
    public class DataSqlite : IData
    {
        public void BulkCopy<T>(IEnumerable<T> ts, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            throw new NotImplementedException();
        }

        public (string, DbParameter) GetConditions(string typename, string typevalue, datatype type)
        {
            throw new NotImplementedException();
        }

        public (string, DbParameter[]) GetFindSql<T>(params object[] objects) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public string GetIdentitySql(string seqName = "")
        {
            throw new NotImplementedException();
        }

        public (string, DbParameter[]) GetInsertIntSql<T>(T t, string ConnectionString) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public (string, List<DbParameter[]>) GetInsertSql<T>(IEnumerable<T> ts) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public (string, DbParameter[]) GetInsertSql<T>(T t) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public string GetPaerSql(string sql, int size, int number, out int count, string ConnType, string ConnString, DbParameter[] param = null, string order = "")
        {
            throw new NotImplementedException();
        }

        public string GetQueryOneSql(string sql)
        {
            throw new NotImplementedException();
        }

        public (string, List<DbParameter[]>) GetRemoveSql<T>(IEnumerable<T> ts) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public (string, DbParameter[]) GetRemoveSql<T>(T t) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public (string, DbParameter[]) GetSqlAndParameter(string sql, params object[] objects)
        {
            throw new NotImplementedException();
        }

        public string GetTableKey(string tablename, string ConnectionString)
        {
            throw new NotImplementedException();
        }

        public (string, List<DbParameter[]>) GetUpdateSql<T>(IEnumerable<T> ts) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public (string, DbParameter[]) GetUpdateSql<T>(T t) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public List<T> ReaderToList<T>(DbDataReader reader) where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
