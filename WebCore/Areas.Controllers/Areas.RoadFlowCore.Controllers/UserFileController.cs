using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class UserFileController : Controller
    {
        // Methods
        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string AddDir()
        {
            string str = base.Request.Form["DirName"];
            if (str.IsNullOrWhiteSpace())
            {
                return "文件夹名称不能为空!";
            }
            string path = base.Request.Querys("id").DESDecrypt() + "/" + str;
            if (Directory.Exists(path))
            {
                return "文件夹已经存在!";
            }
            try
            {
                Directory.CreateDirectory(path);
                return "1";
            }
            catch (IOException exception)
            {
                return exception.Message;
            }
        }

        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string Delete()
        {
            string[] strArray = base.Request.Forms("files").Split(',', (StringSplitOptions)StringSplitOptions.None);
            try
            {
                foreach (string str in strArray)
                {
                    string path = str.DESDecrypt();
                    if (RoadFlow.Business.UserFile.HasAccess(path, Current.UserId))
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path);
                        }
                        else if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                }
                return "删除成功!";
            }
            catch (IOException exception)
            {
                return exception.Message;
            }
        }

        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string DeleteShare()
        {
            string[] strArray = base.Request.Forms("files").Split(',', (StringSplitOptions)StringSplitOptions.None);
            UserFileShare share = new UserFileShare();
            foreach (string str in strArray)
            {
                share.DeleteByFileId(str);
            }
            return "1";
        }

        [Validate(CheckApp = false)]
        public IActionResult Index()
        {
            Guid userId = Current.UserId;
            if (GuidExtensions.IsEmptyGuid(userId))
            {
                userId = Current.EnterpriseWeiXinUserId;
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["id"]= new UserFile().GetUserRoot(userId).DESEncrypt();
            base.ViewData["isselect"]= base.Request.Querys("isselect");
            base.ViewData["ismobile"]= base.Request.Querys("ismobile");
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult List()
        {
            Guid userId = Current.UserId;
            if (GuidExtensions.IsEmptyGuid(userId))
            {
                userId = Current.EnterpriseWeiXinUserId;
            }
            string str = base.Request.Querys("id");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid"), "&isselect=", base.Request.Querys("isselect"), "&ismobile=", base.Request.Querys("ismobile") };
            string str2 = string.Concat((string[])textArray1);
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["id"]= str;
            base.ViewData["appid"]= base.Request.Querys("appid");
            base.ViewData["isselect"]= base.Request.Querys("isselect");
            base.ViewData["ismobile"]= base.Request.Querys("ismobile");
            base.ViewData["path"]= UserFile.GetLinkPath(str.DESDecrypt(), userId, str2);
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult MoveTo()
        {
            base.ViewData["appid"]= base.Request.Querys("appid");
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string MoveToSave()
        {
            string str = base.Request.Forms("movetodir");
            string[] strArray = base.Request.Querys("movedir").Split(',', (StringSplitOptions)StringSplitOptions.None);
            if (strArray.Length == 0)
            {
                return "没有要移动的文件或文件夹!";
            }
            if (str.IsNullOrWhiteSpace())
            {
                return "要移动到的文件夹为空!";
            }
            List<string> list = new List<string>();
            foreach (string str3 in strArray)
            {
                list.Add(str3.DESDecrypt());
            }
            return UserFile.MoveTo(list.ToArray(), str.DESDecrypt());
        }

        [Validate(CheckApp = false)]
        public IActionResult MyShare()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }




        /// <summary>
        /// 我分享用户
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false)]
        public IActionResult MyShareUser()
        {
            string str = base.Request.Querys("fileid");
            string str2 = base.Request.Querys("userid");
            DataTable myShareUsers = new UserFileShare().GetMyShareUsers(str, Current.UserId);
            JArray array = new JArray();
            User user = new User();
            foreach (DataRow row in myShareUsers.Rows)
            {
                Guid guid = StringExtensions.ToGuid(row["UserID"].ToString());
                JObject obj1 = new JObject();
                obj1.Add("Name", (JToken)(user.GetName(guid) + " (" + user.GetOrganizeMainShowHtml(guid, false) + ")"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            base.ViewData["json"]= array.ToString();
            return this.View();
        }









        [Validate(CheckApp = false)]
        public string QueryList()
        {
            UserFile file = new UserFile();
            string str = base.Request.Querys("id").DESDecrypt();
            string str2 = base.Request.Forms("sidx");
            string str3 = base.Request.Forms("sord");
            string str4 = base.Request.Forms("searchword");
            bool flag = "1".Equals(base.Request.Querys("isselect"));
            List<ValueTuple<string, string, DateTime, int, long>> list = file.GetSubDirectoryAndFiles(str, Current.UserIdOrWeiXinId, str4, str2, str3.EqualsIgnoreCase("asc") ? 0 : 1);
            string[] textArray1 = new string[] { "&appid=", base.Request.Querys("appid"), "rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid"), "&isselect=", base.Request.Querys("isselect"), "&ismobile=", base.Request.Querys("ismobile") };
            string str5 = string.Concat((string[])textArray1);
            JArray array = new JArray();
            foreach (ValueTuple<string, string, DateTime, int, long> tuple in list)
            {
                string str6 = flag ? UserFile.GetRelativePath(tuple.Item2).DESEncrypt() : tuple.Item2.Replace(@"\", "/").DESEncrypt();
                string str7 = string.Empty;
                if (tuple.Item4 == 0)
                {
                    string[] textArray2 = new string[] { "<a class=\"blue\" href=\"List?id=", (str + "/" + tuple.Item1).DESEncrypt(), str5, "\">", tuple.Item1, "</a>" };
                    str7 = string.Concat((string[])textArray2);
                }
                else
                {
                    string[] textArray3 = new string[] { "<a class=\"blue\" href=\"", base.Url.Content("~/RoadFlowCore/Controls/ShowFile?fullpath=1&file=" + tuple.Item2.DESEncrypt()), "\" target=\"_blank\">", tuple.Item1, "</a>" };
                    str7 = string.Concat((string[])textArray3);
                }
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)str6);
                obj1.Add("Name", (JToken)str7);
                obj1.Add("Name1", tuple.Item1);
                obj1.Add("Date", flag ? ((JToken)DateTimeExtensions.ToShortDateTimeString(tuple.Item3)) : ((JToken)DateTimeExtensions.ToDateTimeString(tuple.Item3)));
                obj1.Add("Type", (tuple.Item4 == 0) ? "文件夹" : "文件");
                obj1.Add("Size", (tuple.Item5 == 0) ? ((JToken)"") : ((JToken)tuple.Item5.ToFileSize()));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            return array.ToString(0, Array.Empty<JsonConverter>());
        }


        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string QueryMyShare()
        {
            int num3;
            string str = base.Request.Forms("FileName");
            string str2 = base.Request.Forms("sidx");
            string str3 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str4 = (str2.IsNullOrEmpty() ? "ShareDate" : str2) + " " + (str3.IsNullOrEmpty() ? "ASC" : str3);
            DataTable table = new UserFileShare().GetMySharePagerList(out num3, pageSize, pageNumber, Current.UserId, str, str4);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                DateTime time = StringExtensions.ToDateTime(row["ExpireDate"].ToString());
                string str5 = row["FileId"].ToString();
                string path = str5.DESDecrypt();
                string str7 = string.Empty;
                string str8 = string.Empty;
                string[] textArray1 = new string[] { "&appid=", base.Request.Querys("appid"), "rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid"), "&isselect=", base.Request.Querys("isselect"), "&ismobile=", base.Request.Querys("ismobile") };
                string str9 = string.Concat((string[])textArray1);
                if (System.IO.File.Exists(path))
                {
                    str7 = "文件";
                    string[] textArray2 = new string[] { "<a target=\"_blank\" href=\"", base.Url.Content("~/RoadFlowCore/Controls/ShowFile?fullpath=1&file=" + str5), "\">", row["FileName"].ToString(), "</a>" };
                    str8 = string.Concat((string[])textArray2);
                }
                else if (Directory.Exists(path))
                {
                    str7 = "文件夹";
                    string[] textArray3 = new string[] { "<a href=\"javascript:;\" onclick=\"showDir('", base.Url.Content("~/RoadFlowCore/UserFile/"), "ShareDirList?id=", str5, str9, "', '", str5, "');\">", row["FileName"].ToString(), "</a>" };
                    str8 = string.Concat((string[])textArray3);
                }
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)str5);
                obj1.Add("FileName", (JToken)str8);
                obj1.Add("ShareDate", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["ShareDate"].ToString())));
                obj1.Add("ExpireDate", (time.Year == DateTime.MaxValue.Year) ? ((JToken)"永久有效") : ((JToken)DateTimeExtensions.ToDateTimeString(time)));
                obj1.Add("Type", (JToken)str7);
                string[] textArray4 = new string[] { "<a href=\"javascript:;\" onclick=\"ViewUser('", str5, "','", row["ShareUserId"].ToString(), "');\"><i class=\"fa fa-search\"></i> 查看</a>" };
                obj1.Add("UserId", (JToken)string.Concat((string[])textArray4));

                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string QueryShareDirList()
        {
            UserFile file = new UserFile();
            string str = base.Request.Querys("id").DESDecrypt();
            string str2 = base.Request.Forms("sidx");
            string str3 = base.Request.Forms("sord");
            string str4 = base.Request.Forms("searchword");
            string str5 = base.Request.Querys("fileid");
            if (!new UserFileShare().IsAccess(str5, Current.UserIdOrWeiXinId, ""))
            {
                return "[]";
            }
            bool flag = "1".Equals(base.Request.Querys("isselect"));
            List<ValueTuple<string, string, DateTime, int, long>> list = file.GetSubDirectoryAndFiles(str, Guid.Empty, str4, str2, str3.EqualsIgnoreCase("asc") ? 0 : 1);
            string[] textArray1 = new string[] { "&appid=", base.Request.Querys("appid"), "rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid"), "&isselect=", base.Request.Querys("isselect"), "&ismobile=", base.Request.Querys("ismobile") };
            string str6 = string.Concat((string[])textArray1);
            JArray array = new JArray();
            foreach (ValueTuple<string, string, DateTime, int, long> tuple in list)
            {
                string str7 = flag ? UserFile.GetRelativePath(tuple.Item2).DESEncrypt() : tuple.Item2.Replace(@"\", "/").DESEncrypt();
                string str8 = string.Empty;
                if (tuple.Item4 == 0)
                {
                    string[] textArray2 = new string[] { "<a class=\"blue\" href=\"ShareDirList?id=", (str + "/" + tuple.Item1).DESEncrypt(), str6, "&fileid=", str5, "\">", tuple.Item1, "</a>" };
                    str8 = string.Concat((string[])textArray2);
                }
                else
                {
                    string[] textArray3 = new string[5];
                    textArray3[0] = "<a target=\"_blank\" href=\"";
                    object[] objArray1 = new object[] { tuple.Item2, "?", Current.UserIdOrWeiXinId, "?", str5 };
                    textArray3[1] = (string)base.Url.Content("~/RoadFlowCore/Controls/ShowFile?fullpath=1&checkshare=1&file=" + string.Concat((object[])objArray1).DESEncrypt());
                    textArray3[2] = "\">";
                    textArray3[3] = tuple.Item1;
                    textArray3[4] = "</a>";
                    str8 = string.Concat((string[])textArray3);
                }
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)str7);
                obj1.Add("Name", (JToken)str8);
                obj1.Add("Name1", tuple.Item1);
                obj1.Add("Date", flag ? ((JToken)DateTimeExtensions.ToShortDateTimeString(tuple.Item3)) : ((JToken)DateTimeExtensions.ToDateTimeString(tuple.Item3)));
                obj1.Add("Type", (tuple.Item4 == 0) ? "文件夹" : "文件");
                obj1.Add("Size", (tuple.Item5 == 0) ? ((JToken)"") : ((JToken)tuple.Item5.ToFileSize()));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            return array.ToString(0, Array.Empty<JsonConverter>());
        }


        //改变
        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string QueryShareMy()
        {
            int num3;
            string str = base.Request.Forms("FileName");
            string str2 = base.Request.Forms("sidx");
            string str3 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str4 = (str2.IsNullOrEmpty() ? "ShareDate" : str2) + " " + (str3.IsNullOrEmpty() ? "ASC" : str3);
            DataTable table = new UserFileShare().GetShareMyPagerList(out num3, pageSize, pageNumber, Current.UserId, str, str4);
            JArray array = new JArray();
            User user = new User();

            foreach (DataRow row in table.Rows)
            {
                DateTime time = StringExtensions.ToDateTime(row["ExpireDate"].ToString());
                string str5 = row["FileId"].ToString();
                string path = str5.DESDecrypt();
                string str7 = string.Empty;
                string str8 = string.Empty;
                string[] textArray1 = new string[] { "&appid=", base.Request.Querys("appid"), "rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid"), "&isselect=", base.Request.Querys("isselect"), "&ismobile=", base.Request.Querys("ismobile") };
                string str9 = string.Concat((string[])textArray1);
                if (System.IO.File.Exists(path))
                {
                    str7 = "文件";
                    string[] textArray2 = new string[] { "<a target=\"_blank\" href=\"", base.Url.Content("~/RoadFlowCore/Controls/ShowFile?fullpath=1&checkshare=1&file=" + (path + "?" + Current.UserIdOrWeiXinId).DESEncrypt()), "\">", row["FileName"].ToString(), "</a>" };
                    str8 = string.Concat((string[])textArray2);
                }
                else if (Directory.Exists(path))
                {
                    str7 = "文件夹";
                    string[] textArray3 = new string[] { "<a href=\"javascript:;\" onclick=\"showDir('", base.Url.Content("~/RoadFlowCore/UserFile/"), "ShareDirList?id=", str5, str9, "', '", str5, "');\">", row["FileName"].ToString(), "</a>" };
                    str8 = string.Concat((string[])textArray3);
                }
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)str5);
                obj1.Add("FileName", (JToken)str8);
                obj1.Add("ShareDate", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["ShareDate"].ToString())));
                obj1.Add("ExpireDate", (time.Year == DateTime.MaxValue.Year) ? ((JToken)"永久有效") : ((JToken)DateTimeExtensions.ToDateTimeString(time)));
                obj1.Add("ShareUserId", (JToken)user.GetName(StringExtensions.ToGuid(row["ShareUserId"].ToString())));

                obj1.Add("Type", (JToken)str7);
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string ReName()
        {
            string str = base.Request.Forms("file");
            string str2 = base.Request.Forms("newname");
            if (str2.IsNullOrWhiteSpace())
            {
                return "要重命名的名称为空!";
            }
            if (str.IsNullOrWhiteSpace())
            {
                return "文件为空";
            }
            return UserFile.ReName(str.DESDecrypt(), str2);
        }

        [Validate(CheckApp = false)]
        public IActionResult Share()
        {
            base.ViewData["appid"]= base.Request.Querys("appid");
            base.ViewData["queryString"]=base.Request.UrlQuery();
            base.ViewData["sharedir"]= base.Request.Forms("sharedir");
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult ShareDirList()
        {
            string str = base.Request.Querys("id");
            string str2 = base.Request.Querys("fileid");
            string[] textArray1 = new string[] { "&fileid=", str2, "&appid=", base.Request.Querys("appid"), "rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid"), "&isselect=", base.Request.Querys("isselect"), "&ismobile=", base.Request.Querys("ismobile") };
            string query = string.Concat((string[])textArray1);
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["path"]= UserFile.GetLinkShparePath(str.DESDecrypt(), str2.DESDecrypt(), query);
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult ShareMy()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string ShareSave()
        {
            string str = base.Request.Forms("ShareUser");
            string str2 = base.Request.Forms("sharedir");
            string str3 = base.Request.Forms("ExpireDate");
            string str4 = base.Request.Forms("NoExpireDate");
            if (str2.IsNullOrWhiteSpace() || str.IsNullOrWhiteSpace())
            {
                return "要分享的目录文件或人员为空!";
            }
            DateTime? nullable = null;
            if ("1".Equals(str4))
            {
                nullable = new DateTime?(DateTime.MaxValue);
            }
            else
            {
                DateTime time;
                if (StringExtensions.IsDateTime(str3, out time))
                {
                    nullable = new DateTime?(time);
                }
            }
            if (!nullable.HasValue)
            {
                return "请设置有效期!";
            }
            int num = new UserFileShare().Share(str2, str, Current.UserId, nullable.Value);
            return "1";
        }

        [Validate(CheckApp = false)]
        public IActionResult Tree()
        {
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid"), "&isselect=", base.Request.Querys("isselect"), "&ismobile=", base.Request.Querys("ismobile") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            base.ViewData["appid"]= base.Request.Querys("appid");
            return this.View();
        }

        [Validate(CheckApp = false)]
        public string Tree1()
        {
            Guid userId = Current.UserId;
            if (GuidExtensions.IsEmptyGuid(userId))
            {
                userId = Current.EnterpriseWeiXinUserId;
            }
            return new UserFile().GetUserDirectoryJSON(userId, "", "1".Equals(base.Request.Querys("isselect")));
        }

        [Validate(CheckApp = false)]
        public string TreeRefresh()
        {
            string str = base.Request.Querys("refreshid");
            if (str.IsNullOrWhiteSpace())
            {
                return "[]";
            }
            Guid userId = Current.UserId;
            if (GuidExtensions.IsEmptyGuid(userId))
            {
                userId = Current.EnterpriseWeiXinUserId;
            }
            return new UserFile().GetUserDirectoryJSON(userId, str.DESDecrypt(), "1".Equals(base.Request.Querys("isselect")));
        }
    }






}
