<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>无标题页</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="txt1" Text="1" runat="server"></asp:TextBox>到<asp:TextBox ID="txt2" Text="20" runat="server"></asp:TextBox>行 <asp:Button ID="btn1" runat="server" Text="获取数据" OnClick="btn1_Click" />
        <br /> 
        <asp:GridView ID="gv1" runat="server" AutoGenerateColumns="true"></asp:GridView>
    </div>
    </form>
</body>
</html>