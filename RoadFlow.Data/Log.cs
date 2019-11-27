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
    public class Log
    {
        // Methods
        public int Add(RoadFlow.Model.Log log)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.Log>(log);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.Log Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.Log>(objects);
            }
        }

        public DataTable GetPagerList(out int count, int size, int number, string title, string type, string userId, string date1, string date2, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetLogSql(title, type, userId, date1, date2, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }
    }


}
