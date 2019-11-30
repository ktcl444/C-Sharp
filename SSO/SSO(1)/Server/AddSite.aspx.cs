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

public partial class AddSite : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        SSO.ISite site = new SSO.DefaultSite();
        site.SiteID = txtSiteID.Text.Trim();       
        SSO.RSACryption rsa = new SSO.RSACryption();
        string publicKey = "";
        string privateKey = "";
        rsa.RSAKey(out privateKey, out publicKey);
        site.PublicKey = publicKey;
        site.PublicAndPrivateKey = privateKey;
        site.FromUrlKey = TextBox1.Text.Trim();     
        site.HomePage = TextBox4.Text.Trim();
        site.UidField = TextBox5.Text.Trim();
        site.LogOutUrl = TextBox7.Text.Trim();
        if (!site.Validate())
        {
            Response.Write("此网站编号已经存在");
            return;
        }
        site.Add();
        TextBox6.Text = site.PublicAndPrivateKey;
        Response.Write("添加成功");
    }
}
