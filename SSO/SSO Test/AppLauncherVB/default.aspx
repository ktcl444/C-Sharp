<%@ Page Language="vb" AutoEventWireup="false" Codebehind="default.aspx.vb" Inherits="AppLauncherVB._default"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
    <HEAD>
        <title>default</title>
        <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
        <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    </HEAD>
    <body MS_POSITIONING="FlowLayout">
        <FORM id="Form1" method="post" runat="server">
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
                                <asp:LinkButton id=lnkApp runat="server" AppID='<%# DataBinder.Eval(Container.DataItem, "iAppID") %>' UserID='<%# DataBinder.Eval(Container.DataItem, "iUserID") %>' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "sURL") %>' Text='<%# DataBinder.Eval(Container.DataItem, "sAppName") %>'>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="sDesc" HeaderText="Description"></asp:BoundColumn>
                    </Columns>
                    <PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
                </asp:DataGrid></P>
            <P>
                <asp:Label id="lblMessage" runat="server"></asp:Label></P>
        </FORM>
    </body>
</HTML>
