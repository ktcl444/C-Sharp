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
using System.Configuration;
using Contoso.Identity.Template;

namespace Contoso.AppDevASPSample
{
	/// <summary>
	/// The Employee class is a sample business object that manages
	/// employee info
	/// </summary>
	public class Employee
	{

		private const int READ_EMPLOYEE_OPERATION_ID = 61;
		private const int WRITE_EMPLOYEE_OPERATION_ID = 62;

		private String _error = null;

		/// <summary>
		/// Returns the last error message from the employee business
		/// object
		/// </summary>
		public String Error
		{
			get {return _error;}
		}

		/// <summary>
		/// Authorize against role and get a list of employees
		/// </summary>
		/// <returns>A dataset containing Contoso employees</returns>
		public DataSet GetEmployees()
		{
			//Do access AzMan check and get data from back-end
			DataSet employeeDS = null;
			AzMan authManager = new AzMan();
			if (authManager.IsAllowedAccess(this.GetType().Name,"All", READ_EMPLOYEE_OPERATION_ID))
			{
				SqlData employeeData = new SqlData();
				employeeDS = employeeData.GetDataset("SELECT Firstname, Lastname, Title FROM Employees");
				if (employeeData.Error != null)
					_error = employeeData.Error;
			}
			else
			{
				_error = "msgAccessDenied";
			}
			return employeeDS;

		}
	}
}
