/*----------------------------------------------------------------
// Copyright (C) 2007 ���ϻ��� ��Ȩ���С� 
//
// �ļ�����IUserData.cs
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
    public interface IUser
    {
        string Uid { get;set;}
        string Pwd { get;set;}
        bool Validate();
        void Register();     
    }
}
