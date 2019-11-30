<%@ Page language="c#" Codebehind="SSProvUpdate.aspx.cs" AutoEventWireup="false" Inherits="MIISWorkflow.ContractorUpdate" validateRequest="false"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Contractor Request</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body bgColor="white">
		<asp:label id="lblHeading" style="Z-INDEX: 114; LEFT: 32px" ForeColor="Purple" Font-Bold="True"
			Font-Names="Tahoma" runat="server"> Contractor Update</asp:label>
		<TABLE id="Table1" style="FONT-SIZE: small; Z-INDEX: 113; LEFT: 32px; WIDTH: 500px; FONT-FAMILY: 'Tw Cen MT Condensed'; HEIGHT: 3px; BACKGROUND-COLOR: #e5ecf9"
			borderColor="#ccccff" cellSpacing="0" cellPadding="0" width="300" border="1">
			<TR>
				<TD style="WIDTH: 50%"></TD>
				<TD>
					<asp:hyperlink id="apvlnkContractorApproval" ForeColor="Purple" runat="server" NavigateUrl="SSProvApproval.aspx">Contractor Approval</asp:hyperlink></TD>
				<TD><A href="SSprovStatus.aspx">Contractor Status</A></TD>
			</TR>
		</TABLE>
		<p><asp:label id="lblTextWelcome" style="Z-INDEX: 101; LEFT: 32px" Font-Names="Tahoma" runat="server"
				Font-Size="Smaller">Welcome</asp:label><asp:label id="lblUserID" style="Z-INDEX: 115; LEFT: 88px" Font-Bold="True" Font-Names="Tahoma"
				runat="server" Font-Size="Smaller">Label</asp:label>
		<p>
			<form id="Form1" method="post" runat="server">
				<asp:label id="lblTextEnterNewInfo" style="Z-INDEX: 103; LEFT: 32px" Font-Names="Tahoma" runat="server"
					Font-Size="Smaller">Enter new information for the contractor</asp:label><asp:label id="lblContractorID" Font-Bold="True" Font-Names="Tahoma" runat="server" Font-Size="Smaller"
					Width="56px">Label</asp:label>
				<p></p>
				<TABLE id="Table2" style="FONT-SIZE: smaller; FONT-FAMILY: tahoma; Font-Names: Tahoma"
					cellSpacing="0" cellPadding="0" width="300" border="0">
					<TR>
						<TD>First Name :</TD>
						<TD><asp:textbox id="txtFirstName" tabIndex="1" runat="server" MaxLength="30" EnableViewState="true"
								BackColor="#EDEFFF" BorderStyle="Solid"></asp:textbox></TD>
					</TR>
					<TR>
						<TD>Last Name:</TD>
						<TD><asp:textbox id="txtLastName" tabIndex="2" runat="server" MaxLength="30" BackColor="#EDEFFF"
								BorderStyle="Solid"></asp:textbox></TD>
					</TR>
					<TR>
						<TD>Department :</TD>
						<TD><asp:textbox id="txtDepartment" tabIndex="4" runat="server" MaxLength="30" BackColor="#EDEFFF"
								BorderStyle="Solid"></asp:textbox></TD>
					</TR>
				</TABLE>
				<P></P>
				<asp:button id="btnSubmit" style="Z-INDEX: 110; LEFT: 128px" tabIndex="5" Font-Names="Tahoma"
					runat="server" Width="80px" Text="Submit"></asp:button></form>
			<asp:label id="lblTextResult" style="Z-INDEX: 101; LEFT: 32px" Font-Names="Tahoma" runat="server"
				Font-Size="Smaller"></asp:label>
	</body>
</HTML>
