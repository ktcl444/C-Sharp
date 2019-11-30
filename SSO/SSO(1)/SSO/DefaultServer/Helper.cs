/*----------------------------------------------------------------
// Copyright (C) 2007 蚂蚁互动 版权所有。 
//
// 文件名：Helper.cs
// 文件功能描述：
// 
// 创建标识：jillzhang
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;

namespace SSO
{
    public class Helper
    {
        public static string GetUrl(string fromUrl, string uidText, string loginId, ISite site)
        {
            string url = "";          
            RSACryption cryption = new RSACryption();
            string str2 = DateTime.Now.ToString();
            string str3 = cryption.RSAEncrypt(site.PublicKey, uidText);
            url = UrlOper.AddParam(UrlOper.AddParam(UrlOper.AddParam(fromUrl, site.UidField, str3), "timestamp", str2), "loginid", loginId);
            string str4 = uidText + "|" + str2 + "|" + loginId;
            string strHashData = "";
            cryption.GetHash(str4, ref strHashData);
            string str6 = "";
            cryption.SignatureFormatter(site.PublicAndPrivateKey, strHashData, ref str6);
            return UrlOper.AddParam(url, "signtext", str6);
        }
    }
}
