using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Communication.AudioMixerHelper mic = new Communication.AudioMixerHelper();
            if (!mic.GetMute())
            {
                mic.SetMute(true);
            }
        }

        private void TestArrayList()
        {
            ArrayList al = new ArrayList();
            al.Add("1");
            if(al.Contains("1"))
            {
                al.Remove("1");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("222.22".Replace(",", ""));
            MessageBox.Show("".Replace(",", ""));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.tabControl1.TabPages.Remove(tabPage1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.tabControl1.TabPages.Insert  (0,tabPage1 );
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //string sql = textBox1.Text;
            //textBox1.Text = RemoveSqlComment(sql);
        }



        private static string RemoveSqlComment(string sql)
        {
            return System.Text.RegularExpressions.Regex.Replace
            (
              sql,
              @"(?ms)\[[^\]]*\]|'[^']*'|--.*?$|/\*(?>/\*(?<C>)|\*/(?<-C>)|(?!/\*|\*/).)*(?(C)(?!))\*/",
              delegate(System.Text.RegularExpressions.Match m)
              {
                  switch (m.Value[0])
                  {
                      case '-': return "";
                      case '/': return " ";
                      default: return m.Value;
                  }
              }
            );
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ArrayList a = new ArrayList();
            a.Add("AAA");
            a.Add("bbb");
            if (a.Contains("aaa"))
            {
                MessageBox.Show("aaa");
            }
            if (a.Contains("BBB"))
            {
                MessageBox.Show("BBB");
            }
        }
    }
}
