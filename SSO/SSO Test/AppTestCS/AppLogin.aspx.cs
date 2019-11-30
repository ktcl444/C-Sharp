using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using AppLauncherDataCS;
using System.Web.Security;

namespace AppTestCS
{
	/// <summary>
	/// Summary description for AppLogin.
	/// </summary>
	public class AppLogin : System.Web.UI.Page
	{
    private void Page_Load(object sender, System.EventArgs e)
    {
	    VerifyToken();
    }

    private void VerifyToken()
    {
      Apps app = new Apps();
      AppToken al;

      try
      {
        al = app.VerifyLoginToken(
          Request.QueryString["Token"].ToString());

        if(al.LoginID.Trim() == "")
        {
          // Not a valid login
          // Redirect them to default page 
          // This will put them at the login page
          Response.Redirect("default.aspx");
        }
        else
        {
          // Create a Forms Authentication Cookie
          // Set Forms authentication variables
          FormsAuthentication.Initialize();
          FormsAuthentication.SetAuthCookie(
            al.LoginID.ToString(), false);

          // Set the Application Token Object
          Application["AppToken"] = al;

          // Redirect to Default page
          Response.Redirect("default.aspx");
        }
      }
      catch
      {
        // Redirect them to the login page via the Default page
        Response.Redirect("default.aspx");
      }
    }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion
	}
}
