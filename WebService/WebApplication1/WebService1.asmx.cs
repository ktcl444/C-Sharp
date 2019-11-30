using System.Data;
using System.Web.Services;
using System.Xml.Serialization;
using WebApplication1.Entity;

namespace WebApplication1
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod, XmlInclude(typeof(AppResponse))]
        public DataSet Execute(byte[] request)
        {
            var appRequest = ObjectSerialiaze.XmlDeSerial(request,typeof(AppRequest));
            return new AppService().GetCustomDataTable();
        }
    }
}
