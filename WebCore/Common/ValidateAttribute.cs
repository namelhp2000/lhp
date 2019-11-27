using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RoadFlow.Business;
using RoadFlow.Business.EnterpriseWeiXin;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WebCore
{
 


    #region 流程2.8.5
    public class ValidateAttribute : Attribute, IActionFilter, IFilterMetadata
    {
        // Fields
        [CompilerGenerated]
        private bool CheckAppk__BackingField = true;
    [CompilerGenerated]
        private bool CheckLogink__BackingField = true;
    [CompilerGenerated]
        private bool CheckUrlk__BackingField = true;

    // Methods
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Guid empty = Guid.Empty;
            bool flag = true;
            string str = string.Empty;
            int loginType = Current.LoginType;
            if (this.CheckLogin)
            {
                empty = RoadFlow.Business.User.CurrentUserId;
                if (GuidExtensions.IsEmptyGuid(empty))
                {
                    flag = false;
                }
                else if ((Config.SingleLogin && !Config.IsDebug) && (loginType == 0))
                {
                    string str3;
                    RoadFlow.Model.OnlineUser user = RoadFlow.Business.OnlineUser.Get(empty, 0);
                    string str2 = context.HttpContext.Request.Cookies.TryGetValue("rf_login_uniqueid", out str3) ? str3 : "";
                    if ((user == null) || !user.UniqueId.Equals(str2))
                    {
                        flag = false;
                        str = (user == null) ? string.Empty : user.IP;
                    }
                }
            }
            if ((GuidExtensions.IsEmptyGuid(empty) && this.CheckEnterPriseWeiXinLogin) && (Current.WeiXinId.IsEmptyGuid() && !RoadFlow.Business.EnterpriseWeiXin.Common.CheckLogin(true)))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="登录验证错误";
                context.Result=result1;
            }
            else if (!flag && (loginType == 0))
            {
                if (Tools.IsAjax(context.HttpContext.Request))
                {
                    ContentResult result2 = new ContentResult();
                    result2.Content="{\"loginstatus\":-1, \"url\":\"\"}";
                    context.Result=result2;
                }
                else
                {
                    ContentResult result;
                    if (context.Controller.ToString().Contains("Controllers.HomeController"))
                    {
                        result = new ContentResult();
                        string[] textArray1 = new string[] { "<script>", str.IsNullOrWhiteSpace() ? "" : ((string)("alert('您的帐号已经在" + str + "登录!');")), "top.location='", Config.LoginUrl, "';</script>" };
                        result.Content=string.Concat((string[])textArray1);
                        result.ContentType="text/html";
                        context.Result=result;
                    } 
                    else
                    {
                        string uRL = Tools.GetURL(context.HttpContext.Request);
                        result = new ContentResult();
                        string[] textArray2 = new string[] { "<script>", str.IsNullOrWhiteSpace() ? "" : ((string)("alert('您的帐号已经在" + str + "登录!');")), "top.lastURL='", uRL, "';top.roadflowCurrentWindow=window;top.login();</script>" };
                        result.Content=string.Concat((string[])textArray2);
                        result.ContentType="text/html";
                        context.Result=result;
                    }
                }
            }
            else
            {
                RoadFlow.Business.OnlineUser.UpdateLast(empty, 0, "");
                if ((!this.CheckApp || this.ValidateApp(context.HttpContext.Request.Querys("appid"), empty)) && ((this.CheckUrl && !this.ValidateURL(context.HttpContext.Request)) && !Config.IsDebug))
                {
                    ContentResult result3 = new ContentResult();
                    result3.Content="URL检查错误";
                    context.Result=result3;
                }
            }
        }

        private bool ValidateApp(string appId, Guid userId)
        {
            Guid guid;
            return ((StringExtensions.IsGuid(appId, out guid) && !GuidExtensions.IsEmptyGuid(userId)) && new MenuUser().GetListByMenuId(guid).Exists(delegate (RoadFlow.Model.MenuUser p) {
                return p.Users.ContainsIgnoreCase(userId.ToString());
            }));
        }

        private bool ValidateURL(HttpRequest request)
        {
            string str = request.Headers["Referer"].ToString();
            if (str.IsNullOrWhiteSpace())
            {
                return false;
            }
            string str2 = request.Host.Host;
            Uri uri = new Uri(str);
            bool flag = str2.EqualsIgnoreCase(uri.Host);
            if (!flag)
            {
                Log.Add("URL检查错误-" + str2 + "--" + uri.Host, "", LogType.其它, "", "", "", "", "", "", "", "");
            }
            return flag;
        }

        // Properties
        public bool CheckApp
        {
            [CompilerGenerated]
            get
            {
                return CheckAppk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                CheckAppk__BackingField = value;
            }
        }

        public bool CheckEnterPriseWeiXinLogin { get; set; }

        public bool CheckLogin
        {
            [CompilerGenerated]
            get
            {
                return CheckLogink__BackingField;
            }
            [CompilerGenerated]
            set
            {
                CheckLogink__BackingField = value;
            }
        }

        public bool CheckUrl
        {
            [CompilerGenerated]
            get
            {
                return CheckUrlk__BackingField;
            }
            [CompilerGenerated]
            set
            {
                CheckUrlk__BackingField = value;
            }
        }
    }



    #endregion


}
