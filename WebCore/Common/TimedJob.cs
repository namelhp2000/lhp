using Pomelo.AspNetCore.TimedJob;
using RoadFlow.Business;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WebCore.Common
{
    public class TimedJob : Job
    {
        // Fields
        private readonly FlowTask flowTask = new FlowTask();

        // Methods
        [Invoke(IsEnabled = true, Begin = "2018-08-26 00:00", Interval = 0xea60, SkipWhileExecuting = true)]
        public void AutoSubmitFlowTask()
        {
            this.flowTask.AutoSubmitExpireTask();
        }


        [Invoke(IsEnabled = true, Begin = "2018-08-26 00:00", Interval = 0x2bf20, SkipWhileExecuting = true)]
        public void CheckOnlineUser()
        {
            if ((OnlineUser.OnlineUsers != null) && Enumerable.Any<KeyValuePair<string, RoadFlow.Model.OnlineUser>>((IEnumerable<KeyValuePair<string, RoadFlow.Model.OnlineUser>>)OnlineUser.OnlineUsers))
            {
                IEnumerable<RoadFlow.Model.OnlineUser> enumerable = Enumerable.Where<RoadFlow.Model.OnlineUser>((IEnumerable<RoadFlow.Model.OnlineUser>)OnlineUser.OnlineUsers.Values,s_c.s_9__4_0 ?? (s_c.s_9__4_0 = new Func<RoadFlow.Model.OnlineUser, bool>(s_c.s_9.CheckOnlineUserb__4_0)));
                List<ValueTuple<Guid, int>> list = new List<ValueTuple<Guid, int>>();
                foreach (RoadFlow.Model.OnlineUser user in enumerable)
                {
                    list.Add(new ValueTuple<Guid, int>(user.UserId, user.LoginType));
                }
                foreach (ValueTuple<Guid, int> local1 in list)
                {
                    Guid userId = local1.Item1;
                    int loginType = local1.Item2;
                    OnlineUser.Remove(userId, loginType);
                }
            }
        }

        [Invoke(IsEnabled = true, Begin = "2018-08-26 00:00", Interval = 0x2bf20, SkipWhileExecuting = true)]
        public void RemindTask()
        {
            this.flowTask.RemindTask(null);
        }

        // Nested Types
        [Serializable, CompilerGenerated]
        private sealed class s_c
    {
        // Fields
        public static readonly TimedJob.s_c  s_9 = new TimedJob.s_c();
        public static Func<RoadFlow.Model.OnlineUser, bool> s_9__4_0;

        // Methods
        internal bool CheckOnlineUserb__4_0(RoadFlow.Model.OnlineUser p)
        {
            return (p.LastTime.AddMinutes((Config.SessionTimeout <= 0) ? ((double)20) : ((double)Config.SessionTimeout)) < Current.DateTime);
        }
    }


}


}
