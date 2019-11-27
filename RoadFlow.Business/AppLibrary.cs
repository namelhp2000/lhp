
using Newtonsoft.Json.Linq;
using RoadFlow.Data.DataAutoFac;
using RoadFlow.Mapper;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoadFlow.Business
{




    #region  流程2.8.5
   
    public class AppLibrary
    {
        // Fields 普通方式实现
     private readonly RoadFlow.Data.AppLibrary appLibraryData2 = new RoadFlow.Data.AppLibrary();// RoadFlow.Data.DataIocHelper.DataIoc<RoadFlow.Data.AppLibrary>();//ReflectionHelper.CreateInstance<RoadFlow.Data.AppLibrary>(typeof(RoadFlow.Data.AppLibrary)); //new RoadFlow.Data.AppLibrary();
      //private readonly RoadFlow.Data.AppLibrary appLibraryData = RoadFlow.Utility.IocHelper.Create<RoadFlow.Data.AppLibrary>();// RoadFlow.Data.DataIocHelper.DataIoc<RoadFlow.Data.AppLibrary>();

        // public RoadFlow.Data.AppLibrary  appLibraryData { get => RoadFlow.Utility.IocHelper.Create<RoadFlow.Data.AppLibrary>(); }

            //使用容器能够使用  必须使用接口
         private  IApp  appLibraryData5 { get => RoadFlow.Utility.DataIocHelper.DataIoc<IApp>(); }


        private IApp appLibraryData1 { get => RoadFlow.Data.DataAutoFac.AppFactory.GetDbHelper("App1"); }


        //通过容器实现 不需要接口
        private RoadFlow.Data.AppLibrary appLibraryData { get => RoadFlow.Utility.DataIocHelper.DataIoc1<RoadFlow.Data.AppLibrary>(); }

        //使用映射实现
        private  RoadFlow.Data.AppLibrary appLibraryData3 { get => RoadFlow.Utility.DataIocHelper.FactoryData<RoadFlow.Data.AppLibrary>(); }


        // Methods
        public int Add(RoadFlow.Model.AppLibrary appLibrary)
        {
            return this.appLibraryData.Add(appLibrary);
        }

        public int Delete(Guid id)
        {
            RoadFlow.Model.AppLibrary appLibrary = this.Get(id);
            if (appLibrary != null)
            {
                return this.appLibraryData.Delete(appLibrary);
            }
            return 0;
        }

        public List<RoadFlow.Model.AppLibrary> Delete(string idString)
        {
            List<RoadFlow.Model.AppLibrary> list = new List<RoadFlow.Model.AppLibrary>();
            char[] separator = new char[] { ',' };
            List<RoadFlow.Model.AppLibrary> all = this.GetAll();
            string[] strArray = idString.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                Guid guid;
                if (strArray[i].IsGuid(out guid))
                {
                    list.Add(all.Find(delegate (RoadFlow.Model.AppLibrary p) {
                        return p.Id == guid;
                    }));
                }
            }
            this.appLibraryData.Delete(list.ToArray());
            return list;
        }

        public RoadFlow.Model.AppLibrary Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.AppLibrary p) {
                return p.Id == id;
            });
        }

        public List<RoadFlow.Model.AppLibrary> GetAll()
        {
            return this.appLibraryData.GetAll();
        }

        public RoadFlow.Model.AppLibrary GetByCode(string code)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.AppLibrary p) {
                return p.Code.EqualsIgnoreCase(code);
            });
        }

        public string GetExportString(string ids)
        {
            if (ids.IsNullOrWhiteSpace())
            {
                return "[]";
            }
            JArray array = new JArray();
            char[] separator = new char[] { ',' };
            string[] strArray = ids.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                Guid guid;
                if (strArray[i].IsGuid(out guid))
                {
                    RoadFlow.Model.AppLibrary library = this.Get(guid);
                    if (library != null)
                    {
                        array.Add(JObject.FromObject(library));
                    }
                }
            }
            return array.ToString();
        }

        public List<RoadFlow.Model.AppLibrary> GetListByType(Guid typeId)
        {
            List<RoadFlow.Model.AppLibrary> list = new List<RoadFlow.Model.AppLibrary>();
            List<RoadFlow.Model.AppLibrary> all = this.GetAll();
            using (List<Guid>.Enumerator enumerator = new Dictionary().GetAllChildsId(typeId, true).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Guid id = enumerator.Current;
                    list.AddRange((IEnumerable<RoadFlow.Model.AppLibrary>)all.FindAll(delegate (RoadFlow.Model.AppLibrary p) {
                        return p.Type == id;
                    }));
                }
            }
            return list;
        }

        public DataTable GetPagerList(out int count, int size, int number, string title, string address, string typeId, string order)
        {
            //Dictionary<ValueTuple<string, string>, datatype> dics = new Dictionary<ValueTuple<string, string>, datatype>();
            //dics.Add(new ValueTuple<string, string>("Title", title), datatype.stringType);
            //dics.Add(new ValueTuple<string, string>("Address", address), datatype.stringType);
            //dics.Add(new ValueTuple<string, string>("Type", typeId), datatype.stringTypeIn);
            //return GetPagerTemplate.GetPagerList(out count, size, number, dics, "RF_AppLibrary", order);

          //  string rootPath = RoadFlow.Utility.IocHelper.Create<Microsoft.AspNetCore.Hosting.IHostingEnvironment>().WebRootPath; //; AutofacHelper.GetScopeService<IHostingEnvironment>().WebRootPath;
             return this.appLibraryData.GetPagerList(out count, size, number, title, address, typeId, order);
        }

        public string GetTypeOptions(string value = "")
        {
            return new Dictionary().GetOptionsByCode("system_applibrarytype", ValueField.Id, value, true);
        }

        public string Import(string json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return "要导入的json为空!";
            }
            JArray array = null;
            try
            {
                array = JArray.Parse(json);
            }
            catch
            {
                array = null;
            }
            if (array == null)
            {
                return "json解析错误!";
            }
            using (IEnumerator<JToken> enumerator = array.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    RoadFlow.Model.AppLibrary appLibrary = ((JObject)enumerator.Current).ToObject<RoadFlow.Model.AppLibrary>();
                    if (appLibrary != null)
                    {
                        if (this.Get(appLibrary.Id) != null)
                        {
                            this.Update(appLibrary);
                        }
                        else
                        {
                            this.Add(appLibrary);
                        }
                    }
                }
            }
            return "1";
        }

        public int Update(RoadFlow.Model.AppLibrary appLibrary)
        {
            return this.appLibraryData.Update(appLibrary);
        }
    }


  


    #endregion

}
