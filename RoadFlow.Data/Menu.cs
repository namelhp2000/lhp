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
    public class Menu
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_menu";

        // Methods
        public int Add(RoadFlow.Model.Menu menu)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.Menu>(menu);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_menu");
            IO.Remove("roadflow_cache_menu_applibrary");
           // IO.Remove("roadflow_cache_menuuser");
        }

        //public int Delete(RoadFlow.Model.Menu[] menus)
        //{
        //    this.ClearCache();
        //    MenuUser user = new MenuUser();
        //    user.ClearCache();
        //    new UserShortcut().ClearCache();
        //    using (DataContext context = new DataContext())
        //    {
        //        context.RemoveRange<RoadFlow.Model.Menu>(menus);
        //        foreach (RoadFlow.Model.Menu menu in menus)
        //        {
        //            List<RoadFlow.Model.MenuUser> listByMenuId = user.GetListByMenuId(menu.Id);
        //            if (listByMenuId.Count > 0)
        //            {
        //                context.RemoveRange<RoadFlow.Model.MenuUser>((IEnumerable<RoadFlow.Model.MenuUser>)listByMenuId);
        //            }
        //            List<RoadFlow.Model.UserShortcut> list2 = new UserShortcut().GetListByMenuId(menu.Id);
        //            if (list2.Count > 0)
        //            {
        //                context.RemoveRange<RoadFlow.Model.UserShortcut>((IEnumerable<RoadFlow.Model.UserShortcut>)list2);
        //            }
        //        }
        //        return context.SaveChanges();
        //    }
        //}

        public int Delete(RoadFlow.Model.Menu[] menus)
        {
            int num = 0;
            MenuUser user = new MenuUser();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.Menu>(menus);
                foreach (RoadFlow.Model.Menu menu in menus)
                {
                    List<RoadFlow.Model.MenuUser> listByMenuId = user.GetListByMenuId(menu.Id);
                    if (listByMenuId.Count > 0)
                    {
                        context.RemoveRange<RoadFlow.Model.MenuUser>((IEnumerable<RoadFlow.Model.MenuUser>)listByMenuId);
                    }
                    List<RoadFlow.Model.UserShortcut> list2 = new UserShortcut().GetListByMenuId(menu.Id);
                    if (list2.Count > 0)
                    {
                        context.RemoveRange<RoadFlow.Model.UserShortcut>((IEnumerable<RoadFlow.Model.UserShortcut>)list2);
                    }
                }
                num = context.SaveChanges();
            }
            user.ClearCache();
            new UserShortcut().ClearCache();
            this.ClearCache();
            return num;
        }




        public RoadFlow.Model.Menu Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.Menu>(objects);
            }
        }

        public List<RoadFlow.Model.Menu> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_menu");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.Menu> list = context.QueryAll<RoadFlow.Model.Menu>();
                    IO.Insert("roadflow_cache_menu", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.Menu>)obj2;
        }

        public List<RoadFlow.Model.Menu> GetChilds(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Query<RoadFlow.Model.Menu>("SELECT * FROM RF_Menu WHERE ParentId={0}", objects);
            }
        }

        public DataTable GetMenuAppDataTable()
        {
            object obj2 = IO.Get("roadflow_cache_menu_applibrary");
            if (obj2 != null)
            {
                return (DataTable)obj2;
            }
            using (DataContext context = new DataContext())
            {
                string sql = "SELECT a.*,b.Address,b.OpenMode,b.Width,b.Height FROM RF_Menu a LEFT JOIN RF_AppLibrary b ON a.AppLibraryId=b.Id ORDER BY a.Sort";
                DataTable dataTable = context.GetDataTable(sql, (DbParameter[])null);
                IO.Insert("roadflow_cache_menu_applibrary", dataTable);
                return dataTable;
            }
        }

        public int Update(RoadFlow.Model.Menu menu)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.Menu>(menu);
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.Menu[] menus)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.Menu>(menus);
                return context.SaveChanges();
            }
        }
    }


}
