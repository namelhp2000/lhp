using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Pinyin;
using RoadFlow.Pinyin.format;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;


namespace RoadFlow.Utility
{
    public static class StringExtensions
    {


        #region  字符串对比操作

        /// <summary>
        /// 对比类
        /// </summary>
        private class CompareText : IEqualityComparer<string>
        {
            private StringComparison m_comparisonType { get; set; }
            public int GetHashCode(string t) { return t.GetHashCode(); }
            public CompareText(StringComparison comparisonType) { this.m_comparisonType = comparisonType; }
            public bool Equals(string x, string y)
            {
                if (x == y) { return true; }
                if (x == null || y == null) { return false; }
                else { return x.Equals(y, m_comparisonType); }
            }
        }



        /// <summary>
        /// 判断列表是否包含字符串  忽略大小写
        /// </summary>
        /// <param name="list">列表</param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this List<string> list, string str)
        {
            using (List<string>.Enumerator enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.EqualsIgnoreCase(str))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断字符串1是否包含  字符串2   忽略大小写
        /// </summary>
        /// <param name="str1">字符串</param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string str1, string str2)
        {
            return (((str1 != null) && (str2 != null)) && (str1.IndexOf(str2, (StringComparison)StringComparison.CurrentCultureIgnoreCase) >= 0));
        }



        /// <summary>
        /// 忽略大小写 对比字符串
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            if (str1 != null)
            {
                return str1.Equals(str2, (StringComparison)StringComparison.CurrentCultureIgnoreCase);
            }
            return (str2 == null);
        }




        /// <summary>
        /// 指定字符在此实例中的第一个匹配项的索引   子字符串在字符串的位置  忽视大小写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="subString"></param>
        /// <returns></returns>
        public static int IndexOfIgnoreCase(this string str, string subString)
        {
            return str.IndexOf(subString, (StringComparison)StringComparison.CurrentCultureIgnoreCase);
        }


        /// <summary>
        /// 忽略大小写替代字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="oldStr">旧字符串</param>
        /// <param name="newStr">替代新字符串</param>
        /// <returns></returns>
        public static string ReplaceIgnoreCase(this string str, string oldStr, string newStr)
        {
            if (!str.IsNullOrWhiteSpace())
            {
                return Regex.Replace(str, oldStr.IsNullOrEmpty() ? "" : oldStr, newStr.IsNullOrEmpty() ? "" : newStr, (RegexOptions)RegexOptions.IgnoreCase);
            }
            return "";
        }


        /// <summary>
        /// 字符串是否包含标点符号(不包括_下画线)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool InPunctuation(this string str)
        {
            foreach (char c in str.ToCharArray())
            {
                if (char.IsPunctuation(c) && c != '_')
                    return true;
            }
            return false;

        }


        /// <summary>
        /// 返回一个值，该值指示指定的 System.String 对象是否出现在此字符串中。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value">要搜寻的字符串。</param>
        /// <param name="comparisonType">指定搜索规则的枚举值之一</param>
        /// <returns>如果 value 参数出现在此字符串中，或者 value 为空字符串 ("")，则为 true；否则为 false</returns>
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            if (source == null || value == null) { return false; }
            if (value == "") { return true; }
            return (source.IndexOf(value, comparisonType) >= 0);
        }

        /// <summary>
        /// 通过使用默认的相等或字符比较器确定序列是否包含指定的元素。
        /// </summary>
        /// <param name="source">要在其中定位某个值的序列。</param>
        /// <param name="value">要在序列中定位的值。</param>
        /// <param name="comparisonType">指定搜索规则的枚举值之一</param>
        /// <exception cref="System.ArgumentNullException">source 为 null</exception>
        /// <returns>如果源序列包含具有指定值的元素，则为 true；否则为 false。</returns>
        public static bool Contains(this string[] source, string value, StringComparison comparisonType)
        {
            return System.Linq.Enumerable.Contains(source, value, new CompareText(comparisonType));
        }


        #endregion


        #region 字符串加解密及编码操作



        /// <summary>
        /// 获取字节大小
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int Size(this string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }


        /// <summary>
        /// 转换为MD5加密后的字符串（默认加密为32位）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToMD5String(this string str)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(str);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            md5.Dispose();

