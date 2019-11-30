using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using ClassLibrary1;

namespace WebService1
{
    /// <summary>
    /// Service1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string TestEnum(EnumHelper.MessageLevel messageLevel)
        {
            if (messageLevel == EnumHelper.MessageLevel.Nomarl)
            {
                return "Normal";
            }
            else if (messageLevel == EnumHelper.MessageLevel.Super)
            {
                return "Super";
            }
            else
            {
                return "undefiend";
            }
        }
    }
}
