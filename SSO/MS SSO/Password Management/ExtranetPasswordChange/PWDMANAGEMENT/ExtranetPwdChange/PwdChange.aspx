<%@ Page language="c#" Codebehind="PwdChange.aspx.cs" AutoEventWireup="false" Inherits="Contoso.PwdManagement.PwdChange" validateRequest="false"%>
<%@ Register TagPrefix="uc1" TagName="PwdChangeControl" Src="Template/PwdChangeControl.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>PwdChangeNew</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="Styles.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 104; LEFT: 15px; POSITION: absolute; TOP: 34px" cellSpacing="1"
				cellPadding="1" width="300" border="0">
				<TR>
					<TD style="WIDTH: 312px" colSpan="2">
						<asp:Label id="lblPageHeader" runat="server" CssClass="HeaderText">Password change</asp:Label></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 312px" colSpan="2">&nbsp;
						<uc1:PwdChangeControl id="PwdChangeControl1" runat="server"></uc1:PwdChangeControl></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 312px" colSpan="2">
						<asp:Label id="lblError" runat="server" CssClass="ErrorText" Width="312px"></asp:Label></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
