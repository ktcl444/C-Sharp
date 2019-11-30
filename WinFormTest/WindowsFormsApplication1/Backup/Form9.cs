using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace WindowsFormsApplication1
{
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
            
            AllDataRightsSql = GetAllDataRightsSql();
            AllStationObjectSql = GetAllStationObjectSql();
            InitCheckResultListView();
           
        }

        private static string AllDataRightsSql;
        private static string AllStationObjectSql;
        private NoSortHashtable CheckResults;
        private static string SqlConnectingString = "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}";

         struct  StationCheckResult
        {
            public string stationGUID;
            public string stationName;
            public string buName;
            public NoSortHashtable projectCheckResults;
        }

         struct ProjectCheckResult
        {
             public string projectGUID;
             public string projectName;
             public string parentProjectName;
        }

        private void InitCheckResultListView()
        {
            this.listView1.Columns.Add("序号",40);
            this.listView1.Columns.Add("公司名称",100);
            this.listView1.Columns.Add("岗位名称",100);
            this.listView1.Columns.Add("一级项目（有权限）",100);
            this.listView1.Columns.Add("二级项目（无权限）",100);
        }

        private bool CheckSqlConnetionString()
        {
            if (string.IsNullOrEmpty(txtDBName.Text))
            {
                MessageBox.Show("请输入数据库名称", "参数错误");
                return false;
            }
            if (string.IsNullOrEmpty(txtServerName .Text))
            {
                MessageBox.Show("请输入服务器地址", "参数错误");
                return false;
            }
            if (string.IsNullOrEmpty(txtUserName .Text))
            {
                MessageBox.Show("请输入账号", "参数错误");
                return false;
            }
            if (string.IsNullOrEmpty(txtUserPassword .Text))
            {
                MessageBox.Show("请输入密码", "参数错误");
                return false;
            }
            return true;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (CheckSqlConnetionString())
            {
                try
                {
                    SqlDbHelper.Instance().ConnectionString = string.Format(SqlConnectingString,txtServerName.Text,txtDBName.Text,txtUserName.Text,txtUserPassword.Text  );

                    DisableForm("检查中...");
                    this.txtCheckResult.Clear();
                    this.listView1.Items.Clear();

                    string result = CheckStationDataRight();
                    //this.txtCheckResult.AppendText(result);

                    MessageBox.Show("检查完成！", "检查结果");

                    ShowCheckResult();


                    EnableForm();
                }
                catch (Exception ex)
                {
                    EnableForm();
                    this.txtCheckResult.AppendText(ex.Message);
                }
            }
        }

        private void DisableForm(string title)
        {
            this.Enabled = false;
            this.Text = title;
        }

        private void EnableForm()
        {
            this.Text = "数据权限检查工具";
            this.Enabled = true;
        }

        private void ExportToExcel()
        {
            if (CheckResults != null && CheckResults.Count > 0)
            {
                System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();

                sfd.DefaultExt = "xls";

                sfd.Filter = "Excel文件(*.xls)|*.xls";

                if (sfd.ShowDialog() == DialogResult.OK)
                {

                    DoExport(this.listView1 , sfd.FileName);

                } 
            }
            else
            {
                MessageBox.Show("未进行检查或者检查结果为空！");
            }
        }

        private void DoExport(ListView listView, string strFileName)
        {

            int rowNum = listView.Items.Count;

            int columnNum = listView.Items[0].SubItems.Count;

            int rowIndex = 1;

            int columnIndex = 0;

            if (rowNum == 0 || string.IsNullOrEmpty(strFileName))
            {

                return;

            }

            if (rowNum > 0)
            {



                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();

                if (xlApp == null)
                {

                    MessageBox.Show("无法创建excel对象，可能您的系统没有安装excel");

                    return;

                }

                xlApp.DefaultFilePath = "";

                xlApp.DisplayAlerts = true;

                xlApp.SheetsInNewWorkbook = 1;


                Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);

                //将ListView的列名导入Excel表第一行 

                foreach (ColumnHeader dc in listView.Columns)
                {

                    columnIndex++;

                    xlApp.Cells[rowIndex, columnIndex] = dc.Text;

                }
                //[提取工作簿中的活动工作表]
                Microsoft.Office.Interop.Excel.Worksheet myWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.ActiveSheet;
                Microsoft.Office.Interop.Excel.Range myRange = myWorksheet.get_Range(
 myWorksheet.Cells[1, 1],
 myWorksheet.Cells[rowNum, 1]);
                myRange.ColumnWidth  = 10;

                myRange = myWorksheet.get_Range(
 myWorksheet.Cells[1, 2],
 myWorksheet.Cells[rowNum, 2]);
                myRange.ColumnWidth = 50;


                myRange = myWorksheet.get_Range(
 myWorksheet.Cells[1, 3],
 myWorksheet.Cells[rowNum, 3]);
                myRange.ColumnWidth = 30;


                myRange = myWorksheet.get_Range(
 myWorksheet.Cells[1, 4],
 myWorksheet.Cells[rowNum, 4]);
                myRange.ColumnWidth = 30;

                myRange = myWorksheet.get_Range(
myWorksheet.Cells[1, 5],
myWorksheet.Cells[rowNum, 5]);
                myRange.ColumnWidth = 30;

                myRange = myWorksheet.get_Range(
 myWorksheet.Cells[1, 1],
 myWorksheet.Cells[1, 5]);
                myRange.Font.Bold  = true;

                myRange = myWorksheet.get_Range(
myWorksheet.Cells[1, 1],
myWorksheet.Cells[rowNum + 1, 5]);
                myRange.Borders.LineStyle   = 7;


                //将ListView中的数据导入Excel中 

                for (int i = 0; i < rowNum; i++)
                {

                    rowIndex++;

                    columnIndex = 0;

                    for (int j = 0; j < columnNum; j++)
                    {

                        columnIndex++;

                        //注意这个在导出的时候加了“\t” 的目的就是避免导出的数据显示为科学计数法。可以放在每行的首尾。 

                        xlApp.Cells[rowIndex, columnIndex] = Convert.ToString(listView.Items[i].SubItems[j].Text) + "\t";

                    }

                }

                //例外需要说明的是用strFileName,Excel.XlFileFormat.xlExcel9795保存方式时当你的Excel版本不是95、97 而是2003、2007 时导出的时候会报一个错误：异常来自 HRESULT:0x800A03EC。解决办法就是换成strFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal。 

                xlBook.SaveAs(strFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myRange);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myWorksheet);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlBook);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlApp);

                MessageBox.Show("导出成功！","导出结果");

            }

        }

        private void ShowCheckResult()
        {
            if (CheckResults != null && CheckResults.Count > 0)
            {
                int i = 1;

                foreach (string stationResultKey in CheckResults.Keys )
                {
                    StationCheckResult stationResult = (StationCheckResult)CheckResults[stationResultKey];
                    if (stationResult.projectCheckResults != null && stationResult.projectCheckResults.Count > 0)
                    {
                        foreach (string projectResultKey in stationResult.projectCheckResults.Keys)
                        {
                            ProjectCheckResult projectResult = (ProjectCheckResult)stationResult.projectCheckResults[projectResultKey];
                            string key = Guid.NewGuid().ToString();
                            this.listView1.Items.Add(key, i.ToString(), 0);
                            this.listView1.Items[key].SubItems.Add(stationResult.buName);
                            this.listView1.Items[key].SubItems.Add(stationResult.stationName);
                            this.listView1.Items[key].SubItems.Add(projectResult.parentProjectName);
                            this.listView1.Items[key].SubItems.Add(projectResult.projectName);
                            i++;
                        }
                    }
                }

                //foreach (StationCheckResult stationResult in CheckResults.Values )
                //{
                //    if (stationResult.projectCheckResults != null && stationResult.projectCheckResults.Count > 0)
                //    {
                //        foreach (ProjectCheckResult projectResult in stationResult.projectCheckResults.Values )
                //        {
                //            string key = Guid.NewGuid().ToString();
                //            this.listView1.Items.Add(key, i.ToString(),0);
                //            this.listView1.Items[key].SubItems.Add(stationResult.buName );
                //            this.listView1.Items[key].SubItems.Add(stationResult.stationName );
                //            this.listView1.Items[key].SubItems.Add(projectResult.parentProjectName  );
                //            this.listView1.Items[key].SubItems.Add(projectResult.projectName );
                //            i++;
                //        }
                //    }
                //}
            }
        }

        private string CheckStationDataRight()
        {
            CheckResults = new NoSortHashtable();
            string checkResult = string.Empty;
            //1，获得所有岗位权限
            DataTable allStationObjectDT = SqlDbHelper.Instance().GetDataTable(AllStationObjectSql);
            //2，获得所有权限定义
            DataTable allDataRightsDT = SqlDbHelper.Instance().GetDataTable(AllDataRightsSql);
            if (allStationObjectDT == null || allStationObjectDT.Rows.Count == 0)
            {
                return "获取岗位数据权限为空。";
            }
            if (allDataRightsDT == null || allDataRightsDT.Rows.Count == 0)
            {
                return "获取数据权限定义为空。";
            }
            string stationName = string.Empty;
            string buName = string.Empty;
            string objectGUID = string.Empty;
            string objectHierarchycode = string.Empty;
            string stationGUID = string.Empty;
            StationCheckResult stationResult;
            //3，遍历岗位权限
            foreach ( DataRow stationObjectDr  in allStationObjectDT.Rows )
	        {
                DataRow [] dataRightDrs;
                stationName = Convert.ToString(stationObjectDr["StationName"]);
                buName = Convert.ToString(stationObjectDr["namepath"]);
                objectGUID = Convert.ToString(stationObjectDr["ObjectGUID"]);
                stationGUID = Convert.ToString(stationObjectDr["StationGUID"]);

                if (CheckResults.ContainsKey(stationGUID))
                {
                    stationResult = (StationCheckResult)CheckResults[stationGUID];
                }
                else
                {
                    stationResult = new StationCheckResult();
                    stationResult.buName = buName;
                    stationResult.stationGUID = stationGUID;
                    stationResult.stationName = stationName;
                    stationResult.projectCheckResults = new NoSortHashtable();
                    CheckResults.Add(stationGUID, stationResult);
                }

                dataRightDrs = allDataRightsDT.Select("_guid = '" + objectGUID + "'");
                if (dataRightDrs.Length >0)
                {
                    //4，通过所有权限定义数据集找出岗位权限中数据对象的下级
                    string checkResultTitle = "公司：" + buName + " 岗位：" + stationName + " 缺少以下数据权限：" + System.Environment.NewLine.ToString() ;
                    objectHierarchycode = Convert.ToString(dataRightDrs[0]["_hierarchycode"]);
                    string parentObjectName = Convert.ToString(dataRightDrs[0]["_name"]);
                    DataRow[] childDataRightDrs;
                    childDataRightDrs = allDataRightsDT.Select("_hierarchycode + '.' like '" + objectHierarchycode + ".*' AND _guid <> '" + objectGUID + "'", "_hierarchycode");
                    if (childDataRightDrs.Length > 0)
                    {
                        //5，通过岗位权限数据集，查找是否所有下级都已经授权
                        string checkResultContent = string.Empty;
                        ProjectCheckResult projectResult;
                        foreach (DataRow childDataRightDr in childDataRightDrs)
                        {
                            string childObjectGUID = Convert.ToString(childDataRightDr["_guid"]);
                            string childObjectName =  Convert.ToString(childDataRightDr["_name"]);
                            if (allStationObjectDT.Select("StationGUID = '" + stationGUID + "' AND ObjectGUID = '" + childObjectGUID + "'").Length == 0)
                            {
                                //6，未授权的子级对象需要提示
                                checkResultContent = checkResultContent + "    " + "名称：" + childObjectName + " 父级名称：" + parentObjectName + System.Environment.NewLine.ToString();

                                if (stationResult.projectCheckResults.ContainsKey(childObjectGUID))
                                {
                                    projectResult = (ProjectCheckResult)stationResult.projectCheckResults[childObjectGUID];
                                }
                                else
                                {
                                    projectResult = new ProjectCheckResult();
                                    projectResult.parentProjectName = parentObjectName;
                                    projectResult.projectGUID = childObjectGUID;
                                    projectResult.projectName = childObjectName;
                                    stationResult.projectCheckResults.Add(childObjectGUID, projectResult);
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(checkResultContent))
                        {
                            checkResult = checkResult + checkResultTitle + checkResultContent;
                        }
                    }
                }
            }

            return checkResult;
        }

        private string GetAllStationObjectSql()
        {
            StringBuilder s = new StringBuilder();
            s.Append("SELECT  myStationObject.StationGUID ,");
            s.Append(" myStationObject.ObjectGUID ,");
            s.Append(" mystation.StationName ,");
            s.Append("  myBusinessUnit.BUName,");

                      s.Append("  myBusinessUnit.namepath,");
                      s.Append("   myBusinessUnit.HierarchyCode");

            s.Append(" FROM    dbo.myStationObject");
            s.Append(" INNER JOIN dbo.myStation ON dbo.myStationObject.StationGUID = dbo.myStation.StationGUID");
            s.Append(" INNER JOIN dbo.myBusinessUnit ON dbo.myStation.BUGUID = dbo.myBusinessUnit.BUGUID");
            s.Append("  ORDER BY HierarchyCode,namepath,StationName");
            return s.ToString();
        }

        private string GetAllDataRightsSql()
        {
            StringBuilder s = new StringBuilder();
            //s.Append("SELECT top 1000000 BUGUID AS _guid, BUName AS _name, OrderHierarchyCode AS _hierarchycode, '公司' AS _sourcetype");
            //s.Append(" ,[Level] AS _level,BUGUID AS _buguid,0 AS _isshare");
            //s.Append(" FROM myBusinessUnit");
            //s.Append(" WHERE BUType = 0");
            //s.Append(" UNION");
            s.Append(" SELECT top 1000000 p1.ProjGUID AS _guid,p1.ProjName AS _name,");
            s.Append(" case when p2.ProjShortCode is null then bu.OrderHierarchyCode + '.0' + p1.ProjShortCode else bu.OrderHierarchyCode + '.0' + p2.ProjShortCode + '.0' + p1.ProjShortCode end AS _hierarchycode");
            s.Append(" ,'项目' as _sourcetype,(bu.[Level]+p1.[Level]-1) AS _level,p1.BUGUID AS _buguid,0 AS _isshare");
            s.Append(" FROM p_Project p1");
            s.Append(" left join p_Project p2 ON p2.ProjCode=p1.ParentCode");
            s.Append(" INNER JOIN myBusinessUnit bu ON bu.BUGUID=p1.BUGUID");
            s.Append(" WHERE p1.ProjShortCode is not null");
            s.Append(" UNION");
            s.Append(" SELECT top 1000000 p1.ProjGUID AS _guid,p1.ProjName+'(共享)' AS _name,");
            s.Append(" case when p2.ProjShortCode is null then bu.OrderHierarchyCode + '.1' + p1.ProjShortCode else bu.OrderHierarchyCode + '.1' + p2.ProjShortCode + '.1' + p1.ProjShortCode end AS _hierarchycode");
            s.Append(" ,'项目' AS _sourcetype,(bu.[Level]+p1.[Level]-1) AS _level,kp.BUGUID AS _buguid,1 AS _isshare");
            s.Append(" FROM k_ProjectShare kp");
            s.Append(" INNER JOIN p_Project p1 ON p1.ProjGUID=kp.ProjGUID");
            s.Append(" left join p_Project p2 ON p2.ProjCode=p1.ParentCode");
            s.Append(" INNER JOIN myBusinessUnit bu ON bu.BUGUID=kp.BUGUID");
            s.Append(" WHERE p1.ProjShortCode is not null");
            s.Append(" ORDER BY _hierarchycode");
            return s.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DisableForm("导出中...");
            ExportToExcel();
            EnableForm();
        }
    }
}
