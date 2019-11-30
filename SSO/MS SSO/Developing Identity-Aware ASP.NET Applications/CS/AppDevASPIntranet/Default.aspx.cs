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
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using Contoso.Web.UI.Localization;

namespace Contoso.AppDevASPSample
{
	/// <summary>
	/// The default page of the intranet application. The page is made localizable by
	/// deriving from LocalizedPage
	/// </summary>
	public class _Default : LocalizedPage
	{
		protected System.Web.UI.WebControls.DataGrid EmployeeGrid;
		protected System.Web.UI.WebControls.Label lblPageHeader;
		protected System.Web.UI.WebControls.Label lblError;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			this.LocalizeControls();
			EmployeeGrid.Columns[0].HeaderText = this.GetResourceString("strColumnHeaderFirstName");
			EmployeeGrid.Columns[1].HeaderText = this.GetResourceString("strColumnHeaderLastName");
			EmployeeGrid.Columns[2].HeaderText = this.GetResourceString("strColumnHeaderTitle");

			//Get and display employee data
			Employee emp = new Employee();
			EmployeeGrid.DataSource = emp.GetEmployees();
			EmployeeGrid.DataBind();
			if (emp.Error != null)
				if(emp.Error.StartsWith("msg"))
                    lblError.Text = this.GetResourceString(emp.Error);
				else
					lblError.Text = emp.Error;
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
