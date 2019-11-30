using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
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
            if (CheckMaintenanceState())
            {
                context.Response.Write("维护状态中,请稍后再访问.");
            }
        }

        #endregion

        private bool CheckMaintenanceState()
        {
            if (HttpContext.Current.Cache.Get("RunningState") != null && HttpContext.Current.Cache.Get("RunningState").ToString() == "Maintenance")
            {
                return true;
            }
            return false;
        }
    }
}
