using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace SqlDependencyTest
{
    public partial class SearchDependencySQLFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //

        private DataTable GetAllTableAndView()
        {
            string sql = "SELECT name FROM dbo.sysobjects WHERE xtype = 'U' OR xtype = 'v'";
            DataTable dtAllTableAndView = DBHelper.GetDataTable(sql, "SQLDependencyTestConnectionString3");
            return dtAllTableAndView;
        }

        private DataTable GetNeedCacheDBOBject()
        {
            string sql = "SELECT DBObjectName FROM dbo.NeedCacheDBObject";
            DataTable dtNeedCacheDBOBject = DBHelper.GetDataTable(sql, "SQLDependencyTestConnectionString");
            return dtNeedCacheDBOBject;
        }

        protected void btnFilterFile_Click(object sender, EventArgs e)
        {
            string sql = "select * from tbSql";
            DataTable dt = DBHelper.GetDataTable(sql, "SQLDependencyTestConnectionString");
            if (dt != null && dt.Rows.Count > 0)
            {
                StringBuilder sqls = new StringBuilder();
                string deleteSql = "delete from DependencySqlFile;";
                sqls.Append(deleteSql);
                string insertSql = "insert into DependencySqlFile(id,TbSqlID,TableName,NeedModify) values(newid(),'{0}','{1}','{2}');";
                DataTable dtNeedCacheDBOBject = GetNeedCacheDBOBject();
                DataTable dtAllTableAndView = GetAllTableAndView();
                foreach (DataRow dr in dt.Rows)
                {
                    string textData = Convert.ToString(dr["textdata"]);
                    string tbSqlID = Convert.ToString(dr["id"]);
                    if (!string.IsNullOrEmpty(textData))
                    {
                        string tableNames = GetTableNames(textData, dtAllTableAndView);
                        if (!string.IsNullOrEmpty(tableNames))
                        {
                            sqls.Append(string.Format(insertSql, tbSqlID, tableNames.Replace("'", "''"), GetNeedModify(tableNames, dtNeedCacheDBOBject).ToString()));
                        }
                    }
                }
                DBHelper.ExcuteSql(sqls.ToString(), "SQLDependencyTestConnectionString");
            }
        }

        private Boolean GetNeedModify(string tableNames, DataTable dtNeedCacheDBOBject)
        {
            //DataTable dtNeedCacheDBOBject = GetNeedCacheDBOBject();
            if (dtNeedCacheDBOBject != null && dtNeedCacheDBOBject.Rows.Count > 0)
            {
                DataView dv;
                string[] tableName = tableNames.Split(',');
                foreach (string name in tableName)
                {
                    dv = dtNeedCacheDBOBject.DefaultView;
                    dv.RowFilter = string.Format("DBObjectName = '{0}'", name);
                    if (dv.Count > 0)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

         protected void Button1_Click(object sender, EventArgs e)
        {
            string sourceString = txtSource.Text;
            string checkString = txtMacth.Text;
            if (CheckAccurateMatchMain(sourceString, checkString))
            {
                string s = string.Empty;
            }
        }

         private bool CheckAccurateMatchMain(string sourceString, string checkString)
         {
             string lowerSourceString = sourceString.ToLower();
             string lowerCheckString = checkString.ToLower();
             return CheckAccurateMatch(lowerSourceString, lowerCheckString) || CheckAccurateMatch(lowerSourceString, "dbo."  + lowerCheckString);
         }

        private bool CheckAccurateMatch(string sourceString, string checkString)
        {
            int startIndex = sourceString.IndexOf(checkString);
            if (startIndex > -1)
            {
                if (startIndex >= 1)
                {
                    string sStart = sourceString.Substring(startIndex - 1, 1);
                    if (sStart != "," && sStart != " ")
                    {
                        return CheckAccurateMatch(sourceString.Substring(startIndex + checkString.Length),checkString);
                    }
                }

                if (startIndex + checkString.Length == sourceString.Length)
                {
                    return true;
                }
                else
                {
                    string eEnd = sourceString.Substring(startIndex + checkString.Length, 1);
                    if (eEnd == "," || eEnd == " " || eEnd == ".")
                    {
                        return true;
                    }
                    else
                    {
                        return CheckAccurateMatch(sourceString.Substring(startIndex + checkString.Length), checkString);
                    }
                }
            }
            return false;
        }

        private string GetArrayListString(ArrayList tableNames)
        {
            string returnValue = string.Empty;
            foreach (string tableName in tableNames)
            {
                returnValue = returnValue + tableName + ",";
            }
            returnValue = returnValue.Substring(0, returnValue.Length - 1);
            return returnValue;
        }

        private string GetTableNames(string sql, DataTable dtAllTableAndView)
        {
            //DataTable dtAllTableAndView = GetAllTableAndView();
            if (dtAllTableAndView != null && dtAllTableAndView.Rows.Count > 0)
            {
                string dbObjectName = string.Empty;
                string returnValue = string.Empty;
                ArrayList tableNames = new ArrayList();
                foreach (DataRow dr in dtAllTableAndView.Rows)
                {
                    dbObjectName = Convert.ToString(dr["name"]);
                    if (CheckAccurateMatchMain(sql, dbObjectName))
                    {
                        if (!tableNames.Contains(dbObjectName))
                        {
                            tableNames.Add(dbObjectName);
                        }
                    }
                }
                if (tableNames.Count > 0)
                {
                    return GetArrayListString(tableNames);
                }
            }
            return string.Empty;
            //ArrayList tableNames = new ArrayList();
            //ToLowerString(ref sql);
            //ReplaceSuperfluousSpace(ref sql);
            //GetTableNames(sql, tableNames);
            //if (tableNames != null && tableNames.Count  > 0)
            //{
            //    string returnValue = string.Empty;
            //    foreach (string tableName in tableNames)
            //    {
            //        returnValue = returnValue + tableName + ",";
            //    }
            //    returnValue = returnValue.Substring(0, returnValue.Length - 1);
            //    return returnValue;
            //}
            //return string.Empty ;
        }

        private void GetTableNames(string sql, ArrayList tableNames)
        {
            int startIndex = sql.IndexOf("from");
            if (startIndex > -1)
            {
                startIndex = startIndex + 5;
                int endIndex = sql.LastIndexOf("where");
                int length = 0;
                if (endIndex < 0)
                {
                    endIndex = sql.Length - 1;
                    length = sql.Length - startIndex - 1;
                }
                else
                {
                    length = endIndex - startIndex - 1;
                }
                string sql2 = sql.Substring(startIndex, length);
                if (!string.IsNullOrEmpty(sql2))
                {
                    //子查询继续遍历
                    if (sql2.Substring(0, 1) == "(")
                    {
                        GetTableNames(sql2, tableNames);
                    }
                    else
                    {
                        //a,b,c 类型的直接获取表名
                        if (sql2.IndexOf(",") > -1)
                        {
                            string[] tableNames2 = sql2.Split(',');
                            //string tableName;
                            foreach (string tableName in tableNames2)
                            {
                                AddTableName(ref tableNames, tableName);
                            }
                        }
                        else
                        {
                            //单表直接获取
                            if (sql2.IndexOf(" ") < 0)
                            {
                                AddTableName(ref tableNames, sql2);
                            }
                            else
                            {
                                startIndex = 0;
                                endIndex = sql2.IndexOf(" ");
                                AddTableName(ref tableNames, sql2.Substring(startIndex, endIndex - startIndex));
                                for (int i = endIndex + 1; i < sql2.Length; i++)
                                {
                                    // inner join b on -> b
                                    if (i + 5 < sql2.Length)
                                    {
                                        if (sql2.Substring(i, 5) == "join ")
                                        {
                                            startIndex = i + 5;
                                            string s = sql2.Substring(i + 5);
                                            endIndex = startIndex + s.IndexOf(" ");
                                            string tableName = sql2.Substring(startIndex, endIndex - startIndex);
                                            AddTableName(ref tableNames, tableName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //from XXXX where -> XXXX

            }
        }

        private void AddTableName(ref ArrayList tableNames, string tablename)
        {
            if (tablename.IndexOf(" ") > -1)
            {
                tablename = tablename.Substring(0, tablename.IndexOf(" "));
            }
            if (!tableNames.Contains(tablename))
            {
                tableNames.Add(tablename);
            }
        }

        private void ReplaceSuperfluousSpace(ref string sql)
        {
            sql = sql.Replace(@"\s+ ", " ");
        }

        private void ReplaceJoinOn(string sql)
        {
            sql = sql.Replace("inner join", "");
            sql = sql.Replace("outer join", "");
            sql = sql.Replace("left join", "");
            sql = sql.Replace("right join", "");
            sql = sql.Replace("left outer join", "");
            sql = sql.Replace("right outer join", "");
            sql = sql.Replace(@"on\s+([\w|\W]+)\s+,", ",");
        }

        private void ToLowerString(ref string sql)
        {
            sql = sql.ToLower();
        }

        protected void txtAnalysisXml_Click(object sender, EventArgs e)
        {
            string[] paths = new string[] { this.txtCbglPath.Text.Trim(), this.txtSlxtPath.Text.Trim() };
            foreach (string path in paths)
            {
                AnalysisXml(path);
            }
        }

        private string GetMapFilePath(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    int startIndex = filePath.IndexOf("Map");
                    return filePath.Substring(startIndex + 3);
                }
                return string.Empty;
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }

        private void ExcuteInsertSql(ArrayList insertSqls)
        {
            StringBuilder sql = new StringBuilder();
            foreach (string insertSql in insertSqls)
            {
                sql.Append(insertSql);
            }

            string excuteSql = sql.ToString();
            if (!string.IsNullOrEmpty(excuteSql))
            {
                DBHelper.ExcuteSql(excuteSql, "SQLDependencyTestConnectionString");
            }
        }

        private string TrimString(string sql)
        {
            string returnValue = sql.Trim ();
            returnValue = returnValue.Replace("\r\n", "");
            returnValue = Regex.Replace(returnValue, @"\s+", " "); 
            //returnValue = returnValue.Replace("\\s+", " ");
            return returnValue;
        }

        private void AnalysisXml(string[] xmlPaths)
        {
            if (xmlPaths.Length > 0)
            {
                ArrayList sqlCollection = new ArrayList();
                XmlDocument xmlDoc;
                XmlNodeList sqlNodes;
                string sql = string.Empty;
                string insertSql = "insert into TbSQL(id,[function],content,textdata,FilePath,StartLine) values (newid(),'','','{0}','{1}','0');";
                ArrayList insertSqls = new ArrayList();
                foreach (string xmlPath in xmlPaths)
                {
                    xmlDoc = new XmlDocument();
                    try
                    {
                        xmlDoc.Load(xmlPath);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    sqlNodes = xmlDoc.SelectNodes("/page/controls/control/datasource/sql");
                    if (sqlNodes != null && sqlNodes.Count > 0)
                    {
                        foreach (XmlNode sqlNode in sqlNodes)
                        {
                            sql = TrimString(sqlNode.InnerText);
                            if (!string.IsNullOrEmpty(sql) && sql.Length > 6 && sql.Substring(0, 6).ToLower() == "select")
                            {
                                if (!sqlCollection.Contains(sql))
                                {
                                    sqlCollection.Add(sql);
                                    insertSqls.Add(string.Format(insertSql, sql.Replace("'", "''"), GetMapFilePath(xmlPath)));
                                }
                            }
                        }
                    }
                }
                ExcuteInsertSql(insertSqls);
            }
        }

        private void AnalysisXml(string path)
        {
            if (Directory.Exists(path))
            {
                string[] xmlPaths = Directory.GetFiles(@path, "*.xml",SearchOption.AllDirectories );
                AnalysisXml(xmlPaths);
            }
        }

        protected void btnGetContainDBObject_Click(object sender, EventArgs e)
        {
            DataTable dtAllTableAndView = GetAllTableAndView();
            DataTable dtNeedCacheDBOBject = GetNeedCacheDBOBject();
            txtContainDBObject.Text  = GetTableNames(txtSql.Text , dtAllTableAndView);
            txtNeedCache.Text = GetNeedModify(txtContainDBObject.Text, dtNeedCacheDBOBject).ToString();
        }

        private void InsertUserActionStringCache()
        {
            string sql = "SELECT TOP 310000 ActionCode,ObjectType,Application FROM dbo.myUserRights ORDER BY ObjectType ";
            DataTable userActionDT = DBHelper.GetDataTable(sql, "SQLDependencyTestConnectionString3");
            string functionCode = string.Empty;
            string actionString = string.Empty;
            Hashtable userActionRightsHashTable = new Hashtable();
            foreach (DataRow userActionDR in userActionDT.Rows)
            {
                if (string.Compare(Convert.ToString(userActionDR["ObjectType"]), functionCode) == 0)
                {
                    actionString += (Convert.ToString(userActionDR["ActionCode"]) + ",");
                }
                else
                {
                    if (!string.IsNullOrEmpty(functionCode) && !string.IsNullOrEmpty(actionString))
                    {
                        userActionRightsHashTable.Add(functionCode, actionString);
                    }
                    functionCode = Convert.ToString(userActionDR["ObjectType"]);
                    actionString = Convert.ToString(userActionDR["ActionCode"]) + ",";
                }
            }
            HttpContext.Current.Cache.Insert("ActionString", userActionRightsHashTable);
        }

        protected void btnInsertUserActionStringCache_Click(object sender, EventArgs e)
        {
            InsertUserActionStringCache();
        }

        protected void txtMapPath_TextChanged(object sender, EventArgs e)
        {
   
        }

        private void AnalysisMapXml(string[] childDirectories)
        {
            string[] excludedDirectories = new string[] { "Cbgl", "CgZtb", "CgZtbWeb", "CustomControl", "GlJsc", "Hyxt", "Kfxt", "MyChart", "MyWorkflow", "PubProject", "Qmys", "SlPda", "Slxt", "Tzfx", "Xmjd", "Zlxt" };
            string directoryName = string.Empty;
            foreach (string childDirectory in childDirectories)
            {
                directoryName = childDirectory.Substring(childDirectory.LastIndexOf("\\") + 1);
                if (excludedDirectories.Contains<string>(directoryName))
                {
                    continue;
                }
                else
                {
                    AnalysisXml(childDirectory);
                }
            }

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            string path = txtMapPath.Text.Trim();
            if (Directory.Exists(path))
            {
                string[] childDirectories = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
                AnalysisMapXml(childDirectories);
                string[] xmlPaths = Directory.GetFiles(@path, "*.xml", SearchOption.TopDirectoryOnly);
                AnalysisXml(xmlPaths);
            }
        }
    }
}
