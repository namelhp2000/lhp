using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{

  

   

    [Area("RoadFlowCore")]
    public class DocController : Controller
    {
        // Methods
        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string DeleteDir()
        {
            Guid guid;
            string str = base.Request.Querys("dirid");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return "id错误!";
            }
            DocDir dir = new DocDir();
            if (guid == dir.GetRoot().Id)
            {
                return "不能删除根栏目!";
            }
            if (dir.HasDoc(guid))
            {
                return "该栏目下有文档,不能删除!";
            }
            RoadFlow.Model.DocDir docDir = new RoadFlow.Model.DocDir();
            docDir.Id=guid;
            dir.Delete(docDir);
            Log.Add("删除了文档栏目-" + str, "", LogType.系统管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate(CheckApp = false)]
        public IActionResult DirEdit()
        {
            Guid guid;
            string str = base.Request.Querys("dirid");
            string str2 = base.Request.Querys("currentdirid");
            RoadFlow.Model.DocDir dir = null;
            DocDir dir2 = new DocDir();
            if (StringExtensions.IsGuid(str2, out guid))
            {
                dir = dir2.Get(guid);
            }
            if (dir == null)
            {
                RoadFlow.Model.DocDir dir1 = new RoadFlow.Model.DocDir();
                dir1.Id=Guid.NewGuid();
                dir1.ParentId=StringExtensions.ToGuid(str);
                dir = dir1;
                dir.Sort = dir2.GetMaxSort(dir.ParentId);
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["parentName"]= dir2.GetName(dir.ParentId);
            base.ViewData["refreshId"]= dir.ParentId;
            return this.View(dir);
        }

        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string DocDelete()
        {
            Guid guid;
            string str = base.Request.Querys("docid");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return "id错误!";
            }
            new Doc().Delete(guid);
            Log.Add("删除了文档-" + str, "", LogType.系统管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate(CheckApp = false)]
        public IActionResult DocEdit()
        {
            Guid guid;
            string str = base.Request.Querys("docid");
            string str2 = base.Request.Querys("dirid");
            Doc doc = new Doc();
            RoadFlow.Model.Doc doc2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                doc2 = doc.Get(guid);
            }
            if (doc2 == null)
            {
                RoadFlow.Model.Doc doc1 = new RoadFlow.Model.Doc();
                doc1.Id=Guid.NewGuid();
                doc1.DirId=StringExtensions.ToGuid(str2);
                doc1.WriteTime= DateTimeExtensions.Now;
                doc1.WriteUserID=Current.UserId;
                doc1.WriteUserName = Current.UserName;
                doc1.ReadCount = 0;
                doc1.DocRank = 0;
                doc2 = doc1;
                doc2.DirName = new DocDir().GetName(doc2.DirId);
            }
            string[] textArray1 = new string[] { "dirid=", base.Request.Querys("dirid"), "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid"), "&isnoread=", base.Request.Querys("isnoread"), "&pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber") };
            string str3 = string.Concat((string[])textArray1);
            base.ViewData["query"]= str3;
            base.ViewData["docid"]= str;
            base.ViewData["rank"]= doc.GetRankOptions(doc2.DocRank);
            return this.View(doc2);
        }

        [Validate(CheckApp = false)]
        public IActionResult Index()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult List()
        {
            Guid guid2;
            Guid userId = Current.UserId;
            string str = base.Request.Querys("dirid");
            DocDir dir = new DocDir();
            RoadFlow.Model.DocDir dir2 = StringExtensions.IsGuid(str, out guid2) ? dir.Get(guid2) : null;
            base.ViewData["isPublish"]= dir.IsPublish(dir2, userId) ? "1" : "0";
            base.ViewData["isManage"]= dir.IsManage(dir2, userId) ? "1" : "0";
            string[] textArray1 = new string[] { "dirid=", base.Request.Querys("dirid"), "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid"), "&isnoread=", base.Request.Querys("isnoread") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            base.ViewData["dirId"]= base.Request.Querys("dirid");
            base.ViewData["isNoRead"]= "0";
            return this.View();
        }

        [Validate(CheckApp = false)]
        public string Query()
        {
            Guid guid;
            int num4;
            string str = base.Request.Forms("Title1");
            string str2 = base.Request.Forms("Date1");
            string str3 = base.Request.Forms("Date2");
            string str4 = base.Request.Querys("dirid");
            string str5 = base.Request.Forms("sidx");
            string str6 = base.Request.Forms("sord");
            string str7 = base.Request.Querys("isnoread");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            bool flag = "asc".EqualsIgnoreCase(str6);
            string str8 = (str5.IsNullOrEmpty() ? "DocRank,WriteTime" : str5) + " " + (str6.IsNullOrEmpty() ? "DESC" : str6);
            DocDir dir = new DocDir();
            if (StringExtensions.IsGuid(str4, out guid))
            {
                str4 = StringExtensions.JoinSqlIn<Guid>(dir.GetAllChildsId(guid, true), true);
            }
            int num3 = "1".Equals(str7) ? 0 : -1;
            Guid userId = Current.UserId;
            DataTable table = new Doc().GetPagerList(out num4, pageSize, pageNumber, userId, str, str4, str2, str3, str8, num3);
            JArray array = new JArray();
            Dictionary dictionary = new Dictionary();
            DocUser user = new DocUser();
            foreach (DataRow row in table.Rows)
            {
                string[] textArray1 = new string[] { "<a href=\"javascript:void(0);\" class=\"blue\" onclick=\"showDoc('", row["Id"].ToString(), "');\">", row["Title"].ToString(), "</a>", user.IsRead(StringExtensions.ToGuid(row["Id"].ToString()), userId) ? "" : "<img style=\"border:0;margin-left:4px;vertical-align:middle;\" src=\"/RoadFlowResources/images/loading/new.png\"/>" };
                string str9 = string.Concat((string[])textArray1);
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Title", (JToken)str9);
                obj1.Add("DirName", (JToken)row["DirName"].ToString());
                obj1.Add("WriteUserName", (JToken)row["WriteUserName"].ToString());
                obj1.Add("WriteTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["WriteTime"].ToString())));
                obj1.Add("Rank", (JToken)dictionary.GetTitle("system_documentrank", row["DocRank"].ToString()));
                obj1.Add("ReadCount", (JToken)row["ReadCount"].ToString());
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num4, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate(CheckApp = false)]
        public string QueryRead()
        {
            int num3;
            string str = base.Request.Forms("sidx");
            string str2 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(false);
            int pageNumber = Tools.GetPageNumber();
            bool flag = "asc".EqualsIgnoreCase(str2);
            string str3 = (str.IsNullOrEmpty() ? "UserId" : str) + " " + (str2.IsNullOrEmpty() ? "ASC" : str2);
            string str4 = base.Request.Querys("docid");
            DocUser user = new DocUser();
            Organize organize = new Organize();
            User user2 = new User();
            DataTable table = user.GetDocReadPagerList(out num3, pageSize, pageNumber, str4, str3);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                DateTime time;
                JObject obj1 = new JObject();
                obj1.Add("UserId", (JToken)user2.GetName(StringExtensions.ToGuid(row["UserId"].ToString())));
                obj1.Add("Organize", (JToken)user2.GetOrganizeMainShowHtml(StringExtensions.ToGuid(row["UserId"].ToString()), true));
                obj1.Add("IsRead", row["IsRead"].ToString().Equals("0") ? "未读" : "已读");
                obj1.Add("ReadTime", StringExtensions.IsDateTime(row["ReadTime"].ToString(), out time) ? ((JToken)DateTimeExtensions.ToDateTimeString(time)) : ((JToken)""));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string SaveDirEdit(RoadFlow.Model.DocDir docDirModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            DocDir dir = new DocDir();
            if (StringExtensions.IsGuid(base.Request.Querys("currentdirid"), out guid))
            {
                RoadFlow.Model.DocDir dir2 = dir.Get(guid);
                string oldContents = (dir2 == null) ? "" : dir2.ToString();
                dir.Update(docDirModel);
                Log.Add("修改了文档栏目-" + docDirModel.Name, "", LogType.系统管理, oldContents, docDirModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                dir.Add(docDirModel);
                Log.Add("添加了文档栏目-" + docDirModel.Name, docDirModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string SaveDocEdit(RoadFlow.Model.Doc docModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            Doc doc = new Doc();
            List<RoadFlow.Model.User> allUsers = new List<RoadFlow.Model.User>();
            if (docModel.ReadUsers.IsNullOrWhiteSpace())
            {
                RoadFlow.Model.DocDir dir = new DocDir().Get(docModel.DirId);
                if (dir != null)
                {
                    allUsers = new Organize().GetAllUsers(dir.ReadUsers);
                }
            }
            else
            {
                allUsers = new Organize().GetAllUsers(docModel.ReadUsers);
            }
            if (!Enumerable.Any<RoadFlow.Model.User>((IEnumerable<RoadFlow.Model.User>)allUsers))
            {
                return "没有阅读人员!";
            }
            if (StringExtensions.IsGuid(base.Request.Querys("docid"), out guid))
            {
                RoadFlow.Model.Doc doc2 = doc.Get(guid);
                string oldContents = (doc2 == null) ? "" : doc2.ToString();
                doc.Update(docModel, allUsers);
                Log.Add("修改了文档-" + docModel.Title, "", LogType.系统管理, oldContents, docModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                doc.Add(docModel, allUsers);
                Log.Add("添加了文档-" + docModel.Title, docModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate(CheckApp = false)]
        public IActionResult Show()
        {
            Guid guid;
            string str = base.Request.Querys("docid");
            string str2 = base.Request.Querys("isread");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="文档Id错误";
                return result1;
            }
            Doc doc = new Doc();
            RoadFlow.Model.Doc doc2 = doc.Get(guid);
            if (doc2 == null)
            {
                ContentResult result2 = new ContentResult();
                result2.Content="未找到当前文档";
                return result2;
            }
            doc.UpdateReadCount(doc2);
            new DocUser().UpdateIsRead(doc2.Id, Current.UserId, 1);
            RoadFlow.Model.DocDir dir = new DocDir().Get(doc2.DirId);
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["isPublish"]= new DocDir().IsPublish(dir, Current.UserId) ? "1" : "0";
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["isread"]= str2;
            return this.View(doc2);
        }

        [Validate(CheckApp = false)]
        public IActionResult ShowRead()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Tree()
        {
            base.ViewData["appid"]= base.Request.Querys("appid");
            base.ViewData["tabid"]= base.Request.Querys("tabid");
            return this.View();
        }

        [Validate(CheckApp = false)]
        public string Tree1()
        {
            return new DocDir().GetTreeJson(Current.UserId);
        }

        [Validate(CheckApp = false)]
        public string TreeRefresh()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("refreshid"), out guid))
            {
                return "";
            }
            return new DocDir().GetRefreshJson(guid, Current.UserId);
        }
    }


   




 


}
