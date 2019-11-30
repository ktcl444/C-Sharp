using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        private FileStream fs;

        private void button1_Click(object sender, EventArgs e)
        {
            //初始化一个OpenFileDialog类 
            OpenFileDialog fileDialog = new OpenFileDialog();

            //判断用户是否正确的选择了文件 
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //获取用户选择文件的路径
                this.textBox1.Text = fileDialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filePath = this.textBox1.Text;
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("请选择一个文件");
            }
            else
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                MessageBox.Show("独占成功！");
            }
         
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (fs == null)
            {
                MessageBox.Show("文件未被独占！");
            }
            else
            {

                fs.Close();
                MessageBox.Show("解除成功！");
            }
        }
    }
}
