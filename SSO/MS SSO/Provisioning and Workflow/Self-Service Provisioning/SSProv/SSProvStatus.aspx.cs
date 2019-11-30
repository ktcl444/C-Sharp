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
using System.IO;
using System.Configuration;

namespace MIISWorkflow
{
	/// <summary>
	/// Describes the ContractorStatus page to display the 
	/// contractor status details in a data grid.
	/// </summary>
	public class ContractorStatus : System.Web.UI.Page
	{		
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.WebControls.Label lblHeading;
		protected System.Web.UI.WebControls.Label lblTextWelcome;
		protected System.Web.UI.WebControls.Label lblUserID;
		protected System.Web.UI.WebControls.Label lblStatus;
		protected System.Web.UI.WebControls.DataGrid dgContractorsGrid;
		protected System.Web.UI.WebControls.Label lblTextResult;
		protected System.Web.UI.WebControls.HyperLink apvlnkSSProvApproval;
			
		string userName = string.Empty;

		/// <summary>
		/// Initial page load function, displays the name of the user who 
		/// logged in. Also, loads the datagrid and controls, with the details
		/// of the contractor status appropriately.
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		/// <returns>void</returns>
		private void Page_Load(object sender, System.EventArgs e)
		{
			// If the user is authenticated, then display his user name.
			if( User.Identity.IsAuthenticated )
			{
				userName = User.Identity.Name.ToLower();
				lblUserID.Text = userName;

				if( IsPostBack == false )
				{
					// It's the first time the user enters this page.
					LoadDataGridAndUpdateControls();
				}
			}
			else
			{
				Response.Write( Workflow.IIS_ERROR );
				Response.End();
			}
		}

		/// <summary>
		/// Loads the datagrid and other controls; displays
		/// the details and status of all the contractors based 
		/// on the user who logs in.
		/// </summary>
		/// <returns>void</returns>
		private void LoadDataGridAndUpdateControls()
		{
			if( !CanApprove())
			{
				// Hide the link to Contractor Approval if 
				// the user can not approve
				apvlnkSSProvApproval.Visible = false;
			}

			try
			{
				// Load the data grid on the page with the contractors data.
				dgContractorsGrid.DataSource = 
					Workflow.GetRequestedContractors( userName );
				dgContractorsGrid.DataBind();
			}
			catch (Exception exception )
			{
				lblStatus.Text = "<b>Error :</b>" + exception.Message;
			}

			// This control is assigned to false and should be
			// enabled only if we pass the checks below.
			dgContractorsGrid.Visible = false;

			if( Workflow.lastError != null )
			{
				// There is a problem. Possibly a database connectivity issue.
				lblStatus.Text = "<b>ERROR:</b>" + Workflow.lastError;
			}
			else
			{
				if( dgContractorsGrid.Items.Count == 0 )
				{
					lblStatus.Text = "You have no requested contractors.";
				}
				else
				{
					// There are contractors requested. List them all.
					lblStatus.Text = 
						"Here is the status of all contractors requested by you :";
					dgContractorsGrid.Visible = true;
				}
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
			this.dgContractorsGrid.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgContractorsGrid_ItemCreated);
			this.dgContractorsGrid.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgContractorsGrid_ItemCommand);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Event Handler triggered if any event happens
		/// inside the datagrid for the events in the datagrid,
		/// ie. Edit, View History or Terminate
		/// the corresponding actions are defined.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void dgContractorsGrid_ItemCommand(
					object source, DataGridCommandEventArgs e )
		{
			// Contractor status and ID from the datagrid, 
			// is assigned to a string.
			string contractorID     = e.Item.Cells[0].Text;
			string contractorStatus = e.Item.Cells[4].Text;

