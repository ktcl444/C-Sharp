using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logout_Http : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //this.Response.ContentType = "Text/HTML";
        //Response.Write(Logout());
        //Response.End();
        //Response.Write(string.Empty);
        //Response.Write("OnJSONPServerResponse('" + Logout() + "');");
        Response.DisableKernelCache();
        Response.Cache.SetNoStore();
        Logout();
        Response.End();
    }

    private string Logout()
    {
        try
        {
            Session.RemoveAll();
        }
        catch(Exception ex)
        { 
            return "<xml result=\"false\" errormessage=\"" + ex.Message + "\"></xml>";
        }
        return   "<xml result=\"true\"></xml>";
    }
}
