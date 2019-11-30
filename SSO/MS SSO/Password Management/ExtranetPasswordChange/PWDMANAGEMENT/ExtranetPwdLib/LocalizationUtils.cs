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
using System.Reflection;

namespace Contoso.Web.UI.Localization
{
	/// <summary>
	/// Helper class to recursively localize controls. Used by classes
	/// LocalizedPage and LocalizedUserControl to localize all controls
	/// on a page or a user control
	/// </summary>
	public class LocalizationUtils
	{
		private ResourceManager resourceManager = null;

		public ResourceManager ResourceManager
		{
			get {return resourceManager;}
			set {resourceManager = value;}
		}

		/// <summary>
		/// Get a resource string for a control property from the 
		/// resource manager.
		/// </summary>
		/// <param name="genericControl">The control for which the resource 
		/// string is being sought.</param>
		/// <param name="attributeSuffix">The attribute suffix for the desired
		/// resource.</param>
		/// <returns></returns>
		private string GetResourceString(System.Web.UI.Control genericControl, string attributeSuffix)
		{
			object theObject = null;
			try
			{
				theObject = resourceManager.GetObject
					(genericControl.ID + attributeSuffix);
			}
			catch (SystemException)
			{
				//TODO: Do Catch Implementation
				
			}
			return (string)theObject;
		}

		/// <summary>
		/// Get an assembly global resource string from an a assembly
		/// global resource manager.
		/// </summary>
		/// <param name="messageId"></param>
		/// <returns></returns>
		public static string GetGlobalString(string messageId)
		{
			ResourceManager globalResourceManager = null;

			//Grab the browser's requested language and apply it to the current
			//thread or use "en" if not specified
			if (HttpContext.Current.Request.UserLanguages != null && 
				HttpContext.Current.Request.UserLanguages.Length>0)
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = 
					System.Globalization.CultureInfo.CreateSpecificCulture
					(HttpContext.Current.Request.UserLanguages[0]);
			}
			else
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = 
					System.Globalization.CultureInfo.CreateSpecificCulture("en");
			}
			System.Threading.Thread.CurrentThread.CurrentUICulture = 
				System.Threading.Thread.CurrentThread.CurrentCulture;

			Assembly thisAssembly = Assembly.GetCallingAssembly();
			globalResourceManager = new ResourceManager(
				thisAssembly.GetName().Name + "." + thisAssembly.GetName().Name,
				thisAssembly);
			
