/*----------------------------------------------------------------
// Copyright (C) 2007 ���ϻ��� ��Ȩ���С� 
//
// �ļ�����IUserStateContainer.cs
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
    public interface IUserStateContainer
    {
        void Add(string uid);
        List<string> GetList();
        bool Check(string uid);
        void Remove(string uid);
        int GetUserCount();
    }
}
