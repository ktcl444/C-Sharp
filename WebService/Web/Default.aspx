<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Web.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
       <script src="/js/Ajax.js"></script>
      <script src="/js/utility.js"></script>
    <script>
        function Test() {
            var params = [];
            params[0] = "123";
            params[1] = "456";
            //alert(TTest());
            
            var pageParams = {};

            var ps = { serviceName: "Contract", methodName: "Save", customParams: params, queryString: pageParams };
            var response = Mysoft.call('Web.Proxy.ApplicationHandler', 'ProcessRequestService', ps);

            if (typeof (response) != 'undefined' && response.HasReturnValue) {
                alert(response.Result);
            }
        }
        
        function Show(message) {
            alert(message);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" Text="Button" OnClientClick="Test()"/>
    </div>
    </form>
</body>
</html>
