using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{


    [Area("RoadFlowCore")]
    public class ProgramDesignerController : Controller
    {
        // Methods
        [Validate]
        public string Copy_Export()
        {
            string str = base.Request.Querys("programid");
            List<RoadFlow.Model.ProgramField> all = new ProgramField().GetAll(StringExtensions.ToGuid(str));
            if (all.Count == 0)
            {
                return "没有要复制的字段!";
            }
            List<RoadFlow.Model.ProgramExport> list2 = new List<RoadFlow.Model.ProgramExport>();
            ProgramExport export = new ProgramExport();
            foreach (RoadFlow.Model.ProgramField field in all)
            {
                if (!field.Field.IsNullOrWhiteSpace())
                {
                    RoadFlow.Model.ProgramExport export2 = new RoadFlow.Model.ProgramExport
                    {
                        Align = field.Align,
                        CustomString = field.CustomString
                    };
                    export2.DataType=0;
                    export2.Field = field.Field;
                    export2.Id=Guid.NewGuid();
                    export2.ProgramId=field.ProgramId;
                    export2.ShowFormat = field.ShowFormat;
                    export2.ShowTitle = field.ShowTitle;
                    export2.ShowType=new int?(field.ShowType);
                    export2.Sort = field.Sort;
                    list2.Add(export2);
                }
            }
            export.DeleteAndAdd(list2.ToArray());
            return "复制成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            string str = base.Request.Forms("ids");
            RoadFlow.Business.Program program = new RoadFlow.Business.Program();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.Program program2 = program.Get(guid);
                    if (program2 != null)
                    {
                        program2.Status = 2;
                        program.Update(program2);
                    }
                }
            }
            return "删除成功!";
        }

        [Validate]
        public string Delete_Export()
        {
            string[] strArray = (base.Request.Forms("ids") ?? "").Split(',', (StringSplitOptions)StringSplitOptions.None);
            List<RoadFlow.Model.ProgramExport> list = new List<RoadFlow.Model.ProgramExport>();
            ProgramExport export = new ProgramExport();
            foreach (string str in strArray)
            {
                Guid guid;
                if (StringExtensions.IsGuid(str, out guid))
                {
                    RoadFlow.Model.ProgramExport export2 = export.Get(guid);
                    if (export2 != null)
                    {
                        list.Add(export2);
                    }
                }
            }
            export.Delete(list.ToArray());
            Log.Add("删除了应用程序设计导出", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate]
        public string Delete_Set_Button()
        {
            ProgramButton button = new ProgramButton();
            string str = base.Request.Forms("ids") ?? "";
            List<RoadFlow.Model.ProgramButton> list = new List<RoadFlow.Model.ProgramButton>();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.ProgramButton button2 = button.Get(guid);
                    if (button2 != null)
                    {
                        list.Add(button2);
                    }
                }
            }
            button.Delete(list.ToArray());
            Log.Add("删除了应用程序设计按钮", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate]
        public string Delete_Set_Query()
        {
            ProgramQuery query = new ProgramQuery();
            string str = base.Request.Forms("ids") ?? "";
            List<RoadFlow.Model.ProgramQuery> list = new List<RoadFlow.Model.ProgramQuery>();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.ProgramQuery query2 = query.Get(guid);
                    if (query2 != null)
                    {
                        list.Add(query2);
                    }
                }
            }
            query.Delete(list.ToArray());
            Log.Add("删除了应用程序设计查询", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate]
        public string DeleteSet_ListField()
        {
            string str = base.Request.Forms("ids") ?? "";
            List<RoadFlow.Model.ProgramField> list = new List<RoadFlow.Model.ProgramField>();
            ProgramField field = new ProgramField();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                RoadFlow.Model.ProgramField field2 = field.Get(StringExtensions.ToGuid(str2));
                if (field2 != null)
                {
                    list.Add(field2);
                }
            }
            field.Delete(list.ToArray());
            Log.Add("删除了应用程序设计字段", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate]
        public IActionResult Edit()
        {
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
        public IActionResult List()
        {
            string str = base.Request.Querys("appid");
            string str2 = base.Request.Querys("tabid");
            string str3 = base.Request.Querys("typeid");
            base.ViewData["appId"]= str;
            base.ViewData["tabId"]= str2;
            base.ViewData["typeId"]= str3;
            string[] textArray1 = new string[] { "appid=", str, "&tabid=", str2, "&typeid=", str3 };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View();
        }

        [Validate]
        public string Publish()
        {
            Guid guid;
            string str = base.Request.Querys("programid");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return "ID错误!";
            }
            string contents = new RoadFlow.Business.Program().Publish(guid,null);
            Log.Add("发布了程序设计-" + str, contents, LogType.系统管理, "", "", "", "", "", "", "", "");
            if (!"1".Equals(contents))
            {
                return contents;
            }
            return "发布成功!";
        }


        public string PublishBatch()
        {
            string str = base.Request.Forms("ids");
            RoadFlow.Business.Program program = new RoadFlow.Business.Program();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    string contents = program.Publish(guid,null);
                    Log.Add("发布了程序设计-" + str, contents, LogType.系统管理, "", "", "", "", "", "", "", "");
                    if (!"1".Equals(contents))
                    {
                        return contents;
                    }
                }
            }
            return "批量发布成功!";
        }




        [Validate]
        public string Query()
        {
            Guid guid;
            int num3;
            string str = base.Request.Forms("sidx");
            string str2 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str3 = (str.IsNullOrEmpty() ? "CreateTime" : str) + " " + (str2.IsNullOrEmpty() ? "ASC" : str2);
            string str4 = base.Request.Forms("Name");
            string str5 = base.Request.Querys("typeid");
            string str6 = string.Empty;
            if (StringExtensions.IsGuid(str5, out guid))
            {
                str6 = StringExtensions.JoinSqlIn<Guid>(new RoadFlow.Business.Dictionary().GetAllChildsId(guid, true), true);
            }
            DataTable table = new RoadFlow.Business.Program().GetPagerData(out num3, pageSize, pageNumber, str4, str6, str3);
            JArray array = new JArray();
            RoadFlow.Business.Dictionary dictionary = new RoadFlow.Business.Dictionary();
            User user = new User();
            foreach (DataRow row in table.Rows)
            {
               //if(row["Status"].ToString()!="2")
               // {
                    JObject obj1 = new JObject();
                    obj1.Add("id", (JToken)row["Id"].ToString());
                    obj1.Add("Name", (JToken)row["Name"].ToString());
                    obj1.Add("Type", (JToken)dictionary.GetTitle(StringExtensions.ToGuid(row["Type"].ToString())));
                    obj1.Add("CreateTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["CreateTime"].ToString())));
                    obj1.Add("CreateUserId", (JToken)user.GetName(StringExtensions.ToGuid(row["CreateUserId"].ToString())));
                    obj1.Add("Status", (row["Status"].ToString().ToInt(-2147483648) == 0) ? "设计中" : "已发布");
                    obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + row["Id"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                    JObject obj2 = obj1;
                    array.Add(obj2);
                //}
               //else
               // {
               //     num3--;
               // }
            
        }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);

        }

        [Validate(CheckApp = false)]
        public IActionResult Run()
        {
            Guid guid;
            Guid guid2;
            Guid guid3;
            string str = base.Request.Querys("programid");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                ContentResult result1 = new ContentResult();
                result1.Content="ID错误!";
                return result1;
            }
            RoadFlow.Business.Program program = new RoadFlow.Business.Program();
            RoadFlow.Model.ProgramRun runModel = program.GetRunModel(guid);
            if (runModel == null)
            {
                ContentResult result2 = new ContentResult();
                result2.Content="未找到运行时实体，程序未发布!";
                return result2;
            }
            string str2 = base.Request.Querys("appid");
            List<string> list = program.GetButtonHtml(runModel, Current.UserId, StringExtensions.IsGuid(str2, out guid2) ? guid2 : Guid.Empty,null);
            runModel.Button_Toolbar = list[0];
            runModel.Button_Normal = list[1];
            runModel.Button_List = list[2];
            string str3 = string.Empty;
            string str4 = runModel.EditModel.ToString();
            string number = runModel.Width.GetNumber();
            string str6 = runModel.Height.GetNumber();
            if (StringExtensions.IsGuid(runModel.FormId, out guid3))
            {
                RoadFlow.Model.AppLibrary library = new AppLibrary().Get(guid3);
                if (library != null)
                {
                    str3 = "/RoadFlowCore/FlowRun/FormEdit?applibraryid=" + library.Id.ToString();
                }
            }
            base.ViewData["edit_url"]= str3;
            base.ViewData["edit_model"]= str4;
            base.ViewData["edit_width"]= number;
            base.ViewData["edit_height"]= str6;
            base.ViewData["edit_title"]= runModel.Name;
            base.ViewData["pagesize"]= base.Request.Querys("pagesize");
            base.ViewData["pagenumber"]= base.Request.Querys("pagenumber");
            string[] textArray1 = new string[] { "/RoadFlowCore/ProgramDesigner/Run?programid=", str, "&appid=", base.Request.Querys("appid"), "&rf_appopenmodel=", base.Request.Querys("rf_appopenmodel"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["backUrl"]= string.Concat((string[])textArray1);
            base.ViewData["tabid"]= base.Request.Querys("tabid");
            base.ViewData["isToolbar"]= runModel.Button_Toolbar.IsNullOrWhiteSpace() ? "0" : "1";
            base.ViewData["isQuery"]= (runModel.ProgramQueries.Count > 0) ? "1" : "0";
            base.ViewData["isQueryNewtr"]= (runModel.ButtonLocation == 0) ? "1" : "0";
            string[] textArray2 = new string[] { "programid=", str, "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"]= string.Concat((string[])textArray2);
            base.ViewData["eid"]= base.Request.Querys("eid");
            base.ViewData["values"]= base.Request.Querys("values");
            base.ViewData["pkfield"]= base.Request.Querys("pkfield");
            base.ViewData["titlefield"]= base.Request.Querys("titlefield");
            base.ViewData["GroupHeaders"]= runModel.GroupHeaders;

            //分组 

            base.ViewData["GroupField"] = runModel.GridGroups_String["GroupField"].ToJson();
            base.ViewData["GroupText"] = runModel.GridGroups_String["GroupText"].ToJson();
            base.ViewData["GroupOrder"] = runModel.GridGroups_String["GroupOrder"].ToJson();


            base.ViewData["GroupSummary"] = runModel.GridGroups_Bool["GroupSummary"].ToJson();
            base.ViewData["GroupColumnShow"] = runModel.GridGroups_Bool["GroupColumnShow"].ToJson();
            base.ViewData["GroupCollapse"] = runModel.GridGroups_Bool["GroupCollapse"].ToJson();
            base.ViewData["GroupDataSorted"] = runModel.GridGroups_Bool["GroupDataSorted"].ToJson();
            base.ViewData["ShowSummaryOnHide"] = runModel.GridGroups_Bool["ShowSummaryOnHide"].ToJson();










            return this.View(runModel);
        }



       



        //修改过
        [Validate(CheckApp = false), ValidateAntiForgeryToken]
        public string Run_Delete()
        {
            Guid guid;
            Guid guid2;
            Guid guid3;
            if (!StringExtensions.IsGuid(base.Request.Querys("programid"), out guid))
            {
                return "{\"success\":0,\"msg\":\"应用程序ID错误\"}";

            }
            RoadFlow.Model.ProgramRun runModel = new RoadFlow.Business.Program().GetRunModel(guid,null);
            if (runModel == null)
            {
                return "{\"success\":0,\"msg\":\"未找到程序运行时实体\"}";

            }
            if (!StringExtensions.IsGuid(runModel.FormId, out guid2))
            {
                return "{\"success\":0,\"msg\":\"没有设置表单,不能删除\"}";

            }
            RoadFlow.Model.AppLibrary library = new AppLibrary().Get(guid2);
            if (library == null)
            {
                return "{\"success\":0,\"msg\":\"没有找到对应的应用程序库\"}";

            }
            if (!StringExtensions.IsGuid(library.Code, out guid3))
            {
                return "{\"success\":0,\"msg\":\"未找到表单Id\"}";

            }
            string str = base.Request.Forms("delid");
            if (str.IsNullOrWhiteSpace())
            {
                return "{\"success\":0,\"msg\":\"要删除的Id为空\"}";

            }
            List<string> list = new List<string>();
            foreach (string str3 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                list.Add(new Form().DeleteFormData(guid3, str3));
            }
            object[] objArray1 = new object[] { "删除了表单数据 - 应用名称：", runModel.Name, " - 应用ID：", runModel.Id };
            Log.Add(string.Concat((object[])objArray1), JsonConvert.SerializeObject(list), LogType.其它, "", "", "ID：" + str, "", "", "", "", "");
            if ((list.Count > 0) && "1".Equals(Enumerable.First<string>((IEnumerable<string>)list)))
            {
                return "{\"success\":1,\"msg\":\"删除成功\"}";
            }
            return ("{\"success\":0,\"msg\":\"" + ((list.Count > 0) ? Enumerable.First<string>((IEnumerable<string>)list) : "未返回") + "\"}");
        }

        [Validate(CheckApp = false)]
        public void Run_Export()
        {
            Guid guid;
            if (StringExtensions.IsGuid(base.Request.Querys("programid"), out guid))
            {
                RoadFlow.Business.Program program = new RoadFlow.Business.Program();
                RoadFlow.Model.ProgramRun runModel = program.GetRunModel(guid,null);
                if (runModel != null)
                {
                    NPOIHelper.ExportByWeb(program.GetExportData(runModel, base.Request), runModel.ExportHeaderText, runModel.ExportFileName.IsNullOrWhiteSpace() ? (runModel.Name + ".xlsx") : runModel.ExportFileName, base.Response, runModel.ExportTemplate);
                }
            }
        }

        [Validate(CheckApp = false)]
        public string Run_Query()
        {
            Guid guid;
            int num3;
            if (!StringExtensions.IsGuid(base.Request.Querys("programid"), out guid))
            {
                return "ID错误";
            }
            RoadFlow.Business.Program program = new RoadFlow.Business.Program();
            RoadFlow.Model.ProgramRun runModel = program.GetRunModel(guid,null);
            if (runModel == null)
            {
                return "未找到程序运行时实体";
            }
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            DataTable table = program.GetData(runModel, base.Request, pageSize, pageNumber, out num3);
            JArray array = new JArray();
            RoadFlow.Model.User user = Current.User;
            int num4 = 0;

            foreach (DataRow row in table.Rows)
            {
                JObject obj2 = new JObject();
                if (table.Columns.Contains("id"))
                {
                    obj2.Add("id", (JToken)row["id"].ToString());
                }
                foreach (RoadFlow.Model.ProgramField field in runModel.ProgramFields)
                {
                    string str = string.Empty;
                    if (field.Field.IsNullOrWhiteSpace())
                    {
                        if (100 == field.ShowType)
                        {
                            str = Wildcard.Filter(runModel.Button_List, user, row);
                            obj2.Add("opation", (JToken)str);
                        }
                        else if (1 == field.ShowType)//修改过
                        {
                            int num5 = (((pageNumber - 1) * pageSize) + 1) + num4;
                            str = ((int)num5).ToString();
                            obj2.Add("rowserialnumber", (JToken)str);
                        }

                        continue;
                    }
                    str = table.Columns.Contains(field.Field) ? row[field.Field].ToString() : "";
                    if (((field.ShowType != 0) && (field.ShowType != 100)) && !str.IsNullOrWhiteSpace())
                    {
                        switch (field.ShowType)
                        {
                            case 2:
                                DateTime time;
                                if (StringExtensions.IsDateTime(str, out time))
                                {
                                    str = time.ToString(field.ShowFormat.IsNullOrWhiteSpace() ? "yyyy-MM-dd HH:mm:ss" : field.ShowFormat);
                                }
                                break;

                            case 3:
                                if (!field.ShowFormat.IsNullOrWhiteSpace())
                                {
                                    str = StringExtensions.ToDecimal(str, (decimal)-79228162514264337593543950335M).ToString(field.ShowFormat);
                                }
                                break;

                            case 4:
                                str = new Dictionary().GetTitles(str);
                                break;

                            case 5:
                                str = new Organize().GetNames(str);
                                break;

                            case 6:
                                str = Wildcard.Filter(field.CustomString, Current.User, row);
                                break;

                            case 7:
                                str = new User().GetNames(str);
                                break;
                        }
                    }
                    obj2.Add(field.Field, (JToken)str);
                }
                num4++;

                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate, ValidateAntiForgeryToken]
        public string Save_Export(RoadFlow.Model.ProgramExport programExportModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            ProgramExport export = new ProgramExport();
            if (StringExtensions.IsGuid(base.Request.Querys("exportid"), out guid))
            {
                RoadFlow.Model.ProgramExport export2 = export.Get(guid);
                string oldContents = (export2 == null) ? "" : export2.ToString();
                export.Update(programExportModel);
                Log.Add("修改了应用程序设计导出-" + programExportModel.Field, "", LogType.系统管理, oldContents, programExportModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                export.Add(programExportModel);
                Log.Add("添加了应用程序设计导出-" + programExportModel.Field, programExportModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveAttr(RoadFlow.Model.Program programModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            RoadFlow.Business.Program program = new RoadFlow.Business.Program();
            if (StringExtensions.IsGuid(base.Request.Querys("programid"), out guid))
            {
                RoadFlow.Model.Program program2 = program.Get(guid);
                string oldContents = (program2 == null) ? "" : program2.ToString();
                program.Update(programModel);
                Log.Add("修改了应用程序设计属性-" + programModel.Name, "", LogType.系统管理, oldContents, programModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                program.Add(programModel);
                Log.Add("添加了应用程序设计属性-" + programModel.Name, programModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveSet_Button(RoadFlow.Model.ProgramButton programButtonModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            ProgramButton button = new ProgramButton();
            if (StringExtensions.IsGuid(base.Request.Querys("buttonid"), out guid))
            {
                RoadFlow.Model.ProgramButton button2 = button.Get(guid);
                string oldContents = (button2 == null) ? "" : button2.ToString();
                button.Update(programButtonModel);
                Log.Add("修改了应用程序设计按钮-" + programButtonModel.ButtonName, "", LogType.系统管理, oldContents, programButtonModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                button.Add(programButtonModel);
                Log.Add("添加了应用程序设计按钮-" + programButtonModel.ButtonName, programButtonModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveSet_ListField(RoadFlow.Model.ProgramField programFieldModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            ProgramField field = new ProgramField();
           

            if (StringExtensions.IsGuid(base.Request.Querys("fieldid"), out guid))
            {
                RoadFlow.Model.ProgramField field2 = field.Get(guid);
                string oldContents = (field2 == null) ? "" : field2.ToString();
                field.Update(programFieldModel);
                Log.Add("修改了应用程序设计字段-" + programFieldModel.Field, "", LogType.系统管理, oldContents, programFieldModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                //重复显示字段设置，导致错误
                if (field.GetAll(base.Request.Querys("programid").ToGuid()).FindAll(x => x.Field == programFieldModel.Field).Count > 0 && !programFieldModel.Field.IsNullOrEmpty())
                {
                    return "字段重复，请重新设置！";
                }
                field.Add(programFieldModel);
                Log.Add("添加了应用程序设计字段-" + programFieldModel.Field, programFieldModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveSet_Query(RoadFlow.Model.ProgramQuery programQueryModel)
        {
            string QueryField = base.Request.Forms("QueryField");
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            int? nullable = programQueryModel.DataSource;
          

            if (!QueryField.IsNullOrWhiteSpace()&& QueryField!= programQueryModel.Field)
            {
                programQueryModel.Field = QueryField;
            }
            int num = 1;
            if ((nullable.GetValueOrDefault() == num) & nullable.HasValue)
            {
                programQueryModel.DictValue = base.Request.Forms("DataSourceString_dict");
            }
            if (programQueryModel.InputType == 6)
            {
                string str = base.Request.Forms("DataSource_Organize_Range");
                string str2 = base.Request.Forms("DataSource_Organize_Type_Unit");
                string str3 = base.Request.Forms("DataSource_Organize_Type_Dept");
                string str4 = base.Request.Forms("DataSource_Organize_Type_Station");
                string str5 = base.Request.Forms("DataSource_Organize_Type_Role");
                string str6 = base.Request.Forms("DataSource_Organize_Type_User");
                string str7 = base.Request.Forms("DataSource_Organize_Type_More");
               
                string[] textArray1 = new string[] { "{\"unit\":\"", str2, "\",\"range\":\"", str,"\",\"dept\":\"", str3, "\",\"role\":\"", str5, "\",\"user\":\"", str6, "\",\"more\":\"", str7, "\",\"station\":\"", str4, "\"}" };
                string str8 = string.Concat((string[])textArray1);
                programQueryModel.OrgAttribute = str8;
            }
            else if (programQueryModel.InputType == 5)
            {
                string str9 = base.Request.Forms("DictValueField");
                programQueryModel.OrgAttribute = str9;
            }


            ProgramQuery query = new ProgramQuery();
            if (StringExtensions.IsGuid(base.Request.Querys("queryid"), out guid))
            {
                RoadFlow.Model.ProgramQuery query2 = query.Get(guid);
                string oldContents = (query2 == null) ? "" : query2.ToString();
                query.Update(programQueryModel);
                Log.Add("修改了应用程序设计查询-" + programQueryModel.Field, "", LogType.系统管理, oldContents, programQueryModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                query.Add(programQueryModel);
                Log.Add("添加了应用程序设计查询-" + programQueryModel.Field, programQueryModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveSet_Validate()
        {
            string[] strArray = (base.Request.Forms("filedindex") ?? "").Split(',', (StringSplitOptions)StringSplitOptions.None);
            ProgramValidate validate = new ProgramValidate();
            Guid guid = StringExtensions.ToGuid(base.Request.Querys("programid"));
            if (GuidExtensions.IsEmptyGuid(guid))
            {
                return "程序ID错误!";
            }
            List<RoadFlow.Model.ProgramValidate> list = new List<RoadFlow.Model.ProgramValidate>();
            foreach (string str in strArray)
            {
                string str2 = base.Request.Forms("tablename_" + str);
                string str3 = base.Request.Forms("fieldname_" + str);
                string str4 = base.Request.Forms("fieldnote_" + str);
                string str5 = base.Request.Forms("valdate_" + str);
                string str6 = base.Request.Forms("status_" + str);
                RoadFlow.Model.ProgramValidate validate1 = new RoadFlow.Model.ProgramValidate
                {
                    FieldName = str3,
                    FieldNote = str4
                };
                validate1.Id=Guid.NewGuid();
                validate1.ProgramId=guid;
                validate1.TableName = str2;
                validate1.Validate = str5.ToInt(0);
                validate1.Status = str6.ToInt(0);
                RoadFlow.Model.ProgramValidate validate2 = validate1;
                list.Add(validate2);
            }
            int num = validate.Add(list.ToArray());
            return "保存成功!";
        }

        [Validate]
        public IActionResult Set_Attr()
        {
            Guid guid;
            Guid guid2;
            string str = base.Request.Querys("programid");
            string str2 = base.Request.Querys("typeid");
            RoadFlow.Model.Program program = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                program = new RoadFlow.Business.Program().Get(guid);
            }
            if (program == null)
            {
                Guid guid3;
                RoadFlow.Model.Program program1 = new RoadFlow.Model.Program();
                program1.Id=Guid.NewGuid();
                program1.CreateTime= DateTimeExtensions.Now;
                program1.CreateUserId=Current.UserId;
                program = program1;
                if (StringExtensions.IsGuid(str2, out guid3))
                {
                    program.Type=guid3;
                }
            }
            string str3 = string.Empty;
            if (StringExtensions.IsGuid(program.FormId, out guid2))
            {
                RoadFlow.Model.AppLibrary library = new AppLibrary().Get(guid2);
                if (library != null)
                {
                    str3 = library.Type.ToString();
                }
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            string[] textArray1 = new string[] { "pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber"), "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid"), "&typeid=", base.Request.Querys("typeid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            base.ViewData["appTypeOptions"]= new Dictionary().GetOptionsByCode("system_applibrarytype", ValueField.Id, program.Type.ToString(), true,true);
            base.ViewData["formTypeOptions"]= new Dictionary().GetOptionsByCode("system_applibrarytype", ValueField.Id, str3, true,true);
            base.ViewData["dbconnOptions"]= new DbConnection().GetOptions(program.ConnId.ToString());
            return this.View(program);
        }

        [Validate]
        public IActionResult Set_Button()
        {
            ProgramButton button = new ProgramButton();
            Dictionary dictionary = new Dictionary();
            JArray array = new JArray();
            foreach (RoadFlow.Model.ProgramButton button2 in button.GetAll(StringExtensions.ToGuid(base.Request.Querys("programid"))))
            {
                string str = string.Empty;
                if (!button2.Ico.IsNullOrWhiteSpace())
                {
                    if (button2.Ico.IsFontIco())
                    {
                        str = "<i class=\"fa " + button2.Ico + "\"></i>";
                    }
                    else
                    {
                        str = "<img src=\"" + button2.Ico + "\"/>";
                    }
                }
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)button2.Id);
                obj1.Add("buttonName", (JToken)button2.ButtonName);
                obj1.Add("ico", (JToken)str);
                obj1.Add("showType", (button2.ShowType == 0) ? "工具栏按钮" : ((button2.ShowType == 1) ? "普通按钮" : "列表按钮"));
                obj1.Add("isValidate", (button2.IsValidateShow == 1) ? "是" : "否");
                obj1.Add("sort", (JToken)button2.Sort);
                obj1.Add("opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + button2.Id.ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            string[] textArray1 = new string[] { "programid=", base.Request.Querys("programid"), "&pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber"), "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid"), "&typeid=", base.Request.Querys("typeid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            base.ViewData["list"]=array.ToString();
            return this.View();
        }

        [Validate]
        public IActionResult Set_Button_Edit()
        {
            Guid guid;
            string str = base.Request.Querys("buttonid");
            string str2 = base.Request.Querys("programid");
            RoadFlow.Model.Program program = new RoadFlow.Business.Program().Get(StringExtensions.ToGuid(str2));
            if (program == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content="未找到程序设计实体!";
                return result1;
            }
            ProgramButton button = new ProgramButton();
            RoadFlow.Model.ProgramButton button2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                button2 = button.Get(guid);
            }
            if (button2 == null)
            {
                RoadFlow.Model.ProgramButton button1 = new RoadFlow.Model.ProgramButton();
                button1.Id=Guid.NewGuid();
                button1.ProgramId=program.Id;
                button1.ShowType = -1;
                button1.Sort = button.GetMaxSort(program.Id);
                button2 = button1;
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["buttonOptions"]= new SystemButton().GetOptions(button2.ButtonId.ToString());
            base.ViewData["buttonJson"]= new SystemButton().GetAllJson().ToString(0, Array.Empty<JsonConverter>());
            return this.View(button2);
        }

        [Validate]
        public IActionResult Set_Export()
        {
            ProgramExport export = new ProgramExport();
            Dictionary dictionary = new Dictionary();
            JArray array = new JArray();
            foreach (RoadFlow.Model.ProgramExport export2 in export.GetAll(StringExtensions.ToGuid(base.Request.Querys("programid"))))
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)export2.Id);
                obj1.Add("fieldName", (JToken)export2.Field);
                obj1.Add("showName", export2.ShowTitle.IsNullOrEmpty() ? ((JToken)export2.Field) : ((JToken)export2.ShowTitle));
                obj1.Add("showType", (JToken)dictionary.GetTitle("programdesigner_showtype", export2.ShowType.ToString()));
                obj1.Add("dataType", (JToken)dictionary.GetTitle("programdesigner_exportdatatype", export2.DataType.ToString()));
                obj1.Add("align", (JToken)dictionary.GetTitle("programdesigner_algin", export2.Align.ToString()));
                obj1.Add("width", (JToken)export2.Width);
                obj1.Add("sort", (JToken)export2.Sort);
                obj1.Add("opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + export2.Id.ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            string[] textArray1 = new string[] { "programid=", base.Request.Querys("programid"), "&pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber"), "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid"), "&typeid=", base.Request.Querys("typeid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            base.ViewData["list"]= array.ToString();
            return this.View();
        }

        [Validate]
        public IActionResult Set_Export_Edit()
        {
            Guid guid;
            string str = base.Request.Querys("exportid");
            string str2 = base.Request.Querys("programid");
            RoadFlow.Model.Program program = new RoadFlow.Business.Program().Get(StringExtensions.ToGuid(str2));
            if (program == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content="未找到程序设计实体!";
                return result1;
            }
            ProgramExport export = new ProgramExport();
            RoadFlow.Model.ProgramExport export2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                export2 = export.Get(guid);
            }
            if (export2 == null)
            {
                RoadFlow.Model.ProgramExport export1 = new RoadFlow.Model.ProgramExport();
                export1.Id=Guid.NewGuid();
                export1.ProgramId=program.Id;
                export1.Sort = export.GetMaxSort(program.Id);
                export2 = export1;
            }
            Dictionary dictionary = new Dictionary();
            base.ViewData["fieldOptions"]= new ProgramField().GetFieldOptions(program.ConnId, program.SqlString, export2.Field, null);
            base.ViewData["dataTypeOptions"]= dictionary.GetOptionsByCode("programdesigner_exportdatatype", ValueField.Value, export2.DataType.ToString(), true,true);
            base.ViewData["showTypeOptions"]= dictionary.GetOptionsByCode("programdesigner_showtype", ValueField.Value, export2.ShowType.ToString(), true,true);
            base.ViewData["alignOptions"]= dictionary.GetOptionsByCode("programdesigner_algin", ValueField.Value, export2.Align, true,true);
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View(export2);
        }

        [Validate]
        public IActionResult Set_ListField()//改变
        {
            ProgramField field = new ProgramField();
            Dictionary dictionary = new Dictionary();
            List<RoadFlow.Model.ProgramField> all = field.GetAll(StringExtensions.ToGuid(base.Request.Querys("programid")));
            JArray array = new JArray();
            foreach (RoadFlow.Model.ProgramField field2 in all)
            {
               
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)field2.Id);
                obj1.Add("fieldName", (JToken)field2.Field);
                obj1.Add("showName", field2.ShowTitle.IsNullOrEmpty() ? ((JToken)field2.Field) : ((JToken)field2.ShowTitle));
                obj1.Add("showType", (JToken)dictionary.GetTitle("programdesigner_showtype", ((int)field2.ShowType).ToString()));
                obj1.Add("align", field2.Align.Equals("left") ? "左对齐" : (field2.Align.Equals("center") ? "居中对齐" : "右对齐"));


                obj1.Add("summaryType", (JToken)dictionary.GetTitle("programdesigner_summaryType", field2.SummaryType.IsNullOrEmpty()?"": field2.SummaryType));
                obj1.Add("formatter", (JToken)dictionary.GetTitle("programdesigner_formatter", field2.Formatter.IsNullOrEmpty()?"": field2.Formatter));



                obj1.Add("width", (JToken)field2.Width);

                obj1.Add("isFreeze", (field2.IsFreeze == 1) ? "是" : "否");

                obj1.Add("isSort", (JToken)field2.IsSort);
                obj1.Add("isDefaultSort", (field2.IsDefaultSort == 1) ? "是" : "否");

                obj1.Add("sortWay", field2.SortWay.IsNullOrEmpty()?"": ((field2.SortWay == "desc") ? "降序排序" : "<span style='color:blue'>升序排序</span>"));
                obj1.Add("sort", (JToken)field2.Sort);
                obj1.Add("opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + field2.Id.ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            string[] textArray1 = new string[] { "programid=", base.Request.Querys("programid"), "&pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber"), "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid"), "&typeid=", base.Request.Querys("typeid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            base.ViewData["list"]= array.ToString();
            return this.View();
        }

        [Validate]
        public IActionResult Set_ListField_Edit()
        {
            Guid guid;
            string str = base.Request.Querys("fieldid");
            string str2 = base.Request.Querys("programid");
            RoadFlow.Model.Program program = new RoadFlow.Business.Program().Get(StringExtensions.ToGuid(str2));
            if (program == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content="未找到程序设计实体!";
                return result1;
            }
            ProgramField field = new ProgramField();
            RoadFlow.Model.ProgramField field2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                field2 = field.Get(guid);
            }
            if (field2 == null)
            {
                RoadFlow.Model.ProgramField field1 = new RoadFlow.Model.ProgramField();
                field1.Id=Guid.NewGuid();
                field1.ProgramId=program.Id;
                field1.Sort = field.GetMaxSort(program.Id);
                field1.IsSort = string.Empty;

                field2 = field1;
            }
            Dictionary dictionary = new Dictionary();
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["fieldOptions"]= field.GetFieldOptions(program.ConnId, program.SqlString, field2.Field, null);
            base.ViewData["showTypeOptions"]= dictionary.GetOptionsByCode("programdesigner_showtype", ValueField.Value, ((int)field2.ShowType).ToString(), true,true);
            base.ViewData["alignOptions"]= dictionary.GetOptionsByCode("programdesigner_algin", ValueField.Value, field2.Align, true,true);
            base.ViewData["summaryTypeOption"] = dictionary.GetOptionsByCode("programdesigner_summaryType", ValueField.Value, field2.SummaryType, true,true);
            base.ViewData["formatterOption"] = dictionary.GetOptionsByCode("programdesigner_formatter", ValueField.Value, field2.Formatter, true,true);
            return this.View(field2);
        }

        [Validate(CheckApp = false)]
        public string Set_ListField_Options()
        {
            Guid guid;
            Guid guid2;
            if (!StringExtensions.IsGuid(base.Request.Forms("applibaryid"), out guid))
            {
                return "";
            }
            RoadFlow.Model.AppLibrary library = new AppLibrary().Get(guid);
            if (((library == null) || library.Code.IsNullOrWhiteSpace()) || !StringExtensions.IsGuid(library.Code, out guid2))
            {
                return "";
            }
            RoadFlow.Model.Program program2 = new RoadFlow.Business.Program().Get(guid2);
            if (program2 == null)
            {
                return "";
            }
            return new ProgramField().GetFieldOptions(program2.ConnId, program2.SqlString, "", null);
        }

        [Validate]
        public IActionResult Set_Query()
        {
            ProgramQuery query = new ProgramQuery();
            Dictionary dictionary = new Dictionary();
            JArray array = new JArray();
            foreach (RoadFlow.Model.ProgramQuery query2 in query.GetAll(StringExtensions.ToGuid(base.Request.Querys("programid"))))
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)query2.Id);
                obj1.Add("fieldName", (JToken)query2.Field);
                obj1.Add("showTitle", query2.ShowTitle.IsNullOrEmpty() ? ((JToken)query2.Field) : ((JToken)query2.ShowTitle));
                obj1.Add("operator", (JToken)query2.Operators);
                obj1.Add("controlName", (JToken)query2.ControlName);
                obj1.Add("inputType", (JToken)dictionary.GetTitle("programdesigner_inputtype", ((int)query2.InputType).ToString()));
                obj1.Add("sort", (JToken)query2.Sort);
                obj1.Add("opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + query2.Id.ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            string[] textArray1 = new string[] { "programid=", base.Request.Querys("programid"), "&pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber"), "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid"), "&typeid=", base.Request.Querys("typeid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            base.ViewData["list"]= array.ToString();
            return this.View();
        }

        [Validate]
        public IActionResult Set_Query_Edit()
        {
            Guid guid;
            string str = base.Request.Querys("queryid");
            string str2 = base.Request.Querys("programid");
            RoadFlow.Model.Program program = new RoadFlow.Business.Program().Get(StringExtensions.ToGuid(str2));
            if (program == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content="未找到程序设计实体!";
                return result1;
            }
            ProgramQuery query = new ProgramQuery();
            RoadFlow.Model.ProgramQuery query2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                query2 = query.Get(guid);
            }
            if (query2 == null)
            {
                RoadFlow.Model.ProgramQuery query1 = new RoadFlow.Model.ProgramQuery();
                query1.Id=Guid.NewGuid();
                query1.ProgramId=program.Id;
                query1.Sort = query.GetMaxSort(program.Id);
                query2 = query1;
            }
            Dictionary dictionary = new Dictionary();
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["fieldOptions"]= new ProgramField().GetFieldOptions(program.ConnId, program.SqlString, query2.Field, null);
            base.ViewData["controlTypeOptions"]= dictionary.GetOptionsByCode("programdesigner_inputtype", ValueField.Value, ((int)query2.InputType).ToString(), true,true);
            base.ViewData["operatorOptions"]= dictionary.GetOptionsByCode("programdesigner_operators", ValueField.Value, query2.Operators, true,true);
            base.ViewData["dataSourceOptions"]= dictionary.GetOptionsByCode("programdesigner_datasource", ValueField.Value, query2.DataSource.ToString(), true,true);
            base.ViewData["dbconnOptions"]= new DbConnection().GetOptions(query2.ConnId);
            return this.View(query2);
        }

        [Validate]
        public IActionResult Set_Validate()
        {
            Guid guid;
            Guid guid2;
            string str = base.Request.Querys("programid");
            RoadFlow.Model.Program program = new RoadFlow.Business.Program().Get(StringExtensions.ToGuid(str));
            if (program == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content="未找到程序设计实体!";
                return result1;
            }
            if (!StringExtensions.IsGuid(program.FormId, out guid))
            {
                ContentResult result2 = new ContentResult();
                result2.Content="程序未设置表单!";
                return result2;
            }
            RoadFlow.Model.AppLibrary library = new AppLibrary().Get(guid);
            if (library == null)
            {
                ContentResult result3 = new ContentResult();
                result3.Content="未在应用程序库中找到该表单!";
                return result3;
            }
            if (!StringExtensions.IsGuid(library.Code, out guid2))
            {
                ContentResult result4 = new ContentResult();
                result4.Content="不是表单设计器设计的表单，不能设置验证!";
                return result4;
            }
            RoadFlow.Model.Form form = new Form().Get(guid2);
            if (form == null)
            {
                ContentResult result5 = new ContentResult();
                result5.Content="未找到该表单!";
                return result5;
            }
            JObject obj2 = null;
            try
            {
                obj2 = JObject.Parse(form.attribute);
            }
            catch
            {
                ContentResult result6 = new ContentResult();
                result6.Content="表单属性不是有效的JSON!";
                return result6;
            }
            string str2 = obj2.Value<string>("dbConn");
            string str3 = obj2.Value<string>("dbTable");
            DbConnection connection = new DbConnection();
            Dictionary<string, List<RoadFlow.Model.TableField>> dictionary = new Dictionary<string, List<RoadFlow.Model.TableField>>();
            List<RoadFlow.Model.TableField> tableFields = connection.GetTableFields(StringExtensions.ToGuid(str2), str3);
            dictionary.Add(str3, tableFields);
            List<RoadFlow.Model.ProgramValidate> all = new ProgramValidate().GetAll(program.Id);
            if (!form.SubtableJSON.IsNullOrWhiteSpace())
            {
                JArray array2 = null;
                try
                {
                    array2 = JArray.Parse(form.SubtableJSON);
                }
                catch
                {
                }
                if (array2 != null)
                {
                    foreach (JObject obj3 in array2)
                    {
                        string str4 = obj3.Value<string>("secondtable");
                        if (!str4.IsNullOrWhiteSpace())
                        {
                            dictionary.Add(str4, connection.GetTableFields(StringExtensions.ToGuid(str2), str4));
                        }
                    }
                }
            }
            JArray array = new JArray();
            int num = 0;
            using (Dictionary<string, List<RoadFlow.Model.TableField>>.Enumerator enumerator2 = dictionary.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    KeyValuePair<string, List<RoadFlow.Model.TableField>> dict = enumerator2.Current;
                    using (List<RoadFlow.Model.TableField>.Enumerator enumerator3 = dict.Value.GetEnumerator())
                    {
                        while (enumerator3.MoveNext())
                        {
                            RoadFlow.Model.TableField field = enumerator3.Current;
                            RoadFlow.Model.ProgramValidate validate = all.Find(delegate (RoadFlow.Model.ProgramValidate p) {
                                return p.TableName.EqualsIgnoreCase(dict.Key) && p.FieldName.EqualsIgnoreCase(field.FieldName);
                            });
                            int num2 = (validate == null) ? 0 : validate.Validate;
                            int num3 = (validate == null) ? 0 : validate.Status;
                            JObject obj5 = new JObject();
                            obj5.Add("tableName", dict.Key);
                            obj5.Add("fieldName", (JToken)field.FieldName);
                            obj5.Add("fieldNote", (JToken)field.Comment);
                            object[] objArray1 = new object[] { "<select name=\"status_", (int)num, "\"><option value=\"0\"", (num3 == 0) ? " selected=\"selected\"" : "", ">编辑</option><option value=\"1\"", (1 == num3) ? " selected=\"selected\"" : "", ">只读</option><option value=\"2\"", (2 == num3) ? " selected=\"selected\"" : "", ">隐藏</option></select>" };
                            obj5.Add("Status", (JToken)string.Concat((object[])objArray1));
                            object[] objArray2 = new object[] {
                            "<input type=\"hidden\" name=\"tablename_", (int) num, "\" value=\"", dict.Key, "\"/><input type=\"hidden\" name=\"fieldname_", (int) num, "\" value=\"", field.FieldName, "\"/><input type=\"hidden\" name=\"fieldnote_", (int) num, "\" value=\"", field.Comment, "\"/><input type=\"hidden\" name=\"filedindex\" value=\"", (int) num, "\"/><select name=\"valdate_", (int) num,
                            "\"><option value=\"0\"", (num2 == 0) ? " selected=\"selected\"" : "", ">不检查</option><option value=\"1\"", (1 == num2) ? " selected=\"selected\"" : "", ">允许为空,非空时检查</option><option value=\"2\"", (2 == num2) ? " selected=\"selected\"" : "", ">不允许为空,并检查</option></select>"
                         };
                            obj5.Add("validateType", (JToken)string.Concat((object[])objArray2));
                            JObject obj4 = obj5;
                            num++;
                            array.Add(obj4);
                        }
                        continue;
                    }
                }
            }
            base.ViewData["list"]= array.ToString();
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }




        [Validate]
        public IActionResult Tree()
        {
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["rootId"]= new Dictionary().GetIdByCode("system_applibrarytype");
            return this.View();
        }


        #region  分组设置


        [Validate]
        public IActionResult Set_ListGroup()
        {
            
            string str = base.Request.Querys("programid");
            RoadFlow.Model.Program program = new RoadFlow.Business.Program().Get(StringExtensions.ToGuid(str));
            if (program == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content = "未找到程序设计实体!";
                return result1;
            }
            if (program.IsGroup==0)
            {
                ContentResult result2 = new ContentResult();
                result2.Content = "程序未设置分组!";
                return result2;
            }

            ProgramGroup field = new ProgramGroup();
            Dictionary dictionary = new Dictionary();
            List<RoadFlow.Model.ProgramGroup> all = field.GetAll(StringExtensions.ToGuid(base.Request.Querys("programid")));
            JArray array = new JArray();
            foreach (RoadFlow.Model.ProgramGroup field2 in all)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)field2.Id);
                obj1.Add("groupField", (JToken)field2.GroupField);

                obj1.Add("isGroupSummary", (field2.IsGroupSummary == 1) ? "是" : "否");

                obj1.Add("isGroupColumnShow", (field2.IsGroupColumnShow == 1) ? "是" : "否");

                obj1.Add("groupText", field2.GroupText.IsNullOrEmpty() ?"": ((JToken)field2.GroupText));

                obj1.Add("isGroupCollapse", (field2.IsGroupCollapse == 1) ? "是" : "否");

                obj1.Add("groupOrder", field2.GroupOrder.IsNullOrEmpty() ? "" : ((field2.GroupOrder=="desc")?"降序排序": "<span style='color:blue'>升序排序</span>"));



                obj1.Add("isGroupDataSorted", (field2.IsGroupDataSorted == 1) ? "是" : "否");

                obj1.Add("isShowSummaryOnHide", (field2.IsShowSummaryOnHide == 1) ? "是" : "否");
                obj1.Add("sort", (JToken)field2.Sort);
                obj1.Add("opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + field2.Id.ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            string[] textArray1 = new string[] { "programid=", base.Request.Querys("programid"), "&pagesize=", base.Request.Querys("pagesize"), "&pagenumber=", base.Request.Querys("pagenumber"), "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid"), "&typeid=", base.Request.Querys("typeid") };
            base.ViewData["query"] = string.Concat((string[])textArray1);
            base.ViewData["list"] = array.ToString();
            return this.View();
        }

        [Validate]
        public IActionResult Set_ListGroup_Edit()
        {
            Guid guid;
            string str = base.Request.Querys("fieldid");
            string str2 = base.Request.Querys("programid");
            RoadFlow.Model.Program program = new RoadFlow.Business.Program().Get(StringExtensions.ToGuid(str2));
            if (program == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content = "未找到程序设计实体!";
                return result1;
            }
            ProgramGroup field = new ProgramGroup();
            RoadFlow.Model.ProgramGroup field2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                field2 = field.Get(guid);
            }
            if (field2 == null)
            {
                RoadFlow.Model.ProgramGroup field1 = new RoadFlow.Model.ProgramGroup();
                field1.Id = Guid.NewGuid();
                field1.ProgramId = program.Id;
                field1.Sort = field.GetMaxSort(program.Id);
                field1.IsGroupSummary = 1;
                field1.IsGroupColumnShow = 1;
                field1.GroupOrder = "";
                field2 = field1;
            }
            Dictionary dictionary = new Dictionary();
            base.ViewData["queryString"] = base.Request.UrlQuery();
            base.ViewData["fieldOptions"] = field.GetFieldOptions(program.ConnId, program.SqlString, field2.GroupField, null);
          
            return this.View(field2);
        }


        [Validate, ValidateAntiForgeryToken]
        public string SaveSet_ListGroup(RoadFlow.Model.ProgramGroup programFieldModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            ProgramGroup field = new ProgramGroup();
            if (StringExtensions.IsGuid(base.Request.Querys("fieldid"), out guid))
            {
                RoadFlow.Model.ProgramGroup field2 = field.Get(guid);
                string oldContents = (field2 == null) ? "" : field2.ToString();
                field.Update(programFieldModel);
                Log.Add("修改了应用程序设计字段分组-" + programFieldModel.GroupField, "", LogType.系统管理, oldContents, programFieldModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                field.Add(programFieldModel);
                Log.Add("添加了应用程序设计字段分组-" + programFieldModel.GroupField, programFieldModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }





        [Validate]
        public string DeleteSet_ListGroup()
        {
            string str = base.Request.Forms("ids") ?? "";
            List<RoadFlow.Model.ProgramGroup> list = new List<RoadFlow.Model.ProgramGroup>();
            ProgramGroup field = new ProgramGroup();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                RoadFlow.Model.ProgramGroup field2 = field.Get(StringExtensions.ToGuid(str2));
                if (field2 != null)
                {
                    list.Add(field2);
                }
            }
            field.Delete(list.ToArray());
            Log.Add("删除了应用程序设计字段分组", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        #endregion




        public string GetFormatterOptions()
        {
            string str = base.Request.Forms("code");
            Dictionary dic = new Dictionary();
            RoadFlow.Model.Dictionary dicModel = dic.Get(str);
            string getFormatterOption = " ";
            if(dicModel!=null)
            {
                getFormatterOption = dicModel.Note;
            }
            return getFormatterOption;
        }


    }




}
