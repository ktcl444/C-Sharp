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

public partial class Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        SSO.IUser user = new SSO.DefaultUser();
        user.Uid = txtUid.Text.Trim();
        user.Pwd = txtPwd.Text.Trim();
        if (!user.Validate())
        {
            Response.Write("对不起，该用户已经存在");
            return;
        }
        user.Register();
        Response.Write("注册成功");
    }
}
