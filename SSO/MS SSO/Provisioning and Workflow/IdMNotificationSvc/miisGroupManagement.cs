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
	/// user Group Mangement operations.
	/// </summary>
	public class miisGroupManagement
	{

		// Declaration
		int delayHoursBeforeSend = 0;
		string queueName = String.Empty;
		string groupChangeEmailBody = String.Empty;

		/// <summary>
		/// Gets and sets values for all the configuration
		/// parameters from the config file by parsing it
		/// </summary>
		public miisGroupManagement()
		{
			try
			{
				delayHoursBeforeSend = 
					Convert.ToInt32(ConfigurationSettings.AppSettings[
							"accountCreationTime"]);
				queueName = 
					ConfigurationSettings.AppSettings["groupAccountQueueName"];
				groupChangeEmailBody = ConfigurationSettings.
										AppSettings["groupChangeEmailBody"];
			}
			catch ( Exception exception )
			{
				EventLog.WriteEntry("IdMNotificationSvc",exception.Message , 
							EventLogEntryType.Warning);
			}
		}

		/// <summary>
		/// Notification will be carried on by Processing 
		/// upto 10 messages with a delay of X hours (allowing account
		/// and mailbox creation to replicate in AD and Exchange)
		/// </summary>
		/// <returns></returns>
		public void Notify()
		{
			
			// Declarations
			Message message = null;
			DateTime processNoEarlierThan;

			try
			{
				EventLog.WriteEntry("IdMNotificationSvc", 
					"Processing group membership notifications", 
					EventLogEntryType.Information);

				// Gets access to a queue on a Message Queuing server, 
				// and provides a Message Queuing internal transaction.
				MessageQueue queue = new MessageQueue(queueName);
				MessageQueueTransaction mqTransaction = new 
											MessageQueueTransaction();

				for ( int intValue = 0; intValue < 10; intValue++ )
				{

					// Set the properties for the MessageReadPropertyFilter
					queue.MessageReadPropertyFilter.Body = false;
					queue.MessageReadPropertyFilter.SentTime = true;
					message = queue.Peek(new TimeSpan(0,0,0,0,1));
					if (message != null)
					{
						processNoEarlierThan = 
							message.SentTime.AddHours(delayHoursBeforeSend);
						if (DateTime.Now > processNoEarlierThan)
						{
							// The first message is ready to be processed.
							queue.MessageReadPropertyFilter.Body = true;
							queue.Formatter = new XmlMessageFormatter( 
									new string[] { "System.String" } );
							mqTransaction.Begin();
							message = queue.Receive(mqTransaction);

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

			catch (MessageQueueException mqException)
			{
				// Log real errors to event log and ignore 
				// time-out for reading an empty queue
				if ( mqException.MessageQueueErrorCode != 
						MessageQueueErrorCode.IOTimeout )
					EventLog.WriteEntry( "IdMNotificationSvc", 
						String .Format("Message: {0} Code: {1}", 
						mqException.Message, mqException.MessageQueueErrorCode), 
						EventLogEntryType.Warning );
			}
			catch ( Exception exception )
			{
				EventLog.WriteEntry( "IdMNotificationSvc", 
					exception.Message, EventLogEntryType.Warning );
			}
		}

		/// <summary>
		/// Parses the Queue message to form an Email and notify 
		/// the User by sending E-mail to the user's account.
		/// </summary>
		/// <param name="message">Message to be parsed into Email</param>
		/// <param name="mqTransaction">Transaction to be carried out for message
		/// parsing into Email</param>
		/// <returns></returns>
		private void ProcessMessage( Message message, 
							MessageQueueTransaction mqTransaction )
		{

			// Start the process of parsing of message by first  
			// specifying the messages to be parsed
			EventLog.WriteEntry( "IdMNotificationSvc", 
				String.Format("Processing {0}", message.Body), 
				EventLogEntryType.Information );
			
			// Declarations
			XmlDocument body = new XmlDocument();
			XmlElement account;
			string to = String.Empty;
			string cc = String.Empty;
			string action = String.Empty;
			string groupDisplayName = String.Empty;
			string subject = String.Empty;
			StringBuilder emailBody = new StringBuilder(); 

			try
			{
				// Parse the Message into Email format and notify the 
				// user about the new account created. Also verify 
				// the User account and password
				body.LoadXml(message.Body.ToString());

				account = (XmlElement)body.SelectSingleNode("account");
				subject = account.Attributes.GetNamedItem("subject").Value;
				action = account.Attributes.GetNamedItem("action").Value;
				to = account.Attributes.GetNamedItem("email").Value;
				groupDisplayName = 
					account.Attributes.GetNamedItem( 
							"groupDisplayName" ).Value;
				emailBody = 
					emailBody.AppendFormat(groupChangeEmailBody ,action.ToLower(), 
					groupDisplayName);

				// Send the mail based on the criterion passed and 
				// commit the transaction upon success
				SMTPMailer.SendMail(to, cc, subject, emailBody);
				mqTransaction.Commit();
			}
			
			catch ( Exception exception )
			{
				EventLog.WriteEntry( "IdMNotificationSvc", 
					exception.Message, EventLogEntryType.Warning );
			}						
		}
	}
}
