using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Data.RapidDevelopment
{
    public  class FactoryDataType
    {
        static FactoryDataType()
        {
            _container = new IocHelper();
            _container.RegisterType<RapidHelper, SqlServerHelper>(DatabaseType.SqlServer.ToString());
            _container.RegisterType<RapidHelper, MySqlHelper>(DatabaseType.MySql.ToString());
            _container.RegisterType<RapidHelper, PostgreSqlHelper>(DatabaseType.PostgreSql.ToString());
        }

        private static IocHelper _container { get; }

        /// <summary>
        /// 获取指定的数据库帮助类
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="conStr">连接字符串</param>
        /// <returns></returns>
        public static RapidHelper GetDbHelper(DatabaseType dbType, string conStr)
        {
          
           return _container.Resolve<RapidHelper>(dbType.ToString(), conStr);
        }

        /// <summary>
        /// 获取指定的数据库帮助类
        /// </summary>
        /// <param name="dbType">数据库类型字符串</param>
        /// <param name="conStr">连接字符串</param>
        /// <returns></returns>
        public static RapidHelper GetDbHelper(string dbTypeStr, string conStr)
        {
            DatabaseType dbType = DbProviderFactoryHelper.DbTypeStrToDbType(dbTypeStr);
            return GetDbHelper(dbType, conStr);
        }
    }
}
