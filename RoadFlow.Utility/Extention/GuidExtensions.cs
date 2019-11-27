using System;

namespace RoadFlow.Utility
{
    public static class GuidExtensions
    {
        /// <summary>
        /// Guid是否为空
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsEmptyGuid(this Guid guid)
        {
            return (guid == Guid.Empty);
        }


        public static bool IsEmptyGuid(this Guid? guid)
        {
            if (guid.HasValue)
            {
                return (guid.Value == Guid.Empty);
            }
            return true;
        }





        /// <summary>
        /// 判断是否Guid不为空
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsNotEmptyGuid(this Guid guid)
        {
            return (guid != Guid.Empty);
        }

        public static bool IsNotEmptyGuid(this Guid? guid)
        {
            return (guid.HasValue && (guid.Value != Guid.Empty));
        }





        /// <summary>
        /// Guid转成int
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static int ToInt(this Guid guid)
        {
            return Math.Abs(guid.GetHashCode());
        }


        public static string ToLowerNString(this Guid guid)
        {
            return guid.ToString("N").ToLower();
        }

        /// <summary>
        /// Guid转换成小写
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToLowerString(this Guid guid)
        {
            return guid.ToString().ToLower();
        }


        public static string ToNString(this Guid guid)
        {
            return guid.ToString("N");
        }



        /// <summary>
        /// Guid转成字符串
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToUpperString(this Guid guid)
        {
            return guid.ToString().ToUpper();
        }

        public static string ToUpperNString(this Guid guid)
        {
            return guid.ToString("N").ToUpper();
        }





        /// <summary>
        /// 转为有序的GUID
        /// 注：长度为50字符
        /// </summary>
        /// <param name="guid">新的GUID</param>
        /// <returns></returns>
        public static string ToSequentialGuid(this Guid guid)
        {
            var timeStr = (DateTime.Now.ToCstTime().Ticks / 10000).ToString("x8");
            var newGuid = $"{timeStr.PadLeft(13, '0')}-{guid}";

            return newGuid;
        }






    }


}
