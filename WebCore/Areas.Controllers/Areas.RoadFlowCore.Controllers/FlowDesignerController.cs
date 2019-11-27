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


    #region 新的方法 RoadFlowCore工作流2.8.3更新日志

    [Area("RoadFlowCore")]
    public class FlowDesignerController : Controller
    {
        // Methods
        [Validate]
        public void Export()
        {
            string exportFlowString = new RoadFlow.Business.Flow().GetExportFlowString(base.Request.Querys("flowid"));
            byte[] bytes = Encoding.UTF8.GetBytes(exportFlowString);
            base.Response.Headers.Add("Server-FileName", "exportflow.json");
            base.Response.ContentType="application/octet-stream";
            base.Response.Headers.Add("Content-Disposition", "attachment; filename=exportflow.json");
            int length = bytes.Length;
            base.Response.Headers.Add("Content-Length", (StringValues)((int)length).ToString());
            base.Response.Body.Write(bytes);
            base.Response.Body.Flush();
        }

        [Validate]
        public string GetJSON()
        {
            Guid guid;
            //***************************
            Guid guid2;
            Guid guid3;
            string str = base.Request.Querys("dynamicstepid");
            string str2 = base.Request.Querys("groupid");

            if (StringExtensions.IsGuid(str, out guid) && StringExtensions.IsGuid(str2, out guid2))
            {
                RoadFlow.Model.FlowDynamic dynamic = new RoadFlow.Business.FlowDynamic().Get(guid, guid2);
                if (dynamic != null)
                {
                    return dynamic.FlowJSON;
                }
                return "{}";
            }

            if (!StringExtensions.IsGuid(base.Request.Querys("flowid"), out guid3))
            {
                return "{}";
            }
            RoadFlow.Model.Flow flow = new RoadFlow.Business.Flow().Get(guid3);
            if (flow != null)
            {
                return flow.DesignerJSON;
            }
            return "";
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
            RoadFlow.Business.Flow flow = new RoadFlow.Business.Flow();
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
                    string str2 = flow.ImportFlow(json);
                    if (!"1".Equals(str2))
                    {
                        builder.Append(str2 + ",");
                    }
                }
            }
            if (builder.Length > 0)
            {
                base.ViewData["errmsg"]= builder.ToString().TrimEnd((char)0xff0c);
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate]
        public IActionResult Index()
        {
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            return this.View();
        }

        [Validate]
        public IActionResult Index1()
        {
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["flowId"]= base.Request.Querys("flowid");
            base.ViewData["isNewFlow"]= base.Request.Querys("isnewflow");
            return this.View();
        }

        public string Install()
        {
            string json = base.Request.Forms("json");
            string str2 = new RoadFlow.Business.Flow().Install(json);
            if (!"1".Equals(str2))
            {
                return str2;
            }
            return "安装成功!";
        }

        [Validate]
        public IActionResult List()
        {
            string str = base.Request.Querys("typeid");
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["typeId"]= str;
            base.ViewData["query"]= "typeid=" + str + "&appid=" + base.Request.Querys("appid");
            return this.View();
        }

        [Validate]
        public IActionResult ListDelete()
        {
            string str = base.Request.Querys("typeid");
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["typeId"]= str;
            base.ViewData["query"]= "typeid=" + str + "&appid=" + base.Request.Querys("appid");
            return this.View();
        }

        [Validate]
        public IActionResult Opation()
        {
            string str = base.Request.Querys("op");
            string str2 = string.Empty;
            if (str == "save")
            {
                str2 = "正在保存...";
            }
            else if (str == "install")
            {
                str2 = "正在安装...";
            }
            else if (str == "uninstall")
            {
                str2 = "正在卸载...";
            }
            else if (str == "delete")
            {
                str2 = "正在删除...";
            }
            base.ViewData["msg"]= str2;
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["openerid"]= base.Request.Querys("openerid");
            base.ViewData["op"]= str;
            return this.View();
        }

   


        #region 3.8.8方法
        [Validate]
        public string Query()
        {
            Guid guid;
            int num3;
            string str = base.Request.Forms("flow_name");
            string str2 = base.Request.Forms("typeid");
            string str3 = base.Request.Forms("sidx");
            string str4 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str5 = (str3.IsNullOrEmpty() ? "CreateDate" : str3) + " " + (str4.IsNullOrEmpty() ? "DESC" : str4);
            if (StringExtensions.IsGuid(str2, out guid))
            {
                str2 = StringExtensions.JoinSqlIn<Guid>(new RoadFlow.Business.Dictionary().GetAllChildsId(guid, true), true);
            }
            RoadFlow.Business.Flow flow = new RoadFlow.Business.Flow();
            RoadFlow.Business.FlowApiSystem system = new RoadFlow.Business.FlowApiSystem();
            DataTable table = flow.GetPagerList(out num3, pageSize, pageNumber, flow.GetManageFlowIds(Current.UserId), str, str2, str5, -1);
            JArray array = new JArray();
            RoadFlow.Business.User user = new RoadFlow.Business.User();
            foreach (DataRow row in table.Rows)
            {
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)row["Id"].ToString());
                obj3.Add("Name", (JToken)row["Name"].ToString());
                obj3.Add("CreateDate", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["CreateDate"].ToString())));
                obj3.Add("CreateUser", (JToken)user.GetName(StringExtensions.ToGuid(row["CreateUser"].ToString())));
                obj3.Add("Status", (JToken)flow.GetStatusTitle(row["Status"].ToString().ToInt(-2147483648)));
                obj3.Add("SystemId", (JToken)system.GetName(StringExtensions.ToGuid(row["SystemId"].ToString())));
                obj3.Add("Note", (JToken)row["Note"].ToString());
                string[] textArray1 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"openflow('", row["Id"].ToString(), "', '", row["Name"].ToString(), "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>" };
                obj3.Add("Opation", (JToken)string.Concat((string[])textArray1));
                JObject obj2 = obj3;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }


        #endregion
        [Validate]
        public string QueryDelete()
        {
            int num3;
            string str = base.Request.Forms("flow_name");
            string str2 = base.Request.Forms("typeid");
            string str3 = base.Request.Forms("sidx");
            string str4 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str5 = (str3.IsNullOrEmpty() ? "CreateDate" : str3) + " " + (str4.IsNullOrEmpty() ? "DESC" : str4);
            RoadFlow.Business.Flow flow = new RoadFlow.Business.Flow();
            DataTable table = flow.GetPagerList(out num3, pageSize, pageNumber, flow.GetManageFlowIds(Current.UserId), str, "", str5, 3);
            JArray array = new JArray();
            RoadFlow.Business.User user = new RoadFlow.Business.User();
            foreach (DataRow row in table.Rows)
            {
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)row["Id"].ToString());
                obj3.Add("Name", (JToken)row["Name"].ToString());
                obj3.Add("CreateDate", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["CreateDate"].ToString())));
                obj3.Add("CreateUser", (JToken)user.GetName(StringExtensions.ToGuid(row["CreateUser"].ToString())));
                obj3.Add("Status", (JToken)flow.GetStatusTitle(row["Status"].ToString().ToInt(-2147483648)));
                obj3.Add("Note", (JToken)row["Note"].ToString());
                string[] textArray1 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"reply('", row["Id"].ToString(), "', '", row["Name"].ToString(), "');return false;\"><i class=\"fa fa-reply\"></i>还原</a>" };
                obj3.Add("Opation", (JToken)string.Concat((string[])textArray1));
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
            if (!StringExtensions.IsGuid(base.Request.Forms("flowid"), out guid))
            {
                return "流程ID错误!";
            }
            RoadFlow.Business.Flow flow = new RoadFlow.Business.Flow();
            RoadFlow.Model.Flow flow2 = flow.Get(guid);
            if (flow2 == null)
            {
                return "没有找到要还原的流程!";
            }
            flow2.Status = 0;
            flow.Update(flow2);
            return "还原成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string Save()
        {
            string json = base.Request.Forms("json");
            string str2 = new RoadFlow.Business.Flow().Save(json);
            if (!"1".Equals(str2))
            {
                return str2;
            }
            return "保存成功!";
        }


        /// <summary>
        /// 另存为
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult SaveAs()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            string[] textArray1 = new string[] { "typeid=", base.Request.Querys("typeid"), "&appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View();
        }

        /// <summary>
        /// 另存为保存
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string SaveAsSave()
        {
            Guid guid;
            Guid guid2;
            string str = base.Request.Forms("newflowname");
            string str2 = base.Request.Querys("flowid");
            if (str.IsNullOrWhiteSpace())
            {
                return "{\"success\":0,\"msg\":\"新的流程名称不能为空!\"}";
            }
            if (!StringExtensions.IsGuid(str2, out guid))
            {
                return "{\"success\":0,\"msg\":\"流程Id错误!\"}";
            }
            string str3 = new RoadFlow.Business.Flow().SaveAs(guid, str.Trim());
            if (StringExtensions.IsGuid(str3, out guid2))
            {
                return ("{\"success\":1,\"msg\":\"另存成功!\",\"newId\":\"" + str3 + "\"}");
            }
            return ("{\"success\":0,\"msg\":\"" + str3 + "\"}");
        }









        [Validate]
        public IActionResult Set_Flow()
        {
            string str = base.Request.Querys("flowid");
            string str2 = base.Request.Querys("isadd");
            if (str.IsNullOrWhiteSpace())
            {
                str = Guid.NewGuid().ToString();
            }
            base.ViewData["isAdd"]= str2;
            base.ViewData["openerid"]= base.Request.Querys("openerid");
            base.ViewData["flowId"]= str;
            base.ViewData["defaultManager"]= "u_" + Current.UserId;
            base.ViewData["dbconnOptions"]= new RoadFlow.Business.DbConnection().GetOptions("");
          //  base.ViewData["flowTypeOptions"]= new RoadFlow.Business.Dictionary().GetOptionsByCode("system_applibrarytype_flow", ValueField.Id, "", true);
            base.ViewData["flowTypeOptions"]= new RoadFlow.Business.Dictionary().GetOptionsByCode("system_applibrarytype_flow", ValueField.Id, "", true, true);
            base.ViewData["flowSystemOptions"]= new RoadFlow.Business.FlowApiSystem().GetAllOptions("");

            return this.View();
        }

        [Validate]
        public IActionResult Set_Line()
        {
            base.ViewData["openerid"]= base.Request.Querys("openerid");
            base.ViewData["lineId"]= base.Request.Querys("id");
            base.ViewData["fromId"]= base.Request.Querys("from");
            base.ViewData["toId"]= base.Request.Querys("to");
            return this.View();
        }

        [Validate]
        public IActionResult Set_Step()
        {
            base.ViewData["stepId"]= base.Request.Querys("id");
            base.ViewData["x"]= base.Request.Querys("x");
            base.ViewData["y"]= base.Request.Querys("y");
            base.ViewData["width"]= base.Request.Querys("width");
            base.ViewData["height"]= base.Request.Querys("height");
            base.ViewData["issubflow"]= base.Request.Querys("issubflow");
            base.ViewData["openerid"]= base.Request.Querys("openerid");
            base.ViewData["formTypes"] = new RoadFlow.Business.Dictionary().GetOptionsByCode("system_applibrarytype", ValueField.Id, "", true, false);
           // base.ViewData["formTypes"]= new RoadFlow.Business.Dictionary().GetOptionsByCode("system_applibrarytype", ValueField.Id, "", true);
            base.ViewData["flowOptions"]= new RoadFlow.Business.Flow().GetOptions("");
            base.ViewData["button_List"] = new RoadFlow.Business.FlowButton().ButtonList();
            return this.View();
        }

        [Validate]
        public IActionResult Tree()
        {
            base.ViewData["rootId"]= new RoadFlow.Business.Dictionary().GetIdByCode("system_applibrarytype_flow").ToString();
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["openerid"]= base.Request.Querys("openerid");
            return this.View();
        }

        [Validate, ValidateAntiForgeryToken]
        public string UnInstall()
        {
            string[] strArray = base.Request.Forms("flowid").Split(',', (StringSplitOptions)StringSplitOptions.None);
            int num = base.Request.Forms("thoroughdelete").ToInt(0);
            string str = base.Request.Forms("status");
            RoadFlow.Business.Flow flow = new RoadFlow.Business.Flow();
            foreach (string str2 in strArray)
            {
                RoadFlow.Model.Flow flow2 = flow.Get(StringExtensions.ToGuid(str2));
                if (flow2 != null)
                {
                    if (num == 0)
                    {
                        int num3 = str.ToInt(3);
                        RoadFlow.Business.Log.Add(("2".Equals(str) ? "卸载" : "删除") + "了流程-" + flow2.Name, flow2.ToString(), LogType.流程管理, "", "", "", "", "", "", "", "");
                        flow2.Status = num3;
                        flow.Update(flow2);
                    }
                    else
                    {
                        flow.Delete(flow2);
                        RoadFlow.Model.AppLibrary byCode = new RoadFlow.Business.AppLibrary().GetByCode(flow2.Id.ToString());
                        if (byCode != null)
                        {
                            new RoadFlow.Business.AppLibrary().Delete(byCode.Id);
                        }
                        new RoadFlow.Business.FlowTask().DeleteByFlowId(flow2.Id);
                        RoadFlow.Business.Log.Add("彻底删除了流程-" + flow2.Name, flow2.ToString(),LogType.流程管理, "", "", "", "", "", "", "", "");
                    }
                    flow.ClearCache(flow2.Id);
                }
            }
            return "删除成功";
        }
    }


  




    #endregion



}
