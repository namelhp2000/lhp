using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Data
{
    public class SystemField
    {
        // Methods
        public int Add(RoadFlow.Model.SystemField systemField)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.SystemField>(systemField);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.SystemField systemField)
        {
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.SystemField>(systemField);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.SystemField[] systemField)
        {
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.SystemField>(systemField);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.SystemField Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.SystemField>(objects);
            }
        }

        public List<RoadFlow.Model.SystemField> GetAll()
        {
            using (DataContext context = new DataContext())
            {
                return Enumerable.ToList<RoadFlow.Model.SystemField>((IEnumerable<RoadFlow.Model.SystemField>)Enumerable.OrderBy<RoadFlow.Model.SystemField, string>((IEnumerable<RoadFlow.Model.SystemField>)context.QueryAll<RoadFlow.Model.SystemField>(), key => key.FieldName));
            }
        }

        public int Update(RoadFlow.Model.SystemField systemField)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.SystemField>(systemField);
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.SystemField[] systemField)
        {
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.SystemField>(systemField);
                return context.SaveChanges();
            }
        }


    }
}
