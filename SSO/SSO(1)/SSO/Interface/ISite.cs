/*----------------------------------------------------------------
// Copyright (C) 2007 蚂蚁互动 版权所有。 
//
// 文件名：ISite.cs
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
    public interface ISite
    {
        string SiteID { get;set;}
        string PublicKey { get;set;}
        string PublicAndPrivateKey { get;set;}

        #region 来源网址相关信息
        /// <summary>
        /// 来源地址的键
        /// </summary>
        string FromUrlKey { get;set;}     
        #endregion       

        string HomePage { get;set;}

        string UidField { get;set;}

        string LogOutUrl { get;set;}        

        void Add();

        bool Validate();
    }
}
