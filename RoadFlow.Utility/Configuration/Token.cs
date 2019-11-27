using Microsoft.IdentityModel.Tokens;
using RoadFlow.Utility.Cache;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace RoadFlow.Utility
{



    #region 新方法

    public class Token
    {
        // Methods
        public static string CreateToken(string key, string issuer, string audience, out DateTime expireTime)
        {
            string[] textArray1 = new string[] { key, "_", issuer, "_", audience };
            string str = string.Concat((string[])textArray1);
            object obj2 = IO.Get(str);
            if (obj2 != null)
            {
                ValueTuple<string, DateTime> tuple1 = (ValueTuple<string, DateTime>)obj2;
                string str2 = tuple1.Item1;
                expireTime = tuple1.Item2.ToLocalTime();
                return str2;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            DateTime time = DateTime.UtcNow;
            DateTime expiry = time.AddMinutes(120.0);
            JwtSecurityToken token = new JwtSecurityToken(issuer, audience, null, new DateTime?(time), new DateTime?(expiry), new SigningCredentials(new SymmetricSecurityKey(bytes), "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256"));
            string text1 = new JwtSecurityTokenHandler().WriteToken(token);
            ValueTuple<string, DateTime> tuple = new ValueTuple<string, DateTime>(text1, expiry);
            IO.Insert(str, tuple, expiry);
            expireTime = expiry;
            return text1;
        }

        public static ValueTuple<string, DateTime> ParseToken(string token)
        {
            JwtSecurityToken token2 = new JwtSecurityToken(token);
            DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0);
            return new ValueTuple<string, DateTime>(token2.Payload.Iss, time.AddSeconds((double)token2.Payload.Exp.Value).ToLocalTime());
        }
    }









    #endregion
}
