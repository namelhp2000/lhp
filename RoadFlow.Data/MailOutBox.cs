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


    public class MailOutBox
    {
        // Methods
        public int Add(RoadFlow.Model.MailOutBox mailOutBox)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.MailOutBox>(mailOutBox);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.MailOutBox mailOutBox)
        {
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.MailOutBox>(mailOutBox);
                return context.SaveChanges();
            }
        }

        public int Delete(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                context.Execute("DELETE RF_MailOutBox WHERE Id={0}", objects);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.MailOutBox Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.MailOutBox>(objects);
            }
        }

        public DataTable GetPagerList(out int count, int size, int number, Guid currentUserId, string subject, string date1, string date2, string order, int status = -1)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetMailOutBoxSql(currentUserId, subject, date1, date2, status);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }


        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="mailOutBox"></param>
        /// <param name="mailContent"></param>
        /// <param name="receiveUsers"></param>
        /// <param name="carbonCopyUsers"></param>
        /// <param name="secretCopyUsers"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public int Send(RoadFlow.Model.MailOutBox mailOutBox, RoadFlow.Model.MailContent mailContent, List<RoadFlow.Model.User> receiveUsers, List<RoadFlow.Model.User> carbonCopyUsers, List<RoadFlow.Model.User> secretCopyUsers, bool isAdd)
        {
            using (DataContext context = new DataContext())
            {
                int num = 0;
                if (isAdd)
                {
                    num += context.Add<RoadFlow.Model.MailOutBox>(mailOutBox);
                }
                else
                {
                    num += context.Update<RoadFlow.Model.MailOutBox>(mailOutBox);
                    context.Remove<RoadFlow.Model.MailContent>(mailContent);
                }
                context.Add<RoadFlow.Model.MailContent>(mailContent);
                if (mailOutBox.Status == 1)
                {
                    List<RoadFlow.Model.MailInBox> list = new List<RoadFlow.Model.MailInBox>();
                    foreach (RoadFlow.Model.User user in receiveUsers)
                    {
                        RoadFlow.Model.MailInBox box = new RoadFlow.Model.MailInBox
                        {
                            ContentsId = mailContent.Id,
                            Id = Guid.NewGuid(),
                            IsRead = 0,
                            SendDateTime = mailOutBox.SendDateTime,
                            SendUserId = mailOutBox.UserId,
                            Subject = mailOutBox.Subject,
                            SubjectColor = mailOutBox.SubjectColor,
                            UserId = user.Id,
                            OutBoxId = mailOutBox.Id,
                            MailType = 1

                        };
                        list.Add(box);
                    }
                    foreach (RoadFlow.Model.User user2 in carbonCopyUsers)
                    {
                        RoadFlow.Model.MailInBox box2 = new RoadFlow.Model.MailInBox
                        {
                            ContentsId = mailContent.Id,
                            Id = Guid.NewGuid(),
                            IsRead = 0,
                            SendDateTime = mailOutBox.SendDateTime,
                            SendUserId = mailOutBox.UserId,
                            Subject = mailOutBox.Subject,
                            SubjectColor = mailOutBox.SubjectColor,
                            UserId = user2.Id,
                            OutBoxId = mailOutBox.Id,
                            MailType = 2
                        };
                        list.Add(box2);
                    }
                    foreach (RoadFlow.Model.User user3 in secretCopyUsers)
                    {
                        RoadFlow.Model.MailInBox box3 = new RoadFlow.Model.MailInBox
                        {
                            ContentsId = mailContent.Id,
                            Id = Guid.NewGuid(),
                            IsRead = 0,
                            SendDateTime = mailOutBox.SendDateTime,
                            SendUserId = mailOutBox.UserId,
                            Subject = mailOutBox.Subject,
                            SubjectColor = mailOutBox.SubjectColor,
                            UserId = user3.Id,
                            OutBoxId = mailOutBox.Id,
                            MailType = 3
                        };
                        list.Add(box3);
                    }
                    context.AddRange<RoadFlow.Model.MailInBox>((IEnumerable<RoadFlow.Model.MailInBox>)list);
                }
                return context.SaveChanges();
            }
        }
        public int Update(RoadFlow.Model.MailOutBox mailOutBox)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.MailOutBox>(mailOutBox);
                return context.SaveChanges();
            }
        }

        public bool Withdraw(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                if (context.Find<RoadFlow.Model.MailOutBox>(objects) == null || context.Find<RoadFlow.Model.MailOutBox>(objects).SendDateTime.AddHours(RoadFlow.Utility.Config.MailWithdrawTime )<=DateTime.Today)
                {
                    return false;
                }
                    object[] objArray2 = new object[] { id };
                    context.Execute("DELETE RF_MailInBox WHERE OutBoxId={0}", objArray2);
                    object[] objArray3 = new object[] { id };
                    context.Execute("UPDATE RF_MailOutBox SET Status=0 WHERE Id={0}", objArray3);
                    context.SaveChanges();
                return true;
            }
        }
    }


  



    #endregion


}
