using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility.Cache;
using RoadFlow.Model.FlowRunModel;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Business
{
    public class FlowDynamic
    {
        // Fields
        private readonly RoadFlow.Data.FlowDynamic flowDynamicData = new RoadFlow.Data.FlowDynamic();

        // Methods
        public int Add(RoadFlow.Model.FlowDynamic flowDynamic)
        {
            return this.flowDynamicData.Add(flowDynamic);
        }

        /// <summary>
        /// 动态添加流程步骤
        /// </summary>
        /// <param name="executeModel"></param>
        /// <returns></returns>
        public RoadFlow.Model.FlowRun Add(Execute executeModel, List<RoadFlow.Model.FlowTask> groupTasks)
        {
            string str3;
            if ((executeModel == null) || executeModel.GroupId.IsEmptyGuid())
            {
                return null;
            }
            string flowJSON = string.Empty;
            if (((groupTasks != null) && (groupTasks.Count() > 0)) && groupTasks.Exists(
                key=>key.BeforeStepId.IsNotEmptyGuid()))
            {
                RoadFlow.Model.FlowDynamic dynamic = new FlowDynamic().Get(Enumerable.First<RoadFlow.Model.FlowTask>(Enumerable.Where<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)Enumerable.OrderByDescending<RoadFlow.Model.FlowTask, int>((IEnumerable<RoadFlow.Model.FlowTask>)groupTasks,
                   key=>key.Sort), 
                   key=> key.BeforeStepId.IsNotEmptyGuid())).BeforeStepId.Value, executeModel.GroupId);
                if (dynamic != null)
                {
                    flowJSON = dynamic.FlowJSON;
                }
            }

            if (flowJSON.IsNullOrWhiteSpace())
            {
                RoadFlow.Model.Flow flow = new Flow().Get(executeModel.FlowId);
                if (flow != null)
                {
                    flowJSON = flow.RunJSON.IsNullOrWhiteSpace() ? flow.DesignerJSON : flow.RunJSON;
                }
            }
            if (flowJSON.IsNullOrWhiteSpace())
            {
                return null;
            }
            JObject obj2 = null;
            try
            {
                obj2 = JObject.Parse(flowJSON);   
            }
            catch
            {
                return null;
            }
            if (!obj2.ContainsKey("steps"))
            {
                return null;
            }
            JArray array = obj2.Value<JArray>("steps");
            JArray array2 = obj2.Value<JArray>("lines");
            List<IGrouping<Guid?, ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>> list = Enumerable.ToList<IGrouping<Guid?, ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>>(Enumerable.GroupBy<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>, Guid?>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)executeModel.Steps.FindAll(
                key=> key.Item3.IsNotEmptyGuid()), 
                key=>key.Item3));
            string json = string.Empty;
            User user = new User();
            foreach (IGrouping<Guid?, ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>> grouping in list)
            {
                Guid? nullable;
                int num = 0;
                using (IEnumerator<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>> enumerator2 = grouping.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?> tuple = enumerator2.Current;
                        Guid guid = tuple.Item1;
                        string str5 = tuple.Item2;
                        Guid? beforeStepId = tuple.Item3;
                        int? nullable2 = tuple.Item4;
                        List<RoadFlow.Model.User> users = tuple.Item5;
                        //动态添加步骤位置
                        IEnumerable<JToken> enumerable = from p in array where beforeStepId.ToString().ToLower()==p["id"].ToString().ToLower() select p;
                        if (Enumerable.Count<JToken>(enumerable) == 0)
                        {
                            num++;
                        }
                        else
                        {
                            JToken token = Enumerable.First<JToken>(enumerable);
                            Guid guid2 = guid;
                            nullable = beforeStepId;
                            if (nullable2.HasValue ? (guid2 != nullable.GetValueOrDefault()) : true)
                            {
                                JToken token2 = token.DeepClone();

                                token2["id"]= (JToken)guid;
                                token2["name"] =(JToken)str5;
                                token2["dynamic"]= 0;
                                token2["behavior"]["runSelect"]= 0;
                                token2["behavior"]["defaultHandler"]= (JToken)user.GetUserIds(users);
                                token2["position"]["y"]= token2["position"]["y"].ToString().ToInt(-2147483648) + (70 * num);
                                array.Add(token2);
                            }
                            else
                            {
                                token["dynamic"]= 0;
                                token["behavior"]["runSelect"]= 0;
                                token["behavior"]["defaultHandler"]= (JToken)user.GetUserIds(users);
                            }


                            //TODO动态添加顺序审批连接线方式
                            if (nullable2.HasValue && (nullable2.Value == 1))
                            {
                                if (num == (Enumerable.Count<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)grouping) - 1))
                                {
                                    IEnumerable<JToken> oldLines = from p in array2 where beforeStepId.ToString().ToLower() == p["from"].ToString().ToLower() select  p;
                                    if (Enumerable.Any<JToken>(oldLines))
                                    {
                                        JToken token3 = Enumerable.First<JToken>(oldLines).DeepClone();
                                        token3["id"]= (JToken)Guid.NewGuid().ToString();
                                        token3["to"]= (JToken)Enumerable.ElementAt<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)grouping, num).Item1.ToString();
                                        token3["from"]= (num == 0) ? ((JToken)beforeStepId.Value.ToString()) : ((JToken)Enumerable.ElementAt<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)grouping, num - 1).Item1.ToString());
                                        guid2 = guid;
                                        nullable = beforeStepId;
                                        if (nullable.HasValue ? (guid2 != nullable.GetValueOrDefault()) : true)
                                        {
                                            array2.Add(token3);
                                        }
                                        //不明白条件意义在何处
                                        if (Enumerable.Any<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)(from p in (IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)grouping   select p)))
                                        {
                                            Enumerable.First<JToken>(oldLines)["from"]= (JToken)Enumerable.Last<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)grouping).Item1.ToString();
                                        }
                                    }
                                    else
                                    {
                                        IEnumerable<JToken> enumerable2 = from p in array2 where beforeStepId.ToString().ToLower() == p["from"].ToString().ToLower() select p;
                                        if (Enumerable.Any<JToken>(enumerable2))
                                        {
                                            JToken token4 = Enumerable.First<JToken>(enumerable2).DeepClone();
                                            token4["id"]=(JToken)Guid.NewGuid().ToString();
                                            token4["to"]= (JToken)Enumerable.ElementAt<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)grouping, num).Item1.ToString();
                                            token4["from"]= (num == 0) ? ((JToken)beforeStepId.Value.ToString()) : ((JToken)Enumerable.ElementAt<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)grouping, num - 1).Item1.ToString());
                                            guid2 = guid;
                                            nullable = beforeStepId;
                                            if (nullable.HasValue ? (guid2 != nullable.GetValueOrDefault()) : true)
                                            {
                                                array2.Add(token4);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    IEnumerable<JToken> enumerable3 = from p in array2 where beforeStepId.ToString().ToLower() == p["from"].ToString().ToLower() select p;
                                    if (Enumerable.Any<JToken>(enumerable3))
                                    {
                                        JToken token5 = Enumerable.First<JToken>(enumerable3).DeepClone();
                                        token5["id"]= (JToken)Guid.NewGuid().ToString();
                                        token5["from"]= (num == 0) ? ((JToken)beforeStepId.Value.ToString()) : ((JToken)Enumerable.ElementAt<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)grouping, num - 1).Item1.ToString());
                                        token5["to"]= (JToken)Enumerable.ElementAt<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>((IEnumerable<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>)grouping, num).Item1.ToString();
                                        guid2 = guid;
                                        nullable = beforeStepId;
                                        if (nullable.HasValue ? (guid2 != nullable.GetValueOrDefault()) : true)
                                        {
                                            array2.Add(token5);
                                        }
                                    }
                                }
                            }
                            else   //动态添加并行审批的连接线方式
                            {
                                IEnumerable<JToken> enumerable4 = from p in array2 where beforeStepId.ToString().ToLower() == p["to"].ToString().ToLower() select p;
                                if (Enumerable.Any<JToken>(enumerable4))
                                {
                                    JToken token6 = Enumerable.First<JToken>(enumerable4).DeepClone();
                                    token6["id"]= (JToken)Guid.NewGuid().ToString();
                                    token6["to"]= (JToken)guid;
                                    guid2 = guid;
                                    nullable = beforeStepId;
                                    if (nullable.HasValue ? (guid2 != nullable.GetValueOrDefault()) : true)
                                    {
                                        array2.Add(token6);
                                    }
                                }
                                IEnumerable<JToken> enumerable5 = from p in array2 where beforeStepId.ToString().ToLower() == p["from"].ToString().ToLower() select p;
                                int num2 = Enumerable.Count<JToken>(enumerable5);
                                for (int i = 0; i < num2; i++)
                                {
                                    JToken token7 = Enumerable.ElementAt<JToken>(enumerable5, i).DeepClone();
                                    token7["id"]= (JToken)Guid.NewGuid().ToString();
                                    token7["from"]= (JToken)guid;
                                    guid2 = guid;
                                    nullable = beforeStepId;
                                    if (nullable.HasValue ? (guid2 != nullable.GetValueOrDefault()) : true)
                                    {
                                        array2.Add(token7);
                                    }
                                }
                            }
                            num++;
                        }
                    }
                }
                RoadFlow.Model.FlowDynamic flowDynamic = this.Get(grouping.Key.Value, executeModel.GroupId);
                string str4 = obj2.ToString(0, Array.Empty<JsonConverter>());
                if (flowDynamic == null)
                {
                    flowDynamic = new RoadFlow.Model.FlowDynamic
                    {
                        StepId = grouping.Key.Value,
                        GroupId = executeModel.GroupId,
                        FlowJSON = str4
                    };
                    this.Add(flowDynamic);
                }
                else
                {
                    flowDynamic.FlowJSON = str4;
                    this.Update(flowDynamic);
                }
                IO.Remove("roadflow_cache_flowrun__" + flowDynamic.StepId.ToNString() + "_" + flowDynamic.GroupId.ToNString());
                nullable= grouping.Key;
                Guid? nullable3 = Enumerable.Last<IGrouping<Guid?, ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>>((IEnumerable<IGrouping<Guid?, ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>>)list).Key;
                if ((nullable.HasValue == nullable3.HasValue) ? (nullable.HasValue ? (nullable.GetValueOrDefault() == nullable3.GetValueOrDefault()) : true) : false)
                {
                    json = str4;
                }
            }
            return new RoadFlow.Business.Flow().GetFlowRunModel(json,  out str3);
        }

        public int Delete(RoadFlow.Model.FlowDynamic flowDynamic)
        {
            return this.flowDynamicData.Delete(flowDynamic);
        }

        public int Delete(Guid stepId, Guid groupId)
        {
            return this.flowDynamicData.Delete(stepId, groupId);
        }

        public RoadFlow.Model.FlowDynamic Get(Guid StepId, Guid groupId)
        {
            return this.flowDynamicData.Get(StepId, groupId);
        }

        public int Update(RoadFlow.Model.FlowDynamic flowDynamic)
        {
            return this.flowDynamicData.Update(flowDynamic);
        }

       
    }

}
