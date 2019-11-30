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
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Messaging;
using System.Diagnostics;
using System.Configuration;

namespace Contoso.Notifications
{
	/// <summary>
	/// Provides the notification method implementation for
	/// user account provisioning.
	/// </summary>
	
	public class AccountProvisioning
	{

		// Declarations
		int delayHoursBeforeSend = 1;
		string queueName = String.Empty;
		string subMail = String.Empty;
		string bodyMail = String.Empty;
		string msgUserPassword = String.Empty;

		/// <summary>
		/// Retrives values for the cutomizable parameters from the config file
		/// </summary>
		public AccountProvisioning()
		{
			try
			{
				// Retrieves the values for the variables
				// from the config file
				delayHoursBeforeSend = 	Convert.ToInt32( 
					ConfigurationSettings.AppSettings["accountCreationTime"] );
				queueName = ConfigurationSettings.AppSettings
												["accountQueueName"];
				subMail = ConfigurationSettings.
									AppSettings["accountProvisioningMailSub"];
				bodyMail = ConfigurationSettings.
									AppSettings["accountProvisioningMailBody"];
				msgUserPassword = ConfigurationSettings.
									AppSettings["accountPasswordMailBody"];
			}
			catch ( Exception exception )
			{
				EventLog.WriteEntry("IdMNotificationSvc",exception.Message , 
						EventLogEntryType.Warning);
			}
		}

		/// <summary>
		/// Processes the messages present in the provisioning message queue.
		/// Notifications are carried on by Processing upto 10 messages with a 
		/// delay of X hours (allowing account and mailbox creation to replicate
		/// in AD and Exchange)
		/// </summary>
		/// <returns></returns>
		public void Notify()
		{
			
			// Declarations
			Message message = null;
			DateTime processNoEarlierThen;

			try
			{
				EventLog.WriteEntry("IdMNotificationSvc", 
					"Processing account provisioning notifications", 
					EventLogEntryType.Information);
				// Gets access to a queue on a Message Queuing server, 
				// and provides a Message Queuing internal transaction.
				MessageQueue messageQueue = new MessageQueue( queueName );
				MessageQueueTransaction mqTransaction = 
											new MessageQueueTransaction();

				for ( int intValue = 0; intValue < 10; intValue++ )
				{

					// Set the properties for the MessageReadPropertyFilter
					messageQueue.MessageReadPropertyFilter.Body = false;
					messageQueue.MessageReadPropertyFilter.SentTime = true;
					message = messageQueue.Peek(new TimeSpan(0,0,0,0,1));
					if (message != null)
					{
						processNoEarlierThen = 
							message.SentTime.AddHours(delayHoursBeforeSend);
						if (DateTime.Now > processNoEarlierThen)
						{

							// The first message is ready to be processed.
							messageQueue.MessageReadPropertyFilter.Body = true;
							messageQueue.Formatter = new XmlMessageFormatter(
											new string[] {"System.String"});
							mqTransaction.Begin();
							message = messageQueue.Receive(mqTransaction);

							// Calling the method for parsing the message
							// and notifying the User by sending a mail
							ProcessMessage(message, mqTransaction);
						}
						else
						{
							// The first message is to new, wait...
							return;
						}
					}
				}
			}
			
			catch ( MessageQueueException messageQueueException )
			{
				// Log real errors to event log and ignore 
				// time-out for reading an empty messageQueue
				if ( messageQueueException.MessageQueueErrorCode != 
					MessageQueueErrorCode.IOTimeout )
					EventLog.WriteEntry( "IdMNotificationSvc", 
						String .Format("Message: {0} Code: {1}", 
						messageQueueException.Message, 
						messageQueueException.MessageQueueErrorCode), 
						EventLogEntryType.Warning );
			}
			catch ( Exception exception )
			{
				EventLog.WriteEntry("IdMNotificationSvc", 
						exception.Message, EventLogEntryType.Error);
			}
		}

		/// <summary>
		/// Parses the message into Email and notify 
		/// the User by sending E-mail to the user account.
		/// </summary>
		/// <param name="message">Message to be parsed into Email</param>
		/// <param name="mqTransaction">Transaction to be carried out for 
		/// message parsing into Email</param>
		/// <returns></returns>
		private void ProcessMessage( 
				Message message, MessageQueueTransaction mqTransaction )
		{		
			// Start the process of parsing of message by first  
			// specifying the messages to be parsed
			EventLog.WriteEntry("IdMNotificationSvc", 
				String.Format("Processing {0}", message.Body), 
						EventLogEntryType.Information);
			
			// Declarations
			XmlDocument body = new XmlDocument();
			XmlElement account;
			string to = String.Empty;
			string cc = String.Empty;
			string accountName = String.Empty;
			string accountId = String.Empty;
			string initialPassword = String.Empty;
			string subject = subMail;
			string userPrincipalName = String.Empty;
			StringBuilder emailBody = new StringBuilder(); 

			try
			{

				// Parse the Message into Email format and notify the 
				// user about the new account created. Also verify 
				// the User account and password
				body.LoadXml(message.Body.ToString());

				account = (XmlElement)body.SelectSingleNode("account");
				to = account.Attributes.GetNamedItem("managerSmtp").Value;
				accountId = 
					account.Attributes.GetNamedItem("employeeId").Value;
				initialPassword = 
					account.Attributes.GetNamedItem("userPassword").Value;
				accountName = 
					account.Attributes.GetNamedItem("employeeName").Value;
				userPrincipalName = 
					account.Attributes.GetNamedItem("userPrincipalName").Value;
				emailBody = 
					emailBody.AppendFormat( bodyMail, accountName, accountId );
				emailBody = 
					emailBody.AppendFormat( msgUserPassword,initialPassword);
				if (userPrincipalName.ToLower().IndexOf("@perimeter.contoso.com") >=0)
				{
					emailBody.AppendFormat("This is the user's external account (Extranet).");
				}
				else
				{
					emailBody.AppendFormat("This is the user's internal account (Intranet).");
				}
				
				// Send the mail based on the criterion passed and 
				// commit the transaction upon success
				SMTPMailer.SendMail( to, cc, subject, emailBody );
				mqTransaction.Commit();
			}

			catch ( Exception exception )
			{	
				mqTransaction.Abort();
				EventLog.WriteEntry("IdMNotificationSvc", 
					exception.Message, EventLogEntryType.Warning);
			}
		}
	}
}
