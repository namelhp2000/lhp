using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
    public class Dictionary
    {
        // Fields
        private readonly RoadFlow.Data.Dictionary dictionaryData = new RoadFlow.Data.Dictionary();

        // Methods
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public int Add(RoadFlow.Model.Dictionary dictionary)
        {
            return this.dictionaryData.Add(dictionary);
        }

        /// <summary>
        /// 使用递归  添加子节点
        /// </summary>
        /// <param name="dictionary">当前字典</param>
        /// <param name="dictionaries">字典列表</param>
        private void AddChilds(RoadFlow.Model.Dictionary dictionary, List<RoadFlow.Model.Dictionary> dictionaries)
        {
            List<RoadFlow.Model.Dictionary> childs = this.GetChilds(dictionary.Id);
            if (childs.Count != 0)
            {
                foreach (RoadFlow.Model.Dictionary dictionary2 in childs)
                {
                    dictionaries.Add(dictionary2);
                    this.AddChilds(dictionary2, dictionaries);
                }
            }
        }

        /// <summary>
        /// 使用递归 添加父节点
        /// </summary>
        /// <param name="dictionary">当前字典</param>
        /// <param name="dictionaries">字典列表</param>
        /// <param name="rootId">截至的根位置</param>
        private void AddParents(RoadFlow.Model.Dictionary dictionary, List<RoadFlow.Model.Dictionary> dictionaries, Guid rootId)
        {
            RoadFlow.Model.Dictionary dictionary2 = this.Get(dictionary.ParentId);
            if ((dictionary2 != null) && (dictionary2.Id != rootId))
            {
                dictionaries.Add(dictionary2);
                this.AddParents(dictionary2, dictionaries, rootId);
            }
        }

        /// <summary>
        /// 检验id与编码是否一致
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckCode(Guid id, string code)
        {
            RoadFlow.Model.Dictionary dictionary = this.Get(code);
            if (dictionary != null)
            {
                return (dictionary.Id == id);
            }
            return true;
        }

        /// <summary>
        /// 通过id删除字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.Dictionary> Delete(Guid id)
        {
            RoadFlow.Model.Dictionary dictionary = this.Get(id);
            if (dictionary == null)
            {
                return new List<RoadFlow.Model.Dictionary>();
            }
            List<RoadFlow.Model.Dictionary> allChilds = this.GetAllChilds(id);
            allChilds.Add(dictionary);
            this.dictionaryData.Delete(allChilds.ToArray());
            return allChilds;
        }

        /// <summary>
        /// 通过id获取当前字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RoadFlow.Model.Dictionary Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.Dictionary p) {
                return p.Id == id;
            });
        }

        /// <summary>
        /// 通过编码获取当前字典
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public RoadFlow.Model.Dictionary Get(string code)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.Dictionary p) {
                return p.Code.EqualsIgnoreCase(code);
            });
        }

        /// <summary>
        /// 获取全部字典
        /// </summary>
        /// <returns></returns>
        public List<RoadFlow.Model.Dictionary> GetAll()
        {
            return this.dictionaryData.GetAll();
        }

        /// <summary>
        /// 通过id获取所有的子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.Dictionary> GetAllChilds(Guid id)
        {
            List<RoadFlow.Model.Dictionary> dictionaries = new List<RoadFlow.Model.Dictionary>();
            RoadFlow.Model.Dictionary dictionary = this.Get(id);
            if (dictionary != null)
            {
                this.AddChilds(dictionary, dictionaries);
            }
            return dictionaries;
        }

        /// <summary>
        /// 通过编码获取所有的子节点
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.Dictionary> GetAllChilds(string code)
        {
            RoadFlow.Model.Dictionary dictionary = this.Get(code);
            if (dictionary != null)
            {
                return this.GetAllChilds(dictionary.Id);
            }
            return new List<RoadFlow.Model.Dictionary>();
        }

        /// <summary>
        /// 通过id获取所有子节点id列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMe">是否包含本身</param>
        /// <returns></returns>
        public List<Guid> GetAllChildsId(Guid id, bool isMe = true)
        {
            List<Guid> list = new List<Guid>();
            if (isMe)
            {
                list.Add(id);
            }
            foreach (RoadFlow.Model.Dictionary dictionary in this.GetAllChilds(id))
            {
                list.Add(dictionary.Id);
            }
            return list;
        }

        /// <summary>
        /// 通过id获取所有父节点字典
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMe">包含本身</param>
        /// <param name="rootId">截至的根位置</param>
        /// <returns></returns>
        public List<RoadFlow.Model.Dictionary> GetAllParent(Guid id, bool isMe = true, string rootId = "")
        {
            Guid guid2;
            List<RoadFlow.Model.Dictionary> dictionaries = new List<RoadFlow.Model.Dictionary>();
            Guid guid = rootId.IsGuid(out guid2) ? guid2 : this.GetRootId();
            RoadFlow.Model.Dictionary dictionary = this.Get(id);
            if (isMe && (dictionary.Id != guid))
            {
                dictionaries.Add(dictionary);
            }
            this.AddParents(dictionary, dictionaries, guid);
            return dictionaries;
        }

        /// <summary>
        /// 获取字典父级的标题名称
        /// </summary>
        /// <param name="id">通过当前的id</param>
        /// <param name="isMe">名称是否是否自身</param>
        /// <param name="isRoot">是否包含根</param>
        /// <param name="rootId">截至的根位置</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public string GetAllParentTitle(Guid id, bool isMe = true, bool isRoot = true, string rootId = "", bool reverse = true)
        {
            Guid guid2;
            List<RoadFlow.Model.Dictionary> list = this.GetAllParent(id, isMe, rootId);
            StringBuilder builder = new StringBuilder();
            Guid guid = rootId.IsGuid(out guid2) ? guid2 : Guid.Empty;
            if (!reverse)
            {
                list.Reverse();
            }
            foreach (RoadFlow.Model.Dictionary dictionary in list)
            {
                if (isRoot || (dictionary.Id != guid))
                {
                    builder.Append(dictionary.Title);
                    builder.Append(reverse ? ((string)" / ") : ((string)@" \ "));
                }
            }
            char[] trimChars = new char[] { ' ', reverse ? '/' : '\\', ' ' };
            return builder.ToString().TrimEnd(trimChars);
        }

        /// <summary>
        ///通过id 获取复选框
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="name"></param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="attr">属性</param>
        /// <param name="isAllChild">是否所有子节点</param>
        /// <returns></returns>
        public string GetCheckboxs(Guid id, string name, ValueField valueField = 0, string value = "", string attr = "", bool isAllChild = false)
        {
            return this.GetRadioOrCheckBox(id, name, 1, valueField, value, attr, isAllChild);
        }

        /// <summary>
        /// 通过编码获取复选框
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="name"></param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="attr">属性</param>
        /// <param name="isAllChild">是否所有子节点</param>
        /// <returns></returns>
        public string GetCheckboxs(string code, string name, ValueField valueField = 0, string value = "", string attr = "", bool isAllChild = false)
        {
            RoadFlow.Model.Dictionary dictionary = this.Get(code);
            if (dictionary != null)
            {
                return this.GetRadioOrCheckBox(dictionary.Id, name, 1, valueField, value, attr, isAllChild);
            }
            return string.Empty;
        }
        /// <summary>
        /// 通过id获取子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.Dictionary> GetChilds(Guid id)
        {
            return Enumerable.ToList<RoadFlow.Model.Dictionary>((IEnumerable<RoadFlow.Model.Dictionary>)Enumerable.OrderBy<RoadFlow.Model.Dictionary, int>((IEnumerable<RoadFlow.Model.Dictionary>)this.GetAll().FindAll(delegate (RoadFlow.Model.Dictionary p) {
                return p.ParentId == id;
            }), key=>key.Sort));
        }

        /// <summary>
        /// 通过编码获取子节点
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.Dictionary> GetChilds(string code)
        {
            RoadFlow.Model.Dictionary dict = this.Get(code);
            if (dict == null)
            {
                return new List<RoadFlow.Model.Dictionary>();
            }
            return Enumerable.ToList<RoadFlow.Model.Dictionary>((IEnumerable<RoadFlow.Model.Dictionary>)Enumerable.OrderBy<RoadFlow.Model.Dictionary, int>((IEnumerable<RoadFlow.Model.Dictionary>)this.GetAll().FindAll(delegate (RoadFlow.Model.Dictionary p) {
                return p.ParentId == dict.Id;
            }),key=>key.Sort));
        }

        /// <summary>
        /// 通过字典的唯一编码，获取对应的Guid
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Guid GetIdByCode(string code)
        {
            RoadFlow.Model.Dictionary dictionary = this.Get(code);
            if (dictionary != null)
            {
                return dictionary.Id;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 获取最大排序数值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetMaxSort(Guid id)
        {
            List<RoadFlow.Model.Dictionary> childs = this.GetChilds(id);
            if (childs.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.Dictionary>((IEnumerable<RoadFlow.Model.Dictionary>)childs,
                key=>key.Sort) + 5);
        }

        /// <summary>
        /// 通过编码获取选项项目
        /// </summary>
        /// <param name="code"></param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="isAllChild"></param>
        /// <returns></returns>
        public string GetOptionsByCode(string code, ValueField valueField = 0, string value = "", bool isAllChild = true)
        {
            if (code.IsNullOrWhiteSpace())
            {
                return "";
            }
            RoadFlow.Model.Dictionary dictionary = this.Get(code);
            if (dictionary == null)
            {
                return "";
            }
            return this.GetOptionsByID(dictionary.Id, valueField, value, isAllChild);
        }

        /// <summary>
        /// 通过id获取对应选项项目
        /// </summary>
        /// <param name="id"></param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="isAllChild"></param>
        /// <returns></returns>
        public string GetOptionsByID(Guid id, ValueField valueField = 0, string value = "", bool isAllChild = true)
        {
            if (id.IsEmptyGuid())
            {
                return "";
            }
            List<RoadFlow.Model.Dictionary> dictList = isAllChild ? this.GetAllChilds(id) : this.GetChilds(id);
            StringBuilder builder = new StringBuilder(dictList.Count * 60);
            StringBuilder builder2 = new StringBuilder();
            foreach (RoadFlow.Model.Dictionary dictionary in dictList)
            {
                if (dictionary.Status != 1)
                {
                    builder2.Clear();
                    int parentCount = this.GetParentCount(dictList, dictionary);
                    for (int i = 0; i < (parentCount - 1); i++)
                    {
                        builder2.Append("&nbsp;&nbsp;");
                    }
                    if (parentCount > 0)
                    {
                        builder2.Append("├");
                    }
                    string optionValue = this.GetOptionValue(valueField, dictionary);
                    object[] objArray1 = new object[] { optionValue, optionValue.Equals(value) ? " selected=\"selected\"" : "", builder2.ToString(), dictionary.Title };
                    builder.AppendFormat("<option value=\"{0}\"{1}>{2}{3}</option>", (object[])objArray1);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 通过字段名称获取选项值
        /// </summary>
        /// <param name="valueField"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private string GetOptionValue(ValueField valueField, RoadFlow.Model.Dictionary dictionary)
        {
            string title = string.Empty;
            switch (valueField)
            {
                case ValueField.Id:
                    title = dictionary.Id.ToString();
                    break;

                case ValueField.Title:
                    title = dictionary.Title;
                    break;

                case ValueField.Code:
                    title = dictionary.Code;
                    break;

                case ValueField.Value:
                    title = dictionary.Value;
                    break;

                case ValueField.Other:
                    title = dictionary.Other;
                    break;

                case ValueField.Note:
                    title = dictionary.Note;
                    break;
            }
            return (title ?? string.Empty);
        }

        /// <summary>
        /// 获取父节点数
        /// </summary>
        /// <param name="dictList"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        private int GetParentCount(List<RoadFlow.Model.Dictionary> dictList, RoadFlow.Model.Dictionary dict)
        {
            int num = 0;
            RoadFlow.Model.Dictionary parentDict = dictList.Find(delegate (RoadFlow.Model.Dictionary p) {
                return p.Id == dict.ParentId;
            });
            while (parentDict != null)
            {
                parentDict = dictList.Find(delegate (RoadFlow.Model.Dictionary p) {
                    return p.Id == parentDict.ParentId;
                });
                num++;
            }
            return num;
        }

        /// <summary>
        ///通过id 获取单选按钮获取复选框
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type">类型是0单选按钮，还是1复选框</param>
        /// <param name="valueField"></param>
        /// <param name="value">显示选中的值</param>
        /// <param name="attr">属性</param>
        /// <param name="isAllChild">是否获取所有子节点</param>
        /// <returns></returns>
        private string GetRadioOrCheckBox(Guid id, string name, int type, ValueField valueField = 0, string value = "", string attr = "", bool isAllChild = false)
        {
            if (id.IsEmptyGuid())
            {
                return "";
            }
            //是否获取所有子节点，或者是当前子节点
            List<RoadFlow.Model.Dictionary> list1 = isAllChild ? this.GetAllChilds(id) : this.GetChilds(id);
            StringBuilder builder = new StringBuilder(list1.Count * 60);
            foreach (RoadFlow.Model.Dictionary dictionary in list1)
            {

                string optionValue = this.GetOptionValue(valueField, dictionary);
                string[] textArray1 = new string[] { "<input type=\"", (type == 0) ? "radio" : "checkbox", "\" value=\"", optionValue, "\"" };
                builder.Append(string.Concat((string[])textArray1));
                if (optionValue.Equals(value))
                {
                    builder.Append(" checked=\"checked\"");
                }
                string[] textArray2 = new string[] { " id=\"", name, "_", dictionary.Id.ToString("N"), "\" name=\"", name, "\"" };
                builder.Append(string.Concat((string[])textArray2));
                if (!attr.IsNullOrWhiteSpace())
                {
                    builder.Append(" " + attr);
                }
                builder.Append(" style=\"vertical-align:middle\"/>");

                string[] textArray3 = new string[] { "<label style=\"vertical-align:middle\" for=\"", name, "_", dictionary.Id.ToString("N"), "\">", dictionary.Title, "</label>" };
                builder.Append(string.Concat((string[])textArray3));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 通过id获取单选按钮
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="attr"></param>
        /// <param name="isAllChild"></param>
        /// <returns></returns>
        public string GetRadios(Guid id, string name, ValueField valueField = 0, string value = "", string attr = "", bool isAllChild = false)
        {
            return this.GetRadioOrCheckBox(id, name, 0, valueField, value, attr, isAllChild);
        }

        /// <summary>
        /// 通过编码获取单选按钮
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="attr"></param>
        /// <param name="isAllChild"></param>
        /// <returns></returns>
        public string GetRadiosByCode(string code, string name, ValueField valueField = 0, string value = "", string attr = "", bool isAllChild = false)
        {
            RoadFlow.Model.Dictionary dictionary = this.Get(code);
            if (dictionary != null)
            {
                return this.GetRadioOrCheckBox(dictionary.Id, name, 0, valueField, value, attr, isAllChild);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取根节点
        /// </summary>
        /// <returns></returns>
        public Guid GetRootId()
        {
            RoadFlow.Model.Dictionary dictionary = this.GetAll().Find(
                key=> (key.ParentId == Guid.Empty));
            if (dictionary != null)
            {
                return dictionary.Id;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 通过id获取标题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetTitle(Guid id)
        {
            RoadFlow.Model.Dictionary dictionary = this.Get(id);
            if (dictionary != null)
            {
                return dictionary.Title;
            }
            return "";
        }

        /// <summary>
        /// 通过编码获取标题
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetTitle(string code)
        {
            RoadFlow.Model.Dictionary dictionary = this.Get(code);
            if (dictionary != null)
            {
                return dictionary.Title;
            }
            return "";
        }

        /// <summary>
        /// 通过id子节点获取值对应的标题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetTitle(Guid id, string value)
        {
            foreach (RoadFlow.Model.Dictionary dictionary in this.GetAllChilds(id))
            {
                if (dictionary.Value.Equals(value))
                {
                    return dictionary.Title;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 通过编码子节点获取值对应的标题
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetTitle(string code, string value)
        {
            foreach (RoadFlow.Model.Dictionary dictionary in this.GetAllChilds(code))
            {
                if (dictionary.Value.Equals(value))
                {
                    return dictionary.Title;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 通过id字符串获取标题
        /// </summary>
        /// <param name="idString"></param>
        /// <returns></returns>
        public string GetTitles(string idString)
        {
            if (idString.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            char[] separator = new char[] { ',' };
            string[] strArray = idString.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                Guid guid;
                if (strArray[i].IsGuid(out guid))
                {
                    RoadFlow.Model.Dictionary dictionary = this.Get(guid);
                    if (dictionary != null)
                    {
                        builder.Append(dictionary.Title);
                        builder.Append("、");
                    }
                }
            }
            char[] trimChars = new char[] { '、' };
            return builder.ToString().TrimEnd(trimChars);
        }

        /// <summary>
        /// 判断id是否有子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasChilds(Guid id)
        {
            return this.GetAll().Exists(delegate (RoadFlow.Model.Dictionary p) {
                return p.ParentId == id;
            });
        }

        /// <summary>
        /// 更新字典
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public int Update(RoadFlow.Model.Dictionary dictionary)
        {
            return this.dictionaryData.Update(dictionary);
        }

        /// <summary>
        /// 批量更新子节点
        /// </summary>
        /// <param name="dictionarys"></param>
        /// <returns></returns>
        public int Update(RoadFlow.Model.Dictionary[] dictionarys)
        {
            return this.dictionaryData.Update(dictionarys);
        }

        /// <summary>
        /// 通过id获取子节点的字符串
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetExportString(string id)
        {
            Guid guid;
            if (!id.IsGuid(out guid))
            {
                return string.Empty;
            }
            JArray array = new JArray();
            foreach (RoadFlow.Model.Dictionary dictionary in this.GetAllChilds(guid, true))
            {
                array.Add(JObject.FromObject(dictionary));
            }
            return array.ToString();
        }

        /// <summary>
        /// 通过id获取所有子节点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMe">是否包含本身</param>
        /// <returns></returns>
        public List<RoadFlow.Model.Dictionary> GetAllChilds(Guid id, bool isMe = false)
        {
            List<RoadFlow.Model.Dictionary> dictionaries = new List<RoadFlow.Model.Dictionary>();
            RoadFlow.Model.Dictionary dictionary = this.Get(id);
            if (dictionary != null)
            {
                if (isMe)
                {
                    dictionaries.Add(dictionary);
                }
                this.AddChilds(dictionary, dictionaries);
            }
            return dictionaries;
        }



        /// <summary>
        /// 导入功能通过json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
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
                    RoadFlow.Model.Dictionary dictionary = ((JObject)enumerator.Current).ToObject<RoadFlow.Model.Dictionary>();
                    if (dictionary != null)
                    {
                        if (this.Get(dictionary.Id) != null)
                        {
                            this.Update(dictionary);
                        }
                        else
                        {
                            this.Add(dictionary);
                        }
                    }
                }
            }
            return "1";
        }



        public string GetOptionsByID(Guid id, ValueField valueField = 0, string value = "", bool isAllChild = true, bool existsFlowType = true)
        {
            if (id.IsEmptyGuid())
            {
                return "";
            }
            List<RoadFlow.Model.Dictionary> dictList = isAllChild ? this.GetAllChilds(id, false) : this.GetChilds(id);
            StringBuilder builder = new StringBuilder(dictList.Count * 60);
            StringBuilder builder2 = new StringBuilder();
            List<RoadFlow.Model.Dictionary> list2 = existsFlowType ? new List<RoadFlow.Model.Dictionary>() : this.GetAllChilds("system_applibrarytype_flow", true);
            using (List<RoadFlow.Model.Dictionary>.Enumerator enumerator = dictList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    RoadFlow.Model.Dictionary child = enumerator.Current;
                    if ((child.Status != 1) && (existsFlowType || !list2.Exists(delegate (RoadFlow.Model.Dictionary p) {
                        return p.Id == child.Id;
                    })))
                    {
                        builder2.Clear();
                        int parentCount = this.GetParentCount(dictList, child);
                        for (int i = 0; i < parentCount; i++)
                        {
                            builder2.Append("&nbsp;&nbsp;");
                        }
                        if (parentCount > 0)
                        {
                            builder2.Append("├");
                        }
                        string optionValue = this.GetOptionValue(valueField, child);
                        object[] objArray1 = new object[] { optionValue, optionValue.Equals(value) ? " selected=\"selected\"" : "", builder2.ToString(), child.Title };
                        builder.AppendFormat("<option value=\"{0}\"{1}>{2}{3}</option>", (object[])objArray1);
                    }
                }
            }
            return builder.ToString();
        }

        public List<RoadFlow.Model.Dictionary> GetAllChilds(string code, bool isMe = false)
        {
             RoadFlow.Model.Dictionary dictionary = this.Get(code);
            if (dictionary != null)
            {
                return this.GetAllChilds(dictionary.Id, isMe);
            }
            return new List<RoadFlow.Model.Dictionary>();
        }


        public string GetOptionsByCode(string code, ValueField valueField = 0, string value = "", bool isAllChild = true, bool existsFlowType = true)
        {
            if (code.IsNullOrWhiteSpace())
            {
                return "";
            }
            RoadFlow.Model.Dictionary dictionary = this.Get(code);
            if (dictionary == null)
            {
                return "";
            }
            return this.GetOptionsByID(dictionary.Id, valueField, value, isAllChild, existsFlowType);
        }











     

   
}


}
