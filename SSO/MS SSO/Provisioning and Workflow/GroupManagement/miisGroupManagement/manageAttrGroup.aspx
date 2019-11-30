<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ManageAttrGroup.aspx.vb" Inherits="gp.AddAttributeGroup" validateRequest="false"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>manageAttributeGroupDefinitions</title>
		<meta name="vs_showGrid" content="True">
		<meta name="vs_snapToGrid" content="True">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript">
			function confirm_delete()
			{
			if (confirm("This action will result in all groups being deleted that are associated with this definition.  Are you sure you want to delete this item?")==true)
				return true;
			else
				return false;
			}
		</script>
	</HEAD>
	<body bgColor="#4682b4" MS_POSITIONING="GridLayout" alink="#000000" vlink="#000000" link="#000000">
		<FORM id="dataGridForm" name="dataGridForm" method="post" runat="server">
			<asp:datagrid id=dgAttributeGroupDefinitions style="Z-INDEX: 100; LEFT: 8px; POSITION: absolute; TOP: 112px" tabIndex=7 runat="server" Font-Names="Arial" AllowPaging="True" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" DataSource="<%# dsAttributeGroupDefinitions1 %>" DataKeyField="objectUID" DataMember="attributeGroupDefinitions" AutoGenerateColumns="False" BackColor="White" Width="768px" >
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
				<ItemStyle Font-Names="Arial" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				<HeaderStyle Font-Names="Arial" Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
				<FooterStyle HorizontalAlign="Left" ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
				<Columns>
					<asp:TemplateColumn>
						<ItemTemplate>
							<CENTER>
								<asp:Image id="autoImage" runat="server" ImageUrl='./images/attributeGroup.gif'></asp:Image></CENTER>
						</ItemTemplate>
						<EditItemTemplate>
							<CENTER>
								<asp:Image id="autoImageEdit" runat="server" ImageUrl='./images/attributeGroup.gif'></asp:Image></CENTER>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn Visible="False" DataField="objectUID" SortExpression="objectUID" ReadOnly="True"
						HeaderText="objectUID">
						<ItemStyle Wrap="False"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn HeaderText="uniqueID">
						<HeaderStyle Wrap="False"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="uniqueGroupIDLabel" runat="server" text='<%#DataBinder.Eval(Container.DataItem , "uniqueGroupID")%>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox id="uniqueGroupIDLabelEdit" runat="server" width="100px" text='<%#DataBinder.Eval(Container.DataItem , "uniqueGroupID")%>'>
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="displayName">
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="displayNameLabel" runat="server" text='<%#GetDisplayName(DataBinder.Eval(Container.DataItem , "displayName"))%>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox id="displayNameLabelEdit" runat="server" width="150px" text='<%#DataBinder.Eval(Container.DataItem , "displayName")%>'>
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="defType">
						<ItemTemplate>
							<asp:Label id="attributeGroupTypeLabel" runat="server" text='<%#DataBinder.Eval(Container.DataItem , "attributeGroupType")%>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList id="attributeGroupTypeDropDownListEdit" runat="server" Width="60px" OnSelectedIndexChanged="selectionChange"
								AutoPostBack="True">
								<asp:ListItem Value="single">single</asp:ListItem>
								<asp:ListItem Value="linked">linked</asp:ListItem>
							</asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="attribute">
						<ItemTemplate>
							<asp:Label id="attributeLabel" runat="server" text='<%#GetAttribute(DataBinder.Eval(Container.DataItem , "attribute"))%>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList id="attributeDropDownListEdit" runat="server" Width="125px">
								<asp:ListItem Value=""></asp:ListItem>
							</asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn Visible="False" HeaderText="linkAttribute">
						<ItemTemplate>
							<asp:Label id="linkAttributeLabel" runat="server" text='<%#GetLinkAttribute(DataBinder.Eval(Container.DataItem , "linkAttribute"))%>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList id="linkAttributeDropDownListEdit" runat="server" Width="125px">
								<asp:ListItem Value=""></asp:ListItem>
							</asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn Visible="False" HeaderText="linkAttributeKey">
						<ItemTemplate>
							<asp:Label id="linkAttributeKeyLabel" runat="server" text='<%#getLinkAttributeKey(DataBinder.Eval(Container.DataItem , "linkAttributeKey"))%>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList id="linkAttributeKeyDropDownListEdit" runat="server" Width="125px">
								<asp:ListItem Value=""></asp:ListItem>
							</asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="groupType">
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="groupTypeLabel" runat="server" text='<%#getGroupTypeName(DataBinder.Eval(Container.DataItem , "groupType"))%>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList id="groupTypeDropDownList" runat="server" Width="155px">
								<asp:ListItem Value="Sec Group - Univ">Sec Group - Univ</asp:ListItem>
								<asp:ListItem Value="Sec Group - Global">Sec Group - Global</asp:ListItem>
								<asp:ListItem Value="Sec Group - DomLocal">Sec Group - DomLocal</asp:ListItem>
								<asp:ListItem Value="Dist Group - Univ">Dist Group - Univ</asp:ListItem>
								<asp:ListItem Value="Dist Group - Global">Dist Group - Global</asp:ListItem>
								<asp:ListItem Value="Dist Group - DomLocal">Dist Group - DomLocal</asp:ListItem>
							</asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="mail">
						<ItemTemplate>
							<center>
								<asp:Image id="mailEnabled" runat="server" ImageUrl='<%#getMailEnabledStatus(DataBinder.Eval(Container.DataItem , "mailEnabled"))%>'>
								</asp:Image>
							</center>
						</ItemTemplate>
						<EditItemTemplate>
							<center>
								<asp:ImageButton id="mailEnabledEdit" CommandName="mailEnabledEdit" runat="server" ImageUrl='<%#getMailEnabledStatusEdit(DataBinder.Eval(Container.DataItem , "mailEnabled"))%>'>
								</asp:ImageButton>
							</center>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:EditCommandColumn ButtonType="PushButton" UpdateText="Update" CancelText="Cancel" EditText="Edit"></asp:EditCommandColumn>
					<asp:TemplateColumn>
						<ItemTemplate>
							<asp:Button id="btnDelete" runat="server" Text="Delete" CommandName="Delete"></asp:Button>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Center" ForeColor="Black" Position="Top" BackColor="#999999" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
			<asp:label id="lblPageHeading" style="Z-INDEX: 108; LEFT: 336px; POSITION: absolute; TOP: 24px"
				runat="server" Width="312px" Font-Names="Arial" ForeColor="LightGray" Font-Bold="True"
				Font-Size="Large">Attribute Group Definitions</asp:label>
			<asp:button id="btnDefineGroups" style="Z-INDEX: 107; LEFT: 109px; POSITION: absolute; TOP: 114px"
				tabIndex="5" runat="server" Width="160px" BackColor="#999999" BorderColor="Gray" Font-Names="Arial"
				Height="22px" Text="Back to Group Definitions"></asp:button>
			<asp:button id="btnAddAttributeRow" style="Z-INDEX: 101; LEFT: 10px; POSITION: absolute; TOP: 114px"
				tabIndex="5" runat="server" Font-Names="Arial" BorderColor="Gray" BackColor="#999999" Width="97px"
				Text="Add Definition" Height="22px"></asp:button>
			<asp:image id="logo" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				Width="272px" Height="64px" ImageUrl=".\images\pagehead.gif"></asp:image>
			<asp:label id="lblError" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 176px" runat="server"
				Font-Names="Arial Black" Width="752px" Height="96px" ForeColor="LightGray"></asp:label>
			<asp:label id="lblPageTitle" style="Z-INDEX: 105; LEFT: 33px; POSITION: absolute; TOP: 72px"
				runat="server" Font-Names="Arial" Width="192px" ForeColor="LightGray" Font-Bold="True">MIIS Group Management</asp:label>
			<HR style="Z-INDEX: 106; LEFT: 8px; POSITION: absolute; TOP: 96px" width="100%" color="lightgrey"
				SIZE="1">
		</FORM>
	</body>
</HTML>
