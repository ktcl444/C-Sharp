using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Security.Policy;

namespace WindowsFormsApplication1
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                IPHostEntry h = Dns.GetHostByName(this.textBox1.Text);
                IPAddress[] ips = h.AddressList;

                this.textBox2.Text = ips[0].ToString();
            }
            catch (Exception ex)
            {
                this.textBox2.Text = ex.ToString();
            }



        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                IPHostEntry h = Dns.GetHostEntry(this.textBox1.Text);
                IPAddress[] ips = h.AddressList;


                this.textBox3.Text = ips[0].ToString();
            }
            catch (Exception ex)
            {

                this.textBox3.Text = ex.ToString();
            }
 
        }
    }
}
