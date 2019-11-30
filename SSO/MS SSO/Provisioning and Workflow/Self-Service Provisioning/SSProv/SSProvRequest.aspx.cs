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
using System.Globalization;
using System.IO;
using System.Configuration;

namespace MIISWorkflow
{
	/// <summary>
	/// Implements code behind the contractor request process.
	/// Authorization based on role is required to access this page. 
	/// </summary>
	public class ContractorRequest : System.Web.UI.Page
	{
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.WebControls.Label lblHeading;
		protected System.Web.UI.WebControls.Label lblTextWelcome;
		protected System.Web.UI.WebControls.Label lblUserID;
		protected System.Web.UI.WebControls.Label lblTextEnterNewInfo;
		protected System.Web.UI.WebControls.TextBox txtFirstName;
		protected System.Web.UI.WebControls.TextBox txtLastName;
		protected System.Web.UI.WebControls.TextBox txtStartDate;
		protected System.Web.UI.WebControls.TextBox txtEndDate;
		protected System.Web.UI.WebControls.Button btnStartDate;
		protected System.Web.UI.WebControls.Button btnEndDate;
		protected System.Web.UI.WebControls.Calendar cldrStartDate;
		protected System.Web.UI.WebControls.Calendar cldrEndDate;
		protected System.Web.UI.WebControls.Button btnSubmit;
		protected System.Web.UI.WebControls.Label lblTextResult;
		protected System.Web.UI.WebControls.HyperLink apvlnkContractorApproval;
		
		// Global declaration
		private int contractPeriod = 0;

		/// <summary>
		/// Initial operation of page load based on the authentication.
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		/// <returns>void</returns>
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Check whether the user who logged in is auntheticated.
			// If yes, then get the user name of the User and display
			// it. Otherwise send a response to the user asking to enable
			// Windows aunthentication in IIS.
			if( User.Identity.IsAuthenticated )
			{
				string userName = User.Identity.Name.ToLower();
				lblUserID.Text = userName;
			}
			else
			{
				Response.Write( Workflow.IIS_ERROR );
				Response.End();
			}

			// Hide the calendar controls on the page load
			cldrStartDate.Visible = false;
			cldrEndDate.Visible = false;

			// Check if the user has access to approve any contractor request.
			if( !CanApprove())
			{
				// If not, then make the link for the approval page invisible.
				apvlnkContractorApproval.Visible = false;
			}
			
