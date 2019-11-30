<%@ Page Language="vb" AutoEventWireup="false" Codebehind="BuildMail.aspx.vb" Inherits="gp.BuildMail"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>buildMail</title>
		<meta name="vs_snapToGrid" content="True">
		<meta name="vs_showGrid" content="True">
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body MS_POSITIONING="GridLayout" bgColor="steelblue">
		<form id="Form1" method="post" runat="server">
			<asp:button id="btnSave" style="Z-INDEX: 102; LEFT: 280px; POSITION: absolute; TOP: 280px" tabIndex="8"
				runat="server" Text="Update" Font-Names="Arial" Width="62" Height="24"></asp:button>
			<asp:textbox id="txtDefault" style="Z-INDEX: 107; LEFT: 40px; POSITION: absolute; TOP: 144px"
				tabIndex="7" runat="server" BackColor="White" Height="24px" Width="336px" Font-Names="Arial"
				MaxLength="5120" Enabled="False" Visible="False">teest</asp:textbox>
			<asp:button id="btnCancel" style="Z-INDEX: 103; LEFT: 344px; POSITION: absolute; TOP: 280px"
				tabIndex="9" runat="server" Text="Cancel" Font-Names="Arial" Width="62" Height="24"></asp:button>
			<asp:label id="lblError" style="Z-INDEX: 104; LEFT: 24px; POSITION: absolute; TOP: 40px" runat="server"
				Font-Names="Arial Black" Width="368px" Height="160px" ForeColor="LightGray" BackColor="#000084"></asp:label>
			<asp:RadioButtonList id="rdblMail" style="Z-INDEX: 105; LEFT: 16px; POSITION: absolute; TOP: 32px" runat="server"
				BackColor="#000084" ForeColor="White" Height="192px" Width="248px" Font-Names="Arial" AutoPostBack="True">
				<asp:ListItem Value="none" Selected="True">Mail disabled</asp:ListItem>
				<asp:ListItem Value="default">Mail enabled with default alias</asp:ListItem>
				<asp:ListItem Value="custom">Mail enabled with custom alias</asp:ListItem>
			</asp:RadioButtonList>
			<asp:textbox id="txtCustom" style="Z-INDEX: 106; LEFT: 40px; POSITION: absolute; TOP: 208px"
				tabIndex="7" runat="server" Height="24px" Width="336px" Font-Names="Arial" MaxLength="5120"
				Visible="False"></asp:textbox>
			<asp:Panel id="pnlMailStatus" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" BackColor="#000084" ForeColor="White" Font-Bold="True" Height="264px" Width="400px"
				Font-Names="Arial">&nbsp;Mail Status for group 
</asp:Panel>
			<asp:Label id="lblDisplayName" style="Z-INDEX: 108; LEFT: 176px; POSITION: absolute; TOP: 10px"
				runat="server" BackColor="#000084" ForeColor="White" Font-Bold="True" Font-Names="Arial"></asp:Label>
		</form>
	</body>
</HTML>
