<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddSite.aspx.cs" Inherits="AddSite" %>

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
                        站点编号：</td>
                    <td>
                        <asp:TextBox ID="txtSiteID" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        安全级别：</td>
                    <td>
                        <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow">
                            <asp:ListItem Selected="True">明文</asp:ListItem>
                            <asp:ListItem>密文</asp:ListItem>
                        </asp:RadioButtonList></td>
                </tr>
            <tr>
                <td>
                    来源字段：</td>
                <td>
                    <asp:TextBox ID="TextBox1" runat="server">fromurl</asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    密钥字段：</td>
                <td>
                    <asp:TextBox ID="TextBox2" runat="server">key</asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    IV字段：</td>
                <td>
                    <asp:TextBox ID="TextBox3" runat="server">iv</asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    默认页地址：</td>
                <td>
                    <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="height: 26px">
                    Uid字段：</td>
                <td style="height: 26px">
                    <asp:TextBox ID="TextBox5" runat="server">uid</asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    登出地址:</td>
                <td>
                    <asp:TextBox ID="TextBox7" runat="server"></asp:TextBox></td>
            </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="注册" />
                        <asp:TextBox ID="TextBox6" runat="server" Height="163px" TextMode="MultiLine" Width="230px"></asp:TextBox></td>
                </tr>
            </table>
    </div>
    </form>
</body>
</html>
