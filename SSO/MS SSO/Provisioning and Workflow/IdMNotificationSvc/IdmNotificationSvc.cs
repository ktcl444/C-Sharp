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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace Contoso.Notifications
{
	/// <summary>
	/// Inherits ServiceBase class to implement IdMNotificationSvc
	/// to run as a windows service. This service performs notification
	/// functionality for the scenarios Account Provisioning, Account Expiry,
	/// Password Expiry, Group Membership and Workflow Provisionig.
	/// </summary>
	public class IdMNotificationSvc : System.ServiceProcess.ServiceBase
	{
		// Declarations
		public static string eventSource = "IdMNotificationSvc";
		protected Thread				m_thread;
		protected ManualResetEvent		m_shutdownEvent;
		protected TimeSpan				m_delay;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Calls the InitializeComponent required by the 
		/// Component Designer and sets the Time span for the same.
		/// </summary>
		/// <returns></returns>
		public IdMNotificationSvc()
		{
			// This call is required by the Windows.Forms 
			// Component Designer.
			InitializeComponent();
			m_delay = new TimeSpan(0, 0, 0, 10, 0 );
		}

		/// <summary>
		/// Forms the main Entry point for the process.
		/// And loads the service into the memory to start 
		/// running the service.
		/// </summary>
		/// <returns></returns>
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
	
			// More than one user Service may run within the same process.
			//
			// Note:
			// To add another service to this process, change the following 
			// line to create a second service object. For example,
			//
			// ServicesToRun = new System.ServiceProcess.ServiceBase[] 
			// {new Service1(), new MySecondUserService()};
			//
			ServicesToRun = 
				new System.ServiceProcess.ServiceBase[] 
					{ 
						new IdMNotificationSvc() 
					};

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary> 
		/// Most required method for Designer support.
		/// Gets or sets the short name used to identify the
		/// service to the system.
		/// </summary>
		/// <returns></returns>
		private void InitializeComponent()
		{
			// 
			// IdMNotificationSvc
			// 
			this.ServiceName = "IdMNotificationSvc";

		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		/// <param name="isDisposing">isDisposing</param>
		/// <returns></returns>		 
		protected override void Dispose( bool isDisposing )
		{
			if( isDisposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( isDisposing );
		}

		/// <summary>
		/// Sets things in motion so that the
		/// service can do its work.
		/// </summary>
		/// <param name="args">string args</param>	
		/// <returns></returns>	
		protected override void OnStart(string[] args)
		{
			// create the threadstart object to wrap the delegate method
			ThreadStart threadStart = new ThreadStart( this.ServiceMain );

			// create the manual reset event and
			// set it to an initial state of unsignaled
			m_shutdownEvent = new ManualResetEvent(false);

			// create the worker thread
			m_thread = new Thread( threadStart );

			// go ahead and start the worker thread
			m_thread.Start();

			// call the base class so it has a chance
			// to perform any work it needs to
			base.OnStart( args );
		}
 
		/// <summary>
		/// Denotes the operations at service stop
		/// </summary>
		/// <returns></returns>
		protected override void OnStop()
		{
			// signal the event to shutdown
			m_shutdownEvent.Set();

			// wait for the thread to stop giving it 10 seconds
			m_thread.Join(10000);

			// call the base class 
			base.OnStop();
		}

		/// <summary>
		/// Calls the respective notification methods
		/// of accountProvision, contractorWorkflow, group, 
		/// accountExpirations and passwordExpirations to notify 
		/// user by sending Email.
		/// </summary>
		/// <returns></returns>
		protected void ServiceMain() 
		{
			bool isSignaled = false;
			AccountProvisioning accountProvision = 
									new AccountProvisioning();
			ContractorWorkflow contractorWorkflow = 
									new ContractorWorkflow();
			miisGroupManagement group = 
									new miisGroupManagement();
			AccountExpirations accountExpirations = 
									new AccountExpirations();
			PasswordExpirations passwordExpirations = 
									new PasswordExpirations();
            
			while( true ) 
			{
				// wait for the event to be signaled
				// or for the configured delay
				isSignaled = m_shutdownEvent.WaitOne( m_delay, true );

				// if we were signaled to shutdown, exit the loop
				if( isSignaled == true )
					break;

				// Read account creation messages from MSMQ (sent by MIIS MV 
				// extension), and send e-mail notifications with a delay of
				// X hours (allowing account and mailbox creation to replicate
				// in AD and Exchange). Using MSMQ allows for delaying 
				// notifications as well as creates a loose and async coupling
				// between MIIS provisioning and the notification process
				accountProvision.Notify();

				// Read account creation messages from MSMQ (sent by 
				// Conttractor Request application), and send e-mail 
				// notifications. Using MSMQ creates a loose and async 
				// coupling between application provisioning and the 
				// notification process
				contractorWorkflow.Notify();

				// Read group membership change messages from MSMQ (sent
				// by the group populator), and send e-mail notifications.
				group.Notify();
				
				// Search AD for accounts that have the accountExpires 
				// property set to a date within X days and send notification
				accountExpirations.Notify();

				// Read the Extranet AD, look for passwordLastSet property
				// on the account and PwdMaxAge on the domain. Send 
				// notifications X days before password expires
				passwordExpirations.Notify();

			}
		}
	}
}
