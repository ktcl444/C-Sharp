<%@ Page language="c#" Codebehind="SSProvRequest.aspx.cs" AutoEventWireup="false" Inherits="MIISWorkflow.ContractorRequest" validateRequest="false"%>
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
			Font-Names="Tahoma" runat="server">New Contractor Request</asp:label>
		<TABLE id="Table1" style="FONT-SIZE: small; Z-INDEX: 113; LEFT: 32px; WIDTH: 500px; FONT-FAMILY: 'Tw Cen MT Condensed'; HEIGHT: 3px; BACKGROUND-COLOR: #e5ecf9"
			borderColor="#ccccff" cellSpacing="0" cellPadding="0" width="300" border="1">
			<TR>
				<TD style="WIDTH: 50%"></TD>
				<TD><asp:hyperlink id="apvlnkContractorApproval" ForeColor="Purple" runat="server" NavigateUrl="SSProvApproval.aspx">Contractor Approval</asp:hyperlink><A id="ApprovalLink" href="SSProvApproval.aspx"></A></TD>
				<TD><A href="SSProvStatus.aspx">Contractor Status</A></TD>
			</TR>
		</TABLE>
		<p><asp:label id="lblTextWelcome" style="Z-INDEX: 101; LEFT: 32px" Font-Names="Tahoma" runat="server"
				Font-Size="Smaller">Welcome</asp:label><asp:label id="lblUserID" style="Z-INDEX: 115; LEFT: 88px" Font-Bold="True" Font-Names="Tahoma"
				runat="server" Font-Size="Smaller">Label</asp:label>
		<p><asp:label id="lblTextEnterNewInfo" style="Z-INDEX: 103; LEFT: 32px" Font-Names="Tahoma" runat="server"
				Font-Size="Smaller">Enter new contractor information :</asp:label>
		<p></p>
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table2" style="FONT-SIZE: smaller; FONT-FAMILY: tahoma; Font-Names: Tahoma"
				cellSpacing="0" cellPadding="0" width="300" border="0">
				<TR>
					<TD>First Name :</TD>
					<TD><asp:textbox id="txtFirstName" tabIndex="1" runat="server" MaxLength="30" BackColor="#EDEFFF"
							BorderStyle="Solid"></asp:textbox></TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 30px">Last Name:</TD>
					<TD style="HEIGHT: 30px"><asp:textbox id="txtLastName" tabIndex="2" runat="server" MaxLength="30" BackColor="#EDEFFF"
							BorderStyle="Solid"></asp:textbox></TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 23px">Start Date :</TD>
					<TD style="HEIGHT: 23px">
						<P><asp:textbox id="txtStartDate" tabIndex="6" runat="server" MaxLength="30" BackColor="#EDEFFF"
								BorderStyle="Solid"></asp:textbox><asp:button id="btnStartDate" runat="server" Text="..."></asp:button></P>
					</TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 23px">End Date :</TD>
					<TD style="HEIGHT: 23px">
						<P><asp:textbox id="txtEndDate" tabIndex="6" runat="server" MaxLength="30" BackColor="#EDEFFF" BorderStyle="Solid"></asp:textbox><asp:button id="btnEndDate" runat="server" Text="..."></asp:button></P>
					</TD>
				</TR>
			</TABLE>
			<P><asp:calendar id="cldrStartDate" runat="server"></asp:calendar><asp:calendar id="cldrEndDate" runat="server"></asp:calendar></P>
			<P><asp:button id="btnSubmit" style="Z-INDEX: 110; LEFT: 128px" tabIndex="5" Font-Names="Tahoma"
					runat="server" Text="Submit" Width="80px"></asp:button></P>
		</form>
		<P><asp:label id="lblTextResult" style="Z-INDEX: 101; LEFT: 32px" Font-Names="Tahoma" runat="server"
				Font-Size="Smaller"></asp:label></P>
		<P>&nbsp;</P>
	</body>
</HTML>
