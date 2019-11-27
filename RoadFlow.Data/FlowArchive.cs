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
    public class FlowArchive
    {
        // Methods
        public int Add(RoadFlow.Model.FlowArchive flowArchive)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.FlowArchive>(flowArchive);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.FlowArchive Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.FlowArchive>(objects);
            }
        }

        public DataTable GetPagerData(out int count, int size, int number, string flowId, string stepName, string title, string date1, string date2, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetFlowArchiveSql(flowId, stepName, title, date1, date2, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }
    }


}
