/*----------------------------------------------------------------
// Copyright (C) 2007 蚂蚁互动 版权所有。 
//
// 文件名：UserState.cs
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
    public class UserState : IUserStateContainer
    {
        private static List<string> users = new List<string>();
        private static readonly UserState instance = new UserState();
        private UserState()
        {
        }
        public static UserState Instance
        {
            get
            {
                return instance;
            }
        }
        public void Add(string uid)
        {
            lock (users)
            {
                bool has = false;
                foreach (string s in users)
                {
                    if (s == uid)
                    {
                        has = true;
                        break;
                    }
                }
                if (!has)
                {
                    users.Add(uid);
                }
            }
        }
        public List<string> GetList()
        {
            return users;
        }
        public bool Check(string uid)
        {
            lock (users)
            {
                foreach (string s in users)
                {
                    if (s == uid)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void  Remove(string uid)
        {
            lock (users)
            {
                users.Remove(uid);
            }
        }

        public int GetUserCount()
        {
            lock (users)
            {
                return users.Count;
            }
        }
    }
}
