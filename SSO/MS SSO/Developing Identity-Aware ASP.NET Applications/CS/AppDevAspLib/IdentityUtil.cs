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
using System.Web;
using System.Web.Security;
using System.Configuration;

namespace Contoso.Identity.Template
{
	/// <summary>
	/// IdentityInfo helper class that can read the current
	/// authentication mode (Windows, Forms, Passport etc) and
	/// provide information whether we currently are impersonating
	/// the user or not
	/// </summary>
	public class IdentityUtil
	{
		/// <summary>
		///	Figure out if we are impersonating the client user or not.
		///	
		///	When using ASP.NET Forms Authentication, the impersonate
		///	attribute of the identity element is always set to false to 
		///	ensure that ASP.NET runs under a priveleged account that can
		///	impersonate users. Contoso chose to use a custom setting to
		///	select model
		///	
		///	When using other authentication mechanisms, such as Windows
		///	Integrated, X.509 certificates and Passport the model is 
		///	directly related to the impersonate attribute of the identity
		///	element.
		/// </summary>
		/// <returns>true if we are impersonating and false on any error</returns>
		public static Boolean IsImpersonating()
		{
			ConfigXmlDocument configDoc = null;
			String useImpersonationModel = null;
			XmlNode identityNode = null;
			XmlAttribute impersonateAttribute = null;
			Boolean isImpersonatingInPlatform = false;
			Boolean isImpersonatingInCode = false;

			try
			{
				//Check for impersonation in platform
				configDoc = new ConfigXmlDocument();
				configDoc.Load(HttpContext.Current.Request.PhysicalApplicationPath + "web.config");
				identityNode = configDoc.SelectSingleNode("configuration/system.web/identity");
				if (identityNode != null)
				{
					impersonateAttribute = identityNode.Attributes["impersonate"];
					isImpersonatingInPlatform = Convert.ToBoolean(impersonateAttribute.Value);
				}

				//Check for impersonation in code
				useImpersonationModel = ConfigurationSettings.AppSettings["UseImpersonationModel"];
				if (useImpersonationModel != null && Convert.ToBoolean(useImpersonationModel) == true)
					isImpersonatingInCode = true;	//default to not impersonate

				if (isImpersonatingInCode || isImpersonatingInPlatform)
					return true;
				else
					return false;
			}
			catch (Exception ex)
			{
				//TODO: Handle configuration exception gracefully
				return false;
			}

		}

		/// <summary>
		/// Figure out authentication mode from the User.Identity type
		/// </summary>
		/// <returns></returns>
		public static String AuthenticationMode()
		{
			if (HttpContext.Current.User == null)
				return "None";

			if (HttpContext.Current.User.Identity is FormsIdentity)
				return "Forms";
			else if (HttpContext.Current.User.Identity is PassportIdentity)
				return "Passport";
			else
				return "Windows";

		}

	}
}
