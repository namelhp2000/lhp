using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{

 

  
  
    [Area("RoadFlowCore")]
    public class FormDesignerController : Controller
    {
        // Methods
        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            string[] strArray = base.Request.Forms("formid").Split(',', (StringSplitOptions)StringSplitOptions.None);
            int delete = base.Request.Forms("thoroughdelete").ToInt(0);
            foreach (string str in strArray)
            {
                Guid guid;
                if (!StringExtensions.IsGuid(str, out guid))
                {
                    return "表单ID为空!";
                }
                RoadFlow.Model.Form form = new RoadFlow.Business.Form().Get(guid);
                if (form == null)
                {
                    return "没有找到要删除的表单!";
                }
                int num3 = new RoadFlow.Business.Form().DeleteAndApplibrary(form, delete);
                RoadFlow.Business.Log.Add(((delete != 0) ? "彻底" : "") + "删除了表单-" + form.Name, form.ToString(), LogType.流程管理, "", "", "", "", "", "", "", "");
            }
            return "删除成功!";
        }

        [Validate]
        public void Export()
        {
            string exportFormString = new RoadFlow.Business.Form().GetExportFormString(base.Request.Querys("formid"));
            byte[] bytes = Encoding.UTF8.GetBytes(exportFormString);
            base.Response.Headers.Add("Server-FileName", "exportform.json");
            base.Response.ContentType="application/octet-stream";
            base.Response.Headers.Add("Content-Disposition", "attachment; filename=exportform.json");
            int length = bytes.Length;
            base.Response.Headers.Add("Content-Length", (StringValues)((int)length).ToString());
            base.Response.Body.Write(bytes);
            base.Response.Body.Flush();
        }

        public string GetChildOptions()
        {
            string source = base.Request.Forms("source");
            string parentValue = base.Request.Forms("value");
            string connId = base.Request.Forms("connid");
            string text = base.Request.Forms("text");
            string dictValueField = base.Request.Forms("dictvaluefield");
            string dictId = base.Request.Forms("dictid");
            string str7 = base.Request.Forms("dictIschild");
            string defaultValue = base.Request.Forms("defaultvalue");
            return new RoadFlow.Business.Form().GetChildOptions(source, connId, text, parentValue, dictValueField, dictId, defaultValue, "1".Equals(str7));
        }

        [Validate]
        public IActionResult Import()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate, ValidateAntiForgeryToken]
        public IActionResult ImportSave()
        {
            IFormFileCollection files = base.Request.Form.Files;
            if (files.Count == 0)
            {
                base.ViewData["errmsg"]= "您没有选择要导入的文件!";
                return this.View();
            }
            RoadFlow.Business.Form form = new RoadFlow.Business.Form();
            StringBuilder builder = new StringBuilder();
            using (IEnumerator<IFormFile> enumerator = files.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Stream stream = enumerator.Current.OpenReadStream();
                    int length = (int)stream.Length;
                    byte[] buffer = new byte[length];
                    for (int i = 0; i < length; i += stream.Read(buffer, i, 0x400))
                    {
                    }
                    string json = Encoding.UTF8.GetString(buffer);
                    string str2 = form.ImportForm(json);
                    if (!"1".Equals(str2))
                    {
                        builder.Append(str2 + "，");
                    }
                }
            }
            if (builder.Length > 0)
            {
                base.ViewData["errmsg"]= builder.ToString().TrimEnd((char)0xff0c);
            }
            base.ViewData["queryString"]=base.Request.UrlQuery();
            return this.View();
        }

        [Validate]
        public IActionResult Index()
        {
            string str = base.Request.UrlQuery();
            base.ViewData["queryString"]= str.IsNullOrWhiteSpace() ? "?1=1" : base.Request.UrlQuery();
            return this.View();
        }

        [Validate]
        public IActionResult Index1()
        {
            Guid guid;
            string str = base.Request.Querys("formid");
            string attribute = "{}";
            string subtableJSON = "[]";
            string eventJSON = "[]";
            string html = string.Empty;
            if (StringExtensions.IsGuid(str, out guid))
            {
                RoadFlow.Model.Form form = new RoadFlow.Business.Form().Get(guid);
                if (form != null)
                {
                    if(Config.IsFormPower)
                    {
                        if (!form.ManageUser.ContainsIgnoreCase(GuidExtensions.ToLowerString(Current.UserId)))
                        {
                            ContentResult result1 = new ContentResult();
                            result1.Content="您不能管理当前表单";
                            return result1;
                        }

                    }
                    attribute = form.attribute;
                    subtableJSON = form.SubtableJSON;
                    eventJSON = form.EventJSON;
                    html = form.Html;
                }
            }
            else
            {
                guid = Guid.NewGuid();
            }
            base.ViewData["formid"] = guid;
            base.ViewData["attr"]= attribute;
            base.ViewData["subtable"]= subtableJSON;
            base.ViewData["events"]= eventJSON;
            base.ViewData["html"]= html;
            base.ViewData["isNewForm"]= base.Request.Querys("isnewform");
            base.ViewData["typeId"]= base.Request.Querys("typeid");
            string[] textArray1 = new string[] { "typeid=", base.Request.Querys("typeid"), "&appid=", base.Request.Querys("appid"), "&iframeid=", base.Request.Querys("iframeid"), "&openerid=", base.Request.Querys("openerid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            base.ViewData["dbconnOptions"]= new RoadFlow.Business.DbConnection().GetOptions("");
            return this.View();
        }

        [Validate]
        public IActionResult List()
        {
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["iframeId"]= base.Request.Querys("iframeid");
            base.ViewData["openerId"]= base.Request.Querys("openerid");
            base.ViewData["typeid"] = base.Request.Querys("typeid");
            string[] textArray1 = new string[] { "typeid=", base.Request.Querys("typeid"), "&appid=", base.Request.Querys("appid"), "&iframeid=", base.Request.Querys("iframeid"), "&openerid=", base.Request.Querys("openerid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View();
        }

        [Validate]
        public IActionResult ListDelete()
        {
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["iframeId"]= base.Request.Querys("iframeid");
            base.ViewData["openerId"]= base.Request.Querys("openerid");
            base.ViewData["typeid"] = base.Request.Querys("typeid");
            string[] textArray1 = new string[] { "typeid=", base.Request.Querys("typeid"), "&appid=", base.Request.Querys("appid"), "&iframeid=", base.Request.Querys("iframeid"), "&openerid=", base.Request.Querys("openerid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View();
        }

        [Validate]
        public string QueryDeleteList()
        {
            int num3;
            string str = base.Request.Forms("form_name");
            string str2 = base.Request.Querys("typeid");
            string str3 = base.Request.Forms("sidx");
            string str4 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str5 = (str3.IsNullOrEmpty() ? "CreateDate" : str3) + " " + (str4.IsNullOrEmpty() ? "DESC" : str4);
            DataTable table = new DataTable();
            if(Config.IsFormPower)
            {
                table = new RoadFlow.Business.Form().GetPagerList(out num3, pageSize, pageNumber, Current.UserId, str, "", str5, 2);

            }
            else
            {
                table = new RoadFlow.Business.Form().GetPagerList(out num3, pageSize, pageNumber, str, "", str5, 2);
            }

            
            JArray array = new JArray();
            RoadFlow.Business.User user = new RoadFlow.Business.User();
            foreach (DataRow row in table.Rows)
            {
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)row["Id"].ToString());
                obj3.Add("Name", (JToken)row["Name"].ToString());
                obj3.Add("CreateUserName", (JToken)row["CreateUserName"].ToString());
                obj3.Add("CreateTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["CreateDate"].ToString())));
                obj3.Add("LastModifyTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["EditDate"].ToString())));
                string[] textArray1 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"reply('", row["Id"].ToString(), "', '", row["Name"].ToString(), "');return false;\"><i class=\"fa fa-reply\"></i>还原</a>" };
                obj3.Add("Edit", (JToken)string.Concat((string[])textArray1));
                JObject obj2 = obj3;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate]
        public string QueryList()
        {
            Guid guid;
            int num3;
            string str = base.Request.Forms("form_name");
            string str2 = base.Request.Querys("typeid");
            string str3 = base.Request.Forms("sidx");
            string str4 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str5 = (str3.IsNullOrEmpty() ? "CreateDate" : str3) + " " + (str4.IsNullOrEmpty() ? "DESC" : str4);
            if (StringExtensions.IsGuid(str2, out guid))
            {
                str2 = StringExtensions.JoinSqlIn<Guid>(new RoadFlow.Business.Dictionary().GetAllChildsId(guid, true), true);
            }


            DataTable table = new DataTable();
            if (Config.IsFormPower)
            {
                table = new RoadFlow.Business.Form().GetPagerList(out num3, pageSize, pageNumber, Current.UserId, str, str2, str5, -1);


            }
            else
            {
                table = new RoadFlow.Business.Form().GetPagerList(out num3, pageSize, pageNumber, str, str2, str5, -1);
            }


         
            JArray array = new JArray();
            RoadFlow.Business.User user = new RoadFlow.Business.User();
            foreach (DataRow row in table.Rows)
            {
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)row["Id"].ToString());
                obj3.Add("Name", (JToken)row["Name"].ToString());
                obj3.Add("CreateUserName", (JToken)row["CreateUserName"].ToString());
                obj3.Add("CreateTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["CreateDate"].ToString())));
                obj3.Add("LastModifyTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["EditDate"].ToString())));
                string[] textArray1 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"openform('", row["Id"].ToString(), "', '", row["Name"].ToString(), "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>" };
                obj3.Add("Edit", (JToken)string.Concat((string[])textArray1));
                JObject obj2 = obj3;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate, ValidateAntiForgeryToken]
        public string Reply()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Forms("formid"), out guid))
            {
                return "表单ID错误!";
            }
            RoadFlow.Business.Form form = new RoadFlow.Business.Form();
            RoadFlow.Model.Form form2 = form.Get(guid);
            if (form2 == null)
            {
                return "没有找到要还原的表单!";
            }
            form2.Status = 0;
            form.Update(form2);
            return "还原成功!";
        }

        [Validate]
        public IActionResult Tree()
        {
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["iframeId"]= base.Request.Querys("iframeid");
            base.ViewData["openerId"]= base.Request.Querys("openerid");
            base.ViewData["rootId"]= new RoadFlow.Business.Dictionary().GetIdByCode("system_applibrarytype_form");
            return this.View();
        }
    }


    



  





}
