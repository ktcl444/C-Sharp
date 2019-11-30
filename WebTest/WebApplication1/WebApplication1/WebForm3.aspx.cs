using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;

namespace WebApplication1
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excel = null;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                workbook = excel.Workbooks.Open("c:\\test.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                excel.Visible = false;
            }
            catch (Exception ex)
            {
                this.TextBox1.Text = ex.Message;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            this.TextBox1.Text = this.Input1 .Value;
            string s = this.Select1.Items[this.Select1.SelectedIndex].Value ;
            this.TextArea.Value = "";
        }

        protected void Button2_Click(object sender, EventArgs e)
        {

        }
    }
}
