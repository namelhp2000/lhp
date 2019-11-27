using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business.EnterpriseWeiXin;
using RoadFlow.Business.SignalR;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RoadFlow.Business
{
    public class Message
    {
        // Fields
        private RoadFlow.Data.Message messageData = new RoadFlow.Data.Message();

        // Methods
        public int Add(RoadFlow.Model.Message message)
        {
            return this.messageData.Add(message);
        }

        public int Add(RoadFlow.Model.Message messages, RoadFlow.Model.MessageUser[] messageUsers)
        {
            return this.messageData.Add(messages, messageUsers);
        }

        public RoadFlow.Model.Message Get(Guid id)
        {
            return this.messageData.Get(id);
        }

      //  [return: TupleElementNames(new string[] { null, "count" })]
        public ValueTuple<RoadFlow.Model.Message, int> GetNoRead(Guid userId)
        {
            return this.messageData.GetNoRead(userId);
        }

        public DataTable GetSendList(out int count, int size, int number, string userId, string contents, string date1, string date2, string status, string order)
        {
            return this.messageData.GetSendList(out count, size, number, userId, contents, date1, date2, status, order);
        }

        public string GetSendTypeString(string sendType)
        {
            if (sendType.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            char[] separator = new char[] { ',' };
            StringBuilder builder = new StringBuilder();
            string[] strArray = sendType.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                int num2;
                if (strArray[i].IsInt(out num2))
                {
                    switch (num2)
                    {
                        case 0:
                            builder.Append("站内消息、");
                            break;

                        case 1:
                            builder.Append("手机短信、");
                            break;

                        case 2:
                            builder.Append("微信、");
                            break;
                    }
                }
            }
            char[] trimChars = new char[] { '、' };
            return builder.ToString().TrimEnd(trimChars);
        }

        public string Send(RoadFlow.Model.Message message, List<RoadFlow.Model.User> receiveUsers = null)
        {
            if (message == null)
            {
                return "消息为空!";
            }
            if (receiveUsers == null)
            {
                receiveUsers = new Organize().GetAllUsers(message.ReceiverIdString);
            }
            if (!Enumerable.Any<RoadFlow.Model.User>((IEnumerable<RoadFlow.Model.User>)receiveUsers))
            {
                return "消息没有接收人!";
            }
            if (message.ReceiverIdString.IsNullOrEmpty())
            {
                StringBuilder builder = new StringBuilder();
                foreach (RoadFlow.Model.User user in receiveUsers)
                {
                    builder.Append("u_");
                    builder.Append(user.Id);
                    builder.Append(",");
                }
                char[] trimChars = new char[] { ',' };
                message.ReceiverIdString = builder.ToString().TrimEnd(trimChars);
            }
            List<RoadFlow.Model.MessageUser> list = new List<RoadFlow.Model.MessageUser>();
            List<string> userIds = new List<string>();
            char[] separator = new char[] { ',' };
            string[] strArray = message.SendType.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                JObject obj1;
                int num2;
                if (strArray[i].IsInt(out num2))
                {
                    switch (num2)
                    {
                        case 0:
                            goto Label_0106;

                        case 1:
                            goto Label_0180;

                        case 2:
                            goto Label_01D6;
                    }
                }
                continue;
                Label_0106:
                foreach (RoadFlow.Model.User user2 in receiveUsers)
                {
                    RoadFlow.Model.MessageUser user1 = new RoadFlow.Model.MessageUser
                    {
                        MessageId = message.Id,
                        UserId = user2.Id,
                        IsRead = 0
                    };
                    list.Add(user1);
                    userIds.Add(user2.Id.ToString().ToLower());
                }
                continue;
                Label_0180:
                foreach (RoadFlow.Model.User user3 in receiveUsers)
                {
                    if (!user3.Mobile.IsNullOrWhiteSpace())
                    {
                        RoadFlow.Utility.SMS.SMS.SendSMS(message.Contents.RemoveHTML(), user3.Mobile.Trim());
                    }
                }
                continue;
                Label_01D6:
                obj1 = new JObject();
                obj1.Add("content", (JToken)(message.Contents + (message.SenderName.IsNullOrWhiteSpace() ? "" : ("  发送人：" + message.SenderName))));
                JObject contentJson = obj1;
                Common.SendMessage(receiveUsers, contentJson, "text", 0);
            }
            if (Enumerable.Any<string>((IEnumerable<string>)userIds))
            {
                JObject obj4 = new JObject();
                obj4.Add("id", (JToken)message.Id.ToString());
                obj4.Add("title", "消息");
                obj4.Add("contents", (JToken)message.Contents);
                obj4.Add("count", 1);
                JObject obj3 = obj4;
                new SignalRHub().SendMessage(obj3.ToString(0, Array.Empty<JsonConverter>()), userIds, "");
            }
            if (this.Add(message, list.ToArray()) <= 0)
            {
                return "发送失败!";
            }
            return "1";
        }

        public string Send(RoadFlow.Model.User user, string content, string sendType = "0", RoadFlow.Model.User sender = null)
        {
            List<RoadFlow.Model.User> users = new List<RoadFlow.Model.User> {
            user
        };
            return this.Send(users, content, sendType, sender);
        }

        public string Send(List<RoadFlow.Model.User> users, string content, string sendType = "0", RoadFlow.Model.User sender = null)
        {
            RoadFlow.Model.Message message = new RoadFlow.Model.Message
            {
                Contents = content,
                Id = Guid.NewGuid(),
                SendTime = DateTimeExtensions.Now,
                SendType = sendType,
                SiteMessage = ("," + sendType + ",").Contains(",0,") ? 1 : 0,
                Type = (sender == null) ? 2 : 1
            };
            if (sender != null)
            {
                message.SenderId = new Guid?(sender.Id);
                message.SenderName = sender.Name;
            }
            return this.Send(message, users);
        }

        public string Send(Guid userId, string content, string sendType = "0", RoadFlow.Model.User sender = null)
        {
            List<RoadFlow.Model.User> users = new List<RoadFlow.Model.User> {
            new User().Get(userId)
        };
            return this.Send(users, content, sendType, sender);
        }
    }


   



}
