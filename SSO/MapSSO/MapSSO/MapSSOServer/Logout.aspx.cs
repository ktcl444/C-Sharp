using System;
using System.Web.UI;

using Mysoft.Map.Application.SSO;

public partial class Logout :Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SSOServer.Logout();
    }
}
