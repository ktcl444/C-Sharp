using System;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace AppLauncherDataCS
{
	/// <summary>
	/// Summary description for Apps.
	/// </summary>
	public class Apps
	{
    string mConnectString;

		public Apps()
		{
      mConnectString = ConfigurationSettings.
        AppSettings["eSecurityConnectString"];
		}

    public string CreateLoginToken(string appName, 
      string loginID, int userID, int appID)
    {
      SqlCommand cmd = new SqlCommand();
      SqlParameter param;
      string token;
      string sql;

      // Generate a new Token
      token = GenerateToken();

      sql = "INSERT INTO esAppToken(sToken, sAppName, ";
      sql += " sLoginID, iUserID, iAppID, dtCreated) ";
      sql += " VALUES(@sToken, @sAppName, @sLoginID, ";
      sql += "        @iUserID, @iAppID, @dtCreated) ";

      param = new SqlParameter("@sToken", SqlDbType.Char);
      param.Value = token;
      cmd.Parameters.Add(param);

      param = new SqlParameter("@sAppName", SqlDbType.Char);
      param.Value = appName;
      cmd.Parameters.Add(param);

      param = new SqlParameter("@sLoginID", SqlDbType.Char);
      param.Value = Apps.LoginIDNoDomain(loginID);
      cmd.Parameters.Add(param);

      param = new SqlParameter("@iUserID", SqlDbType.Int);
      param.Value = userID;
      cmd.Parameters.Add(param);

      param = new SqlParameter("@iAppID", SqlDbType.Int);
      param.Value = appID;
      cmd.Parameters.Add(param);

      param = new SqlParameter("@dtCreated", SqlDbType.DateTime);
      param.Value = DateTime.Now;
      cmd.Parameters.Add(param);

      try
      {
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sql;

        cmd.Connection = new SqlConnection(mConnectString);
        cmd.Connection.Open();

        cmd.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        if (cmd.Connection.State != ConnectionState.Closed)
        {
          cmd.Connection.Close();
          cmd.Connection.Dispose();
        }
      }

      return token;
    }

    public string GenerateToken()
    {
      return System.Guid.NewGuid().ToString();
    }

    public AppToken VerifyLoginToken(string Token)
    {
      AppToken al = new AppToken();
      DataSet ds = new DataSet();
      SqlCommand cmd;
      DataRow dr;
      SqlDataAdapter da;
      string sql;

      sql = "SELECT iAppTokenID, sAppName, sLoginID, ";
      sql += " iAppID, iUserID ";
      sql += " FROM esAppToken";
      sql += " WHERE sToken = @sToken ";

      try
      {
        cmd = new SqlCommand(sql);
        cmd.Parameters.Add(new 
          SqlParameter("@sToken", SqlDbType.Char));
        cmd.Parameters["@sToken"].Value = Token;
        cmd.Connection = new SqlConnection(mConnectString);

        da = new SqlDataAdapter(cmd);
        da.Fill(ds);

        if (ds.Tables[0].Rows.Count > 0)
        {
          dr = ds.Tables[0].Rows[0];

          al.LoginID = dr["sLoginID"].ToString();
          al.AppName = dr["sAppName"].ToString();
          al.AppKey = Convert.ToInt32(dr["iAppID"]);
          al.LoginKey = Convert.ToInt32(dr["iUserID"]);

          DeleteToken(Convert.ToInt32(dr["iAppTokenID"]));
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }

      return al;
    }

    public void DeleteToken(int AppTokenID)
    {
      SqlCommand cmd = new SqlCommand();
      string sql;

      sql = "DELETE FROM esAppToken ";
      sql += " WHERE iAppTokenID = @iAppTokenID";
      
      try
      {
        cmd.CommandText = sql;
        cmd.Parameters.Add(new 
          SqlParameter("@iAppTokenID", SqlDbType.Int));
        cmd.Parameters["@iAppTokenID"].Value = AppTokenID;

        cmd.Connection = 
          new SqlConnection(mConnectString);
        cmd.Connection.Open();

        cmd.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        if (cmd.Connection.State != ConnectionState.Closed)
        {
          cmd.Connection.Close();
          cmd.Connection.Dispose();
        }
      }
    }

    public DataSet GetAppsByLoginID(string loginID)
    {
      DataSet ds = new DataSet();
      SqlCommand cmd;
      SqlDataAdapter da;
      string sql;

      sql = "SELECT esApps.iAppID, esAppsUsers.iUserID, ";
      sql += " esApps.sAppName, esApps.sDesc, esApps.sURL ";
      sql += " FROM esApps";
      sql += " INNER JOIN esAppsUsers ";
      sql += " ON esApps.iAppID = esAppsUsers.iAppID ";
      sql += " INNER JOIN esUsers ";
      sql += " ON esAppsUsers.iUserID = esUsers.iUserID ";
      sql += " WHERE sLoginID = @sLoginID ";
      sql = String.Format(sql, Apps.LoginIDNoDomain(loginID));

      try
      {
        cmd = new SqlCommand(sql);
        cmd.Parameters.Add(new 
          SqlParameter("@sLoginID", SqlDbType.Char));
        cmd.Parameters["@sLoginID"].Value = Apps.LoginIDNoDomain(loginID);
        cmd.Connection = new SqlConnection(mConnectString);

        da = new SqlDataAdapter(cmd);

        da.Fill(ds);

        return ds;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public static string LoginIDNoDomain(string loginID)
    {
      if (loginID.IndexOf("\\") >= 0)
      {
        loginID = loginID.Substring(loginID.IndexOf("\\")+1);
      }

      return loginID;
    }
	}
}
