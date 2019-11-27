using RoadFlow.Business.EnterpriseWeiXin;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace RoadFlow.Business
{
    public class Log
    {
        // Methods
        public static void Add(RoadFlow.Model.Log log)
        {
            Task.Run(delegate {
                AddLog(log);
            });
        }
        private static readonly Dictionary<string, string> EN_US_Type;
        private static readonly Dictionary<string, string> ZH_Type;





        // Methods
        static Log()
        {
            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
            dictionary1.Add("用户登录", "用戶登錄");
            dictionary1.Add("系统管理", "系統管理");
            dictionary1.Add("流程管理", "流程管理");
            dictionary1.Add("表单管理", "表單管理");
            dictionary1.Add("流程运行", "流程運行");
            dictionary1.Add("系统异常", "系統異常");
            dictionary1.Add("其它", "其它");
            ZH_Type = dictionary1;
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
            dictionary2.Add("用户登录", "User login");
            dictionary2.Add("系统管理", "System management");
            dictionary2.Add("流程管理", "Workflow management");
            dictionary2.Add("表单管理", "Form management");
            dictionary2.Add("流程运行", "Workflow run");
            dictionary2.Add("系统异常", "Exception");
            dictionary2.Add("其它", "Other");
            EN_US_Type = dictionary2;
        }





        public static void Add(Exception err, string title = "")
        {
            RoadFlow.Model.Log log = new RoadFlow.Model.Log
            {
                Id = Guid.NewGuid(),
                Title = title.IsNullOrWhiteSpace() ? err.Message : title,
                Type = GetLogType("系统异常", Tools.GetCurrentLanguage()),
                IPAddress = Tools.GetIP(),
                URL = Tools.GetAbsoluteURL(null),
                WriteTime = DateTimeExtensions.Now,
                Referer = Tools.GetReferer()
            };
            Guid currentUserId = User.CurrentUserId;
            if (currentUserId.IsEmptyGuid())
            {
                currentUserId = Common.GetUserId();
            }
            if (currentUserId.IsNotEmptyGuid())
            {
                log.UserId = new Guid?(currentUserId);
            }
            RoadFlow.Model.User currentUser = User.CurrentUser;
            if (currentUser == null)
            {
                currentUser = Common.GetUser();
            }
            if (currentUser != null)
            {
                log.UserName = currentUser.Name;
            }
            log.Contents = err.StackTrace;
            log.Others = err.Source + "（" + err.Message + "）";
            log.BrowseAgent = Tools.GetBrowseAgent();
            AddLog(log);
        }


        public static void Add(string title, string contents = "", LogType type = LogType.其它 , string oldContents = "", string newContents = "", string others = "", string browseAgent = "", string ipAddress = "", string url = "", string userId = "", string userName = "")
        {
            Guid guid;
            RoadFlow.Model.Log log = new RoadFlow.Model.Log
            {
                Id = Guid.NewGuid(),
                Title = title,
                Type = type.ToString(),
                UserId = new Guid?(userId.IsGuid(out guid) ? guid : User.CurrentUserId),
                UserName = userName.IsNullOrWhiteSpace() ? User.CurrentUserName : userName,
                IPAddress = ipAddress.IsNullOrWhiteSpace() ? Tools.GetIP() : ipAddress,
                URL = url.IsNullOrWhiteSpace() ? Tools.GetAbsoluteURL(null) : url,
                WriteTime = DateTimeExtensions.Now,
                Referer = Tools.GetReferer()
            };
            if (!contents.IsNullOrWhiteSpace())
            {
                log.Contents = contents;
            }
            if (!others.IsNullOrWhiteSpace())
            {
                log.Others = others;
            }
            if (!oldContents.IsNullOrWhiteSpace())
            {
                log.OldContents = oldContents;
            }
            if (!newContents.IsNullOrWhiteSpace())
            {
                log.NewContents = newContents;
            }
            log.BrowseAgent = browseAgent.IsNullOrWhiteSpace() ? Tools.GetBrowseAgent() : browseAgent;
            AddLog(log);
        }

        public static void Add1(string title, string contents = "", LogType type = LogType.其它, string oldContents = "", string newContents = "", string others = "", string browseAgent = "", string ipAddress = "", string url = "", string userId = "", string userName = "",string CityAddress="")
        {
            Guid guid;
            RoadFlow.Model.Log log = new RoadFlow.Model.Log
            {
                Id = Guid.NewGuid(),
                Title = title,
                Type = type.ToString(),
                UserId = new Guid?(userId.IsGuid(out guid) ? guid : User.CurrentUserId),
                UserName = userName.IsNullOrWhiteSpace() ? User.CurrentUserName : userName,
                IPAddress = ipAddress.IsNullOrWhiteSpace() ? Tools.GetIP() : ipAddress,
                CityAddress = CityAddress,
                URL = url.IsNullOrWhiteSpace() ? Tools.GetAbsoluteURL(null) : url,
                WriteTime = DateTimeExtensions.Now,
                Referer = Tools.GetReferer()
            };
            if (!contents.IsNullOrWhiteSpace())
            {
                log.Contents = contents;
            }
            if (!others.IsNullOrWhiteSpace())
            {
                log.Others = others;
            }
            if (!oldContents.IsNullOrWhiteSpace())
            {
                log.OldContents = oldContents;
            }
            if (!newContents.IsNullOrWhiteSpace())
            {
                log.NewContents = newContents;
            }
            log.BrowseAgent = browseAgent.IsNullOrWhiteSpace() ? Tools.GetBrowseAgent() : browseAgent;
            AddLog(log);
        }


        public static string GetLogType(string type, string language)
        {
            string str;
            if (language != "zh-CN")
            {
                if (language != "zh")
                {
                    string str2;
                    if (language != "en-US")
                    {
                        return type;
                    }
                    if (!EN_US_Type.TryGetValue(type, out str2))
                    {
                        return type;
                    }
                    return str2;
                }
            }
            else
            {
                return type;
            }
            if (!ZH_Type.TryGetValue(type, out str))
            {
                return type;
            }
            return str;
        }



        private static void AddLog(RoadFlow.Model.Log log)
        {
            new RoadFlow.Data.Log().Add(log);
        }

        public RoadFlow.Model.Log Get(Guid id)
        {
            return new RoadFlow.Data.Log().Get(id);
        }

        public DataTable GetPagerList(out int count, int size, int number, string title, string type, string userId, string date1, string date2, string order)
        {
            return new RoadFlow.Data.Log().GetPagerList(out count, size, number, title, type, userId, date1, date2, order);
        }

        public string GetTypeOptions(string value = "")
        {
            StringBuilder builder = new StringBuilder();
            foreach (object obj2 in Enum.GetValues((Type)typeof(LogType)))
            {
                builder.AppendFormat("<option value=\"{0}\" {1}>{0}</option>", obj2, obj2.ToString().Equals(value) ? "selected=\"selected\"" : "");
            }
            return builder.ToString();
        }

       
    }


}
