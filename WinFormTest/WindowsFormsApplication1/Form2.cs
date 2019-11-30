using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label3.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
             int n;
             double  a, b, sum = 0;
             for(a = 1, b = 1, n = 3; n <= 1002; n++)
             {
              sum = a + b;
              a = b;
              b = sum;
             }
             MessageBox.Show(sum.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sourceSQL = txtSourceSQL.Text;
            txtNewSQL.Text = Regex.Replace(sourceSQL, @".*?FROM\b", "FROM", RegexOptions.Singleline);
            //txtOldSQL.Text = sourceSQL.Substring(sourceSQL.IndexOf("FROM "));
        
        }
    }
}