			try
			{
				string strContractorPeriod = ConfigurationSettings.
											AppSettings["contractPeriod"];
				contractPeriod = Convert.ToInt32(strContractorPeriod);
			}
			catch(Exception)
			{
				// Error in getting the default configuration for 
				// contract period. Set value to 90 days
				contractPeriod = 90;
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
			this.btnStartDate.Click += new System.EventHandler(this.btnStartDate_Click);
			this.btnEndDate.Click += new System.EventHandler(this.btnEndDate_Click);
			this.cldrStartDate.SelectionChanged += new System.EventHandler(this.cldrStartDate_SelectionChanged);
			this.cldrEndDate.SelectionChanged += new System.EventHandler(this.cldrEndDate_SelectionChanged);
			this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Event handler for the submit button click. Validates the
		/// details of the contractor and inserts into the database.
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		/// <returns>void</returns>
		private void btnSubmit_Click(object sender, System.EventArgs e)
		{
			// Create an object for the entity of the contractor
			// to get all the details from the UI.
			
			UserIdentity user = new UserIdentity();
			
			// Check for null entries
			if(txtFirstName.Text.Trim() =="" || txtLastName.Text.Trim() ==""||
				txtStartDate.Text.Trim() =="" || txtEndDate.Text.Trim() =="")
			{
				lblTextResult.Text = "<b>ERROR: </b>" +
									"All the fields are required.";
				return;
			}	
			// Assign the corresponding details from the UI
			// to the attributes of the entity.
			user.FIRST_NAME = txtFirstName.Text.Trim();
			user.LAST_NAME  = txtLastName.Text.Trim();
			try
			{
				DateTime startDateTime = DateTime.Parse(txtStartDate.Text.Trim());
				DateTime endDateTime = DateTime.Parse(txtEndDate.Text.Trim());
				
				// Don't allow a request for a start date in the past
				if(startDateTime.Date.CompareTo(DateTime.Now.Date)<0)
				{
					lblTextResult.Text = "<b>ERROR:</b> Start date cannot be " 
						+"earlier than today's date";
					return;					
				}
				//Check the validity of start and end dates
				if(endDateTime.Date.CompareTo(startDateTime)>=0)
				{
					user.START_DATE = startDateTime.ToString(
						"d", DateTimeFormatInfo.InvariantInfo);
					user.END_DATE = endDateTime.ToString(
						"d", DateTimeFormatInfo.InvariantInfo);
				}
				else
				{
					lblTextResult.Text = "<b>ERROR:</b> End date cannot be " 
										+"earlier than start date";
					return;
				}				
			}
			catch(Exception exception)
			{
				lblTextResult.Text = "<b>ERROR:</b> Invalid date format - "
									+ exception.Message;
				return;
			}

			// Validate the input data against the empty string and
			// invalid characters.
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
			if( Workflow.ValidateDateStringData( user.START_DATE ) == false )
			{
				lblTextResult.Text = "<b>ERROR:</b> Invalid START DATE.";
				return;
			}
			if( Workflow.ValidateDateStringData( user.END_DATE ) == false )
			{
				lblTextResult.Text = "<b>ERROR:</b> Invalid END DATE.";
				return;
			}
			
			// Go ahead and create this contractor in the database. 
			// This task is done through a method in the Workflow
			// called InsertUserIdentity which will take the object 
			// of the entity and name of the requester to return the 
			// ID of the new contractor.

			string contractorID = 
					Workflow.InsertUserIdentity( 
							user, User.Identity.Name.ToLower(
							System.Globalization.CultureInfo.InvariantCulture));

			if( Workflow.lastError != null )
			{
				// There is a problem. Possibly a database 
				// connectivity issue.
				lblTextResult.Text = "<b>ERROR:</b>" + Workflow.lastError;
			}
			else
			{
				// If the returned contractor ID is null,
				// then the error is displayed.
				if( contractorID == null )
				{
					lblTextResult.Text = 
						"<b>ERROR:</b> Failed to add the contractor.";
				}
				else
				{
					// Otherwise, the message about the success in 
					// the insertion of the message should be displayed.
					string fullName = user.FIRST_NAME + " " + user.LAST_NAME;

					lblTextResult.Text = "Successfully added <b>" + 
						fullName + "</b> with the assigned contractor ID <b>"
						+ contractorID + "</b>. Click on Contractor Status "
						+ "link above to follow the approval status.";

					// Clean the Edit Boxes on the page.
					txtFirstName.Text = string.Empty;
					txtLastName.Text  = string.Empty;
				}
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
			
			// Initialize the store with the configuration XML.
			try
			{
				store.Initialize(0, @Workflow.policyURL, null);

				// In that XML, open the contractor request application,
				// and assign it to an object of the IAzApplication
				app = store.OpenApplication(Workflow.STR_CONTRACTOR_REQUEST, null);
			}			
			catch (Exception exception)
			{
				lblTextResult.Text = "<b>Error: </b>" + 
							"An error occured in Authorization policy store - "+
										exception.Message;
			}

			// Get the ID of the currect user, who logged 
			// into the application.
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
				null, // no scopes in this application
				operations, 
				null,
				null,
				null,
				null,
				null
				); 

			bool bAuthorized = true; 
			// If any one value in the results, not equal to zero,
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

		/// <summary>
		/// Event handler for the btnStartDate button click.
		/// Displays the calendar to select start date
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		/// <returns>void</returns>
		private void btnStartDate_Click(object sender, System.EventArgs e)
		{
			cldrStartDate.Visible = true;
		}

		/// <summary>
		/// Event handler for the btnEndDate button click.
		/// Displays the calendar to select end date
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		/// <returns>void</returns>
		private void btnEndDate_Click(object sender, System.EventArgs e)
		{
			cldrEndDate.Visible = true;
		}

		/// <summary>
		/// Event handler for the start date calendar selection.
		/// Inserts the selected date into the start date text field.
		/// Also calculates the end date for the default contract period
		/// and inserts it to the end date text field.
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		/// <returns>void</returns>
		private void cldrStartDate_SelectionChanged(
						object sender, System.EventArgs e )
		{
			txtStartDate.Text = 
				cldrStartDate.SelectedDate.ToShortDateString();
			cldrStartDate.Visible = false;
			
			if(txtStartDate.Text != "")
			{
				DateTime myDateTime = DateTime.Parse
					(txtStartDate.Text,DateTimeFormatInfo.InvariantInfo);
				// From the selected start date, as a default value,
				// 90 days should be added to the end date.
				myDateTime = myDateTime.AddDays(contractPeriod);
				txtEndDate.Text = myDateTime.ToShortDateString();
			}
		}

		/// <summary>
		/// Event handler for the end date calendar selection.
		/// Inserts the selected date into the end date text field.
		/// Also validates the end date not to be less than the start date.
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		/// <returns>void</returns>
		private void cldrEndDate_SelectionChanged( 
						object sender, System.EventArgs e )
		{
				txtEndDate.Text = 
					cldrEndDate.SelectedDate.ToShortDateString();
				cldrEndDate.Visible = false;
		}
	}
}
