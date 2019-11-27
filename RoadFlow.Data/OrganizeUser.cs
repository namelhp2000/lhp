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
    public class OrganizeUser
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_organizeuser";

        // Methods
        public int Add(RoadFlow.Model.OrganizeUser organizeUser)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.OrganizeUser>(organizeUser);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_organizeuser");
        }

        public int Delete(RoadFlow.Model.OrganizeUser organizeUser)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.OrganizeUser>(organizeUser);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.OrganizeUser> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_organizeuser");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.OrganizeUser> list = context.QueryAll<RoadFlow.Model.OrganizeUser>();
                    IO.Insert("roadflow_cache_organizeuser", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.OrganizeUser>)obj2;
        }

        public int Update(RoadFlow.Model.OrganizeUser organizeUser)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.OrganizeUser>(organizeUser);
                return context.SaveChanges();
            }
        }

        public int Update(List<Tuple<RoadFlow.Model.OrganizeUser, int>> tuples)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                foreach (Tuple<RoadFlow.Model.OrganizeUser, int> tuple in tuples)
                {
                    if (tuple.Item2 == 0)
                    {
                        context.Remove<RoadFlow.Model.OrganizeUser>(tuple.Item1);
                    }
                    else if (tuple.Item2 == 1)
                    {
                        context.Add<RoadFlow.Model.OrganizeUser>(tuple.Item1);
                    }
                    else if (tuple.Item2 == 2)
                    {
                        context.Update<RoadFlow.Model.OrganizeUser>(tuple.Item1);
                    }
                }
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.OrganizeUser[] organizeUsers)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.OrganizeUser>(organizeUsers);
                return context.SaveChanges();
            }
        }
    }


}
