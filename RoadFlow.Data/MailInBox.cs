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

    public class MailInBox
    {
        // Methods
        public int Add(RoadFlow.Model.MailInBox mailInBox)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.MailInBox>(mailInBox);
                return context.SaveChanges();
            }
        }

        public bool AllNoRead(Guid outBoxId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { outBoxId };
                if (context.GetDataTable("SELECT Id FROM RF_MailInBox WHERE OutBoxId={0} AND IsRead=1", objects).Rows.Count > 0)
                {
                    return false;
                }
                object[] objArray2 = new object[] { outBoxId };
                return (context.GetDataTable("SELECT Id FROM RF_MailDeletedBox WHERE OutBoxId={0} AND IsRead=1", objArray2).Rows.Count == 0);
            }
        }

        public int Delete(RoadFlow.Model.MailInBox mailInBox)
        {
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.MailInBox>(mailInBox);
                return context.SaveChanges();
            }
        }

        public int Delete(Guid id, int status)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                RoadFlow.Model.MailInBox t = context.Find<RoadFlow.Model.MailInBox>(objects);
                if (t != null)
                {
                    if (status == 0)
                    {
                        RoadFlow.Model.MailDeletedBox box2 = new RoadFlow.Model.MailDeletedBox
                        {
                            Id = t.Id,
                            IsRead = t.IsRead,
                            OutBoxId = t.OutBoxId,
                            ReadDateTime = t.ReadDateTime,
                            SendDateTime = t.SendDateTime,
                            SendUserId = t.SendUserId,
                            Subject = t.Subject,
                            SubjectColor = t.SubjectColor,
                            UserId = t.UserId,
                            ContentsId=t.ContentsId
                        };
                        context.Add<RoadFlow.Model.MailDeletedBox>(box2);
                    }
                    context.Remove<RoadFlow.Model.MailInBox>(t);
                }
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.MailInBox Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.MailInBox>(objects);
            }
        }

        public DataTable GetPagerList(out int count, int size, int number, Guid currentUserId, string subject, string userId, string date1, string date2, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetMailInBoxSql(currentUserId, subject, userId, date1, date2);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int Update(RoadFlow.Model.MailInBox mailInBox)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.MailInBox>(mailInBox);
                return context.SaveChanges();
            }
        }

        public int UpdateIsRead(Guid id, int status, bool isUpdateDate = false)
        {
            using (DataContext context = new DataContext())
            {
                if (isUpdateDate)
                {
                    object[] objects = new object[] { (int)status, DateTimeExtensions.Now, id };
                    context.Execute("UPDATE RF_MailInBox SET IsRead={0},ReadDateTime={1} WHERE Id={2}", objects);
                }
                else
                {
                    object[] objArray2 = new object[] { (int)status, id };
                    context.Execute("UPDATE RF_MailInBox SET IsRead={0} WHERE Id={1}", objArray2);
                }
                return context.SaveChanges();
            }
        }
    }


  




    #endregion

}
