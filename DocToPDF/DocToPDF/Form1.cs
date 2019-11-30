using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DocToPDF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        //DocToPDF2. ConvertWordToPdf(@"c:\test.doc", @"c:\test.pdf", true);
            MessageBox.Show(Test.DocToPDF(),"");
        }
    }
}
