<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="ERP系统"></asp:Label>
        <asp:Button ID="Button1" runat="server" Text="登出" onclick="Button1_Click" />
    
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="http://localhost:8052">转到EKP</asp:HyperLink>
    
    </div>
    </form>
</body>
</html>
