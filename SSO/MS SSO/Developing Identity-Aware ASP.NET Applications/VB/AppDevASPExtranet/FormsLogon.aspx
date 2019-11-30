<%@ Register TagPrefix="uc1" TagName="LogonControl" Src="Template/LogonControl.ascx" %>
<%@ Page language="vb" Codebehind="FormsLogon.aspx.vb" AutoEventWireup="false" Inherits="AppDevASPExtranet.Contoso.AppDevASPSample.FormsLogon" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WebForm1</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="Styles.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 103; LEFT: 12px; POSITION: absolute; TOP: 23px" cellSpacing="1"
				cellPadding="1" width="300" border="0">
				<TR>
					<TD><asp:label id="lblPageHeader" runat="server" CssClass="HeaderText">ASP.NET Forms Auth</asp:label></TD>
				</TR>
				<TR>
					<TD><uc1:logoncontrol id="LogonControl1" runat="server"></uc1:logoncontrol></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
