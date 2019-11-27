using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
    public class Organize
    {
        // Fields
        private readonly RoadFlow.Data.Organize organizeData = new RoadFlow.Data.Organize();
        public const string PREFIX_RELATION = "r_";
        public const string PREFIX_USER = "u_";
        public const string PREFIX_WORKGROUP = "w_";

        // Methods
        public int Add(RoadFlow.Model.Organize organize)
        {
            return this.organizeData.Add(organize);
        }

        private void AddChilds(RoadFlow.Model.Organize organize, List<RoadFlow.Model.Organize> organizes, List<RoadFlow.Model.Organize> all)
        {
            if (organize != null)
            {
                foreach (RoadFlow.Model.Organize organize2 in Enumerable.ToList<RoadFlow.Model.Organize>((IEnumerable<RoadFlow.Model.Organize>)Enumerable.OrderBy<RoadFlow.Model.Organize, int>((IEnumerable<RoadFlow.Model.Organize>)all.FindAll(delegate (RoadFlow.Model.Organize p) {
                    return p.ParentId == organize.Id;
                }), key=>key.Sort)))
                {
                    organizes.Add(organize2);
                    this.AddChilds(organize2, organizes, all);
                }
            }
        }

        private void AddParent(RoadFlow.Model.Organize organize, List<RoadFlow.Model.Organize> organizes, List<RoadFlow.Model.Organize> all)
        {
            if (organize != null)
            {
                RoadFlow.Model.Organize organize2 = all.Find(delegate (RoadFlow.Model.Organize p) {
                    return p.Id == organize.ParentId;
                });
                if (organize2 != null)
                {
                    organizes.Add(organize2);
                    this.AddParent(organize2, organizes, all);
                }
            }
        }

        public int Delete(Guid id)
        {
            RoadFlow.Model.Organize organize = this.Get(id);
            if (organize == null)
            {
                return 0;
            }
            if (id == this.GetRootId())
            {
                Log.Add("删除组织架构根节点失败", "", LogType.系统管理, "", "", "", "", "", "", "", "");
                return 0;
            }
            List<RoadFlow.Model.Organize> allChilds = this.GetAllChilds(id, false);
            allChilds.Add(organize);
            List<RoadFlow.Model.User> list2 = new List<RoadFlow.Model.User>();
            List<RoadFlow.Model.OrganizeUser> list3 = new List<RoadFlow.Model.OrganizeUser>();
            OrganizeUser user = new OrganizeUser();
            foreach (RoadFlow.Model.Organize organize2 in allChilds)
            {
                list2.AddRange((IEnumerable<RoadFlow.Model.User>)this.GetAllUsers(organize2.Id, false));
                list3.AddRange((IEnumerable<RoadFlow.Model.OrganizeUser>)user.GetListByOrganizeId(organize2.Id));
            }
            int num = this.organizeData.Delete(allChilds.ToArray(), list2.ToArray(), list3.ToArray());
            if (Config.Enterprise_WeiXin_IsUse)
            {
                RoadFlow.Business.EnterpriseWeiXin.Organize organize3 = new RoadFlow.Business.EnterpriseWeiXin.Organize();
                foreach (RoadFlow.Model.User user2 in list2)
                {
                    organize3.DeleteUser(user2.Account);
                }
            }
            string[] textArray1 = new string[] { "删除了组织架构及其下级和人员-", organize.Name, "-共", ((int)num).ToString(), "条数据" };
            string[] textArray2 = new string[] { "组织：", JsonConvert.SerializeObject(allChilds), "人员：", JsonConvert.SerializeObject(list2), "人员与架构关系：", JsonConvert.SerializeObject(list3) };
            Log.Add(string.Concat((string[])textArray1), string.Concat((string[])textArray2), LogType.系统管理, "", "", "", "", "", "", "", "");
            return num;
        }
        /// <summary>
        /// 根据id获取对应组织
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RoadFlow.Model.Organize Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.Organize p) {
                return p.Id == id;
            });
        }

        /// <summary>
        /// 获取组织所有数据
        /// </summary>
        /// <returns></returns>
        public List<RoadFlow.Model.Organize> GetAll()
        {
            return new RoadFlow.Integrate.Organize().GetAllOrganize();
        }

        public List<RoadFlow.Model.Organize> GetAllChilds(Guid id, bool isMe = false)
        {
            List<RoadFlow.Model.Organize> organizes = new List<RoadFlow.Model.Organize>();
            RoadFlow.Model.Organize organize = this.Get(id);
            if (organize != null)
            {
                if (isMe)
                {
                    organizes.Add(organize);
                }
                List<RoadFlow.Model.Organize> all = this.GetAll();
                this.AddChilds(organize, organizes, all);
            }
            return organizes;
        }

        public List<RoadFlow.Model.Organize> GetAllParents(Guid id, bool isMe = true)
        {
            List<RoadFlow.Model.Organize> organizes = new List<RoadFlow.Model.Organize>();
            RoadFlow.Model.Organize organize = this.Get(id);
            if (organize != null)
            {
                if (isMe)
                {
                    organizes.Add(organize);
                }
                List<RoadFlow.Model.Organize> all = this.GetAll();
                this.AddParent(organize, organizes, all);
            }
            return organizes;
        }

        public List<RoadFlow.Model.User> GetAllUsers(string idString)
        {
            List<RoadFlow.Model.User> list = new List<RoadFlow.Model.User>();
            if (idString.IsNullOrWhiteSpace())
            {
                return list;
            }
            User user = new User();
            OrganizeUser user2 = new OrganizeUser();
            WorkGroup group = new WorkGroup();
            char[] separator = new char[] { ',' };
            foreach (string str in idString.Split(separator))
            {
                Guid guid;
                if (str.IsGuid(out guid))
                {
                    list.AddRange((IEnumerable<RoadFlow.Model.User>)this.GetAllUsers(guid, true));
                }
                else if (str.StartsWith("u_"))
                {
                   RoadFlow.Model. User user3 = user.Get(str.RemoveUserPrefix().ToGuid());
                    if (user3 != null)
                    {
                        list.Add(user3);
                    }

                    //    list.Add(user.Get(str.RemoveUserPrefix().ToGuid()));
                }
                else if (str.StartsWith("r_"))
                {
                    RoadFlow.Model.OrganizeUser user4 = user2.Get(str.RemoveUserRelationPrefix().ToGuid());
                    if (user4 != null)
                    {
                        RoadFlow.Model.User user5 = user.Get(user4.UserId);
                        if (user5 != null)
                        {
                            user5.PartTimeId = new Guid?(user4.Id);
                            list.Add(user5);
                        }

                        //    user4.PartTimeId = new Guid?(user3.Id);
                        //   list.Add(user4);
                    }
                }
                else if (str.StartsWith("w_"))
                {
                    list.AddRange((IEnumerable<RoadFlow.Model.User>)group.GetAllUsers(str.RemoveWorkGroupPrefix().ToGuid()));
                }
            }
            return Enumerable.ToList<RoadFlow.Model.User>(Enumerable.Distinct<RoadFlow.Model.User>((IEnumerable<RoadFlow.Model.User>)list, new RoadFlow.Model.User()));
        }

        public List<RoadFlow.Model.User> GetAllUsers(Guid id, bool hasPartTime = true)
        {
            List<RoadFlow.Model.User> list = new List<RoadFlow.Model.User>();
            OrganizeUser user = new OrganizeUser();
            User user2 = new User();
            foreach (RoadFlow.Model.Organize organize in this.GetAllChilds(id, true))
            {
                foreach (RoadFlow.Model.OrganizeUser user3 in user.GetListByOrganizeId(organize.Id))
                {
                    if (hasPartTime || (user3.IsMain == 1))
                    {
                        RoadFlow.Model.User user4 = user2.Get(user3.UserId);
                        if (user3.IsMain == 0)
                        {
                            user4.PartTimeId = new Guid?(user3.Id);
                        }
                        if (user4 != null)
                        {
                            list.Add(user4);
                        }
                    }
                }
            }
            return Enumerable.ToList<RoadFlow.Model.User>(Enumerable.Distinct<RoadFlow.Model.User>((IEnumerable<RoadFlow.Model.User>)list, new RoadFlow.Model.User()));
        }

        public string GetAllUsersId(string idString)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.User user in this.GetAllUsers(idString))
            {
                
                    builder.Append(user.Id);
                    builder.Append(",");
                
               
            }
            char[] trimChars = new char[] { ',' };
            return builder.ToString().TrimEnd(trimChars);
        }

        /// <summary>
        /// 根据id获取对应所有的子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.Organize> GetChilds(Guid id)
        {
            return Enumerable.ToList<RoadFlow.Model.Organize>((IEnumerable<RoadFlow.Model.Organize>)Enumerable.OrderBy<RoadFlow.Model.Organize, int>((IEnumerable<RoadFlow.Model.Organize>)this.GetAll().FindAll(delegate (RoadFlow.Model.Organize p) {
                return p.ParentId == id;
            }), key=>key.Sort));
        }

        public int GetMaxSort(Guid id)
        {
            List<RoadFlow.Model.Organize> childs = this.GetChilds(id);
            if (childs.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.Organize>((IEnumerable<RoadFlow.Model.Organize>)childs,key=>key.Sort) + 5);
        }

        public string GetName(Guid id)
        {
            RoadFlow.Model.Organize organize = this.Get(id);
            if (organize != null)
            {
                return organize.Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// 通过组织字符串获取组织名称
        /// </summary>
        /// <param name="idString"></param>
        /// <returns></returns>
        public string GetNames(string idString)
        {
            if (idString.IsNullOrWhiteSpace())
            {
                return "";
            }
            User user = new User();
            OrganizeUser user2 = new OrganizeUser();
            StringBuilder builder = new StringBuilder();
            WorkGroup group = new WorkGroup();
            char[] separator = new char[] { ',' };
            foreach (string str in idString.Split(separator))
            {
                Guid guid;
                if (str.IsGuid(out guid))
                {
                    builder.Append(this.GetName(guid));
                    builder.Append("、");
                }
                else if (str.StartsWith("u_"))
                {
                    builder.Append(user.GetName(str.RemoveUserPrefix().ToGuid()));
                    builder.Append("、");
                }
                else if (str.StartsWith("r_"))
                {
                    RoadFlow.Model.OrganizeUser user3 = user2.Get(str.RemoveUserRelationPrefix().ToGuid());
                    if (user3 != null)
                    {
                        builder.Append(user.GetName(user3.UserId));
                        builder.Append("、");
                    }
                }
                else if (str.StartsWith("w_"))
                {
                    builder.Append(group.GetName(str.RemoveWorkGroupPrefix().ToGuid()));
                    builder.Append("、");
                }
            }
            char[] trimChars = new char[] { '、' };
            return builder.ToString().TrimEnd(trimChars);
        }

        public string GetOrganizeAttrString(string json)
        {
            JObject obj2 = null;
            try
            {
                obj2 = JObject.Parse(json);
            }
            catch
            {
            }
            if (obj2 == null)
            {
                return string.Empty;
            }
            string str = obj2.Value<string>("unit");
            string str2 = obj2.Value<string>("dept");
            string str3 = obj2.Value<string>("station");
            string str4 = obj2.Value<string>("user");
            string str5 = obj2.Value<string>("more");
            string str6 = obj2.Value<string>("group");
            string str7 = obj2.Value<string>("role");
            string str8 = obj2.Value<string>("rootid");
            StringBuilder builder1 = new StringBuilder();
            builder1.Append(" unit=\"" + (str.IsNullOrWhiteSpace() ? "0" : str) + "\"");
            builder1.Append(" dept=\"" + (str2.IsNullOrWhiteSpace() ? "0" : str2) + "\"");
            builder1.Append(" station=\"" + (str3.IsNullOrWhiteSpace() ? "0" : str3) + "\"");
            builder1.Append(" user=\"" + (str4.IsNullOrWhiteSpace() ? "0" : str4) + "\"");
            builder1.Append(" more=\"" + (str5.IsNullOrWhiteSpace() ? "0" : str5) + "\"");
            builder1.Append(" group=\"" + (str6.IsNullOrWhiteSpace() ? "0" : str6) + "\"");
            builder1.Append(" role=\"" + (str7.IsNullOrWhiteSpace() ? "0" : str7) + "\"");
            builder1.Append(" rootid=\"" + (str8.IsNullOrWhiteSpace() ? "0" : str8) + "\"");
            return builder1.ToString();
        }

        public RoadFlow.Model.Organize GetParent(Guid id)
        {
            RoadFlow.Model.Organize organize = this.Get(id);
            if (organize != null)
            {
                return this.Get(organize.ParentId);
            }
            return null;
        }

        public string GetParentsName(Guid id, bool isRoot = true)
        {
            List<RoadFlow.Model.Organize> allParents = this.GetAllParents(id, false);
            allParents.Reverse();
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.Organize organize in allParents)
            {
                if (isRoot || !organize.ParentId.IsEmptyGuid())
                {
                    builder.Append(organize.Name);
                    builder.Append(@" \ ");
                }
            }
            return builder.ToString().TrimEnd(new char[] { ' ', '\\', ' ' });
        }

        public RoadFlow.Model.Organize GetRoot()
        {
            return this.GetAll().Find(key=>key.ParentId==Guid.Empty);
        }

        /// <summary>
        /// 获取组织根
        /// </summary>
        /// <returns></returns>
        public Guid GetRootId()
        {
            RoadFlow.Model.Organize root = this.GetRoot();
            if (root != null)
            {
                return root.Id;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 在组织中根据id获取对应的用户数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hasPartTime"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.User> GetUsers(Guid id, bool hasPartTime = true)
        {
            List<RoadFlow.Model.User> list = new List<RoadFlow.Model.User>();
            User user = new User();
            foreach (RoadFlow.Model.OrganizeUser user2 in new OrganizeUser().GetListByOrganizeId(id))
            {
                if (hasPartTime || (user2.IsMain == 1))
                {
                    RoadFlow.Model.User user3 = user.Get(user2.UserId);
                    if (user2.IsMain == 0)
                    {
                        user3.PartTimeId = new Guid?(user2.Id);
                    }
                    if (user3 != null)
                    {
                        list.Add(user3);
                    }
                }
            }
            return Enumerable.ToList<RoadFlow.Model.User>(Enumerable.Distinct<RoadFlow.Model.User>((IEnumerable<RoadFlow.Model.User>)list, new RoadFlow.Model.User()));
        }

        /// <summary>
        /// 判断id是否有子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasChilds(Guid id)
        {
            return this.GetAll().Exists(delegate (RoadFlow.Model.Organize p) {
                return p.ParentId == id;
            });
        }

        public bool HasUsers(Guid id)
        {
            return new OrganizeUser().HasUser(id);
        }

        public int Update(RoadFlow.Model.Organize organize)
        {
            new MenuUser().UpdateAllUseUserAsync();

            return this.organizeData.Update(organize);
        }

        public int Update(RoadFlow.Model.Organize[] organizes)
        {
            new MenuUser().UpdateAllUseUserAsync();

            return this.organizeData.Update(organizes);
        }

       
}



 

}
