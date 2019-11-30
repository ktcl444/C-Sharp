using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class frmExcelPermission : Form
    {
        public frmExcelPermission()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filePath = this.textBox1.Text;
            string newFilePath = this.textBox2.Text;
            string userName = this.textBox3.Text;
            try
            {
                ExcelPermission.SetExcelPermission(filePath, newFilePath, userName);
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }
        }
    }
}
