<%@ Control Language="c#" AutoEventWireup="false" Codebehind="LogonControl.ascx.cs" Inherits="Contoso.Web.UI.WebControls.LogonControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<LINK href="../Styles.css" type="text/css" rel="stylesheet">
<TABLE id="Table1" cellSpacing="1" cellPadding="1" width="300" border="0">
	<TR>
		<TD style="HEIGHT: 88px" vAlign="top">
			<asp:Label id="lblDomainNameHeader" Width="169px" CssClass="LabelText" runat="server">Domain name:</asp:Label></TD>
		<TD style="HEIGHT: 88px" vAlign="top" align="left">
			<P align="right">
				<asp:TextBox id="txtDomainName" Width="221px" runat="server"></asp:TextBox></P>
			<P align="right">
				<asp:Label id="lblDomainNameHelp" runat="server" CssClass="LabelHelpText" Width="215px" Height="36px">Label</asp:Label></P>
		</TD>
	</TR>
	<TR>
		<TD>
			<asp:Label id="lblUserNameHeader" Width="169px" CssClass="LabelText" runat="server">User name:</asp:Label></TD>
		<TD>
			<P align="right">
				<asp:TextBox id="txtUsername" Width="221px" runat="server"></asp:TextBox></P>
		</TD>
	</TR>
	<TR>
		<TD>
			<asp:Label id="lblPasswordHeader" Width="169px" CssClass="LabelText" runat="server">Password:</asp:Label></TD>
		<TD>
			<P align="right">
				<asp:TextBox id="txtPassword" Width="221px" runat="server" TextMode="Password"></asp:TextBox></P>
		</TD>
	</TR>
	<TR>
		<TD></TD>
		<TD>
			<P align="right">
				<asp:Button id="btnLogon" Width="83px" CssClass="ButtonText" runat="server" Text="Logon"></asp:Button></P>
		</TD>
	</TR>
	<TR>
		<TD colSpan="2">
			<asp:Label id="lblError" runat="server" CssClass="ErrorText" Width="467px"></asp:Label></TD>
	</TR>
</TABLE>
