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
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.Interop.Security.AzRoles;
using System.Security.Principal;
using System.Configuration;

namespace MIISWorkflow
{
	/// <summary>
	/// Describes Contractor Approval page, accessible only to the users in
	/// ContractApprover's group.
	/// </summary>
	public class ContractorApproval : System.Web.UI.Page
	{

		protected System.Web.UI.WebControls.DataGrid dgContractorsGrid;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.WebControls.Label lblHeading;
		protected System.Web.UI.WebControls.Label lblTextWelcome;
		protected System.Web.UI.WebControls.Label lblIDLabel;
		protected System.Web.UI.WebControls.Label lblTextResult;
		protected System.Web.UI.WebControls.Label lblStatus;
		protected System.Web.UI.WebControls.Button btnSubmit;

		string userName = string.Empty;

		/// <summary>
		/// Page load event for the ContractorApproval page. 
		/// This event calls the "LoadDataGridAndUpdateControls"
		/// method for loading the contractors data in the 
		/// data grid on page load.
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event argument</param>
		/// <returns></returns>
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Check for user authentication
			if( User.Identity.IsAuthenticated )
			{
				// Check whether the user who logged in is auntheticated.
				// If yes, then get the user name of the User and display
				// it in the label assigned to it. Otherwise send a
				// response to the user asking to enable Windows aunthentication in IIS.
				userName = User.Identity.Name.ToLower();
				lblIDLabel.Text = userName;

				if( IsPostBack == false )
				{
					// It's the first time the user enters this page.
					LoadDataGridAndUpdateControls();
				}
			}
			else
			{
				// Display the message for enabling windows authentication
				// to load the web page
				Response.Write(Workflow.IIS_ERROR);
				Response.End();
			}
		}

