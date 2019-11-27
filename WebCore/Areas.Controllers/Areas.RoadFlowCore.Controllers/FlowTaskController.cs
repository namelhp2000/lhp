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
using RoadFlow.Model.FlowRunModel;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{
 

    #region  新的方法

    [Area("RoadFlowCore")]
    public class FlowTaskController : Controller
    {
        // Methods
        [Validate]
        public IActionResult ChangeStatus()
        {
            string str = base.Request.Querys("taskid");
            RoadFlow.Model.FlowTask task2 = new RoadFlow.Business.FlowTask().Get(StringExtensions.ToGuid(str));
            if (task2 == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content="未找到当前任务!";
                return result1;
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["statusOptions"]= new RoadFlow.Business.FlowTask().GetExecuteTypeOptions(task2.ExecuteType);
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Completed()
        {
            base.ViewData["flowOptions"]= new RoadFlow.Business.Flow().GetOptions("");
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");

            return this.View();
        }

        [Validate]
        public string DeleteInstance()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("groupid"), out guid))
            {
                return "组ID错误!";
            }
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            List<RoadFlow.Model.FlowTask> listByGroupId = task.GetListByGroupId(guid);
            if (listByGroupId.Count > 0)
            {
                task.DeleteByGroupId(listByGroupId.ToArray());
                RoadFlow.Business.Log.Add("删除了流程实例-" + Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId).Title, JsonConvert.SerializeObject(listByGroupId), LogType.流程运行, "", "", "", "", "", "", "", "");
            }
            return "删除成功!";
        }

        public string DeleteTask()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("deltaskid"), out guid))
            {
                return "Id错误!";
            }
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            RoadFlow.Model.FlowTask taskModel = task.Get(guid);
            if (taskModel != null)
            {
                Guid guid2;
                task.DeleteByGroupId(taskModel.GroupId);
                RoadFlow.Model.FlowRun flowRunModel = new RoadFlow.Business.Flow().GetFlowRunModel(taskModel.FlowId, true);
                if (flowRunModel == null)
                {
                    return "作废成功!";
                }
                Step step = flowRunModel.Steps.Find(delegate (Step p) {
                    return p.Id == taskModel.StepId;
                });
                if (step == null)
                {
                    return "作废成功!";
                }
                RoadFlow.Model.AppLibrary library = new RoadFlow.Business.AppLibrary().Get(step.StepForm.Id);
                if ((library != null) && StringExtensions.IsGuid(library.Code, out guid2))
                {
                    new RoadFlow.Business.Form().DeleteFormData(guid2, taskModel.InstanceId);
                }
            }
            return "作废成功!";
        }

        [Validate]
        public IActionResult Designate()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Detail()
        {
            string str = base.Request.Querys("flowid");
            string str2 = base.Request.Querys("groupid");
            string str3 = base.Request.Querys("taskie");
            string str4 = base.Request.Querys("appid");
            string str5 = base.Request.Querys("tabid");
            string str6 = base.Request.Querys("displaymodel");
            string str7 = base.Request.Querys("ismobile");
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            List<RoadFlow.Model.FlowTask> listByGroupId = task.GetListByGroupId(StringExtensions.ToGuid(str2));
            JArray array = new JArray();
            JArray array2 = new JArray();
            foreach (RoadFlow.Model.FlowTask task2 in listByGroupId)
            {
                JObject obj1 = new JObject();
                obj1.Add("StepName", (JToken)task2.StepName);
                obj1.Add("SenderName", (JToken)task2.SenderName);
                obj1.Add("SenderTime", (JToken)DateTimeExtensions.ToDateTimeString(task2.ReceiveTime));
                obj1.Add("ReceiveName", (JToken)task2.ReceiveName);
                obj1.Add("CompletedTime1", (JToken)DateTimeExtensions.ToDateTimeString(task2.CompletedTime1));
                obj1.Add("StatusTitle", (JToken)task.GetExecuteTypeTitle(task2.ExecuteType));
                obj1.Add("Comment", (JToken)task2.Comments);
                obj1.Add("Note", (JToken)task2.Note);
                JObject obj2 = obj1;
                array.Add(obj2);


                JObject obj4 = new JObject();
                obj4.Add("stepid", (JToken)task2.StepId);
                obj4.Add("prevstepid", (JToken)task2.PrevStepId);
                obj4.Add("status", (JToken)task2.Status);
                obj4.Add("sender", (JToken)task2.SenderName);
                obj4.Add("sendtime", (JToken)DateTimeExtensions.ToDateTimeString(task2.ReceiveTime));
                obj4.Add("receiver", (JToken)task2.ReceiveName);
                obj4.Add("completedtime1", (JToken)DateTimeExtensions.ToDateTimeString(task2.CompletedTime1));
                obj4.Add("comment", (JToken)task2.Comments);
                obj4.Add("sort", (JToken)task2.Sort);
                obj4.Add("tasktype", (JToken)task2.TaskType);
                obj4.Add("statustitle", (JToken)task.GetExecuteTypeTitle(task2.ExecuteType));
                JObject obj3 = obj4;
                array2.Add(obj3);

            }
            base.ViewData["json"]= array.ToString();
            base.ViewData["json1"]=array2.ToString();
            base.ViewData["displaymodel"]= str6.IsNullOrWhiteSpace() ? "0" : str6;
            base.ViewData["tabid"]= str5;
            string[] textArray1 = new string[] { "flowid=", str, "&stepid=", base.Request.Querys("stepid"), "&groupid=", str2, "&taskid=", str3, "&appid=", str4, "&tabid=", str5, "&iframeid=", base.Request.Querys("iframeid"), "&ismobile=", str7 };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            base.ViewData["flowid"] = str;
            base.ViewData["ismobile"]= str7;






            IOrderedEnumerable<RoadFlow.Model.FlowTask> enumerable = Enumerable.OrderByDescending<RoadFlow.Model.FlowTask, int>(Enumerable.Where<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId, 
                key=> GuidExtensions.IsNotEmptyGuid(key.BeforeStepId)), 
               key=>key.Sort);
            if (Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable))
            {
                base.ViewData["stepid"] = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable).BeforeStepId.ToString();
                base.ViewData["groupid"] = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable).GroupId.ToString();
            }
            else
            {
                base.ViewData["stepid"] = string.Empty;
                base.ViewData["groupid"] = string.Empty;
            }




            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult DetailSubFlow()
        {
            Guid guid;
            string str = base.Request.Querys("taskid");
            string str2 = base.Request.Querys("ismobile");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="任务ID错误!";
                return result1;
            }
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            RoadFlow.Model.FlowTask task2 = task.Get(guid);
            if (task2 == null)
            {
                ContentResult result2 = new ContentResult();
                result2.Content="未找到当前任务!";
                return result2;
            }
            if (task2.SubFlowGroupId.IsNullOrWhiteSpace())
            {
                ContentResult result3 = new ContentResult();
                result3.Content="未找到当前任务的子流程任务!";
                return result3;
            }
            string[] strArray = task2.SubFlowGroupId.Split(',', (StringSplitOptions)StringSplitOptions.None);
            if (strArray.Length == 1)
            {
                List<RoadFlow.Model.FlowTask> listByGroupId = task.GetListByGroupId(StringExtensions.ToGuid(strArray[0]));
                if (listByGroupId.Count > 0)
                {
                    object[] objArray1 = new object[] { "Detail?flowid=", Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId).FlowId, "&stepid=", Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId).StepId, "&groupid=", strArray[0], "&taskid=", Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId).Id, "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid"), "&iframeid=", base.Request.Querys("iframeid"), "&ismobile=", str2 };
                    return this.Redirect(string.Concat((object[])objArray1));
                }
            }
            else
            {
                JArray array = new JArray();
                foreach (string str3 in strArray)
                {
                    List<RoadFlow.Model.FlowTask> list2 = task.GetListByGroupId(StringExtensions.ToGuid(str3));
                    if (list2.Count != 0)
                    {
                        RoadFlow.Model.FlowTask task3 = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list2);
                        JObject obj3 = new JObject();
                        obj3.Add("FlowName", (JToken)task3.FlowName);
                        obj3.Add("StepName", (JToken)task3.StepName);
                        obj3.Add("SenderName", (JToken)task3.SenderName);
                        obj3.Add("SenderTime", (JToken)DateTimeExtensions.ToDateTimeString(task3.ReceiveTime));
                        obj3.Add("ReceiveName", (JToken)task3.ReceiveName);
                        obj3.Add("CompletedTime1", task3.CompletedTime1.HasValue ? ((JToken)task3.CompletedTime1.Value.ToShortDateString()) : ((JToken)""));
                        obj3.Add("StatusTitle", (JToken)task.GetExecuteTypeTitle(task3.ExecuteType));
                        object[] objArray2 = new object[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"detail('", task3.FlowId, "', '", task3.GroupId, "');return false;\"><i class=\"fa fa-search\"></i>查看</a>" };
                        obj3.Add("Show", (JToken)string.Concat((object[])objArray2));
                        JObject obj2 = obj3;
                        array.Add(obj2);
                    }
                }
                base.ViewData["json"]= array.ToString();
                base.ViewData["ismobile"]= str2;
            }
            return this.View();
        }


        public string EntrustWithdrawTask()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("taskid"), out guid))
            {
                return "任务ID错误!";
            }
            FlowTask task = new FlowTask();
            RoadFlow.Model.FlowTask flowTaskModel = task.Get(guid);
            if (flowTaskModel == null)
            {
                return "未找到任务!";
            }
            if (!flowTaskModel.EntrustUserId.HasValue)
            {
                return "该任务不是委托任务!";
            }
            flowTaskModel.ReceiveId=flowTaskModel.EntrustUserId.Value;
            Guid? nullable = flowTaskModel.EntrustUserId;
            flowTaskModel.ReceiveName = new User().GetName(nullable.Value);
            flowTaskModel.EntrustUserId=(Guid?)null;
            flowTaskModel.Note = "";
            task.Update(flowTaskModel);
            return "收回成功，请到待办事项中查看!";
        }






        [Validate]
        public IActionResult GoTo()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("taskid"), out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="任务ID错误!";
                return result1;
            }
            RoadFlow.Model.FlowTask task = new RoadFlow.Business.FlowTask().Get(guid);
            if (task == null)
            {
                ContentResult result2 = new ContentResult();
                result2.Content="未找到当前任务!";
                return result2;
            }
            RoadFlow.Model.FlowRun flowRunModel = new RoadFlow.Business.Flow().GetFlowRunModel(task.FlowId, true,null);
            if (flowRunModel == null)
            {
                ContentResult result3 = new ContentResult();
                result3.Content="未找到当前流程运行时!";
                return result3;
            }
            base.ViewData["steps"]= flowRunModel.Steps;
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Hasten()
        {
            Guid guid;
            Guid taskId;
            string str = base.Request.Querys("hastentaskid");
            if (!StringExtensions.IsGuid(base.Request.Querys("hastengroupid"), out guid) || !StringExtensions.IsGuid(str, out taskId))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="任务Id错误!";
                return result1;
            }
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            List<RoadFlow.Model.FlowTask> list2 = task.GetListByGroupId(guid).FindAll(delegate (RoadFlow.Model.FlowTask p) {
                if (p.PrevId == taskId)
                {
                    int[] digits = new int[2];
                    digits[1] = 1;
                    return p.Status.In(digits);
                }
                return false;
            });
            List<Guid> list3 = new List<Guid>();

            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.FlowTask task2 in list2)
            {
                int[] numArray1 = new int[2];
                numArray1[1] = 1;
                if (task2.Status.In(numArray1) && !list3.Contains(task2.ReceiveId))
                {
                    list3.Add(task2.ReceiveId);
                    string[] textArray1 = new string[] { "<input checked=\"checked\" type=\"checkbox\" style=\"vertical-align:middle;\" value=\"u_", task2.ReceiveId.ToString(), "\" name=\"Users\" id=\"Users_", task2.ReceiveId.ToString("N"), "\" /><label style=\"vertical-align:middle;\" for=\"Users_", task2.ReceiveId.ToString("N"), "\">", task2.ReceiveName, "</label>" };
                    builder.Append(string.Concat((string[])textArray1));
                }
            }
            base.ViewData["Users"]=builder.ToString();
            base.ViewData["Contents"]= "您有一个待办事项“" + (Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list2) ? Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list2).Title : "") + "”，请尽快处理！";
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string HastenSave()
        {
            Guid guid;
            Guid taskId;
            string str = base.Request.Querys("hastentaskid");
            if (!StringExtensions.IsGuid(base.Request.Querys("hastengroupid"), out guid) || !StringExtensions.IsGuid(str, out taskId))
            {
                return "Id错误!";
            }
            string str3 = base.Request.Forms("Users");
            string str4 = base.Request.Forms("Model");
            string str5 = base.Request.Forms("Contents");
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            List<RoadFlow.Model.FlowTask> list2 = task.GetListByGroupId(guid).FindAll(delegate (RoadFlow.Model.FlowTask p) {
                if (p.PrevId == taskId)
                {
                    int[] digits = new int[2];
                    digits[1] = 1;
                    return p.Status.In(digits);
                }
                return false;
            });
            List<RoadFlow.Model.FlowTask> list3 = new List<RoadFlow.Model.FlowTask>();
            foreach (RoadFlow.Model.FlowTask task2 in list2)
            {
                int[] numArray1 = new int[2];
                numArray1[1] = 1;
                if (task2.Status.In(numArray1) && str3.ContainsIgnoreCase(task2.ReceiveId.ToString()))
                {
                    list3.Add(task2);
                }
            }
            task.SendMessage(list3, Current.User, str4, str5);
            return "催办成功!";
        }

        [Validate]
        public IActionResult Instance()
        {
            base.ViewData["appid"]= base.Request.Querys("appid");
            base.ViewData["query"]= "&appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            base.ViewData["flowOptions"]= new RoadFlow.Business.Flow().GetManageInstanceOptions(Current.UserId, "");
            return this.View();
        }

        [Validate]
        public IActionResult InstanceManage()
        {
            string str = base.Request.Querys("groupid");
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            List<RoadFlow.Model.FlowTask> listByGroupId = task.GetListByGroupId(StringExtensions.ToGuid(str));
            JArray array = new JArray();
            foreach (RoadFlow.Model.FlowTask task2 in listByGroupId)
            {
                JObject obj2 = new JObject();
                string str2 = "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"cngStatus('" + task2.Id.ToString() + "');\"><i class=\"fa fa-exclamation\"></i>状态</a>";
                int[] digits = new int[2];
                digits[1] = 1;
                if (task2.ExecuteType.In(digits) && (5 != task2.TaskType))
                {
                    string[] textArray1 = new string[] { str2, "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"designate('", task2.Id.ToString(), "');\"><i class=\"fa fa-hand-o-right\"></i>指派</a><a class=\"list\" href=\"javascript:void(0);\" onclick=\"goTo('", task2.Id.ToString(), "');\"><i class=\"fa fa-level-up\"></i>跳转</a>" };
                    str2 = string.Concat((string[])textArray1);
                }
                obj2.Add("id", (JToken)task2.Id);
                obj2.Add("StepID", (JToken)task2.StepName);
                obj2.Add("SenderName", (JToken)task2.SenderName);
                obj2.Add("ReceiveTime", (JToken)DateTimeExtensions.ToDateTimeString(task2.ReceiveTime));
                obj2.Add("ReceiveName", (JToken)task2.ReceiveName);
                obj2.Add("CompletedTime1", task2.CompletedTime1.HasValue ? ((JToken)DateTimeExtensions.ToDateTimeString(task2.CompletedTime1.Value)) : ((JToken)""));
                obj2.Add("Status", (JToken)task.GetExecuteTypeTitle(task2.ExecuteType));
                obj2.Add("Comment", (JToken)task2.Comments);
                obj2.Add("Note", (JToken)task2.Note);
                obj2.Add("Opation", (JToken)str2);
                array.Add(obj2);
            }
            base.ViewData["json"]= array.ToString();
            base.ViewData["appid"]= base.Request.Querys("appid");
            base.ViewData["iframeid"]= base.Request.Querys("iframeid");
            return this.View();
        }



        public IActionResult MyEntrust()
        {
            base.ViewData["flowOptions"]= new Flow().GetOptions("");
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            return this.View();
        }


        [Validate(CheckApp = false)]
        public IActionResult MyStarts()
        {
            base.ViewData["flowOptions"]= new Flow().GetOptions("");
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            return this.View();
        }



        [Validate(CheckApp = false)]
        public string QueryMyStarts()
        {
            int num3;
            string str = base.Request.Querys("appid");
            string str2 = base.Request.Querys("tabid");
            string flowId = base.Request.Forms("flowid");
            string title = base.Request.Forms("title");
            string startDate = base.Request.Forms("date1");
            string endDate = base.Request.Forms("date2");
            string status = base.Request.Forms("status");
            string str8 = base.Request.Forms("sidx");
            string str9 = base.Request.Forms("sord");
            string order = (str8.IsNullOrEmpty() ? "ReceiveTime" : str8) + " " + (str9.IsNullOrEmpty() ? "ASC" : str9);
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            DataTable table = new FlowTask().GetMyStartList(pageSize, pageNumber, Current.UserId, flowId, title, startDate, endDate, status, order, out num3);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                object[] objArray1 = new object[0x15];
                objArray1[0] = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('";
                objArray1[1] = base.Url.Content("~/RoadFlowCore/FlowRun/Index");
                objArray1[2] = "?";
                object[] objArray2 = new object[] { row["FlowId"], row["StepId"], row["InstanceId"], row["Id"], row["GroupId"], str };
                objArray1[3] = string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1", (object[])objArray2);
                objArray1[4] = "','";
                objArray1[5] = row["Title"].ToString().RemoveHTML().UrlEncode();
                objArray1[6] = "','";
                objArray1[7] = row["Id"];
                objArray1[8] = "',";
                objArray1[9] = (int)num4;
                objArray1[10] = ",";
                objArray1[11] = (int)num5;
                objArray1[12] = ",";
                objArray1[13] = (int)num6;
                objArray1[14] = ");return false;\"><i class=\"fa fa-file-text-o\"></i>表单</a><a class=\"list\" href=\"javascript:void(0);\" onclick=\"detail('";
                objArray1[15] = row["FlowId"];
                objArray1[0x10] = "','";
                objArray1[0x11] = row["GroupId"];
                objArray1[0x12] = "','";
                objArray1[0x13] = row["Id"];
                objArray1[20] = "');return false;\"><i class=\"fa fa-navicon\"></i>过程</a>";
                StringBuilder builder = new StringBuilder(string.Concat((object[])objArray1));
                object[] objArray3 = new object[0x11];
                objArray3[0] = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('";
                objArray3[1] = base.Url.Content("~/RoadFlowCore/FlowRun/Index");
                objArray3[2] = "?";
                object[] objArray4 = new object[] { row["FlowId"], row["StepId"], row["InstanceId"], row["Id"], row["GroupId"], str };
                objArray3[3] = string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1", (object[])objArray4);
                objArray3[4] = "','";
                objArray3[5] = row["Title"].ToString().RemoveHTML().UrlEncode();
                objArray3[6] = "','";
                objArray3[7] = row["Id"];
                objArray3[8] = "',";
                objArray3[9] = (int)num4;
                objArray3[10] = ",";
                objArray3[11] = (int)num5;
                objArray3[12] = ",";
                objArray3[13] = (int)num6;
                objArray3[14] = ");return false;\">";
                objArray3[15] = row["Title"];
                objArray3[0x10] = "</a>";
                string str11 = string.Concat((object[])objArray3);
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Title", (JToken)str11);
                obj1.Add("FlowName", (JToken)row["FlowName"].ToString());
                obj1.Add("ReceiveTime", (JToken)row["ReceiveTime"].ToString().ToDateTime().ToDateTimeString());
                obj1.Add("CurrentStepName", row["CurrentStepName"].ToString().IsNullOrWhiteSpace() ? ((JToken)"<span style=\"color:#09863e;\">已完成</span>") : ((JToken)row["CurrentStepName"].ToString()));
                obj1.Add("Opation", (JToken)builder.ToString());
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray5 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray5);

        }






        [Validate(CheckApp = false)]
        public string QueryCompleted()
        {
            int num3;
            string str = base.Request.Querys("appid");
            string str2 = base.Request.Querys("tabid");
            string str3 = base.Request.Forms("flowid");
            string str4 = base.Request.Forms("title");
            string str5 = base.Request.Forms("date1");
            string str6 = base.Request.Forms("date2");
            string str7 = base.Request.Forms("sidx");
            string str8 = base.Request.Forms("sord");
            string str9 = (str7.IsNullOrEmpty() ? "CompletedTime1" : str7) + " " + (str8.IsNullOrEmpty() ? "ASC" : str8);
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            DataTable table = task.GetCompletedTask(pageSize, pageNumber, Current.UserId, str3, str4, str5, str6, str9, out num3);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                bool flag2;
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                object[] objArray1 = new object[0x15];
                objArray1[0] = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('";
                objArray1[1] = base.Url.Content("~/RoadFlowCore/FlowRun/Index");
                objArray1[2] = "?";
                object[] objArray2 = new object[] { row["FlowId"], row["StepId"], row["InstanceId"], row["Id"], row["GroupId"], str };
                objArray1[3] = string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1", (object[])objArray2);
                objArray1[4] = "','";
                objArray1[5] = row["Title"].ToString().RemoveHTML().UrlEncode();
                objArray1[6] = "','";
                objArray1[7] = row["Id"];
                objArray1[8] = "',";
                objArray1[9] = (int)num4;
                objArray1[10] = ",";
                objArray1[11] = (int)num5;
                objArray1[12] = ",";
                objArray1[13] = (int)num6;
                objArray1[14] = ");return false;\"><i class=\"fa fa-file-text-o\"></i>表单</a><a class=\"list\" href=\"javascript:void(0);\" onclick=\"detail('";
                objArray1[15] = row["FlowId"];
                objArray1[0x10] = "','";
                objArray1[0x11] = row["GroupId"];
                objArray1[0x12] = "','";
                objArray1[0x13] = row["Id"];
                objArray1[20] = "');return false;\"><i class=\"fa fa-navicon\"></i>过程</a>";
                StringBuilder builder = new StringBuilder(string.Concat((object[])objArray1));
                bool flag = task.IsHasten(StringExtensions.ToGuid(row["Id"].ToString()), out flag2);
                if (flag2)
                {
                    object[] objArray3 = new object[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"withdraw('", row["Id"], "','", row["GroupId"], "');return false;\"><i class=\"fa fa-mail-reply\"></i>收回</a>" };
                    builder.Append(string.Concat((object[])objArray3));
                }
                if (flag)
                {
                    object[] objArray4 = new object[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"hasten('", row["Id"], "','", row["GroupId"], "');return false;\"><i class=\"fa fa-bullhorn\"></i>催办</a>" };
                    builder.Append(string.Concat((object[])objArray4));
                }
                object[] objArray5 = new object[0x11];
                objArray5[0] = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('";
                objArray5[1] = base.Url.Content("~/RoadFlowCore/FlowRun/Index");
                objArray5[2] = "?";
                object[] objArray6 = new object[] { row["FlowId"], row["StepId"], row["InstanceId"], row["Id"], row["GroupId"], str };
                objArray5[3] = string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1", (object[])objArray6);
                objArray5[4] = "','";
                objArray5[5] = row["Title"].ToString().RemoveHTML().UrlEncode();
                objArray5[6] = "','";
                objArray5[7] = row["Id"];
                objArray5[8] = "',";
                objArray5[9] = (int)num4;
                objArray5[10] = ",";
                objArray5[11] = (int)num5;
                objArray5[12] = ",";
                objArray5[13] = (int)num6;
                objArray5[14] = ");return false;\">";
                objArray5[15] = row["Title"];
                objArray5[0x10] = "</a>";
                string str10 = string.Concat((object[])objArray5);
                string str11 = row["Note"].ToString();

                //string executeTypeTitle = task.GetExecuteTypeTitle(row["ExecuteType"].ToString().ToInt(-2147483648));
                //string str12 = row["Note"].ToString();
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Title", (JToken)str10);
                obj1.Add("FlowName", (JToken)row["FlowName"].ToString());
                obj1.Add("StepName", (JToken)row["StepName"].ToString());
                obj1.Add("SenderName", (JToken)row["SenderName"].ToString());
                obj1.Add("ReceiveTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["ReceiveTime"].ToString())));
                obj1.Add("CompletedTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["CompletedTime"].ToString())));
                obj1.Add("Note", (JToken)str11);
                obj1.Add("Opation", (JToken)builder.ToString());
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray7 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray7);
        }

        [Validate]
        public string QueryInstance()
        {
            int num3;
            Func<RoadFlow.Model.FlowTask, bool> s_9__0=null;

            //  Func<RoadFlow.Model.FlowTask, bool> s_9__0;
            string str = base.Request.Querys("appid");
            string str2 = base.Request.Querys("tabid");
            string str3 = base.Request.Forms("FlowID");
            string Title = base.Request.Forms("Title");
            string str4 = base.Request.Forms("ReceiveID");
            string str5 = base.Request.Forms("Date1");
            string str6 = base.Request.Forms("Date2");
            string str7 = base.Request.Forms("sidx");
            string str8 = base.Request.Forms("sord");
            string str9 = (str7.IsNullOrEmpty() ? "ReceiveTime" : str7) + " " + (str8.IsNullOrEmpty() ? "DESC" : str8);
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            Guid userId = new RoadFlow.Business.User().GetUserId(str4);
            if (str3.IsNullOrWhiteSpace())
            {
                str3 = StringExtensions.JoinSqlIn<Guid>(new RoadFlow.Business.Flow().GetManageInstanceFlowIds(Current.UserId), true);
            }
            DataTable table = task.GetInstanceList(pageSize, pageNumber, str3, Title, GuidExtensions.IsEmptyGuid(userId) ? "" : userId.ToString(), str5, str6, str9, out num3);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                List<RoadFlow.Model.FlowTask> listByGroupId = task.GetListByGroupId(StringExtensions.ToGuid(row["GroupId"].ToString()));
                RoadFlow.Model.FlowTask task2 = null;
                if (!Title.IsNullOrWhiteSpace())
                {
                    task2 = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)Enumerable.ThenBy<RoadFlow.Model.FlowTask, int>(Enumerable.OrderByDescending<RoadFlow.Model.FlowTask, int>(Enumerable.Where<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId, s_9__0 ?? (s_9__0 = delegate (RoadFlow.Model.FlowTask p) {
                        return p.Title.ContainsIgnoreCase(Title);
                    })), key=>key.Sort), 
                   key=> key.Status));
                }
                if (task2 == null)
                {
                    task2 = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)Enumerable.ThenBy<RoadFlow.Model.FlowTask, int>(Enumerable.OrderByDescending<RoadFlow.Model.FlowTask, int>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId,
                        key=>key.Sort),
                      key=> key.Status));
                }
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                string[] textArray1 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"manage('", task2.Id.ToString(), "', '", task2.GroupId.ToString(), "');\"><i class=\"fa fa-check-square-o\"></i>管理</a>" };
                string str10 = string.Concat((string[])textArray1);
                string[] textArray2 = new string[] { str10, "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"delete1('", task2.Id.ToString(), "', '", task2.GroupId.ToString(), "');\"><i class=\"fa fa-trash-o\"></i>删除</a>" };
                str10 = string.Concat((string[])textArray2);
                string executeTypeTitle = task.GetExecuteTypeTitle(task2.ExecuteType);
                object[] objArray1 = new object[0x11];
                objArray1[0] = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('";
                objArray1[1] = base.Url.Content("~/RoadFlowCore/FlowRun/Index");
                objArray1[2] = "?";
                object[] objArray2 = new object[] { task2.FlowId.ToString(), task2.StepId.ToString(), task2.InstanceId, task2.Id.ToString(), task2.GroupId.ToString(), str };
                objArray1[3] = string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1", (object[])objArray2);
                objArray1[4] = "','";
                objArray1[5] = task2.Title.TrimAll();
                objArray1[6] = "','";
                objArray1[7] = task2.Id.ToString();
                objArray1[8] = "',";
                objArray1[9] = (int)num4;
                objArray1[10] = ",";
                objArray1[11] = (int)num5;
                objArray1[12] = ",";
                objArray1[13] = (int)num6;
                objArray1[14] = ");return false;\">";
                objArray1[15] = task2.Title;
                objArray1[0x10] = "</a>";
                string str12 = string.Concat((object[])objArray1);
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)task2.Id.ToString());
                obj1.Add("Title", (JToken)str12);
                obj1.Add("FlowName", (JToken)task2.FlowName);
                obj1.Add("StepName", (JToken)task2.StepName);
                obj1.Add("ReceiveName", (JToken)task2.ReceiveName);
                obj1.Add("ReceiveTime", (JToken)DateTimeExtensions.ToDateTimeString(task2.ReceiveTime));
                obj1.Add("StatusTitle", (JToken)executeTypeTitle);
                obj1.Add("Opation", (JToken)str10);
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray3 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray3);
        }

        public string QueryMyEntrust()
        {
            int num3;
            string str = base.Request.Querys("appid");
            string str2 = base.Request.Querys("tabid");
            string str3 = base.Request.Forms("flowid");
            string str4 = base.Request.Forms("title");
            string str5 = base.Request.Forms("date1");
            string str6 = base.Request.Forms("date2");
            string str7 = base.Request.Forms("sidx");
            string str8 = base.Request.Forms("sord");
            string str9 = (str7.IsNullOrEmpty() ? "ReceiveTime" : str7) + " " + (str8.IsNullOrEmpty() ? "ASC" : str8);
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            FlowTask task = new FlowTask();
            DataTable table = task.GetEntrustTask(pageSize, pageNumber, Current.UserId, str3, str4, str5, str6, str9, out num3);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                StringBuilder builder = new StringBuilder();
                if (task.IsEntrustWithdraw(StringExtensions.ToGuid(row["Id"].ToString())))
                {
                    builder.Append("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"withdraw('" + row["Id"] + "');return false;\"><i class=\"fa fa-mail-reply\"></i>收回</a>");
                }
                object[] objArray1 = new object[0x11];
                objArray1[0] = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('";
                objArray1[1] = base.Url.Content("~/RoadFlowCore/FlowRun/Index");
                objArray1[2] = "?";
                object[] objArray2 = new object[] { row["FlowId"], row["StepId"], row["InstanceId"], row["Id"], row["GroupId"], str };
                objArray1[3] = string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}&display=1", (object[])objArray2);
                objArray1[4] = "','";
                objArray1[5] = row["Title"].ToString().RemoveHTML().UrlEncode();
                objArray1[6] = "','";
                objArray1[7] = row["Id"];
                objArray1[8] = "',";
                objArray1[9] = (int)num4;
                objArray1[10] = ",";
                objArray1[11] = (int)num5;
                objArray1[12] = ",";
                objArray1[13] = (int)num6;
                objArray1[14] = ");return false;\">";
                objArray1[15] = row["Title"];
                objArray1[0x10] = "</a>";
                string str10 = string.Concat((object[])objArray1);
                string executeTypeTitle = task.GetExecuteTypeTitle(row["ExecuteType"].ToString().ToInt(-2147483648));
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Title", (JToken)str10);
                obj1.Add("FlowName", (JToken)row["FlowName"].ToString());
                obj1.Add("StepName", (JToken)row["StepName"].ToString());
                obj1.Add("SenderName", (JToken)row["SenderName"].ToString());
                obj1.Add("ReceiveTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["ReceiveTime"].ToString())));
                obj1.Add("ReceiveName", (JToken)row["ReceiveName"].ToString());
                obj1.Add("Status", (JToken)executeTypeTitle);
                obj1.Add("Opation", (JToken)builder.ToString());
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray3 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray3);
        }







        [Validate(CheckApp = false)]
        public string QueryWait()
        {
            int num3;
            string str = base.Request.Querys("appid");
            string str2 = base.Request.Querys("tabid");
            string str3 = base.Request.Forms("flowid");
            string str4 = base.Request.Forms("title");
            string str5 = base.Request.Forms("date1");
            string str6 = base.Request.Forms("date2");
            string str7 = base.Request.Forms("sidx");
            string str8 = base.Request.Forms("sord");
            string str9 = (str7.IsNullOrEmpty() ? "ReceiveTime" : str7) + " " + (str8.IsNullOrEmpty() ? "ASC" : str8);
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            DataTable table = task.GetWaitTask(pageSize, pageNumber, Current.UserId, str3, str4, str5, str6, str9, out num3,0);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                DateTime time;
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                object[] objArray1 = new object[0x15];
                objArray1[0] = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('";
                objArray1[1] = base.Url.Content("~/RoadFlowCore/FlowRun/Index");
                objArray1[2] = "?";
                object[] objArray2 = new object[] { row["FlowId"], row["StepId"], row["InstanceId"], row["Id"], row["GroupId"], str };
                objArray1[3] = string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}", (object[])objArray2);
                objArray1[4] = "','";
                objArray1[5] = row["Title"].ToString().RemoveHTML().UrlEncode();
                objArray1[6] = "','";
                objArray1[7] = row["Id"];
                objArray1[8] = "',";
                objArray1[9] = (int)num4;
                objArray1[10] = ",";
                objArray1[11] = (int)num5;
                objArray1[12] = ",";
                objArray1[13] = (int)num6;
                objArray1[14] = ");return false;\"><i class=\"fa fa-pencil-square-o\"></i>处理</a><a class=\"list\" href=\"javascript:void(0);\" onclick=\"detail('";
                objArray1[15] = row["FlowId"];
                objArray1[0x10] = "','";
                objArray1[0x11] = row["GroupId"];
                objArray1[0x12] = "','";
                objArray1[0x13] = row["Id"];
                objArray1[20] = "');return false;\"><i class=\"fa fa-navicon\"></i>过程</a>";
                StringBuilder builder = new StringBuilder(string.Concat((object[])objArray1));
                if (task.IsDelete(StringExtensions.ToGuid(row["Id"].ToString()), null))
                {
                    object[] objArray3 = new object[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"delTask('", row["FlowId"], "','", row["GroupId"], "','", row["Id"], "');return false;\"><i class=\"fa fa-remove\"></i>作废</a>" };
                    builder.Append(string.Concat((object[])objArray3));
                }
                object[] objArray4 = new object[0x11];
                objArray4[0] = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('";
                objArray4[1] = base.Url.Content("~/RoadFlowCore/FlowRun/Index");
                objArray4[2] = "?";
                object[] objArray5 = new object[] { row["FlowId"], row["StepId"], row["InstanceId"], row["Id"], row["GroupId"], str };
                objArray4[3] = string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}", (object[])objArray5);
                objArray4[4] = "','";
                objArray4[5] = row["Title"].ToString().RemoveHTML().UrlEncode();
                objArray4[6] = "','";
                objArray4[7] = row["Id"];
                objArray4[8] = "',";
                objArray4[9] = (int)num4;
                objArray4[10] = ",";
                objArray4[11] = (int)num5;
                objArray4[12] = ",";
                objArray4[13] = (int)num6;
                objArray4[14] = ");return false;\">";
                objArray4[15] = row["Title"];
                objArray4[0x10] = "</a>";
                string str10 = string.Concat((object[])objArray4);
                string str11 = "<div>正常</div>";
                if (StringExtensions.IsDateTime(row["CompletedTime"].ToString(), out time))
                {
                    DateTime time2 = DateTimeExtensions.Now;
                    if (time2 > time)
                    {
                        str11 = "<div title='要求完成时间：" + time.ToString("yyyy-MM-dd HH:mm") + "' style='color:red;font-weight:bold;'>已超期</div>";
                    }
                    else
                    {
                        TimeSpan span = (TimeSpan)(time - time2);
                        if (span.Days < 3)
                        {
                            span = (TimeSpan)(time - time2);
                            double num7 = Math.Ceiling(span.TotalDays);
                            string str12 = (num7 == 0.0) ? (((int)(span = (TimeSpan)(time - time2)).Hours) + "小时") : (((double)num7).ToString() + "天");
                            string[] textArray1 = new string[] { "<div title='要求完成时间：", time.ToString("yyyy-MM-dd HH:mm"), "' style='color:#ff7302;font-weight:bold;'>", str12, "内到期</div>" };
                            str11 = string.Concat((string[])textArray1);
                        }
                        else
                        {
                            str11 = "<div title='要求完成时间：" + time.ToString("yyyy-MM-dd HH:mm") + "' style=''>正常</div>";
                        }
                    }
                }
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Title", (JToken)str10);
                obj1.Add("FlowName", (JToken)row["FlowName"].ToString());
                obj1.Add("StepName", (JToken)row["StepName"].ToString());
                obj1.Add("SenderName", (JToken)row["SenderName"].ToString());
                obj1.Add("ReceiveTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["ReceiveTime"].ToString())));
                obj1.Add("StatusTitle", (JToken)str11);
                obj1.Add("Note", (JToken)row["Note"].ToString());
                obj1.Add("Opation", (JToken)builder.ToString());
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray6 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray6);
        }


        [Validate(CheckApp = false)]
        public string QueryWaitBatch()
        {
            int num3;
            string str = base.Request.Querys("appid");
            string str2 = base.Request.Querys("tabid");
            string flowId = base.Request.Forms("flowid");
            string title = base.Request.Forms("title");
            string startDate = base.Request.Forms("date1");
            string endDate = base.Request.Forms("date2");
            string str7 = base.Request.Forms("sidx");
            string str8 = base.Request.Forms("sord");
            string order = (str7.IsNullOrEmpty() ? "ReceiveTime" : str7) + " " + (str8.IsNullOrEmpty() ? "ASC" : str8);
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            FlowTask task = new FlowTask();
            DataTable table = task.GetWaitTask(pageSize, pageNumber, Current.UserId, flowId, title, startDate, endDate, order, out num3, 1);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                DateTime time;
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                object[] objArray1 = new object[0x15];
                objArray1[0] = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('";
                objArray1[1] = base.Url.Content("~/RoadFlowCore/FlowRun/Index");
                objArray1[2] = "?";
                object[] objArray2 = new object[] { row["FlowId"], row["StepId"], row["InstanceId"], row["Id"], row["GroupId"], str };
                objArray1[3] = string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}", (object[])objArray2);
                objArray1[4] = "','";
                objArray1[5] = row["Title"].ToString().RemoveHTML().UrlEncode();
                objArray1[6] = "','";
                objArray1[7] = row["Id"];
                objArray1[8] = "',";
                objArray1[9] = (int)num4;
                objArray1[10] = ",";
                objArray1[11] = (int)num5;
                objArray1[12] = ",";
                objArray1[13] = (int)num6;
                objArray1[14] = ");return false;\"><i class=\"fa fa-pencil-square-o\"></i>处理</a><a class=\"list\" href=\"javascript:void(0);\" onclick=\"detail('";
                objArray1[15] = row["FlowId"];
                objArray1[0x10] = "','";
                objArray1[0x11] = row["GroupId"];
                objArray1[0x12] = "','";
                objArray1[0x13] = row["Id"];
                objArray1[20] = "');return false;\"><i class=\"fa fa-navicon\"></i>过程</a>";
                StringBuilder builder = new StringBuilder(string.Concat((object[])objArray1));
                if (task.IsDelete(row["Id"].ToString().ToGuid(), null))
                {
                    object[] objArray3 = new object[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"delTask('", row["FlowId"], "','", row["GroupId"], "','", row["Id"], "');return false;\"><i class=\"fa fa-remove\"></i>作废</a>" };
                    builder.Append(string.Concat((object[])objArray3));
                }
                object[] objArray4 = new object[0x11];
                objArray4[0] = "<a href=\"javascript:void(0);\" class=\"list\" onclick=\"openTask('";
                objArray4[1] = base.Url.Content("~/RoadFlowCore/FlowRun/Index");
                objArray4[2] = "?";
                object[] objArray5 = new object[] { row["FlowId"], row["StepId"], row["InstanceId"], row["Id"], row["GroupId"], str };
                objArray4[3] = string.Format("flowid={0}&stepid={1}&instanceid={2}&taskid={3}&groupid={4}&appid={5}", (object[])objArray5);
                objArray4[4] = "','";
                objArray4[5] = row["Title"].ToString().RemoveHTML().UrlEncode();
                objArray4[6] = "','";
                objArray4[7] = row["Id"];
                objArray4[8] = "',";
                objArray4[9] = (int)num4;
                objArray4[10] = ",";
                objArray4[11] = (int)num5;
                objArray4[12] = ",";
                objArray4[13] = (int)num6;
                objArray4[14] = ");return false;\">";
                objArray4[15] = row["Title"];
                objArray4[0x10] = "</a>";
                string str10 = string.Concat((object[])objArray4);
                string str11 = "<div>正常</div>";
                if (row["CompletedTime"].ToString().IsDateTime(out time))
                {
                    DateTime now = DateTimeExtensions.Now;
                    if (now > time)
                    {
                        str11 = "<div title='要求完成时间：" + time.ToString("yyyy-MM-dd HH:mm") + "' style='color:red;font-weight:bold;'>已超期</div>";
                    }
                    else
                    {
                        TimeSpan span = (TimeSpan)(time - now);
                        if (span.Days < 3)
                        {
                            span = (TimeSpan)(time - now);
                            double num7 = Math.Ceiling(span.TotalDays);
                            string str12 = (num7 == 0.0) ? (((int)(span = (TimeSpan)(time - now)).Hours) + "小时") : (((double)num7).ToString() + "天");
                            string[] textArray1 = new string[] { "<div title='要求完成时间：", time.ToString("yyyy-MM-dd HH:mm"), "' style='color:#ff7302;font-weight:bold;'>", str12, "内到期</div>" };
                            str11 = string.Concat((string[])textArray1);
                        }
                        else
                        {
                            str11 = "<div title='要求完成时间：" + time.ToString("yyyy-MM-dd HH:mm") + "' style=''>正常</div>";
                        }
                    }
                }
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Title", (JToken)str10);
                obj1.Add("FlowName", (JToken)row["FlowName"].ToString());
                obj1.Add("StepName", (JToken)row["StepName"].ToString());
                obj1.Add("SenderName", (JToken)row["SenderName"].ToString());
                obj1.Add("ReceiveTime", (JToken)row["ReceiveTime"].ToString().ToDateTime().ToDateTimeString());
                obj1.Add("StatusTitle", (JToken)str11);
                obj1.Add("Note", (JToken)row["Note"].ToString());
                obj1.Add("Opation", (JToken)builder.ToString());
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray6 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray6);
        }




    





        /// <summary>
        /// 保存指派
        /// </summary>
        /// <returns></returns>
        [Validate, ValidateAntiForgeryToken]
        public string SaveDesignate()
        {
            Guid guid;
            string str = base.Request.Querys("taskid");
            string str2 = base.Request.Forms("user");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return "任务ID错误!";
            }
            if (str2.IsNullOrWhiteSpace())
            {
                return "没有选择要指派的人员!";
            }
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            RoadFlow.Model.FlowTask task2 = task.Get(guid);
            if (task2 == null)
            {
                return "没有找到当前任务!";
            }
            string str3 = task.Designate(task2, new RoadFlow.Business.Organize().GetAllUsers(str2));
            RoadFlow.Business.Log.Add("指派了任务-" + task2.Title, task2.ToString() + "-" + str3, LogType.流程运行, "", "", str2, "", "", "", "", "");
            if (!"1".Equals(str3))
            {
                return str3;
            }
            return "指派成功!";
        }



        /// <summary>
        /// 保存跳转
        /// </summary>
        /// <returns></returns>
        [Validate, ValidateAntiForgeryToken]
        public string SaveGoTo()
        {
            Guid guid;
            string str = base.Request.Querys("taskid");
            string str2 = base.Request.Forms("step");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return "任务ID错误!";
            }
            RoadFlow.Model.FlowTask task2 = new RoadFlow.Business.FlowTask().Get(guid);
            if (task2 == null)
            {
                return "未找到当前任务!";
            }
            Dictionary<Guid, List<RoadFlow.Model.User>> dictionary = new Dictionary<Guid, List<RoadFlow.Model.User>>();
            RoadFlow.Business.Organize organize = new RoadFlow.Business.Organize();
            foreach (string str4 in str2.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid2;
                if (StringExtensions.IsGuid(str4, out guid2))
                {
                    string str5 = base.Request.Forms("member_" + str4);
                    if (!str5.IsNullOrWhiteSpace())
                    {
                        dictionary.Add(guid2, organize.GetAllUsers(str5));
                    }
                }
            }
            string str3 = new RoadFlow.Business.FlowTask().GoTo(task2, dictionary);
            RoadFlow.Business.Log.Add("跳转了任务-" + task2.Title, task2.ToString() + "-" + str3, LogType.流程运行, "", "", JsonConvert.SerializeObject(dictionary), "", "", "", "", "");
            if (!"1".Equals(str3))
            {
                return str3;
            }
            return "跳转成功!";
        }

        /// <summary>
        /// 保存状态
        /// </summary>
        /// <returns></returns>
        [Validate, ValidateAntiForgeryToken]
        public string SaveStatus()
        {
            Guid guid;
            int num;
            string str = base.Request.Forms("status");
            if (!StringExtensions.IsGuid(base.Request.Querys("taskid"), out guid))
            {
                return "任务ID错误!";
            }
            if (!str.IsInt(out num))
            {
                return "状态错误!";
            }
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            RoadFlow.Model.FlowTask flowTaskModel = task.Get(guid);
            string oldContents = flowTaskModel.ToString();
            if (flowTaskModel == null)
            {
                return "未找到当前任务!";
            }
            flowTaskModel.ExecuteType = num;
            if (num < 2)
            {
                flowTaskModel.Status = num;
            }
            else
            {
                flowTaskModel.Status = 2;
            }
            task.Update(flowTaskModel);
            RoadFlow.Business.Log.Add("改变了任务状态-" + flowTaskModel.Title, "", LogType.流程运行, oldContents, flowTaskModel.ToString(), "", "", "", "", "", "");
            return "保存成功!";
        }

        [Validate(CheckApp = false)]
        public IActionResult Wait()
        {
            base.ViewData["flowOptions"]= new RoadFlow.Business.Flow().GetOptions("");
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            return this.View();
        }


        [Validate(CheckApp = false)]
        public IActionResult WaitBatch()
        {
            base.ViewData["flowOptions"]= new Flow().GetOptions("");
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            return this.View();
        }



        [Validate(CheckApp = false)]
        public IActionResult WaitBatchConfirm()
        {
            base.ViewData["taskIds"]= base.Request.Querys("taskids");
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["commentOptions"]= new FlowComment().GetOptionsByUserId(Current.UserId);
            return this.View();
        }



        [ValidateAntiForgeryToken, Validate(CheckApp = false), HttpPost]
        public string WaitBatchExecute()
        {
            string[] strArray = base.Request.Forms("taskids").Split(',', (StringSplitOptions)StringSplitOptions.None);
            string str = base.Request.Forms("executetype");
            string comment = base.Request.Forms("comment");
            int num = 0;
            FlowTask task = new FlowTask();
            foreach (string str3 in strArray)
            {
                Guid guid;
                if (str3.IsGuid(out guid))
                {
                    ExecuteResult result = task.AutoSubmit(guid, str.IsNullOrWhiteSpace() ? "submit" : str, comment);
                    if (result.IsSuccess)
                    {
                        num++;
                    }
                    else
                    {
                        Log.Add("批量提交待办事项发生了错误 - " + result.Messages, result.ToString(), LogType.流程运行, "", "", "", "", "", "", "", "");
                    }
                }
            }
            return ("成功处理了" + ((int)num).ToString() + "个待办事项!");
        }








        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string Withdraw()
        {
            Guid guid;
            Guid taskId;
            string str = base.Request.Querys("withdrawtaskid");
            if (!StringExtensions.IsGuid(base.Request.Querys("withdrawgroupid"), out guid) || !StringExtensions.IsGuid(str, out taskId))
            {
                return "任务Id错误!";
            }
            RoadFlow.Business.FlowTask task = new RoadFlow.Business.FlowTask();
            List<RoadFlow.Model.FlowTask> listByGroupId = task.GetListByGroupId(guid);
            List<RoadFlow.Model.FlowTask> list2 = listByGroupId.FindAll(delegate (RoadFlow.Model.FlowTask p) {
                return (p.PrevId == taskId) && (p.Status == 0);
            });
            List<RoadFlow.Model.FlowTask> list3 = new List<RoadFlow.Model.FlowTask>();
            foreach (RoadFlow.Model.FlowTask task3 in list2)
            {
                list3.Add(task3);
            }
            List<RoadFlow.Model.FlowTask> list4 = new List<RoadFlow.Model.FlowTask>();
            RoadFlow.Model.FlowTask task2 = listByGroupId.Find(delegate (RoadFlow.Model.FlowTask p) {
                return p.Id == taskId;
            });
            if (task2 == null)
            {
                return "未找到当前任务!";
            }
            task2.Status = 0;
            task2.ExecuteType = 0;
            task2.Comments = "";
            task2.IsSign = 0;
            task2.CompletedTime1=(DateTime?)null;
            list4.Add(task2);
            task.Update(list3, list4, null, null);
            return "收回成功!";
        }

      
    }


 

    #endregion


}
