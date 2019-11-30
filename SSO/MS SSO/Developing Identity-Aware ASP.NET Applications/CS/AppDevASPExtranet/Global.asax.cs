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
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;
using System.Security.Principal;
using System.Configuration;
using System.Runtime.InteropServices;
using Contoso.Identity.Template;

namespace Contoso.AppDevASPSample 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private WindowsImpersonationContext wic;  
		
		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			AzMan authManager = new AzMan();
			authManager.Initialize(@"msxml://C:\AppDevASPExtranet.xml", "AppDevASPExtranet");
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{
			if (wic != null)
				wic.Undo();
		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{
			string formsCookieName = FormsAuthentication.FormsCookieName;
			HttpCookie formsCookie = Request.Cookies[formsCookieName];

			if (User == null || User.Identity == null)
				return;

			if( User.Identity is FormsIdentity ) 
			{
				
				// Extract the forms authentication cookie
				if(null == formsCookie)
				{
					// There is no authentication cookie.
					return;
				}

				FormsAuthenticationTicket authTicket = null;
				try
				{
					authTicket = FormsAuthentication.Decrypt(formsCookie.Value);
				}
				catch
				{
					//TODO: Log exception details (omitted for simplicity)
					return;
				}
				if (null == authTicket)
				{
					// Cookie failed to decrypt.
					return;
				}

				// Create an Identity object with a UPN and impersonate
				// if configured for custom impersonation
				String useImpersonationModel = ConfigurationSettings.AppSettings["UseImpersonationModel"];
				if (useImpersonationModel == null)
					return;	//default to not impersonate

				if (Convert.ToBoolean(useImpersonationModel) == true)
				{
					WindowsIdentity wid = new WindowsIdentity(authTicket.Name);
					wic = wid.Impersonate();
				}
				
				// Now we are impersonating just as if ASP.NET had been set up for
				// impersonation
			}

		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{

		}
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}

