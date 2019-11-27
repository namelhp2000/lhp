using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business.EnterpriseWeiXin;
using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model.FlowRunModel;
using RoadFlow.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace RoadFlow.Business
{





    #region  新的方法 RoadFlowCore工作流2.8.3更新日志

    public class FlowTask
    {

        /// <summary>
        /// 添加任务
        /// </summary>
        private List<RoadFlow.Model.FlowTask> addTasks;
        /// <summary>
        /// 执行结果
        /// </summary>
        private ExecuteResult executeResult;
        /// <summary>
        /// 执行Sqls字符串
        /// </summary>
        private List<ValueTuple<string, object[], int>> executeSqls;
        /// <summary>
        /// 流程任务Data类
        /// </summary>
        private readonly RoadFlow.Data.FlowTask flowTaskData = new RoadFlow.Data.FlowTask();
        /// <summary>
        /// 锁对象列表
        /// </summary>

        private static List<string> lockObject = new List<string>();

        /// <summary>
        /// 下个流程任务
        /// </summary>
        private List<RoadFlow.Model.FlowTask> nextStepTasks;
        /// <summary>
        /// 移除任务
        /// </summary>
        private List<RoadFlow.Model.FlowTask> removeTasks;
        /// <summary>
        /// 更新任务
        /// </summary>
        private List<RoadFlow.Model.FlowTask> updateTasks;

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="flowTask"></param>
        /// <returns></returns>
        public int Add(RoadFlow.Model.FlowTask flowTask)
        {
            return this.flowTaskData.Add(flowTask);
        }

        /// <summary>
        /// 加签
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        public void AddWrite(RoadFlow.Model.FlowRun flowRunModel, Execute executeModel)
        {
            RoadFlow.Model.FlowTask currentTask;
            JObject obj2 = null;
            try
            {
                obj2 = JObject.Parse(executeModel.ParamsJSON);
            }
            catch
            {
            }
            if (obj2 == null)
            {
                this.executeResult.Messages = "加签参数错误!";
                this.executeResult.DebugMessages = this.executeResult.Messages;
                this.executeResult.IsSuccess = false;
            }
            else
            {
                JObject obj3 = obj2.Value<JObject>("addwrite");
                int num = -2147483648;
                int num2 = -2147483648;
                string str = string.Empty;
                if (obj3 != null)
                {
                    num = obj3.Value<string>("addtype").ToInt(-2147483648);
                    num2 = obj3.Value<string>("writetype").ToInt(-2147483648);
                    str = obj3.Value<string>("member");
                }
                if (((num == -2147483648) || (num2 == -2147483648)) || str.IsNullOrWhiteSpace())
                {
                    this.executeResult.Messages = "加签参数错误!";
                    this.executeResult.DebugMessages = this.executeResult.Messages;
                    this.executeResult.IsSuccess = false;
                }
                else
                {
                    currentTask = this.Get(executeModel.TaskId);
                    if (currentTask == null)
                    {
                        this.executeResult.Messages = "没有找到当前任务";
                        this.executeResult.DebugMessages = this.executeResult.Messages;
                        this.executeResult.IsSuccess = false;
                    }
                    else
                    {
                        List<RoadFlow.Model.User> allUsers = new Organize().GetAllUsers(str);
                        if (!Enumerable.Any<RoadFlow.Model.User>((IEnumerable<RoadFlow.Model.User>)allUsers))
                        {
                            this.executeResult.Messages = "没有接收人";
                            this.executeResult.DebugMessages = this.executeResult.Messages;
                            this.executeResult.IsSuccess = false;
                        }
                        else
                        {
                            FlowEntrust entrust = new FlowEntrust();
                            User user = new User();
                            int num3 = 1;
                            string str2 = currentTask.StepName + "(" + ((num == 1) ? "前加签" : ((num == 2) ? "后加签" : "并签")) + ")";
                            List<RoadFlow.Model.FlowTask> listByGroupId = this.GetListByGroupId(currentTask.GroupId);
                            for (int i = 0; i < allUsers.Count; i++)
                            {
                                RoadFlow.Model.User user4 = allUsers[i];
                                string entrustUserId = entrust.GetEntrustUserId(currentTask.FlowId, user4);
                                if (!entrustUserId.IsNullOrWhiteSpace())
                                {
                                    RoadFlow.Model.User user2 = user.Get(entrustUserId);
                                    if (user2 != null)
                                    {
                                        RoadFlow.Model.User user3 = user2.Clone();
                                        user3.Note = user4.Id.ToString();
                                        user4 = user3;
                                    }
                                }
                                int sort = (num == 3) ? currentTask.Sort : (currentTask.Sort + 1);
                                if (!this.addTasks.Exists(delegate (RoadFlow.Model.FlowTask p)
                                {
                                    return ((p.ReceiveId == user4.Id) && (p.StepId == currentTask.StepId)) && (p.Sort == sort);
                                }) && !listByGroupId.Exists(delegate (RoadFlow.Model.FlowTask p)
                                {
                                    return ((p.ReceiveId == user4.Id) && (p.StepId == currentTask.StepId)) && (p.Status != 2);
                                }))
                                {
                                    Guid guid;
                                    RoadFlow.Model.FlowTask task = new RoadFlow.Model.FlowTask();
                                    if (user4.Note.IsGuid(out guid))
                                    {
                                        task.EntrustUserId = new Guid?(guid);
                                    }
                                    task.ExecuteType = 0;
                                    task.FlowId = currentTask.FlowId;
                                    task.FlowName = currentTask.FlowName;
                                    task.GroupId = currentTask.GroupId;
                                    task.Id = Guid.NewGuid();
                                    task.InstanceId = currentTask.InstanceId;
                                    task.IsAutoSubmit = 0;
                                    task.OtherType = ("1" + ((int)num).ToString() + ((int)num2).ToString()).ToInt(-2147483648);
                                    task.PrevId = currentTask.Id;
                                    task.PrevStepId = currentTask.StepId;
                                    task.ReceiveId = user4.Id;
                                    task.ReceiveName = user4.Name;
                                    task.ReceiveOrganizeId = user4.PartTimeId;
                                    task.ReceiveTime = DateTimeExtensions.Now;
                                    task.SenderId = currentTask.ReceiveId;
                                    task.SenderName = currentTask.ReceiveName;
                                    task.Sort = sort;
                                    task.Status = (num3 == 1) ? 0 : -1;
                                    task.ExecuteType = task.Status;
                                    task.StepId = currentTask.StepId;
                                    task.StepName = str2;
                                    task.StepSort = (num == 3) ? 1 : ((num2 == 3) ? num3++ : 1);
                                    task.TaskType = 5 + num;
                                    task.Title = currentTask.Title;
                                    task.IsBatch = currentTask.IsBatch;
                                    this.addTasks.Add(task);
                                }
                            }
                            currentTask.Status = 2;
                            currentTask.ExecuteType = 13;
                            currentTask.Comments = executeModel.Comment;
                            currentTask.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                            currentTask.IsSign = executeModel.IsSign;
                            this.updateTasks.Add(currentTask);
                            this.executeResult.Messages = "加签成功!";
                            this.executeResult.DebugMessages = this.executeResult.Messages;
                            this.executeResult.IsSuccess = true;
                            this.executeResult.NextTasks = this.addTasks;
                        }
                    }
                }
            }
        }





        /// <summary>
        /// 自动提交
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ExecuteResult AutoSubmit(Guid taskId, string type = "submit", string comment = "")

        {
            ExecuteResult result = new ExecuteResult();
            RoadFlow.Model.FlowTask task = this.Get(taskId);
            if (task == null)
            {
                result.Messages = "当前任务为空!";
                result.DebugMessages = result.Messages;
                result.IsSuccess = false;
                return result;
            }
            Flow flow = new Flow();
            RoadFlow.Model.FlowRun flowRunModel = new Flow().GetFlowRunModel(task.FlowId, true, task);
            if (flowRunModel == null)
            {
                result.Messages = "未找到流程运行时实体!";
                result.DebugMessages = result.Messages;
                result.IsSuccess = false;
                return result;
            }
            JArray array = new JArray();

            switch (type.ToLower())
            {
                case "submit":
                    {
                        ValueTuple<string, string, List<Step>> tuple1 = this.GetNextSteps(flowRunModel, task.StepId, task.GroupId, taskId, task.InstanceId, task.ReceiveId, false, false, null, null);
                        string str3 = tuple1.Item1;
                        string str4 = tuple1.Item2;
                        List<Step> list = tuple1.Item3;
                        foreach (Step step in list)
                        {
                            if (!step.RunDefaultMembers.IsNullOrWhiteSpace() || flow.IsLastStep(flowRunModel, task.StepId))
                            {
                                JObject obj1 = new JObject();
                                obj1.Add("id", (JToken)step.Id);
                                obj1.Add("member", (JToken)step.RunDefaultMembers);
                                obj1.Add("completedtime", (step.WorkTime > decimal.Zero) ? ((JToken)new WorkDate().GetWorkDateTime((double)((double)step.WorkTime), null).ToDateTimeString()) : ((JToken)string.Empty));
                                JObject obj3 = obj1;
                                array.Add(obj3);
                            }
                        }
                        if (((array.Count != 0) || (list.Count <= 0)) || (!type.EqualsIgnoreCase("submit") && !type.EqualsIgnoreCase("back")))
                        {
                            break;
                        }
                        return new ExecuteResult { CurrentTask = task, DebugMessages = "接收步骤没有默认处理人 " + str4, IsSuccess = false, Messages = "接收步骤没有默认处理人", NextTasks = new List<RoadFlow.Model.FlowTask>() };
                    }
                case "back":
                    {
                        ValueTuple<string, string, List<Step>> tuple2 = this.GetBackSteps(task.FlowId, task.StepId, task.GroupId, taskId, task.InstanceId, task.ReceiveId, null);
                        string str5 = tuple2.Item1;
                        string str6 = tuple2.Item2;
                        List<Step> list2 = tuple2.Item3;
                        foreach (Step step2 in list2)
                        {
                            JObject obj5 = new JObject();
                            obj5.Add("id", (JToken)step2.Id);
                            obj5.Add("member", (JToken)step2.RunDefaultMembers);
                            JObject obj6 = obj5;
                            array.Add(obj6);
                        }
                        if (array.Count == 0)
                        {
                            return new ExecuteResult { CurrentTask = task, DebugMessages = str6, IsSuccess = false, Messages = str6, NextTasks = new List<RoadFlow.Model.FlowTask>() };
                        }
                        break;
                    }
            }







            JObject obj4 = new JObject();
            obj4.Add("id", (JToken)taskId.ToString());
            obj4.Add("flowId", (JToken)task.FlowId);
            obj4.Add("instanceId", (JToken)task.InstanceId);
            obj4.Add("title", (JToken)task.Title);
            obj4.Add("comment", (JToken)comment);

            obj4.Add("sign", 0);
            obj4.Add("senderId", (JToken)task.SenderId);
            obj4.Add("type", (JToken)type);
            obj4.Add("note", "");
            obj4.Add("steps", array);
            ValueTuple<Execute, string> executeModel = this.GetExecuteModel(obj4.ToString());
            Execute execute = executeModel.Item1;
            string str = executeModel.Item2;
            if (execute == null)
            {
                result.Messages = str;
                result.DebugMessages = result.Messages;
                result.IsSuccess = false;
                return result;
            }
            if (task.IsAutoSubmit == 1)
            {
                execute.IsAutoSubmit = true;
            }
            RoadFlow.Model.User currentUser = User.CurrentUser;
            if (currentUser != null)
            {
                execute.Sender = currentUser;
            }
            return this.Execute(execute, null);

            //execute.IsAutoSubmit = true;
            //return this.Execute(execute);
        }





        /// <summary>
        /// 自动提交过期任务
        /// </summary>
        public void AutoSubmitExpireTask()
        {
            IEnumerator enumerator = this.flowTaskData.GetExpireTasks().Rows.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Guid guid;
                if (((DataRow)enumerator.Current)["Id"].ToString().IsGuid(out guid))
                {
                    ExecuteResult result = this.AutoSubmit(guid, "submit");
                    if (!result.IsSuccess)
                    {
                        RoadFlow.Model.FlowTask flowTaskModel = this.Get(guid);
                        if (flowTaskModel != null)
                        {
                            flowTaskModel.IsAutoSubmit = 2;
                            this.Update(flowTaskModel);
                        }
                    }
                    Log.Add("超时自动提交了任务-" + guid.ToString(), JsonConvert.SerializeObject(result), LogType.流程运行, "", "", "", "", "", "", "", "");
                }
            }

        }




        /// <summary>
        /// 退回
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        public void Back(RoadFlow.Model.FlowRun flowRunModel, Execute executeModel)
        {
            int maxSort;
            Step currentStep;
            Predicate<RoadFlow.Model.FlowTask> s_9__7 = null;
            List<RoadFlow.Model.FlowTask> listByGroupId = this.GetListByGroupId(executeModel.GroupId);
            RoadFlow.Model.FlowTask currentTask = listByGroupId.Find(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.Id == executeModel.TaskId;
            });
            if (currentTask == null)
            {
                this.executeResult.DebugMessages = "当前任务为空";
                this.executeResult.IsSuccess = false;
                this.executeResult.Messages = "当前任务为空";
            }
            else
            {
                int[] digits = new int[] { -1, 2 };
                if (currentTask.Status.In(digits))
                {
                    this.executeResult.DebugMessages = "当前任务" + ((currentTask.Status == -1) ? "等待中" : "已处理") + "!";
                    this.executeResult.IsSuccess = false;
                    this.executeResult.Messages = this.executeResult.DebugMessages;
                }
                else if (currentTask.ReceiveId != executeModel.Sender.Id)
                {
                    this.executeResult.DebugMessages = "当前任务接收人不属于当前用户";
                    this.executeResult.IsSuccess = false;
                    this.executeResult.Messages = "您不能处理当前任务";
                }
                else
                {
                    currentStep = flowRunModel.Steps.Find(delegate (Step p)
                    {
                        return p.Id == currentTask.StepId;
                    });
                    if (currentStep == null)
                    {
                        this.executeResult.DebugMessages = "未找到当前步骤运行时";
                        this.executeResult.IsSuccess = false;
                        this.executeResult.Messages = "未找到当前步骤运行时";
                    }
                    else
                    {
                        List<RoadFlow.Model.FlowTask>.Enumerator enumerator;
                        DateTime? nullable;
                        this.executeResult.CurrentTask = currentTask;
                        if (executeModel.Title.IsNullOrWhiteSpace())
                        {
                            executeModel.Title = currentTask.Title;
                        }
                        if (currentTask.OtherType == 1)
                        {
                            executeModel.OtherType = 1;
                        }
                        bool flag = true;
                        int backModel = currentStep.StepBase.BackModel;
                        maxSort = Enumerable.Max<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId.FindAll(delegate (RoadFlow.Model.FlowTask p)
                        {
                            return p.StepId == currentTask.StepId;
                        }), key=>key.Sort);
                        List<RoadFlow.Model.FlowTask> list2 = listByGroupId.FindAll(delegate (RoadFlow.Model.FlowTask p)
                        {
                            return ((p.StepId == currentTask.StepId) && (p.Sort == maxSort)) && (p.TaskType != 5);
                        });
                        if (backModel == 1)
                        {
                            switch (currentStep.StepBase.HanlderModel)
                            {
                                case 0:
                                    backModel = 2;
                                    break;

                                case 1:
                                    backModel = 3;
                                    break;

                                case 2:
                                    backModel = 5;
                                    break;

                                case 3:
                                    backModel = 4;
                                    break;

                                case 4:
                                    backModel = 6;
                                    break;
                            }
                        }
                        int[] numArray2 = new int[] { 6, 7 };
                        if (currentTask.TaskType.In(numArray2))
                        {
                            char[] chArray = ((int)currentTask.OtherType).ToString().ToCharArray();
                            if (chArray.Length == 3)
                            {
                                switch (((char)chArray[2]).ToString().ToInt(-2147483648))
                                {
                                    case 1:
                                        backModel = 2;
                                        break;

                                    case 2:
                                        backModel = 3;
                                        break;

                                    case 3:
                                        backModel = 2;
                                        break;
                                }
                            }
                        }
                        switch (backModel)
                        {
                            case 0:
                                this.executeResult.DebugMessages = "当前步骤设置为不能退回";
                                this.executeResult.IsSuccess = false;
                                this.executeResult.Messages = "当前步骤设置为不能退回";
                                return;

                            case 2:
                                foreach (RoadFlow.Model.FlowTask task2 in list2)
                                {
                                    if ((task2.Id != currentTask.Id) && (task2.Status != 2))
                                    {
                                        RoadFlow.Model.FlowTask task3 = task2.Clone();
                                        task3.ExecuteType = 5;
                                        task3.Status = 2;
                                        task3.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                                        this.updateTasks.Add(task3);
                                    }
                                }
                                break;

                            case 3:
                                flag = !list2.Exists(delegate (RoadFlow.Model.FlowTask p)
                                {
                                    return (p.Status != 2) && (p.Id != currentTask.Id);
                                });
                                break;

                            case 5:
                                {
                                    decimal percentage = currentStep.StepBase.Percentage;
                                    flag = (((Enumerable.Count<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list2,
                                       key=> ((key.Status == 2) && (key.ExecuteType == 3))) + 1) / list2.Count) * 100) >= percentage;
                                    if (!flag)
                                    {
                                        break;
                                    }
                                    using (enumerator = list2.FindAll(s_9__7 ?? (s_9__7 = delegate (RoadFlow.Model.FlowTask p)
                                    {
                                        return (p.Status != 2) && (p.Id != currentTask.Id);
                                    })).GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            RoadFlow.Model.FlowTask task4 = enumerator.Current.Clone();
                                            task4.ExecuteType = 5;
                                            task4.Status = 2;
                                            task4.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                                            this.updateTasks.Add(task4);
                                        }
                                        break;
                                    }
                                }
                            case 6:
                                if (currentTask.StepSort != 1)
                                {
                                    RoadFlow.Model.FlowTask task5 = list2.Find(delegate (RoadFlow.Model.FlowTask p)
                                    {
                                        return p.StepSort == (currentTask.StepSort - 1);
                                    });
                                    if (task5 != null)
                                    {
                                        RoadFlow.Model.FlowTask task6 = task5.Clone();
                                        task6.ExecuteType = 0;
                                        task6.Status = 0;
                                        nullable = null;
                                        task6.CompletedTime1 = nullable;
                                        task6.Note = currentTask.ReceiveName + "退回";
                                        task6.Comments = "";
                                        task6.IsSign = 0;
                                        nullable = null;
                                        task6.OpenTime = nullable;
                                        this.updateTasks.Add(task6);
                                        this.executeResult.NextTasks.Add(task6);
                                    }
                                    flag = false;
                                }
                                break;
                        }
                        RoadFlow.Model.FlowTask task = currentTask.Clone();
                        task.ExecuteType = 3;
                        task.Status = 2;
                        task.Comments = executeModel.Comment;
                        task.IsSign = executeModel.IsSign;
                        task.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                        task.Attachment = executeModel.Attachment;
                        this.updateTasks.Add(task);
                        if (flag)
                        {
                            if ((currentStep.StepBase.HanlderModel == 4) && (currentTask.StepSort == 1))
                            {
                                List<RoadFlow.Model.FlowTask> list3 = listByGroupId.FindAll(delegate (RoadFlow.Model.FlowTask p)
                                {
                                    return p.StepId == currentStep.Id;
                                });
                                if (list3.Count > 0)
                                {
                                    int currentMaxSort = Enumerable.Max<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list3, 
                                       key=>key.Sort);


                                    using (enumerator = list3.FindAll(delegate (RoadFlow.Model.FlowTask p)
                                    {
                                        return ((p.Sort == currentMaxSort) && (p.Id != currentTask.Id)) && (p.Status < 2);
                                    }).GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            RoadFlow.Model.FlowTask task7 = enumerator.Current.Clone();
                                            this.removeTasks.Add(task7);
                                        }
                                    }
                                }
                            }
                            using (List<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>.Enumerator enumerator2 = executeModel.Steps.GetEnumerator())
                            {
                                while (enumerator2.MoveNext())
                                {
                                    s_c__DisplayClass42_2 class_3 = new s_c__DisplayClass42_2();
                                    ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?> tuple = enumerator2.Current;


                                    Guid stepId = tuple.Item1;
                                    class_3.stepId = stepId;
                                    List<RoadFlow.Model.User> list4 = tuple.Item5;
                                    DateTime? nullable2 = tuple.Item6;
                                    Step nextStep = flowRunModel.Steps.Find(new Predicate<Step>(class_3.Backb__12));
                                    if (nextStep != null)
                                    {
                                        if (nextStep.StepBase.HanlderModel == 4)
                                        {
                                            List<RoadFlow.Model.FlowTask> list5 = listByGroupId.FindAll(delegate (RoadFlow.Model.FlowTask p)
                                            {
                                                return (p.StepId == nextStep.Id) && (p.TaskType != 5);
                                            });
                                            if (list5.Count > 0)
                                            {
                                                int maxSort1 = Enumerable.Max<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list5,
                                                    key=>key.Sort);

                                                IOrderedEnumerable<RoadFlow.Model.FlowTask> enumerable = Enumerable.OrderByDescending<RoadFlow.Model.FlowTask, int>((IEnumerable<RoadFlow.Model.FlowTask>)list5.FindAll(delegate (RoadFlow.Model.FlowTask p)
                                                {
                                                    return p.Sort == maxSort1;
                                                }), key=> key.StepSort);
                                                foreach (RoadFlow.Model.FlowTask task8 in enumerable)
                                                {
                                                    RoadFlow.Model.FlowTask task9 = task8.Clone();
                                                    task9.Id = Guid.NewGuid();
                                                    task9.Title = executeModel.Title;
                                                    task9.ExecuteType = 0;
                                                    task9.Status = (task8.Id == Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable).Id) ? 0 : -1;
                                                    task9.ExecuteType = task9.Status;
                                                    nullable = null;
                                                    task9.CompletedTime1 = nullable;
                                                    task9.Comments = "";
                                                    task9.IsSign = 0;
                                                    nullable = null;
                                                    task9.OpenTime = nullable;
                                                    task9.Note = "退回任务";
                                                    task9.Sort = currentTask.Sort + 1;
                                                    task9.SenderId = currentTask.ReceiveId;
                                                    task9.SenderName = currentTask.ReceiveName;
                                                    task9.ReceiveTime = DateTimeExtensions.Now;
                                                    task9.PrevId = currentTask.Id;
                                                    task9.PrevStepId = currentTask.StepId;
                                                    task9.OtherType = executeModel.OtherType;
                                                    task9.SubFlowGroupId = task8.SubFlowGroupId;
                                                    task9.TaskType = 4;
                                                    task9.CompletedTime = nullable;
                                                    nullable = null;
                                                    task9.RemindTime = nullable;
                                                    this.addTasks.Add(task9);
                                                    this.executeResult.NextTasks.Add(task9);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int num6 = 1;
                                            using (List<RoadFlow.Model.User>.Enumerator enumerator4 = list4.GetEnumerator())
                                            {
                                                while (enumerator4.MoveNext())
                                                {
                                                    RoadFlow.Model.User receiveUser = enumerator4.Current;
                                                    if (!this.addTasks.Exists(delegate (RoadFlow.Model.FlowTask p)
                                                    {
                                                        return (p.ReceiveId == receiveUser.Id) && (p.StepId == nextStep.Id);
                                                    }) && !listByGroupId.Exists(delegate (RoadFlow.Model.FlowTask p)
                                                    {
                                                        return ((p.ReceiveId == receiveUser.Id) && (p.StepId == nextStep.Id)) && (p.Status != 2);
                                                    }))
                                                    {
                                                        Guid guid;

                                                        Guid? partTimeId;


                                                        RoadFlow.Model.FlowTask task10 = listByGroupId.Find(delegate (RoadFlow.Model.FlowTask p)
                                                        {
                                                            return ((p.StepId == stepId) && (p.ReceiveId == receiveUser.Id)) && (p.Sort == (currentTask.Sort - 1));
                                                        });
                                                        string str = (task10 != null) ? task10.SubFlowGroupId : string.Empty;

                                                        DateTime? dt = nullable2;
                                                        DateTime? nullable4 = null;
                                                        WorkDate date = new WorkDate();
                                                        if (!dt.HasValue && (nextStep.WorkTime > decimal.Zero))
                                                        {
                                                            nullable = null;
                                                            dt = new DateTime?(date.GetWorkDateTime((double)((double)nextStep.WorkTime), nullable));
                                                        }
                                                        if (dt.HasValue && (nextStep.ExpiredPrompt > 0))
                                                        {
                                                            nullable4 = new DateTime?(date.GetWorkDateTime((double)(nextStep.ExpiredPromptDays - (nextStep.ExpiredPromptDays * 2M)), dt));
                                                        }




                                                        RoadFlow.Model.FlowTask task11 = new RoadFlow.Model.FlowTask
                                                        {
                                                            ExecuteType = 0,
                                                            FlowId = executeModel.FlowId,
                                                            FlowName = flowRunModel.Name,
                                                            GroupId = currentTask.GroupId,
                                                            Id = Guid.NewGuid(),
                                                            InstanceId = (task10 != null) ? task10.InstanceId : executeModel.InstanceId,


                                                            IsSign = 0,
                                                            OtherType = executeModel.OtherType,
                                                            Note = "退回",
                                                            PrevId = currentTask.Id,
                                                            PrevStepId = currentTask.StepId,
                                                            ReceiveId = receiveUser.Id,
                                                            ReceiveName = receiveUser.Name,
                                                            ReceiveTime = DateTimeExtensions.Now,
                                                            SenderId = currentTask.ReceiveId,
                                                            SenderName = currentTask.ReceiveName,
                                                            Sort = currentTask.Sort + 1,
                                                            Status = 0,
                                                            StepId = stepId,
                                                            StepName = nextStep.Name,
                                                            StepSort = num6,
                                                            TaskType = 4,
                                                            Title = executeModel.Title,
                                                            IsAutoSubmit = nextStep.ExpiredExecuteModel,
                                                            SubFlowGroupId = str,
                                                            BeforeStepId = (task10 != null) ? task10.BeforeStepId : (partTimeId = null),
                                                            CompletedTime = dt,
                                                            RemindTime = nullable4,
                                                            IsBatch = new int?(nextStep.BatchExecute)

                                                        };

                                                        partTimeId = receiveUser.PartTimeId;

                                                        if (partTimeId.HasValue)
                                                        {
                                                            task11.ReceiveOrganizeId = receiveUser.PartTimeId;
                                                        }


                                                        if (receiveUser.Note.IsGuid(out guid))
                                                        {
                                                            task11.EntrustUserId = new Guid?(guid);
                                                            task11.Note = "委托";
                                                        }
                                                        this.addTasks.Add(task11);
                                                        this.executeResult.NextTasks.Add(task11);
                                                    }
                                                }
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        this.executeResult.IsSuccess = true;
                    }
                }
            }
        }


        /// <summary>
        /// 前加签
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        /// <param name="currentTask"></param>
        public void BeforAddWrite(RoadFlow.Model.FlowRun flowRunModel, Execute executeModel, RoadFlow.Model.FlowTask currentTask)
        {
            char[] chArray = ((int)currentTask.OtherType).ToString().ToCharArray();
            if (chArray.Length != 3)
            {
                this.executeResult.DebugMessages = "加签参数错误!";
                this.executeResult.IsSuccess = false;
                this.executeResult.Messages = "加签参数错误!";
            }
            else
            {
                int num = ((char)chArray[1]).ToString().ToInt(-2147483648);
                int num2 = ((char)chArray[2]).ToString().ToInt(-2147483648);
                bool flag = false;
                List<RoadFlow.Model.FlowTask> list = this.GetListByGroupId(currentTask.GroupId).FindAll(delegate (RoadFlow.Model.FlowTask p)
                {
                    return (p.Sort == currentTask.Sort) && (p.Id != currentTask.Id);
                });
                switch (num2)
                {
                    case 1:
                        flag = !list.Exists(key=> (key.Status != 2));
                        break;

                    case 2:
                        flag = true;
                        foreach (RoadFlow.Model.FlowTask task2 in list)
                        {
                            if (task2.Status != 2)
                            {
                                task2.Status = 2;
                                task2.ExecuteType = 4;
                                this.updateTasks.Add(task2);
                            }
                        }
                        break;

                    case 3:
                        {
                            RoadFlow.Model.FlowTask task = list.Find(delegate (RoadFlow.Model.FlowTask p)
                            {
                                return p.StepSort == (currentTask.StepSort + 1);
                            });
                            if (task != null)
                            {
                                task.Status = 0;
                                task.ExecuteType = 0;
                                this.updateTasks.Add(task);
                                List<RoadFlow.Model.FlowTask> list1 = new List<RoadFlow.Model.FlowTask> {
                            task
                        };
                                this.executeResult.NextTasks = list1;
                            }
                            else
                            {
                                flag = true;
                            }
                            break;
                        }
                }
                currentTask.Status = 2;
                currentTask.ExecuteType = 2;
                currentTask.IsSign = executeModel.IsSign;
                currentTask.Comments = executeModel.Comment;
                currentTask.ExecuteType = 2;
                currentTask.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                this.updateTasks.Add(currentTask);
                this.executeResult.IsSuccess = true;
                if (flag && (num == 1))
                {
                    RoadFlow.Model.FlowTask task3 = this.Get(currentTask.PrevId);
                    if (task3 != null)
                    {
                        RoadFlow.Model.FlowTask task4 = task3.Clone();
                        task4.Id = Guid.NewGuid();
                        task4.PrevId = currentTask.Id;
                        task4.Status = 0;
                        task4.ExecuteType = 0;
                        task4.IsSign = 0;
                        task4.Comments = null;
                        task4.CompletedTime1 = null;
                        task4.OpenTime = null;
                        this.addTasks.Add(task4);
                        List<RoadFlow.Model.FlowTask> list2 = new List<RoadFlow.Model.FlowTask> {
                        task4
                    };
                        this.executeResult.NextTasks = list2;
                    }
                }
            }
        }



        /// <summary>
        /// 抄送
        /// </summary>
        /// <param name="currentTask"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        public string CopyFor(RoadFlow.Model.FlowTask currentTask, List<RoadFlow.Model.User> users)
        {
            if ((currentTask == null) || (users == null))
            {
                return "当前任务或当前用户为空!";
            }
            List<RoadFlow.Model.FlowTask> addTasks = new List<RoadFlow.Model.FlowTask>();
            List<RoadFlow.Model.FlowTask> listByGroupId = this.GetListByGroupId(currentTask.GroupId);
            using (List<RoadFlow.Model.User>.Enumerator enumerator = users.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    RoadFlow.Model.User user = enumerator.Current;
                    if (!addTasks.Exists(delegate (RoadFlow.Model.FlowTask p)
                    {
                        return (p.ReceiveId == user.Id) && (p.StepId == currentTask.StepId);
                    }) && !listByGroupId.Exists(delegate (RoadFlow.Model.FlowTask p)
                    {
                        return ((p.ReceiveId == user.Id) && (p.StepId == currentTask.StepId)) && (p.Status != 2);
                    }))
                    {
                        RoadFlow.Model.FlowTask task = currentTask.Clone();
                        task.Id = Guid.NewGuid();
                        task.ReceiveId = user.Id;
                        task.ReceiveName = user.Name;
                        task.ReceiveTime = DateTimeExtensions.Now;
                        task.SenderId = currentTask.ReceiveId;
                        task.SenderName = currentTask.ReceiveName;
                        DateTime? nullable = null;
                        task.CompletedTime1 = nullable;
                        nullable = null;
                        task.CompletedTime = nullable;
                        task.Comments = "";
                        task.ExecuteType = 0;
                        task.Status = 0;
                        task.TaskType = 5;
                        task.Note = "抄送任务";
                        task.IsSign = 0;
                        if (user.PartTimeId.HasValue)
                        {
                            task.ReceiveOrganizeId = user.PartTimeId;
                        }
                        addTasks.Add(task);
                    }
                }
            }
            if (this.Update(null, null, addTasks, null) <= 0)
            {
                return "没有抄送给任何人员!";
            }
            return "1";
        }


        /// <summary>
        /// 完成后抄送
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        public void CopyForCompleted(RoadFlow.Model.FlowRun flowRunModel, Execute executeModel)
        {
            RoadFlow.Model.FlowTask task = this.Get(executeModel.TaskId);
            if (task == null)
            {
                this.executeResult.DebugMessages = "当前任务为空";
                this.executeResult.IsSuccess = false;
                this.executeResult.Messages = "当前任务为空";
            }
            else
            {
                int[] digits = new int[] { -1, 2 };
                if (task.Status.In(digits))
                {
                    this.executeResult.DebugMessages = "当前任务" + ((task.Status == -1) ? "等待中" : "已处理") + "!";
                    this.executeResult.IsSuccess = false;
                    this.executeResult.Messages = this.executeResult.DebugMessages;
                }
                else
                {
                    task.ExecuteType = 8;
                    task.Status = 2;
                    task.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                    this.updateTasks.Add(task);
                    this.executeResult.IsSuccess = true;
                }
            }
        }

        /// <summary>
        /// 通过流程id删除
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public int DeleteByFlowId(Guid flowId)
        {
            return this.flowTaskData.DeleteByFlowId(flowId);
        }

        /// <summary>
        /// 批量删除组
        /// </summary>
        /// <param name="flowTaskModels"></param>
        /// <returns></returns>
        public int DeleteByGroupId(RoadFlow.Model.FlowTask[] flowTaskModels)
        {
            return this.flowTaskData.DeleteByGroupId(Enumerable.First<RoadFlow.Model.FlowTask>(flowTaskModels).GroupId);
        }

        /// <summary>
        /// 通过组id删除
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public int DeleteByGroupId(Guid groupId)
        {
            return this.flowTaskData.DeleteByGroupId(groupId);
        }

        /// <summary>
        /// 指派
        /// </summary>
        /// <param name="currentTask"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        public string Designate(RoadFlow.Model.FlowTask currentTask, List<RoadFlow.Model.User> users)
        {
            currentTask.ExecuteType = 9;
            currentTask.Status = 2;
            currentTask.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
            List<RoadFlow.Model.FlowTask> updateTasks = new List<RoadFlow.Model.FlowTask> {
            currentTask
        };
            List<RoadFlow.Model.FlowTask> listByGroupId = this.GetListByGroupId(currentTask.GroupId);
            List<RoadFlow.Model.FlowTask> nextTasks = new List<RoadFlow.Model.FlowTask>();
            FlowEntrust entrust = new FlowEntrust();
            User user = new User();
            for (int i = 0; i < users.Count; i++)
            {
                RoadFlow.Model.User userModel = users[i];
                string entrustUserId = entrust.GetEntrustUserId(currentTask.FlowId, userModel);
                bool flag = !entrustUserId.IsNullOrWhiteSpace();
                if (flag)
                {
                    RoadFlow.Model.User user2 = user.Get(entrustUserId);
                    if (user2 != null)
                    {
                        RoadFlow.Model.User user3 = user2.Clone();
                        user3.Note = userModel.Id.ToString();
                        userModel = user3;
                    }
                }
                if (!nextTasks.Exists(delegate (RoadFlow.Model.FlowTask p)
                {
                    return (p.ReceiveId == userModel.Id) && (p.StepId == currentTask.StepId);
                }) && !listByGroupId.Exists(delegate (RoadFlow.Model.FlowTask p)
                {
                    return (((p.ReceiveId == userModel.Id) && (p.Id != currentTask.Id)) && (p.StepId == currentTask.StepId)) && (p.Status != 2);
                }))
                {
                    Guid guid;
                    RoadFlow.Model.FlowTask task = currentTask.Clone();
                    task.Id = Guid.NewGuid();
                    task.ReceiveId = userModel.Id;
                    task.ReceiveName = userModel.Name;
                    if (userModel.PartTimeId.HasValue)
                    {
                        task.ReceiveOrganizeId = new Guid?(userModel.PartTimeId.Value);
                    }
                    task.ReceiveTime = DateTimeExtensions.Now;
                    task.Status = 0;
                    task.ExecuteType = 0;
                    task.TaskType = 1;
                    task.Note = "指派任务";
                    task.Comments = "";
                    task.IsSign = 0;
                    if (userModel.PartTimeId.HasValue)
                    {
                        task.ReceiveOrganizeId = new Guid?(userModel.PartTimeId.Value);
                    }
                    if (flag && userModel.Note.IsGuid(out guid))
                    {
                        task.EntrustUserId = new Guid?(guid);
                    }
                    nextTasks.Add(task);
                }
            }
            if (!Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)nextTasks))
            {
                updateTasks.Clear();
            }
            this.SendMessage(nextTasks, User.CurrentUser, "", "");
            if (this.Update(null, updateTasks, nextTasks, null) <= 0)
            {
                return "没有指派给任何人员!";
            }
            return "1";
        }





        /// <summary>
        /// 执行***************核心***************
        /// </summary>
        /// <param name="executeModel"></param>
        /// <returns></returns>
        public ExecuteResult Execute(Execute executeModel, RoadFlow.Model.FlowRun flowRunModel = null)
        {

            this.executeResult = new ExecuteResult();
            this.nextStepTasks = new List<RoadFlow.Model.FlowTask>();
            this.addTasks = new List<RoadFlow.Model.FlowTask>();
            this.updateTasks = new List<RoadFlow.Model.FlowTask>();
            this.removeTasks = new List<RoadFlow.Model.FlowTask>();
            this.executeSqls = new List<ValueTuple<string, object[], int>>();


            if (flowRunModel == null)
            {
                RoadFlow.Model.FlowTask currentTask = null;
                if (Config.EnableDynamicStep && executeModel.GroupId.IsNotEmptyGuid())
                {
                    IEnumerable<RoadFlow.Model.FlowTask> enumerable = Enumerable.Where<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)Enumerable.OrderByDescending<RoadFlow.Model.FlowTask, int>((IEnumerable<RoadFlow.Model.FlowTask>)this.GetListByGroupId(executeModel.GroupId),
                       key=>key.Sort), 
                        key=> key.BeforeStepId.IsNotEmptyGuid());

                    if (Enumerable.Any<RoadFlow.Model.FlowTask>(enumerable))
                    {
                        currentTask = Enumerable.First<RoadFlow.Model.FlowTask>(enumerable);
                    }
                }
                flowRunModel = new Flow().GetFlowRunModel(executeModel.FlowId, true, currentTask);
            }

            if (flowRunModel == null)
            {
                this.executeResult.DebugMessages = "未找到流程运行时!";
                this.executeResult.IsSuccess = false;
                this.executeResult.Messages = "未找到流程运行时!";
                return this.executeResult;
            }
            Step currentStepModel = flowRunModel.Steps.Find(delegate (Step p)
            {
                return p.Id == executeModel.StepId;
            });
            if ((currentStepModel == null) && (executeModel.ExecuteType != OperationType.AddWrite))
            {
                this.executeResult.DebugMessages = "未找到步骤运行时!";
                this.executeResult.IsSuccess = false;
                this.executeResult.Messages = "未找到步骤运行时!";
                return this.executeResult;
            }

            EventParam param = new EventParam
            {
                FlowId = executeModel.FlowId,
                GroupId = executeModel.GroupId,
                InstanceId = executeModel.InstanceId,
                StepId = executeModel.StepId,
                TaskId = executeModel.TaskId,
                TaskTitle = executeModel.Title,
                FlowRunModel = flowRunModel
            };
            if (!lockObject.ContainsIgnoreCase(executeModel.GroupId.ToString()))
            {
                lockObject.Add(executeModel.GroupId.ToString());
            }
            string str = Enumerable.First<string>((IEnumerable<string>)lockObject, delegate (string p)
            {
                return p.EqualsIgnoreCase(executeModel.GroupId.ToString());
            });



            //  FlowTaskLock lock2 = @lock;
            lock (str)
            {
                object[] objArray4;
                int num;
                string str2;
                string str5;
                switch (executeModel.ExecuteType)
                {
                    case OperationType.Submit:
                    case OperationType.FreeSubmit:
                    case OperationType.Completed:
                        if (!currentStepModel.StepEvent.SubmitBefore.IsNullOrWhiteSpace())
                        {
                            str2 = currentStepModel.StepEvent.SubmitBefore.Trim();
                            if (!str2.StartsWith("[sql]"))
                            {
                                break;
                            }
                            this.executeSqls.Add(new ValueTuple<string, object[], int>(Wildcard.Filter(str2, null, null).TrimStart("[sql]".ToCharArray()), null, 0));
                        }
                        goto Label_048F;

                    case OperationType.Save:
                        this.Save(flowRunModel, executeModel);
                        goto Label_09DA;


                    case OperationType.Back:
                        if (!currentStepModel.StepEvent.BackBefore.IsNullOrWhiteSpace())
                        {
                            str5 = currentStepModel.StepEvent.BackBefore.Trim();
                            if (!str5.StartsWith("[sql]"))
                            {
                                goto Label_06EB;
                            }
                            this.executeSqls.Add(new ValueTuple<string, object[], int>(Wildcard.Filter(str5, null, null).TrimStart("[sql]".ToCharArray()), null, 0));
                        }
                        goto Label_083C;


                    case OperationType.Redirect:
                        this.Redirect(flowRunModel, executeModel);
                        goto Label_09DA;


                    case OperationType.AddWrite:
                        this.AddWrite(flowRunModel, executeModel);
                        goto Label_09DA;


                    case OperationType.CopyforCompleted:
                        this.CopyForCompleted(flowRunModel, executeModel);
                        goto Label_09DA;


                    case OperationType.TaskEnd:
                        this.TaskEnd(flowRunModel, executeModel);
                        goto Label_09DA;


                    default:
                        goto Label_09DA;

                }
                object[] args = new object[] { param };
                ValueTuple<object, Exception> tuple1 = Tools.ExecuteMethod(str2, args);
                object obj2 = tuple1.Item1;
                Exception exception = tuple1.Item2;
                string str3 = string.Empty;
                if (obj2 != null)
                {
                    str3 = obj2.ToString();
                }
                if (exception != null)
                {
                    string[] textArray1 = new string[] { "执行提交前事件发生了错误-", flowRunModel.Name, "-", currentStepModel.Name, "-", currentStepModel.StepEvent.SubmitBefore };
                    string[] textArray2 = new string[] { "参数：", param.ToString(), " 错误：", exception.Message, exception.StackTrace };
                    Log.Add(string.Concat((string[])textArray1), string.Concat((string[])textArray2), LogType.流程运行, "", "", "", "", "", "", "", "");
                }
                if ((!str3.IsNullOrWhiteSpace() && !"1".Equals(str3)) && !"true".EqualsIgnoreCase(str3))
                {
                    if ("false".EqualsIgnoreCase(str3))
                    {
                        str3 = "不能提交!";
                    }
                    this.executeResult.DebugMessages = str3;
                    this.executeResult.IsSuccess = false;
                    this.executeResult.Messages = str3;
                    return this.executeResult;
                }
            Label_048F:
                this.Submit(flowRunModel, executeModel);
                if (!currentStepModel.StepEvent.SubmitAfter.IsNullOrWhiteSpace())
                {
                    if (this.executeResult.CurrentTask != null)
                    {
                        param.TaskId = this.executeResult.CurrentTask.Id;
                        param.GroupId = this.executeResult.CurrentTask.GroupId;
                        param.TaskTitle = this.executeResult.CurrentTask.Title;
                        if (param.InstanceId.IsNullOrWhiteSpace())
                        {
                            param.InstanceId = this.executeResult.CurrentTask.InstanceId;
                        }
                    }
                    string str4 = currentStepModel.StepEvent.SubmitAfter.Trim();
                    if (str4.StartsWith("[sql]"))
                    {
                        object[] objArray2 = new object[] { param.TaskId, param.InstanceId, param.GroupId };
                        object[] objArray = objArray2;
                        this.executeSqls.Add(new ValueTuple<string, object[], int>(Wildcard.Filter(str4, null, null).TrimStart("[sql]".ToCharArray()), objArray, 1));
                    }
                    else
                    {
                        object[] objArray3 = new object[] { param };
                        ValueTuple<object, Exception> tuple2 = Tools.ExecuteMethod(str4, objArray3);
                        object obj3 = tuple2.Item1;
                        Exception exception2 = tuple2.Item2;
                        if (obj3 != null)
                        {
                            obj3.ToString();
                        }
                        if (exception2 != null)
                        {
                            string[] textArray3 = new string[] { "执行退回前事件发生了错误-", flowRunModel.Name, "-", currentStepModel.Name, "-", currentStepModel.StepEvent.SubmitBefore };
                            string[] textArray4 = new string[] { "参数：", param.ToString(), " 错误：", exception2.Message, exception2.StackTrace };
                            Log.Add(string.Concat((string[])textArray3), string.Concat((string[])textArray4), LogType.流程运行, "", "", "", "", "", "", "", "");
                        }
                    }
                }
                goto Label_09DA;
            Label_06EB:
                objArray4 = new object[] { param };
                ValueTuple<object, Exception> tuple3 = Tools.ExecuteMethod(str5, objArray4);
                object obj4 = tuple3.Item1;
                Exception exception3 = tuple3.Item2;
                string str6 = string.Empty;
                if (obj4 != null)
                {
                    str6 = obj4.ToString();
                }
                if (exception3 != null)
                {
                    string[] textArray5 = new string[] { "执行退回前事件发生了错误-", flowRunModel.Name, "-", currentStepModel.Name, "-", currentStepModel.StepEvent.SubmitBefore };
                    string[] textArray6 = new string[] { "参数：", param.ToString(), " 错误：", exception3.Message, exception3.StackTrace };
                    Log.Add(string.Concat((string[])textArray5), string.Concat((string[])textArray6), LogType.流程运行, "", "", "", "", "", "", "", "");
                }
                if ((!str6.IsNullOrWhiteSpace() && !"1".Equals(str6)) && !"true".EqualsIgnoreCase(str6))
                {
                    if ("false".EqualsIgnoreCase(str6))
                    {
                        str6 = "不能退回!";
                    }
                    this.executeResult.DebugMessages = str6;
                    this.executeResult.IsSuccess = false;
                    this.executeResult.Messages = str6;
                    return this.executeResult;
                }
            Label_083C:
                this.Back(flowRunModel, executeModel);
                if (!currentStepModel.StepEvent.BackAfter.IsNullOrWhiteSpace())
                {
                    string str7 = currentStepModel.StepEvent.BackAfter.Trim();
                    if (str7.StartsWith("[sql]"))
                    {
                        this.executeSqls.Add(new ValueTuple<string, object[], int>(Wildcard.Filter(str7, null, null).TrimStart("[sql]".ToCharArray()), null, 1));
                    }
                    else
                    {
                        object[] objArray5 = new object[] { param };
                        ValueTuple<object, Exception> tuple4 = Tools.ExecuteMethod(str7, objArray5);
                        object obj5 = tuple4.Item1;
                        Exception exception4 = tuple4.Item2;
                        if (obj5 != null)
                        {
                            obj5.ToString();
                        }
                        if (exception4 != null)
                        {
                            string[] textArray7 = new string[] { "执行退回后事件发生了错误-", flowRunModel.Name, "-", currentStepModel.Name, "-", currentStepModel.StepEvent.SubmitBefore };
                            string[] textArray8 = new string[] { "参数：", param.ToString(), " 错误：", exception4.Message, exception4.StackTrace };
                            Log.Add(string.Concat((string[])textArray7), string.Concat((string[])textArray8), LogType.流程运行, "", "", "", "", "", "", "", "");
                        }
                    }
                }
            Label_09DA:
                num = this.Update(this.removeTasks, this.updateTasks, this.addTasks, this.executeSqls);
                if (this.executeResult.DebugMessages.IsNullOrEmpty())
                {

                    this.executeResult.DebugMessages = "执行成功，影响的行数：" + ((int)num).ToString();
                }
            }
            if (this.executeResult.Messages.IsNullOrWhiteSpace())
            {
                this.executeResult.Messages = this.GetExecuteMessage(this.executeResult, executeModel, currentStepModel);
            }
            if (executeModel.ExecuteType != OperationType.Save)
            {
                this.SendMessage(this.executeResult.NextTasks, executeModel.Sender, "0,1,2", "");
            }
            return this.executeResult;
        }








        public RoadFlow.Model.FlowTask Get(Guid id)
        {
            return this.flowTaskData.Get(id);
        }




        #region 2.8.8方法

        /// <summary>
        /// 获取回退步骤
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="stepId"></param>
        /// <param name="groupId"></param>
        /// <param name="taskId"></param>
        /// <param name="instanceId"></param>
        /// <param name="userId"></param>
        /// <param name="groupTasks"></param>
        /// <returns></returns>
        // [return: TupleElementNames(new string[] { "html", "message", "backSteps" })]
        public ValueTuple<string, string, List<Step>> GetBackSteps(Guid flowId, Guid stepId, Guid groupId, Guid taskId, string instanceId, Guid userId, List<RoadFlow.Model.FlowTask> groupTasks = null)
        {
            Step step;
            List<Step>.Enumerator enumerator;
            List<Step> list = new List<Step>();
            Flow flow = new Flow();
            groupTasks = groupTasks ?? this.GetListByGroupId(groupId);


            RoadFlow.Model.FlowTask taskModel = groupTasks.Find(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.Id == taskId;
            });
            if (taskModel == null)
            {
                return new ValueTuple<string, string, List<Step>>("", "未找到当前任务!", list);
            }
            RoadFlow.Model.FlowTask currentTask = null;

            if (Config.EnableDynamicStep && taskModel.BeforeStepId.IsEmptyGuid())

            {
                IEnumerable<RoadFlow.Model.FlowTask> enumerable = Enumerable.Where<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)Enumerable.OrderByDescending<RoadFlow.Model.FlowTask, int>((IEnumerable<RoadFlow.Model.FlowTask>)groupTasks,
                   key=>key.Sort), 
                    key=> key.BeforeStepId.IsNotEmptyGuid());

                if (Enumerable.Any<RoadFlow.Model.FlowTask>(enumerable))
                {
                    currentTask = Enumerable.First<RoadFlow.Model.FlowTask>(enumerable);
                }
            }

            RoadFlow.Model.FlowRun flowRunModel = flow.GetFlowRunModel(flowId, true, currentTask);
            if (flowRunModel == null)
            {
                return new ValueTuple<string, string, List<Step>>("", "未找到流程运行时实体!", list);
            }
            Step stepModel = flowRunModel.Steps.Find(delegate (Step p)
            {
                return p.Id == stepId;
            });
            if (stepModel == null)
            {
                return new ValueTuple<string, string, List<Step>>("", "未找到步骤运行时实体!", list);
            }
            if (flowRunModel.FirstStepId == stepModel.Id)
            {
                return new ValueTuple<string, string, List<Step>>("", "第一步不能退回!", list);
            }

            int backModel = stepModel.StepBase.BackModel;
            if (backModel == 0)
            {
                return new ValueTuple<string, string, List<Step>>("", "当前步骤设置为不能退回!", list);
            }
            bool flag = false;
            bool flag2 = false;
            int hanlderModel = stepModel.StepBase.HanlderModel;
            int[] digits = new int[] { 6, 7 };
            if (taskModel.TaskType.In(digits))
            {
                RoadFlow.Model.FlowTask task2 = this.Get(taskModel.PrevId);
                if (task2 != null)
                {
                    stepModel.RunDefaultMembers = task2.ReceiveOrganizeId.HasValue ? ("r_" + task2.ReceiveOrganizeId.Value.ToString()) : ("u_" + task2.ReceiveId.ToString());
                    list.Add(stepModel);
                    flag2 = true;
                }
            }
            else if ((backModel == 1) && (hanlderModel == 4))
            {
                RoadFlow.Model.FlowTask task3 = groupTasks.Find(delegate (RoadFlow.Model.FlowTask p)
                {
                    return (((p.StepId == taskModel.StepId) && (p.TaskType != 5)) && (p.Sort == taskModel.Sort)) && (p.StepSort == (taskModel.StepSort - 1));
                });

                if (task3 != null)
                {
                    stepModel.RunDefaultMembers = task3.ReceiveOrganizeId.HasValue ? ("r_" + task3.ReceiveOrganizeId.Value.ToString()) : ("u_" + task3.ReceiveId.ToString());
                    list.Add(stepModel);
                    flag = true;
                    flag2 = true;
                }
            }
            if (!Enumerable.Any<Step>((IEnumerable<Step>)list))
            {
                switch (stepModel.StepBase.BackType)
                {
                    case 0:
                        using (enumerator = flow.GetPrevSteps(flowRunModel, taskModel.StepId).GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Step prevStep = enumerator.Current;
                                List<RoadFlow.Model.FlowTask> list2 = groupTasks.FindAll(delegate (RoadFlow.Model.FlowTask p)
                                {
                                    return (p.StepId == prevStep.Id) && (p.TaskType != 5);
                                });

                                if (list2.Count > 0)
                                {
                                    int prevMaxSort = Enumerable.Max<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list2,
                                        key=>key.Sort);

                                    List<RoadFlow.Model.FlowTask> list3 = list2.FindAll(delegate (RoadFlow.Model.FlowTask p)
                                  {
                                      return p.Sort == prevMaxSort;
                                  });
                                    if (list3.Count > 0)
                                    {
                                        StringBuilder builder2 = new StringBuilder();
                                        foreach (RoadFlow.Model.FlowTask task4 in list3)
                                        {
                                            builder2.Append(task4.ReceiveOrganizeId.HasValue ? ("r_" + task4.ReceiveOrganizeId.Value.ToString()) : ("u_" + task4.ReceiveId.ToString()));
                                            builder2.Append(",");
                                        }
                                        char[] trimChars = new char[] { ',' };
                                        prevStep.RunDefaultMembers = builder2.ToString().TrimEnd(trimChars);
                                        list.Add(prevStep);
                                    }
                                }
                            }
                            break;
                        }
                        goto Label_04DE;

                    case 1:
                        goto Label_04DE;

                    case 2:
                        {
                            if (!stepModel.StepBase.BackStepId.HasValue)
                            {
                                foreach (Step step3 in this.GetTaskInstanceSteps(flowRunModel, groupTasks))
                                {
                                    if (step3.Id != taskModel.StepId)
                                    {
                                        step3.RunDefaultMembers = this.GetStepHandler(groupTasks, step3.Id);
                                        list.Add(step3);
                                    }
                                }
                                break;
                            }
                            Step step2 = flowRunModel.Steps.Find(delegate (Step p)
                            {
                                return p.Id == stepModel.StepBase.BackStepId.Value;
                            });
                            if (step2 != null)
                            {
                                step2.RunDefaultMembers = this.GetStepHandler(groupTasks, step2.Id);
                                list.Add(step2);
                            }
                            break;
                        }
                }
            }
            goto Label_0608;
        Label_04DE:
            step = flowRunModel.Steps.Find(delegate (Step p)
            {
                return p.Id == flowRunModel.FirstStepId;
            });
            if (step != null)
            {
                step.RunDefaultMembers = "u_" + this.GetFirstSenderId(groupTasks).ToString();
                list.Add(step);
            }
        Label_0608:
            if (!Enumerable.Any<Step>((IEnumerable<Step>)list))
            {
                return new ValueTuple<string, string, List<Step>>("", "未找到要退回的步骤!", list);
            }
            StringBuilder builder = new StringBuilder();
            int num3 = 0;
            using (enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Step backStep = enumerator.Current;
                    if (!flag && (hanlderModel == 4))
                    {
                        List<RoadFlow.Model.FlowTask> list4 = groupTasks.FindAll(delegate (RoadFlow.Model.FlowTask p)
                        {
                            return (p.StepId == backStep.Id) && (p.TaskType != 5);
                        });
                        if (list4.Count > 0)
                        {
                            int maxSort = Enumerable.Max<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list4,
                               key=>key.Sort);

                            IOrderedEnumerable<RoadFlow.Model.FlowTask> enumerable = Enumerable.OrderByDescending<RoadFlow.Model.FlowTask, int>((IEnumerable<RoadFlow.Model.FlowTask>)list4.FindAll(delegate (RoadFlow.Model.FlowTask p)
                            {
                                return p.Sort == maxSort;
                            }), key=>key.StepSort);
                            if (Enumerable.Count<FlowTask>((IEnumerable<FlowTask>)enumerable) > 0)
                            {
                                backStep.RunDefaultMembers = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable).ReceiveOrganizeId.HasValue ? ("r_" + Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable).ReceiveOrganizeId.Value.ToString()) : ("u_" + Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable).ReceiveId.ToString());
                            }
                        }
                    }
                    else if (backModel == 4)
                    {
                        backStep.RunDefaultMembers = "u_" + taskModel.SenderId.ToString();
                        flag2 = true;
                    }

                    string str = (num3++ == 0) ? "checked=\"checked\"" : "";
                    builder.Append("<tr><td><div>");
                    object[] objArray1 = new object[] { "<input type=\"radio\" ", str, " style=\"vertical-align:middle;\" value=\"", backStep.Id, "\" name=\"stepid\" id=\"step_", backStep.Id, "\"/>" };
                    builder.Append(string.Concat((object[])objArray1));
                    object[] objArray2 = new object[] { "<label style=\"vertical-align:middle;\" for=\"step_", backStep.Id, "\">", backStep.Name, flag2 ? ("(" + new Organize().GetNames(backStep.RunDefaultMembers) + ")") : "", "</label>" };
                    builder.Append(string.Concat((object[])objArray2));
                    if (1 == stepModel.StepBase.BackSelectUser)
                    {
                        builder.Append("</div><div style=\"margin:1px 0 3px 0;\">");
                        object[] objArray3 = new object[] { "<input type=\"text\" isChangeType=\"0\" class=\"mymember\" style=\"width:70%;\" id=\"user_", backStep.Id, "\" rootid=\"", backStep.RunDefaultMembers, "\" value=\"", backStep.RunDefaultMembers, "\" />" };
                        builder.Append(string.Concat((object[])objArray3));
                        builder.Append("</div>");
                    }
                    else
                    {
                        object[] objArray4 = new object[] { "<input type=\"hidden\" id=\"user_", backStep.Id, "\" value=\"", backStep.RunDefaultMembers, "\" />" };
                        builder.Append(string.Concat((object[])objArray4));
                        builder.Append("</div>");
                    }
                    builder.Append("</tr></td>");
                }
            }
            return new ValueTuple<string, string, List<Step>>(builder.ToString(), "", list);
        }



        #endregion


        /// <summary>
        /// 获取完成任务
        /// </summary>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="userId"></param>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="order"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataTable GetCompletedTask(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            return this.flowTaskData.GetCompletedList(size, number, userId, flowId, title, startDate, endDate, order, out count);
        }


        /// <summary>
        /// 获取抄送用户
        /// </summary>
        /// <param name="stepModel"></param>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        /// <param name="groupTasks"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.User> GetCopyForUsers(Step stepModel, RoadFlow.Model.FlowRun flowRunModel, Execute executeModel, List<RoadFlow.Model.FlowTask> groupTasks)
        {
            string[] strArray;
            int num;
            StringBuilder builder = new StringBuilder();
            if (!stepModel.StepCopyFor.MemberId.IsNullOrWhiteSpace())
            {
                builder.Append(stepModel.StepCopyFor.MemberId);
                builder.Append(",");
            }
            if (!stepModel.StepCopyFor.MethodOrSql.IsNullOrWhiteSpace())
            {
                string str = stepModel.StepCopyFor.MethodOrSql.Trim();
                if (str.StartsWith("select", (StringComparison)StringComparison.CurrentCultureIgnoreCase))
                {
                    if (flowRunModel.Databases.Count > 0)
                    {
                        StringBuilder builder2 = new StringBuilder();
                        foreach (DataRow row in new DbConnection().GetDataTable(Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases).ConnectionId, Wildcard.Filter(str, null, null), (DbParameter[])null).Rows)
                        {
                            builder2.Append(row[0]);
                            builder2.Append(",");
                        }
                        builder.Append(builder2.ToString());
                    }
                }
                else
                {
                    EventParam param = new EventParam
                    {
                        FlowId = flowRunModel.Id,
                        GroupId = executeModel.GroupId,
                        InstanceId = executeModel.InstanceId,
                        StepId = stepModel.Id,
                        TaskId = executeModel.TaskId
                    };
                    object[] args = new object[] { param };
                    ValueTuple<object, Exception> tuple1 = Tools.ExecuteMethod(str.Trim(), args);
                    object obj2 = tuple1.Item1;
                    if (obj2 != null)
                    {
                        builder.Append(obj2.ToString());
                        builder.Append(",");
                    }
                }
            }
            if (!stepModel.StepCopyFor.HandlerType.IsNullOrWhiteSpace())
            {
                User user = new User();
                char[] separator = new char[] { ',' };
                strArray = stepModel.StepCopyFor.HandlerType.Split(separator);
                for (num = 0; num < strArray.Length; num++)
                {
                    int num2;
                    if (!strArray[num].IsInt(out num2))
                    {
                        continue;
                    }
                    string prevStepHandler = string.Empty;
                    Guid firstSenderId = this.GetFirstSenderId(groupTasks);
                    switch (num2)
                    {
                        case 0:
                            prevStepHandler = "u_" + firstSenderId;
                            break;

                        case 1:
                            prevStepHandler = this.GetPrevStepHandler(groupTasks, executeModel.TaskId);
                            break;

                        case 2:
                            prevStepHandler = user.GetLeader(firstSenderId.ToString()).Item1;
                            break;

                        case 3:
                            prevStepHandler = user.GetLeader(firstSenderId.ToString()).Item2;
                            break;

                        case 4:
                            prevStepHandler = user.GetParentLeader(firstSenderId.ToString()).Item1;
                            break;

                        case 5:
                            prevStepHandler = user.GetUserIds(user.GetOrganizeUsers(firstSenderId.ToString()));
                            break;

                        case 6:
                            prevStepHandler = user.GetAllParentLeader(firstSenderId.ToString()).Item1;
                            break;
                    }
                    builder.Append(prevStepHandler);
                    builder.Append(",");
                }
            }
            if (!stepModel.StepCopyFor.Steps.IsNullOrWhiteSpace())
            {
                char[] chArray2 = new char[] { ',' };
                strArray = stepModel.StepCopyFor.Steps.Split(chArray2);
                for (num = 0; num < strArray.Length; num++)
                {
                    Guid guid2;
                    if (strArray[num].IsGuid(out guid2))
                    {
                        builder.Append(this.GetStepHandler(groupTasks, guid2));
                        builder.Append(",");
                    }
                }
            }
            char[] trimChars = new char[] { ',' };
            return new Organize().GetAllUsers(builder.ToString().TrimEnd(trimChars));
        }



        /// <summary>
        /// 获取默认成员
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="currentStepModel"></param>
        /// <param name="nextStepModel"></param>
        /// <param name="groupTasks"></param>
        /// <param name="taskId"></param>
        /// <param name="instanceId"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        // [return: TupleElementNames(new string[] { "defaultMembers", "orgSelectType", "selectRange" })]
        private ValueTuple<string, string, string> GetDefaultMember(RoadFlow.Model.FlowRun flowRunModel, Step currentStepModel, Step nextStepModel, List<RoadFlow.Model.FlowTask> groupTasks, Guid taskId, string instanceId, Guid currentUserId)
        {
            if ((flowRunModel.Debug > 0) && flowRunModel.DebugUserIds.ContainsIgnoreCase(currentUserId.ToString()))
            {
                return new ValueTuple<string, string, string>("u_" + currentUserId.ToString(), "", "");
            }
            string userIds = string.Empty;
            string str2 = string.Empty;
            string str3 = string.Empty;
            RoadFlow.Model.FlowTask task = groupTasks.Find(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.Id == taskId;
            });
            Guid firstSenderId = this.GetFirstSenderId(groupTasks);
            if (firstSenderId.IsEmptyGuid() && (currentStepModel.Id == flowRunModel.FirstStepId))
            {
                firstSenderId = currentUserId;
            }
            User user = new User();
            switch (nextStepModel.StepBase.HandlerType.ToInt(-2147483648))
            {
                case 0:
                    str2 = "unit='1' dept='1' station='1' workgroup='1' user='1'";
                    break;

                case 1:
                    str2 = "unit='0' dept='1' station='0' workgroup='0' user='0'";
                    break;

                case 2:
                    str2 = "unit='0' dept='0' station='1' workgroup='0' user='0'";
                    break;

                case 3:
                    str2 = "unit='0' dept='0' station='0' workgroup='1' user='0'";
                    break;

                case 4:
                    str2 = "unit='0' dept='0' station='0' workgroup='0' user='1'";
                    break;

                case 5:
                    userIds = "u_" + firstSenderId.ToString();
                    break;

                case 6:
                    userIds = (task == null) ? ("u_" + currentUserId) : this.GetStepHandler(groupTasks, task.StepId);
                    break;

                case 7:
                    userIds = nextStepModel.StepBase.HandlerStepId.HasValue ? this.GetStepHandler(groupTasks, nextStepModel.StepBase.HandlerStepId.Value) : string.Empty;
                    if (userIds.IsNullOrWhiteSpace() && (currentStepModel.Id == flowRunModel.FirstStepId))
                    {
                        userIds = "u_" + currentUserId.ToString();
                    }
                    break;

                case 8:
                    if (flowRunModel.Databases.Count > 0)
                    {
                        string str6 = string.Empty;
                        userIds = !str6.IsNullOrWhiteSpace() ? str6 : this.GetFieldValue(Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases), nextStepModel.StepBase.ValueField, instanceId);
                    }
                    break;

                case 9:
                    userIds = user.GetLeader(firstSenderId.ToString()).Item1;
                    break;

                case 10:
                    userIds = user.GetLeader(firstSenderId.ToString()).Item2;
                    break;

                case 11:
                    userIds = (task == null) ? user.GetLeader("u_" + currentUserId).Item1 : user.GetLeader(Enumerable.ToList<string>(this.GetStepHandler(groupTasks, task.StepId).Split(',', (StringSplitOptions)StringSplitOptions.None))).Item1;

                    break;

                case 12:
                    userIds = (task == null) ? user.GetLeader("u_" + currentUserId).Item2 : user.GetLeader(Enumerable.ToList<string>(this.GetStepHandler(groupTasks, task.StepId).Split(',', (StringSplitOptions)StringSplitOptions.None))).Item2;

                    break;

                case 13:
                    userIds = user.GetParentLeader(firstSenderId.ToString()).Item1;
                    break;

                case 14:
                    userIds = (task == null) ? user.GetParentLeader("u_" + currentUserId).Item1 : user.GetParentLeader(Enumerable.ToList<string>(this.GetStepHandler(groupTasks, task.StepId).Split(',', (StringSplitOptions)StringSplitOptions.None))).Item1;
                    break;

                case 15:
                    userIds = (task == null) ? user.GetUserIds(user.GetOrganizeUsers("u_" + currentUserId)) : user.GetUserIds(user.GetOrganizeUsers(Enumerable.ToList<string>(this.GetStepHandler(groupTasks, task.StepId).Split(',', (StringSplitOptions)StringSplitOptions.None))));
                    break;


                case 0x10:
                    userIds = user.GetUserIds(user.GetOrganizeUsers(firstSenderId.ToString()));
                    break;

                case 0x11:
                    userIds = user.GetAllParentLeader(firstSenderId.ToString()).Item1;
                    break;

                case 0x12:
                    userIds = (task == null) ? user.GetAllParentLeader(firstSenderId.ToString()).Item1 : user.GetAllParentLeader(Enumerable.ToList<string>(this.GetStepHandler(groupTasks, task.StepId).Split(',', (StringSplitOptions)StringSplitOptions.None))).Item1;
                    break;

            }
            string defaultHandlerSqlOrMethod = nextStepModel.StepBase.DefaultHandlerSqlOrMethod;
            if (!defaultHandlerSqlOrMethod.IsNullOrWhiteSpace())
            {
                if (defaultHandlerSqlOrMethod.Trim().StartsWith("select", (StringComparison)StringComparison.CurrentCultureIgnoreCase))
                {
                    if (flowRunModel.Databases.Count > 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        DataTable table = null;
                        try
                        {
                            table = new DbConnection().GetDataTable(Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases).ConnectionId, Wildcard.Filter(defaultHandlerSqlOrMethod, null, null), (DbParameter[])null);
                        }
                        catch
                        {
                        }
                        if ((table != null) && (table.Columns.Count > 0))
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                builder.Append(row[0].ToString());
                                builder.Append(",");
                            }
                        }
                        userIds = builder.ToString().TrimEnd(',');
                    }

                }

                else
                {
                    EventParam param = new EventParam
                    {
                        FlowId = flowRunModel.Id,
                        GroupId = (groupTasks.Count > 0) ? Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)groupTasks).GroupId : Guid.Empty,
                        InstanceId = instanceId,
                        StepId = currentStepModel.Id,
                        TaskId = taskId
                    };
                    object[] args = new object[] { param };
                    ValueTuple<object, Exception> tuple1 = Tools.ExecuteMethod(defaultHandlerSqlOrMethod.Trim(), args);
                    object obj2 = tuple1.Item1;
                    Exception exception = tuple1.Item2;

                    if (obj2 != null)
                    {
                        userIds = obj2.ToString();
                    }
                }
            }
            if (!nextStepModel.StepBase.DefaultHandler.IsNullOrWhiteSpace())
            {
                userIds = userIds + "," + nextStepModel.StepBase.DefaultHandler;
            }

            string str5 = userIds.IsNullOrWhiteSpace() ? string.Empty : ((IEnumerable<string>)Enumerable.ToList<string>(Enumerable.Distinct<string>(userIds.Split(',', (StringSplitOptions)StringSplitOptions.None)))).JoinList<string>(",", "", "");

            if (nextStepModel.StepBase.HandlerType.ToInt(-2147483648) > 6)
            {
                str3 = str5;
            }
            return new ValueTuple<string, string, string>(str5, str2, str3);
        }


        /// <summary>
        /// 获取动态字段步骤Html
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="step"></param>
        /// <param name="instanceId"></param>
        /// <param name="localizer"></param>
        /// <returns></returns>
        public string GetDynamicFieldStepHtml(RoadFlow.Model.FlowRun flowRunModel, Step step, string instanceId, IStringLocalizer localizer = null)
        {
            if (flowRunModel.Databases.Count == 0)
            {
                return string.Empty;
            }
            char[] separator = new char[] { '.' };
            string[] strArray = step.DynamicField.Split(separator);
            if (strArray.Length != 3)
            {
                return string.Empty;
            }
            Guid id = strArray[0].ToGuid();
            string tableName = strArray[1];
            string fieldName = strArray[2];
            string str = string.Empty;
            try
            {
                str = new DbConnection().GetFieldValue(id, tableName, fieldName, Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases).PrimaryKey, instanceId);
            }
            catch
            {
            }
            if (str.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            char[] chArray2 = new char[] { ',' };
            StringBuilder builder = new StringBuilder();
            int num = 0;
            foreach (string str4 in str.Split(chArray2))
            {
                string str5 = Guid.NewGuid().ToString("N");
                string str6 = (num == 0) ? step.Id.ToString() : Guid.NewGuid().ToString();
                string str7 = step.Id.ToString();
                string names = new Organize().GetNames(str4);
                string[] textArray1 = new string[] {
                "<tr data-dynamic=\"1\" id=\"", str5, "\" data-beforestepid=\"", str7, "\"><td><input type=\"hidden\" value=\"", str7, "\" id=\"before_", str6, "\">", (num == 0) ? ((string) string.Concat((string[]) new string[] { "<input type=\"radio\" checked=\"checked\" style=\"display:none;\" value=\"0\" id=\"parallelorserial_", str7, "\" name=\"parallelorserial_", str7, "\"/>" })) : "", "<input type=\"checkbox\" checked=\"checked\" disabled=\"disbaled\" value=\"", str6, "\" id=\"step_", str6, "\" name=\"step\" style=\"vertical-align:middle;\"><label for=\"step_", str6,
                "\"><input type=\"text\" id=\"name_", str6, "\" class=\"mytext\" style=\"width: 130px;\" value=\"", names, "\"></label></td></tr>"
             };
                builder.Append(string.Concat((string[])textArray1));
                string[] textArray3 = new string[] { "<tr data-dynamic=\"1\" id=\"", str5, "_1\" data-beforestepid=\"", str7, "\"><td style=\"padding: 2px 0 4px 0;\"><input type=\"text\" class=\"mymember\" opener=\"parent\" ismobile=\"\" id=\"user_", str6, "\" name=\"user_", str6, "\" value=\"", str4, "\" ischangetype=\"1\" rootid=\"\" style=\"width:45%;\" readonly=\"\">" };
                builder.Append(string.Concat((string[])textArray3));
                if (1 == step.SendSetWorkTime)
                {
                    string[] textArray4 = new string[] { "<span style=\"margin-left:5px;\">", (localizer == null) ? "完成时间" : localizer["FlowSend_CompletedTime"], "：</span><input type=\"text\" class=\"mycalendar\" istime=\"1\" dayafter=\"1\" value=\"\" style=\"width:120px;\" id=\"CompletedTime_", str6, "\" name=\"CompletedTime_", str6, "\">" };
                    builder.Append(string.Concat((string[])textArray4));
                }
                builder.Append("</td></tr>");
                num++;
            }
            return builder.ToString();
        }





        /// <summary>
        /// 获取动态任务
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public ValueTuple<RoadFlow.Model.FlowTask, RoadFlow.Model.FlowTask, List<RoadFlow.Model.FlowTask>> GetDynamicTask(Guid groupId, Guid? taskId = new Guid?())
        {
            if (!Config.EnableDynamicStep || groupId.IsEmptyGuid())
            {
                return new ValueTuple<RoadFlow.Model.FlowTask, RoadFlow.Model.FlowTask, List<RoadFlow.Model.FlowTask>>(null, taskId.HasValue ? Get(taskId.Value) : null, null);
            }
            List<RoadFlow.Model.FlowTask> listByGroupId = this.GetListByGroupId(groupId);
            if (listByGroupId.Count == 0)
            {
                return new ValueTuple<RoadFlow.Model.FlowTask, RoadFlow.Model.FlowTask, List<RoadFlow.Model.FlowTask>>(null, null, listByGroupId);
            }
            RoadFlow.Model.FlowTask task = null;
            RoadFlow.Model.FlowTask task2 = null;
            IOrderedEnumerable<RoadFlow.Model.FlowTask> enumerable = Enumerable.OrderByDescending<RoadFlow.Model.FlowTask, int>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId.FindAll(
                key=> key.BeforeStepId.IsNotEmptyGuid()), 
                key=>key.Sort);


            if (Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable))
            {
                task = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable);
            }
            if (taskId.IsNotEmptyGuid())
            {
                task2 = listByGroupId.Find(delegate (RoadFlow.Model.FlowTask p)
                {
                    return p.Id == taskId.Value;
                });
            }
            return new ValueTuple<RoadFlow.Model.FlowTask, RoadFlow.Model.FlowTask, List<RoadFlow.Model.FlowTask>>(task, task2, listByGroupId);
        }



        /// <summary>
        /// 获取委托任务
        /// </summary>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="userId"></param>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="order"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataTable GetEntrustTask(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            return this.flowTaskData.GetEntrustList(size, number, userId, flowId, title, startDate, endDate, order, out count);
        }


        /// <summary>
        /// 获取执行信息
        /// </summary>
        /// <param name="executeResultModel"></param>
        /// <param name="executeModel"></param>
        /// <param name="currentStepModel"></param>
        /// <returns></returns>
        public string GetExecuteMessage(ExecuteResult executeResultModel, Execute executeModel, Step currentStepModel)
        {

            string str = string.Empty;
            if (!currentStepModel.SendShowMessage.IsNullOrWhiteSpace())
            {
                return Wildcard.Filter(currentStepModel.SendShowMessage, null, null);
            }

            List<RoadFlow.Model.FlowTask> list = this.executeResult.NextTasks.FindAll(
                key=> (key.TaskType != 5));


            switch (executeModel.ExecuteType)
            {
                case OperationType.Submit:
                case OperationType.FreeSubmit:
                    {
                        if (!executeResultModel.IsSuccess || (Enumerable.Count<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list) <= 0))
                        {
                            return "已发送,等待他人处理!";
                        }

                        List<string> list2 = new List<string>();


                        foreach (IGrouping<string, RoadFlow.Model.FlowTask> grouping in Enumerable.GroupBy<RoadFlow.Model.FlowTask, string>((IEnumerable<RoadFlow.Model.FlowTask>)list,
                            key=> key.StepName))
                        {
                            list2.Add(grouping.Key);

                        }
                        return ("已发送到：" + ((IEnumerable<string>)list2).JoinList<string>("、", "", ""));

                    }
                case OperationType.Save:
                    if (executeResultModel.IsSuccess)
                    {
                        str = "已保存!";
                    }
                    return str;

                case OperationType.Back:
                    {
                        if (!executeResultModel.IsSuccess || (Enumerable.Count<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list) <= 0))
                        {
                            return "已退回,等待他人处理!";
                        }
                        List<string> list3 = new List<string>();

                        foreach (IGrouping<string, RoadFlow.Model.FlowTask> grouping2 in Enumerable.GroupBy<RoadFlow.Model.FlowTask, string>((IEnumerable<RoadFlow.Model.FlowTask>)list,
                           key=> key.StepName))
                        {
                            list3.Add(grouping2.Key);
                        }
                        return ("已退回到：" + ((IEnumerable<string>)list3).JoinList<string>("、", "", ""));

                    }
                case OperationType.Completed:
                    return (executeResultModel.IsSuccess ? "已完成!" : "已发送,等待他人处理!");

                case OperationType.Redirect:
                    {
                        if (!this.executeResult.IsSuccess || (Enumerable.Count<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list) <= 0))
                        {
                            return "没有接收人!";
                        }
                        List<string> list4 = new List<string>();
                        foreach (IGrouping<string, RoadFlow.Model.FlowTask> grouping3 in Enumerable.GroupBy<RoadFlow.Model.FlowTask, string>((IEnumerable<RoadFlow.Model.FlowTask>)list, 
                            key=> key.StepName))
                        {
                            list4.Add(grouping3.Key);
                        }
                        return ("已转交给：" + ((IEnumerable<string>)list4).JoinList<string>("、", "", ""));

                    }
                case OperationType.AddWrite:
                    return str;

                case OperationType.CopyforCompleted:
                    if (!this.executeResult.IsSuccess)
                    {
                        return "处理失败!";
                    }
                    return "已完成!";
            }
            return str;
        }


        /// <summary>
        /// 我的开始流程
        /// </summary>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="userId"></param>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="order"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataTable GetMyStartList(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string status, string order, out int count)
        {
            return this.flowTaskData.GetMyStartList(size, number, userId, flowId, title, startDate, endDate, status, order, out count);
        }










        /// <summary>
        /// 获取执行实体
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public ValueTuple<Execute, string> GetExecuteModel(string json)
        {
            Guid guid;
            Guid flowId = new Guid();
            Guid guid3;
            if (json.IsNullOrWhiteSpace())
            {
                return new ValueTuple<Execute, string>(null, "参数为空");
            }
            JObject obj2 = null;
            try
            {
                obj2 = JObject.Parse(json);
            }
            catch
            {
                return new ValueTuple<Execute, string>(null, "参数解析错误");
            }
            string str = obj2.Value<string>("id");
            string str2 = obj2.Value<string>("flowId");
            if (!str.IsGuid(out guid) && !str2.IsGuid(out flowId))
            {
                return new ValueTuple<Execute, string>(null, "任务ID和流程ID同时为空");
            }
            RoadFlow.Model.FlowTask task = guid.IsEmptyGuid() ? null : this.Get(guid);
            if (flowId.IsEmptyGuid() && (task != null))
            {
                flowId = task.FlowId;
            }
            RoadFlow.Model.FlowRun flowRunModel = new Flow().GetFlowRunModel(flowId, true, task);
            if (flowRunModel == null)
            {
                return new ValueTuple<Execute, string>(null, "流程运行时实体为空");
            }
            obj2.Value<string>("instanceId");
            string str3 = obj2.Value<string>("title");
            string str4 = obj2.Value<string>("comment");
            string str5 = obj2.Value<string>("type");
            string str6 = obj2.Value<string>("note");
            if (!obj2.Value<string>("senderId").IsGuid(out guid3))
            {
                return new ValueTuple<Execute, string>(null, "发送人为空");
            }
            RoadFlow.Model.User user = new User().Get(guid3);
            if (user == null)
            {
                return new ValueTuple<Execute, string>(null, "没有找到发送人");
            }




            if (str5.IsNullOrEmpty())
            {
                return new ValueTuple<Execute, string>(null, "执行类型为空");
            }
            int num = obj2.Value<string>("sign").ToInt(0);
            JArray array = obj2.Value<JArray>("steps");
            Execute execute = new Execute
            {
                Comment = str4,
                FlowId = (task == null) ? flowId : task.FlowId
            };




            execute.ExecuteType = this.GetExecuteType(str5);





            execute.GroupId = (task == null) ? Guid.Empty : task.GroupId;
            execute.InstanceId = (task == null) ? string.Empty : task.InstanceId;
            execute.IsSign = num;
            execute.Note = str6;
            execute.Sender = user;
            execute.StepId = (task == null) ? flowRunModel.FirstStepId : task.StepId;
            execute.TaskId = (task == null) ? Guid.Empty : task.Id;
            execute.Title = str3;

            List<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>> list = new List<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>();


            Organize organize = new Organize();
            foreach (JObject obj3 in obj2.Value<JArray>("steps"))
            {
                Guid id = obj3.Value<string>("id").ToGuid();
                Step step = flowRunModel.Steps.Find(delegate (Step p)
                {
                    return p.Id == id;
                });
                if (step != null)
                {
                    DateTime time;
                    Guid guid4;
                    int num2;
                    string str7 = obj3.Value<string>("name");
                    string str8 = obj3.Value<string>("beforestepid");
                    string str9 = obj3.Value<string>("parallelorserial");
                    string idString = obj3.Value<string>("member");




                    DateTime? nullable = obj3.Value<string>("completedtime").IsDateTime(out time) ? new DateTime?(time) : ((step.WorkTime > decimal.Zero) ? new DateTime?(DateTimeExtensions.Now.AddDays((double)((double)step.WorkTime))) : null);
                    list.Add(new ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>(id, str7, str8.IsGuid(out guid4) ? new Guid?(guid4) : null, str9.IsInt(out num2) ? new int?(num2) : null, organize.GetAllUsers(idString), nullable));

                }
            }
            execute.Steps = list;
            return new ValueTuple<Execute, string>(execute, string.Empty);
        }




        /// <summary>
        /// 获取执行类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public OperationType GetExecuteType(string type)
        {
            if (!type.IsNullOrWhiteSpace())
            {
                string s = type.ToLower();
                switch (PrivateImplementationDetails.ComputeStringHash(s))
                {
                    case 0x5bb421a2:
                        if (s == "back")
                        {
                            return OperationType.Back;
                        }
                        break;

                    case 0x889f5ae3:
                        if (s == "addwrite")
                        {
                            return OperationType.AddWrite;
                        }
                        break;

                    case 0x6c681ab:
                        if (s == "freesubmit")
                        {
                            return OperationType.FreeSubmit;
                        }
                        break;

                    case 0x173b3eb1:
                        if (s == "redirect")
                        {
                            return OperationType.Redirect;
                        }
                        break;

                    case 0xccff7e48:
                        if (s == "save")
                        {
                            return OperationType.Save;
                        }
                        break;

                    case 0xe6b52521:
                        if (s == "taskend")
                        {
                            return OperationType.TaskEnd;
                        }
                        break;

                    case 0xf4748112:
                        if (s == "completed")
                        {
                            return OperationType.Completed;
                        }
                        break;

                    case 0xf7eb0225:
                        if (s == "submit")
                        {
                            return OperationType.Submit;
                        }
                        break;

                    case 0xfaaf6c44:
                        if (s == "copyforcompleted")
                        {
                            return OperationType.CopyforCompleted;
                        }
                        break;
                }
            }
            return OperationType.Save;
        }



        /// <summary>
        /// 获取执行类型选项
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetExecuteTypeOptions(int value = -100)
        {
            Dictionary<int, string> dictionary1 = new Dictionary<int, string>();
            dictionary1.Add(-1, "等待中");
            dictionary1.Add(0, "未处理");
            dictionary1.Add(1, "处理中");
            dictionary1.Add(2, "已完成");
            dictionary1.Add(3, "已退回");
            dictionary1.Add(4, "他人已处理");
            dictionary1.Add(5, "他人已退回");
            dictionary1.Add(6, "已转交");
            dictionary1.Add(7, "已委托");
            dictionary1.Add(8, "已阅知");
            dictionary1.Add(9, "已指派");
            dictionary1.Add(10, "已跳转");
            dictionary1.Add(11, "已终止");
            dictionary1.Add(12, "他人已终止");
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<int, string> pair in dictionary1)
            {
                builder.Append("<option value=\"" + ((int)pair.Key).ToString() + "\"");
                if (pair.Key == value)
                {
                    builder.Append(" selected=\"selected\"");
                }
                builder.Append(">" + pair.Value + "</option>");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取执行类型标题
        /// </summary>
        /// <param name="executeType"></param>
        /// <returns></returns>
        public string GetExecuteTypeTitle(int executeType)
        {
            string str = string.Empty;
            switch (executeType)
            {
                case -1:
                    return "等待中";

                case 0:
                    return "未处理";

                case 1:
                    return "处理中";

                case 2:
                    return "已完成";

                case 3:
                    return "已退回";

                case 4:
                    return "他人已处理";

                case 5:
                    return "他人已退回";

                case 6:
                    return "已转交";

                case 7:
                    return "已委托";

                case 8:
                    return "已阅知";

                case 9:
                    return "已指派";

                case 10:
                    return "已跳转";

                case 11:
                    return "已终止";

                case 12:
                    return "他人已终止";

                case 13:
                    return "已加签";
            }
            return str;
        }


        /// <summary>
        /// 获取字段值
        /// </summary>
        /// <param name="database"></param>
        /// <param name="fieldString"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public string GetFieldValue(Database database, string fieldString, string instanceId)
        {
            if ((!fieldString.IsNullOrWhiteSpace() && !instanceId.IsNullOrWhiteSpace()) && (database != null))
            {
                Guid guid;
                char[] separator = new char[] { '.' };
                string[] strArray = fieldString.Split(separator);
                string str = string.Empty;
                string str2 = string.Empty;
                string str3 = string.Empty;
                if (strArray.Length != 0)
                {
                    str = strArray[0];
                }
                if (strArray.Length > 1)
                {
                    str2 = strArray[1];
                }
                if (strArray.Length > 2)
                {
                    str3 = strArray[2];
                }
                string primaryKey = database.PrimaryKey;
                if ((str.IsGuid(out guid) && !str2.IsNullOrWhiteSpace()) && (!str3.IsNullOrWhiteSpace() && !primaryKey.IsNullOrWhiteSpace()))
                {
                    return new DbConnection().GetFieldValue(guid, str2, str3, primaryKey, instanceId);
                }
            }
            return string.Empty;
        }


        /// <summary>
        /// 获取第一发送id
        /// </summary>
        /// <param name="groupTasks"></param>
        /// <returns></returns>
        public Guid GetFirstSenderId(List<RoadFlow.Model.FlowTask> groupTasks)
        {
            if (groupTasks.Count != 0)
            {
                RoadFlow.Model.FlowTask task = groupTasks.Find(
                    key=> (key.PrevId == Guid.Empty));
                if (task != null)
                {
                    return task.SenderId;
                }
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 获取第一发送id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Guid GetFirstSenderId(Guid groupId)
        {
            return this.GetFirstSenderId(this.GetListByGroupId(groupId));
        }



        /// <summary>
        /// 获取第一个任务
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        /// <returns></returns>
        public RoadFlow.Model.FlowTask GetFirstTask(RoadFlow.Model.FlowRun flowRunModel, Execute executeModel)
        {
            Step step = flowRunModel.Steps.Find(delegate (Step p)
            {
                return p.Id == flowRunModel.FirstStepId;
            });
            if (step == null)
            {
                throw new Exception("未找到流程第一步!");
            }




            if (executeModel.Title.IsNullOrWhiteSpace())
            {
                executeModel.Title = flowRunModel.Name + " - " + executeModel.Sender.Name;
            }
            return new RoadFlow.Model.FlowTask
            {
                Comments = executeModel.Comment,
                ExecuteType = 1,
                FlowId = executeModel.FlowId,
                FlowName = flowRunModel.Name,
                GroupId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                InstanceId = executeModel.InstanceId,
                IsSign = executeModel.IsSign,
                Note = executeModel.Note,
                OtherType = executeModel.OtherType,
                PrevId = Guid.Empty,
                PrevStepId = Guid.Empty,
                ReceiveId = executeModel.Sender.Id,
                ReceiveName = executeModel.Sender.Name,
                ReceiveTime = DateTimeExtensions.Now,
                SenderId = executeModel.Sender.Id,
                SenderName = executeModel.Sender.Name,
                Sort = 1,
                Status = 1,
                StepId = flowRunModel.FirstStepId,
                StepName = this.GetStepName(flowRunModel, executeModel.StepId),
                StepSort = 1,
                TaskType = 0,
                Title = executeModel.Title,
                IsAutoSubmit = 0,
                IsBatch = new int?(step.BatchExecute)
            };
        }







        /// <summary>
        /// 获取实例列表
        /// </summary>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="receiveId"></param>
        /// <param name="receiveDate1"></param>
        /// <param name="receiveDate2"></param>
        /// <param name="order"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataTable GetInstanceList(int size, int number, string flowId, string title, string receiveId, string receiveDate1, string receiveDate2, string order, out int count)
        {
            return this.flowTaskData.GetInstanceList(size, number, flowId, title, receiveId, receiveDate1, receiveDate2, order, out count);
        }



        /// <summary>
        /// 获取连接标识步骤
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="prevSteps"></param>
        /// <returns></returns>
        public Guid GetJoinSignStepId(RoadFlow.Model.FlowRun flowRunModel, List<Step> prevSteps)
        {
            List<List<Step>> list = new List<List<Step>>();
            Guid firstStepId = flowRunModel.FirstStepId;
            Flow flow = new Flow();
            foreach (Step step in prevSteps)
            {
                list.Add(flow.GetRangeSteps(flowRunModel, firstStepId, step.Id));
            }
            if (!Enumerable.Any<List<Step>>((IEnumerable<List<Step>>)list))
            {
                return Guid.Empty;
            }
            List<Step> list2 = Enumerable.First<List<Step>>((IEnumerable<List<Step>>)list);
            list.Remove(list2);
            List<Step> list3 = new List<Step>();
            using (List<Step>.Enumerator enumerator = list2.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Predicate<Step> s_9__0 = null;
                    Step step = enumerator.Current;
                    using (List<List<Step>>.Enumerator enumerator2 = list.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            Step step2 = enumerator2.Current.Find(s_9__0 ?? (s_9__0 = delegate (Step p)
                            {
                                return p.Id == step.Id;
                            }));
                            if (step2 != null)
                            {
                                list3.Add(step2);
                            }
                        }
                        continue;
                    }
                }
            }
            if (!Enumerable.Any<Step>((IEnumerable<Step>)list3))
            {
                return Guid.Empty;
            }
            return Enumerable.Last<Step>((IEnumerable<Step>)list3).Id;
        }


        /// <summary>
        /// 通过成组id获取列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.FlowTask> GetListByGroupId(Guid groupId)
        {
            return Enumerable.ToList<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)Enumerable.ThenBy<RoadFlow.Model.FlowTask, string>(Enumerable.ThenBy<RoadFlow.Model.FlowTask, string>(Enumerable.ThenBy<RoadFlow.Model.FlowTask, DateTime>(Enumerable.ThenBy<RoadFlow.Model.FlowTask, int>(Enumerable.OrderBy<RoadFlow.Model.FlowTask, int>((IEnumerable<RoadFlow.Model.FlowTask>)this.flowTaskData.GetListByGroupId(groupId),
                key=>key.Sort), 
               key=> key.StepSort), 
               key=> key.ReceiveTime),
               key=> key.StepName), 
                key=> key.ReceiveName));
        }

        /// <summary>
        /// 通过成组子流程id获取列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.FlowTask> GetListBySubFlowGroupId(Guid groupId)
        {
            return this.flowTaskData.GetListBySubFlowGroupId(groupId);
        }

        public RoadFlow.Model.FlowTask GetMaxByGroupId(Guid groupId)
        {
            return this.flowTaskData.GetMaxByGroupId(groupId);
        }


        /// <summary>
        /// 获取下一个步骤
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId"></param>
        /// <param name="groupId"></param>
        /// <param name="taskId"></param>
        /// <param name="instanceId"></param>
        /// <param name="userId"></param>
        /// <param name="isFreeSend"></param>
        /// <param name="isMobile"></param>
        /// <returns></returns>
        public ValueTuple<string, string, List<Step>> GetNextSteps(RoadFlow.Model.FlowRun flowRunModel, Guid stepId, Guid groupId, Guid taskId, string instanceId, Guid userId, bool isFreeSend, bool isMobile = false, List<RoadFlow.Model.FlowTask> groupTasks = null, IStringLocalizer localizer = null)
        {
            ValueTuple<Step, string> tuple2;
            groupTasks = groupTasks ?? this.GetListByGroupId(groupId);
            RoadFlow.Model.FlowTask currentTask = groupTasks.Find(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.Id == taskId;
            });



            List<Step> list = new List<Step>();
            Flow flow = new Flow();
            Step stepModel = flowRunModel.Steps.Find(delegate (Step p)
            {
                return p.Id == stepId;
            });
            if (stepModel == null)
            {
                return new ValueTuple<string, string, List<Step>>("", "未找到步骤运行时!", list);
            }

            List<ValueTuple<Step, string>> list2 = new List<ValueTuple<Step, string>>();
            bool flag = (currentTask != null) && (currentTask.TaskType == 6);
            if (((currentTask != null) && (currentTask.TaskType == 4)) && (1 == stepModel.StepBase.SendToBackStep))
            {
                string stepHandler = this.GetStepHandler(groupTasks, currentTask.PrevStepId);
                Step step = flowRunModel.Steps.Find(delegate (Step p)
                {
                    return p.Id == currentTask.PrevStepId;
                });
                if (step != null)
                {
                    list2.Add(new ValueTuple<Step, string>(step, stepHandler));
                }
            }
            else
            {
                bool flag6 = false;
                if ((stepModel.StepBase.HanlderModel == 4) && (currentTask != null))
                {
                    RoadFlow.Model.FlowTask task = groupTasks.Find(delegate (RoadFlow.Model.FlowTask p)
                    {
                        return ((p.StepId == stepModel.Id) && (p.Sort == currentTask.Sort)) && (p.StepSort == (currentTask.StepSort + 1));
                    });
                    if (task != null)
                    {
                        list2.Add(new ValueTuple<Step, string>(stepModel, task.ReceiveOrganizeId.HasValue ? ("r_" + task.ReceiveOrganizeId.Value.ToString()) : ("u_" + task.ReceiveId.ToString())));
                        flag6 = true;
                    }
                }
                if (flag)
                {
                    char[] chArray = ((int)currentTask.OtherType).ToString().ToCharArray();
                    if (chArray.Length != 3)
                    {
                        return new ValueTuple<string, string, List<Step>>("", "加签参数错误!", list);
                    }
                    int num2 = ((char)chArray[1]).ToString().ToInt(-2147483648);
                    int num3 = ((char)chArray[2]).ToString().ToInt(-2147483648);
                    if (num3 == 3)
                    {

                        RoadFlow.Model.FlowTask task2 = groupTasks.Find(delegate (RoadFlow.Model.FlowTask p)
                        {
                            return ((p.StepId == currentTask.StepId) && (p.Sort == currentTask.Sort)) && (p.StepSort == (currentTask.StepSort + 1));
                        });
                        if (task2 != null)
                        {
                            list2.Add(new ValueTuple<Step, string>(stepModel, task2.ReceiveOrganizeId.HasValue ? ("r_" + task2.ReceiveOrganizeId.Value.ToString()) : ("u_" + task2.ReceiveId.ToString())));
                        }
                    }
                    if (!Enumerable.Any<ValueTuple<Step, string>>((IEnumerable<ValueTuple<Step, string>>)list2))
                    {
                        RoadFlow.Model.FlowTask task3 = this.Get(currentTask.PrevId);
                        if (task3 != null)
                        {
                            list2.Add(new ValueTuple<Step, string>(stepModel, task3.ReceiveOrganizeId.HasValue ? ("r_" + task3.ReceiveOrganizeId.Value.ToString()) : ("u_" + task3.ReceiveId.ToString())));
                        }
                    }
                }
                if (isFreeSend)
                {
                    list2.Add(new ValueTuple<Step, string>(stepModel, string.Empty));
                }
                if ((!flag6 && !isFreeSend) && !flag)
                {
                    List<Step> nextSteps = flow.GetNextSteps(flowRunModel, stepId);

                    foreach (Step step2 in nextSteps)
                    {
                        list2.Add(new ValueTuple<Step, string>(step2, string.Empty));
                    }
                }
            }


            if (list2.Count == 0)
            {
                return new ValueTuple<string, string, List<Step>>("", "当前步骤没有后续步骤", list);
            }
            User user = new User();
            for (int i = 0; i < list2.Count(); i++)
            {
                tuple2 = list2[i];
                Step nextStep = tuple2.Item1;
                string str2 = tuple2.Item2;

                if ((nextStep.StepBase.SkipIdenticalUser == 1) || !nextStep.StepBase.SkipMethod.IsNullOrWhiteSpace())
                {
                    bool flag20 = false;
                    if (nextStep.StepBase.SkipIdenticalUser == 1)
                    {
                        ValueTuple<string, string, string> tuple1 = this.GetDefaultMember(flowRunModel, stepModel, nextStep, groupTasks, taskId, instanceId, userId);
                        string organizeIds = tuple1.Item1;
                        string str4 = tuple1.Item2;
                        string str5 = tuple1.Item3;

                        if (user.Contains(organizeIds, userId))
                        {
                            flag20 = true;
                        }
                    }
                    if (!flag20 && !nextStep.StepBase.SkipMethod.IsNullOrWhiteSpace())
                    {
                        EventParam param = new EventParam
                        {
                            FlowId = flowRunModel.Id,
                            GroupId = groupId,
                            InstanceId = instanceId,
                            Other = currentTask,
                            StepId = stepId,
                            TaskId = taskId,
                            TaskTitle = (currentTask != null) ? currentTask.Title : string.Empty
                        };
                        object[] args = new object[] { param };
                        ValueTuple<object, Exception> tuple3 = Tools.ExecuteMethod(nextStep.StepBase.SkipMethod.Trim(), args);
                        object obj2 = tuple3.Item1;
                        Exception exception = tuple3.Item2;

                        if ((obj2 != null) && (obj2.ToString().Equals("1") || obj2.ToString().EqualsIgnoreCase("true")))
                        {
                            flag20 = true;
                        }
                    }
                    if (flag20)
                    {
                        list2.RemoveAt(i);
                        List<Step> list4 = flow.GetNextSteps(flowRunModel, nextStep.Id);

                        using (List<Step>.Enumerator enumerator2 = list4.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                Step nStep = enumerator2.Current;
                                if (nextStep.StepBase.FlowType == 0)
                                {
                                    string str6;
                                    Line lineModel = flowRunModel.Lines.Find(delegate (Line p)
                                    {
                                        return (p.FromId == nextStep.Id) && (p.ToId == nStep.Id);
                                    });
                                    if ((lineModel != null) && !this.LinePass(lineModel, flowRunModel, stepModel, groupTasks, taskId, instanceId, userId, out str6))
                                    {
                                        continue;
                                    }
                                }
                                list2.Add(new ValueTuple<Step, string>(nStep, string.Empty));
                            }
                        }
                    }
                }
            }
            Dictionary<Guid, string> nextStepsHandle = this.GetNextStepsHandle(groupTasks);
            StringBuilder builder = new StringBuilder();
            using (List<ValueTuple<Step, string>>.Enumerator enumerator3 = list2.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    tuple2 = enumerator3.Current;
                    Step nextStep2 = tuple2.Item1;
                    string str7 = tuple2.Item2;
                    if ((!isFreeSend && (stepModel.StepBase.HanlderModel != 4)) && (stepModel.StepBase.FlowType == 0))
                    {
                        string str18;
                        Line line2 = flowRunModel.Lines.Find(delegate (Line p)
                        {
                            return (p.FromId == stepModel.Id) && (p.ToId == nextStep2.Id);
                        });
                        if ((line2 != null) && !this.LinePass(line2, flowRunModel, stepModel, groupTasks, taskId, instanceId, userId, out str18))
                        {
                            continue;
                        }
                    }
                    string str = str7;
                    if (nextStepsHandle.ContainsKey(nextStep2.Id))
                    {
                        str = nextStepsHandle[nextStep2.Id];
                    }
                    ValueTuple<string, string, string> tuple4 = !str.IsNullOrWhiteSpace() ? new ValueTuple<string, string, string>(str, "unit='1' dept='1' station='1' workgroup='1' user='1'", str) : this.GetDefaultMember(flowRunModel, stepModel, nextStep2, groupTasks, taskId, instanceId, userId);
                    string str9 = tuple4.Item1;
                    string str10 = tuple4.Item2;
                    string selectRange = tuple4.Item3;
                    if (!nextStep2.StepBase.SelectRange.IsNullOrWhiteSpace())
                    {
                        selectRange = nextStep2.StepBase.SelectRange;
                    }

                    if (((nextStep2.Dynamic == 2) && !nextStep2.DynamicField.IsNullOrWhiteSpace()) && (flowRunModel.Databases.Count > 0))
                    {
                        builder.Append(this.GetDynamicFieldStepHtml(flowRunModel, nextStep2, instanceId));
                        list.Add(nextStep2);
                    }
                    else
                    {


                        string str12 = string.Empty;
                        string str13 = (stepModel.StepBase.FlowType == 0) ? " disabled=\"disbaled\"" : "";
                        string str14 = (stepModel.StepBase.FlowType == 1) ? "radio" : "checkbox";
                        string str15 = (nextStep2.StepBase.RunSelect == 0) ? " disabled=\"disbaled\"" : "";
                        string str16 = selectRange.IsNullOrWhiteSpace() ? string.Empty : " isChangeType=\"0\"";
                        int[] digits = new int[] { 1, 2 };

                        if (stepModel.StepBase.FlowType.In(digits) && (nextStep2.Id == Enumerable.First<ValueTuple<Step, string>>((IEnumerable<ValueTuple<Step, string>>)list2).Item1.Id))
                        {
                            str12 = " checked=\"checked\"";
                        }
                        else
                        {
                            int[] numArray2 = new int[2];
                            numArray2[1] = 3;
                            if (stepModel.StepBase.FlowType.In(numArray2))
                            {
                                str12 = " checked=\"checked\"";
                            }
                        }
                        bool flag31 = (1 == nextStep2.SendSetWorkTime) && !flag;

                        builder.Append("<tr");
                        string str17 = Guid.NewGuid().ToLowerNString();
                        if (nextStep2.Dynamic == 1)
                        {
                            string[] textArray1 = new string[] { " data-dynamic=\"1\" id=\"", str17, "\" data-beforestepid=\"", nextStep2.Id.ToString(), "\"" };
                            builder.Append(string.Concat((string[])textArray1));
                        }
                        builder.Append("><td>");



                        string[] textArray2 = new string[] { "<input type=\"", str14, "\"", str12, str13, " value=\"", nextStep2.Id.ToString(), "\" id=\"step_", nextStep2.Id.ToString(), "\" name=\"step\" style=\"vertical-align:middle;\" />" };
                        builder.Append(string.Concat((string[])textArray2));
                        string[] textArray3 = new string[] { "<label for=\"step_", nextStep2.Id.ToString(), "\" style=\"vertical-align:middle;\">", nextStep2.Name, "</label>" };
                        builder.Append(string.Concat((string[])textArray3));
                        string[] textArray4 = new string[] { "<input type=\"hidden\" id=\"name_", nextStep2.Id.ToString(), "\" value=\"", nextStep2.Name, "\" />" };
                        builder.Append(string.Concat((string[])textArray4));
                        if (nextStep2.Dynamic == 1)
                        {
                            string[] textArray5 = new string[] { "<input type=\"hidden\" id=\"before_", nextStep2.Id.ToString(), "\" value=\"", nextStep2.Id.ToString(), "\" />" };
                            builder.Append(string.Concat((string[])textArray5));
                            builder.Append("<label class=\"flowsendstepadd\">");
                            object[] objArray2 = new object[] { "<i onclick=\"dynamicAdd('", str17, "', '", nextStep2.Id.ToString(), "', ", (int)nextStep2.SendSetWorkTime, ", this);\" class=\"fa fa-plus-square-o\" title=\"", (localizer == null) ? "添加步骤" : localizer["FlowSend_AddStep"], "\"></i>" };
                            builder.Append(string.Concat((object[])objArray2));
                            builder.Append("</label>");
                            builder.Append("<label style=\"margin-left:6px;\">");
                            string[] textArray6 = new string[] { "<input type=\"radio\" value=\"0\" checked=\"checked\" title=\"", (localizer == null) ? "添加的步骤并行审批" : localizer["FlowSend_AddStepParallel"], "\" id=\"parallelorserial_", nextStep2.Id.ToString(), "_0\" name=\"parallelorserial_", nextStep2.Id.ToString(), "\" style=\"vertical-align:middle;\"/>" };
                            builder.Append(string.Concat((string[])textArray6));
                            string[] textArray7 = new string[] { "<label for=\"parallelorserial_", nextStep2.Id.ToString(), "_0\" title=\"", (localizer == null) ? "添加的步骤并行审批" : localizer["FlowSend_AddStepParallel"], "\" style=\"vertical-align:middle;\">", (localizer == null) ? "并行审批" : localizer["FlowSend_AddStepParallel1"], "</label>" };
                            builder.Append(string.Concat((string[])textArray7));
                            string[] textArray8 = new string[] { "<input type=\"radio\" value=\"1\" title=\"", (localizer == null) ? "添加的步骤串行审批" : localizer["FlowSend_AddStepSerial"], "\" id=\"parallelorserial_", nextStep2.Id.ToString(), "_1\" name=\"parallelorserial_", nextStep2.Id.ToString(), "\" style=\"vertical-align:middle;\"/>" };
                            builder.Append(string.Concat((string[])textArray8));
                            string[] textArray9 = new string[] { "<label for=\"parallelorserial_", nextStep2.Id.ToString(), "_1\" title=\"", (localizer == null) ? "添加的步骤顺序审批" : localizer["FlowSend_AddStepSerial1"], "\" style=\"vertical-align:middle;\">", (localizer == null) ? "顺序审批" : localizer["FlowSend_AddStepSerial2"], "</label>" };
                            builder.Append(string.Concat((string[])textArray9));
                            builder.Append("</label>");

                        }
                        builder.Append("</td></tr>");
                        builder.Append("<tr");
                        if (nextStep2.Dynamic == 1)
                        {
                            string[] textArray10 = new string[] { " data-dynamic=\"1\" id=\"", str17, "_1\" data-beforestepid=\"", nextStep2.Id.ToString(), "\"" };
                            builder.Append(string.Concat((string[])textArray10));
                        }
                        builder.Append("><td style=\"padding: 2px 0 4px 0;\">");
                        string[] textArray11 = new string[] {
                    "<input type=\"text\" class=\"mymember\" opener=\"parent\" ismobile=\"", isMobile ? "1" : "0", "\" id=\"user_", nextStep2.Id.ToString(), "\" name=\"user_", nextStep2.Id.ToString(), "\" value=\"", str9, "\" ", str10, str15, str16, " rootid=\"", selectRange, "\" style=\"width:", (isMobile & flag31) ? "21%" : "43%",
                    "\" />"
                 };
                        builder.Append(string.Concat((string[])textArray11));
                        if (flag31)
                        {
                            builder.Append("<span style=\"margin-left:5px;\">完成时间：</span>");
                            string[] textArray12 = new string[] { "<input type=\"text\" class=\"mycalendar\" istime=\"1\" dayafter=\"1\" value=\"\" style=\"width:120px;\" id=\"CompletedTime_", nextStep2.Id.ToString(), "\" name=\"CompletedTime_", nextStep2.Id.ToString(), "\" />" };
                            builder.Append(string.Concat((string[])textArray12));
                        }
                        builder.Append("</td></tr>");
                        nextStep2.RunDefaultMembers = str9;
                        list.Add(nextStep2);
                    }
                }
            }
            return new ValueTuple<string, string, List<Step>>(builder.ToString(), "", list);

        }


        /// <summary>
        /// 得到步骤由流程处理人设置的后续步骤处理人员
        /// </summary>
        /// <param name="groupTasks">实例组列表</param>
        /// <returns></returns>

        public Dictionary<Guid, string> GetNextStepsHandle(List<RoadFlow.Model.FlowTask> groupTasks)
        {
            Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
            foreach (RoadFlow.Model.FlowTask task in Enumerable.OrderBy<RoadFlow.Model.FlowTask, int>((IEnumerable<RoadFlow.Model.FlowTask>)groupTasks,
                key=>key.Sort))
            {
                if (!task.NextStepsHandle.IsNullOrWhiteSpace())
                {
                    try
                    {
                        foreach (JObject obj1 in JArray.Parse(task.NextStepsHandle))
                        {
                            Guid guid;
                            string str = obj1.Value<string>("handle");
                            if (obj1.Value<string>("stepId").IsGuid(out guid) && !str.IsNullOrWhiteSpace())
                            {
                                if (dictionary.ContainsKey(guid))
                                {
                                    dictionary[guid] = str;
                                }
                                else
                                {
                                    dictionary.Add(guid, str);
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return dictionary;
        }


        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public string GetPrevInstanceID(string taskId)
        {
            Guid guid;
            if (taskId.IsGuid(out guid))
            {
                RoadFlow.Model.FlowTask task = this.Get(guid);
                if ((task == null) || task.PrevId.IsEmptyGuid())
                {
                    return string.Empty;
                }
                RoadFlow.Model.FlowTask task2 = this.Get(task.PrevId);
                if (task2 != null)
                {
                    return task2.InstanceId;
                }
            }
            return string.Empty;
        }


        /// <summary>
        /// 获取前实例id
        /// </summary>
        /// <param name="groupTasks"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public string GetPrevStepHandler(List<RoadFlow.Model.FlowTask> groupTasks, Guid taskId)
        {
            RoadFlow.Model.FlowTask currentTask = groupTasks.Find(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.Id == taskId;
            });
            if (currentTask == null)
            {
                return string.Empty;
            }
            RoadFlow.Model.FlowTask prevTask = groupTasks.Find(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.Id == currentTask.PrevId;
            });
            if (prevTask == null)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.FlowTask task in groupTasks.FindAll(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.PrevId == prevTask.PrevId;
            }))
            {
                if (task.TaskType != 5)
                {
                    if (task.ReceiveOrganizeId.HasValue)
                    {
                        builder.Append("r_" + task.ReceiveOrganizeId.Value.ToString());
                    }
                    else
                    {
                        builder.Append("u_" + task.ReceiveId.ToString());
                    }
                    builder.Append(",");
                }
            }
            char[] trimChars = new char[] { ',' };
            return builder.ToString().TrimEnd(trimChars);
        }


        /// <summary>
        /// 获取前标题
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public string GetPrevTitle(string taskId)
        {
            Guid guid;
            if (taskId.IsGuid(out guid))
            {
                RoadFlow.Model.FlowTask task = this.Get(guid);
                if ((task == null) || task.PrevId.IsEmptyGuid())
                {
                    return string.Empty;
                }
                RoadFlow.Model.FlowTask task2 = this.Get(task.PrevId);
                if (task2 != null)
                {
                    return task2.Title;
                }
            }
            return string.Empty;
        }



        /// <summary>
        /// 获取步骤处理
        /// </summary>
        /// <param name="groupTasks"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public string GetStepHandler(List<RoadFlow.Model.FlowTask> groupTasks, Guid stepId)
        {
            List<RoadFlow.Model.FlowTask> list = groupTasks.FindAll(delegate (RoadFlow.Model.FlowTask p)
            {
                return (p.StepId == stepId) && (p.TaskType != 5);
            });
            if (!Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list))
            {
                return string.Empty;
            }
            int maxSort = Enumerable.Max<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list, 
                key=>key.Sort);
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.FlowTask task in list.FindAll(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.Sort == maxSort;
            }))
            {
                if (task.ReceiveOrganizeId.HasValue)
                {
                    builder.Append("r_" + task.ReceiveOrganizeId.Value.ToString());
                }
                else
                {
                    builder.Append("u_" + task.ReceiveId.ToString());
                }
                builder.Append(",");
            }
            char[] trimChars = new char[] { ',' };
            return builder.ToString().TrimEnd(trimChars);
        }
        /// <summary>
        /// 获取步骤名称
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public string GetStepName(RoadFlow.Model.FlowRun flowRunModel, Guid stepId)
        {
            if (flowRunModel != null)
            {
                Step step = flowRunModel.Steps.Find(delegate (Step p)
                {
                    return p.Id == stepId;
                });
                if (step != null)
                {
                    return step.Name;
                }
            }
            return string.Empty;
        }



        /// <summary>
        /// 获取任务实例步骤
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="groupTasks"></param>
        /// <returns></returns>
        public List<Step> GetTaskInstanceSteps(RoadFlow.Model.FlowRun flowRunModel, List<RoadFlow.Model.FlowTask> groupTasks)
        {
            List<Step> list = new List<Step>();
            using (IEnumerator<IGrouping<Guid, RoadFlow.Model.FlowTask>> enumerator = Enumerable.GroupBy<RoadFlow.Model.FlowTask, Guid>((IEnumerable<RoadFlow.Model.FlowTask>)Enumerable.OrderBy<RoadFlow.Model.FlowTask, int>((IEnumerable<RoadFlow.Model.FlowTask>)groupTasks, 
                key=>key.Sort),
                key=> key.StepId).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    IGrouping<Guid, RoadFlow.Model.FlowTask> step = enumerator.Current;
                    Step step1 = flowRunModel.Steps.Find(delegate (Step p)
                    {
                        return p.Id == step.Key;
                    });
                    if (step1 != null)
                    {
                        list.Add(step1);
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// 获取等待任务
        /// </summary>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="userId"></param>
        /// <param name="flowId"></param>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="order"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataTable GetWaitTask(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count, int isBatch = 0)
        {
            return this.flowTaskData.GetWaitList(size, number, userId, flowId, title, startDate, endDate, order, out count, isBatch);
        }



        /// <summary>
        /// 跳转
        /// </summary>
        /// <param name="currentTask"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        public string GoTo(RoadFlow.Model.FlowTask currentTask, Dictionary<Guid, List<RoadFlow.Model.User>> steps)
        {
            if (currentTask == null)
            {
                return "当前任务为空!";
            }
            List<RoadFlow.Model.FlowTask> nextTasks = new List<RoadFlow.Model.FlowTask>();
            List<RoadFlow.Model.FlowTask> updateTasks = new List<RoadFlow.Model.FlowTask>();
            List<RoadFlow.Model.FlowTask> listByGroupId = this.GetListByGroupId(currentTask.GroupId);
            int maxSort = Enumerable.Max<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId.FindAll(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.StepId == currentTask.StepId;
            }),key=>key.Sort);
            foreach (RoadFlow.Model.FlowTask task in listByGroupId.FindAll(delegate (RoadFlow.Model.FlowTask p)
            {
                return (p.StepId == currentTask.StepId) && (p.Sort == maxSort);
            }))
            {
                if (task.Status != 2)
                {
                    task.Status = 2;
                    task.ExecuteType = 10;
                    task.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                    updateTasks.Add(task);
                }
            }
            RoadFlow.Model.FlowRun flowRunModel = new Flow().GetFlowRunModel(currentTask.FlowId, true, currentTask);
            if (flowRunModel == null)
            {
                return "未找到流程运行时实体";
            }
            FlowEntrust entrust = new FlowEntrust();
            User user = new User();
            using (Dictionary<Guid, List<RoadFlow.Model.User>>.Enumerator enumerator2 = steps.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    KeyValuePair<Guid, List<RoadFlow.Model.User>> step = enumerator2.Current;
                    Step step1 = flowRunModel.Steps.Find(delegate (Step p)
                    {
                        return p.Id == step.Key;
                    });
                    if (step1 != null)
                    {
                        for (int i = 0; i < step.Value.Count; i++)
                        {
                            RoadFlow.Model.User user1 = step.Value[i];
                            string entrustUserId = entrust.GetEntrustUserId(currentTask.FlowId, user1);
                            bool flag = !entrustUserId.IsNullOrWhiteSpace();
                            if (flag)
                            {
                                RoadFlow.Model.User user2 = user.Get(entrustUserId);
                                if (user2 != null)
                                {
                                    RoadFlow.Model.User user3 = user2.Clone();
                                    user3.Note = user1.Id.ToString();
                                    user1 = user3;
                                }
                            }
                            if (!nextTasks.Exists(delegate (RoadFlow.Model.FlowTask p)
                            {
                                return (p.ReceiveId == user1.Id) && (p.StepId == currentTask.StepId);
                            }) && !listByGroupId.Exists(delegate (RoadFlow.Model.FlowTask p)
                            {
                                return ((p.ReceiveId == user1.Id) && (p.StepId == currentTask.StepId)) && (p.Status != 2);
                            }))
                            {
                                Guid guid;
                                RoadFlow.Model.FlowTask task2 = new RoadFlow.Model.FlowTask();
                                if (step1.WorkTime > decimal.Zero)
                                {
                                    task2.CompletedTime = new DateTime?(DateTimeExtensions.Now.AddDays((double)((double)step1.WorkTime)));
                                }
                                if (user1.PartTimeId.HasValue)
                                {
                                    task2.ReceiveOrganizeId = user1.PartTimeId;
                                }
                                if (flag && user1.Note.IsGuid(out guid))
                                {
                                    task2.EntrustUserId = new Guid?(guid);
                                }
                                task2.ExecuteType = 0;
                                task2.FlowId = currentTask.FlowId;
                                task2.FlowName = currentTask.FlowName;
                                task2.GroupId = currentTask.GroupId;
                                task2.Id = Guid.NewGuid();
                                task2.InstanceId = currentTask.InstanceId;
                                task2.IsAutoSubmit = step1.ExpiredExecuteModel;
                                task2.IsSign = 0;
                                task2.Note = "跳转任务";
                                task2.PrevId = currentTask.Id;
                                task2.PrevStepId = currentTask.StepId;
                                task2.ReceiveId = user1.Id;
                                task2.ReceiveName = user1.Name;
                                task2.ReceiveTime = DateTimeExtensions.Now;
                                task2.SenderId = currentTask.ReceiveId;
                                task2.SenderName = currentTask.ReceiveName;
                                task2.Sort = currentTask.Sort + 1;
                                task2.Status = 0;
                                task2.StepId = step1.Id;
                                task2.StepName = step1.Name;
                                task2.StepSort = 1;
                                task2.TaskType = 9;
                                task2.Title = currentTask.Title;
                                task2.OtherType = currentTask.OtherType;
                                task2.IsBatch = new int?(step1.BatchExecute);

                                nextTasks.Add(task2);
                            }
                        }
                    }
                }
            }
            this.SendMessage(nextTasks, User.CurrentUser, "", "");
            if (this.Update(null, updateTasks, nextTasks, null) <= 0)
            {
                return "没有跳转给任何人员!";
            }
            return "1";
        }


        /// <summary>
        /// 是否删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="flowRunModel"></param>
        /// <returns></returns>
        public bool IsDelete(Guid taskId, RoadFlow.Model.FlowRun flowRunModel = null)
        {
            RoadFlow.Model.FlowTask task = this.Get(taskId);
            if (task == null)
            {
                return false;
            }
            if (flowRunModel == null)
            {
                flowRunModel = new Flow().GetFlowRunModel(task.FlowId, true, null);
            }
            if (flowRunModel == null)
            {
                return false;
            }
            return ((task.StepId == flowRunModel.FirstStepId) && (task.ReceiveId == this.GetFirstSenderId(task.GroupId)));
        }



        /// <summary>
        /// 是否委托收回
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool IsEntrustWithdraw(Guid taskId)
        {
            RoadFlow.Model.FlowTask task = this.Get(taskId);
            if (task == null)
            {
                return false;
            }
            return (task.Status < 1);
        }


        /// <summary>
        /// 是否加速
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="isWithdraw"></param>
        /// <returns></returns>
        public bool IsHasten(Guid taskId, out bool isWithdraw)
        {
            isWithdraw = false;
            RoadFlow.Model.FlowTask task = this.Get(taskId);
            if (task == null)
            {
                return false;
            }
            List<RoadFlow.Model.FlowTask> listByGroupId = this.GetListByGroupId(task.GroupId);
            List<RoadFlow.Model.FlowTask> list = listByGroupId.FindAll(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.PrevId == taskId;
            });
            isWithdraw = Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list) && !Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list, 
               key=> (key.Status > 0));
            return listByGroupId.Exists(delegate (RoadFlow.Model.FlowTask p)
            {
                if (p.PrevId == taskId)
                {
                    int[] digits = new int[2];
                    digits[1] = 1;
                    return p.Status.In(digits);
                }
                return false;
            });
        }


        /// <summary>
        /// 线通过限制
        /// </summary>
        /// <param name="lineModel"></param>
        /// <param name="flowRunModel"></param>
        /// <param name="currentStepModel"></param>
        /// <param name="groupTasks"></param>
        /// <param name="taskId"></param>
        /// <param name="instanceId"></param>
        /// <param name="currentUserId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool LinePass(Line lineModel, RoadFlow.Model.FlowRun flowRunModel, Step currentStepModel, List<RoadFlow.Model.FlowTask> groupTasks, Guid taskId, string instanceId, Guid currentUserId, out string msg)
        {
            msg = string.Empty;
            if (lineModel == null)
            {
                return false;
            }
            bool flag = this.LinePass_SqlWhere(lineModel.SqlWhere, flowRunModel, instanceId);
            if (!(((lineModel.JudgeType != 1) && !lineModel.SqlWhere.IsNullOrWhiteSpace()) & flag))
            {
                JArray array;
                try
                {
                    array = JArray.Parse(lineModel.OrganizeExpression);
                }
                catch
                {
                    array = new JArray();
                }
                bool flag2 = this.LinePass_Organize(array, flowRunModel, currentStepModel, currentUserId, groupTasks);
                if (((lineModel.JudgeType != 1) && (array.Count > 0)) & flag2)
                {
                    return true;
                }
                bool flag3 = this.LinePass_Method(lineModel.CustomMethod, flowRunModel, currentStepModel, groupTasks, instanceId, taskId, out msg);
                if (((lineModel.JudgeType != 1) && !lineModel.CustomMethod.IsNullOrWhiteSpace()) & flag3)
                {
                    return true;
                }
                if (lineModel.JudgeType == 1)
                {
                    return ((flag & flag2) & flag3);
                }
                if ((!flag || lineModel.SqlWhere.IsNullOrWhiteSpace()) && (!flag2 || (array.Count <= 0)))
                {
                    return (flag3 && !lineModel.CustomMethod.IsNullOrWhiteSpace());
                }
            }
            return true;

        }

        private bool LinePass_Method(string method, RoadFlow.Model.FlowRun flowRunModel, Step currentStepModel, List<RoadFlow.Model.FlowTask> groupTasks, string instanceId, Guid taskId, out string msg)
        {
            msg = string.Empty;
            if (!method.IsNullOrWhiteSpace())
            {
                EventParam param = new EventParam
                {
                    FlowId = flowRunModel.Id,
                    GroupId = (groupTasks.Count > 0) ? Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)groupTasks).GroupId : Guid.Empty,
                    InstanceId = instanceId,
                    StepId = currentStepModel.Id,
                    TaskId = taskId
                };
                object[] args = new object[] { param };
                ValueTuple<object, Exception> tuple1 = Tools.ExecuteMethod(method.Trim(), args);
                object obj2 = tuple1.Item1;
                if (obj2 == null)
                {
                    return false;
                }
                msg = obj2.ToString();
                if (!"1".Equals(msg.ToLower()))
                {
                    return "true".Equals(msg.ToLower());
                }
            }
            return true;
        }





        private bool LinePass_Organize(JArray organizeExpressionJArray, RoadFlow.Model.FlowRun flowRunModel, Step currentStepModel, Guid currentUserId, List<RoadFlow.Model.FlowTask> groupTasks)
        {
            if (organizeExpressionJArray.Count == 0)
            {
                return true;
            }
            Guid guid = currentUserId;
            Guid firstSenderId = this.GetFirstSenderId(groupTasks);
            new Organize();
            User user = new User();
            if (firstSenderId.IsEmptyGuid() && (currentStepModel.Id == flowRunModel.FirstStepId))
            {
                firstSenderId = guid;
            }
            StringBuilder builder = new StringBuilder();
            foreach (JObject obj2 in organizeExpressionJArray)
            {
                if (obj2.Count != 0)
                {
                    string str = obj2.Value<string>("usertype");
                    string str2 = obj2.Value<string>("in1");
                    string str3 = obj2.Value<string>("users");
                    string str4 = obj2.Value<string>("selectorganize");
                    string str5 = obj2.Value<string>("tjand");
                    string str6 = obj2.Value<string>("khleft");
                    string str7 = obj2.Value<string>("khright");
                    Guid guid3 = "0".Equals(str) ? guid : firstSenderId;
                    string memberIds = "";
                    bool flag = false;
                    ValueTuple<string, string> leader = user.GetLeader(guid3.ToString());
                    if ("0".Equals(str3))
                    {
                        memberIds = str4;
                    }
                    else if ("1".Equals(str3))
                    {
                        memberIds = leader.Item1;
                    }
                    else if ("2" == str3)
                    {
                        memberIds = leader.Item2;
                    }
                    if ("0" == str2)
                    {
                        flag = user.IsIn(guid3.ToString(), memberIds);
                    }
                    else if ("1" == str2)
                    {
                        flag = !user.IsIn(guid3.ToString(), memberIds);
                    }
                    if (!str6.IsNullOrEmpty())
                    {
                        builder.Append(str6);
                    }
                    builder.Append(flag ? ((string)" true ") : ((string)" false "));
                    if (!str7.IsNullOrEmpty())
                    {
                        builder.Append(str7);
                    }
                    builder.Append(str5);
                }
            }
            return ((builder.Length == 0) || ((bool)((bool)Tools.ExecuteExpression(builder.ToString()))));
        }



        private bool LinePass_SqlWhere(string sqlWhere, RoadFlow.Model.FlowRun flowRunModel, string instanceId)
        {
            if (sqlWhere.IsNullOrWhiteSpace())
            {
                return true;
            }
            if ((flowRunModel == null) || (flowRunModel.Databases.Count == 0))
            {
                return false;
            }
            DbConnection connection = new DbConnection();
            if (((Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases) == null) || Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases).ConnectionId.IsEmptyGuid()) || (Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases).Table.IsNullOrWhiteSpace() || Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases).PrimaryKey.IsNullOrWhiteSpace()))
            {
                return false;
            }
            RoadFlow.Model.DbConnection dbConnectionModel = connection.Get(Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases).ConnectionId);
            if (dbConnectionModel == null)
            {
                return false;
            }
            string[] textArray1 = new string[] { "SELECT ", Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases).PrimaryKey, " FROM ", Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases).Table, " WHERE ", Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases).PrimaryKey, "={0}", sqlWhere.Trim().ToLower().StartsWith("and") ? " " : " AND ", sqlWhere };
            string sql = string.Concat((string[])textArray1);
            try
            {
                object[] objects = new object[] { instanceId };
                return (connection.GetDataTable(dbConnectionModel, sql, objects).Rows.Count > 0);
            }
            catch
            {
                return false;
            }
        }







        /// <summary>
        /// 转交
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        public void Redirect(RoadFlow.Model.FlowRun flowRunModel, Execute executeModel)
        {
            List<RoadFlow.Model.FlowTask> listByGroupId = this.GetListByGroupId(executeModel.GroupId);
            RoadFlow.Model.FlowTask currentTask = listByGroupId.Find(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.Id == executeModel.TaskId;
            });
            if (currentTask == null)
            {
                this.executeResult.DebugMessages = "当前任务为空";
                this.executeResult.IsSuccess = false;
                this.executeResult.Messages = "当前任务为空";
            }
            else
            {
                int[] digits = new int[] { -1, 2 };
                if (currentTask.Status.In(digits))
                {
                    this.executeResult.DebugMessages = "当前任务" + ((currentTask.Status == -1) ? "等待中" : "已处理") + "!";
                    this.executeResult.IsSuccess = false;
                    this.executeResult.Messages = this.executeResult.DebugMessages;
                }
                else if ((executeModel.Steps.Count == 0) || (Enumerable.First<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)executeModel.Steps).Item5.Count == 0))
                {
                    this.executeResult.DebugMessages = "要转交的步骤或接收人员为空";
                    this.executeResult.IsSuccess = false;
                    this.executeResult.Messages = "接收人员为空";
                }
                else
                {
                    currentTask.ExecuteType = 6;
                    currentTask.Status = 2;
                    currentTask.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                    this.updateTasks.Add(currentTask);
                    this.executeResult.CurrentTask = currentTask;
                    FlowEntrust entrust = new FlowEntrust();
                    User user = new User();
                    List<RoadFlow.Model.User> list2 = Enumerable.First<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)executeModel.Steps).Item5;

                    for (int i = 0; i < list2.Count; i++)
                    {
                        RoadFlow.Model.User user1 = list2[i];
                        string entrustUserId = entrust.GetEntrustUserId(currentTask.FlowId, user1);
                        bool flag = !entrustUserId.IsNullOrWhiteSpace();
                        if (flag)
                        {
                            RoadFlow.Model.User user2 = user.Get(entrustUserId);
                            if (user2 != null)
                            {
                                RoadFlow.Model.User user3 = user2.Clone();
                                user3.Note = user1.Id.ToString();
                                user1 = user3;
                            }
                        }
                        if (!this.addTasks.Exists(delegate (RoadFlow.Model.FlowTask p)
                        {
                            return (p.ReceiveId == user1.Id) && (p.StepId == currentTask.StepId);
                        }) && !listByGroupId.Exists(delegate (RoadFlow.Model.FlowTask p)
                        {
                            return ((p.ReceiveId == user1.Id) && (p.StepId == currentTask.StepId)) && (p.Status != 2);
                        }))
                        {
                            Guid guid;
                            RoadFlow.Model.FlowTask task = currentTask.Clone();
                            task.Id = Guid.NewGuid();
                            task.SenderId = currentTask.ReceiveId;
                            task.SenderName = currentTask.ReceiveName;
                            task.ExecuteType = 0;
                            task.Status = 0;
                            task.Comments = "";
                            task.IsSign = 0;
                            task.ReceiveName = user1.Name;
                            task.ReceiveId = user1.Id;
                            task.CompletedTime1 = null;
                            task.ReceiveTime = DateTimeExtensions.Now;
                            task.Note = "转交任务";
                            if (user1.PartTimeId.HasValue)
                            {
                                task.ReceiveOrganizeId = user1.PartTimeId;
                            }
                            if (flag && user1.Note.IsGuid(out guid))
                            {
                                task.EntrustUserId = new Guid?(guid);
                            }
                            this.addTasks.Add(task);
                            this.executeResult.NextTasks.Add(task);
                        }
                    }
                    this.executeResult.IsSuccess = true;
                }
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        public void Save(RoadFlow.Model.FlowRun flowRunModel, Execute executeModel)
        {
            if ((executeModel.StepId == flowRunModel.FirstStepId) && executeModel.TaskId.IsEmptyGuid())
            {
                RoadFlow.Model.FlowTask firstTask = this.GetFirstTask(flowRunModel, executeModel);
                this.addTasks.Add(firstTask);
                this.executeResult.NextTasks.Add(firstTask);
                this.executeResult.CurrentTask = firstTask;
            }
            else
            {
                RoadFlow.Model.FlowTask task2 = this.Get(executeModel.TaskId);
                if (task2 == null)
                {
                    this.executeResult.DebugMessages = "当前任务为空!";
                    this.executeResult.IsSuccess = false;
                    this.executeResult.Messages = "当前任务为空!";
                    return;
                }
                int[] digits = new int[] { -1, 2 };
                if (task2.Status.In(digits))
                {
                    this.executeResult.DebugMessages = "当前任务" + ((task2.Status == -1) ? "等待中" : "已处理") + "!";
                    this.executeResult.IsSuccess = false;
                    this.executeResult.Messages = this.executeResult.DebugMessages;
                    return;
                }
                if (!executeModel.Title.IsNullOrWhiteSpace())
                {
                    task2.Title = executeModel.Title;
                }
                task2.InstanceId = executeModel.InstanceId;
                task2.Comments = executeModel.Comment;
                task2.Attachment = executeModel.Attachment;
                this.updateTasks.Add(task2);
                this.executeResult.NextTasks.Add(task2);
                this.executeResult.CurrentTask = task2;
            }
            this.executeResult.IsSuccess = true;
        }



        //***************************************注意**********
        //[AsyncStateMachine((typeof(SendMessaged__39)))]
        //public void SendMessage(List<RoadFlow.Model.FlowTask> nextTasks, RoadFlow.Model.User sender, string sendModel = "", string contents = "")
        //{
        //    SendMessaged__31 d__ = new SendMessaged__31();
        //    d__.nextTasks = nextTasks;
        //    d__.sender = sender;
        //    d__.sendModel = sendModel;
        //    d__.contents = contents;
        //    d__.s_t__builder = AsyncVoidMethodBuilder.Create();
        //    d__.s_1__state = -1;
        //    d__.s_t__builder.Start<SendMessaged__31>(ref d__);
        //}




        [AsyncStateMachine(typeof(SendMessaged__39))]
        public void SendMessage(List<RoadFlow.Model.FlowTask> nextTasks, RoadFlow.Model.User sender, string sendModel = "", string contents = "")
        {
            SendMessaged__39 d__ = new SendMessaged__39();
            d__.nextTasks = nextTasks;
            d__.sender = sender;
            d__.sendModel = sendModel;
            d__.contents = contents;
            d__.s_t__builder = AsyncVoidMethodBuilder.Create();
            d__.s_1__state = -1;
            d__.s_t__builder.Start<SendMessaged__39>(ref d__);
        }


        /// <summary>
        /// 开始流程
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="instanceId"></param>
        /// <param name="title"></param>
        /// <param name="sender"></param>
        /// <param name="users"></param>
        /// <param name="completedTime"></param>
        /// <returns></returns>
        public ExecuteResult StartFlow(Guid flowId, string instanceId, string title, RoadFlow.Model.User sender, List<RoadFlow.Model.User> users, DateTime? completedTime = new DateTime?())
        {
            ExecuteResult result = new ExecuteResult();
            if (flowId.IsEmptyGuid())
            {
                result.Messages = "流程ID为空";
                result.DebugMessages = result.Messages;
                result.IsSuccess = false;
                return result;
            }
            if ((users == null) || !Enumerable.Any<RoadFlow.Model.User>((IEnumerable<RoadFlow.Model.User>)users))
            {
                result.Messages = "没有接收人";
                result.DebugMessages = result.Messages;
                result.IsSuccess = false;
                return result;
            }
            RoadFlow.Model.FlowRun flowRunModel = new Flow().GetFlowRunModel(flowId, true, null);
            if (flowRunModel == null)
            {
                result.Messages = "未找到流程运行时实体";
                result.DebugMessages = result.Messages;
                result.IsSuccess = false;
                return result;
            }
            if (flowRunModel.Status != 1)
            {
                result.Messages = "流程未发布";
                result.DebugMessages = result.Messages + " - 流程状态：" + ((int)flowRunModel.Status).ToString();
                result.IsSuccess = false;
                return result;
            }
            Step step = flowRunModel.Steps.Find(delegate (Step p)
            {
                return p.Id == flowRunModel.FirstStepId;
            });
            if (step == null)
            {
                result.Messages = "流程没有第一步";
                result.DebugMessages = result.Messages;
                result.IsSuccess = false;
                return result;
            }
            List<RoadFlow.Model.FlowTask> addTasks = new List<RoadFlow.Model.FlowTask>();
            Guid guid = Guid.NewGuid();
            FlowEntrust entrust = new FlowEntrust();
            User user = new User();
            for (int i = 0; i < users.Count; i++)
            {
                Guid guid2;
                RoadFlow.Model.User user2 = users[i];
                string entrustUserId = entrust.GetEntrustUserId(flowId, user2);
                bool flag1 = (bool)!entrustUserId.IsNullOrWhiteSpace();
                if (flag1)
                {
                    RoadFlow.Model.User user3 = user.Get(entrustUserId);
                    if (user3 != null)
                    {
                        RoadFlow.Model.User user1 = user3.Clone();
                        user1.Note = user2.Id.ToString();
                        user2 = user1;
                    }
                }
                RoadFlow.Model.FlowTask task = new RoadFlow.Model.FlowTask();
                DateTime? nullable = completedTime;
                task.CompletedTime = nullable.HasValue ? nullable : ((step.WorkTime > decimal.Zero) ? new DateTime?(DateTimeExtensions.Now.AddDays((double)((double)step.WorkTime))) : null);
                if (flag1 && user2.Note.IsGuid(out guid2))
                {
                    task.EntrustUserId = new Guid?(guid2);
                }
                task.ExecuteType = 0;
                task.FlowId = flowId;
                task.FlowName = flowRunModel.Name;
                task.GroupId = guid;
                task.Id = Guid.NewGuid();
                task.InstanceId = instanceId;
                task.IsAutoSubmit = step.ExpiredExecuteModel;
                task.PrevId = Guid.Empty;
                task.PrevStepId = Guid.Empty;
                task.ReceiveId = user2.Id;
                task.ReceiveName = user2.Name;
                task.ReceiveOrganizeId = user2.PartTimeId;
                task.ReceiveTime = DateTimeExtensions.Now;
                task.SenderId = (sender == null) ? Guid.Empty : sender.Id;
                task.SenderName = (sender == null) ? "" : sender.Name;
                task.Sort = 1;
                task.Status = 0;
                task.StepId = flowRunModel.FirstStepId;
                task.StepName = step.Name;
                task.StepSort = 1;
                task.TaskType = 0;
                task.Title = title.IsNullOrWhiteSpace() ? string.Concat((string[])new string[] { flowRunModel.Name, " - ", step.Name, " -", user2.Name }) : title;
                addTasks.Add(task);
            }
            this.Update(null, null, addTasks, null);
            result.Messages = "发起成功";
            result.DebugMessages = result.Messages;
            result.IsSuccess = true;
            result.NextTasks = addTasks;
            return result;
        }




        /// <summary>
        /// 步骤是否通过
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="step"></param>
        /// <param name="countersignatureStep"></param>
        /// <param name="groupTasks"></param>
        /// <returns></returns>
        private bool StepIsPass(RoadFlow.Model.FlowRun flowRunModel, Step step, Step countersignatureStep, List<RoadFlow.Model.FlowTask> groupTasks)
        {
            List<RoadFlow.Model.FlowTask> list = groupTasks.FindAll(delegate (RoadFlow.Model.FlowTask p)
            {
                return (p.StepId == step.Id) && (p.TaskType != 5);
            });
            int maxSort = (list.Count == 0) ? -1 : Enumerable.Max<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list,
              key=>key.Sort);
            List<RoadFlow.Model.FlowTask> list2 = (-1 == maxSort) ? new List<RoadFlow.Model.FlowTask>() : list.FindAll(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.Sort == maxSort;
            });
            if (list2.Count == 0)
            {
                Guid? countersignatureStartStepId = countersignatureStep.StepBase.CountersignatureStartStepId;
                using (List<Step>.Enumerator enumerator = new Flow().GetRangeSteps(flowRunModel, countersignatureStartStepId.HasValue ? countersignatureStartStepId.GetValueOrDefault() : Guid.Empty, step.Id).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Step rangeStep = enumerator.Current;
                        if (groupTasks.Exists(delegate (RoadFlow.Model.FlowTask p)
                        {
                            return (p.StepId == rangeStep.Id) && (p.TaskType != 5);
                        }))
                        {
                            return false;
                        }
                    }
                }


                return true;
            }
            switch (step.StepBase.HanlderModel)
            {
                case 0:
                    return !list2.Exists(key=> (key.Status != 2));


                case 1:
                    return list2.Exists(key=> (key.ExecuteType == 2));


                case 2:
                    return (((Enumerable.Count<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list2, 
                        key=> (key.Status == 2)) / list2.Count) * 100) >= step.StepBase.Percentage);


                case 3:
                    return !list2.Exists(key=> (key.Status != 2));

                case 4:
                    return !list2.Exists(key=> (key.Status != 2));
            }
            return true;
        }


        //***************************************
        /// <summary>
        /// 流程任务提交
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="executeModel"></param>
        public void Submit(RoadFlow.Model.FlowRun flowRunModel, Execute executeModel)
        {
            List<RoadFlow.Model.FlowTask> listByGroupId;
            List<RoadFlow.Model.FlowTask>.Enumerator enumerator;


            RoadFlow.Model.FlowTask currentTask;
            Predicate<RoadFlow.Model.FlowTask> s_9__12 = null;
            Predicate<RoadFlow.Model.FlowTask> s_9__15 = null;
            Predicate<RoadFlow.Model.FlowTask> s_9__17 = null;

            bool flag = (executeModel.StepId == flowRunModel.FirstStepId) && executeModel.TaskId.IsEmptyGuid();
            if (flag)
            {
                if (executeModel.Title.IsNullOrWhiteSpace())
                {
                    executeModel.Title = flowRunModel.Name + "-" + this.GetStepName(flowRunModel, executeModel.StepId);
                }
                currentTask = this.GetFirstTask(flowRunModel, executeModel);
                listByGroupId = new List<RoadFlow.Model.FlowTask> {
                     currentTask
                  };
                executeModel.TaskId = currentTask.Id;

                executeModel.GroupId = currentTask.GroupId;
                //针对动态流程进行管控

                if (Config.EnableDynamicStep && executeModel.Steps.Exists(key => key.Item3.IsNotEmptyGuid()))
                {
                    RoadFlow.Model.FlowRun run = new FlowDynamic().Add(executeModel, listByGroupId);
                    if (run != null)
                    {
                        flowRunModel = run;
                    }
                    executeModel.Steps.RemoveAll(key =>
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


            }
            else
            {
                listByGroupId = this.GetListByGroupId(executeModel.GroupId);
                currentTask = listByGroupId.Find(delegate (RoadFlow.Model.FlowTask p)
                {
                    return p.Id == executeModel.TaskId;
                });
            }
            //获取当前流程任务
            this.executeResult.CurrentTask = currentTask;
            if (currentTask == null)
            {
                this.executeResult.DebugMessages = "当前任务为空!";
                this.executeResult.IsSuccess = false;
                this.executeResult.Messages = "当前任务为空!";
                return;
            }
            int[] digits = new int[] { -1, 2 };
            if (currentTask.Status.In(digits))
            {
                this.executeResult.DebugMessages = "当前任务" + ((currentTask.Status == -1) ? "等待中" : "已处理") + "!";
                this.executeResult.IsSuccess = false;
                this.executeResult.Messages = this.executeResult.DebugMessages;
                return;
            }
            if ((currentTask.ReceiveId != executeModel.Sender.Id) && (currentTask.IsAutoSubmit == 0))
            {
                this.executeResult.DebugMessages = "当前任务接收人不属于当前用户!";
                this.executeResult.IsSuccess = false;
                this.executeResult.Messages = "您不能处理当前任务!";
                return;
            }
            Step currentStep = flowRunModel.Steps.Find(delegate (Step p)
            {
                return p.Id == currentTask.StepId;
            });
            if (currentStep == null)
            {
                this.executeResult.DebugMessages = "未找到当前步骤运行时实体!";
                this.executeResult.IsSuccess = false;
                this.executeResult.Messages = "未找到当前步骤运行时!";
                return;
            }
            if (executeModel.Title.IsNullOrWhiteSpace())
            {
                executeModel.Title = currentTask.Title;
            }
            if (currentTask.InstanceId.IsNullOrWhiteSpace() && !executeModel.InstanceId.IsNullOrWhiteSpace())
            {
                currentTask.InstanceId = executeModel.InstanceId;
            }
            if (currentTask.TaskType == 6)
            {
                this.BeforAddWrite(flowRunModel, executeModel, currentTask);
                return;
            }
            if (currentTask.OtherType == 1)
            {
                executeModel.OtherType = 1;
            }
            bool flag2 = true;
            if (!currentTask.SubFlowGroupId.IsNullOrWhiteSpace() && (currentStep.StepSubFlow.SubFlowStrategy != 1))
            {
                // char[] separator = new char[] { ',' };
                string[] strArray = currentTask.SubFlowGroupId.Split(',', (StringSplitOptions)StringSplitOptions.None);


                for (int i = 0; i < strArray.Length; i++)
                {
                    Guid guid;
                    if (strArray[i].IsGuid(out guid) && this.GetListByGroupId(guid).Exists(key => ((key.Status < 2) && (key.TaskType != 5))))
                    {
                        this.executeResult.DebugMessages = "当前步骤对应的子流程还未完成，不能提交!";
                        this.executeResult.IsSuccess = false;
                        this.executeResult.Messages = "当前步骤对应的子流程还未完成，不能提交!";
                        return;
                    }
                }
            }
            int maxSort = Enumerable.Max<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId.FindAll(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.StepId == currentTask.StepId;
            }), key=>key.Sort);
            List<RoadFlow.Model.FlowTask> list2 = listByGroupId.FindAll(delegate (RoadFlow.Model.FlowTask p)
            {
                return ((p.StepId == currentTask.StepId) && (p.Sort == maxSort)) && (p.TaskType != 5);
            });
            if (list2.Count > 0)
            {
                int hanlderModel = currentStep.StepBase.HanlderModel;
                if (currentTask.TaskType == 7)
                {
                    char[] chArray = ((int)currentTask.OtherType).ToString().ToCharArray();
                    if (chArray.Length == 3)
                    {
                        switch (((char)chArray[2]).ToString().ToInt(-2147483648))
                        {
                            case 1:
                                hanlderModel = 0;
                                break;

                            case 2:
                                hanlderModel = 1;
                                break;

                            case 3:
                                hanlderModel = 4;
                                break;
                        }
                    }
                }
                switch (hanlderModel)
                {
                    case 0:
                        flag2 = !list2.Exists(delegate (RoadFlow.Model.FlowTask p)
                        {
                            return (p.Status != 2) && (p.Id != currentTask.Id);
                        });
                        break;

                    case 1:
                        foreach (RoadFlow.Model.FlowTask task2 in list2)
                        {
                            if ((task2.Id != currentTask.Id) && (task2.Status != 2))
                            {
                                RoadFlow.Model.FlowTask task3 = task2.Clone();
                                task3.ExecuteType = 4;
                                task3.Status = 2;
                                task3.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                                this.updateTasks.Add(task3);
                            }
                        }
                        break;

                    case 2:
                        {
                            decimal percentage = currentStep.StepBase.Percentage;
                            flag2 = (((Enumerable.Count<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)list2,
                                key => key.Status == 2) + 1) / list2.Count) * 100) >= percentage;

                            if (!flag2)
                            {
                                break;
                            }
                            using (enumerator = list2.FindAll(s_9__12 ?? (s_9__12 = delegate (RoadFlow.Model.FlowTask p)
                            {
                                return (p.Status != 2) && (p.Id != currentTask.Id);
                            })).GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    RoadFlow.Model.FlowTask task4 = enumerator.Current.Clone();
                                    task4.ExecuteType = 4;
                                    task4.Status = 2;
                                    task4.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                                    this.updateTasks.Add(task4);
                                }
                                break;
                            }
                        }
                    case 4:
                        {
                            IOrderedEnumerable<RoadFlow.Model.FlowTask> enumerable = Enumerable.OrderBy<RoadFlow.Model.FlowTask, int>((IEnumerable<RoadFlow.Model.FlowTask>)list2.FindAll(delegate (RoadFlow.Model.FlowTask p)
                            {
                                return p.StepSort > currentTask.StepSort;
                            }),key=>key.StepSort);

                            if (Enumerable.Count<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable) > 0)
                            {
                                RoadFlow.Model.FlowTask task5 = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)enumerable).Clone();
                                task5.ExecuteType = 0;
                                task5.Status = 0;
                                this.updateTasks.Add(task5);
                                flag2 = false;
                                this.executeResult.NextTasks.Add(task5);
                                this.executeResult.IsSuccess = true;
                            }
                            break;
                        }
                }
            }
            RoadFlow.Model.FlowTask task = currentTask.Clone();
            task.ExecuteType = 2;
            task.Comments = executeModel.Comment;
            task.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
            task.IsSign = executeModel.IsSign;
            task.Attachment = executeModel.Attachment;
            task.Status = 2;
            if (flag)
            {
                this.addTasks.Add(task);
            }
            else
            {
                this.updateTasks.Add(task);
            }
            if (flag2)
            {
                List<RoadFlow.Model.User>.Enumerator enumerator4;

                foreach (RoadFlow.Model.FlowTask task6 in list2.FindAll(key=> (key.Status == -1)))
                {
                    if (currentStep.StepBase.HanlderModel == 4)
                    {
                        RoadFlow.Model.FlowTask task7 = task6.Clone();
                        task7.ExecuteType = 4;
                        task7.Status = 2;
                        task7.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                        this.updateTasks.Add(task7);
                    }
                    else
                    {
                        this.removeTasks.Add(task6);
                    }
                }
                using (List<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>.Enumerator enumerator2 = executeModel.Steps.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {

                        //*********
                        s_c__DisplayClass41_0 class_ = new s_c__DisplayClass41_0();
                        s_c__DisplayClass41_1 class_2 = new s_c__DisplayClass41_1();



                        class_.currentStep = currentStep;
                        class_.currentTask = currentTask;
                        class_.executeModel = executeModel;
                        class_.flowRunModel = flowRunModel;
                        //class_.maxSort 


                        DateTime? nullable5 = new DateTime();
                        List<Step> prevSteps;
                        bool flag5;
                        int num7;
                        ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?> tuple = enumerator2.Current;
                        Guid stepId = tuple.Item1;
                        //步骤*********************注意下是哪个stepid
                        //currentStep.Id;      stepId;  currentTask.StepId;  executeModel.StepId;
                        class_2.stepId = stepId;


                        string str = tuple.Item2;
                        Guid? nullable = tuple.Item3;
                        List<RoadFlow.Model.User> list3 = tuple.Item5;
                        DateTime? nullable2 = tuple.Item6;




                        Step nextStpe = flowRunModel.Steps.Find(new Predicate<Step>(class_2.Submitb__14));
                        if (nextStpe == null)
                        {
                            continue;
                        }
                        DateTime? dt = nullable2;

                        DateTime? nullable4 = null;
                        WorkDate date = new WorkDate();

                        if (!dt.HasValue && (nextStpe.WorkTime > decimal.Zero))
                        {
                            nullable5 = null;
                            dt = new DateTime?(date.GetWorkDateTime((double)((double)nextStpe.WorkTime), nullable5));
                        }
                        if (dt.HasValue && (nextStpe.ExpiredPrompt > 0))
                        {
                            nullable4 = new DateTime?(date.GetWorkDateTime((double)(nextStpe.ExpiredPromptDays - (nextStpe.ExpiredPromptDays * 2M)), dt));
                        }

                        bool flag4 = true;
                        int countersignature = nextStpe.StepBase.Countersignature;
                        if (countersignature != 0)
                        {
                            flag4 = false;
                            prevSteps = new Flow().GetPrevSteps(flowRunModel, nextStpe.Id);
                            switch (countersignature)
                            {
                                case 1:
                                    goto Label_09FE;

                                case 2:
                                    goto Label_0A83;

                                case 3:
                                    goto Label_0BF2;
                            }
                        }
                        goto Label_0D94;
                    Label_09FE:
                        flag5 = true;
                        foreach (Step step in prevSteps)
                        {
                            if ((step.Id != currentStep.Id) && !this.StepIsPass(flowRunModel, step, nextStpe, listByGroupId))
                            {
                                flag5 = false;
                                break;
                            }
                        }
                        flag4 = flag5;
                        goto Label_0D94;
                    Label_0A83:
                        flag4 = true;

                        using (List<Step>.Enumerator enumerator3 = prevSteps.GetEnumerator())
                        {
                            while (enumerator3.MoveNext())
                            {
                                if (enumerator3.Current.Id != currentStep.Id)
                                {
                                    using (enumerator = listByGroupId.FindAll(s_9__15 ?? (s_9__15 = new Predicate<RoadFlow.Model.FlowTask>(class_.Submitb__15))).GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            RoadFlow.Model.FlowTask stepTask = enumerator.Current;
                                            if (((stepTask.Status != 2) && (stepTask.Id != currentTask.Id)) && !this.updateTasks.Exists(delegate (RoadFlow.Model.FlowTask p)
                                            {
                                                return p.Id == stepTask.Id;
                                            }))
                                            {
                                                RoadFlow.Model.FlowTask task8 = stepTask.Clone();
                                                task8.Status = 2;
                                                task8.ExecuteType = 4;
                                                task8.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                                                this.updateTasks.Add(task8);
                                            }
                                        }
                                        continue;
                                    }
                                }
                            }
                            goto Label_0D94;

                        }
                    Label_0BF2:
                        num7 = 1;
                        foreach (Step step2 in prevSteps)
                        {
                            if (this.StepIsPass(flowRunModel, step2, nextStpe, listByGroupId))
                            {
                                num7++;
                                if (((num7 / prevSteps.Count) * 100) >= nextStpe.StepBase.CountersignaturePercentage)
                                {
                                    flag4 = true;
                                    break;
                                }
                            }
                        }
                        if (flag4)
                        {
                            using (enumerator = listByGroupId.FindAll(s_9__17 ?? (s_9__17 = new Predicate<RoadFlow.Model.FlowTask>(class_.Submitb__17))).GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    RoadFlow.Model.FlowTask stepTask = enumerator.Current;
                                    if (((stepTask.Status != 2) && (stepTask.Id != currentTask.Id)) && !this.updateTasks.Exists(delegate (RoadFlow.Model.FlowTask p)
                                    {
                                        return p.Id == stepTask.Id;
                                    }))
                                    {
                                        RoadFlow.Model.FlowTask task9 = stepTask.Clone();
                                        task9.Status = 2;
                                        task9.ExecuteType = 4;
                                        task9.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                                        this.updateTasks.Add(task9);
                                    }
                                }
                            }
                        }
                    Label_0D94:
                        if (!flag4)
                        {
                            this.executeResult.Messages = "已发送，等待其他步骤处理!";
                        }
                        else
                        {
                            //  List<RoadFlow.Model.User>.Enumerator enumerator4;
                            if (nextStpe.Type == 1)
                            {
                                Guid subFlowId = nextStpe.StepSubFlow.SubFlowId;
                                RoadFlow.Model.FlowRun subFlowRunModel = new Flow().GetFlowRunModel(subFlowId, true, null);
                                if (subFlowRunModel != null)
                                {
                                    string name = subFlowRunModel.Name;
                                    Step step3 = subFlowRunModel.Steps.Find(delegate (Step p)
                                    {
                                        return p.Id == subFlowRunModel.FirstStepId;
                                    });
                                    if (step3 != null)
                                    {
                                        string str3 = step3.Name;
                                        string subFlowActivationBefore = nextStpe.StepEvent.SubFlowActivationBefore;
                                        int taskType = nextStpe.StepSubFlow.TaskType;
                                        string str5 = string.Empty;
                                        string str6 = string.Empty;
                                        if (!subFlowActivationBefore.IsNullOrWhiteSpace())
                                        {
                                            EventParam param = new EventParam
                                            {
                                                FlowId = currentTask.FlowId,
                                                GroupId = currentTask.GroupId,
                                                InstanceId = currentTask.InstanceId,
                                                StepId = currentTask.StepId,
                                                TaskId = currentTask.Id,
                                                TaskTitle = currentTask.Title
                                            };
                                            object[] args = new object[] { param };
                                            ValueTuple<object, Exception> tuple1 = Tools.ExecuteMethod(subFlowActivationBefore, args);
                                            object obj2 = tuple1.Item1;
                                            if (obj2 != null)
                                            {
                                                try
                                                {
                                                    JObject obj1 = JObject.Parse(obj2.ToString());
                                                    str5 = obj1.Value<string>("instanceId");
                                                    str6 = obj1.Value<string>("title");
                                                }
                                                catch
                                                {
                                                }
                                            }
                                        }
                                        int num9 = 1;
                                        Guid guid3 = Guid.NewGuid();
                                        StringBuilder builder = new StringBuilder();
                                        foreach (RoadFlow.Model.User user in list3)
                                        {
                                            Guid guid5;
                                            Guid guid4 = (taskType == 0) ? guid3 : Guid.NewGuid();
                                            if (taskType == 1)
                                            {
                                                builder.Append(guid4);
                                                builder.Append(",");
                                            }
                                            RoadFlow.Model.FlowTask task11 = new RoadFlow.Model.FlowTask
                                            {
                                                ExecuteType = (num9 == 1) ? 0 : -1,
                                                FlowId = subFlowId,
                                                FlowName = name,
                                                GroupId = guid4,
                                                Id = Guid.NewGuid(),
                                                InstanceId = str5,
                                                IsSign = 0,
                                                PrevId = Guid.Empty,
                                                PrevStepId = Guid.Empty,
                                                ReceiveId = user.Id,
                                                ReceiveName = user.Name,
                                                ReceiveTime = DateTimeExtensions.Now,
                                                SenderId = executeModel.Sender.Id,
                                                SenderName = executeModel.Sender.Name,
                                                Sort = 1,
                                                Status = (num9 == 1) ? 0 : -1,
                                                StepId = subFlowRunModel.FirstStepId,
                                                StepName = str3,
                                                StepSort = (nextStpe.StepBase.HanlderModel == 4) ? num9++ : num9,
                                                TaskType = 0,
                                                Title = str6,
                                                IsAutoSubmit = step3.ExpiredExecuteModel,
                                                CompletedTime = dt,
                                                IsBatch = new int?(step3.BatchExecute),
                                                OtherType = 1
                                            };
                                            if (user.PartTimeId.HasValue)
                                            {
                                                task11.ReceiveOrganizeId = user.PartTimeId;
                                            }
                                            if (user.Note.IsGuid(out guid5))
                                            {
                                                task11.EntrustUserId = new Guid?(guid5);
                                                task11.Note = "委托任务";
                                            }
                                            this.addTasks.Add(task11);
                                        }
                                        RoadFlow.Model.FlowTask task10 = currentTask.Clone();
                                        task10.SenderId = currentTask.ReceiveId;
                                        task10.SenderName = currentTask.ReceiveName;
                                        task10.Status = 0;
                                        task10.ExecuteType = 0;
                                        task10.Comments = "";
                                        task10.IsSign = 0;
                                        task10.Sort = currentTask.Sort + 1;
                                        nullable5 = null;
                                        task10.OpenTime = nullable5;
                                        nullable5 = null;
                                        task10.CompletedTime1 = nullable5;
                                        task10.CompletedTime = dt;
                                        task10.Id = Guid.NewGuid();
                                        task10.PrevId = currentTask.Id;
                                        task10.PrevStepId = currentTask.StepId;
                                        task10.Title = executeModel.Title;
                                        task10.ReceiveTime = DateTimeExtensions.Now;
                                        task10.IsAutoSubmit = nextStpe.ExpiredExecuteModel;
                                        task10.StepId = nextStpe.Id;
                                        task10.StepName = nextStpe.Name;
                                        task10.SubFlowGroupId = (builder.Length == 0) ? guid3.ToString() : builder.ToString().TrimEnd(new char[] { ',' });
                                        task10.IsBatch = new int?(nextStpe.BatchExecute);
                                        this.addTasks.Add(task10);
                                        this.executeResult.NextTasks.Add(task10);
                                    }
                                }
                                continue;
                            }
                            int num6 = 1;

                            using (enumerator4 = list3.GetEnumerator())
                            {
                                while (enumerator4.MoveNext())
                                {
                                    RoadFlow.Model.User receiveUser = enumerator4.Current;



                                    if ((currentStep.StepBase.HanlderModel == 3) || (!this.addTasks.Exists(delegate (RoadFlow.Model.FlowTask p)
                                    {
                                        return (p.ReceiveId == receiveUser.Id) && (p.StepId == nextStpe.Id);
                                    }) && !listByGroupId.Exists(delegate (RoadFlow.Model.FlowTask p)
                                    {
                                        return ((p.ReceiveId == receiveUser.Id) && (p.StepId == nextStpe.Id)) && (p.Status != 2);
                                    })))
                                    {

                                        Guid guid6;
                                        RoadFlow.Model.FlowTask task12 = new RoadFlow.Model.FlowTask
                                        {
                                            ExecuteType = (num6 == 1) ? 0 : -1,
                                            FlowId = executeModel.FlowId,
                                            FlowName = flowRunModel.Name,
                                            GroupId = currentTask.GroupId,
                                            Id = Guid.NewGuid(),
                                            InstanceId = executeModel.InstanceId,
                                            IsSign = 0,
                                            OtherType = executeModel.OtherType,
                                            PrevId = currentTask.Id,
                                            PrevStepId = currentTask.StepId,
                                            ReceiveId = receiveUser.Id,
                                            ReceiveName = receiveUser.Name,
                                            ReceiveTime = DateTimeExtensions.Now,
                                            SenderId = executeModel.Sender.Id,
                                            SenderName = executeModel.Sender.Name,
                                            Sort = currentTask.Sort + 1,
                                            Status = (num6 == 1) ? 0 : -1,
                                            StepId = stepId,
                                            StepName = str.IsNullOrWhiteSpace() ? nextStpe.Name : str.Trim(),
                                            StepSort = (nextStpe.StepBase.HanlderModel == 4) ? num6++ : num6,
                                            TaskType = 0,
                                            Title = executeModel.Title,
                                            IsAutoSubmit = nextStpe.ExpiredExecuteModel,
                                            CompletedTime = dt,
                                            RemindTime = nullable4,
                                            IsBatch = new int?(nextStpe.BatchExecute)


                                        };
                                        if (receiveUser.PartTimeId.IsNotEmptyGuid())
                                        {
                                            task12.ReceiveOrganizeId = receiveUser.PartTimeId;
                                        }
                                        if (receiveUser.Note.IsGuid(out guid6))
                                        {
                                            task12.EntrustUserId = new Guid?(guid6);
                                            task12.Note = "委托";
                                        }

                                        if (nextStpe.DataEditModel == 1)
                                        {
                                            if (currentTask.TaskType == 4)
                                            {
                                                RoadFlow.Model.FlowTask task13 = listByGroupId.Find(delegate (RoadFlow.Model.FlowTask p)
                                                {
                                                    return (p.StepId == nextStpe.Id) && (p.ReceiveId == receiveUser.Id);
                                                });
                                                task12.InstanceId = (task13 != null) ? task13.InstanceId : null;
                                            }
                                            else
                                            {
                                                task12.InstanceId = null;
                                            }
                                        }
                                        if (nullable.IsNotEmptyGuid())
                                        {
                                            task12.BeforeStepId = new Guid?(nullable.Value);
                                        }



                                        this.addTasks.Add(task12);
                                        this.executeResult.NextTasks.Add(task12);
                                    }
                                }
                            }


                            if (nextStpe.StepCopyFor.CopyforTime == 0)
                            {


                                using (enumerator4 = this.GetCopyForUsers(nextStpe, flowRunModel, executeModel, listByGroupId).GetEnumerator())
                                {
                                    while (enumerator4.MoveNext())
                                    {
                                        RoadFlow.Model.User copyForUser = enumerator4.Current;
                                        if (!this.addTasks.Exists(delegate (RoadFlow.Model.FlowTask p)
                                        {
                                            return (p.ReceiveId == copyForUser.Id) && (p.StepId == nextStpe.Id);
                                        }) && !listByGroupId.Exists(delegate (RoadFlow.Model.FlowTask p)
                                        {
                                            return ((p.ReceiveId == copyForUser.Id) && (p.StepId == nextStpe.Id)) && (p.Sort == (currentTask.Sort + 1));
                                        }))
                                        {
                                            RoadFlow.Model.FlowTask task14 = new RoadFlow.Model.FlowTask
                                            {
                                                ExecuteType = 0,
                                                FlowId = executeModel.FlowId,
                                                FlowName = flowRunModel.Name,
                                                GroupId = currentTask.GroupId,
                                                Id = Guid.NewGuid(),
                                                InstanceId = executeModel.InstanceId,
                                                IsSign = 0,
                                                Note = "抄送",
                                                OtherType = executeModel.OtherType,
                                                PrevId = currentTask.Id,
                                                PrevStepId = currentTask.StepId,
                                                ReceiveId = copyForUser.Id,
                                                ReceiveName = copyForUser.Name,
                                                ReceiveTime = DateTimeExtensions.Now,
                                                SenderId = executeModel.Sender.Id,
                                                SenderName = executeModel.Sender.Name,
                                                Sort = currentTask.Sort + 1,
                                                Status = 0,
                                                StepId = stepId,
                                                StepName = nextStpe.Name,
                                                StepSort = 1,
                                                TaskType = 5,
                                                Title = executeModel.Title,
                                                IsAutoSubmit = 0,
                                                IsBatch = new int?(nextStpe.BatchExecute)
                                            };
                                            this.addTasks.Add(task14);
                                            this.executeResult.NextTasks.Add(task14);
                                        }
                                    }
                                    continue;
                                }
                            }
                        }
                    }
                }
                if (currentStep.StepCopyFor.CopyforTime == 1)
                {
                    using (enumerator4 = this.GetCopyForUsers(currentStep, flowRunModel, executeModel, listByGroupId).GetEnumerator())
                    {
                        while (enumerator4.MoveNext())
                        {
                            RoadFlow.Model.User copyForUser = enumerator4.Current;
                            if (!listByGroupId.Exists(delegate (RoadFlow.Model.FlowTask p)
                            {
                                return (p.ReceiveId == copyForUser.Id) && (p.StepId == currentStep.Id);
                            }))
                            {
                                RoadFlow.Model.FlowTask task15 = new RoadFlow.Model.FlowTask
                                {
                                    ExecuteType = 0,
                                    FlowId = executeModel.FlowId,
                                    FlowName = flowRunModel.Name,
                                    GroupId = currentTask.GroupId,
                                    Id = Guid.NewGuid(),
                                    InstanceId = executeModel.InstanceId,
                                    IsSign = 0,
                                    Note = "抄送",
                                    OtherType = executeModel.OtherType,
                                    PrevId = currentTask.PrevId,
                                    PrevStepId = currentTask.PrevStepId,
                                    ReceiveId = copyForUser.Id,
                                    ReceiveName = copyForUser.Name,
                                    ReceiveTime = DateTimeExtensions.Now,
                                    SenderId = executeModel.Sender.Id,
                                    SenderName = executeModel.Sender.Name,
                                    Sort = currentTask.Sort,
                                    Status = 0,
                                    StepId = currentStep.Id,
                                    StepName = currentStep.Name,
                                    StepSort = 1,
                                    TaskType = 5,
                                    Title = executeModel.Title,
                                    IsAutoSubmit = 0,
                                    IsBatch = new int?(currentStep.BatchExecute)
                                };
                                this.addTasks.Add(task15);
                            }
                        }
                    }
                }
            }


            this.executeResult.IsSuccess = true;
            bool flag3 = flag2 && (this.addTasks.Count == 0);

            if (flag3)
            {
                using (enumerator = listByGroupId.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        RoadFlow.Model.FlowTask task2 = enumerator.Current;
                        if ((task2.Status != 2) && (task2.TaskType != 5))
                        {
                            RoadFlow.Model.FlowTask task16 = this.updateTasks.Find(delegate (RoadFlow.Model.FlowTask p)
                            {
                                return p.Id == task2.Id;
                            });
                            if (((task16 == null) || (task16.Status != 2)) && (this.removeTasks.Find(delegate (RoadFlow.Model.FlowTask p)
                            {
                                return p.Id == task2.Id;
                            }) == null))
                            {
                                flag3 = false;
                                goto Label_1D70;
                            }
                        }
                    }
                }
            }
        Label_1D70:
            if (flag3)
            {
                RoadFlow.Model.FlowTask task17 = listByGroupId.Find(delegate (RoadFlow.Model.FlowTask p)
                {
                    return ((p.PrevId == Guid.Empty) && (p.PrevStepId == Guid.Empty)) && (p.StepId == flowRunModel.FirstStepId);
                });
                if ((task17 != null) && (task17.OtherType == 1))
                {
                    List<RoadFlow.Model.FlowTask> mainTasks = this.GetListBySubFlowGroupId(task17.GroupId);



                    if (mainTasks.Count > 0)
                    {
                        RoadFlow.Model.FlowRun run2 = new Flow().GetFlowRunModel(Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)mainTasks).FlowId, true, null);
                        if (run2 != null)
                        {
                            Step step4 = run2.Steps.Find(delegate (Step p)
                            {
                                return p.Id == Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)mainTasks).StepId;
                            });
                            if ((step4 != null) && (step4.StepSubFlow.SubFlowStrategy == 2))
                            {
                                this.executeResult.AutoSubmitTasks.AddRange((IEnumerable<RoadFlow.Model.FlowTask>)mainTasks);
                            }
                            if ((step4 != null) && !step4.StepEvent.SubFlowCompletedBefore.IsNullOrWhiteSpace())
                            {
                                EventParam param2 = new EventParam
                                {
                                    FlowId = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)mainTasks).FlowId,
                                    GroupId = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)mainTasks).GroupId,
                                    InstanceId = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)mainTasks).InstanceId,
                                    StepId = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)mainTasks).StepId,
                                    TaskId = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)mainTasks).Id,
                                    TaskTitle = Enumerable.First<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)mainTasks).Title
                                };
                                object[] objArray2 = new object[] { param2 };
                                Exception exception = Tools.ExecuteMethod(step4.StepEvent.SubFlowCompletedBefore.Trim(), objArray2).Item2;
                                if (exception != null)
                                {
                                    string[] textArray1 = new string[] { "执行子流程完成后事件发生了错误-", run2.Name, "-", step4.Name, "-", step4.StepEvent.SubFlowCompletedBefore };
                                    string[] textArray2 = new string[] { "参数：", param2.ToString(), " 错误：", exception.Message, exception.StackTrace };
                                    Log.Add(string.Concat((string[])textArray1), string.Concat((string[])textArray2), LogType.流程运行, "", "", "", "", "", "", "", "");
                                }
                            }
                        }
                    }
                }
            }
            if (flag3)
            {
                Guid connectionId = flowRunModel.TitleField.ConnectionId;
                string field = flowRunModel.TitleField.Field;
                string table = flowRunModel.TitleField.Table;
                string str9 = flowRunModel.TitleField.Value;
                RoadFlow.Model.DbConnection connection = new DbConnection().Get(connectionId);
                if (((connection != null) && !field.IsNullOrWhiteSpace()) && !table.IsNullOrWhiteSpace())
                {
                    Database database = Enumerable.Any<Database>((IEnumerable<Database>)flowRunModel.Databases) ? Enumerable.First<Database>((IEnumerable<Database>)flowRunModel.Databases) : null;
                    string str10 = string.Empty;
                    string primaryKey = string.Empty;
                    if (database != null)
                    {
                        str10 = database.Table;
                        primaryKey = database.PrimaryKey;
                    }
                    if (str10.EqualsIgnoreCase(table) && !primaryKey.EqualsIgnoreCase(field))
                    {
                        string[] textArray3 = new string[] { "UPDATE ", table, " SET ", field, "={0} WHERE ", primaryKey, "={1}" };
                        string sql = string.Concat((string[])textArray3);
                        object[] objArray3 = new object[] { str9.IsNullOrEmpty() ? "1" : str9, currentTask.InstanceId };
                        object[] objects = objArray3;
                        if (connection.ConnString.EqualsIgnoreCase(Config.ConnectionString))
                        {
                            this.executeSqls.Add(new ValueTuple<string, object[], int>(sql, objects, 1));
                        }
                        else
                        {
                            using (DataContext context = new DataContext(connection.ConnType, connection.ConnString, true))
                            {
                                context.Execute(sql, objects);
                                context.SaveChanges();
                            }
                        }
                    }
                }

                foreach (RoadFlow.Model.FlowTask task18 in Enumerable.Where<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)listByGroupId,
                   key=> key.BeforeStepId.IsNotEmptyGuid()))
                {
                    IO.Remove("roadflow_cache_flowrun__" + task18.BeforeStepId.Value.ToNString() + "_" + task18.GroupId.ToNString());
                }
                lockObject.Remove(executeModel.GroupId.ToString());
            }
        }







        public void TaskEnd(RoadFlow.Model.FlowRun flowRunModel, Execute executeModel)
        {
            List<RoadFlow.Model.FlowTask> listByGroupId = this.GetListByGroupId(executeModel.GroupId);
            RoadFlow.Model.FlowTask task = listByGroupId.Find(delegate (RoadFlow.Model.FlowTask p)
            {
                return p.Id == executeModel.TaskId;
            });
            if (task == null)
            {
                this.executeResult.DebugMessages = "当前任务为空";
                this.executeResult.IsSuccess = false;
                this.executeResult.Messages = "当前任务为空";
            }
            else
            {
                int[] digits = new int[] { -1, 2 };
                if (task.Status.In(digits))
                {
                    this.executeResult.DebugMessages = "当前任务" + ((task.Status == -1) ? "等待中" : "已处理") + "!";
                    this.executeResult.IsSuccess = false;
                    this.executeResult.Messages = this.executeResult.DebugMessages;
                }
                else
                {
                    this.executeResult.CurrentTask = task;
                    using (List<RoadFlow.Model.FlowTask>.Enumerator enumerator = listByGroupId.FindAll(key => key.Status < 2).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            RoadFlow.Model.FlowTask task2 = enumerator.Current.Clone();
                            task2.ExecuteType = 12;
                            task2.Status = 2;
                            if (task2.Id == task.Id)
                            {
                                task2.Comments = executeModel.Comment;
                                task2.IsSign = executeModel.IsSign;
                                task2.ExecuteType = 11;
                            }
                            task2.CompletedTime1 = new DateTime?(DateTimeExtensions.Now);
                            this.updateTasks.Add(task2);
                        }
                    }
                    this.executeResult.IsSuccess = true;
                    this.executeResult.Messages = "已终止!";
                }
            }
        }

        public int Update(RoadFlow.Model.FlowTask flowTaskModel)
        {
            return this.flowTaskData.Update(flowTaskModel);
        }

        public int Update(List<RoadFlow.Model.FlowTask> removeTasks, List<RoadFlow.Model.FlowTask> updateTasks, List<RoadFlow.Model.FlowTask> addTasks, List<ValueTuple<string, object[], int>> executeSqls)
        {
            return this.flowTaskData.Update(removeTasks, updateTasks, addTasks, executeSqls);
        }



        public void RemindTask(Dictionary<string, string> lang = null)
        {
            DataTable remindTasks = this.flowTaskData.GetRemindTasks();
            Message message = new Message();
            foreach (DataRow row in remindTasks.Rows)
            {
                string[] textArray1 = new string[] { (lang == null) ? "您有一个待办事项“" : ((string)lang["RemindTask_Todoitems"]), row["Title"].ToString(), "”，", (lang == null) ? "将在" : ((string)lang["RemindTask_Bein"]), row["CompletedTime"].ToString().ToDateTime().ToShortDateTimeString(), (lang == null) ? "超期，请尽快处理!" : ((string)lang["RemindTask_Overdue"]) };
                string content = string.Concat((string[])textArray1);
                string str2 = message.Send(row["ReceiveId"].ToString().ToGuid(), content, "0,1,2", null);
                if ("1".Equals(str2))
                {
                    int num = this.flowTaskData.UpdateRemind(row["Id"].ToString().ToGuid(), DateTimeExtensions.Now.AddDays(1.0));
                }
                Log.Add(((lang == null) ? "待办任务超期提醒消息" : lang["RemindTask_LogTitle"]) + "-" + row["ReceiveName"], content, LogType.流程运行, "", "", ((lang == null) ? "发送消息返回：" : lang["RemindTask_LogReturn"]) + str2, "", "", "", "", "");
            }
        }











      

        [CompilerGenerated]
        private struct SendMessaged__39 : IAsyncStateMachine
        {
            // Fields
            public int s_1__state;
            public AsyncVoidMethodBuilder s_t__builder;
            private TaskAwaiter s_u__1;
            public string contents;
            public List<RoadFlow.Model.FlowTask> nextTasks;
            public RoadFlow.Model.User sender;
            public string sendModel;

            // Methods
            void IAsyncStateMachine.MoveNext()
            {
                int num = this.s_1__state;
                try
                {
                    TaskAwaiter awaiter;
                    if (num != 0)
                    {
                        FlowTask.s_c__DisplayClass39_0 class_1 = new s_c__DisplayClass39_0();
                        List<RoadFlow.Model.FlowTask> nextTasks = this.nextTasks;
                        RoadFlow.Model.User sender = this.sender;
                        string sendModel = this.sendModel;
                        string contents = this.contents;
                        class_1.nextTasks = nextTasks;
                        class_1.sender = sender;
                        class_1.sendModel = sendModel;
                        class_1.contents = contents;


                        awaiter = Task.Run(new Action(class_1.SendMessageb__0)).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.s_1__state = num = 0;
                            this.s_u__1 = awaiter;
                            this.s_t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FlowTask.SendMessaged__39>(ref awaiter, ref this);
                            return;
                        }
                    }
                    else
                    {
                        awaiter = this.s_u__1;
                        this.s_u__1 = new TaskAwaiter();
                        this.s_1__state = num = -1;
                    }
                    awaiter.GetResult();
                }
                catch (Exception exception)
                {
                    this.s_1__state = -2;
                    this.s_t__builder.SetException(exception);
                    return;
                }
                this.s_1__state = -2;
                this.s_t__builder.SetResult();
            }

            [DebuggerHidden]
            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.s_t__builder.SetStateMachine(stateMachine);
            }



        }


        [CompilerGenerated]
        sealed class s_c__DisplayClass39_0
        {
            // Fields
            public string contents;
            public List<RoadFlow.Model.FlowTask> nextTasks;
            public RoadFlow.Model.User sender;
            public string sendModel;

            // Methods
            internal void SendMessageb__0()
            {
                if ((this.nextTasks != null) && Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)this.nextTasks))
                {
                    Message message = new Message();
                    User user = new User();
                    HttpContext httpContext = Tools.HttpContext;
                    foreach (RoadFlow.Model.FlowTask task in this.nextTasks)
                    {
                        if (((task.TaskType != 5) && (task.Status != -1)) && ((this.sender == null) || (task.ReceiveId != this.sender.Id)))
                        {
                            RoadFlow.Model.User receiveUser = user.Get(task.ReceiveId);
                            if (receiveUser != null)
                            {
                                string str = (httpContext == null) ? "0" : httpContext.Request.Querys("rf_appopenmodel");
                                object[] objArray1 = new object[] { "Index", task.FlowId, task.StepId, task.Id, task.GroupId, task.InstanceId, (httpContext == null) ? "" : httpContext.Request.Querys("appid"), (httpContext == null) ? "" : httpContext.Request.Querys("tabid"), str };
                                string str2 = string.Format("/RoadFlowCore/FlowRun/{0}?flowid={1}&stepid={2}&taskid={3}&groupid={4}&instanceid={5}&appid={6}&tabid={7}&rf_appopenmodel={8}", (object[])objArray1);
                                if (("," + this.sendModel + ",").Contains(",1,") && !receiveUser.Mobile.IsNullOrWhiteSpace())
                                {
                                    RoadFlow.Utility.SMS.SMS.SendSMS(this.contents.IsNullOrWhiteSpace() ? ("您有一个待办事项“" + task.Title + "”，请尽快处理！") : this.contents, receiveUser.Mobile);
                                }
                                if (("," + this.sendModel + ",").Contains(",0,"))
                                {
                                    string[] textArray1 = new string[] { this.contents.IsNullOrWhiteSpace() ? ((string)("您有一个待办事项“" + task.Title + "”")) : ((string)this.contents), "，<a href=\"javascript:;\" class=\"blue1\" onclick=\"top.openApp('", str2, "',", str.IsNullOrWhiteSpace() ? "0" : ((string)str), ",'", task.Title, "');top.closeMessage();\">点击处理！</a>" };
                                    message.Send(task.ReceiveId, string.Concat((string[])textArray1), "0", this.sender);
                                }
                                if ((Config.Enterprise_WeiXin_IsUse && ("," + this.sendModel + ",").Contains(",2,")) && (!receiveUser.Mobile.IsNullOrWhiteSpace() || !receiveUser.Email.IsNullOrWhiteSpace()))
                                {
                                    JObject obj3 = new JObject();
                                    obj3.Add("title", "待办事项");
                                    obj3.Add("description", !this.contents.IsNullOrWhiteSpace() ? ((JToken)this.contents) : ((JToken)string.Concat((string[])new string[] { "<div class=\"gray\">发送人:", task.SenderName, "  时间:", task.ReceiveTime.ToShortDateTimeString(), "</div><div class=\"normal\">", (this.contents.IsNullOrWhiteSpace() ? ((string)("您有一个待办事项“" + task.Title + "”，请尽快处理！")) : ((string)this.contents)), "</div>" })));
                                    obj3.Add("url", (JToken)(Config.Enterprise_WeiXin_WebUrl + str2 + "&ismobile=1"));
                                    obj3.Add("btntxt", "处理");
                                    JObject contentJson = obj3;
                                    EnterpriseWeiXin.Common.SendMessage(receiveUser, contentJson, "textcard", 0);
                                }
                            }
                        }
                    }
                }
            }
        }


        [CompilerGenerated]
        private sealed class s_c__DisplayClass42_2
        {
            // Fields
            public FlowTask.s_c__DisplayClass42_0 s_8__locals2 = new s_c__DisplayClass42_0();
            public Guid stepId;

            // Methods
            internal bool Backb__12(Step p)
            {
                return (p.Id == this.stepId);
            }
        }


        [CompilerGenerated]
        private sealed class s_c__DisplayClass42_0
        {

            // Fields
            public Predicate<RoadFlow.Model.FlowTask> s_9__7 = null;
            public Step currentStep = null;
            public RoadFlow.Model.FlowTask currentTask = null;
            public Execute executeModel = null;
            public int maxSort = 0;

            // Methods
            internal bool Backb__0(RoadFlow.Model.FlowTask p)
            {
                return (p.Id == this.executeModel.TaskId);
            }

            internal bool Backb__1(Step p)
            {
                return (p.Id == this.currentTask.StepId);
            }

            internal bool Backb__2(RoadFlow.Model.FlowTask p)
            {
                return (p.StepId == this.currentTask.StepId);
            }

            internal bool Backb__4(RoadFlow.Model.FlowTask p)
            {
                return (((p.StepId == this.currentTask.StepId) && (p.Sort == this.maxSort)) && (p.TaskType != 5));
            }

            internal bool Backb__5(RoadFlow.Model.FlowTask p)
            {
                return ((p.Status != 2) && (p.Id != this.currentTask.Id));
            }

            internal bool Backb__7(RoadFlow.Model.FlowTask p)
            {
                return ((p.Status != 2) && (p.Id != this.currentTask.Id));
            }

            internal bool Backb__8(RoadFlow.Model.FlowTask p)
            {
                return (p.StepSort == (this.currentTask.StepSort - 1));
            }

            internal bool Backb__9(RoadFlow.Model.FlowTask p)
            {
                return (p.StepId == this.currentStep.Id);
            }
        }






        [CompilerGenerated]
        private sealed class s_c__DisplayClass41_0
        {
            // Fields
            public Predicate<RoadFlow.Model.FlowTask> s_9__12 = null;
            public Predicate<RoadFlow.Model.FlowTask> s_9__15 = null;
            public Predicate<RoadFlow.Model.FlowTask> s_9__17 = null;
            public Step currentStep;
            public RoadFlow.Model.FlowTask currentTask;
            public Execute executeModel;
            public RoadFlow.Model.FlowRun flowRunModel;
            public int maxSort = 0;

            // Methods
            internal bool Submitb__1(RoadFlow.Model.FlowTask p)
            {
                return (p.Id == this.executeModel.TaskId);
            }

            internal bool Submitb__10(RoadFlow.Model.FlowTask p)
            {
                return (p.StepSort > this.currentTask.StepSort);
            }

            internal bool Submitb__12(RoadFlow.Model.FlowTask p)
            {
                return ((p.Status != 2) && (p.Id != this.currentTask.Id));
            }

            internal bool Submitb__15(RoadFlow.Model.FlowTask p)
            {
                return ((p.Sort == this.currentTask.Sort) && (p.TaskType != 5));
            }

            internal bool Submitb__17(RoadFlow.Model.FlowTask p)
            {
                return ((p.Sort == this.currentTask.Sort) && (p.TaskType != 5));
            }

            internal bool Submitb__2(Step p)
            {
                return (p.Id == this.currentTask.StepId);
            }

            internal bool Submitb__28(RoadFlow.Model.FlowTask p)
            {
                return (((p.PrevId == Guid.Empty) && (p.PrevStepId == Guid.Empty)) && (p.StepId == this.flowRunModel.FirstStepId));
            }

            internal bool Submitb__3(RoadFlow.Model.FlowTask p)
            {
                return (p.StepId == this.currentTask.StepId);
            }

            internal bool Submitb__5(RoadFlow.Model.FlowTask p)
            {
                return (((p.StepId == this.currentTask.StepId) && (p.Sort == this.maxSort)) && (p.TaskType != 5));
            }

            internal bool Submitb__8(RoadFlow.Model.FlowTask p)
            {
                return ((p.Status != 2) && (p.Id != this.currentTask.Id));
            }
        }




        [CompilerGenerated]
        private sealed class s_c__DisplayClass41_1
        {
            // Fields
            public FlowTask.s_c__DisplayClass41_0 locals1 = new s_c__DisplayClass41_0();
            public Guid stepId;

            // Methods
            internal bool Submitb__14(Step p)
            {
                return (p.Id == this.stepId);
            }
        }







        #endregion

    }


    [CompilerGenerated]
    internal sealed class PrivateImplementationDetails
    {
        // Fields
        // internal static readonly __StaticArrayInitTypeSize  // =16 1456763F890A84558F99AFA687C36B9037697848; // data size: 16 bytes
        // internal static readonly __StaticArrayInitTypeSize1   // =6 CB0DEAC164F33F822B26979E2CE8BF185969601F; // data size: 6 bytes

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
        [StructLayout(LayoutKind.Explicit, Size = 0x10, Pack = 1)]
        private struct __StaticArrayInitTypeSize
        {
        }

        [StructLayout(LayoutKind.Explicit, Size = 6, Pack = 1)]
        private struct __StaticArrayInitTypeSize1
        {
        }
    }


}
