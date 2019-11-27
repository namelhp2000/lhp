using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{
   

    [Area("RoadFlowCore")]
    public class MailController : Controller
    {
        // Methods
        [Validate(CheckApp = false)]
        public string ChangeStatus()
        {
            Mail mail = new Mail();
            foreach (string str2 in base.Request.Forms("ids").Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.MailInBox mailInBox = mail.GetMailInBox(guid);
                    if ((mailInBox != null) && (mailInBox.IsRead != 1))
                    {
                        mail.UpdateIsRead(guid, 1, true);
                    }
                }
            }
            return "标记成功!";
        }

        [Validate(CheckApp = false)]
        public IActionResult DeletedBox()
        {
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&rf_appopenmodel=" + base.Request.Querys("rf_appopenmodel");
            return this.View();
        }

        [Validate(CheckApp = false)]
        public string DeleteDeletedMail()
        {
            Mail mail = new Mail();
            foreach (string str2 in base.Request.Forms("ids").Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.MailDeletedBox mailDeletedBox = mail.GetMailDeletedBox(guid);
                    if (mailDeletedBox != null)
                    {
                        mail.DeleteMailDeletedBox(mailDeletedBox);
                    }
                }
            }
            return "删除成功!";
        }

        [Validate(CheckApp = false)]
        public string DeleteDraftMail()
        {
            Mail mail = new Mail();
            foreach (string str2 in base.Request.Forms("ids").Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    mail.DeleteMailOutBox(guid);
                }
            }
            return "删除成功!";
        }

        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string DeleteMail()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Forms("mailid"), out guid))
            {
                return "邮件Id错误!";
            }
            int num = base.Request.Forms("status").ToInt(0);
            new Mail().DeleteMailInBox(guid, num);
            return (((1 == num) ? "彻底" : "") + "删除成功!");
        }

        [Validate(CheckApp = false)]
        public string DeleteMail1()
        {
            Mail mail = new Mail();
            foreach (string str2 in base.Request.Forms("ids").Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    mail.DeleteMailInBox(guid, 0);
                }
            }
            return "删除成功!";
        }

        [Validate(CheckApp = false)]
        public string DeleteOutMail()
        {
            Mail mail = new Mail();
            foreach (string str2 in base.Request.Forms("ids").Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    mail.DeleteMailOutBox(guid);
                }
            }
            return "删除成功!";
        }

        [Validate(CheckApp = false)]
        public IActionResult DraftBox()
        {
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["query"]="appid=" + base.Request.Querys("appid") + "&rf_appopenmodel=" + base.Request.Querys("rf_appopenmodel");
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult InBox()
        {
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&rf_appopenmodel=" + base.Request.Querys("rf_appopenmodel");
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult OutBox()
        {
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&rf_appopenmodel=" + base.Request.Querys("rf_appopenmodel");
            return this.View();
        }

        [Validate(CheckApp = false)]
        public string QueryDeletedBox()
        {
            int num3;
            string str = base.Request.Forms("sidx");
            string str2 = base.Request.Forms("sord");
            string str3 = base.Request.Forms("Subject");
            string str4 = base.Request.Forms("SendUser");
            string str5 = base.Request.Forms("Date1");
            string str6 = base.Request.Forms("Date2");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str7 = (str.IsNullOrEmpty() ? "SendDateTime" : str) + " " + (str2.IsNullOrEmpty() ? "DESC" : str2);
            DataTable table = new Mail().GetMailDeletedBoxPagerList(out num3, pageSize, pageNumber, Current.UserId, str3, str4.IsNullOrWhiteSpace() ? "" : str4.RemoveUserRelationPrefix(), str5, str6, str7);
            JArray array = new JArray();
            User user = new User();
            Organize organize = new Organize();
            foreach (DataRow row in table.Rows)
            {
                StringBuilder builder = new StringBuilder();
                if (row["SubjectColor"].ToString().IsNullOrWhiteSpace())
                {
                    builder.Append(row["Subject"].ToString());
                }
                else
                {
                    string[] textArray1 = new string[] { "<span style=\"color:", row["SubjectColor"].ToString(), "\">", row["Subject"].ToString(), "</span>" };
                    builder.Append(string.Concat((string[])textArray1));
                }
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Subject", (JToken)builder.ToString());
                obj1.Add("SendUserId", (JToken)(user.GetName(StringExtensions.ToGuid(row["SendUserId"].ToString())) + "<span style=\"color:#666;margin-left:5px;\">(" + user.GetOrganizeMainShowHtml(StringExtensions.ToGuid(row["SendUserId"].ToString()), false) + ")</span>"));
                obj1.Add("SendDateTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["SendDateTime"].ToString())));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate(CheckApp = false)]
        public string QueryDraftBox()
        {
            int num3;
            string str = base.Request.Forms("sidx");
            string str2 = base.Request.Forms("sord");
            string str3 = base.Request.Forms("Subject");
            string str4 = base.Request.Forms("Date1");
            string str5 = base.Request.Forms("Date2");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str6 = (str.IsNullOrEmpty() ? "SendDateTime" : str) + " " + (str2.IsNullOrEmpty() ? "DESC" : str2);
            DataTable table = new Mail().GetMailOutBoxPagerList(out num3, pageSize, pageNumber, Current.UserId, str3, str4, str5, str6, 0);
            JArray array = new JArray();
            User user = new User();
            Organize organize = new Organize();
            foreach (DataRow row in table.Rows)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("<a href=\"javascript:;\" class=\"blue\" onclick=\"detail('" + row["Id"].ToString() + "');\">");
                if (row["SubjectColor"].ToString().IsNullOrWhiteSpace())
                {
                    builder.Append(row["Subject"].ToString());
                }
                else
                {
                    string[] textArray1 = new string[] { "<span style=\"color:", row["SubjectColor"].ToString(), "\">", row["Subject"].ToString(), "</span>" };
                    builder.Append(string.Concat((string[])textArray1));
                }
                builder.Append("</a>");
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Subject", (JToken)builder.ToString());
                obj1.Add("SendDateTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["SendDateTime"].ToString())));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate(CheckApp = false)]
        public string QueryInBox()
        {
            int num3;
            string str = base.Request.Forms("sidx");
            string str2 = base.Request.Forms("sord");
            string str3 = base.Request.Forms("Subject");
            string str4 = base.Request.Forms("SendUser");
            string str5 = base.Request.Forms("Date1");
            string str6 = base.Request.Forms("Date2");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str7 = (str.IsNullOrEmpty() ? "SendDateTime" : str) + " " + (str2.IsNullOrEmpty() ? "DESC" : str2);
            DataTable table = new Mail().GetMailInBoxPagerList(out num3, pageSize, pageNumber, Current.UserId, str3, str4.IsNullOrWhiteSpace() ? "" : str4.RemoveUserRelationPrefix(), str5, str6, str7);
            JArray array = new JArray();
            User user = new User();
            Organize organize = new Organize();
            foreach (DataRow row in table.Rows)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("<a href=\"javascript:;\" class=\"blue\" onclick=\"detail('" + row["Id"].ToString() + "');\">");
                if (row["SubjectColor"].ToString().IsNullOrWhiteSpace())
                {
                    builder.Append(row["Subject"].ToString());
                }
                else
                {
                    string[] textArray1 = new string[] { "<span style=\"color:", row["SubjectColor"].ToString(), "\">", row["Subject"].ToString(), "</span>" };
                    builder.Append(string.Concat((string[])textArray1));
                }
                builder.Append("</a>");
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Subject", (JToken)builder.ToString());
                obj1.Add("SendDateTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["SendDateTime"].ToString())));
                obj1.Add("SendUserId", (JToken)(user.GetName(StringExtensions.ToGuid(row["SendUserId"].ToString())) + "<span style=\"color:#666;margin-left:5px;\">(" + user.GetOrganizeMainShowHtml(StringExtensions.ToGuid(row["SendUserId"].ToString()), false) + ")</span>"));
                obj1.Add("IsRead", row["IsRead"].ToString().Equals("0") ? "<span class=\"noread\">未读</span>" : "<span class=\"read\">已读</span>");
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate(CheckApp = false)]
        public string QueryOutBox()
        {
            int num3;
            string str = base.Request.Forms("sidx");
            string str2 = base.Request.Forms("sord");
            string str3 = base.Request.Forms("Subject");
            string str4 = base.Request.Forms("Date1");
            string str5 = base.Request.Forms("Date2");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str6 = (str.IsNullOrEmpty() ? "SendDateTime" : str) + " " + (str2.IsNullOrEmpty() ? "DESC" : str2);
            DataTable table = new Mail().GetMailOutBoxPagerList(out num3, pageSize, pageNumber, Current.UserId, str3, str4, str5, str6, 1);
            JArray array = new JArray();
            User user = new User();
            Organize organize = new Organize();
            foreach (DataRow row in table.Rows)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("<a href=\"javascript:;\" class=\"blue\" onclick=\"detail('" + row["Id"].ToString() + "');\">");
                if (row["SubjectColor"].ToString().IsNullOrWhiteSpace())
                {
                    builder.Append(row["Subject"].ToString());
                }
                else
                {
                    string[] textArray1 = new string[] { "<span style=\"color:", row["SubjectColor"].ToString(), "\">", row["Subject"].ToString(), "</span>" };
                    builder.Append(string.Concat((string[])textArray1));
                }
                builder.Append("</a>");
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Subject", (JToken)builder.ToString());
                obj1.Add("SendDateTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["SendDateTime"].ToString())));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate(CheckApp = false)]
        public string Recovery()
        {
            string str = base.Request.Forms("ids");
            Mail mail = new Mail();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    mail.RecoveryMailDeletedBox(guid);
                }
            }
            return "邮件已成功还原至收件箱!";
        }

        [Validate(CheckApp = false)]
        public string SaveSendMail()
        {
            Guid guid;
            string str = base.Request.Forms("ReceiveUsers");
            string str2 = base.Request.Forms("Subject");
            string str3 = base.Request.Forms("SubjectColor");
            string str4 = base.Request.Forms("Contents");
            string str5 = base.Request.Forms("Files");
            string str6 = base.Request.Forms("CarbonCopy");
            string str7 = base.Request.Forms("SecretCopy");
            string str8 = base.Request.Querys("id");
            int num = base.Request.Forms("Status").ToInt(0);
            if ((str2.IsNullOrWhiteSpace() || str4.IsNullOrWhiteSpace()) || ((num == 1) && str.IsNullOrWhiteSpace()))
            {
                return "{\"success\":0,\"message\":\"数据验证错误!\"}";
            }
            Mail mail = new Mail();
            RoadFlow.Model.MailOutBox mailOutBox = null;
            RoadFlow.Model.MailContent mailContent = null;
            bool isAdd = false;
            if (StringExtensions.IsGuid(str8, out guid))
            {
                mailOutBox = mail.GetMailOutBox(guid);
            }
            if (mailOutBox == null)
            {
                isAdd = true;
                RoadFlow.Model.MailOutBox box1 = new RoadFlow.Model.MailOutBox();
                box1.Id=Guid.NewGuid();
                box1.ContentsId=Guid.NewGuid();
                mailOutBox = box1;
            }
            RoadFlow.Model.MailContent content1 = new RoadFlow.Model.MailContent();
            content1.Id=mailOutBox.ContentsId;
            mailContent = content1;
            mailContent.Contents = str4;
            mailContent.Files = str5;
            mailOutBox.ReceiveUsers = str;
            mailOutBox.SendDateTime= DateTimeExtensions.Now;
            mailOutBox.Status = num;
            mailOutBox.Subject = str2.Trim();
            if (!str3.IsNullOrWhiteSpace())
            {
                mailOutBox.SubjectColor = str3;
            }
            if (!str6.IsNullOrWhiteSpace())
            {
                mailOutBox.CarbonCopy = str6;
            }
            if (!str7.IsNullOrWhiteSpace())
            {
                mailOutBox.SecretCopy = str7;
            }

            mailOutBox.UserId=Current.UserId;
            int num2 = mail.Send(mailOutBox, mailContent, isAdd);
            string[] textArray1 = new string[] { "{\"success\":1,\"message\":\"", (num == 0) ? "保存" : "发送", "成功!\",\"id\":\"", (num == 0) ? ((string)mailOutBox.Id.ToString()) : "", "\"}" };
            return string.Concat((string[])textArray1);
        }

        [Validate(CheckApp = false)]
        public IActionResult SendMail()
        {
            string str = base.Request.Querys("opation");
            Mail mail = new Mail();
            RoadFlow.Model.MailOutBox mailOutBox = null;
            string contents = string.Empty;
            string files = string.Empty;
            string receiveUsers = string.Empty;
            string subject = string.Empty;
            string subjectColor = string.Empty;
            string carbonCopy = string.Empty;
            string secretCopy = string.Empty;

            bool flag = str.Equals("redirect");
            bool flag2 = str.Equals("return");
            bool flag3 = str.Equals("againedit");
            if ((flag | flag2) | flag3)
            {
                Guid guid;
                if (StringExtensions.IsGuid(base.Request.Querys("mailid"), out guid))
                {
                    if (flag3)
                    {
                        mailOutBox = mail.GetMailOutBox(guid);
                    }
                    else
                    {
                        RoadFlow.Model.MailInBox mailInBox = mail.GetMailInBox(guid);
                        if (mailInBox != null)
                        {
                            mailOutBox = mail.GetMailOutBox(mailInBox.OutBoxId);
                            if (mailOutBox != null)
                            {
                                mailOutBox.ReceiveUsers = flag ? "" : ("u_" + mailInBox.SendUserId.ToString());
                                mailOutBox.SubjectColor = string.Empty;
                                mailOutBox.Subject = (flag ? "转发" : (flag2 ? "回复" : "")) + "：" + mailInBox.Subject;
                            }
                        }
                    }
                }
            }
            else
            {
                Guid guid3;
                if (StringExtensions.IsGuid(base.Request.Querys("id"), out guid3))
                {
                    mailOutBox = mail.GetMailOutBox(guid3);
                }
            }
            if (mailOutBox != null)
            {
                receiveUsers = mailOutBox.ReceiveUsers;

                subject = mailOutBox.Subject;
                subjectColor = mailOutBox.SubjectColor;
                if (flag3)
                {
                    carbonCopy = mailOutBox.CarbonCopy;
                    secretCopy = mailOutBox.SecretCopy;
                }

                RoadFlow.Model.MailContent mailContent = mail.GetMailContent(mailOutBox.ContentsId);
                if (mailContent != null)
                {
                    if (flag | flag2)
                    {
                        string[] textArray1 = new string[] { "<p></p><div style=\"padding:4px;margin-bottom:8px;border:1px solid #ccc;border-radius:4px;\"><div style=\"padding:5px 0;\">------------------原始邮件------------------</div><div>发件人：", new User().GetName(mailOutBox.UserId), " (", new User().GetOrganizeMainShowHtml(mailOutBox.UserId, false), ")&nbsp;&nbsp;&nbsp;&nbsp;收件时间：", DateTimeExtensions.ToShortDateTimeString(mailOutBox.SendDateTime), "</div>", mailContent.Contents, mailContent.Files.IsNullOrWhiteSpace() ? ((string)string.Empty) : ((string)("<div>附件：" + mailContent.Files.ToFilesShowString(true, true) + "</div>")), "</div>" };
                        contents = string.Concat((string[])textArray1);
                    }
                    else
                    {
                        contents = mailContent.Contents;
                    }
                    files = mailContent.Files;
                }
            }
            base.ViewData["contents"]= contents;
            base.ViewData["files"]= files;
            base.ViewData["receiveUsers"]= receiveUsers;

            base.ViewData["CarbonCopy"]= carbonCopy;
            base.ViewData["SecretCopy"]= secretCopy;

            base.ViewData["subject"]=subject;
            base.ViewData["subjectColor"]= subjectColor;
            base.ViewData["refreshtabid"]= base.Request.Querys("refreshtabid");
            string[] textArray2 = new string[] { "appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"]= string.Concat((string[])textArray2);
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult ShowMail()
        {
            Guid guid;
            string str = base.Request.Querys("mailid");
            RoadFlow.Model.MailInBox mailInBox = null;
            Mail mail = new Mail();
            if (StringExtensions.IsGuid(str, out guid))
            {
                mailInBox = mail.GetMailInBox(guid);
            }
            if (mailInBox == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content="未找到该邮件!";
                return result1;
            }
            if (mailInBox.IsRead == 0)
            {
                mail.UpdateIsRead(mailInBox.Id, 1, true);
            }
            string contents = string.Empty;
            string str3 = string.Empty;
            RoadFlow.Model.MailContent mailContent = mail.GetMailContent(mailInBox.ContentsId);
            if (mailContent != null)
            {
                contents = mailContent.Contents;
                str3 = mailContent.Files.ToFilesShowString(true, true);

            }
            base.ViewData["contents"]= contents;
            base.ViewData["files"]= str3;
            base.ViewData["mailid"]= mailInBox.Id;
            base.ViewData["mailOutBox"]=new Mail().GetMailOutBox(mailInBox.OutBoxId);

            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid"), "&pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View(mailInBox);
        }

        [Validate(CheckApp = false)]
        public IActionResult ShowOutMail()
        {
            Guid guid;
            string str = base.Request.Querys("mailid");
            RoadFlow.Model.MailOutBox mailOutBox = null;
            Mail mail = new Mail();
            if (StringExtensions.IsGuid(str, out guid))
            {
                mailOutBox = mail.GetMailOutBox(guid);
            }
            if (mailOutBox == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content="未找到该邮件!";
                return result1;
            }
            string contents = string.Empty;
            string str3 = string.Empty;
            RoadFlow.Model.MailContent mailContent = mail.GetMailContent(mailOutBox.ContentsId);
            if (mailContent != null)
            {
                contents = mailContent.Contents;
                str3 = mailContent.Files.ToFilesShowString(false, true);


            }
            base.ViewData["contents"]=contents;
            base.ViewData["files"]= str3;
            base.ViewData["mailid"]= mailOutBox.Id;
          


            base.ViewData["query"]="appid=" + base.Request.Querys("appid");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber") };
            base.ViewData["query1"]= string.Concat((string[])textArray1);
            base.ViewData["refreshtabid"]= base.Request.Querys("refreshtabid");
            base.ViewData["isWithdraw"]= (bool)mail.IsWithdraw(guid);
            return this.View(mailOutBox);
        }

        [Validate(CheckApp = false)]
        public string WithdrawOutMail()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Forms("id"), out guid))
            {
                return "邮件ID错误!";
            }
            if (new Mail().Withdraw(guid))
            {
                return "邮件已成功撤回至草稿箱!";
            }
            return "撤回失败!";
        }
    }







   
}
