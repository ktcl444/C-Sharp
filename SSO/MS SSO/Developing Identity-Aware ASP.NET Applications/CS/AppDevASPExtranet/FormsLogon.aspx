<%@ Register TagPrefix="uc1" TagName="LogonControl" Src="Template/LogonControl.ascx" %>
<%@ Page language="c#" Codebehind="FormsLogon.aspx.cs" AutoEventWireup="false" Inherits="Contoso.AppDevASPSample.FormsLogon" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WebForm1</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="Styles.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 103; LEFT: 12px; POSITION: absolute; TOP: 23px" cellSpacing="1"
				cellPadding="1" width="300" border="0">
				<TR>
					<TD>
						<asp:Label id="lblPageHeader" runat="server" CssClass="HeaderText">ASP.NET Forms Auth</asp:Label></TD>
				</TR>
				<TR>
					<TD>
						<uc1:LogonControl id="LogonControl1" runat="server"></uc1:LogonControl></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
