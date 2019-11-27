using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{



    [Area("RoadFlowCore")]
    public class VoteController : Controller
    {
        // Methods
        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            string[] strArray = base.Request.Forms("ids").Split(',', (StringSplitOptions)StringSplitOptions.None);
            Vote vote = new Vote();
            Guid userId = Current.UserId;
            int num = 0;
            foreach (string str in strArray)
            {
                Guid guid2;
                if (StringExtensions.IsGuid(str, out guid2))
                {
                   RoadFlow.Model. Vote vote2 = vote.GetVote(guid2);
                    if ((vote2 != null) && (vote2.CreateUserId == userId))
                    {
                        vote.DeleteVote(guid2, true);
                        num++;
                    }
                }
            }
            return ("共删除了" + ((int)num).ToString() + "项问卷数据!");
        }

        [Validate]
        public string DeleteItem()
        {
            Guid guid;
            Guid guid2;
            string str = base.Request.Querys("itemid");
            string str2 = base.Request.Querys("id");
            if (!StringExtensions.IsGuid(str, out guid) || !StringExtensions.IsGuid(str2, out guid2))
            {
                return "id错误!";
            }
            Vote vote = new Vote();
            RoadFlow.Model.Vote vote2 = vote.GetVote(guid2);
            if ((vote2 == null) || (vote2.Status != 0))
            {
                return "该问卷已发布,不能删除选题!";
            }
            if (vote.DeleteVoteItem(guid) <= 0)
            {
                return "删除失败!";
            }
            return "删除成功!";
        }

        [Validate]
        public IActionResult Edit()
        {
            Guid guid;
            string str = base.Request.Querys("id");
            RoadFlow.Model.Vote vote = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                vote = new Vote().GetVote(guid);
            }
            if (vote == null)
            {
                RoadFlow.Model.Vote vote1 = new RoadFlow.Model.Vote();
                vote1.Id=Guid.NewGuid();
                vote1.CreateTime=Current.DateTime;
                vote1.CreateUserId=Current.UserId;
                vote1.EndTime= DateTimeExtensions.MaxValue;
                vote1.Anonymous = 0;
                vote1.Status = 0;
                vote = vote1;
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["pageSize"]= base.Request.Querys("pagesize");
            base.ViewData["pageNumber"]= base.Request.Querys("pagenumber");
            return this.View(vote);
        }

        [Validate]
        public IActionResult EditItem()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("id"), out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="ID错误";
                return result1;
            }
            Vote vote = new Vote();
            RoadFlow.Model.Vote vote2 = vote.GetVote(guid);
            List<RoadFlow.Model.VoteItem> voteItems = vote.GetVoteItems(guid);
            List<RoadFlow.Model.VoteItemOption> voteItemOptions = vote.GetVoteItemOptions(guid);
            base.ViewData["voteItemOptions"]= voteItemOptions;
            base.ViewData["vote"]= vote2;
            string[] textArray1 = new string[] { "pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber"), "&id=", base.Request.Querys("id"), "&appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid"), "&iframeid=", base.Request.Querys("iframeid"), "&openerid=", base.Request.Querys("openerid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View(voteItems);
        }

        [Validate]
        public IActionResult EditOption()
        {
            Guid guid;
            string str = base.Request.Querys("id");
            string str2 = base.Request.Querys("itemid");
            RoadFlow.Model.VoteItem voteItem = null;
            List<RoadFlow.Model.VoteItemOption> itemOptions = new List<RoadFlow.Model.VoteItemOption>();
            Vote vote = new Vote();
            if (StringExtensions.IsGuid(str2, out guid))
            {
                voteItem = vote.GetVoteItem(guid);
                itemOptions = vote.GetItemOptions(guid);
            }
            if (voteItem == null)
            {
                RoadFlow.Model.VoteItem item1 = new RoadFlow.Model.VoteItem();
                item1.Id=Guid.NewGuid();
                item1.VoteId=StringExtensions.ToGuid(str);
                item1.SelectModel = 1;
                item1.Sort = vote.GetVoteItemMaxSort(StringExtensions.ToGuid(str));
                voteItem = item1;
            }
            base.ViewData["voteItemOptions"]= itemOptions;
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View(voteItem);
        }

        [Validate]
        public IActionResult Index()
        {
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View();
        }

        [Validate]
        public string Publish()
        {
            Guid guid;
            string str = base.Request.Forms("id");
            string str2 = base.Request.Forms("status");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return "id错误!";
            }
            string str3 = str2.Equals("1") ? new Vote().Publish(guid) : new Vote().UnPublish(guid);
            if (!str3.Equals("1"))
            {
                return str3;
            }
            return "操作成功!";
        }

        [Validate]
        public string Query()
        {
            int num3;
            string str = base.Request.Forms("Topic");
            string str2 = base.Request.Forms("Date1");
            string str3 = base.Request.Forms("Date2");
            string str4 = base.Request.Forms("sidx");
            string str5 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str6 = (str4.IsNullOrEmpty() ? "CreateTime" : str4) + " " + (str5.IsNullOrEmpty() ? "DESC" : str5);
            Vote vote = new Vote();
            Organize organize = new Organize();
            DataTable table = vote.GetVotePagerList(out num3, pageSize, pageNumber, Current.UserId, str, str2, str3, str6);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                DateTime time=new DateTime();

                string str7 = row["Status"].ToString();
                string str8 = string.Empty;
                string str9 = string.Empty;
                if (str7.Equals("0"))
                {
                    str9 = "未发布";
                    str8 = "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"publish('" + row["Id"].ToString() + "',1,this);return false;\"><i class=\"fa fa-check-square-o\"></i>发布</a>";
                }
                else if (str7.Equals("1"))
                {
                    str9 = "已发布";
                    str8 = "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"publish('" + row["Id"].ToString() + "',0,this);return false;\"><i class=\"fa fa-close\"></i>取消发布</a>";
                }
                else if (str7.Equals("2"))
                {
                    str9 = "已提交";
                }
                else if (str7.Equals("3"))
                {
                    str9 = "全部已提交";
                }
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)row["Id"].ToString());
                obj3.Add("Topic", (JToken)row["Topic"].ToString());
                obj3.Add("CreateTime", (JToken)DateTimeExtensions.ToShortDateTimeString(StringExtensions.ToDateTime(row["CreateTime"].ToString())));
                obj3.Add("PartakeUsers", (JToken)organize.GetNames(row["PartakeUsers"].ToString()));
                obj3.Add("EndTime", StringExtensions.IsDateTime(row["EndTime"].ToString(), out time) ? ((time.Year == DateTimeExtensions.MaxValue.Year) ? ((JToken)"无") : ((JToken)DateTimeExtensions.ToShortDateTimeString(time))) : ((JToken)"无"));

                obj3.Add("Status", (JToken)str9);
                string[] textArray1 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('", row["Id"].ToString(), "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a><a class=\"list\" href=\"javascript:void(0);\" onclick=\"editOption('", row["Id"].ToString(), "');return false;\"><i class=\"fa fa-square-o\"></i>选题</a>", str8 };
                obj3.Add("Opation", (JToken)string.Concat((string[])textArray1));
                JObject obj2 = obj3;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate, ValidateAntiForgeryToken]
        public string QueryResultList()
        {
            int num3;
            string str = base.Request.Forms("Topic");
            string str2 = base.Request.Forms("Date1");
            string str3 = base.Request.Forms("Date2");
            string str4 = base.Request.Forms("sidx");
            string str5 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str6 = (str4.IsNullOrEmpty() ? "CreateTime" : str4) + " " + (str5.IsNullOrEmpty() ? "DESC" : str5);
            Vote vote = new Vote();
            User user = new User();
            Organize organize = new Organize();
            DataTable table = vote.GetResultPagerList(out num3, pageSize, pageNumber, Current.UserId, str, str2, str3, str6);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                DateTime time=new DateTime();

                string str7 = row["Status"].ToString();
                string str8 = string.Empty;
                if (str7.Equals("1"))
                {
                    str8 = "待提交";
                }
                else if (str7.Equals("2"))
                {
                    str8 = "已有人提交";
                }
                else if (str7.Equals("3"))
                {
                    str8 = "全部提交";
                }
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Topic", (JToken)row["Topic"].ToString());
                obj1.Add("CreateUserId", (JToken)user.GetName(StringExtensions.ToGuid(row["CreateUserId"].ToString())));
                obj1.Add("CreateTime", (JToken)DateTimeExtensions.ToShortDateTimeString(StringExtensions.ToDateTime(row["CreateTime"].ToString())));
                obj1.Add("EndTime", StringExtensions.IsDateTime(row["EndTime"].ToString(), out time) ? ((time.Year == DateTimeExtensions.MaxValue.Year) ? ((JToken)"无") : ((JToken)DateTimeExtensions.ToShortDateTimeString(time))) : ((JToken)"无"));


                obj1.Add("Status", (JToken)str8);
                obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"view('" + row["Id"].ToString() + "');return false;\"><i class=\"fa fa-sticky-note-o\"></i>查看</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate]
        public string QueryResultUser()
        {
            int num3;
            string str = base.Request.Forms("userNmae");
            string str2 = base.Request.Forms("userOrganize");
            string str3 = base.Request.Forms("sidx");
            string str4 = base.Request.Forms("sord");
            string str5 = base.Request.Querys("id");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str6 = (str3.IsNullOrEmpty() ? "CreateTime" : str3) + " " + (str4.IsNullOrEmpty() ? "DESC" : str4);
            DataTable table = new Vote().GetPartakeUserPagerList(out num3, pageSize, pageNumber, StringExtensions.ToGuid(str5), str, str2, str6);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                DateTime time=new DateTime();
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)row["Id"].ToString());
                obj3.Add("UserName", (JToken)row["UserName"].ToString());
                obj3.Add("UserOrganize", (JToken)row["UserOrganize"].ToString());
                obj3.Add("Status", row["Status"].ToString().Equals("1") ? "已提交" : "<span style='color:#666;'>未提交</span>");
                obj3.Add("SubmitTime", StringExtensions.IsDateTime(row["SubmitTime"].ToString(), out time) ? ((JToken)DateTimeExtensions.ToDateTimeString(time)) : ((JToken)""));
                obj3.Add("Opation", row["Status"].ToString().Equals("1") ? ((JToken)string.Concat((string[])new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"view('", row["VoteId"].ToString(), "', '", row["UserId"].ToString(), "', '", row["UserName"].ToString().Replace("'", ""), "');return false;\"><i class=\"fa fa-search\"></i>查看</a>" })) : ((JToken)""));
                JObject obj2 = obj3;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate(CheckApp = false, CheckUrl = false)]
        public string QueryWaitSubmit()
        {
            int num3;
            string str = base.Request.Forms("Topic");
            string str2 = base.Request.Forms("Date1");
            string str3 = base.Request.Forms("Date2");
            string str4 = base.Request.Forms("sidx");
            string str5 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str6 = (str4.IsNullOrEmpty() ? "CreateTime" : str4) + " " + (str5.IsNullOrEmpty() ? "ASC" : str5);
            Vote vote = new Vote();
            User user = new User();
            Organize organize = new Organize();
            DataTable table = vote.GetWaitSubmitPagerList(out num3, pageSize, pageNumber, Current.UserId, str, str2, str3, str6);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                DateTime time=new DateTime();

                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Topic", (JToken)row["Topic"].ToString());
                obj1.Add("CreateUserId", (JToken)user.GetName(StringExtensions.ToGuid(row["CreateUserId"].ToString())));
                obj1.Add("CreateTime", (JToken)DateTimeExtensions.ToShortDateTimeString(StringExtensions.ToDateTime(row["CreateTime"].ToString())));
                obj1.Add("EndTime", StringExtensions.IsDateTime(row["EndTime"].ToString(), out time) ? ((time.Year == DateTimeExtensions.MaxValue.Year) ? ((JToken)"无") : ((JToken)DateTimeExtensions.ToShortDateTimeString(time))) : ((JToken)"无"));

                obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"submit1('" + row["Id"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>提交</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate]
        public IActionResult ResultList()
        {
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View();
        }

        [Validate]
        public IActionResult ResultView()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("id"), out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="ID错误!";
                return result1;
            }
            Vote vote = new Vote();
            if (!vote.GetVoteResultUsers(guid).Exists(
                key=> (key.UserId == Current.UserId)))
            {
                ContentResult result2 = new ContentResult();
                result2.Content="您无权查看该项问卷结果!";
                return result2;
            }
            RoadFlow.Model.Vote vote2 = vote.GetVote(guid);
            List<RoadFlow.Model.VoteItem> voteItems = vote.GetVoteItems(guid);
            List<RoadFlow.Model.VoteItemOption> voteItemOptions = vote.GetVoteItemOptions(guid);
            List<RoadFlow.Model.VoteResult> voteResults = vote.GetVoteResults(guid);
            base.ViewData["vote"]= vote2;


            base.ViewData["EndTime"] = vote2.EndTime?.Year == DateTimeExtensions.MaxValue.Year ? "无" : DateTimeExtensions.ToShortDateTimeString((DateTime)vote2.EndTime);
            base.ViewData["Note"] = vote2.Note.IsNullOrEmpty() ? 0 : 1;
            base.ViewData["voteItemOptions"]= voteItemOptions;
            base.ViewData["voteResults"]= voteResults;
            string[] textArray1 = new string[] { "pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber"), "&id=", base.Request.Querys("id"), "&appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"]=string.Concat((string[])textArray1);
            return this.View(voteItems);
        }

        [Validate]
        public IActionResult ResultViewUser()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("id"), out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="ID错误!";
                return result1;
            }
            Vote vote = new Vote();
            if (!vote.GetVoteResultUsers(guid).Exists(
                key=> (key.UserId == Current.UserId)))
            {
                ContentResult result2 = new ContentResult();
                result2.Content="您无权查看该项问卷结果!";
                return result2;
            }
            RoadFlow.Model.Vote vote2 = vote.GetVote(guid);
            if (vote2.Anonymous == 1)
            {
                ContentResult result3 = new ContentResult();
                result3.Content="该问卷是匿名提交,不能按人员查看!";
                return result3;
            }




            List<RoadFlow.Model.VotePartakeUser> partakeUsers = vote.GetPartakeUsers(guid, -1);
            base.ViewData["vote"]= vote2;

            base.ViewData["EndTime"] = vote2.EndTime?.Year == DateTimeExtensions.MaxValue.Year ? "无" : DateTimeExtensions.ToShortDateTimeString((DateTime)vote2.EndTime);
            base.ViewData["Note"] = vote2.Note.IsNullOrEmpty() ? 0 : 1;

            string[] textArray1 = new string[] { "pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber"), "&id=", base.Request.Querys("id"), "&appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View(partakeUsers);
        }

        [Validate]
        public IActionResult ResultViewUserShow()
        {
            Guid guid;
            Guid userGuid;
            string str = base.Request.Querys("voteid");
            string str2 = base.Request.Querys("userid");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="问卷ID错误!";
                return result1;
            }
            if (!StringExtensions.IsGuid(str2, out userGuid))
            {
                ContentResult result2 = new ContentResult();
                result2.Content="用户ID错误!";
                return result2;
            }
            Vote vote = new Vote();
            if (!vote.GetVoteResultUsers(guid).Exists(
               key=> (key.UserId == Current.UserId)))
            {
                ContentResult result3 = new ContentResult();
                result3.Content="您无权查看该项问卷结果!";
                return result3;
            }
            RoadFlow.Model.Vote vote2 = vote.GetVote(guid);
            if (vote2.Anonymous == 1)
            {
                ContentResult result4 = new ContentResult();
                result4.Content="该问卷是匿名提交,不能按人员查看!";
                return result4;
            }

            List<RoadFlow.Model.VoteItem> voteItems = vote.GetVoteItems(guid);
            List<RoadFlow.Model.VoteItemOption> voteItemOptions = vote.GetVoteItemOptions(guid);
            List<RoadFlow.Model.VoteResult> list3 = vote.GetVoteResults(guid).FindAll(delegate (RoadFlow.Model.VoteResult p) {
                return p.UserId == userGuid;
            });
            base.ViewData["vote"]= vote2;

            base.ViewData["EndTime"] = vote2.EndTime?.Year == DateTimeExtensions.MaxValue.Year ? "无" : DateTimeExtensions.ToShortDateTimeString((DateTime)vote2.EndTime);
            base.ViewData["Note"] = vote2.Note.IsNullOrEmpty() ? 0 : 1;
            base.ViewData["voteItemOptions"]=voteItemOptions;
            base.ViewData["voteResults"]= list3;
            return this.View(voteItems);
        }

        [Validate]
        public string Save(RoadFlow.Model.Vote voteModel)
        {
            Guid guid;
            //if (!base.ModelState.IsValid)
            //{
            //    return Tools.GetValidateErrorMessag(base.ModelState);
            //}
            if (voteModel.EndTime.ToString().IsNullOrEmpty())
            {
                voteModel.EndTime= DateTimeExtensions.MaxValue;
            }
            Vote vote = new Vote();
            if (StringExtensions.IsGuid(base.Request.Querys("id"), out guid))
            {
                RoadFlow.Model.Vote vote2 = vote.GetVote(guid);
                if ((vote2 != null) && (vote2.Status != 0))
                {
                    return "该问卷已发布,不能修改!";
                }
                string oldContents = (vote2 == null) ? "" : vote2.ToString();
                vote.UpdateVote(voteModel);
                Log.Add("修改了问卷调查-" + voteModel.Topic, "", LogType.系统管理, oldContents, voteModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                vote.AddVote(voteModel);
                Log.Add("添加了问卷调查-" + voteModel.Topic, voteModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate]
        public string SaveOption(RoadFlow.Model.VoteItem voteItemModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            Vote vote = new Vote();
            RoadFlow.Model.Vote vote2 = vote.GetVote(voteItemModel.VoteId);
            if ((vote2 != null) && (vote2.Status != 0))
            {
                return "该问卷已发布,不能修改!";
            }
            if (StringExtensions.IsGuid(base.Request.Querys("itemid"), out guid))
            {
                vote.UpdateVoteItem(voteItemModel);
            }
            else
            {
                vote.AddVoteItem(voteItemModel);
            }
            string[] strArray = base.Request.Forms("trindex").Split(',', (StringSplitOptions)StringSplitOptions.None);
            int num = 0;
            foreach (string str in strArray)
            {
                string str2 = base.Request.Forms("title_" + str);
                int num3 = base.Request.Forms(("isinput_" + str)).ToInt(0);
                string str3 = base.Request.Forms("style_" + str);
                if (!str2.IsNullOrWhiteSpace())
                {
                    Guid guid2;
                    if (StringExtensions.IsGuid(str, out guid2))
                    {
                        RoadFlow.Model.VoteItemOption voteItemOption = vote.GetVoteItemOption(guid2);
                        if (voteItemOption != null)
                        {
                            voteItemOption.IsInput = num3;
                            voteItemOption.OptionTitle = str2.Trim();
                            voteItemOption.InputStyle = str3;
                            voteItemOption.Sort = num;
                            vote.UpdateVoteItemOption(voteItemOption);
                        }
                    }
                    else
                    {
                        RoadFlow.Model.VoteItemOption option1 = new RoadFlow.Model.VoteItemOption();
                        option1.Id=Guid.NewGuid();
                        option1.ItemId=voteItemModel.Id;
                        option1.VoteId=voteItemModel.VoteId;
                        option1.IsInput = num3;
                        option1.OptionTitle = str2.Trim();
                        option1.InputStyle = str3;
                        option1.Sort = num;
                        RoadFlow.Model.VoteItemOption option2 = option1;
                        vote.AddVoteItemOption(option2);
                    }
                    num++;
                }
            }
            return "保存成功!";
        }

        public IActionResult Submit()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("id"), out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="ID错误";
                return result1;
            }
            Vote vote = new Vote();
            RoadFlow.Model.Vote vote2 = vote.GetVote(guid);

            if (vote2 == null)
            {
                ContentResult result2 = new ContentResult();
                result2.Content="未找到该问卷!";
                return result2;
            }
            if (vote2.EndTime < DateTimeExtensions.Now)
            {
                ContentResult result3 = new ContentResult();
                result3.Content="该问卷已超过结束时间,不能提交!";
                return result3;
            }



            List<RoadFlow.Model.VoteItem> voteItems = vote.GetVoteItems(guid);
            List<RoadFlow.Model.VoteItemOption> voteItemOptions = vote.GetVoteItemOptions(guid);
            base.ViewData["vote"]= vote2;

          
            base.ViewData["EndTime"] = vote2.EndTime?.Year == DateTimeExtensions.MaxValue.Year ? "无" : DateTimeExtensions.ToShortDateTimeString((DateTime)vote2.EndTime);
            base.ViewData["Note"] = vote2.Note.IsNullOrEmpty() ? 0 : 1;
            base.ViewData["voteItemOptions"]= voteItemOptions;
            string[] textArray1 = new string[] { "pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber"), "&id=", base.Request.Querys("id"), "&appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            base.ViewData["isfromlist"]= base.Request.Querys("isfromlist");
            return this.View(voteItems);
        }

        [Validate(CheckApp = false, CheckUrl = false), ValidateAntiForgeryToken]
        public string SubmitSave()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Forms("VoteId"), out guid))
            {
                return "问卷ID错误!";
            }
           
            Vote vote = new Vote();
            RoadFlow.Model.Vote vote2 = vote.GetVote(guid);
            if (vote2 == null)
            {
                return "未找到该问卷!";
            }
            if (vote2.EndTime < DateTimeExtensions.Now)
            {
                return "该问卷已超过结束时间,不能提交!";
            }


            Guid userId = Current.UserId;
            List<RoadFlow.Model.VoteResult> list = new List<RoadFlow.Model.VoteResult>();
            foreach (string str2 in base.Request.Forms("VoteItemId").Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                foreach (string str3 in base.Request.Forms(("item_" + str2)).Split(',', (StringSplitOptions)StringSplitOptions.None))
                {
                    string str = base.Request.Forms("text_" + str3);
                    RoadFlow.Model.VoteResult result1 = new RoadFlow.Model.VoteResult();
                    result1.Id=Guid.NewGuid();
                    result1.ItemId=StringExtensions.ToGuid(str2);
                    result1.VoteId=guid;
                    result1.OptionId=StringExtensions.ToGuid(str3);
                    result1.UserId=userId;
                    RoadFlow.Model.VoteResult result = result1;
                    if (!str.IsNullOrWhiteSpace())
                    {
                        result.OptionOther = str;
                    }
                    list.Add(result);
                }
            }
            vote.AddVoteResults(list);
            return "提交成功!";
        }

        [Validate]
        public IActionResult WaitSubmit()
        {
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View();
        }

       
}



 



}
