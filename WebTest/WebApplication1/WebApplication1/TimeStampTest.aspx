<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeStampTest.aspx.cs" Inherits="WebApplication1.TimeStampTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
        <asp:TextBox ID="TextBox1" runat="server">2014-09-29 09:17:00.000</asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
        <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>

    </form>
</body>
</html>
