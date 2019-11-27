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
    public class MenuUser
    {
        // Fields
        public const string CACHEKEY = "roadflow_cache_menuuser";

        // Methods
        public int Add(RoadFlow.Model.MenuUser menuUser)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.MenuUser>(menuUser);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_menuuser");
        }

        public int Delete(RoadFlow.Model.MenuUser menuUser)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.MenuUser>(menuUser);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.MenuUser[] menuUsers)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.MenuUser>(menuUsers);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.MenuUser> GetAll(out bool isCache)
        {
            object obj2 = IO.Get("roadflow_cache_menuuser");
            if (obj2 == null)
            {
                isCache = false;
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.MenuUser> list = context.QueryAll<RoadFlow.Model.MenuUser>();
                    IO.Insert("roadflow_cache_menuuser", list);
                    return list;
                }
            }
            isCache = true;
            return (List<RoadFlow.Model.MenuUser>)obj2;
        }

        public List<RoadFlow.Model.MenuUser> GetListByMenuId(Guid menuId)
        {
            bool flag;
            return this.GetAll(out flag).FindAll(delegate (RoadFlow.Model.MenuUser p) {
                return p.MenuId == menuId;
            });
        }

        public List<RoadFlow.Model.MenuUser> GetListByOrgId(string orgId)
        {
            bool flag;
            return this.GetAll(out flag).FindAll(delegate (RoadFlow.Model.MenuUser p) {
                return p.Organizes.EqualsIgnoreCase(orgId);
            });
        }

        public int Update(RoadFlow.Model.MenuUser menuUser)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.MenuUser>(menuUser);
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.MenuUser[] menuUsers)
        {
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.MenuUser>(menuUsers);
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.MenuUser[] menuUsers, string orgId)
        {
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.MenuUser>(this.GetListByOrgId(orgId).ToArray());
                context.AddRange<RoadFlow.Model.MenuUser>(menuUsers);
                this.ClearCache();
                return context.SaveChanges();
            }
        }
    }


}
