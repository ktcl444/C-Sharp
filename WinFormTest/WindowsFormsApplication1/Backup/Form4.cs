using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = this.textBox1.Text;
                if (File.Exists(filePath))
                {
                    //FileVersionInfo f = FileVersionInfo.GetVersionInfo(filePath);
                    //string realVersion = f.FileMajorPart + "." + f.FileMinorPart;
                    //this.textBox2.Text = realVersion;

                    string realVersion =  System.Reflection.Assembly.LoadFile(filePath).GetName().Version.ToString() ;
                    this.textBox2.Text = realVersion;

                }
                else
                {
                    this.textBox2.Text = "文件不存在";
                }
            }
            catch (Exception ex)
            {
                this.textBox2.Text = ex.Message;
            }



        }
    }
}
