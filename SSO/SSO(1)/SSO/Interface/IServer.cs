/*----------------------------------------------------------------
// Copyright (C) 2007 ���ϻ��� ��Ȩ���С� 
//
// �ļ�����IServer.cs
// �ļ�����������
// 
// ������ʶ��jillzhang
// �޸ı�ʶ��
// �޸�������
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
