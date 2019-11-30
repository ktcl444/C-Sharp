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
using System.Configuration.Install;

namespace Contoso.AppDevASPSample
{
	/// <summary>
	/// Creates an Event source when installing and deletes it
	/// on uninstall
	/// </summary>
	[RunInstaller(true)]
	public class EventlogInstaller : Installer
	{
		private const String SOURCE_NAME = "AppDevASPExtranet";

		/// <summary>
		/// Overrides Install and creates an event source 
		/// </summary>
		/// <param name="savedState"></param>
		public override void Install(IDictionary savedState)
		{
			base.Install(savedState);
			if (!System.Diagnostics.EventLog.SourceExists(SOURCE_NAME)) 
			{         
				System.Diagnostics.EventLog.CreateEventSource(
					SOURCE_NAME,"Application");
			}
		}

		/// <summary>
		/// Overrides Uninstall and deletes an event source 
		/// </summary>
		/// <param name="savedState"></param>
		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);
			if (System.Diagnostics.EventLog.SourceExists(SOURCE_NAME)) 
			{         
				System.Diagnostics.EventLog.DeleteEventSource(
					SOURCE_NAME,"Application");
			}
		}
	}
}
