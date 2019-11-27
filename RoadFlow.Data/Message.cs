using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace RoadFlow.Data
{
    public class Message
    {
        // Methods
        public int Add(RoadFlow.Model.Message message)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.Message>(message);
                return context.SaveChanges();
            }
        }

        public int Add(RoadFlow.Model.Message message, RoadFlow.Model.MessageUser[] messageUsers)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.Message>(message);
                foreach (RoadFlow.Model.MessageUser user in messageUsers)
                {
                    object[] objects = new object[] { user.MessageId, user.UserId, (int)user.IsRead, user.ReadTime };
                    context.Execute("INSERT INTO RF_MessageUser VALUES({0},{1},{2},{3})", objects);
                }
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.Message Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.Message>(objects);
            }
        }

      //  [return: TupleElementNames(new string[] { null, "count" })]
        public ValueTuple<RoadFlow.Model.Message, int> GetNoRead(Guid userId)
        {
            using (DataContext context = new DataContext())
            {
                int num;
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetMessageSendListSql(userId.ToString(), "", "", "", "1", "SendTime DESC");
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, 1, 1, out num, param, "SendTime DESC");
                DataTable dataTable = context.GetDataTable(str2, param);
                RoadFlow.Model.Message message = null;
                if (dataTable.Rows.Count > 0)
                {
                    message = this.Get(dataTable.Rows[0]["Id"].ToString().ToGuid());
                }
                return new ValueTuple<RoadFlow.Model.Message, int>(message, num);
            }
        }

        public DataTable GetSendList(out int count, int size, int number, string userId, string contents, string date1, string date2, string status, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetMessageSendListSql(userId, contents, date1, date2, status, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }
    }


}
