<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SqlDependencyTest._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <p/>
    
        <asp:Label ID="Label1" runat="server" Text="公司 dbo.t_BusinessUnit"></asp:Label>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        DataKeyNames="BUGUID" DataSourceID="SQLDependencyTest_BU">
        <Columns>
            <asp:BoundField DataField="BUGUID" HeaderText="BUGUID" ReadOnly="True" 
                SortExpression="BUGUID" />
            <asp:BoundField DataField="BUName" HeaderText="BUName" 
                SortExpression="BUName" />
            <asp:BoundField DataField="BUCode" HeaderText="BUCode" 
                SortExpression="BUCode" />
            <asp:BoundField DataField="Level" HeaderText="Level" SortExpression="Level" />
            <asp:BoundField DataField="ParentBUGUID" HeaderText="ParentBUGUID" 
                SortExpression="ParentBUGUID" />
            <asp:CommandField ShowEditButton="True" />
        </Columns>
        <HeaderStyle BackColor="Silver" />
    </asp:GridView>
    <p/>    
        <asp:Label ID="Label2" runat="server" Text="用户 dbo.t_user"></asp:Label>
    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
        DataKeyNames="UserGUID" DataSourceID="SQLDependencyTest_User" >
        <Columns>
            <asp:BoundField DataField="UserGUID" HeaderText="UserGUID" 
                SortExpression="UserGUID" ReadOnly="True" />
            <asp:BoundField DataField="UserName" HeaderText="UserName" 
                SortExpression="UserName" />
            <asp:BoundField DataField="UserCode" HeaderText="UserCode" 
                SortExpression="UserCode" />
            <asp:BoundField DataField="Sex" HeaderText="Sex" 
                SortExpression="Sex" />
            <asp:BoundField DataField="Birthday" HeaderText="Birthday" 
                SortExpression="Birthday" />
            <asp:BoundField DataField="Address" HeaderText="Address" 
                SortExpression="Address" />
            <asp:BoundField DataField="BUGUID" HeaderText="BUGUID" 
                SortExpression="BUGUID" />
            <asp:CommandField ShowEditButton="True" />
        </Columns>
        <HeaderStyle BackColor="Silver" />
    </asp:GridView>
    
        <asp:Button ID="Button9" runat="server" onclick="Button9_Click" Text="Button" 
            Visible="False" />
    
      <p/>    
        <asp:Label ID="Label3" runat="server" Text="缓存测试"></asp:Label>
          2<p/>
           <asp:TextBox ID="TextBox9" runat="server" TextMode="MultiLine" Width="500px" 
                 Height="100px" Visible="False">111</asp:TextBox>
            <asp:TextBox ID="txtRemoveReason" runat="server" Width="328px"></asp:TextBox>
         <p/> 
        <asp:TextBox ID="TextBox1" runat="server" TextMode="MultiLine" Width="500px" 
                 Height="100px">SELECT BUGUID FROM dbo.t_BusinessUnit WHERE BUName = &#39;平台开发部&#39;</asp:TextBox>

                         <asp:Button ID="Button10" runat="server" onclick="Button10_Click" 
                 Text="移除缓存" />

                         <asp:Button ID="Button1"
            runat="server" Text="插入缓存" onclick="Button1_Click" />
            
             <asp:Button ID="Button2" 
                 runat="server" Text="显示缓存" onclick="Button2_Click" />
                     <asp:TextBox ID="TextBox5" runat="server" TextMode="MultiLine"></asp:TextBox>
                     <p/> 
        <asp:TextBox ID="TextBox2" runat="server" TextMode="MultiLine" Width="500px" Height="100px">SELECT BUName FROM dbo.t_BusinessUnit WHERE BUName = &#39;平台开发部&#39;</asp:TextBox>
                         <asp:Button ID="Button3"
            runat="server" Text="插入缓存" onclick="Button3_Click" /><asp:Button ID="Button4" runat="server" 
                             Text="显示缓存" onclick="Button4_Click" />
                         <asp:TextBox ID="TextBox6" runat="server" TextMode="MultiLine"></asp:TextBox>
                     <p/> 
        <asp:TextBox ID="TextBox3" runat="server" TextMode="MultiLine" Width="500px" Height="100px">SELECT BUName FROM dbo.t_BusinessUnit INNER JOIN dbo.t_user ON dbo.t_BusinessUnit.BUGUID=dbo.t_user.BUGUID WHERE dbo.t_BusinessUnit.BUName='平台开发部'</asp:TextBox>
                         <asp:Button ID="Button5"
            runat="server" Text="插入缓存" onclick="Button5_Click" /><asp:Button ID="Button6" runat="server" 
                             Text="显示缓存" onclick="Button6_Click" />
                         <asp:TextBox ID="TextBox7" runat="server" TextMode="MultiLine"></asp:TextBox>
                     <p/> 
        <asp:TextBox ID="TextBox4" runat="server" TextMode="MultiLine" Width="500px" Height="100px">SELECT UserGUID, UserName, UserCode, Sex, Birthday, Address, BUGUID FROM dbo.t_user WHERE UserCode=&#39;kongy&#39;</asp:TextBox>
                         <asp:Button ID="Button7"
            runat="server" Text="插入缓存" onclick="Button7_Click" /><asp:Button ID="Button8" runat="server" 
                             Text="显示缓存" onclick="Button8_Click" />
               <asp:TextBox ID="TextBox8" runat="server" TextMode="MultiLine"></asp:TextBox>
               <p />
               <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False" 
        DataKeyNames="BUGUID" >
        <Columns>
            <asp:BoundField DataField="BUGUID" HeaderText="BUGUID" ReadOnly="True" 
                SortExpression="BUGUID" />
            <asp:BoundField DataField="BUName" HeaderText="BUName" 
                SortExpression="BUName" />
            <asp:BoundField DataField="BUCode" HeaderText="BUCode" 
                SortExpression="BUCode" />
            <asp:BoundField DataField="Level" HeaderText="Level" SortExpression="Level" />
            <asp:BoundField DataField="ParentBUGUID" HeaderText="ParentBUGUID" 
                SortExpression="ParentBUGUID" />
        </Columns>
        <HeaderStyle BackColor="Silver" />
    </asp:GridView>
                 <p/> 
    <asp:SqlDataSource ID="SQLDependencyTest_User" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SQLDependencyTestConnectionString %>" 
        
        
        
        
        SelectCommand="SELECT [UserGUID], [UserName], [UserCode], [Sex], [Birthday], [Address], [BUGUID] FROM [t_user] ORDER BY [UserCode]" 
        UpdateCommand="UPDATE t_user SET UserName =@UserName, UserCode =@UserCode, Sex =@Sex, Birthday =@Birthday , Address =@Address , BUGUID =@BUGUID WHERE UserGUID = @UserGUID ">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SQLDependencyTest_BU" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SQLDependencyTestConnectionString %>" 
        
        SelectCommand="SELECT [BUName], [BUGUID], [BUCode], [Level], [ParentBUGUID] FROM [t_BusinessUnit] ORDER BY [Level]" 
        UpdateCommand="UPDATE t_BusinessUnit SET BUName =@BUName, BUCode =@BUCode, Level =@Level, ParentBUGUID =@ParentBUGUID WHERE BUGUID=@BUGUID">
    </asp:SqlDataSource>
    
    </div>
    </form>
</body>
</html>
