using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoadFlow.Utility.SMS.LuoSiMao;

namespace RoadFlow.Utility.SMS.Extensions
{
    public static class SMSExtensions
    {
        /// <summary>
        /// 注册LuoSiMao短信服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="key">密钥</param>
        public static void AddLuoSiMao(this IServiceCollection services, string key)
        {
            services.TryAddSingleton<ISmsConfigProvider>(new RoadFlow.Utility.SMS.LuoSiMao.SmsConfigProvider(key));
        }
    }
}
