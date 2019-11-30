using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient ;
using System.Web.Caching;
using System.Collections;

namespace SqlDependencyTest
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //SqlConnection.ClearAllPools();
            //TestChinese();
        }

        private void TestChinese()
        {
            string s = "陈勇";
           
                 System.Text.Encoding gb = System.Text.Encoding.GetEncoding("gb2312");
                 byte[] b = gb.GetBytes(s);
                 string s2 = gb.GetString(b, 0, b.Length);

        }

        private void InsertCache(TextBox sqlTextBox,TextBox resultTextBox)
        {
            string sql = sqlTextBox.Text.Trim();
            SqlCacheDependency scd ;
            DataSet ds = DBHelper.GetDataSet(sql, "Test", out scd);
            try
            {

                //CacheDependency[] sqlDependencys = new[] { null, scd };

                //AggregateCacheDependency aggregateCacheDependency = new AggregateCacheDependency();

                //aggregateCacheDependency.Add(sqlDependencys);


                 //HttpContext.Current.Cache.Insert(sql, ds);
                HttpContext.Current.Cache.Insert(sql, ds, scd, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, new CacheItemRemovedCallback(ItemRemovedCallback));
                //HttpContext.Current.Cache.Insert(sql, ds, scd);
                resultTextBox.Text = "缓存成功插入";
            }
            catch (Exception ex)
            {
                resultTextBox.Text = "缓存插入异常:" + ex.Message;
            }
        }

        protected void Button10_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Cache.Remove(this.TextBox1.Text.Trim());
        }


        private void ItemRemovedCallback(string key, object value, CacheItemRemovedReason reson)
        {
            string removeInfo = "CacheItemRemoved" + System.Environment.NewLine + "key : " + key + System.Environment.NewLine + "Reson :" + reson.ToString();
            this.txtRemoveReason.Text = removeInfo;
            //Response.Write(removeInfo);

        //    // 摘要:
        ////     该项是通过指定相同键的 System.Web.Caching.Cache.Insert(System.String,System.Object)
        ////     方法调用或 System.Web.Caching.Cache.Remove(System.String) 方法调用从缓存中移除的。
        //Removed = 1,
        ////
        //// 摘要:
        ////     从缓存移除该项的原因是它已过期。
        //Expired = 2,
        ////
        //// 摘要:
        ////     之所以从缓存中移除该项，是因为系统要通过移除该项来释放内存。
        //Underused = 3,
        ////
        //// 摘要:
        ////     从缓存移除该项的原因是与之关联的缓存依赖项已更改。
        //DependencyChanged = 4,
        }

        private void ShowCache(TextBox sqlTextBox,TextBox resultTextBox)
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



        protected void Button1_Click(object sender, EventArgs e)
        {
           InsertCache(this.TextBox1, this.TextBox5);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            ShowCache(this.TextBox1, this.TextBox5);
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            InsertCache(this.TextBox2, this.TextBox6);
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            ShowCache(this.TextBox2, this.TextBox6);
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            InsertCache(this.TextBox3, this.TextBox7);
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            ShowCache(this.TextBox3, this.TextBox7);
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
            InsertCache(this.TextBox4, this.TextBox8);
        }

        protected void Button8_Click(object sender, EventArgs e)
        {
            ShowCache(this.TextBox4, this.TextBox8);
        }

        protected void Button9_Click(object sender, EventArgs e)
        {
        SqlConnection conn = DBHelper.GetSqlConnection();
            SqlCommand cmd = new SqlCommand("xTestPtoc",conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameterCollection SqlParams = cmd.Parameters;
              SqlParams.Add(new  SqlParameter("@OutParam", SqlDbType.VarChar,int.MaxValue));
             SqlParams["@OutParam"].Direction = ParameterDirection.Output;

             if (conn.State == ConnectionState.Closed)
             {
                 conn.Open();
             }
                cmd.ExecuteNonQuery();
             if (SqlParams["@OutParam"] != null && SqlParams["@OutParam"].Value.ToString () != string.Empty )
            {
                int resultlength = SqlParams["@OutParam"].Value.ToString().Length ;
            }

        }



      

    }
}
