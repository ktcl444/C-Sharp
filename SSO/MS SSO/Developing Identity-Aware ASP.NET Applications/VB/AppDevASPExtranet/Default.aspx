<%@ Page language="vb" Codebehind="Default.aspx.vb" AutoEventWireup="false" Inherits="AppDevASPExtranet.Contoso.AppDevASPSample._Default" %>
<%@ Register TagPrefix="uc1" TagName="IdentityViewerControl" Src="Template/IdentityViewerControl.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Default</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="Styles.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 104; LEFT: 15px; POSITION: absolute; TOP: 34px" cellSpacing="1"
				cellPadding="1" width="300" border="0">
				<TR>
					<TD style="WIDTH: 580px">
						<asp:Label id="lblPageHeader" runat="server" CssClass="HeaderText">Employees</asp:Label></TD>
					<TD style="WIDTH: 3px"></TD>
					<TD></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 580px">
						<asp:Label id="lblError" runat="server" CssClass="ErrorText" Width="576px"></asp:Label></TD>
					<TD style="WIDTH: 3px"></TD>
					<TD></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 580px">
						<asp:DataGrid id="EmployeeGrid" runat="server" CellPadding="3" BackColor="White" BorderWidth="1px"
							BorderStyle="None" BorderColor="#CCCCCC" AutoGenerateColumns="False">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#669999"></SelectedItemStyle>
							<ItemStyle ForeColor="#000066"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#006699"></HeaderStyle>
							<FooterStyle ForeColor="#000066" BackColor="White"></FooterStyle>
							<Columns>
								<asp:BoundColumn DataField="FirstName" HeaderText="First name"></asp:BoundColumn>
								<asp:BoundColumn DataField="LastName" HeaderText="Last name"></asp:BoundColumn>
								<asp:BoundColumn DataField="Title" HeaderText="Title"></asp:BoundColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Left" ForeColor="#000066" BackColor="White" Mode="NumericPages"></PagerStyle>
						</asp:DataGrid></TD>
					<TD style="WIDTH: 3px"></TD>
					<TD>
						<uc1:IdentityViewerControl id="IdentityViewerControl1" runat="server"></uc1:IdentityViewerControl></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
