using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SqlDependencyTest
{
    public partial class Default2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            InseerCache(this.Textbox1, this.TextBox9, new TextBox[] { this.Textbox10});
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            ShowCache(this.Textbox1, this.TextBox9);
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            InseerCache(this.Textbox7, this.TextBox8,new TextBox[] { this.Textbox2, this.Textbox3 });
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            ShowCache(this.Textbox7, this.TextBox8);
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            InseerCache(this.Textbox4, this.TextBox12, new TextBox[] { this.Textbox5, this.Textbox6 });
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            ShowCache(this.Textbox4, this.TextBox12);
        }

        private void InseerCache(TextBox sqlTextBox, TextBox resultTextBox,TextBox[] dependencyTextBoxs)
        {
            string sql = sqlTextBox.Text.Trim();
            string[] dependencySqls = new string[dependencyTextBoxs.Length];
            for (int i = 0; i < dependencyTextBoxs.Length; i++)
            {
                dependencySqls[i] = dependencyTextBoxs[i].Text.Trim();
            }
            DataSet ds = DBHelper.GetDataSet(sql, dependencySqls, "SQLDependencyTestConnectionString2");
           resultTextBox.Text = "缓存插入成功";
        }

        private void ShowCache(TextBox sqlTextBox, TextBox resultTextBox)
        {
            string sql = sqlTextBox.Text.Trim();
            if (HttpContext.Current.Cache.Get(sql) != null)
            {
                resultTextBox.Text = "缓存依然健在";
            }
            else
            {
                resultTextBox.Text = "缓存已被干掉";
            }
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Cache.Remove(this.Textbox1.Text.Trim());
        }
    }
}
