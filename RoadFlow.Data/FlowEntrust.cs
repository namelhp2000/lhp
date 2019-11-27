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
    public class FlowEntrust
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_flowentrust";

        // Methods
        public int Add(RoadFlow.Model.FlowEntrust flowEntrust)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.FlowEntrust>(flowEntrust);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_flowentrust");
        }

        public int Delete(RoadFlow.Model.FlowEntrust[] flowEntrusts)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.FlowEntrust>(flowEntrusts);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.FlowEntrust Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.FlowEntrust p) {
                return p.Id == id;
            });
        }

        public List<RoadFlow.Model.FlowEntrust> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_flowentrust");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.FlowEntrust> list = context.QueryAll<RoadFlow.Model.FlowEntrust>();
                    IO.Insert("roadflow_cache_flowentrust", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.FlowEntrust>)obj2;
        }

        public DataTable GetPagerList(out int count, int size, int number, string userId, string date1, string date2, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetFlowEntrustSql(userId, date1, date2, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int Update(RoadFlow.Model.FlowEntrust flowEntrust)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.FlowEntrust>(flowEntrust);
                return context.SaveChanges();
            }
        }
    }


}
