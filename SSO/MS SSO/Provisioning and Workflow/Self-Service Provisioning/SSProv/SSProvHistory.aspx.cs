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

namespace MIISWorkflow
{
	/// <summary>
	/// Describes the ContractorHistory page to display the action history of 
	/// any particular contractor request
	/// </summary>
	public class ContractorHistory : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lblHeading;
		protected System.Web.UI.WebControls.Label lblTextWelcome;
		protected System.Web.UI.WebControls.Label lblUserID;
		protected System.Web.UI.WebControls.Label lblStatus;
		protected System.Web.UI.WebControls.DataGrid dgHistoryGrid;

		// Global Declaration
		string userName = string.Empty;

		/// <summary>
		/// Initial page load function; displays the datagrid
		/// with the history details like department,
		/// date and time of join, requester, status, etc
		/// of the selected contractor.
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		/// <returns>void</returns>
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Get the ID of the user, if the user is valid.
			if( User.Identity.IsAuthenticated )
			{
				userName = User.Identity.Name.ToLower();
				lblUserID.Text = userName;

				// Get the ID of the contractor from the 
				// current session.
				string currentContractorID = 
							(string)Session[ "contractorID" ];

				if( currentContractorID == null )
				{
					// The user tries to access this web page directly.
					// In this case, there will be no contractor ID to
					// process. This is an error. The user should use 
					// the links from other pages instead.
					lblStatus.Text = "<b>Error:</b> Accessing this " +
									"page directly is not allowed.";
					dgHistoryGrid.Visible = false;
				}
				else
				{
					try
					{
						// We have a contractorID at hand. Display the history
						// information for it.
						dgHistoryGrid.DataSource = 
							Workflow.GetHistoryForContractor( 
							currentContractorID );
						dgHistoryGrid.DataBind( );
					}
					catch (Exception exception)
					{
						lblStatus.Text = "Error: " + exception.Message;
					}
					
					// This control is assigned to false and should be
					// enabled only if we pass the checks below.
					dgHistoryGrid.Visible = false;

					if( Workflow.lastError != null )
					{
						// There is a problem. Possibly a database
						// connectivity issue.
						lblStatus.Text = "<b>ERROR:</b>" + Workflow.lastError;
					}
					else
					{
						// If the number of items in the grid is zero, then 
						// error is displayed.
						if( dgHistoryGrid.Items.Count == 0 )
						{
							lblStatus.Text = "There is no history information "
								+ "available for the contractor <b>" + 
								currentContractorID + "</b>.";
						}
						else
						{
							lblStatus.Text = "Here is the history information " 
								+ "for the contractor <b>" + currentContractorID 
								+ "</b> :";
							dgHistoryGrid.Visible = true;
						}
					}
				}
			}
			//Ask the user to enable windows authentication on IIS
			else
			{
				Response.Write(Workflow.IIS_ERROR);
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
