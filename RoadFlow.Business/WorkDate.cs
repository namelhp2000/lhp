using RoadFlow.Utility;
using System;
using System.Collections.Generic;

namespace RoadFlow.Business
{
    public class WorkDate
    {
        // Fields
        private readonly RoadFlow.Data.WorkDate workDateData = new RoadFlow.Data.WorkDate();

        // Methods
        public int Add(RoadFlow.Model.WorkDate[] workDates, int year)
        {
            return this.workDateData.Add(workDates, year);
        }

        public int Delete(int year)
        {
            return this.workDateData.Delete(year);
        }

        public int GetMinYear()
        {
            return this.workDateData.GetMinYear();
        }

        public DateTime GetWorkDateTime(double days, DateTime? dt = new DateTime?())
        {
            int num2;
            DateTime dateTime = (dt.HasValue && dt.HasValue) ? dt.Value : DateTimeExtensions.Now;
            List<RoadFlow.Model.WorkDate> yearList = this.GetYearList(dateTime.Year);
            int num = (int)Math.Floor(days);
            for (int i = 0; i < num; i = num2 + 1)
            {
                if (yearList.Exists(delegate (RoadFlow.Model.WorkDate p) {
                    return (p.WorkDay == dateTime.AddDays((double)i).Date) && (p.IsWork == 0);
                }))
                {
                    num++;
                }
                num2 = i;
            }
            return dateTime.AddDays(num + (days - Math.Floor(days)));
        }

        public List<RoadFlow.Model.WorkDate> GetYearList(int year)
        {
            return this.workDateData.GetYearList(year);
        }
    }


   



}
