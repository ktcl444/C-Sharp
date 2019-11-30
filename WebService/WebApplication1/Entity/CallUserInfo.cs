using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Entity
{
    public class CallUserInfo
    {
        public Guid UserGuid { get;  set; }
        public string UserName { get;  set; }

        public CallUserInfo(Guid userGuid, string userName)
        {
            this.UserGuid = userGuid;
            this.UserName = userName;
        }

        public CallUserInfo()
        {
            //UserGuid = Guid.Empty;
            //UserName = string.Empty;
        }
    }
}