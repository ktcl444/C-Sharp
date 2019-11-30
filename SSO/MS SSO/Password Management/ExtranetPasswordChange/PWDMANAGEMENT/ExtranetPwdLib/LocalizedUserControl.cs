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
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Web.UI.Localization
{
	/// <summary>
	/// Inherits UserControls from this LocalizedUserControl instead of
	/// inheriting directly from System.Web.UI.UserControl. This class
	/// provide localization of user controls using a resource file
	/// </summary>
	public class LocalizedUserControl : System.Web.UI.UserControl
	{
		private ResourceManager _resourceManager = null;

		/// <summary>
		/// Localizes the page by loading the appropriate resource and then 
		/// walking through all of the controls.
		/// </summary>
		protected void LocalizeControls()
		{
			//Get the calling assembly object and the page that hosts the control
			System.Reflection.Assembly thisAssembly = 
				System.Reflection.Assembly.GetCallingAssembly();
			System.Type thisPageType = this.Page.GetType().BaseType;
			System.Type thisControlType = this.GetType().BaseType;
			
			string baseName = thisPageType.Namespace + ".template." +  
							thisControlType.Name;

			//Load the appropriate resource
			_resourceManager = new System.Resources.ResourceManager
							(baseName,  thisAssembly);

			//Do the localization
			LocalizationUtils localizationUtil = new LocalizationUtils();
			localizationUtil.ResourceManager = _resourceManager;
			localizationUtil.LocalizeControls(this.Controls);

		}

		/// <summary>
		/// Get a resource string from the resource manager.
		/// </summary>
		/// <param name="genericControl">The control for which the resource 
		/// string is being sought.</param>
		/// <param name="attributeSuffix">The attribute suffix for the 
		/// desired resource.</param>
		/// <returns></returns>
		public string GetResourceString(String name)
		{
			String resourceString = "";
			try
			{
				resourceString = _resourceManager.GetString(name);
			}
			catch (SystemException)
			{
				//String not in resource file (not localizable)
				//TODO: Do Catch Implementation

			}
			// returns the string from the resource file
			return resourceString;
		}

	}	

}
