using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Entity
{
    public class AppRequest
    {
        public string AppServiceName { get; set; }

        public string MethodName { get; set; }

        public CallUserInfo UserInfo { get; set; }

        public WebRequestInfo WebRequestInfo { get; set; }
        public object[] Parameters { get; set; }
    }
}