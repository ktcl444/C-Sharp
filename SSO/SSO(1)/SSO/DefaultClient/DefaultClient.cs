/*----------------------------------------------------------------
// Copyright (C) 2007 蚂蚁互动 版权所有。 
//
// 文件名：DefaultClient.cs
// 文件功能描述：
// 
// 创建标识：jillzhang
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;

namespace SSO
{
    public  class DefaultClient:IClient
    {
        private string _loginAddress = "";
        private string _logoutAddress = "";
        private string _uidFiled = "uid";
        private string _fromUrlField = "fromurl";
        private string _privateKey = ConfigurationManager.AppSettings["privatekey"];
        private string _publicKey = ConfigurationManager.AppSettings["publickey"];
        private string _siteID = ConfigurationManager.AppSettings["siteid"];
        private int _timeOut = Convert.ToInt32(ConfigurationManager.AppSettings["timeout"]);
        private string _uid = "";
        private ILoginRequestContainer loginRequestContainer = LoginRequestContainer.Instance;
        private IUserStateContainer userContainer = UserState.Instance;
        private RSACryption rsa = new RSACryption();

        public DefaultClient()
        {
            this._loginAddress = ConfigurationManager.AppSettings["loginaddress"];
            this._logoutAddress = ConfigurationManager.AppSettings["logoutaddress"];
            this._uidFiled = ConfigurationManager.AppSettings["uidfield"];
            this._fromUrlField = ConfigurationManager.AppSettings["fromurlfield"];
        }

        private bool CheckState()
        {
            HttpContext current = HttpContext.Current;
            if (current.Session["uid"] == null)
            {
                return false;
            }
            this._uid = current.Session["uid"].ToString();
            return true;
        }

        public bool Login(out string exInfo)
        {
            HttpContext current = HttpContext.Current;
            exInfo = "";
            if (!this.CheckState())
            {
                if (current.Request.QueryString["uid"] == null)
                {
                    this.TryLogin();
                }
                string userIndentity = "";
                if (!this.Resolve(out userIndentity, out exInfo))
                {
                    return false;
                }
                this.SaveToken(userIndentity);
            }
            return true;
        }

        public void LogOut()
        {
            HttpContext current = HttpContext.Current;
            if (current.Session["uid"] != null)
            {
                string str = current.Session["uid"].ToString();
                string str2 = this._siteID + "|" + str + "|";
                string strHashData = "";
                this.rsa.GetHash(str2, ref strHashData);
                string str4 = "";
                this.rsa.SignatureFormatter(this._privateKey, strHashData, ref str4);
                string url = UrlOper.AddParam(UrlOper.AddParam(UrlOper.AddParam(UrlOper.AddParam(this._logoutAddress, "siteid", this._siteID), "signtext", str4), _uidFiled, str), "loginid", "");
                current.Response.Redirect(url);
            }
        }

        private bool Resolve(out string userIndentity, out string exInfo)
        {
            HttpContext current = HttpContext.Current;
            DESCryption cryption = new DESCryption();
            userIndentity = "";
            exInfo = "";
            string str = current.Request.QueryString[_uidFiled];
            string str2 = this.rsa.RSADecrypt(this._privateKey, str);
            string str3 = current.Request.QueryString["signtext"];
            string str4 = current.Request.QueryString["timestamp"];
            string id = current.Request.QueryString["loginid"];
            if (!this.loginRequestContainer.Check(id))
            {
                exInfo = "登录请求已经过期或者是伪造请求!";
                return false;
            }
            this.loginRequestContainer.Remove(id);
            if (string.IsNullOrEmpty(str3) || string.IsNullOrEmpty(str4))
            {
                exInfo = "数据被非法篡改";
                return false;
            }
            DateTime time = Convert.ToDateTime(str4);
            TimeSpan span = (TimeSpan)(DateTime.Now - time);
            string str6 = str2 + "|" + str4 + "|" + id;
            string strHashData = "";
            this.rsa.GetHash(str6, ref strHashData);
            if (!this.rsa.SignatureDeformatter(this._privateKey, strHashData, str3))
            {
                exInfo = "数据被非法篡改";
                return false;
            }
            if (span.TotalSeconds > this._timeOut)
            {
                exInfo = "超时";
                return false;
            }
            userIndentity = str2;
            return true;
        }

        private void SaveToken(string uid)
        {
            HttpContext current = HttpContext.Current;
            current.Session["uid"] = uid;
            this._uid = current.Session["uid"].ToString();
            userContainer.Add(this._uid);
        }

        private void TryLogin()
        {
            HttpContext current = HttpContext.Current;
            LoginRequest r = new LoginRequest();
            string str = this._siteID + "||" + r.Identity;
            string strHashData = "";
            this.rsa.GetHash(str, ref strHashData);
            string str3 = "";
            this.rsa.SignatureFormatter(this._privateKey, strHashData, ref str3);
            string url = UrlOper.AddParam(UrlOper.AddParam(UrlOper.AddParam(UrlOper.AddParam(UrlOper.AddParam(this._loginAddress, "siteid", this._siteID), "loginid", r.Identity), "signtext", str3),_uidFiled, ""), _fromUrlField, current.Request.Url.ToString());
            this.loginRequestContainer.Add(r);
            current.Response.Redirect(url);
        }

        public string LoginAddress
        {
            get
            {
                return this._loginAddress;
            }
        }

        public string LogoutAddress
        {
            get
            {
                return this._logoutAddress;
            }
        }
        public string UidField
        {
            get
            {
                return this._uidFiled;
            }
        }

        public string FromUrlField
        {
            get
            {
                return this._fromUrlField;
            }
        }


        public string PrivateKey
        {
            get
            {
                return this._privateKey;
            }
        }

        public string SiteID
        {
            get
            {
                return this._siteID;
            }
        }

        public int TimeOut
        {
            get
            {
                return this._timeOut;
            }
        }

        public string Uid
        {
            get
            {
                return this._uid;
            }
        }
    }
}

