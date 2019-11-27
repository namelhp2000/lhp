using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;
using WebCore.Models;

namespace WebCore.Controllers
{
 


   


    #region 新的方法  RoadFlowCore 2.8.3工作流

    public class HomeController : Controller
    {
      
        public string CheckLogin()
        {
           

            string IP = "";
            string str = base.Request.Forms("Account");
            string str2 = base.Request.Forms("Password");
            string str3 = base.Request.Forms("VCode");
            string str4 = base.Request.Forms("Force");
            if (RoadFlow.Utility.Config.IsLoginAddress)
            {
                 IP = base.Request.Forms("IP");
                string City = base.Request.Forms("City");
            }
            string[] textArray1 = new string[] { "帐号：", str, " 密码：", str2, " 验证码：", str3 };
            string contents = string.Concat((string[])textArray1);
            User user = new User();
            if (str.IsNullOrWhiteSpace() || str2.IsNullOrWhiteSpace())
            {
                SessionExtensions.SetString(base.HttpContext.Session, "rf_isvalidatecode", "1");
                Log.Add("用户登录失败 - 帐号密码不能为空", contents, LogType.用户登录, "", "", "", "", "", "", "", "");
                return "{\"status\":0,\"msg\":\"帐号密码不能为空!\"}";
            }
            string str6 = SessionExtensions.GetString(base.HttpContext.Session, "rf_validatecode");
            if ("1".Equals(SessionExtensions.GetString(base.HttpContext.Session, "rf_isvalidatecode")) && !str6.EqualsIgnoreCase(str3))
            {
                SessionExtensions.SetString(base.HttpContext.Session, "rf_isvalidatecode", "1");
                return "{\"status\":0,\"msg\":\"验证码错误!\"}";
            }
            RoadFlow.Model.User byAccount = user.GetByAccount(str.Trim());
            if (byAccount == null)
            {
                SessionExtensions.SetString(base.HttpContext.Session, "rf_isvalidatecode", "1");
                Log.Add("用户登录失败 - 帐号:" + str, contents, LogType.用户登录, "", "", "", "", "", "", "", "");
                return "{\"status\":0,\"msg\":\"帐号错误!\"}";
            }
            if (byAccount.Password.IsNullOrWhiteSpace() || !byAccount.Password.EqualsIgnoreCase(user.GetMD5Password(byAccount.Id, str2)))
            {
                SessionExtensions.SetString(base.HttpContext.Session, "rf_isvalidatecode", "1");
                Log.Add("用户登录失败 - 帐号:" + str, contents, LogType.用户登录, "", "", "", "", "", "", "", "");
                return "{\"status\":0,\"msg\":\"密码错误!\"}";
            }
            if (user.IsFrozen(byAccount))
            {
                SessionExtensions.SetString(base.HttpContext.Session, "rf_isvalidatecode", "1");
                Log.Add("用户登录失败 - 用户已被冻结-" + byAccount.Account, "", LogType.用户登录, "", "", "", "", "", "", "", "");
                return "{\"status\":0,\"msg\":\"用户已被冻结!\"}";
            }
            if (Config.SingleLogin)
            {
                string str9;
                RoadFlow.Model.OnlineUser user4 = OnlineUser.Get(byAccount.Id, 0);
                string str8 = base.HttpContext.Request.Cookies.TryGetValue("rf_login_uniqueid", out str9) ? str9 : "";
                if (((user4 != null) && "0".Equals(str4)) && !user4.UniqueId.Equals(str8))
                {
                    return ("{\"status\":2,\"msg\":\"用户已在" + user4.IP + "登录，您要强行登录吗?\"}");
                }
            }
            base.HttpContext.Session.Remove("rf_isvalidatecode");
            SessionExtensions.SetString(base.HttpContext.Session, Config.UserIdSessionKey, byAccount.Id.ToString());
            string str7 = GuidExtensions.ToUpperString(Guid.NewGuid());
            CookieOptions options1 = new CookieOptions();
            options1.Expires=new DateTimeOffset?((DateTimeOffset)Current.DateTime.AddYears(1));
            base.HttpContext.Response.Cookies.Append("rf_login_uniqueid", str7.ToString(), options1);
            RoadFlow.Model.OnlineUser user1 = new RoadFlow.Model.OnlineUser();
            user1.UserId=byAccount.Id;
            user1.UserName = byAccount.Name;
            user1.UserOrganize = user.GetOrganizeMainShowHtml(byAccount.Id, false);

            user1.IP = Tools.GetIP() ;
            //NLog.LogManager.Configuration.Variables["IP"] = Tools.GetIP();
            //NLog.LogManager.Configuration.Variables["UserName"] = byAccount.Name;
            if (RoadFlow.Utility.Config.IsLoginAddress)
            {
                user1.City = NetExtension.GetLocation(IP);//Tools.GetOuterIP();
            }

            user1.LastTime=Current.DateTime;
            user1.LastUrl = base.Request.Path.HasValue ? base.Request.Path.Value : "";
            user1.LoginType = 0;
            user1.UniqueId = str7;
            user1.BrowseAgent = Tools.GetBrowseAgent();
            RoadFlow.Model.OnlineUser onlineUser = user1;
            onlineUser.LoginTime=onlineUser.LastTime;
            OnlineUser.Add(onlineUser);
            if (RoadFlow.Utility.Config.IsLoginAddress)
            {
                Log.Add1("用户登录成功-" + byAccount.Account, "", LogType.用户登录, "", "", "", "", "", "", "", "", user1.City);
            }
            else
            {
                Log.Add("用户登录成功-" + byAccount.Account, "", LogType.用户登录, "", "", "", "", "", "", "", "");
            }
            return "{\"status\":1,\"msg\":\"登录成功!\"}";
        }


      



