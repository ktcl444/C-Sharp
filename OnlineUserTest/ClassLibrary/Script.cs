using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClassLibrary
{
    /// <summary>
    /// Script 生成执行xmlhttp的js脚本。
    /// </summary>
    public class Script : PlaceHolder
    {
        /// <summary>
        /// 设置js自动刷新的间隔时间，默认为25秒。
        /// </summary>
        public virtual int RefreshTime
        {
            get
            {
                object obj1 = this.ViewState["RefreshTime"];
                if (obj1 != null) { return int.Parse(((string)obj1).Trim()); }
                return 25;
            }
            set
            {
                this.ViewState["RefreshTime"] = value;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //从web.config中读取xmlhttp的访问地址
            string refreshUrl = (string)ConfigurationManager .AppSettings["refreshUrl"];
            string scriptString = @" <script language=""JavaScript"">" + writer.NewLine;
            scriptString += @"  window.attachEvent(""onload"", " + this.ClientID + @"_postRefresh);" + writer.NewLine;
            scriptString += @"  var " + this.ClientID + @"_xmlhttp=null;" + writer.NewLine;
            scriptString += @"  function " + this.ClientID + @"_postRefresh(){" + writer.NewLine;
            scriptString += @"   var " + this.ClientID + @"_xmlhttp = new ActiveXObject(""Msxml2.XMLHTTP"");" + writer.NewLine;
            scriptString += @"   " + this.ClientID + @"_xmlhttp.Open(""POST"", """ + refreshUrl + @""", false);" + writer.NewLine;
            scriptString += @"   " + this.ClientID + @"_xmlhttp.Send();" + writer.NewLine;
            scriptString += @"   var refreshStr= " + this.ClientID + @"_xmlhttp.responseText;" + writer.NewLine;

            scriptString += @"   try {" + writer.NewLine;
            scriptString += @"    var refreshStr2=refreshStr;" + writer.NewLine;
            //scriptString += @"    alert(refreshStr2);"+writer.NewLine;
            scriptString += @"   }" + writer.NewLine;
            scriptString += @"   catch(e) {}" + writer.NewLine;
            scriptString += @"   setTimeout(""" + this.ClientID + @"_postRefresh()""," + this.RefreshTime.ToString() + @"000);" + writer.NewLine;
            scriptString += @"  }" + writer.NewLine;
            scriptString += @"<";
            scriptString += @"/";
            scriptString += @"script>" + writer.NewLine;

            writer.Write(writer.NewLine);
            writer.Write(scriptString);
            writer.Write(writer.NewLine);
            base.Render(writer);
        }
    }



}
