<%@ Control Language="vb" AutoEventWireup="false" Codebehind="IdentityViewerControl.ascx.vb" Inherits="AppDevASPExtranet.Contoso.Web.UI.WebControls.IdentityViewerControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<LINK href="../Styles.css" type="text/css" rel="stylesheet">
<TABLE id="Table1" style="WIDTH: 499px; HEIGHT: 138px" cellSpacing="1" cellPadding="1"
	width="499" border="1">
	<TR>
		<TD style="WIDTH: 139px" vAlign="top"><asp:label id="lblIdentityModelHeader" runat="server" CssClass="IdentityInfoText">Identity model:</asp:label></TD>
		<TD>
			<P><asp:label id="lblIdentityModel" runat="server" CssClass="IdentityInfoText"></asp:label></P>
			<P><asp:label id="lblChangeIdentityModel" runat="server" CssClass="IdentityModelInfoText"></asp:label><FONT size="1"></P>
			</FONT></TD>
	</TR>
	</FONT>
	<TR>
		<TD style="WIDTH: 139px"><asp:label id="lblAuthNModeHeader" runat="server" CssClass="IdentityInfoText">AuthN mode:</asp:label></TD>
		<TD><asp:label id="lblAuthNMode" runat="server" CssClass="IdentityInfoText"></asp:label></TD>
	</TR>
	<TR>
		<TD style="WIDTH: 139px"><asp:label id="lblHttpContextHeader" runat="server" CssClass="IdentityInfoText">HttpContext:</asp:label></TD>
		<TD><asp:label id="lblHttpContext" runat="server" CssClass="IdentityInfoText"></asp:label></TD>
	</TR>
	<TR>
		<TD style="WIDTH: 139px"><asp:label id="lblWindowsIdentityHeader" runat="server" CssClass="IdentityInfoText">WindowsIdentity:</asp:label></TD>
		<TD><asp:label id="lblWindowsIdentity" runat="server" CssClass="IdentityInfoText"></asp:label></TD>
	</TR>
	<TR>
		<TD style="WIDTH: 139px"><asp:label id="lblThreadHeader" runat="server" CssClass="IdentityInfoText">Thread:</asp:label></TD>
		<TD><asp:label id="lblThread" runat="server" CssClass="IdentityInfoText"></asp:label></TD>
	</TR>
</TABLE>
