using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DataAccess
{
    
    /// <summary>
    /// SqlDbHelper:操作SQL Server数据库的通用类
    /// </summary>
    public class SqlDbHelper
    {
        #region 字段
        /// <summary>
        /// 私有字段
        /// </summary>
        private string connectionString;

        #endregion

        #region 属性

        /// <summary>
        /// 公共属性 数据库连接字符串
        /// </summary>
        public string ConnectiongString
        {
            set { connectionString = value; }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlDbHelper()
        {
            //修改connectionString为项目中的数据库连接字符串
            //connectionString="";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strConnectionString">数据库连接字符串</param>
        public SqlDbHelper(string strConnectionString)
        {
            connectionString = strConnectionString;
        }

        #endregion

        #region 判断数据库是否连接

        /// <summary>
        /// 判断数据库是否连接
        /// </summary>
        /// <returns>是否连接</returns>
        public bool IsConnected()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 执行一个查询，并返回结果集

        /// <summary>
        /// 执行一个查询，并返回结果集
        /// </summary>
        /// <param name="sql">要查询的SQL文本命令</param>
        /// <returns>查询结果集</returns>
        public DataTable ExecuteDataTable(string sql)
        {
            return ExecuteDataTable(sql, CommandType.Text, null);
        }

        /// <summary>
        /// 执行一个查询，并返回结果集
        /// </summary>
        /// <param name="sql">要查询的SQL文本命令</param>
        /// <param name="commandType">查询语句类型，存储过程或SQL文本命令</param>
        /// <returns>查询结果集</returns>
        public DataTable ExecuteDataTable(string sql, CommandType commandType)
        {
            return ExecuteDataTable(sql, commandType, null);
        }

        /// <summary>
        /// 执行一个查询，并返回结果集
        /// </summary>
        /// <param name="sql">要查询的SQL文本命令</param>
        /// <param name="commandType">查询语句类型，存储过程或SQL文本命令</param>
        /// <param name="parameters">T-SQL语句或存储过程的参数组</param>
        /// <returns>查询结果集</returns>
        public DataTable ExecuteDataTable(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            //实例化DataTable，用于装载查询结果集
            DataTable data = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    //指定CommandType
                    command.CommandType = commandType;

                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    //实例化SqlDataAdapter
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    //填充DataTable
                    adapter.Fill(data);
                }
            }
            return data;

        }

        #endregion

        #region 返回一个DataReader对象实例

        /// <summary>
        /// 返回一个DataReader对象实例
        /// </summary>
        /// <param name="sql">要查询的SQL文本命令</param>
        /// <returns>DataReader对象实例</returns>
        public SqlDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, CommandType.Text, null);
        }

        /// <summary>
        /// 返回一个DataReader对象实例
        /// </summary>
        /// <param name="sql">要查询的SQL文本命令</param>
        /// <param name="commandType">要执行的查询语句的类型，存储过程或SQL文本命令</param>
        /// <returns>DataReader对象实例</returns>
        public SqlDataReader ExecuteReader(string sql, CommandType commandType)
        {
            return ExecuteReader(sql, commandType, null);
        }

        /// <summary>
        /// 返回一个DataReader对象实例
        /// </summary>
        /// <param name="sql">要查询的SQL文本命令</param>
        /// <param name="commandType">要执行的查询语句的类型，存储过程或SQL文本命令</param>
        /// <param name="parameters">T-SQL语句或存储过程的参数数组</param>
        /// <returns>DataReader对象实例</returns>
        public SqlDataReader ExecuteReader(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);

            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            connection.Open();
            //参数CommandBehavior.CloseConnection表示，关闭Reader对象的同时关闭Connection对象
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        #endregion

        #region 执行查询结果，返回第一行的第一列

        /// <summary>
        /// 执行查询结果，返回第一行的第一列
        /// </summary>
        /// <param name="sql">要查询的SQL文本命令</param>
        /// <returns>返回第一行的第一列</returns>
        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, CommandType.Text, null);
        }

        /// <summary>
        /// 执行查询结果，返回第一行的第一列
        /// </summary>
        /// <param name="sql">要查询的SQL文本命令</param>
        /// <param name="commandType">要执行的查询语句的类型，存储过程或SQL文本命令</param>
        /// <returns>返回第一行的第一列</returns>
        public object ExecuteScalar(string sql, CommandType commandType)
        {
            return ExecuteScalar(sql, commandType, null);
        }

        /// <summary>
        /// 执行查询结果，返回第一行的第一列
        /// </summary>
        /// <param name="sql">要查询的SQL文本命令</param>
        /// <param name="commandType">要执行的查询语句的类型，存储过程或SQL文本命令</param>
        /// <param name="parameters">T-SQL语句或存储过程的参数数组</param>
        /// <returns>返回第一行的第一列</returns>
        public object ExecuteScalar(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            object result = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;

                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    connection.Open();
                    result = command.ExecuteScalar();
                }

            }
            return result;
        }

        #endregion

        #region 对数据库进行增删改操作

        /// <summary>
        /// 对数据库进行增删改操作
        /// </summary>
        /// <param name="sql">要执行的SQL文本命令</param>
        /// <returns>返回受影响的函数</returns>
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, CommandType.Text, null);
        }

        /// <summary>
        /// 对数据库进行增删改操作
        /// </summary>
        /// <param name="sql">要执行的SQL文本命令</param>
        /// <param name="commandType">要执行的查询语句的类型，存储过程或SQL文本命令</param>
        /// <returns>返回受影响的函数</returns>
        public int ExecuteNonQuery(string sql, CommandType commandType)
        {
            return ExecuteNonQuery(sql, commandType, null);
        }

        /// <summary>
        /// 对数据库进行增删改操作
        /// </summary>
        /// <param name="sql">要执行的SQL文本命令</param>
        /// <param name="commandType">要执行的查询语句的类型，存储过程或SQL文本命令</param>
        /// <param name="parameters">T-SQL语句或存储过程的参数数组</param>
        /// <returns>返回受影响的函数</returns>
        public int ExecuteNonQuery(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;

                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();
                    count = command.ExecuteNonQuery();

                }

            }
            return count;

        }


        #endregion

    }

}
