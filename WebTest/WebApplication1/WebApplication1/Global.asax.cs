using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.IO;

namespace WebApplication1
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            //String AppGirdExDir = Server.MapPath("/XmlTemp/AppGridEx/");
            //Application["AppGirdExDir"] = AppGirdExDir;
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            //Directory.Delete(Application["AppGirdExDir"] + Session.SessionID, true);
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}