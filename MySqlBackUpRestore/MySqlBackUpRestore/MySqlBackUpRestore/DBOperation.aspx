<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DBOperation.aspx.cs" Inherits="MySqlBackUpRestore.DBOperation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="lblServerName" runat="server" Text="服务器"></asp:Label><asp:TextBox ID="txtServerName" runat="server">10.5.23.11</asp:TextBox>
            <br />
            <asp:Label ID="lblPort" runat="server" Text="端口号"></asp:Label><asp:TextBox ID="txtPort" runat="server">3306</asp:TextBox>
            <br />
            <asp:Label ID="lblUserName" runat="server" Text="用户名"></asp:Label><asp:TextBox ID="txtUserName" runat="server">sa</asp:TextBox>
            <br />
            <asp:Label ID="lblPassword" runat="server" Text="密码"></asp:Label><asp:TextBox ID="txtPassword" runat="server">95938</asp:TextBox>
            <br />
            <asp:Label ID="lblDataBaseName" runat="server" Text="备份数据库名称"></asp:Label><asp:TextBox ID="txtDataBaseName" runat="server">kongy_test</asp:TextBox>
            <asp:TextBox ID="txtDataBaseName0" runat="server">kongy_test2</asp:TextBox>
            <br />
            <asp:Button ID="btnBackUp" runat="server" Text="备份" OnClick="BtnBackUpClick" />
        </div>
        <div>
            <asp:Label ID="lblRestoreDataBaseName" runat="server" Text="还原数据库名称"></asp:Label><asp:TextBox ID="txtRestoreDataBaseName" runat="server">kongy_test_restore</asp:TextBox>
            <br />
            <asp:Button ID="btnRestore" runat="server" Text="还原" OnClick="BtnRestoreClick" />
        </div>
        <div>
            <asp:TextBox ID="txtResult" runat="server" TextMode="MultiLine" Width="300px"></asp:TextBox>
        </div>
    </form>
</body>
</html>
