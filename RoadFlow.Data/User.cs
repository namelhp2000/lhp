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
    public class User
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_user";

        // Methods
        public int Add(RoadFlow.Model.User user, RoadFlow.Model.OrganizeUser organizeUser)
        {
            this.ClearCache();
            new OrganizeUser().ClearCache();
            new HomeSet().ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.User>(user);
                context.Add<RoadFlow.Model.OrganizeUser>(organizeUser);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_user");
        }

        public int Delete(RoadFlow.Model.User user, RoadFlow.Model.OrganizeUser[] organizeUsers)
        {
            this.ClearCache();
            new OrganizeUser().ClearCache();
            new HomeSet().ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.User>(user);
                context.RemoveRange<RoadFlow.Model.OrganizeUser>(organizeUsers);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.User> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_user");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.User> list = context.QueryAll<RoadFlow.Model.User>();
                    IO.Insert("roadflow_cache_user", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.User>)obj2;
        }

        public int Update(RoadFlow.Model.User user)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.User>(user);
                return context.SaveChanges();
            }
        }
    }


}
