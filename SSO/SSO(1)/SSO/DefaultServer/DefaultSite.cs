/*----------------------------------------------------------------
// Copyright (C) 2007 蚂蚁互动 版权所有。 
//
// 文件名：DefaultSite.cs
// 文件功能描述：
// 
// 创建标识：jillzhang
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace SSO
{
    public class DefaultSite:ISite
    {
        #region 私有字段
        private string _siteID = "";
        private string _publicKey="";
        private string _privateAndPublicKey = "";
        private SecurityType _securityLevel = SecurityType.PlainText;
        private string _fromUrlKey = "fromurl";
        private string _cryptkeyFiled = "key";
        private string _cryptivFiled = "iv";
        private string _homePage = "";
        private string _uidField = "uid";
        private string _logOutUrl = "";
        private int _timeOut = 10;
        string cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sites.config");
        XmlDocument doc = new XmlDocument();
        #endregion

        public DefaultSite()
        {
            doc.Load(cfgPath);
        }

        #region 加载某一站点的信息
        /// <summary>
        /// 加载某一站点的信息
        /// </summary>
        /// <param name="siteID"></param>
        public DefaultSite(string siteID)
        {
            doc.Load(cfgPath);
            XmlElement root = doc.DocumentElement;
            XmlNode node = root.SelectSingleNode(string.Format("site[@siteid='{0}']", siteID));
            if (node == null)
            {
                throw new Exception(string.Format("没有编号为{0}的站点配置信息",siteID));                
            }
            XmlAttribute att = node.Attributes["publickey"];
            if (att != null)
            {
                _publicKey = att.Value;
            }
            att = node.Attributes["publicandprivatekey"];
            if (att != null)
            {
                _privateAndPublicKey = att.Value;
            }
            att = node.Attributes["securitylevel"];
            if (att != null)
            {
                string level = att.Value;
                if (level != "Encrypted")
                {
                    _securityLevel = SecurityType.PlainText;
                }
                else
                {
                    _securityLevel = SecurityType.Encrypted;
                }
            }
            if (_securityLevel == SecurityType.Encrypted&&(string.IsNullOrEmpty(_privateAndPublicKey)||string.IsNullOrEmpty(_publicKey)))
            {
                throw new Exception("设置了加密，但没有设置公钥或者私钥！");
            }
            att = node.Attributes["fromurlkey"];
              if (att != null)
              {
                  _fromUrlKey = att.Value;
              }
              att = node.Attributes["cryptkeyfield"];
              if (att != null)
              {
                  _cryptkeyFiled = att.Value;
              }
              att = node.Attributes["ivkeyfield"];
              if (att != null)
              {
                  _cryptivFiled = att.Value;
              }
            this._siteID = siteID;
            att = node.Attributes["homepage"];
            if (att != null)
            {
                _homePage = att.Value;
            }
            att = node.Attributes["uidfield"];
            if (att != null)
            {
                _uidField = att.Value;
            }
            att = node.Attributes["logouturl"];
            if (att != null)
            {
                _logOutUrl = att.Value;
            }
            att = node.Attributes["timeout"];
            if (att != null)
            {
                _timeOut = Convert.ToInt32(att.Value);
            }
        }
        #endregion

        #region 公共属性
        public string SiteID
        {
            get
            {
                return _siteID;
            }
            set
            {
                _siteID = value;
            }
        }
        public string PublicKey
        {
            get
            {
                return _publicKey;
            }
            set
            {
                 _publicKey =value;
            }
        }
        public string PublicAndPrivateKey
        {
            get
            {
                return _privateAndPublicKey;
            }
            set
            {
                _privateAndPublicKey = value;
            }
        }
        public string FromUrlKey
        {
            get
            {
                return _fromUrlKey;
            }
            set
            {
                _fromUrlKey = value;
            }
        }
        public string CryptKeyFiled
        {
            get
            {
                return _cryptkeyFiled;
            }
            set
            {
                _cryptkeyFiled = value;
            }
        }
        public string IVKeyField
        {
            get
            {
                return _cryptivFiled;
            }
            set
            {
                _cryptivFiled = value;
            }
        }
        public SecurityType SecurityLevel
        {
            get
            {
                return _securityLevel;
            }
            set
            {
                _securityLevel = value;
            }
        }
        public string HomePage
        {
            get
            {
                return _homePage;
            }
            set
            {
                _homePage = value;
            }
        }
        public string UidField
        {
            get
            {
                return _uidField;
            }
            set
            {
                _uidField = value;
            }
        }
        public string LogOutUrl
        {
            get
            {
                return _logOutUrl;
            }
            set
            {
                _logOutUrl = value;
            }
        }
        public int TimeOut
        {
            get
            {
                return _timeOut;
            }
            set
            {
                _timeOut = value;
            }
        }
        #endregion

        #region 验证站点是否存在
        public bool Validate()
        {
            XmlElement root = doc.DocumentElement;
            XmlNode node = root.SelectSingleNode(string.Format("site[@siteid='{0}']", _siteID));
            if (node == null)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 添加新站点
        public void Add()
        {
            XmlElement root = doc.DocumentElement;
            XmlNode node = doc.CreateElement("site");
            XmlAttribute att = doc.CreateAttribute("siteid");
            att.Value = _siteID;
            node.Attributes.Append(att);
            att = doc.CreateAttribute("securitylevel");
            att.Value = _securityLevel.ToString();
            node.Attributes.Append(att);
            att = doc.CreateAttribute("publickey");
            att.Value = _publicKey;
            node.Attributes.Append(att);
            att = doc.CreateAttribute("publicandprivatekey");
            att.Value = _privateAndPublicKey;
            node.Attributes.Append(att);
            att = doc.CreateAttribute("fromurlkey");
            att.Value = _fromUrlKey;
            node.Attributes.Append(att);
            att = doc.CreateAttribute("cryptkeyfield");
            att.Value = _cryptkeyFiled;
            node.Attributes.Append(att);
            att = doc.CreateAttribute("ivkeyfield");
            att.Value = _cryptivFiled;
            node.Attributes.Append(att);
            att = doc.CreateAttribute("homepage");
            att.Value = _homePage;
            node.Attributes.Append(att);
            att = doc.CreateAttribute("uidfield");
            att.Value = _uidField;
            node.Attributes.Append(att);
            att = doc.CreateAttribute("logouturl");
            att.Value = _logOutUrl;
            node.Attributes.Append(att);
            att = doc.CreateAttribute("timeout");
            att.Value = _timeOut.ToString();
            node.Attributes.Append(att);
            root.AppendChild(node);
            doc.Save(cfgPath);
        }
        #endregion
    }
}
