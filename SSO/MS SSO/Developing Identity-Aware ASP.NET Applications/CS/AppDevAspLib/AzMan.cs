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
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using System.Runtime.InteropServices;
using Microsoft.Interop.Security.AzRoles;
using Contoso.Web.UI.Localization;

namespace Contoso.Identity.Template
{
	/// <summary>
	/// Helper class for common Authorization Manager (AzMan) functions.
	/// </summary>
	public class AzMan
	{
		public const int AZ_AZSTORE_FLAG_ACCESS_CHECK = 0;		//The authorization store is opened for use by the Update method and the AccessCheck method
		public const int AZ_AZSTORE_FLAG_CREATE = 1;			//The system attempts to create the policy store specified by the bstrPolicyURL parameter
		public const int AZ_AZSTORE_FLAG_MANAGE_STORE_ONLY = 2; //An existing store is opened for management purposes. Run-time routines such as AccessCheck cannot be performed
		public const int AZ_AZSTORE_FLAG_BATCH_UPDATE = 4;		//The provider is notified that many objects will be modified or created. The provider then optimizes submission of the changes for better performance

		/// <summary>
		/// Prepare AzMan when using trusted subsystem model
		/// </summary>
		public void Initialize(String azPolicyURL, String applicationName)
		{
			try
			{
				if (IdentityUtil.IsImpersonating() == true)
					return;

				AzAuthorizationStoreClass store = new AzAuthorizationStoreClass();
				store.Initialize(AZ_AZSTORE_FLAG_ACCESS_CHECK, 
					azPolicyURL,
	 				null);

				IAzApplication app = store.OpenApplication(applicationName, null);
				HttpContext.Current.Application.Add("AzManApp",app);
			}
			catch (Exception ex)
			{
				throw new Exception(LocalizationUtils.GetGlobalString("msgUnableToInitAzMan"), ex);
			}
		}

		/// <summary>
		/// Allow access if using impersonation/delegation model
		/// and check access when using trusted subsystem model
		/// </summary>
		/// <param name="objectName"></param>
		/// <param name="scope"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public Boolean IsAllowedAccess(String objectName, String scope, Int32 operation)
		{
			if (IdentityUtil.IsImpersonating() == true)
				return true;

			//Get the application
			IAzApplication app = (IAzApplication)HttpContext.Current.Application.Get("AzManApp");
			if (app == null)
				throw new Exception("Unable to get AzMan application");

			HandleRef token;

			if(HttpContext.Current.User.Identity is FormsIdentity)
			{
				// For ASP.NET FormsAuthentication in the Extranet, 
				// we need to do a Windows logon to creat a token
				WindowsIdentity formsUser = new WindowsIdentity(HttpContext.Current.User.Identity.Name);
				token = new HandleRef(this, formsUser.Token);
			}
			else
			{
				// For Windows Integrated Authentication in the Intranet scenario 
				// as well as the Client Certificate and Passport Authentication 
				// in the Extranet scenario we can ask IIS for the token of the 
				// browser user
				token = new HandleRef(this, ((WindowsIdentity)HttpContext.Current.User.Identity).Token);
			}

			//Create Client Context
			IAzClientContext clientContext = app.InitializeClientContextFromToken( 
				(UInt64)token.Handle,
				0);

			object[] scopes = new object[] {scope};
			object[] operations = new object[] {operation};
			
			object[] results = (object[])clientContext.AccessCheck(objectName, scopes, operations, null, null, null, null, null);

			bool isAuthorized = true;
			foreach (int result in results)
			{
				if (result != 0) // zero = no error
				{
					isAuthorized = false;
					break;
				}
			}

			return isAuthorized;

		}

	}
}
