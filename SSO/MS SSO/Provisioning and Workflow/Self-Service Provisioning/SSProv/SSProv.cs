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
using System.Data;
using System.Data.SqlClient;
using System.Management;
using Microsoft.Interop.Security.AzRoles;
using System.DirectoryServices;
using System.Messaging;
using System.Configuration;
using System.Xml;
using System.IO;

namespace MIISWorkflow
{
	/// <summary>
	/// Represents the entity for a Contractor with the required
	/// attributes like contractor ID, first name, last name, department, 
	/// manager who raises a request, start date and end date.
	/// </summary>
	public class UserIdentity
	{
		public string CONTRACTOR_ID = string.Empty;
		public string FIRST_NAME = string.Empty;
		public string LAST_NAME = string.Empty;
		public string DEPARTMENT = string.Empty;
		public string REQUESTER = string.Empty;
		public string START_DATE = string.Empty;
		public string END_DATE = string.Empty;

	}

	/// <summary>
	/// This class contains all the related methods that are being called at
	/// appropriate places for the work flow sequence across the application.
	/// </summary>
	public class Workflow
	{
		// Global declarations
		public const string IIS_ERROR = "<b>Error:</b> Please enable Windows"
								+ "authentication on IIS in order to load " + 
											"this web page.";
		public const string STR_CONTRACTOR_REQUEST = "Contractor Request";
		public const string APPROVE_ACCOUNT_REQUEST = 
										"Approve Account Request";
		const string CONTRACTOR_APPROVER = "Contractor Approver";
			
		// The methods in this class use lastError string as 
		// a storage for the last catastrophic error.
		public static string lastError = string.Empty;

		// Get the connection string from configuration file
		// that is used in connecting to SQLServer.
		private static string mySqlConnectionString = 
			ConfigurationSettings.AppSettings["mySqlConnectionString"];
 
		//Get the queue name from the configuration file
		private static string queueName = ConfigurationSettings.AppSettings
			["contractorQueueName"];

		//Get the Authorization store policy URL from the configuration file
		public static string policyURL = 
			ConfigurationSettings.AppSettings["policyURL"];
				
