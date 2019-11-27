using System.Text.RegularExpressions;

namespace RoadFlow.Pinyin
{
    public static class PinyinUtil
    {
        // Methods
        public static bool IsHanzi(char ch)
        {
            return Regex.IsMatch(((char)ch).ToString(), @"[\u4e00-\u9fbb]");
        }
    }


}
