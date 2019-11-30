<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<iframe id="testIframe" src="http://10.5.22.144:205/Login.aspx" width="100%" style="display:none" ></iframe>

    <form id="form1" runat="server">
    <div>
            <asp:Label ID="Label1" runat="server" Text="用户名:"></asp:Label>
        <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="密码:"></asp:Label>
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <asp:Button ID="btnLogin" runat="server" Text="登录" onclick="btnLogin_Click" />
    <input type="button" value="添加" onclick="window.parent.document.all.form1.a.value='我是新添加的内容';">
    </div>
    </form>
</body>
</html>
