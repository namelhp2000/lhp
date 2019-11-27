using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RoadFlow.Data
{
    public class UserFileShare
    {
        // Methods
        public int Add(RoadFlow.Model.UserFileShare userFileShare)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.UserFileShare>(userFileShare);
                return context.SaveChanges();
            }
        }

        public int Add(IEnumerable<RoadFlow.Model.UserFileShare> userFileShares)
        {
            using (DataContext context = new DataContext())
            {
                context.AddRange<RoadFlow.Model.UserFileShare>(userFileShares);
                return context.SaveChanges();
            }
        }

        public int DeleteByFileId(string fileId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { fileId };
                context.Execute("DELETE FROM RF_UserFileShare WHERE FileId={0}", objects);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.UserFileShare Get(string fileId, Guid userId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { fileId, userId };
                return context.Find<RoadFlow.Model.UserFileShare>(objects);
            }
        }

        public List<RoadFlow.Model.UserFileShare> GetListByFileId(string fileId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { fileId };
                return context.Query<RoadFlow.Model.UserFileShare>("SELECT * FROM RF_UserFileShare WHERE FileId={0}", objects);
            }
        }

        public DataTable GetMySharePagerList(out int count, int size, int number, Guid userId, string fileName, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetUserFileShareSql(userId, fileName, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }


        public DataTable GetMyShareUsers(string fileId, Guid userId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { fileId, userId };
                return context.GetDataTable("SELECT UserId FROM RF_UserFileShare WHERE FileId={0} AND ShareUserId={1}", objects);
            }
        }



        public DataTable GetShareMyPagerList(out int count, int size, int number, Guid userId, string fileName, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetUserFileShareMySql(userId, fileName, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int Share(IEnumerable<RoadFlow.Model.UserFileShare> userFileShares, string fileId)
        {
            using (DataContext context = new DataContext())
            {
                List<RoadFlow.Model.UserFileShare> list = new List<RoadFlow.Model.UserFileShare>();
                List<RoadFlow.Model.UserFileShare> listByFileId = this.GetListByFileId(fileId);
                using (IEnumerator<RoadFlow.Model.UserFileShare> enumerator = userFileShares.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        RoadFlow.Model.UserFileShare share = enumerator.Current;
                        if (!listByFileId.Exists(delegate (RoadFlow.Model.UserFileShare p) {
                            return p.UserId == share.UserId;
                        }))
                        {
                            list.Add(share);
                        }
                    }
                }
                if (Enumerable.Any<RoadFlow.Model.UserFileShare>((IEnumerable<RoadFlow.Model.UserFileShare>)list))
                {
                    context.AddRange<RoadFlow.Model.UserFileShare>((IEnumerable<RoadFlow.Model.UserFileShare>)list);
                }
                return context.SaveChanges();
            }
        }
    }






}
