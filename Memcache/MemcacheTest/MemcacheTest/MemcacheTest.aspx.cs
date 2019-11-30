using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using DataAccess;
using System.Diagnostics;
using System.Xml;
using System.Data;

namespace MemcacheTest
{
    public partial class MemcacheTest : System.Web.UI.Page
    {
        private static MemcachedClient client = new MemcachedClient();
        private static SqlDbHelper sqlHelper;
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlHelper = new SqlDbHelper(this.txtDBConnection.Text);
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (client.Store (StoreMode.Set  , this.txtCacheKey.Text, this.txtCacheValue.Text))
            {
                ShowOperatorInfo(GetSecondsElapsed(sw.ElapsedMilliseconds),"单对象缓存成功！");
            }
            else
            {
                ShowOperatorInfo(GetSecondsElapsed(sw.ElapsedMilliseconds), "单对象缓存失败！");
            }            
        }

        protected void btnGet_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            object cacheValue = client.Get(this.txtCacheKey.Text);
            if (cacheValue != null)
            {
                this.txtGetCacheValue.Text = cacheValue.ToString();
                ShowOperatorInfo(GetSecondsElapsed(sw.ElapsedMilliseconds),"单对象缓存获取成功！");
            }
            else
            {
                this.txtGetCacheValue.Text = string.Empty;
                ShowOperatorInfo(GetSecondsElapsed(sw.ElapsedMilliseconds), "单对象缓存获取为空！");
            }
        }

        private decimal GetSecondsElapsed(long milliseconds)
        {
            decimal result = Convert.ToDecimal(milliseconds) / 1000m;
            return Math.Round(result, 6);
        }

        private void ShowOperatorInfo(decimal operatorTime)
        {
            ShowOperatorInfo(operatorTime,string.Empty );
        }

        private void ShowOperatorInfo(decimal operatorTime,string message)
        {
            string operInfo = string.Format("{1} 本次操作耗时 {0} 秒！", operatorTime.ToString(), message);
            if (string.IsNullOrEmpty(this.txtOperatorInfo.Text))
            {
                this.txtOperatorInfo.Text = operInfo;
            }
            else
            {
                this.txtOperatorInfo.Text = this.txtOperatorInfo.Text + Environment.NewLine + operInfo;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            client.FlushAll();
                ShowOperatorInfo(GetSecondsElapsed(sw.ElapsedMilliseconds),"清空缓存！");    
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            this.txtOperatorInfo.Text = string.Empty;
        }

        private XmlNode ReadXml(string filePath)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Server.MapPath(filePath));
            return xml.DocumentElement;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            int times = Convert.ToInt32(this.txtFileCacheTimes.Text);
            string filePath = this.txtFilePath.Text;
            client.Store(StoreMode.Set, filePath, ReadXml(filePath).InnerXml);
            string nodeXml = string.Empty;
            Stopwatch stopwatch2 = Stopwatch.StartNew();
            for (int j = 0; j < times; j++)
            {
                nodeXml = client.Get(filePath).ToString();
            }
            decimal secondsElapsed2 = GetSecondsElapsed(stopwatch2.ElapsedMilliseconds);
            ShowOperatorInfo(secondsElapsed2, string.Format("缓存读取文件 {0} 次！", times.ToString()));   
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            int times = Convert.ToInt32(this.txtFileCacheTimes.Text);
            XmlNode node = null;
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            for (int i = 0; i < times; i++)
            {
                node = ReadXml(this.txtFilePath.Text);
            }
            decimal secondsElapsed1 = GetSecondsElapsed(stopwatch1.ElapsedMilliseconds);
            ShowOperatorInfo(secondsElapsed1, string.Format("正常读取文件 {0} 次！", times.ToString()));          
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            int times = Convert.ToInt32(this.txtSQLCacheTimes.Text);
            string sql = this.txtSQL.Text;
            client.Store(StoreMode.Set, "CacheSQL",sqlHelper.ExecuteDataTable(sql));  
            DataTable dt = null;
            Stopwatch stopwatch2 = Stopwatch.StartNew();
            for (int j = 0; j < times; j++)
            {
                dt = client.Get<DataTable>("CacheSQL");
                //dt = (DataTable )client.Get("CacheSQL");
            }
            decimal secondsElapsed2 = GetSecondsElapsed(stopwatch2.ElapsedMilliseconds);
            ShowOperatorInfo(secondsElapsed2, string.Format("缓存读取SQL {0} 次，记录数 {1} 条！", times.ToString(), dt.Rows.Count.ToString()));   
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            string sql = this.txtSQL.Text;
            int times = Convert.ToInt32(this.txtSQLCacheTimes.Text);
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            DataTable dt = null;
            for (int i = 0; i < times; i++)
            {
                dt = sqlHelper.ExecuteDataTable(sql);
            }
            decimal secondsElapsed1 = GetSecondsElapsed(stopwatch1.ElapsedMilliseconds);
            ShowOperatorInfo(secondsElapsed1, string.Format("正常读取SQL {0} 次，记录数 {1} 条！", times.ToString(), dt.Rows.Count.ToString()));   
        }

        protected void Button8_Click(object sender, EventArgs e)
        {
            int cacheNumber = Convert.ToInt32(this.txtCacheNumber.Text);
            client.FlushAll();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            for (int i = 0; i < cacheNumber; i++)
            {
                client.Store(StoreMode.Set, i.ToString(), i.ToString());
            }
            decimal secondsElapsed1 = GetSecondsElapsed(stopwatch1.ElapsedMilliseconds);
            ShowOperatorInfo(secondsElapsed1, string.Format("缓存 {0} 个对象！", cacheNumber.ToString()));   
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            int cacheNumber = Convert.ToInt32(this.txtCacheNumber.Text);
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            client.Store(StoreMode.Set, (cacheNumber + 1).ToString(), (cacheNumber + 1).ToString());
            decimal secondsElapsed1 = GetSecondsElapsed(stopwatch1.ElapsedMilliseconds);
            ShowOperatorInfo(secondsElapsed1, string.Format("缓存中存在 {0} 个对象时，写入缓存！", cacheNumber.ToString()));   
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
            int cacheNumber = Convert.ToInt32(this.txtCacheNumber.Text);
            Random Rnd = new Random();
           int key = Rnd.Next(0, cacheNumber);
            Stopwatch stopwatch1 = Stopwatch.StartNew();
           string value = client.Get(key.ToString()).ToString();
            decimal secondsElapsed1 = GetSecondsElapsed(stopwatch1.ElapsedMilliseconds);
            ShowOperatorInfo(secondsElapsed1, string.Format("缓存中存在 {0} 个对象时，读取缓存！", cacheNumber.ToString()));  
        }

        protected void Button9_Click(object sender, EventArgs e)
        {
            int cacheNumber = Convert.ToInt32(this.txtCacheNumber.Text);
            Random Rnd = new Random();
            int key = Rnd.Next(0, cacheNumber);
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            string value = client.Remove(key.ToString()).ToString();
            decimal secondsElapsed1 = GetSecondsElapsed(stopwatch1.ElapsedMilliseconds);
            ShowOperatorInfo(secondsElapsed1, string.Format("缓存中存在 {0} 个对象时，删除缓存！", cacheNumber.ToString()));  
        }
    }
}