using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RoadFlow.Utility
{
    /// <summary>
    /// 配置项
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 初始化密码
        /// </summary>
        [CompilerGenerated]
        private static string InitUserPasswordk__BackingField = "111";
        /// <summary>
        /// 开启调试模式
        /// </summary>
        [CompilerGenerated]
        private static bool IsDebugk__BackingField = false;
        /// <summary>
        /// 登录地址
        /// </summary>
        [CompilerGenerated]
        private static string LoginUrlk__BackingField = "/Home/Login";
        /// <summary>
        /// 每页大小
        /// </summary>
        [CompilerGenerated]
        private static int PageSizek__BackingField = 15;
        /// <summary>
        /// 用户Session键
        /// </summary>
        [CompilerGenerated]
        private static string UserIdSessionKeyk__BackingField = "RoadFlowUserId";
        /// <summary>
        /// MySql连接字符串
        /// </summary>
        [CompilerGenerated]
        private static string ConnectionString_MySqlk__BackingField;
        /// <summary>
        /// Oracle连接字符串
        /// </summary>
        [CompilerGenerated]
        private static string ConnectionString_Oraclek__BackingField;
        /// <summary>
        /// SqlServer连接字符串
        /// </summary>
        [CompilerGenerated]
        private static string ConnectionString_SqlServerk__BackingField;
        /// <summary>
        /// 数据类型
        /// </summary>
        [CompilerGenerated]
        private static string DatabaseTypek__BackingField = "sqlserver";
        /// <summary>
        /// 企业微信的appid
        /// </summary>
        [CompilerGenerated]
        private static string Enterprise_WeiXin_AppIdk__BackingField;
        /// <summary>
        /// 系统中是否要使用企业微信
        /// </summary>
        [CompilerGenerated]
        private static bool Enterprise_WeiXin_IsUsek__BackingField;
        /// <summary>
        /// 系统的外网地址
        /// </summary>
        [CompilerGenerated]
        private static string Enterprise_WeiXin_WebUrlk__BackingField;
        /// <summary>
        /// 如果是开启了调试模式时，当用户登录失效时默认登录人员
        /// </summary>
        [CompilerGenerated]
        private static string DebugUserIdk__BackingField;




        /// <summary>
        /// 如果是开启了调试模式时，当用户登录失效时默认登录人员
        /// </summary>
        public static string DebugUserId
        {
            [CompilerGenerated]
            get
            {
                return DebugUserIdk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                DebugUserIdk__BackingField = value;
            }
        }















        /// <summary>
        /// 连接数据字符串 
        /// TODO数据库配置
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                switch (DatabaseType)
                {
                    case "sqlserver":
                        return ConnectionString_SqlServer;

                    case "mysql":
                        return ConnectionString_MySql;

                    case "oracle":
                        return ConnectionString_Oracle;
                    case "postgresql":
                        return ConnectionString_PostgreSql;
                    case "sqlite":
                        return ConnectionString_Sqlite;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// MySql连接字符串
        /// </summary>
        public static string ConnectionString_MySql
        {
            [CompilerGenerated]
            get
            {
                return ConnectionString_MySqlk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                ConnectionString_MySqlk__BackingField = value;
            }
        }

        /// <summary>
        /// Oracle连接字符串
        /// </summary>
        public static string ConnectionString_Oracle
        {
            [CompilerGenerated]
            get
            {
                return ConnectionString_Oraclek__BackingField;
            }
            [CompilerGenerated]
            set
            {
                ConnectionString_Oraclek__BackingField = value;
            }
        }

        /// <summary>
        /// SQLServer连接字符串
        /// </summary>
        public static string ConnectionString_SqlServer
        {
            [CompilerGenerated]
            get
            {
                return ConnectionString_SqlServerk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                ConnectionString_SqlServerk__BackingField = value;
            }
        }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static string DatabaseType
        {
            [CompilerGenerated]
            get
            {
                return DatabaseTypek__BackingField;
            }
            [CompilerGenerated]
            set
            {
                DatabaseTypek__BackingField = value;
            }
        }

        /// <summary>
        /// 企业微信的appid
        /// </summary>
        public static string Enterprise_WeiXin_AppId
        {
            [CompilerGenerated]
            get
            {
                return Enterprise_WeiXin_AppIdk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                Enterprise_WeiXin_AppIdk__BackingField = value;
            }
        }

        /// <summary>
        /// 系统中是否要使用企业微信
        /// </summary>
        public static bool Enterprise_WeiXin_IsUse
        {
            [CompilerGenerated]
            get
            {
                return Enterprise_WeiXin_IsUsek__BackingField;
            }
            [CompilerGenerated]
            set
            {
                Enterprise_WeiXin_IsUsek__BackingField = value;
            }
        }

        /// <summary>
        /// 系统的外网地址
        /// </summary>
        public static string Enterprise_WeiXin_WebUrl
        {
            [CompilerGenerated]
            get
            {
                return Enterprise_WeiXin_WebUrlk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                Enterprise_WeiXin_WebUrlk__BackingField = value;
            }
        }

        /// <summary>
        /// 初始化密码
        /// </summary>
        public static string InitUserPassword
        {
            [CompilerGenerated]
            get
            {
                return InitUserPasswordk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                InitUserPasswordk__BackingField = value;
            }
        }

        /// <summary>
        /// 开启调试模式
        /// </summary>
        public static bool IsDebug
        {
            [CompilerGenerated]
            get
            {
                return IsDebugk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                IsDebugk__BackingField = value;
            }
        }

        /// <summary>
        ///  登录地址
        /// </summary>
        public static string LoginUrl
        {
            [CompilerGenerated]
            get
            {
                return LoginUrlk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                LoginUrlk__BackingField = value;
            }
        }

        /// <summary>
        /// 页码大小
        /// </summary>
        public static int PageSize
        {
            [CompilerGenerated]
            get
            {
                return PageSizek__BackingField;
            }
            [CompilerGenerated]
            set
            {
                PageSizek__BackingField = value;
            }
        }
        /// <summary>
        /// 用户Session键
        /// </summary>
        public static string UserIdSessionKey
        {
            [CompilerGenerated]
            get
            {
                return UserIdSessionKeyk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                UserIdSessionKeyk__BackingField = value;
            }
        }
        /// <summary>
        /// 是否开启单点登录
        /// </summary>
        public static bool SingleLogin
        {
            [CompilerGenerated]
            get
            {
                return SingleLogink__BackingField;
            }
            [CompilerGenerated]
            set
            {
                SingleLogink__BackingField = value;
            }
        }

        /// <summary>
        /// 是否开启单点登录
        /// </summary>
        [CompilerGenerated]
        private static bool SingleLogink__BackingField = true;


        /// <summary>
        /// 附件及文件保存目录
        /// </summary>
        public static string FilePath
        {
            [CompilerGenerated]
            get
            {
                return FilePathk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                FilePathk__BackingField = value;
            }
        }
        /// <summary>
        /// 附件及文件保存目录
        /// </summary>
        [CompilerGenerated]
        private static string FilePathk__BackingField = @"d:/RoadFlowFiles";

        /// <summary>
        /// 是否将程序错误信息输出到浏览器
        /// </summary>
        public static int ShowError
        {
            [CompilerGenerated]
            get
            {
                return ShowErrork__BackingField;
            }
            [CompilerGenerated]
            set
            {
                ShowErrork__BackingField = value;
            }
        }

        /// <summary>
        /// 是否将程序错误信息输出到浏览器
        /// </summary>
        [CompilerGenerated]
        private static int ShowErrork__BackingField = 0;


        /// <summary>
        /// session过期时间，单位（分钟）
        /// </summary>
        public static int SessionTimeout
        {
            [CompilerGenerated]
            get
            {
                return SessionTimeoutk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                SessionTimeoutk__BackingField = value;
            }
        }
        /// <summary>
        /// session过期时间，单位（分钟）
        /// </summary>
        [CompilerGenerated]
        private static int SessionTimeoutk__BackingField = 20;


        /// <summary>
        /// 项目名称
        /// </summary>
        [CompilerGenerated]
        private static string ProjectName__BackingField = "RoadFlowCore.NET工作流平台";

        /// <summary>
        /// 项目名称
        /// </summary>
        public static string ProjectName
        {
            [CompilerGenerated]
            get
            {
                return ProjectName__BackingField;
            }
            [CompilerGenerated]
            set
            {
                ProjectName__BackingField = value;
            }
        }


        /// <summary>
        /// 是否显示登录地址
        /// </summary>
        public static bool IsLoginAddress
        {
            [CompilerGenerated]
            get
            {
                return IsLoginAddress__BackingField;
            }
            [CompilerGenerated]
            set
            {
                IsLoginAddress__BackingField = value;
            }
        }

        /// <summary>
        /// 是否显示登录地址
        /// </summary>
        [CompilerGenerated]
        private static bool IsLoginAddress__BackingField;




        /// <summary>
        /// 撤回邮件有效时间
        /// </summary>
        [CompilerGenerated]
        private static double MailWithdrawTime__BackingField = 24;


        /// <summary>
        /// 邮件撤回时间按小时设置
        /// </summary>
        public static double MailWithdrawTime
        {
            [CompilerGenerated]
            get
            {
                return MailWithdrawTime__BackingField;
            }
            [CompilerGenerated]
            set
            {
                MailWithdrawTime__BackingField = value;
            }
        }




        /// <summary>
        /// 系统表
        /// </summary>
        [CompilerGenerated]
        private static string SystemTables__BackingField;
        public static string SystemTables
        {
            [CompilerGenerated]
            get
            {
                return SystemTables__BackingField;
            }
            [CompilerGenerated]
            set
            {
                SystemTables__BackingField = value;
            }
        }
        /// <summary>
        /// 系统表
        /// </summary>
        public static List<string> systemDataTables
        {
            [CompilerGenerated]
            get
            {
                List<string> list = new List<string>();
                string str = SystemTables;
                if (!str.IsNullOrEmpty())
                {
                    foreach (string str2 in str.Split(new char[] { ',' }))
                    {
                        if (!str2.IsNullOrEmpty())
                        {
                            list.Add(str2);
                        }
                    }
                }
                return list;
            }

        }



        /// <summary>
        ///显示备注说明
        /// </summary>
        [CompilerGenerated]
        private static string IsRemarks__BackingField;
        /// <summary>
        /// 显示备注说明
        /// </summary>
        public static string IsRemarks
        {
            [CompilerGenerated]
            get
            {
                return IsRemarks__BackingField;
            }
            [CompilerGenerated]
            set
            {
                IsRemarks__BackingField = value;
            }
        }


        /// <summary>
        /// 发动机中心是否使用
        /// </summary>
        [CompilerGenerated]
        private static bool EngineCenter_IsUsek__BackingField;



        /// <summary>
        /// 发动机中心是否使用
        /// </summary>
        public static bool EngineCenter_IsUse
        {
            [CompilerGenerated]
            get
            {
                return EngineCenter_IsUsek__BackingField;
            }
            [CompilerGenerated]
            set
            {
                EngineCenter_IsUsek__BackingField = value;
            }
        }



        /// <summary>
        ///缓存设置
        /// </summary>
        [CompilerGenerated]
        private static string CacheName__BackingField;
        /// <summary>
        /// 缓存设置
        /// </summary>
        public static string CacheName
        {
            [CompilerGenerated]
            get
            {
                return CacheName__BackingField;
            }
            [CompilerGenerated]
            set
            {
                CacheName__BackingField = value;
            }
        }

        /// <summary>
        /// CookieName名称
        /// </summary>
        [CompilerGenerated]
        private static string CookieNamek__BackingField = "RoadFlowCore.Session";

        /// <summary>
        /// CookieName名称
        /// </summary>
        public static string CookieName
        {
            [CompilerGenerated]
            get
            {
                return CookieNamek__BackingField;
            }
            [CompilerGenerated]
            set
            {
                CookieNamek__BackingField = value;
            }
        }



        /// <summary>
        /// 文件扩展名
        /// </summary>
        [CompilerGenerated]
        private static string UploadFileExtNamesk__BackingField;




        /// <summary>
        /// 文件扩展名
        /// </summary>
        public static string UploadFileExtNames
        {
            [CompilerGenerated]
            get
            {
                return UploadFileExtNamesk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                UploadFileExtNamesk__BackingField = value;
            }
        }





        /// <summary>
        /// 前台安装数据库的链接方式
        /// </summary>
        [CompilerGenerated]
        private static string SQLServerconfigk__BackingField;





        /// <summary>
        /// 前台安装数据库的链接方式
        /// </summary>
        public static string SQLServerconfig
        {
            [CompilerGenerated]
            get
            {
                return SQLServerconfigk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                SQLServerconfigk__BackingField = value;
            }
        }



        /// <summary>
        /// 是否设置表单权限
        /// </summary>
        [CompilerGenerated]
        private static bool IsFormPower__BackingField;



        /// <summary>
        /// 开启调试模式
        /// </summary>
        public static bool IsFormPower
        {
            [CompilerGenerated]
            get
            {
                return IsFormPower__BackingField;
            }
            [CompilerGenerated]
            set
            {
                IsFormPower__BackingField = value;
            }
        }




        /// <summary>
        /// 是否显示在线日志
        /// </summary>
        public static bool IsLog
        {
            [CompilerGenerated]
            get
            {
                return IsLog__BackingField;
            }
            [CompilerGenerated]
            set
            {
                IsLog__BackingField = value;
            }
        }

        /// <summary>
        /// 是否显示在线日志
        /// </summary>
        [CompilerGenerated]
        private static bool IsLog__BackingField;


        /// <summary>
        /// PostgreSql连接字符串
        /// </summary>
        [CompilerGenerated]
        private static string ConnectionString_PostgreSql__BackingField;

        /// <summary>
        /// PostgreSql连接字符串
        /// </summary>
        public static string ConnectionString_PostgreSql
        {
            [CompilerGenerated]
            get
            {
                return ConnectionString_PostgreSql__BackingField;
            }
            [CompilerGenerated]
            set
            {
                ConnectionString_PostgreSql__BackingField = value;
            }
        }



        /// <summary>
        /// PostgreSql连接字符串
        /// </summary>
        [CompilerGenerated]
        private static string ConnectionString_Sqlite__BackingField;

        /// <summary>
        /// PostgreSql连接字符串
        /// </summary>
        public static string ConnectionString_Sqlite
        {
            [CompilerGenerated]
            get
            {
                return ConnectionString_Sqlite__BackingField;
            }
            [CompilerGenerated]
            set
            {
                ConnectionString_Sqlite__BackingField = value;
            }
        }



        /// <summary>
        /// 是否启动动态步骤
        /// </summary>
        [CompilerGenerated]
        private static bool EnableDynamicStepk__BackingField = false;




        /// <summary>
        /// 是否启动动态步骤
        /// </summary>
        public static bool EnableDynamicStep
        {
            [CompilerGenerated]
            get
            {
                return EnableDynamicStepk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                EnableDynamicStepk__BackingField = value;
            }
        }


        /// <summary>
        /// 获取当前语言
        /// </summary>
        [CompilerGenerated]
        private static string Language_Currentk__BackingField = "zh-CN";



        public static string Language_Current
        {
            [CompilerGenerated]
            get
            {
                return Language_Currentk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                Language_Currentk__BackingField = value;
            }
        }


        /// <summary>
        /// //是否要同步组织架构(1:同步 0:不同步)
        /// </summary>
        [CompilerGenerated]
        private static bool Enterprise_WeiXin_IsSyncOrgk__BackingField;


        /// <summary>
        /// //是否要同步组织架构(1:同步 0:不同步)
        /// </summary>
        public static bool Enterprise_WeiXin_IsSyncOrg
        {
            [CompilerGenerated]
            get
            {
                return Enterprise_WeiXin_IsSyncOrgk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                Enterprise_WeiXin_IsSyncOrgk__BackingField = value;
            }
        }


        [CompilerGenerated]
        private static string WeiXin_AppIdk__BackingField;



        public static string WeiXin_AppId
        {
            [CompilerGenerated]
            get
            {
                return WeiXin_AppIdk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                WeiXin_AppIdk__BackingField = value;
            }
        }





        [CompilerGenerated]
        private static string WeiXin_AppSecretk__BackingField;





        public static string WeiXin_AppSecret
        {
            [CompilerGenerated]
            get
            {
                return WeiXin_AppSecretk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                WeiXin_AppSecretk__BackingField = value;
            }
        }


        [CompilerGenerated]
        private static bool WeiXin_IsUsek__BackingField;






        public static bool WeiXin_IsUse
        {
            [CompilerGenerated]
            get
            {
                return WeiXin_IsUsek__BackingField;
            }
            [CompilerGenerated]
            set
            {
                WeiXin_IsUsek__BackingField = value;
            }
        }

        /// <summary>
        /// 微信公众号配置
        /// </summary>
        [CompilerGenerated]
        private static string WeiXin_WebUrlk__BackingField;




        public static string WeiXin_WebUrl
        {
            [CompilerGenerated]
            get
            {
                return WeiXin_WebUrlk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                WeiXin_WebUrlk__BackingField = value;
            }
        }



        /// <summary>
        /// 系统版本号
        /// </summary>
        [CompilerGenerated]
        private static string SystemVersionk__BackingField;





        /// <summary>
        /// 系统版本号
        /// </summary>
        public static string SystemVersion
        {
            [CompilerGenerated]
            get
            {
                return SystemVersionk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                SystemVersionk__BackingField = value;
            }
        }




        /// <summary>
        /// //是否以IFRAME方式集成(其它系统以IFRAME方式直接加载本系统页面，本系统独立部署)
        /// </summary>
        [CompilerGenerated]
        private static bool IsIntegrateIframek__BackingField;

        /// <summary>
        /// //是否以IFRAME方式集成(其它系统以IFRAME方式直接加载本系统页面，本系统独立部署)
        /// </summary>
        public static bool IsIntegrateIframe
        {
            [CompilerGenerated]
            get
            {
                return IsIntegrateIframek__BackingField;
            }
            [CompilerGenerated]
            set
            {
                IsIntegrateIframek__BackingField = value;
            }
        }




    }


}
