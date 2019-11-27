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
    public class UserShortcut
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_usershortcut";

        // Methods
        public int Add(RoadFlow.Model.UserShortcut[] userShortcuts, Guid userId)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { userId };
                context.Execute("DELETE FROM RF_UserShortcut WHERE UserId={0}", objects);
                if (userShortcuts.Length != 0)
                {
                    context.AddRange<RoadFlow.Model.UserShortcut>(userShortcuts);
                }
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_usershortcut");
        }

        public int Delete(Guid userId)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { userId };
                context.Execute("DELETE FROM RF_UserShortcut WHERE UserId={0}", objects);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.UserShortcut> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_usershortcut");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.UserShortcut> list = context.QueryAll<RoadFlow.Model.UserShortcut>();
                    IO.Insert("roadflow_cache_usershortcut", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.UserShortcut>)obj2;
        }

        public List<RoadFlow.Model.UserShortcut> GetListByMenuId(Guid menuId)
        {
            return Enumerable.ToList<RoadFlow.Model.UserShortcut>((IEnumerable<RoadFlow.Model.UserShortcut>)Enumerable.OrderBy<RoadFlow.Model.UserShortcut, int>((IEnumerable<RoadFlow.Model.UserShortcut>)this.GetAll().FindAll(delegate (RoadFlow.Model.UserShortcut p)
            {
                return p.MenuId == menuId;
            }), key => key.Sort));
        }

        public List<RoadFlow.Model.UserShortcut> GetListByUserId(Guid userId)
        {         
            return Enumerable.ToList<RoadFlow.Model.UserShortcut>((IEnumerable<RoadFlow.Model.UserShortcut>)Enumerable.OrderBy<RoadFlow.Model.UserShortcut, int>((IEnumerable<RoadFlow.Model.UserShortcut>)this.GetAll().FindAll(delegate (RoadFlow.Model.UserShortcut p)
            {
                return p.UserId == userId;
            }), key => key.Sort));

        }

        public int Update(RoadFlow.Model.UserShortcut[] userShortcuts)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.UserShortcut>(userShortcuts);
                return context.SaveChanges();
            }
        }


    }
}
