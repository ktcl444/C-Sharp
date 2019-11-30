using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HanlderTest
{
    public class MaintenanceHanlder : IHttpHandler 
    {
        #region IHttpHandler 成员

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Redirect("Message.htm");
                //context.Response.Write("维护状态中,请稍后再访问.");
        }


        #endregion
    }
}
