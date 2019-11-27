using System;

namespace RoadFlow.Utility.Cache
{
    internal class Factory
    {
        // Methods
        /// <summary>
        /// 通过创建工厂创建缓存
        /// </summary>
        /// <returns></returns>
        public static ICache GetInstance()
        {

            ICache Cache = null;
            switch (Config.CacheName)
            {

                case "CoreCache": Cache = new CoreCache(); break;
                case "RedisCache": Cache = new RedisCache(); break;
                case "EasyCaching":
                    //var services = new ServiceCollection();
                    //services.AddCache(options => options.UseInMemory());
                    //var serviceProvider = services.BuildServiceProvider();
                    Cache = new EasyCaching();// serviceProvider.GetService<ICache>();
                    break;
                default: throw new Exception("请指定缓存类型！");
            }
            if (Cache == null)
            {
                return new CoreCache();
            }
            return Cache;
        }
    }


}
