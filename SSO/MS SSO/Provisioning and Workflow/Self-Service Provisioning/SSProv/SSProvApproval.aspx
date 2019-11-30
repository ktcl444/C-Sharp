<%@ Page language="c#" Codebehind="SSProvApproval.aspx.cs" AutoEventWireup="false" Inherits="MIISWorkflow.ContractorApproval" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>ContractorApproval</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body MS_POSITIONING="GridLayout" bgColor="#ffffff">
		<asp:Label id="lblHeading" runat="server" Font-Names="Tahoma" Font-Bold="True" ForeColor="Purple">Contractor Approval</asp:Label>
		<TABLE id="Table1" style="FONT-SIZE: small; Z-INDEX: 105; LEFT: 80px; WIDTH: 500px; FONT-FAMILY: 'Tw Cen MT Condensed'; HEIGHT: 3px; BACKGROUND-COLOR: #e5ecf9"
			cellSpacing="0" cellPadding="0" width="300" border="1" borderColor="#ccccff">
			<TR>
				<TD style="WIDTH: 50%"></TD>
				<TD><a href="SSProvRequest.aspx">Contractor Request</a></TD>
				<TD><a href="SSProvStatus.aspx">Contractor Status</a></TD>
			</TR>
		</TABLE>
		<p>
			<asp:label id="lblTextWelcome" style="Z-INDEX: 101; LEFT: 32px" runat="server" Font-Size="Smaller"
				Font-Names="Tahoma">Welcome</asp:label>
			<asp:Label id="lblIDLabel" style="Z-INDEX: 115; LEFT: 88px" runat="server" Font-Names="Tahoma"
				Font-Size="Smaller" Font-Bold="True">Label</asp:Label>
		<p>
			<asp:label id="lblStatus" style="Z-INDEX: 102; LEFT: 32px" runat="server" Font-Size="Smaller"
				Font-Names="Tahoma"></asp:label>
			<form id="Form1" method="post" runat="server">
				<asp:datagrid id="dgContractorsGrid" runat="server" Font-Names="Tahoma" Font-Size="Smaller" AutoGenerateColumns="False"
					CellPadding="3" BackColor="White" BorderWidth="1px" BorderStyle="None" BorderColor="#E7E7FF">
					<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="#F7F7F7" BackColor="#738A9C"></SelectedItemStyle>
					<EditItemStyle Wrap="False"></EditItemStyle>
					<AlternatingItemStyle Wrap="False" BackColor="#F7F7F7"></AlternatingItemStyle>
					<ItemStyle Wrap="False" ForeColor="#4A3C8C" BackColor="#E7E7FF"></ItemStyle>
					<HeaderStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#4A3C8C"></HeaderStyle>
					<FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
					<Columns>
						<asp:BoundColumn DataField="CONTRACTOR_ID" HeaderText="ID"></asp:BoundColumn>
						<asp:BoundColumn DataField="FIRST_NAME" HeaderText="FIRST NAME"></asp:BoundColumn>
						<asp:BoundColumn DataField="LAST_NAME" HeaderText="LAST NAME"></asp:BoundColumn>
						<asp:BoundColumn DataField="DEPARTMENT" HeaderText="DEPARTMENT"></asp:BoundColumn>
						<asp:BoundColumn DataField="REQUESTER" HeaderText="REQUESTER"></asp:BoundColumn>
						<asp:BoundColumn DataField="LAST_MODIFIED" HeaderText="REQUEST DATE" DataFormatString="{0:MM/dd/yyyy h:mm tt}"></asp:BoundColumn>
						<asp:BoundColumn DataField="STATUS" HeaderText="STATUS"></asp:BoundColumn>
						<asp:TemplateColumn HeaderText="APPROVE/DENY">
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
							<ItemTemplate>
								<asp:RadioButtonList id="ApprovalResult" ForeColor="Purple" Font-Names="Tahoma" runat="server" Width="98px"
									Font-Size="X-Small" RepeatDirection="Horizontal">
									<asp:ListItem Value="Approve">Approve</asp:ListItem>
									<asp:ListItem Value="Deny">Deny</asp:ListItem>
								</asp:RadioButtonList>
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
					<PagerStyle HorizontalAlign="Right" ForeColor="#4A3C8C" BackColor="#E7E7FF" Mode="NumericPages"></PagerStyle>
				</asp:datagrid>
				<p>
					<asp:button id="btnSubmit" style="Z-INDEX: 101; LEFT: 32px" tabIndex="4" runat="server" Width="120px"
						Font-Names="Tahoma" Text="Submit Selections"></asp:button>
			</form>
			<asp:label id="lblTextResult" style="Z-INDEX: 101; LEFT: 32px" runat="server" Font-Size="Smaller"
				Font-Names="Tahoma"></asp:label>
		</p>
	</body>
</HTML>
