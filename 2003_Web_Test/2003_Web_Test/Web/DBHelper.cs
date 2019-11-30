using System;

using System.Web;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Web.Caching ;

namespace localhost
{
	public class DBHelper
	{

		private static SqlConnection conn;
		private static Hashtable cmds = new Hashtable();
		private static Hashtable sqlDependencies = new Hashtable();
		private static string connectionString = string.Empty;

		public static string GetSqlConnectionString()
		{
			if (connectionString.Length == 0)
			{
				connectionString = System.Configuration.ConfigurationSettings.AppSettings["SQLDependencyTestConnectionString"].ToString();
			}
			return connectionString;
		}



		public static SqlConnection GetSqlConnection()
		{
			string connectString = GetSqlConnectionString();
			if (conn == null)
			{
				conn = new SqlConnection(connectString);
			}
			return conn;
		}

		public static SqlCommand GetSqlCommand(string sql)
		{
			return new SqlCommand(sql, GetSqlConnection());
		}



	}
}
