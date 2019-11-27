using System;

namespace RoadFlow.Utility
{
    public static class MathExtensions
    {
        /// <summary>
        /// 判断数字是否包含
        /// </summary>
        /// <param name="digit"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static bool In(this int digit, params int[] digits)
        {
            int[] numArray = digits;
            for (int i = 0; i < numArray.Length; i++)
            {
                if (numArray[i] == digit)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否不包含
        /// </summary>
        /// <param name="digit"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static bool NotIn(this int digit, params int[] digits)
        {
            int[] numArray = digits;
            for (int i = 0; i < numArray.Length; i++)
            {
                if (numArray[i] == digit)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ToFileSize(this long size)
        {
            double num;
            if (size < 0x400L)
            {
                return (((long)size) + "BT");
            }
            if (size < 0x100000L)
            {
                num = ((double)size) / 1024.0;
                return (((double)num).ToString("0.00") + "KB");
            }
            if (size < 0x40000000L)
            {
                num = ((double)size) / 1048576.0;
                return (((double)num).ToString("0.00") + "MB");
            }
            num = ((double)size) / 1073741824.0;
            return (((double)num).ToString("0.00") + "GB");
        }

        /// <summary>
        /// 向上取整  4.5 结果5
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int IntCeiling(Decimal size)
        {
            return (int)Math.Ceiling(size);
        }


        /// <summary>
        /// 向上取整  4.5 结果4
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int IntFloor(Decimal size)
        {
            return (int)Math.Floor(size);
        }


        /// <summary>
        /// 向上四舍五入取整  4.5 结果5
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int IntRound(Decimal size)
        {
            return (int)Math.Round(size);
        }


        /// <summary>
        ///获取页码值
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int PageValue(int count, int size)
        {
            int page = 0;
            if (count % size == 0)
            {
                page = count / size;
            }
            else
            {
                page = count / size + 1;
            }
            return page;
        }



    }



}
