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
    public class WorkGroup
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_workgroup";

        // Methods
        public int Add(RoadFlow.Model.WorkGroup workGroup)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.WorkGroup>(workGroup);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_workgroup");
            new HomeSet().ClearCache();
        }

        public int Delete(RoadFlow.Model.WorkGroup workGroup)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.WorkGroup>(workGroup);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.WorkGroup> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_workgroup");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.WorkGroup> list = context.QueryAll<RoadFlow.Model.WorkGroup>();
                    IO.Insert("roadflow_cache_workgroup", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.WorkGroup>)obj2;
        }

        public int Update(RoadFlow.Model.WorkGroup workGroup)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.WorkGroup>(workGroup);
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.WorkGroup[] workGroups)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.WorkGroup>(workGroups);
                return context.SaveChanges();
            }
        }
    }


}
