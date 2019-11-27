using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace RoadFlow.Data
{
    public class Flow
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_flow";

        // Methods

            /// <summary>
            /// 添加流程
            /// </summary>
            /// <param name="flow"></param>
            /// <returns></returns>
        public int Add(RoadFlow.Model.Flow flow)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.Flow>(flow);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_flow");
        }

        public int Delete(RoadFlow.Model.Flow flow)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.Flow>(flow);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.Flow Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.Flow p) {
                return p.Id == id;
            });
        }

        public List<RoadFlow.Model.Flow> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_flow");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.Flow> list = context.QueryAll<RoadFlow.Model.Flow>();
                    IO.Insert("roadflow_cache_flow", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.Flow>)obj2;
        }

        public DataTable GetPagerList(out int count, int size, int number, List<Guid> flowIdList, string name, string type, string order)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetFlowSql(flowIdList, name, type, order);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int Install(RoadFlow.Model.Flow flow)
        {
            using (DataContext context = new DataContext())
            {
                AppLibrary library1 = new AppLibrary();
                RoadFlow.Model.AppLibrary t = library1.GetAll().Find(delegate (RoadFlow.Model.AppLibrary p) {
                    return p.Code.EqualsIgnoreCase(flow.Id.ToString());
                });
                bool flag = false;
                if (t == null)
                {
                    flag = true;
                    t = new RoadFlow.Model.AppLibrary
                    {
                        Id = Guid.NewGuid()
                    };
                }
                t.Title = flow.Name;
                t.Address = "/RoadFlowCore/FlowRun/Index?flowid=" + flow.Id.ToString();
                t.Code = flow.Id.ToString();
                t.OpenMode = 0;
                t.Type = flow.FlowType;
                t.Note = flow.Note;
                //!(context.Add<RoadFlow.Model.AppLibrary>(t)>0) && !true)前面不断是false或者true 都是法拉瑟；
                if (!flag || (!(context.Add<RoadFlow.Model.AppLibrary>(t)>0) && !true))
                {
                    context.Update<RoadFlow.Model.AppLibrary>(t);
                }
                context.Update<RoadFlow.Model.Flow>(flow);
                this.ClearCache();
                library1.ClearCache();
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.Flow flow)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.Flow>(flow);
                return context.SaveChanges();
            }
        }


        public DataTable GetPagerList(out int count, int size, int number, List<Guid> flowIdList, string name, string type, string order, int status = -1)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetFlowSql(flowIdList, name, type, order, status);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }




    }


}
