using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text.RegularExpressions;

using System.IO;
using System.Reflection;

namespace ConsoleApplication1.SyntaxEngineClass
{
    public class SqlQueryProvider
    {
        private static Regex g_ParagraphSeparator = new Regex(@"^[\t ]*GO[\t ]*$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static IDictionary<int, string> SplitQuery(string query)
        {
            query = query.Replace("\r\n", "\n").Replace('\r', '\n');
            Dictionary<int, string> qs = new Dictionary<int, string>();
            string[] lines = query.Split('\n');
            int startLine = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (g_ParagraphSeparator.IsMatch(lines[i]))
                {
                    qs.Add(startLine, string.Join("\n", lines, startLine, i - startLine));
                    startLine = i + 1;
                }
                else
                {
                    if (i == lines.Length - 1)
                    {
                        qs.Add(startLine, string.Join("\n", lines, startLine, i - startLine));
                    }
                }
            }
            return qs;
        }

        private static string BuildDBExceptionMessage(QueryException ex)
        {
            //if (ex is SqlException)
            //{
            //    SqlException de = (sqlexception)ex;
            //    return string.Format("消息 {0}，级别 {1}，状态 {2}，第 {3} 行\r\n{4}", de.Number, de.Class, de.State, lineNumber, de.Message);
            //}
            //else
            //{
            //    return "Sql Executing Error." + ex.Message;
            //}
            return string.Format("第 {0} 行发生错误 :\r\n{1}", ex.LineNumber.ToString(), ex.Message);
        }

        private static string GetSQLByFile(string[] filePaths)
        {
            FileStream fs = null;
            byte[] buf;
            string sql = string.Empty;
            try
            {
                foreach (string filePath in filePaths)
                {
                    if (fs != null)
                    {
                        fs.Flush();
                    }
                    fs = File.OpenRead(filePath);
                    buf = new byte[fs.Length];
                    fs.Read(buf, 0, (int)fs.Length);
                    string temp = Encoding.UTF8.GetString(buf).Substring(1);
                    if (sql.Length == 0)
                    {
                        sql = temp;
                    }
                    else
                    {
                        sql = sql + "\r\n" + temp;
                    }
                }
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return sql;
        }

        public static string CheckSyntaxByFile(string connectionString, string[] filePaths)
        {
            string returnValue = string.Empty;
            string sql = GetSQLByFile(filePaths);
            if (sql.Length != 0)
            {
                returnValue = CheckSyntaxBySql(connectionString, sql);
            }
            return returnValue;
        }

        public static string CheckSyntaxByFile(string connectionString, string filePath)
        {
            string[] filePaths = new string[1];
            filePaths[0] = filePath;
            return CheckSyntaxByFile(connectionString, filePaths);
        }

        public static string ExcuteSql(string connectionString, string filePath)
        {
            string returnValue = string.Empty;
            string[] filePaths = new string[1];
            filePaths[0] = filePath;
            string query = GetSQLByFile(filePaths);
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = conn.CreateCommand();
            IDictionary<int, string> queries = SplitQuery(query);
            conn.Open();

            SqlTransaction tran = conn.BeginTransaction();
            cmd.Transaction = tran;
            try
            {
                foreach (KeyValuePair<int, string> q in queries)
                {

                    cmd.CommandText = q.Value;
                    // 异常在下面一行抛出
                    if (q.Value.ToString().Length != 0)
                    {
                        cmd.ExecuteNonQuery();
                    }

                }
                tran.Commit();
            }
            catch(Exception ex)
            {
                returnValue = ex.Message;
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                cmd.Dispose();
                conn.Dispose();
            }
            return returnValue;

        }

        public static string CheckSyntaxBySql(string connectionString, string query)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = conn.CreateCommand();
            IDictionary<int, string> queries = SplitQuery(query);
            //IDictionary<int, string> queries = new Dictionary<int,string>();
            //queries.Add(0, query);
            try
            {
                conn.Open();
                cmd.CommandText = "SET PARSEONLY ON";
                cmd.ExecuteNonQuery();
                try
                {
                    foreach (KeyValuePair<int, string> q in queries)
                    {
                        try
                        {
                            cmd.CommandText = q.Value;
                            // 异常在下面一行抛出
                            if (q.Value.ToString().Length != 0)
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch (Exception de)
                        {
                            throw new QueryException(q.Key, de.Message, query, de);
                        }
                    }
                }
                finally
                {
                    cmd.CommandText = "SET PARSEONLY OFF";
                    cmd.ExecuteNonQuery();
                }
                return string.Empty;
            }
            catch (QueryException qe)
            {
                return BuildDBExceptionMessage(qe);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                cmd.Dispose();
                conn.Dispose();
            }
        }
    }

    public class QueryException : DbException
    {
        public QueryException(int lineNumber, string message, string queryString, Exception innerException)
            : base(message, innerException)
        {
            m_LineNumber = lineNumber;
            m_QueryString = queryString;
        }
        private string m_QueryString;
        public string QueryString
        {
            get
            {
                return m_QueryString;
            }
        }
        private int m_LineNumber;
        public int LineNumber
        {
            get
            {
                return m_LineNumber;
            }
        }
    }

}
