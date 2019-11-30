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
using System.Configuration;

namespace AppTestCS
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public class _default : System.Web.UI.Page
	{
    protected System.Web.UI.WebControls.Label Label1;
    protected System.Web.UI.WebControls.Label lblGuid;
    protected System.Web.UI.WebControls.Label Label3;
    protected System.Web.UI.WebControls.Label Label2;
    protected System.Web.UI.WebControls.Label lblAppName;
    protected System.Web.UI.WebControls.Label lblLoginID;
  
		private void Page_Load(object sender, System.EventArgs e)
		{
      lblLoginID.Text = User.Identity.Name;
      lblAppName.Text =
        ConfigurationSettings.AppSettings["eSecurityAppName"];

      if(User.IsInRole("Admin"))
      {
        lblLoginID.Visible = false;
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
