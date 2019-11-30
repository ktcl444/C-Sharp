<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="SSOLab.SSOServer.WebApp.SignIn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="300">
            <tr>
                <td>
                    用户名
                </td>
                <td>
                    <asp:TextBox ID="txtUsername" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    密码
                </td>
                <td>
                    <asp:TextBox ID="txtPassword" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="btnSignIn" runat="server" OnClick="btnSignIn_Click" Text="登录" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
