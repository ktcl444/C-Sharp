/*----------------------------------------------------------------
// Copyright (C) 2007 ���ϻ��� ��Ȩ���С� 
//
// �ļ�����ISite.cs
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
    public interface ISite
    {
        string SiteID { get;set;}
        string PublicKey { get;set;}
        string PublicAndPrivateKey { get;set;}

        #region ��Դ��ַ�����Ϣ
        /// <summary>
        /// ��Դ��ַ�ļ�
        /// </summary>
        string FromUrlKey { get;set;}     
        #endregion       

        string HomePage { get;set;}

        string UidField { get;set;}

        string LogOutUrl { get;set;}        

        void Add();

        bool Validate();
    }
}
