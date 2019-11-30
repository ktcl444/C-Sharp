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


namespace MIISWorkflow
{
	/// <summary>
	/// Describes the Contractor Update page where the details of contractor
	/// can be edited and updated.
	/// </summary>
	public class ContractorUpdate : System.Web.UI.Page
	{
		//Web controls declaration
		//protected System.Web.UI.WebControls.HyperLink HyperLink1;
		protected System.Web.UI.WebControls.HyperLink HyperLink2;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.WebControls.Label lblHeading;
		protected System.Web.UI.WebControls.Label lblTextWelcome;
		protected System.Web.UI.WebControls.Label lblUserID;
		protected System.Web.UI.WebControls.Label lblTextEnterNewInfo;
		protected System.Web.UI.WebControls.Label lblContractorID;
		protected System.Web.UI.WebControls.TextBox txtFirstName;
		protected System.Web.UI.WebControls.TextBox txtLastName;
		protected System.Web.UI.WebControls.TextBox txtDepartment;
		protected System.Web.UI.WebControls.Button btnSubmit;
		protected System.Web.UI.WebControls.Label lblTextResult;
		protected System.Web.UI.WebControls.HyperLink apvlnkContractorApproval;

		string userName = string.Empty;

		/// <summary>
		/// Initial Page Load function. Name of the logged in user
		/// and the existing values of the contractor is displayed
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		/// <returns>void</returns>
		private void Page_Load(object sender, System.EventArgs e)
		{
			if( !IsPostBack )
			{
				// Get the ID of the user, if the user is valid.
				if( User.Identity.IsAuthenticated )
				{
					userName = User.Identity.Name.ToLower();
					lblUserID.Text = userName;

					// Check if the user has access to approve any contractor request.
					if( !CanApprove())
					{
						// If not, then make the link for the approval page invisible.
						apvlnkContractorApproval.Visible = false;
					}

					// Get the ID of the contractor from the current session.
					string currentContractorID = 
						(string)Session[ "contractorID" ];

					if( currentContractorID == null )
					{
						// The user tries to access this web page directly.
						// In this case, there will be no contractor ID
						// to process. This is an error. The user should
						// use the links from other pages instead.
						lblTextResult.Text = 
							"<b>Error:</b> Accessing this page directly is not allowed.";
						Form1.Visible = false;
					}
					else
					{
						// We have a contractorID at hand. 
						// So go ahead and display the update menu.
						lblContractorID.Text = currentContractorID;

						// This GetUserIdentity is a method in the Workflow,
						// it will get the details of any given contractor ID,
						// to display them in the update menu.
						UserIdentity contractorInfo = 
							Workflow.GetUserIdentity( currentContractorID );

						if( Workflow.lastError != null )
						{
							// There is a problem. Possibly a database connectivity issue.
							lblTextResult.Text = "<b>ERROR:</b>" + Workflow.lastError;
							Form1.Visible = false;
						}
						else
						{
							// If the details obtained for a specific contractor ID
							// is not null, then display them.
							if( contractorInfo != null )
							{
								txtFirstName.Text  = contractorInfo.FIRST_NAME;
								txtLastName.Text   = contractorInfo.LAST_NAME;
								txtDepartment.Text = contractorInfo.DEPARTMENT;
							}
						}
					}
				}
				else
				{
					Response.Write( Workflow.IIS_ERROR );
					Response.End();
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
			this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Handler code for the click of the submit button.
		/// Updates the edited values of contractor to the database.
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		/// <returns>void</returns>
		private void btnSubmit_Click(object sender, System.EventArgs e)
		{
			// Instantiate an object for the user entity.
			UserIdentity user = new UserIdentity();

			// Validate the edited data
			if(txtFirstName.Text ==""||txtLastName.Text ==""||txtDepartment.Text =="")
			{
				lblTextResult.Text = "<b>ERROR:</b> Field's can not be empty";
				return;
			}

			//
			// Assigns values to the attributes of the user entity.
			//
			user.CONTRACTOR_ID = lblContractorID.Text.Trim();
			user.FIRST_NAME    = txtFirstName.Text.Trim();
			user.LAST_NAME     = txtLastName.Text.Trim();
			user.DEPARTMENT    = txtDepartment.Text.Trim();

			//
			// Validate the input data against the empty string and
			// invalid characters.
			//
			if( Workflow.ValidateStringData( user.FIRST_NAME ) == false )
			{
				lblTextResult.Text = "<b>ERROR:</b> Invalid FIRST NAME.";
				return;
			}

			if( Workflow.ValidateStringData( user.LAST_NAME ) == false )
			{
				lblTextResult.Text = "<b>ERROR:</b> Invalid LAST NAME.";
				return;
			}

			if( Workflow.ValidateStringData( user.DEPARTMENT ) == false )
			{
				lblTextResult.Text = "<b>ERROR:</b> Invalid DEPARTMENT.";
				return;
			}

			// It is now safe to update this contractor with the new information.
			string contractorID = 
				Workflow.UpdateUserIdentity( user, User.Identity.Name.ToLower() );

			if( Workflow.lastError != null )
			{
				// There is a problem. Possibly a database connectivity issue.
				lblTextResult.Text = "<b>ERROR:</b>" + Workflow.lastError;
			}
			else
			if( contractorID == null )
			{
				lblTextResult.Text = 
					"<b>ERROR:</b> Failed to update the contractor.";
			}
			else
			{
				string fullName = user.FIRST_NAME + " " + user.LAST_NAME;

				lblTextResult.Text = "Successfully updated <b>" 
					+ fullName + "</b> with the contractor ID <b>" 
					+ contractorID + "</b>. Click on Contractor Status" 
					+ " link above to follow the approval status.";
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
