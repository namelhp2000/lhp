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



   

    public class OnlineUser
    {
        // Fields
        public static Dictionary<string, RoadFlow.Model.OnlineUser> OnlineUsers;

        // Methods
        public static void Add(RoadFlow.Model.OnlineUser onlineUser)
        {
            string str = onlineUser.UserId.ToUpperString() + "_" + ((int)onlineUser.LoginType).ToString();
            if (OnlineUsers == null)
            {
                Dictionary<string, RoadFlow.Model.OnlineUser> dictionary1 = new Dictionary<string, RoadFlow.Model.OnlineUser>();
                dictionary1.Add(str, onlineUser);
                OnlineUsers = dictionary1;
            }
            else if (OnlineUsers.ContainsKey(str))
            {
                OnlineUsers[str]=onlineUser;
            }
            else
            {
                OnlineUsers.Add(str, onlineUser);
            }
        }

        public static int Count()
        {
            if (OnlineUsers == null)
            {
                return 0;
            }
            return Enumerable.Count<RoadFlow.Model.OnlineUser>((IEnumerable<RoadFlow.Model.OnlineUser>)OnlineUsers.Values,  key=> key.LoginType == 0);
        }

        public static RoadFlow.Model.OnlineUser Get(Guid userId, int loginType = 0)
        {
            string str = userId.ToUpperString() + "_" + ((int)loginType).ToString();
            if ((OnlineUsers != null) && OnlineUsers.ContainsKey(str))
            {
                return OnlineUsers[str];
            }
            return null;
        }

        public static List<RoadFlow.Model.OnlineUser> GetAll()
        {
            if (OnlineUsers != null)
            {
                return Enumerable.ToList<RoadFlow.Model.OnlineUser>((IEnumerable<RoadFlow.Model.OnlineUser>)OnlineUsers.Values);
            }
            return new List<RoadFlow.Model.OnlineUser>();
        }

        public static void Remove(Guid userId, int loginType = -1)
        {
            if (OnlineUsers != null)
            {
                if (loginType == -1)
                {
                    OnlineUsers.Remove(userId.ToUpperString() + "_0");
                    OnlineUsers.Remove(userId.ToUpperString() + "_1");
                }
                else
                {
                    string str = userId.ToUpperString() + "_" + ((int)loginType).ToString();
                    OnlineUsers.Remove(str);
                }
            }
        }

        public static void UpdateLast(Guid userId, int loginType = 0, string url = "")
        {
            if (OnlineUsers != null)
            {
                string str = userId.ToUpperString() + "_" + ((int)loginType).ToString();
                if (OnlineUsers.ContainsKey(str))
                {
                    OnlineUsers[str].LastTime = DateTimeExtensions.Now;
                    OnlineUsers[str].LastUrl = url.IsNullOrWhiteSpace() ? Tools.HttpContext.Request.Url() : url;
                }
            }
        }

       
}



  



}
