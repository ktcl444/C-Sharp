using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;

namespace CacheMemoryTest
{
    public partial class Default2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Test();
        }

        private void Test()
        {
            string path2 = "/ExceptionLog/ExceptionLog_2012-09-12 09 44 37.820_13f2122c-c806-43ca-892d-243166dc5a3f.log";
            DirectoryInfo di2 = new DirectoryInfo(HttpContext.Current.Server.MapPath(path2));
            string path = "/ExceptionLog/ExceptionLog_2012-09-12 09-44-37-820_13f2122c-c806-43ca-892d-243166dc5a3f.log";
            DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(path));
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string s = this.TextBox1.Text;
            this.TextBox2.Text = Convert.ToBase64String(Encoding.Default.GetBytes(s));
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            string s = this.TextBox2.Text;


            byte[] cc = Convert.FromBase64String(s);
            this.TextBox1.Text = Encoding.Default.GetString(cc);
        }
    }
}