        public string ClearSession()
        {
            OnlineUser.Remove(Current.UserId, -1);
            base.HttpContext.Session.Clear();
            return "";
        }

        [ResponseCache(Duration = 0, Location =  ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ErrorViewModel model1 = new ErrorViewModel
            {
                RequestId = ((Activity.Current == null) ? null : Activity.Current.Id) ?? base.HttpContext.TraceIdentifier
               // RequestId = ((Activity.Current == null) ? false : ((bool)Activity.Current.Id)) ?? base.HttpContext.TraceIdentifier
            };
            return this.View(model1);
        }

        [Validate(CheckApp = false)]
        public IActionResult Home()
        {
            return this.View();
        }

        [Validate(CheckUrl = false)]
        public IActionResult Index()
        {
            Guid userId = Current.UserId;
            RoadFlow.Model.User user = Current.User;
            if(user==null)
            {
                return this.Redirect("~/Home/Login");
            }
            User user2 = new User();
            Menu menu = new Menu();
            ValueTuple<RoadFlow.Model.Message, int> noRead = new Message().GetNoRead(userId);
            RoadFlow.Model.Message message = noRead.Item1;
            int num = noRead.Item2;
            string str = "";
            if ((message != null) && (num > 0))
            {
                string contents = message.Contents;
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)message.Id.ToString());
                obj1.Add("title", (message.Type == 1) ? "待办事项" : "未读消息");
                obj1.Add("contents", (JToken)contents);
                obj1.Add("count", (JToken)num);
                str = obj1.ToString(0, Array.Empty<JsonConverter>());
            }
            base.ViewData["noReadMsgHtml"]=str;
            base.ViewData["menu"]= menu.GetUserMenuHtml(userId);
            base.ViewData["menumin"]= menu.GetUserMinMenuHtml(userId);
            base.ViewData["headImage"]= user2.GetHeadImageSrc(user, Current.WebRootPath);
            base.ViewData["userName"]= user.Name;
            base.ViewData["currentDate"]= DateTimeExtensions.ToLongDate2(DateTimeExtensions.Now);
            base.ViewData["loginURL"]= Config.LoginUrl;
            base.ViewData["rootDir"]= base.Url.Content("~/").Equals("/") ? "" : base.Url.Content("~/");
            return this.View();
        }

        public IActionResult Login()
        {
           // RoadFlow.Cache.IO.Insert("rf_core_theme", "blue", new DateTime(2099, 1, 1));
            var s = RoadFlow.Utility.Cache.IO.Get("rf_core_theme");
            string str = SessionExtensions.GetString(base.HttpContext.Session, "rf_validatecode");
            base.ViewData["isVcode"]= str.IsNullOrWhiteSpace() ? "0" : str;
            base.ViewData["isSession"]= base.Request.Querys("issession");
            base.ViewData["IsLoginAddress"] = Config.IsLoginAddress?"1":"0";
            return this.View();
        }

