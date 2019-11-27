using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
   

    #region 新的方法2.8.3

    public class User
    {
        // Fields
        private readonly RoadFlow.Data.User userData = new RoadFlow.Data.User();

        // Methods
        public int Add(RoadFlow.Model.User user, RoadFlow.Model.OrganizeUser organizeUser)
        {
            new MenuUser().UpdateAllUseUserAsync();
            return this.userData.Add(user, organizeUser);
        }

        public bool CheckAccount(string account, Guid id)
        {
            RoadFlow.Model.User byAccount = this.GetByAccount(account);
            if (byAccount == null)
            {
                return false;
            }
            return (byAccount.Id != id);
        }

        public bool Contains(string organizeIds, Guid userId)
        {
            return new Organize().GetAllUsers(organizeIds).Exists(delegate (RoadFlow.Model.User p) {
                return p.Id == userId;
            });
        }

        public Bitmap CreateSignImage(string UserName)
        {
            int num;
            int num2;
            if (UserName.IsNullOrEmpty())
            {
                return null;
            }
            Random random = new Random(UserName.GetHashCode());
            Size empty = Size.Empty;
            Font font = new Font("隶书", 16f);
            using (Bitmap bitmap2 = new Bitmap(5, 5))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap2))
                {
                    SizeF ef = graphics.MeasureString(UserName, font, 0x2710);
                    empty.Width = ((int)ef.Width) + 4;
                    empty.Height = (int)ef.Height;
                }
            }
            Bitmap bitmap = new Bitmap(empty.Width, empty.Height);
            using (Graphics graphics2 = Graphics.FromImage(bitmap))
            {
                graphics2.Clear(Color.White);
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment= StringAlignment.Center;
                    format.LineAlignment= StringAlignment.Center;
                    format.FormatFlags = StringFormatFlags.NoWrap;// 0x1000;
                    graphics2.DrawString(UserName, font, Brushes.Red, new RectangleF(0f, 2f, (float)empty.Width, (float)empty.Height), format);
                }
            }
            Color color = Color.Red;
            int num3 = ((empty.Width * empty.Height) * 8) / 100;
            for (int i = 0; i < num3; i++)
            {
                num = random.Next(0, 4);
                num2 = random.Next(empty.Height);
                bitmap.SetPixel(num, num2, color);
                num = random.Next(empty.Width - 4, empty.Width);
                num2 = random.Next(empty.Height);
                bitmap.SetPixel(num, num2, color);
            }
            int num4 = ((empty.Width * empty.Height) * 20) / 100;
            for (int j = 0; j < num4; j++)
            {
                num = random.Next(empty.Width);
                num2 = random.Next(0, 4);
                bitmap.SetPixel(num, num2, color);
                num = random.Next(empty.Width);
                num2 = random.Next(empty.Height - 4, empty.Height);
                bitmap.SetPixel(num, num2, color);
            }
            int num5 = (empty.Width * empty.Height) / 150;
            for (int k = 0; k < num5; k++)
            {
                num = random.Next(empty.Width);
                num2 = random.Next(empty.Height);
                bitmap.SetPixel(num, num2, color);
            }
            font.Dispose();
            return bitmap;
        }

        public int Delete(Guid id)
        {
            List<RoadFlow.Model.OrganizeUser> listByUserId = new OrganizeUser().GetListByUserId(id);
            RoadFlow.Model.User user = this.Get(id);
            if (user == null)
            {
                return 0;
            }
            if (Config.Enterprise_WeiXin_IsUse)
            {
                new RoadFlow.Business.EnterpriseWeiXin.Organize().DeleteUser(user.Account);
            }
            Log.Add("删除了一个用户-" + user.Name, user.ToString(), LogType.系统管理, "", "", JsonConvert.SerializeObject(listByUserId), "", "", "", "", "");
            return this.userData.Delete(user, listByUserId.ToArray());
        }

        public bool Exists(string userId, string memeberIds)
        {
            return this.IsIn(userId, memeberIds);
        }

        /// <summary>
        /// 根据id获取用户数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RoadFlow.Model.User Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.User p) {
                return p.Id == id;
            });
        }

        public RoadFlow.Model.User Get(string id)
        {
            Guid guid3;
            Guid empty = Guid.Empty;
            Guid guid = Guid.Empty;
            if (id.IsGuid(out guid3))
            {
                empty = guid3;
            }
            else if (id.StartsWith("u_"))
            {
                empty = id.RemoveUserPrefix().ToGuid();
            }
            else if (id.StartsWith("r_"))
            {
                RoadFlow.Model.OrganizeUser user = new OrganizeUser().Get(id.RemoveUserRelationPrefix().ToGuid());
                if (user != null)
                {
                    empty = user.UserId;
                    guid = user.Id;
                }
            }
            if (empty.IsEmptyGuid())
            {
                return null;
            }
            RoadFlow.Model.User user2 = this.Get(empty);
            if ((user2 != null) && guid.IsNotEmptyGuid())
            {
                user2.PartTimeId = new Guid?(guid);
            }
            return user2;
        }

        /// <summary>
        /// 获取用户所有数据
        /// </summary>
        /// <returns></returns>
        public List<RoadFlow.Model.User> GetAll()
        {
            return new RoadFlow.Integrate.Organize().GetAllUser();
        }

     //   [return: TupleElementNames(new string[] { "leader", "chargeLeader" })]
        public ValueTuple<string, string> GetAllParentLeader(List<string> userIds)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            foreach (string str in userIds)
            {
                Guid organizeId = this.GetOrganizeId(str);
                if (organizeId.IsEmptyGuid())
                {
                    return new ValueTuple<string, string>(string.Empty, string.Empty);
                }
                foreach (RoadFlow.Model.Organize organize in new Organize().GetAllParents(organizeId, true))
                {
                    builder.Append(organize.Leader);
                    builder.Append(",");
                    builder2.Append(organize.ChargeLeader);
                    builder2.Append(",");
                }
            }
            char[] trimChars = new char[] { ',' };
            char[] chArray2 = new char[] { ',' };
            return new ValueTuple<string, string>(builder.ToString().TrimEnd(trimChars), builder2.ToString().TrimEnd(chArray2));
        }

      //  [return: TupleElementNames(new string[] { "leader", "chargeLeader" })]
        public ValueTuple<string, string> GetAllParentLeader(string userId)
        {
            Guid organizeId = this.GetOrganizeId(userId);
            if (organizeId.IsEmptyGuid())
            {
                return new ValueTuple<string, string>(string.Empty, string.Empty);
            }
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            foreach (RoadFlow.Model.Organize organize in new Organize().GetAllParents(organizeId, true))
            {
                builder.Append(organize.Leader);
                builder.Append(",");
                builder2.Append(organize.ChargeLeader);
                builder2.Append(",");
            }
            char[] trimChars = new char[] { ',' };
            char[] chArray2 = new char[] { ',' };
            return new ValueTuple<string, string>(builder.ToString().TrimEnd(trimChars), builder2.ToString().TrimEnd(chArray2));
        }

        public RoadFlow.Model.User GetByAccount(string account)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.User p) {
                return p.Account.EqualsIgnoreCase(account);
            });
        }

        public RoadFlow.Model.User GetByRelationId(string id)
        {
            if (!id.StartsWith("r_"))
            {
                return null;
            }
            RoadFlow.Model.OrganizeUser user = new OrganizeUser().Get(id.RemoveUserRelationPrefix().ToGuid());
            if (user == null)
            {
                return null;
            }
            return this.Get(user.UserId);
        }

        public RoadFlow.Model.Organize GetDept(string userId)
        {
            Guid organizeId = this.GetOrganizeId(userId);
            if (organizeId.IsEmptyGuid())
            {
                return null;
            }
            RoadFlow.Model.Organize organize = new Organize().Get(organizeId);
            if (organize.Type == 2)
            {
                return organize;
            }
            return new Organize().GetAllParents(organizeId, false).Find(key=>key.Type==2);
        }

        public string GetHeadImageSrc(RoadFlow.Model.User user, string wwwrootPath)
        {
            if (user == null)
            {
                return "";
            }
            string headImg = user.HeadImg;
            if (!headImg.IsNullOrWhiteSpace() && File.Exists(wwwrootPath + headImg))
            {
                return headImg;
            }
            return "/RoadFlowResources/Images/userHeads/default.jpg";
        }

        public string GetInitPassword(Guid id)
        {
            return this.GetMD5Password(id, Config.InitUserPassword);
        }

       // [return: TupleElementNames(new string[] { "leader", "chargeLeader" })]
        public ValueTuple<string, string> GetLeader(List<string> userIds)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            foreach (string str in userIds)
            {
                ValueTuple<string, string> leader = this.GetLeader(str);
                string str2 = leader.Item1;
                string str3 = leader.Item2;
                builder.Append(str2);
                builder.Append(",");
                builder2.Append(str3);
                builder2.Append(",");
            }
            char[] trimChars = new char[] { ',' };
            char[] chArray2 = new char[] { ',' };
            return new ValueTuple<string, string>(builder.ToString().TrimEnd(trimChars), builder2.ToString().TrimEnd(chArray2));
        }

        //[return: TupleElementNames(new string[] { "leader", "chargeLeader" })]
        public ValueTuple<string, string> GetLeader(string userId)
        {
            RoadFlow.Model.Organize organize = new Organize().Get(this.GetOrganizeId(userId));
            if (organize != null)
            {
                return new ValueTuple<string, string>(organize.Leader, organize.ChargeLeader);
            }
            return new ValueTuple<string, string>("", "");
        }

        public string GetMD5Password(Guid userId, string password)
        {
            return (userId.ToString().ToUpper() + password).MD5();
        }

        public string GetMobile(Guid userId)
        {
            RoadFlow.Model.User user = this.Get(userId);
            if (user != null)
            {
                return user.Mobile.Trim1();
            }
            return "";
        }

        public string GetName(Guid id)
        {
            RoadFlow.Model.User user = this.Get(id);
            if (user != null)
            {
                return user.Name;
            }
            return string.Empty;
        }

        public string GetNames(string ids)
        {
            StringBuilder builder = new StringBuilder();
            char[] separator = new char[] { ',' };
            foreach (string str in ids.Split(separator))
            {
                Guid guid;
                string str2 = str;
                if (str.StartsWith("u_"))
                {
                    str2 = str.RemoveUserPrefix();
                }
                else if (str.StartsWith("r_"))
                {
                    RoadFlow.Model.OrganizeUser user2 = new OrganizeUser().Get(str.RemoveUserRelationPrefix().ToGuid());
                    if (user2 != null)
                    {
                        str2 = user2.UserId.ToString();
                    }
                }
                if (str2.IsGuid(out guid))
                {
                    RoadFlow.Model.User user = this.Get(guid);
                    if (user != null)
                    {
                        builder.Append(user.Name);
                        builder.Append("、");
                    }
                }
            }
            char[] trimChars = new char[] { '、' };
            return builder.ToString().TrimEnd(trimChars);
        }

        public Guid GetOrganizeId(string userId)
        {
            Guid guid2;
            Guid empty = Guid.Empty;
            if (userId.IsGuid(out guid2))
            {
                RoadFlow.Model.OrganizeUser mainByUserId = new OrganizeUser().GetMainByUserId(guid2);
                if (mainByUserId != null)
                {
                    empty = mainByUserId.OrganizeId;
                }
                return empty;
            }
            if (userId.StartsWith("u_"))
            {
                RoadFlow.Model.OrganizeUser user2 = new OrganizeUser().GetMainByUserId(userId.RemoveUserPrefix().ToGuid());
                if (user2 != null)
                {
                    empty = user2.OrganizeId;
                }
                return empty;
            }
            if (userId.StartsWith("r_"))
            {
                RoadFlow.Model.OrganizeUser user3 = new OrganizeUser().Get(userId.RemoveUserRelationPrefix().ToGuid());
                if (user3 != null)
                {
                    empty = user3.OrganizeId;
                }
            }
            return empty;
        }

        public string GetOrganizeMainShowHtml(Guid id, bool isShowRoot = true)
        {
            RoadFlow.Model.OrganizeUser mainByUserId = new OrganizeUser().GetMainByUserId(id);
            Organize organize = new Organize();
            if (mainByUserId != null)
            {
                return (organize.GetParentsName(mainByUserId.OrganizeId, isShowRoot) + @" \ " + organize.GetName(mainByUserId.OrganizeId));
            }
            return "";
        }

        public string GetOrganizesShowHtml(Guid id, bool isShowRoot = true)
        {
            StringBuilder builder = new StringBuilder();
            Organize organize = new Organize();
            foreach (RoadFlow.Model.OrganizeUser user in new OrganizeUser().GetListByUserId(id))
            {
                builder.Append("<div>" + organize.GetParentsName(user.OrganizeId, isShowRoot));
                builder.Append(@" \ ");
                builder.Append(organize.GetName(user.OrganizeId));
                if (user.IsMain == 0)
                {
                    builder.Append("<span style='color:#999;'>[兼任]</span>");
                }
                builder.Append("</div>");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取组织显示的Html
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isShowRoot"></param>
        /// <returns></returns>
        public string GetOrganizesShowHtml1(Guid id, bool isShowRoot = true)
        {
            StringBuilder builder = new StringBuilder();
            Organize organize = new Organize();
            foreach (RoadFlow.Model.OrganizeUser user in new OrganizeUser().GetListByUserId(id))
            {
                builder.Append("<span>(" + organize.GetParentsName(user.OrganizeId, isShowRoot));
                builder.Append(@" \ ");
                builder.Append(organize.GetName(user.OrganizeId));
                if (user.IsMain == 0)
                {
                    builder.Append("<span style='color:#999;'>[兼任]</span>");
                }
                builder.Append(")</span>");
            }
            return builder.ToString();
        }


        public RoadFlow.Model. User GetByWeiXinOpenId(string openId)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.User p) {
                return !p.WeiXinOpenId.IsNullOrWhiteSpace() && p.WeiXinOpenId.Equals(openId);
            });
        }









        public List<RoadFlow.Model.User> GetOrganizeUsers(List<string> userIds)
        {
            List<RoadFlow.Model.User> list = new List<RoadFlow.Model.User>();
            Organize organize = new Organize();
            foreach (string str in userIds)
            {
                Guid organizeId = this.GetOrganizeId(str);
                list.AddRange((IEnumerable<RoadFlow.Model.User>)organize.GetAllUsers(organizeId, true));
            }
            return list;
        }

        public List<RoadFlow.Model.User> GetOrganizeUsers(string userId)
        {
            Guid organizeId = this.GetOrganizeId(userId);
            return new Organize().GetAllUsers(organizeId, true);
        }

        //[return: TupleElementNames(new string[] { "leader", "chargeLeader" })]
        public ValueTuple<string, string> GetParentLeader(List<string> userIds)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            foreach (string str in userIds)
            {
                ValueTuple<string, string> parentLeader = this.GetParentLeader(str);
                string str2 = parentLeader.Item1;
                string str3 = parentLeader.Item2;
                builder.Append(str2);
                builder.Append(",");
                builder2.Append(str3);
                builder2.Append(",");
            }
            char[] trimChars = new char[] { ',' };
            char[] chArray2 = new char[] { ',' };
            return new ValueTuple<string, string>(builder.ToString().TrimEnd(trimChars), builder2.ToString().TrimEnd(chArray2));
        }

      //  [return: TupleElementNames(new string[] { "leader", "chargeLeader" })]
        public ValueTuple<string, string> GetParentLeader(string userId)
        {
            RoadFlow.Model.Organize parentOrganize = this.GetParentOrganize(userId);
            if (parentOrganize != null)
            {
                return new ValueTuple<string, string>(parentOrganize.Leader, parentOrganize.ChargeLeader);
            }
            return new ValueTuple<string, string>(string.Empty, string.Empty);
        }

        public RoadFlow.Model.Organize GetParentOrganize(string userId)
        {
            Guid organizeId = this.GetOrganizeId(userId);
            return new Organize().GetParent(organizeId);
        }

        public string GetSignSrc(string userId = "")
        {
            return ("/RoadFlowResources/images/userSigns/" + (userId.IsNullOrWhiteSpace() ? CurrentUserId.ToUpperString() : userId.ToUpper()) + "/default.png");
        }

        public RoadFlow.Model.Organize GetStation(string userId)
        {
            Guid organizeId = this.GetOrganizeId(userId);
            if (organizeId.IsEmptyGuid())
            {
                return null;
            }
            RoadFlow.Model.Organize organize = new Organize().Get(organizeId);
            if (organize.Type == 3)
            {
                return organize;
            }
            return new Organize().GetAllParents(organizeId, false).Find(key=>key.Type==3);
        }

        public RoadFlow.Model.Organize GetUnit(string userId)
        {
            Guid organizeId = this.GetOrganizeId(userId);
            if (organizeId.IsEmptyGuid())
            {
                return null;
            }
            RoadFlow.Model.Organize organize = new Organize().Get(organizeId);
            if (organize.Type == 1)
            {
                return organize;
            }
            return new Organize().GetAllParents(organizeId, false).Find(key=>key.Type==1);
        }

        public Guid GetUserId(string userId)
        {
            if (!userId.IsNullOrWhiteSpace())
            {
                Guid guid;
                if (userId.StartsWith("u_"))
                {
                    return userId.RemoveUserPrefix().ToGuid();
                }
                if (userId.IsGuid(out guid))
                {
                    return guid;
                }
                if (userId.StartsWith("r_"))
                {
                    Guid id = userId.RemoveUserRelationPrefix().ToGuid();
                    RoadFlow.Model.OrganizeUser user = new OrganizeUser().Get(id);
                    if (user != null)
                    {
                        return user.UserId;
                    }
                }
            }
            return Guid.Empty;
        }

        public string GetUserIds(List<RoadFlow.Model.User> users)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.User user in users)
            {
                builder.Append("u_");
                builder.Append(user.Id);
                builder.Append(",");
            }
            char[] trimChars = new char[] { ',' };
            return builder.ToString().TrimEnd(trimChars);
        }

        public List<RoadFlow.Model.WorkGroup> GetWorkGroups(Guid userId)
        {
            Predicate<RoadFlow.Model.User> s_9__0=null;
            List<RoadFlow.Model.WorkGroup> list = new List<RoadFlow.Model.WorkGroup>();
            if (!userId.IsEmptyGuid())
            {
                WorkGroup group = new WorkGroup();
                foreach (RoadFlow.Model.WorkGroup group2 in group.GetAll())
                {
                    if (group.GetAllUsers(group2.Id).Exists(s_9__0 ?? (s_9__0 = delegate (RoadFlow.Model.User p) {
                        return p.Id == userId;
                    })))
                    {
                        list.Add(group2);
                    }
                }
            }
            return list;
        }

        public string GetWorkGroupsId(Guid userId)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.WorkGroup group in this.GetWorkGroups(userId))
            {
                builder.Append(group.Id);
                builder.Append(",");
            }
            char[] trimChars = new char[] { ',' };
            return builder.ToString().TrimEnd(trimChars);
        }

        public string GetWorkGroupsName(Guid userId)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.WorkGroup group in this.GetWorkGroups(userId))
            {
                builder.Append(group.Name);
                builder.Append("、");
            }
            char[] trimChars = new char[] { '、' };
            return builder.ToString().TrimEnd(trimChars);
        }

        public bool InitUserPassword(Guid id)
        {
            RoadFlow.Model.User user = this.Get(id);
            if (user == null)
            {
                return false;
            }
            user.Password = this.GetInitPassword(id);
            this.Update(user);
            return true;
        }

        public bool IsFrozen(RoadFlow.Model.User user)
        {
            if (user.Status == 1)
            {
                return true;
            }
            Organize organize = new Organize();
            foreach (RoadFlow.Model.OrganizeUser user2 in new OrganizeUser().GetListByUserId(user.Id))
            {
                RoadFlow.Model.Organize organize2 = organize.Get(user2.OrganizeId);
                if ((organize2 != null) && (organize2.Status == 1))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsIn(string userId, string memberIds)
        {
            Guid guid;
            List<RoadFlow.Model.User> allUsers = new Organize().GetAllUsers(memberIds);
            Guid userGuid = Guid.Empty;
            if (userId.IsGuid(out guid))
            {
                userGuid = guid;
            }
            else if (userId.StartsWith("u_"))
            {
                userGuid = userId.RemoveUserPrefix().ToGuid();
            }
            else if (userId.StartsWith("r_"))
            {
                userGuid = userId.RemoveUserRelationPrefix().ToGuid();
            }
            if (userGuid.IsEmptyGuid())
            {
                return false;
            }
            return allUsers.Exists(delegate (RoadFlow.Model.User p) {
                return p.Id == userGuid;
            });
        }

        public int Update(RoadFlow.Model.User user)
        {
            return this.userData.Update(user);
        }

        // Properties
        public static RoadFlow.Model.User CurrentUser
        {
            get
            {
                Guid currentUserId = CurrentUserId;
                if (!currentUserId.IsEmptyGuid())
                {
                    return new User().Get(currentUserId);
                }
                return null;
            }
        }

        public static Guid CurrentUserId
        {
            get
            {
                HttpContext httpContext = Tools.HttpContext;
                if (httpContext != null)
                {
                    Guid guid;
                    Guid guid2;
                    if (SessionExtensions.GetString(httpContext.Session, Config.UserIdSessionKey).IsGuid(out guid))
                    {
                        return guid;
                    }
                    if (Config.IsDebug && Config.DebugUserId.IsGuid(out guid2))
                    {
                        return guid2;
                    }
                }
                return Guid.Empty;
            }
        }

        public static string CurrentUserName
        {
            get
            {
                RoadFlow.Model.User currentUser = CurrentUser;
                if (CurrentUser != null)
                {
                    return CurrentUser.Name;
                }
                return string.Empty;
            }
        }

      
}



 


    #endregion

}
