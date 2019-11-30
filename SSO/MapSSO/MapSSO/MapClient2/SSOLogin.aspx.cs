using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Security;
using Mysoft.Map.Application.SSO;

public partial class SSOLogin : System.Web.UI.Page
{
    //private const string SSOServer_LoginUrl = "http://localhost:12000/Login.aspx";
    //private const string SSOServer_ValidateUrl = "http://localhost:12000/Validate.aspx";

    protected void Page_Load(object sender, EventArgs e)
    {
        SSOClient.Login();
        //if (HttpContext.Current.Request.QueryString["ServiceTicket"] != null && HttpContext.Current.Request.QueryString["SiteID"] != null)
        //{
        //    string serviceTicket = HttpContext.Current.Request.QueryString["ServiceTicket"];
        //    string siteID = HttpContext.Current.Request.QueryString["SiteID"];

        //    //TODO 验证签名
        //    string strSignText = HttpContext.Current.Request.QueryString["SignText"];
        //    if (string.IsNullOrEmpty(strSignText))
        //    {
        //        Response.Write("数据被非法篡改");
        //        return;
        //    }
        //    string strSourceSign = RSAHelper.Decrypt(serviceTicket, string.Empty) + "||" + RSAHelper.Decrypt(siteID, string.Empty);
        //    string strHashData = string.Empty;
        //    RSAHelper.GetHash(strSourceSign, ref strHashData);
        //    string strSign = string.Empty;
        //    if (!RSAHelper.SignatureDeformatter(string.Empty, strHashData, strSignText))
        //    {
        //        Response.Write("数据被非法篡改");
        //        return;
        //    }

        //    string returnUrl = SSOUtil.GetHostUrl() + FormsAuthentication.LoginUrl;
        //    Response.Redirect(UrlHelper.AddParam(UrlHelper.AddParam(UrlHelper.AddParam(SSOServer_ValidateUrl, "ReturnUrl", returnUrl), "ServiceTicket", serviceTicket), "SiteID", siteID));
        //}
        ////else if (HttpContext.Current.Request.QueryString["User"] != null)
        //else if (HttpContext.Current.Request.Cookies["SSOServer TGC"] != null)
        //{
        //    //TODO 验证签名
        //    FormsAuthentication.RedirectFromLoginPage(RSAHelper.Decrypt(HttpContext.Current.Request.Cookies["SSOServer TGC"].Value, string.Empty), false);
        //    //FormsAuthentication.RedirectFromLoginPage(HttpContext.Current.Request.QueryString["User"], false);
        //}
        //else
        //{
        //    //string returnUrl = HttpContext.Current.Request.Url.ToString();
        //    string returnUrl = SSOUtil.GetHostUrl() + FormsAuthentication.LoginUrl;
        //    Response.Redirect(UrlHelper.AddParam(UrlHelper.AddParam(SSOServer_LoginUrl, "ReturnUrl", returnUrl), "SiteID", RSAHelper.Encrypt("2", string.Empty)));
        //}
    }
}
