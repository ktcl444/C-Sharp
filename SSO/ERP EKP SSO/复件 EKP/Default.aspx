<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <script language=javascript>

            function CallJSONPServer(url) {                                 // 调用JSONP服务器，url为请求服务器地址
           
                var oldScript = document.getElementById(url);       // 如果页面中注册了调用的服务器，则重新调用
                if (oldScript) {
                    oldScript.setAttribute("src", url);
                    return;
                }
                var script = document.createElement("script");       // 如果未注册该服务器，则注册并请求之
                script.setAttribute("type", "text/javascript");
                script.setAttribute("src", url);
                script.setAttribute("id", url);
                document.appendChild(script);
            }

            function Logout() {
                CallJSONPServer("http://erp.test.com.cn/Logout_Http.aspx");
                window.navigate("/Login.aspx?Logout=1");
            }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="EKP系统"></asp:Label>
        <asp:Button ID="Button1" runat="server" Text="登出" onclick="Button1_Click" Visible=false/>
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="http://erp.test.com.cn" Target=_blank>转到ERP</asp:HyperLink>
      <asp:Literal id="litScript" runat="server"></asp:Literal>
      <input type="button" onclick="Logout();" value="Logout"/>
    </div>
    </form>
</body>
</html>
