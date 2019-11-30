using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strUserName = string.Empty;
        string strPassWord = string.Empty;
        try
        {
            strUserName = Request.Cookies["User"].Values["userName"].ToString();
            strPassWord = Request.Cookies["User"].Values["password"].ToString();
        }
        catch
        {

        }
        if (string.IsNullOrEmpty(strUserName) || string.IsNullOrEmpty(strPassWord))
        {
            Response.Redirect("Login.aspx");
        }
    }
}
