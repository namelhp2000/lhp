using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Data.DataAutoFac
{
    public  class AppFactory
    {
        static AppFactory()
        {
            _container = new IocHelper();
            _container.RegisterType<IApp, App>("App");
            _container.RegisterType<IApp, App1>("App1");
          
        }

        private static IocHelper _container { get; }

        /// <summary>
        /// 获取指定的数据库帮助类
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="conStr">连接字符串</param>
        /// <returns></returns>
        public static IApp GetDbHelper(string name)
        {
          
           return _container.Resolve<IApp>(name);
        }

       
    }
}
