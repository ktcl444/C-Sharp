/*----------------------------------------------------------------
// Copyright (C) 2007 ���ϻ��� ��Ȩ���С� 
//
// �ļ�����UserState.cs
// �ļ�����������
// 
// ������ʶ��jillzhang
// �޸ı�ʶ��
// �޸�������
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