		/// <summary>
		/// This method loads the data grid with the contractor
		/// details for approval.
		/// </summary>
		/// <returns>void</returns>
		private void LoadDataGridAndUpdateControls()
		{
			//
			// Load the data grid on the page with the contractors 
			// data.Calls the "GetContractorsForApproval" method 
			// for getting the details of the contractors for approval.
			//
			dgContractorsGrid.DataSource = 
				Workflow.GetContractorsForApproval( userName );
			
			// Bind the Data grid with the contractor details.
			dgContractorsGrid.DataBind();

			//
			// These controls are assigned to false and should be 
			// enabled only if we pass the checks below.
			//
			dgContractorsGrid.Visible = false;
			btnSubmit.Visible    = false;

			if( Workflow.lastError != null )
			{
				//
				// There is a problem. Possibly a database 
				// connectivity issue.
				//
				lblStatus.Text = "<b>ERROR:</b>" + Workflow.lastError;
			}
			else
				if( dgContractorsGrid.Items.Count == 0 )
			{
				//
				// No approval requests are found for this user 
				// in the database.
				//
				lblStatus.Text = "There are no pending approvals " +
									"awaiting you at this time.";	
			}
			else
			{
				// There are pending approvals for this username. 
				// List them all.
				lblStatus.Text = "Here is the status of all approval " +
					"requests submitted to you. Check the ones you'd " +
					"like to approve and leave the ones you'd like to "+
					"deny empty.";
				dgContractorsGrid.Visible = true;
				btnSubmit.Visible = true;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnSubmit.Click += new System.EventHandler(this.ButtonSubmit_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// This event is fired upon pressing the "Submit"
		/// button.
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Argument</param>
		/// <returns>void</returns>
		private void ButtonSubmit_Click( 
						object sender, System.EventArgs e )
		{
			string associateID = string.Empty;
			int approveCount   = 0;
			int denyCount      = 0;

			//
			// perform accesscheck against authorization manager
			//
			AzAuthorizationStore store = 
						new AzAuthorizationStoreClass();
			try
			{
				store.Initialize(0, @Workflow.policyURL, null);

				IAzApplication app = store.OpenApplication(
					Workflow.STR_CONTRACTOR_REQUEST, null);
				//
				// Get the ID of the currect user, who logged into the application.
				//
				WindowsIdentity id = (WindowsIdentity)User.Identity;

				//
				// Get the token for the current user.
				//
				IntPtr htoken = id.Token;

				//
				// Get the client context with the token of the current user.
				//
				IAzClientContext ctx =
					app.InitializeClientContextFromToken((UInt64)htoken, null);

				//
				// Approve Contractor Request operation in AzMan Policy
				// and set the object for set of specified operations.
				//
				object[] operations = new Object[1]; 

				// Approve Contractor Request operation in AzMan Policy
				operations[0] = 1;  
				
				//
				// The AccessCheck method determines whether the current client 
				// context is allowed to perform the specified operations.
				//
				object[] results = (object[]) ctx.AccessCheck(
					Workflow.APPROVE_ACCOUNT_REQUEST, 
					null, // no scopes in this application
					operations, 
					null,
					null,
					null,
					null,
					null
					); 

				bool bAuthorized = true; 
				//
				// If each value in the results, not equal to zero,
				// then the authorization is set to false.
				//
				foreach (int iResCode in results) 
				{ 
					if ( iResCode != 0 ) // zero = no error 
					{ 
						bAuthorized = false; 
						break; 
					} 
				}

				if(!bAuthorized)
				{
					// User does not have the permissions to approve 
					// acount requests
					lblTextResult.Text = "You do not have the rights to " +
						"				approve or deny account requests.";
					lblTextResult.Text += " Please contact your system " +
								"administrator if you are a valid approver.";
					return;
				}
			}
			catch (Exception exception)
			{
				lblTextResult.Text = "<b>Error: </b>" + 
					"An error occured in Authorization policy store -"
					+ exception.Message;
			}
			

			//
			// Enumerate each contractor on the page and approve or 
			// deny them based on the user's selection.
			//
			for( int i = 0; i < dgContractorsGrid.Items.Count; i++ )
			{
				associateID = dgContractorsGrid.Items[i].Cells[0].Text;

				RadioButtonList approvalResult = 
					(RadioButtonList)dgContractorsGrid.
						Items[i].Cells[7].FindControl( "ApprovalResult" );

				if( approvalResult.SelectedValue == "Approve" )
				{
					Workflow.ApproveContractor( associateID );

					if( Workflow.lastError != null )
						break;
					else
						approveCount++;
				}
				else if( approvalResult.SelectedValue == "Deny" )
				{
					Workflow.DenyContractor( associateID );

					if( Workflow.lastError != null )
						break;
					else
						denyCount++;
				}
			}

			if( Workflow.lastError != null )
			{
				//
				// There is a reported problem. Bail with the error. 
				// If there were any approvals done until we hit this 
				// error, the WorkflowMA will not be executed below. 
				// That's expected. The objects will wait in "Approved"
				// state until this issue is resolved.
				//
				lblTextResult.Text = "<b>ERROR:</b>" + Workflow.lastError;
			}
			else
			{
				lblTextResult.Text = string.Empty;

				if( approveCount > 0 )
				{
					//
					// There are some approvals. Run the WorkflowMA and 
					// report the approvals count to the user.
					//
					bool runResult = Workflow.RunWorkflowMA( );

					if( runResult == true )
					{
						lblTextResult.Text = 
							"Provisioned <b>" + approveCount.ToString() + 
													"</b> contractor(s). ";
					}
					else
					{
						lblTextResult.Text = "<b>Error:</b> Failed to import " +
							"approvals to MIIS. Please verify MIIS is up and running.";
					}
				}

				if( denyCount > 0 )
				{
					//
					// There are some denials. Denials do not effect the 
					// CS object state. We do not need to run the 
					// WorkflowMA. Just report the count to the user.
					//
					lblTextResult.Text += "Denied <b>" + 
						denyCount.ToString() + "</b> contractor(s).";
				}

				if( approveCount == 0 && denyCount == 0 )
				{
					// Nothing is approved or denied if the user 
					// simply clicked the Submit button.
					lblTextResult.Text += "No contractors are selected.";
				}

				//
				// Refresh the Contractors Grid on the page to
				// reflect the latest changes.
				//
				LoadDataGridAndUpdateControls();
			}
		}
	}
}
