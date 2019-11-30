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
	/// contractor provisioning workflow sequences.
	/// </summary>
	
	public class ContractorWorkflow
	{
		string queueName = String.Empty;
		string subContractorApproved = String.Empty;
		string msgContractorApproved = String.Empty;
		string pwdContractorApproved = String.Empty;
		string subContractorDenied = String.Empty;
		string bodyContractorDenied = String.Empty;
		string subContractorRequest = String.Empty;
		string bodyContractorRequest = String.Empty;
		string lnkContractorRequest = String.Empty;
		int messagesToProcess = 10;

		/// <summary>
		/// Gets the location of the contractor queue
		/// </summary>
		public ContractorWorkflow()
		{
			try
			{
				queueName = ConfigurationSettings.AppSettings
											["contractorQueueName"];
				subContractorApproved = ConfigurationSettings.
									AppSettings["subContractorApproved"];
				msgContractorApproved = ConfigurationSettings.
									AppSettings["msgContractorApproved"];
				pwdContractorApproved = ConfigurationSettings.
									AppSettings["pwdContractorApproved"];
				subContractorDenied = ConfigurationSettings.
									AppSettings["subContractorDenied"];
				bodyContractorDenied = ConfigurationSettings.
									AppSettings["bodyContractorDenied"];
				subContractorRequest = ConfigurationSettings.
									AppSettings["subContractorRequest"];
				bodyContractorRequest = ConfigurationSettings.
									AppSettings["bodyContractorRequest"];
				lnkContractorRequest = ConfigurationSettings.
									AppSettings["lnkContractorRequest"];
			}
			catch (Exception exception)
			{
				EventLog.WriteEntry("IdMNotificationSvc", 
						exception.Message , EventLogEntryType.Warning);
			}
		}

		/// <summary>
		/// Process up to 10 messages with a delay of X hours
		/// (allowing account and mailbox creation
		/// to replicate in AD and Exchange)
		/// </summary>
		/// <returns></returns>
		public void Notify()
		{

			// Declarations
			Message message = null;

			try
			{
				EventLog.WriteEntry("IdMNotificationSvc", 
					"Processing Contractor Workflow notifications", 
					EventLogEntryType.Information);

				// Gets access to a queue on a Message Queuing server, 
				// and provides a Message Queuing internal transaction.
				MessageQueue messageQueue = new MessageQueue(queueName);
				MessageQueueTransaction messageQueueTransaction = 
					new MessageQueueTransaction();

				for (int intValue = 0; intValue < messagesToProcess; intValue++)
				{		
			
					// Set the properties for the MessageReadPropertyFilter
					messageQueue.MessageReadPropertyFilter.Body = false;
					messageQueue.MessageReadPropertyFilter.SentTime = true;
					message = messageQueue.Peek(new TimeSpan(0,0,0,0,1));
					if (message != null)
					{

						//The first message is ready to be processed...
						messageQueue.MessageReadPropertyFilter.Body = true;
						messageQueue.Formatter = new XmlMessageFormatter
											(new string[] {"System.String"});
						messageQueueTransaction.Begin();
						message = 
							messageQueue.Receive(messageQueueTransaction);

						// Calling the method for parsing the message
						// and notifying the User by sending a mail
						ProcessMessage(message, messageQueueTransaction);
					}
				}
			}

			catch (MessageQueueException mqException)
			{

				// Log real errors to event log and ignore time-out 
				// for reading an empty messageQueue
				if (mqException.MessageQueueErrorCode != 
					MessageQueueErrorCode.IOTimeout)
					EventLog.WriteEntry("IdMNotificationSvc", String .Format(
						"Message: {0} Code: {1}", mqException.Message, 
									 mqException.MessageQueueErrorCode), 
												EventLogEntryType.Warning);
			}
			catch (Exception exception)
			{
				EventLog.WriteEntry("IdMNotificationSvc", 
					exception.Message, EventLogEntryType.Warning);
			}
		}

		/// <summary>
		/// Parse the message from the queue and send e-mail with the
		/// attributes provided by the message
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="messageQueueTransaction">Message Queue Transaction
		/// </param>
		/// <returns></returns>
		private void ProcessMessage(Message message, 
						MessageQueueTransaction messageQueueTransaction)
		{
			EventLog.WriteEntry("IdMNotificationSvc", 
						String.Format("Processing {0}", message.Body), 
											EventLogEntryType.Information);
			
			// Declarations
			XmlDocument body = new XmlDocument();
			XmlElement account;
			string to = String.Empty;
			string cc = String.Empty;
			string subject = String.Empty;
			StringBuilder emailBody = new StringBuilder(); 

			try
			{

				// Load the body of one message at a time to XML format,
				// and parse all the values and then assign the corresponding
				// values in it to various declared variables
				body.LoadXml(message.Body.ToString());

				account = (XmlElement)body.SelectSingleNode("account");
				string managerSmtp = account.Attributes.
					GetNamedItem("managerSmtp").Value;
				string approverSmtp = account.Attributes.
					GetNamedItem("approverSmtp").Value;
				string initialPassword = account.Attributes.
					GetNamedItem("userPassword").Value;
				string lastName = account.Attributes.
					GetNamedItem("lastName").Value;
				string firstName = account.Attributes.
					GetNamedItem("firstName").Value;

				if(message.Label == "ContractorApproved")
				{

					// If the message is for approved contractor, 
					// assign the following values to the properties of mail
					// to be sent to the manager, who raised the request
					to = managerSmtp;
					subject = subContractorApproved;
					emailBody = emailBody.AppendFormat(msgContractorApproved, 
													firstName, lastName);
					// The password is getting sent from the SSProv app, 
					// but it's not being used because the provisioning 
					// code in the MIIS Metaverse Extension always generates
					// an initial password for any new account. The following
					// should be uncommented if MIIS or some other provisioning
					// agent is not setting a new password.
					//emailBody = emailBody.AppendFormat
					//			(pwdContractorApproved, initialPassword);
				}
				else if(message.Label == "ContractorDenied")
				{

					// If the message is for denied contractor, 
					// assign the following values to the properties of mail
					// to be sent to the manager, who raised the request
					to = managerSmtp;
					subject = subContractorDenied;
					emailBody = emailBody.AppendFormat(bodyContractorDenied, 
						firstName, lastName);
				}
				else if(message.Label == "ContractorRequest")
				{

					// If the message was for a new contractor request
					// assign the following values to the properties of mail
					// to be sent to the approver, for each new request
					to = approverSmtp;
					subject = subContractorRequest;
					emailBody = emailBody.AppendFormat( bodyContractorRequest );
					emailBody = emailBody.AppendFormat( lnkContractorRequest );
				}

				// Send the mail based on the criterion passed and 
				// commit the transaction upon success
				SMTPMailer.SendMail(to, cc, subject, emailBody);
				messageQueueTransaction.Commit();				
			}

			catch (Exception exception)
			{
				messageQueueTransaction.Abort();
				EventLog.WriteEntry("IdMNotificationSvc",
					exception.Message, EventLogEntryType.Warning);
			}
		}
	}
}
