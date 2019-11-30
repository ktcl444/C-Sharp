<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CacheTest.aspx.cs" Inherits="Map.Web.Test.Cachetest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>无标题页</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:TextBox ID="TextBox1" runat="server" Rows="6" TextMode="MultiLine" 
            Width="600px"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Button" />
        <asp:Button ID="TestCache" runat="server" Text="TestSetCache" 
            onclick="TestCache_Click" />
    
        <br />
        <br />
        <br />
    
        <asp:TextBox ID="TextBox5" runat="server" Rows="6" TextMode="MultiLine" 
            Width="600px"></asp:TextBox>
        <asp:Button ID="TestCache0" runat="server" Text="TestGetCache" 
            onclick="TestCache0_Click" />
        <br />
        <br />
        <br />
        <asp:TextBox ID="TextBox2" runat="server" Rows="6" TextMode="MultiLine" 
            Width="600px"></asp:TextBox>
        <asp:Button ID="TestMemcached" runat="server" Text="TestSetMemcached" 
            onclick="TestMemcached_Click" />
        <br />
        <br />
        <br />
        <asp:TextBox ID="TextBox6" runat="server" Rows="6" TextMode="MultiLine" 
            Width="600px"></asp:TextBox>
        <asp:Button ID="TestMemcached0" runat="server" Text="TestGetMemcached" 
            onclick="TestMemcached0_Click" />
        <br />
        <br />
        <br />
        <asp:TextBox ID="TextBox3" runat="server" Rows="6" TextMode="MultiLine" 
            Width="600px"></asp:TextBox>
        <asp:Button ID="TestCacheDB" runat="server" Text="TestCacheDB" 
            onclick="TestCacheDB_Click" />
        <br />
        <br />
        <br />
        <asp:TextBox ID="TextBox4" runat="server" Rows="6" TextMode="MultiLine" 
            Width="600px"></asp:TextBox>
        <asp:Button ID="TestMemCachedDB" runat="server" Text="TestMemCachedDB" 
            onclick="TestMemCachedDB_Click" />
    
    </div>
    </form>
</body>
</html>
