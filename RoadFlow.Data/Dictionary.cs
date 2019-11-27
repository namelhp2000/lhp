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
    public class Dictionary
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_dictionary";

        // Methods
        public int Add(RoadFlow.Model.Dictionary dictionary)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.Dictionary>(dictionary);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_dictionary");
        }

        public int Delete(RoadFlow.Model.Dictionary dictionary)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.Dictionary>(dictionary);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.Dictionary[] dictionarys)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.Dictionary>(dictionarys);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.Dictionary> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_dictionary");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.Dictionary> list = context.QueryAll<RoadFlow.Model.Dictionary>();
                    IO.Insert("roadflow_cache_dictionary", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.Dictionary>)obj2;
        }

        public int Update(RoadFlow.Model.Dictionary dictionary)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.Dictionary>(dictionary);
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.Dictionary[] dictionarys)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.Dictionary>(dictionarys);
                return context.SaveChanges();
            }
        }
    }


}