			// Switch case is used to consider the three possible events
			// of the datagrid and they are Edit, View History or Terminate
			switch( e.CommandName )
			{
				// The event for the click of the Edit button in the
				// datagrid, is handled.
				case "Edit" :
			
					// If the status of the user is one among
					// "Approved", "Waiting for approval" or "Terminated"
					// then editing is not possible
					if( contractorStatus == "Approved" ||
						contractorStatus == "Waiting for approval" ||
						contractorStatus == "Terminated" )
					{
					
						// Editing these contractors is not allowed.
						lblTextResult.Text = 
							"This contractor's status is set to <b>" 
							  + contractorStatus + 
							"</b>.  Editing is not allowed at this state.";
					}
					else
					{
						// Otherwise, redirect the response to SSProvUPdate page
						Session.Add( "contractorID", contractorID );
						Response.Redirect( "SSProvUpdate.aspx" );
					}
					break;

				// The event for the click of the Terminate cell in 
				// the datagrid, is handled.
				case "Terminate":
					//
					// If the status of the user is one among "Approved"
					// or "Terminated" then termination is not possible
					//
					if( contractorStatus == "Approved" ||
						contractorStatus == "Terminated" )
					{
						lblTextResult.Text = 
							"Termination operation is not applicable " +
							 "to a contractor with status set to <b>"
							    + contractorStatus + "</b>.";
					}
					else
					{
						// Terminate the contractor. If the contractor 
						// is terminated successfully, run the WorkflowMA. 
						
						if( Workflow.TerminateContractor( contractorID ) )
						{
							// Termination successful on the database.
							bool runResult = Workflow.RunWorkflowMA();

							if( runResult == true )
							{
								// Refresh the Contractors Grid on the page
								// to reflect the latest changes.
								LoadDataGridAndUpdateControls();
	
								// If the variable called lastError of the Workflow
								// is not equal to null, then display the error.
								if( Workflow.lastError != null )
								{
									lblTextResult.Text = "<b>ERROR:</b>" + 
															Workflow.lastError;
								}
								else
								{
									dgContractorsGrid.DataBind();
									lblTextResult.Text = 
										"Successfully terminated the contractor <b>"
										+ contractorID + "</b>.";
								}
							}
							else
							{
								// If the variable called lastError of the 
								// Workflow is not equal to null, then display the error.
								if( Workflow.lastError != null )
								{
									lblTextResult.Text = "<b>ERROR:</b>" + 
																Workflow.lastError;
								}
								else
									lblTextResult.Text = 
									"<b>ERROR:</b> Failed to terminate the contractor";
							}
						}
						else
						{
							// Termination failed. Check if it's a 
							// database connectivity issue.
							if( Workflow.lastError != null )
							{
								lblTextResult.Text = "<b>ERROR:</b>" + 
															Workflow.lastError;
							}
							else
							{
								lblTextResult.Text = 
										"Failed to terminate this contractor.";
							}
						}
					}
					break;

				// The event for the click of the View History 
				// button in the datagrid, is handled.
				case "ViewHistory":
					//
					// Add the contractor ID into a session and redirect
					// flow control to ContractorHistory page.
					//
					Session.Add( "contractorID", contractorID );
					Response.Redirect( "SSProvHistory.aspx" );
					break;
			}
		}

		/// <summary>
		/// Event Handler fired when Terminate action is performed.
		/// </summary>
		/// <param name="source">Source Object</param>
		/// <param name="e">Event Arguments</param>
		/// <returns>void</returns>
		private void dgContractorsGrid_ItemCreated
						(object source, DataGridItemEventArgs e )
		{
			// Add client side scripting behind each Terminate 
			// linkbutton on the page. The client script pops up
			// the dialog that is used in confirming whether
			// the user really wants to terminate the contractor.
			LinkButton terminateBtn = 
				(LinkButton)e.Item.Cells[8].FindControl( "Terminate" );

			if( terminateBtn != null )
			{
				terminateBtn.Attributes.Add(
						"onclick", "return confirm_delete();" );
			}
		}
		/// <summary>
		/// Checks whether the logged in user has the right to approve
		/// the request
		/// </summary>
		/// <returns>Returns true if the user has approval right</returns>
		private bool CanApprove()
		{
			// Instantiate the class called AzAuthorizationStoreClass
			AzAuthorizationStore store = new AzAuthorizationStoreClass();
			IAzApplication app = null;
			
			//
			// Initialize the store with the configuration XML.
			//
			try
			{
				store.Initialize(0, @Workflow.policyURL, null);
				
				// In that XML, open the contractor request application,
				// and assign it to an object of the IAzApplication
				app = store.OpenApplication 
								(Workflow.STR_CONTRACTOR_REQUEST, null);
			}			
			catch (Exception exception)
			{
				lblTextResult.Text = "<b>Error: </b>" + exception.Message;
			}			
			
			// Get the ID of the currect user, 
			// who logged into the application.
			WindowsIdentity id = (WindowsIdentity)User.Identity;

			// Get the token for the current user.
			IntPtr htoken = id.Token;

			// Get the client context with the token of the current user.
			IAzClientContext ctx =
				app.InitializeClientContextFromToken((UInt64)htoken, null);
            
			// Approve Contractor Request operation in AzMan Policy
			// and set the object for set of specified operations.
			object[] operations = new Object[1]; 
			operations[0] = 1; 
						
			// The AccessCheck method determines whether the 
			// current client context is allowed to perform 
			// the specified operations.
			object[] results = (object[]) ctx.AccessCheck(
								Workflow.APPROVE_ACCOUNT_REQUEST, 
				//scope,
				null, // no scopes in this application
				operations, 
				null,
				null,
				null,
				null,
				null
				); 

			bool bAuthorized = true; 

			// If each value in the results, not equal to zero,
			// then the authorization is set to false.
			foreach (int iResCode in results) 
			{ 
				if ( iResCode != 0 ) // zero = no error 
				{ 
					bAuthorized = false; 
					break; 
				} 
			}
			return bAuthorized;
		}
	}
}
