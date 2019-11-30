<%@ Page language="c#" Codebehind="SSProvStatus.aspx.cs" AutoEventWireup="false" Inherits="MIISWorkflow.ContractorStatus" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>ContractorStatus</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script type="text/javascript">
		function confirm_delete()
		{
		  if( confirm("Are you sure you want to terminate this contractor?")==true )
		    return true;
		  else
		    return false;
		}
		</script>
	</HEAD>
	<body bgColor="#ffffff" MS_POSITIONING="GridLayout">
		<asp:label id="lblHeading" style="Z-INDEX: 105; LEFT: 32px" runat="server" Font-Names="Tahoma"
			Font-Bold="True" ForeColor="Purple">Requested Contractors' Status</asp:label>
		<TABLE id="Table1" style="FONT-SIZE: small; Z-INDEX: 104; LEFT: 32px; WIDTH: 500px; FONT-FAMILY: 'Tw Cen MT Condensed'; HEIGHT: 3px; BACKGROUND-COLOR: #e5ecf9"
			borderColor="#ccccff" cellSpacing="0" cellPadding="0" width="300" border="1">
			<TR>
				<TD style="WIDTH: 50%"></TD>
				<TD><A href="SSProvRequest.aspx">Contractor Request</A></TD>
				<TD>
					<asp:HyperLink id="apvlnkSSProvApproval" ForeColor="Purple" runat="server" NavigateUrl="SSProvApproval.aspx">Contractor Approval</asp:HyperLink><A href="SSProvApproval.aspx"></A></TD>
			</TR>
		</TABLE>
		<p>
			<asp:label id="lblTextWelcome" style="Z-INDEX: 101; LEFT: 32px" runat="server" Font-Size="Smaller"
				Font-Names="Tahoma">Welcome</asp:label>
			<asp:Label id="lblUserID" style="Z-INDEX: 115; LEFT: 88px" runat="server" Font-Names="Tahoma"
				Font-Size="Smaller" Font-Bold="True">Label</asp:Label>
		<p><asp:label id="lblStatus" style="Z-INDEX: 101; LEFT: 32px" runat="server" Font-Names="Tahoma"
				Font-Size="Smaller"></asp:label>
			<form id="Form1" method="post" runat="server">
				<asp:datagrid id="dgContractorsGrid" style="Z-INDEX: 103; LEFT: 32px" runat="server" Font-Names="Tahoma"
					Font-Size="Smaller" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" BackColor="White"
					CellPadding="3" AutoGenerateColumns="False">
					<SelectedItemStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#738A9C"></SelectedItemStyle>
					<AlternatingItemStyle BackColor="#F7F7F7"></AlternatingItemStyle>
					<ItemStyle ForeColor="#4A3C8C" BackColor="#E7E7FF"></ItemStyle>
					<HeaderStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#4A3C8C"></HeaderStyle>
					<FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
					<Columns>
						<asp:BoundColumn DataField="CONTRACTOR_ID" HeaderText="ID"></asp:BoundColumn>
						<asp:BoundColumn DataField="FIRST_NAME" HeaderText="FIRST NAME"></asp:BoundColumn>
						<asp:BoundColumn DataField="LAST_NAME" HeaderText="LAST NAME"></asp:BoundColumn>
						<asp:BoundColumn DataField="DEPARTMENT" HeaderText="DEPARTMENT"></asp:BoundColumn>
						<asp:BoundColumn DataField="STATUS" HeaderText="STATUS"></asp:BoundColumn>
						<asp:BoundColumn DataField="LAST_MODIFIED" HeaderText="LAST MODIFIED" DataFormatString="{0:MM/dd/yyyy h:mm tt}"></asp:BoundColumn>
						<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="Update" CancelText="Cancel" EditText="Edit"></asp:EditCommandColumn>
						<asp:ButtonColumn Text="View history" CommandName="ViewHistory"></asp:ButtonColumn>
						<asp:TemplateColumn>
							<ItemTemplate>
								<asp:LinkButton id="Terminate" ForeColor="Purple" runat="server" CommandName="Terminate">Terminate</asp:LinkButton>
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
					<PagerStyle HorizontalAlign="Right" ForeColor="#4A3C8C" BackColor="#E7E7FF" Mode="NumericPages"></PagerStyle>
				</asp:datagrid></form>
			<asp:label id="lblTextResult" style="Z-INDEX: 101; LEFT: 32px" runat="server" Font-Names="Tahoma"
				Font-Size="Smaller"></asp:label></p>
	</body>
</HTML>
