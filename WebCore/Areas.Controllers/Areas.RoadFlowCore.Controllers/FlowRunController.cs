using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

    #region 新的方法2.8.3
    [Area("RoadFlowCore")]
    public class FlowRunController : Controller
    {
        // Methods
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult AddWrite()
        {
            base.ViewData["queryString"] = base.Request.UrlQuery();
            base.ViewData["rf_appopenmodel"] = base.Request.Querys("rf_appopenmodel");
            base.ViewData["openerId"] = base.Request.Querys("openerid");
            return this.View();
        }




        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult Execute()
        {
            s_c__DisplayClass1_0 class_ = new s_c__DisplayClass1_0();
            Guid guid;
            Guid guid2;
            string str = base.Request.Forms("params");
            string str2 = base.Request.Forms("issign");
            string str3 = base.Request.Forms("comment");
            string str4 = base.Request.Forms("attachment");
            string str5 = base.Request.Querys("flowid");
            string str6 = base.Request.Querys("instanceid");
            string str7 = base.Request.Querys("taskid");
            string str8 = base.Request.Querys("stepid");
            string str9 = base.Request.Querys("groupid");
            string str10 = base.Request.Querys("ismobile");
            bool flag = "1".Equals(base.Request.Forms("form_iscustomeform"));
            if (str6.IsNullOrWhiteSpace())
            {
                str6 = base.Request.Forms("instanceid");
            }
            if (str6.IsNullOrWhiteSpace())
            {
                str6 = base.Request.Forms("form_instanceid");
            }
            base.ViewData["rf_appopenmodel"] = base.Request.Querys("rf_appopenmodel");
            if (str.IsNullOrWhiteSpace())
            {
                base.ViewData["alertMsg"] = "执行参数为空";
                base.ViewData["success"] = "0";
                return this.View();
            }
            if (!StringExtensions.IsGuid(str5, out guid))
            {
                base.ViewData["alertMsg"] = "流程ID错误";
                base.ViewData["success"] = "0";
                return this.View();
            }
            Flow flow = new Flow();
            FlowTask task = new FlowTask();

            ValueTuple<RoadFlow.Model.FlowTask, RoadFlow.Model.FlowTask, List<RoadFlow.Model.FlowTask>> dynamicTask = task.GetDynamicTask(StringExtensions.ToGuid(str9), new Guid?(StringExtensions.ToGuid(str7)));
            RoadFlow.Model.FlowTask task2 = dynamicTask.Item1;
            RoadFlow.Model.FlowTask task3 = dynamicTask.Item2;
            List<RoadFlow.Model.FlowTask> list = dynamicTask.Item3;


            //动态获取值有问题
            RoadFlow.Model.FlowRun flowRunModel = flow.GetFlowRunModel(guid, true,task2);
            if (flowRunModel == null)
            {
                base.ViewData["alertMsg"] = "未找到流程运行实体";
                base.ViewData["success"] = "0";
                return this.View();
            }
            JObject obj2 = JObject.Parse(str);
            string str11 = obj2.Value<string>("type");
            if (str11.IsNullOrWhiteSpace())
            {
                base.ViewData["alertMsg"] = "未找到要处理的类型";
                base.ViewData["success"] = "0";
                return this.View();
            }
            Execute executeModel = new Execute();
            switch (PrivateImplementationDetails.ComputeStringHash(str11))
            {
                case 0x5bb421a2:
                    if (str11 == "back")
                    {
                        executeModel.ExecuteType = OperationType.Back;
                        break;
                    }
                    break;

                case 0x889f5ae3:
                    if (str11 == "addwrite")
                    {
                        executeModel.ExecuteType = OperationType.AddWrite;
                        break;
                    }
                    break;

                case 0x6c681ab:
                    if (str11 == "freesubmit")
                    {
                        executeModel.ExecuteType = OperationType.FreeSubmit;
                        break;
                    }
                    break;

                case 0x173b3eb1:
                    if (str11 == "redirect")
                    {
                        executeModel.ExecuteType = OperationType.Redirect;
                        break;
                    }
                    break;

                case 0xccff7e48:
                    if (str11 == "save")
                    {
                        executeModel.ExecuteType = OperationType.Save;
                        break;
                    }
                    break;

                case 0xe6b52521:
                    if (str11 == "taskend")
                    {
                        executeModel.ExecuteType = OperationType.TaskEnd;
                        break;
                    }
                    break;

                case 0xf4748112:
                    if (str11 == "completed")
                    {
                        executeModel.ExecuteType = OperationType.Completed;
                        break;
                    }
                    break;

                case 0xf7eb0225:
                    if (str11 == "submit")
                    {
                        executeModel.ExecuteType = OperationType.Submit;
                        break;
                    }
                    break;

                case 0xfaaf6c44:
                    if (str11 == "copyforcompleted")
                    {
                        executeModel.ExecuteType = OperationType.CopyforCompleted;
                        break;
                    }
                    break;
            }
            executeModel.IsAutoSubmit = false;
            executeModel.Comment = str3.Trim();
            executeModel.FlowId = guid;
            executeModel.GroupId = StringExtensions.ToGuid(str9);
            executeModel.InstanceId = str6;
            executeModel.IsSign = str2.ToInt(0);
            executeModel.Note = string.Empty;

            executeModel.OtherType = 0;
            executeModel.Sender = Current.User;
            executeModel.StepId = StringExtensions.IsGuid(str8, out guid2) ? guid2 : flowRunModel.FirstStepId;
            executeModel.TaskId = StringExtensions.ToGuid(str7);
            executeModel.ParamsJSON = str;
            executeModel.Attachment = str4;

          

            List<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>> list2 = new List<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>();

         
            Step step = flowRunModel.Steps.Find(delegate (Step p)
            {
                return p.Id == executeModel.StepId;
            });
            if (step == null)
            {
                base.ViewData["alertMsg"] = "未找到当前步骤!";
                base.ViewData["success"] = "0";
                return this.View();
            }
            JArray array = obj2.Value<JArray>("steps");
            if (array != null)
            {
                Organize organize = new Organize();
                FlowEntrust entrust = new FlowEntrust();
                User user = new User();
                foreach (JObject obj3 in array)
                {
                    Guid guid3;
                    string str14 = obj3.Value<string>("id");
                    string str15 = obj3.Value<string>("name");
                    string str16 = obj3.Value<string>("beforestepid");


                    string str17 = obj3.Value<string>("parallelorserial");
                    string str18 = obj3.Value<string>("member");
                    string str19 = obj3.Value<string>("completedtime");


                    if (StringExtensions.IsGuid(str14, out guid3))
                    {
                        DateTime time;
                        DateTime? nullable = null;
                        if (StringExtensions.IsDateTime(str19, out time))
                        {
                            nullable = new DateTime?(time);
                        }
                        if (((executeModel.ExecuteType == OperationType.Submit) || (executeModel.ExecuteType == OperationType.Back)) || ((executeModel.ExecuteType == OperationType.Redirect) || (executeModel.ExecuteType == OperationType.FreeSubmit)))
                        {
                            Guid guid4;
                            int num2;


                            List<RoadFlow.Model.User> allUsers = organize.GetAllUsers(str18);
                            for (int i = 0; i < allUsers.Count; i++)
                            {
                                RoadFlow.Model.User user2 = allUsers[i].Clone();
                                user2.Note = "";
                                string entrustUserId = entrust.GetEntrustUserId(guid, user2);
                                if (!entrustUserId.IsNullOrWhiteSpace())
                                {
                                    RoadFlow.Model.User user3 = user.Get(entrustUserId);
                                    if (user3 != null)
                                    {
                                        user2 = user3.Clone();
                                        user2.Note = allUsers[i].Id.ToString();
                                    }
                                }
                                allUsers[i] = user2;
                            }
                            list2.Add(new ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>(guid3, str15, StringExtensions.IsGuid(str16, out guid4) ? new Guid?(guid4) : null, str17.IsInt(out num2) ? new int?(num2) : null, allUsers, nullable));

                        }
                    }
                }
            }
            executeModel.Steps = list2;
            if (((executeModel.ExecuteType != OperationType.CopyforCompleted) && (executeModel.ExecuteType != OperationType.TaskEnd) && (step.StepBase.FlowType != 0)) || (executeModel.ExecuteType == OperationType.Save))
            {
                try
                {
                    ValueTuple<string, string> tuple2 = new Form().SaveData(base.Request);
                    string str21 = tuple2.Item1;
                    string str22 = tuple2.Item2;
                    if (executeModel.InstanceId.IsNullOrWhiteSpace())
                    {
                        executeModel.InstanceId = str21;
                    }
                }
                catch (Exception exception)
                {
                   // Log.Add(exception, "处理流程（" + flowRunModel.Name + "）保存数据时发生了错误");
                    Log.Add(exception, "处理流程（" + flowRunModel.Name + "）保存数据时发生了错误");
                    base.ViewData["debutMsg"]= (flowRunModel.Debug == 1) ? string.Concat((string[])new string[] { "执行参数：<br/>", str, "<br/><br/>参数实体：<br/>", executeModel.ToString(), "<br/><br/>执行结果：<br/>", exception.Message, "<br/>", exception.StackTrace }) : "";
                    base.ViewData["alertMsg"]= "操作发生了错误，请联系管理员！";
                    base.ViewData["success"]= "0";
                    return this.View();
                }
               
            }
            string str12 = string.Empty;
            if (flag)
            {
                str12 = base.Request.Forms("customformtitle");
            }
            else
            {
                string str23 = base.Request.Forms("form_dbtabletitleexpression");
                if (str23.IsNullOrWhiteSpace())
                {
                    str12 = base.Request.Forms(base.Request.Forms("form_dbtable") + "-" + base.Request.Forms("form_dbtabletitle"));
                }
                else
                {
                    string tableName = base.Request.Forms("Form_DBTable");
                    str12 = new Form().ReplaceTitleExpression(str23, tableName, executeModel.InstanceId, base.Request);
                }
            }
            executeModel.Title = str12;

          
                //判断？？？？
                if ((Config.EnableDynamicStep && (executeModel.Steps.Exists(
                    key=> GuidExtensions.IsNotEmptyGuid(key.Item3)) && (flowRunModel.FirstStepId != step.Id)) && (GuidExtensions.IsNotEmptyGuid(executeModel.TaskId) && GuidExtensions.IsNotEmptyGuid(executeModel.GroupId))))
            {
                RoadFlow.Model.FlowRun run2 = new FlowDynamic().Add(executeModel,list);
                if (run2 != null)
                {
                    flowRunModel = run2;
                }
                executeModel.Steps.RemoveAll(
                   key =>
                   {
                       if (key.Item4.HasValue)
                       {
                           int? nullable = key.Item4;
                           int num = 1;
                           if ((nullable.GetValueOrDefault() == num) & nullable.HasValue)
                           {
                               Guid guid = key.Item1;
                               Guid? nullable2 = key.Item3;
                               return (!nullable2.HasValue || (guid != nullable2.GetValueOrDefault()));
                           }
                       }
                       return false;
                   });
            }






            ExecuteResult result = task.Execute(executeModel,flowRunModel);
            if(result.AutoSubmitTasks!=null)
            {
                if ((result != null) && Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)result.AutoSubmitTasks))
                {
                    foreach (RoadFlow.Model.FlowTask task5 in result.AutoSubmitTasks)
                    {
                        ExecuteResult result3 = task.AutoSubmit(task5.Id, "submit");
                        string[] textArray1 = new string[] { "流程完成后自动提交了任务 - ", result3.IsSuccess ? "成功" : "失败", " - ", task5.FlowName, " - ", task5.StepName, " - ", task5.Title };
                        Log.Add(string.Concat((string[])textArray1), task5.ToString(), LogType.流程运行, "", "", "执行结果：" + result3.ToString(), "", "", "", "", "");
                    }
                }
            }
               

            

            if ((step.Archives == 1) && (((executeModel.ExecuteType == OperationType.Submit) || (executeModel.ExecuteType == OperationType.FreeSubmit)) || (executeModel.ExecuteType == OperationType.Completed)))
            {
                List<Database> list4 = flowRunModel.Databases;
                if (list4.Count > 0)
                {
                    string str28;
                    string str26 = base.Request.Forms("form_commentlist_div_textarea");
                    Database database = Enumerable.First<Database>((IEnumerable<Database>)list4);
                    string str27 = new Form().GetFormData(database.ConnectionId.ToString(), database.Table, database.PrimaryKey, executeModel.InstanceId, executeModel.StepId.ToString(), executeModel.FlowId.ToString(), base.Request.Forms("form_dataformatjson"), out str28);
                    string str29 = string.Empty;
                    string receiveName = string.Empty;
                    if (result.CurrentTask != null)
                    {
                        str29 = result.CurrentTask.ReceiveOrganizeId.HasValue ? ("r_" + result.CurrentTask.ReceiveOrganizeId.Value.ToString()) : ("u_" + result.CurrentTask.ReceiveId.ToString());
                        receiveName = result.CurrentTask.ReceiveName;
                    }
                    else
                    {
                        str29 = "u_" + executeModel.Sender.Id.ToString();
                        receiveName = executeModel.Sender.Name;
                    }
                    string str31 = base.Request.Forms("form_body_div_textarea");
                    RoadFlow.Model.FlowArchive archive1 = new RoadFlow.Model.FlowArchive
                    {
                        Comments = str26.Trim(),
                        DataJson = str27
                    };
                    archive1.FlowId = flowRunModel.Id;
                    archive1.FlowName = flowRunModel.Name;
                    archive1.Id = Guid.NewGuid();
                    archive1.InstanceId = executeModel.InstanceId;
                    archive1.StepId = step.Id;
                    archive1.StepName = step.Name;
                    archive1.TaskId = (result.CurrentTask == null) ? executeModel.TaskId : result.CurrentTask.Id;
                    archive1.Title = (result.CurrentTask == null) ? executeModel.Title : result.CurrentTask.Title;
                    archive1.UserId = str29;
                    archive1.UserName = receiveName;
                    archive1.WriteTime = DateTimeExtensions.Now;
                    archive1.FormHtml = str31;
                    RoadFlow.Model.FlowArchive flowArchive = archive1;
                    new FlowArchive().Add(flowArchive);
                }
            }
            string[] textArray3 = new string[] { "处理了流程 ", flowRunModel.Name, "] - 步骤 ", task.GetStepName(flowRunModel, executeModel.StepId), " - 标题 ", executeModel.Title };
            Log.Add(string.Concat((string[])textArray3), str, LogType.流程运行, "执行参数：" + executeModel.ToString(), "", "返回：" + result.ToString(), "", "", "", "", "");
            string str13 = string.Empty;
            class_.executeModel = executeModel;

            RoadFlow.Model.FlowTask task4 = result.NextTasks.Find(new Predicate<RoadFlow.Model.FlowTask>(((result.NextTasks == null) ? null : (Predicate<RoadFlow.Model.FlowTask>)class_.Executeb__1)));
            if (task4 != null)
            {
                object[] objArray1 = new object[] { "Index", task4.FlowId, task4.StepId, task4.Id, task4.GroupId, task4.InstanceId, base.Request.Querys("appid"), base.Request.Querys("tabid"), base.Request.Querys("rf_appopenmodel"), base.Request.Querys("ismobile") };
                str13 = string.Format(base.Url.Content("~/RoadFlowCore/FlowRun/{0}?flowid={1}&stepid={2}&taskid={3}&groupid={4}&instanceid={5}&appid={6}&tabid={7}&rf_appopenmodel={8}&ismobile={9}"), (object[])objArray1);
            }
            else if (Config.IsIntegrateIframe)
            {
                if (executeModel.ExecuteType == OperationType.Save)
                {
                    object[] objArray2 = new object[] { "Index", task4.FlowId, task4.StepId, task4.Id, task4.GroupId, task4.InstanceId, base.Request.Querys("appid"), base.Request.Querys("tabid"), base.Request.Querys("rf_appopenmodel"), base.Request.Querys("ismobile") };
                    str13 = string.Format(base.Url.Content("~/RoadFlowCore/FlowRun/{0}?flowid={1}&stepid={2}&taskid={3}&groupid={4}&instanceid={5}&appid={6}&tabid={7}&rf_appopenmodel={8}&ismobile={9}"), (object[])objArray2);
                }
                else
                {
                    str13 = base.Url.Content("~/RoadFlowCore/FlowTask/Wait");
                }
            }

            base.ViewData["debutMsg"] = (flowRunModel.Debug == 1) ? string.Concat((string[])new string[] { "执行参数：<br/>", str, "<br/><br/>参数实体：<br/>", executeModel.ToString(), "<br/><br/>执行结果：<br/>", result.ToString() }) : "";
            base.ViewData["alertMsg"] = result.Messages;
            base.ViewData["success"] = result.IsSuccess ? "1" : "0";
            base.ViewData["ismobile"] = str10;
            base.ViewData["url"] = str13;
            return this.View();
        }










        /// <summary>
        /// 退回
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult FlowBack()
        {
            Guid guid;
            Guid guid2;
            string str = base.Request.Querys("flowid");
            string str2 = base.Request.Querys("taskid");
            string str3 = base.Request.Querys("stepid");
            string str4 = base.Request.Querys("groupid");
            string str5 = base.Request.Querys("instanceid");
            if (str5.IsNullOrWhiteSpace())
            {
                str5 = base.Request.Querys("instanceid1");
            }
            if (!StringExtensions.IsGuid(str, out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content = "流程ID错误";
                return result1;
            }
            if (!StringExtensions.IsGuid(str3, out guid2))
            {
                ContentResult result2 = new ContentResult();
                result2.Content = "步骤ID错误";
                return result2;
            }
            ValueTuple<string, string, List<Step>> tuple1 = new FlowTask().GetBackSteps(guid, guid2, StringExtensions.ToGuid(str4), StringExtensions.ToGuid(str2), str5, Current.UserId,null);

          //  ValueTuple<string, string, List<Step>> tuple1 = new FlowTask().GetBackSteps(guid, guid2, StringExtensions.ToGuid(str4), StringExtensions.ToGuid(str2), str5, Current.UserId);
            string str6 = tuple1.Item1;
            string str7 = tuple1.Item2;
            List<Step> list = tuple1.Item3;
            base.ViewData["backSteps"] = str6;
            base.ViewData["message"] = str7;
            base.ViewData["tabId"] = base.Request.Querys("tabid");
            base.ViewData["iframeId"] = base.Request.Querys("iframeid");
            base.ViewData["openerId"] = base.Request.Querys("openerid");
            return this.View();
        }

        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult FlowCopyFor()
        {
            base.ViewData["taskId"] = base.Request.Querys("taskid");
            return this.View();
        }

        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true), ValidateAntiForgeryToken]
        public string FlowCopyForSave()
        {
            string str = base.Request.Forms("taskid");
            string str2 = base.Request.Forms("user");
            List<RoadFlow.Model.User> allUsers = new Organize().GetAllUsers(str2);
            if (allUsers.Count == 0)
            {
                return "没有接收人!";
            }
            RoadFlow.Model.FlowTask task2 = new FlowTask().Get(StringExtensions.ToGuid(str));
            string str3 = new FlowTask().CopyFor(task2, allUsers);
            if (!"1".Equals(str3))
            {
                return str3;
            }
            return "抄送成功!";
        }


        /// <summary>
        /// 转交
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult FlowRedirect()
        {
            base.ViewData["stepId"] = base.Request.Querys("stepid");
            base.ViewData["openerId"] = base.Request.Querys("openerid");
            return this.View();
        }


        /// <summary>
        /// 发送或者步骤内发送
        /// </summary>
        /// <returns></returns>
        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult FlowSend()
        {
            Guid guid;
            Guid stepId;
            string str = base.Request.Querys("flowid");
            string str2 = base.Request.Querys("taskid");
            string str3 = base.Request.Querys("stepid");
            string str4 = base.Request.Querys("groupid");
            string str5 = base.Request.Querys("instanceid");
            string str6 = base.Request.Querys("freedomsend");
            string str7 = base.Request.Querys("ismobile");
            if (str5.IsNullOrWhiteSpace())
            {
                str5 = base.Request.Querys("instanceid1");
            }
            if (!StringExtensions.IsGuid(str, out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content = "流程ID错误!";
                return result1;
            }
            if (!StringExtensions.IsGuid(str3, out stepId))
            {
                ContentResult result2 = new ContentResult();
                result2.Content = "步骤ID错误!";
                return result2;
            }

            Guid guid2 = StringExtensions.ToGuid(str4);
            Guid guid3 = StringExtensions.ToGuid(str2);
            ValueTuple<RoadFlow.Model.FlowTask, RoadFlow.Model.FlowTask, List<RoadFlow.Model.FlowTask>> dynamicTask = new FlowTask().GetDynamicTask(guid2, null);
            RoadFlow.Model.FlowTask task = dynamicTask.Item1;
            RoadFlow.Model.FlowTask task2 = dynamicTask.Item2;
            List<RoadFlow.Model.FlowTask> list = dynamicTask.Item3;



            //动态
            RoadFlow.Model.FlowRun flowRunModel = new Flow().GetFlowRunModel(guid, true,task);
            if (flowRunModel == null)
            {
                ContentResult result3 = new ContentResult();
                result3.Content = "未找到流程运行时!";
                return result3;
            }
            ValueTuple<string, string, List<Step>> tuple2 = new FlowTask().GetNextSteps(flowRunModel, stepId, guid2, guid3, str5, Current.UserId, "1".Equals(str6), "1".Equals(str7),list);
            string str8 = tuple2.Item1;
            string str9 = tuple2.Item2;
            List<Step> list2 = tuple2.Item3;
            Step step = flowRunModel.Steps.Find(delegate (Step p)
            {
                return p.Id == stepId;
            });
            base.ViewData["nextSteps"] = str8;
            base.ViewData["openerId"] = base.Request.Querys("openerid");
            base.ViewData["freedomSend"] = str6;
            base.ViewData["tabId"] = base.Request.Querys("tabid");
            base.ViewData["iframeId"] = base.Request.Querys("iframeid");
            base.ViewData["nextStepCount"] = (int)list2.Count;
            base.ViewData["autoConfirm"] = (step != null) ? ((int)step.StepBase.AutoConfirm) : ((int)0);
            base.ViewData["isAddWrite"] = "0";


            //****************新增********
            base.ViewData["isMobile"]= base.Request.Querys("ismobile");


            return this.View();
        }

        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public string FormDelete()
        {
            Guid guid;
            string str = base.Request.Querys("connId");
            string str2 = base.Request.Querys("secondtable");
            string str3 = base.Request.Querys("secondtablePrimaryKey");
            string str4 = base.Request.Querys("secondtableId");
            if ((!StringExtensions.IsGuid(str, out guid) || str2.IsNullOrWhiteSpace()) || (str3.IsNullOrWhiteSpace() || str4.IsNullOrWhiteSpace()))
            {
                return "连接ID、表名、表主键、主键值不能为空!";
            }
            DbConnection connection = new DbConnection();
            string[] textArray1 = new string[] { "DELETE FROM ", str2, " WHERE ", str3, "={0}" };
            object[] objArray1 = new object[] { str4 };
            string str5 = connection.ExecuteSQL(guid, string.Concat((string[])textArray1), objArray1);
            Log.Add("删除了表单数据-" + str2, "连接ID：" + str + ", 主键值：" + str3, LogType.流程运行, "", "", "", "", "", "", "", "");
            if (!str5.IsInt())
            {
                return str5;
            }
            return "删除成功!";
        }

        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult FormEdit()
        {
            Guid guid;
            string str = base.Request.Querys("applibraryid");
            string str2 = base.Request.Querys("instanceid");
            string str3 = base.Request.Querys("appid");
            string str4 = base.Request.Querys("tabid");
            string str5 = base.Request.Querys("display");
            string str6 = base.Request.Querys("returnurl");
            string str7 = base.Request.Querys("showtoolbar");
            string str8 = base.Request.Querys("secondtable");
            string str9 = base.Request.Querys("primarytableid");
            string str10 = base.Request.Querys("secondtablerelationfield");
            if (str5.IsNullOrEmpty())
            {
                str5 = "0";
            }
            if (!StringExtensions.IsGuid(str, out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content = "应用程序库ID错误!";
                return result1;
            }
            bool flag = false;
            RoadFlow.Model.AppLibrary library = new AppLibrary().Get(guid);
            string address = string.Empty;
            if (library == null)
            {
                ContentResult result2 = new ContentResult();
                result2.Content = "未找到应用!";
                return result2;
            }
            flag = library.Code.IsNullOrWhiteSpace();
            if (flag)
            {
                address = library.Address;
            }
            else
            {
                //修改抓取文件路径
                address = base.Url.Content("~/Areas/RoadFlowCore/Views/FormDesigner/form/" + library.Address);
                address = address.Substring(0, address.IndexOf(System.IO.Path.GetExtension(address))) + ".cshtml";
            }
            string[] textArray1 = new string[] {
            "applibraryid=", str, "&instanceid=", str2, "&appid=", str3, "&programid=", base.Request.Querys("programid"), "&tabid=", str4, "&dilplay=", str5, "&iframeid=", base.Request.Querys("iframeid"), "&showtoolbar=", str7,
            "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&isrefresparent=", base.Request.Querys("isrefresparent"), "&primarytableid=", str9, "&secondtablerelationfield=", str10, "&secondtable=", str8, "&returnurl=", base.Request.Querys("returnurl").UrlEncode(), "&openerid=", base.Request.Querys("openerid")
         };
            string str12 = string.Concat((string[])textArray1);
            List<RoadFlow.Model.FlowButton> list = new List<RoadFlow.Model.FlowButton>();
            if (!flag)
            {
                List<StepButton> list2 = new List<StepButton>();

                if (!"1".Equals(str5))
                {

                    RoadFlow.Model.FlowButton button1 = new RoadFlow.Model.FlowButton();
                    button1.Id = Guid.NewGuid();
                    button1.Title = "确认保存";
                    button1.Ico = "fa-save";
                    button1.Script = "saveEditFormData(a);";
                    button1.Sort = 8;
                    list.Add(button1);
                }
                RoadFlow.Model.FlowButton button2 = new RoadFlow.Model.FlowButton();
                button2.Id = Guid.NewGuid();
                button2.Title = "关闭窗口";
                button2.Ico = "fa-close";
                button2.Script = "closeWindw(" + base.Request.Querys("rf_appopenmodel") + ");";
                button2.Sort = 9;
                list.Add(button2);
                if (!str6.IsNullOrWhiteSpace())
                {
                    RoadFlow.Model.FlowButton button3 = new RoadFlow.Model.FlowButton();
                    button3.Id = Guid.NewGuid();
                    button3.Title = "返回";
                    button3.Ico = "fa-mail-reply";
                    button3.Script = "window.location='" + str6 + "';";
                    button3.Sort = 0;
                    list.Add(button3);
                }
            }
            base.ViewData["tabId"] = str4;
            base.ViewData["appId"] = str3;
            base.ViewData["instanceId"] = str2;
            base.ViewData["display"] = str5;
            base.ViewData["formUrl"] = address;
            base.ViewData["query"] = str12;
            base.ViewData["isCustomeForm"] = flag ? "1" : "0";
            base.ViewData["flowButtons"]= Enumerable.ToList<RoadFlow.Model.FlowButton>((IEnumerable<RoadFlow.Model.FlowButton>)Enumerable.OrderBy<RoadFlow.Model.FlowButton, int>((IEnumerable<RoadFlow.Model.FlowButton>)list,
               key=>key.Sort));
            base.ViewData["request"] = base.Request;
            base.ViewData["title"] = library.Title;
            base.ViewData["secondtablerelationfield"] = str10;
            base.ViewData["primarytableid"] = str9;
            base.ViewData["secondtable"] = str8;
            return this.View();
        }

        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult FormEditSave()
        {
            ValueTuple<string, string> tuple1 = new Form().SaveData(base.Request);
            string str = tuple1.Item1;
            string str2 = tuple1.Item2;
            if (str2.IsNullOrWhiteSpace() && !str.IsNullOrWhiteSpace())
            {
                string[] textArray1 = new string[] {
                "FormEdit?instanceid=", str, "&applibraryid=", base.Request.Querys("applibraryid"), "&programid=", base.Request.Querys("programid"), "&appid=", base.Request.Querys("appid").Split(',', (StringSplitOptions) StringSplitOptions.None)[0], "&tabid=", base.Request.Querys("tabid"), "&dilplay=", base.Request.Querys("display"), "&isrefresparent=", base.Request.Querys("isrefresparent"), "&showtoolbar=", base.Request.Querys("showtoolbar"),
                "&primarytableid=", base.Request.Querys("primarytableid"), "&secondtablerelationfield=", base.Request.Querys("secondtablerelationfield"), "&secondtable=", base.Request.Querys("secondtable"), "&iframeid=", base.Request.Querys("iframeid"), "&openerid=", base.Request.Querys("openerid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&returnurl=", base.Request.Querys("returnurl").UrlEncode()
             };
                string str3 = string.Concat((string[])textArray1);
                base.ViewData["issuccess"] = "1";
                base.ViewData["msg"] = "保存成功!";
                base.ViewData["url"] = str3;
            }
            else
            {
                base.ViewData["issuccess"] = "0";
                base.ViewData["msg"] = str2;
                base.ViewData["url"] = "";
            }
            base.ViewData["isrefreshflow"] = (!base.Request.Querys("secondtablerelationfield").IsNullOrWhiteSpace() && !base.Request.Querys("secondtable").IsNullOrWhiteSpace()) ? "1" : "0";
            base.ViewData["rf_appopenmodel"] = base.Request.Querys("rf_appopenmodel");
            base.ViewData["isrefresparent"] = base.Request.Querys("isrefresparent");
            return this.View();
        }

        [Validate(CheckApp = false, CheckUrl = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult Index()
        {
            Guid guid;
            Guid guid2;
          
            string str = base.Request.Querys("flowid");
            string str2 = base.Request.Querys("stepid");
            string str3 = base.Request.Querys("taskid");
            string str4 = base.Request.Querys("groupid");
            string str5 = base.Request.Querys("instanceid");
            string str6 = base.Request.Querys("appid");
            string str7 = base.Request.Querys("tabid");
            string str8 = base.Request.Querys("display");
            string str9 = base.Request.Querys("showtoolbar");
            string str10 = base.Request.Querys("ismobile");
            int digit = base.Request.Querys("rf_appopenmodel").ToInt(0);
            if (str8.IsNullOrEmpty())
            {
                str8 = "0";
            }
            if (!StringExtensions.IsGuid(str, out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content = "流程ID错误!";
                return result1;
            }
            Flow flow = new Flow();

            FlowTask task = new FlowTask();
            ValueTuple<RoadFlow.Model.FlowTask, RoadFlow.Model.FlowTask, List<RoadFlow.Model.FlowTask>> dynamicTask = task.GetDynamicTask(StringExtensions.ToGuid(str4), new Guid?(StringExtensions.ToGuid(str3)));
            RoadFlow.Model.FlowTask task2 = dynamicTask.Item1;
            RoadFlow.Model.FlowTask flowTaskModel = dynamicTask.Item2;
            List<RoadFlow.Model.FlowTask> list = dynamicTask.Item3;


            //动态
            RoadFlow.Model.FlowRun flowRunModel = flow.GetFlowRunModel(guid, true, task2);
            if (flowRunModel == null)
            {
                ContentResult result2 = new ContentResult();
                result2.Content = "未找到流程运行时实体!";
                return result2;
            }
            //当前步骤Guid
            Guid stepId = StringExtensions.IsGuid(str2, out guid2) ? guid2 : flowRunModel.FirstStepId;

            if ((flowTaskModel == null) && (flowRunModel.Status != 1))
            {
                string statusTitle = flow.GetStatusTitle(flowRunModel.Status);
                ContentResult result3 = new ContentResult();
                result3.Content="该流程" + statusTitle + ",不能发起新实例!";
                return result3;
            }

            if ((flowTaskModel != null) && (flowTaskModel.ReceiveId != Current.UserId))
            {
                ContentResult result4 = new ContentResult();
                result4.Content="您不能处理当前任务!";
                return result4;
            }


            if ((flowTaskModel != null) && !"1".Equals(str8))
            {
                int[] digits = new int[2];
                digits[1] = 1;
                if (flowTaskModel.Status.NotIn(digits))
                {
                    ContentResult result5 = new ContentResult();
                    result5.Content="该任务" + ((flowTaskModel.Status == -1) ? "等待中" : "已完成") + ",不能处理!";
                    return result5;
                }
            }
            if ((flowTaskModel != null) && (flowTaskModel.TaskType == 5))
            {
                str8 = "1";
            }

          
            bool flag = (flowTaskModel != null) && flowTaskModel.TaskType.In(new int[] { 6, 7, 8 });
            Step step = flowRunModel.Steps.Find(delegate (Step p)
            {
                return p.Id == stepId;
            });
            if (step == null)
            {
                ContentResult result6 = new ContentResult();
                result6.Content = "未找到当前步骤运行时实体!";
                return result6;
            }
            if (((1 == step.StepBase.ConcurrentModel) && (flowTaskModel != null)) && (flowTaskModel.TaskType != 5))
            {
                RoadFlow.Model.FlowTask task3 = task.GetListByGroupId(flowTaskModel.GroupId).Find(delegate (RoadFlow.Model.FlowTask p)
                {
                    return ((p.TaskType != 5) && (p.Status == 1)) && (p.ReceiveId != flowTaskModel.ReceiveId);
                });
                if (task3 != null)
                {
                    string str16 = string.Empty;
                    if (digit == 0)
                    {
                        str16 = "top.mainTab.closeTab();";
                    }
                    else
                    {
                        int[] numArray2 = new int[] { 1, 2 };
                        if (digit.In(numArray2))
                        {
                            str16 = "top.mainDialog.close();";
                        }
                        else if (digit.In(new int[] { 3, 4, 5 }))
                        {
                            str16 = "window.close();";
                        }
                    }
                    ContentResult result = new ContentResult();
                    string[] textArray1 = new string[] { "<script>alert('当前任务正由", task3.ReceiveName, "处理中,请等待!');try{", str16, "}catch(e){}</script>" };
                    result.Content = string.Concat((string[])textArray1);
                    result.ContentType = "text/html";
                    return result;
                }
            }
            if ((flowTaskModel != null) && (flowTaskModel.Status == 0))
            {
                flowTaskModel.Status = 1;
                flowTaskModel.ExecuteType = 1;
                if (!flowTaskModel.OpenTime.HasValue)
                {
                    flowTaskModel.OpenTime = new DateTime?(DateTimeExtensions.Now);
                }
                task.Update(flowTaskModel);
            }
            object[] objArray1 = new object[] {
            "flowid=", str, "&taskid=", str3, "&groupid=", str4, "&stepid=", stepId, "&instanceid=", str5, "&appid=", str6, "&tabid=", str7, "&display=", str8,
            "&showtoolbar=", str9, "&rf_appopenmodel=", ((int) digit).ToString(), "&ismobile=", str10
         };
            string str11 = string.Concat((object[])objArray1);
            List<StepButton> list2 = new List<StepButton>();
            List<RoadFlow.Model.FlowButton> list3 = new List<RoadFlow.Model.FlowButton>();
            FlowButton button = new FlowButton();
            if ("1".Equals(str8))
            {
                if (!"0".Equals(str9))
                {
                    StepButton button1 = new StepButton();
                    button1.Id = StringExtensions.ToGuid("cadd9f81-2b8c-479b-a7f1-3cec775768fa");
                    button1.Sort = 0;
                    list2.Add(button1);
                    StepButton button5 = new StepButton();
                    button5.Id = StringExtensions.ToGuid("d9511329-a03e-4af2-84e5-73beda0d3f42");
                    button5.Sort = 1;
                    list2.Add(button5);
                    if ((flowTaskModel != null) && (flowTaskModel.TaskType == 5))
                    {
                        int[] numArray3 = new int[2];
                        numArray3[1] = 1;
                        if (flowTaskModel.Status.In(numArray3))
                        {
                            StepButton button6 = new StepButton();
                            button6.Id = StringExtensions.ToGuid("aa27eab5-66f0-4cde-958f-1f28da4b4392");
                            button6.Sort = 1;
                            list2.Add(button6);
                        }

                    }
                 }
            }
           
            else
            {
                list2.AddRange((IEnumerable<StepButton>)step.StepButtons);
            }
            if ((flowTaskModel != null) && (flowTaskModel.OtherType == 1))
            {
                StepButton button7 = new StepButton();
                button7.Id = StringExtensions.ToGuid("1673ff03-48c6-465a-9caa-46b776c932a9");
                button7.Sort = list2.Count + 5;
                list2.Add(button7);
            }
            foreach (StepButton button2 in list2)
            {
                if (GuidExtensions.IsEmptyGuid(button2.Id))
                {
                    RoadFlow.Model.FlowButton button8 = new RoadFlow.Model.FlowButton();
                    button8.Id = Guid.Empty;
                    button8.Title = "";
                    RoadFlow.Model.FlowButton button3 = button8;
                    list3.Add(button3);
                }
                else
                {
                    RoadFlow.Model.FlowButton button4 = button.Get(button2.Id).Clone();
                    if (button4 != null)
                    {
                        button4.Title = button2.ShowTitle.IsNullOrWhiteSpace() ? button4.Title : button2.ShowTitle;
                        button4.Sort = button2.Sort;
                        list3.Add(button4);
                    }
                }
            }
            StepForm stepForm = step.StepForm;
           
            string str12 = string.Empty;
            bool flag2 = false;
            if (stepForm != null)
            {
                Guid guid3 = stepForm.Id;
                if ("1".Equals(str10) && GuidExtensions.IsNotEmptyGuid(stepForm.MobileId))
                {
                    guid3 = stepForm.MobileId;
                }
                RoadFlow.Model.AppLibrary library = new AppLibrary().Get(guid3);
                if (library != null)
                {
                    flag2 = library.Code.IsNullOrWhiteSpace();
                    if (flag2)
                    {
                        str12 = new Menu().GetAddress(library.Address, str11, "");
                    }
                    else
                    {
                        str12 = base.Url.Content("~/Areas/RoadFlowCore/Views/FormDesigner/form/" + library.Address);
                        str12 = str12.Substring(0, str12.IndexOf(System.IO.Path.GetExtension(str12))) + ".cshtml";
                    }
                }
            }
            RoadFlow.Model.User user = Current.User;
            if (user == null)
            {
                ContentResult result7 = new ContentResult();
                result7.Content = "未找到当前登录用户!";
                return result7;
            }
            string path = Current.WebRootPath + "/RoadFlowResources/images/userSigns/" + GuidExtensions.ToUpperString(user.Id);
            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                info.Create();
            }
            if (!System.IO.File.Exists(path + "/default.png"))
            {
                new User().CreateSignImage(user.Name).Save(path + "/default.png", ImageFormat.Png);
            }
            string str14 = base.Url.Content("~/RoadFlowResources/images/userSigns/" + GuidExtensions.ToUpperString(user.Id) + "/default.png");
            base.ViewData["tabId"] = str7;
            base.ViewData["appId"] = str6;
            base.ViewData["instanceId"] = str5;
            base.ViewData["display"] =  str8;
            base.ViewData["formUrl"] = str12;
            base.ViewData["query"] = str11;
            base.ViewData["isCustomeForm"] = flag2 ? "1" : "0";
            base.ViewData["isDebug"] = (int)flowRunModel.Debug;
            base.ViewData["signType"] = ((flowTaskModel != null) && (flowTaskModel.TaskType == 5)) ? ((int)0) : ((int)step.SignatureType);
            base.ViewData["attachment"]= (int)step.Attachment;
            base.ViewData["flowType"] = (int)step.StepBase.FlowType;
            base.ViewData["isArchives"] = (int)step.Archives;
            base.ViewData["flowButtons"] = list3;
            base.ViewData["signSrc"] = str14;
            base.ViewData["request"] = base.Request;
            base.ViewData["showComment"] = (int)step.CommentDisplay;
            base.ViewData["commentOptions"] = new FlowComment().GetOptionsByUserId(user.Id);
            base.ViewData["ismobile"] = str10;
            base.ViewData["Title"] = (flowTaskModel != null) ? flowTaskModel.Title : "";
            base.ViewData["comments"]= (flowTaskModel != null) ? flowTaskModel.Comments : string.Empty;
            base.ViewData["uploadAttachment"]= (flowTaskModel != null) ? flowTaskModel.Attachment : string.Empty;

            return this.View();
        }
      

        #region 2.8.8方法
        [Validate(CheckApp = false, CheckUrl = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult Print()
        {
            Guid guid;
            Guid guid2;
            Guid guid4;
            Guid stepId;
            string str = base.Request.Querys("flowid");
            string str2 = base.Request.Querys("stepid");
            string str3 = base.Request.Querys("taskid");
            string str4 = base.Request.Querys("groupid");
            string str5 = base.Request.Querys("instanceid");
            string str6 = base.Request.Querys("appid");
            string str7 = base.Request.Querys("tabid");
            string str8 = base.Request.Querys("showarchive");
            string str9 = base.Request.Querys("ismobile");
            if (str5.IsNullOrWhiteSpace())
            {
                str5 = base.Request.Querys("instanceid1");
            }
            if (!StringExtensions.IsGuid(str, out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="流程ID错误!";
                return result1;
            }
            if (!StringExtensions.IsGuid(str3, out guid2))
            {
                ContentResult result2 = new ContentResult();
                result2.Content="未找到当前任务，请先保存再打印!";
                return result2;
            }
            RoadFlow.Model.FlowRun flowRunModel = new Flow().GetFlowRunModel(guid, true,null);
            if (flowRunModel == null)
            {
                ContentResult result3 = new ContentResult();
                result3.Content="未找到当前流程运行时实体!";
                return result3;
            }
            if (!StringExtensions.IsGuid(str2, out stepId))
            {
                ContentResult result4 = new ContentResult();
                result4.Content="未找到当前步骤，请先保存再打印!";
                return result4;
            }
            Step step = flowRunModel.Steps.Find(delegate (Step p) {
                return p.Id == stepId;
            });
            if (step == null)
            {
                ContentResult result5 = new ContentResult();
                result5.Content="未找到当前步骤运行时实体!";
                return result5;
            }
            object[] objArray1 = new object[] { "flowid=", str, "&taskid=", str3, "&groupid=", str4, "&stepid=", stepId, "&instanceid=", str5, "&appid=", str6, "&tabid=", str7, "&dilplay=1" };
            string str10 = string.Concat((object[])objArray1);
            StepForm stepForm = step.StepForm;
            string str11 = string.Empty;
            bool flag = false;
            if ((stepForm != null) && (stepForm != null))
            {
                Guid guid3 = stepForm.Id;
                if ("1".Equals(str9) && GuidExtensions.IsNotEmptyGuid(stepForm.MobileId))
                {
                    guid3 = stepForm.MobileId;
                }
                RoadFlow.Model.AppLibrary library = new AppLibrary().Get(guid3);
                if (library != null)
                {
                    flag = library.Code.IsNullOrWhiteSpace();
                    if (flag)
                    {
                        str11 = new Menu().GetAddress(library.Address, str10, "");
                    }
                    else
                    {
                        str11 = base.Url.Content("~/Areas/RoadFlowCore/Views/FormDesigner/form/" + library.Address);
                        str11 = str11.Substring(0, str11.IndexOf(System.IO.Path.GetExtension(str11))) + ".cshtml";

                    //    str11 = base.Url.Content("~/wwwroot/RoadFlowResources/scripts/formDesigner/form/" + library.Address);
                    }
                }
            }
            string comments = string.Empty;
            string formHtml = string.Empty;
            string dataJson = string.Empty;
            if ("1".Equals(str8) && StringExtensions.IsGuid(base.Request.Querys("archiveid"), out guid4))
            {
               RoadFlow.Model.FlowArchive archive = new FlowArchive().Get(guid4);
                if (archive != null)
                {
                    comments = archive.Comments;
                    formHtml = archive.FormHtml;
                    dataJson = archive.DataJson;
                }
            }
            base.ViewData["comments"]= comments;
            base.ViewData["formUrl"]= str11;
            base.ViewData["isCustomeForm"]= flag ? "1" : "0";
            base.ViewData["request"]= base.Request;
            base.ViewData["showarchive"]= str8;
            base.ViewData["formHtml"]= formHtml;
            base.ViewData["formData"]= dataJson.IsNullOrWhiteSpace() ? "[]" : dataJson;
            return this.View();
        }




   



        #endregion


        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult SaveData()
        {
            ValueTuple<string, string> tuple1 = new Form().SaveData(base.Request);
            string str = tuple1.Item1;
            string str2 = tuple1.Item2;
            base.ViewData["instanceId"] = str;
            base.ViewData["errMsg"] = str2;
            base.ViewData["opation"] = base.Request.Querys("opation");
            return this.View();
        }


        public string SaveSetNextStepHandler()
        {
            Guid guid;
            string str = base.Request.Forms("steps");
            string str2 = base.Request.Querys("flowid");
            if (!StringExtensions.IsGuid(base.Request.Querys("taskid"), out guid))
            {
                return "未找到当前任务,不能保存!";
            }
            FlowTask task = new FlowTask();
            RoadFlow.Model.FlowTask flowTaskModel = task.Get(guid);
            if (flowTaskModel == null)
            {
                return "未找到当前任务,不能保存!";
            }
            JArray array = new JArray();
            foreach (string str4 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                string str5 = base.Request.Forms("handler_" + str4);
                JObject obj1 = new JObject();
                obj1.Add("stepId", (JToken)str4);
                obj1.Add("handle", (JToken)str5);
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            flowTaskModel.NextStepsHandle = array.ToString(0, Array.Empty<JsonConverter>());
            task.Update(flowTaskModel);
            return "保存成功!";
        }



        public IActionResult SetNextStepHandler()
        {
            Guid guid;
            Guid guid2;
            string str = base.Request.Querys("stepid");
            string str2 = base.Request.Querys("flowid");
            string str3 = base.Request.Querys("taskid");
            string str4 = base.Request.Querys("groupid");
            if (!StringExtensions.IsGuid(str2, out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="流程Id错误!";
                return result1;
            }
            if (!str3.IsGuid())
            {
                ContentResult result2 = new ContentResult();
                result2.Content="请先保存当前任务再指定!";
                return result2;
            }
            Flow flow = new Flow();
            FlowTask task = new FlowTask();
            RoadFlow.Model.FlowRun run = flow.GetFlowRunModel(guid, true, null);
            if (run == null)
            {
                ContentResult result3 = new ContentResult();
                result3.Content="未找到流程运行时!";
                return result3;
            }
            if (!StringExtensions.IsGuid(str, out guid2))
            {
                guid2 = run.FirstStepId;
            }
            List<Step> allNextSteps = flow.GetAllNextSteps(run, guid2);
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["rf_appopenmodel"]= base.Request.Querys("rf_appopenmodel");
            base.ViewData["openerId"]= base.Request.Querys("openerid");
            base.ViewData["nextSteps"]= allNextSteps;
            base.ViewData["nextStepsHandle"]= task.GetNextStepsHandle(task.GetListByGroupId(StringExtensions.ToGuid(str4)));
            return this.View();
        }









        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult ShowDesign()
        {
            Guid guid;
            string str = base.Request.Querys("groupid");
            string str2 = string.Empty;
          
                if (Config.EnableDynamicStep && StringExtensions.IsGuid(str, out guid))
            {
                IOrderedEnumerable<RoadFlow.Model.FlowTask> enumerable = Enumerable.OrderByDescending<RoadFlow.Model.FlowTask, int>(Enumerable.Where<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)new FlowTask().GetListByGroupId(guid),
                   key=> GuidExtensions.IsNotEmptyGuid(key.BeforeStepId)),
                    key=>key.Sort);
                if (Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable))
                {
                    str2 = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable).BeforeStepId.Value.ToString();
                }
            }
            base.ViewData["stepid"]= str2;
            base.ViewData["groupid"]= str2.IsNullOrWhiteSpace() ? "" : str;



            base.ViewData["flowid"] = base.Request.Querys("flowid");
            base.ViewData["tabid"] = base.Request.Querys("tabid");
            base.ViewData["appid"] = base.Request.Querys("appid");
            return this.View();
        }


        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult ShowMainForm()
        {
            FlowTask task = new FlowTask();
            string str = base.Request.Querys("taskid");
            RoadFlow.Model.FlowTask task2 = task.Get(StringExtensions.ToGuid(str));
            if (task2 == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content = "未找到当前任务!";
                return result1;
            }
            List<RoadFlow.Model.FlowTask> listBySubFlowGroupId = task.GetListBySubFlowGroupId(task2.GroupId);
            if (listBySubFlowGroupId.Count == 0)
            {
                ContentResult result2 = new ContentResult();
                result2.Content = "未找到当前任务的主流程任务!";
                return result2;
            }
            RoadFlow.Model.FlowTask task3 = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)listBySubFlowGroupId);
            object[] objArray1 = new object[] { "Index?flowid=", task3.FlowId, "&stepid=", task3.StepId, "&instanceid=", task3.InstanceId, "&taskid=", task3.Id, "&groupid=", task3.GroupId, "&appid=", base.Request.Querys("appid"), "&display=1&showtoolbar=0&tabid=", base.Request.Querys("tabid"), "&ismobile=", base.Request.Querys("ismobile") };
            string str2 = string.Concat((object[])objArray1);
            return this.Redirect(str2);
        }

        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public IActionResult Sign()
        {
            base.ViewData["openerId"] = base.Request.Querys("openerid");
            base.ViewData["queryString"] = base.Request.UrlQuery();
            base.ViewData["buttonid"] = base.Request.Querys("buttonid");
            return this.View();
        }

        [Validate(CheckApp = false, CheckEnterPriseWeiXinLogin = true)]
        public string SignCheck()
        {
            string str = base.Request.Forms("pass");
            if (str.IsNullOrWhiteSpace())
            {
                return "密码不能为空!";
            }
            RoadFlow.Model.User user = Current.User;
            if (user == null)
            {
                return "未找到您的用户信息!";
            }
            if (!user.Password.Equals(new User().GetMD5Password(user.Id, str.Trim())))
            {
                return "密码错误!";
            }
            return "1";
        }

        [Validate(CheckApp = false)]
        public IActionResult Starts()
        {
            List<RoadFlow.Model.FlowRun> startFlows = new Flow().GetStartFlows(Current.UserId);


            return this.View(startFlows);
        }

      

        [CompilerGenerated]
        private sealed class s_c__DisplayClass1_0
        {
            // Fields
            public Execute executeModel;

            // Methods
            internal bool Executeb__0(Step p)
            {
                return (p.Id == this.executeModel.StepId);
            }

            internal bool Executeb__1(RoadFlow.Model.FlowTask p)
            {
                if (p.ReceiveId == this.executeModel.Sender.Id)
                {
                    int[] digits = new int[2];
                    digits[1] = 1;
                    return p.Status.In(digits);
                }
                return false;
            }
        }


       





    }

   



    [CompilerGenerated]
    internal sealed class PrivateImplementationDetails
    {
        // Fields
        //internal static readonly __StaticArrayInitTypeSize=12 47EA6D32BC6D62754EB881E3FE0CC2FCDB3630B3; // data size: 12 bytes
        // internal static readonly __StaticArrayInitTypeSize=12 7E42F36CF90B2C9173D2DDDEF8259D2A78AFFB10; // data size: 12 bytes

        // Methods
        internal static uint ComputeStringHash(string s)
        {
            uint num = 0x811c9dc5;
            if (s != null)
            {
                num = 0x811c9dc5;
                for (int i = 0; i < s.Length; i++)
                {
                    num = (s[i] ^ num) * 0x1000193;
                }
            }
            return num;
        }

        // Nested Types
        [StructLayout(LayoutKind.Explicit, Size = 12, Pack = 1)]
        private struct __StaticArrayInitTypeSize
        {
        }
    }





    #endregion





 


  

   

}
