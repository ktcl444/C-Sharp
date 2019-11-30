using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //string strUserName = string.Empty;
        //string strPassWord = string.Empty;
        //try
        //{
        //    strUserName = Request.Cookies["User"]["userName"].ToString();
        //    strPassWord = Request.Cookies["User"]["password"].ToString();
        //}
        //catch { }
        //if (string.IsNullOrEmpty(strUserName))
        //{
        //    Response.Redirect("Login.aspx");
        //}
        //Response.Write("欢迎您," + User.Identity.Name.Split(new char[] { '\\' })[1]);
        //if (HttpContext.Current.User.Identity.IsAuthenticated)
        //{
        //    Response.Write("欢迎您," + HttpContext.Current.User.Identity.Name.Split(new char[] { '\\' })[1]);
        //}
        //else
        //{
        //    Response.Redirect("Login.aspx");
        //}
        if (Session["UserName"] != null)
        {
            Response.Write("欢迎您," + Session["UserName"].ToString());
        }
        else
        {
            if (HttpContext.Current.Request.Cookies["User"] != null)
            {
                Session["UserName"] = HttpContext.Current.Request.Cookies["User"]["UserName"].ToString();
                Response.Write("欢迎您," + HttpContext.Current.Request.Cookies["User"]["UserName"].ToString());
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Session.RemoveAll();
        HttpCookie cookie = HttpContext.Current.Request.Cookies["User"];
        if (cookie != null)
        {
            cookie.Domain = "test.com.cn";
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);
            Response.Redirect("Default.aspx");
        }
        //FormsAuthentication.SignOut();
        //FormsAuthentication.RedirectToLoginPage();
    }
}
