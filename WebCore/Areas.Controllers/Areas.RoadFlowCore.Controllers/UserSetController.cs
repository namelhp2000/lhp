using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class UserSetController : Controller
    {
        // Methods
        [Validate]
        public IActionResult EditPass()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate]
        public IActionResult Info()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View(Current.User);
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveEditPass()
        {
            string str = base.Request.Forms("oldpass").Trim();
            string str2 = base.Request.Forms("newpass").Trim();
            string str3 = base.Request.Forms("newpass1").Trim();
            RoadFlow.Model.User user = Current.User;
            if (user == null)
            {
                return "未找到您的登录信息，请重新登录!";
            }
            if (!str2.Equals(str3))
            {
                return "两次输入密码不一致!";
            }
            User user2 = new User();
            if (!user.Password.Equals(user2.GetMD5Password(user.Id, str)))
            {
                return "旧密码不正确!";
            }
            user.Password = user2.GetMD5Password(user.Id, str2);
            user2.Update(user);
            return "密码修改成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveInfo()
        {
            RoadFlow.Model.User user = Current.User;
            if (user == null)
            {
                return "未找到您的登录信息，请重新登录!";
            }
            user.Tel = base.Request.Forms("Tel");
            user.Mobile = base.Request.Forms("MobilePhone");
            user.Fax = base.Request.Forms("Fax");
            user.Email = base.Request.Forms("Email");
            user.QQ = base.Request.Forms("QQ");
            user.WeiXin = base.Request.Forms("WeiXin");
            user.OtherTel = base.Request.Forms("OtherTel");
            user.Note = base.Request.Forms("Note");
            new User().Update(user);
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveShortcut()
        {
            string str = base.Request.Forms("menuid");
            UserShortcut shortcut = new UserShortcut();
            Guid userId = Current.UserId;
            List<RoadFlow.Model.UserShortcut> list = new List<RoadFlow.Model.UserShortcut>();
            int num = 0;
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid2;
                if (StringExtensions.IsGuid(str2, out guid2))
                {
                    RoadFlow.Model.UserShortcut shortcut1 = new RoadFlow.Model.UserShortcut();
                    shortcut1.Id=Guid.NewGuid();
                    shortcut1.MenuId=guid2;
                    shortcut1.UserId=userId;
                    shortcut1.Sort = num += 5;
                    RoadFlow.Model.UserShortcut shortcut2 = shortcut1;
                    list.Add(shortcut2);
                }
            }
            shortcut.Add(list.ToArray(), userId);
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveShortcutSort()
        {
            string str = base.Request.Forms("sort");
            UserShortcut shortcut = new UserShortcut();
            List<RoadFlow.Model.UserShortcut> list = new List<RoadFlow.Model.UserShortcut>();
            int num = 0;
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.UserShortcut shortcut2 = shortcut.Get(guid);
                    if (shortcut2 != null)
                    {
                        shortcut2.Sort = num += 5;
                        list.Add(shortcut2);
                    }
                }
            }
            shortcut.Update(list.ToArray());
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveUserHead()
        {
            string str = base.Request.Forms("x");
            string str2 = base.Request.Forms("y");
            string str3 = base.Request.Forms("x2");
            string str4 = base.Request.Forms("y2");
            string str5 = base.Request.Forms("w");
            string str6 = base.Request.Forms("h");
         //   string str7 = Current.ContentRootPath +  (base.Request.Forms("img") ?? "").DESDecrypt();
            string str7 = UserFile.RootPath + (base.Request.Forms("img") ?? "").DESDecrypt();
            RoadFlow.Model.User user = Current.User;
            if (str7.IsNullOrEmpty() || !System.IO.File.Exists(str7))
            {
                return "文件不存在!";
            }
            string[] textArray1 = new string[] { "/RoadFlowResources/images/userHeads/", user.Id.ToString("N"), "/", Guid.NewGuid().ToString("N"), ".jpg" };
            string str8 = base.Url.Content(string.Concat((string[])textArray1));
            bool flag = ImgHelper.CutAvatar(str7, Current.WebRootPath + str8, str.ToInt(-2147483648), str2.ToInt(-2147483648), str5.ToInt(-2147483648), str6.ToInt(-2147483648));
            if (!(user is null) & flag)
            {
                user.HeadImg = str8;
                new User().Update(user);
                return "保存成功!";
            }
            return "保存失败!";
        }

        public IActionResult Shortcut()
        {
            string menuTreeTableHtml = string.Empty;
            Menu menu = new Menu();
            RoadFlow.Model.User user = Current.User;
            menuTreeTableHtml = menu.GetMenuTreeTableHtml(string.Empty, new Guid?(user.Id));
            List<RoadFlow.Model.UserShortcut> listByUserId = new UserShortcut().GetListByUserId(user.Id);
            base.ViewData["shortcuts"] = listByUserId;
            base.ViewData["shortcutJson"]= JsonConvert.SerializeObject(listByUserId);

           // base.ViewData["shortcutJson"]= JsonConvert.SerializeObject(listByUserId);
            base.ViewData["menuhtml"]= menuTreeTableHtml;
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate]
        public IActionResult Sign()
        {
            string webRootPath = Current.WebRootPath;
            RoadFlow.Model.User user = Current.User;
            string path = webRootPath + "/RoadFlowResources/images/userSigns/" + user.Id.ToString("N");
            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                info.Create();
            }
            string str3 = string.Empty;
            if ((base.Request.Method.EqualsIgnoreCase("post") && (base.Request.Form.Files != null)) && (base.Request.Form.Files.Count > 0))
            {
                IFormFile file = base.Request.Form.Files[0];
                if (file.Length > 0L)
                {
                    string extension = Path.GetExtension(file.FileName);
                    if (extension.IsNullOrWhiteSpace() || ((!extension.EqualsIgnoreCase(".gif") && !extension.EqualsIgnoreCase(".jpg")) && !extension.EqualsIgnoreCase(".png")))
                    {
                        str3 = "alert('只能上传gif,jpg,png类型的图片文件!'); window.location = window.location;";
                        base.ViewData["script"]= str3;
                        return this.View();
                    }
                    string contents = path + "/default.png";
                    using (FileStream stream = System.IO .File.Create(contents))
                    {
                        file.CopyTo((Stream)stream);
                        stream.Flush();
                    }
                    Log.Add("修改了签名", contents, LogType.系统管理, "", "", "", "", "", "", "", "");
                    str3 = "alert('上传成功!'); window.location = window.location;";
                    base.ViewData["script"]= str3;
                }
            }
            if (base.Request.Method.EqualsIgnoreCase("post") && !base.Request.Forms("reset").IsNullOrEmpty())
            {
                string str7 = path + "/default.png";
                if (System.IO.File.Exists(str7))
                {
                   System.IO. File.Delete(str7);
                    Log.Add("恢复了签名", str7, LogType.系统管理, "", "", "", "", "", "", "", "");
                }
                str3 = "alert('已恢复为默认签名!'); window.location = window.location;";
                base.ViewData["script"]= str3;
            }
            if (!System.IO.File.Exists(path + "/default.png"))
            {
               new User().CreateSignImage(user.Name).Save(path + "/default.png", ImageFormat.Png);
            }
            string str4 = base.Url.Content("~/RoadFlowResources/images/userSigns/" + user.Id.ToString("N") + "/default.png");
          

            base.ViewData["signSrc"]= str4;
            return this.View();
        }

        public IActionResult UserHeader()
        {
            RoadFlow.Model.User user = Current.User;
            string str = string.Empty;
            if (!user.HeadImg.IsNullOrWhiteSpace() && System.IO.File.Exists(Current.WebRootPath + user.HeadImg))
            {
                str = base.Url.Content(user.HeadImg);
            }
            if (str.IsNullOrWhiteSpace())
            {
                str = base.Url.Content("~/RoadFlowResources/images/userHeads/default.jpg");
            }
            base.ViewData["headImg1"] = (user != null && !user.HeadImg.IsNullOrEmpty() && System.IO.File.Exists(Current.WebRootPath + Url.Content("~" + user.HeadImg)) ? Url.Content("~" + user.HeadImg) : str);
            
           
            return this.View();   
        }
    }


   



}