	/// <summary>
	/// Validate the input data for empty string and 
	/// invalid characters. 
	/// </summary>
	/// <param name="input"></param>		
	/// <returns>Returns true if the string is valid</returns>
	public static bool ValidateStringData( string input )
		{
			lastError = null;

			// Empty string is considered to be invalid
			
			if( input.Trim() == "" )
			{
				return false;
			}
			else
			{				
				// This string shows the set of invalid characters and is used
				// when validating the string inputs on the webpages. The goal
				// is to have a simple defense implementation against SQL 
				// injection.
				
				string InvalidChars = "~!@#$%^&*()<>-=_+,.?/\\:;\"'`[]|";

				//Return true or false based on the validation
				if( input.IndexOfAny( InvalidChars.ToCharArray() ) > -1 )
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}   

		/// <summary>
		/// Validate the input date string data for empty string and 
		/// invalid characters. 
		/// </summary>
		/// <param name="input"></param>		
		/// <returns>Returns true if the date string is valid</returns>
		public static bool ValidateDateStringData( string input )
		{
			lastError = null;

			// Empty string is considered to be invalid
			
			if( input.Trim() == "" )
			{
				return false;
			}
			else
			{				
				// This string shows the set of invalid characters and is used
				// when validating the string inputs on the webpages. The goal
				// is to have a simple defense implementation against SQL 
				// injection.
				
				string InvalidChars = "~!@#$%^&*()<>-=_+,.?\\:;\"'`[]|";

				//Return true or false based on the validation
				if( input.IndexOfAny( InvalidChars.ToCharArray() ) > -1 )
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}   

		/// <summary>
		/// Get the SQL connection for the database. 
		/// </summary>
		/// <returns>SQL connection</returns>
		private static SqlConnection GetSqlConnection()
		{
			lastError = null;			
			
			
			try
			{
				// Instantiate the SQL Connection
				SqlConnection wfConnection = 
					new SqlConnection(mySqlConnectionString);
				
				// Open the SQL Connection
				wfConnection.Open();									
			
				// return the connection object
				if( wfConnection.State == ConnectionState.Open)
				{
					return wfConnection;
				}
				else
				{
					return null;
				}
			}	
			catch(SqlException sqlException)
			{
				// A connection level exception has occured
				lastError = "Failed to open a connection to the database. "+
								sqlException.Message;
				return null;

			}
		}

		/// <summary>
		/// Insert the contractor details into the database
		/// with values for all the attributes. 
		/// </summary>
		/// <param name="user">UserIdentity object with all the 
		/// attribute details</param>
		/// <param name="requester">Name of the manager who
		/// raised the request</param> 
		/// <returns>Returns the ID of the contractor created</returns>
		public static string InsertUserIdentity( 
				UserIdentity user, string requester )
		{
			lastError = null;

			// Get the SQL connection to the database by calling the 
			// "GetSqlConnection" method
			
			SqlConnection wfConnection = GetSqlConnection();

			// If connection is null, return null.
			if( wfConnection == null )
				return null;

			// Instantiate the Sql Command 
			SqlCommand wfCommand = new SqlCommand();

			// Get the SQl Connection
			wfCommand.Connection = wfConnection;

			// If the contractor has no ContractorID, it will
			// be automatically assigned an ID.First ContractorID 
			// will start with C100000 and will increase by 1 as 
			// new contractors are added.
			
			string contractorID = "";
			int rowsAffected = 0;
			SqlDataReader dataReader = null;
			
			try
			{
				if( user.CONTRACTOR_ID == "" )
				{
					// A query to assign the max of existing contractor
					// IDs to the new contractor.
					wfCommand.CommandText = 
						"SELECT 'contractorID'= " +
						"MAX(CONTRACTOR_ID) FROM CONTRACTORS";

					// Execute the Data Reader for reading each 
					// row from the database 
					dataReader = wfCommand.ExecuteReader(); 

					// Read the record from the Data Reader
					dataReader.Read();

					// If the data returned contains null value,
					// set the contractor ID to C100000
					if( !dataReader.IsDBNull(0) )
					{
						// Get the new Contractor ID
						contractorID = dataReader.GetString(0);
						contractorID = contractorID.Substring(1);
						int ID = Int32.Parse(contractorID) + 1; 
						contractorID = "C" + ID.ToString();
					}
					else 
					{
						// Set the new Contractor ID
						contractorID = "C100000";
					}
					dataReader.Close();
					// Get the Contractor ID into entity
					user.CONTRACTOR_ID = contractorID;
				}
				
				// Instantiate array of strings to store User details
				string[] attributes = new string[4];

				// Get more details for the contractor request
				attributes = 
					GetAdditionalAttributesFromUserName( requester );

				// Frame sql command for inserting Contractor details 
				// into the Contractor table.
				wfCommand.CommandText = 
					"INSERT INTO CONTRACTORS VALUES(" +
					"'" + user.CONTRACTOR_ID + "'," +
					"'" + user.FIRST_NAME + "'," +
					"'" + user.LAST_NAME + "'," +
					"'" + attributes[3] + "'," +
					"'" + user.START_DATE + "'," +
					"'" + user.END_DATE + "'," +
					"'" + attributes[0] + "'," +
					"'" + attributes[1] + "'," +
					"'" + attributes[2] + "'," +
					"'" + requester + "'," +
					"CURRENT_TIMESTAMP," +
					"'Waiting for approval'," +
					"0," +
					"1" +
					")";


				// Execute the Sql Query and return the 
				// number of rows affected.
				rowsAffected = wfCommand.ExecuteNonQuery(); 
				
			}
			catch( Exception e)
			{
				// Display the error message.
				lastError = "An error occured in Workflow.InsertUserIdentity() " 
					+ e.Message;
				return null;
			}
			finally
			{
				try
				{
					wfConnection.Close();
				}
				catch(SqlException sqlException)
				{
					lastError = sqlException.Message;
				}
			}
			if( rowsAffected > 0 )
			{
				// At this stage, send an e-mail to ALL the 
				// potential approvers for this contractor.
							
				// open the authorization store
				AzAuthorizationStore store = 
					new AzAuthorizationStoreClass();
				try
				{
					store.Initialize(0, @Workflow.policyURL, null);

					// open Contractor Request application in the store
					IAzApplication app = store.OpenApplication(
						Workflow.STR_CONTRACTOR_REQUEST, null);

					// Get the Contractor Approval role
					IAzRole ARRoleApprover = 
						app.OpenRole(CONTRACTOR_APPROVER, null);
					
					// Gets the Names of the contractor approvers
					Object[] Members = 
						(Object[])ARRoleApprover.MembersName;
					
					// Sends a notification e-mail to each member of 
					// approver's role
					foreach (String str in Members) 
					{			
						SendNotification("ContractorRequest", str, 
							user.CONTRACTOR_ID, "");
					}

				}
				catch(Exception exception)
				{
					lastError = "<b>Error: </b>" + 
					"An error occured in Authorization policy store - "+
					exception.Message;
				}
			}			
			// return the Contractor ID
			return user.CONTRACTOR_ID;
		}
	
		/// <summary>
		/// Called to get the details of the existing contractor 
		/// from the database based on the Contractor ID.
		/// </summary>
		/// <param name="contractorID">
		/// The ID for which the details are to be obtained
		/// </param>
		/// <returns>UserIdentity object for the requested contractor ID</returns>
		public static UserIdentity GetUserIdentity( 
									string contractorID )
		{
			UserIdentity user = null;
			lastError = null;
			SqlDataReader dataReader = null;

			// Get the SQL connection to the database
			// by calling the "GetSqlConnection" method
			SqlConnection wfConnection = GetSqlConnection();

			// Check for the Sql Connection
			if( wfConnection == null )
				return null;

			try
			{
				
				// Execute a sql command for getting 
				// contractor details from the database

				SqlCommand wfCommand = new SqlCommand(
					"SELECT CONTRACTOR_ID,FIRST_NAME,LAST_NAME,DEPARTMENT,"
					+ "START_DATE,END_DATE,"
					+ "REQUESTER FROM CONTRACTORS WHERE CONTRACTOR_ID='"
					+ contractorID + "'",wfConnection );

				// Execute the Data Reader
				dataReader = wfCommand.ExecuteReader( );

				if( dataReader.Read() )
				{
					// Create a User Identity object
					user = new UserIdentity();

					// Get the Values from the Data Reader
					user.CONTRACTOR_ID = dataReader.GetString(0);
					user.FIRST_NAME    = dataReader.GetString(1);
					user.LAST_NAME     = dataReader.GetString(2);
					user.DEPARTMENT    = dataReader.GetString(3);
					user.START_DATE    = dataReader.GetString(4);
					user.END_DATE      = dataReader.GetString(5);					user.REQUESTER     = dataReader.GetString(6);
				}
				dataReader.Close();
			}			
			catch( Exception exception)
			{
				lastError = "A error occured in Workflow.GetUserIdentity "
					+exception.Message;
			}
			//Ensure that database connection and DataReader objects are closed
			finally
			{
				try
				{
					wfConnection.Close();
				}
				catch (SqlException e)
				{
					lastError = e.Message;
				}
			}

			return user;
		}			
			
		/// <summary>
		/// Updates the attributes of the contractor, with
		/// the given new set of input values.
		/// </summary>
		/// <param name="user">Details of the contractor whose details
		/// are to  be updated.</param>
		/// <param name="requester">Name of the requester.</param>
		/// <returns>Contractor ID or null</returns>
		public static string UpdateUserIdentity( 
				UserIdentity user, string requester )
		{
			lastError = null;

			// Get the SQL connection to the database
			// by calling the "GetSqlConnection" method
			SqlConnection wfConnection = GetSqlConnection();

			// If the connection is null, 
			// then return null.
			if( wfConnection == null )
				return null;

			// Create sql command for updating
			// the Contractor table and execute it.
			
			SqlCommand wfCommand = new SqlCommand( 
				"UPDATE CONTRACTORS SET " +
				"FIRST_NAME = '" + user.FIRST_NAME  + "'," +
				"LAST_NAME = '"  + user.LAST_NAME   + "'," +
				"DEPARTMENT = '" + user.DEPARTMENT  + "'," +
				"REQUESTER = '"  + requester + "'," +
				"LAST_MODIFIED = CURRENT_TIMESTAMP,"+
				"STATUS = 'Waiting for approval' "  +
				"WHERE CONTRACTOR_ID = '" + user.CONTRACTOR_ID + "'",
				wfConnection );

			try
			{
				// Execute the Sql Query and return the 
				// number of rows affected.
				int rowsAffected = wfCommand.ExecuteNonQuery();

				if( rowsAffected > 0 )
				{
					// At this stage, send an e-mail to ALL the potential
					// approvers for this contractor.
							
					// open the authorization store
					AzAuthorizationStore store = 
						new AzAuthorizationStoreClass();
			
					store.Initialize(0, @Workflow.policyURL, null);

					// open Contractor Request application in the store
					IAzApplication app = store.OpenApplication(
						Workflow.STR_CONTRACTOR_REQUEST, null);

					// Get the Contractor Approval role
					IAzRole ARRoleApprover = 
							app.OpenRole(CONTRACTOR_APPROVER, null);
					
					// Gets the Names of the contractor approvers
					Object[] Members = 
							(Object[])ARRoleApprover.MembersName;
					
					// Sends a notification e-mail to each member of 
					// approver's role
					foreach (String str in Members) 
					{			
						SendNotification("ContractorRequest", str, 
							user.CONTRACTOR_ID, "");
					}
				}
				else
				{
					return null;
				}
			}
			catch( Exception e )
			{
				// Display the error message.
				lastError = "An error occured in Workflow.UpdateUserIdentity() " + 
					e.Message;
			}
				//Close the database connection before the method ends
			finally
			{
				try
				{
					wfConnection.Close();
				}
				catch(SqlException e)
				{
					lastError = e.Message;
				}
			}
				//Return the contractor ID
			return user.CONTRACTOR_ID;
		}

		/// <summary>
		/// Returns the approver's alias from the user name
		/// </summary>
		/// <param name="username"></param>
		/// <returns>string: approver alias</returns>
		public static string MailFromUserName( string username )
		{
			// Declaration
			string anrName = string.Empty;
			string target = "\\";
			string strApproverAlias = string.Empty;

			try
			{
				// If the user value is equal to null 
				// then return null.
				if(username.Equals(""))
					return("");			

				char[] anyOf = target.ToCharArray();

				// requester names need to be stripped of domain prefix
				if(username.IndexOfAny(anyOf, 0) != -1)
				{
					string[] parts = username.Split(new char[]{'\\'});
					anrName = parts[1];
				}
				else 
				{
					// Set the anrName to the User name
					anrName = username;
				}
			
				// Searching in the Active Directory
				DirectorySearcher searcher = new DirectorySearcher();

				// try to find a user or group object in GC that 
				// corresponds to user name
				searcher.Filter = 
					"(&(anr=" + anrName + ")(objectClass=*))";

				// Getting the first Search result
				SearchResult searchResult = searcher.FindOne();

				// Getting all the directory entries corresponding
				// to the search.
				DirectoryEntry approverDirectoryEntry = 
								searchResult.GetDirectoryEntry();

				// get the alias of the object
				strApproverAlias = 
					approverDirectoryEntry.Properties["mail"].Value.ToString();
			}			
			catch(Exception exception)
			{
				lastError = "An error occured in Workflow.MailFromUserName() "+
					"for user: "+username+ " with anrName:"+anrName+
					exception.Message;
			}
			
			// Return the Approvers alias
			return strApproverAlias;
		}
		
		/// <summary>
		/// Called to obtain the additional
		/// attributes for the given user name.
		/// </summary>
		/// <param name="username">Name of the user</param>
		/// <returns>attributes of the user name</returns>
		public static string[] GetAdditionalAttributesFromUserName( 
			string username )
		{
			string[] attributes = new string[4];
			string anrName = string.Empty;
			string target = "\\";

			try
			{
				// If the user name equals to null then set
				// all the attributes of the user to null.
				if(username.Equals(""))
				{
					attributes[0] = "";
					attributes[1] = "";
					attributes[2] = "";
					attributes[3] = "";
					return attributes;
				}

				char[] anyOf = target.ToCharArray();

				// requester names need to be stripped of domain prefix
				if(username.IndexOfAny(anyOf, 0) != -1)
				{
					string[] parts = username.Split(new char[]{'\\'});
					anrName = parts[1];
				}
				else 
				{
					// Set the anrName to the User name
					anrName = username;
				}
			
				// Seaching for details in the active directory
				DirectorySearcher searcher = new DirectorySearcher();

				// try to find a user or group object in GC that 
				// corresponds to user name
				searcher.Filter = "(&(anr=" + anrName + ")(objectClass=*))";

				// Getting the first Search result
				SearchResult searchResult = searcher.FindOne();

				// Getting all the directory entries corresponding
				// to the search.
				DirectoryEntry oneDirectoryEntry = 
						searchResult.GetDirectoryEntry();

				// Get the attributes like Employee ID,Company
				// name and Department for the User
				attributes[0] = 
					oneDirectoryEntry.Properties["l"].Value.ToString();
				attributes[1] = 
					oneDirectoryEntry.Properties["employeeID"].Value.ToString();
				attributes[2] = 
					oneDirectoryEntry.Properties["company"].Value.ToString();
				attributes[3] = 
					oneDirectoryEntry.Properties["department"].Value.ToString();
			}
			catch( Exception exception )
			{
				lastError = exception.Message;
			}

			// Return the User Attributes 
			return attributes;			
		}
		
		/// <summary>
		/// This function is used to get all the
		/// contractors those need to be approved.
		/// </summary>
		/// <param name="approverName">Name of the approver</param>
		/// <returns>Returns the requests pending under the approverName</returns>
		public static DataView GetContractorsForApproval( 
			string approverName )
		{
			lastError = null;
			DataView source = null;
						
			// Get the SQL connection to the database
			// by calling the "GetSqlConnection" method
			SqlConnection wfConnection = GetSqlConnection();
			
			// If the connection is null, then return null.
			if( wfConnection == null )
				return null;
			try
			{

				// Check if there are pending contractor requests first.
				SqlCommand wfCommand = new SqlCommand( 
					"select count(*) from CONTRACTORS where STATUS" +
					"='Waiting for approval'", wfConnection );

				// Execute the query and returns the first 
				// row in the result set
				int entryCount = (int)wfCommand.ExecuteScalar();

				if( entryCount > 0 )
				{
					// There are entries waiting for approval.
			
					//Retrieve the entries waiting for approval
                    SqlDataAdapter wfAdapter = new SqlDataAdapter(
						"SELECT CONTRACTOR_ID,FIRST_NAME,LAST_NAME," + 
						"DEPARTMENT,REQUESTER,LAST_MODIFIED,STATUS FROM " +
						"CONTRACTORS WHERE STATUS='Waiting for approval'",
						wfConnection );

					// Instantiate the Data set
					DataSet ds = new DataSet();

					// Fill the Data Adapter with Data set
					wfAdapter.Fill(ds);

					// Instantiate the Data View
					source = new DataView(ds.Tables[0]);

				}
				else
				{
					// No approval requests at this time.
					return null;
				}
			}
			catch( SqlException e )
			{
				// Display the error message
				lastError ="An error occured in Workflow.GetContractorsForApproval() "+
					e.Message;
			}
			finally
			{
				//Close the sql connection
				try
				{
					wfConnection.Close();
				}
				catch(SqlException sqlException)
				{
					lastError = sqlException.Message;
				}
			}

			// Return the DataView object
			return source;
		}

		/// <summary>
		/// Gets the list of contactor requests raised by any given requester.
		/// </summary>
		/// <param name="requesterName">Name of the requester</param>
		/// <returns>Returns the requests under the requesterName</returns>
		public static DataView GetRequestedContractors( 
			string requesterName )
		{
			lastError = null;
            DataView source = null;

			// Get the SQL connection to the database
			// by calling the "GetSqlConnection" method
			SqlConnection wfConnection = GetSqlConnection();

			if( wfConnection == null )
				return null;

			try
			{
				
				// Getting the Contractor details from the database
				// based on the requester.
				SqlDataAdapter wfAdapter = new SqlDataAdapter(
					"SELECT CONTRACTOR_ID,FIRST_NAME,LAST_NAME," +
					"DEPARTMENT,LAST_MODIFIED,STATUS FROM CONTRACTORS " 
					+"WHERE REQUESTER='" + requesterName + "'",
					wfConnection );

				// Instantiate the Data Set
				DataSet ds = new DataSet();

				// Fill the Data Adapter with the Data Set
				wfAdapter.Fill(ds);

				// Instantiate the Data View
				source = new DataView(ds.Tables[0]);

			}
			catch( Exception exception )
			{
				// Display the error message
				lastError = "An error in retrieving requested contractors - "
							+ exception.Message;
			}
			finally
			{
				// Close the Sql Conection
				try
				{
					wfConnection.Close();
				}
				catch(SqlException sqlException)
				{
					lastError = sqlException.Message;
				}
			}

			// Return the data view
			return source;
		}

		/// <summary>
		/// Gets the history of a contractor
		/// </summary>
		/// <param name="contractorID"> Contractor ID</param>
		/// <returns>Returns the contractor history details 
		/// from the database</returns>
		public static DataView GetHistoryForContractor( 
			string contractorID )
		{
			lastError = null;
			DataView source = null;

			// Get the SQL connection to the database
			// by calling the "GetSqlConnection" method
			SqlConnection wfConnection = GetSqlConnection();

			if( wfConnection == null )
				return null;

			try
			{
				
				// Getting the Contractor history from the database
				// corresponding to the particular contractor.
				SqlDataAdapter wfAdapter = new SqlDataAdapter(
					"SELECT FIRST_NAME,LAST_NAME,DEPARTMENT,REQUESTER," +
					"LAST_MODIFIED,STATUS FROM CONTRACTORS_HISTORY WHERE " +
					"CONTRACTOR_ID='" + contractorID + "' order by timestamp",
					wfConnection );

				// Instantiate the Data Set
				DataSet ds = new DataSet();

				// Fill the Data Adapter with the Data Set
				wfAdapter.Fill(ds);

				// Instantiate the Data View
				source = new DataView(ds.Tables[0]);
			
			}
			catch( Exception e)
			{
				lastError = "Error in retrieving contractor history - " 
								+ e.Message;
			}
			finally
			{
				// Close the Sql Connection
				try
				{
					wfConnection.Close();
				}
				catch(SqlException e)
				{
					lastError = e.Message;
				}
			}

			// Return the Source value
			return source;
		}

		/// <summary>
		/// Triggered when the approver approves any contractor request.
		/// </summary>
		/// <param name="ID">ID of the contractor</param>
		/// <returns>Returns true on success</returns>
		public static bool ApproveContractor( string ID )
		{
			lastError = null;

			// Get the SQL connection to the database
			// by calling the "GetSqlConnection" method
			SqlConnection wfConnection = GetSqlConnection();
				
			if( wfConnection == null )
				return false;

			try
			{
				// Generate a password for the contractor to be approved.
				string password = GeneratePassword();

				// Execute the APPROVE_CONTRACTOR.
				SqlCommand wfCommand = new SqlCommand( 
					"EXEC APPROVE_CONTRACTOR " + ID + "," + @password,
					wfConnection );

				// Execute the Query
				wfCommand.ExecuteNonQuery();

				// At this stage, send an e-mail to the original requester
				// stating that the new contractor is approved.
				SendNotification("ContractorApproved", "", ID, password);				
			}			
			catch(Exception e)
			{
				lastError = "An exception has occured in Contractor Approval - "
								+ e.Message;
				return false;
			}
			finally
			{
				// Close the Sql Connection
				try
				{
					wfConnection.Close();
				}
				catch(SqlException e)
				{
					lastError = e.Message;
				}
			}
	
			return true;
		}

		/// <summary>
		/// Triggered when the approver denies the request
		/// </summary>
		/// <param name="ID">ID of the contractor</param>
		/// <returns>Returns true upon success</returns>
		public static bool DenyContractor( string ID )
		{
			// Declaration
			lastError = null;

			// Get the SQL connection to the database
			// by calling the "GetSqlConnection" method
			SqlConnection wfConnection = GetSqlConnection();

			if( wfConnection == null )
				return false;

			try
			{
			
				// Execute the stored procedure DENY_CONTRACTOR
				SqlCommand wfCommand = new SqlCommand( 
					"EXEC DENY_CONTRACTOR " + ID,
					wfConnection );

				// Execute the Query
				wfCommand.ExecuteNonQuery();

				// At this stage, send an e-mail to the original requester 
				// stating that the new contractor is denied.
				SendNotification("ContractorDenied", "", ID, "");
			}
			catch( SqlException sqlException )
			{
				// Display the error message
				lastError = "A error has occured in deny contractor - "
							+ sqlException.Message; 
			}
			// To ensure that connection is closed before the method exit
			finally
			{
				// Close the SQL connection
				try
				{
					wfConnection.Close();
				}
				catch(SqlException sqlException)
				{
					lastError = sqlException.Message;
				}
			}
			return true;
		}

		/// <summary>
		/// To run the WorkFlow Management Agent to update
		/// the contractor provisioning\deprovisioning changes in the 
		/// metaverse and connected directory.
		/// </summary>
		/// <returns>Returns true upon successfull Run</returns>
		public static bool RunWorkflowMA( )
		{
			lastError = null;

			// Set the values for maName,runProfile and
			// runResult
			string runProfile = "Delta Import and Export";
			Object runResult  = null;
			string workflowMA = ConfigurationSettings.AppSettings["workflowMA"];

			// Run the Workflow MA in Delta Import and Export mode.
			
			// Instantiate the Management scope
			ManagementScope myScope = 
				new ManagementScope( 
					"root\\MicrosoftIdentityIntegrationServer" );

			// Instantiate the SQL query to search and run the Workflow MA
			SelectQuery myQuery = 
				new SelectQuery( "MIIS_ManagementAgent", "Name='" +
									workflowMA + "'" );
									
			ManagementObjectSearcher searcher = 
				new ManagementObjectSearcher( myScope, myQuery );            

			foreach( ManagementObject ma in searcher.Get() )
			{
				runResult = ma.InvokeMethod( "Execute", 
								new object[1]{ runProfile } );
			}	
			
			// Export the contractor to Intranet Active Directory
			// Set the values for maName,runProfile and
			// runResult
			string runProfileAD = "Export";
			Object runResultAD  = null;
			string intranetADMA = ConfigurationSettings.AppSettings["intranetADMA"];

			// Run the Intranet Active Directory MA in Export.
			
			// Instantiate the SQL query to search and run the Intranet AD MA
			SelectQuery intranetADQuery = 
				new SelectQuery( "MIIS_ManagementAgent", "Name='" +
				intranetADMA + "'" );
									
			searcher = new ManagementObjectSearcher( myScope, intranetADQuery );            

			foreach( ManagementObject ma in searcher.Get() )
			{
				runResultAD = ma.InvokeMethod( "Execute", 
					new object[1]{ runProfileAD } );
			}	
			
			// Return true if the MA run is successfull
			return( runResult != null && runResultAD != null);
		}

		/// <summary>
		/// To terminate an existing contractor
		/// </summary>
		/// <param name="ID">ID of the contractor.</param>
		/// <returns>Retruns true upon success</returns>
		public static bool TerminateContractor( string ID )
		{			
			lastError = null;
			int rows = 0;

			// Get the SQL connection to the database
			// by calling the "GetSqlConnection" method
			SqlConnection wfConnection = GetSqlConnection();

			if( wfConnection == null )
				return false;

			try
			{
				// Execute the command
				SqlCommand wfCommand = new SqlCommand( 
					"EXEC TERMINATE_CONTRACTOR " + ID,
					wfConnection );

				// Execute the SQL Query
				rows = wfCommand.ExecuteNonQuery();
				
			}
			catch( SqlException sqlException )
			{
				// Display the error message 
				lastError = "A database error has occured in Terminate" +
							" Contractor - " + sqlException.Message;
			}
			finally
			{
				// Close the SQL connection
				try
				{
					wfConnection.Close();
				}
				catch(SqlException sqlException)
				{
					lastError = sqlException.Message;
				}
			}
            
			// Return true if command succeeds
			return( rows > 0 );
		}

		/// <summary>
		/// Send notifications at variuos instances during
		/// of the WorkFlow application flow.
		/// </summary>
		/// <param name="type">Type of the notification</param>
		/// <param name="approver">Name of the approver</param>
		/// <param name="employeeID">ID of the employee</param>
		/// <param name="userPassword">Password of the employee</param>
		/// <returns>Return true upon success</returns>
		public static bool SendNotification( string type, 
			string approver, string employeeID, string userPassword)
		{
			lastError = null;
			
			try
			{
				// If the message queue doesn't exist, create it
				if(!MessageQueue.Exists(queueName))
				{
					MessageQueue.Create(queueName,true);
				}

				// Open the private queue named SelfServiceProvisioning 
				// on the local machine (".")
				MessageQueue queue = new MessageQueue(queueName);

				//Create a new message
				Message msg = new Message();

				// Give a meaningful label so the receiver quickly 
				// can see what it is without reading the body
				msg.Label = type;

				// Write some XML data into the body
				msg.Body = BuildXMLData(approver, employeeID, userPassword);

				// Send the message and tell MSMQ to commit the 
				// transaction (TransactionType = Single message)
				queue.Send(msg, MessageQueueTransactionType.Single);
			}			
			catch(Exception exception)
			{
				lastError = "Notification Error:"
					+"An error in message queue operation - "+exception.Message;
			}
			
			//Return false if any exception has occured
			if(lastError!=null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		///	To parse the XML document to read and write 
		///	the corresponding data. 
		/// </summary>
		/// <param name="approver">approver's ID</param>
		/// <param name="ID">contractor's ID</param>
		/// <param name="userPassword">password of the approver</param>
		/// <returns>Formatted XML data string</returns>
		public static string BuildXMLData( 
			string approver, string ID, string userPassword )
		{

			// Instantiate the String writer
			StringWriter body = new StringWriter();
			try
			{
				lastError = null;
				// Call the method for getting user identity
				UserIdentity user = GetUserIdentity(ID);			

				// Instantiate the XML writer
				XmlTextWriter writer = new XmlTextWriter(body);
			
				// Instantiate the Writer methods
				writer.WriteStartDocument();
				writer.WriteStartElement("account");
				writer.WriteAttributeString("employeeId", ID);
				writer.WriteAttributeString("userPassword", userPassword);
				string managerSmtp = MailFromUserName(user.REQUESTER);
				writer.WriteAttributeString("managerSmtp", managerSmtp);
				string approverSmtp = MailFromUserName(approver);
				writer.WriteAttributeString("approverSmtp", approverSmtp);
				writer.WriteAttributeString("lastName", user.LAST_NAME);
				writer.WriteAttributeString("firstName", user.FIRST_NAME);
				writer.WriteEndElement();
				writer.WriteEndDocument();
			}			
			catch ( Exception exception)
			{
				lastError = "BuildXML Error: " + exception.Message;
			}
			// Return the built string
			return body.ToString();
		}

		/// <summary>
		/// For any approved contractor, ID will be created and a corresponding
		/// password is be automatically generated as a temporary one.
		/// </summary>
		/// <returns>Return the generated password</returns>
		public static string GeneratePassword()
		{
			// Instantiate the Password Generator
			PasswordGenerator passwordGenerator = new PasswordGenerator();

			// Return the value obtained from the Generate method
			return passwordGenerator.Generate();
		}
	}
}

