using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Data
{
    public class FlowButton
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_flowbutton";

        // Methods
        public int Add(RoadFlow.Model.FlowButton flowButton)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.FlowButton>(flowButton);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_flowbutton");
        }

        public int Delete(RoadFlow.Model.FlowButton[] flowButtons)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.FlowButton>(flowButtons);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.FlowButton> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_flowbutton");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.FlowButton> list = Enumerable.ToList<RoadFlow.Model.FlowButton>((IEnumerable<RoadFlow.Model.FlowButton>)Enumerable.OrderBy<RoadFlow.Model.FlowButton, int>((IEnumerable<RoadFlow.Model.FlowButton>)context.QueryAll<RoadFlow.Model.FlowButton>(), key => key.Sort));
                    IO.Insert("roadflow_cache_flowbutton", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.FlowButton>)obj2;
        }

        public int Update(RoadFlow.Model.FlowButton flowButton)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.FlowButton>(flowButton);
                return context.SaveChanges();
            }
        }
    }
}
