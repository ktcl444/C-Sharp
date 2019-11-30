using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SSO
{
    public class SSOUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                Random random = new Random(unchecked(i * (int)(DateTime.Now.Ticks)));

                int ret = random.Next(122);

                while (ret < 48 || (ret > 57 && ret < 65) || (ret > 90 && ret < 97))
                {
                    ret = random.Next(122);
                }

                sb.Append((char)ret);
            }
            return sb.ToString();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DESEncrypt(string text, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = System.Security.Cryptography.CipherMode.ECB;
            des.Padding = PaddingMode.Zeros;
            des.Key = ASCIIEncoding.ASCII.GetBytes(key);

            byte[] inputBuffer = Encoding.GetEncoding("UTF-8").GetBytes(text);
            byte[] outputBuffer = des.CreateEncryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

            return Convert.ToBase64String(outputBuffer);
        }

        public static string DESDecrypt(string text, string key)
        {

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = System.Security.Cryptography.CipherMode.ECB;
            des.Padding = PaddingMode.Zeros;
            des.Key = ASCIIEncoding.ASCII.GetBytes(key);

            byte[] inputBuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = des.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

            return Encoding.GetEncoding("UTF-8").GetString(outputBuffer);
        }

        public static string GetSiteUrl()
        {
            string path = HttpContext.Current.Request.ApplicationPath;
            if (path.EndsWith("/") && path.Length == 1)
            {
                return GetHostUrl();
            }
            else
            {
                return GetHostUrl() + path;
            }
        }

        public static string GetHostUrl()
        {
            return string.Format("{0}://{1}:{2}",
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Host,
                HttpContext.Current.Request.Url.Port);
        }
    }
}
