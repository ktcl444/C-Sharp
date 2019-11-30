using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SSOLab.SSOServer.Components;
using System.Xml;
using System.Web.Security;

namespace SSOLab.App1.WebApp
{
    public partial class SSOController : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.Params["isSubmit"]) && Request.Params["isSubmit"] == "1")
            {
                try
                {
                    string ssoKey = "XD0cEmXD0IcmYD0gBmYE0OdnYE1jHnZE1USnZF3y3GYpm93Gjp2s2GSog32FfoDm2FZoaG2FZnmhpkVBlJXkWB1eGkWCqA2lWC7k2lXCw1dlXDMqolZr805InrQk4Ixq";

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
