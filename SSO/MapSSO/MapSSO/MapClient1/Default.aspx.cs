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
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Write("欢迎您:" + Environment.NewLine +HttpContext.Current.User.Identity.Name);
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        SSOClient.Logout();
    }

}
