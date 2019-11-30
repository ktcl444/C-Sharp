<%@ Page Language="vb" AutoEventWireup="false" Codebehind="SelectExcept.aspx.vb" Inherits="gp.SelectException"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>Select Exception</title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body MS_POSITIONING="GridLayout" bgColor="steelblue">
		<form id="Form1" method="post" runat="server">
			<asp:Panel id="pnlSelect" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 56px" runat="server"
				ForeColor="White" Font-Bold="True" Font-Names="Arial" BackColor="#000084" Width="496px"
				Height="226px">&nbsp;Select Member:</asp:Panel>
			<asp:Label id="lblError" style="Z-INDEX: 110; LEFT: 24px; POSITION: absolute; TOP: 176px" runat="server"
				Height="96px" Width="464px" Font-Names="Arial Black" ForeColor="LightGray"></asp:Label>
			<asp:dropdownlist id="ddlSearchAttribute" style="Z-INDEX: 109; LEFT: 7px; POSITION: absolute; TOP: 23px"
				runat="server" Width="161" Height="22" AutoPostBack="True" tabIndex="1" Font-Names="Arial"></asp:dropdownlist>
			<asp:label id="lblSearch" style="Z-INDEX: 106; LEFT: 8px; POSITION: absolute; TOP: 0px" runat="server"
				ForeColor="LightGray" Font-Bold="True" Font-Names="Arial" Width="184px" Height="22px">Specify Search Criteria:</asp:label>
			<asp:dropdownlist id="ddlSearchOperation" style="Z-INDEX: 105; LEFT: 176px; POSITION: absolute; TOP: 24px"
				runat="server" BackColor="LightGray" Width="161px" Height="22px" AutoPostBack="True" Enabled="False"
				tabIndex="2" Font-Names="Arial"></asp:dropdownlist>
			<asp:textbox id="txtSearchValue" style="Z-INDEX: 104; LEFT: 344px; POSITION: absolute; TOP: 24px"
				runat="server" BackColor="LightGray" Width="161px" Height="22px" Enabled="False" tabIndex="3"
				Font-Names="Arial"></asp:textbox>
			<asp:button id="btnSrchGroups" style="Z-INDEX: 103; LEFT: 512px; POSITION: absolute; TOP: 24px"
				runat="server" Width="97" Height="24" Text="Search" tabIndex="4" Font-Names="Arial"></asp:button>
			<asp:ListBox id="lbSelect" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 80px" runat="server"
				Font-Names="Arial" Width="480px" Height="186px" tabIndex="5" SelectionMode="Multiple"></asp:ListBox>
			<asp:Button id="btnSave" style="Z-INDEX: 107; LEFT: 512px; POSITION: absolute; TOP: 224px" runat="server"
				Width="97" Height="24" Text="Add" tabIndex="6" Font-Names="Arial"></asp:Button>
			<asp:Button id="btnCancel" style="Z-INDEX: 108; LEFT: 512px; POSITION: absolute; TOP: 256px"
				runat="server" Width="97" Height="24" Text="Cancel" tabIndex="7" Font-Names="Arial"></asp:Button>
			<input type="hidden" id="control" runat="server" NAME="control" style="LEFT: 10px; TOP: 16px">
			<asp:Label id="lblDisplay" style="Z-INDEX: 111; LEFT: 16px; POSITION: absolute; TOP: 288px"
				runat="server" Font-Names="Arial" ForeColor="LightGray" Font-Size="Smaller">* only the first 20 results will be displayed</asp:Label>
		</form>
	</body>
</HTML>
