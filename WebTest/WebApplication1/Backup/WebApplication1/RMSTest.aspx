<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RMSTest.aspx.cs" Inherits="WebApplication1.RMSTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
           <asp:TextBox ID="txtUserName" runat="server">kongy@mysoft.com.cn</asp:TextBox>
           <asp:TextBox ID="txtException" runat="server" Width="521px"></asp:TextBox>
    <p>1，原始模式</p>
        <asp:Button ID="Button1" runat="server" Text="RMS加密" onclick="Button1_Click" />
        <hr />
    <p>2，代码内身份模拟</p>
        <asp:Label ID="Label1" runat="server" Text="Label">模拟用户</asp:Label>
     <asp:TextBox ID="TextBox1" runat="server">mysoft\tfsbuild</asp:TextBox>
     <p />
        <asp:Label ID="Label2" runat="server" Text="Label">模拟用户密码</asp:Label>
     <asp:TextBox ID="TextBox2" runat="server">1qaz7410</asp:TextBox>
        <p />
        <asp:Label ID="Label3" runat="server" Text="Label">模拟域名</asp:Label>
     <asp:TextBox ID="TextBox3" runat="server">mysoft.com.cn</asp:TextBox>
         <p />
     <asp:Button ID="Button2" runat="server" Text="RMS加密" onclick="Button2_Click"/>
        <hr />
    <p>3，web.config身份模拟</p>    
    <asp:Button ID="Button3" runat="server" Text="RMS加密" onclick="Button3_Click"/>
    
           <hr />
    <p>0，清空文件</p>    
    <asp:Button ID="Button4" runat="server" Text="清空文件" onclick="Button4_Click"/>
    </div>
    </form>
</body>
</html>
