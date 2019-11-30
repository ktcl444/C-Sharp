using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using DataTable = System.Data.DataTable;

namespace ExcelTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var openFile = this.openFileDialog1;

            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == DialogResult.Cancel) return;
            var excelFilePath = openFile.FileName;


            var app = new Application();
            Sheets sheets;
            object oMissiong = System.Reflection.Missing.Value;
            Workbook workbook = null;
            DataTable dt = new DataTable();

            try
            {
                workbook = app.Workbooks.Open(excelFilePath, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong,
                    oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);
                sheets = workbook.Worksheets;

                //将数据读入到DataTable中
                Worksheet worksheet = (Worksheet)sheets.get_Item(1);//读取第一张表  
                if (worksheet == null) return;
                int iRowCount = worksheet.UsedRange.Rows.Count;
                var test = new ArrayList();
                var replaceColumns = this.txtReplaceColumns.Text.Split(',');
                for (int iRow = Convert.ToInt16(this.txtRow.Text); iRow <= iRowCount; iRow++)
                {
                    var param = new List<string>();
                    foreach (var replaceColumn in replaceColumns)
                    {

                        var columns = replaceColumn.Split('=');
                        var cell = (Range)worksheet.Cells[iRow, Convert.ToInt16(columns[0])];
                        var value = cell.Value2 == null ? "" : cell.Text.ToString();
                        if (columns.Length > 1 && !string.IsNullOrEmpty(columns[1]))
                        {
                            var values = value.Split(columns[1][0]);
                            param.AddRange(values);
                        }
                        else
                        {
                            param.Add(value);

                        }
                        ;
                    }

                    test.Add(setFormat(param));
                    //                    id = (Range)worksheet.Cells[iRow, 2];
                    //                    time = (Range)worksheet.Cells[iRow, 7];
                    //                    test.Add(setFormat((id.Value2 == null) ? "" : id.Text.ToString(), (time.Value2 == null) ? "" : time.Text.ToString()));
                }

                var result = test.Cast<object>().Aggregate(string.Empty, (current, s) => current + (s + Environment.NewLine));
                this.textBox3.Text = result;
            }
            catch (Exception ex) { this.textBox3.Text = ex.Message; }
            finally
            {
                workbook.Close(false, oMissiong, oMissiong);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                workbook = null;
                app.Workbooks.Close();
                app.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                app = null;
            }
        }

        private string setFormat(List<string> param)
        {
            var sql = "update xb_bill set begin_time = '{0}',end_time = '{1}' where id = '{2}'; ";
            return string.Format(sql, param.ToArray());
        }

        private string setFormat(string id, string time)
        {
            var sql = "update xb_bill set begin_time = '{0}',end_time = '{1}' where id = '{2}'; ";
            var timeRegion = time.Split('-');
            return string.Format(sql, timeRegion[0], timeRegion[1], id);
        }
    }
}
