using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Caching;
using System.Collections;
using System.IO;

namespace CacheMemoryTest
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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

        protected void Button1_Click(object sender, EventArgs e)
        {
            string sql = TextBox1.Text.Trim();
            DataTable  dt = DBHelper.GetDataTable(sql, "SQLDependencyTestConnectionString3");
           
            try
            {
                HttpContext.Current.Cache.Insert(sql, dt);
                this.TextBox2.Text = GetSize(HttpContext.Current.Cache.Get(sql)).ToString();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            
        }

        private int GetSize(object Myobject)
        {
             MemoryStream Stream = new MemoryStream();
             LosFormatter FM = new LosFormatter();
            FM.Serialize(Stream, Myobject);
			byte[] O = Stream.ToArray();
			return O.Length ;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            string sql = TextBox3.Text.Trim();
           string s = DBHelper.GetDateItemString (sql, "SQLDependencyTestConnectionString3");

            try
            {
                HttpContext.Current.Cache.Insert(sql, s);
            
            }
            catch (Exception ex)
            {
                string s2 = ex.Message;
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            string sql = TextBox4.Text.Trim();
            try
            {
                DataTable dt = DBHelper.GetDataTable(sql, "SQLDependencyTestConnectionString4");
                HttpContext.Current.Cache.Insert(sql, dt, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, new CacheItemRemovedCallback(ItemRemovedCallback));
                this.TextBox5.Text = "插入成功";
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                this.TextBox5.Text = s;
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            string sql = TextBox6.Text.Trim();
            try
            {
                string st = DBHelper.GetDateItemString (sql, "SQLDependencyTestConnectionString3");
                int number = Convert.ToInt32(txtCacheNumber.Text.Trim());
                for (int i = 0; i < number; i++)
                {
                    HttpContext.Current.Cache.Insert(sql + i.ToString(), st,null,Cache.NoAbsoluteExpiration,Cache.NoSlidingExpiration,CacheItemPriority.High,null );
               
                }
               this.txtCacheNumberResult .Text = "插入成功";
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                this.txtCacheNumberResult.Text = s;
            }
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            string sql = TextBox7.Text.Trim();
            try
            {
                DataTable dt = DBHelper.GetDataTable(sql, "SQLDependencyTestConnectionString3");
                HttpContext.Current.Cache.Insert(sql , dt, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Low , null);

                this.TextBox8.Text = "插入成功";
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                this.TextBox8.Text = s;
            }
        }

        protected void btnGetCacheNumber_Click(object sender, EventArgs e)
        {
            TextBox9.Text = CacheHelper.GetNumber().ToString();
        }

        protected void btnGetCache_Click(object sender, EventArgs e)
        {
            string key = txtCacheKey.Text.Trim();
            if (HttpContext.Current.Cache.Get(key) == null)
            {
                txtGetCacheResult.Text = "缓存项不存在";
            }
            else 
            {

                txtGetCacheResult.Text = "缓存项健在";
            }

           
        }

    }
}
