using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{

   

    

    [Area("RoadFlowCore")]
    public class FormDesignerPluginController : Controller
    {
        // Methods
        [Validate(CheckApp = false)]
        public IActionResult Attribute()
        {
            base.ViewData["userId"]= "u_" + Current.UserId.ToString();
            base.ViewData["dbconnOptions"]= new RoadFlow.Business.DbConnection().GetOptions("");
            base.ViewData["formTypeOptions"]= new RoadFlow.Business.Dictionary().GetOptionsByCode("system_applibrarytype_form", ValueField.Id, "", true);
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Button()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Checkbox()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult DataTable()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Datetime()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Events()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Files()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Hidden()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Html()
        {
            return this.View();
        }
        [Validate(CheckApp = false)]
        public IActionResult Scripts()
        {
            return this.View();
        }


        [Validate(CheckApp = false)]
        public IActionResult Label()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Lrselect()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Organize()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public string PublishForm()
        {
            Guid guid;
            Guid guid2;
            string str = base.Request.Forms("attr");
            string str2 = base.Request.Forms("event");
            string str3 = base.Request.Forms("subtable");
            string str4 = base.Request.Forms("html");
            string str5 = base.Request.Forms("formHtml");
            JObject obj2 = null;
            try
            {
                obj2 = JObject.Parse(str);
            }
            catch
            {
                return "属性JSON解析错误!";
            }
            string str6 = obj2.Value<string>("id");
            string str7 = obj2.Value<string>("name");
            string str8 = obj2.Value<string>("formType");
            string str9 = obj2.Value<string>("manageUser");

            if (!StringExtensions.IsGuid(str6, out guid))
            {
                return "表单ID不能为空!";
            }
            if (str7.IsNullOrWhiteSpace())
            {
                return "表单名称为空,请在表单属性中填写名称!";
            }
            if (!StringExtensions.IsGuid(str8, out guid2))
            {
                return "表单分类不能为空,请在表单属性中选择分类!";
            }

            if (str9.IsNullOrWhiteSpace())
            {
                str9 = "u_" + Current.UserId.ToString();
            }



            RoadFlow.Business.Form form = new RoadFlow.Business.Form();
            RoadFlow.Model.Form form2 = form.Get(guid);
            bool flag = false;
            if (form2 == null)
            {
                RoadFlow.Model.Form form1 = new RoadFlow.Model.Form();
                form1.Id=guid;
                form1.Status = 0;
                form1.CreateDate= DateTimeExtensions.Now;
                form1.CreateUserId=Current.UserId;
                form1.CreateUserName = Current.UserName;
                form2 = form1;
                flag = true;
            }
            form2.Name = str7.Trim();
            form2.FormType=guid2;
            form2.EventJSON = str2;
            form2.SubtableJSON = str3;
            form2.attribute = str;
            form2.Html = str4;
            form2.EditDate= DateTimeExtensions.Now;
            form2.Status = 1;
            form2.RunHtml = str5;
            form2.ManageUser = str9.ToLower();

            int num = flag ? form.Add(form2) : form.Update(form2);
            string path = Current.ContentRootPath + "/Areas/RoadFlowCore/Views/FormDesigner/form/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Stream stream = (Stream)System.IO.File.Open(path + form2.Id + ".cshtml", (FileMode)FileMode.OpenOrCreate, (FileAccess)FileAccess.ReadWrite, (FileShare)FileShare.None);
            stream.SetLength(0L);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(str5);
            writer.Close();
            stream.Close();
            RoadFlow.Business.AppLibrary library = new RoadFlow.Business.AppLibrary();
            RoadFlow.Model.AppLibrary byCode = library.GetByCode(form2.Id.ToString());
            bool flag2 = false;
            if (byCode == null)
            {
                flag2 = true;
                RoadFlow.Model.AppLibrary library1 = new RoadFlow.Model.AppLibrary();
                library1.Id=Guid.NewGuid();
                library1.Code = form2.Id.ToString();
                byCode = library1;
            }
            byCode.Title = form2.Name;
            byCode.Type=form2.FormType;
            byCode.Address = form2.Id.ToString() + ".rfhtml";
            int num2 = flag2 ? library.Add(byCode) : library.Update(byCode);
            RoadFlow.Business.Log.Add("发布了表单-" + str7, form2.ToString(), LogType.流程管理, "", "", str5, "", "", "", "", "");
            return "发布成功!";
        }

        [Validate(CheckApp = false)]
        public IActionResult Radio()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public string SaveForm()
        {
            Guid guid;
            Guid guid2;
            string str = base.Request.Forms("attr");
            string str2 = base.Request.Forms("event");
            string str3 = base.Request.Forms("subtable");
            string str4 = base.Request.Forms("html");
            JObject obj2 = null;
            try
            {
                obj2 = JObject.Parse(str);
            }
            catch
            {
                return "属性JSON解析错误!";
            }
            string str5 = obj2.Value<string>("id");
            string str6 = obj2.Value<string>("name");
            string str7 = obj2.Value<string>("formType");
            string str8 = obj2.Value<string>("manageUser");

            if (!StringExtensions.IsGuid(str5, out guid))
            {
                return "表单ID不能为空!";
            }
            if (str6.IsNullOrWhiteSpace())
            {
                return "表单名称为空,请在表单属性中填写名称!";
            }
            if (!StringExtensions.IsGuid(str7, out guid2))
            {
                return "表单分类不能为空,请在表单属性中选择分类!";
            }
            if (str8.IsNullOrWhiteSpace())
            {
                str8 = "u_" + Current.UserId.ToString();
            }

            RoadFlow.Business.Form form = new RoadFlow.Business.Form();
            RoadFlow.Model.Form form2 = form.Get(guid);
            bool flag = false;
            if (form2 == null)
            {
                RoadFlow.Model.Form form1 = new RoadFlow.Model.Form();
                form1.Id=guid;
                form1.Status = 0;
                form1.CreateDate= DateTimeExtensions.Now;
                form1.CreateUserId=Current.UserId;
                form1.CreateUserName = Current.UserName;
                form2 = form1;
                flag = true;
            }
            form2.Name = str6.Trim();
            form2.FormType=guid2;
            form2.EventJSON = str2;
            form2.SubtableJSON = str3;
            form2.attribute = str;
            form2.Html = str4;
            form2.EditDate= DateTimeExtensions.Now;
            form2.ManageUser = str8.ToLower();

            int num = flag ? form.Add(form2) : form.Update(form2);
            RoadFlow.Business.Log.Add("保存了表单-" + str6, form2.ToString(), LogType.流程管理, "", "", "", "", "", "", "", "");
            return "保存成功!";
        }

        [Validate(CheckApp = false)]
        public IActionResult Select()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult SelectDiv()
        {
            base.ViewData["formTypes"]= new RoadFlow.Business.Dictionary().GetOptionsByCode("system_applibrarytype", ValueField.Id, "", true);
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult SerialNumber()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Signature()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult SubTable()
        {
            base.ViewData["appid"]= base.Request.Querys("appid");
            base.ViewData["formTypes"]= new RoadFlow.Business.Dictionary().GetOptionsByCode("system_applibrarytype_form", ValueField.Id, "", true);
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult SubtableSet()
        {
            base.ViewData["eid"]= base.Request.Querys("eid");
            base.ViewData["dbconn"]= base.Request.Querys("dbconn");
            base.ViewData["secondtable"]=base.Request.Querys("secondtable");
            base.ViewData["connOptions"]= new RoadFlow.Business.DbConnection().GetOptions(base.Request.Querys("dbconn"));
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Text()
        {
            return this.View();
        }

        [Validate(CheckApp = false)]
        public IActionResult Textarea()
        {
            return this.View();
        }
    }

    



 

}
