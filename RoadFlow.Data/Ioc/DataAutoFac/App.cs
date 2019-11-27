using Microsoft.Extensions.DependencyModel;
using RoadFlow.Mapper;
using RoadFlow.Utility;
using RoadFlow.Utility.Cache;
using RoadFlow.Utility.Dependencys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace RoadFlow.Data.DataAutoFac
{
    //注册使用以下 ITransientDependency接口方式
    public class App:IApp, ITransientDependency
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_applibrary";

        // Methods
        public int Add(RoadFlow.Model.AppLibrary appLibrary)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.AppLibrary>(appLibrary);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_applibrary");
            new Menu().ClearCache();
        }

        public int Delete(RoadFlow.Model.AppLibrary appLibrary)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.AppLibrary>(appLibrary);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.AppLibrary[] appLibrarys)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.AppLibrary>(appLibrarys);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.AppLibrary> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_applibrary");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.AppLibrary> list = context.QueryAll<RoadFlow.Model.AppLibrary>();
                    IO.Insert("roadflow_cache_applibrary", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.AppLibrary>)obj2;
        }

        public DataTable GetPagerList(out int count, int size, int number, string title, string address, string typeId, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetApplibrarySql(title, address, typeId, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int Update(RoadFlow.Model.AppLibrary appLibrary)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.AppLibrary>(appLibrary);
                return context.SaveChanges();
            }
        }
    }
}
