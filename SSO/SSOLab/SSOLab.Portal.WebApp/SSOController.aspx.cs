using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using SSOLab.SSOServer.Components;
using System.Xml;
//该源码下载自www.51aspx.com(５１ａsｐｘ．ｃｏｍ)

namespace SSOLab.Portal.WebApp
{
    public partial class SSOController : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.Params["isSubmit"]) && Request.Params["isSubmit"] == "1")
            {
                try
                {                    
                    string ssoKey = "Xj1wD4DT7UicRVxOBJdjAg2AjErHkoEDlB9GqMJtYMwbfbnc9slagStcVt0Y3lY0XVKDnn6nO9cnCPDwM0tJU6iCBlWEoomDfjAjhobLurOxHR8ua8a25NGNQXQ1Q34X";

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
