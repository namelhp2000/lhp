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
    public class VoteItem
    {
        // Methods
        public int Add(RoadFlow.Model.VoteItem voteItem)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.VoteItem>(voteItem);
                return context.SaveChanges();
            }
        }

        public int Delete(Guid id, bool isDeleteOptions = true)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                context.Execute("DELETE FROM RF_VoteItem WHERE Id={0}", objects);
                if (isDeleteOptions)
                {
                    object[] objArray2 = new object[] { id };
                    context.Execute("DELETE FROM RF_VoteItemOption WHERE VoteId={0}", objArray2);
                }
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.VoteItem Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.VoteItem>(objects);
            }
        }

        public List<RoadFlow.Model.VoteItem> GetItems(Guid voteId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { voteId };
                return context.Query<RoadFlow.Model.VoteItem>("SELECT * FROM RF_VoteItem WHERE VoteId={0}", objects);
            }
        }

        public int Update(RoadFlow.Model.VoteItem voteItem)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.VoteItem>(voteItem);
                return context.SaveChanges();
            }
        }
    }




}
