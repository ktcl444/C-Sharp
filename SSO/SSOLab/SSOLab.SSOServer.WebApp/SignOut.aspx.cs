using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace SSOLab.SSOServer.WebApp
{
    public partial class SignOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();

            Session.Remove("USER_IS_LONGIN");
            Session.Remove("USER_ID");

            string returnUrl = HttpUtility.UrlDecode(Request.Params["ReturnUrl"]);
            Response.Redirect(returnUrl);
        }
    }
}
