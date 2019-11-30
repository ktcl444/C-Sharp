<%@ Page language="c#" Codebehind="SSProvHistory.aspx.cs" AutoEventWireup="false" Inherits="MIISWorkflow.ContractorHistory" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>ContractorHistory</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body MS_POSITIONING="GridLayout" bgColor="#ffffff">
		<asp:Label id="lblHeading" style="Z-INDEX: 101; LEFT: 32px" runat="server" Font-Bold="True"
			Font-Names="Tahoma" ForeColor="Purple">Contractor History</asp:Label>
		<TABLE id="Table1" style="FONT-SIZE: small; Z-INDEX: 100; LEFT: 32px; WIDTH: 500px; FONT-FAMILY: 'Tw Cen MT Condensed'; HEIGHT: 3px; BACKGROUND-COLOR: #e5ecf9"
			cellSpacing="0" cellPadding="0" width="300" border="1" borderColor="#ccccff">
			<TR>
				<TD style="WIDTH: 75%"></TD>
				<TD><a href="SSProvStatus.aspx">Contractor Status</a></TD>
			</TR>
		</TABLE>
		<p>
			<asp:label id="lblTextWelcome" style="Z-INDEX: 101; LEFT: 32px" runat="server" Font-Size="Smaller"
				Font-Names="Tahoma">Welcome</asp:label>
			<asp:Label id="lblUserID" style="Z-INDEX: 115; LEFT: 88px" runat="server" Font-Names="Tahoma"
				Font-Size="Smaller" Font-Bold="True">Label</asp:Label>
		<p>
			<asp:label id="lblStatus" style="Z-INDEX: 105; LEFT: 32px" runat="server" Font-Names="Tahoma"
				Font-Size="Smaller"></asp:label></p>
		<asp:datagrid id="dgHistoryGrid" style="Z-INDEX: 101; LEFT: 8px" Font-Names="Tahoma" runat="server"
			Font-Size="Smaller" AutoGenerateColumns="False" CellPadding="3" BackColor="White" BorderWidth="1px"
			BorderStyle="None" BorderColor="#E7E7FF" EnableViewState="False">
			<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="#F7F7F7" BackColor="#738A9C"></SelectedItemStyle>
			<EditItemStyle Wrap="False"></EditItemStyle>
			<AlternatingItemStyle Wrap="False" BackColor="#F7F7F7"></AlternatingItemStyle>
			<ItemStyle Wrap="False" ForeColor="#4A3C8C" BackColor="#E7E7FF"></ItemStyle>
			<HeaderStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#4A3C8C"></HeaderStyle>
			<FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
			<Columns>
				<asp:BoundColumn DataField="LAST_MODIFIED" HeaderText="DATE &amp; TIME" DataFormatString="{0:MM/dd/yyyy h:mm tt}"></asp:BoundColumn>
				<asp:BoundColumn DataField="FIRST_NAME" HeaderText="FIRST NAME"></asp:BoundColumn>
				<asp:BoundColumn DataField="LAST_NAME" HeaderText="LAST NAME"></asp:BoundColumn>
				<asp:BoundColumn DataField="DEPARTMENT" HeaderText="DEPARTMENT"></asp:BoundColumn>
				<asp:BoundColumn DataField="REQUESTER" HeaderText="REQUESTER"></asp:BoundColumn>
				<asp:BoundColumn DataField="STATUS" HeaderText="STATUS"></asp:BoundColumn>
			</Columns>
			<PagerStyle HorizontalAlign="Right" ForeColor="#4A3C8C" BackColor="#E7E7FF" Mode="NumericPages"></PagerStyle>
		</asp:datagrid>
	</body>
</HTML>
