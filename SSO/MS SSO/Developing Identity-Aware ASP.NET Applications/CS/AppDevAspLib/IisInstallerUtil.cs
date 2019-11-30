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
using System.DirectoryServices;

namespace Contoso.Configuration.Install
{
	/// <summary>
	/// Tools that wrap common IIS configuration code. Used by
	/// KerberosAuthInstaller and FormsAuthInstaller
	/// </summary>
	public class IisInstallerUtil
	{
		private const Int32 MD_SERVER_STATE_STARTED = 2;

		//Find the server number for the virtual server hosting this VDIR
		public static Int32 FindServerNum(String port)
		{
			String machineName = System.Environment.MachineName;
			String IISObjectPath = "IIS://" + machineName + "/W3SVC";
			DirectoryEntry IISObject;
			ArrayList servers = new ArrayList();
			Int32 serverNumber;
			String[] nameParts;
			String name;
			String serverBindings;
			String[] serverBindingParts;
			String serverPort;

			//Iterates through the W3SVC folder to get the name
			//of each child object.
			IISObject = GetIisObject(IISObjectPath);

			foreach (DirectoryEntry IISChildObject in IISObject.Children)
			{
				nameParts  = IISChildObject.Path.Split(new Char[] {'/'});
				name = nameParts[nameParts.Length-1];
				try
				{
					//If the name can be converted to an integer (port number),
					//add it to the Servers collection.
					serverNumber = Convert.ToInt32(name);
					servers.Add(serverNumber);
				}
				catch
				{
					//If the name cannot be converted to an integer,
					//it isn't a port and can be ignored.
				}
			}

			//Iterates through each server, removing each inactive server
			//without the correct port.
			for (Int32 i = 0;  i < servers.Count; i++)
			{
				IISObjectPath = "IIS://" + machineName + "/W3SVC/" + servers[i].ToString();
				IISObject = GetIisObject(IISObjectPath);
			
				//Gets the Port Number of the current IISObject.
				serverBindings = (String)IISObject.Properties["ServerBindings"].Value;
				serverBindingParts = serverBindings.Split(new char[] {':'});
				serverPort = serverBindingParts[1];
								
				//Determines if this is our server. IIS can only have one 
				//active port, so if the port is active it is the port where
				//the application is installed.
				if (port == serverPort)
					if(Convert.ToInt32(IISObject.Properties["ServerState"].Value) == MD_SERVER_STATE_STARTED)
						return Convert.ToInt32(servers[i]);
					else						
						//Not the active Port, so remove it from the
						//collection.
						servers.RemoveAt(i);
			}
						
			//Checks how many servers are left. If one, we've found it
			//otherwise, report an error.
			switch (servers.Count)
			{
				case 0:
					throw new Exception("No Active Servers with the requested port were found. Port=" + port + ". ");
				case 1:
					return Convert.ToInt32(servers[0]);
				default:
					throw new Exception("More than one Active servers with the requested port were found. Port=" + port + ". ");
			}
		}

		//Verify if a certain object exists
		public static DirectoryEntry GetIisObject(String strFullObjectPath)
		{
			DirectoryEntry IISObject;

			try
			{
				IISObject = new DirectoryEntry(strFullObjectPath);
				return IISObject;
			}
			catch (Exception ex)
			{
				throw new Exception("Error opening: " + strFullObjectPath + ". " + ex.Message);
			}
		}
	}
}
