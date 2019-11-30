using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data.SqlClient;

namespace SqlDependencyTest
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            SqlDependency.Start(DBHelper.GetSqlConnectionString());
            //SqlDependency.Start(DBHelper.GetSqlConnectionString("SQLDependencyTestConnectionString2"));
            CacheHelper.Clear();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception objErr = Server.GetLastError().GetBaseException();
            if (objErr.GetType().Name.Equals("SqlException"))
            {
                SqlException sqlEx = (SqlException)objErr;
                if (sqlEx.Number == 233 && sqlEx.Source == ".Net SqlClient Data Provider")
                { 
                    
                }
            }
          

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            SqlDependency.Stop(DBHelper.GetSqlConnectionString());
            SqlDependency.Stop(DBHelper.GetSqlConnectionString("SQLDependencyTestConnectionString2"));
        }
    }
}