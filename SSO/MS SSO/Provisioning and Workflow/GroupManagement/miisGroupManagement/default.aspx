<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Default.aspx.vb" Inherits="gp.DefaultClass" validateRequest="false"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>miisGroupManagement</title>
		<meta content="False" name="vs_snapToGrid">
		<meta content="True" name="vs_showGrid">
		<META http-equiv="Content-Type" content="text/html; charset=windows-1252">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript">
			function confirm_delete()
			{
			if (confirm("This action will delete all clauses and exceptions associated with the group.  Are you sure you want to delete this item?")==true)
				return true;
			else
				return false;
			}
		</script>
	</HEAD>
	<body bgColor="#4682b4" MS_POSITIONING="GridLayout" alink="#000000" vlink="#000000" link="#000000">
		<form id="dataGridForm" name="dataGridForm" method="post" runat="server">
			<asp:datagrid id=dgGroupDefinitions style="Z-INDEX: 100; LEFT: 8px; POSITION: absolute; TOP: 112px" tabIndex=7 runat="server" AllowPaging="True" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="3" GridLines="Vertical" DataSource="<%# dsGroupDefinitions1 %>" DataKeyField="objectUID" DataMember="groupDefinitions" AutoGenerateColumns="False" Width="768px" Font-Names="Arial" >
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
				<ItemStyle Font-Names="Arial" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				<HeaderStyle Font-Names="Arial" Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
				<FooterStyle HorizontalAlign="Left" ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
				<Columns>
					<asp:TemplateColumn>
						<ItemTemplate>
							<CENTER>
								<asp:Image id=autoImage runat="server" ImageUrl='<%#getObjectType(DataBinder.Eval(Container.DataItem , "objectType"))%>'>
								</asp:Image></CENTER>
						</ItemTemplate>
						<EditItemTemplate>
							<CENTER>
								<asp:Image id=autoImageEdit runat="server" ImageUrl='<%#getObjectType(DataBinder.Eval(Container.DataItem , "objectType"))%>'>
								</asp:Image></CENTER>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn Visible="False" DataField="objectUID" SortExpression="objectUID" ReadOnly="True"
						HeaderText="objectUID">
						<ItemStyle Wrap="False"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="displayName" SortExpression="displayName" HeaderText="Group Name">
						<HeaderStyle Wrap="False"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn Visible="False" HeaderText="Description">
						<ItemTemplate>
							<asp:Label id="descriptionLabel" runat="server" text='<%#getDescription(DataBinder.Eval(Container.DataItem , "description"))%>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox id="descriptionLabelEdit" runat="server" text='<%#DataBinder.Eval(Container.DataItem , "description")%>'>
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Group Type">
						<HeaderStyle Wrap="False"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="groupTypeLabel" runat="server" text='<%#getGroupTypeName(DataBinder.Eval(Container.DataItem , "groupType"))%>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList id="groupTypeDropDownList" runat="server" Width="150px">
								<asp:ListItem Value="Sec Group - Univ">Sec Group - Univ</asp:ListItem>
								<asp:ListItem Value="Sec Group - Global">Sec Group - Global</asp:ListItem>
								<asp:ListItem Value="Sec Group - DomLocal">Sec Group - DomLocal</asp:ListItem>
								<asp:ListItem Value="Dist Group - Univ">Dist Group - Univ</asp:ListItem>
								<asp:ListItem Value="Dist Group - Global">Dist Group - Global</asp:ListItem>
								<asp:ListItem Value="Dist Group - DomLocal">Dist Group - DomLocal</asp:ListItem>
							</asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Enabled">
						<ItemTemplate>
							<center>
								<asp:Image id="enabledImage" runat="server" ImageUrl='<%#getenabledFlagStatus(DataBinder.Eval(Container.DataItem , "enabledFlag"))%>'>
								</asp:Image>
							</center>
						</ItemTemplate>
						<EditItemTemplate>
							<center>
								<asp:ImageButton id="enabledImageEdit" CommandName="enabledImageEdit" runat="server" ImageUrl='<%#getenabledFlagStatusEdit(DataBinder.Eval(Container.DataItem , "enabledFlag"))%>'>
								</asp:ImageButton>
							</center>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Mail">
						<ItemTemplate>
							<center>
								<asp:Image id="mailEnabled" runat="server" ImageUrl='<%#getMailNickNameStatus(DataBinder.Eval(Container.DataItem , "mailNickName"))%>'>
								</asp:Image>
							</center>
						</ItemTemplate>
						<EditItemTemplate>
							<center>
								<asp:ImageButton id="mailEnabledEdit" CommandName="mailEnabledEdit" runat="server" ImageUrl='<%#getMailNickNameStatusEdit(DataBinder.Eval(Container.DataItem , "mailNickName"))%>'>
								</asp:ImageButton>
							</center>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Clause">
						<ItemTemplate>
							<center>
								<asp:Image id="clauseImage" runat="server" ImageUrl='<%#getClauseStatus(DataBinder.Eval(Container.DataItem , "objectUID"))%>'>
								</asp:Image>
							</center>
						</ItemTemplate>
						<EditItemTemplate>
							<center>
								<asp:ImageButton id="clauseImageEdit" CommandName="clauseImageEdit" runat="server" ImageUrl='<%#getClauseStatusEdit(DataBinder.Eval(Container.DataItem , "objectUID"))%>'>
								</asp:ImageButton>
							</center>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Exceptions">
						<ItemTemplate>
							<center>
								<asp:Image id="exceptionsImage" runat="server" ImageUrl='<%#getExceptionStatus(DataBinder.Eval(Container.DataItem , "objectUID"))%>'>
								</asp:Image>
							</center>
						</ItemTemplate>
						<EditItemTemplate>
							<center>
								<asp:ImageButton id="exceptionsImageEdit" CommandName="exceptionsImageEdit" runat="server" ImageUrl='<%#getExceptionStatusEdit(DataBinder.Eval(Container.DataItem , "objectUID"))%>'>
								</asp:ImageButton>
							</center>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn Visible="False" DataField="maxExcept" SortExpression="maxExcept" HeaderText="Max Excpt">
						<HeaderStyle Wrap="False"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
					</asp:BoundColumn>
					<asp:EditCommandColumn ButtonType="PushButton" UpdateText="Update" CancelText="Cancel" EditText="Edit"></asp:EditCommandColumn>
					<asp:TemplateColumn>
						<ItemTemplate>
							<asp:Button id="btnDelete" runat="server" Text="Delete" CommandName="Delete"></asp:Button>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle NextPageText="&lt;font size=-1&gt;&lt;a href=./manageAttrGroup.aspx&gt;Define Attribute Groups&lt;/a&gt;&lt;/font&gt;"
					HorizontalAlign="Center" ForeColor="Black" Position="Top" BackColor="#999999" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
			<asp:button id="btnDefineAttributeDefs" style="Z-INDEX: 113; LEFT: 105px; POSITION: absolute; TOP: 114px"
				tabIndex="5" runat="server" Font-Names="Arial" Width="150px" BackColor="#999999" BorderColor="Gray"
				Text="Define Attribute Groups" Height="22px"></asp:button><asp:button id="btnAddRow" style="Z-INDEX: 102; LEFT: 10px; POSITION: absolute; TOP: 114px"
				tabIndex="5" runat="server" BorderColor="Gray" BackColor="#999999" Width="94" Font-Names="Arial" Height="22px" Text="Add Group"></asp:button><asp:button id="btnSrchGroups" style="Z-INDEX: 103; LEFT: 679px; POSITION: absolute; TOP: 66px"
				tabIndex="4" runat="server" Width="95" Font-Names="Arial" Height="24" Text="Search"></asp:button><asp:textbox id="txtSearchValue" style="Z-INDEX: 104; LEFT: 613px; POSITION: absolute; TOP: 34px"
				tabIndex="3" runat="server" BackColor="LightGray" Width="161px" Font-Names="Arial" Height="22px" Enabled="False"></asp:textbox><asp:dropdownlist id="ddlSearchAttribute" style="Z-INDEX: 105; LEFT: 283px; POSITION: absolute; TOP: 34px"
				tabIndex="1" runat="server" Width="161" Font-Names="Arial" Height="22" AutoPostBack="True">
				<asp:ListItem></asp:ListItem>
				<asp:ListItem Value="All" Selected="True">All</asp:ListItem>
				<asp:ListItem Value="groupDefinitions.displayName">Group Name</asp:ListItem>
				<asp:ListItem Value="groupDefinitions.description">Description</asp:ListItem>
				<asp:ListItem Value="clause">Clause</asp:ListItem>
				<asp:ListItem Value="groupdefinitions.enabledFlag">Enabled</asp:ListItem>
				<asp:ListItem Value="exception">Exception</asp:ListItem>
				<asp:ListItem Value="groupDefinitions.groupType">Group Type</asp:ListItem>
				<asp:ListItem Value="groupDefinitions.mailNickName">Mail Alias</asp:ListItem>
			</asp:dropdownlist><asp:dropdownlist id="ddlSearchOperation" style="Z-INDEX: 106; LEFT: 448px; POSITION: absolute; TOP: 34px"
				tabIndex="2" runat="server" BackColor="LightGray" Width="161px" Font-Names="Arial" Height="22px" Enabled="False"
				AutoPostBack="True"></asp:dropdownlist><asp:label id="lblSearch" style="Z-INDEX: 107; LEFT: 283px; POSITION: absolute; TOP: 10px"
				runat="server" Width="184px" Font-Names="Arial" Height="22px" Font-Bold="True" ForeColor="LightGray">Specify Search Criteria:</asp:label><asp:image id="logo" style="Z-INDEX: 108; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				Width="272px" Height="64px" ImageUrl=".\images\pagehead.gif"></asp:image><asp:label id="lblError" style="Z-INDEX: 109; LEFT: 16px; POSITION: absolute; TOP: 176px" runat="server"
				Width="752px" Font-Names="Arial Black" Height="96px" ForeColor="LightGray"></asp:label><asp:label id="lblPageTitle" style="Z-INDEX: 110; LEFT: 33px; POSITION: absolute; TOP: 72px"
				runat="server" Width="192px" Font-Names="Arial" Font-Bold="True" ForeColor="LightGray">MIIS Group Management</asp:label><asp:checkbox id="cbIncludeGroupAuto" style="Z-INDEX: 111; LEFT: 283px; POSITION: absolute; TOP: 66px"
				runat="server" Font-Names="Arial" Text="Include attribute based groups" ForeColor="LightGray"></asp:checkbox>
			<HR style="Z-INDEX: 112; LEFT: 8px; POSITION: absolute; TOP: 96px" width="100%" color="lightgrey"
				SIZE="1">
		</form>
	</body>
</HTML>
