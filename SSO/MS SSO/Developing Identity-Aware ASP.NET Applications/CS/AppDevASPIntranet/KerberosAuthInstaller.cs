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
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.DirectoryServices;
using Contoso.Configuration.Install;

namespace Contoso.AppDevASPSample
{
	/// <summary>
	/// This installer makes sure anonymous access is disabled
	/// for this VDIR.
	/// </summary>
	[RunInstaller(true)]
	public class KerberosInstaller : Installer
	{
		private const Int32  MD_AUTH_ANONYMOUS = 1;
		private const Int32 MD_AUTH_NT = 4;

		public override void Install(IDictionary savedState)
		{
			base.Install(savedState);

			//Disable anonymous and let IIS do Kerberos authN
			SetSPNEGOAuthN();

		}


		//Clears the anonymous bit and sets the SPNEGO bit in IIS
		//metabase for the VDIR
		private void SetSPNEGOAuthN()
		{
			String strVDir = null;
			Int32 intServerNum = 1;
			
			//Is being run from installUtil, not setup
			if (this.Context.Parameters["VDir"] == null)
				return;

			try
			{
				strVDir = this.Context.Parameters["VDir"];
				intServerNum = IisInstallerUtil.FindServerNum(this.Context.Parameters["Port"]);
			}
			catch
			{
				throw new Exception("Unable to read VDir or Port param");
			}

			String strObjectPath = "IIS://" + System.Environment.MachineName + "/W3SVC/" + intServerNum.ToString() + "/ROOT/" + strVDir;

			DirectoryEntry IISVdir = IisInstallerUtil.GetIisObject(strObjectPath);

			try
			{
				//Change the MD_AUTH_ANONYMOUS
				if (Convert.ToBoolean(IISVdir.Properties["AuthAnonymous"].Value) != false)
				{
					IISVdir.Properties["AuthFlags"].Value = Convert.ToInt32(IISVdir.Properties["AuthFlags"].Value) ^ MD_AUTH_ANONYMOUS;
				}
				//Make sure IIS Integrated is checked when disabling anonymous
				IISVdir.Properties["AuthFlags"].Value = Convert.ToInt32(IISVdir.Properties["AuthFlags"].Value) | MD_AUTH_NT;
				IISVdir.Invoke("SetInfo", null);
			}
			catch
			{
				throw new Exception("Unable to enable/disable anonymous access");
			}
		}

	}
}
