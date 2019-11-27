using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace RoadFlow.Mapper
{
  

    #region 新的方法2.8.3
    internal interface IData
    {
        /// <summary>
        /// 批量复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="dbConnection"></param>
        /// <param name="dbTransaction"></param>
        void BulkCopy<T>(IEnumerable<T> ts, DbConnection dbConnection, DbTransaction dbTransaction);
        /// <summary>
        /// 获取查找Sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <returns></returns>
        ValueTuple<string, DbParameter[]> GetFindSql<T>(params object[] objects) where T : class, new();
        /// <summary>
        /// 获取主键Sql语句
        /// </summary>
        /// <param name="seqName"></param>
        /// <returns></returns>
        string GetIdentitySql(string seqName = "");

        /// <summary>
        /// 获取批量插入语句 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        ValueTuple<string, List<DbParameter[]>> GetInsertSql<T>(IEnumerable<T> ts) where T : class, new();
        /// <summary>
        /// 获取插入语句 主键是Guid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        ValueTuple<string, DbParameter[]> GetInsertSql<T>(T t) where T : class, new();
        /// <summary>
        /// 获取插入语句 主键是int
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        ValueTuple<string, DbParameter[]> GetInsertIntSql<T>(T t, string ConnectionString) where T : class, new();
        /// <summary>
        /// 获取查询单条Sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string GetQueryOneSql(string sql);
        /// <summary>
        /// 获取批量移除sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        ValueTuple<string, List<DbParameter[]>> GetRemoveSql<T>(IEnumerable<T> ts) where T : class, new();
        /// <summary>
        /// 获取移除sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        ValueTuple<string, DbParameter[]> GetRemoveSql<T>(T t) where T : class, new();
        /// <summary>
        /// 获取对象转换成参数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        ValueTuple<string, DbParameter[]> GetSqlAndParameter(string sql, params object[] objects);
        /// <summary>
        /// 获取批量更新Sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        ValueTuple<string, List<DbParameter[]>> GetUpdateSql<T>(IEnumerable<T> ts) where T : class, new();
        /// <summary>
        /// 获取更新Sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        ValueTuple<string, DbParameter[]> GetUpdateSql<T>(T t) where T : class, new();
        /// <summary>
        /// 数据读取到List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        List<T> ReaderToList<T>(DbDataReader reader) where T : class, new();


        /// <summary>
        /// 获取表主键
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        string GetTableKey(string tablename, string ConnectionString);

      /// <summary>
      /// 分页功能
      /// </summary>
      /// <param name="sql"></param>
      /// <param name="size"></param>
      /// <param name="number"></param>
      /// <param name="count"></param>
      /// <param name="ConnType"></param>
      /// <param name="ConnString"></param>
      /// <param name="param"></param>
      /// <param name="order"></param>
      /// <returns></returns>
        string GetPaerSql(string sql, int size, int number, out int count, string ConnType, string ConnString, DbParameter[] param = null, string order = "");

        /// <summary>
        /// QueryField设置参数条件
        /// </summary>
        /// <param name="typename"></param>
        /// <param name="typevalue"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ValueTuple<string, DbParameter> GetConditions(string typename, string typevalue, datatype type);
       
    }

    #endregion

}
