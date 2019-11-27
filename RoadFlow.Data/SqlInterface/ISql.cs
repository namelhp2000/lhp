using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Http.Features;

namespace RoadFlow.Data.SqlInterface
{
    #region 
    public interface ISql
    {
        // Methods

        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetApplibrarySql(string title, string address, string typeId, string order);
        DbParameter GetDbParameter(string name, object value);
        string GetDbTablesSql();
        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetDocReadUserListSql(string docId, string order);
        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetDocSql(Guid userId, string title, string dirId, string date1, string date2, string order, int read);
        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter> GetFieldValueSql(string tableName, string fieldName, string primaryKey, string primaryKeyValue);
        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetFlowArchiveSql(string flowId, string stepName, string title, string date1, string date2, string order);
        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetFlowEntrustSql(string userId, string date1, string date2, string order);
        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetFlowSql(List<Guid> flowIdList, string name, string type, string order);
        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
       // ValueTuple<string, DbParameter[]> GetFormSql(string name, string type, string order);

        //[return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetFormSql(Guid userId, string name, string type, string order, int status = -1);


        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetHomeSetSql(string name, string title, string type, string oreder);
        string GetIdentitySql(string seqName = "");
        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetLogSql(string title, string type, string userId, string date1, string date2, string order);
        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetMessageReadUserListSql(string messageId, string order);
        //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetMessageSendListSql(string userId, string content, string date1, string date2, string status, string order);
        string GetPaerSql(string sql, int size, int number, out int count, DbParameter[] param = null, string order = "");
       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetProgramSql(string name, string types, string order);
        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetQueryCompletedTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order);
        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetQueryGroupMaxTaskSql(Guid gropuId);
        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetQueryInstanceSql(string flowId, string title, string receiveId, string receiveDate1, string receiveDate2, string order);
        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetQueryWaitTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order ,int isBatch = 0);
 

 

        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetSaveDataSql(Dictionary<string, object> dicts, string tableName, string primaryKey, int flag);
        string GetTableFieldsSql(string tableName);



        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetUserFileShareSql(Guid userId, string fileName, string order);
        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetUserFileShareMySql(Guid userId, string fileName, string order);


        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetFlowSql(List<Guid> flowIdList, string name, string type, string order, int status = -1);



        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetFormSql(string name, string type, string order, int status = -1);


        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetQueryEntrustTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order);

        #region 2.8.5新增流程
        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetMailDeletedBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2);


      //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetMailInBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2);

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetMailOutBoxSql(Guid currentUserId, string subject, string date1, string date2, int status);
        #endregion



        #region 2.8.8新增问卷调查


      //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetPartakeUserSql(Guid voteId, string name, string org);


        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetVoteResultSql(Guid currentUserId, string topic, string date1, string date2);
       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetVoteSql(Guid currentUserId, string topic, string date1, string date2);
       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetWaitSubmitVoteSql(Guid currentUserId, string topic, string date1, string date2);


        #endregion



       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        ValueTuple<string, DbParameter[]> GetQueryMyStartTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string status, string order);



    }
    #endregion








}
