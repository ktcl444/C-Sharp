///////////////////////////////////////////////////////////////////////////////
//
// Microsoft Solutions for Security
// Copyright (c) 2004 Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
// AND INFORMATION REMAINS WITH THE USER.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using Contoso.Identity.Template;
using Contoso.Web.UI.Localization;

namespace Contoso.Web.UI.WebControls
{

	/// <summary>
	///		Summary description for LogonControl.
	/// </summary>
	public class LogonControl : LocalizedUserControl
	{
		protected System.Web.UI.WebControls.Button btnLogon;
		protected System.Web.UI.WebControls.TextBox txtPassword;
		protected System.Web.UI.WebControls.TextBox txtUsername;
		protected System.Web.UI.WebControls.TextBox txtDomainName;
		protected System.Web.UI.WebControls.Label lblDomainNameHeader;
		protected System.Web.UI.WebControls.Label lblUserNameHeader;
		protected System.Web.UI.WebControls.Label lblPasswordHeader;
		protected System.Web.UI.WebControls.Label lblDomainNameHelp;
		protected System.Web.UI.WebControls.Label lblError;

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.LocalizeControls();
		}

		private void btnLogon_Click(object sender, System.EventArgs e)
		{
			// Path to your LDAP directory server.
			String adPath = ConfigurationSettings.AppSettings["LDAPBindingString"];
			LdapAuthentication adAuth = new LdapAuthentication(adPath);
			try
			{
				if(true == adAuth.IsAuthenticated(txtDomainName.Text, txtUsername.Text, txtPassword.Text))
				{
					FormsAuthentication.RedirectFromLoginPage(txtUsername.Text + "@" + txtDomainName.Text, false);
				}
				else
				{
					lblError.Text = this.GetResourceString("msgInvalidUPN");
				}
			}
			catch(Exception ex)
			{
				lblError.Text = ex.Message;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnLogon.Click += new System.EventHandler(this.btnLogon_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
