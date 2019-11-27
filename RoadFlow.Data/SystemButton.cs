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
    public class SystemButton
    {
        // Methods
        public int Add(RoadFlow.Model.SystemButton systemButton)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.SystemButton>(systemButton);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.SystemButton systemButton)
        {
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.SystemButton>(systemButton);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.SystemButton[] systemButtons)
        {
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.SystemButton>(systemButtons);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.SystemButton Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.SystemButton>(objects);
            }
        }

        public List<RoadFlow.Model.SystemButton> GetAll()
        {
            using (DataContext context = new DataContext())
            {
                return Enumerable.ToList<RoadFlow.Model.SystemButton>((IEnumerable<RoadFlow.Model.SystemButton>)Enumerable.OrderBy<RoadFlow.Model.SystemButton, int>((IEnumerable<RoadFlow.Model.SystemButton>)context.QueryAll<RoadFlow.Model.SystemButton>(), key => key.Sort));
            }
        }

        public int Update(RoadFlow.Model.SystemButton systemButton)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.SystemButton>(systemButton);
                return context.SaveChanges();
            }
        }

        public int Update(RoadFlow.Model.SystemButton[] systemButtons)
        {
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.SystemButton>(systemButtons);
                return context.SaveChanges();
            }
        }
    }
}
