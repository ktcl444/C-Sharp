using System;
using System.Web.UI;

using Mysoft.Map.Application.SSO;

public partial class LoginList : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SSOServer.InitLoginList();
    }
}
