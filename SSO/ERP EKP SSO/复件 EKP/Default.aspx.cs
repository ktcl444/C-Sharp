using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Threading;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //string strUserName = string.Empty;
        //string strPassWord = string.Empty;
        //try
        //{
        //    strUserName = Request.Cookies["User"].Values["userName"].ToString();
        //    strPassWord = Request.Cookies["User"].Values["password"].ToString();
        //}
        //catch
        //{

        //}
        //if (string.IsNullOrEmpty(strUserName) || string.IsNullOrEmpty(strPassWord))
        //{
        //    Response.Redirect("Login.aspx");
        //}
        //if (User.Identity.IsAuthenticated)
        //{
        //    //Response.Write("欢迎您," + User.Identity.Name);
        //    Response.Write("欢迎您," + User.Identity.Name.Split(new char[] { '\\' })[1]);
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
        //litScript.Text = "<script>ERPLogout()</script>";
        litScript.Text = "<script>CallJSONPServer('http://erp.test.com.cn/Logout_Http.aspx')</script>";

        Session.RemoveAll();
        HttpCookie cookie = HttpContext.Current.Request.Cookies["User"];
        if (cookie != null)
        {
            cookie.Domain = "test.com.cn";
            cookie.Expires = DateTime.Now.AddHours(-24);
            Response.Cookies.Add(cookie);
            Response.Redirect("Default.aspx");
        }
        //FormsAuthentication.SignOut();
        //FormsAuthentication.RedirectToLoginPage();
    }
}
