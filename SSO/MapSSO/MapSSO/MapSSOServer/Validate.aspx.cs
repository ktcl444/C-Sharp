using System;
using Mysoft.Map.Application.SSO;

public partial class Validate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SSOServer.ValidateServiceTicket();
    }
}
