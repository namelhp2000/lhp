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
    public class Organize
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_organize";

        // Methods
        public int Add(RoadFlow.Model.Organize organize)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.Organize>(organize);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_organize");
            new HomeSet().ClearCache();
        }

        public int Delete(RoadFlow.Model.Organize[] organizes, RoadFlow.Model.User[] users, RoadFlow.Model.OrganizeUser[] organizeUsers)
        {
            this.ClearCache();
            new OrganizeUser().ClearCache();
            new User().ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.Organize>(organizes);
                context.RemoveRange<RoadFlow.Model.User>(users);
                context.RemoveRange<RoadFlow.Model.OrganizeUser>(organizeUsers);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.Organize> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_organize");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.Organize> list = context.QueryAll<RoadFlow.Model.Organize>();
                    IO.Insert("roadflow_cache_organize", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.Organize>)obj2;
        }

        public int Update(RoadFlow.Model.Organize organize)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.Organize>(organize);
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.Organize[] organizes)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.Organize>(organizes);
                return context.SaveChanges();
            }
        }
    }


}
