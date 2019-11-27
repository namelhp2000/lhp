using Microsoft.AspNetCore.Http;
using RoadFlow.Business.EnterpriseWeiXin;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCore
{
   

    #region 新的方法 RoadFlowCore 2.8.3工作流
    public class Current
    {
        // Properties
        public static string BaseUrl
        {
            get
            {
                return Tools.GetHttpHost(null);
            }
        }

        public static string ContentRootPath
        {
            get
            {
                return Tools.GetContentRootPath();
            }
        }

        public static DateTime DateTime
        {
            get
            {
                return DateTimeExtensions.Now;
            }
        }

        public static Guid EnterpriseWeiXinUserId
        {
            get
            {
                return RoadFlow.Business.EnterpriseWeiXin.Common.GetUserId();
            }
        }

        public static HttpContext HttpContext
        {
            get
            {
                return Tools.HttpContext;
               
            }
        }



        public static string Language
        {
            get
            {
                return "zh-CN";
            }
        }




        public static int LoginType
        {
            get
            {
                int? nullable = SessionExtensions.GetInt32(HttpContext.Session, "rf_login_type");
                if (nullable.HasValue)
                {
                    return nullable.Value;
                }
                int num = Tools.IsPhoneAccess(HttpContext.Request) ? 1 : 0;
                SessionExtensions.SetInt32(HttpContext.Session, "rf_login_type", num);
                return num;
            }
        }

        public static RoadFlow.Model.User User
        {
            get
            {
                RoadFlow.Model.User currentUser = RoadFlow.Business.User.CurrentUser;
                if (currentUser != null)
                {
                    return currentUser;
                }
                return RoadFlow.Business.EnterpriseWeiXin.Common.GetUser();
            }
        }

        public static Guid UserId
        {
            get
            {
                return RoadFlow.Business.User.CurrentUserId;
            }
        }

        public static Guid UserIdOrWeiXinId
        {
            get
            {
                Guid userId = UserId;
                if (!GuidExtensions.IsEmptyGuid(userId))
                {
                    return userId;
                }
                return EnterpriseWeiXinUserId;
            }
        }

        public static string UserName
        {
            get
            {
                RoadFlow.Model.User user = User;
                if (user != null)
                {
                    return user.Name;
                }
                return string.Empty;
            }
        }

        public static string WebRootPath
        {
            get
            {
                return Tools.GetWebRootPath();
            }
        }


        public static Guid WeiXinId
        {
            get
            {
                Guid guid;
                string str2;
                if (SessionExtensions.GetString(HttpContext.Session, "roadflow_weixin_userid").IsGuid(out guid))
                {
                    return guid;
                }
                if (!HttpContext.Request.Cookies.TryGetValue("roadflow_weixin_openid", out str2) || str2.IsNullOrWhiteSpace())
                {
                    return Guid.Empty;
                }
              RoadFlow.Model.  User byWeiXinOpenId = new RoadFlow.Business.User().GetByWeiXinOpenId(str2);
                if (byWeiXinOpenId == null)
                {
                    return Guid.Empty;
                }
                SessionExtensions.SetString(HttpContext.Session, "roadflow_weixin_userid", byWeiXinOpenId.Id.ToString());
                return byWeiXinOpenId.Id;
            }
        }


    }


    #endregion

}
