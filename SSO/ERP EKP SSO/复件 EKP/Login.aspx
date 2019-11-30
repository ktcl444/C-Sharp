<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<script language=javascript>
    window.onload = function() {
        //document.domain = "test.com.cn";
    }
</script>
    <title></title>
</head>
<body>
<iframe src="http://127.0.0.1:206/Login.aspx"></iframe>
    <form id="form1" runat="server">
    <div>
            <asp:Label ID="Label1" runat="server" Text="用户名:"></asp:Label>
        <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="密码:"></asp:Label>
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <asp:Button ID="btnLogin" runat="server" Text="登录" onclick="btnLogin_Click" />
    <input name="a" type="text" value=""/>
    </div>
    </form>
</body>
</html>
