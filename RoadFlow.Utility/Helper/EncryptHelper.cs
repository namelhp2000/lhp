using RoadFlow.Utility.Helper.Internal;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace RoadFlow.Utility
{
    /// <summary>
    /// 加密操作
    /// 说明：
    /// 1. AES加密整理自支付宝SDK
    /// 2. RSA加密采用 https://github.com/stulzq/DotnetCore.RSA/blob/master/DotnetCore.RSA/RSAHelper.cs
    /// </summary>
    public static class EncryptHelper
    {

        #region Md5加密

        /// <summary>
        /// Md5加密，返回16位结果
        /// </summary>
        /// <param name="value">值</param>
        public static string Md5By16(string value)
        {
            return Md5By16(value, Encoding.UTF8);
        }

        /// <summary>
        /// Md5加密，返回16位结果
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">字符编码</param>
        public static string Md5By16(string value, Encoding encoding)
        {
            return Md5(value, encoding, 4, 8);
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        private static string Md5(string value, Encoding encoding, int? startIndex, int? length)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            var md5 = new MD5CryptoServiceProvider();
            string result;
            try
            {
                var hash = md5.ComputeHash(encoding.GetBytes(value));
                result = startIndex == null ? BitConverter.ToString(hash) : BitConverter.ToString(hash, startIndex.SafeValue(), length.SafeValue());
            }
            finally
            {
                md5.Clear();
            }
            return result.Replace("-", "");
        }

        /// <summary>
        /// Md5加密，返回32位结果
        /// </summary>
        /// <param name="value">值</param>
        public static string Md5By32(string value)
        {
            return Md5By32(value, Encoding.UTF8);
        }

        /// <summary>
        /// Md5加密，返回32位结果
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">字符编码</param>
        public static string Md5By32(string value, Encoding encoding)
        {
            return Md5(value, encoding, null, null);
        }

        #endregion

        #region DES加密

        /// <summary>
        /// DES密钥,24位字符串
        /// </summary>
        public static string DesKey = "#s^un2ye21fcv%|f0XpR,+vh";

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        public static string DesEncrypt(object value)
        {
            return DesEncrypt(value, DesKey);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,24位</param>
        public static string DesEncrypt(object value, string key)
        {
            return DesEncrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,24位</param>
        /// <param name="encoding">编码</param>
        public static string DesEncrypt(object value, string key, Encoding encoding)
        {
            string text = value.SafeString();
            if (ValidateDes(text, key) == false)
                return string.Empty;
            using (var transform = CreateDesProvider(key).CreateEncryptor())
            {
                return GetEncryptResult(text, encoding, transform);
            }
        }

        /// <summary>
        /// 验证Des加密参数
        /// </summary>
        private static bool ValidateDes(string text, string key)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(key))
                return false;
            return key.Length == 24;
        }

        /// <summary>
        /// 创建Des加密服务提供程序
        /// </summary>
        private static TripleDESCryptoServiceProvider CreateDesProvider(string key)
        {
            return new TripleDESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(key), Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
        }

        /// <summary>
        /// 获取加密结果
        /// </summary>
        private static string GetEncryptResult(string value, Encoding encoding, ICryptoTransform transform)
        {
            var bytes = encoding.GetBytes(value);
            var result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return System.Convert.ToBase64String(result);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        public static string DesDecrypt(object value)
        {
            return DesDecrypt(value, DesKey);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,24位</param>
        public static string DesDecrypt(object value, string key)
        {
            return DesDecrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,24位</param>
        /// <param name="encoding">编码</param>
        public static string DesDecrypt(object value, string key, Encoding encoding)
        {
            string text = value.SafeString();
            if (!ValidateDes(text, key))
                return string.Empty;
            using (var transform = CreateDesProvider(key).CreateDecryptor())
            {
                return GetDecryptResult(text, encoding, transform);
            }
        }

        /// <summary>
        /// 获取解密结果
        /// </summary>
        private static string GetDecryptResult(string value, Encoding encoding, ICryptoTransform transform)
        {
            var bytes = System.Convert.FromBase64String(value);
            var result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return encoding.GetString(result);
        }


        #region  RoadFlowCore加密解密

        // Fields
        private static readonly string encryptKey = "4h!@w$rn";

        /// <summary>
        ///RoadFlowCore DES解密
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public static string DESDecrypt(string decryptString)
        {
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                int num = decryptString.Length / 2;
                byte[] buffer = new byte[num];
                for (int i = 0; i < num; i++)
                {
                    int num3 = Convert.ToInt32(decryptString.Substring(i * 2, 2), 0x10);
                    buffer[i] = (byte)num3;
                }
                string encryptKey = EncryptHelper.encryptKey;
                provider.Key = Encoding.ASCII.GetBytes(encryptKey);
                provider.IV = Encoding.ASCII.GetBytes(encryptKey);
                MemoryStream stream = new MemoryStream();
                CryptoStream stream1 = new CryptoStream((Stream)stream, provider.CreateDecryptor(), (CryptoStreamMode)CryptoStreamMode.Write);
                stream1.Write(buffer, 0, buffer.Length);
                stream1.FlushFinalBlock();
                return Encoding.Default.GetString(stream.ToArray());
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// RoadFlowCore DES加密
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string DESEncrypt(string encryptString)
        {
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                byte[] bytes = Encoding.Default.GetBytes(encryptString);
                string encryptKey = EncryptHelper.encryptKey;
                provider.Key = Encoding.ASCII.GetBytes(encryptKey);
                provider.IV = Encoding.ASCII.GetBytes(encryptKey);
                MemoryStream stream1 = new MemoryStream();
                CryptoStream stream2 = new CryptoStream((Stream)stream1, provider.CreateEncryptor(), (CryptoStreamMode)CryptoStreamMode.Write);
                stream2.Write(bytes, 0, bytes.Length);
                stream2.FlushFinalBlock();
                StringBuilder builder = new StringBuilder();
                foreach (byte num2 in stream1.ToArray())
                {
                    builder.AppendFormat("{0:X2}", (byte)num2);
                }
                return builder.ToString();
            }
            catch
            {
                return "";
            }
        }



        /// <summary>
        /// SHA1
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SHA1_(string str)
        {
            return BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", "");
        }



        #endregion



        #endregion

        #region AES加密

        /// <summary>
        /// 128位0向量
        /// </summary>
        private static byte[] _iv;
        /// <summary>
        /// 128位0向量
        /// </summary>
        private static byte[] Iv
        {
            get
            {
                if (_iv == null)
                {
                    var size = 16;
                    _iv = new byte[size];
                    for (int i = 0; i < size; i++)
                        _iv[i] = 0;
                }
                return _iv;
            }
        }

        /// <summary>
        /// AES密钥
        /// </summary>
        public static string AesKey = "QaP1AF8utIarcBqdhYTZpVGbiNQ9M6IL";

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        public static string AesEncrypt(string value)
        {
            return AesEncrypt(value, AesKey);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public static string AesEncrypt(string value, string key)
        {
            return AesEncrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public static string AesEncrypt(string value, string key, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rijndaelManaged = CreateRijndaelManaged(key, encoding);
            using (var transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV))
            {
                return GetEncryptResult(value, encoding, transform);
            }
        }

        /// <summary>
        /// 创建RijndaelManaged
        /// </summary>
        private static RijndaelManaged CreateRijndaelManaged(string key, Encoding encoding, CipherMode cipherMode = CipherMode.CBC)
        {
            return new RijndaelManaged
            {
                Key = encoding.GetBytes(key),
                Mode = cipherMode,
                Padding = PaddingMode.PKCS7,
                IV = Iv
            };
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        public static string AesDecrypt(string value)
        {
            return AesDecrypt(value, AesKey);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥</param>
        public static string AesDecrypt(string value, string key)
        {
            return AesDecrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        /// <param name="cipherMode">密码模式</param>
        public static string AesDecrypt(string value, string key, Encoding encoding, CipherMode cipherMode = CipherMode.CBC)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rijndaelManaged = CreateRijndaelManaged(key, encoding, cipherMode);
            using (var transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV))
            {
                return GetDecryptResult(value, encoding, transform);
            }
        }

        #endregion

        #region RSA签名



        #region 签名方法
        /// <summary>
        /// RSA加密，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public static string RsaSign(string value, string key)
        {
            return RsaSign(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// RSA加密，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public static string RsaSign(string value, string key, Encoding encoding)
        {
            return RsaSign(value, key, encoding, RSAType.RSA);
        }

        /// <summary>
        /// RSA加密，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public static string Rsa2Sign(string value, string key)
        {
            return Rsa2Sign(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// RSA加密，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public static string Rsa2Sign(string value, string key, Encoding encoding)
        {
            return RsaSign(value, key, encoding, RSAType.RSA2);
        }

        /// <summary>
        /// Rsa加密
        /// </summary>
        private static string RsaSign(string value, string key, Encoding encoding, RSAType type)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rsa = new RsaHelper(type, encoding, key);
            return rsa.Sign(value);
        }

        #endregion


        #region  公钥加密
        /// <summary>
        /// Rsa公钥加密
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="encoding"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string EncryptRsa(string value, string key, Encoding encoding, RSAType type)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rsa = new RsaHelper(type, encoding, publicKey: key);

            //方法一
            //  return rsa.EncryptRsa(value);
            return rsa.Encrypt(value);
        }


        /// <summary>
        /// Rsa公钥加密，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public static string EncryptRsa(string value, string key)
        {
            return EncryptRsa(value, key, Encoding.UTF8, RSAType.RSA);
        }


        #endregion


        #region 私钥解密
        /// <summary>
        /// Rsa私钥解密
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="encoding"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string DecryptRsa(string value, string key, Encoding encoding, RSAType type)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rsa = new RsaHelper(type, encoding, key);
            //方法一
            // return rsa.DecryptRsa(value);
            return rsa.Decrypt(value);
        }



        /// <summary>
        /// RSA私钥解密，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public static string DecryptRsa(string value, string key)
        {
            return DecryptRsa(value, key, Encoding.UTF8, RSAType.RSA);
        }

        #endregion







        #region 验证方法

        /// <summary>
        /// Rsa验签，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        public static bool RsaVerify(string value, string publicKey, string sign)
        {
            return RsaVerify(value, publicKey, sign, Encoding.UTF8);
        }

        /// <summary>
        /// Rsa验签，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        /// <param name="encoding">编码</param>
        public static bool RsaVerify(string value, string publicKey, string sign, Encoding encoding)
        {
            return RsaVerify(value, publicKey, sign, encoding, RSAType.RSA);
        }

        /// <summary>
        /// Rsa验签，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        public static bool Rsa2Verify(string value, string publicKey, string sign)
        {
            return Rsa2Verify(value, publicKey, sign, Encoding.UTF8);
        }

        /// <summary>
        /// Rsa验签，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        /// <param name="encoding">编码</param>
        public static bool Rsa2Verify(string value, string publicKey, string sign, Encoding encoding)
        {
            return RsaVerify(value, publicKey, sign, encoding, RSAType.RSA2);
        }

        /// <summary>
        /// Rsa验签
        /// </summary>
        private static bool RsaVerify(string value, string publicKey, string sign, Encoding encoding, RSAType type)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(sign))
                return false;
            var rsa = new RsaHelper(type, encoding, publicKey: publicKey);
            return rsa.Verify(value, sign);
        }
        #endregion



        #endregion



        #region HmacSha256加密

        /// <summary>
        /// HMACSHA256加密
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="key">密钥</param>
        public static string HmacSha256(string value, string key)
        {
            return HmacSha256(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// HMACSHA256加密
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">字符编码</param>
        public static string HmacSha256(string value, string key, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var sha256 = new HMACSHA256(encoding.GetBytes(key));
            var hash = sha256.ComputeHash(encoding.GetBytes(value));
            return string.Join("", hash.ToList().Select(t => t.ToString("x2")).ToArray());
        }

        #endregion

        #region 3DES 加密解密
        /// <summary>
        ///3DES加密
        /// </summary>
        /// <param name="originalValue">加密数据</param>
        /// <param name="key">24位字符的密钥字符串</param>
        /// <param name="IV">8位字符的初始化向量字符串</param>
        /// <returns></returns>
        public static string DES3Encrypt(string originalValue, string key, string IV)
        {

            SymmetricAlgorithm sa;
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            sa = new TripleDESCryptoServiceProvider();
            sa.Key = Encoding.UTF8.GetBytes(key);
            sa.IV = Encoding.UTF8.GetBytes(IV);
            ct = sa.CreateEncryptor();
            byt = Encoding.UTF8.GetBytes(originalValue);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }
        /// <summary>
        /// 3DES解密
        /// </summary>
        /// <param name="data">解密数据</param>
        /// <param name="key">24位字符的密钥字符串(需要和加密时相同)</param>
        /// <param name="iv">8位字符的初始化向量字符串(需要和加密时相同)</param>
        /// <returns></returns>
        public static string DES3Decrypst(string data, string key, string IV)
        {
            SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();
            mCSP.Key = Encoding.UTF8.GetBytes(key);
            mCSP.IV = Encoding.UTF8.GetBytes(IV);
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);
            byt = Convert.FromBase64String(data);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Encoding.UTF8.GetString(ms.ToArray());

        }


        #endregion

    }
}
