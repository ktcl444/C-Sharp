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
using System.Xml;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Contoso.Identity.Template;
using Contoso.Web.UI.Localization;

namespace Contoso.Web.UI.WebControls
{

	/// <summary>
	///		Summary description for IdentityViewerControl.
	/// </summary>
	public class IdentityViewerControl : LocalizedUserControl
	{
		protected System.Web.UI.WebControls.Label lblIdentityModelHeader;
		protected System.Web.UI.WebControls.Label lblIdentityModel;
		protected System.Web.UI.WebControls.Label lblChangeIdentityModel;
		protected System.Web.UI.WebControls.Label lblAuthNModeHeader;
		protected System.Web.UI.WebControls.Label lblAuthNMode;
		protected System.Web.UI.WebControls.Label lblHttpContextHeader;
		protected System.Web.UI.WebControls.Label lblHttpContext;
		protected System.Web.UI.WebControls.Label lblWindowsIdentityHeader;
		protected System.Web.UI.WebControls.Label lblWindowsIdentity;
		protected System.Web.UI.WebControls.Label lblThreadHeader;
		protected System.Web.UI.WebControls.Label lblThread;

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.LocalizeControls();
			ShowIdentityInfo(lblIdentityModel, lblAuthNMode, lblThread, lblWindowsIdentity, lblHttpContext);
		}

		private void ShowIdentityInfo(Label identityModel, Label authnMode, Label threadUser, Label windowsIdentityUser, Label httpContextUser)
		{
			String changeIdentityModel;

			authnMode.Text = System.Threading.Thread.CurrentPrincipal.Identity.AuthenticationType;
			threadUser.Text = System.Threading.Thread.CurrentPrincipal.Identity.Name;
			windowsIdentityUser.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
			httpContextUser.Text = HttpContext.Current.User.Identity.Name;

			identityModel.ForeColor = System.Drawing.Color.Blue;
			if (IdentityUtil.IsImpersonating())
			{
				identityModel.Text = this.GetResourceString("strImpersonateDelegate");
				if (IdentityUtil.AuthenticationMode() == "Forms")
				{
					changeIdentityModel = this.GetResourceString("strFormsChangeToTrusted");
				}
				else
				{
					changeIdentityModel = this.GetResourceString("strFormsChangeToImpersonateDelegate");
				}
			}
			else
			{
				identityModel.Text = this.GetResourceString("strTrustedSubsystem");
				if (IdentityUtil.AuthenticationMode() == "Forms")
				{
					changeIdentityModel = this.GetResourceString("strFormsChangeToImpersonateDelegate");
				}
				else
				{
					changeIdentityModel = this.GetResourceString("strNonFormsChangeToImpersonateDelegate");
				}
			}
			changeIdentityModel = changeIdentityModel.Replace("[PhysicalApplicationPath]", HttpContext.Current.Request.PhysicalApplicationPath);
			lblChangeIdentityModel.Text = changeIdentityModel;

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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
