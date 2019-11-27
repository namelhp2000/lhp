using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace RoadFlow.Data
{
    public class MessageUser
    {
        // Methods
        public DataTable GetReadUserList(out int count, int size, int number, string messageId, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> messageReadUserListSql = sql1.SqlInstance.GetMessageReadUserListSql(messageId, order);
                string sql = messageReadUserListSql.Item1;
                DbParameter[] param = messageReadUserListSql.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int UpdateIsRead(Guid messageId, Guid userId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { DateTimeExtensions.Now, messageId, userId };
                context.Execute("UPDATE RF_MessageUser SET IsRead=1,ReadTime={0} WHERE MessageId={1} AND UserId={2}", objects);
                return context.SaveChanges();
            }
        }


        public int DeleteSerd(Guid messageId, Guid userId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] {  messageId, userId };
                context.Execute("Delete from RF_MessageUser  WHERE MessageId={0}  ", objects);
                context.Execute("Delete from RF_Message  WHERE Id={0} and  SenderId={1}", objects);
                return context.SaveChanges();
            }
        }


    }


}
