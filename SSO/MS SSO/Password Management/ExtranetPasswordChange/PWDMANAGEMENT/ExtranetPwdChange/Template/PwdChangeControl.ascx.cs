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
using Contoso.Web.UI.Localization;
using System.Text.RegularExpressions;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace Contoso.Web.UI.WebControls
{

	/// <summary>
	///	Inherits the LocalizedUserControl class to decribe 
	///	the Extranet Password Change User Iinterface and operations.
	/// </summary>
	public class PwdChangeControl : Contoso.Web.UI.Localization.LocalizedUserControl
	{
		protected System.Web.UI.WebControls.Label lblOldPwdHeader;
		protected System.Web.UI.WebControls.Label lblNewPwdHeader;
		protected System.Web.UI.WebControls.Label lblConfirmPwdHeader;
		protected System.Web.UI.WebControls.TextBox txtOldPwd;
		protected System.Web.UI.WebControls.TextBox txtNewPwd;
		protected System.Web.UI.WebControls.TextBox txtConfirmPwd;
		protected System.Web.UI.WebControls.Button btnChange;
		protected System.Web.UI.WebControls.Label lblError;
		protected System.Web.UI.WebControls.Label lblDomainAndUserNameHeader;
		protected System.Web.UI.WebControls.TextBox txtDomainAndUserName;

		private const int MAX_PWD_LENGTH = 25;

		/// <summary>
		/// Calls the method to localize controls on page load
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">EventArgs</param>
		/// <returns></returns>
		private void Page_Load(object sender, System.EventArgs e)
		{
			this.LocalizeControls();
		}

		/// <summary>
		/// Validates the input and then changes the password 
		/// by verifying the user credentials
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">EventArgs</param>
		/// <returns></returns>
		private void btnChange_Click(object sender, System.EventArgs e)
		{
			string userLDAPBindingString;
			string userName;
			string[] names;
			object[] parameters;
			DirectoryEntry user;

			if (!ValidateInput())
				return;

			// Split the text of txtDomainAndUserName to 
			// get the userName
			names = txtDomainAndUserName.Text.Split(new char[]{'\\'});
			if (names.Length != 2)
			{
				lblError.Text = GetResourceString("msgDomainNameMissing");
				return;
			}
			userName = names[1];

			try 
			{ 
				// Path to your LDAP directory server.
				userLDAPBindingString = ConfigurationSettings.AppSettings
					["usersLDAPBindingString"];
				userLDAPBindingString = userLDAPBindingString.Replace
					("{user}", userName);

				// DirectoryEntry class is instantiated encapsulates a node
				// or object in the Active Directory hierarchy.
				user = new DirectoryEntry ( 
						userLDAPBindingString, 
						txtDomainAndUserName.Text, 
						txtOldPwd.Text, 
						AuthenticationTypes.Secure); 
				
				parameters = new object[]{txtOldPwd.Text, txtNewPwd.Text}; 
				// Calls the ChangeMethod on the native Active Directory. 
				user.Invoke("ChangePassword" , parameters); 
				lblError.Text = GetResourceString("msgPasswordChanged");
			} 
			catch(COMException comex) 
			{ 
				lblError.Text = comex.Message;
			}
			catch(Exception ex) 
			{ 
				if (ex.InnerException != null)
					lblError.Text = ex.InnerException.Message;
				else
					lblError.Text = ex.Message;
			}

		}
		/// <summary>
		/// Validates all the inputs against specifications 
		/// </summary>
		/// <returns>True if validation success</returns>
		private bool ValidateInput()
		{
			// If the controls password and confirm password 
			// not equal
			if (txtNewPwd.Text != txtConfirmPwd.Text)
			{
				lblError.Text = GetResourceString("msgPwdNotConfirmed");
				return false;
			}

			// If password is greater than max allowed length
			if (txtNewPwd.Text.Length > MAX_PWD_LENGTH)
			{
				lblError.Text = GetResourceString("msgPwdToLong");
				return false;
			}

			// In the given credentials, if the domain name is missing
			if (txtDomainAndUserName.Text.IndexOf("\\", 0, txtDomainAndUserName.Text.Length) == 0)
			{
				lblError.Text = GetResourceString("msgDomainNameMissing");
				return false;
			}

			return true;
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
			this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
