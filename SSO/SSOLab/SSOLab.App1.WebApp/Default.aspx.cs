using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//5-1aspx
namespace SSOLab.App1.WebApp
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblUserID.Text = HttpContext.Current.User.Identity.Name;
        }
    }
}
