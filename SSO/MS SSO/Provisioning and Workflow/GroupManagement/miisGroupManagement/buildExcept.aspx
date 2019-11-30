<%@ Page Language="vb" AutoEventWireup="false" Codebehind="BuildExcept.aspx.vb" Inherits="gp.BuildExcept"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>Exception Builder</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript">
			function confirm_exceptions()
			{
			if (confirm("The max number of exceptions have been reached for this group.  In order to aviod this message, consider modifying the group clause to minimize exceptions.  Do you want to overide the max number and add another exception?")==true)
				return true;
			else
				return false;
			}
		</script>
	</HEAD>
	<body bgColor="steelblue" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:listbox id="lbInclude" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 40px" tabIndex="1"
				runat="server" Height="184" Width="281" Font-Names="Arial" SelectionMode="Multiple"></asp:listbox>
			<asp:Label id="lblError" style="Z-INDEX: 111; LEFT: 24px; POSITION: absolute; TOP: 40px" runat="server"
				Font-Names="Arial Black" Width="552px" Height="96px" ForeColor="LightGray"></asp:Label><asp:panel id="pnlExclude" style="Z-INDEX: 102; LEFT: 312px; POSITION: absolute; TOP: 8px"
				runat="server" Height="267px" Width="298px" Font-Names="Arial" BackColor="#000084" Font-Bold="True" ForeColor="White">&nbsp;Manually Excluded Members:</asp:panel>
			<asp:button id="btnAddExclude" style="Z-INDEX: 106; LEFT: 320px; POSITION: absolute; TOP: 232px"
				tabIndex="5" runat="server" Height="24px" Width="139px" Font-Names="Arial" Text="Add Exclusion"></asp:button><asp:listbox id="lbExclude" style="Z-INDEX: 104; LEFT: 320px; POSITION: absolute; TOP: 40px"
				tabIndex="4" runat="server" Height="184px" Width="281px" Font-Names="Arial" SelectionMode="Multiple"></asp:listbox><asp:button id="btnAddInclude" style="Z-INDEX: 105; LEFT: 16px; POSITION: absolute; TOP: 232px"
				tabIndex="2" runat="server" Height="24px" Width="139px" Font-Names="Arial" Text="Add Inclusion"></asp:button><asp:button id="btnDeleteInclude" style="Z-INDEX: 107; LEFT: 160px; POSITION: absolute; TOP: 232px"
				tabIndex="3" runat="server" Height="24" Width="139" Font-Names="Arial" Text="Delete Selected"></asp:button><asp:button id="btnDeleteExclude" style="Z-INDEX: 108; LEFT: 464px; POSITION: absolute; TOP: 232px"
				tabIndex="6" runat="server" Height="24" Width="139" Font-Names="Arial" Text="Delete Selected"></asp:button><asp:panel id="pnlInclude" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				Height="267px" Width="298px" Font-Names="Arial" BackColor="#000084" Font-Bold="True" ForeColor="White">&nbsp;Manually Included Members:</asp:panel>
			<asp:button id="btnClose" style="Z-INDEX: 109; LEFT: 548px; POSITION: absolute; TOP: 280px"
				tabIndex="7" runat="server" Height="24" Width="61" Font-Names="Arial" Text="Close"></asp:button>
			<asp:Label id="lblPreserveMember" style="Z-INDEX: 110; LEFT: 8px; POSITION: absolute; TOP: 284px"
				runat="server" Width="360px" Font-Names="Arial" ForeColor="LightGray" Font-Size="Smaller">Number of days to automatically preserve group membership:</asp:Label>
			<asp:TextBox id="txtPreserveMember" style="Z-INDEX: 112; LEFT: 368px; POSITION: absolute; TOP: 280px"
				runat="server" Width="24px" AutoPostBack="True"></asp:TextBox></form>
	</body>
</HTML>
