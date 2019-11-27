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
    public class Vote
    {
        // Methods
        public int Add(RoadFlow.Model.Vote vote)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.Vote>(vote);
                return context.SaveChanges();
            }
        }

        public int Delete(Guid id, bool isDeleteAll = true)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                context.Execute("DELETE FROM RF_Vote WHERE Id={0}", objects);
                if (isDeleteAll)
                {
                    object[] objArray2 = new object[] { id };
                    context.Execute("DELETE FROM RF_VoteItem WHERE VoteId={0}", objArray2);
                    object[] objArray3 = new object[] { id };
                    context.Execute("DELETE FROM RF_VoteItemOption WHERE VoteId={0}", objArray3);
                    object[] objArray4 = new object[] { id };
                    context.Execute("DELETE FROM RF_VotePartakeUser WHERE VoteId={0}", objArray4);
                    object[] objArray5 = new object[] { id };
                    context.Execute("DELETE FROM RF_VoteResult WHERE VoteId={0}", objArray5);
                    object[] objArray6 = new object[] { id };
                    context.Execute("DELETE FROM RF_VoteResultUser WHERE VoteId={0}", objArray6);
                }
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.Vote Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.Vote>(objects);
            }
        }

        public DataTable GetPagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetVoteSql(currentUserId, topic, date1, date2);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public DataTable GetPartakeUserPagerList(out int count, int size, int number, Guid voteId, string name, string org, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetPartakeUserSql(voteId, name, org);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public DataTable GetResultPagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetVoteResultSql(currentUserId, topic, date1, date2);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public DataTable GetWaitSubmitPagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetWaitSubmitVoteSql(currentUserId, topic, date1, date2);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int Update(RoadFlow.Model.Vote vote)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.Vote>(vote);
                return context.SaveChanges();
            }
        }
    }




}
