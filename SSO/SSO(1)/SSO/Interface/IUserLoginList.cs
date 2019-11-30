/*----------------------------------------------------------------
// Copyright (C) 2007 蚂蚁互动 版权所有。 
//
// 文件名：IUserLoginList.cs
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
    public interface IUserLoginList
    {
        void Add(string uid, string siteID);
        List<string> GetLoginSites(string uid);
        void DeleteUser(string uid);       
    }
}
