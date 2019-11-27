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
    public class AppLibraryButton
    {
        // Fields
        private const string CACHEKEY = "roadflow_cache_applibrarybutton";

        // Methods
        public int Add(RoadFlow.Model.AppLibraryButton appLibraryButton)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.AppLibraryButton>(appLibraryButton);
                return context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            IO.Remove("roadflow_cache_applibrarybutton");
        }

        public int Delete(RoadFlow.Model.AppLibraryButton appLibraryButton)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.AppLibraryButton>(appLibraryButton);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.AppLibraryButton[] appLibraryButtons)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.AppLibraryButton>(appLibraryButtons);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.AppLibraryButton> GetAll()
        {
            object obj2 = IO.Get("roadflow_cache_applibrarybutton");
            if (obj2 == null)
            {
                using (DataContext context = new DataContext())
                {
                    List<RoadFlow.Model.AppLibraryButton> list = context.QueryAll<RoadFlow.Model.AppLibraryButton>();
                    IO.Insert("roadflow_cache_applibrarybutton", list);
                    return list;
                }
            }
            return (List<RoadFlow.Model.AppLibraryButton>)obj2;
        }

        public int Update(RoadFlow.Model.AppLibraryButton appLibraryButton)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.AppLibraryButton>(appLibraryButton);
                return context.SaveChanges();
            }
        }

        public int Update(List<Tuple<RoadFlow.Model.AppLibraryButton, int>> tuples)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                foreach (Tuple<RoadFlow.Model.AppLibraryButton, int> tuple in tuples)
                {
                    
                    if (tuple.Item2 == 0)
                    {
                        context.Remove<RoadFlow.Model.AppLibraryButton>(tuple.Item1);
                    }
                    else if (1 == tuple.Item2)
                    {
                        context.Update<RoadFlow.Model.AppLibraryButton>(tuple.Item1);
                    }
                    else if (2 == tuple.Item2)
                    {
                        context.Add<RoadFlow.Model.AppLibraryButton>(tuple.Item1);
                    }
                }
                return context.SaveChanges();
            }
        }
    }


}
