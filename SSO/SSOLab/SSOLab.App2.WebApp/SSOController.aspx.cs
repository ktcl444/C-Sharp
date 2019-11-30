using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using SSOLab.SSOServer.Components;
using System.Web.Security;

namespace SSOLab.App2.WebApp
{
    public partial class SSOController : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.Params["isSubmit"]) && Request.Params["isSubmit"] == "1")
            {
                try
                {
                    string ssoKey = "XBtyndN8yHpZCiM1eO9XtE1qii9Oey17CYosH8cM7nRnXBIBjdN811pZrtw1PfhcBDyq7S9OeHcGmWAR7ycM7aloXBCsXQhe10FgrBEwPfSndDZpGwxbL55ymWAmhycM";

                    string userInfo = SSOUtil.DESDecrypt(Request.Params["sso_userinfo"], ssoKey.Substring(ssoKey.Length / 2 - 1, 8));
                    Response.Write(userInfo);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(userInfo);

                    if (xmlDoc.SelectSingleNode("/userinfo/islongin").InnerText == "true")
                    {
                        FormsAuthentication.RedirectFromLoginPage(xmlDoc.SelectSingleNode("/userinfo/username").InnerText, false);
                    }
                    else
                    {
                        string returnUrl = SSOUtil.GetHostUrl() + FormsAuthentication.LoginUrl;

                        Response.Redirect(Request.Params["sso_signinurl"] + "?ReturnUrl=" + HttpUtility.UrlEncode(returnUrl));
                    }
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
            }
        }
    }
}
