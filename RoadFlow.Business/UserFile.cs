using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
    public class UserFile
    {

        /// <summary>
        /// 获取链接路径
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="userId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string GetLinkPath(string fullPath, Guid userId, string query)
        {
            string str = RootPath + "/UserFiles";
            char[] chArray1 = new char[] { '/' };
            string strArray1 = fullPath.Replace(@"\", "/").TrimStart(RootPath.Replace(@"\", "/").ToCharArray());
            string[] strArray = strArray1.TrimStart("UserFiles".ToCharArray()).Replace(@"\", "/").Trim().Split(chArray1, (StringSplitOptions)StringSplitOptions.RemoveEmptyEntries); ;
            string[] strArray2 = fullPath.Replace(@"\", "/").TrimStart(str.Replace(@"\", "/").ToCharArray()).Replace(@"\", "/").Trim().Split(chArray1, (StringSplitOptions)StringSplitOptions.RemoveEmptyEntries);
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            for (int i = 0; i < strArray.Length; i++)
            {
                string str2 = strArray[i];
                if (!str2.IsNullOrWhiteSpace())
                {
                    string str3 = str2.Equals(userId.ToUpperString()) ? "我的文件" : str2;
                    builder2.Append("/" + str2);
                    string str4 = (str + builder2.ToString()).DESEncrypt();
                    string[] textArray1 = new string[] { "<a class=\"blue1\" href=\"List?id=", str4, "&", query, "\">", str3, "</a>" };
                    builder.Append(string.Concat((string[])textArray1));
                    if (i < (strArray.Length - 1))
                    {
                        builder.Append("<i class=\"fa fa-angle-right\"></i>");
                    }
                }
            }
            return builder.ToString();
        }



        /// <summary>
        /// 获取文件展示字符串
        /// </summary>
        /// <param name="files"></param>
        /// <param name="showImg"></param>
        /// <param name="newRow"></param>
        /// <returns></returns>
        public static string GetFilesShowString(string files, bool showImg = false, bool newRow = true)
        {
            if (files.IsNullOrEmpty())
            {
                return "";
            }


            string[] strArray = files.Split(new char[] { '|' });
            StringBuilder builder = new StringBuilder();
            int num = 0;
            foreach (string str in strArray)
            {
                string path = Config.FilePath + str.DESDecrypt();
                string path1 = "../Controls/ShowFile?file=" + str;

                string str3 = " style=\"margin-bottom:4px;\"";

                builder.AppendFormat("<div{0}><a target=\"_blank\" href=\"{1}\" class=\"blue1\">{2}</a></div>", str3, path1, (++num).ToString() + "、" + Path.GetFileName(path));

            }
            builder.Append("<div style=\"clear:both;\"></div>");
            return builder.ToString();
        }



        public static string GetLinkShparePath(string fullPath, string dirPath, string query)
        {
            string fullName = new DirectoryInfo(dirPath).Parent.FullName;
            char[] chArray1 = new char[] { '/' };
            string[] strArray = fullPath.Replace(@"\", "/").TrimStart(fullName.Replace(@"\", "/").ToCharArray()).Replace(@"\", "/").Trim().Split(chArray1, (StringSplitOptions)StringSplitOptions.RemoveEmptyEntries);
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            for (int i = 0; i < strArray.Length; i++)
            {
                string str = strArray[i];
                if (!str.IsNullOrWhiteSpace())
                {
                    builder2.Append("/" + str);
                    string str3 = (fullName + builder2.ToString()).DESEncrypt();
                    string[] textArray1 = new string[] { "<a class=\"blue1\" href=\"ShareDirList?id=", str3, "&", query, "\">", str, "</a>" };
                    builder.Append(string.Concat((string[])textArray1));
                    if (i < (strArray.Length - 1))
                    {
                        builder.Append("<i class=\"fa fa-angle-right\"></i>");
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取相对路径
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetRelativePath(string fullPath)
        {
            if (fullPath.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            string rootPath = RootPath;
            string str2 = fullPath.Replace(@"\", "/").TrimStart(rootPath.Replace(@"\", "/").ToCharArray()).Replace(@"\", "/").Trim();
            if (!str2.StartsWith("/"))
            {
                return ("/" + str2);
            }
            return str2;
        }

        public List<ValueTuple<string, string, DateTime, int, long>> GetSubDirectoryAndFiles(string directory, Guid userId, string searchPattern = "", string order = "", int orderType = 0)
        {
            List<ValueTuple<string, string, DateTime, int, long>> list = new List<ValueTuple<string, string, DateTime, int, long>>();
            if (HasAccess(directory, userId))
            {
                new JArray();
                DirectoryInfo info = new DirectoryInfo(directory);
                if (!info.Exists)
                {
                    return list;
                }
                if (!searchPattern.IsNullOrEmpty() && !Enumerable.Contains<char>((IEnumerable<char>)searchPattern, '*'))
                {
                    searchPattern = "*" + searchPattern + "*";
                }
                foreach (DirectoryInfo info2 in searchPattern.IsNullOrEmpty() ? info.EnumerateDirectories() : info.EnumerateDirectories(searchPattern, (SearchOption)SearchOption.AllDirectories))
                {
                    list.Add(new ValueTuple<string, string, DateTime, int, long>(info2.Name, info2.FullName, info2.LastWriteTime, 0, 0L));
                }
                foreach (FileInfo info3 in searchPattern.IsNullOrEmpty() ? info.EnumerateFiles() : info.EnumerateFiles(searchPattern, (SearchOption)SearchOption.AllDirectories))
                {
                    list.Add(new ValueTuple<string, string, DateTime, int, long>(info3.Name, info3.FullName, info3.LastWriteTime, 1, info3.Length));
                }
                if (!order.IsNullOrWhiteSpace())
                {
                    switch (order.ToLower())
                    {
                        case "name":
                            return ((orderType == 0) ? Enumerable.ToList<ValueTuple<string, string, DateTime, int, long>>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)Enumerable.OrderBy<ValueTuple<string, string, DateTime, int, long>, string>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)list, key => key.Item1)) : Enumerable.ToList<ValueTuple<string, string, DateTime, int, long>>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)Enumerable.OrderByDescending<ValueTuple<string, string, DateTime, int, long>, string>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)list, key => key.Item1)));

                        case "date":
                            return ((orderType == 0) ? Enumerable.ToList<ValueTuple<string, string, DateTime, int, long>>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)Enumerable.OrderBy<ValueTuple<string, string, DateTime, int, long>, DateTime>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)list, key => key.Item3)) : Enumerable.ToList<ValueTuple<string, string, DateTime, int, long>>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)Enumerable.OrderByDescending<ValueTuple<string, string, DateTime, int, long>, DateTime>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)list, key => key.Item3)));

                        case "type":
                            return ((orderType == 0) ? Enumerable.ToList<ValueTuple<string, string, DateTime, int, long>>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)Enumerable.OrderBy<ValueTuple<string, string, DateTime, int, long>, int>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)list, key => key.Item4)) : Enumerable.ToList<ValueTuple<string, string, DateTime, int, long>>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)Enumerable.OrderByDescending<ValueTuple<string, string, DateTime, int, long>, int>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)list, key => key.Item4)));

                        case "size":
                            return ((orderType == 0) ? Enumerable.ToList<ValueTuple<string, string, DateTime, int, long>>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)Enumerable.OrderBy<ValueTuple<string, string, DateTime, int, long>, long>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)list, key => key.Item5)) : Enumerable.ToList<ValueTuple<string, string, DateTime, int, long>>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)Enumerable.OrderByDescending<ValueTuple<string, string, DateTime, int, long>, long>((IEnumerable<ValueTuple<string, string, DateTime, int, long>>)list, key => key.Item5)));
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取用户文件夹json
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="directory"></param>
        /// <param name="isSelect"></param>
        /// <returns></returns>
        public string GetUserDirectoryJSON(Guid userId, string directory = "", bool isSelect = false)
        {
            string path = directory.IsNullOrWhiteSpace() ? this.GetUserRoot(userId) : directory;
            if (!HasAccess(path, userId))
            {
                return "[]";
            }
            IEnumerable<string> enumerable = Directory.EnumerateDirectories(path);
            string str2 = path.DESEncrypt();
            JArray array = new JArray();
            foreach (string str3 in enumerable)
            {
                bool flag = Enumerable.Any<string>(Directory.EnumerateDirectories(str3));
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)str3.DESEncrypt());
                obj1.Add("parentID", (JToken)str2);
                obj1.Add("title", (JToken)Path.GetFileName(str3));
                obj1.Add("type", 1);
                obj1.Add("ico", flag ? "" : "fa-folder");
                obj1.Add("hasChilds", flag ? 1 : 0);
                JObject obj3 = obj1;
                array.Add(obj3);
            }
            if (!directory.IsNullOrWhiteSpace())
            {
                return array.ToString(0, Array.Empty<JsonConverter>());
            }
            JArray array2 = new JArray();
            JObject obj6 = new JObject();
            obj6.Add("id", (JToken)str2);
            obj6.Add("parentID", 0);
            obj6.Add("title", "我的文件");
            obj6.Add("type", 0);
            obj6.Add("ico", "fa-hdd-o");
            obj6.Add("hasChilds", Enumerable.Any<string>(enumerable) ? 1 : 0);
            JObject obj2 = obj6;
            array2.Add(obj2);
            if (!isSelect)
            {
                JObject obj7 = new JObject();
                obj7.Add("id", "myshare");
                obj7.Add("parentID", 0);
                obj7.Add("title", "我分享的文件");
                obj7.Add("type", 0);
                obj7.Add("ico", "fa-share-alt-square");
                obj7.Add("hasChilds", 0);
                JObject obj4 = obj7;
                array2.Add(obj4);
                JObject obj8 = new JObject();
                obj8.Add("id", "sharemy");
                obj8.Add("parentID", 0);
                obj8.Add("title", "我收到的文件");
                obj8.Add("type", 0);
                obj8.Add("ico", "fa-external-link-square");
                obj8.Add("hasChilds", 0);
                JObject obj5 = obj8;
                array2.Add(obj5);
            }
            obj2.Add("childs", array);
            return array2.ToString(0, Array.Empty<JsonConverter>());
        }


        /// <summary>
        /// 获取用户路径
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserRoot(Guid userId)
        {
            string path = RootPath + "/UserFiles/" + userId.ToUpperString() + "/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static bool HasAccess(string path, Guid userId)
        {
            string str = (Config.FilePath.IsNullOrWhiteSpace() ? Tools.GetContentRootPath() : Config.FilePath).Replace("/", "").Replace(@"\", "");
            string str2 = path.Replace("/", "").Replace(@"\", "");
            bool flag = str2.StartsWith(str, (StringComparison)StringComparison.CurrentCultureIgnoreCase);
            if (userId.IsNotEmptyGuid())
            {
                flag = str2.StartsWith(str + "UserFiles" + userId.ToUpperString(), (StringComparison)StringComparison.CurrentCultureIgnoreCase) || str2.StartsWith(str + "Attachment" + userId.ToUpperString(), (StringComparison)StringComparison.CurrentCultureIgnoreCase);
            }
            return flag;
        }

        public static string MoveTo(string[] paths, string newPath)
        {
            if (!Directory.Exists(newPath))
            {
                return "没有找到要移动到的文件夹!";
            }
            StringBuilder builder = new StringBuilder();
            try
            {
                foreach (string str in paths)
                {
                    string path = Path.Combine(newPath, Path.GetFileName(str));
                    if (Directory.Exists(str))
                    {
                        if (!Directory.Exists(path))
                        {
                            Directory.Move(str, path);
                        }
                        else
                        {
                            builder.Append("文件夹" + Path.GetFileName(str) + "在目标文件中已经存在，");
                        }
                    }
                    else if (File.Exists(str))
                    {
                        if (!File.Exists(path))
                        {
                            File.Move(str, path);
                        }
                        else
                        {
                            builder.Append("文件" + Path.GetFileName(str) + "在目标文件中已经存在，");
                        }
                    }
                }
            }
            catch (IOException exception1)
            {
                return exception1.Message;
            }
            if (builder.Length != 0)
            {
                char[] trimChars = new char[] { (char)0xff0c };
                return (builder.ToString().TrimEnd(trimChars) + "!");
            }
            return "1";
        }

        public static string ReName(string path, string newName)
        {
            if (!File.Exists(path))
            {
                if (!Directory.Exists(path))
                {
                    return "文件或目录不存在!";
                }
                try
                {
                    Directory.Move(path, Path.Combine(Directory.GetParent(path).FullName, newName));
                }
                catch (IOException exception1)
                {
                    return exception1.Message;
                }
                return "1";
            }
            try
            {
                File.Move(path, Path.Combine(Path.GetDirectoryName(path), newName + Path.GetExtension(path)));
            }
            catch (IOException exception2)
            {
                return exception2.Message;
            }
            return "1";
        }

        // Properties
        public static string RootPath
        {
            get
            {
                if (!Config.FilePath.IsNullOrWhiteSpace())
                {
                    return Config.FilePath;
                }
                return Tools.GetContentRootPath();
            }
        }


    }


}
