using System;
using System.Configuration;

using System.Data;
using System.Data.SqlClient;

/// <summary>
/// SqlDbHelper 的摘要说明
/// </summary>
public sealed class SqlDbHelper
{
    //单态模式
    #region 单态模式
    static SqlDbHelper sqlDbHelper = new SqlDbHelper();
    public static SqlDbHelper Instance() { return sqlDbHelper; }
    private SqlDbHelper()
    {
    }
    #endregion

    private string connectionString;
    public string ConnectionString
    {
        get { return connectionString; }
        set { connectionString = value; }
    }

    public DataTable GetDataTable(string strSql)
    {
        string connStr = this.connectionString;
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            SqlCommand comm = new SqlCommand(strSql, conn);
            SqlDataAdapter da = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

    }

    public int ExecuteNonQuery(string strSql)
    {
        string connStr = this.connectionString;
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            SqlCommand comm = new SqlCommand(strSql, conn);
            if (conn.State == ConnectionState.Closed) conn.Open();
            return comm.ExecuteNonQuery();
        }
    }
}


