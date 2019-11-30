using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyIISManager
{
    public class IISWebServerFactory
    {
        public static IIISWebServer GetIISWebServer(string machineName)
        {
            IISVersionEnum iisVersion = IISWebService.GetIISVersion(machineName);
           if (iisVersion == IISVersionEnum.IIS6)
           {
               return new IISWebServer();
           }
           else if (iisVersion == IISVersionEnum.IIS7)
           {
               return new IIS7WebServer();
           }
           return null;
        }
    }
}
