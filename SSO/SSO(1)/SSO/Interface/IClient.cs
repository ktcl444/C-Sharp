/*----------------------------------------------------------------
// Copyright (C) 2007 ���ϻ��� ��Ȩ���С� 
//
// �ļ�����IClient.cs
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
