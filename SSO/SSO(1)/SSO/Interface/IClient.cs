/*----------------------------------------------------------------
// Copyright (C) 2007 蚂蚁互动 版权所有。 
//
// 文件名：IClient.cs
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
    public interface IClient
    {
        bool Login(out string exInfo);
        void LogOut();
        string SiteID { get;}
        string PrivateKey { get;}
        int TimeOut { get;}
        string LoginAddress { get;}
        string LogoutAddress { get;}
        string Uid { get;}
        string UidField { get;}
        string FromUrlField { get;}
    }
}
