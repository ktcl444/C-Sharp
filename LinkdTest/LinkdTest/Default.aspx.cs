using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkdTest
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string path = Server.MapPath("/");
            string filePath = System.IO.Path.Combine(path, "UpFiles\\1.txt");
            if (System.IO.File.Exists(filePath))
            {
                this.TextBox1.Text = "找到文件";
            }
            else
            {
                this.TextBox1.Text = "找不到文件";
            }
        }
    }
}
