using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class TimeStampTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //DateTime timeStamp = new DateTime(1970, 1, 1); //得到1970年的时间戳
            //long a = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000 -8 * 60 * 60;
          

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse("1357647015" + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
              this.TextBox2.Text = dtStart.Add(toNow).ToString("yyyy-MM-dd hh:mm:ss");


            this.TextBox1.Text = ConvertDateTimeInt(DateTime.Now).ToString();
        }

        private static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

    }
}