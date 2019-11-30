using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;

namespace WindowsFormsApplication1
{
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            decimal second = Convert.ToDecimal(this.textBox1 .Text);
            timer1.Interval = Convert.ToInt16(1000 * second);
            timer1.Start();
            this.textBox2.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string AppPoolName = "DefaultAppPool";  //应用程序池的名称，如“DefaultAppPool”,默认应用程序池
            string method = "Recycle";  // 当method="Recycle"时就是回收，为“Start”时是启动，为“Stop”时是停止。

            try
            {
                DirectoryEntry appPool = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
                DirectoryEntry findPool = appPool.Children.Find(AppPoolName, "IIsApplicationPool");
                findPool.Invoke(method, null);
                appPool.CommitChanges();
                appPool.Close();
                this.textBox2.AppendText(string.Format("应用程序池{0}{1}成功", AppPoolName, method) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                this.textBox2.AppendText(string.Format("应用程序池{0}{1}失败", AppPoolName, method) + Environment.NewLine);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
    }
}
