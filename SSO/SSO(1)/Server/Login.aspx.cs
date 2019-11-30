using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Login : System.Web.UI.Page
{ 
    SSO.IServer server;
    string defaultJumpUrl = "default.aspx";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            server = new SSO.DefaultServer();
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
            return;
        }
        string uid = server.CheckExistToken();        
        if (!string.IsNullOrEmpty(uid))
        {
            server.Jump(uid, defaultJumpUrl);
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {       
        SSO.IUser user = new SSO.DefaultUser();
        user.Uid = txtUid.Text.Trim();
        user.Pwd = txtPwd.Text.Trim();
        if (server.CheckUser(user) == 1)
        {
            server.SaveToken(user);
            server.Jump(user.Uid, defaultJumpUrl);
        }
    }
}
