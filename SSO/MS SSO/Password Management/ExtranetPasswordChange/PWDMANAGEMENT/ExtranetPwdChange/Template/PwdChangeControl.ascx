<%@ Control Language="c#" AutoEventWireup="false" Codebehind="PwdChangeControl.ascx.cs" Inherits="Contoso.Web.UI.WebControls.PwdChangeControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<LINK href="../Styles.css" type="text/css" rel="stylesheet">
<TABLE id="Table1" cellSpacing="1" cellPadding="1" width="464" border="0" style="WIDTH: 464px; HEIGHT: 256px">
	<TR>
		<TD style="WIDTH: 222px">
			<asp:Label id="lblDomainAndUserNameHeader" runat="server" CssClass="LabelText" Width="169px">Domain\User:</asp:Label></TD>
		<TD align="right">
			<asp:TextBox id="txtDomainAndUserName" runat="server" Width="221px" MaxLength="272"></asp:TextBox></TD>
	</TR>
	<TR>
		<TD style="WIDTH: 222px">
			<asp:Label id="lblOldPwdHeader" Width="169px" CssClass="LabelText" runat="server">Old password:</asp:Label></TD>
		<TD>
			<P align="right">
				<asp:TextBox id="txtOldPwd" Width="221px" runat="server" TextMode="Password" MaxLength="127"></asp:TextBox></P>
		</TD>
	</TR>
	<TR>
		<TD style="WIDTH: 222px">
			<asp:Label id="lblNewPwdHeader" Width="169px" CssClass="LabelText" runat="server">New password:</asp:Label></TD>
		<TD>
			<P align="right">
				<asp:TextBox id="txtNewPwd" Width="221px" runat="server" TextMode="Password" MaxLength="127"></asp:TextBox></P>
		</TD>
	</TR>
	<TR>
		<TD style="WIDTH: 222px">
			<asp:Label id="lblConfirmPwdHeader" runat="server" CssClass="LabelText" Width="192px">Confirm password:</asp:Label></TD>
		<TD align="right">
			<asp:TextBox id="txtConfirmPwd" runat="server" Width="221px" TextMode="Password" MaxLength="127"></asp:TextBox></TD>
	</TR>
	<TR>
		<TD style="WIDTH: 222px"></TD>
		<TD>
			<P align="right">
				<asp:Button id="btnChange" Width="83px" CssClass="ButtonText" runat="server" Text="Change"></asp:Button></P>
		</TD>
	</TR>
	<TR>
		<TD colSpan="2">
			<asp:Label id="lblError" runat="server" CssClass="ErrorText" Width="467px"></asp:Label></TD>
	</TR>
</TABLE>
