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
    public class DocUser
    {
        // Methods
        public int Delete(Guid userId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { userId };
                context.Execute("DELETE FROM RF_DocUser WHERE UserId={0}", objects);
                return context.SaveChanges();
            }
        }

        public DataTable GetDocReadPagerList(out int count, int size, int number, string docId, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> docReadUserListSql = sql1.SqlInstance.GetDocReadUserListSql(docId, order);
                string sql = docReadUserListSql.Item1;
                DbParameter[] param = docReadUserListSql.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public bool IsRead(Guid docId, Guid userId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { docId, userId };
                string str = context.ExecuteScalarString("SELECT IsRead FROM RF_DocUser WHERE DocId={0} AND UserId={1}", objects);
                return "1".Equals(str);
            }
        }

        public int UpdateIsRead(Guid docId, Guid userId, int isRead)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { (int)isRead, (1 == isRead) ? new DateTime?(DateTimeExtensions.Now) : null, docId, userId };
                context.Execute("UPDATE RF_DocUser SET IsRead={0},ReadTime={1} WHERE DocId={2} AND UserId={3}", objects);
                return context.SaveChanges();
            }
        }
    }


}
