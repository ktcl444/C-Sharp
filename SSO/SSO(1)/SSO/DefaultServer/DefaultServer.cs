/*----------------------------------------------------------------
// Copyright (C) 2007 蚂蚁互动 版权所有。 
//
// 文件名：SSOServer.cs
// 文件功能描述：
// 
// 创建标识：jillzhang
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;
using System.Web.UI;

namespace SSO
{
    public class DefaultServer:IServer
    {
        private string _loginId;
        private string _uid;
        private string cfgPath = "";
        private XmlDocument doc = new XmlDocument();
        private ISite site;

        public DefaultServer()
        {
            HttpContext current = HttpContext.Current;
            string str = current.Request.QueryString["siteid"];
            if (!string.IsNullOrEmpty(str))
            {
                this.site = new DefaultSite(str);
                RSACryption cryption = new RSACryption();
                string str2 = current.Request.QueryString["signtext"];
                string str3 = current.Request.QueryString[site.UidField];
                string str4 = current.Request.QueryString["loginid"];
                string str5 = this.site.SiteID + "|" + str3 + "|" + str4;
                string strHashData = "";
                cryption.GetHash(str5, ref strHashData);
                if (!cryption.SignatureDeformatter(this.site.PublicKey, strHashData, str2))
                {
                    this.site = null;
                    throw new Exception("登录请求数据被非法篡改");
                }
                this._uid = str3;
                this._loginId = str4;
            }
            this.cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.config");
            this.doc.Load(this.cfgPath);
        }

        public string CheckExistToken()
        {
            HttpContext current = HttpContext.Current;
            if (current.Session["uid"] != null)
            {
                if (this.site != null)
                {
                    UserLoginLog.CreateInstance().Add(current.Session["uid"].ToString(), this.site.SiteID);
                }
                return current.Session["uid"].ToString();
            }
            return null;
        }

        public int CheckUser(IUser user)
        {
            XmlElement documentElement = this.doc.DocumentElement;
            string xpath = string.Format("user[@uid='{0}']", user.Uid);
            XmlNode node = documentElement.SelectSingleNode(xpath);
            if (node == null)
            {
                return -1;
            }
            XmlAttribute attribute = node.Attributes["pwd"];
            if (attribute.Value == user.Pwd)
            {
                return 1;
            }
            return 0;
        }

        public void Jump(string uid, string defaultJumpUrl)
        {
            HttpContext current = HttpContext.Current;
            string url = "";
            if ((this.site == null) || string.IsNullOrEmpty(this.site.SiteID))
            {
                url = defaultJumpUrl;
                current.Response.Redirect(url);
            }
            string str2 = current.Request.QueryString[this.site.FromUrlKey];
            if (string.IsNullOrEmpty(str2))
            {
                url = Helper.GetUrl(this.site.HomePage, uid, this._loginId, this.site);
                current.Response.Redirect(url);
            }
            url = Helper.GetUrl(str2, uid, this._loginId, this.site);
            current.Response.Redirect(url);
        }

        public void LogOut(string uid)
        {
            HttpContext current = HttpContext.Current;
            current.Session.Remove("uid");
            StringBuilder builder = new StringBuilder();
            List<string> loginSites = UserLoginLog.CreateInstance().GetLoginSites(uid);
            foreach (string str in loginSites)
            {
                ISite site = new DefaultSite(str);
                builder.AppendLine("<iframe width='0px' height='0px' style='display:none'  src='" + site.LogOutUrl + "'/>");
            }
            current.Response.Write(builder.ToString());
            UserLoginLog.CreateInstance().DeleteUser(uid);
        }

        public void SaveToken(IUser user)
        {
            HttpContext.Current.Session["uid"] = user.Uid;
            if (this.site != null)
            {
                UserLoginLog.CreateInstance().Add(user.Uid, this.site.SiteID);
            }
        }

        public string LoginId
        {
            get
            {
                return this._loginId;
            }
        }

        public ISite Site
        {
            get
            {
                return this.site;
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

