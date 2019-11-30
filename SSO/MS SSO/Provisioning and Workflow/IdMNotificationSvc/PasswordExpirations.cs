///////////////////////////////////////////////////////////////////////////////
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
using System.Diagnostics;
using System.Configuration;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace Contoso.Notifications
{
	/// <summary>
	/// Interface used to get date/time values in 100 nanoseconds,
	/// which is defined in activeds.dll
	/// </summary>
	[ComImport(), Guid("9068270b-0939-11d1-8be1-00c04fd8d503"), 
		InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]

	public interface IADsLargeInteger 
	{
		// Gets the Higher and Lower part of IADsLargeInteger Interface
		[DispId(2)] int HighPart{get;}
		[DispId(3)] int LowPart{get;}
	}

	/// <summary>
	/// Provides the notification method implementation for
	/// user password expirations.
	/// </summary>
	public class PasswordExpirations
	{
		// Declaration for constant
		const int ADS_UF_DONT_EXPIRE_PASSWD = 0x10000;

		// Declaration for Global variables
		string userLDAPBindingString = String.Empty;
		string passwordPolicyDomain = System.Environment.UserDomainName;
		string remoteLDAPCredentials = String.Empty;
		string pwdExpiryNotification = String.Empty;
		string pwdExpirySubject = String.Empty;
		int hoursBetweenNotifications = 24;
		int daysBeforeNotification = 5;
		static DateTime lastRun;
		
		/// <summary>
		/// Gets and sets the values of the configuration
		/// parameters from the config file by parsing
		/// </summary>
		public PasswordExpirations()
		{
			try
			{

				// Retrieves the values of the variables from config file 
				userLDAPBindingString = ConfigurationSettings.AppSettings[ 
												"passwordLDAPBindingString" ];
				passwordPolicyDomain = ConfigurationSettings.AppSettings[ 
									"passwordPolicyDomainLDAPBindingString" ];
				hoursBetweenNotifications = Convert.ToInt32
							( ConfigurationSettings.AppSettings[ 
								"hoursBetweenPasswordExpiryNotifications" ] );
				daysBeforeNotification = Convert.ToInt32
							( ConfigurationSettings.AppSettings[ 
								"passwordExpiryNotificationTime" ] );
				remoteLDAPCredentials = ConfigurationSettings.AppSettings[ 
													"remoteLDAPCredentials" ];
				pwdExpiryNotification = ConfigurationSettings.
										  AppSettings["pwdExpiryNotification"];
				pwdExpirySubject = ConfigurationSettings.
											AppSettings["pwdExpirySubject"];
			}
			catch ( Exception exception )
			{
				EventLog.WriteEntry("IdMNotificationSvc", 
					exception.Message,	EventLogEntryType.Warning);
			}
		}	

		/// <summary>
		/// Notifies all the users regarding the password expiration by 
		/// sending an Email specifying the days left for expiration.
		/// </summary>
		/// <returns></returns>
		
		public void Notify()
		{

			// Declarations
			DateTime runNoEarlierThan;
			StringBuilder emailBody = new StringBuilder();
			DateTime maxDate = DateTime.MaxValue;
			long maxDateFileTime = maxDate.ToFileTime();
			TimeSpan maxPwdAge;
			DateTime passwordLastSet;
			TimeSpan timeToExpiration;
			DirectoryEntry domain = new DirectoryEntry();
			DirectoryEntry userContainer = new DirectoryEntry();
			DirectorySearcher searchUsers = new DirectorySearcher();
			int userAccountControl = 0;
			bool isPasswordExpires = false;
			long passwordLastSetFileTime;
			string remoteLDAPUser = String.Empty;
			string remoteLDAPPassword = String.Empty;
			
			try
			{
				// Checks for the Password Expiration period of the User
				// and sends out respective notification Email to the 
				// Users specifying the period of their Password expiration.
				runNoEarlierThan = 
					lastRun.AddHours( hoursBetweenNotifications );
				if ( DateTime.Now < runNoEarlierThan )
					return;

				EventLog.WriteEntry( "IdMNotificationSvc",
					"Processing password expriration notifications" , 
						EventLogEntryType.Information );

				if (remoteLDAPCredentials.Length > 0)
				{

					// Split the user ID and password from
					// the supplied remote credentials
					string[] credentials = 
						remoteLDAPCredentials.Split( new char[]{';'} );
					remoteLDAPUser = credentials[0];
					remoteLDAPPassword = credentials[1];
				}

				// Get domain MaxPwdAge
				if ( remoteLDAPUser != null & remoteLDAPUser != "")
				{
					domain = new DirectoryEntry( 
						passwordPolicyDomain, remoteLDAPUser, 
						remoteLDAPPassword, AuthenticationTypes.Secure );
				}
				else
				{
					domain = new DirectoryEntry( passwordPolicyDomain );
				}
				
				// Call the "GetTimeSpanValue" method and assign to maxPwdAge
				maxPwdAge = GetTimeSpanValue( domain.Properties[
												"MaxPwdAge" ].Value );

				// Get users and check if password expires and also when 
				// password was last set from the Active Directory
				if ( remoteLDAPUser != "" & remoteLDAPUser != null)
				{
					userContainer = new DirectoryEntry( userLDAPBindingString, 
									remoteLDAPUser, remoteLDAPPassword, 
										AuthenticationTypes.Secure );
				}
				else
				{
					userContainer = 
						new DirectoryEntry( userLDAPBindingString );
				}

				// Search the Users, Root and Scope using 
				// directory search
				searchUsers = new DirectorySearcher( 
					"(&(objectCategory=Person)(objectClass=user))");
						searchUsers.SearchRoot = userContainer;
						searchUsers.SearchScope = SearchScope.Subtree;

				// Performs Notification against each User seach results 
				// by sending Password expiration mail
				foreach ( SearchResult user in searchUsers.FindAll() )
				{

					//Check if "Password never expires" is set
					userAccountControl = ( int )user.Properties
											[ "UserAccountControl" ][0];
					isPasswordExpires = !( Convert.ToBoolean( 
						ADS_UF_DONT_EXPIRE_PASSWD & userAccountControl ) );

					if ( isPasswordExpires )
					{
						passwordLastSetFileTime = 
							( long )user.Properties[ "pwdLastSet" ][0];
						if ( passwordLastSetFileTime < maxDateFileTime )
						{

							// Calculate the following variables: password
							// last set and time to expiration
							passwordLastSet = DateTime.FromFileTime( 
												passwordLastSetFileTime );
							timeToExpiration = maxPwdAge.Subtract( 
								DateTime.Now.Subtract( passwordLastSet ) );
							if ( timeToExpiration.Days > 0 && 
								timeToExpiration.Days < daysBeforeNotification )
							{
								EventLog.WriteEntry( "IdMNotificationSvc", 
									String.Format( "Sending notification to {0}",
										user.Properties["name"][0] ), 
											EventLogEntryType.Information );
								//clear the message body if there is any
								if (emailBody.Length > 0)
								{
									emailBody.Replace(emailBody.ToString(),String.Empty);
								}
								emailBody.AppendFormat( pwdExpiryNotification 
														,timeToExpiration.Days);

								
								if(user.Properties.Contains("mail"))
								{
									// Send the mail with the following parameters
									SMTPMailer.SendMail(user.Properties
										["mail"][0].ToString(), null,
										pwdExpirySubject, emailBody);
								}
								else
								{
									EventLog.WriteEntry("IdMNotificationSvc", 
										String.Format( 
										"Password expiry notification to user {0} failed"+
										"as there is no 'mail' attribute for the user", 
										user.Properties["name"][0]), 
										EventLogEntryType.Warning);
								}
							}
						}
					}
				}	
			}
			
			catch ( Exception exception )
			{
				EventLog.WriteEntry( "IdMNotificationSvc", 
					exception.Message, EventLogEntryType.Warning );
			}

			// Set the Date time to present Date time
			lastRun = DateTime.Now;
		}

		/// <summary>
		/// Gets the time span value from IADsLargeInteger
		/// Interface. The IADsLargeInteger interface inherits the methods 
		/// of the standard COM interfaces.
		/// </summary>
		/// <param name="IADsLargeIntegerCOMObject">IADsLargeIntegerCOMObject
		/// </param>
		/// <returns>TimeSpan</returns>
		private TimeSpan GetTimeSpanValue( 
				object IADsLargeIntegerCOMObject )
		{
			TimeSpan timeSpanValue;
			IADsLargeInteger li = 
				( IADsLargeInteger )IADsLargeIntegerCOMObject;

			uint low = (uint) li.LowPart;
			long high = li.HighPart;
			high <<= 32;
			long total = low | high;
		
			// Calculate the time span value
			timeSpanValue = new TimeSpan( System.Math.Abs( total ) );
			return timeSpanValue;
		}
	}	
}
