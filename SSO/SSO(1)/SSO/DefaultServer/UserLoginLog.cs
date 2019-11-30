/*----------------------------------------------------------------
// Copyright (C) 2007 ���ϻ��� ��Ȩ���С� 
//
// �ļ�����UserLoginLog.cs
// �ļ�����������
// 
// ������ʶ��jillzhang
// �޸ı�ʶ��
// �޸�������
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SSO
{
    public class UserLoginLog:IUserLoginList
    {
        private static readonly UserLoginLog instance = new UserLoginLog();
        public static Hashtable table = new Hashtable();
        private UserLoginLog()
        {

        }
        public static UserLoginLog CreateInstance()
        {
            return instance;
        }
        public void Add(string uid, string siteID)
        {
            lock (table)
            {
                List<string> sites = new List<string>();
                if (table.Contains(uid))
                {
                    sites = table[uid] as List<string>;
                }
                if(sites.Contains(siteID))
                {
                    return;
                }
                sites.Add(siteID);
                table[uid] = sites;
            }
        }
        public List<string> GetLoginSites(string uid)
        {          
            lock (table)
            {
                List<string> sites = new List<string>();
                if (table.Contains(uid))
                {
                    sites = table[uid] as List<string>;
                }
                return sites;
            }             
        }
        public void DeleteUser(string uid)
        {
            lock (table)
            {
                table.Remove(uid);            
            }
        }
    }
}
