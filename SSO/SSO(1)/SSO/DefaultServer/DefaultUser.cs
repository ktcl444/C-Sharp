/*----------------------------------------------------------------
// Copyright (C) 2007 ���ϻ��� ��Ȩ���С� 
//
// �ļ�����DefaultUser.cs
// �ļ�����������
// 
// ������ʶ��jillzhang
// �޸ı�ʶ��
// �޸�������
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace SSO
{
    public class DefaultUser:IUser
    {
        private string _uid;
        string cfgPath = "";
        XmlDocument doc = new XmlDocument();
        public DefaultUser()
        {
            cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.config");
            doc.Load(cfgPath);
        }
        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        private string _pwd;

        public string Pwd
        {
            get { return _pwd; }
            set { _pwd = value; }
        }
        public bool Validate()
        {
            XmlElement root = doc.DocumentElement;
            XmlNode n = root.SelectSingleNode("user[@uid='" + _uid + "']");
            if (n != null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// ע�����û�
        /// </summary>
        /// <returns>1-ע��ɹ� 0- �Ѿ����ڵ��û����</returns>
        public void Register()
        {
            XmlElement root = doc.DocumentElement;       
            XmlNode n = doc.CreateElement("user");
            XmlAttribute att = doc.CreateAttribute("uid");
            att.Value = _uid;
            n.Attributes.Append(att);
            att = doc.CreateAttribute("pwd");
            att.Value = _pwd;
            n.Attributes.Append(att);
            root.AppendChild(n);
            doc.Save(cfgPath);           
        }   
    }
}
