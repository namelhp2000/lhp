using System;
using System.Collections.Generic;
using System.Linq;
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
    public class FlowCommentController : Controller
    {
        // Methods
        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            string str = base.Request.Forms("ids");
            List<RoadFlow.Model.FlowComment> list = new List<RoadFlow.Model.FlowComment>();
            FlowComment comment = new FlowComment();
            List<RoadFlow.Model.FlowComment> all = comment.GetAll();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid fid;
                if (StringExtensions.IsGuid(str2, out fid))
                {
                    RoadFlow.Model.FlowComment comment2 = all.Find(delegate (RoadFlow.Model.FlowComment p) {
                        return p.Id == fid;
                    });
                    if (comment2 != null)
                    {
                        list.Add(comment2);
                    }
                }
            }
            comment.Delete(list.ToArray());
            Log.Add("删除了流程意见", JsonConvert.SerializeObject(list), LogType.流程管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate]
        public IActionResult Edit()
        {
            Guid guid;
            FlowComment comment = new FlowComment();
            RoadFlow.Model.FlowComment comment2 = null;
            string str = base.Request.Querys("commentid");
            string str2 = base.Request.Querys("isoneself");
            if (StringExtensions.IsGuid(str, out guid))
            {
                comment2 = comment.Get(guid);
            }
            if (comment2 == null)
            {
                RoadFlow.Model.FlowComment comment1 = new RoadFlow.Model.FlowComment();
                comment1.Id=Guid.NewGuid();
                comment1.Sort = comment.GetMaxSort();
                comment1.AddType = "1".Equals(str2) ? 0 : 1;
                comment2 = comment1;
                if ("1".Equals(str2))
                {
                    comment2.UserId=Current.UserId;
                }
            }
            base.ViewData["isOneSelf"]= str2;
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View(comment2);
        }

        [Validate]
        public IActionResult Index()
        {
            bool flag = "1".Equals(base.Request.Querys("isoneself"));
            Guid userId = Current.UserId;
            List<RoadFlow.Model.FlowComment> all = new FlowComment().GetAll();
            JArray array = new JArray();
            User user = new User();
            foreach (RoadFlow.Model.FlowComment comment in all)
            {
                if (!flag || (comment.UserId == userId))
                {
                    JObject obj1 = new JObject();
                    obj1.Add("id", (JToken)comment.Id);
                    obj1.Add("Comments", (JToken)comment.Comments);
                    obj1.Add("UserId", !GuidExtensions.IsEmptyGuid(comment.UserId) ? ((JToken)user.GetName(comment.UserId)) : ((JToken)"全部人员"));
                    obj1.Add("AddType", (comment.AddType == 0) ? "用户添加" : "管理员添加");
                    obj1.Add("Sort", (JToken)comment.Sort);
                    obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + comment.Id + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                    JObject obj2 = obj1;
                    array.Add(obj2);
                }
            }
            base.ViewData["json"]= array.ToString();
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["isoneself"]= base.Request.Querys("isoneself");
            return this.View();
        }

        [Validate, ValidateAntiForgeryToken]
        public string Save(RoadFlow.Model.FlowComment flowCommentModel)
        {
            Guid guid;
            if (!base.Request.Forms("UserId").IsNullOrWhiteSpace())
            {
                flowCommentModel.UserId=new User().GetUserId(base.Request.Forms("UserId"));
            }
            FlowComment comment = new FlowComment();
            if (StringExtensions.IsGuid(base.Request.Querys("commentid"), out guid))
            {
                RoadFlow.Model.FlowComment comment2 = comment.Get(guid);
                string oldContents = (comment2 == null) ? "" : comment2.ToString();
                comment.Update(flowCommentModel);
                Log.Add("修改了流程意见-" + flowCommentModel.Id, "", LogType.系统管理, oldContents, flowCommentModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                comment.Add(flowCommentModel);
                Log.Add("添加了流程意见-" + flowCommentModel.Id, flowCommentModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }
    }






}
