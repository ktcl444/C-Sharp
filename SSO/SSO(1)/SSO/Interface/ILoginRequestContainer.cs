/*----------------------------------------------------------------
// Copyright (C) 2007 ���ϻ��� ��Ȩ���С� 
//
// �ļ�����ILoginRequestContainer.cs
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
    public interface ILoginRequestContainer
    {
        void Add(ILoginRequest r);
        bool Check(string id);
        void Remove(string id);
    }
}
