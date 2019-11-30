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
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.DirectoryServices;
using Contoso.Web.UI.Localization;

namespace Contoso.Identity.Template
{
	/// <summary>
	/// The LdapAuthentication class provides authentication of
	/// a user against an LDAP directory.
	/// </summary>
	public class LdapAuthentication
	{
		private const String SOURCE_NAME = "AppDevASPExtranet";

		private string _path;
		private string _filterAttribute;

		/// <summary>
		/// Public default constructor of the LdapAuthentication
		///  helper class
		/// </summary>
		public LdapAuthentication()
		{
		}

		/// <summary>
		/// Public constructor of the LdapAuthentication class that
		/// initializes the LDAP path to be used
		/// </summary>
		public LdapAuthentication(string path)
		{
			_path = path;
		}

		/// <summary>
		/// Forces authentication by binding to the native AdsObject.
		/// </summary>
		/// <param name="domain"></param>
		/// <param name="username"></param>
		/// <param name="pwd"></param>
		/// <returns>Returns true after a succesful binding and returns
		/// false if any kind of error occurs</returns>
		public bool IsAuthenticated(string domain, string userName, string pwd)
		{
			string domainAndUsername = domain + @"\" + userName;

			try
			{
				DirectoryEntry entry = new DirectoryEntry( _path, domainAndUsername, pwd, AuthenticationTypes.Secure);

				// Bind to the native AdsObject to force authentication.
				Object obj = entry.NativeObject;
				DirectorySearcher search = new DirectorySearcher(entry);
				search.Filter = "(userPrincipalName=" + userName + "@" + domain + ")";
				search.PropertiesToLoad.Add("cn");
				SearchResult result = search.FindOne();
				if(null == result)
				{
					return false;
				}

				// Audit successful logon here
//				EventLog.WriteEntry(sourceName, "Logon success for user " + domainAndUsername, EventLogEntryType.SuccessAudit);				

				// Update the new path to the user in the directory
				_path = result.Path;
				_filterAttribute = (String)result.Properties["cn"][0];
			}
			catch (Exception ex)
			{
				// Audit logon failure here
				EventLog.WriteEntry(SOURCE_NAME, LocalizationUtils.GetGlobalString("msgLogonFailureEvent") + " " + domainAndUsername, EventLogEntryType.FailureAudit);

				throw new Exception(LocalizationUtils.GetGlobalString("msgLogonFailure") + " " + ex.Message);
			}

			return true;
		}

	}
}