            return sb.ToString();
        }




        /// <summary> /// 字符串转Unicode码
        /// /// </summary> 
        /// /// <returns>The to unicode.</returns> 
        /// /// <param name="value">Value.</param> 
        public static string StringToUnicode(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            { // 取两个字符，每个字符都是右对齐。
                stringBuilder.AppendFormat("u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }

        /// <summary> 
        /// /// Unicode转字符串 
        /// /// </summary> 
        /// /// <returns>The to string.</returns>
        /// /// <param name="unicode">Unicode.</param>
        public static string UnicodeToString(string unicode)
        {
            string resultStr = "";
            string[] strList = unicode.Split('u');
            for (int i = 1; i < strList.Length; i++)
            {
                resultStr += (char)int.Parse(strList[i], System.Globalization.NumberStyles.HexNumber);
            }
            return resultStr;
        }






        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DESDecrypt(this string str)
        {
            if (!str.IsNullOrWhiteSpace())
            {
                return EncryptHelper.DESDecrypt(str);
            }
            return "";
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DESEncrypt(this string str)
        {
            if (!str.IsNullOrWhiteSpace())
            {
                return EncryptHelper.DESEncrypt(str);
            }
            return "";
        }



        /// <summary>
        /// Html解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string str)
        {
            return WebUtility.HtmlDecode(str);
        }


        /// <summary>
        /// Html编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string str)
        {
            return WebUtility.HtmlEncode(str);
        }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5(this string str)
        {
            return EncryptHelper.Md5By32(str.Trim1());
        }




        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlDecode(this string url)
        {
            return WebUtility.UrlDecode(url);
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlEncode(this string url)
        {
            return WebUtility.UrlEncode(url);
        }

        /// <summary>
        /// 返回统一编码字符串
        /// </summary>
        /// <param name="unicode"></param>
        /// <returns></returns>
        public static string unicode2String(string unicode)
        {
            StringBuilder string1 = new StringBuilder();

            if (unicode.StartsWith("&#x"))
            {
                string[] hex = unicode.Replace("&#x", "").Split(';');
                for (int i = 0; i < hex.Length - 1; i++)
                {
                    int data = Convert.ToInt32(hex[i], 16);
                    string1.Append((char)data);
                }
            }
            else if (unicode.StartsWith("&#"))
            {
                string[] hex = unicode.Replace("&#", "").Split(';');
                for (int i = 0; i < hex.Length; i++)
                {
                    int data = Convert.ToInt32(hex[i], 10);
                    string1.Append((char)data);
                }
            }

            return string1.ToString();
        }








        /// <summary>
        /// Base64加密
        /// 注:默认采用UTF8编码
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        public static string Base64Encode(this string source)
        {
            return Base64Encode(source, Encoding.UTF8);
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <param name="encoding">加密采用的编码方式</param>
        /// <returns></returns>
        public static string Base64Encode(this string source, Encoding encoding)
        {
            string encode = string.Empty;
            byte[] bytes = encoding.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }

        /// <summary>
        /// Base64解密
        /// 注:默认使用UTF8编码
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(this string result)
        {
            return Base64Decode(result, Encoding.UTF8);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <param name="encoding">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(this string result, Encoding encoding)
        {
            string decode = string.Empty;
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encoding.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }

        /// <summary>
        /// 计算SHA1摘要
        /// 注：默认使用UTF8编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static byte[] ToSHA1Bytes(this string str)
        {
            return str.ToSHA1Bytes(Encoding.UTF8);
        }

        /// <summary>
        /// 计算SHA1摘要
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static byte[] ToSHA1Bytes(this string str, Encoding encoding)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] inputBytes = encoding.GetBytes(str);
            byte[] outputBytes = sha1.ComputeHash(inputBytes);

            return outputBytes;
        }

        /// <summary>
        /// 转为SHA1哈希加密字符串
        /// 注：默认使用UTF8编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToSHA1String(this string str)
        {
            return str.ToSHA1String(Encoding.UTF8);
        }

        /// <summary>
        /// 转为SHA1哈希
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToSHA1String(this string str, Encoding encoding)
        {
            byte[] sha1Bytes = str.ToSHA1Bytes(encoding);
            string resStr = BitConverter.ToString(sha1Bytes);
            return resStr.Replace("-", "").ToLower();
        }




        #endregion



        #region 字符串请求操作

        /// <summary>
        /// 表单请求，  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns>值为空，返回空  否则返回对应值</returns>
        public static string Forms(this HttpRequest request, string key)
        {
            return (string.IsNullOrEmpty(request.Form[key])) ? string.Empty : (string)request.Form[key];

        }


        /// <summary>
        /// 请求查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns>空值，返回空， 否则返回对应值</returns>
        public static string Querys(this HttpRequest request, string key)
        {
            return (string.IsNullOrEmpty(request.Query[key])) ? string.Empty : (string)request.Query[key];

        }



        /// <summary>
        /// Url链接
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string Url(this HttpRequest request)
        {
            return ((request.Path.HasValue ? request.Path.Value : "") + request.UrlQuery());
        }


        /// <summary>
        /// Url链接全部字符串
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string UrlQuery(this HttpRequest request)
        {
            QueryString str = request.QueryString;
            if (!str.HasValue)
            {
                return string.Empty;
            }
            return str.Value;
        }


        #endregion


        #region   字符串正则操作


        /// <summary>
        /// 检查SQL语句
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ChkSQL(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            str = str.Replace("'", "''");
            return str;
        }


        /// <summary>
        /// 获取Img
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetImg(this string text)
        {
            string str = string.Empty;
            Match match = new Regex(@"<img\s+[^>]*\s*src\s*=\s*([']?)(?<url>\S+)'?[^>]*>", (RegexOptions)RegexOptions.Compiled).Match(text.ToLower());
            if (match.Success)
            {
                str = match.Result("${url}").Replace("\"", "").Replace("'", "");
            }
            return str;
        }



        /// <summary>
        /// 获取Img数组
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] GetImgs(this string text)
        {
            List<string> list = new List<string>();
            for (Match match = new Regex(@"<img\s+[^>]*\s*src\s*=\s*([']?)(?<url>\S+)'?[^>]*>", (RegexOptions)RegexOptions.Compiled).Match(text.ToLower()); match.Success; match = match.NextMatch())
            {
                list.Add(match.Result("${url}").Replace("\"", "").Replace("'", ""));
            }
            return list.ToArray();
        }




        /// <summary>
        /// 判断是否电话号码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsTelNumber(this string str)
        {
            return (!str.StartsWith("-") && str.Replace("-", "").IsDigital());
        }


        /// <summary>
        /// 判断是否是数值
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsNumber(string strNumber)
        {
            return new Regex(@"^([0-9])[0-9]*(\.\w*)?$").IsMatch(strNumber.Trim());
        }


        #endregion


        #region 类型数据判断

        /// <summary>
        /// 判断object对象是否为数组
        /// </summary>
        public static bool IsArray(object o)
        {
            return o is Array;
        }








        /// <summary>
        /// 是否是日期
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string str)
        {
            DateTime time;
            return DateTime.TryParse(str, out time);
        }

        /// <summary>
        /// 是否是日期 外面传参
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string str, out DateTime dt)
        {
            return DateTime.TryParse(str, out dt);
        }


        /// <summary>
        /// 是否小数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDecimal(this string str)
        {
            decimal num;
            return decimal.TryParse(str, out num);
        }

        /// <summary>
        /// 是否小数  传参
        /// </summary>
        /// <param name="str"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsDecimal(this string str, out decimal d)
        {
            return decimal.TryParse(str, out d);
        }

        /// <summary>
        /// 是否是数字 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDigital(this string str)
        {
            char[] chArray = str.ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                if (!char.IsDigit(chArray[i]))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 判断是否 字体Ico  开始是否fa
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsFontIco(this string str)
        {
            return str.Trim1().StartsWith("fa");
        }


        /// <summary>
        /// 是否Guid
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsGuid(this string str)
        {
            Guid guid;
            return Guid.TryParse(str, out guid);
        }

        /// <summary>
        /// 是否Guid  传参
        /// </summary>
        /// <param name="str"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsGuid(this string str, out Guid guid)
        {
            return Guid.TryParse(str, out guid);
        }


        /// <summary>
        /// 判断是否是整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(this string str)
        {
            int num;
            return int.TryParse(str, out num);
        }

        /// <summary>
        /// 判断是否整数  传参
        /// </summary>
        /// <param name="str"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool IsInt(this string str, out int i)
        {
            return int.TryParse(str, out i);
        }


        /// <summary>
        /// 判断是否null或者空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }


        /// <summary>
        ///判断是否null或者空白值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }


        /// <summary>
        /// 判断是否日期
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str)
        {
            DateTime time;
            if (!DateTime.TryParse(str, out time))
            {
                return DateTime.MinValue;
            }
            return time;
        }

        /// <summary>
        /// 判断是否是小数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string str, [Optional, DecimalConstant(0, 0x80, (uint)uint.MaxValue, (uint)uint.MaxValue, (uint)uint.MaxValue)] decimal defaultValue)
        {
            decimal num;
            if (!decimal.TryParse(str, out num))
            {
                return defaultValue;
            }
            return num;
        }



        /// <summary>
        /// 判断是否Guid
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string str)
        {
            Guid guid;
            if (!Guid.TryParse(str, out guid))
            {
                return Guid.Empty;
            }
            return guid;
        }


        /// <summary>
        /// 判断是否整数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string str, int defaultValue = -2147483648)
        {
            int num;
            if (!int.TryParse(str, out num))
            {
                return defaultValue;
            }
            return num;
        }



        /// <summary>
        /// 字符串转换成int
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static int StrToInt(object strValue)
        {
            return StrToInt(strValue, 0);
        }

        /// <summary>
        /// 字符串转换整数
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static int StrToInt(object strValue, int defValue)
        {
            if (((strValue == null) || (strValue.ToString() == string.Empty)) || (strValue.ToString().Length > 10))
            {
                return defValue;
            }
            string str = strValue.ToString();
            string strNumber = str[0].ToString();
            if (((str.Length == 10) && IsNumber(strNumber)) && (int.Parse(strNumber) > 1))
            {
                return defValue;
            }
            if ((str.Length == 10) && !IsNumber(strNumber))
            {
                return defValue;
            }
            int num = defValue;
            if ((strValue != null) && new Regex("^([-]|[0-9])[0-9]*$").IsMatch(strValue.ToString()))
            {
                num = Convert.ToInt32(strValue);
            }
            return num;
        }


        /// <summary>
        /// 内部数组
        /// </summary>
        /// <param name="strSearch"></param>
        /// <param name="stringArray"></param>
        /// <param name="caseInsensetive"></param>
        /// <returns></returns>
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return (GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0);
        }
        /// <summary>
        /// 判断转换数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="stringarray"></param>
        /// <param name="strsplit"></param>
        /// <returns></returns>
        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, SplitString(stringarray, strsplit), false);
        }

        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, SplitString(stringarray, ","), false);
        }





        /// <summary>
        /// 获取内部数组Id
        /// </summary>
        /// <param name="strSearch"></param>
        /// <param name="stringArray"></param>
        /// <param name="caseInsensetive"></param>
        /// <returns></returns>
        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                    {
                        return i;
                    }
                }
                else if (strSearch == stringArray[i])
                {
                    return i;
                }
            }
            return -1;
        }



        /// <summary>
        /// 获取内数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="stringarray"></param>
        /// <param name="strsplit"></param>
        /// <param name="caseInsensetive"></param>
        /// <returns></returns>
        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, strsplit), caseInsensetive);
        }



        /// <summary>
        /// 判断是否是日期格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDateTime1(string str)
        {
            bool flag;
            str = str.Replace("/", "-");
            if (!(flag = Regex.IsMatch(str, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$")))
            {
                flag = Regex.IsMatch(str, @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
            }
            return flag;
        }

        /// <summary>
        /// 字符串浮点
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static float StrToFloat(object strValue)
        {
            return StrToFloat(strValue, 0f);
        }

        /// <summary>
        /// 字符串转浮点方法
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static float StrToFloat(object strValue, float defValue)
        {
            if (strValue == null)
            {
                return defValue;
            }
            char[] separator = new char[] { '.' };
            if ((strValue.ToString().Trim() + ".").Split(separator)[0].Length > 15)
            {
                return defValue;
            }
            float num = defValue;
            if ((strValue != null) && new Regex(@"^([-]|[0-9])[0-9]*(\.\w*)?$").IsMatch(strValue.ToString().Trim()))
            {
                num = Convert.ToSingle(strValue.ToString().Trim());
            }
            return num;
        }




        #endregion


        #region  字符串拼接拆分操作


        /// <summary>
        /// 替换BR
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string replaceBR(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            if ((str.ToLower().Replace(" ", "").IndexOf("<br/>") == -1) && (str.ToLower().IndexOf("<br>") == -1))
            {
                return str.Replace("\n", "<br/>");
            }
            return str;
        }




        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="strSplit"></param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (strContent.IndexOf(strSplit) < 0)
            {
                return new string[] { strContent };
            }
            return Regex.Split(strContent, strSplit.Replace(".", @"\."), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 替换字符串
        /// </summary>
        /// <param name="SourceString"></param>
        /// <param name="SearchString"></param>
        /// <param name="ReplaceString"></param>
        /// <param name="IsCaseInsensetive"></param>
        /// <returns></returns>
        public static string ReplaceString(string SourceString, string SearchString, string ReplaceString, bool IsCaseInsensetive)
        {
            if (ReplaceString == null)
            {
                ReplaceString = string.Empty;
            }
            if (string.IsNullOrEmpty(SourceString))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(SearchString))
            {
                return SourceString;
            }
            return Regex.Replace(SourceString, Regex.Escape(SearchString), ReplaceString, IsCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }




        /// <summary>
        /// 字符串格式符
        /// </summary>
        /// <param name="strList">列表</param>
        /// <param name="split">拆分</param>
        /// <returns></returns>
        public static string ToString(this IList<string> strList, char split)
        {
            return strList.ToString(split.ToString());
        }




        /// <summary>
        /// 添加双引号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddDoubleQuotes(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            char[] separator = new char[] { ',' };
            StringBuilder builder = new StringBuilder();
            foreach (string str2 in str.Split(separator))
            {
                string str3 = str2.ContainsIgnoreCase("asc") ? "ASC" : (str2.ContainsIgnoreCase("desc") ? "DESC" : "ASC");
                string[] textArray1 = new string[] { "\"", str2.ReplaceIgnoreCase("asc", "").ReplaceIgnoreCase("desc", "").Trim1(), "\" ", str3, "," };
                builder.Append(string.Concat((string[])textArray1));
            }
            char[] trimChars = new char[] { ',' };
            return builder.ToString().TrimEnd(trimChars);
        }

        /// <summary>
        /// 连接List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="split">连接分隔符</param>
        /// <param name="prefix">前缀</param>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public static string JoinList<T>(this IEnumerable<T> ts, string split = ",", string prefix = "", string suffix = "")
        {
            if ((ts == null) || !Enumerable.Any<T>(ts))
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            foreach (T local in ts)
            {
                builder.Append(prefix);
                builder.Append(local);
                builder.Append(suffix);
                builder.Append(split);
            }
            return builder.ToString().TrimEnd(split.ToCharArray());





        }



        /// <summary>
        /// 拼接sql in的方法
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isSignle"></param>
        /// <returns></returns>
        public static string ToSqlIn(this string str, bool isSignle = true)
        {
            if (str.IsNullOrEmpty())
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            char[] chArray1 = new char[] { ',' };
            foreach (string str2 in str.Split(chArray1, (StringSplitOptions)StringSplitOptions.RemoveEmptyEntries))
            {
                if (isSignle)
                {
                    builder.Append("'");
                }
                builder.Append(str2);
                if (isSignle)
                {
                    builder.Append("'");
                }
                builder.Append(",");
            }
            char[] trimChars = new char[] { ',' };
            return builder.ToString().TrimEnd(trimChars);
        }




        /// <summary>
        /// 转换为用分隔符连接的字符串
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="quotes">引号，默认不带引号，范例：单引号 "'"</param>
        /// <param name="separator">分隔符，默认使用逗号分隔</param>
        public static string Join<T>(this IEnumerable<T> list, string quotes = "", string separator = ",")
        {
            return StringHelper.Join(list, quotes, separator);
        }

        /// <summary>
        /// 针对列表添加前缀与后缀操作方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts">列表</param>
        /// <param name="split">分裂符</param>
        /// <param name="prefix">前缀</param>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public static string JoinList<T>(this List<T> ts, string split = ",", string prefix = "", string suffix = "")
        {
            if ((ts == null) || (ts.Count == 0))
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            foreach (T local in ts)
            {
                builder.Append(prefix);
                builder.Append(local);
                builder.Append(suffix);
                builder.Append(split);
            }
            return builder.ToString().TrimEnd(split.ToCharArray());
        }


        /// <summary>
        /// 针对sql添加单引号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="single">添加单引号'</param>
        /// <returns></returns>
        public static string JoinSqlIn<T>(this List<T> ts, bool single = true)
        {
            return ts.JoinList<T>(",", (single ? "'" : ""), (single ? "'" : ""));
        }



        #endregion


        #region 字符串前端操作



        /// <summary>
        /// 移除html格式
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                string pattern = "<[^>]*>";
                return Regex.Replace(content, pattern, string.Empty, RegexOptions.IgnoreCase);
            }
            return string.Empty;
        }


        /// <summary>
        /// 移除html格式
        /// </summary>
        /// <param name="str"></param>
        /// <param name="filterItem"></param>
        /// <returns></returns>
        public static string RemoveHtml(string str, string filterItem)
        {
            if (!string.IsNullOrEmpty(filterItem))
            {
                foreach (string str2 in StringExtensions.SplitString(filterItem, ","))
                {
                    string s = str2.ToLower();
                    uint num2 = PrivateImplementationDetails.ComputeStringHash(s);
                    if (num2 <= 0xac307ed6)
                    {
                        if (num2 <= 0x290182c1)
                        {
                            switch (num2)
                            {
                                case 0x203e6faa:
                                    {
                                        if (s == "script")
                                        {
                                            str = ScriptFilter(str, str2, 2);
                                        }
                                        continue;
                                    }
                                case 0x274e1290:
                                    {
                                        if (s == "font")
                                        {
                                            str = ScriptFilter(str, str2, 3);
                                        }
                                        continue;
                                    }
                            }
                            if ((num2 == 0x290182c1) && (s == "span"))
                            {
                                str = ScriptFilter(str, str2, 3);
                            }
                        }
                        else
                        {
                            switch (num2)
                            {
                                case 0x4a9c9bdf:
                                    {
                                        if (s == "table")
                                        {
                                            str = ScriptFilter(ScriptFilter(ScriptFilter(ScriptFilter(ScriptFilter(str, str2, 3), "Tbody", 3), "Tr", 3), "Td", 3), "Th", 3);
                                        }
                                        continue;
                                    }
                                case 0x84e72504:
                                    {
                                        if (s == "img")
                                        {
                                            str = ScriptFilter(str, str2, 1);
                                        }
                                        continue;
                                    }
                            }
                            if ((num2 == 0xac307ed6) && (s == "style"))
                            {
                                str = ScriptFilter(str, str2, 2);
                            }
                        }
                    }
                    else if (num2 <= 0xd775a7d0)
                    {
                        switch (num2)
                        {
                            case 0xb8c60cba:
                                {
                                    if (s == "object")
                                    {
                                        str = ScriptFilter(str, str2, 2);
                                    }
                                    continue;
                                }
                            case 0xd00085a1:
                                {
                                    if (s == "iframe")
                                    {
                                        str = ScriptFilter(str, str2, 2);
                                    }
                                    continue;
                                }
                        }
                        if ((num2 == 0xd775a7d0) && (s == "html"))
                        {
                            str = RemoveHtml(str);
                        }
                    }
                    else
                    {
                        switch (num2)
                        {
                            case 0xe40c292c:
                                {
                                    if (s == "a")
                                    {
                                        str = ScriptFilter(str, str2, 3);
                                    }
                                    continue;
                                }
                            case 0xe562ab48:
                                {
                                    if (s == "div")
                                    {
                                        str = ScriptFilter(str, str2, 3);
                                    }
                                    continue;
                                }
                        }
                        if ((num2 == 0xf50c43ef) && (s == "p"))
                        {
                            str = ScriptFilter(str, str2, 3);
                        }
                    }
                }
            }
            return str;
        }


        /// <summary>
        /// 脚本过滤
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="tagName"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public static string ScriptFilter(string conStr, string tagName, int filterType)
        {
            string input = conStr;
            switch (filterType)
            {
                case 1:
                    return Regex.Replace(input, "<" + tagName + "([^>])*>", string.Empty, RegexOptions.IgnoreCase);

                case 2:
                    {
                        string[] textArray1 = new string[] { "<", tagName, "([^>])*>.*?</", tagName, "([^>])*>" };
                        return Regex.Replace(input, string.Concat(textArray1), string.Empty, RegexOptions.IgnoreCase);
                    }
                case 3:
                    return Regex.Replace(Regex.Replace(input, "<" + tagName + "([^>])*>", string.Empty, RegexOptions.IgnoreCase), "</" + tagName + "([^>])*>", string.Empty, RegexOptions.IgnoreCase);
            }
            return input;
        }




        /// <summary>
        /// 剪切http字符串
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="startStr"></param>
        /// <param name="overStr"></param>
        /// <param name="includekeytag"></param>
        /// <returns></returns>
        public static string CutHttpString(string conStr, string startStr, string overStr, bool includekeytag)
        {
            if (includekeytag)
            {
                return CutHttpString(conStr, startStr, overStr, 1);
            }
            return CutHttpString(conStr, startStr, overStr, 2);
        }

        /// <summary>
        /// 剪切http字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start"></param>
        /// <param name="last"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string CutHttpString(string str, string start, string last, int n)
        {
            if (str.ToLower().IndexOf(start.ToLower()) >= 0)
            {
                if (str.ToLower().IndexOf(last.ToLower()) >= 0)
                {
                    switch (n)
                    {
                        case 1:
                            str = str.Substring(str.ToLower().IndexOf(start.ToLower()), str.Length - str.ToLower().IndexOf(start.ToLower()));
                            str = str.Substring(0, str.ToLower().IndexOf(last.ToLower()) + last.Length);
                            return str;

                        case 2:
                            str = str.Substring(str.ToLower().IndexOf(start.ToLower()) + start.Length, (str.Length - str.ToLower().IndexOf(start.ToLower())) - start.Length);
                            str = str.Substring(0, str.ToLower().IndexOf(last.ToLower()));
                            return str;

                        case 3:
                            str = str.Substring(str.ToLower().LastIndexOf(start.ToLower()), str.Length - str.ToLower().LastIndexOf(start.ToLower()));
                            str = str.Substring(0, str.ToLower().LastIndexOf(last.ToLower()) + last.Length);
                            return str;

                        case 4:
                            str = str.Substring(str.ToLower().LastIndexOf(start.ToLower()) + start.Length, (str.Length - str.ToLower().LastIndexOf(start.ToLower())) - start.Length);
                            str = str.Substring(0, str.ToLower().LastIndexOf(last.ToLower()));
                            return str;

                        case 5:
                            str = str.Substring(str.ToLower().IndexOf(start.ToLower()), str.Length - str.ToLower().IndexOf(start.ToLower()));
                            str = str.Substring(0, str.ToLower().LastIndexOf(last.ToLower()) + last.Length);
                            return str;

                        case 6:
                            str = str.Substring(str.ToLower().IndexOf(start.ToLower()) + start.Length, (str.Length - str.ToLower().IndexOf(start.ToLower())) - start.Length);
                            str = str.Substring(0, str.ToLower().LastIndexOf(last.ToLower()));
                            return str;
                    }
                    str = "";
                    return str;
                }
                switch (n)
                {
                    case 7:
                        str = str.Substring(0, str.ToLower().IndexOf(start.ToLower()) + start.Length);
                        return str;

                    case 8:
                        str = str.Substring(0, str.ToLower().IndexOf(start.ToLower()));
                        return str;

                    case 9:
                        str = str.Substring(0, str.ToLower().LastIndexOf(start.ToLower()) + start.Length);
                        return str;

                    case 10:
                        str = str.Substring(0, str.ToLower().LastIndexOf(start.ToLower()));
                        return str;

                    case 11:
                        str = str.Substring(str.ToLower().IndexOf(start.ToLower()), str.Length - str.ToLower().IndexOf(start.ToLower()));
                        return str;

                    case 12:
                        str = str.Substring(str.ToLower().IndexOf(start.ToLower()) + start.Length, (str.Length - str.ToLower().IndexOf(start.ToLower())) - start.Length);
                        return str;

                    case 13:
                        str = str.Substring(str.ToLower().LastIndexOf(start.ToLower()), str.Length - str.ToLower().LastIndexOf(start.ToLower()));
                        return str;

                    case 14:
                        str = str.Substring(str.ToLower().LastIndexOf(start.ToLower()) + start.Length, (str.Length - str.ToLower().LastIndexOf(start.ToLower())) - start.Length);
                        return str;
                }
                str = "";
                return str;
            }
            str = string.Empty;
            return str;
        }




        /// <summary>
        /// 清除Html格式
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string ClearHtml(string strHtml)
        {
            if (!string.IsNullOrEmpty(strHtml))
            {
                Match match = null;
                for (match = new Regex(@"<\/?[^>]*>|\[\/?[^>]*\]", RegexOptions.IgnoreCase).Match(strHtml); match.Success; match = match.NextMatch())
                {
                    strHtml = strHtml.Replace(match.Groups[0].ToString(), "");
                }
                return strHtml;
            }
            strHtml = string.Empty;
            return strHtml;
        }



        /// <summary>
        /// 删除HTML
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string RemoveHTML(this string Htmlstring)
        {
            Htmlstring = Regex.Replace(Htmlstring, "<script[^>]*?>.*?</script>", "", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "<(.[^>]*)>", "", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "-->", "", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "<!--.*", "", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(quot|#34);", "\"", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(amp|#38);", "&", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(lt|#60);", "<", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(gt|#62);", ">", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(nbsp|#160);", "   ", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(iexcl|#161);", "\x00a1", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(cent|#162);", "\x00a2", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(pound|#163);", "\x00a3", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(copy|#169);", "\x00a9", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", (RegexOptions)RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            return Htmlstring;
        }


        /// <summary>
        /// 删除页面标签
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemovePageTag(this string html)
        {
            if (html.IsNullOrEmpty())
            {
                return string.Empty;
            }
            Regex regex = new Regex(@"<html\s*", (RegexOptions)RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(@"<head[\s\S]+</head\s*>", (RegexOptions)RegexOptions.IgnoreCase);
            Regex regex3 = new Regex(@"<body\s*", (RegexOptions)RegexOptions.IgnoreCase);
            Regex regex4 = new Regex(@"<form\s*", (RegexOptions)RegexOptions.IgnoreCase);
            Regex regex5 = new Regex("</(form|body|head|html)>", (RegexOptions)RegexOptions.IgnoreCase);
            html = new Regex("<!DOCTYPE[^>]*>", (RegexOptions)RegexOptions.IgnoreCase).Replace(html, "");
            html = regex.Replace(html, "<html　 ");
            html = regex2.Replace(html, "");
            html = regex3.Replace(html, "<body　 ");
            html = regex4.Replace(html, "<form　 ");
            html = regex5.Replace(html, "</$1　>");
            return html;
        }

        /// <summary>
        /// 删除脚本
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemoveScript(this string html)
        {
            if (html.IsNullOrEmpty())
            {
                return string.Empty;
            }
            Regex regex = new Regex(@" href *= *[\s\S]*script *:", (RegexOptions)RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(@" on[\s\S]*=", (RegexOptions)RegexOptions.IgnoreCase);
            Regex regex3 = new Regex(@"<iframe[\s\S]+</iframe *>", (RegexOptions)RegexOptions.IgnoreCase);
            Regex regex4 = new Regex(@"<frameset[\s\S]+</frameset *>", (RegexOptions)RegexOptions.IgnoreCase);
            html = new Regex(@"<script[\s\S]+</script *>", (RegexOptions)RegexOptions.IgnoreCase).Replace(html, "");
            html = regex.Replace(html, "");
            html = regex2.Replace(html, " _disibledevent=");
            html = regex3.Replace(html, "");
            html = regex4.Replace(html, "");
            return html;
        }




        /// <summary>
        /// 附件 前端链接显示方法
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isBr"></param>
        /// <returns></returns>
        public static string ToFilesShowString(this string str, bool isBr = true)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            char[] separator = new char[] { '|' };
            foreach (string str2 in str.Split(separator))
            {
                if (!str2.IsNullOrWhiteSpace())
                {
                    string fileName = Path.GetFileName(str2.DESDecrypt());
                    builder.Append(isBr ? ((string)"<div style=\"margin-bottom:4px;\">") : ((string)"<span style=\"margin-right:4px;\">"));
                    string[] textArray1 = new string[] { "<a class=\"blue1\" target=\"_blank\" href=\"/RoadFlowCore/Controls/ShowFile?file=", str2, "\">", fileName, "</a>" };
                    builder.Append(string.Concat((string[])textArray1));
                    builder.Append(isBr ? ((string)"</div>") : ((string)"</span>"));
                    if (!isBr)
                    {
                        builder.Append("、");
                    }
                }
            }
            char[] trimChars = new char[] { '、' };
            return builder.ToString().TrimEnd(trimChars);
        }




        /// <summary>
        /// 前端局部视图调用方法
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RenderPartialStringForm(string s)
        {
            string s1 = System.IO.Path.GetFileNameWithoutExtension(s);
            return "../FormDesigner/form/" + s1;
        }



        #endregion


        #region 字符串格式操作




        /// <summary>
        /// 转化为小写字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToLowerCases(string s)
        {
            return s.ToLower();
        }

        /// <summary>
        /// 转化为大小字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToUpperCases(string s)
        {
            return s.ToUpper();
        }









        /// <summary>
        /// 字符串格式
        /// </summary>
        /// <param name="strList"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string ToString(this IList<string> strList, string split)
        {
            StringBuilder sb = new StringBuilder(strList.Count * 10);
            for (int i = 0; i < strList.Count; i++)
            {
                sb.Append(strList[i]);
                if (i < strList.Count - 1)
                {
                    sb.Append(split);
                }
            }
            return sb.ToString();
        }







        /// <summary>
        /// 返回带星期的日期格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToDateWeekString(this DateTime date)
        {
            string week = string.Empty;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Friday: week = "五"; break;
                case DayOfWeek.Monday: week = "一"; break;
                case DayOfWeek.Saturday: week = "六"; break;
                case DayOfWeek.Sunday: week = "日"; break;
                case DayOfWeek.Thursday: week = "四"; break;
                case DayOfWeek.Tuesday: week = "二"; break;
                case DayOfWeek.Wednesday: week = "三"; break;
            }
            return date.ToString("yyyy年M月d日 ") + "星期" + week;
        }
        /// <summary>
        /// 返回带星期的日期时间格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToDateTimeWeekString(this DateTime date)
        {
            string week = string.Empty;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Friday: week = "五"; break;
                case DayOfWeek.Monday: week = "一"; break;
                case DayOfWeek.Saturday: week = "六"; break;
                case DayOfWeek.Sunday: week = "日"; break;
                case DayOfWeek.Thursday: week = "四"; break;
                case DayOfWeek.Tuesday: week = "二"; break;
                case DayOfWeek.Wednesday: week = "三"; break;
            }
            return date.ToString("yyyy年M月d日H时m分") + " 星期" + week;
        }

        #endregion


        #region 字符串过滤操作


        /// <summary>
        /// 去除前缀br
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string LosePreBr(string text)
        {
            if (text.Length >= 6)
            {
                text = text.Replace("<br/>", "<br />");
                while ((text.Substring(0, 6) == "<br />") || (text.Substring(0, 1) == "\r"))
                {
                    if (text.Substring(0, 1) == "\r")
                    {
                        text = text.Substring(2);
                    }
                    else
                    {
                        text = text.Substring(6);
                    }
                    if (text.Length < 6)
                    {
                        break;
                    }
                }
                if (text.StartsWith("<p"))
                {
                    text = StringExtensions.RemoveHtml(text, "p");
                }
            }
            return text;
        }


        /// <summary>
        /// 剔除前后空白
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Trim1(this string str)
        {
            if (!str.IsNullOrEmpty())
            {
                return str.Trim();
            }
            return "";
        }

        /// <summary>
        /// 剔除所有的空白
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimAll(this string str)
        {
            return Regex.Replace(str, @"\s", "");
        }






        /// <summary>
        /// 过滤文件img字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string ToFilesImgString(this string str, int width = 0, int height = 0)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            char[] separator = new char[] { '|' };
            foreach (string str2 in str.Split(separator))
            {
                if (!str2.IsNullOrWhiteSpace())
                {
                    Path.GetFileName(str2.DESDecrypt());
                    string[] textArray1 = new string[] { "<a target=\"_blank\" href=\"/RoadFlowCore/Controls/ShowFile?file=", str2, "\"><img border=\"0\" style=\"border:none 0;margin:3px 12px 3px 0;", (width != 0) ? ((string)("width:" + ((int)width) + "px;")) : "", (height != 0) ? ((string)("height:" + ((int)height) + "px;")) : "", "\" src=\"/RoadFlowCore/Controls/ShowFile?file=", str2, "\"/></a>" };
                    builder.Append(string.Concat((string[])textArray1));
                }
            }
            return builder.ToString();
        }




        /// <summary>
        /// 删除用户的前缀
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveUserPrefix(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return "";
            }
            if (!str.StartsWith("u_"))
            {
                return str;
            }
            char[] trimChars = new char[] { 'u', '_' };
            return str.TrimStart(trimChars);
        }



        /// <summary>
        /// 删除用户关系前缀
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveUserRelationPrefix(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return "";
            }
            if (!str.StartsWith("r_"))
            {
                return str;
            }
            char[] trimChars = new char[] { 'r', '_' };
            return str.TrimStart(trimChars);
        }

        /// <summary>
        /// 删除工作组前缀
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveWorkGroupPrefix(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return "";
            }
            if (!str.StartsWith("w_"))
            {
                return str;
            }
            char[] trimChars = new char[] { 'w', '_' };
            return str.TrimStart(trimChars);
        }




        /// <summary>
        /// 过滤器选择Sql  更新/新增/删除
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string FilterSelectSql(this string sql)
        {
            if (!sql.IsNullOrWhiteSpace())
            {
                return sql.ReplaceIgnoreCase("DELETE ", "").ReplaceIgnoreCase("UPDATE ", "").ReplaceIgnoreCase("INSERT ", "").ReplaceIgnoreCase("TRUNCATE TABLE", "").ReplaceIgnoreCase("DROP", "");
            }
            return "";
        }





        /// <summary>
        /// 移除末尾字符串
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="removeValue">要移除的值</param>
        public static string RemoveEnd(string value, string removeValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            if (string.IsNullOrWhiteSpace(removeValue))
                return value.SafeString();
            if (value.ToLower().EndsWith(removeValue.ToLower()))
                return value.Remove(value.Length - removeValue.Length, removeValue.Length);
            return value;
        }


        /// <summary>
        /// 删除Json字符串中键中的@符号
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <returns></returns>
        public static string RemoveAt(this string jsonStr)
        {
            Regex reg = new Regex("\"@([^ \"]*)\"\\s*:\\s*\"(([^ \"]+\\s*)*)\"");
            string strPatten = "\"$1\":\"$2\"";
            return reg.Replace(jsonStr, strPatten);
        }


        /// <summary>
        /// 去除字符串标点符号和空字符
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string RemovePunctuationOrEmpty(this string str)
        {
            StringBuilder NewString = new StringBuilder(str.Length);
            char[] charArr = str.ToCharArray();
            foreach (char symbols in charArr)
            {
                if (!char.IsPunctuation(symbols) && !char.IsWhiteSpace(symbols))
                {
                    NewString.Append(symbols);
                }
            }
            return NewString.ToString();
        }


        /// <summary>
        /// 过滤sql
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceSql(this string str)
        {
            str = str.Replace("'", "").Replace("--", " ").Replace(";", "");
            return str;
        }

        /// <summary>
        /// 过滤查询sql
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceSelectSql(this string str)
        {
            if (str.IsNullOrEmpty()) return "";
            str = str.ToLower().Replace("delete", "").Replace("update", "").Replace("insert", "");
            return str;
        }


        #endregion


        #region  字符串截取操作

        /// <summary>
        /// 省略表示
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <param name="show"></param>
        /// <returns></returns>
        public static string CutString(this string str, int len, string show = "...")
        {
            return Interruption(str, len, show);
        }


        /// <summary>
        /// 获取数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetNumber(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            foreach (char ch in str.ToCharArray())
            {
                int num2;
                if (int.TryParse(((char)ch).ToString(), out num2))
                {
                    builder.Append(ch);
                }
            }
            char[] trimChars = new char[] { '0' };
            return builder.ToString().TrimStart(trimChars);
        }



        /// <summary>
        /// 切断  长字符 串转化为“...”
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="len">长度</param>
        /// <param name="show">超过使用...</param>
        /// <returns></returns>
        public static string CutOut(this string str, int len, string show = "…")
        {
            int num = 0;
            string str2 = "";
            byte[] bytes = new ASCIIEncoding().GetBytes(str);
            int length = bytes.Length;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0x3f)
                {
                    num += 2;
                }
                else
                {
                    num++;
                }
                try
                {
                    str2 = str2 + str.Substring(i, 1);
                }
                catch
                {
                    break;
                }
                if (num > len)
                {
                    break;
                }
            }
            if (length > len)
            {
                str2 = str2 + show;
            }
            char ch = '\n';
            return str2.Replace("&nbsp;", " ").Replace("&lt;", "<").Replace("&gt;", ">").Replace(((char)ch).ToString(), "<br>");
        }



        /// <summary>
        /// 减少字幕
        /// </summary>
        /// <param name="contents">内容</param>
        /// <param name="len">长度</param>
        /// <param name="show">展示方式</param>
        /// <returns></returns>
        public static string CutSubTitle(this string contents, int len, string show = "…")
        {
            return contents.RemoveHTML().CutOut(len, show);
        }







        /// <summary>
        /// 获取左边多少个字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Left(this string str, int len)
        {
            if (str == null || len < 1) { return ""; }
            if (len < str.Length)
            { return str.Substring(0, len); }
            else
            { return str; }
        }
        /// <summary>
        /// 获取右边多少个字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Right(this string str, int len)
        {
            if (str == null || len < 1) { return ""; }
            if (len < str.Length)
            { return str.Substring(str.Length - len); }
            else
            { return str; }
        }

        #endregion


        #region 字符串拼音汉语操作




        /// <summary>
        /// 将全角数字转换为数字
        /// </summary>
        /// <param name="SBCCase"></param>
        /// <returns></returns>
        public static string SBCCaseToNumberic(string SBCCase)
        {
            char[] c = SBCCase.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new string(c);
        }



        /// <summary>
        /// 字符串转换成拼音显示方式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToPinYing(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            PinyinOutputFormat format = new PinyinOutputFormat(ToneFormat.WITHOUT_TONE, CaseFormat.LOWERCASE, VCharFormat.WITH_U_AND_COLON);
            return Pinyin4Net.GetPinyin(str, format).TrimAll();
        }



        /// <summary>
        /// 获取汉字拼音的第一个字母
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string ToChineseSpell(this string strText)
        {
            int len = strText.Length;
            string myStr = "";
            for (int i = 0; i < len; i++)
            {
                myStr += getSpell(strText.Substring(i, 1));
            }
            return myStr.ToLower();
        }
        /// <summary>
        /// 获取汉字拼音
        /// </summary>
        /// <param name="cnChar"></param>
        /// <returns></returns>
        public static string getSpell(this string cnChar)
        {
            byte[] arrCN = Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
                    }
                }
                return "x";
            }
            else return cnChar;
        }

        /// <summary>
        /// 截取字符串，汉字两个字节，字母一个字节
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="strLength">字符串长度</param>
        /// <returns></returns>
        public static string Interruption(this string str, int len, string show)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                { tempLen += 2; }
                else
                { tempLen += 1; }
                try
                { tempString += str.Substring(i, 1); }
                catch
                { break; }
                if (tempLen > len) break;
            }
            //如果截过则加上半个省略号 
            byte[] mybyte = System.Text.Encoding.Default.GetBytes(str);
            if (mybyte.Length > len)
                tempString += show;
            tempString = tempString.Replace("&nbsp;", " ");
            tempString = tempString.Replace("&lt;", "<");
            tempString = tempString.Replace("&gt;", ">");
            tempString = tempString.Replace('\n'.ToString(), "<br>");
            return tempString;
        }

        /// <summary>
        /// 截取字符串，汉字两个字节，字母一个字节
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="strLength">字符串长度</param>
        /// <returns></returns>


        /// <summary>
        /// 数字转换成大写数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string CapitalFigures(int num)
        {
            if (num < 0)
            {
                num = 0;
            }
            string[] strArray = new string[] {
        "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二", "十三", "十四", "十五", "十六",
        "十七", "十八", "十八", "十九", "二十", "二十一", "二十二", "二十三", "二十四", "二十五"
     };
            if (num < 0x19)
            {
                return strArray[num];
            }
            return Convert.ToString(num);
        }



        /// <summary>
        /// 判断是否汉字
        /// </summary>
        /// <param name="CString"></param>
        /// <returns></returns>
        public static bool IsChina(string CString)
        {
            CString = CString.Replace("&#x", "\\u").Replace(";", "");
            bool BoolValue = false;
            for (int i = 0; i < CString.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) < Convert.ToInt32(Convert.ToChar(128)))
                {
                    BoolValue = false;
                }
                else
                {
                    return BoolValue = true;
                }
            }
            return BoolValue;
        }

        /// <summary>
        /// 转为首字母小写
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToFirstLowerStr(this string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        /// <summary>
        /// 转为首字母大写
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToFirstUpperStr(this string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }


        #endregion


        #region 字符串其他操作



        public static string Trim(string returnStr)
        {
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr.Trim();
            }
            return string.Empty;
        }


        public static string RndNumber(int VcodeNum)
        {
            string vchar = "0,1,2,3,4,5,6,7,8,9";
            return MakeRndNum(VcodeNum, vchar);
        }

        public static string MakeRndNum(int VcodeNum, string Vchar)
        {
            char[] separator = new char[] { ',' };
            string[] strArray = Vchar.Split(separator);
            StringBuilder builder = new StringBuilder();
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < VcodeNum; i++)
            {
                builder.Append(strArray[random.Next(0, strArray.Length)].ToString());
            }
            return builder.ToString();
        }





        public static int[] arrayRandom(int MAXCARDS)
        {
            int[] numArray = new int[MAXCARDS];
            for (int i = 0; i < MAXCARDS; i++)
            {
                numArray[i] = i;
            }
            Random random = new Random();
            int index = 0;
            int num2 = 0;
            for (int j = 0; j < numArray.Length; j++)
            {
                index = random.Next(0, numArray.Length);
                if (index != j)
                {
                    num2 = numArray[j];
                    numArray[j] = numArray[index];
                    numArray[index] = num2;
                }
            }
            return numArray;
        }




        public static string RndNum(int VcodeNum, int Type = 0)
        {
            string vchar = string.Empty;
            switch (Type)
            {
                case 1:
                    vchar = "0,1,2,3,4,5,6,7,8,9";
                    break;

                case 2:
                    vchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
                    break;

                case 3:
                    vchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z";
                    break;

                case 4:
                    vchar = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
                    break;

                default:
                    vchar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z";
                    break;
            }
            return MakeRndNum(VcodeNum, vchar);
        }





        public static string FormatHumanizedTime(DateTime Time)
        {
            TimeSpan span = DateTime.Now.Subtract(Time);
            if (span.TotalDays <= 1.0)
            {
                if (span.TotalMinutes < 1.0)
                {
                    return (Math.Abs(span.Seconds) + "秒前");
                }
                if ((span.TotalMinutes < 60.0) && (span.TotalMinutes > 0.0))
                {
                    return (span.Minutes + "分钟前");
                }
                if ((span.TotalHours < 24.0) && (span.TotalHours > 0.0))
                {
                    return (span.Hours + "小时前");
                }
            }
            return Time.ToString();
        }









        /// <summary>
        /// 换行符
        /// </summary>
        public static string Line => Environment.NewLine;





        /// <summary>
        /// 产生随机字符串
        /// </summary>
        /// <returns>字符串位数</returns>
        public static string GetRandom(int length)
        {
            int number;
            char code;
            string checkCode = String.Empty;
            System.Random random = new Random();

            for (int i = 0; i < length + 1; i++)
            {
                number = random.Next();
                if (number % 2 == 0)
                    code = (char)('0' + (char)(number % 10));
                else
                    code = (char)('A' + (char)(number % 26));
                checkCode += code.ToString();
            }
            return checkCode;
        }

        /// <summary>
        /// 使用CDATA的方式将value保存在指定的element中
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetCDataValue(this System.Xml.Linq.XElement element, string value)
        {
            element.RemoveNodes();
            element.Add(new System.Xml.Linq.XCData(value));
        }


        #endregion


        #region  字符串转换类型操作


        /// <summary>
        /// 其他类型转Char
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string NunToChar(int number)
        {
            number += 0x41;
            if ((0x41 <= number) && (90 >= number))
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] bytes = new byte[] { (byte)number };
                return encoding.GetString(bytes);
            }
            return "";
        }


        /// <summary>
        /// 将Json字符串转为JObject
        /// </summary>
        /// <param name="jsonStr">Json字符串</param>
        /// <returns></returns>
        public static JObject ToJObject(this string jsonStr)
        {
            return jsonStr == null ? JObject.Parse("{}") : JObject.Parse(jsonStr.Replace("&nbsp;", ""));
        }


        /// <summary>
        /// 转为字节数组
        /// </summary>
        /// <param name="base64Str">base64字符串</param>
        /// <returns></returns>
        public static byte[] ToBytes_FromBase64Str(this string base64Str)
        {
            return Convert.FromBase64String(base64Str);
        }



        /// <summary>
        /// 序列化对象为xml字符串
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <returns>xml格式字符串</returns>
        public static string Serialize(this object obj)
        {
            if (obj == null) { return ""; }
            Type type = obj.GetType();
            if (type.IsSerializable)
            {
                try
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(type);
                    XmlWriterSettings xset = new XmlWriterSettings();
                    xset.CloseOutput = true;
                    xset.Encoding = Encoding.UTF8;
                    xset.Indent = true;
                    xset.CheckCharacters = false;
                    System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(sb, xset);
                    xs.Serialize(xw, obj);
                    xw.Flush();
                    xw.Close();
                    return sb.ToString();
                }
                catch { return ""; }
            }
            else
            {
                return "";
            }
        }


        public static T ToObject<T>(this string Json)
        {
            return Json == null ? default(T) : JsonConvert.DeserializeObject<T>(Json);
        }




        /// <summary>
        /// string转byte[]
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static byte[] ToBytes(this string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        /// <summary>
        /// string转byte[]
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="theEncoding">需要的编码</param>
        /// <returns></returns>
        public static byte[] ToBytes(this string str, Encoding theEncoding)
        {
            return theEncoding.GetBytes(str);
        }



        /// <summary>
        /// 将ASCII码形式的字符串转为对应字节数组
        /// 注：一个字节一个ASCII码字符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static byte[] ToASCIIBytes(this string str)
        {
            return str.ToList().Select(x => (byte)x).ToArray();
        }








        /// <summary>
        /// 将XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xmlStr">XML字符串</param>
        /// <returns></returns>
        public static T XmlStrToObject<T>(this string xmlStr)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);
            string jsonJsonStr = JsonConvert.SerializeXmlNode(doc);

            return JsonConvert.DeserializeObject<T>(jsonJsonStr);
        }

        /// <summary>
        /// 将XML字符串反序列化为对象
        /// </summary>
        /// <param name="xmlStr">XML字符串</param>
        /// <returns></returns>
        public static JObject XmlStrToJObject(this string xmlStr)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);
            string jsonJsonStr = JsonConvert.SerializeXmlNode(doc);

            return JsonConvert.DeserializeObject<JObject>(jsonJsonStr);
        }

        /// <summary>
        /// 将Json字符串转为List'T'
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this string jsonStr)
        {
            return string.IsNullOrEmpty(jsonStr) ? null : JsonConvert.DeserializeObject<List<T>>(jsonStr);
        }

        /// <summary>
        /// 将Json字符串转为DataTable
        /// </summary>
        /// <param name="jsonStr">Json字符串</param>
        /// <returns></returns>
        public static DataTable ToDataTable(this string jsonStr)
        {
            return jsonStr == null ? null : JsonConvert.DeserializeObject<DataTable>(jsonStr);
        }


        /// <summary>
        /// 将Json字符串转为JArray
        /// </summary>
        /// <param name="jsonStr">Json字符串</param>
        /// <returns></returns>
        public static JArray ToJArray(this string jsonStr)
        {
            return jsonStr == null ? JArray.Parse("[]") : JArray.Parse(jsonStr.Replace("&nbsp;", ""));
        }

        /// <summary>
        /// json数据转实体类,仅仅应用于单个实体类，速度非常快
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T ToEntity<T>(this string json)
        {
            if (json == null || json == "")
                return default(T);

            Type type = typeof(T);
            object obj = Activator.CreateInstance(type, null);

            foreach (var item in type.GetProperties())
            {
                PropertyInfo info = obj.GetType().GetProperty(item.Name);
                string pattern;
                pattern = "\"" + item.Name + "\":\"(.*?)\"";
                foreach (Match match in Regex.Matches(json, pattern))
                {
                    switch (item.PropertyType.ToString())
                    {
                        case "System.String": info.SetValue(obj, match.Groups[1].ToString(), null); break;
                        case "System.Int32": info.SetValue(obj, match.Groups[1].ToString().ToInt(), null); ; break;
                        case "System.Int64": info.SetValue(obj, Convert.ToInt64(match.Groups[1].ToString()), null); ; break;
                        case "System.DateTime": info.SetValue(obj, Convert.ToDateTime(match.Groups[1].ToString()), null); ; break;
                    }
                }
            }
            return (T)obj;
        }






        /// <summary>
        /// 转为网络终结点IPEndPoint
        /// </summary>=
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static IPEndPoint ToIPEndPoint(this string str)
        {
            IPEndPoint iPEndPoint = null;
            try
            {
                string[] strArray = str.Split(':').ToArray();
                string addr = strArray[0];
                int port = Convert.ToInt32(strArray[1]);
                iPEndPoint = new IPEndPoint(IPAddress.Parse(addr), port);
            }
            catch
            {
                iPEndPoint = null;
            }

            return iPEndPoint;
        }

        /// <summary>
        /// 将枚举类型的文本转为枚举类型
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="enumText">枚举文本</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string enumText)
        {
            var values = typeof(T).GetEnumValues().CastToList<T>();
            return values.Where(x => x.ToString() == enumText).FirstOrDefault();
        }

        #endregion


        #region  类型描述操作


        /// <summary>
        /// 获取类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        public static Type GetType<T>()
        {
            return GetType(typeof(T));
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="type">类型</param>
        public static Type GetType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }


        /// <summary>
        /// 获取枚举描述,使用System.ComponentModel.Description特性设置描述
        /// </summary>
        /// <param name="instance">枚举实例</param>
        public static string Description(this System.Enum instance)
        {
            return EnumHelper.GetDescription(instance.GetType(), instance);
        }







        /// <summary>
        /// 检测对象是否为null,为null则抛出<see cref="ArgumentNullException"/>异常
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="parameterName">参数名</param>
        public static void CheckNull(this object obj, string parameterName)
        {
            if (obj == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// 安全获取值，当值为null时，不会抛出异常
        /// </summary>
        /// <param name="value">可空值</param>
        public static T SafeValue<T>(this T? value) where T : struct
        {
            return value ?? default(T);
        }



        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="value">布尔值</param>
        public static string Description(this bool value)
        {
            return value ? "是" : "否";
        }

        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="value">布尔值</param>
        public static string Description(this bool? value)
        {
            return value == null ? "" : Description(value.Value);
        }

        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="instance">枚举实例</param>
        public static int Value(this System.Enum instance)
        {
            if (instance == null)
                return 0;
            return EnumHelper.GetValue(instance.GetType(), instance);
        }



        #endregion



        public static string RemoveXss(string input)
        {
            string str;
            string str2;
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            input = Regex.Replace(input, "&", "&amp;");
            input = Regex.Replace(input, "'", "&acute;");
            input = Regex.Replace(input, "\"", "&quot;");
            input = Regex.Replace(input, "<", "&lt;");
            input = Regex.Replace(input, ">", "&gt;");
            input = Regex.Replace(input, "/", "&#47;");
            do
            {
                str = input;
                input = Regex.Replace(input, @"(&#*\w+)[\x00-\x20]+;", "$1;");
                input = Regex.Replace(input, "(&#x*[0-9A-F]+);*", "$1;", RegexOptions.IgnoreCase);
                input = Regex.Replace(input, "&(amp|lt|gt|nbsp|quot);", "&amp;$1;");
                input = HtmlDecode(input);
            }
            while (str != input);
            input = Regex.Replace(input, "(?<=(<[\\s\\S]*=\\s*\"[^\"]*))>(?=([^\"]*\"[\\s\\S]*>))", "&gt;", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"(?<=(<[\s\S]*=\s*'[^']*))>(?=([^']*'[\s\S]*>))", "&gt;", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"(?<=(<[\s\S]*=\s*`[^`]*))>(?=([^`]*`[\s\S]*>))", "&gt;", RegexOptions.IgnoreCase);
            do
            {
                str = input;
                input = Regex.Replace(input, @"(<[^>]+?style[\x00-\x20]*=[\x00-\x20]*[^>]*?)\\([^>]*>)", "$1/$2", RegexOptions.IgnoreCase);
            }
            while (str != input);
            input = Regex.Replace(input, @"[\x00-\x08\x0b-\x0c\x0e-\x19]", string.Empty);
            input = Regex.Replace(input, "(<[^>]+?[\\x00-\\x20\"'/])(on|xmlns)[^>]*>", "$1>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "([a-z]*)[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*)[\\x00-\\x20]*j[\\x00-\\x20]*a[\\x00-\\x20]*v[\\x00-\\x20]*a[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:", "$1=$2nojavascript...", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "([a-z]*)[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*)[\\x00-\\x20]*v[\\x00-\\x20]*b[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:", "$1=$2novbscript...", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"(<[^>]+style[\x00-\x20]*=[\x00-\x20]*[^>]*?)/\*[^>]*\*/([^>]*>)", "$1$2", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "(<[^>]+?)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*?[eｅＥ][xｘＸ][pｐＰ][rｒＲ][eｅＥ][sｓＳ][sｓＳ][iｉＩ][oｏＯ][nｎＮ][\\x00-\\x20]*[\\(\\（][^>]*>", "$1>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "(<[^>]+?)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*?behaviour[^>]*>", "$1>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "(<[^>]+?)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*?behavior[^>]*>", "$1>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "(<[^>]+?)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*?s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:*[^>]*>", "$1>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"</*\w+:\w[^>]*>", "　");
            do
            {
                str2 = input;
                input = Regex.Replace(input, "</*(applet|meta|xml|blink|link|style|script|embed|object|iframe|frame|frameset|ilayer|layer|bgsound|title|base)[^>]*>?", "　", RegexOptions.IgnoreCase);
            }
            while (str2 != input);
            return input;
        }


        public static string PercentValue(int molecular, int denominator)
        {
            double percent = Convert.ToDouble(molecular) / Convert.ToDouble(denominator);
            return string.Format("{0:0.00%}", percent);
        }


        public static string PercentValue1(int molecular, int denominator)
        {
            double percent = Convert.ToDouble(molecular) / Convert.ToDouble(denominator);
            return percent.ToString("p2");
        }

        /// <summary>
        /// 文件展示转化成字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isBr"></param>
        /// <param name="isIndex"></param>
        /// <returns></returns>
        public static string ToFilesShowString(this string str, bool isBr = true, bool isIndex = true)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            int num = 1;
            char[] separator = new char[] { '|' };
            foreach (string str2 in str.Split(separator))
            {
                if (!str2.IsNullOrWhiteSpace())
                {
                    string fileName = Path.GetFileName(str2.DESDecrypt());
                    builder.Append(isBr ? ((string)"<div style=\"margin-bottom:4px;\">") : ((string)"<span style=\"margin-right:4px;\">"));
                    builder.Append("<a class=\"blue1\" target=\"_blank\" href=\"/RoadFlowCore/Controls/ShowFile?file=" + str2 + "\">");
                    builder.Append(isIndex ? (((int)num++).ToString() + "、") : string.Empty);
                    builder.Append(fileName + "</a>");
                    builder.Append(isBr ? ((string)"</div>") : ((string)"</span>"));
                    if (!isBr)
                    {
                        builder.Append("；");
                    }
                }
            }
            char[] trimChars = new char[] { (char)0xff1b };
            return builder.ToString().TrimEnd(trimChars);
        }




    }






}
