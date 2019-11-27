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
    public class VoteResult
    {
        // Methods
        public int Add(RoadFlow.Model.VoteResult voteResult)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.VoteResult>(voteResult);
                return context.SaveChanges();
            }
        }

        public int AddRange(List<RoadFlow.Model.VoteResult> voteResults)
        {
            if ((voteResults == null) || (voteResults.Count == 0))
            {
                return 0;
            }
            Guid voteId = Enumerable.First<RoadFlow.Model.VoteResult>((IEnumerable<RoadFlow.Model.VoteResult>)voteResults).VoteId;
            Guid userId = Enumerable.First<RoadFlow.Model.VoteResult>((IEnumerable<RoadFlow.Model.VoteResult>)voteResults).UserId;
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { voteId, userId };
                context.Execute("DELETE FROM RF_VoteResult WHERE VoteId={0} AND UserId={1}", objects);
                context.AddRange<RoadFlow.Model.VoteResult>((IEnumerable<RoadFlow.Model.VoteResult>)voteResults);
                object[] objArray2 = new object[] { DateTimeExtensions.Now, voteId, userId };
                context.Execute("UPDATE RF_VotePartakeUser SET Status=1,SubmitTime={0} WHERE VoteId={1} AND UserId={2}", objArray2);
                object[] objArray3 = new object[] { voteId, userId };
                if (context.GetDataTable("SELECT Id FROM RF_VotePartakeUser WHERE Status=0 And VoteId={0} AND UserId!={1}", objArray3).Rows.Count == 0)
                {
                    object[] objArray4 = new object[] { voteId };
                    context.Execute("UPDATE RF_Vote SET Status=3 WHERE Id={0}", objArray4);
                }
                else
                {
                    object[] objArray5 = new object[] { voteId };
                    context.Execute("UPDATE RF_Vote SET Status=2 WHERE Id={0}", objArray5);
                }
                return context.SaveChanges();
            }
        }

        public int Delete(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                context.Execute("DELETE FROM RF_VoteResult WHERE Id={0}", objects);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.VoteResult> GetVoteResults(Guid voteId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { voteId };
                return context.Query<RoadFlow.Model.VoteResult>("SELECT * FROM RF_VoteResult WHERE VoteId={0}", objects);
            }
        }

        public int Update(RoadFlow.Model.VoteResult voteResult)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.VoteResult>(voteResult);
                return context.SaveChanges();
            }
        }
    }




}
