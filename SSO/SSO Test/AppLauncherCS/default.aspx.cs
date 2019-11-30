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

using System.Configuration;
using System.Data.SqlClient;
using AppLauncherDataCS;

namespace AppLauncherCS
{
  /// <summary>
  /// Summary description for _default.
  /// </summary>
  public class _default : System.Web.UI.Page
  {
    protected System.Web.UI.WebControls.Label Label1;
    protected System.Web.UI.WebControls.Label lblLogin;
    protected System.Web.UI.WebControls.Label lblMessage;
    protected System.Web.UI.WebControls.DataGrid grdApps;
  
    private void Page_Load(object sender, System.EventArgs e)
    {
      // Display the User Name
      // Without the Domain prefix
      lblLogin.Text = "Applications Available for: " + 
        Apps.LoginIDNoDomain(User.Identity.Name);

      AppLoad();
    }

    private void AppLoad()
    {
      Apps app = new Apps();

      try
      {
        // Load applications for this user
        grdApps.DataSource =
          app.GetAppsByLoginID(User.Identity.Name);
        grdApps.DataBind();

      }
      catch (Exception ex)
      {
        lblMessage.Text = ex.Message;
      }
    }

    private void grdApps_ItemCommand(object source, 
      System.Web.UI.WebControls.DataGridCommandEventArgs e)
    {
      Apps app = new Apps();
      bool redirect = false;
      string token = String.Empty;
      LinkButton lb;

      try
      {
        lb = (LinkButton) e.Item.Cells[0].Controls[1];
       
        // Create a Token for this user/app
        token = app.CreateLoginToken(
          lb.Text, 
          User.Identity.Name, 
          Convert.ToInt32(lb.Attributes["UserID"]), 
          Convert.ToInt32(lb.Attributes["AppID"]));

        redirect = true;
      }
      catch (Exception ex)
      {
        redirect = false;
        lblMessage.Text = ex.Message;
      }

      if (redirect)
      {
        // Redirect to web application 
        // passing in the generated token
        Response.Redirect(e.CommandArgument.ToString() +
          "?Token=" + token, false);
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
      this.grdApps.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdApps_ItemCommand);
      this.Load += new System.EventHandler(this.Page_Load);

    }
    #endregion

  }
}
