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
    public class WorkDate
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_workdate";

        // Methods
        public int Add(RoadFlow.Model.WorkDate[] workDates, int year)
        {
            this.ClearCache(year);
            using (DataContext context = new DataContext())
            {
                string sql = context.IsOracle ? ("DELETE FROM RF_WorkDate WHERE TO_CHAR(WorkDay,'yyyy')=" + ((int)year)) : ("DELETE FROM RF_WorkDate WHERE YEAR(WorkDay)=" + ((int)year));
                context.Execute(sql, (DbParameter[])null);
                if (workDates.Length != 0)
                {
                    context.AddRange<RoadFlow.Model.WorkDate>(workDates);
                }
                return context.SaveChanges();
            }
        }

        public void ClearCache(int year)
        {
            IO.Remove("roadflow_cache_workdate_" + ((int)year).ToString());
        }

        public int Delete(int year)
        {
            this.ClearCache(year);
            using (DataContext context = new DataContext())
            {
                List<RoadFlow.Model.WorkDate> yearList = this.GetYearList(year);
                context.RemoveRange<WorkDate>((IEnumerable<WorkDate>)yearList);
                return context.SaveChanges();
            }
        }

        public int GetMinYear()
        {
            using (DataContext context = new DataContext())
            {
                int num;
                string sql = context.IsOracle ? "SELECT MIN(TO_CHAR(WorkDay,'yyyy')) FROM RF_WorkDate" : "SELECT MIN(YEAR(WorkDay)) FROM RF_WorkDate";
                return (context.ExecuteScalarString(sql, (DbParameter[])null).IsInt(out num) ? num : DateTimeExtensions.Now.Year);
            }
        }

        public List<RoadFlow.Model.WorkDate> GetYearList(int year)
        {
            object obj2 = IO.Get("roadflow_cache_workdate_" + ((int)year).ToString());
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    string sql = context.IsOracle ? ("SELECT * FROM RF_WorkDate WHERE TO_CHAR(WorkDay,'yyyy')=" + ((int)year)) : ("SELECT * FROM RF_WorkDate WHERE YEAR(WorkDay)=" + ((int)year));
                    List<RoadFlow.Model.WorkDate> list = context.Query<RoadFlow.Model.WorkDate>(sql, (DbParameter[])null);
                    IO.Insert("roadflow_cache_workdate", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.WorkDate>)obj2;
        }
    }


}
