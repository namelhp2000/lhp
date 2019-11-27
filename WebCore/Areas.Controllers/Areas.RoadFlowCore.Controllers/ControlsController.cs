using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{



  

    [Area("RoadFlowCore")]
    public class ControlsController : Controller
    {
        // Fields
        private readonly string attachmentPath = ("/Attachment/" + GuidExtensions.ToUpperString(Current.UserId) + "/");
        private readonly string contentRootPath = Current.ContentRootPath;
        private readonly string webRootPath = Current.WebRootPath;

        // Methods
        [Validate(CheckApp = false, CheckUrl = false), ValidateAntiForgeryToken]
        public string DeleteFile()
        {
            string str = base.Request.Forms("file").ToString().DESDecrypt();
            if (str.IsNullOrWhiteSpace())
            {
                return "文件为空!";
            }
            FileInfo info = new FileInfo(("1".Equals(base.Request.Forms("fullpath")) ? "" : this.attachmentPath) + str);
            if (info.Exists)
            {
                info.Delete();
            }
            return "1";
        }

        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public string Dictionary_GetNames()
        {
            string str = base.Request.Forms("values");
            StringBuilder builder = new StringBuilder();
            Dictionary dictionary = new Dictionary();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.Dictionary dictionary2 = dictionary.Get(guid);
                    if (dictionary2 != null)
                    {
                        builder.Append(dictionary2.Title);
                        builder.Append("、");
                    }
                }
            }
            return builder.ToString().TrimEnd('、');
        }

        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public IActionResult Dictionary_Index()
        {
            string str = base.Request.Querys("values");
            string str2 = base.Request.Querys("datasource");
            StringBuilder builder = new StringBuilder();
            Dictionary dictionary = new Dictionary();
            foreach (string str3 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if ((str2 == "0") && StringExtensions.IsGuid(str3, out guid))
                {
                    RoadFlow.Model.Dictionary dictionary2 = dictionary.Get(guid);
                    if (dictionary2 != null)
                    {
                        builder.Append("<div onclick=\"currentDel=this;showinfo('{0}');\" class=\"selectorDiv\" ondblclick=\"currentDel=this;del();\" value=\"" + str3 + "\">");
                        builder.Append(dictionary2.Title);
                        builder.Append("</div>");
                    }
                }
            }
            base.ViewData["defaults"] = builder.ToString();
            base.ViewData["ismore"] = base.Request.Querys("ismore");
            base.ViewData["isparent"] = base.Request.Querys("isparent");
            base.ViewData["ischild"] = base.Request.Querys("ischild");
            base.ViewData["isroot"] = base.Request.Querys("isroot");
            base.ViewData["root"] = base.Request.Querys("root");
            base.ViewData["eid"] = base.Request.Querys("eid");
            base.ViewData["datasource"] = str2;
            base.ViewData["ismobile"] = base.Request.Querys("ismobile");
            return this.View();
        }

        private ValueTuple<string, string> GetHeadType(string extName)
        {
            if (!extName.IsNullOrWhiteSpace())
            {
                string str = extName.Trim().ToLower();
                if (",jpg,jpeg,png,gif,tif,tiff,".Contains("," + str + ","))
                {
                    return new ValueTuple<string, string>("inline", "image/" + str);
                }
                if (",txt,".Contains("," + str + ","))
                {
                    return new ValueTuple<string, string>("inline", "text/plain");
                }
                if (",pdf,".Contains("," + str + ","))
                {
                    return new ValueTuple<string, string>("inline", "application/pdf");
                }
                if (",json,".Contains("," + str + ","))
                {
                    return new ValueTuple<string, string>("inline", "application/json");
                }
                if (",doc,docx,dot,".Contains("," + str + ","))
                {
                    return new ValueTuple<string, string>("inline", "application/msword");
                }
                if (",xls,xlsx,".Contains("," + str + ","))
                {
                    return new ValueTuple<string, string>("inline", "application/vnd.ms-excel");
                }
                if (",ppt,pptx,pps,pot,ppa,".Contains("," + str + ","))
                {
                    return new ValueTuple<string, string>("inline", "application/vnd.ms-powerpoint");
                }
            }
            return new ValueTuple<string, string>("attachment", "application/octet-stream");
        }

        private string GetUploadFileName(string saveDir, string fileName)
        {
            if (System.IO.File.Exists(saveDir + fileName))
            {
                string extension = Path.GetExtension(fileName);
                string str2 = Path.GetFileNameWithoutExtension(fileName) + "_" + Tools.GetRandomString(6).ToUpper();
                return this.GetUploadFileName(saveDir, str2 + extension);
            }
            return fileName;
        }



        private bool IsUpload(string extName)
        {
            if (!Config.UploadFileExtNames.IsNullOrWhiteSpace())
            {
                if (Config.UploadFileExtNames == "*")
                {
                    return true;
                }
                return ("," + Config.UploadFileExtNames + ",").ContainsIgnoreCase(("," + extName + ","));
            }
            return !",exe,msi,bat,cshtml,asp,aspx,ashx,ascx,cs,dll,js,vbs,css,".ContainsIgnoreCase(("," + extName + ","));
        }




        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public string Member_GetNames()
        {
            return new Organize().GetNames(base.Request.Forms("value"));
        }

        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public string Member_GetNote()
        {
            string str = base.Request.Querys("id");
            if (!str.IsNullOrWhiteSpace())
            {
                Guid guid;
                Organize organize = new Organize();
                User user = new User();
                OrganizeUser user2 = new OrganizeUser();
                if (str.StartsWith("u_"))
                {
                    RoadFlow.Model.OrganizeUser mainByUserId = user2.GetMainByUserId(StringExtensions.ToGuid(str.RemoveUserPrefix()));
                    return (organize.GetParentsName(mainByUserId.OrganizeId, true) + @" \ " + organize.GetName(mainByUserId.OrganizeId));
                }
                if (str.StartsWith("r_"))
                {
                    RoadFlow.Model.OrganizeUser user4 = user2.Get(StringExtensions.ToGuid(str.RemoveUserRelationPrefix()));
                    return (organize.GetParentsName(user4.OrganizeId, true) + @" \ " + organize.GetName(user4.OrganizeId) + "[兼任]");
                }
                if (str.StartsWith("w_"))
                {
                    return "";
                }
                if (StringExtensions.IsGuid(str, out guid))
                {
                    return (organize.GetParentsName(guid, true) + @" \ " + organize.GetName(guid));
                }
            }
            return "";
        }

        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public IActionResult Member_Index()
        {
            string str = base.Request.Method.EqualsIgnoreCase("post") ? base.Request.Forms("value") : "";
            string str2 = base.Request.Querys("eid");
            string str3 = base.Request.Querys("isunit");
            string str4 = base.Request.Querys("isdept");
            string str5 = base.Request.Querys("isstation");
            string str6 = base.Request.Querys("isuser");
            string str7 = base.Request.Querys("ismore");
            string str8 = base.Request.Querys("isall");
            string str9 = base.Request.Querys("isgroup");
            string str10 = base.Request.Querys("isrole");
            string str11 = base.Request.Querys("rootid");
            string str12 = base.Request.Querys("ischangetype");
            string str13 = base.Request.Querys("isselect");
            string str14 = base.Request.Querys("ismobile");
            Organize organize = new Organize();
            StringBuilder builder = new StringBuilder();
            foreach (string str15 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                if (!str15.IsNullOrEmpty())
                {
                    string names = organize.GetNames(str15);
                    if (!names.IsNullOrEmpty())
                    {
                        builder.AppendFormat("<div onclick=\"currentDel=this;showinfo('{0}');\" class=\"selectorDiv\" ondblclick=\"currentDel=this;del();\" value=\"{0}\">", str15);
                        builder.Append(names);
                        builder.Append("</div>");
                    }
                }
            }
            base.ViewData["eid"] = str2;
            base.ViewData["isunit"] = str3;
            base.ViewData["isdept"] = str4;
            base.ViewData["isstation"] = str5;
            base.ViewData["isuser"] = str6;
            base.ViewData["ismore"] = str7;
            base.ViewData["isall"] = str8;
            base.ViewData["isgroup"] = str9;
            base.ViewData["isrole"] = str10;//未使用
            base.ViewData["rootid"] = str11;
            base.ViewData["ischangetype"] = str12; //未使用
            base.ViewData["isselect"] = str13;
            base.ViewData["values"] = str;
            base.ViewData["userprefix"] = "u_";
            base.ViewData["relationprefix"] = "r_";
            base.ViewData["workgroupprefix"] = "w_";
            base.ViewData["defaultValues"] = builder.ToString();
            base.ViewData["ismobile"] = str14;

            return this.View();
        }




        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public string SelectDiv_GetTitle()
        {
            Guid guid;
            Guid guid2;
            if (!StringExtensions.IsGuid(base.Request.Querys("applibaryid"), out guid))
            {
                return "";
            }
            RoadFlow.Model.AppLibrary library = new AppLibrary().Get(guid);
            if (((library == null) || library.Code.IsNullOrWhiteSpace()) || !StringExtensions.IsGuid(library.Code, out guid2))
            {
                return "";
            }
            string str2 = base.Request.Querys("titlefield");
            string str3 = base.Request.Querys("pkfield");
            string str4 = base.Request.Querys("values");
            return new RoadFlow.Business.Program().GetTitles(str4, str3, str2, guid2);
        }

        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public IActionResult SelectDiv_Index()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("applibaryid"), out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content = "参数错误!";
                return result1;
            }
            RoadFlow.Model.AppLibrary library = new AppLibrary().Get(guid);
            if ((library != null) && !library.Address.IsNullOrEmpty())
            {
                string str2 = base.Request.UrlQuery();
                string str3 = library.Address + (library.Address.Contains("?") ? ("&" + str2.TrimStart('?')) : str2);
                return this.Redirect(str3);
            }
            ContentResult result2 = new ContentResult();
            result2.Content = "参数错误!";
            return result2;
        }

        public string SelectIco_File()
        {
            XElement element = new XElement("Root");
            string str = base.Request.Querys("path");
            if (str.IsNullOrWhiteSpace())
            {
                str = "/RoadFlowResources/images/ico";
            }
            string str2 = ",.jpg,.gif,.png,";
            if (Directory.Exists(this.webRootPath + str))
            {
                DirectoryInfo info = new DirectoryInfo(this.webRootPath + str);
                foreach (FileInfo info2 in Enumerable.Where<FileInfo>(info.GetFiles(),
                    key => ((key.Attributes & ((FileAttributes)((int)FileAttributes.Hidden))) == ((FileAttributes)0))))
                {
                    if (str2.IndexOf("," + info2.Extension.ToLower() + ",") != -1)
                    {
                        XElement content = new XElement("Icon");
                        element.Add(content);
                        content.SetAttributeValue("title", info2.Name);
                        content.SetAttributeValue("path", "/RoadFlowResources/images/ico/" + info2.Name);
                        content.SetAttributeValue("path1", "/RoadFlowResources/images/ico/" + info2.Name);
                    }
                }
            }
            if (Directory.Exists(this.webRootPath + str + "/newioc"))
            {
                DirectoryInfo info = new DirectoryInfo(this.webRootPath + str + "/newioc");
                foreach (FileInfo info2 in Enumerable.Where<FileInfo>(info.GetFiles(),
                   key => ((key.Attributes & ((FileAttributes)((int)FileAttributes.Hidden))) == ((FileAttributes)0))))
                {
                    if (str2.IndexOf("," + info2.Extension.ToLower() + ",") != -1)
                    {
                        XElement content = new XElement("Icon");
                        element.Add(content);
                        content.SetAttributeValue("title", info2.Name);
                        content.SetAttributeValue("path", "/RoadFlowResources/images/ico/newioc/" + info2.Name);
                        content.SetAttributeValue("path1", "/RoadFlowResources/images/ico/newioc/" + info2.Name);
                    }
                }
            }
            return element.ToString();
        }

        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public IActionResult SelectIco_Index()
        {
            base.ViewData["source"] = base.Request.Querys("source");
            base.ViewData["id"] = base.Request.Querys("id");
            base.ViewData["isImg"] = base.Request.Querys("isimg");
            base.ViewData["isfont"] = base.Request.Querys("isfont");

            return this.View();
        }

        [Validate(CheckApp = false, CheckUrl = false)]
        public void ShowFile()
        {
            string str = base.Request.Querys("file").DESDecrypt();
            if (!str.IsNullOrWhiteSpace())
            {
                bool flag = "1".Equals(base.Request.Querys("fullpath"));
                if ("1".Equals(base.Request.Querys("checkshare")))
                {
                    Guid guid;
                    string[] strArray = str.Split('?', (StringSplitOptions)StringSplitOptions.None);
                    if (strArray.Length < 2)
                    {
                        return;
                    }
                    str = strArray[0];
                    string str3 = strArray[1];
                    string str4 = (strArray.Length > 2) ? strArray[2] : string.Empty;
                    if ((!StringExtensions.IsGuid(str3, out guid) || !guid.Equals(Current.UserIdOrWeiXinId)) || !new UserFileShare().IsAccess(str.DESEncrypt(), guid, str4))
                    {
                        return;
                    }
                }
                FileInfo info = new FileInfo(flag ? str : (UserFile.RootPath + str));
                if (info.Exists && (!flag || UserFile.HasAccess(info.DirectoryName, Guid.Empty)))
                {
                    string str2 = info.Name.UrlEncode();
                    base.Response.Headers.Add("Server-FileName", (StringValues)str2);
                    ValueTuple<string, string> headType = this.GetHeadType(Path.GetExtension(str).TrimStart('.'));
                    base.Response.ContentType = headType.Item2;
                    base.Response.Headers.Add("Content-Disposition", (StringValues)(headType.Item1 + "; filename=" + str2));
                    base.Response.Headers.Add("Content-Length", (StringValues)((long)info.Length).ToString());
                    using (FileStream stream = info.OpenRead())
                    {
                        byte[] buffer = new byte[0x800];
                        for (int i = stream.Read(buffer, 0, buffer.Length); i > 0; i = stream.Read(buffer, 0, buffer.Length))
                        {
                            base.Response.Body.WriteAsync(buffer, 0, i);
                            base.Response.Body.FlushAsync();
                        }
                    }
                    base.Response.Body.FlushAsync();
                    base.Response.Body.Close();
                }
            }
        }

        [Validate(CheckApp = false, CheckUrl = false)]
        public string UploadFiles_GetShowString()
        {
            string str = base.Request.Forms("showtype");
            string str2 = base.Request.Forms("width");
            string str3 = base.Request.Forms("height");
            string str4 = base.Request.Forms("files");
            if (str.ToInt(0) == 1)
            {
                return str4.ToFilesImgString(str2.ToInt(0), str3.ToInt(0));
            }
            return str4.ToFilesShowString(true);
        }





        [Validate(CheckLogin = true, CheckApp = false, CheckUrl = false)]
        public IActionResult UploadFiles_Index()
        {
            JArray array = new JArray();
            string str = base.Request.Method.EqualsIgnoreCase("post") ? base.Request.Forms("value") : "";
            foreach (string str2 in (str ?? "").Split('|', (StringSplitOptions)StringSplitOptions.None))
            {
                string str3 = str2.DESDecrypt();
                FileInfo info = new FileInfo(UserFile.RootPath + str3);
                if (info.Exists)
                {
                    JObject obj1 = new JObject();
                    obj1.Add("id", (JToken)str2);
                    obj1.Add("name", (JToken)info.Name);
                    obj1.Add("size", (JToken)info.Length.ToFileSize());
                    JObject obj2 = obj1;
                    array.Add(obj2);
                }
            }
            base.ViewData["fileType"] = base.Request.Querys("filetype");
            base.ViewData["eid"] = base.Request.Querys("eid");
            base.ViewData["userId"] = Current.UserId;
            base.ViewData["values"] = array.ToString();
            base.ViewData["ismobile"] = base.Request.Querys("ismobile");
            base.ViewData["filepath"] = base.Request.Querys("filepath");
            base.ViewData["isselectuserfile"] = base.Request.Querys("isselectuserfile");
            return this.View();
        }











        #region  新文件上传功能，通过配置实现

        /// <summary>
        /// 保存编辑器文件
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckUrl = false)]
        public string SaveCKEditorFiles()
        {
            IFormFileCollection files = base.Request.Form.Files;
            JObject obj2 = new JObject();
            if (files.Count == 0)
            {
                obj2.Add("number", -1);
                obj2.Add("message", "没有要上传的文件");
                JObject obj1 = new JObject();
                obj1.Add("error", obj2);
                return obj1.ToString();
            }
            IFormFile file = files[0];
            string extName = Path.GetExtension(file.FileName).TrimStart('.');
            if (!this.IsUpload(extName))
            {
                obj2.Add("number", -1);
                obj2.Add("message", "不能上传该类型文件");
                JObject obj4 = new JObject();
                obj4.Add("error", obj2);
                return obj4.ToString();
            }
            DateTime time = DateTimeExtensions.Now;
            string[] textArray1 = new string[] { ((int)time.Year).ToString(), "/", time.ToString("MM"), "/", time.ToString("dd") };
            string str2 = string.Concat((string[])textArray1);
            string saveDir = UserFile.RootPath + this.attachmentPath + str2 + "/";
            string fileName = file.FileName.Replace(" ", "");
            string uploadFileName = this.GetUploadFileName(saveDir, fileName);
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            using (FileStream stream = System.IO.File.Create(saveDir + uploadFileName))
            {
                file.CopyTo((Stream)stream);
                stream.FlushAsync();
            }
            JObject obj5 = new JObject();
            obj5.Add("fileName", (JToken)uploadFileName);
            obj5.Add("uploaded", 1);
            obj5.Add("url", (JToken)(base.Url.Content("~/RoadFlowCore/Controls/ShowFile?file=") + (this.attachmentPath + str2 + "/" + uploadFileName).DESEncrypt()));
            JObject obj3 = obj5;
            return obj3.ToString();
        }

        /// <summary>
        /// 上传文件保存
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckUrl = false), ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        public string UploadFiles_Save()
        {
            DateTime dateTime = Current.DateTime;
            string str = dateTime.ToString("yyyy");
            string str2 = dateTime.ToString("MM");
            string str3 = dateTime.ToString("dd");
            IFormFileCollection files = base.Request.Form.Files;
            string str4 = base.Request.Forms("filetype");
            JObject obj2 = new JObject();
            if (files.Count > 0)
            {
                IFormFile file = files[0];
                string extName = Path.GetExtension(file.FileName).TrimStart('.');
                if (!this.IsUpload(extName))
                {
                    obj2.Add("error", "不能上传该类型文件");
                    return obj2.ToString();
                }
                if (!str4.IsNullOrWhiteSpace() && !("," + str4 + ",").ContainsIgnoreCase(("," + extName + ",")))
                {
                    obj2.Add("error", "不能上传该类型文件");
                    return obj2.ToString();
                }
                string[] textArray1 = new string[] { UserFile.RootPath, this.attachmentPath, str, "/", str2, "/", str3, "/" };
                string saveDir = string.Concat((string[])textArray1);
                string fileName = file.FileName.Replace(" ", "");
                string uploadFileName = this.GetUploadFileName(saveDir, fileName);
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }
                using (FileStream stream = System.IO.File.Create(saveDir + uploadFileName))
                {
                    file.CopyTo((Stream)stream);
                    stream.FlushAsync();
                }
                string[] textArray2 = new string[] { this.attachmentPath, str, "/", str2, "/", str3, "/", uploadFileName };
                obj2.Add("id", (JToken)string.Concat((string[])textArray2).DESEncrypt());
                obj2.Add("size", (JToken)file.Length.ToFileSize());
            }
            return obj2.ToString();
        }

        /// <summary>
        /// 用户文件上传
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckUrl = false)]
        [DisableRequestSizeLimit]
        public string UserFiles_Save()
        {


            string str = base.Request.Forms("filepath");
            IFormFileCollection files = base.Request.Form.Files;
            JObject obj2 = new JObject();
            if (files.Count == 0)
            {
                obj2.Add("number", -1);
                obj2.Add("message", "没有要上传的文件");
                JObject obj1 = new JObject();
                obj1.Add("error", obj2);
                return obj1.ToString();
            }
            IFormFile file = files[0];
            string extName = Path.GetExtension(file.FileName).TrimStart('.');
            if (!this.IsUpload(extName))
            {
                obj2.Add("number", -1);
                obj2.Add("message", "不能上传该类型文件");
                JObject obj3 = new JObject();
                obj3.Add("error", obj2);
                return obj3.ToString();
            }
            string saveDir = str.DESDecrypt() + "/";
            string fileName = file.FileName.Replace(" ", "");
            string uploadFileName = this.GetUploadFileName(saveDir, fileName);
            if (!UserFile.HasAccess(saveDir, Current.UserId))
            {
                obj2.Add("number", -1);
                obj2.Add("message", "不能在该目录上传文件");
                JObject obj4 = new JObject();
                obj4.Add("error", obj2);
                return obj4.ToString();
            }
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            using (FileStream stream = System.IO.File.Create(saveDir + uploadFileName))
            {
                file.CopyTo((Stream)stream);
                stream.FlushAsync();
            }
            obj2.Add("id", (JToken)(saveDir + uploadFileName).DESEncrypt());
            obj2.Add("size", (JToken)file.Length.ToFileSize());
            return obj2.ToString();
        }





        #endregion














      




    }
}
