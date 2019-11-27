using RoadFlow.Mapper;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Data.SqlInterface
{



    #region 新的方法 2.8.3

    public class SqlServer : ISql
    {
        // Fields
        private readonly string connStr;
        private readonly RoadFlow.Model.DbConnection dbConnectionModel;
        private readonly string dbType;

        // Methods
        public SqlServer(RoadFlow.Model.DbConnection dbConnection, string dataBaseType)
        {
            this.dbType = dataBaseType.ToLower();
            if (dbConnection != null)
            {
                this.connStr = dbConnection.ConnString;
                this.dbConnectionModel = dbConnection;
            }
        }

        //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetApplibrarySql(string title, string address, string typeId, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Title,Title)>0");
                list.Add(new SqlParameter("@Title", title.Trim()));
            }
            if (!address.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Address,Address)>0");
                list.Add(new SqlParameter("@Address", address.Trim()));
            }
            if (!typeId.IsNullOrWhiteSpace())
            {
                builder.Append(" AND Type IN(" + typeId + ")");
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Title,Address,Type FROM RF_AppLibrary WHERE 1=1" + builder.ToString(), list.ToArray());
        }

        public DbParameter GetDbParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        public string GetDbTablesSql()
        {
            return "SELECT name TABLE_NAME,(select top 1 cast([value] as varchar) [value] from sys.extended_properties where major_id=sysobjects.id and minor_id=0) COMMENTS from sysobjects WHERE xtype='U' or xtype='V' ORDER BY xtype,name";
        }

        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetDocReadUserListSql(string docId, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new SqlParameter("@DocId", docId)
        };
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_DocUser WHERE DocId=@DocId" + builder.ToString(), list.ToArray());
        }

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetDocSql(Guid userId, string title, string dirId, string date1, string date2, string order, int read)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new SqlParameter("@UserId", userId)
        };
            if (read >= 0)
            {
                list.Add(new SqlParameter("@IsRead", (int)read));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Title,a.Title)>0");
                list.Add(new SqlParameter("@Title", title.Trim()));
            }
            if (!dirId.IsNullOrWhiteSpace())
            {
                builder.Append(" AND DirId IN(" + dirId + ")");
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND a.WriteTime>=@WriteTime");
                list.Add(new SqlParameter("@WriteTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND a.WriteTime<@WriteTime1");
                list.Add(new SqlParameter("@WriteTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>(("SELECT Id,DirId,DirName,Title,WriteTime,WriteUserName,ReadCount,DocRank FROM RF_Doc a WHERE EXISTS(SELECT Id FROM RF_DocUser WHERE DocId=a.Id AND UserId=@UserId" + ((read >= 0) ? " AND IsRead=@IsRead" : "") + ")") + builder.ToString(), list.ToArray());
        }

        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter> GetFieldValueSql(string tableName, string fieldName, string primaryKey, string primaryKeyValue)
        {
            string[] textArray1 = new string[] { "SELECT ", fieldName, " FROM ", tableName, " WHERE ", primaryKey, "=@primarykeyvalue" };
            return new ValueTuple<string, DbParameter>(string.Concat((string[])textArray1), new SqlParameter("@primarykeyvalue", primaryKeyValue));
        }

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFlowArchiveSql(string flowId, string stepName, string title, string date1, string date2, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (flowId.IsGuid(out guid))
            {
                builder.Append(" AND FlowId=@FlowId");
                list.Add(new SqlParameter("@FlowId", guid));
            }
            if (!stepName.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@StepName,StepName)>0");
                list.Add(new SqlParameter("@StepName", stepName));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Title,Title)>0");
                list.Add(new SqlParameter("@Title", title));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND WriteTime>=@WriteTime");
                list.Add(new SqlParameter("@WriteTime", time));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND WriteTime<@WriteTime1");
                list.Add(new SqlParameter("@WriteTime1", time2.AddDays(1.0).ToDateString()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,FlowId,FlowName,StepId,StepName,TaskId,GroupId,InstanceId,Title,UserName,WriteTime FROM RF_FlowArchive WHERE 1=1" + builder.ToString(), list.ToArray());
        }

        //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFlowEntrustSql(string userId, string date1, string date2, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (userId.IsGuid(out guid))
            {
                builder.Append(" AND UserId=@UserId");
                list.Add(new SqlParameter("@UserId", guid));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND StartTime>=@StartTime");
                list.Add(new SqlParameter("@StartTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND StartTime<@StartTime1");
                list.Add(new SqlParameter("@StartTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_FlowEntrust WHERE 1=1" + builder.ToString(), list.ToArray());
        }

        //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        //public ValueTuple<string, DbParameter[]> GetFlowSql(List<Guid> flowIdList, string name, string type, string order, int status = -1)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    List<DbParameter> list = new List<DbParameter>();
        //    if (!name.IsNullOrWhiteSpace())
        //    {
        //        builder.Append(" AND CHARINDEX(@Name,Name)>0");
        //        list.Add(new SqlParameter("@Name", name.Trim()));
        //    }
        //    if (!type.IsNullOrWhiteSpace())
        //    {
        //        builder.Append(" AND FlowType IN(" + type + ")");
        //    }
        //    return new ValueTuple<string, DbParameter[]>(("SELECT Id,Name,CreateDate,CreateUser,Status,Note FROM RF_Flow WHERE Id IN(" + flowIdList.JoinSqlIn<Guid>(true) + ")" + ((status != -1) ? (" AND Status=" + ((int)status).ToString()) : " AND Status!=3")) + builder.ToString(), list.ToArray());
        //}

        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFormSql(string name, string type, string order, int status = -1)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Name,Name)>0");
                list.Add(new SqlParameter("@Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FormType IN(" + type + ")");
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateUserName,CreateDate,EditDate,Status FROM RF_Form WHERE " + ((status == -1) ? "Status!=2" : ("Status=" + ((int)status).ToString())) + builder.ToString(), list.ToArray());
        }


        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFormSql(Guid userId, string name, string type, string order, int status = -1)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new SqlParameter("@ManageUser", userId.ToLowerString())
    };
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Name,Name)>0");
                list.Add(new SqlParameter("@Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FormType IN(" + type + ")");
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateUserName,CreateDate,EditDate,Status FROM RF_Form WHERE CHARINDEX(@ManageUser,ManageUser)>0 AND " + ((status == -1) ? "Status!=2" : ("Status=" + ((int)status).ToString())) + builder.ToString(), list.ToArray());
        }














        //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetHomeSetSql(string name, string title, string type, string order)
        {
            int num;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Name,Name)>0");
                list.Add(new SqlParameter("@Name", name.Trim()));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Title,Title)>0");
                list.Add(new SqlParameter("@Title", title.Trim()));
            }
            if (type.IsInt(out num))
            {
                builder.Append(" AND Type=@Type");
                list.Add(new SqlParameter("@Type", (int)num));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_HomeSet WHERE 1=1" + builder.ToString(), list.ToArray());
        }

        public string GetIdentitySql(string seqName = "")
        {
            return "SELECT @@IDENTITY";
        }

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetLogSql(string title, string type, string userId, string date1, string date2, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Title,Title)>0");
                list.Add(new SqlParameter("@Title", title.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND Type=@Type");
                list.Add(new SqlParameter("@Type", type));
            }
            if (userId.IsGuid(out guid))
            {
                builder.Append(" AND UserId=@UserId");
                list.Add(new SqlParameter("@UserId", guid));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND WriteTime>=@WriteTime");
                list.Add(new SqlParameter("@WriteTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND WriteTime<@WriteTime1");
                list.Add(new SqlParameter("@WriteTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Title,Type,WriteTime,UserName,IPAddress,CityAddress FROM RF_Log WHERE 1=1" + builder.ToString(), list.ToArray());
        }

        //[return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMessageReadUserListSql(string messageId, string order)
        {
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_MessageUser WHERE MessageId=@MessageId", new List<DbParameter> { new SqlParameter("@MessageId", messageId) }.ToArray());
        }

        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMessageSendListSql(string userId, string content, string date1, string date2, string status, string order)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if ("1".Equals(status))
            {
                builder.Append(" AND Exists(SELECT Id From RF_MessageUser WHERE RF_MessageUser.MessageId=RF_Message.Id AND IsRead=0 AND UserId=@UserId)");
                list.Add(new SqlParameter("@UserId", userId));
            }
            else if ("2".Equals(status))
            {
                builder.Append(" AND Exists(SELECT Id From RF_MessageUser WHERE RF_MessageUser.MessageId=RF_Message.Id AND IsRead=1 AND UserId=@UserId)");
                list.Add(new SqlParameter("@UserId", userId));
            }
            else
            {
                Guid guid;
                if ("0".Equals(status) && userId.IsGuid(out guid))
                {
                    builder.Append(" AND SenderId=@SenderId");
                    list.Add(new SqlParameter("@SenderId", guid));
                }
            }
            if (!content.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Contents,Contents)>0");
                list.Add(new SqlParameter("@Contents", content.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendTime>=@SendTime");
                list.Add(new SqlParameter("@SendTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendTime<@SendTime1");
                list.Add(new SqlParameter("@SendTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Contents,SendType,SenderName,SendTime,Files FROM RF_Message WHERE 1=1" + builder.ToString(), list.ToArray());
        }

        public string GetPaerSql(string sql, int size, int number, out int count, DbParameter[] param = null, string order = "")
        {
            int num;
            string str = string.Empty;
            using (DataContext context = (this.dbConnectionModel == null) ? new DataContext() : new DataContext(this.dbConnectionModel.ConnType, this.dbConnectionModel.ConnString, true))
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

        //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetProgramSql(string name, string types, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Name,Name)>0");
                list.Add(new SqlParameter("@Name", name.Trim()));
            }
            if (!types.IsNullOrWhiteSpace())
            {
                builder.Append(" AND Type IN(" + types + ")");
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,Type,CreateTime,PublishTime,CreateUserId,Status FROM RF_Program WHERE Status IN(0,1)" + builder.ToString(), list.ToArray());
        }

        //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryCompletedTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new SqlParameter("@ReceiveId", userId)
        };
            if (flowId.IsGuid(out guid))
            {
                builder.Append(" AND FlowId=@FlowId");
                list.Add(new SqlParameter("@FlowId", guid));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Title,Title)>0");
                list.Add(new SqlParameter("@Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND CompletedTime1>=@CompletedTime1");
                list.Add(new SqlParameter("@CompletedTime1", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND CompletedTime1<@CompletedTime11");
                list.Add(new SqlParameter("@CompletedTime11", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,FlowId,FlowName,StepId,StepName,GroupId,InstanceId,Title,SenderId,SenderName,ReceiveTime,CompletedTime,CompletedTime1,Status,ExecuteType,Note  FROM RF_FlowTask WHERE ReceiveId=@ReceiveId AND ExecuteType>1" + builder.ToString(), list.ToArray());
        }

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryEntrustTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new SqlParameter("@EntrustUserId", userId)
        };
            if (flowId.IsGuid(out guid))
            {
                builder.Append(" AND FlowId=@FlowId");
                list.Add(new SqlParameter("@FlowId", guid));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Title,Title)>0");
                list.Add(new SqlParameter("@Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=@ReceiveTime");
                list.Add(new SqlParameter("@ReceiveTime", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<@ReceiveTime1");
                list.Add(new SqlParameter("@ReceiveTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,FlowId,FlowName,StepId,StepName,GroupId,InstanceId,Title,SenderName,ReceiveTime,ReceiveName,CompletedTime1,Status,ExecuteType,Note FROM RF_FlowTask WHERE EntrustUserId=@EntrustUserId" + builder.ToString(), list.ToArray());
        }

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryGroupMaxTaskSql(Guid gropuId)
        {
            string str = "SELECT TOP 1 * FROM RF_FlowTask WHERE GroupId=@GroupId ORDER BY Sort DESC";
            SqlParameter[] parameterArray = new SqlParameter[] { new SqlParameter("@GroupId", gropuId) };
            return new ValueTuple<string, DbParameter[]>(str, parameterArray);
        }

        //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryInstanceSql(string flowId, string title, string receiveId, string receiveDate1, string receiveDate2, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!flowId.IsNullOrWhiteSpace())
            {
                Guid guid2;
                if (flowId.IsGuid(out guid2))
                {
                    builder.Append(" AND FlowId=@FlowId");
                    list.Add(new SqlParameter("@FlowId", guid2));
                    goto Label_0073;
                }
                if (true)
                {
                    builder.Append(" AND FlowId IN(" + flowId + ")");
                    goto Label_0073;
                }
            }
            builder.Append(" AND 1=0");
            Label_0073:
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Title,Title)>0");
                list.Add(new SqlParameter("@Title", title.Trim()));
            }
            if (receiveId.IsGuid(out guid))
            {
                builder.Append(" AND ReceiveId=@ReceiveId");
                list.Add(new SqlParameter("@ReceiveId", guid));
            }
            if (receiveDate1.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=@ReceiveTime");
                list.Add(new SqlParameter("@ReceiveTime", time.GetDate()));
            }
            if (receiveDate2.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<@ReceiveTime1");
                list.Add(new SqlParameter("@ReceiveTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT *,ROW_NUMBER() OVER(ORDER BY ReceiveTime DESC) AS PagerAutoRowNumber FROM(SELECT GroupId,MAX(ReceiveTime) ReceiveTime From RF_FlowTask WHERE 1=1 " + builder.ToString() + " GROUP BY GroupId) TaskTemp", list.ToArray());
        }

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryWaitTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order, int isBatch = 0)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new SqlParameter("@ReceiveId", userId)
    };
            if (!flowId.IsNullOrWhiteSpace())
            {
                Guid guid;
                if (flowId.IsGuid(out guid))
                {
                    builder.Append(" AND FlowId=@FlowId");
                    list.Add(new SqlParameter("@FlowId", guid));
                }
                else
                {
                    builder.Append(" AND FlowId IN(" + flowId.ToSqlIn(true) + ")");
                }
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Title,Title)>0");
                list.Add(new SqlParameter("@Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=@ReceiveTime");
                list.Add(new SqlParameter("@ReceiveTime", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<@ReceiveTime1");
                list.Add(new SqlParameter("@ReceiveTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>(("SELECT Id,FlowId,FlowName,StepId,StepName,InstanceId,GroupId,TaskType,Title,SenderId,SenderName,ReceiveTime,CompletedTime,Status,Note,IsBatch FROM RF_FlowTask WHERE ReceiveId=@ReceiveId" + ((isBatch == 0) ? "" : " AND IsBatch=1") + " AND Status IN(0,1)") + builder.ToString(), list.ToArray());
        }

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetSaveDataSql(Dictionary<string, object> dicts, string tableName, string primaryKey, int flag)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();

            if (flag == 0)
            {
                builder.Append("DELETE FROM " + tableName);
            }
            else if (1 == flag)
            {
                //      builder.Append("set identity_insert  " + tableName + " ON  ");
                builder.Append("INSERT INTO " + tableName + "(");
                foreach (KeyValuePair<string, object> pair in dicts)
                {
                    //优化插入off的错误
                    //if (pair.Key != primaryKey || (pair.Key == primaryKey && (pair.Value !=null && pair.Value.ToString()!="")))
                    //{
                    builder.Append(pair.Key);
                    if (!pair.Key.Equals(Enumerable.Last<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>)dicts).Key))
                    {
                        builder.Append(",");
                    }
                    //}
                }
                builder.Append(") VALUES(");
            }
            else if (2 == flag)
            {
                builder.Append("UPDATE " + tableName + " SET ");
            }
            foreach (KeyValuePair<string, object> pair3 in dicts)
            {
                if (flag == 0)
                {
                    builder.Append(" WHERE " + primaryKey + "=@" + primaryKey);
                    list.Add(new SqlParameter("@" + primaryKey, dicts[primaryKey]));
                }
                else if (1 == flag)
                {
                    //优化插入off的错误
                    //if (pair3.Key != primaryKey || (pair3.Key == primaryKey && (pair3.Value != null && pair3.Value.ToString() != "")))
                    //{ 
                    string introduced6 = "@" + pair3.Key;
                    list.Add(new SqlParameter(introduced6, pair3.Value));
                    builder.Append("@" + pair3.Key);
                    if (!pair3.Key.Equals(Enumerable.Last<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>)dicts).Key))
                    {
                        builder.Append(",");
                    }
                    //}
                }
                else if (2 == flag)
                {
                    string introduced7 = "@" + pair3.Key;
                    list.Add(new SqlParameter(introduced7, pair3.Value));
                    if (!pair3.Key.EqualsIgnoreCase(primaryKey))
                    {
                        builder.Append(pair3.Key + "=@" + pair3.Key);
                        if (!pair3.Key.Equals(Enumerable.Last<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>)dicts).Key))
                        {
                            builder.Append(",");
                        }
                    }
                }
            }
            if (1 == flag)
            {
                builder.Append(")");

                //  builder.Append("  set identity_insert   " + tableName + " OFF ");
            }
            else if (2 == flag)
            {
                builder.Append(" WHERE  " + primaryKey + "=@" + primaryKey);
            }
            return new ValueTuple<string, DbParameter[]>(builder.ToString(), list.ToArray());
        }

        public string GetTableFieldsSql(string tableName)
        {
            return string.Format("select a.name as f_name,b.name as t_name,a.prec as [length],a.isnullable as is_null,a.cdefault as cdefault,COLUMNPROPERTY(OBJECT_ID('{0}'),a.name,'IsIdentity') as isidentity,(select top 1 text from sysobjects d inner join syscolumns e on e.id=d.id inner join syscomments f on f.id=e.cdefault where d.name='{0}' and e.name=a.name) as defaultvalue,cast([value] as varchar(500)) as comments from sys.syscolumns a left join sys.types b on b.user_type_id=a.xtype left join sys.extended_properties x on x.major_id=OBJECT_ID('{0}') and x.minor_id=a.colid where OBJECT_ID('{0}')=id order by a.colid", tableName);
        }

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetUserFileShareMySql(Guid userId, string fileName, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new SqlParameter("@UserId", userId),
            new SqlParameter("@ExpireDate", DateTimeExtensions.Now)
        };
            if (!fileName.IsNullOrEmpty())
            {
                builder.Append(" AND CHARINDEX(@FileName,FileName)>0");
                list.Add(new SqlParameter("@FileName", fileName));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_UserFileShare WHERE UserId=@UserId AND ExpireDate>@ExpireDate" + builder.ToString(), list.ToArray());
        }

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetUserFileShareSql(Guid userId, string fileName, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new SqlParameter("@ShareUserId", userId)
        };
            if (!fileName.IsNullOrEmpty())
            {
                builder.Append(" WHERE CHARINDEX(@FileName,FileName)>0");
                list.Add(new SqlParameter("@FileName", fileName));
            }
            return new ValueTuple<string, DbParameter[]>(("SELECT *,ROW_NUMBER() OVER(ORDER BY " + order + ") AS PagerAutoRowNumber FROM (SELECT FileId,ShareUserId,MAX(FileName) FileName,MAX(ShareDate) ShareDate,MAX(ExpireDate) ExpireDate FROM RF_UserFileShare group by FileId,ShareUserId HAVING ShareUserId=@ShareUserId) as Temp_RF_UserFileShare") + builder.ToString(), list.ToArray());
        }



        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFlowSql(List<Guid> flowIdList, string name, string type, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Name,Name)>0");
                list.Add(new SqlParameter("@Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FlowType IN(" + type + ")");
            }
            return new ValueTuple<string, DbParameter[]>(("SELECT Id,Name,CreateDate,CreateUser,Status,Note,SystemId  FROM RF_Flow WHERE Status!=3 AND Id IN(" + flowIdList.JoinSqlIn<Guid>(true) + ")") + builder.ToString(), list.ToArray());
        }




        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFlowSql(List<Guid> flowIdList, string name, string type, string order, int status = -1)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();


            if (((flowIdList == null) || (flowIdList.Count <= 0)))
            {
                builder.Append(" AND 1=0");
            }
            else
            {
                builder.Append(" AND Id IN(" + flowIdList.JoinSqlIn<Guid>(true) + ")");
            }

            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Name,Name)>0");
                list.Add(new SqlParameter("@Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FlowType IN(" + type + ")");
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateDate,CreateUser,Status,Note,SystemId  FROM RF_Flow WHERE" + ((status != -1) ? (" Status=" + ((int)status).ToString()) : " Status!=3") + builder.ToString(), list.ToArray());
        }







        // //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        //public ValueTuple<string, DbParameter[]> GetFormSql(string name, string type, string order)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    List<DbParameter> list = new List<DbParameter>();
        //    if (!name.IsNullOrWhiteSpace())
        //    {
        //        builder.Append(" AND CHARINDEX(@Name,Name)>0");
        //        list.Add(new SqlParameter("@Name", name.Trim()));
        //    }
        //    if (!type.IsNullOrWhiteSpace())
        //    {
        //        builder.Append(" AND FormType IN(" + type + ")");
        //    }
        //    return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateUserName,CreateDate,EditDate,Status FROM RF_Form WHERE Status!=2" + builder.ToString(), list.ToArray());
        //}





        #region 2.8.5新增

        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMailDeletedBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new SqlParameter("@UserId", currentUserId)
    };
            if (userId.IsGuid(out guid))
            {
                builder.Append(" AND SendUserId=@SendUserId");
                list.Add(new SqlParameter("@SendUserId", guid));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Subject,Subject)>0");
                list.Add(new SqlParameter("@Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendDateTime>=@SendDateTime");
                list.Add(new SqlParameter("@SendDateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendDateTime<@SendDateTime1");
                list.Add(new SqlParameter("@SendDateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_MailDeletedBox WHERE UserId=@UserId" + builder.ToString(), list.ToArray());
        }








        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMailInBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new SqlParameter("@UserId", currentUserId)
    };
            if (userId.IsGuid(out guid))
            {
                builder.Append(" AND SendUserId=@SendUserId");
                list.Add(new SqlParameter("@SendUserId", guid));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Subject,Subject)>0");
                list.Add(new SqlParameter("@Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendDateTime>=@SendDateTime");
                list.Add(new SqlParameter("@SendDateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendDateTime<@SendDateTime1");
                list.Add(new SqlParameter("@SendDateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_MailInBox WHERE UserId=@UserId" + builder.ToString(), list.ToArray());
        }









        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMailOutBoxSql(Guid currentUserId, string subject, string date1, string date2, int status = -1)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new SqlParameter("@UserId", currentUserId)
    };
            if (status != -1)
            {
                builder.Append(" AND Status=@Status");
                list.Add(new SqlParameter("@Status", (int)status));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Subject,Subject)>0");
                list.Add(new SqlParameter("@Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendDateTime>=@SendDateTime");
                list.Add(new SqlParameter("@SendDateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendDateTime<@SendDateTime1");
                list.Add(new SqlParameter("@SendDateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_MailOutBox WHERE UserId=@UserId" + builder.ToString(), list.ToArray());
        }




        #endregion


        #region 2.8.8新增问卷



        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetPartakeUserSql(Guid voteId, string name, string org)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new SqlParameter("@VoteId", voteId.ToString())
    };
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@UserName,UserName)>0");
                list.Add(new SqlParameter("@UserName", name.Trim()));
            }
            if (!org.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@UserOrganize,UserOrganize)>0");
                list.Add(new SqlParameter("@UserOrganize", org.Trim()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_VotePartakeUser WHERE VoteId=@VoteId" + builder.ToString(), list.ToArray());
        }







        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetVoteResultSql(Guid currentUserId, string topic, string date1, string date2)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new SqlParameter("@UserId", currentUserId)
    };
            if (!topic.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Topic,Topic)>0");
                list.Add(new SqlParameter("@Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND CreateTime>=@CreateTime");
                list.Add(new SqlParameter("@CreateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND CreateTime<@CreateTime1");
                list.Add(new SqlParameter("@CreateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_Vote WHERE EXISTS(SELECT Id FROM RF_VoteResultUser WHERE UserId=@UserId AND VoteId=RF_Vote.Id)" + builder.ToString(), list.ToArray());
        }

        //    [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetVoteSql(Guid currentUserId, string topic, string date1, string date2)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new SqlParameter("@CreateUserId", currentUserId)
    };
            if (!topic.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Topic,Topic)>0");
                list.Add(new SqlParameter("@Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND CreateTime>=@CreateTime");
                list.Add(new SqlParameter("@CreateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND CreateTime<@CreateTime1");
                list.Add(new SqlParameter("@CreateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_Vote WHERE CreateUserId=@CreateUserId" + builder.ToString(), list.ToArray());
        }

        //[return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetWaitSubmitVoteSql(Guid currentUserId, string topic, string date1, string date2)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new SqlParameter("@UserId", currentUserId)
    };
            if (!topic.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Topic,Topic)>0");
                list.Add(new SqlParameter("@Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND CreateTime>=@CreateTime");
                list.Add(new SqlParameter("@CreateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND CreateTime<@CreateTime1");
                list.Add(new SqlParameter("@CreateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_Vote WHERE EXISTS(SELECT Id FROM RF_VotePartakeUser WHERE UserId=@UserId AND VoteId=RF_Vote.Id AND Status=0)" + builder.ToString(), list.ToArray());
        }



        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryMyStartTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string status, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            string str = string.Empty;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            int num = status.ToInt(-1);
            str = "SELECT * FROM (SELECT Id,PrevId,FlowId,FlowName,StepId,GroupId,InstanceId,Title,SenderId,ReceiveTime,(select top 1 StepName from RF_FlowTask where GroupId=aaa.GroupId and TaskType!=5 and Status!=2 order by Sort desc) as CurrentStepName FROM RF_FlowTask aaa) bbb WHERE SenderId=@SenderId AND PrevId=@PrevId";
            list.Add(new SqlParameter("@SenderId", userId));
            list.Add(new SqlParameter("@PrevId", Guid.Empty));
            if (flowId.IsGuid(out guid))
            {
                builder.Append(" AND FlowId=@FlowId");
                list.Add(new SqlParameter("@FlowId", guid));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(@Title,Title)>0");
                list.Add(new SqlParameter("@Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=@ReceiveTime");
                list.Add(new SqlParameter("@ReceiveTime", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<@ReceiveTime1");
                list.Add(new SqlParameter("@ReceiveTime1", time2.AddDays(1.0).GetDate()));
            }
            if (num > -1)
            {
                builder.Append(" AND CurrentStepName " + ((num == 0) ? "IS NOT NULL" : "IS NULL"));
            }
            return new ValueTuple<string, DbParameter[]>(str + builder.ToString(), list.ToArray());
        }










        #endregion


    }





    #endregion


}
