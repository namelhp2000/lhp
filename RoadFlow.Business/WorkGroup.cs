using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Business
{
    public class WorkGroup
    {
        // Fields
        private readonly RoadFlow.Data.WorkGroup workGroupData = new RoadFlow.Data.WorkGroup();

        // Methods
        public int Add(RoadFlow.Model.WorkGroup workGroup)
        {
            new MenuUser().UpdateAllUseUserAsync();

            return this.workGroupData.Add(workGroup);
        }

        public int Delete(RoadFlow.Model.WorkGroup workGroup)
        {
            new MenuUser().UpdateAllUseUserAsync();
            return this.workGroupData.Delete(workGroup);
        }

        /// <summary>
        /// 根据id获取工作组数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RoadFlow.Model.WorkGroup Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.WorkGroup p)
            {
                return p.Id == id;
            });
        }

        /// <summary>
        /// 获取工作组所有数据
        /// </summary>
        /// <returns></returns>
        public List<RoadFlow.Model.WorkGroup> GetAll()
        {
            return Enumerable.ToList<RoadFlow.Model.WorkGroup>((IEnumerable<RoadFlow.Model.WorkGroup>)Enumerable.OrderBy<RoadFlow.Model.WorkGroup, int>((IEnumerable<RoadFlow.Model.WorkGroup>)new RoadFlow.Integrate.Organize().GetAllWorkGroup(), key => key.Sort));
        }

        public List<RoadFlow.Model.User> GetAllUsers(Guid id)
        {
            RoadFlow.Model.WorkGroup group = this.Get(id);
            if ((group != null) && !group.Members.IsNullOrWhiteSpace())
            {
                return new Organize().GetAllUsers(group.Members);
            }
            return new List<RoadFlow.Model.User>();
        }

        public int GetMaxSort()
        {
            List<RoadFlow.Model.WorkGroup> all = this.GetAll();
            if (all.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.WorkGroup>((IEnumerable<RoadFlow.Model.WorkGroup>)all, key => key.Sort) + 5);
        }

        public string GetName(Guid id)
        {
            RoadFlow.Model.WorkGroup group = this.Get(id);
            if (group != null)
            {
                return group.Name;
            }
            return string.Empty;
        }

        public int Update(RoadFlow.Model.WorkGroup workGroup)
        {
            new MenuUser().UpdateAllUseUserAsync();

            return this.workGroupData.Update(workGroup);
        }

        public int Update(RoadFlow.Model.WorkGroup[] workGroups)
        {
            new MenuUser().UpdateAllUseUserAsync();
            return this.workGroupData.Update(workGroups);
        }


    }

}
