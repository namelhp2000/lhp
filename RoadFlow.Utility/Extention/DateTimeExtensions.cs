using NodaTime;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RoadFlow.Utility
{
    /// <summary>
    /// 日期扩展
    /// </summary>
    public static class DateTimeExtensions
    {

        /// <summary>
        /// 获取格式化字符串，带时分秒，格式："yyyy-MM-dd HH:mm:ss"
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="removeSecond">是否移除秒</param>
        public static string ToDateTimeString(this DateTime dateTime, bool removeSecond = false)
        {
            if (removeSecond)
                return dateTime.ToString("yyyy-MM-dd HH:mm");
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取格式化字符串，带时分秒，格式："yyyy-MM-dd HH:mm:ss"
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="removeSecond">是否移除秒</param>
        public static string ToDateTimeString(this DateTime? dateTime, bool removeSecond = false)
        {
            if (dateTime == null)
                return string.Empty;
            return ToDateTimeString(dateTime.Value, removeSecond);
        }

        /// <summary>
        /// 获取格式化字符串，不带时分秒，格式："yyyy-MM-dd"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 获取格式化字符串，不带时分秒，格式："yyyy-MM-dd"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToDateString(this DateTime? dateTime)
        {
            if (dateTime == null)
                return string.Empty;
            return ToDateString(dateTime.Value);
        }

        /// <summary>
        /// 获取格式化字符串，不带年月日，格式："HH:mm:ss"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 获取格式化字符串，不带年月日，格式："HH:mm:ss"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToTimeString(this DateTime? dateTime)
        {
            if (dateTime == null)
                return string.Empty;
            return ToTimeString(dateTime.Value);
        }

        /// <summary>
        /// 获取格式化字符串，带毫秒，格式："yyyy-MM-dd HH:mm:ss.fff"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToMillisecondString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// 获取格式化字符串，带毫秒，格式："yyyy-MM-dd HH:mm:ss.fff"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToMillisecondString(this DateTime? dateTime)
        {
            if (dateTime == null)
                return string.Empty;
            return ToMillisecondString(dateTime.Value);
        }

        /// <summary>
        /// 获取格式化字符串，不带时分秒，格式："yyyy年MM月dd日"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToChineseDateString(this DateTime dateTime)
        {
            return string.Format("{0}年{1}月{2}日", dateTime.Year, dateTime.Month, dateTime.Day);
        }

        /// <summary>
        /// 获取格式化字符串，不带时分秒，格式："yyyy年MM月dd日"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToChineseDateString(this DateTime? dateTime)
        {
            if (dateTime == null)
                return string.Empty;
            return ToChineseDateString(dateTime.SafeValue());
        }

        /// <summary>
        /// 获取格式化字符串，带时分秒，格式："yyyy年MM月dd日 HH时mm分"
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="removeSecond">是否移除秒</param>
        public static string ToChineseDateTimeString(this DateTime dateTime, bool removeSecond = false)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("{0}年{1}月{2}日", dateTime.Year, dateTime.Month, dateTime.Day);
            result.AppendFormat(" {0}时{1}分", dateTime.Hour, dateTime.Minute);
            if (removeSecond == false)
                result.AppendFormat("{0}秒", dateTime.Second);
            return result.ToString();
        }

        /// <summary>
        /// 获取格式化字符串，带时分秒，格式："yyyy年MM月dd日 HH时mm分"
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="removeSecond">是否移除秒</param>
        public static string ToChineseDateTimeString(this DateTime? dateTime, bool removeSecond = false)
        {
            if (dateTime == null)
                return string.Empty;
            return ToChineseDateTimeString(dateTime.Value, removeSecond);
        }

        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="span">时间间隔</param>
        public static string Description(this TimeSpan span)
        {
            StringBuilder result = new StringBuilder();
            if (span.Days > 0)
                result.AppendFormat("{0}天", span.Days);
            if (span.Hours > 0)
                result.AppendFormat("{0}小时", span.Hours);
            if (span.Minutes > 0)
                result.AppendFormat("{0}分", span.Minutes);
            if (span.Seconds > 0)
                result.AppendFormat("{0}秒", span.Seconds);
            if (span.Milliseconds > 0)
                result.AppendFormat("{0}毫秒", span.Milliseconds);
            if (result.Length > 0)
                return result.ToString();
            return $"{span.TotalSeconds * 1000}毫秒";
        }


        /// <summary>
        /// 获取日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetDate(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd").ToDateTime();
        }

        ///// <summary>
        ///// 获取yyyy-MM-dd日期字符串
        ///// </summary>
        ///// <param name="date"></param>
        ///// <returns></returns>
        //public static string ToDateString(this DateTime date)
        //{
        //    return date.ToString("yyyy-MM-dd");
        //}

        /// <summary>
        /// 获取时间字符串
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }


        /// <summary>
        /// 获取时间字符串
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime? date)
        {
            if (!date.HasValue)
            {
                return string.Empty;
            }
            return date.Value.ToDateTimeString();
        }

        /// <summary>
        /// 获取yyyy年M月d日格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToLongDate2(this DateTime date)
        {
            return date.ToString("yyyy年M月d日");
        }

        /// <summary>
        /// 获取yyyy年M月d日格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToLongDate(this DateTime date)
        {
            return date.ToString("yyyy年MM月dd日");
        }



        /// <summary>
        /// 获取M月d日格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToLongDate1(this DateTime date)
        {
            return date.ToString("M月d日");
        }


        /// <summary>
        /// 获取M月d日格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToLongDateWeek(this DateTime date)
        {
            return date.ToString("dddd");
        }



        /// <summary>
        /// 获取yyyy-MM-dd HH:mm格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToShortDateTimeString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm");
        }







        /// <summary>
        /// 获取当前时间
        /// </summary>
        public static DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }



        /// <summary>
        /// 取每月的第一True/最末一天
        /// </summary>
        /// <param name="time">传入时间</param>
        /// <param name="firstDay">第一天还是最末一天</param>
        /// <returns></returns>
        public static DateTime DayOfMonth(DateTime time, bool firstDay)
        {
            DateTime time1 = new DateTime(time.Year, time.Month, 1);
            if (firstDay) return time1;
            else return time1.AddMonths(1).AddDays(-1);
        }
        /// <summary>
        /// 取每季度的第一True/最末一天
        /// </summary>
        /// <param name="time">传入时间</param>
        /// <param name="firstDay">第一天还是最末一天</param>
        /// <returns></returns>
        public static DateTime DayOfQuarter(DateTime time, bool firstDay)
        {
            int m = 0;
            switch (time.Month)
            {
                case 1:
                case 2:
                case 3:
                    m = 1; break;
                case 4:
                case 5:
                case 6:
                    m = 4; break;
                case 7:
                case 8:
                case 9:
                    m = 7; break;
                case 10:
                case 11:
                case 12:
                    m = 10; break;
            }

            DateTime time1 = new DateTime(time.Year, m, 1);
            if (firstDay) return time1;
            else return time1.AddMonths(3).AddDays(-1);
        }
        /// <summary>
        /// 取每年的第一True/最末一天
        /// </summary>
        /// <param name="time">传入时间</param>
        /// <param name="firstDay">第一天还是最末一天</param>
        /// <returns></returns>
        public static DateTime DayOfYear(DateTime time, bool firstDay)
        {
            if (firstDay) return new DateTime(time.Year, 1, 1);
            else return new DateTime(time.Year, 12, 31);
        }



        /// <summary>
        /// 返回标准日期格式string(yyyy-MM-dd)
        /// </summary>
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 返回指定日期格式
        /// </summary>
        public static string GetDate(string datetimestr, string replacestr)
        {
            if (datetimestr == null)
            {
                return replacestr;
            }

            if (datetimestr.Equals(""))
            {
                return replacestr;
            }

            try
            {
                datetimestr = Convert.ToDateTime(datetimestr).ToString("yyyy-MM-dd").Replace("1900-01-01", replacestr);
            }
            catch
            {
                return replacestr;
            }
            return datetimestr;

        }


        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回相对于当前时间的相对天数
        /// </summary>
        public static string GetDateTime(int relativeday)
        {
            return DateTime.Now.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTimeF()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTimeWeek()
        {
            return DateTime.Now.ToString("MM月dd日 dddd");
            // DateTime.Now.ToString("当前时间：" + "yyyy年MM月dd日 dddd tt HH:mm:ss");
        }





        /// <summary>
        /// 返回标准时间 
        /// </sumary>
        public static string GetStandardDateTime(string fDateTime, string formatStr)
        {
            if (fDateTime == "0000-0-0 0:00:00")
            {

                return fDateTime;
            }
            DateTime s = Convert.ToDateTime(fDateTime);
            return s.ToString(formatStr);
        }

        /// <summary>
        /// 返回标准时间 yyyy-MM-dd HH:mm:ss
        /// </sumary>
        public static string GetStandardDateTime(string fDateTime)
        {
            return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 判断是否时间格式
        /// </summary>
        /// <returns></returns>
        public static bool IsTime(string timeval)
        {
            return Regex.IsMatch(timeval, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }



        /// <summary>
        /// 转为标准时间（北京时间，解决Linux时区问题）
        /// </summary>
        /// <param name="dt">当前时间</param>
        /// <returns></returns>
        public static DateTime ToCstTime(this DateTime dt)
        {
            Instant now = SystemClock.Instance.GetCurrentInstant();
            var shanghaiZone = DateTimeZoneProviders.Tzdb["Asia/Shanghai"];
            return now.InZone(shanghaiZone).ToDateTimeUnspecified();
        }


        ///   <summary> 
        ///  获取某一日期是该年中的第几周
        ///   </summary> 
        ///   <param name="dateTime"> 日期 </param> 
        ///   <returns> 该日期在该年中的周数 </returns> 
        public static int GetWeekOfYear(this DateTime dateTime)
        {
            GregorianCalendar gc = new GregorianCalendar();
            return gc.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }


        /// <summary>
        /// 获取Js格式的timestamp
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <returns></returns>
        [Obsolete]
        public static long ToJsTimestamp(this DateTime dateTime)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long result = (dateTime.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
            return result;
        }

        /// <summary>
        /// 获取js中的getTime()
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns></returns>
        public static Int64 JsGetTime(this DateTime dt)
        {
            Int64 retval = 0;
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (dt.ToUniversalTime() - st);
            retval = (Int64)(t.TotalMilliseconds + 0.5);
            return retval;
        }




        /// <summary>
        /// 星期几  
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="num">0~6   0 代表星期一，以此类推，6 代表星期天</param>
        /// <returns></returns>
        public static DateTime WhatDayWeek(this DateTime dt, int num = 0)
        {

            var i = dt.DayOfWeek - DayOfWeek.Monday == -1 ? 6 : dt.DayOfWeek - DayOfWeek.Monday;
            var ts = new TimeSpan(i, 0, 0, 0);

            return dt.Subtract(ts).Date.AddDays(num); ;
        }




        public static DateTime MaxValue
        {
            get
            {
                return DateTime.MaxValue;
            }
        }

        public static DateTime MinValue
        {
            get
            {
                return DateTime.MinValue;
            }
        }



        /// <summary>
        /// 获取日期差
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string GetDateDiff(DateTime src)
        {
            string result = null;

            var currentSecond = (long)(DateTime.Now - src).TotalSeconds;

            long minSecond = 60;                //60s = 1min  
            var hourSecond = minSecond * 60;   //60*60s = 1 hour  
            var daySecond = hourSecond * 24;   //60*60*24s = 1 day  
            var weekSecond = daySecond * 7;    //60*60*24*7s = 1 week  
            var monthSecond = daySecond * 30;  //60*60*24*30s = 1 month  
            var yearSecond = daySecond * 365;  //60*60*24*365s = 1 year  

            if (currentSecond >= yearSecond)
            {
                var year = (int)(currentSecond / yearSecond);
                result = $"{year}年前";
            }
            else if (currentSecond < yearSecond && currentSecond >= monthSecond)
            {
                var month = (int)(currentSecond / monthSecond);
                result = $"{month}个月前";
            }
            else if (currentSecond < monthSecond && currentSecond >= weekSecond)
            {
                var week = (int)(currentSecond / weekSecond);
                result = $"{week}周前";
            }
            else if (currentSecond < weekSecond && currentSecond >= daySecond)
            {
                var day = (int)(currentSecond / daySecond);
                result = $"{day}天前";
            }
            else if (currentSecond < daySecond && currentSecond >= hourSecond)
            {
                var hour = (int)(currentSecond / hourSecond);
                result = $"{hour}小时前";
            }
            else if (currentSecond < hourSecond && currentSecond >= minSecond)
            {
                var min = (int)(currentSecond / minSecond);
                result = $"{min}分钟前";
            }
            else if (currentSecond < minSecond && currentSecond >= 0)
            {
                result = "刚刚";
            }
            else
            {
                result = src.ToString("yyyy/MM/dd HH:mm:ss");
            }
            return result;
        }





    }


}
