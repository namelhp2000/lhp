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
    public class DocDir
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_docdir";

        // Methods
        public int Add(RoadFlow.Model.DocDir docDir)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.DocDir>(docDir);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_docdir");
        }

        public int Delete(RoadFlow.Model.DocDir[] docDirs)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.DocDir>(docDirs);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.DocDir> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_docdir");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.DocDir> list = Enumerable.ToList<RoadFlow.Model.DocDir>((IEnumerable<RoadFlow.Model.DocDir>)Enumerable.OrderBy<RoadFlow.Model.DocDir, int>((IEnumerable<RoadFlow.Model.DocDir>)context.QueryAll<RoadFlow.Model.DocDir>(), key=>key.Sort));
                    IO.Insert("roadflow_cache_docdir", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.DocDir>)obj2;
        }

        public RoadFlow.Model.DocDir GetRoot()
        {
            List<RoadFlow.Model.DocDir> all = this.GetAll();
            if (all.Count == 0)
            {
                return null;
            }
            return all.Find(key=>key.ParentId==Guid.Empty);
        }

        public bool HasDoc(List<Guid> dirIds)
        {
            using (DataContext context = new DataContext())
            {
                return (context.GetDataTable("SELECT COUNT(Id) FROM RF_Doc WHERE DirId IN(" + dirIds.JoinSqlIn<Guid>(true).ToUpper() + ")", (DbParameter[])null).Rows[0][0].ToString().ToInt(-2147483648) > 0);
            }
        }

        public int Update(RoadFlow.Model.DocDir docDir)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.DocDir>(docDir);
                return context.SaveChanges();
            }
        }


    }
}
