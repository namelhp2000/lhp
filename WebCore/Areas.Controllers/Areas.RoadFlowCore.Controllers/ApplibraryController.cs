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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{




   
    [Area("RoadFlowCore")]
    public class ApplibraryController : Controller
    {
        // Methods
        [Validate]
        public IActionResult Button()
        {
            string str = base.Request.Querys("id");
            SystemButton button = new SystemButton();
            List<RoadFlow.Model.AppLibraryButton> listByApplibraryId = new AppLibraryButton().GetListByApplibraryId(StringExtensions.ToGuid(str));
            base.ViewData["buttonJSON"]=JsonConvert.SerializeObject(new SystemButton().GetAll());
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["buttonOptions"]= button.GetOptions("", "zh-CN");
            base.ViewData["buttonTypeOptions"]=button.GetButtonTypeOptions("");
            return this.View(listByApplibraryId);
        }

        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            string str = base.Request.Forms("ids");
            List<RoadFlow.Model.AppLibrary> list = new AppLibrary().Delete(str);
            Log.Add("删除了应用程序库", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate]
        public IActionResult Edit()
        {
            Guid guid;
            string str = base.Request.Querys("id");
            string str2 = base.Request.Querys("appid");
            string str3 = base.Request.Querys("tabid");
            string str4 = base.Request.Querys("typeid");
            string str5 = base.Request.Querys("pagesize");
            string str6 = base.Request.Querys("pagenumber");
            AppLibrary library = new AppLibrary();
            RoadFlow.Model.AppLibrary library2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                library2 = library.Get(guid);
            }
            if (library2 == null)
            {
                Guid guid2;
                RoadFlow.Model.AppLibrary library1 = new RoadFlow.Model.AppLibrary();
                library1.Id=Guid.NewGuid();
                library2 = library1;
                if (StringExtensions.IsGuid(str4, out guid2))
                {
                    library2.Type=guid2;
                }
            }
            base.ViewData["typeOptions"]= new Dictionary().GetOptionsByCode("system_applibrarytype", ValueField.Id, library2.Type.ToString(), true);
            base.ViewData["openModelOptions"]= new Dictionary().GetOptionsByCode("system_appopenmodel", ValueField.Value, ((int)library2.OpenMode).ToString(), true);
            base.ViewData["appId"]= str2;
            base.ViewData["tabId"]= str2;
            base.ViewData["typeId"]= str4;
            base.ViewData["pageSize"]= str5;
            base.ViewData["pageNumber"]= str6;
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View(library2);
        }

        [Validate]
        public void Export()
        {
            string exportString = new AppLibrary().GetExportString(base.Request.Querys("ids"));
            byte[] bytes = Encoding.UTF8.GetBytes(exportString);
            base.Response.Headers.Add("Server-FileName", "dictionary.json");
            base.Response.ContentType="application/octet-stream";
            base.Response.Headers.Add("Content-Disposition", "attachment; filename=applibrary.json");
            int length = bytes.Length;
            base.Response.Headers.Add("Content-Length", (StringValues)((int)length).ToString());
            base.Response.Body.Write(bytes);
            base.Response.Body.Flush();
        }

        public string GetOptionsByAppType()
        {
            string str = base.Request.Forms("type");
            string str2 = base.Request.Forms("value");
            List<RoadFlow.Model.AppLibrary> listByType = new AppLibrary().GetListByType(StringExtensions.ToGuid(str));
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.AppLibrary library in listByType)
            {
                builder.Append("<option value=\"" + library.Id + "\"");
                if (library.Id == StringExtensions.ToGuid(str2))
                {
                    builder.Append(" selected=\"selected\"");
                }
                builder.Append(">");
                builder.Append(library.Title);
                builder.Append("</option>");
            }
            return builder.ToString();
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
            AppLibrary library = new AppLibrary();
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
                    string str2 = library.Import(json);
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
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate]
        public IActionResult Index()
        {
            base.ViewData["iframeid"]= base.Request.Querys("appid") + "_iframe";
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            return this.View();
        }

        [Validate]
        public IActionResult List()
        {
            string str = base.Request.Querys("appid");
            string str2 = base.Request.Querys("tabid");
            string str3 = base.Request.Querys("typeid");
            base.ViewData["appId"]= str;
            base.ViewData["tabId"]= str;
            base.ViewData["typeId"]= str3;
            string[] textArray1 = new string[] { "typeid=", str3, "&appid=", str, "&tabid=", str2 };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View();
        }

        [Validate]
        public string Query()
        {
            Guid guid;
            int num3;
            string str = base.Request.Forms("Title");
            string str2 = base.Request.Forms("Address");
            string str3 = base.Request.Forms("typeid");
            string str4 = base.Request.Forms("sidx");
            string str5 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str6 = (str4.IsNullOrEmpty() ? "Type,Title" : str4) + " " + (str5.IsNullOrEmpty() ? "ASC" : str5);
            Dictionary dictionary = new Dictionary();
            if (StringExtensions.IsGuid(str3, out guid))
            {
                str3 = StringExtensions.JoinSqlIn<Guid>(dictionary.GetAllChildsId(guid, true), true);
            }
            DataTable table = new AppLibrary().GetPagerList(out num3, pageSize, pageNumber, str, str2, str3, str6);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)row["Id"].ToString());
                obj3.Add("Title", (JToken)row["Title"].ToString());
                obj3.Add("Address", (JToken)row["Address"].ToString());
                obj3.Add("TypeTitle", (JToken)dictionary.GetTitle(StringExtensions.ToGuid(row["Type"].ToString())));
                string[] textArray1 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('", row["Id"].ToString(), "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a><a class=\"list\" href=\"javascript:void(0);\" onclick=\"editButton('", row["Id"].ToString(), "');return false;\"><i class=\"fa fa-square-o\"></i>按钮</a>" };
                obj3.Add("Opation", (JToken)string.Concat((string[])textArray1));
                JObject obj2 = obj3;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate, ValidateAntiForgeryToken]
        public string Save(RoadFlow.Model.AppLibrary appLibraryModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            AppLibrary library = new AppLibrary();
            if (StringExtensions.IsGuid(base.Request.Querys("id"), out guid))
            {
                RoadFlow.Model.AppLibrary library2 = library.Get(guid);
                string oldContents = (library2 == null) ? "" : library2.ToString();
                library.Update(appLibraryModel);
                Log.Add("修改了应用程序库-" + appLibraryModel.Title, "", LogType.系统管理, oldContents, appLibraryModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                library.Add(appLibraryModel);
                Log.Add("添加了应用程序库-" + appLibraryModel.Title, appLibraryModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveButton()
        {
            string str = base.Request.Forms("buttonindex");
            string str2 = base.Request.Querys("id");
            AppLibraryButton button = new AppLibraryButton();
            List<RoadFlow.Model.AppLibraryButton> listByApplibraryId = button.GetListByApplibraryId(StringExtensions.ToGuid(str2));
            List<Tuple<RoadFlow.Model.AppLibraryButton, int>> list2 = new List<Tuple<RoadFlow.Model.AppLibraryButton, int>>();
            foreach (RoadFlow.Model.AppLibraryButton button2 in listByApplibraryId)
            {
                if (!str.ContainsIgnoreCase(button2.Id.ToString()))
                {
                    list2.Add(new Tuple<RoadFlow.Model.AppLibraryButton, int>(button2, 0));
                }
            }
            foreach (string str3 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid indexId;
                string str4 = base.Request.Forms("button_" + str3);
                string str5 = base.Request.Forms("buttonname_" + str3);
                string str6 = base.Request.Forms("buttonevents_" + str3);
                string str7 = base.Request.Forms("buttonico_" + str3);
                string str8 = base.Request.Forms("showtype_" + str3);
                string str9 = base.Request.Forms("buttonsort_" + str3);
                if (!str5.IsNullOrEmpty())
                {
                    if (StringExtensions.IsGuid(str3, out indexId))
                    {
                        RoadFlow.Model.AppLibraryButton button4 = listByApplibraryId.Find(delegate (RoadFlow.Model.AppLibraryButton p) {
                            return p.Id == indexId;
                        });
                        if (button4 != null)
                        {
                            button4.ButtonId=new Guid?(StringExtensions.ToGuid(str4));
                            button4.AppLibraryId=StringExtensions.ToGuid(str2);
                            button4.Events = str6;
                            button4.Ico = str7;
                            button4.IsValidateShow = 1;
                            button4.Name = str5;
                            button4.ShowType = str8.ToInt(0);
                            button4.Sort = str9.ToInt(0);
                            list2.Add(new Tuple<RoadFlow.Model.AppLibraryButton, int>(button4, 1));
                            continue;
                        }
                    }
                    RoadFlow.Model.AppLibraryButton button1 = new RoadFlow.Model.AppLibraryButton();
                    button1.Id=Guid.NewGuid();
                    button1.ButtonId=new Guid?(StringExtensions.ToGuid(str4));
                    button1.AppLibraryId=StringExtensions.ToGuid(str2);
                    button1.Events = str6;
                    button1.Ico = str7;
                    button1.IsValidateShow = 1;
                    button1.Name = str5;
                    button1.ShowType = str8.ToInt(0);
                    button1.Sort = str9.ToInt(0);
                    RoadFlow.Model.AppLibraryButton button3 = button1;
                    list2.Add(new Tuple<RoadFlow.Model.AppLibraryButton, int>(button3, 2));
                }
            }
            int num = button.Update(list2);
            Log.Add("保存了应用程序库按钮-影响行数-" + ((int)num), JsonConvert.SerializeObject(list2), LogType.系统管理, "", "", "", "", "", "", "", "");
            return "保存成功!";
        }

        [Validate]
        public IActionResult Tree()
        {
            base.ViewData["rootId"]= new Dictionary().GetIdByCode("system_applibrarytype");
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            return this.View();
        }
    }


   






    

}
