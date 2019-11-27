using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RoadFlow.Data
{
    public class FlowTask
    {
        // Methods
        public int Add(RoadFlow.Model.FlowTask flowTask)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.FlowTask>(flowTask);
                return context.SaveChanges();
            }
        }

        public int DeleteByFlowId(Guid flowId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { flowId };
                context.Execute("DELETE FROM RF_FlowTask WHERE FlowId={0}", objects);
                return context.SaveChanges();
            }
        }


        public int DeleteByGroupId(Guid groupId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { groupId };
                context.Execute("DELETE FROM RF_FlowTask WHERE GroupId={0}", objects);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.FlowTask Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.FlowTask>(objects);
            }
        }

        public DataTable GetCompletedList(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetQueryCompletedTaskSql(userId, flowId, title, startDate, endDate, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }


        public DataTable GetEntrustList(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetQueryEntrustTaskSql(userId, flowId, title, startDate, endDate, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }





        public DataTable GetExpireTasks()
        {
            using (DataContext context = new DataContext())
            {
                string sql = "SELECT Id FROM RF_FlowTask WHERE IsAutoSubmit=1 AND Status IN(0,1) AND CompletedTime<{0}";
                object[] objects = new object[] { DateTimeExtensions.Now };
                return context.GetDataTable(sql, objects);
            }
        }


        public DataTable GetInstanceList(int size, int number, string flowId, string title, string receiveId, string receiveDate1, string receiveDate2, string order, out int count)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetQueryInstanceSql(flowId, title, receiveId, receiveDate1, receiveDate2, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }


        public DataTable GetMyStartList(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string status, string order, out int count)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetQueryMyStartTaskSql(userId, flowId, title, startDate, endDate, status, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }




        public List<RoadFlow.Model.FlowTask> GetListByGroupId(Guid groupId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { groupId };
                return context.Query<RoadFlow.Model.FlowTask>("SELECT * FROM RF_FlowTask WHERE GroupId={0}", objects);
            }
        }

        public List<RoadFlow.Model.FlowTask> GetListBySubFlowGroupId(Guid groupId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Query<RoadFlow.Model.FlowTask>("SELECT * FROM RF_FlowTask WHERE SubFlowGroupId LIKE '%" + groupId.ToString().ToUpper() + "%'", (DbParameter[])null);
            }
        }

        public RoadFlow.Model.FlowTask GetMaxByGroupId(Guid groupId)
        {
            ValueTuple<string, DbParameter[]> queryGroupMaxTaskSql = new DbconnnectionSql(Config.DatabaseType).SqlInstance.GetQueryGroupMaxTaskSql(groupId);
            string sql = queryGroupMaxTaskSql.Item1;
            DbParameter[] parameters = queryGroupMaxTaskSql.Item2;
            using (DataContext context = new DataContext())
            {
                return context.QueryOne<RoadFlow.Model.FlowTask>(sql, parameters);
            }
        }

        public DataTable GetWaitList(int size, int number, Guid userId, string flowId, string title, string startDate, string endDate, string order, out int count, int isBatch = 0)

        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetQueryWaitTaskSql(userId, flowId, title, startDate, endDate, order, isBatch);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int Update(RoadFlow.Model.FlowTask flowTaskModel)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.FlowTask>(flowTaskModel);
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.FlowTask[] flowTaskModels)
        {
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.FlowTask>(flowTaskModels);
                return context.SaveChanges();
            }
        }


        public int Update(List<RoadFlow.Model.FlowTask> removeTasks, List<RoadFlow.Model.FlowTask> updateTasks, List<RoadFlow.Model.FlowTask> addTasks, List<ValueTuple<string, object[]>> executeSqls)
        {
            using (DataContext context = new DataContext())
            {


                if ((removeTasks != null) && Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)removeTasks))
                {
                    context.RemoveRange<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)removeTasks);
                }
                if ((updateTasks != null) && Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)updateTasks))
                {
                    context.UpdateRange<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)updateTasks);
                }
                if ((addTasks != null) && Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)addTasks))
                {
                    context.AddRange<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)addTasks);
                }
                if ((executeSqls != null) && Enumerable.Any<ValueTuple<string, object[]>>((IEnumerable<ValueTuple<string, object[]>>)executeSqls))
                {
                    foreach (ValueTuple<string, object[]> local1 in executeSqls)
                    {
                        string sql = local1.Item1;
                        object[] objects = local1.Item2;
                        context.Execute(sql, objects);
                    }
                }
                return context.SaveChanges();
            }
        }



        public int Update(List<RoadFlow.Model.FlowTask> removeTasks, List<RoadFlow.Model.FlowTask> updateTasks, List<RoadFlow.Model.FlowTask> addTasks, List<ValueTuple<string, object[], int>> executeSqls)
        {
            using (DataContext context = new DataContext())
            {
                if ((executeSqls != null) && Enumerable.Any<ValueTuple<string, object[], int>>((IEnumerable<ValueTuple<string, object[], int>>)executeSqls))
                {
                    foreach (ValueTuple<string, object[], int> local1 in executeSqls)
                    {
                        string sql = local1.Item1;
                        object[] objects = local1.Item2;
                        if (local1.Item3 == 0)
                        {
                            context.Execute(sql, objects);
                        }
                    }
                }
                if ((removeTasks != null) && Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)removeTasks))
                {
                    context.RemoveRange<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)removeTasks);
                }
                if ((updateTasks != null) && Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)updateTasks))
                {
                    context.UpdateRange<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)updateTasks);
                }
                if ((addTasks != null) && Enumerable.Any<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)addTasks))
                {
                    context.AddRange<RoadFlow.Model.FlowTask>((IEnumerable<RoadFlow.Model.FlowTask>)addTasks);
                }
                if ((executeSqls != null) && Enumerable.Any<ValueTuple<string, object[], int>>((IEnumerable<ValueTuple<string, object[], int>>)executeSqls))
                {
                    foreach (ValueTuple<string, object[], int> local2 in executeSqls)
                    {
                        string str2 = local2.Item1;
                        object[] objArray2 = local2.Item2;
                        if (local2.Item3 == 1)
                        {
                            context.Execute(str2, objArray2);
                        }
                    }
                }
                return context.SaveChanges();
            }
        }


        public DataTable GetRemindTasks()
        {
            using (DataContext context = new DataContext())
            {
                string sql = "SELECT Id,Title,ReceiveId,ReceiveName,CompletedTime FROM RF_FlowTask WHERE Status IN(0,1) AND RemindTime<{0}";
                object[] objects = new object[] { DateTimeExtensions.Now };
                return context.GetDataTable(sql, objects);
            }
        }


        public int UpdateRemind(Guid taskId, DateTime remindTime)
        {
            using (DataContext context = new DataContext())
            {
                string sql = "UPDATE RF_FlowTask SET RemindTime={0} WHERE Id={1}";
                object[] objects = new object[] { remindTime, taskId };
                context.SaveChanges();
                return context.Execute(sql, objects);
            }
        }





    }


}