        public IActionResult Login1()
        {
            string str = SessionExtensions.GetString(base.HttpContext.Session, "rf_validatecode");
            base.ViewData["isVcode"]= str.IsNullOrWhiteSpace() ? "0" : str;
            return this.View();
        }

        [Validate(CheckApp = false)]
        public string Menu()
        {
            Guid guid2;
            Guid guid = StringExtensions.IsGuid(base.Request.Querys("userid"), out guid2) ? guid2 : Current.UserId;
            bool flag = "1".Equals(base.Request.Querys("showsource"));
            if (GuidExtensions.IsEmptyGuid(guid))
            {
                return "[]";
            }
            return new Menu().GetUserMenuJsonString(guid, base.Url.Content("~/").TrimEnd('/'), flag);
        }

        [Validate(CheckApp = false)]
        public string MenuRefresh()
        {
            Guid guid2;
            Guid guid3;
            string str = base.Request.Querys("userid");
            string str2 = base.Request.Querys("refreshid");
            bool flag = "1".Equals(base.Request.Querys("showsource"));
            Guid guid = StringExtensions.IsGuid(str, out guid2) ? guid2 : Current.UserId;
            if (StringExtensions.IsGuid(str2, out guid3) && !GuidExtensions.IsEmptyGuid(guid3))
            {
                return new Menu().GetUserMenuRefreshJsonString(guid, guid3, base.Url.Content("~/").TrimEnd('/'), flag);
            }
            return "[]";
        }

        [Validate(CheckApp = false)]
        public string MenuRefresh1()
        {
            Guid guid2;
            string str = base.Request.Querys("refreshid");
            string str2 = base.Request.Querys("isrefresh1");
            Guid userId = Current.UserId;
            if (StringExtensions.IsGuid(str, out guid2) && GuidExtensions.IsNotEmptyGuid(guid2))
            {
                return new Menu().GetUserMenuChilds(userId, guid2, base.Url.Content("~/").TrimEnd('/'), str2);
            }
            return "";
        }

        public IActionResult Quit()
        {
            this.ClearSession();
            return this.Redirect("~/Home/Login");
        }

        public void VCode()
        {
            #region 新的验证码方法

            //var captchaCode = ImageCaptchaHelper.GenerateCaptchaCode();
            //var result = ImageCaptchaHelper.GenerateCaptcha(100, 32, captchaCode);


            //MemoryStream validateImg = new MemoryStream(result.CaptchaByteData);
            //SessionExtensions.SetString(base.HttpContext.Session, "rf_validatecode", captchaCode);
            //ResponseExtensions.Clear(base.Response);
            //base.Response.ContentType = "image/png";
            //base.Response.Body.Write(result.CaptchaByteData, 0, (int)validateImg.Length);
            //base.Response.Body.Flush();
            //base.Response.Body.Close();

            #endregion

            #region 旧的方法

            string str2;
            string str = Current.WebRootPath + "/RoadFlowResources/images/vcodebg.png";
            MemoryStream validateImg = Tools.GetValidateImg(out str2, str);
            SessionExtensions.SetString(base.HttpContext.Session, "rf_validatecode", str2);
            ResponseExtensions.Clear(base.Response);
            base.Response.ContentType = "image/png";
            base.Response.Body.WriteAsync(validateImg.GetBuffer(), 0, (int)validateImg.Length);
            base.Response.Body.FlushAsync(); 
            base.Response.Body.Close();
           
            #endregion
        }


        [Validate(CheckApp = false)]
        public void ThemeName()
        {
            string str = base.Request.Querys("themeName");
            object b = RoadFlow.Utility.Cache.IO.Get("rf_core_theme");
            if(b.IsNullOrEmpty() || str!=b.ToString().ToLower())
            {
                var s = RoadFlow.Utility.Cache.IO.Insert("rf_core_theme", str, new DateTime(2099, 1, 1));
            }
            
        }


    }


   


    #endregion
}


