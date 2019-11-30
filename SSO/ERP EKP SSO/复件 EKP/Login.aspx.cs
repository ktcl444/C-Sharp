using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Text.RegularExpressions;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["Logout"] != null)
        {
            Logout();
            Response.Redirect("Login.aspx");
        }

        if (Session["UserName"] != null)
        {
            Response.Redirect("Default.aspx");
        }
        else
        {
            if (HttpContext.Current.Request.Cookies["User"] != null)
            {
                Response.Redirect("Default.aspx");
            }
        }
    }


    private void Logout()
    {
        Session.RemoveAll();
        HttpCookie cookie = HttpContext.Current.Request.Cookies["User"];
        if (cookie != null)
        {
            cookie.Domain = "test.com.cn";
            cookie.Expires = DateTime.Now.AddHours(-24);
            Response.Cookies.Add(cookie);
            Response.Redirect("Default.aspx");
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (VerifyInput())
        {
            //string p = @"http://[^\.]*\.(?<domain>[^/]*)";
            //Regex reg = new Regex(p, RegexOptions.IgnoreCase);
            //Match m = reg.Match(Request.Url.ToString());
            //Response.Write(m.Groups["domain"].Value);

            //FormsAuthentication.SetAuthCookie(txtUserName.Text, true);
            //FormsAuthentication.RedirectFromLoginPage(txtUserName.Text, false);
            //FormsAuthentication.
            //SSOServer.Login(this.txtUserName.Text, this.txtPassword.Text);
            HttpCookie cookie = new HttpCookie("User");
            cookie.Values.Add("userName", this.txtUserName.Text.Trim());
            cookie.Values.Add("password", this.txtPassword.Text.Trim());
            cookie.Path = "/";
            cookie.Domain = "test.com.cn";
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cookie);
            Session.RemoveAll();
            Session["UserName"] = this.txtUserName.Text.Trim();
            Response.Redirect("Default.aspx");
        }
        else
        {
            return;
        }
    }

    private bool VerifyInput()
    {
        if (string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtPassword.Text))
        {
            Response.Write("请输入用户名和密码.");
            return false;
        }
        return true;
    }
}
