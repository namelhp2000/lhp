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
    #region 流程2.8.5

    public class MailDeletedBox
    {
        // Methods
        public int Add(RoadFlow.Model.MailDeletedBox mailDeletedBox)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.MailDeletedBox>(mailDeletedBox);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.MailDeletedBox mailDeletedBox)
        {
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.MailDeletedBox>(mailDeletedBox);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.MailDeletedBox Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.MailDeletedBox>(objects);
            }
        }

        public DataTable GetPagerList(out int count, int size, int number, Guid currentUserId, string subject, string userId, string date1, string date2, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetMailDeletedBoxSql(currentUserId, subject, userId, date1, date2);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int Recovery(RoadFlow.Model.MailDeletedBox mailDeletedBox)
        {
            using (DataContext context = new DataContext())
            {
                RoadFlow.Model.MailInBox t = new RoadFlow.Model.MailInBox
                {
                    ContentsId = mailDeletedBox.ContentsId,
                    Id = mailDeletedBox.Id,
                    IsRead = mailDeletedBox.IsRead,
                    OutBoxId = mailDeletedBox.OutBoxId,
                    ReadDateTime = mailDeletedBox.ReadDateTime,
                    SendDateTime = mailDeletedBox.SendDateTime,
                    SendUserId = mailDeletedBox.SendUserId,
                    Subject = mailDeletedBox.Subject,
                    SubjectColor = mailDeletedBox.SubjectColor,
                    UserId = mailDeletedBox.UserId
                };
                context.Add<RoadFlow.Model.MailInBox>(t);
                context.Remove<RoadFlow.Model.MailDeletedBox>(mailDeletedBox);
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.MailDeletedBox mailDeletedBox)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.MailDeletedBox>(mailDeletedBox);
                return context.SaveChanges();
            }
        }
    }


  



    #endregion

}
