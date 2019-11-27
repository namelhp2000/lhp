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
    public class VotePartakeUser
    {
        // Methods
        public int Add(RoadFlow.Model.VotePartakeUser votePartakeUser)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.VotePartakeUser>(votePartakeUser);
                return context.SaveChanges();
            }
        }

        public int Add(List<RoadFlow.Model.VotePartakeUser> votePartakeUsers, List<RoadFlow.Model.VoteResultUser> voteResultUsers)
        {
            if ((votePartakeUsers == null) || (votePartakeUsers.Count == 0))
            {
                return 0;
            }
            Guid voteId = Enumerable.First<RoadFlow.Model.VotePartakeUser>((IEnumerable<RoadFlow.Model.VotePartakeUser>)votePartakeUsers).VoteId;
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { voteId };
                context.Execute("DELETE FROM RF_VotePartakeUser WHERE VoteId={0}", objects);
                if ((votePartakeUsers != null) && (votePartakeUsers.Count > 0))
                {
                    context.AddRange<RoadFlow.Model.VotePartakeUser>((IEnumerable<RoadFlow.Model.VotePartakeUser>)votePartakeUsers);
                }
                object[] objArray2 = new object[] { voteId };
                context.Execute("DELETE FROM RF_VoteResultUser WHERE VoteId={0}", objArray2);
                if ((voteResultUsers != null) && (voteResultUsers.Count > 0))
                {
                    context.AddRange<RoadFlow.Model.VoteResultUser>((IEnumerable<RoadFlow.Model.VoteResultUser>)voteResultUsers);
                }
                object[] objArray3 = new object[] { voteId };
                context.Execute("UPDATE RF_Vote SET Status=1 WHERE Id={0}", objArray3);
                return context.SaveChanges();
            }
        }

        public int Delete(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                context.Execute("DELETE FROM RF_VotePartakeUser WHERE Id={0}", objects);
                return context.SaveChanges();
            }
        }

        public int DeleteByVoteId(Guid voteId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { voteId };
                context.Execute("DELETE FROM RF_VotePartakeUser WHERE VoteId={0}", objects);
                object[] objArray2 = new object[] { voteId };
                context.Execute("DELETE FROM RF_VoteResultUser WHERE VoteId={0}", objArray2);
                object[] objArray3 = new object[] { voteId };
                context.Execute("UPDATE RF_Vote SET Status=0 WHERE Id={0}", objArray3);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.VotePartakeUser> GetPartakeUsers(Guid voteID, int status = -1)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { voteID };
                return context.Query<RoadFlow.Model.VotePartakeUser>("SELECT * FROM RF_VotePartakeUser WHERE VoteId={0}" + ((status != -1) ? (" AND Status=" + ((int)status).ToString()) : ""), objects);
            }
        }

        public int Update(RoadFlow.Model.VotePartakeUser votePartakeUser)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.VotePartakeUser>(votePartakeUser);
                return context.SaveChanges();
            }
        }
    }




}
