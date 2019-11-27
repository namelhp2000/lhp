using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCoreApi.Controllers
{
    [Route("RoadFlowCoreApi/[controller]"), ApiController]
    public class OrganizeController : ControllerBase
    {
        /// <summary>
        /// 获取组织第一级
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetOneLevel"), ApiValidate]
        public string GetOneLevel()
        {
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }

            int num = bodyJObject.Value<string>("showtype").ToInt(0);
            string str2 = bodyJObject.Value<string>("rootid");
            string searchWord = bodyJObject.Value<string>("searchword");
            bool flag = true;
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
                    JObject obj18 = new JObject();
                    obj18.Add("id", (JToken)guid2);
                    obj18.Add("parentID", (JToken)Guid.Empty);
                    obj18.Add("title", (JToken)("查询“" + searchWord + "”的结果"));
                    obj18.Add("ico", "fa-search");
                    obj18.Add("link", "");
                    obj18.Add("type", 1);
                    obj18.Add("members", "");
                    obj18.Add("hasChilds", (JToken)list2.Count);
                    JObject obj3 = obj18;
                    array.Add(obj3);
                    JArray array2 = new JArray();
                    foreach (RoadFlow.Model.WorkGroup group2 in list2)
                    {
                        JObject obj19 = new JObject();
                        obj19.Add("id", (JToken)group2.Id);
                        obj19.Add("parentID", (JToken)guid2);
                        obj19.Add("title", (JToken)group2.Name);
                        obj19.Add("ico", "fa-slideshare");
                        obj19.Add("link", "");
                        obj19.Add("type", 5);
                        obj19.Add("members", (JToken)group2.Members);
                        obj19.Add("hasChilds", 0);
                        JObject obj4 = obj19;
                        array2.Add(obj4);
                    }
                    obj3.Add("childs", array2);
                }
                else
                {
                    List<RoadFlow.Model.Organize> list3 = organize.GetAll().FindAll(delegate (RoadFlow.Model.Organize p) {
                        return p.Name.ContainsIgnoreCase(searchWord.Trim());
                    });
                    List<RoadFlow.Model.User> list4 = user.GetAll().FindAll(delegate (RoadFlow.Model.User p) {
                        return p.Name.ContainsIgnoreCase(searchWord.Trim());
                    });
                    JObject obj20 = new JObject();
                    obj20.Add("id", (JToken)guid2);
                    obj20.Add("parentID", (JToken)Guid.Empty);
                    obj20.Add("title", (JToken)("查询“" + searchWord + "”的结果"));
                    obj20.Add("ico", "fa-search");
                    obj20.Add("link", "");
                    obj20.Add("type", 1);
                    obj20.Add("leader", "");
                    obj20.Add("charegLeader", "");
                    obj20.Add("hasChilds", list3.Count + list4.Count);
                    JObject obj5 = obj20;
                    array.Add(obj5);
                    JArray array3 = new JArray();
                    foreach (RoadFlow.Model.Organize organize2 in list3)
                    {
                        JObject obj21 = new JObject();
                        obj21.Add("id", (JToken)organize2.Id);
                        obj21.Add("parentID", (JToken)guid2);
                        obj21.Add("title", (JToken)organize2.Name);
                        obj21.Add("ico", "");
                        obj21.Add("link", "");
                        obj21.Add("type", (JToken)organize2.Type);
                        obj21.Add("leader", (JToken)organize2.Leader);
                        obj21.Add("charegLeader", (JToken)organize2.ChargeLeader);
                        obj21.Add("hasChilds", organize.HasChilds(organize2.Id) ? 1 : 0);
                        JObject obj6 = obj21;
                        array3.Add(obj6);
                    }
                    foreach (RoadFlow.Model.User user3 in list4)
                    {
                        JObject obj22 = new JObject();
                        obj22.Add("id", (JToken)user3.Id);
                        obj22.Add("parentID", (JToken)guid2);
                        obj22.Add("title", (JToken)user3.Name);
                        obj22.Add("ico", "fa-user");
                        obj22.Add("link", "");
                        obj22.Add("userID", (JToken)user3.Id);
                        obj22.Add("type", 4);
                        obj22.Add("hasChilds", 0);
                        JObject obj7 = obj22;
                        array3.Add(obj7);
                    }
                    obj5.Add("childs", array3);
                }
                return array.ToString();
            }
            if (1 == num)
            {
                List<RoadFlow.Model.WorkGroup> list5 = group.GetAll();
                foreach (RoadFlow.Model.WorkGroup group3 in list5)
                {
                    JObject obj23 = new JObject();
                    obj23.Add("id", (JToken)group3.Id);
                    obj23.Add("parentID", (JToken)Guid.Empty);
                    obj23.Add("title", (JToken)group3.Name);
                    obj23.Add("ico", "fa-slideshare");
                    obj23.Add("link", "");
                    obj23.Add("type", 5);
                    obj23.Add("members", (JToken)group3.Members);
                    obj23.Add("hasChilds", 0);
                    JObject obj8 = obj23;
                    array.Add(obj8);
                }
                return array.ToString();
            }
            if (str2.IsNullOrWhiteSpace())
            {
                str2 = rootId.ToString();
            }
            char[] chArray1 = new char[] { ',' };
            string[] strArray = str2.Split(chArray1, (StringSplitOptions)StringSplitOptions.RemoveEmptyEntries);
            foreach (string str4 in strArray)
            {
                Guid guid3;
                if (StringExtensions.IsGuid(str4, out guid3))
                {
                    RoadFlow.Model.Organize organize3 = organize.Get(guid3);
                    if (organize3 != null)
                    {
                        JObject obj24 = new JObject();
                        obj24.Add("id", (JToken)organize3.Id);
                        obj24.Add("parentID", (JToken)organize3.ParentId);
                        obj24.Add("title", (JToken)organize3.Name);
                        obj24.Add("ico", (organize3.Id == rootId) ? "fa-sitemap" : "");
                        obj24.Add("link", "");
                        obj24.Add("type", (JToken)organize3.Type);
                        obj24.Add("leader", (JToken)organize3.Leader);
                        obj24.Add("charegLeader", (JToken)organize3.ChargeLeader);
                        obj24.Add("hasChilds", organize.HasChilds(organize3.Id) ? 1 : (flag ? (organize.HasUsers(organize3.Id) ? 1 : 0) : 0));
                        JObject obj9 = obj24;
                        array.Add(obj9);
                    }
                }
                else if (str4.StartsWith("u_"))
                {
                    RoadFlow.Model.User user4 = user.Get(StringExtensions.ToGuid(str4.RemoveUserPrefix()));
                    if (user4 != null)
                    {
                        JObject obj25 = new JObject();
                        obj25.Add("id", (JToken)user4.Id);
                        obj25.Add("parentID", (JToken)Guid.Empty);
                        obj25.Add("title", (JToken)user4.Name);
                        obj25.Add("ico", "fa-user");
                        obj25.Add("link", "");
                        obj25.Add("type", 4);
                        obj25.Add("hasChilds", 0);
                        JObject obj10 = obj25;
                        array.Add(obj10);
                    }
                }
                else if (str4.StartsWith("r_"))
                {
                    RoadFlow.Model.OrganizeUser user5 = user2.Get(StringExtensions.ToGuid(str4.RemoveUserRelationPrefix()));
                    if (user5 != null)
                    {
                        RoadFlow.Model.User user6 = user.Get(user5.UserId);
                        if (user6 != null)
                        {
                            JObject obj26 = new JObject();
                            obj26.Add("id", (JToken)user5.Id);
                            obj26.Add("parentID", (JToken)Guid.Empty);
                            obj26.Add("title", (JToken)(user6.Name + "<span style='color:#666;'>[兼任]</span>"));
                            obj26.Add("ico", "fa-user");
                            obj26.Add("link", "");
                            obj26.Add("type", 6);
                            obj26.Add("userID", (JToken)user6.Id);
                            obj26.Add("hasChilds", 0);
                            JObject obj11 = obj26;
                            array.Add(obj11);
                        }
                    }
                }
                else if (str4.StartsWith("w_"))
                {
                    RoadFlow.Model.WorkGroup group4 = group.Get(StringExtensions.ToGuid(str4.RemoveWorkGroupPrefix()));
                    if (group4 != null)
                    {
                        JObject obj27 = new JObject();
                        obj27.Add("id", (JToken)group4.Id);
                        obj27.Add("parentID", (JToken)Guid.Empty);
                        obj27.Add("title", (JToken)group4.Name);
                        obj27.Add("ico", "fa-slideshare");
                        obj27.Add("link", "");
                        obj27.Add("type", 5);
                        obj27.Add("members", (JToken)group4.Members);
                        obj27.Add("hasChilds", (JToken)group.GetAllUsers(group4.Id).Count);
                        JObject obj12 = obj27;
                        array.Add(obj12);
                    }
                }
            }
            if (strArray.Length == 1)
            {
                Guid guid;
                string str5 = strArray[0];
                if (StringExtensions.IsGuid(str5, out guid))
                {
                    JObject obj13 =(JObject)array[0];
                    JArray array4 = new JArray();
                    List<RoadFlow.Model.Organize> childs = organize.GetChilds(guid);
                    List<RoadFlow.Model.OrganizeUser> list7 = all.FindAll(delegate (RoadFlow.Model.OrganizeUser p) {
                        return p.OrganizeId == guid;
                    });
                    foreach (RoadFlow.Model.Organize organize4 in childs)
                    {
                        JObject obj28 = new JObject();
                        obj28.Add("id", (JToken)organize4.Id);
                        obj28.Add("parentID", (JToken)organize4.ParentId);
                        obj28.Add("title", (JToken)organize4.Name);
                        obj28.Add("ico", "");
                        obj28.Add("link", "");
                        obj28.Add("type", (JToken)organize4.Type);
                        obj28.Add("leader", (JToken)organize4.Leader);
                        obj28.Add("charegLeader", (JToken)organize4.ChargeLeader);
                        obj28.Add("hasChilds", organize.HasChilds(organize4.Id) ? 1 : (flag ? (organize.HasUsers(organize4.Id) ? 1 : 0) : 0));
                        JObject obj14 = obj28;
                        array4.Add(obj14);
                    }
                    if (flag)
                    {
                        List<RoadFlow.Model.User> users = organize.GetUsers(guid, true);
                        using (List<RoadFlow.Model.User>.Enumerator enumerator6 = users.GetEnumerator())
                        {
                            while (enumerator6.MoveNext())
                            {
                                RoadFlow.Model.User userModel = enumerator6.Current;
                                RoadFlow.Model.OrganizeUser user7 = list7.Find(delegate (RoadFlow.Model.OrganizeUser p) {
                                    return p.UserId == userModel.Id;
                                });
                                bool flag18 = user7.IsMain != 1;
                                JObject obj29 = new JObject();
                                obj29.Add("id", flag18 ? ((JToken)user7.Id) : ((JToken)userModel.Id));
                                obj29.Add("parentID", (JToken)guid);
                                obj29.Add("title", (JToken)(userModel.Name + (flag18 ? "<span style='color:#666;'>[兼任]</span>" : "")));
                                obj29.Add("ico", "fa-user");
                                obj29.Add("link", "");
                                obj29.Add("userID", (JToken)userModel.Id);
                                obj29.Add("type", flag18 ? 6 : 4);
                                obj29.Add("hasChilds", 0);
                                JObject obj15 = obj29;
                                array4.Add(obj15);
                            }
                        }
                    }
                    obj13.Add("childs", array4);
                }
                else if (str5.StartsWith("w_"))
                {
                    JObject obj16 =(JObject)array[0];
                    JArray array5 = new JArray();
                    List<RoadFlow.Model.User> allUsers = group.GetAllUsers(StringExtensions.ToGuid(str5.RemoveWorkGroupPrefix()));
                    foreach (RoadFlow.Model.User user8 in allUsers)
                    {
                        JObject obj30 = new JObject();
                        obj30.Add("id", (JToken)user8.Id);
                        obj30.Add("parentID", (JToken)str5.RemoveWorkGroupPrefix());
                        obj30.Add("title", (JToken)user8.Name);
                        obj30.Add("ico", "fa-user");
                        obj30.Add("link", "");
                        obj30.Add("type", 4);
                        obj30.Add("hasChilds", 0);
                        JObject obj17 = obj30;
                        array5.Add(obj17);
                    }
                    obj16.Add("childs", array5);
                }
            }
            return array.ToString(0, Array.Empty<JsonConverter>());
        }


        /// <summary>
        /// 获取组织第二级
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetSecondLevel"), ApiValidate]
        public string GetSecondLevel()
        {
            Guid guid;
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }

            int num = bodyJObject.Value<string>("showtype").ToInt(0);
            bool flag = true;
            string str2 = bodyJObject.Value<string>("refreshid");
            Organize organize = new Organize();
            JArray array = new JArray();
            if (1 == num)
            {
                return "";
            }
            if (StringExtensions.IsGuid(str2, out guid))
            {
                List<RoadFlow.Model.Organize> childs = organize.GetChilds(guid);
                foreach (RoadFlow.Model.Organize organize2 in childs)
                {
                    JObject obj5 = new JObject();
                    obj5.Add("id", (JToken)organize2.Id);
                    obj5.Add("parentID", (JToken)organize2.ParentId);
                    obj5.Add("title", (JToken)organize2.Name);
                    obj5.Add("ico", "");
                    obj5.Add("link", "");
                    obj5.Add("type", (JToken)organize2.Type);
                    obj5.Add("leader", (JToken)organize2.Leader);
                    obj5.Add("charegLeader", (JToken)organize2.ChargeLeader);
                    obj5.Add("hasChilds", organize.HasChilds(organize2.Id) ? 1 : (flag ? (organize.HasUsers(organize2.Id) ? 1 : 0) : 0));
                    JObject obj3 = obj5;
                    array.Add(obj3);
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
                            bool flag5 = user.IsMain != 1;
                            JObject obj6 = new JObject();
                            obj6.Add("id", flag5 ? ((JToken)user.Id) : ((JToken)userModel.Id));
                            obj6.Add("parentID", (JToken)guid);
                            obj6.Add("title", (JToken)(userModel.Name + (flag5 ? "<span style='color:#666;'>[兼任]</span>" : "")));
                            obj6.Add("ico", "fa-user");
                            obj6.Add("link", "");
                            obj6.Add("userID", (JToken)userModel.Id);
                            obj6.Add("type", flag5 ? 6 : 4);
                            obj6.Add("hasChilds", 0);
                            JObject obj4 = obj6;
                            array.Add(obj4);
                        }
                    }
                }
            }
            return array.ToString(0, Array.Empty<JsonConverter>());
        }
    }





}
