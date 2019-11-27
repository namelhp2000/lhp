using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Business
{
    public class OrganizeUser
    {
        // Fields
        private readonly RoadFlow.Data.OrganizeUser organizeUserData = new RoadFlow.Data.OrganizeUser();

        // Methods
        public int Add(RoadFlow.Model.OrganizeUser organizeUser)
        {
            return this.organizeUserData.Add(organizeUser);
        }

        /// <summary>
        /// 根据id获取组织用户数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RoadFlow.Model.OrganizeUser Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.OrganizeUser p) {
                return p.Id == id;
            });
        }

        /// <summary>
        /// 获取所有组织用户
        /// </summary>
        /// <returns></returns>
        public List<RoadFlow.Model.OrganizeUser> GetAll()
        {
            return new RoadFlow.Integrate.Organize().GetAllOrganizeUser();
        }

        /// <summary>
        /// 通过组织id获取所有的组织id列表
        /// </summary>
        /// <param name="organizeId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.OrganizeUser> GetListByOrganizeId(Guid organizeId)
        {
            return Enumerable.ToList<RoadFlow.Model.OrganizeUser>((IEnumerable<RoadFlow.Model.OrganizeUser>)Enumerable.OrderBy<RoadFlow.Model.OrganizeUser, int>((IEnumerable<RoadFlow.Model.OrganizeUser>)this.GetAll().FindAll(delegate (RoadFlow.Model.OrganizeUser p) {
                return p.OrganizeId == organizeId;
            }), key=>key.Sort));
        }

        public List<RoadFlow.Model.OrganizeUser> GetListByUserId(Guid userId)
        {
            return Enumerable.ToList<RoadFlow.Model.OrganizeUser>((IEnumerable<RoadFlow.Model.OrganizeUser>)Enumerable.OrderByDescending<RoadFlow.Model.OrganizeUser, int>((IEnumerable<RoadFlow.Model.OrganizeUser>)this.GetAll().FindAll(delegate (RoadFlow.Model.OrganizeUser p) {
                return p.UserId == userId;
            }), key=>key.IsMain));
        }

        public RoadFlow.Model.OrganizeUser GetMainByUserId(Guid userId)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.OrganizeUser p) {
                return (p.UserId == userId) && (p.IsMain == 1);
            });
        }

        public int GetMaxSort(Guid organizeId)
        {
            List<RoadFlow.Model.OrganizeUser> listByOrganizeId = this.GetListByOrganizeId(organizeId);
            if (listByOrganizeId.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.OrganizeUser>((IEnumerable<RoadFlow.Model.OrganizeUser>)listByOrganizeId, key=>key.Sort) + 5);
        }

        public bool HasUser(Guid organizeId)
        {
            return this.GetAll().Exists(delegate (RoadFlow.Model.OrganizeUser p) {
                return p.OrganizeId == organizeId;
            });
        }

        public int Update(List<Tuple<RoadFlow.Model.OrganizeUser, int>> tuples)
        {
            return this.organizeUserData.Update(tuples);
        }

        public int Update(RoadFlow.Model.OrganizeUser[] organizeUsers)
        {
            return this.organizeUserData.Update(organizeUsers);
        }

      
}



 

}
