using RoadFlow.Pinyin.exception;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RoadFlow.Pinyin.format
{
    public static class PinyinFormatter
    {
        // Methods
        private static string convertToneNumber2ToneMark(string pinyin)
        {
            string input = pinyin.ToLower();
            Regex regex = new Regex("[a-z]*[1-5]?");
            if (!regex.IsMatch(input))
            {
                return input;
            }
            char ch = '$';
            int length = -1;
            regex = new Regex("[a-z]*[1-5]");
            if (!regex.IsMatch(input))
            {
                return input.Replace("v", "\x00fc");
            }
            int numericValue = (int)char.GetNumericValue(input[input.Length - 1]);
            int index = input.IndexOf('a');
            int num4 = input.IndexOf('e');
            int num5 = input.IndexOf("ou", (StringComparison)StringComparison.Ordinal);
            if (-1 != index)
            {
                length = index;
                ch = 'a';
            }
            else if (-1 != num4)
            {
                length = num4;
                ch = 'e';
            }
            else if (-1 != num5)
            {
                length = num5;
                ch = "ou"[0];
            }
            else
            {
                regex = new Regex("[aeiouv]");
                for (int i = input.Length - 1; i >= 0; i--)
                {
                    char ch3 = input[i];
                    if (regex.IsMatch(((char)ch3).ToString()))
                    {
                        length = i;
                        ch = input[i];
                        break;
                    }
                }
            }
            if (('$' == ch) || (-1 == length))
            {
                return input;
            }
            int num6 = numericValue - 1;
            int num7 = ("aeiouv".IndexOf(ch) * 5) + num6;
            char ch2 = "ā\x00e1ă\x00e0aē\x00e9ĕ\x00e8eī\x00edĭ\x00eciō\x00f3ŏ\x00f2oū\x00faŭ\x00f9uǖǘǚǜ\x00fc"[num7];
            StringBuilder builder1 = new StringBuilder();
            builder1.Append(input.Substring(0, length).Replace("v", "\x00fc"));
            builder1.Append(ch2);
            builder1.Append(input.Substring(length + 1).Replace("v", "\x00fc"));
            string str2 = builder1.ToString();
            return new Regex("[0-9]").Replace(str2, "");
        }

        public static string Format(string py, PinyinOutputFormat format)
        {
            if ((format.GetToneFormat == ToneFormat.WITH_TONE_MARK) && ((VCharFormat.WITH_V == format.GetVCharFormat) || (format.GetVCharFormat == VCharFormat.WITH_U_AND_COLON)))
            {
                throw new PinyinException("\"v\"或\"u:\"不能添加声调");
            }
            string input = py;
            switch (format.GetToneFormat)
            {
                case ToneFormat.WITH_TONE_MARK:
                    input = convertToneNumber2ToneMark(input.Replace("u:", "v"));
                    break;

                case ToneFormat.WITHOUT_TONE:
                    input = new Regex("[1-5]").Replace(input, "");
                    break;
            }
            VCharFormat getVCharFormat = format.GetVCharFormat;
            if (getVCharFormat == VCharFormat.WITH_V)
            {
                input = input.Replace("u:", "v");
            }
            else if (getVCharFormat == VCharFormat.WITH_U_UNICODE)
            {
                input = input.Replace("u:", "\x00fc");
            }
            switch (format.GetCaseFormat)
            {
                case CaseFormat.CAPITALIZE_FIRST_LETTER:
                    {
                        List<string> list1 = new List<string> { "a", "e", "o" };
                        if (!list1.Contains(input.ToLower()))
                        {
                            input = input.Substring(0, 1).ToUpper() + ((input.Length == 1) ? "" : input.Substring(1));
                        }
                        return input;
                    }
                case CaseFormat.LOWERCASE:
                    return input.ToLower();

                case CaseFormat.UPPERCASE:
                    return input.ToUpper();
            }
            return input;
        }
    }


}
