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
    public class FlowApiSystem
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_flowapisystem";

        // Methods
        public int Add(RoadFlow.Model.FlowApiSystem flowApiSystem)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.FlowApiSystem>(flowApiSystem);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_flowapisystem");
        }

        public int Delete(RoadFlow.Model.FlowApiSystem flowApiSystem)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.FlowApiSystem>(flowApiSystem);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.FlowApiSystem[] flowApiSystems)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.FlowApiSystem>(flowApiSystems);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.FlowApiSystem> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_flowapisystem");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.FlowApiSystem> list = context.QueryAll<RoadFlow.Model.FlowApiSystem>();
                    IO.Insert("roadflow_cache_flowapisystem", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.FlowApiSystem>)obj2;
        }

        public int Update(RoadFlow.Model.FlowApiSystem flowApiSystem)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.FlowApiSystem>(flowApiSystem);
                return context.SaveChanges();
            }
        }
    }




}
