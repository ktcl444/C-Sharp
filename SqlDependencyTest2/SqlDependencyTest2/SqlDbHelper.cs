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
        this.connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Conn"].ConnectionString;
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

    public SqlDependency AddSqlDependency(string strSql, OnChangeEventHandler sqlDep_OnChange)
    {
        string connStr = this.connectionString;
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            SqlCommand comm = new SqlCommand(strSql, conn);
            // 添加依赖
            SqlDependency sqlDep = new SqlDependency(comm);
            sqlDep.OnChange += sqlDep_OnChange;
            if (conn.State == ConnectionState.Closed) conn.Open();
            comm.ExecuteNonQuery();
            return sqlDep;
        }
    }

    /**//// <summary>
    /// 对表增加依赖列，用于Sql依赖，或者用某个int列也可
    /// </summary>
    /// <param name="tableName">表名，如果不是dbo所有者，请提供包括所有者的完整表名</param>
    /// <returns></returns>
    public int AddDependencyCloumn(string tableName)
    {
        return this.ExecuteNonQuery(string.Format("declare @num int "
            + "set @num = (select count(*) from syscolumns where id=object_id('{0}') and name = 'dep') "
            + "if @num = 0 ALTER TABLE {0} ADD dep bit NOT NULL CONSTRAINT dep{0} DEFAULT 0 ", tableName));
    }

}


