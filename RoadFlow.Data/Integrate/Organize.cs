using System;
using System.Collections.Generic;

namespace RoadFlow.Integrate
{
    /// <summary>
    /// 引用第三方组织架构
    /// </summary>
    public class Organize
    {
        
        /// <summary>
        /// 获取所有组织
        /// </summary>
        /// <returns></returns>
        public List<RoadFlow.Model.Organize> GetAllOrganize()
        {
            return new RoadFlow.Data.Organize().GetAll();
        }

        /// <summary>
        /// 获取所有组织用户
        /// </summary>
        /// <returns></returns>
        public List<RoadFlow.Model.OrganizeUser> GetAllOrganizeUser()
        {
            return new RoadFlow.Data.OrganizeUser().GetAll();
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public List<RoadFlow.Model.User> GetAllUser()
        {
            return new RoadFlow.Data.User().GetAll();
        }

        /// <summary>
        /// 获取所有工作用户
        /// </summary>
        /// <returns></returns>
        public List<RoadFlow.Model.WorkGroup> GetAllWorkGroup()
        {
            return new RoadFlow.Data.WorkGroup().GetAll();
        }
    }


}
