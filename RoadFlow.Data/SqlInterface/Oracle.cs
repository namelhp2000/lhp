using Oracle.ManagedDataAccess.Client;
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
    public class Oracle : ISql
    {
        // Fields
        private readonly string connStr;
        private readonly RoadFlow.Model.DbConnection dbConnectionModel;
        private readonly string dbType;

        // Methods
        public Oracle(RoadFlow.Model.DbConnection dbConnection, string dataBaseType)
        {
            this.dbType = dataBaseType.ToLower();
            if (dbConnection != null)
            {
                this.connStr = dbConnection.ConnString;
                this.dbConnectionModel = dbConnection;
            }
        }
    
       // [return: System.Runtime.CompilerServices.TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetApplibrarySql(string title, string address, string typeId, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(:Title,Title)>0");
                list.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (!address.IsNullOrWhiteSpace())
            {
                builder.Append(" AND CHARINDEX(:Address,Address)>0");
                list.Add(new OracleParameter(":Address", address.Trim()));
            }
            if (!typeId.IsNullOrWhiteSpace())
            {
                builder.Append(" AND Type IN(" + typeId.ToUpper() + ")");
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Title,Address,Type FROM RF_AppLibrary WHERE 1=1" + builder.ToString(), list.ToArray());
        }

        public DbParameter GetDbParameter(string name, object value)
        {
            return new OracleParameter(name, Common.GetParameterValue(value));
        }

        public string GetDbTablesSql()
        {
            return "select a.TABLE_NAME,b.COMMENTS  from user_tables a,user_tab_comments b WHERE a.TABLE_NAME=b.TABLE_NAME and a.TABLE_NAME not like '%$%' and a.TABLE_NAME not like 'LOGMNR%' and a.TABLE_NAME not like 'HELP%' and a.TABLE_NAME not like 'SPRING%' and a.TABLE_NAME not like 'SQLPLUS%' order by TABLE_NAME";
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetDocReadUserListSql(string docId, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new OracleParameter(":DocId", docId.ToUpper())
        };
            return new ValueTuple<string, DbParameter[]>("SELECT DOCID,USERID,ISREAD,READTIME FROM RF_DocUser WHERE DocId=:DocId" + builder.ToString(), list.ToArray());
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetDocSql(Guid userId, string title, string dirId, string date1, string date2, string order, int read)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new OracleParameter(":UserId", userId.ToUpperString())
        };
            if (read >= 0)
            {
                list.Add(new OracleParameter(":IsRead", (int)read));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,:Title,1,1)>0");
                list.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (!dirId.IsNullOrWhiteSpace())
            {
                builder.Append(" AND DirId IN(" + dirId.ToUpper() + ")");
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND a.WriteTime>=:WriteTime");
                list.Add(new OracleParameter(":WriteTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND a.WriteTime<:WriteTime1");
                list.Add(new OracleParameter(":WriteTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>(("SELECT Id,DirId,DirName,Title,WriteTime,WriteUserName,ReadCount,DocRank FROM RF_Doc a WHERE EXISTS(SELECT Id FROM RF_DocUser WHERE DocId=a.Id AND UserId=:UserId" + ((read >= 0) ? " AND IsRead=:IsRead" : "") + ")") + builder.ToString(), list.ToArray());
        }

      //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter> GetFieldValueSql(string tableName, string fieldName, string primaryKey, string primaryKeyValue)
        {
            string[] textArray1 = new string[] { "SELECT ", fieldName, " FROM ", tableName, " WHERE ", primaryKey, "=:primarykeyvalue" };
            return new ValueTuple<string, DbParameter>(string.Concat((string[])textArray1), new OracleParameter(":primarykeyvalue", primaryKeyValue));
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
                builder.Append(" AND FlowId=:FlowId");
                list.Add(new OracleParameter(":FlowId", flowId.ToUpper()));
            }
            if (!stepName.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(StepName,:StepName,1,1)>0");
                list.Add(new OracleParameter(":StepName", stepName));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,:Title,1,1)>0");
                list.Add(new OracleParameter(":Title", title));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND WriteTime>=:WriteTime");
                list.Add(new OracleParameter(":WriteTime", time));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND WriteTime<:WriteTime1");
                list.Add(new OracleParameter(":WriteTime1", time2.AddDays(1.0).ToDateString()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,FlowId,FlowName,StepId,StepName,TaskId,GroupId,InstanceId,Title,UserName,WriteTime FROM RF_FlowArchive WHERE 1=1" + builder.ToString(), list.ToArray());
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFlowEntrustSql(string userId, string date1, string date2, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (userId.IsGuid(out guid))
            {
                builder.Append(" AND UserId=:UserId");
                list.Add(new OracleParameter(":UserId", userId.ToUpper()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND StartTime>=:StartTime");
                list.Add(new OracleParameter(":StartTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND StartTime<:StartTime1");
                list.Add(new OracleParameter(":StartTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,UserId,StartTime,EndTime,FlowId,ToUserId,WriteTime,Note FROM RF_FlowEntrust WHERE 1=1" + builder.ToString(), list.ToArray());
        }

        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFlowSql(List<Guid> flowIdList, string name, string type, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Name,:Name,1,1)>0");
                list.Add(new OracleParameter(":Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FlowType IN(" + type.ToUpper() + ")");
            }
            return new ValueTuple<string, DbParameter[]>(("SELECT Id,Name,CreateDate,CreateUser,Status,Note,SystemId  FROM RF_Flow WHERE Status!=3 AND Id IN(" + flowIdList.JoinSqlIn<Guid>(true).ToUpper() + ")") + builder.ToString(), list.ToArray());
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
                builder.Append(" AND INSTR(Name,:Name,1,1)>0");
                list.Add(new OracleParameter(":Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FlowType IN(" + type.ToUpper() + ")");
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateDate,CreateUser,Status,Note ,SystemId FROM RF_Flow WHERE" + ((status != -1) ? (" Status=" + ((int)status).ToString()) : " Status!=3") + builder.ToString(), list.ToArray());
        }










        ////  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        //public ValueTuple<string, DbParameter[]> GetFormSql(string name, string type, string order)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    List<DbParameter> list = new List<DbParameter>();
        //    if (!name.IsNullOrWhiteSpace())
        //    {
        //        builder.Append(" AND INSTR(Name,:Name,1,1)>0");
        //        list.Add(new OracleParameter(":Name", name.Trim()));
        //    }
        //    if (!type.IsNullOrWhiteSpace())
        //    {
        //        builder.Append(" AND FormType IN(" + type.ToUpper() + ")");
        //    }
        //    return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateUserName,CreateDate,EditDate,Status FROM RF_Form WHERE Status!=2" + builder.ToString(), list.ToArray());
        //}


      //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFormSql(Guid userId, string name, string type, string order, int status = -1)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":MANAGEUSER", userId.ToLowerString())
    };
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Name,:Name,1,1)>0");
                list.Add(new OracleParameter(":Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FormType IN(" + type.ToUpper() + ")");
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateUserName,CreateDate,EditDate,Status FROM RF_Form WHERE INSTR(MANAGEUSER,:MANAGEUSER,1,1)>0 AND " + ((status == -1) ? "Status!=2" : ("Status=" + ((int)status).ToString())) + builder.ToString(), list.ToArray());
        }




      




        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetHomeSetSql(string name, string title, string type, string order)
        {
            int num;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Name,:Name,1,1)>0");
                list.Add(new OracleParameter(":Name", name.Trim()));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,:Title,1,1)>0");
                list.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (type.IsInt(out num))
            {
                builder.Append(" AND Type=:Type");
                list.Add(new OracleParameter(":Type", (int)num));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Type,Name,Title,DataSourceType,DataSource,Ico,BackgroundColor,FontColor,DbConnId,LinkURL,UseOrganizes,UseUsers,Sort,Note FROM RF_HomeSet WHERE 1=1" + builder.ToString(), list.ToArray());
        }

        public string GetIdentitySql(string seqName = "")
        {
            return ("SELECT " + seqName + ".CURRVAL FROM DUAL");
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetLogSql(string title, string type, string userId, string date1, string date2, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,:Title,1,1)>0");
                list.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND Type=:Type");
                list.Add(new OracleParameter(":Type", type));
            }
            if (userId.IsGuid(out guid))
            {
                builder.Append(" AND UserId=:UserId");
                list.Add(new OracleParameter(":UserId", userId.ToUpper()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND WriteTime>=:WriteTime");
                list.Add(new OracleParameter(":WriteTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND WriteTime<:WriteTime1");
                list.Add(new OracleParameter(":WriteTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Title,Type,WriteTime,UserName,IPAddress,CityAddress FROM RF_Log WHERE 1=1" + builder.ToString(), list.ToArray());
        }

      //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMessageReadUserListSql(string messageId, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new OracleParameter(":MessageId", messageId.ToUpper())
        };
            return new ValueTuple<string, DbParameter[]>("SELECT MessageId,UserId,IsRead,ReadTime FROM RF_MessageUser WHERE MessageId=:MessageId" + builder.ToString(), list.ToArray());
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
                builder.Append(" AND Exists(SELECT Id From RF_MessageUser WHERE RF_MessageUser.MessageId=RF_Message.Id AND IsRead=0 AND UserId=:UserId)");
                list.Add(new OracleParameter(":UserId", userId.ToUpper()));
            }
            else if ("2".Equals(status))
            {
                builder.Append(" AND Exists(SELECT Id From RF_MessageUser WHERE RF_MessageUser.MessageId=RF_Message.Id AND IsRead=1 AND UserId=:UserId)");
                list.Add(new OracleParameter(":UserId", userId.ToUpper()));
            }
            else
            {
                Guid guid;
                if ("0".Equals(status) && userId.IsGuid(out guid))
                {
                    builder.Append(" AND SenderId=:SenderId");
                    list.Add(new OracleParameter(":SenderId", userId.ToUpper()));
                }
            }
            if (!content.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Contents,:Contents,1,1)>0");
                list.Add(new OracleParameter(":Contents", content.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendTime>=:SendTime");
                list.Add(new OracleParameter(":SendTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendTime<:SendTime1");
                list.Add(new OracleParameter(":SendTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Contents,SendType,SenderName,SendTime,Files FROM RF_Message WHERE 1=1" + builder.ToString(), list.ToArray());
        }

        public string GetPaerSql(string sql, int size, int number, out int count, DbParameter[] param = null, string order = "")
        {
            int num;
            string str = string.Empty;
            using (DataContext context = (this.dbConnectionModel == null) ? new DataContext() : new DataContext(this.dbConnectionModel.ConnType, this.dbConnectionModel.ConnString, true))
            {
                str = context.ExecuteScalarString(string.Format("SELECT COUNT(*) FROM ({0}) PagerCountTemp", sql), param);
            }
            count = str.IsInt(out num) ? num : 0;
            if (count < (((number * size) - size) + 1))
            {
                number = 1;
            }
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("SELECT * FROM (");
            if (!sql.ContainsIgnoreCase("ROW_NUMBER()") && !sql.ContainsIgnoreCase("ROWNUM"))
            {
                sql = sql.Insert(sql.IndexOfIgnoreCase("from") - 1, ",ROW_NUMBER() OVER(ORDER BY " + order + ") AS PagerAutoRowNumber");
            }
            builder1.Append(sql);
            builder1.AppendFormat(") PagerTempTable", Array.Empty<object>());
            builder1.AppendFormat(" WHERE PagerAutoRowNumber BETWEEN {0} AND {1}", (int)(((number * size) - size) + 1), (int)(number * size));
            return builder1.ToString();
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetProgramSql(string name, string types, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Name,:Name,1,1)>0");
                list.Add(new OracleParameter(":Name", name.Trim()));
            }
            if (!types.IsNullOrWhiteSpace())
            {
                builder.Append(" AND Type IN(" + types.ToUpper() + ")");
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,Type,CreateTime,PublishTime,CreateUserId,Status FROM RF_Program WHERE Status IN(0,1)" + builder.ToString(), list.ToArray());
        }


        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryCompletedTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
            new OracleParameter(":ReceiveId", userId.ToUpperString())
        };
            if (flowId.IsGuid(out guid))
            {
                builder.Append(" AND FlowId=:FlowId");
                list.Add(new OracleParameter(":FlowId", flowId.ToUpper()));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,:Title,1,1)>0");
                list.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND CompletedTime1>=:CompletedTime1");
                list.Add(new OracleParameter(":CompletedTime1", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND CompletedTime1<:CompletedTime11");
                list.Add(new OracleParameter(":CompletedTime11", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,FlowId,FlowName,StepId,StepName,GroupId,InstanceId,Title,SenderId,SenderName,ReceiveTime,CompletedTime,CompletedTime1,Status,ExecuteType,Note FROM RF_FlowTask WHERE ReceiveId=:ReceiveId AND ExecuteType>1" + builder.ToString(), list.ToArray());
        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryGroupMaxTaskSql(Guid gropuId)
        {
            string str = "SELECT * FROM RF_FlowTask WHERE GroupId=:GroupId AND ROWNUM=1 ORDER BY Sort DESC";
            OracleParameter[] parameterArray = new OracleParameter[] { new OracleParameter("@GroupId", gropuId) };
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
                    builder.Append(" AND FlowId=:FlowId");
                    list.Add(new OracleParameter(":FlowId", flowId.ToUpper()));
                    goto Label_0077;
                }
                if ( true)
                {
                    builder.Append(" AND FlowId IN(" + flowId.ToUpper() + ")");
                    goto Label_0077;
                }
            }
            builder.Append(" AND 1=0");
            Label_0077:
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,:Title,1,1)>0");
                list.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (receiveId.IsGuid(out guid))
            {
                builder.Append(" AND ReceiveId=:ReceiveId");
                list.Add(new OracleParameter(":ReceiveId", receiveId.ToUpper()));
            }
            if (receiveDate1.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=:ReceiveTime");
                list.Add(new OracleParameter(":ReceiveTime", time.GetDate()));
            }
            if (receiveDate2.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<:ReceiveTime1");
                list.Add(new OracleParameter(":ReceiveTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT GroupId,ReceiveTime,ROW_NUMBER() OVER(ORDER BY ReceiveTime DESC) AS PagerAutoRowNumber FROM(SELECT GroupId,MAX(ReceiveTime) ReceiveTime From RF_FlowTask WHERE 1=1 " + builder.ToString() + " GROUP BY GroupId) TaskTemp", list.ToArray());
        }

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryWaitTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order, int isBatch = 0)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":ReceiveId", userId.ToUpperString())
    };
            if (!flowId.IsNullOrWhiteSpace())
            {
                Guid guid;
                if (flowId.IsGuid(out guid))
                {
                    builder.Append(" AND FlowId=:FlowId");
                    list.Add(new OracleParameter(":FlowId", guid));
                }
                else
                {
                    builder.Append(" AND FlowId IN(" + flowId.ToSqlIn(true) + ")");
                }
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,:Title,1,1)>0");
                list.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=:ReceiveTime");
                list.Add(new OracleParameter(":ReceiveTime", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<:ReceiveTime1");
                list.Add(new OracleParameter(":ReceiveTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>(("SELECT Id,FlowId,FlowName,StepId,StepName,InstanceId,GroupId,TaskType,Title,SenderId,SenderName,ReceiveTime,CompletedTime,Status,Note,IsBatch FROM RF_FlowTask WHERE ReceiveId=:ReceiveId" + ((isBatch == 0) ? "" : " AND IsBatch=1") + " AND Status IN(0,1)") + builder.ToString(), list.ToArray());
        }

        // [return: TupleElementNames(new string[] { "sql", "parameter" })]
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
                    builder.Append(" WHERE " + primaryKey + "=:" + primaryKey);
                    list.Add(new OracleParameter(":" + primaryKey, Common.GetParameterValue(dicts[primaryKey])));
                }
                else if (1 == flag)
                {
                    string introduced9 = ":" + pair3.Key;
                    list.Add(new OracleParameter(introduced9, Common.GetParameterValue(pair3.Value)));
                    builder.Append(":" + pair3.Key);
                    if (!pair3.Key.Equals(Enumerable.Last<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>)dicts).Key))
                    {
                        builder.Append(",");
                    }
                }
                else if ((2 == flag) && !pair3.Key.EqualsIgnoreCase(primaryKey))
                {
                    builder.Append(pair3.Key + "=:" + pair3.Key);
                    string introduced10 = ":" + pair3.Key;
                    list.Add(new OracleParameter(introduced10, Common.GetParameterValue(pair3.Value)));
                    if (!pair3.Key.Equals(Enumerable.Last<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>)dicts).Key))
                    {
                        builder.Append(",");
                    }
                }
            }
            if (1 == flag)
            {
                builder.Append(")");
            }
            else if (2 == flag)
            {
                builder.Append(" WHERE  " + primaryKey + "=:" + primaryKey);
                IEnumerable<KeyValuePair<string, object>> enumerable = from p in (IEnumerable<KeyValuePair<string, object>>)dicts select p;
                object obj2 = Enumerable.Any<KeyValuePair<string, object>>(enumerable) ? Enumerable.First<KeyValuePair<string, object>>(enumerable).Value : null;
                list.Add(new OracleParameter(":" + primaryKey, Common.GetParameterValue(obj2)));
            }
            return new ValueTuple<string, DbParameter[]>(builder.ToString(), list.ToArray());
        }

        public string GetTableFieldsSql(string tableName)
        {
            return string.Format("SELECT user_tab_columns.COLUMN_NAME as f_name,user_tab_columns.DATA_TYPE as t_name,user_tab_columns.CHAR_LENGTH AS length,CASE user_tab_columns.NULLABLE WHEN 'Y' THEN 1 WHEN 'N' THEN 0 END AS is_null,user_tab_columns.DATA_DEFAULT AS cdefault,0 as isidentity,user_tab_columns.DATA_DEFAULT AS defaultvalue,user_col_comments.comments FROM user_tab_columns,user_col_comments WHERE user_tab_columns.COLUMN_NAME=user_col_comments.COLUMN_NAME and user_tab_columns.TABLE_NAME=user_col_comments.TABLE_NAME and UPPER(user_tab_columns.TABLE_NAME)=UPPER('{0}') and UPPER(user_col_comments.TABLE_NAME)=UPPER('{0}') ORDER BY user_tab_columns.COLUMN_ID", tableName);
        }

        public ValueTuple<string, DbParameter[]> GetUserFileShareMySql(Guid userId, string fileName, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":UserId", userId.ToUpperString()),
        new OracleParameter(":ExpireDate", DateTimeExtensions.Now)
    };
            if (!fileName.IsNullOrEmpty())
            {
                builder.Append(" AND INSTR(FileName,:FileName,1,1)>0");
                list.Add(new OracleParameter("@:FileName", fileName));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT FileId,ShareUserId,FileName,ShareDate,ExpireDate FROM RF_UserFileShare WHERE UserId=:UserId AND ExpireDate>:ExpireDate" + builder.ToString(), list.ToArray());

        }

        public ValueTuple<string, DbParameter[]> GetUserFileShareSql(Guid userId, string fileName, string order)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":ShareUserId", userId.ToUpperString())
    };
            if (!fileName.IsNullOrEmpty())
            {
                builder.Append(" WHERE INSTR(FileName,:FileName,1,1)>0");
                list.Add(new OracleParameter(":FileName", fileName));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT FileId,ShareUserId,FileName,ShareDate,ExpireDate FROM (SELECT FileId,ShareUserId,MAX(FileName) FileName,MAX(ShareDate) ShareDate,MAX(ExpireDate) ExpireDate FROM RF_UserFileShare group by FileId,ShareUserId HAVING ShareUserId=:ShareUserId) Temp_RF_UserFileShare" + builder.ToString(), list.ToArray());

        }

       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        //public ValueTuple<string, DbParameter[]> GetFlowSql(List<Guid> flowIdList, string name, string type, string order, int status = -1)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    List<DbParameter> list = new List<DbParameter>();
        //    if (!name.IsNullOrWhiteSpace())
        //    {
        //        builder.Append(" AND INSTR(Name,:Name,1,1)>0");
        //        list.Add(new OracleParameter(":Name", name.Trim()));
        //    }
        //    if (!type.IsNullOrWhiteSpace())
        //    {
        //        builder.Append(" AND FlowType IN(" + type.ToUpper() + ")");
        //    }
        //    return new ValueTuple<string, DbParameter[]>(("SELECT Id,Name,CreateDate,CreateUser,Status,Note FROM RF_Flow WHERE Id IN(" + flowIdList.JoinSqlIn<Guid>(true).ToUpper() + ")" + ((status != -1) ? (" AND Status=" + ((int)status).ToString()) : " AND Status!=3")) + builder.ToString(), list.ToArray());
        //}




     //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetFormSql(string name, string type, string order, int status = -1)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Name,:Name,1,1)>0");
                list.Add(new OracleParameter(":Name", name.Trim()));
            }
            if (!type.IsNullOrWhiteSpace())
            {
                builder.Append(" AND FormType IN(" + type.ToUpper() + ")");
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,Name,CreateUserName,CreateDate,EditDate,Status FROM RF_Form WHERE " + ((status == -1) ? "Status!=2" : ("Status=" + ((int)status).ToString())) + builder.ToString(), list.ToArray());
        }



       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetQueryEntrustTaskSql(Guid userId, string flowId, string title, string startDate, string endDate, string order)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":EntrustUserId", userId.ToUpperString())
    };
            if (flowId.IsGuid(out guid))
            {
                builder.Append(" AND FlowId=:FlowId");
                list.Add(new OracleParameter(":FlowId", flowId.ToUpper()));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,:Title,1,1)>0");
                list.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=:ReceiveTime");
                list.Add(new OracleParameter(":ReceiveTime", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<:ReceiveTime1");
                list.Add(new OracleParameter(":ReceiveTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT Id,FlowId,FlowName,StepId,StepName,GroupId,InstanceId,Title,SenderName,ReceiveTime,ReceiveName,CompletedTime1,Status,ExecuteType,Note FROM RF_FlowTask WHERE EntrustUserId=:EntrustUserId" + builder.ToString(), list.ToArray());
        }


        #region 2.8.5新增

        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMailDeletedBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":UserId", currentUserId.ToUpperString())
    };
            if (userId.IsGuid(out guid))
            {
                builder.Append(" AND SendUserId=:SendUserId");
                list.Add(new OracleParameter(":SendUserId", guid.ToUpperString()));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                builder.Append(" WHERE INSTR(Subject,:Subject,1,1)>0");
                list.Add(new OracleParameter(":Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendDateTime>=:SendDateTime");
                list.Add(new OracleParameter(":SendDateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendDateTime<:SendDateTime1");
                list.Add(new OracleParameter(":SendDateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT ID,SUBJECT,SUBJECTCOLOR,USERID,SENDUSERID,SENDDATETIME,CONTENTSID,ISREAD,READDATETIME,OUTBOXID FROM RF_MailDeletedBox WHERE UserId=:UserId" + builder.ToString(), list.ToArray());
        }



      //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMailInBoxSql(Guid currentUserId, string subject, string userId, string date1, string date2)
        {
            Guid guid;
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":UserId", currentUserId.ToUpperString())
    };
            if (userId.IsGuid(out guid))
            {
                builder.Append(" AND SendUserId=:SendUserId");
                list.Add(new OracleParameter(":SendUserId", guid.ToUpperString()));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                builder.Append(" WHERE INSTR(Subject,:Subject,1,1)>0");
                list.Add(new OracleParameter(":Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendDateTime>=:SendDateTime");
                list.Add(new OracleParameter(":SendDateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendDateTime<:SendDateTime1");
                list.Add(new OracleParameter(":SendDateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT ID,SUBJECT,SUBJECTCOLOR,USERID,SENDUSERID,SENDDATETIME,CONTENTSID,ISREAD,READDATETIME,OUTBOXID FROM RF_MailInBox WHERE UserId=:UserId" + builder.ToString(), list.ToArray());
        }



       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetMailOutBoxSql(Guid currentUserId, string subject, string date1, string date2, int status = -1)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":UserId", currentUserId.ToUpperString())
    };
            if (status != -1)
            {
                builder.Append(" AND Status=:Status");
                list.Add(new OracleParameter(":Status", (int)status));
            }
            if (!subject.IsNullOrWhiteSpace())
            {
                builder.Append(" WHERE INSTR(Subject,:Subject,1,1)>0");
                list.Add(new OracleParameter(":Subject", subject.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND SendDateTime>=:SendDateTime");
                list.Add(new OracleParameter(":SendDateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND SendDateTime<:SendDateTime1");
                list.Add(new OracleParameter(":SendDateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT ID,SUBJECT,SUBJECTCOLOR,USERID,RECEIVEUSERS,SENDDATETIME,CONTENTSID,STATUS FROM RF_MailOutBox WHERE UserId=:UserId" + builder.ToString(), list.ToArray());
        }


        #endregion


        #region 2.8.8新增问卷


     //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetPartakeUserSql(Guid voteId, string name, string org)
        {
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":VoteId", voteId.ToUpperString())
    };
            if (!name.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Topic,:UserName,1,1)>0");
                list.Add(new OracleParameter(":UserName", name.Trim()));
            }
            if (!org.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Topic,:UserOrganize,1,1)>0");
                list.Add(new OracleParameter(":UserOrganize", org.Trim()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT ID,VOTEID,USERID,USERNAME,USERORGANIZE,STATUS,SUBMITTIME FROM RF_VotePartakeUser WHERE VoteId=:VoteId" + builder.ToString(), list.ToArray());
        }





        //  [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetVoteResultSql(Guid currentUserId, string topic, string date1, string date2)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":UserId", currentUserId.ToUpperString())
    };
            if (!topic.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Topic,:Topic,1,1)>0");
                list.Add(new OracleParameter(":Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND CreateTime>=:CreateTime");
                list.Add(new OracleParameter(":CreateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND CreateTime<:CreateTime1");
                list.Add(new OracleParameter(":CreateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT ID,TOPIC,CREATEUSERID,CREATETIME,PARTAKEUSERS,RESULTVIEWUSERS,ENDTIME,NOTE,STATUS FROM RF_Vote WHERE EXISTS(SELECT Id FROM RF_VoteResultUser WHERE UserId=:UserId AND VoteId=RF_Vote.Id)" + builder.ToString(), list.ToArray());
        }


     //   [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetVoteSql(Guid currentUserId, string topic, string date1, string date2)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":CreateUserId", currentUserId.ToUpperString())
    };
            if (!topic.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Topic,:Topic,1,1)>0");
                list.Add(new OracleParameter(":Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND CreateTime>=:CreateTime");
                list.Add(new OracleParameter(":CreateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND CreateTime<:CreateTime1");
                list.Add(new OracleParameter(":CreateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT ID,TOPIC,CREATEUSERID,CREATETIME,PARTAKEUSERS,RESULTVIEWUSERS,ENDTIME,NOTE,STATUS FROM RF_Vote WHERE CreateUserId=:CreateUserId" + builder.ToString(), list.ToArray());
        }


       // [return: TupleElementNames(new string[] { "sql", "parameter" })]
        public ValueTuple<string, DbParameter[]> GetWaitSubmitVoteSql(Guid currentUserId, string topic, string date1, string date2)
        {
            DateTime time;
            DateTime time2;
            StringBuilder builder = new StringBuilder();
            List<DbParameter> list = new List<DbParameter> {
        new OracleParameter(":UserId", currentUserId.ToUpperString())
    };
            if (!topic.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Topic,:Topic,1,1)>0");
                list.Add(new OracleParameter(":Topic", topic.Trim()));
            }
            if (date1.IsDateTime(out time))
            {
                builder.Append(" AND CreateTime>=:CreateTime");
                list.Add(new OracleParameter(":CreateTime", time.GetDate()));
            }
            if (date2.IsDateTime(out time2))
            {
                builder.Append(" AND CreateTime<:CreateTime1");
                list.Add(new OracleParameter(":CreateTime1", time2.AddDays(1.0).GetDate()));
            }
            return new ValueTuple<string, DbParameter[]>("SELECT ID,TOPIC,CREATEUSERID,CREATETIME,PARTAKEUSERS,RESULTVIEWUSERS,ENDTIME,NOTE,STATUS FROM RF_Vote WHERE EXISTS(SELECT Id FROM RF_VotePartakeUser WHERE UserId=:UserId AND VoteId=RF_Vote.Id AND Status=0)" + builder.ToString(), list.ToArray());
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
            str = "SELECT Id,PrevId,FlowId,FlowName,StepId,GroupId,InstanceId,Title,SenderId,ReceiveTime,CurrentStepName FROM (SELECT Id,PrevId,FlowId,FlowName,StepId,GroupId,InstanceId,Title,SenderId,ReceiveTime,(select StepName from RF_FlowTask where GroupId=aaa.GroupId and TaskType!=5 and Status!=2) as CurrentStepName FROM RF_FlowTask aaa) bbb WHERE SenderId=:SenderId AND PrevId=:PrevId";
            list.Add(new OracleParameter(":SenderId", userId.ToUpperString()));
            list.Add(new OracleParameter(":PrevId", Guid.Empty.ToUpperString()));
            if (flowId.IsGuid(out guid))
            {
                builder.Append(" AND FlowId=:FlowId");
                list.Add(new OracleParameter(":FlowId", flowId.ToUpper()));
            }
            if (!title.IsNullOrWhiteSpace())
            {
                builder.Append(" AND INSTR(Title,:Title,1,1)>0");
                list.Add(new OracleParameter(":Title", title.Trim()));
            }
            if (startDate.IsDateTime(out time))
            {
                builder.Append(" AND ReceiveTime>=:ReceiveTime");
                list.Add(new OracleParameter(":ReceiveTime", time.GetDate()));
            }
            if (endDate.IsDateTime(out time2))
            {
                builder.Append(" AND ReceiveTime<:ReceiveTime1");
                list.Add(new OracleParameter(":ReceiveTime1", time2.AddDays(1.0).GetDate()));
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
