using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using Xunit;

namespace UnitTestUtilty
{
    [TestClass]
    public class EncryptHelperTest
    {
        [TestMethod]
        [Fact]
        public void DesDecryptTest()
        {
            string value = "96655";

            var result = RoadFlow.Utility.EncryptHelper.DESEncrypt(value);


            var output = RoadFlow.Utility.EncryptHelper.DesEncrypt(value);
        }


        //验证是否是正确输出

        private const string DefaultPermissionKey = "<RSAKeyValue><Modulus>0MYOZd0jKOExdLxPiTSxxCVLyPa+w1HR1Z9cspDMFqtF7YTkLR6QHKEBG9reoDMn/6WZctBWKpUfnNNtxDd5RwKtp4LD3GVyX6Aif7/4lx9pGEL4XsvbRSTJD3G2Jo7gFNhc6Fd/Uz7oKCmgisk2TtpCIecnhbuyZ5b+2hudM8U=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";


        private  string PublicPermissionKey = $@"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCQKSwANtEAOhzqt48G0WVgeaLn
pJSSlEzdryVokZ41TqEcyVbm+v8HXrdh+ITNtEfWWNQRmvlzImcF5F7IqnprFAb4
5KRO0KP6DVME3BFqYVlKGScac76BDDuSx+e/Z8pb3O2aAMJDMXxJYzioAxhh/Kd/
kSaMAc3EuZXbFsa+dwIDAQAB";
        private  string PrivatePermissionKey = $@"MIICXAIBAAKBgQCQKSwANtEAOhzqt48G0WVgeaLnpJSSlEzdryVokZ41TqEcyVbm
+v8HXrdh+ITNtEfWWNQRmvlzImcF5F7IqnprFAb45KRO0KP6DVME3BFqYVlKGSca
c76BDDuSx+e/Z8pb3O2aAMJDMXxJYzioAxhh/Kd/kSaMAc3EuZXbFsa+dwIDAQAB
AoGAKzH6+jFynGPNSFMp6vwRKUApHMmGrwj6oy4YwmVnh0eBJPP7MwigI+AwiI2D
lXNgVwyUtpW+Cs6TGgPclrnmJf1/kb86tI+MuTr+7YQQrnHBWOM6BDJ1nk1sOi7L
YCq/h0bW/mMk1TB4T0zF4Shif/suDvzIVMjLYfKBj7DJy8ECQQCpwbg+nn89X8uI
kT0VOJqVuY4QG6lCEClKCmhaRpaVghmajgDBkioBzgEqFfayX2/viPMcm4Dc220i
yO/pUDMhAkEA2WZ+Ib9Pws6iKDHm0SgD+awIXsmqu0JCZBmTGVPW/S07zSUJAge+
51w044jmjaxO6kxKvV/ii/JfICx2IVvWlwJAUJu8cX+xy+MBMwhEiR8nyJEj9GIu
LUCfWpk2lCeQuc3depaTpVdSuyinROTJEEphTM0rJBpzRmyrlij0Q0XiAQJBAJcK
Gj+gBu41WXvLj61ou7pOx1HzkmafVjjte8Fw+kDTmGSigmuirgNXkHc0udlcCUfG
0XZYk7DfJy/XNAthHukCQH4h2EIdkPQ8F/GbxH6MU6FAzO0H+yeH8wyj/zdAjGaZ
PHoYs1D+acnUou8IhYs4O99fxAlG/9Ee4eLIkh62x04=";

        [Fact]
        public void RSATest()
        {
            string token = "Expiration" + "|" + DateTime.Now.AddYears(5).ToString("yyyy-MM-dd hh:mm:ss");
            #region 添加签名与验签都OK
            string strEncrypt = RoadFlow.Utility.EncryptHelper.RsaSign(token, PrivatePermissionKey);
            bool strDecrypt = RoadFlow.Utility.EncryptHelper.RsaVerify(token, PublicPermissionKey, strEncrypt);
            #endregion


            //RSA加密，只能先用公钥加密，然后私钥解密能成

            string token1 = RoadFlow.Utility.EncryptHelper.EncryptRsa(token, PublicPermissionKey);
            string str1 = RoadFlow.Utility.EncryptHelper.DecryptRsa(token1, PrivatePermissionKey);


          


            //验证是否是正确输出




            string ss = "";
        }


    }
}
