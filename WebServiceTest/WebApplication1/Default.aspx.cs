using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication1.ServiceReference1;

namespace WebApplication1
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          //  Service1SoapClient service = new Service1SoapClient();
           // Response.Write(service.TestEnum(MessageLevel.Nomarl));

         //   Response.Write(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            TextBox1.Text = ConfigurationManager.ConnectionStrings["MYSCRM"].ToString();
        }
    }
}
