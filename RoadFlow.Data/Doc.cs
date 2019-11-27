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
    public class Doc
    {
        // Methods
        public int Add(RoadFlow.Model.Doc doc, List<RoadFlow.Model.User> users = null)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.Doc>(doc);
                if (users != null)
                {
                    foreach (RoadFlow.Model.User user in users)
                    {
                        object[] objects = new object[] { doc.Id, user.Id, (int)0, DBNull.Value };
                        context.Execute("INSERT INTO RF_DocUser VALUES({0},{1},{2},{3})", objects);
                    }
                }
                return context.SaveChanges();
            }
        }

        public int Delete(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                context.Execute("DELETE FROM RF_Doc WHERE Id={0}", objects);
                object[] objArray2 = new object[] { id };
                context.Execute("DELETE FROM RF_DocUser WHERE DocId={0}", objArray2);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.Doc Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.Doc>(objects);
            }
        }

        public DataTable GetPagerList(out int count, int size, int number, Guid userId, string title, string dirId, string date1, string date2, string order, int read)
        {
            using (DataContext context = new DataContext())
            {
                DbconnnectionSql sql1 = new DbconnnectionSql(Config.DatabaseType);
                ValueTuple<string, DbParameter[]> tuple1 = sql1.SqlInstance.GetDocSql(userId, title, dirId, date1, date2, order, read);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string str2 = sql1.SqlInstance.GetPaerSql(sql, size, number, out count, param, order);
                return context.GetDataTable(str2, param);
            }
        }

        public int Update(RoadFlow.Model.Doc doc, List<RoadFlow.Model.User> users = null)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.Doc>(doc);
                if (users != null)
                {
                    object[] objects = new object[] { doc.Id };
                    context.Execute("DELETE FROM RF_DocUser WHERE DocId={0}", objects);
                    foreach (RoadFlow.Model.User user in users)
                    {
                        object[] objArray2 = new object[4];
                        objArray2[0] = doc.Id;
                        objArray2[1] = user.Id;
                        objArray2[2] = 0;
                        context.Execute("INSERT INTO RF_DocUser VALUES({0},{1},{2},{3})", objArray2);
                    }
                }
                return context.SaveChanges();
            }
        }

        public int UpdateReadCount(RoadFlow.Model.Doc doc)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { doc.Id };
                context.Execute("UPDATE RF_Doc SET ReadCount=ReadCount+1 WHERE Id={0}", objects);
                return context.SaveChanges();
            }
        }
    }


}
