using RoadFlow.Utility.Cache;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{


  


   

    public class HomeSet
    {
        // Fields
        private RoadFlow.Data.HomeSet homeSetData = new RoadFlow.Data.HomeSet();

        // Methods
        public int Add(RoadFlow.Model.HomeSet homeSet)
        {
            return this.homeSetData.Add(homeSet);
        }

        public int Delete(RoadFlow.Model.HomeSet homeSet)
        {
            return this.homeSetData.Delete(homeSet);
        }

        public int Delete(RoadFlow.Model.HomeSet[] homeSets)
        {
            return this.homeSetData.Delete(homeSets);
        }

        public RoadFlow.Model.HomeSet Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.HomeSet p) {
                return p.Id == id;
            });
        }

        public List<RoadFlow.Model.HomeSet> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_homeset");
            if (obj2 != null)
            {
                return (List<RoadFlow.Model.HomeSet>)obj2;
            }
            Organize organize = new Organize();
            List<RoadFlow.Model.HomeSet> all = this.homeSetData.GetAll();
            foreach (RoadFlow.Model.HomeSet set in all)
            {
                if (!set.UseOrganizes.IsNullOrWhiteSpace())
                {
                    set.UseUsers = organize.GetAllUsersId(set.UseOrganizes);
                }
            }
            IO.Insert("roadflow_cache_homeset", all);
            return all;
        }

        private string GetCsharpMethodString(string method, params object[] param)
        {
            ValueTuple<object, Exception> tuple1 = Tools.ExecuteMethod(method, param);
            object obj2 = tuple1.Item1;
            if (obj2 != null)
            {
                return obj2.ToString();
            }
            return "";
        }

        public string GetDataSourceString(RoadFlow.Model.HomeSet item)
        {
            if (item != null)
            {
                switch (item.DataSourceType)
                {
                    case 0:
                        if (!item.DbConnId.HasValue)
                        {
                            return string.Empty;
                        }
                        return this.GetSqlString(item.DataSource, item.Type, item.DbConnId.Value);

                    case 1:
                        return this.GetCsharpMethodString(item.DataSource, Array.Empty<object>());

                    case 2:
                        return this.GetUrlString(item.DataSource);
                }
            }
            return string.Empty;
        }

        public List<RoadFlow.Model.HomeSet> GetListByUserId(Guid userId)
        {
            new Organize();
            return Enumerable.ToList<RoadFlow.Model.HomeSet>((IEnumerable<RoadFlow.Model.HomeSet>)Enumerable.OrderBy<RoadFlow.Model.HomeSet, int>((IEnumerable<RoadFlow.Model.HomeSet>)this.GetAll().FindAll(delegate (RoadFlow.Model.HomeSet p) {
                if (!p.UseOrganizes.IsNullOrWhiteSpace())
                {
                    return p.UseUsers.ContainsIgnoreCase(userId.ToString());
                }
                return true;
            }), key=>key.Sort));
        }

        public int GetMaxSort()
        {
            List<RoadFlow.Model.HomeSet> all = this.GetAll();
            if (all.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.HomeSet>((IEnumerable<RoadFlow.Model.HomeSet>)all, key=>key.Sort) + 5);
        }

        public DataTable GetPagerData(out int count, int size, int number, string name, string title, string type, string order)
        {
            return this.homeSetData.GetPagerData(out count, size, number, name, title, type, order);
        }

        private string GetSqlString(string sql, int type, Guid dbconnId)
        {
            if (type != 0)
            {
                if ((type - 1) <= 1)
                {
                    try
                    {
                        DataTable table = new DbConnection().GetDataTable(dbconnId, Wildcard.Filter(sql, null, null), (DbParameter[])null);
                        StringBuilder builder = new StringBuilder();
                        builder.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"hometable\"><thead><tr>");
                        foreach (DataColumn column in table.Columns)
                        {
                            builder.Append("<th>" + column.ColumnName + "</th>");
                        }
                        builder.Append("</tr></thead><tbody>");
                        foreach (DataRow row in table.Rows)
                        {
                            builder.Append("<tr>");
                            foreach (DataColumn column2 in table.Columns)
                            {
                                builder.Append("<td>" + row[column2.ColumnName].ToString() + "</td>");
                            }
                            builder.Append("</tr>");
                        }
                        builder.Append("</tbody></table>");
                        return builder.ToString();
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
            }
            else
            {
                return new DbConnection().GetFieldValue(dbconnId, Wildcard.Filter(sql, null, null));
            }
            return string.Empty;
        }

        public string GetUrlString(string url)
        {
            return HttpHelper.HttpGet(url, null, 0);
        }

        public int Update(RoadFlow.Model.HomeSet homeSet)
        {
            return this.homeSetData.Update(homeSet);
        }

      
}



 


   

}
