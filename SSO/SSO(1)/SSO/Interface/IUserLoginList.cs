/*----------------------------------------------------------------
// Copyright (C) 2007 ���ϻ��� ��Ȩ���С� 
//
// �ļ�����IUserLoginList.cs
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
    public interface IUserLoginList
    {
        void Add(string uid, string siteID);
        List<string> GetLoginSites(string uid);
        void DeleteUser(string uid);       
    }
}
