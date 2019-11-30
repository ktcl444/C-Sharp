using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Web.Caching;

namespace CacheMemoryTest
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
                connectionString = WebConfigurationManager.ConnectionStrings["SQLDependencyTestConnectionString"].ConnectionString;
            }
            return connectionString;
        }

        public static string GetSqlConnectionString(string appName)
        {
            if (appName.Length == 0)
            {
                return GetSqlConnectionString();
            }
            return WebConfigurationManager.ConnectionStrings[appName].ConnectionString;
        }

        //public static SqlCacheDependency GetSqlCacheDependency(string sql)
        //{
        //    if (sqlDependencies.Count > 0 && sqlDependencies.ContainsKey(sql))
        //    {
        //        return sqlDependencies[sql] as SqlCacheDependency;
        //    }
        //    else
        //    {
        //        SqlCacheDependency sqlCacheDependency = new SqlCacheDependency(GetSqlCommand(sql));
        //        sqlDependencies.Add(sql, sqlCacheDependency);
        //        return sqlCacheDependency;
        //    }
        //}

        public static SqlConnection GetSqlConnection()
        {
            return GetSqlConnection(string.Empty);
        }

        public static SqlConnection GetSqlConnection(string appName)
        {
            string connectString = GetSqlConnectionString(appName);
            
                conn = new SqlConnection(connectString);
            return conn;
        }

        public static SqlCommand GetSqlCommand(string sql)
        {
            return GetSqlCommand(sql, string.Empty);
        }

        public static SqlCommand GetSqlCommand(string sql, string appName)
        {
            using (SqlConnection conn = GetSqlConnection(appName))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                return new SqlCommand(sql, conn);
            }
           
        }

        public static DataSet GetDataSet(string sql, string tableName, out SqlCacheDependency sqlCacheDependency)
        {
            SqlCommand cmd = GetSqlCommand(sql);
            sqlCacheDependency = new SqlCacheDependency(cmd);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds, tableName);
            return ds;
        }

        public static DataSet GetDataSet(string sql)
        {
            return GetDataSet(sql, string.Empty);
        }

        public static DataSet GetDataSet(string sql, string appName)
        {
            SqlCommand cmd = GetSqlCommand(sql, appName);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }

        public static DataTable GetDataTable(string sql, string appName)
        {
            //SqlCommand cmd = GetSqlCommand(sql, appName);
            //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //adapter.Fill(dt);
            //return dt;

            string connStr = GetSqlConnectionString(appName);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(comm);
                adapter.Fill(dt);
                return dt;
            }
        }

        public static string GetDateItemString(string sql, string appName)
        {
        //    SqlCommand cmd = GetSqlCommand(sql, appName);
        //    return Convert.ToString(cmd.ExecuteScalar());

            string connStr = GetSqlConnectionString(appName);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
     
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        public static DataSet GetDataSet(string sql, string[] dependencySqls, string appName)
        {
            DataSet ds = new DataSet();
            if (CacheHelper.GetCache()[sql] != null)
            {
                ds = CacheHelper.GetCache()[sql] as DataSet;
            }
            else
            {
                AggregateCacheDependency aggregateCacheDependency = new AggregateCacheDependency();
                CacheDependency[] sqlDependencys = new CacheDependency[dependencySqls.Length];
                int i = 0;
                foreach (var dependencySql in dependencySqls)
                {
                    if (!string.IsNullOrEmpty(dependencySql))
                    {
                        //AddSqlDependency(dependencySql, sql, appName);
                        sqlDependencys[i] = AddSqlDependency(dependencySql, appName);
                        i++;
                    }
                }
                ds = GetDataSet(sql, appName);
                CacheHelper.GetCache()[sql] = ds;
                aggregateCacheDependency.Add(sqlDependencys);
                CacheHelper.GetCache().Insert(sql, ds, aggregateCacheDependency);
            }
            return ds;
        }

        //private static void AddSqlDependency(string dependencySql,string sql,string appName)
        //{
        //    SqlDependency sqlDep = AddSqlDependency(dependencySql,appName,
        //  delegate(object sender, SqlNotificationEventArgs e)
        //  {
        //      if (e.Info == SqlNotificationInfo.Invalid)
        //      {
        //          // sqlDbHelper.ExecuteNonQuery("ALTER DATABASE AdventureWorksDW SET ENABLE_BROKER");
        //          // 写文件，数据库未开启Service Broker或者提供了无法通知的语句，例如没有写包括数据库所有者的表名。
        //      }
        //      CacheHelper.GetCache().Remove(sql);
        //  });
        //}

        public static SqlCacheDependency AddSqlDependency(string strSql, string appName)
        {
            string connStr = GetSqlConnectionString(appName);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand comm = new SqlCommand(strSql, conn);
                // 添加依赖
                SqlCacheDependency sqlDep = new SqlCacheDependency(comm);
                if (conn.State == ConnectionState.Closed) conn.Open();
                comm.ExecuteNonQuery();
                return sqlDep;
            }
        }


        public static SqlDependency AddSqlDependency(string strSql, string appName, OnChangeEventHandler sqlDep_OnChange)
        {
            string connStr = GetSqlConnectionString(appName);
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
    }
}
