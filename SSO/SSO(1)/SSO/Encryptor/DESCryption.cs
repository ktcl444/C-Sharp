/*----------------------------------------------------------------
// Copyright (C) 2007 蚂蚁互动 版权所有。 
//
// 文件名：DesCryption.cs
// 文件功能描述：
// 
// 创建标识：jillzhang
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SSO
{
    public class DESCryption
    {
        DESCryptoServiceProvider des;
        Encoding encoding = new UnicodeEncoding();
        public DESCryption()
        {

        }
        public void Dispose()
        {
            des.Clear();
        }
        public void CreateProvider(out string key, out string iv)
        {
            key = "";
            iv = "";
            des = new DESCryptoServiceProvider();
            des.GenerateIV();
            des.GenerateKey();
            iv = encoding.GetString(des.IV);
            byte[] buffer1 = encoding.GetBytes(iv);
            key = encoding.GetString(des.Key);
        }
        #region ========加密========
        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public string Encrypt(string Text)
        {
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cs);
            sw.Write(Text);
            sw.Close();
            cs.Close();
            byte[] buffer = ms.ToArray();
            ms.Close();
            return Convert.ToBase64String(buffer);
        }
        #endregion

        #region ========解密========
        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public string Decrypt(string Text, string key, string iv)
        {
            byte[] keyBuffer = encoding.GetBytes(key);
            byte[] ivBuffer = encoding.GetBytes(iv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Convert.FromBase64String(Text);
            des.Key = keyBuffer;
            des.IV = ivBuffer;
            System.IO.MemoryStream ms = new System.IO.MemoryStream(inputByteArray);
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            string val = sr.ReadLine();
            cs.Close();
            ms.Close();
            des.Clear();
            return val;
        }
        #endregion
    }
}
