using EasyCaching.Core;
using EasyCaching.Core.Configurations;
using EasyCaching.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;



namespace RoadFlow.Utility.Cache
{
    /// <summary>
    /// EasyCaching缓存
    /// </summary>
    public class EasyCaching : ICache
    {

        /// <summary>
        /// 缓存提供器
        /// </summary>
        private readonly IEasyCachingProvider _provider;

        /// <summary>
        /// 初始化缓存
        /// </summary>
        /// <param name="provider">EasyCaching缓存提供器</param>
        public EasyCaching()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddEasyCaching(x =>
            {
                x.UseInMemory();
                //    x.UseMemcached( options => { options.DBConfig.AddServer("127.0.0.1", 11211); }, "MyTest");
            });
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            var factory = serviceProvider.GetService<IEasyCachingProviderFactory>();
            _provider = factory.GetCachingProvider(EasyCachingConstValue.DefaultInMemoryName);
            //   _provider = factory.GetCachingProvider("MyTest");


            //  *****************Redis**********
            //IServiceCollection services = new ServiceCollection();
            //services.AddEasyCaching(x =>
            //    x.UseRedis(options => { options.DBConfig.Endpoints.Add(new ServerEndPoint("127.0.0.1", 6379)); },
            //        "redis1")
            //);
            //IServiceProvider serviceProvider = services.BuildServiceProvider();
            //var factory = serviceProvider.GetRequiredService<IEasyCachingProviderFactory>();
            //_provider = factory.GetCachingProvider("redis1");


            //IServiceCollection services = new ServiceCollection();
            //services.AddEasyCaching(option =>
            //{
            //    option.UseRedis(config =>
            //    {
            //        config.DBConfig = new RedisDBOptions
            //        {
            //            AllowAdmin = true
            //        };
            //        config.DBConfig.Endpoints.Add(new ServerEndPoint("127.0.0.1", 6380));
            //        config.DBConfig.Database = 12;
            //    }, "redis1");

            //    option.UseCSRedis(config =>
            //    {
            //        config.DBConfig = new CSRedisDBOptions
            //        {
            //            ConnectionStrings = new System.Collections.Generic.List<string>
            //            {
            //                "127.0.0.1:6388,defaultDatabase=12,poolsize=10"
            //            }
            //        };
            //    }, "redis2");
            //});


            //IServiceProvider serviceProvider = services.BuildServiceProvider();

            //var factory = serviceProvider.GetService<IEasyCachingProviderFactory>();

            //_e1 = factory.GetCachingProvider("redis1");
            //_e2 = factory.GetCachingProvider("redis2");
            //_r1 = factory.GetRedisProvider("redis1");
            //_r2 = factory.GetRedisProvider("redis2");



        }

        /// <summary>
        /// 获取过期时间间隔
        /// </summary>
        private TimeSpan GetExpiration(TimeSpan? expiration)
        {
            expiration = expiration ?? TimeSpan.FromHours(12);
            return expiration.SafeValue();
        }

        public object Get(string key)
        {
            //if(_provider.Get<object>(key).Value==null)
            // {
            //     return null;
            // }
            return _provider.Get<object>(key).Value;
        }

        public object Insert(string key, object obj)
        {

            DateTime dt = DateTime.Now.AddHours(12);
            return Insert(key, obj, dt);
        }

        public object Insert(string key, object obj, DateTime expiry)
        {
            if (obj == null)
            {
                return null;
            }
            var timeSpan = expiry - DateTime.Now;
            _provider.Set<object>(key, obj, timeSpan);

            return this.Get(key);
        }

        public void Remove(string key)
        {
            _provider.Remove(key);
        }


        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear()
        {
            _provider.Flush();
        }
    }

    /// <summary>
    /// EasyCaching缓存扩展
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 注册EasyCaching缓存操作
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configAction">配置操作</param>
        public static void AddCache(this IServiceCollection services, Action<EasyCachingOptions> configAction)
        {
            services.TryAddScoped<ICache, EasyCaching>();
            services.AddEasyCaching(configAction);
        }
    }


}
