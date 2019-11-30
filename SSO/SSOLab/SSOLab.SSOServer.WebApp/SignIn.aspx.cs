using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using SSOLab.SSOServer.Components;

namespace SSOLab.SSOServer.WebApp
{
    public partial class SignIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            string userID;

            bool passed = new UserService().AuthenticationUser(txtUsername.Text,
                txtPassword.Text,
                out userID);

            if (passed && !String.IsNullOrEmpty(userID))
            {
                FormsAuthentication.SetAuthCookie(userID, true);
                Session["CONTEXT_USER_IS_LONGIN"] = true;
                Session["CONTEXT_USER_ID"] = userID;

                string returnUrl = HttpUtility.UrlDecode(Request.Params["ReturnUrl"]);
                Response.Redirect(returnUrl);
            }
            else
            {
                Session["CONTEXT_USER_IS_LONGIN"] = false;
                Session["CONTEXT_USER_ID"] = String.Empty;

                Response.Write("登录失败！");
            }
        }
    }
}
