using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Mysoft.Map.Application.SSO;

public partial class _Default : System.Web.UI.Page
{
    //private const string SSOServer_LogoutUrl = "http://localhost:12000/Logout.aspx";
    protected void Page_Load(object sender, EventArgs e)
    {

        Response.Write("欢迎您:" + Environment.NewLine + HttpContext.Current.User.Identity.Name);
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        SSOClient.Logout();
        //FormsAuthentication.SignOut();
        //Response.Redirect(UrlHelper.AddParam(SSOServer_LogoutUrl,"ReturnUrl",this.u);
        //Response.Redirect(SSOServer_LogoutUrl);
        //FormsAuthentication.RedirectToLoginPage();
    }
}
