using MySql.Data.MySqlClient;
using RoadFlow.Mapper;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Data.SqlInterface
{
    public class MySql : ISql
    {
        // Fields
        private readonly string connStr;
        private readonly RoadFlow.Model.DbConnection dbConnectionModel;
        private readonly string dbType;

        // Methods
        public MySql(RoadFlow.Model.DbConnection dbConnection, string dataBaseType)
        {
            this.dbType = dataBaseType.ToLower();
            if (dbConnection != null)
            {
                this.connStr = dbConnection.ConnString;
                this.dbConnectionModel = dbConnection;
            }
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetApplibrarySql(string title, string address, string typeId, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,@Title)>0");
                list.Add((DbParameter)new MySqlParameter("@Title", title.Trim()));
            }
            if (!address.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Address,@Address)>0");
                list.Add((DbParameter)new MySqlParameter("@Address", address.Trim()));
            }
            if (!typeId.IsNullOrWhiteSpace())
            {
                builder.Append(" AND Type IN(" + typeId + ")");
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Title,Address,Type FROM RF_AppLibrary WHERE 1=1" + builder.ToString(), list.ToArray());
        }

        public DbParameter GetDbParameter(string name, object value)
        {
            return (DbParameter)new MySqlParameter(name, value);
        }

        public string GetDbTablesSql()
        {
            using (DataContext context = new DataContext(this.dbType, this.connStr, true))
            {
                return ("select TABLE_NAME, TABLE_COMMENT COMMENTS from information_schema.TABLES where TABLE_SCHEMA='" + context.Connection.Database + "' and (table_type='BASE TABLE' or table_type='VIEW')");
            }
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetDocReadUserListSql(string docId, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new MySqlParameter("@DocId", docId)
        };
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_DocUser WHERE DocId=@DocId" + builder.ToString(), list.ToArray());
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetDocSql(Guid userId, string title, string dirId, string date1, string date2, string order, int read)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new MySqlParameter("@UserId", userId)
        };
            if (read >= 0)
            {
                list.Add((DbParameter)new MySqlParameter("@IsRead", (int)read));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(a.Title,@Title)>0");
                list.Add((DbParameter)new MySqlParameter("@Title", title.Trim()));
            }
            if (!dirId.IsNullOrWhiteSpace())
            {
                builder.Append(" AND DirId IN(" + dirId + ")");
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND a.WriteTime>=@WriteTime");
                list.Add((DbParameter)new MySqlParameter("@WriteTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND a.WriteTime<@WriteTime1");
                list.Add((DbParameter)new MySqlParameter("@WriteTime1", time2.AddDays(1.0).GetDate()));
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>(("SELECT Id,DirId,DirName,Title,WriteTime,WriteUserName,ReadCount,DocRank FROM RF_Doc a WHERE EXISTS(SELECT Id FROM RF_DocUser WHERE DocId=a.Id AND UserId=@UserId" + ((read >= 0) ? " AND IsRead=@IsRead" : "") + ")") + builder.ToString(), list.ToArray());
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter> GetFieldValueSql(string tableName, string fieldName, string primaryKey, string primaryKeyValue)
        {
            string[] textArray1 = new string[] { "SELECT ", fieldName, " FROM ", tableName, " WHERE ", primaryKey, " = @primarykeyvalue" };
            return new ValueTuple<string, DbParameter>(string.Concat((string[])textArray1), (DbParameter)new MySqlParameter("@primarykeyvalue", primaryKeyValue));
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
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
                list.Add((DbParameter)new MySqlParameter("@FlowId", guid));
            }
            if (!stepName.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(StepName,@StepName)>0");
                list.Add((DbParameter)new MySqlParameter("@StepName", stepName));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,@Title)>0");
                list.Add((DbParameter)new MySqlParameter("@Title", title));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND WriteTime>=@WriteTime");
                list.Add((DbParameter)new MySqlParameter("@WriteTime", time));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND WriteTime<@WriteTime1");
                list.Add((DbParameter)new MySqlParameter("@WriteTime1", time2.AddDays(1.0).ToDateString()));
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,FlowId,FlowName,StepId,StepName,TaskId,GroupId,InstanceId,Title,UserName,WriteTime FROM RF_FlowArchive WHERE 1=1" + builder.ToString(), list.ToArray());
        }

      //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
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
                list.Add((DbParameter)new MySqlParameter("@UserId", guid));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND StartTime>=@StartTime");
                list.Add((DbParameter)new MySqlParameter("@StartTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND StartTime<@StartTime1");
                list.Add((DbParameter)new MySqlParameter("@StartTime1", time2.AddDays(1.0).GetDate()));
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_FlowEntrust WHERE 1=1" + builder.ToString(), list.ToArray());
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFlowSql(List<Guid> flowIdList, string name, string type, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();





            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Name,@Name)>0");
                list.Add((DbParameter)new MySqlParameter("@Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FlowType IN(" + type + ")");
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>(("SELECT Id,Name,CreateDate,CreateUser,Status,Note,SystemId  FROM RF_Flow WHERE Status!=3 AND Id IN(" + flowIdList.JoinSqlIn<Guid>(true) + ")") + builder.ToString(), list.ToArray());
        }

        // [return: TupleElementNames(new string[] { "sql", "parameter" })]






        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFlowSql(List<Guid> flowIdList, string name, string type, string order, int status = -1)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();



          
            if ((flowIdList == null) || (flowIdList.Count <= 0) )
            {
                builder.Append(" AND 1=0");
            }
            else
            {
                builder.Append(" AND Id IN(" + flowIdList.JoinSqlIn<Guid>(true) + ")");
            }
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Name,@Name)>0");
                list.Add(new MySqlParameter("@Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FlowType IN(" + type + ")");
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateDate,CreateUser,Status,Note ,SystemId  FROM RF_Flow WHERE" + ((status != -1) ? (" Status=" + ((int)status).ToString()) : " Status!=3") + builder.ToString(), list.ToArray());
        }



        //public ValueTuple<string, DbParameter[]> GetFormSql(string name, string type, string order)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    List<DbParameter> list = new List<DbParameter>();
        //    if (!name.IsNullOrWhiteSpace())
        //    {
        //        builder.Append(" AND INSTR(Name,@Name)>0");
        //        list.Add((DbParameter)new MySqlParameter("@Name", name.Trim()));
        //    }
        //    if (!type.IsNullOrWhiteSpace())
        //    {
        //        builder.Append(" AND FormType IN(" + type + ")");
        //    }
        //    if (!order.IsNullOrWhiteSpace())
        //    {
        //        builder.Append(" ORDER BY " + order);
        //    }
        //    return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateUserName,CreateDate,EditDate,Status FROM RF_Form WHERE Status!=2" + builder.ToString(), list.ToArray());
        //}

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFormSql(Guid userId, string name, string type, string order, int status = -1)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@ManageUser", userId.ToLowerString())
    };
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Name,@Name)>0");
                list.Add(new MySqlParameter("@Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FormType IN(" + type + ")");
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateUserName,CreateDate,EditDate,Status FROM RF_Form WHERE INSTR(ManageUser,@ManageUser)>0 AND " + ((status == -1) ? "Status!=2" : ("Status=" + ((int)status).ToString())) + builder.ToString(), list.ToArray());
        }




   




        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetHomeSetSql(string name, string title, string type, string order)
        {
            int num;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Name,@Name)>0");
                list.Add((DbParameter)new MySqlParameter("@Name", name.Trim()));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,@Title)>0");
                list.Add((DbParameter)new MySqlParameter("@Title", title.Trim()));
            }
            if (type.IsInt(out num))
            {
                builder.Append(" AND Type=@Type");
                list.Add((DbParameter)new MySqlParameter("@Type", (int)num));
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
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
                builder.Append(" AND INSTR(Title,@Title)>0");
                list.Add((DbParameter)new MySqlParameter("@Title", title.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND Type=@Type");
                list.Add((DbParameter)new MySqlParameter("@Type", type));
            }
            if (userId.IsGuid(out guid))
            {
                builder.Append(" AND UserId=@UserId");
                list.Add((DbParameter)new MySqlParameter("@UserId", guid));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND WriteTime>=@WriteTime");
                list.Add((DbParameter)new MySqlParameter("@WriteTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND WriteTime<@WriteTime1");
                list.Add((DbParameter)new MySqlParameter("@WriteTime1", time2.AddDays(1.0).GetDate()));
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Title,Type,WriteTime,UserName,IPAddress,CityAddress FROM RF_Log WHERE 1=1" + builder.ToString(), list.ToArray());
        }

      //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMessageReadUserListSql(string messageId, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new MySqlParameter("@MessageId", messageId)
        };
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_MessageUser WHERE MessageId=@MessageId" + builder.ToString(), list.ToArray());
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
                list.Add((DbParameter)new MySqlParameter("@UserId", userId));
            }
            else if ("2".Equals(status))
            {
                builder.Append(" AND Exists(SELECT Id From RF_MessageUser WHERE RF_MessageUser.MessageId=RF_Message.Id AND IsRead=1 AND UserId=@UserId)");
                list.Add((DbParameter)new MySqlParameter("@UserId", userId));
            }
            else
            {
                Guid guid;
                if ("0".Equals(status) && userId.IsGuid(out guid))
                {
                    builder.Append(" AND SenderId=@SenderId");
                    list.Add((DbParameter)new MySqlParameter("@SenderId", guid));
                }
            }
            if (!content.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Contents,@Contents)>0");
                list.Add((DbParameter)new MySqlParameter("@Contents", content.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendTime>=@SendTime");
                list.Add((DbParameter)new MySqlParameter("@SendTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendTime<@SendTime1");
                list.Add((DbParameter)new MySqlParameter("@SendTime1", time2.AddDays(1.0).GetDate()));
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
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
            builder1.Append(sql);
            builder1.AppendFormat(") AS PagerTempTable", Array.Empty<object>());
            builder1.AppendFormat(" LIMIT {0},{1}", (int)((number * size) - size), (int)size);
            return builder1.ToString();
        }

        //[return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetProgramSql(string name, string types, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Name,@Name)>0");
                list.Add((DbParameter)new MySqlParameter("@Name", name.Trim()));
            }
            if (!types.IsNullOrWhiteSpace())
            {
                builder.Append(" AND Type IN(" + types + ")");
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,Type,CreateTime,PublishTime,CreateUserId,Status FROM RF_Program WHERE Status IN(0,1)" + builder.ToString(), list.ToArray());
        }


        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryCompletedTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new MySqlParameter("@ReceiveId", userId)
        };
            if (flowId.IsGuid(out guid))
            {
                builder.Append(" AND FlowId=@FlowId");
                list.Add((DbParameter)new MySqlParameter("@FlowId", guid));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,@Title)>0");
                list.Add((DbParameter)new MySqlParameter("@Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND CompletedTime1>=@CompletedTime1");
                list.Add((DbParameter)new MySqlParameter("@CompletedTime1", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND CompletedTime1<@CompletedTime11");
                list.Add((DbParameter)new MySqlParameter("@CompletedTime11", time2.AddDays(1.0).GetDate()));
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,FlowId,FlowName,StepId,StepName,GroupId,InstanceId,Title,SenderId,SenderName,ReceiveTime,CompletedTime,CompletedTime1,Status,ExecuteType,Note  FROM RF_FlowTask WHERE ReceiveId=@ReceiveId AND ExecuteType>1" + builder.ToString(), list.ToArray());
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryGroupMaxTaskSql(Guid gropuId)
        {
            string str = "SELECT * FROM RF_FlowTask WHERE GroupId=@GroupId ORDER BY Sort DESC LIMIT 1";
            MySqlParameter[] parameterArray = new MySqlParameter[] { new MySqlParameter("@GroupId", gropuId) };
            return new ValueTuple<string, DbParameter[]>(str, parameterArray);
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
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
                    list.Add((DbParameter)new MySqlParameter("@FlowId", guid2));
                    goto Label_0073;
                }
                if ( true)
                {
                    builder.Append(" AND FlowId IN(" + flowId + ")");
                    goto Label_0073;
                }
            }
            builder.Append(" AND 1=0");
            Label_0073:
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,@Title)>0");
                list.Add((DbParameter)new MySqlParameter("@Title", title.Trim()));
            }
            if (receiveId.IsGuid(out guid))
            {
                builder.Append(" AND ReceiveId=@ReceiveId");
                list.Add((DbParameter)new MySqlParameter("@ReceiveId", guid));
            }
            if (receiveDate1.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=@ReceiveTime");
                list.Add((DbParameter)new MySqlParameter("@ReceiveTime", time.GetDate()));
            }
            if (receiveDate2.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<@ReceiveTime1");
                list.Add((DbParameter)new MySqlParameter("@ReceiveTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM(SELECT GroupId,MAX(ReceiveTime) ReceiveTime From RF_FlowTask WHERE 1=1 " + builder.ToString() + " GROUP BY GroupId) TaskTemp ORDER BY ReceiveTime", list.ToArray());
        }

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryWaitTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order, int isBatch = 0)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@ReceiveId", userId)
    };
            if (!flowId.IsNullOrWhiteSpace())
            {
                Guid guid;
                if (flowId.IsGuid(out guid))
                {
                    builder.Append(" AND FlowId=@FlowId");
                    list.Add(new MySqlParameter("@FlowId", guid));
                }
                else
                {
                    builder.Append(" AND FlowId IN(" + flowId.ToSqlIn(true) + ")");
                }
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,@Title)>0");
                list.Add(new MySqlParameter("@Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=@ReceiveTime");
                list.Add(new MySqlParameter("@ReceiveTime", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<@ReceiveTime1");
                list.Add(new MySqlParameter("@ReceiveTime1", time2.AddDays(1.0).GetDate()));
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
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
                builder.Append("INSERT INTO " + tableName + "(");
                foreach (KeyValuePair<string, object> pair in dicts)
                {
                    builder.Append(pair.Key);
                    if (!pair.Key.Equals(Enumerable.Last<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>)dicts).Key))
                    {
                        builder.Append(",");
                    }
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
                    list.Add((DbParameter)new MySqlParameter("@" + primaryKey, dicts[primaryKey]));
                }
                else if (1 == flag)
                {
                    string parameterName = "@" + pair3.Key;
                    list.Add((DbParameter)new MySqlParameter(parameterName, pair3.Value));
                    builder.Append("@" + pair3.Key);
                    if (!pair3.Key.Equals(Enumerable.Last<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>)dicts).Key))
                    {
                        builder.Append(",");
                    }
                }
                else if (2 == flag)
                {
                    string introduced7 = "@" + pair3.Key;
                    list.Add((DbParameter)new MySqlParameter(introduced7, pair3.Value));
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
            }
            else if (2 == flag)
            {
                builder.Append(" WHERE  " + primaryKey + "=@" + primaryKey);
            }
            return new ValueTuple<string, DbParameter[]>(builder.ToString(), list.ToArray());
        }

        public string GetTableFieldsSql(string tableName)
        {
            return string.Format("SELECT COLUMN_NAME f_name,DATA_TYPE t_name, CHARACTER_MAXIMUM_LENGTH length ,CASE IS_NULLABLE WHEN 'NO' THEN 0 WHEN 'YES' THEN 1 END is_null,COLUMN_DEFAULT cdefault,0 isidentity,COLUMN_DEFAULT defaultvalue,COLUMN_COMMENT comments FROM information_schema.TABLES a LEFT JOIN information_schema.COLUMNS b ON a.table_name = b.TABLE_NAME WHERE a.TABLE_NAME='{0}' ORDER BY b.ordinal_position", tableName);
        }



      //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetUserFileShareMySql(Guid userId, string fileName, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@UserId", userId),
        new MySqlParameter("@ExpireDate", DateTimeExtensions.Now)
    };
            if (!fileName.IsNullOrEmpty())
            {
                builder.Append(" AND INSTR(FileName,@FileName)>0");
                list.Add((DbParameter)new MySqlParameter("@FileName", fileName));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_UserFileShare WHERE UserId=@UserId AND ExpireDate>@ExpireDate" + builder.ToString(), list.ToArray());
        }


   



     //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetUserFileShareSql(Guid userId, string fileName, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@ShareUserId", userId)
    };
            if (!fileName.IsNullOrEmpty())
            {
                builder.Append(" AND INSTR(FileName,@FileName)>0");
                list.Add((DbParameter)new MySqlParameter("@FileName", fileName));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT FileId,ShareUserId,MAX(FileName) FileName,MAX(ShareDate) ShareDate,MAX(ExpireDate) ExpireDate FROM RF_UserFileShare group by FileId,ShareUserId HAVING ShareUserId=@ShareUserId" + builder.ToString(), list.ToArray());
        }



      ////  [return: TupleElementNames(new string[] { "sql", "parameter" })]
      //  public ValueTuple<string, DbParameter[]> GetFlowSql(List<Guid> flowIdList, string name, string type, string order, int status = -1)
      //  {
      //      StringBuilder builder = new StringBuilder();
      //      List<DbParameter> list = new List<DbParameter>();
      //      if (!name.IsNullOrWhiteSpace())
      //      {
      //          builder.Append(" AND INSTR(Name,@Name)>0");
      //          list.Add((DbParameter)new MySqlParameter("@Name", name.Trim()));
      //      }
      //      if (!type.IsNullOrWhiteSpace())
      //      {
      //          builder.Append(" AND FlowType IN(" + type + ")");
      //      }
      //      if (!order.IsNullOrWhiteSpace())
      //      {
      //          builder.Append(" ORDER BY " + order);
      //      }
      //      return new ValueTuple<string, DbParameter[]>(("SELECT Id,Name,CreateDate,CreateUser,Status,Note FROM RF_Flow WHERE Id IN(" + flowIdList.JoinSqlIn<Guid>(true) + ") " + ((status != -1) ? (" AND Status=" + ((int)status).ToString()) : " AND Status!=3")) + builder.ToString(), list.ToArray());
      //  }




        //[return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFormSql(string name, string type, string order, int status = -1)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Name,@Name)>0");
                list.Add((DbParameter)new MySqlParameter("@Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FormType IN(" + type + ")");
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateUserName,CreateDate,EditDate,Status FROM RF_Form WHERE " + ((status == -1) ? "Status!=2" : ("Status=" + ((int)status).ToString())) + builder.ToString(), list.ToArray());
        }


      //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryEntrustTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@EntrustUserId", userId)
    };
            if (flowId.IsGuid(out guid))
            {
                builder.Append(" AND FlowId=@FlowId");
                list.Add((DbParameter)new MySqlParameter("@FlowId", guid));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,@Title)>0");
                list.Add((DbParameter)new MySqlParameter("@Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=@ReceiveTime");
                list.Add((DbParameter)new MySqlParameter("@ReceiveTime", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<@ReceiveTime1");
                list.Add((DbParameter)new MySqlParameter("@ReceiveTime1", time2.AddDays(1.0).GetDate()));
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,FlowId,FlowName,StepId,StepName,GroupId,InstanceId,Title,SenderName,ReceiveTime,ReceiveName,CompletedTime1,Status,ExecuteType,Note FROM RF_FlowTask WHERE EntrustUserId=@EntrustUserId" + builder.ToString(), list.ToArray());
        }



      //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMailDeletedBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@UserId", currentUserId)
    };
            if (userId.IsGuid(out guid))
            {
                builder.Append(" AND SendUserId=@SendUserId");
                list.Add(new MySqlParameter("@SendUserId", guid));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Subject,@Subject)>0");
                list.Add(new MySqlParameter("@Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendDateTime>=@SendDateTime");
                list.Add(new MySqlParameter("@SendDateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendDateTime<@SendDateTime1");
                list.Add(new MySqlParameter("@SendDateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_MailDeletedBox WHERE UserId=@UserId" + builder.ToString(), list.ToArray());
        }


     //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMailInBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@UserId", currentUserId)
    };
            if (userId.IsGuid(out guid))
            {
                builder.Append(" AND SendUserId=@SendUserId");
                list.Add(new MySqlParameter("@SendUserId", guid));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Subject,@Subject)>0");
                list.Add(new MySqlParameter("@Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendDateTime>=@SendDateTime");
                list.Add(new MySqlParameter("@SendDateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendDateTime<@SendDateTime1");
                list.Add(new MySqlParameter("@SendDateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_MailInBox WHERE UserId=@UserId" + builder.ToString(), list.ToArray());
        }




     //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMailOutBoxSql(Guid currentUserId, string subject, string date1, string date2, int status = -1)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@UserId", currentUserId)
    };
            if (status != -1)
            {
                builder.Append(" AND Status=@Status");
                list.Add(new MySqlParameter("@Status", (int)status));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Subject,@Subject)>0");
                list.Add(new MySqlParameter("@Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendDateTime>=@SendDateTime");
                list.Add(new MySqlParameter("@SendDateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendDateTime<@SendDateTime1");
                list.Add(new MySqlParameter("@SendDateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_MailOutBox WHERE UserId=@UserId" + builder.ToString(), list.ToArray());
        }




        #region 2.8.8新增问卷

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetPartakeUserSql(Guid voteId, string name, string org)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@VoteId", voteId.ToString())
    };
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(UserName,@UserName)>0");
                list.Add(new MySqlParameter("@UserName", name.Trim()));
            }
            if (!org.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(UserOrganize,@UserOrganize)>0");
                list.Add(new MySqlParameter("@UserOrganize", org.Trim()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_VotePartakeUser WHERE VoteId=@VoteId" + builder.ToString(), list.ToArray());
        }




    




        //[return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetVoteResultSql(Guid currentUserId, string topic, string date1, string date2)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@UserId", currentUserId)
    };
            if (!topic.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Topic,@Topic)>0");
                list.Add(new MySqlParameter("@Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND CreateTime>=@CreateTime");
                list.Add(new MySqlParameter("@CreateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND CreateTime<@CreateTime1");
                list.Add(new MySqlParameter("@CreateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_Vote WHERE EXISTS(SELECT Id FROM RF_VoteResultUser WHERE UserId=@UserId AND VoteId=RF_Vote.Id)" + builder.ToString(), list.ToArray());
        }



       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetVoteSql(Guid currentUserId, string topic, string date1, string date2)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@CreateUserId", currentUserId)
    };
            if (!topic.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Topic,@Topic)>0");
                list.Add(new MySqlParameter("@Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND CreateTime>=@CreateTime");
                list.Add(new MySqlParameter("@CreateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND CreateTime<@CreateTime1");
                list.Add(new MySqlParameter("@CreateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_Vote WHERE CreateUserId=@CreateUserId" + builder.ToString(), list.ToArray());
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetWaitSubmitVoteSql(Guid currentUserId, string topic, string date1, string date2)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new MySqlParameter("@UserId", currentUserId)
    };
            if (!topic.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Topic,@Topic)>0");
                list.Add(new MySqlParameter("@Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND CreateTime>=@CreateTime");
                list.Add(new MySqlParameter("@CreateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND CreateTime<@CreateTime1");
                list.Add(new MySqlParameter("@CreateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT * FROM RF_Vote WHERE EXISTS(SELECT Id FROM RF_VotePartakeUser WHERE UserId=@UserId AND VoteId=RF_Vote.Id AND Status=0)" + builder.ToString(), list.ToArray());
        }




        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryMyStartTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string status, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            string str = string.Empty;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            int num = status.ToInt(-1);
            str = "SELECT * FROM(SELECT Id,PrevId,FlowId,FlowName,StepId,GroupId,InstanceId,Title,SenderId,ReceiveTime,(select StepName from RF_FlowTask where GroupId=aaa.GroupId and TaskType!=5 and Status!=2 order by Sort desc LIMIT 0,1) as CurrentStepName FROM RF_FlowTask aaa) bbb WHERE SenderId=@SenderId AND PrevId=@PrevId";
            list.Add(new MySqlParameter("@SenderId", userId));
            list.Add(new MySqlParameter("@PrevId", Guid.Empty));
            if (flowId.IsGuid(out guid))
            {
                builder.Append(" AND FlowId=@FlowId");
                list.Add(new MySqlParameter("@FlowId", guid));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,@Title)>0");
                list.Add(new MySqlParameter("@Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=@ReceiveTime");
                list.Add(new MySqlParameter("@ReceiveTime", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<@ReceiveTime1");
                list.Add(new MySqlParameter("@ReceiveTime1", time2.AddDays(1.0).GetDate()));
            }
            if (!order.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY " + order);
            }
            if (num > -1)
            {
                builder.Append(" AND CurrentStepName " + ((num == 0) ? "IS NOT NULL" : "IS NULL"));
            }
            return new ValueTuple<string, DbParameter[]>(str + builder.ToString(), list.ToArray());
        }












        #endregion

















    }






}
