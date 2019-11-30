<%@ Control Language="c#" AutoEventWireup="false" Codebehind="IdentityViewerControl.ascx.cs" Inherits="Contoso.Web.UI.WebControls.IdentityViewerControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<LINK href="../Styles.css" type="text/css" rel="stylesheet">
<TABLE id="Table1" style="WIDTH: 499px; HEIGHT: 138px" cellSpacing="1" cellPadding="1"
	width="499" border="1">
	<TR>
		<TD style="WIDTH: 139px" vAlign="top">
			<asp:Label id="lblIdentityModelHeader" CssClass="IdentityInfoText" runat="server">Identity model:</asp:Label></TD>
		<TD>
			<P>
				<asp:Label id="lblIdentityModel" CssClass="IdentityInfoText" runat="server"></asp:Label></P>
			<P>
				<asp:Label id="lblChangeIdentityModel" runat="server" CssClass="IdentityModelInfoText"></asp:Label><FONT size="1"></P>
		</TD>
	</TR>
	</FONT>
	<TR>
		<TD style="WIDTH: 139px">
			<asp:Label id="lblAuthNModeHeader" CssClass="IdentityInfoText" runat="server">AuthN mode:</asp:Label></TD>
		<TD>
			<asp:Label id="lblAuthNMode" CssClass="IdentityInfoText" runat="server"></asp:Label></TD>
	</TR>
	<TR>
		<TD style="WIDTH: 139px">
			<asp:Label id="lblHttpContextHeader" CssClass="IdentityInfoText" runat="server">HttpContext:</asp:Label></TD>
		<TD>
			<asp:Label id="lblHttpContext" CssClass="IdentityInfoText" runat="server"></asp:Label></TD>
	</TR>
	<TR>
		<TD style="WIDTH: 139px">
			<asp:Label id="lblWindowsIdentityHeader" CssClass="IdentityInfoText" runat="server">WindowsIdentity:</asp:Label></TD>
		<TD>
			<asp:Label id="lblWindowsIdentity" CssClass="IdentityInfoText" runat="server"></asp:Label></TD>
	</TR>
	<TR>
		<TD style="WIDTH: 139px">
			<asp:Label id="lblThreadHeader" CssClass="IdentityInfoText" runat="server">Thread:</asp:Label></TD>
		<TD>
			<asp:Label id="lblThread" CssClass="IdentityInfoText" runat="server"></asp:Label></TD>
	</TR>
</TABLE>
