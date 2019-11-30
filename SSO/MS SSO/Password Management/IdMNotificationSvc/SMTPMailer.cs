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
using System.Text;
using System.Web.Mail;
using System.Configuration;
using System.Diagnostics;
using Microsoft.ApplicationBlocks.Data;

namespace Contoso.Notifications
{
	/// <summary>
	/// Provides method used to send e-mail using SMTP sever
	/// </summary>
	public class SMTPMailer
	{

		/// <summary>
		/// Gets the details like To, CC, Subject and Content 
		/// to send mails respectively
		/// </summary>
		/// <param name="to">To Address</param>
		/// <param name="cc">CC Address</param>
		/// <param name="subject">Subject of the mail</param>
		/// <param name="body">Content of the mail</param>
		/// <returns>Boolean</returns>
		public static bool SendMail(string to, string cc, 
								string subject, StringBuilder body)
		{
			// Get the SMTP address from the config file
			string smtpServer = ConfigurationSettings.AppSettings.
														Get("SmtpServer");
			string fromAddress = ConfigurationSettings.
										AppSettings["fromAddress"];

			// If the server name is not found in the config file, 
			// log an entry to the event log
			if (smtpServer == null)
				EventLog.WriteEntry(IdMNotificationSvc.eventSource, 
						"Unable to locate smtp server from application " +
								"configuration file.", System.Diagnostics.
													EventLogEntryType.Error);
			try
			{

				// Assign the given parameters like To, CC, 
				// Subject, Body etc. to the properties and 
				// methods for constructing an e-mail message.
				MailMessage mailMessage = new MailMessage();
				mailMessage.To = to;
				mailMessage.Cc = cc;
				mailMessage.From = fromAddress;
				mailMessage.Subject = subject;
				mailMessage.Body = body.ToString();
	
				// Assign the name of the SMTP server, obtained 
				// from the config file and send the constructed
				// email message
				SmtpMail.SmtpServer = smtpServer;
				SmtpMail.Send(mailMessage);
			}

			// In case of any exception, get the inner text
			// of the message and throw the same
			catch (Exception exception)
			{
				string errorMessage = exception.Message;
				while (exception.InnerException != null)
				{
					errorMessage += "\n\nInner:\n" ;
					errorMessage += exception.InnerException.Message;
					exception = exception.InnerException;
				}
				EventLog.WriteEntry(IdMNotificationSvc.eventSource, 
										exception.Message, System.Diagnostics.
												EventLogEntryType.Warning);
				throw exception;
			}

			return true;
		}
	}
}
