<%@ Page language="c#" Codebehind="Login.aspx.cs" AutoEventWireup="false" Inherits="AppTestCS.Login" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <HEAD>
        <title>Login</title>
        <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
        <meta name="CODE_LANGUAGE" Content="C#">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    </HEAD>
    <body MS_POSITIONING="FlowLayout">
        <form id="Form1" method="post" runat="server">
            <P>
                <asp:Label id="lblLogin" runat="server" Width="122px">Login ID</asp:Label><asp:TextBox id="txtLogin" runat="server" Width="150px"></asp:TextBox></P>
            <P>
                <asp:Button id="btnSignIn" runat="server" Text="Sign In"></asp:Button></P>
        </form>
    </body>
</HTML>