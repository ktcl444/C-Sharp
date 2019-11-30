using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication1.Entity;

namespace Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var ws = new WebService1();
            var request = new AppRequest
                              {
                                  AppServiceName = "Contract",
                                  MethodName = "Save",
                                  Parameters = null,
                                  UserInfo =   new CallUserInfo( Guid.NewGuid(),"kongy"),
                                  WebRequestInfo = new WebRequestInfo {{"1", "1"}}
                              };
            try
            {

                var response = ws.Execute(ObjectSerialiaze.XmlSerial(request, typeof(AppRequest)));

                this.TextBox1.Text = response.ToString();
            }
            catch (SoapException ex)
            {
                var myException = ex;
                throw;
            }
        }
    }
}