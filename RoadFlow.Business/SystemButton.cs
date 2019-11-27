using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using RoadFlow.Utility.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
    public class SystemButton
    {
        // Fields
        private static readonly string CacheKey = "Cache_SystemButton";

        private readonly RoadFlow.Data.SystemButton systemButtonData = new RoadFlow.Data.SystemButton();

        // Methods
        public int Add(RoadFlow.Model.SystemButton systemButton)
        {
            this.ClearCache();

            return this.systemButtonData.Add(systemButton);
        }

        public void ClearCache()
        {
            IO.Remove(CacheKey);
        }



        public int Delete(RoadFlow.Model.SystemButton systemButton)
        {
            this.ClearCache();
            return this.systemButtonData.Delete(systemButton);
        }

        public List<RoadFlow.Model.SystemButton> Delete(string ids)
        {
            List<RoadFlow.Model.SystemButton> list = new List<RoadFlow.Model.SystemButton>();
            char[] separator = new char[] { ',' };
            foreach (string str in ids.Split(separator))
            {
                RoadFlow.Model.SystemButton button = this.Get(str.ToGuid());
                if (button != null)
                {
                    list.Add(button);
                }
            }
            this.systemButtonData.Delete(list.ToArray());
            return list;
        }

        public RoadFlow.Model.SystemButton Get(Guid id)
        {
            return this.systemButtonData.Get(id);
        }

        public List<RoadFlow.Model.SystemButton> GetAll()
        {
            object obj2 = IO.Get(CacheKey);
            if (obj2 != null)
            {
                return (List<RoadFlow.Model.SystemButton>)obj2;
            }
            List<RoadFlow.Model.SystemButton> all = this.systemButtonData.GetAll();
            IO.Insert(CacheKey, all);
            return all;

           
        }

        public string GetButtonTypeOptions(string value = "")
        {
            string[] textArray1 = new string[] { "<option value=\"0\"", "0".EqualsIgnoreCase(value) ? " selected=\"selected\"" : "", ">普通按钮</option><option value=\"1\"", "1".EqualsIgnoreCase(value) ? " selected=\"selected\"" : "", ">列表按钮</option><option value=\"2\"", "2".EqualsIgnoreCase(value) ? " selected=\"selected\"" : "", ">工具栏按钮</option>" };
            return string.Concat((string[])textArray1);
        }

        public int GetMaxSort()
        {
            List<RoadFlow.Model.SystemButton> all = this.GetAll();
            if (all.Count == 0)
            {
                return 5;
            }
            return Enumerable.Max<RoadFlow.Model.SystemButton>((IEnumerable<RoadFlow.Model.SystemButton>)all, key=>key.Sort)+5;
        }

        public string GetOptions(string value = "", string language = "zh-CN")
        {
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.SystemButton button in this.GetAll())
            {
                string str = language.Equals("en-US") ? button.Name_en : (language.Equals("zh") ? button.Name_zh : button.Name);
                string str2 = button.Note.IsNullOrWhiteSpace() ? string.Empty : (" (" + button.Note + ")");
                object[] objArray1 = new object[] { "<option value=\"", button.Id, "\"", button.Id.ToString().EqualsIgnoreCase(value) ? " selected=\"selected\"" : "", ">", str, str2, "</option>" };
                builder.Append(string.Concat((object[])objArray1));
            }
            return builder.ToString();
        }

        public int Update(RoadFlow.Model.SystemButton systemButton)
        {
            this.ClearCache();

            return this.systemButtonData.Update(systemButton);
        }


        public JArray GetAllJson()
        {
            JArray array = new JArray();
            foreach (RoadFlow.Model.SystemButton button in this.GetAll())
            {
                array.Add(JObject.Parse(button.ToString()));
            }
            return array;
        }




}



 

}
