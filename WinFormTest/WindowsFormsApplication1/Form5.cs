using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            decimal  second = Convert.ToDecimal (this.textBox3.Text );
            timer1.Interval =Convert.ToInt16( 1000 * second);
            timer1.Start();
            this.textBox2.Clear();
            this.textBox4.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string filePath = this.textBox1.Text;
            FileVersionInfo f = FileVersionInfo.GetVersionInfo(filePath);
            string realVersion = f.FileMajorPart + "." + f.FileMinorPart;
            this.textBox2.AppendText ( DateTime.Now.ToString()  + ":" +  realVersion + Environment.NewLine );
            if (f.FileMajorPart == 0 && f.FileMinorPart == 0)
            {
                this.textBox4.AppendText(DateTime.Now.ToString() + ":" + realVersion + Environment.NewLine);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox2.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.textBox4.Clear();
        }
    }
}
