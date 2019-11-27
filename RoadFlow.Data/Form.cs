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
    public class Form
    {
        // Methods
        public int Add(RoadFlow.Model.Form form)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.Form>(form);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.Form form)
        {
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.Form>(form);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.Form form, RoadFlow.Model.AppLibrary appLibrary)
        {
            using (DataContext context = new DataContext())
            {
                if (form != null)
                {
                    form.Status = 2;
                    context.Update<RoadFlow.Model.Form>(form);
                }
                if (appLibrary != null)
                {
                    context.Remove<RoadFlow.Model.AppLibrary>(appLibrary);
                    new AppLibrary().ClearCache();
                }
                return context.SaveChanges();
            }
        }






        public RoadFlow.Model.Form Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.Form>(objects);
            }
        }

        public List<RoadFlow.Model.Form> GetAll()
        {
            using (DataContext context = new DataContext())
            {
                return context.QueryAll<RoadFlow.Model.Form>();
            }
        }

        public DataTable GetPagerList(out int count, int size, int number, Guid userId, string name, string type, string order, int status = -1)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetFormSql(userId, name, type, order, status);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int Update(RoadFlow.Model.Form form)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.Form>(form);
                return context.SaveChanges();
            }
        }



        public int Delete(RoadFlow.Model.Form form, RoadFlow.Model.AppLibrary appLibrary, int delete = 0)
        {
            using (DataContext context = new DataContext())
            {
                if (form != null)
                {
                    if (delete == 0)
                    {
                        form.Status = 2;
                        context.Update<RoadFlow.Model.Form>(form);
                    }
                    else
                    {
                        context.Remove<RoadFlow.Model.Form>(form);
                    }
                }
                if (appLibrary != null)
                {
                    context.Remove<RoadFlow.Model.AppLibrary>(appLibrary);
                    new AppLibrary().ClearCache();
                }
                return context.SaveChanges();
            }
        }


        public DataTable GetPagerList(out int count, int size, int number, string name, string type, string order, int status = -1)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetFormSql(name, type, order, status);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }





    }


}
