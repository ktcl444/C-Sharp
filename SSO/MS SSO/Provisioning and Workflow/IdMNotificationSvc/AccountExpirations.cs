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
	/// Provides the notification method implementation for
	/// user account expiry notifications.
	/// </summary>
	
	public class AccountExpirations
	{

		// Declarations 
		string userLDAPBindingString = String.Empty;
		string bodyAccountExpiry = String.Empty;
		string subAccountExpiry = String.Empty;
		int hoursBetweenNotifications = 24;
		int daysBetweenNotifications = 5;
		DateTime lastRun;

		/// <summary>
		/// Collects all the required parameters from
		/// the corresponding locations at config file
		/// </summary>
		public AccountExpirations()
		{
			try
			{
				// Parse the config file to get the configuration parameters
				userLDAPBindingString = ConfigurationSettings.AppSettings[
												   "accountLDAPBindingString"];
				hoursBetweenNotifications = Convert.ToInt32
								(ConfigurationSettings.AppSettings[
									"hoursBetweenAccountExpiryNotifications"]);
				daysBetweenNotifications = 
						Convert.ToInt32(ConfigurationSettings.AppSettings[
								"accountExpiryNotificationTime"]);
				bodyAccountExpiry = ConfigurationSettings.
										AppSettings["bodyAccountExpiry"];
				subAccountExpiry = ConfigurationSettings.
										AppSettings["subAccountExpiry"];
			}
			catch ( Exception exception )
			{
				EventLog.WriteEntry("IdMNotificationSvc", 
						exception.Message , EventLogEntryType.Warning);
			}
		}

		/// <summary>
		/// Queries the Directory Server to get user account expiry details
		/// and notifies the users whose account are about to expire. 
		/// Sends e-mail specifying the days left for expiration.
		/// </summary>
		/// <returns></returns>
		public void Notify()
		{

			// Declaration for variables
			DateTime runNoEarlierThan;
			StringBuilder emailBody = new StringBuilder(); 
			DateTime maxDate = DateTime.MaxValue;
			long maxDateFileTime = maxDate.ToFileTime();
			DateTime expiryDate;
			DateTime notificationDate;
			long accountExpiresFileTime = 0;
			DirectoryEntry userContainer = new DirectoryEntry();
			DirectorySearcher searchUsers = new DirectorySearcher();

			try
			{
				// check if the hours between notifications are as
				// specified, otherwise return
				runNoEarlierThan = 
					lastRun.AddHours(hoursBetweenNotifications);
				if (DateTime.Now < runNoEarlierThan)
					return;

				EventLog.WriteEntry("IdMNotificationSvc",
					"Processing account expriration notifications" , 
										EventLogEntryType.Information);

				// Get the address of the Active directory from the
				// config file and search for the users, root and scope
				userContainer = new DirectoryEntry(userLDAPBindingString);
				searchUsers = new DirectorySearcher(
						"(&(objectCategory=Person)(objectClass=user))");
				searchUsers.SearchRoot = userContainer;
				searchUsers.SearchScope = SearchScope.Subtree;

				// Based on the search results, conforming to security
				// policies, Account expiration mails are sent
				foreach (SearchResult user in searchUsers.FindAll())
				{
					accountExpiresFileTime = 
						(long)user.Properties["accountExpires"][0];
					if (accountExpiresFileTime >0 && 
						accountExpiresFileTime < maxDateFileTime)
					{

						// Calculate the expiration date for an user
						expiryDate = 
							DateTime.FromFileTime(accountExpiresFileTime);
						if (expiryDate > 
							DateTime.MinValue.AddDays(
								daysBetweenNotifications))
						{

							// Manipulate notification date using expiry date
							notificationDate = 
								expiryDate.Subtract(
									new TimeSpan(daysBetweenNotifications,
										0, 0, 0, 0));

							// If the notification date is less than or equal
							// to today's date and time, then compose and
							// send the mail appropriately
							if (notificationDate <= DateTime.Now)
							{
								EventLog.WriteEntry("IdMNotificationSvc", 
									String.Format( 
										"Sending notification to {0}", 
											user.Properties["name"][0]), 
											EventLogEntryType.Information);
								emailBody.AppendFormat( 
									bodyAccountExpiry , expiryDate);
								if(user.Properties.Contains("mail"))
								{
									SMTPMailer.SendMail(
										user.Properties["mail"][0].ToString(), 
										null, subAccountExpiry, 
										emailBody);
								}
								else
								{
									EventLog.WriteEntry("IdMNotificationSvc", 
										String.Format( 
										"Account expiry notification to user {0} failed"+
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
					exception.Message, EventLogEntryType.Error );
			}

			// Set the timestamp as current time
			lastRun = DateTime.Now;

		}
	}
}
