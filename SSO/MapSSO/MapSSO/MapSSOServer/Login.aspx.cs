using System;
using Mysoft.Map.Application.SSO;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (VerifyInput())
        {
            SSOServer.Login(this.txtUserName.Text, this.txtPassword.Text);
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
