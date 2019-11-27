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
    public class FlowComment
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_flowcomment";

        // Methods
        public int Add(RoadFlow.Model.FlowComment flowComments)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.FlowComment>(flowComments);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_flowcomment");
        }

        public int Delete(RoadFlow.Model.FlowComment[] flowComments)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.FlowComment>(flowComments);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.FlowComment> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_flowcomment");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.FlowComment> list = Enumerable.ToList<RoadFlow.Model.FlowComment>((IEnumerable<RoadFlow.Model.FlowComment>)Enumerable.OrderBy<RoadFlow.Model.FlowComment, int>((IEnumerable<RoadFlow.Model.FlowComment>)context.QueryAll<RoadFlow.Model.FlowComment>(), key => key.Sort));
                    IO.Insert("roadflow_cache_flowcomment", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.FlowComment>)obj2;
        }

        public int Update(RoadFlow.Model.FlowComment flowComments)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.FlowComment>(flowComments);
                return context.SaveChanges();
            }
        }

    }
}
