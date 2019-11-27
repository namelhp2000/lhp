using RoadFlow.Pinyin.data;
using RoadFlow.Pinyin.exception;
using RoadFlow.Pinyin.format;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Pinyin
{
    public static class Pinyin4Net
    {
        // Methods
        private static string CapitalizeFirstLetter(StringBuilder buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if ((buffer[i] == ' ') && (i != (buffer.Length - 1)))
                {
                    char c = buffer[i + 1];
                    if (c != ' ')
                    {
                        buffer[i + 1] = char.ToUpper(c);
                    }
                }
            }
            return buffer.ToString();
        }

        public static string[] GetHanzi(string pinyin, bool matchAll)
        {
            return PinyinDB.Instance.GetHanzi(pinyin.ToLower(), matchAll);
        }

        public static string[] GetPinyin(char hanzi)
        {
            if (!PinyinUtil.IsHanzi(hanzi))
            {
                throw new UnsupportedUnicodeException("不支持的字符: 请输入汉字");
            }
            return PinyinDB.Instance.GetPinyin(hanzi);
        }

        public static string GetPinyin(string text, PinyinOutputFormat format)
        {
            return GetPinyin(text, format, false, false, false);
        }

        public static string GetPinyin(string text, PinyinOutputFormat format, bool caseSpread, Func<string[], char, string, string> pinyinHandler)
        {
            Func<string, bool> s_9__0 = i => i.Equals(i);
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            StringBuilder buffer = new StringBuilder();
            List<string> firstLetterBuf = new List<string>();
            foreach (char ch in text)
            {
                if (!PinyinUtil.IsHanzi(ch))
                {
                    buffer.Append(ch);
                }
                else
                {
                    string[] pinyin = PinyinDB.Instance.GetPinyin(ch);
                    buffer.Append((pinyinHandler == null) ? pinyin[0] : pinyinHandler(pinyin, ch, text));
                    firstLetterBuf.Clear();
                    firstLetterBuf.AddRange(Enumerable.Select<string, string>(Enumerable.Where<string>(GetPinyin(ch), s_9__0 ?? (s_9__0 = delegate (string py)
                    {
                        char ch1 = py[0];
                        return !firstLetterBuf.Contains(((char)ch).ToString());
                    })), s_c.s_9__5_1 ?? (s_c.s_9__5_1 = new Func<string, string>(s_c.s_9.GetPinyinb__5_1))));
                    buffer.AppendFormat("[{0}] ", string.Join(",", firstLetterBuf.ToArray()));
                }
            }
            if (!caseSpread)
            {
                return buffer.ToString().Trim();
            }
            switch (format.GetCaseFormat)
            {
                case CaseFormat.CAPITALIZE_FIRST_LETTER:
                    return CapitalizeFirstLetter(buffer).Trim();

                case CaseFormat.LOWERCASE:
                    return buffer.ToString().ToLower();

                case CaseFormat.UPPERCASE:
                    return buffer.ToString().ToUpper();
            }
            return buffer.ToString();
        }

        public static string GetPinyin(string text, PinyinOutputFormat format, bool caseSpread, bool firstLetterOnly, bool multiFirstLetter)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            StringBuilder buffer = new StringBuilder();
            List<string> list = new List<string>();
            foreach (char ch in text)
            {
                if (!PinyinUtil.IsHanzi(ch))
                {
                    buffer.Append(ch);
                }
                else if (!firstLetterOnly)
                {
                    buffer.Append(GetUniqueOrFirstPinyinWithFormat(ch, format) + " ");
                }
                else if (!multiFirstLetter)
                {
                    buffer.AppendFormat("[{0}] ", (char)GetUniqueOrFirstPinyin(ch)[0]);
                }
                else
                {
                    list.Clear();
                    list.AddRange(Enumerable.Distinct<string>(Enumerable.Select<string, string>(GetPinyin(ch), s_c.s_9__4_0 ?? (s_c.s_9__4_0 = new Func<string, string>(s_c.s_9.GetPinyinb__4_0)))));
                    buffer.AppendFormat("[{0}] ", string.Join(",", list.ToArray()));
                }
            }
            if (!firstLetterOnly && caseSpread)
            {
                switch (format.GetCaseFormat)
                {
                    case CaseFormat.CAPITALIZE_FIRST_LETTER:
                        return CapitalizeFirstLetter(buffer);

                    case CaseFormat.LOWERCASE:
                        return buffer.ToString().Trim().ToLower();

                    case CaseFormat.UPPERCASE:
                        return buffer.ToString().Trim().ToUpper();
                }
            }
            return buffer.ToString().Trim();
        }

        public static string[] GetPinyinWithFormat(char hanzi, PinyinOutputFormat format)
        {
            return Enumerable.ToArray<string>(Enumerable.Select<string, string>(GetPinyin(hanzi), delegate (string item)
            {
                return PinyinFormatter.Format(item, format);
            }));
        }

        public static string GetUniqueOrFirstPinyin(char hanzi)
        {
            return GetPinyin(hanzi)[0];
        }

        public static string GetUniqueOrFirstPinyinWithFormat(char hanzi, PinyinOutputFormat format)
        {
            return PinyinFormatter.Format(GetUniqueOrFirstPinyin(hanzi), format);
        }

        // Nested Types
        [Serializable, CompilerGenerated]
        private sealed class s_c
        {
            // Fields
            public static readonly Pinyin4Net.s_c s_9 = new Pinyin4Net.s_c();
            public static Func<string, string> s_9__4_0;
            public static Func<string, string> s_9__5_1;

            // Methods
            internal string GetPinyinb__4_0(string py)
            {
                char ch = py[0];
                return ((char)ch).ToString();
            }

            internal string GetPinyinb__5_1(string py)
            {
                char ch = py[0];
                return ((char)ch).ToString();
            }
        }
    }


}
