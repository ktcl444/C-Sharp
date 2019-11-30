<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="AppLauncherCS._default" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <HEAD>
        <title>Application Launcher</title>
        <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
        <meta name="CODE_LANGUAGE" Content="C#">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    </HEAD>
    <body MS_POSITIONING="FlowLayout">
        <form id="Form1" method="post" runat="server">
            <P>
                <asp:Label id="Label1" runat="server" Font-Bold="True" Font-Size="Large">Application Launcher</asp:Label></P>
            <P>
                <asp:Label id="lblLogin" runat="server"></asp:Label></P>
            <P>
                <asp:DataGrid id="grdApps" runat="server" BorderColor="#999999" BorderStyle="None" BorderWidth="1px"
                    BackColor="White" CellPadding="3" GridLines="Vertical" AutoGenerateColumns="False">
                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                    <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
                    <FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
                    <Columns>
                        <asp:TemplateColumn SortExpression="sAppName" HeaderText="Application">
                            <ItemTemplate>
                                <asp:LinkButton id=lnkApp runat="server" UserID='<%# DataBinder.Eval(Container.DataItem, "iUserID") %>' AppID='<%# DataBinder.Eval(Container.DataItem, "iAppID") %>' Text='<%# DataBinder.Eval(Container.DataItem, "sAppName") %>' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "sURL") %>'>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="sDesc" HeaderText="Description"></asp:BoundColumn>
                    </Columns>
                    <PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
                </asp:DataGrid></P>
            <P>
                <asp:Label id="lblMessage" runat="server"></asp:Label></P>
        </form>
    </body>
</HTML>
