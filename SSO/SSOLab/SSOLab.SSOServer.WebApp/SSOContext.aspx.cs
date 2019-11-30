using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using SSOLab.SSOServer.Components;
using System.Net;
using System.IO;

namespace SSOLab.SSOServer.WebApp
{
    public partial class SSOContext1 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.ContentType = "text/javascript";
            //Response.Buffer = true;
            Response.Expires = 0;
            Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1);
            Response.AddHeader("pragma", "no-cache");
            Response.CacheControl = "no-cache";

            try
            {
                StringBuilder userInfo = new StringBuilder();
                userInfo.Append("<userinfo>");

                if (Session["CONTEXT_USER_IS_LONGIN"] != null &&
                    (bool)Session["CONTEXT_USER_IS_LONGIN"] == true &&
                    Session["CONTEXT_USER_ID"] != null)
                {

                    User user = new UserService().GetUserByID((string)Session["CONTEXT_USER_ID"]);

                    if (user != null)
                    {
                        userInfo.Append("<id>").Append(user.ID).Append("</id>");
                        userInfo.Append("<username>").Append(user.Username).Append("</username>");
                        userInfo.Append("<islongin>true</islongin>");
                    }
                    else
                    {
                        userInfo.Append("<id></id>");
                        userInfo.Append("<username></username>");
                        userInfo.Append("<islongin>false</islongin>");
                    }
                }
                else
                {
                    userInfo.Append("<id></id>");
                    userInfo.Append("<username></username>");
                    userInfo.Append("<islongin>false</islongin>");
                }

                userInfo.Append("<synchdate>").Append(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")).Append("</synchdate>");
                userInfo.Append("</userinfo>");

                StringBuilder sb = new StringBuilder();
                sb.Append("function SSOContext(_signInUrl, _signOutUrl, _userInfo) {").Append("\r\n");
                sb.Append("this.signInUrl=_signInUrl;").Append("\r\n");
                sb.Append("this.signOutUrl=_signOutUrl;").Append("\r\n");
                sb.Append("this.userInfo=_userInfo;").Append("\r\n");
                sb.Append("}").Append("\r\n");
                sb.Append("var ssoContext=");
                sb.Append("new SSOContext(");
                sb.Append("'").Append(SSOUtil.GetSiteUrl() + "/SignIn.aspx").Append("', ");
                sb.Append("'").Append(SSOUtil.GetSiteUrl() + "/SignOut.aspx").Append("', ");

                Application application = new ApplicationService().GetApplicationByName(Request.Params["app"]);

                if (application != null &&
                    !String.IsNullOrEmpty(application.SSOKey) &&
                    application.SSOKey.Length >= 128)
                {
                    sb.Append("'").Append(SSOUtil.DESEncrypt(userInfo.ToString(), application.SSOKey.Substring(application.SSOKey.Length / 2 - 1, 8))).Append("'");
                }

                else
                {
                    sb.Append("' '");
                }

                sb.Append(");").Append("\r\n");


                Response.Write(sb.ToString());

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }

}
