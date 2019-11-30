using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;

namespace SSO
{
    public class RSAHelper
    {
        private const string publicKey = "<RSAKeyValue><Modulus>mE8PFoNc6IoMFam4zCquflCkquFB823eNc6lob2yRqSHx8LFMCpttcF2zEZ5I3uSiOGBP8aijTjD8JSk7TvU+EtZfP04hL+nARAGyQM1u+FiabsBOQlkDbOq59dkThmjSPdHw0x/igRdFHe1Uwru4nRbKs1OWczzUsJWWDLV6+0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private const string privateKey = "<RSAKeyValue><Modulus>mE8PFoNc6IoMFam4zCquflCkquFB823eNc6lob2yRqSHx8LFMCpttcF2zEZ5I3uSiOGBP8aijTjD8JSk7TvU+EtZfP04hL+nARAGyQM1u+FiabsBOQlkDbOq59dkThmjSPdHw0x/igRdFHe1Uwru4nRbKs1OWczzUsJWWDLV6+0=</Modulus><Exponent>AQAB</Exponent><P>1wBBFlOVkA8O8txRISj9ql8wiuAz3jGTKCaGWnojWqjIhw9W84mPbosrzDS59ZaboVJ2198zRl6jwThR0imR8Q==</P><Q>tVpZRyb1MfLPZsRG0eSV9J05+28DX2FcZWHUCzsVBgJXlBiBaRfgi2eR32a9yNbSZXpqUIr5nbdhXuEP2Yr9vQ==</Q><DP>HvtmZbU9xDinSs/80O57P2XgNOMCFm7Gae7DRZ58IcBYxT2spgOYq7Faal7evUkqvCCKB6meVfGlX16iS8q5wQ==</DP><DQ>FhlGW8dBha6i21D7mEQUidRG5n6mmI7SpYAASMYQT8UlSuSZkGbac+JRAjoQ0lJrHPaH0fy9YhygfuFJ/yZSuQ==</DQ><InverseQ>t9Wr+33I+/wMUuEwzF1MjBZ+LfCIK7HqSxR3kv1oxr1sjq3uQXn+feJQQISNyKM44sNJxWa8TodVbtjwidsmRg==</InverseQ><D>HuqVXnWFy3ISJ+eOqmrThrJp6oHU+EvJ+lQbDOzLnklRgnwHuNIz+NvveGGpv0kbIovbx41Te6UVKOWTYNBvVzLH3EqernsNJTGvJrBn5lwsBhS1OWYc6ajGBL4Z0yf7v5eHJxJPnCuXEE1XxksOUArZAhvlcTRHj7Wp+ETPwcE=</D></RSAKeyValue>";

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="key">公钥</param>
        /// <returns></returns>
        public static string Encrypt(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            else
            {
                key = string.IsNullOrEmpty(key) ? publicKey : key;
                Byte[] b;
                RSACryptoServiceProvider rsaEnc = new RSACryptoServiceProvider();
                try
                {
                    rsaEnc.FromXmlString(key);

                    b = rsaEnc.Encrypt(Encoding.UTF8.GetBytes(text), false);
                }
                catch (Exception)
                {
                    return string.Empty;
                }
                return Convert.ToBase64String(b);
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="key">私钥</param>
        /// <returns></returns>
        public static string Decrypt(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            else
            {
                key = string.IsNullOrEmpty(key) ? privateKey : key;
                Byte[] b = Convert.FromBase64String(text);
                RSACryptoServiceProvider rsaEnc = new RSACryptoServiceProvider();
                try
                {
                    rsaEnc.FromXmlString(key);
                    b = rsaEnc.Decrypt(b, false);
                }
                catch (Exception)
                {
                    return string.Empty;
                }
                return Encoding.UTF8.GetString(b);
            }
        }

        //获取Hash描述表 
        public static bool GetHash(string m_strSource, ref string strHashData)
        {
            //从字符串中取得Hash描述 
            try
            {
                byte[] Buffer;
                byte[] HashData;
                HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
                Buffer = Encoding.Default.GetBytes(m_strSource);
                HashData = MD5.ComputeHash(Buffer);

                strHashData = Convert.ToBase64String(HashData);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region RSA签名
        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="p_strKeyPrivate">私钥</param>
        /// <param name="m_strHashbyteSignature">hash码</param>
        /// <param name="m_strEncryptedSignatureData">签名数据</param>
        /// <returns></returns>
        public static bool SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref string m_strEncryptedSignatureData)
        {
            try
            {
                p_strKeyPrivate = string.IsNullOrEmpty(p_strKeyPrivate) ? privateKey : p_strKeyPrivate;
                byte[] HashbyteSignature;
                byte[] EncryptedSignatureData;

                HashbyteSignature = Convert.FromBase64String(m_strHashbyteSignature);
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

                RSA.FromXmlString(p_strKeyPrivate);
                RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
                //设置签名的算法为MD5 
                RSAFormatter.SetHashAlgorithm("MD5");
                //执行签名 
                EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);

                m_strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region RSA 签名验证
        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="p_strKeyPublic">公钥</param>
        /// <param name="p_strHashbyteDeformatter">hash码</param>
        /// <param name="p_strDeformatterData">验证数据</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, string p_strDeformatterData)
        {
            try
            {
                p_strKeyPublic = string.IsNullOrEmpty(p_strKeyPublic) ? publicKey : p_strKeyPublic;
                byte[] DeformatterData;
                byte[] HashbyteDeformatter;

                HashbyteDeformatter = Convert.FromBase64String(p_strHashbyteDeformatter);
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

                RSA.FromXmlString(p_strKeyPublic);
                RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
                //指定解密的时候HASH算法为MD5 
                RSADeformatter.SetHashAlgorithm("MD5");

                DeformatterData = Convert.FromBase64String(p_strDeformatterData);

                if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
