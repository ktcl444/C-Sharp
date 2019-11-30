/*----------------------------------------------------------------
// Copyright (C) 2007 蚂蚁互动 版权所有。 
//
// 文件名：IServer.cs
// 文件功能描述：
// 
// 创建标识：jillzhang
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace SSO
{
    public interface IServer
    {
        ISite Site { get;}
        string Uid { get;}
        void SaveToken(IUser user);
        string CheckExistToken();
        int CheckUser(IUser user);
        void Jump(string uid, string defaultJumpUrl);
        void LogOut(string uid);
    }
}
