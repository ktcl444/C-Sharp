<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemcacheTest.aspx.cs" Inherits="MemcacheTest.MemcacheTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
         <asp:Label ID="Label3" runat="server" Text="数据库连接"></asp:Label>
        <asp:TextBox ID="txtDBConnection" runat="server" Width="530px">Data Source=10.5.23.8;Initial Catalog=广州保利_正式库;User Id=sa;Password=95938;</asp:TextBox>
          <asp:Button ID="btnClear" runat="server" Text="清空缓存" OnClick="btnClear_Click" />
        <asp:Button ID="Button1" runat="server" Text="清空操作信息" OnClick="Button1_Click" />
        <p />
         <asp:Label ID="Label5" runat="server" Text="一 单对象缓存"></asp:Label>
        <hr />
            <asp:Label ID="Label1" runat="server" Text="缓存键"></asp:Label>
         <asp:TextBox ID="txtCacheKey" runat="server"></asp:TextBox>
        <asp:Label ID="Label2" runat="server" Text="缓存值"></asp:Label>
       
        <asp:TextBox ID="txtCacheValue" runat="server"></asp:TextBox>
         <asp:Button ID="btnInsert" runat="server" Text="缓存" OnClick="btnInsert_Click" />
              <p />
        <asp:TextBox ID="txtGetCacheValue" runat="server"></asp:TextBox>
        <asp:Button ID="btnGet" runat="server" Text="读取" OnClick="btnGet_Click" />
        <p />
                 <asp:Label ID="Label6" runat="server" Text="二 文件缓存"></asp:Label>
        <hr />
        <asp:Label ID="Label8" runat="server" Text="文件名称"></asp:Label>
         <asp:TextBox ID="txtFilePath" runat="server">Web.config</asp:TextBox>
                <asp:Label ID="Label9" runat="server" Text="读取次数"></asp:Label>
         <asp:TextBox ID="txtFileCacheTimes" runat="server">1</asp:TextBox>
        <p />
          <asp:Button ID="Button4" runat="server" Text="正常读取" OnClick="Button4_Click"/>
         <asp:Button ID="Button2" runat="server" Text="缓存读取" OnClick="Button2_Click" />
   
                <p />
                 <asp:Label ID="Label7" runat="server" Text="三 数据库缓存"></asp:Label>
        <hr />
          <asp:Label ID="Label10" runat="server" Text="查询SQL"></asp:Label>
        <asp:TextBox ID="txtSQL" runat="server" Width="429px">SELECT TOP 100 UserRightsGUID,UserGUID,StationGUID,ObjectType,Application,ActionCode FROM myUserRights</asp:TextBox>
                <asp:Label ID="Label11" runat="server" Text="读取次数"></asp:Label>
         <asp:TextBox ID="txtSQLCacheTimes" runat="server">1</asp:TextBox>
           <p />
                  <asp:Button ID="Button3" runat="server" Text="正常读取" OnClick="Button3_Click"/>
         <asp:Button ID="Button5" runat="server" Text="缓存读取" OnClick="Button5_Click"/>
                        <p />
                 <asp:Label ID="Label12" runat="server" Text="四 缓存数量级别"></asp:Label>
         <p />
          <asp:Label ID="Label13" runat="server" Text="缓存数量"></asp:Label>
         <asp:TextBox ID="txtCacheNumber" runat="server">1</asp:TextBox>
        <asp:Button ID="Button8" runat="server" Text="写入缓存" OnClick="Button8_Click"/>
         <p />
          <asp:Button ID="Button6" runat="server" Text="随机写缓存" OnClick="Button6_Click"/>
          <asp:Button ID="Button7" runat="server" Text="随机读缓存" OnClick="Button7_Click"/>
          <asp:Button ID="Button9" runat="server" Text="随机删缓存" OnClick="Button9_Click"/>
        <hr />
        <p />
        <asp:Label ID="Label4" runat="server" Text="操作信息："></asp:Label>
        <asp:TextBox ID="txtOperatorInfo" runat="server" TextMode="MultiLine" Width="100%" Height="124px"></asp:TextBox>
    </div>
        
       

    </form>
</body>
</html>
