using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SearchJSFloat.Lib;

namespace WebApplication1
{
    public partial class Pub_XmlHttp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string returnValue = string.Empty;
            string businessInfo = Request.QueryString["p_businessInfo"];
            string businessType = Request.QueryString["p_businessType"];

            switch (businessType)
            {
                case "returnEvalString":
                    returnValue = ReturnEvalString(businessInfo);
                    break;
                case "test":
                    returnValue = "test";
                    break;
                case "test2":
                    returnValue = "test2";
                    break;
                default:
                    break;
            }

            this.Response.ContentType = "text/HTML";
            this.Response.Clear();
            this.Response.Write(returnValue);
            this.Response.End();
        }

        private string ReturnEvalString(string s)
        {
            string s2 = s.Replace ("pp", "+");
            ExpParser parser = new ExpParser(s2);
            return parser.GetExpValue();
        }
    }
}
