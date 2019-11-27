using Microsoft.Extensions.Caching.Memory;
using System;

namespace RoadFlow.Utility.Cache
{
    internal class CoreCache : ICache
    {
        // Fields
        /// <summary>
        /// 超高速缓冲存储系统
        /// </summary>
        private static MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

        // Methods
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            object obj2;
            if (memoryCache.TryGetValue(key, out obj2))
            {
                return obj2;
            }
            return null;
        }


        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object Insert(string key, object obj)
        {
            return CacheExtensions.Set<object>(memoryCache, key, obj);
        }

        /// <summary>
        /// 设置过期，插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public object Insert(string key, object obj, DateTime expiry)
        {
            MemoryCacheEntryOptions options1 = new MemoryCacheEntryOptions();
            options1.SetAbsoluteExpiration(new DateTimeOffset(expiry));

            //  options1.set_AbsoluteExpiration(new DateTimeOffset(expiry));
            MemoryCacheEntryOptions options = options1;
            return CacheExtensions.Set<object>(memoryCache, key, obj, options);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            memoryCache.Remove(key);
        }
    }


}
