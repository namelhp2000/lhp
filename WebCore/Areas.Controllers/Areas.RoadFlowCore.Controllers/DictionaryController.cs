using System;
using System.Collections.Generic;
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
    public class DictionaryController : Controller
    {
        // Methods
        [Validate]
        public IActionResult Body()
        {
            Guid guid;
            string str = base.Request.Querys("id");
            string str2 = base.Request.Querys("parentid");
            RoadFlow.Model.Dictionary dictionary = null;
            Dictionary dictionary2 = new Dictionary();
            if (StringExtensions.IsGuid(str, out guid))
            {
                dictionary = dictionary2.Get(guid);
            }
            if (dictionary == null)
            {
                RoadFlow.Model.Dictionary dictionary1 = new RoadFlow.Model.Dictionary();
                dictionary1.Id=Guid.NewGuid();
                dictionary1.ParentId=StringExtensions.ToGuid(str2);
                dictionary1.Sort = dictionary2.GetMaxSort(StringExtensions.ToGuid(str2));
                dictionary = dictionary1;
            }
            base.ViewData["id"]= str.IsNullOrWhiteSpace() ? "" : str;
            base.ViewData["query"]= base.Request.UrlQuery();
            base.ViewData["query1"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            base.ViewData["refreshId"]= dictionary.ParentId;
            return this.View(dictionary);
        }

        public string CheckCode()
        {
            string str = base.Request.Querys("id");
            string str2 = base.Request.Forms("value");
            if (!str2.IsNullOrEmpty())
            {
                Guid guid;
                if (!StringExtensions.IsGuid(str, out guid))
                {
                    return "id错误";
                }
                if (!new Dictionary().CheckCode(guid, str2))
                {
                    return "唯一代码重复";
                }
            }
            return "1";
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [Validate, ValidateAntiForgeryToken]
        public string DeleteBody()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("id"), out guid))
            {
                return "Id错误";
            }
            if (guid == new Dictionary().GetRootId())
            {
                return "请勿删除根字典!";
            }
            if (guid == StringExtensions.ToGuid("ed6f44b8-a3bc-4743-9fae-c3607406f88f"))
            {
                return "请勿删除系统字典!";
            }
            List<RoadFlow.Model.Dictionary> list = new Dictionary().Delete(guid);
            Log.Add("删除了数据字典", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return ("共删除了" + ((int)list.Count) + "条记录");
        }

        [Validate]
        public IActionResult Index()
        {
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            base.ViewData["rootId"]= new Dictionary().GetRootId();
            return this.View();
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveBody(RoadFlow.Model.Dictionary dictionaryModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            Dictionary dictionary = new Dictionary();
            if (StringExtensions.IsGuid(base.Request.Querys("id"), out guid))
            {
                RoadFlow.Model.Dictionary dictionary2 = dictionary.Get(guid);
                string oldContents = (dictionary2 == null) ? "" : dictionary2.ToString();
                dictionary.Update(dictionaryModel);
                Log.Add("修改了数据字典-" + dictionaryModel.Title, "", LogType.系统管理, oldContents, dictionaryModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                dictionary.Add(dictionaryModel);
                Log.Add("添加了数据字典-" + dictionaryModel.Title, dictionaryModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveSort()
        {
            string str = base.Request.Forms("sort");
            Dictionary dictionary = new Dictionary();
            int num = 0;
            List<RoadFlow.Model.Dictionary> list = new List<RoadFlow.Model.Dictionary>();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.Dictionary dictionary2 = dictionary.Get(guid);
                    if (dictionary2 != null)
                    {
                        dictionary2.Sort = num += 5;
                        list.Add(dictionary2);
                    }
                }
            }
            dictionary.Update(list.ToArray());
            return "保存成功!";
        }

        [Validate]
        public IActionResult Sort()
        {
            Guid guid;
            string str = base.Request.Querys("id");
            Dictionary dictionary = new Dictionary();
            base.ViewData["queryString"]= base.Request.UrlQuery();
            if (StringExtensions.IsGuid(str, out guid))
            {
                RoadFlow.Model.Dictionary dictionary2 = dictionary.Get(guid);
                List<RoadFlow.Model.Dictionary> childs = dictionary.GetChilds(dictionary2.ParentId);
                base.ViewData["refreshId"]= dictionary2.ParentId;
                return this.View(childs);
            }
            ContentResult result1 = new ContentResult();
            result1.Content="没有找到当前字典项";
            return result1;
        }

        [Validate]
        public IActionResult Tree()
        {
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            return this.View();
        }

        public string Tree1()
        {
            Guid guid2;
            string str = base.Request.Querys("root");
            string str2 = base.Request.Querys("tempitem");
            string str3 = base.Request.Querys("tempitemid");
            Dictionary dictionary = new Dictionary();
            Guid guid = StringExtensions.IsGuid(str, out guid2) ? guid2 : dictionary.GetRootId();
            RoadFlow.Model.Dictionary dictionary2 = dictionary.Get(guid);
            if (dictionary2 == null)
            {
                return "[]";
            }
            List<RoadFlow.Model.Dictionary> childs = dictionary.GetChilds(guid);
            JArray array = new JArray();
            JObject obj1 = new JObject();
            obj1.Add("id", (JToken)dictionary2.Id);
            obj1.Add("parentID", (JToken)dictionary2.ParentId);
            obj1.Add("title", (dictionary2.Status == 1) ? ((JToken)("<span style='color:#999'>" + dictionary2.Title + "[作废]</span>")) : ((JToken)dictionary2.Title));
            obj1.Add("type", (childs.Count > 0) ? "0" : "2");
            obj1.Add("ico", "fa-briefcase");
            obj1.Add("hasChilds", (JToken)childs.Count);
            JObject obj2 = obj1;
            JArray array2 = new JArray();
            foreach (RoadFlow.Model.Dictionary dictionary3 in childs)
            {
                JObject obj5 = new JObject();
                obj5.Add("id", (JToken)dictionary3.Id);
                obj5.Add("parentID", (JToken)dictionary3.ParentId);
                obj5.Add("title", (dictionary3.Status == 1) ? ((JToken)("<span style='color:#999'>" + dictionary3.Title + "[作废]</span>")) : ((JToken)dictionary3.Title));
                obj5.Add("type", "2");
                obj5.Add("ico", "");
                obj5.Add("hasChilds", dictionary.HasChilds(dictionary3.Id) ? 1 : 0);
                obj5.Add("childs", new JArray());
                JObject obj3 = obj5;
                array2.Add(obj3);
            }
            if (!str2.IsNullOrWhiteSpace() && !str3.IsNullOrWhiteSpace())
            {
                JObject obj6 = new JObject();
                obj6.Add("id", (JToken)str3);
                obj6.Add("parentID", (JToken)dictionary2.Id);
                obj6.Add("title", (JToken)str2);
                obj6.Add("type", "2");
                obj6.Add("ico", "");
                obj6.Add("hasChilds", 0);
                obj6.Add("childs", new JArray());
                JObject obj4 = obj6;
                array2.Add(obj4);
            }
            obj2.Add("childs", array2);
            array.Add(obj2);
            return array.ToString();
        }




        public string TreeRefresh()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("refreshid"), out guid))
            {
                return "[]";
            }
            Dictionary dictionary = new Dictionary();
            List<RoadFlow.Model.Dictionary> childs = dictionary.GetChilds(guid);
            JArray array = new JArray();
            foreach (RoadFlow.Model.Dictionary dictionary2 in childs)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)dictionary2.Id);
                obj1.Add("parentID", (JToken)dictionary2.ParentId);
                obj1.Add("title", (dictionary2.Status == 1) ? ((JToken)("<span style='color:#999'>" + dictionary2.Title + "[作废]</span>")) : ((JToken)dictionary2.Title));
                obj1.Add("type", "2");
                obj1.Add("ico", "");
                obj1.Add("hasChilds", dictionary.HasChilds(dictionary2.Id) ? 1 : 0);
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            return array.ToString();
        }

        public void Export()
        {
            string exportString = new RoadFlow.Business.Dictionary().GetExportString(base.Request.Querys("id"));
            byte[] bytes = Encoding.UTF8.GetBytes(exportString);
            base.Response.Headers.Add("Server-FileName", "dictionary.json");
            base.Response.ContentType="application/octet-stream";
            base.Response.Headers.Add("Content-Disposition", "attachment; filename=dictionary.json");
            int length = bytes.Length;
            base.Response.Headers.Add("Content-Length", (StringValues)((int)length).ToString());
            base.Response.Body.Write(bytes);
            base.Response.Body.Flush();
        }


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
            Dictionary dictionary = new Dictionary();
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
                    string str2 = dictionary.Import(json);
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


    }

}
