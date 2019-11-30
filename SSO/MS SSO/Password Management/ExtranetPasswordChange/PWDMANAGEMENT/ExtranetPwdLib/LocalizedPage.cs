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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Web.UI.Localization
{
	/// <summary>
	/// Inherits forms fom this LocalizedPage instead of
	/// inheriting directly from System.Web.UI.Page. This class
	/// provide locilization of web forms using a resource file
	/// </summary>
	public class LocalizedPage : System.Web.UI.Page
	{
		private System.Resources.ResourceManager _resourceManager = null;

		/// <summary>
		/// Localizes the page by loading the appropriate resource and then 
		/// walking through all of the controls.
		/// </summary>
		protected void LocalizeControls()
		{
			//Get the calling assembly object
			System.Reflection.Assembly thisAssembly = 
				System.Reflection.Assembly.GetCallingAssembly();
			string baseName = this.GetType().BaseType.FullName;

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
		/// <param name="genericControl">The control for which the 
		/// resource string is being sought.</param>
		/// <param name="attributeSuffix">The attribute suffix for 
		/// the desired resource.</param>
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
			return resourceString;
		}

		/// <summary>
		/// Page load event handler.  Starts the localization process for the page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);

			// Here we grab the browser's requested language and apply it to the current thread
			if (Request.UserLanguages != null && Request.UserLanguages.Length>0)
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
			}
			else
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en");
			}
			System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
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
