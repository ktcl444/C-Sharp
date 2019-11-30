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

namespace Contoso.Identity.Template
{
	/// <summary>
	/// This is a simple class to wrap data access provided to minimize
	/// external dependencies. We recommend you use the DataAccess 
	/// Application Block (DAAB) from Patterns & Practices that can be
	/// found at:
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnbda/html/daab-rm.asp
	/// </summary>
	public class SqlData
	{
		private String _error = null;

		/// <summary>
		/// Public default constructor of the SqlData helper class
		/// </summary>
		public SqlData()
		{
			_error = null;
		}

		/// <summary>
		/// Returns the last error message from a database operation
		/// </summary>
		public String Error
		{
			get {return _error;}
		}

		/// <summary>
		/// Connects to the database, executes the specified command and
		/// returns a dataset. The DAAB includes more variants on this
		/// and implements them in a more robust way
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public DataSet GetDataset(String command)
		{
			SqlConnection cn = new SqlConnection();

			try
			{
				String cnString = ConfigurationSettings.AppSettings["SQLConnectionString"];

				cn.ConnectionString = cnString;
				cn.Open();

				SqlCommand cmd = cn.CreateCommand();
				cmd.CommandText = command;

				SqlDataAdapter a = new SqlDataAdapter();
				a.SelectCommand = cmd;
				
				DataSet ds = new DataSet();
				a.Fill(ds);
				return ds;
				
			}
			catch (Exception ex)
			{
				_error = ex.Message;
				return null;
			}
			finally
			{
				if (cn.State == ConnectionState.Open)
					cn.Close();
			}

		}

	}
}
