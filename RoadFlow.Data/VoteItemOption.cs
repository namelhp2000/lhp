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
    public class VoteItemOption
    {
        // Methods
        public int Add(RoadFlow.Model.VoteItemOption voteItemOption)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.VoteItemOption>(voteItemOption);
                return context.SaveChanges();
            }
        }

        public int Delete(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                context.Execute("DELETE FROM RF_VoteItemOption WHERE Id={0}", objects);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.VoteItemOption Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.VoteItemOption>(objects);
            }
        }

        public List<RoadFlow.Model.VoteItemOption> GetItemOptions(Guid itemId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { itemId };
                return context.Query<RoadFlow.Model.VoteItemOption>("SELECT * FROM RF_VoteItemOption WHERE ItemId={0}", objects);
            }
        }

        public List<RoadFlow.Model.VoteItemOption> GetVoteItemOptions(Guid voteId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { voteId };
                return context.Query<RoadFlow.Model.VoteItemOption>("SELECT * FROM RF_VoteItemOption WHERE VoteId={0}", objects);
            }
        }

        public int Update(RoadFlow.Model.VoteItemOption voteItemOption)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.VoteItemOption>(voteItemOption);
                return context.SaveChanges();
            }
        }
    }




}
