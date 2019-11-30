using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ByteKBMBConvert
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string sByte = textBox1.Text.Trim();
                if (sByte.Length > 0)
                {
                    double iByte = Convert.ToDouble (sByte);
                    double iKb = iByte / 1024;
                    textBox2.Text = System.Math.Round(iKb, 2).ToString();
                }
                else
                {
                    textBox2.Text = "请输入数值";
                }
            }
            catch (Exception ex)
            {

                textBox2.Text = ex.Message;
            }
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string sByte = textBox1.Text.Trim();
                if (sByte.Length > 0)
                {
                    double iByte = Convert.ToDouble(sByte);
                    double iMb = iByte / (1024 * 1024);
                    textBox3.Text =System.Math.Round( iMb,2).ToString();
                }
                else
                {
                    textBox3.Text = "请输入数值";
                }
            }
            catch (Exception ex)
            {

                textBox3.Text = ex.Message;
            }
        }
    }
}
