<%@ Page Language="vb" AutoEventWireup="false" Codebehind="BuildClause.aspx.vb" Inherits="gp.BuildClause"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>Clause Builder</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body bgColor="steelblue" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:textbox id="tbSQLClause" style="Z-INDEX: 104; LEFT: 16px; POSITION: absolute; TOP: 120px"
				tabIndex="7" runat="server" Font-Names="Arial" Width="576px" Height="140px" TextMode="MultiLine"
				MaxLength="5120"></asp:textbox>
			<asp:button id="btnPreview" style="Z-INDEX: 118; LEFT: 8px; POSITION: absolute; TOP: 280px"
				tabIndex="11" runat="server" Text="Preview"></asp:button><asp:label id="lblError" style="Z-INDEX: 117; LEFT: 16px; POSITION: absolute; TOP: 96px" runat="server"
				Font-Names="Arial Black" Width="528px" Height="96px" ForeColor="LightGray"></asp:label><asp:button id="btnSelectCancel" style="Z-INDEX: 116; LEFT: 544px; POSITION: absolute; TOP: 280px"
				tabIndex="12" runat="server" Visible="False" Text="Cancel"></asp:button><asp:panel id="pnlGroupSelect" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 80px"
				runat="server" Font-Names="Arial" Width="448px" Height="232px" ForeColor="White" Visible="False" Font-Bold="True" BackColor="#000084">&nbsp;Select Group</asp:panel><asp:button id="btnReplace" style="Z-INDEX: 112; LEFT: 536px; POSITION: absolute; TOP: 64px"
				tabIndex="5" runat="server" Font-Names="Arial" Width="62px" Height="24px" Text="Replace"></asp:button><asp:dropdownlist id="ddlSearchAttribute" style="Z-INDEX: 111; LEFT: 16px; POSITION: absolute; TOP: 24px"
				tabIndex="1" runat="server" Font-Names="Arial" Width="184px" Height="22" AutoPostBack="True"></asp:dropdownlist><asp:label id="lblSearch" style="Z-INDEX: 110; LEFT: 16px; POSITION: absolute; TOP: 0px" runat="server"
				Font-Names="Arial" Width="584px" Height="22px" ForeColor="LightGray" Font-Bold="True">Specify Clause Criteria:</asp:label><asp:dropdownlist id="ddlSearchOperation" style="Z-INDEX: 108; LEFT: 208px; POSITION: absolute; TOP: 24px"
				tabIndex="2" runat="server" Font-Names="Arial" Width="184px" Height="22" BackColor="LightGray" AutoPostBack="True" Enabled="False"></asp:dropdownlist><asp:textbox id="txtSearchValue" style="Z-INDEX: 107; LEFT: 400px; POSITION: absolute; TOP: 24px"
				tabIndex="3" runat="server" Font-Names="Arial" Width="195px" Height="22px" BackColor="LightGray" Enabled="False"></asp:textbox><asp:button id="btnAppend" style="Z-INDEX: 105; LEFT: 472px; POSITION: absolute; TOP: 64px"
				tabIndex="4" runat="server" Font-Names="Arial" Width="62px" Height="24px" Text="Append"></asp:button><asp:panel id="pnlSQLClause" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 96px"
				runat="server" Font-Names="Arial" Width="592px" Height="176px" ForeColor="White" Font-Bold="True" BackColor="#000084">&nbsp;Where Clause</asp:panel>
			<asp:button id="btnSave" style="Z-INDEX: 106; LEFT: 472px; POSITION: absolute; TOP: 280px" tabIndex="8"
				runat="server" Font-Names="Arial" Width="62" Height="24" Text="Update"></asp:button><asp:button id="btnCancel" style="Z-INDEX: 109; LEFT: 536px; POSITION: absolute; TOP: 280px"
				tabIndex="9" runat="server" Font-Names="Arial" Width="62" Height="24" Text="Cancel"></asp:button><asp:checkbox id="cbClause" style="Z-INDEX: 113; LEFT: 16px; POSITION: absolute; TOP: 48px" tabIndex="6"
				runat="server" Font-Names="Arial" ForeColor="LightGray" Text="Use clause from existing group" AutoPostBack="True"></asp:checkbox><asp:listbox id="lbGroupSelect" style="Z-INDEX: 100; LEFT: 16px; POSITION: absolute; TOP: 104px"
				tabIndex="10" runat="server" Width="432px" Height="200px" Visible="False"></asp:listbox><asp:button id="btnSelect" style="Z-INDEX: 114; LEFT: 480px; POSITION: absolute; TOP: 280px"
				tabIndex="11" runat="server" Visible="False" Text="Select"></asp:button><asp:radiobuttonlist id="rdbAndOr" style="Z-INDEX: 102; LEFT: 416px; POSITION: absolute; TOP: 48px" runat="server"
				Font-Names="Arial" Width="29px" Height="26px" ForeColor="LightGray">
				<asp:ListItem Value="and">and</asp:ListItem>
				<asp:ListItem Value="or">or</asp:ListItem>
			</asp:radiobuttonlist></form>
	</body>
</HTML>
