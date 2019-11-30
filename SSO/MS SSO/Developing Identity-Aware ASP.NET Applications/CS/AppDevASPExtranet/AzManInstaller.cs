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
using System.Configuration;
using System.Configuration.Install;
using System.Runtime.InteropServices;
using Microsoft.Interop.Security.AzRoles;
using Contoso.Identity.Template;

namespace Contoso.AppDevASPSample
{
	/// <summary>
	/// Creates an AZMan policy file in C:\AppDevASPExtranet.xml for
	/// the Extranet sample. The file AzMan store, application,
	/// operations, role definitions and roles that the application 
	/// will use. 
	/// This could be set up using the AzMan UI, but only the
	/// developer knows what the application is doing and what 
	/// operations, tasks etc that it contains. Therefore the
	/// developer should supply an installer for the AzMan policy
	/// </summary>
	[RunInstaller(true)]
	public class AzManInstaller : Installer
	{
		private const String AZ_POLICY_URL = @"msxml://C:\AppDevASPExtranet.xml";
		private const String APPLICATION_NAME = "AppDevASPExtranet";
		private const int READ_EMPLOYEE_OPERATION_ID = 61;
		private const int WRITE_EMPLOYEE_OPERATION_ID = 62;

		//Overrides the Install event and initializes AzMan stores
		//roles, operations etc.
		public override void Install(IDictionary savedState)
		{
			base.Install(savedState);

			AzAuthorizationStoreClass store = CreateStore();
			IAzApplication app = CreateApplication(store);
			CreateOperations(app);
			CreateTasks(app);
			CreateRoleDefinitions(app);
			CreateRoles(app);
		}

		//Create the AzMan store
		private AzAuthorizationStoreClass CreateStore()
		{
			AzAuthorizationStoreClass store = new AzAuthorizationStoreClass();
			//Create a new store for app if it doesn't exist
			try
			{
				store.Initialize(AzMan.AZ_AZSTORE_FLAG_ACCESS_CHECK, AZ_POLICY_URL, null);
			}
			catch
			{
				store.Initialize(AzMan.AZ_AZSTORE_FLAG_CREATE,
					AZ_POLICY_URL,
					null);
				store.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, null);
			}
			return store;
		}

		//Create the AzMan application
		private IAzApplication CreateApplication(AzAuthorizationStoreClass store)
		{

			//Create app in store if it doesn't exist
			IAzApplication app = null;
			try
			{
				app = store.OpenApplication(APPLICATION_NAME, null);
			}
			catch
			{
				app = store.CreateApplication(APPLICATION_NAME, null);
				app.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, null);
			}
			return app;
		}

		//Create the AzMan operations
		private void CreateOperations(IAzApplication app)
		{
			CreateOperation(app, "ReadEmployeeData", READ_EMPLOYEE_OPERATION_ID);
			CreateOperation(app, "WriteEmployeeData", WRITE_EMPLOYEE_OPERATION_ID);
		}

		//Create a single AzMan operation
		private void CreateOperation(IAzApplication app, String operationName, Int32 operationId)
		{
			IAzOperation op = null;
			try
			{
				op = app.CreateOperation(operationName, null);
				op.OperationID = operationId;
				op.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, null);
			}
			catch
			{
				//TODO: Do Catch Implementation
			}
		}

		//Create the AzMan tasks
		private void CreateTasks(IAzApplication app)
		{
			CreateTask(app, "View employees", new String[] {"ReadEmployeeData"});
			CreateTask(app, "Manage employees", new String[] {"ReadEmployeeData", "WriteEmployeeData"});
		}

		//Create a single AzMan task
		private void CreateTask(IAzApplication app, String taskName, String[] operationNames)
		{
			IAzTask task = null;
			try
			{
				task = app.CreateTask(taskName, null);
				foreach(String operationName in operationNames)
				{
					task.AddOperation(operationName, null);
				}
				task.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, null);
			}
			catch
			{
				//TODO: Do Catch Implementation
			}
		}

		//Create the AzMan role definitions
		private void CreateRoleDefinitions(IAzApplication app)
		{
			CreateRoleDefinition(app, "Employee user", new String[] {"View employees"});
			CreateRoleDefinition(app, "Employee manager", new String[] {"Manage employees"});
		}

		//Create a single AzMan role definition
		private void CreateRoleDefinition(IAzApplication app, String roleName, String[] taskNames)
		{
			IAzTask task = null;
			try
			{
				task = app.CreateTask(roleName, null);
				task.IsRoleDefinition = Convert.ToInt32(true);
				foreach(String taskName in taskNames)
				{
					task.AddTask(taskName, null);
				}
				task.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, null);
			}
			catch
			{
				//TODO: Do Catch Implementation
			}
		}
	
		//Create the AzMan roles
		private void CreateRoles(IAzApplication app)
		{
			IAzScope scope = app.CreateScope("All", null);
			scope.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, null);

			CreateRole(scope, "Users", new String[] {"Employee user"}, null);
			CreateRole(scope, "Managers", new String[] {"Employee manager"}, null);
		}

		//Create a single AzMan role
		private void CreateRole(IAzScope scope, String roleName, String[] taskNames, String[] userNames)
		{
			IAzRole role = null;
			try
			{
				role = scope.CreateRole(roleName, null);
				foreach(String taskName in taskNames)
				{
					role.AddTask(taskName, null);
				}
				role.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, null);

				foreach(String userName in userNames)
				{
					role.AddMemberName(userName, null);
				}
				role.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, null);
			}
			catch
			{
				//TODO: Do Catch Implementation
			}
		}

	}
}
