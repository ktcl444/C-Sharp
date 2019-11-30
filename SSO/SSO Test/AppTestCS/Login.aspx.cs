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
using System.Configuration;

namespace AppTestCS
{
	/// <summary>
	/// Summary description for Login.
	/// </summary>
	public class Login : System.Web.UI.Page
	{
    protected System.Web.UI.WebControls.Label lblLogin;
    protected System.Web.UI.WebControls.TextBox txtLogin;
    protected System.Web.UI.WebControls.Button btnSignIn;
  
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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
      this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion

    private void btnSignIn_Click(object sender, System.EventArgs e)
    {
      VerifyUser(txtLogin.Text);
    }

    private void VerifyUser(string loginID)
    {
      AppToken al = new AppToken();
      AppUserRoles app = new AppUserRoles();

      try
      {
        if(app.IsLoginValid(loginID, 
          Convert.ToInt32(ConfigurationSettings.
           AppSettings["eSecurityAppID"])))
        {
          // Retrieve Login Key
          al.LoginKey = app.GetLoginKey(loginID, 
            Convert.ToInt32(ConfigurationSettings.
             AppSettings["eSecurityAppID"]));

          // Load up AppToken Object
          al.LoginID = loginID;
          al.AppKey = Convert.ToInt32(ConfigurationSettings.
             AppSettings["eSecurityAppID"]);
          al.AppName = ConfigurationSettings.
             AppSettings["eSecurityAppName"];

          // Set the Application Token Object
          Application["AppToken"] = al;

          // Create a Forms Authentication Cookie
          // Set Forms authentication variables
          FormsAuthentication.RedirectFromLoginPage(
            al.LoginKey.ToString(), false);
        }
        else
        {
          // Not a valid login
          // Redirect them to the login page via the Default page
          Response.Redirect("default.aspx");
        }
      }
      catch
      {
        // Redirect them to the login page via the Default page
        Response.Redirect("default.aspx");
      }
    }
	}
}
