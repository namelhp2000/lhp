namespace RoadFlow.Utility.SMS
{
    /// <summary>
    /// 发送短信接口，补充完整
    /// </summary>
    public class SMS
    {


        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="mobileNumbers"></param>
        /// <returns></returns>
        public static bool SendSMS(string contents, string mobileNumbers)
        {
            if (contents.IsNullOrWhiteSpace())
            {
                return false;
            }
            char[] separator = new char[] { ',' };
            string[] strArray = mobileNumbers.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                string text1 = strArray[i];
            }
            return true;
        }
    }


}
