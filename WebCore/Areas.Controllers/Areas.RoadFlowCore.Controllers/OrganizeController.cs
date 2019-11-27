using System;
using System.Collections.Generic;
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
    public class OrganizeController : Controller
    {
        // Methods
        [Validate]
        public IActionResult Body()
        {
            Guid guid;
            string str = base.Request.Querys("orgid");
            string str2 = base.Request.Querys("orgparentid");
            string str3 = base.Request.Querys("isadddept");
            string str4 = base.Request.Querys("type");
            string str5 = base.Request.Querys("showtype");
            string str6 = base.Request.Querys("appid");
            string str7 = base.Request.Querys("tabid");
            RoadFlow.Model.Organize organize = null;
            Organize organize2 = new Organize();
            if (StringExtensions.IsGuid(str, out guid) && !"1".Equals(str3))
            {
                organize = organize2.Get(guid);
            }
            if (organize == null)
            {
                RoadFlow.Model.Organize organize1 = new RoadFlow.Model.Organize();
                organize1.Id=Guid.NewGuid();
                organize1.ParentId=StringExtensions.ToGuid(str);
                organize1.Sort = organize2.GetMaxSort(StringExtensions.ToGuid(str));
                organize = organize1;
                organize.IntId = GuidExtensions.ToInt(organize.Id);
                base.ViewData["parentsName"]= "";
            }
            else
            {
                base.ViewData["parentsName"]= organize2.GetParentsName(organize.Id, true);
            }
            base.ViewData["orgId"]= str;
            base.ViewData["isAddDept"]= str3;
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["refreshId"]= organize.ParentId;
            base.ViewData["rootId"]= organize2.GetRootId();
            string[] textArray1 = new string[] { "Body?orgid=", str, "&orgparentid=", str2, "&type=", str4, "&showtype=", str5, "&appid", str6, "&tabid=", str7 };
            base.ViewData["returnUrl"]= string.Concat((string[])textArray1);
            return this.View(organize);
        }

        public string CheckAccount()
        {
            Guid guid;
            string str = base.Request.Querys("id");
            string str2 = base.Request.Forms("value");
            if (str2.IsNullOrEmpty())
            {
                return "账号不能为空";
            }
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return "ID错误";
            }
            if (new User().CheckAccount(str2, guid))
            {
                return "帐号重复";
            }
            return "1";
        }

        public string Delete()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("orgid"), out guid))
            {
                return "id错误!";
            }
            if (guid == new Organize().GetRootId())
            {
                return "请勿删除组织机构根!";
            }
            if (new Organize().Delete(guid) <= 0)
            {
                return "删除失败!";
            }
            return "删除成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string DeleteUser()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("userid"), out guid))
            {
                return "用户ID错误";
            }
            new User().Delete(guid);
            return "删除成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string DeleteWorkGroup()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("workgroupid"), out guid))
            {
                return "ID错误";
            }
            WorkGroup group = new WorkGroup();
            RoadFlow.Model.WorkGroup workGroup = group.Get(guid);
            if (workGroup == null)
            {
                return "未找到要删除的工作组";
            }
            group.Delete(workGroup);
            Log.Add("删除了工作组-" + workGroup.Name, workGroup.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string DeptMove()
        {
            Guid guid;
            Guid guid2;
            string str = base.Request.Forms("toOrgId");
            string str2 = base.Request.Querys("orgid");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return "请选择要移动到的组织架构";
            }
            if (!StringExtensions.IsGuid(str2, out guid2))
            {
                return "没有找到当前组织架构";
            }
            if (guid == guid2)
            {
                return "不能将自己移动到自己";
            }
            Organize organize = new Organize();
            if (guid2 == organize.GetRootId())
            {
                return "不能移动根";
            }
            RoadFlow.Model.Organize organize2 = organize.Get(guid2);
            if (organize2 == null)
            {
                return "没有找到当前组织架构";
            }
            RoadFlow.Model.Organize organize3 = organize.Get(guid);
            if (organize3 == null)
            {
                return "没有找到要移动到的组织架构";
            }
            organize2.ParentId=guid;
            organize2.Sort = organize.GetMaxSort(guid);
            organize.Update(organize2);
            if (Config.Enterprise_WeiXin_IsUse)
            {
                List<RoadFlow.Model.User> allUsers = new Organize().GetAllUsers(organize2.Id, false);
                RoadFlow.Business.EnterpriseWeiXin.Organize organize4 = new RoadFlow.Business.EnterpriseWeiXin.Organize();
                foreach (RoadFlow.Model.User user in allUsers)
                {
                    organize4.UpdateUser(user);
                }
            }
            Log.Add("移动了组织架构-" + organize2.Name + "到" + organize3.Name, organize2.Id + "&" + organize3.Id, LogType.系统管理, "", "", "", "", "", "", "", "");
            return "移动成功!";
        }

        public IActionResult Empty()
        {
            return this.View();
        }

        [Validate]
        public string GetAccountByName()
        {
            return base.Request.Forms("name").ToPinYing();
        }

        [Validate]
        public IActionResult Index()
        {
          RoadFlow.Model. Organize root = new Organize().GetRoot();
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["bodyUrl"]= (root == null) ? string.Empty : string.Concat((string[])new string[] { "Body?orgid=", root.Id.ToString(), "&orgparentid=", root.ParentId.ToString(), "&type=", ((int)root.Type).ToString(), "&showtype=0&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") });
            return this.View();


            //base.ViewData["queryString"]= base.Request.UrlQuery();
            //return this.View();
        }

        [Validate, ValidateAntiForgeryToken]
        public string InitUserPassword()
        {
            Guid guid;
            string str = base.Request.Querys("userid");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return "用户ID错误!";
            }
            bool flag = new User().InitUserPassword(guid);
            Log.Add("初始化了人员密码-" + str, flag ? "初始化成功!" : "初始化失败!", LogType.系统管理, "", "", "", "", "", "", "", "");
            if (!flag)
            {
                return "初始化失败!";
            }
            return "初始化成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string MoveUser()
        {
            Guid guid;
            Guid guid2;
            string contents = base.Request.Forms("toOrgId");
            string str2 = base.Request.Forms("isjz");
            string str3 = base.Request.Querys("userid");
            if (!StringExtensions.IsGuid(contents, out guid))
            {
                return "没有选择要调往的组织";
            }
            RoadFlow.Model.Organize organize = new Organize().Get(guid);
            if (organize == null)
            {
                return "没有找到要调往的组织";
            }
            if (!StringExtensions.IsGuid(str3, out guid2))
            {
                return "人员ID错误";
            }
            OrganizeUser user = new OrganizeUser();
            if ("1".Equals(str2))
            {
                RoadFlow.Model.OrganizeUser user1 = new RoadFlow.Model.OrganizeUser();
                user1.Id=Guid.NewGuid();
                user1.IsMain = 0;
                user1.OrganizeId=guid;
                user1.UserId=guid2;
                user1.Sort = user.GetMaxSort(organize.ParentId);
                RoadFlow.Model.OrganizeUser organizeUser = user1;
                user.Add(organizeUser);
            }
            else
            {
                List<Tuple<RoadFlow.Model.OrganizeUser, int>> list = new List<Tuple<RoadFlow.Model.OrganizeUser, int>>();
                foreach (RoadFlow.Model.OrganizeUser user3 in user.GetListByUserId(guid2))
                {
                    if (user3.IsMain == 1)
                    {
                        user3.OrganizeId=guid;
                        user3.Sort = user.GetMaxSort(organize.ParentId);
                        list.Add(new Tuple<RoadFlow.Model.OrganizeUser, int>(user3, 2));
                    }
                    else
                    {
                        list.Add(new Tuple<RoadFlow.Model.OrganizeUser, int>(user3, 0));
                    }
                }
                user.Update(list);
                if (Config.Enterprise_WeiXin_IsUse)
                {
                    new RoadFlow.Business.EnterpriseWeiXin.Organize().UpdateUser(new User().Get(guid2));
                }
            }
            Log.Add("调动了人员-" + guid2 + ("1".Equals(str2) ? "-兼任" : "-全职"), contents, LogType.系统管理, "", "", "", "", "", "", "", "");
            return "调动成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string Save(RoadFlow.Model.Organize organizeModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            Organize organize = new Organize();
            if (!"1".Equals(base.Request.Querys("isadddept")) && StringExtensions.IsGuid(base.Request.Querys("orgid"), out guid))
            {
                RoadFlow.Model.Organize organize2 = organize.Get(guid);
                string oldContents = (organize2 == null) ? "" : organize2.ToString();
                organize.Update(organizeModel);
                Log.Add("修改了组织机构-" + organizeModel.Name, "", LogType.系统管理, oldContents, organizeModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                organize.Add(organizeModel);
                Log.Add("添加了组织机构-" + organizeModel.Name, organizeModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveSetMenu()
        {
            string str = base.Request.Querys("orgid");
            if (str.IsNullOrWhiteSpace())
            {
                str = base.Request.Querys("workgroupid");
                if (!str.IsNullOrWhiteSpace())
                {
                    str = "w_" + str;
                }
            }
            if (str.IsNullOrWhiteSpace())
            {
                str = base.Request.Querys("userid");
                if (!str.IsNullOrWhiteSpace())
                {
                    str = "u_" + str;
                }
            }
            MenuUser user = new MenuUser();
            Organize organize = new Organize();
            string str2 = base.Request.Forms("menuid");
            List<RoadFlow.Model.MenuUser> list = new List<RoadFlow.Model.MenuUser>();
            foreach (string str3 in str2.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str3, out guid))
                {
                    RoadFlow.Model.MenuUser user1 = new RoadFlow.Model.MenuUser();
                    user1.Id=Guid.NewGuid();
                    user1.Buttons = base.Request.Forms("button_" + str3);
                    user1.MenuId=guid;
                    user1.Organizes = str;
                    user1.Params = base.Request.Forms("params_" + str3);
                    RoadFlow.Model.MenuUser user2 = user1;
                    list.Add(user2);
                }
            }
            int num = user.Update(list.ToArray(), str);
            Log.Add("设置了组织架构菜单-" + str, "影响行数:" + ((int)num).ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            return "设置成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveSort()
        {
            string str = base.Request.Forms("sort");
            Organize organize = new Organize();
            List<RoadFlow.Model.Organize> list = new List<RoadFlow.Model.Organize>();
            int num = 0;
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.Organize organize2 = organize.Get(guid);
                    if (organize2 != null)
                    {
                        organize2.Sort = num += 5;
                        list.Add(organize2);
                    }
                }
            }
            organize.Update(list.ToArray());
            return "排序成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveUser(RoadFlow.Model.User userModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            User user = new User();
            if (!"1".Equals(base.Request.Querys("isadduser")) && StringExtensions.IsGuid(base.Request.Querys("userid"), out guid))
            {
                RoadFlow.Model.User user2 = user.Get(guid);
                string oldContents = (user2 == null) ? "" : user2.ToString();
                user.Update(userModel);
                if (Config.Enterprise_WeiXin_IsUse)
                {
                    new RoadFlow.Business.EnterpriseWeiXin.Organize().UpdateUser(userModel);
                }
                Log.Add("修改了人员-" + userModel.Name, "", LogType.系统管理, oldContents, userModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                Guid guid2;
                if (!StringExtensions.IsGuid(base.Request.Querys("orgid"), out guid2))
                {
                    return "未找到人员对应的组织架构";
                }
                RoadFlow.Model.OrganizeUser user1 = new RoadFlow.Model.OrganizeUser();
                user1.Id=Guid.NewGuid();
                user1.IsMain = 1;
                user1.OrganizeId=guid2;
                user1.UserId=userModel.Id;
                user1.Sort = new OrganizeUser().GetMaxSort(guid2);
                RoadFlow.Model.OrganizeUser organizeUser = user1;
                userModel.Password = user.GetInitPassword(userModel.Id);
                user.Add(userModel, organizeUser);
                if (Config.Enterprise_WeiXin_IsUse)
                {
                    new RoadFlow.Business.EnterpriseWeiXin.Organize().AddUser(userModel);
                }
                Log.Add("添加了人员-" + userModel.Name, userModel.ToString() + "-" + organizeUser.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveWorkGroup(RoadFlow.Model.WorkGroup workGroupModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            WorkGroup group = new WorkGroup();
            if (StringExtensions.IsGuid(base.Request.Querys("workgroupid"), out guid))
            {
                RoadFlow.Model.WorkGroup group2 = group.Get(guid);
                string oldContents = (group2 == null) ? "" : group2.ToString();
                group.Update(workGroupModel);
                Log.Add("修改了工作组-" + workGroupModel.Name, "", LogType.系统管理, oldContents, workGroupModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                group.Add(workGroupModel);
                Log.Add("添加了工作组-" + workGroupModel.Name, workGroupModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate]
        public IActionResult SetMenu()
        {
            string str = base.Request.Querys("orgid");
            if (str.IsNullOrWhiteSpace())
            {
                str = base.Request.Querys("workgroupid");
                if (!str.IsNullOrWhiteSpace())
                {
                    str = "w_" + str;
                }
            }
            if (str.IsNullOrWhiteSpace())
            {
                str = base.Request.Querys("userid");
                if (!str.IsNullOrWhiteSpace())
                {
                    str = "u_" + str;
                }
            }
            base.ViewData["tableHtml"]= new Menu().GetMenuTreeTableHtml(str, (Guid?)null);
            base.ViewData["prevUrl"]= base.Request.Querys("prevurl").UrlDecode();
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult ShowUserMenu()
        {
            Guid guid;
            string name = string.Empty;
            string str2 = base.Request.Querys("userid");
            if (StringExtensions.IsGuid(str2, out guid))
            {
                name = new User().GetName(guid);
            }
            base.ViewData["prevUrl"]= base.Request.Querys("prevUrl");
            base.ViewData["userName"]= name;
            base.ViewData["userId"]= str2;
            return this.View();
        }

        [Validate]
        public IActionResult Sort()
        {
            Guid guid;
            string str = base.Request.Querys("orgparentid");
            IEnumerable<RoadFlow.Model.Organize> childs = (IEnumerable<RoadFlow.Model.Organize>)new List<RoadFlow.Model.Organize>();
            if (StringExtensions.IsGuid(str, out guid))
            {
                childs = (IEnumerable<RoadFlow.Model.Organize>)new Organize().GetChilds(guid);
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["refreshId"]= str;
            return this.View(childs);
        }

        [Validate]
        public IActionResult Tree()
        {
            string str = base.Request.Querys("appid");
            string str2 = base.Request.Querys("tabid");
            base.ViewData["query"]= "appid=" + str + "&tabid=" + str2;
            base.ViewData["appId"]= str;
            return this.View();
        }

        [Validate(CheckApp = false)]
        public string Tree1()
        {
            int num = base.Request.Querys("showtype").ToString().ToInt(0);
            string str = base.Request.Querys("rootid");
            string searchWord = base.Request.Querys("searchword");
            bool flag = !"0".Equals(base.Request.Querys("shouser"));
            Organize organize = new Organize();
            User user = new User();
            WorkGroup group = new WorkGroup();
            OrganizeUser user2 = new OrganizeUser();
            List<RoadFlow.Model.OrganizeUser> all = user2.GetAll();
            Guid rootId = organize.GetRootId();
            JArray array = new JArray();
            if (!searchWord.IsNullOrWhiteSpace())
            {
                Guid guid2 = Guid.NewGuid();
                if (1 == num)
                {
                    List<RoadFlow.Model.WorkGroup> list2 = group.GetAll().FindAll(delegate (RoadFlow.Model.WorkGroup p) {
                        return p.Name.ContainsIgnoreCase(searchWord.Trim());
                    });
                    JObject obj1 = new JObject();
                    obj1.Add("id", (JToken)guid2);
                    obj1.Add("parentID", (JToken)Guid.Empty);
                    obj1.Add("title", (JToken)("查询“" + searchWord + "”的结果"));
                    obj1.Add("ico", "fa-search");
                    obj1.Add("link", "");
                    obj1.Add("type", 1);
                    obj1.Add("hasChilds", (JToken)list2.Count);
                    JObject obj2 = obj1;
                    array.Add(obj2);
                    JArray array2 = new JArray();
                    foreach (RoadFlow.Model.WorkGroup group2 in list2)
                    {
                        JObject obj17 = new JObject();
                        obj17.Add("id", (JToken)group2.Id);
                        obj17.Add("parentID", (JToken)guid2);
                        obj17.Add("title", (JToken)group2.Name);
                        obj17.Add("ico", "fa-slideshare");
                        obj17.Add("link", "");
                        obj17.Add("type", 5);
                        obj17.Add("hasChilds", 0);
                        JObject obj3 = obj17;
                        array2.Add(obj3);
                    }
                    obj2.Add("childs", array2);
                }
                else
                {
                    List<RoadFlow.Model.Organize> list3 = organize.GetAll().FindAll(delegate (RoadFlow.Model.Organize p) {
                        return p.Name.ContainsIgnoreCase(searchWord.Trim());
                    });
                    List<RoadFlow.Model.User> list4 = user.GetAll().FindAll(delegate (RoadFlow.Model.User p) {
                        return p.Name.ContainsIgnoreCase(searchWord.Trim());
                    });
                    JObject obj18 = new JObject();
                    obj18.Add("id", (JToken)guid2);
                    obj18.Add("parentID", (JToken)Guid.Empty);
                    obj18.Add("title", (JToken)("查询“" + searchWord + "”的结果"));
                    obj18.Add("ico", "fa-search");
                    obj18.Add("link", "");
                    obj18.Add("type", 1);
                    obj18.Add("hasChilds", list3.Count + list4.Count);
                    JObject obj4 = obj18;
                    array.Add(obj4);
                    JArray array3 = new JArray();
                    foreach (RoadFlow.Model.Organize organize2 in list3)
                    {
                        JObject obj19 = new JObject();
                        obj19.Add("id", (JToken)organize2.Id);
                        obj19.Add("parentID", (JToken)guid2);
                        obj19.Add("title", (JToken)organize2.Name);
                        obj19.Add("ico", "");
                        obj19.Add("link", "");
                        obj19.Add("type", (JToken)organize2.Type);
                        obj19.Add("hasChilds", organize.HasChilds(organize2.Id) ? 1 : 0);
                        JObject obj5 = obj19;
                        array3.Add(obj5);
                    }
                    foreach (RoadFlow.Model.User user3 in list4)
                    {
                        JObject obj20 = new JObject();
                        obj20.Add("id", (JToken)user3.Id);
                        obj20.Add("parentID", (JToken)guid2);
                        obj20.Add("title", (JToken)user3.Name);
                        obj20.Add("ico", "fa-user");
                        obj20.Add("link", "");
                        obj20.Add("userID", (JToken)user3.Id);
                        obj20.Add("type", 4);
                        obj20.Add("hasChilds", 0);
                        JObject obj6 = obj20;
                        array3.Add(obj6);
                    }
                    obj4.Add("childs", array3);
                }
                return array.ToString();
            }
            if (1 == num)
            {
                foreach (RoadFlow.Model.WorkGroup group3 in group.GetAll())
                {
                    JObject obj21 = new JObject();
                    obj21.Add("id", (JToken)group3.Id);
                    obj21.Add("parentID", (JToken)Guid.Empty);
                    obj21.Add("title", (JToken)group3.Name);
                    obj21.Add("ico", "fa-slideshare");
                    obj21.Add("link", "");
                    obj21.Add("type", 5);
                    obj21.Add("hasChilds", 0);
                    JObject obj7 = obj21;
                    array.Add(obj7);
                }
                return array.ToString();
            }
            if (str.IsNullOrWhiteSpace())
            {
                str = rootId.ToString();
            }
            char[] chArray1 = new char[] { ',' };
            string[] strArray = str.Split(chArray1, (StringSplitOptions)StringSplitOptions.RemoveEmptyEntries);
            foreach (string str2 in strArray)
            {
                Guid guid3;
                if (StringExtensions.IsGuid(str2, out guid3))
                {
                    RoadFlow.Model.Organize organize3 = organize.Get(guid3);
                    if (organize3 != null)
                    {
                        JObject obj22 = new JObject();
                        obj22.Add("id", (JToken)organize3.Id);
                        obj22.Add("parentID", (JToken)organize3.ParentId);
                        obj22.Add("title", (JToken)organize3.Name);
                        obj22.Add("ico", (organize3.Id == rootId) ? "fa-sitemap" : "");
                        obj22.Add("link", "");
                        obj22.Add("type", (JToken)organize3.Type);
                        obj22.Add("hasChilds", organize.HasChilds(organize3.Id) ? 1 : (flag ? (organize.HasUsers(organize3.Id) ? 1 : 0) : 0));
                        JObject obj8 = obj22;
                        array.Add(obj8);
                    }
                }
                else if (str2.StartsWith("u_"))
                {
                    RoadFlow.Model.User user4 = user.Get(StringExtensions.ToGuid(str2.RemoveUserPrefix()));
                    if (user4 != null)
                    {
                        JObject obj23 = new JObject();
                        obj23.Add("id", (JToken)user4.Id);
                        obj23.Add("parentID", (JToken)Guid.Empty);
                        obj23.Add("title", (JToken)user4.Name);
                        obj23.Add("ico", "fa-user");
                        obj23.Add("link", "");
                        obj23.Add("type", 4);
                        obj23.Add("hasChilds", 0);
                        JObject obj9 = obj23;
                        array.Add(obj9);
                    }
                }
                else if (str2.StartsWith("r_"))
                {
                    RoadFlow.Model.OrganizeUser user5 = user2.Get(StringExtensions.ToGuid(str2.RemoveUserRelationPrefix()));
                    if (user5 != null)
                    {
                        RoadFlow.Model.User user6 = user.Get(user5.UserId);
                        if (user6 != null)
                        {
                            JObject obj24 = new JObject();
                            obj24.Add("id", (JToken)user5.Id);
                            obj24.Add("parentID", (JToken)Guid.Empty);
                            obj24.Add("title", (JToken)(user6.Name + "<span style='color:#666;'>[兼任]</span>"));
                            obj24.Add("ico", "fa-user");
                            obj24.Add("link", "");
                            obj24.Add("type", 6);
                            obj24.Add("userID", (JToken)user6.Id);
                            obj24.Add("hasChilds", 0);
                            JObject obj10 = obj24;
                            array.Add(obj10);
                        }
                    }
                }
                else if (str2.StartsWith("w_"))
                {
                    RoadFlow.Model.WorkGroup group4 = group.Get(StringExtensions.ToGuid(str2.RemoveWorkGroupPrefix()));
                    if (group4 != null)
                    {
                        JObject obj25 = new JObject();
                        obj25.Add("id", (JToken)group4.Id);
                        obj25.Add("parentID", (JToken)Guid.Empty);
                        obj25.Add("title", (JToken)group4.Name);
                        obj25.Add("ico", "fa-slideshare");
                        obj25.Add("link", "");
                        obj25.Add("type", 5);
                        obj25.Add("hasChilds", (JToken)group.GetAllUsers(group4.Id).Count);
                        JObject obj11 = obj25;
                        array.Add(obj11);
                    }
                }
            }
            if (strArray.Length == 1)
            {
                Guid guid;
                string str3 = strArray[0];
                if (StringExtensions.IsGuid(str3, out guid))
                {
                    JObject obj12 =(JObject) array[0];
                    JArray array4 = new JArray();
                    List<RoadFlow.Model.Organize> childs = organize.GetChilds(guid);
                    List<RoadFlow.Model.OrganizeUser> list7 = all.FindAll(delegate (RoadFlow.Model.OrganizeUser p) {
                        return p.OrganizeId == guid;
                    });
                    foreach (RoadFlow.Model.Organize organize4 in childs)
                    {
                        JObject obj26 = new JObject();
                        obj26.Add("id", (JToken)organize4.Id);
                        obj26.Add("parentID", (JToken)organize4.ParentId);
                        obj26.Add("title", (JToken)organize4.Name);
                        obj26.Add("ico", "");
                        obj26.Add("link", "");
                        obj26.Add("type", (JToken)organize4.Type);
                        obj26.Add("hasChilds", organize.HasChilds(organize4.Id) ? 1 : (flag ? (organize.HasUsers(organize4.Id) ? 1 : 0) : 0));
                        JObject obj13 = obj26;
                        array4.Add(obj13);
                    }
                    if (flag)
                    {
                        using (List<RoadFlow.Model.User>.Enumerator enumerator6 = organize.GetUsers(guid, true).GetEnumerator())
                        {
                            while (enumerator6.MoveNext())
                            {
                                RoadFlow.Model.User userModel = enumerator6.Current;
                                RoadFlow.Model.OrganizeUser user7 = list7.Find(delegate (RoadFlow.Model.OrganizeUser p) {
                                    return p.UserId == userModel.Id;
                                });
                                bool flag2 = user7.IsMain != 1;
                                JObject obj27 = new JObject();
                                obj27.Add("id", flag2 ? ((JToken)user7.Id) : ((JToken)userModel.Id));
                                obj27.Add("parentID", (JToken)guid);
                                obj27.Add("title", (JToken)(userModel.Name + (flag2 ? "<span style='color:#666;'>[兼任]</span>" : "")));
                                obj27.Add("ico", "fa-user");
                                obj27.Add("link", "");
                                obj27.Add("userID", (JToken)userModel.Id);
                                obj27.Add("type", flag2 ? 6 : 4);
                                obj27.Add("hasChilds", 0);
                                JObject obj14 = obj27;
                                array4.Add(obj14);
                            }
                        }
                    }
                    obj12.Add("childs", array4);
                }
                else if (str3.StartsWith("w_"))
                {
                    JObject obj15 =(JObject) array[0];
                    JArray array5 = new JArray();
                    foreach (RoadFlow.Model.User user8 in group.GetAllUsers(StringExtensions.ToGuid(str3.RemoveWorkGroupPrefix())))
                    {
                        JObject obj28 = new JObject();
                        obj28.Add("id", (JToken)user8.Id);
                        obj28.Add("parentID", (JToken)str3.RemoveWorkGroupPrefix());
                        obj28.Add("title", (JToken)user8.Name);
                        obj28.Add("ico", "fa-user");
                        obj28.Add("link", "");
                        obj28.Add("type", 4);
                        obj28.Add("hasChilds", 0);
                        JObject obj16 = obj28;
                        array5.Add(obj16);
                    }
                    obj15.Add("childs", array5);
                }
            }
            return array.ToString();
        }

        [Validate(CheckApp = false)]
        public string TreeRefresh()
        {
            Guid guid;
            int num = base.Request.Querys("showtype").ToString().ToInt(0);
            bool flag = !"0".Equals(base.Request.Querys("shouser"));
            string str = base.Request.Querys("refreshid");
            Organize organize = new Organize();
            JArray array = new JArray();
            if (1 == num)
            {
                return "";
            }
            if (StringExtensions.IsGuid(str, out guid))
            {
                foreach (RoadFlow.Model.Organize organize2 in organize.GetChilds(guid))
                {
                    JObject obj1 = new JObject();
                    obj1.Add("id", (JToken)organize2.Id);
                    obj1.Add("parentID", (JToken)organize2.ParentId);
                    obj1.Add("title", (JToken)organize2.Name);
                    obj1.Add("ico", "");
                    obj1.Add("link", "");
                    obj1.Add("type", (JToken)organize2.Type);
                    obj1.Add("hasChilds", organize.HasChilds(organize2.Id) ? 1 : (flag ? (organize.HasUsers(organize2.Id) ? 1 : 0) : 0));
                    JObject obj2 = obj1;
                    array.Add(obj2);
                }
                if (flag)
                {
                    List<RoadFlow.Model.User> users = organize.GetUsers(guid, true);
                    List<RoadFlow.Model.OrganizeUser> listByOrganizeId = new OrganizeUser().GetListByOrganizeId(guid);
                    using (List<RoadFlow.Model.User>.Enumerator enumerator2 = users.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            RoadFlow.Model.User userModel = enumerator2.Current;
                            RoadFlow.Model.OrganizeUser user = listByOrganizeId.Find(delegate (RoadFlow.Model.OrganizeUser p) {
                                return p.UserId == userModel.Id;
                            });
                            bool flag2 = user.IsMain != 1;
                            JObject obj4 = new JObject();
                            obj4.Add("id", flag2 ? ((JToken)user.Id) : ((JToken)userModel.Id));
                            obj4.Add("parentID", (JToken)guid);
                            obj4.Add("title", (JToken)(userModel.Name + (flag2 ? "<span style='color:#666;'>[兼任]</span>" : "")));
                            obj4.Add("ico", "fa-user");
                            obj4.Add("link", "");
                            obj4.Add("userID", (JToken)userModel.Id);
                            obj4.Add("type", flag2 ? 6 : 4);
                            obj4.Add("hasChilds", 0);
                            JObject obj3 = obj4;
                            array.Add(obj3);
                        }
                    }
                }
            }
            return array.ToString();
        }

        [Validate]
        public IActionResult Users()
        {
            Guid guid;
            string str = base.Request.Querys("userid");
            string str2 = base.Request.Querys("type");
            string str3 = base.Request.Querys("orgparentid");
            string str4 = base.Request.Querys("orgid");
            string str5 = base.Request.Querys("appid");
            string str6 = base.Request.Querys("tabid");
            string str7 = base.Request.Querys("isadduser");
            User user = new User();
            RoadFlow.Model.User user2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                user2 = user.Get(guid);
            }
            if (user2 == null)
            {
                RoadFlow.Model.User user1 = new RoadFlow.Model.User();
                user1.Id=Guid.NewGuid();
                user1.Password = "1";
                user2 = user1;
                base.ViewData["organizes"]= "";
                base.ViewData["workgroups"]= "";
            }
            else
            {
                base.ViewData["organizes"]= user.GetOrganizesShowHtml(user2.Id, true);
                base.ViewData["workgroups"]= user.GetWorkGroupsName(user2.Id);
            }
            string[] textArray1 = new string[] { "userid=", base.Request.Querys("userid"), "&orgparentid=", base.Request.Querys("orgparentid"), "&type=", base.Request.Querys("type"), "&showtype=", base.Request.Querys("showtype"), "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            string str8 = string.Concat((string[])textArray1);
            base.ViewData["isAddUser"]= str7;
            base.ViewData["refreshId"]=str4.IsGuid() ? str4 : str3;
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["prevUrl"]= ("Users?" + str8).UrlEncode();
            return this.View(user2);
        }

        [Validate]
        public IActionResult UserSort()
        {
            string str = base.Request.Querys("orgparentid");
            Organize organize = new Organize();
            User user = new User();
            List<RoadFlow.Model.User> allUsers = organize.GetAllUsers(StringExtensions.ToGuid(str), true);
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["refreshId"]= str;
            return this.View(allUsers);
        }

        [Validate, ValidateAntiForgeryToken]
        public string UserSortSave()
        {
            string str = base.Request.Forms("sort");
            string str2 = base.Request.Querys("orgparentid");
            OrganizeUser user = new OrganizeUser();
            List<RoadFlow.Model.OrganizeUser> listByOrganizeId = user.GetListByOrganizeId(StringExtensions.ToGuid(str2));
            int num = 0;
            foreach (string str3 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid userId;
                if (StringExtensions.IsGuid(str3, out userId))
                {
                    RoadFlow.Model.OrganizeUser user2 = listByOrganizeId.Find(delegate (RoadFlow.Model.OrganizeUser p) {
                        return p.UserId == userId;
                    });
                    if (user2 != null)
                    {
                        user2.Sort = num += 5;
                    }
                }
            }
            user.Update(listByOrganizeId.ToArray());
            return "排序成功!";
        }

        [Validate]
        public IActionResult WorkGroup()
        {
            Guid guid;
            string str = base.Request.Querys("workgroupid");
            string str2 = base.Request.Querys("type");
            string str3 = base.Request.Querys("showtype");
            string str4 = base.Request.Querys("appid");
            string str5 = base.Request.Querys("tabid");
            string[] textArray1 = new string[] { "type=", str2, "&showtype=", str3, "&appid=", str4, "&tabid=", str5 };
            string str6 = string.Concat((string[])textArray1);
            RoadFlow.Model.WorkGroup group = null;
            WorkGroup group2 = new WorkGroup();
            if (StringExtensions.IsGuid(str, out guid))
            {
                group = group2.Get(guid);
            }
            if (group == null)
            {
                RoadFlow.Model.WorkGroup group1 = new RoadFlow.Model.WorkGroup();
                group1.Id=Guid.NewGuid();
                group1.Sort = group2.GetMaxSort();
                group = group1;
                group.IntId = GuidExtensions.ToInt(group.Id);
            }
            base.ViewData["query"]= str6;
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["prevUrl"]= ("WorkGroup" + base.Request.UrlQuery()).UrlEncode();
            return this.View(group);
        }

        [Validate]
        public IActionResult WorkGroupSort()
        {
            List<RoadFlow.Model.WorkGroup> all = new WorkGroup().GetAll();
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View(all);
        }

        [Validate, ValidateAntiForgeryToken]
        public string WorkGroupSortSave()
        {
            WorkGroup group = new WorkGroup();
            List<RoadFlow.Model.WorkGroup> all = group.GetAll();
            string str = base.Request.Forms("sort");
            int num = 0;
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid wid;
                if (StringExtensions.IsGuid(str2, out wid))
                {
                    RoadFlow.Model.WorkGroup group2 = all.Find(delegate (RoadFlow.Model.WorkGroup p) {
                        return p.Id == wid;
                    });
                    if (group2 != null)
                    {
                        group2.Sort = num += 5;
                    }
                }
            }
            group.Update(all.ToArray());
            return "排序成功!";
        }
    }


   



}
