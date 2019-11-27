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
    public class HomeSet
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_homeset";

        // Methods
        public int Add(RoadFlow.Model.HomeSet homeSet)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.HomeSet>(homeSet);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_homeset");
        }

        public int Delete(RoadFlow.Model.HomeSet homeSet)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.HomeSet>(homeSet);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.HomeSet[] homeSets)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.HomeSet>(homeSets);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.HomeSet> GetAll()
        {
            using (DataContext context = new DataContext())
            {
                return context.QueryAll<RoadFlow.Model.HomeSet>();
            }
        }

        public DataTable GetPagerData(out int count, int size, int number, string name, string title, string type, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetHomeSetSql(name, title, type, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int Update(RoadFlow.Model.HomeSet homeSet)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.HomeSet>(homeSet);
                return context.SaveChanges();
            }
        }
    }


}
