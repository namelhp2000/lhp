using RoadFlow.Pinyin.data;
using RoadFlow.Pinyin.exception;
using RoadFlow.Pinyin.format;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Pinyin
{
    public static class Pinyin4Name
    {
        // Methods
        public static string GetFirstLetter(string firstName)
        {
            string pinyin = GetPinyin(firstName);
            if (pinyin == null)
            {
                return null;
            }
            char[] separator = new char[] { ' ' };
            return string.Join<char>(" ", Enumerable.Select<string, char>(pinyin.Split(separator), s_c.s_9__1_0 ?? (s_c.s_9__1_0 = new Func<string, char>(s_c.s_9.GetFirstLetterb__1_0))));
        }

        public static string[] GetHanzi(string pinyin, bool matchAll)
        {
            return NameDB.Instance.GetHanzi(pinyin.ToLower(), matchAll);
        }

        public static string GetPinyin(string firstName)
        {
            if (!Enumerable.All<char>((IEnumerable<char>)firstName, new Func<char, bool>(PinyinUtil.IsHanzi)))
            {
                throw new UnsupportedUnicodeException("不支持的字符: 请输入汉字字符");
            }
            return NameDB.Instance.GetPinyin(firstName);
        }

        public static string GetPinyinWithFormat(string firstName, PinyinOutputFormat format)
        {
            char[] separator = new char[] { ' ' };
            return string.Join(" ", Enumerable.Select<string, string>(GetPinyin(firstName).Split(separator), delegate (string item)
            {
                return PinyinFormatter.Format(item, format);
            }));
        }

        // Nested Types
        [Serializable, CompilerGenerated]
        private sealed class s_c
        {
            // Fields
            public static readonly Pinyin4Name.s_c s_9 = new Pinyin4Name.s_c();
            public static Func<string, char> s_9__1_0;

            // Methods
            internal char GetFirstLetterb__1_0(string py)
            {
                return py[0];
            }
        }
    }


}
