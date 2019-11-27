using Microsoft.Extensions.Configuration;


namespace RoadFlow.Utility
{
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public static class ConfigHelper
    {

        static ConfigHelper()
        {
            IConfiguration config = AutofacHelper.GetService<IConfiguration>();
            if (config == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(WebHelper.RootPath)//AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json");

                config = builder.Build();
            }

            _config = config;
        }

        private static IConfiguration _config { get; }

        /// <summary>
        /// 从AppSettings获取key的值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            return _config[key];
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="nameOfCon">连接字符串名</param>
        /// <returns></returns>
        public static string GetConnectionString(string nameOfCon)
        {
            return _config.GetConnectionString(nameOfCon);
        }
    }
}
