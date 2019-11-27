using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoadFlow.Utility;
using RoadFlow.Utility.Randoms;

namespace UnitTestUtilty
{
    [TestClass]
    public class RandomBuilderTest
    {
        RandomBuilder rdb = new RandomBuilder();



        [TestMethod]
        public void RandomBuilder()
        {
            var result = rdb.GenerateString(6, null,true);
            var result1= rdb.GenerateLetters(6, true);
            var result2 = rdb.GenerateChinese(6, true);
          
            var output = rdb.GenerateNumbers(6,  true);
         
           


            //��֤�Ƿ�����ȷ���
            Assert.AreEqual(result, output);
        }


        /// <summary>
        /// ͼƬ��֤�����
        /// </summary>
        [TestMethod]
        public void ImageCaptchaHelperTest()
        {
            var captchaCode = ImageCaptchaHelper.GenerateCaptchaCode();
            var result = ImageCaptchaHelper.GenerateCaptcha(100, 36, captchaCode);

            var output = rdb.GenerateNumbers(6, true);
          

            

            //��֤�Ƿ�����ȷ���
            Assert.AreEqual(result, output);
        }


    }
}
