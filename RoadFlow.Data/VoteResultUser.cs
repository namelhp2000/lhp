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
    public class VoteResultUser
    {
        // Methods
        public int Add(RoadFlow.Model.VoteResultUser voteResultUser)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.VoteResultUser>(voteResultUser);
                return context.SaveChanges();
            }
        }

        public int Delete(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                context.Execute("DELETE FROM RF_VoteResultUser WHERE Id={0}", objects);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.VoteResultUser> GetVoteResultUsers(Guid voteId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { voteId };
                return context.Query<RoadFlow.Model.VoteResultUser>("SELECT * FROM RF_VoteResultUser WHERE VoteId={0}", objects);
            }
        }

        public int Update(RoadFlow.Model.VoteResultUser voteResultUser)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.VoteResultUser>(voteResultUser);
                return context.SaveChanges();
            }
        }
    }




}
