using System;

namespace RoadFlow.Utility.Cache
{
    public class IO
    {
        // Methods
        /// <summary>
        /// 通过缓存主键获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            return Factory.GetInstance().Get(key);
        }

        /// <summary>
        /// 插入缓存，通过主键与对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object Insert(string key, object obj)
        {
            return Factory.GetInstance().Insert(key, obj);
        }

        /// <summary>
        /// 设置过期日期，插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static object Insert(string key, object obj, DateTime expiry)
        {
            return Factory.GetInstance().Insert(key, obj, expiry);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            Factory.GetInstance().Remove(key);
        }
    }


}
