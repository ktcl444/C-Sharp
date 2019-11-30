<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PreviewClause.aspx.vb" Inherits="gp.PreviewClause"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>previewClause</title>
		<meta name="vs_snapToGrid" content="False">
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body MS_POSITIONING="GridLayout" bgColor="#4682b4" scroll="yes">
		<form id="Form1" method="post" runat="server">
			<asp:Table id="tbPreview" style="Z-INDEX: 100; LEFT: 8px; POSITION: absolute; TOP: 55px" runat="server"
				Width="567px" CellPadding="1" BorderColor="LightGray" BorderWidth="1px" Font-Names="Arial"
				ForeColor="LightGray" BorderStyle="Solid" GridLines="Both" Font-Size="X-Small">
				<asp:TableRow>
					<asp:TableCell Font-Bold="True" Text="Display Name" ID="displayName"></asp:TableCell>
					<asp:TableCell Font-Bold="True" Text="MV Object UID" ID="mvObjectUID"></asp:TableCell>
				</asp:TableRow>
			</asp:Table>
			<asp:Label id="lblTotal" style="Z-INDEX: 106; LEFT: 487px; POSITION: absolute; TOP: 32px" runat="server"
				Font-Size="X-Small" ForeColor="LightGray" Font-Names="Arial" Width="88px"></asp:Label>
			<asp:Label id="lblNote" style="Z-INDEX: 105; LEFT: 8px; POSITION: absolute; TOP: 32px" runat="server"
				ForeColor="LightGray" Font-Names="Arial" Font-Size="X-Small" Visible="False">* This group has exceptions that are not shown in the preview</asp:Label>
			<asp:Label id="lblError" style="Z-INDEX: 103; LEFT: 9px; POSITION: absolute; TOP: 86px" runat="server"
				ForeColor="LightGray" Font-Names="Arial Black" Width="561px" Height="96px"></asp:Label>
			<asp:Button id="btnClosePreview" style="Z-INDEX: 101; LEFT: 454px; POSITION: absolute; TOP: 8px"
				runat="server" Text="Close Preview" Width="122"></asp:Button>
			<asp:Label id="lblGroupName" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" Font-Names="Arial" ForeColor="LightGray"></asp:Label>
		</form>
	</body>
</HTML>
