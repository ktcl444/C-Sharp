<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CacheMemoryTest._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="TextBox1" runat="server">select top 1000 * from p_room</asp:TextBox>
         <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
        <asp:Button ID="Button1"
            runat="server" Text="插入缓存(DataTable)" onclick="Button1_Click" />
        <asp:TextBox ID="TextBox3" runat="server">select top 1 roomguid from p_room</asp:TextBox>
        <asp:Button ID="Button2" runat="server" Text="插入缓存(String)" 
            onclick="Button2_Click" />
        <asp:Button ID="btnGetCacheNumber" runat="server" 
            onclick="btnGetCacheNumber_Click" Text="获得缓存数量" />
        <asp:TextBox ID="TextBox9" runat="server"></asp:TextBox>
        <br />
         <asp:TextBox ID="txtCacheKey" runat="server" ></asp:TextBox>
        <asp:Button ID="btnGetCache"
            runat="server" Text="获得缓存" onclick="btnGetCache_Click" />
         <asp:TextBox ID="txtGetCacheResult" runat="server" 
            ></asp:TextBox>
    </div>
    <table width=100%>
        <tr>
            <td>SQL</td>
            <td>内存大小</td>
            <td>类型</td>
        </tr>
               <tr>
            <td>select top 3000000 * from p_room</td>
            <td>1.3G</td>
            <td>DataTable</td>
        </tr>
               <tr>
            <td>select top 200000 * from p_room</td>
            <td>881MB</td>
            <td>DataTable</td>
        </tr>
             <tr>
            <td>select top 50000 * from p_room</td>
            <td>222.75MB</td>
            <td>DataTable</td>
        </tr>
             <tr>
            <td>select top 1000 * from p_room</td>
            <td>4.713MB</td>
            <td>DataTable</td>
        </tr>
          <tr>
            <td>select top 1 roomguid from p_room</td>
            <td>90byte</td>
            <td>string</td>
        </tr>
    </table>
    <table style="width:100%;">
        <tr>
            <td colspan="3">
                内存测试:
            </td>
        </tr>
        <tr>
            <td colspan="3">
                1，单个缓存项大小限制</td>
        </tr>
        <tr>
            <td colspan="3">
        <asp:TextBox ID="TextBox4" runat="server" Width="200px">select top 1000 * from myWorkflowBandEntity</asp:TextBox>
        <asp:Button ID="Button3"
            runat="server" Text="插入缓存(DataTable)" onclick="Button3_Click" />
         <asp:TextBox ID="TextBox5" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                2，缓存项数量限制</td>
        </tr>
        <tr>
            <td colspan="3">
        <asp:TextBox ID="TextBox6" runat="server" Width="200px">select top 1 roomguid from p_room</asp:TextBox>
         <asp:TextBox ID="txtCacheNumber" runat="server">10000000</asp:TextBox>
        <asp:Button ID="Button4"
            runat="server" Text="插入缓存(String)" onclick="Button4_Click" />
                <asp:TextBox ID="txtCacheNumberResult" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                3，缓存大小限制</td>
        </tr>
        <tr>
            <td colspan="3">
        <asp:TextBox ID="TextBox7" runat="server" Width="200px">select top 50000 * from p_room</asp:TextBox>
        <asp:Button ID="Button5"
            runat="server" Text="插入缓存(DataTable)" onclick="Button5_Click" />
         <asp:TextBox ID="TextBox8" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                4，清除机制(命中率,优先级,新旧)</td>
        </tr>
        <tr>
            <td colspan="3">
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    <asp:TextBox ID="txtRemoveReason" runat="server" Height="100px" 
        TextMode="MultiLine" Width="100%" ></asp:TextBox>
    </form>
    </body>
</html>
