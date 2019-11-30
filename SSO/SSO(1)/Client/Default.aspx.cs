using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class _Default : System.Web.UI.Page 
{
    SSO.IClient ssoClient = new SSO.DefaultClient();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string exInfo = "";
            if (!ssoClient.Login(out exInfo))
            {
                Response.Write(exInfo);
                return;
            }
           
            StringBuilder sb = new StringBuilder();
            sb.Append("当前共有"+SSO.UserState.Instance.GetUserCount().ToString()+"个联盟用户在线！");
            Label1.Text = ssoClient.Uid + ",欢迎您的来访！"+sb.ToString();
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        ssoClient.LogOut();
    }
}
