using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mysoft.Map.Excel;
using System.IO;

namespace WebApplication1
{
    public partial class RMSTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            txtException.Text = string.Empty;
            string oldFilePath = CreateNewExcel("RMSFile");
            string newFilePath = GetNewExcelPath("RMSFile");
            string userName = txtUserName.Text;
            try
            {                
                ExcelPermission.SetExcelPermission(oldFilePath, newFilePath, userName);
            }
            catch (Exception ex)
            {
                txtException.Text = ex.Message;
            }
        }

        private string CreateNewExcel(string dir)
        {
            //生成 Excel
            ExcelWookbook xls = new ExcelWookbook();
            ExcelWorksheet ws = null;
            xls.Worksheets.Add("Sheet1");
            ws = xls.Worksheets[0];

            ws.Cells[0, 0].Value = "test";
            string filePath = GetNewExcelPath(dir);
            xls.SaveXls(filePath);
            return filePath;
        }

        private string GetNewExcelPath(string dir)
        {
            string filePath = string.Format("{0}", dir);
            filePath = Path.Combine(HttpContext.Current.Server.MapPath(filePath), string.Format("{0}.xls", Guid.NewGuid().ToString()));
            return filePath;
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            txtException.Text = string.Empty;
            string oldFilePath = CreateNewExcel("RMSFile");
            string newFilePath = GetNewExcelPath("RMSFile");
            string userName = txtUserName.Text;
            try
            {
                ExcelPermission.SetExcelPermission(oldFilePath, newFilePath, userName, TextBox1.Text, TextBox2.Text, TextBox3.Text);
            }
            catch (Exception ex)
            {
                txtException.Text = ex.Message;
            }
        }
        protected void Button3_Click(object sender, EventArgs e)
        {
            txtException.Text = string.Empty;
            string oldFilePath = CreateNewExcel("RMSFile_Simulation");
            string newFilePath = GetNewExcelPath("RMSFile_Simulation");
            string userName = txtUserName.Text;
            try
            {
                ExcelPermission.SetExcelPermission(oldFilePath, newFilePath, userName);
            }
            catch (Exception ex)
            {
                txtException.Text = ex.Message;
            }
        }
        protected void Button4_Click(object sender, EventArgs e)
        {
            ClearDir("RMSFile");
            ClearDir("RMSFile_Simulation");
        }

        private void ClearDir(string dir)
        {
            DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(dir));

            if (di.Exists)
            {
                foreach (FileInfo fi in di.GetFiles())
                {
                    try
                    {
                        if (fi.IsReadOnly)
                        {
                            fi.IsReadOnly = false;
                        }
                        if (string.Compare(fi.Name, "web.config", true) != 0)
                        {
                            fi.Delete();
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }

        }
    }
}
