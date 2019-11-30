using System;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace AppLauncherDataCS
{
	/// <summary>
	/// Summary description for AppUserRoles.
	/// </summary>
	public class AppUserRoles
	{

    string mConnectString;

		public AppUserRoles()
		{
      mConnectString = ConfigurationSettings.
        AppSettings["eSecurityConnectString"];
		}

    public int GetLoginKey(string loginID, int appKey)
    {
      DataSet ds = new DataSet();
      SqlCommand cmd;
      SqlDataAdapter da;
      string sql;
      int retValue;

      sql = "SELECT esUsers.iUserID ";
      sql += " FROM esAppsUsers ";
      sql += " INNER JOIN esUsers ";
      sql += " ON esUsers.iUserID = esAppsUsers.iUserID ";
      sql += " WHERE esUsers.sLoginID = @sLoginID ";
      sql += " AND esAppsUsers.iAppID = @iAppID ";

      try
      {
        cmd = new SqlCommand(sql);
        cmd.Connection = new SqlConnection(mConnectString);
        
        cmd.Parameters.Add(new SqlParameter("@sLoginID", SqlDbType.Char));
        cmd.Parameters["@sLoginID"].Value = loginID;

        cmd.Parameters.Add(new SqlParameter("@iAppID", SqlDbType.Int));
        cmd.Parameters["@iAppID"].Value = appKey;

        da = new SqlDataAdapter(cmd);
        da.Fill(ds);

        retValue = 
         Convert.ToInt32(ds.Tables[0].Rows[0]["iUserID"]);
      }
      catch (Exception ex)
      {
        throw ex;
      }

      return retValue;
    }

    public bool IsLoginValid(string loginID, int appKey)
    {
      DataSet ds = new DataSet();
      SqlCommand cmd;
      SqlDataAdapter da;
      string sql;
      bool retValue = false;

      sql = "SELECT Count(*) As TotalRows ";
      sql += " FROM esAppsUsers ";
      sql += " INNER JOIN esUsers ";
      sql += " ON esUsers.iUserID = esAppsUsers.iUserID ";
      sql += " WHERE esUsers.sLoginID = @sLoginID ";
      sql += " AND esAppsUsers.iAppID = @iAppID ";

      try
      {
        cmd = new SqlCommand(sql);
        cmd.Connection = new SqlConnection(mConnectString);
        
        cmd.Parameters.Add(new SqlParameter("@sLoginID", SqlDbType.Char));
        cmd.Parameters["@sLoginID"].Value = loginID;

        cmd.Parameters.Add(new SqlParameter("@iAppID", SqlDbType.Int));
        cmd.Parameters["@iAppID"].Value = appKey;

        da = new SqlDataAdapter(cmd);
        da.Fill(ds);

        if (Convert.ToInt32(
          ds.Tables[0].Rows[0]["TotalRows"]) > 0)
        {
          retValue = true;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }

      return retValue;
    }

    public string[] GetUserRoles(int appID, int userID)
    {
      DataSet ds = new DataSet();
      SqlCommand cmd;
      SqlDataAdapter da;
      string sql;
      string[] aRoles = new string[0];
      int i = 0;

      sql = "SELECT sRole ";
      sql += " FROM esAppUserRoles ";
      sql += " INNER JOIN esAppRoles ";
      sql += " ON esAppUserRoles.iAppRoleID = esAppRoles.iAppRoleID ";
      sql += " WHERE esAppUserRoles.iAppID = @iAppID ";
      sql += " AND esAppUserRoles.iUserID = @iUserID ";

      try
      {
        cmd = new SqlCommand(sql);
        cmd.Connection = new SqlConnection(mConnectString);
        
        cmd.Parameters.Add(new SqlParameter("@iAppID", SqlDbType.Int));
        cmd.Parameters["@iAppID"].Value = appID;

        cmd.Parameters.Add(new SqlParameter("@iUserID", SqlDbType.Int));
        cmd.Parameters["@iUserID"].Value = userID;

        da = new SqlDataAdapter(cmd);
        da.Fill(ds);

        if (ds.Tables[0].Rows.Count > 0)
        {
          aRoles = new string[ds.Tables[0].Rows.Count];
          foreach(DataRow dr in ds.Tables[0].Rows)
          {
            aRoles[i] = dr["sRole"].ToString();
            i++;
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }

      return aRoles;
    }
	}
}
