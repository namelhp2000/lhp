using System;
using System.Collections.Generic;
using System.Linq;


namespace RoadFlow.Utility
{
    /// <summary>
    /// 系统扩展 - 类型转换
    /// </summary>
    public static class ConvertExtensions
    {
        /// <summary>
        /// 安全转换为字符串，去除两端空格，当值为null时返回""
        /// </summary>
        /// <param name="input">输入值</param>
        public static string SafeString(this object input)
        {
            return input?.ToString().Trim() ?? string.Empty;
        }

        /// <summary>
        /// 转换为bool
        /// </summary>
        /// <param name="obj">数据</param>
        public static bool ToBool(this string obj)
        {
            return ConvertHelper.ToBool(obj);
        }

        /// <summary>
        /// 转换为可空bool
        /// </summary>
        /// <param name="obj">数据</param>
        public static bool? ToBoolOrNull(this string obj)
        {
            return ConvertHelper.ToBoolOrNull(obj);
        }

        /// <summary>
        /// 转换为int
        /// </summary>
        /// <param name="obj">数据</param>
        public static int ToInt(this string obj)
        {
            return ConvertHelper.ToInt(obj);
        }

        /// <summary>
        /// 转换为可空int
        /// </summary>
        /// <param name="obj">数据</param>
        public static int? ToIntOrNull(this string obj)
        {
            return ConvertHelper.ToIntOrNull(obj);
        }

        /// <summary>
        /// 转换为long
        /// </summary>
        /// <param name="obj">数据</param>
        public static long ToLong(this string obj)
        {
            return ConvertHelper.ToLong(obj);
        }

        /// <summary>
        /// 转换为可空long
        /// </summary>
        /// <param name="obj">数据</param>
        public static long? ToLongOrNull(this string obj)
        {
            return ConvertHelper.ToLongOrNull(obj);
        }

        /// <summary>
        /// 转换为double
        /// </summary>
        /// <param name="obj">数据</param>
        public static double ToDouble(this string obj)
        {
            return ConvertHelper.ToDouble(obj);
        }

        /// <summary>
        /// 转换为可空double
        /// </summary>
        /// <param name="obj">数据</param>
        public static double? ToDoubleOrNull(this string obj)
        {
            return ConvertHelper.ToDoubleOrNull(obj);
        }

        /// <summary>
        /// 转换为decimal
        /// </summary>
        /// <param name="obj">数据</param>
        public static decimal ToDecimal(this string obj)
        {
            return ConvertHelper.ToDecimal(obj);
        }

        /// <summary>
        /// 转换为可空decimal
        /// </summary>
        /// <param name="obj">数据</param>
        public static decimal? ToDecimalOrNull(this string obj)
        {
            return ConvertHelper.ToDecimalOrNull(obj);
        }

        /// <summary>
        /// 转换为日期
        /// </summary>
        /// <param name="obj">数据</param>
        public static DateTime ToDate(this string obj)
        {
            return ConvertHelper.ToDate(obj);
        }

        /// <summary>
        /// 转换为可空日期
        /// </summary>
        /// <param name="obj">数据</param>
        public static DateTime? ToDateOrNull(this string obj)
        {
            return ConvertHelper.ToDateOrNull(obj);
        }

        ///// <summary>
        ///// 转换为Guid
        ///// </summary>
        ///// <param name="obj">数据</param>
        //public static Guid ToGuid(this string obj)
        //{
        //    return ConvertHelper.ToGuid(obj);
        //}

        /// <summary>
        /// 转换为可空Guid
        /// </summary>
        /// <param name="obj">数据</param>
        public static Guid? ToGuidOrNull(this string obj)
        {
            return ConvertHelper.ToGuidOrNull(obj);
        }

        /// <summary>
        /// 转换为Guid集合
        /// </summary>
        /// <param name="obj">数据,范例: "83B0233C-A24F-49FD-8083-1337209EBC9A,EAB523C6-2FE7-47BE-89D5-C6D440C3033A"</param>
        public static List<Guid> ToGuidList(this string obj)
        {
            return ConvertHelper.ToGuidList(obj);
        }

        /// <summary>
        /// 转换为Guid集合
        /// </summary>
        /// <param name="obj">字符串集合</param>
        public static List<Guid> ToGuidList(this IList<string> obj)
        {
            if (obj == null)
                return new List<Guid>();
            return obj.Select(t => t.ToGuid()).ToList();
        }


        /// <summary>
        /// 获得类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetTypeByString(string type)
        {
            switch (type.ToLower())
            {
                case "system.boolean":
                    return Type.GetType("System.Boolean", true, true);
                case "system.byte":
                    return Type.GetType("System.Byte", true, true);
                case "system.sbyte":
                    return Type.GetType("System.SByte", true, true);
                case "system.char":
                    return Type.GetType("System.Char", true, true);
                case "system.decimal":
                    return Type.GetType("System.Decimal", true, true);
                case "system.double":
                    return Type.GetType("System.Double", true, true);
                case "system.single":
                    return Type.GetType("System.Single", true, true);
                case "system.int32":
                    return Type.GetType("System.Int32", true, true);
                case "system.uint32":
                    return Type.GetType("System.UInt32", true, true);
                case "system.int64":
                    return Type.GetType("System.Int64", true, true);
                case "system.uint64":
                    return Type.GetType("System.UInt64", true, true);
                case "system.object":
                    return Type.GetType("System.Object", true, true);
                case "system.int16":
                    return Type.GetType("System.Int16", true, true);
                case "system.uint16":
                    return Type.GetType("System.UInt16", true, true);
                case "system.string":
                    return Type.GetType("System.String", true, true);
                case "system.datetime":
                case "datetime":
                    return Type.GetType("System.DateTime", true, true);
                case "system.guid":
                    return Type.GetType("System.Guid", true, true);
                default:
                    return Type.GetType(type, true, true);
            }
        }
    }
}
