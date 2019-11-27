using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility.Cache;
using RoadFlow.Model.FlowRunModel;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Localization;

namespace RoadFlow.Business
{
 

    /// <summary>
    /// 流程管理
    /// </summary>
    #region 新的方法 2.8.3
    public class Flow
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_flowrun_";
        private readonly RoadFlow.Data.Flow flowData = new RoadFlow.Data.Flow();

        // Methods
        public int Add(RoadFlow.Model.Flow flow)
        {
            return this.flowData.Add(flow);
        }

        /// <summary>
        /// 新增下一个步骤
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId"></param>
        /// <param name="steps"></param>
        private void AddNextSteps(Model.FlowRun flowRunModel, Guid stepId, List<Step> steps)
        {
            List<Step> nextSteps = this.GetNextSteps(flowRunModel, stepId);
            using (List<Step>.Enumerator enumerator = nextSteps.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Step step = enumerator.Current;
                    if (!steps.Exists(delegate (Step p) {
                        return p.Id == step.Id;
                    }))
                    {
                        steps.Add(step);
                    }
                }
            }
            foreach (Step step in nextSteps)
            {
                this.AddNextSteps(flowRunModel, step.Id, steps);
            }
        }




        /// <summary>
        /// 批量添加步骤
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="runModel"></param>
        /// <param name="stepId1"></param>
        /// <param name="stepId2"></param>
        private void AddRangeSteps(List<List<Step>> steps, RoadFlow.Model.FlowRun runModel, Guid stepId1, Guid stepId2)
        {
            List<Step> list = runModel.Steps;
            using (List<Line>.Enumerator enumerator = runModel.Lines.FindAll(delegate (Line p) {
                return p.FromId == stepId1;
            }).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Line line = enumerator.Current;
                    Step step = list.Find(delegate (Step p) {
                        return p.Id == line.ToId;
                    });
                    if (step != null)
                    {
                        List<Step> list2 = new List<Step> {
                        step
                    };
                        this.AddRangeSteps1(list2, runModel, step.Id, stepId2);
                        steps.Add(Enumerable.ToList<Step>(Enumerable.Distinct<Step>((IEnumerable<Step>)list2)));
                    }
                }
            }
        }


        /// <summary>
        /// 批量添加步骤
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="runModel"></param>
        /// <param name="stepId1"></param>
        /// <param name="stepId2"></param>
        private void AddRangeSteps1(List<Step> steps, RoadFlow.Model.FlowRun runModel, Guid stepId1, Guid stepId2)
        {
            List<Step> list = runModel.Steps;
            using (List<Line>.Enumerator enumerator = runModel.Lines.FindAll(delegate (Line p) {
                return p.FromId == stepId1;
            }).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Line line = enumerator.Current;
                    Step step = list.Find(delegate (Step p) {
                        return p.Id == line.ToId;
                    });
                    if (step != null)
                    {
                        steps.Add(step);
                        this.AddRangeSteps1(steps, runModel, step.Id, stepId2);
                    }
                }
            }
        }



        public void ClearCache()
        {
            this.flowData.ClearCache();
        }


        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="id"></param>

        public void ClearCache(Guid id)
        {
            IO.Remove("roadflow_cache_flowrun_" + id.ToString("N"));
        }



        public int Delete(RoadFlow.Model.Flow flow)
        {
            return this.flowData.Delete(flow);
        }


        public RoadFlow.Model.Flow Get(Guid id)
        {
            return this.flowData.Get(id);
        }




        public List<RoadFlow.Model.Flow> GetAll()
        {
            return this.flowData.GetAll();
        }


        /// <summary>
        /// 获取所有下一步骤
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public List<Step> GetAllNextSteps(Model.FlowRun flowRunModel, Guid stepId)
        {
            List<Step> steps = new List<Step>();
            this.AddNextSteps(flowRunModel, stepId, steps);
            return steps;
        }


      






        /// <summary>
        /// 获取导出流程字符串
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public string GetExportFlowString(string ids)
        {
            if (ids.IsNullOrWhiteSpace())
            {
                return "";
            }
            char[] separator = new char[] { ',' };
            JObject obj2 = new JObject();
            JArray array = new JArray();
            JArray array2 = new JArray();
            JArray array3 = new JArray();
            List<Guid> list = new List<Guid>();
            List<Guid> list2 = new List<Guid>();
            string[] strArray = ids.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                Guid guid;
                if (strArray[i].IsGuid(out guid))
                {
                    RoadFlow.Model.Flow flow = this.Get(guid);
                    if (flow != null)
                    {
                        array.Add(JObject.FromObject(flow));
                        this.GetFlowRunModel(guid, true);
                        if (!flow.RunJSON.IsNullOrWhiteSpace() && !flow.DesignerJSON.IsNullOrWhiteSpace())
                        {
                            JObject obj3 = null;
                            try
                            {
                                obj3 = JObject.Parse(flow.RunJSON.IsNullOrWhiteSpace() ? flow.DesignerJSON : flow.RunJSON);
                            }
                            catch
                            {
                            }
                            if (obj3 != null)
                            {
                                JArray array4 = obj3.Value<JArray>("steps");
                                if (array4 != null)
                                {
                                    AppLibrary library = new AppLibrary();
                                    Form form = new Form();
                                    using (IEnumerator<JToken> enumerator = array4.GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            Guid guid2;
                                            Guid guid3;
                                            JObject obj4 = (JObject)(enumerator.Current).Value<JArray>("forms").First;
                                            if (obj4.Value<string>("id").IsGuid(out guid2))
                                            {
                                                RoadFlow.Model.AppLibrary library2 = library.Get(guid2);
                                                if (library2 != null)
                                                {
                                                    Guid guid4;
                                                    if (!list2.Contains(library2.Id))
                                                    {
                                                        array3.Add(JObject.FromObject(library2));
                                                        list2.Add(library2.Id);
                                                    }
                                                    if (!library2.Code.IsNullOrWhiteSpace() && library2.Code.IsGuid(out guid4))
                                                    {
                                                        RoadFlow.Model.Form form2 = form.Get(guid4);
                                                        if ((form2 != null) && !list.Contains(form2.Id))
                                                        {
                                                            array2.Add(JObject.FromObject(form2));
                                                            list.Add(form2.Id);
                                                        }
                                                    }
                                                }
                                            }
                                            if (obj4.Value<string>("idApp").IsGuid(out guid3))
                                            {
                                                RoadFlow.Model.AppLibrary library3 = library.Get(guid3);
                                                if (library3 != null)
                                                {
                                                    Guid guid5;
                                                    if (!list2.Contains(library3.Id))
                                                    {
                                                        array3.Add(JObject.FromObject(library3));
                                                        list2.Add(library3.Id);
                                                    }
                                                    if (!library3.Code.IsNullOrWhiteSpace() && library3.Code.IsGuid(out guid5))
                                                    {
                                                        RoadFlow.Model.Form form3 = form.Get(guid5);
                                                        if ((form3 != null) && !list.Contains(form3.Id))
                                                        {
                                                            array2.Add(JObject.FromObject(form3));
                                                            list.Add(form3.Id);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            obj2.Add("flows", array);
            obj2.Add("forms", array2);
            obj2.Add("applibrarys", array3);
            return obj2.ToString();
        }



        /// <summary>
        /// 获取字典流程选项
        /// </summary>
        /// <param name="flows"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetFlowOptions(List<RoadFlow.Model.Flow> flows, string value = "")
        {
            StringBuilder builder = new StringBuilder();
            Dictionary dictionary = new Dictionary();
            Guid idByCode = dictionary.GetIdByCode("system_applibrarytype_flow");
            foreach (RoadFlow.Model.Flow flow in flows)
            {
                builder.Append("<option value=\"" + flow.Id + "\"");
                if (flow.Id.ToString().EqualsIgnoreCase(value))
                {
                    builder.Append(" selected=\"selected\"");
                }
                string[] textArray1 = new string[] { ">", flow.Name, " (", dictionary.GetAllParentTitle(flow.FlowType, true, false, idByCode.ToString(), true), ")" };
                builder.Append(string.Concat((string[])textArray1));
                builder.Append("</option>");
            }
            return builder.ToString();
        }


        /// <summary>
        /// 获取流程步骤参数实体
        /// </summary>
        /// <param name="json"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public RoadFlow.Model.FlowRun GetFlowRunModel(string json, out string errMsg, IStringLocalizer localizer = null)
        {
            Guid guid;
            Guid guid2;
            Guid guid3;
            int num;
            Guid? nullable=null;
            errMsg = string.Empty;
            JObject obj2 = null;
            try
            {
                obj2 = JObject.Parse(json);
            }
            catch
            {
                errMsg = (localizer == null) ? "JSON解析错误" : localizer["Save_JsonParseError"];

                return null;
            }
            if (obj2 == null)
            {
                errMsg = (localizer == null) ? "JSON解析错误" : localizer["Save_JsonParseError"];

                return null;
            }
            string str = obj2.Value<string>("id");
            string str2 = obj2.Value<string>("name");
            string str3 = obj2.Value<string>("type");
            string str4 = obj2.Value<string>("systemId");
            string str5 = obj2.Value<string>("manager");
            string str6 = obj2.Value<string>("instanceManager");
            if (!obj2.Value<string>("id").IsGuid(out guid))
            {
                errMsg = (localizer == null) ? "流程ID错误" : localizer["Save_FlowIdError"];
                return null;
            }
            if (str2.IsNullOrWhiteSpace())
            {
                errMsg = (localizer == null) ? "流程名称为空" : localizer["Save_FlowNameEmpty"];

                return null;
            }
            if (!str3.IsGuid(out guid2))
            {
                errMsg = (localizer == null) ? "流程分类错误" : localizer["Save_FlowTypeError"];

                return null;
            }
            if (str5.IsNullOrWhiteSpace())
            {
                errMsg = (localizer == null) ? "流程管理者为空" : localizer["Install_ManagerEmpty"];

                return null;
            }
            if (str6.IsNullOrWhiteSpace())
            {
                errMsg = (localizer == null) ? "流程实例管理者为空" : localizer["Install_InstanceManagerEmpty"];

                return null;
            }
            RoadFlow.Model.Flow flow = this.Get(guid);
            if (flow == null)
            {
                errMsg = (localizer == null) ? "未找到该流程" : localizer["Install_NoFindFlowModel"];

                return null;
            }
            RoadFlow.Model.FlowRun run = new RoadFlow.Model.FlowRun
            {
                Id = guid,
                Name = str2,
                Type = guid2,
                Manager = str5,
                InstanceManager = str6,
                FirstStepId = Guid.Empty,
                Note = obj2.Value<string>("note"),
                SystemId = str3.IsGuid(out guid3) ? new Guid?(guid3) : (nullable),
                Debug = obj2.Value<string>("debug").IsInt(out num) ? num : 0,
                DebugUserIds = new Organize().GetAllUsersId(obj2.Value<string>("debugUsers")),
                Status = flow.Status,
                CreateDate = flow.CreateDate,
                CreateUserId = flow.CreateUser,
                InstallDate = flow.InstallDate,
                InstallUserId = flow.InstallUser,
                Ico = obj2.Value<string>("ico"),
                Color = obj2.Value<string>("color")
            };
            JArray array = obj2.Value<JArray>("databases");
            List<Database> list = new List<Database>();
            if (array != null)
            {
                foreach (JObject obj4 in array)
                {
                    Guid guid4;
                    Database database = new Database
                    {
                        ConnectionId = obj4.Value<string>("link").IsGuid(out guid4) ? guid4 : Guid.Empty,
                        ConnectionName = obj4.Value<string>("linkName"),
                        Table = obj4.Value<string>("table"),
                        PrimaryKey = obj4.Value<string>("primaryKey")
                    };
                    list.Add(database);
                }
            }
            run.Databases = list;
            JObject obj3 = obj2.Value<JObject>("titleField");
            TitleField field = new TitleField();
            if (obj3 != null)
            {
                Guid guid5;
                field.ConnectionId = obj3.Value<string>("link").IsGuid(out guid5) ? guid5 : Guid.Empty;
                field.Table = obj3.Value<string>("table");
                field.Field = obj3.Value<string>("field");
                field.Value = obj3.Value<string>("value");
            }
            run.TitleField = field;
            JArray array2 = obj2.Value<JArray>("steps");
            List<Step> list2 = new List<Step>();
            if (array2 != null)
            {
                foreach (JObject obj5 in array2)
                {
                    int num2;
                    int num3;
                    decimal num4;

                    Guid guid6;
                    int num5;
                    int num6;
                    decimal num7;
                    int num8;
                    int num9;
                    int num10;

                    Step step2 = new Step();
                    JObject obj6 = obj5.Value<JObject>("position");
                    if (obj6 != null)
                    {
                        decimal num11;
                        decimal num12;
                        step2.Position_X = obj6.Value<string>("x").IsDecimal(out num11) ? num11 : decimal.Zero;
                        step2.Position_Y = obj6.Value<string>("y").IsDecimal(out num12) ? num12 : decimal.Zero;
                    }
                    step2.Archives = obj5.Value<string>("archives").IsInt(out num2) ? num2 : 0;
                    step2.ExpiredPrompt = obj5.Value<string>("expiredPrompt").IsInt(out num3) ? num3 : 0;
                    step2.ExpiredPromptDays = obj5.Value<string>("expiredPromptDays").IsDecimal(out num4) ? num4 : decimal.Zero;

                    step2.Id = obj5.Value<string>("id").IsGuid(out guid6) ? guid6 : Guid.Empty;
                    step2.Type = obj5.Value<string>("type").EqualsIgnoreCase("normal") ? 0 : 1;
                    step2.Name = obj5.Value<string>("name");
                    step2.Dynamic = obj5.Value<string>("dynamic").ToInt(0);
                    step2.DynamicField = obj5.Value<string>("dynamicField");

                    step2.Note = obj5.Value<string>("note");
                    step2.CommentDisplay = obj5.Value<string>("opinionDisplay").IsInt(out num5) ? num5 : 0;
                    step2.SignatureType = obj5.Value<string>("signatureType").IsInt(out num6) ? num6 : 0;
                    step2.WorkTime = obj5.Value<string>("workTime").IsDecimal(out num7) ? num7 : decimal.Zero;
                    step2.SendShowMessage = obj5.Value<string>("sendShowMsg");
                    step2.BackShowMessage = obj5.Value<string>("backShowMsg");
                    step2.SendSetWorkTime = obj5.Value<string>("sendSetWorkTime").IsInt(out num8) ? num8 : 0;
                    step2.ExpiredExecuteModel = obj5.Value<string>("timeOutModel").IsInt(out num9) ? num9 : 0;

                    step2.DataEditModel = obj5.Value<string>("dataEditModel").IsInt(out num10) ? num10 : 0;


                    step2.Attachment = obj5.Value<string>("attachment").ToInt(0);
                    step2.BatchExecute = obj5.Value<string>("batch").ToInt(0);



                    JObject obj7 = obj5.Value<JObject>("behavior");
                    StepBase base2 = new StepBase();
                    if (obj7 != null)
                    {
                        int num13;
                        Guid guid7;
                        int num14;
                        int num15;
                        int num16;
                        Guid guid8;
                        int num17;
                        decimal num18;
                        int num19;
                        int num20;

                        Guid guid9;

                        decimal num21;
                        int num22;
                        int num23;



                        base2.BackModel = obj7.Value<string>("backModel").IsInt(out num13) ? num13 : 0;
                        if (obj7.Value<string>("backStep").IsGuid(out guid7))
                        {
                            base2.BackStepId = new Guid?(guid7);
                        }
                        base2.BackType = obj7.Value<string>("backType").IsInt(out num14) ? num14 : 0;
                        base2.BackSelectUser = obj7.Value<string>("backSelectUser").IsInt(out num15) ? num15 : 0;
                        base2.DefaultHandler = obj7.Value<string>("defaultHandler");
                        base2.FlowType = obj7.Value<string>("flowType").IsInt(out num16) ? num16 : 0;

                        if (obj7.Value<string>("handlerStep").IsGuid(out guid8))
                        {
                            base2.HandlerStepId = new Guid?(guid8);
                        }
                        base2.HandlerType = obj7.Value<string>("handlerType");
                        base2.HanlderModel = obj7.Value<string>("hanlderModel").IsInt(out num17) ? num17 : 0;
                        base2.Percentage = obj7.Value<string>("percentage").IsDecimal(out num18) ? num18 : decimal.Zero;
                        base2.RunSelect = obj7.Value<string>("runSelect").IsInt(out num19) ? num19 : 0;
                        base2.SelectRange = obj7.Value<string>("selectRange");
                        base2.ValueField = obj7.Value<string>("valueField");

                        base2.Countersignature = obj7.Value<string>("countersignature").IsInt(out num20) ? num20 : 0;


                        //*****************对比对比**************************
                        base2.CountersignatureStartStepId = obj7.Value<string>("countersignatureStartStep").IsGuid(out guid9) ? new Guid?(guid9) : (nullable = null);

                        base2.CountersignaturePercentage = obj7.Value<string>("countersignaturePercentage").IsDecimal(out num21) ? num21 : decimal.Zero;


                        base2.SubFlowStrategy = obj7.Value<string>("subflowstrategy").IsInt(out num22) ? num22 : 0;
                        base2.ConcurrentModel = obj7.Value<string>("concurrentModel").IsInt(out num23) ? num23 : 0;
                        base2.DefaultHandlerSqlOrMethod = obj7.Value<string>("defaultHandlerSqlOrMethod");
                        base2.AutoConfirm = obj7.Value<string>("autoConfirm").ToInt(0);
                        base2.SkipIdenticalUser = obj7.Value<string>("skipIdenticalUser").ToInt(0);
                        base2.SkipMethod = obj7.Value<string>("skipMethod");
                        base2.SendToBackStep = obj7.Value<string>("sendToBackStep").ToInt(0);
                    }
                    step2.StepBase = base2;
                    StepCopyFor @for = new StepCopyFor();
                    JObject obj8 = obj5.Value<JObject>("copyFor");
                    if (obj8 != null)
                    {
                        @for.MemberId = obj8.Value<string>("memberId");
                        @for.HandlerType = obj8.Value<string>("handlerType");
                        @for.Steps = obj8.Value<string>("steps");
                        @for.MethodOrSql = obj8.Value<string>("methodOrSql");

                        @for.CopyforTime = obj8.Value<string>("time").ToInt(0);

                    }
                    step2.StepCopyFor = @for;
                    List<StepButton> list4 = new List<StepButton>();
                    JArray array4 = obj5.Value<JArray>("buttons");
                    if (array4 != null)
                    {
                        foreach (JObject obj11 in array4)
                        {
                            Guid guid10;
                            StepButton button = new StepButton();
                            if (obj11.Value<string>("id").IsGuid(out guid10))
                            {
                                RoadFlow.Model.FlowButton button2 = new FlowButton().Get(guid10);
                                button.Id = guid10;
                                button.Note = "";
                                string str7 = obj11.Value<string>("showTitle");
                                button.ShowTitle = str7;
                                button.Sort = obj11.Value<int>("sort");
                                if (button2 != null)
                                {
                                    button.Note = button2.Note;
                                    button.ShowTitle = str7.IsNullOrWhiteSpace() ? button2.Title : str7;
                                }
                            }
                            list4.Add(button);
                        }
                    }
                    step2.StepButtons = list4;
                    JObject obj9 = obj5.Value<JObject>("event");
                    StepEvent event2 = new StepEvent();
                    if (obj9 != null)
                    {
                        event2.BackAfter = obj9.Value<string>("backAfter");
                        event2.BackBefore = obj9.Value<string>("backBefore");
                        event2.SubmitAfter = obj9.Value<string>("submitAfter");
                        event2.SubmitBefore = obj9.Value<string>("submitBefore");
                        event2.SubFlowActivationBefore = obj9.Value<string>("subflowActivationBefore");
                        event2.SubFlowCompletedBefore = obj9.Value<string>("subflowCompletedBefore");
                    }
                    step2.StepEvent = event2;
                    JArray array5 = obj5.Value<JArray>("forms");
                    StepForm form = new StepForm();
                    if ((array5 != null) && (array5.Count > 0))
                    {
                        Guid guid11;
                        Guid guid12;
                        JObject obj12 = (JObject)array5.First;
                        if (obj12.Value<string>("id").IsGuid(out guid11))
                        {
                            form.Id = guid11;
                        }
                        form.Name = obj12.Value<string>("name");
                        if (obj12.Value<string>("idApp").IsGuid(out guid12))
                        {
                            form.MobileId = guid12;
                        }
                        form.MobileName = obj12.Value<string>("nameApp");
                    }
                    step2.StepForm = form;
                    JArray array6 = obj5.Value<JArray>("fieldStatus");
                    List<StepFieldStatus> list5 = new List<StepFieldStatus>();
                    if (array6 != null)
                    {
                        foreach (JObject obj13 in array6)
                        {
                            int num24;
                            int num25;
                            StepFieldStatus status = new StepFieldStatus
                            {
                                Check = obj13.Value<string>("check").IsInt(out num24) ? num24 : 0,
                                Field = obj13.Value<string>("field"),
                                Status = obj13.Value<string>("status").IsInt(out num25) ? num25 : 0
                            };
                            list5.Add(status);
                        }
                    }
                    step2.StepFieldStatuses = list5;
                    JObject obj10 = obj5.Value<JObject>("subflow");
                    StepSubFlow flow2 = new StepSubFlow();
                    if (obj10 != null)
                    {
                        Guid guid13;
                        int num26;
                        int num27;
                        if (obj10.Value<string>("flowId").IsGuid(out guid13))
                        {
                            flow2.SubFlowId = guid13;
                        }
                        flow2.SubFlowStrategy = obj10.Value<string>("flowStrategy").IsInt(out num26) ? num26 : 0;
                        flow2.TaskType = obj10.Value<string>("taskType").IsInt(out num27) ? num27 : 0;
                    }
                    step2.StepSubFlow = flow2;
                    list2.Add(step2);
                }
            }
            run.Steps = list2;
            if (list2.Count == 0)
            {
                errMsg = (localizer == null) ? "流程至少需要一个步骤" : localizer["Install_OneStep"];

                return null;
            }
            JArray array3 = obj2.Value<JArray>("lines");
            List<Line> list3 = new List<Line>();
            if (array3 != null)
            {
                foreach (JObject obj14 in array3)
                {
                    Guid guid14;
                    Guid guid15;
                    Guid guid16;
                    Line line = new Line
                    {
                        Id = obj14.Value<string>("id").IsGuid(out guid14) ? guid14 : Guid.Empty,
                        FromId = obj14.Value<string>("from").IsGuid(out guid15) ? guid15 : Guid.Empty,
                        ToId = obj14.Value<string>("to").IsGuid(out guid16) ? guid16 : Guid.Empty,
                        CustomMethod = obj14.Value<string>("customMethod"),
                        SqlWhere = obj14.Value<string>("sql")
                    };
                    if (obj14.Value<JArray>("organize") != null)
                    {
                        line.OrganizeExpression = obj14.Value<JArray>("organize").ToString(0, Array.Empty<JsonConverter>());
                    }
                    list3.Add(line);
                }
            }
            run.Lines = list3;
            Step step = null;
            using (List<Step>.Enumerator enumerator3 = run.Steps.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    Step step1 = enumerator3.Current;
                    if (run.Lines.Find(delegate (Line p) {
                        return p.ToId == step1.Id;
                    }) == null)
                    {
                        step = step1;
                        goto Label_0D82;
                    }
                }
            }
            Label_0D82:
            if (step == null)
            {
                errMsg = (localizer == null) ? "流程没有开始步骤" : localizer["Install_NotStartStep"];

                return null;
            }
            run.FirstStepId = step.Id;
            return run;
        }











    
        /// <summary>
        /// 获取流程步骤实体，通过流程Guid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isCache"></param>
        /// <param name="currentTask"></param>
        /// <returns></returns>
        public RoadFlow.Model.FlowRun GetFlowRunModel(Guid id, bool isCache = true, RoadFlow.Model.FlowTask currentTask = null)
        {
            string str2;
            if ((currentTask != null) && currentTask.BeforeStepId.IsNotEmptyGuid())
            {
                string str4;
                string str3 = "roadflow_cache_flowrun__" + currentTask.BeforeStepId.Value.ToNString() + "_" + currentTask.GroupId.ToNString();
                if (isCache)
                {
                    object obj2 = IO.Get(str3);
                    if (obj2 != null)
                    {
                        return (RoadFlow.Model.FlowRun)obj2;
                    }
                }
                RoadFlow.Model.FlowDynamic dynamic = new FlowDynamic().Get(currentTask.BeforeStepId.Value, currentTask.GroupId);
                RoadFlow.Model.FlowRun run2 = ((dynamic == null) || dynamic.FlowJSON.IsNullOrWhiteSpace()) ? null : this.GetFlowRunModel(dynamic.FlowJSON, out str4);
                if (run2 != null)
                {
                    IO.Insert(str3, run2);
                }
                return run2;
            }
            string key = "roadflow_cache_flowrun_" + id.ToString("N");
            if (isCache)
            {
                object obj3 = IO.Get(key);
                if (obj3 != null)
                {
                    return (RoadFlow.Model.FlowRun)obj3;
                }
            }
            RoadFlow.Model.Flow flow = this.Get(id);
            if (flow == null)
            {
                return null;
            }
            RoadFlow.Model.FlowRun flowRunModel = this.GetFlowRunModel(flow.RunJSON.IsNullOrWhiteSpace() ? flow.DesignerJSON : flow.RunJSON, out str2);
            if (flowRunModel != null)
            {
                IO.Insert(key, flowRunModel);
            }
            return flowRunModel;
        }

      

        /// <summary>
        /// 获取主流程
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.Flow> GetManageFlow(Guid userId)
        {
            List<RoadFlow.Model.Flow> all = this.GetAll();
            if (all.Count == 0)
            {
                return new List<RoadFlow.Model.Flow>();
            }
            return all.FindAll(delegate (RoadFlow.Model.Flow p) {
                return p.Manager.ContainsIgnoreCase(userId.ToString());
            });

        }

        /// <summary>
        /// 通过用户id获取主流程
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Guid> GetManageFlowIds(Guid userId)
        {
            List<Guid> list = new List<Guid>();
            foreach (RoadFlow.Model.Flow flow in this.GetManageFlow(userId))
            {
                list.Add(flow.Id);
            }
            return list;
        }


        /// <summary>
        /// 获取主流程实例
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.Flow> GetManageInstanceFlow(Guid userId)
        {
            List<RoadFlow.Model.Flow> all = this.GetAll();
            if (all.Count == 0)
            {
                return new List<RoadFlow.Model.Flow>();
            }
            return all.FindAll(delegate (RoadFlow.Model.Flow p) {
                return p.InstanceManager.ContainsIgnoreCase(userId.ToString());
            });

        }

        /// <summary>
        /// 通过用户id获取主流程实例
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Guid> GetManageInstanceFlowIds(Guid userId)
        {
            List<Guid> list = new List<Guid>();
            foreach (RoadFlow.Model.Flow flow in this.GetManageInstanceFlow(userId))
            {
                list.Add(flow.Id);
            }
            return list;
        }



        /// <summary>
        /// 获取主实例选项
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetManageInstanceOptions(Guid userId, string value = "")
        {
            return this.GetFlowOptions(this.GetManageInstanceFlow(userId), value);
        }



        /// <summary>
        /// 获取流程名称
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public string GetName(Guid flowId)
        {
            if (!flowId.IsEmptyGuid())
            {
                RoadFlow.Model.Flow flow = this.Get(flowId);
                if (flow != null)
                {
                    return flow.Name;
                }
            }
            return string.Empty;
        }


        /// <summary>
        /// 获取下一个流程步骤
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public List<Step> GetNextSteps(RoadFlow.Model.FlowRun flowRunModel, Guid stepId)
        {
            List<Step> list = new List<Step>();
            if (flowRunModel == null)
            {
                return list;
            }
            using (List<Line>.Enumerator enumerator = flowRunModel.Lines.FindAll(delegate (Line p) {
                return p.FromId == stepId;
            }).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Line line = enumerator.Current;
                    Step step = flowRunModel.Steps.Find(delegate (Step p) {
                        return p.Id == line.ToId;
                    });
                    if (step != null)
                    {
                        list.Add(step);
                    }
                }
            }
            return Enumerable.ToList<Model.FlowRunModel.Step>((IEnumerable<Model.FlowRunModel.Step>)Enumerable.ThenBy<Model.FlowRunModel.Step, decimal>(Enumerable.OrderBy<Model.FlowRunModel.Step, decimal>((IEnumerable<Model.FlowRunModel.Step>)list,
                key=> key.Position_Y),
               key=> key.Position_X));
            //return Enumerable.ToList<Step>((IEnumerable<Step>)Enumerable.ThenBy<Step, decimal>(Enumerable.OrderBy<Step, decimal>((IEnumerable<Step>)list, s_c.s_9__18_1 ?? s_c.s_9__18_1 = new Func<Step, decimal>(s_c.s_9.GetNextStepsb__18_1))), s_c.s_9__18_2 ?? (s_c.s_9__18_2 = new Func<Step, decimal>(s_c.s_9.GetNextStepsb__18_2)));
        }

        /// <summary>
        /// 获取下一个流程步骤
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public List<Step> GetNextSteps(Guid flowId, Guid stepId)
        {
            return this.GetNextSteps(this.GetFlowRunModel(flowId, true,null), stepId);
        }

        /// <summary>
        /// 获取流程选项
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetOptions(string value = "")
        {
            return this.GetFlowOptions(this.GetAll(), value);
        }

       
        public DataTable GetPagerList(out int count, int size, int number, List<Guid> flowIdList, string name, string type, string order, int status = -1)
        {
            return this.flowData.GetPagerList(out count, size, number, flowIdList, name, type, order, status);
        }


        /// <summary>
        /// 获取前一个步骤流程
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public List<Step> GetPrevSteps(RoadFlow.Model.FlowRun flowRunModel, Guid stepId)
        {
            List<Step> list = new List<Step>();
            if ((flowRunModel != null) && !stepId.IsEmptyGuid())
            {
                using (List<Line>.Enumerator enumerator = flowRunModel.Lines.FindAll(delegate (Line p) {
                    return p.ToId == stepId;
                }).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Line line = enumerator.Current;
                        Step step = flowRunModel.Steps.Find(delegate (Step p) {
                            return p.Id == line.FromId;
                        });
                        if (step != null)
                        {
                            list.Add(step);
                        }
                    }
                }
            }
            return list;
        }


      
        /// <summary>
        /// 获取范围流程
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId1"></param>
        /// <param name="stepId2"></param>
        /// <returns></returns>
        public List<Step> GetRangeSteps(RoadFlow.Model.FlowRun flowRunModel, Guid stepId1, Guid stepId2)
        {
            Predicate<Step> s_9__0=null;
            if (!stepId1.IsEmptyGuid())
            {
                foreach (Step step in this.GetNextSteps(flowRunModel, stepId1))
                {
                    List<Step> allNextSteps = this.GetAllNextSteps(flowRunModel, step.Id);
                    if (allNextSteps.Exists(s_9__0 ?? (s_9__0 = delegate (Step p) {
                        return p.Id == stepId2;
                    })))
                    {
                        List<Step> list2 = new List<Step> {
                        step
                    };
                        foreach (Step step2 in allNextSteps)
                        {
                            list2.Add(step2);
                            if (step2.Id == stepId2)
                            {
                                return list2;
                            }
                        }
                    }
                }
            }
            return new List<Step>();

        }

        /// <summary>
        /// 获取开始流程
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.FlowRun> GetStartFlows(Guid userId)
        {
            List<RoadFlow.Model.FlowRun> list = new List<RoadFlow.Model.FlowRun>();
            User user = new User();
            foreach (RoadFlow.Model.Flow flow in this.GetAll())
            {
                RoadFlow.Model.FlowRun flowRunModel = this.GetFlowRunModel(flow.Id, true,null);
                if (((flowRunModel != null) && (flowRunModel.Status == 1)) && !flowRunModel.FirstStepId.IsEmptyGuid())
                {
                    Step step = flowRunModel.Steps.Find(delegate (Step p) {
                        return p.Id == flowRunModel.FirstStepId;
                    });
                    if ((step != null) && (step.StepBase.DefaultHandler.IsNullOrWhiteSpace() || user.Contains(step.StepBase.DefaultHandler, userId)))
                    {
                        list.Add(flowRunModel);
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// 获取状态名称
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public string GetStatusTitle(int status)
        {
            string str = string.Empty;
            switch (status)
            {
                case 0:
                    return "设计中";

                case 1:
                    return "已安装";

                case 2:
                    return "已卸载";

                case 3:
                    return "已删除";
            }
            return str;
        }

        /// <summary>
        /// 获取步骤名称
        /// </summary>
        /// <param name="flowRunModel"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public string GetStepName(RoadFlow.Model.FlowRun flowRunModel, Guid stepId)
        {
            if (!stepId.IsEmptyGuid())
            {
                if (flowRunModel == null)
                {
                    return string.Empty;
                }
                Step step = flowRunModel.Steps.Find(delegate (Step p) {
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
        /// 获取步骤名称
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public string GetStepName(Guid flowId, Guid stepId)
        {
            if (!flowId.IsEmptyGuid())
            {
                return this.GetStepName(this.GetFlowRunModel(flowId, true,null), stepId);
            }
            return string.Empty;
        }

       
        /// <summary>
        /// 导入流程
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string ImportFlow(string json)
        {
            IEnumerator<JToken> enumerator;
            if (json.IsNullOrWhiteSpace())
            {
                return "要导入的JSON为空!";
            }
            JObject obj2 = null;
            try
            {
                obj2 = JObject.Parse(json);
            }
            catch
            {
                return "json解析错误!";
            }
            JArray array = obj2.Value<JArray>("flows");
            if (array != null)
            {
                using (enumerator = array.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        RoadFlow.Model.Flow flow = ((JObject)enumerator.Current).ToObject<RoadFlow.Model.Flow>();
                        if (flow != null)
                        {
                            if (this.Get(flow.Id) != null)
                            {
                                this.Update(flow);
                            }
                            else
                            {
                                this.Add(flow);
                            }
                        }
                    }
                }
            }
            JArray array2 = obj2.Value<JArray>("applibrarys");
            AppLibrary library = new AppLibrary();
            if (array2 != null)
            {
                using (enumerator = array2.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        RoadFlow.Model.AppLibrary appLibrary = ((JObject)enumerator.Current).ToObject<RoadFlow.Model.AppLibrary>();
                        if (appLibrary != null)
                        {
                            if (library.Get(appLibrary.Id) != null)
                            {
                                library.Update(appLibrary);
                            }
                            else
                            {
                                library.Add(appLibrary);
                            }
                        }
                    }
                }
            }
            JArray array3 = obj2.Value<JArray>("forms");
            Form form = new Form();
            if (array3 != null)
            {
                using (enumerator = array3.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        RoadFlow.Model.Form form2 = ((JObject)enumerator.Current).ToObject<RoadFlow.Model.Form>();
                        if (form2 != null)
                        {
                            if (form.Get(form2.Id) != null)
                            {
                                form.Update(form2);
                            }
                            else
                            {
                                form.Add(form2);
                            }
                            if (form2.Status == 1)
                            {
                              
                            
                                string path = Tools.GetContentRootPath() + "/Areas/RoadFlowCore/Views/FormDesigner/form/";
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                FileStream stream1 = File.Open(path + form2.Id + ".cshtml", (FileMode)FileMode.OpenOrCreate, (FileAccess)FileAccess.ReadWrite, (FileShare)FileShare.None);
                                stream1.SetLength(0L);
                                StreamWriter writer1 = new StreamWriter((Stream)stream1, Encoding.UTF8);
                                writer1.Write(form2.RunHtml);
                                writer1.Close();
                                stream1.Close();
                            }
                        }
                    }
                }
            }
            return "1";
        }

        /// <summary>
        /// 安装流程
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string Install(string json)
        {
            string str2;
            string str = this.Save(json);
            if (!"1".Equals(str))
            {
                return str;
            }
            RoadFlow.Model.FlowRun flowRunModel = this.GetFlowRunModel(json, out str2);
            if (flowRunModel == null)
            {
                return str2;
            }
            RoadFlow.Model.Flow flow = this.Get(flowRunModel.Id);
            if (flow == null)
            {
                return "未找到流程实体";
            }
            flow.InstallDate = new DateTime?(DateTimeExtensions.Now);
            flow.InstallUser = new Guid?(User.CurrentUserId);
            flow.RunJSON = json;
            flow.Status = 1;
            this.flowData.Install(flow);
            this.ClearCache(flow.Id);
            Log.Add("安装了流程-" + flow.Name, json, LogType.流程管理, "", "", str2, "", "", "", "", "");
            return "1";
        }

        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string Save(string json)
        {
            JObject obj2 = null;
            Guid guid;
            Guid guid2;
            Guid guid3;
            try
            {
                obj2 = JObject.Parse(json);
            }
            catch
            {
                return "JSON解析错误";
            }
            if (obj2 == null)
            {
                return "JSON解析错误";
            }
            string str = obj2.Value<string>("name");
            string str2 = obj2.Value<string>("type");
            string str3 = obj2.Value<string>("systemId");
            if (!obj2.Value<string>("id").IsGuid(out guid))
            {
                return "流程ID错误";
            }
            if (str.IsNullOrWhiteSpace())
            {
                return "流程名称为空";
            }
            if (!str2.IsGuid(out guid2))
            {
                return "流程分类错误";
            }
            Flow flow = new Flow();
            RoadFlow.Model.Flow flow2 = flow.Get(guid);
            bool flag = false;
            if (flow2 == null)
            {
                flag = true;
                flow2 = new RoadFlow.Model.Flow
                {
                    Id = guid,
                    CreateDate = DateTimeExtensions.Now,
                    CreateUser = User.CurrentUserId,
                    Status = 0
                };
            }
            flow2.DesignerJSON = json;
            flow2.InstanceManager = obj2.Value<string>("instanceManager");
            flow2.Manager = obj2.Value<string>("manager");
            flow2.Name = str;
            flow2.FlowType = guid2;
            flow2.Note = obj2.GetValue("note").ToString();
            flow2.SystemId = str3.IsGuid(out guid3) ? new Guid?(guid3) : null;
            if (flag)
            {
                flow.Add(flow2);
            }
            else
            {
                flow.Update(flow2);
            }
            Log.Add("保存了流程-" + flow2.Name, json, LogType.流程管理, "", "", "", "", "", "", "", "");
            return "1";
        }



        /// <summary>
        /// 流程另存为
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="newFlowName"></param>
        /// <returns></returns>
        public string SaveAs(Guid flowId, string newFlowName)
        {
            RoadFlow.Model.Flow flow = this.Get(flowId);
            if (flow == null)
            {
                return "未找到要另存的流程!";
            }
            flow.Id = Guid.NewGuid();
            flow.CreateDate = DateTimeExtensions.Now;
            if (flow.InstallDate.HasValue)
            {
                flow.InstallDate = new DateTime?(flow.CreateDate);
            }
            flow.Name = newFlowName;
            JObject obj2 = null;
            if (!flow.RunJSON.IsNullOrWhiteSpace())
            {
                obj2 = JObject.Parse(flow.RunJSON);
            }
            else if (!flow.DesignerJSON.IsNullOrWhiteSpace())
            {
                obj2 = JObject.Parse(flow.DesignerJSON);
            }
            if (obj2 != null)
            {
                obj2["id"] = (JToken)flow.Id.ToString();
                obj2["name"] = (JToken)flow.Name;
                JArray array = obj2.Value<JArray>("lines");
                foreach (JObject obj3 in obj2.Value<JArray>("steps"))
                {
                    string str2 = Guid.NewGuid().ToString();
                    string str3 = obj3.Value<string>("id");
                    foreach (JObject obj4 in array)
                    {
                        if (obj4.Value<string>("from").EqualsIgnoreCase(str3))
                        {
                            obj4["from"] = (JToken)str2;
                        }
                        if (obj4.Value<string>("to").EqualsIgnoreCase(str3))
                        {
                            obj4["to"] = (JToken)str2;
                        }
                    }
                    obj3["id"] = (JToken)str2;
                }
                using (IEnumerator<JToken> enumerator = array.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ((JObject)enumerator.Current)["id"] = (JToken)Guid.NewGuid().ToString();
                    }
                }
            }
            string str = obj2.ToString(0, Array.Empty<JsonConverter>());
            if (!flow.RunJSON.IsNullOrWhiteSpace())
            {
                flow.RunJSON = str;
            }
            if (!flow.DesignerJSON.IsNullOrWhiteSpace())
            {
                flow.DesignerJSON = str;
            }
            this.Add(flow);
            return flow.Id.ToString();
        }



        public int Update(RoadFlow.Model.Flow flow)
        {
            return this.flowData.Update(flow);
        }




        public bool IsLastStep(RoadFlow.Model.FlowRun flowRunModel, Guid stepId)
        {
            return (((flowRunModel == null) || (flowRunModel.Lines.Count == 0)) || !flowRunModel.Lines.Exists(delegate (Line p) {
                return p.FromId == stepId;
            }));
        }














       
}


 


    #endregion


}
