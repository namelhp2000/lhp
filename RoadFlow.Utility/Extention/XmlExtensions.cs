
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace RoadFlow.Utility
{
    /// <summary>
    /// XmlExtensions
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// 获取节点 value，element 为 null 则 返回 null
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetValue(this XElement element)
        {
            if (element == null)
                return null;
            return element.Value;
        }


        /// <summary>
        /// 设置声明
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="version"></param>
        /// <param name="encoding"></param>
        /// <param name="standalone"></param>
        /// <returns></returns>
        public static XDeclaration SetDeclaration(this XDocument doc, string version = "1.0", string encoding = "utf-8", string standalone = null)
        {
            if (doc.Declaration != null)
            {
                doc.Declaration.Version = version;
                doc.Declaration.Encoding = encoding;
                doc.Declaration.Standalone = standalone;
            }
            else
            {
                XDeclaration xd = new XDeclaration(version, encoding, standalone);
                doc.Declaration = xd;
            }

            return doc.Declaration;
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="parentNode">上一级节点</param>
        /// <param name="nodeName">节点名称</param>
        /// <returns></returns>
        public static XElement AddNode(this XContainer parentNode, string nodeName)
        {
            XElement node = new XElement(nodeName);
            parentNode.Add(node);
            return node;
        }

        /// <summary>
        /// 添加节点与内容
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static XElement AddNode(this XContainer parentNode, string nodeName, object content)
        {
            XElement node = new XElement(nodeName, content);
            parentNode.Add(node);
            return node;
        }
        /// <summary>
        /// 添加节点内容
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static XElement AddNode(this XContainer parentNode, string nodeName, params object[] content)
        {
            XElement node = new XElement(nodeName, content);
            parentNode.Add(node);
            return node;
        }

        /// <summary>
        /// 添加内容节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XElement AddTextNode(this XContainer parentNode, string nodeName, object value)
        {
            string val = string.Empty;
            if (value != null)
                val = value.ToString();
            XElement node = parentNode.AddTextNode(nodeName, val);
            return node;
        }

        /// <summary>
        /// 添加内容节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XElement AddTextNode(this XContainer parentNode, string nodeName, string value)
        {
            string val = ReplaceInvalidChar(value);
            XElement node = new XElement(nodeName);
            node.Value = val;
            parentNode.Add(node);
            return node;
        }
        /// <summary>
        /// 添加内容节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XElement AddTextNode(this XContainer parentNode, string nodeName, int value)
        {
            return parentNode.AddTextNode(nodeName, value.ToString());
        }

        public static XElement AddCDataNode(this XContainer parentNode, string nodeName, object value)
        {
            string val = string.Empty;
            if (value != null)
                val = value.ToString();
            XElement node = parentNode.AddCDataNode(nodeName, val);
            return node;
        }
        public static XElement AddCDataNode(this XContainer parentNode, string nodeName, string value)
        {
            XElement node = new XElement(nodeName);
            XCData cDataNode = new XCData(ReplaceInvalidChar(value));
            node.Add(cDataNode);
            parentNode.Add(node);
            return node;
        }

        static string ReplaceInvalidChar(string s)
        {
            s = s ?? "";

            string re = "[\x00-\x08]|[\x0B-\x0C]|[\x0E-\x1F]";

            s = s.Replace("\uFFFF", "");
            s = Regex.Replace(s, re, "");

            return s;
        }
    }

}
