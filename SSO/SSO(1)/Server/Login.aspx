﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>无标题页</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <table width="400px">
                <tr>
                    <td>
                        用户名：</td>
                    <td>
                        <asp:TextBox ID="txtUid" runat="server">jillzhang</asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        密码：</td>
                    <td>
                        <asp:TextBox ID="txtPwd" runat="server" TextMode="SingleLine">321456</asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="登录" /></td>
                </tr>
            </table>
    </div>
    </form>
</body>
</html>
