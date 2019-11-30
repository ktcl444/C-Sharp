<%@ Register TagPrefix="uc1" TagName="IdentityViewerControl" Src="Template/IdentityViewerControl.ascx" %>
<%@ Page language="vb" Codebehind="Default.aspx.vb" AutoEventWireup="false" Inherits="AppDevASPIntranet.Contoso.AppDevASPSample._Default" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Default</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="Styles.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 104; LEFT: 18px; POSITION: absolute; TOP: 28px" cellSpacing="1"
				cellPadding="1" width="300" border="0">
				<TR>
					<TD><asp:label id="lblPageHeader" runat="server" CssClass="HeaderText">Employees</asp:label></TD>
					<TD></TD>
					<TD></TD>
				</TR>
				<TR>
					<TD><asp:label id="lblError" runat="server" CssClass="ErrorText" Width="576px"></asp:label></TD>
					<TD></TD>
					<TD></TD>
				</TR>
				<TR>
					<TD><asp:datagrid id="EmployeeGrid" runat="server" AutoGenerateColumns="False" CellPadding="3" BackColor="White"
							BorderWidth="1px" BorderStyle="None" BorderColor="#CCCCCC">
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
						</asp:datagrid></TD>
					<TD></TD>
					<TD><uc1:identityviewercontrol id="IdentityViewerControl1" runat="server"></uc1:identityviewercontrol></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
