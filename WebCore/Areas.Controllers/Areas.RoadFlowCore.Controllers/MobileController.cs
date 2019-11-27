using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class MobileController : Controller
    {
        // Methods
        [ValidateAntiForgeryToken]
        public string CheckLogin()
        {
            string str3;
            string str = base.Request.Forms("account");
            string password = base.Request.Forms("pass");
            if (base.Request.Cookies.TryGetValue("roadflow_weixin_openid", out str3))
            {
                str3.IsNullOrWhiteSpace();
            }
            if (str.IsNullOrWhiteSpace())
            {
                return "帐号为空";
            }
            User user = new User();
          RoadFlow.Model.  User byAccount = user.GetByAccount(str.Trim());
            if (byAccount == null)
            {
                return "帐号错误";
            }
            if (!byAccount.Password.Equals(user.GetMD5Password(byAccount.Id, password)))
            {
                return "密码错误";
            }
            byAccount.WeiXinOpenId = str3;
            user.Update(byAccount);
            SessionExtensions.SetString(base.HttpContext.Session, "roadflow_weixin_userid", byAccount.Id.ToString());
            return "1";
        }

        [Validate(CheckLogin = false, CheckApp = false, CheckUrl = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult CompletedTask()
        {
            return this.View();
        }

        [Validate(CheckLogin = false, CheckApp = false, CheckUrl = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult Document()
        {
            int num;
            string title = "";
            Guid enterpriseWeiXinUserId = Current.EnterpriseWeiXinUserId;
            Doc doc = new Doc();
            DocDir dir = new DocDir();
            string order = "DocRank,WriteTime DESC";
            List<ValueTuple<string, DataTable>> list = new List<ValueTuple<string, DataTable>>();
            DataTable table = doc.GetPagerList(out num, 0x186a0, 1, enterpriseWeiXinUserId, title, "", "", "", order, 0);
            if (table.Rows.Count> 0)
            {
                list.Add(new ValueTuple<string, DataTable>("未读文档", table));
            }
            foreach (RoadFlow.Model.DocDir dir2 in dir.GetReadDirs(enterpriseWeiXinUserId))
            {
                int num2;
                DataTable table2 = doc.GetPagerList(out num2, 5, 1, enterpriseWeiXinUserId, title, "'" + dir2.Id.ToString() + "'", "", "", order, -1);
                if (table2.Rows.Count > 0)
                {
                    string str3 = dir.GetAllParentNames(dir2.Id, true, false, @"\");
                    list.Add(new ValueTuple<string, DataTable>(str3, table2));
                }
            }
            base.ViewData["noReadDoc"]= table;
            return this.View(list);
        }

        [Validate(CheckLogin = false, CheckApp = false, CheckUrl = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult DocumentDir()
        {
            string str = base.Request.Querys("dirid");
            base.ViewData["dirid"]= str;
            base.ViewData["Title"]= new DocDir().GetAllParentNames(str.ToGuid(), true, false, @"\");
            return this.View();
        }

        [Validate(CheckLogin = false, CheckApp = false, CheckUrl = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult DocumentShow()
        {
            Guid guid2;
            Guid enterpriseWeiXinUserId = Current.EnterpriseWeiXinUserId;
            if (!base.Request.Querys("docid").IsGuid(out guid2))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="ID错误";
                return result1;
            }
            RoadFlow.Model.Doc doc = new Doc().Get(guid2);
            if (doc == null)
            {
                ContentResult result2 = new ContentResult();
                result2.Content="未找到文档";
                return result2;
            }
            new Doc().UpdateReadCount(doc);
            new DocUser().UpdateIsRead(doc.Id, enterpriseWeiXinUserId, 1);
            return this.View(doc);
        }

        public IActionResult GetUserAccount()
        {
            string str = base.Request.Querys("code");
            if (str.IsNullOrWhiteSpace())
            {
                ContentResult result1 = new ContentResult();
                result1.Content="身份验证失败";
                return result1;
            }
            string userAccountByCode =RoadFlow.Business.EnterpriseWeiXin. Common.GetUserAccountByCode(str);
            if (userAccountByCode.IsNullOrWhiteSpace())
            {
                ContentResult result2 = new ContentResult();
                result2.Content="身份验证失败";
                return result2;
            }
            User user = new User();
            RoadFlow.Model.User byAccount = user.GetByAccount(userAccountByCode);
            if (byAccount == null)
            {
                ContentResult result3 = new ContentResult();
                result3.Content="没有找到帐号对应的人员";
                return result3;
            }
            if (user.IsFrozen(byAccount))
            {
                ContentResult result4 = new ContentResult();
                result4.Content="该人员状态异常";
                return result4;
            }
            SessionExtensions.SetString(base.HttpContext.Session, RoadFlow.Business.EnterpriseWeiXin.Common.SessionKey, byAccount.Id.ToString());
            string str3 = SessionExtensions.GetString(base.HttpContext.Session, "EnterpriseWeiXin_LastURL");
            string str4 = Guid.NewGuid().ToUpperString();
            RoadFlow.Model.OnlineUser onlineUser = new RoadFlow.Model.OnlineUser
            {
                UserId = byAccount.Id,
                UserName = byAccount.Name,
                UserOrganize = new User().GetOrganizeMainShowHtml(byAccount.Id, false),
                IP = Tools.GetIP(),
                LastTime = Current.DateTime,
                LastUrl = str3,
                LoginType = 1,
                UniqueId = str4,
                BrowseAgent = Tools.GetBrowseAgent()
            };
            onlineUser.LoginTime = onlineUser.LastTime;
            OnlineUser.Add(onlineUser);
            if (!str3.IsNullOrWhiteSpace())
            {
                return this.Redirect(str3);
            }
            ContentResult result5 = new ContentResult();
            result5.Content="";
            return result5;
        }

        public IActionResult Index()
        {
            string str;
            int num;
            if (base.Request.Cookies.TryGetValue("roadflow_weixin_openid", out str))
            {
                str.IsNullOrWhiteSpace();
            }
            Guid weiXinId = Current.WeiXinId;
            if (weiXinId.IsEmptyGuid())
            {
                return this.Redirect(base.Url.Content("~/RoadFlowCore/Mobile/Login"));
            }
            User user = new User();
            RoadFlow.Model.User user2 = user.Get(weiXinId);
            if (user2 == null)
            {
                return this.Redirect(base.Url.Content("~/RoadFlowCore/Mobile/Login"));
            }
            DataTable table = new FlowTask().GetWaitTask(5, 1, weiXinId, "", "", "", "", "ReceiveTime DESC", out num, 0);
            base.ViewData["waitTasks"]= table;
            base.ViewData["waitTaskCount"]= (int)num;
            base.ViewData["headImage"]= user.GetHeadImageSrc(user2, Current.WebRootPath);
            base.ViewData["UserName"]= (user2 == null) ? string.Empty : user2.Name;
            base.ViewData["UserDept"]= (user2 == null) ? string.Empty : user.GetOrganizeMainShowHtml(user2.Id, false);
            return this.View();
        }

        public IActionResult Login()
        {
            return this.View();
        }

        [Validate(CheckLogin = false, CheckApp = false, CheckUrl = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult Message()
        {
            return this.View();
        }

        [Validate(CheckLogin = false, CheckApp = false, CheckUrl = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult StartFlow()
        {
            return this.View();
        }

        [Validate(CheckLogin = false, CheckApp = false, CheckUrl = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult WaitTask()
        {
            return this.View();
        }

        public RedirectResult WeiXin_AuthorizeUrl()
        {
            string url = Config.WeiXin_WebUrl + "/RoadFlowCore/Mobile/WeiXin_GetCode";
            string[] textArray1 = new string[] { "https://open.weixin.qq.com/connect/oauth2/authorize?appid=", Config.WeiXin_AppId, "&redirect_uri=", url.UrlEncode(), "&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect" };
            string str2 = string.Concat((string[])textArray1);
            return this.Redirect(str2);
        }

        public RedirectResult WeiXin_GetCode()
        {
            string str = base.Request.Querys("code");
            string[] textArray1 = new string[] { "https://api.weixin.qq.com/sns/oauth2/access_token?appid=", Config.WeiXin_AppId, "&secret=", Config.WeiXin_AppSecret, "&code=", str, "&grant_type=authorization_code" };
            string contents = HttpHelper.HttpGet(string.Concat((string[])textArray1), null, 0);
            JObject obj2 = JObject.Parse(contents);
            if (obj2.ContainsKey("errcode"))
            {
                Log.Add("获取微信公众号网页授权access_token错误", contents, LogType.系统异常, "", "", "", "", "", "", "", "");
            }
            string str4 = obj2.Value<string>("openid");
            CookieOptions options1 = new CookieOptions();
            options1.Expires=new DateTimeOffset(DateTime.MaxValue);
            base.Response.Cookies.Append("roadflow_weixin_openid", str4, options1);
            return this.Redirect(base.Url.Content("~/RoadFlowCore/Mobile/Index"));
        }
    }


 









}