			object theObject = null;
			try
			{
				theObject = globalResourceManager.GetObject(messageId);
			}
			catch (Exception)
			{
				return "Can't find the localized message for ID '" + messageId + "'";
			}
			return (string)theObject;
		}


		/// <summary>
		/// Localizes the controls.  Recursively calls itself if a control is
		/// found that has child controls.
		/// </summary>
		/// <param name="controls">The collection of controls that will be 
		/// localized.</param>
		public void LocalizeControls(System.Web.UI.ControlCollection controls)
		{
			int i = 0;
			foreach (System.Web.UI.Control genericControl in controls)
			{
				if (genericControl.HasControls() == true)
					LocalizeControls(genericControl.Controls);

				if (genericControl is System.Web.UI.WebControls.AdRotator)
				{
					System.Web.UI.WebControls.AdRotator theControl = 
						(System.Web.UI.WebControls.AdRotator)genericControl;
					theControl.ToolTip = GetResourceString(genericControl, ".ToolTip");
					theControl.AdvertisementFile = GetResourceString
						(genericControl, ".AdvertisementFile");
				}
				else if (genericControl is System.Web.UI.WebControls.Button)
				{
					System.Web.UI.WebControls.Button theControl = 
						(System.Web.UI.WebControls.Button)genericControl;
					theControl.Text = GetResourceString(genericControl, ".Text");
					theControl.ToolTip = GetResourceString(genericControl, ".ToolTip");
				}
				else if (genericControl is System.Web.UI.WebControls.Calendar)
				{
					System.Web.UI.WebControls.Calendar theControl = 
						(System.Web.UI.WebControls.Calendar)genericControl;
					theControl.ToolTip = GetResourceString(genericControl, ".ToolTip");
				}
				else if (genericControl is System.Web.UI.WebControls.CheckBox)
				{
					System.Web.UI.WebControls.CheckBox theControl = 
						(System.Web.UI.WebControls.CheckBox)genericControl;
					theControl.Text = GetResourceString(genericControl, ".Text");
					theControl.ToolTip = GetResourceString(genericControl, ".ToolTip");
				}
				else if (genericControl is System.Web.UI.WebControls.CheckBoxList)
				{
					System.Web.UI.WebControls.CheckBoxList theControl = 
						(System.Web.UI.WebControls.CheckBoxList)genericControl;
					i = 0;
					foreach (System.Web.UI.WebControls.ListItem li in theControl.Items)
					{
						li.Text = GetResourceString(genericControl, "." + i.ToString() + ".Text");
						i++;
					}
					theControl.ToolTip = GetResourceString(genericControl, ".ToolTip");
				}
				else if (genericControl is System.Web.UI.WebControls.DataGrid)
				{
					System.Web.UI.WebControls.DataGrid theControl = 
						(System.Web.UI.WebControls.DataGrid)genericControl;
					theControl.ToolTip = GetResourceString(genericControl, ".ToolTip");
					foreach (System.Web.UI.WebControls.DataGridColumn dgCol in 
						theControl.Columns)
					{
						System.Web.UI.WebControls.BoundColumn bCol = 
							dgCol as System.Web.UI.WebControls.BoundColumn;
						dgCol.HeaderText = GetResourceString
							(genericControl, "." + bCol.DataField + ".HeaderText");
						dgCol.FooterText = GetResourceString
							(genericControl, "." + bCol.DataField + ".FooterText");
					}
				}
				else if (genericControl is System.Web.UI.WebControls.DropDownList)
				{
					System.Web.UI.WebControls.DropDownList theControl = 
						(System.Web.UI.WebControls.DropDownList)genericControl;
					i = 0;
					foreach (System.Web.UI.WebControls.ListItem li in theControl.Items)
					{
						li.Text = GetResourceString(genericControl, "." + i.ToString()
													+ ".Text");
						i++;
					}
				}
				else if (genericControl is System.Web.UI.WebControls.Image)
				{
					System.Web.UI.WebControls.Image theControl = 
						(System.Web.UI.WebControls.Image)genericControl;
					theControl.AlternateText = 
						GetResourceString(genericControl, ".AlternateText");
					theControl.ImageUrl = 
						GetResourceString(genericControl, ".ImageUrl");
					theControl.ToolTip = 
						GetResourceString(genericControl, ".ToolTip");
				}
				else if (genericControl is System.Web.UI.WebControls.ImageButton)
				{
					System.Web.UI.WebControls.ImageButton theControl = 
						(System.Web.UI.WebControls.ImageButton)genericControl;
					theControl.AlternateText = 
						GetResourceString(genericControl, ".AlternateText");
					theControl.ImageUrl = 
						GetResourceString(genericControl, ".ImageUrl");
					theControl.ToolTip = 
						GetResourceString(genericControl, ".ToolTip");
				}
				else if (genericControl is System.Web.UI.WebControls.HyperLink)
				{
					System.Web.UI.WebControls.HyperLink theControl = 
						(System.Web.UI.WebControls.HyperLink)genericControl;
					theControl.Text = 
						GetResourceString(genericControl, ".Text");
					theControl.ToolTip = 
						GetResourceString(genericControl, ".ToolTip");
				}
				else if (genericControl is System.Web.UI.WebControls.Label)
				{
					System.Web.UI.WebControls.Label theControl = 
						(System.Web.UI.WebControls.Label)genericControl;
					theControl.Text = 
						GetResourceString(genericControl, ".Text");
					theControl.ToolTip = 
						GetResourceString(genericControl, ".ToolTip");
				}
				else if (genericControl is System.Web.UI.WebControls.ListBox)
				{
					System.Web.UI.WebControls.ListBox theControl = 
						(System.Web.UI.WebControls.ListBox)genericControl;
					i = 0;
					foreach (System.Web.UI.WebControls.ListItem li in theControl.Items)
					{
						li.Text = GetResourceString(genericControl, "." + i.ToString() 
							+ ".Text");
						i++;
					}
				}
				else if (genericControl is System.Web.UI.WebControls.Literal)
				{
					System.Web.UI.WebControls.Literal theControl = 
						(System.Web.UI.WebControls.Literal)genericControl;
					theControl.Text = GetResourceString(genericControl, ".Text");
				}
				else if (genericControl is System.Web.UI.WebControls.LinkButton)
				{
					System.Web.UI.WebControls.LinkButton theControl = 
						(System.Web.UI.WebControls.LinkButton)genericControl;
					theControl.Text = 
						GetResourceString(genericControl, ".Text");
					theControl.ToolTip = 
						GetResourceString(genericControl, ".ToolTip");
				}
				else if (genericControl is System.Web.UI.WebControls.RadioButton)
				{
					System.Web.UI.WebControls.RadioButton theControl = 
						(System.Web.UI.WebControls.RadioButton)genericControl;
					theControl.Text = GetResourceString(genericControl, ".Text");
					theControl.ToolTip = GetResourceString(genericControl, ".ToolTip");
				}
				else if (genericControl is System.Web.UI.WebControls.RadioButtonList)
				{
					System.Web.UI.WebControls.RadioButtonList theControl = 
						(System.Web.UI.WebControls.RadioButtonList)genericControl;
					i = 0;
					foreach (System.Web.UI.WebControls.ListItem li in theControl.Items)
					{
						li.Text = GetResourceString(genericControl, "." + i.ToString()
							+ ".Text");
						i++;
					}
					theControl.ToolTip = GetResourceString(genericControl, ".ToolTip");
				}
				else if (genericControl is System.Web.UI.WebControls.TextBox)
				{
					System.Web.UI.WebControls.TextBox theControl = 
						(System.Web.UI.WebControls.TextBox)genericControl;
					theControl.ToolTip = 
						GetResourceString(genericControl, ".ToolTip");
				}
			}
		}

	}
}
