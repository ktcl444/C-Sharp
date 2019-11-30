<%@ Page Language="C#" AutoEventWireup="true" validateRequest="false" CodeBehind="SearchDependencySQLFile.aspx.cs" Inherits="SqlDependencyTest.SearchDependencySQLFile" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table width="100%" height="100%">
        <tr>
        <td >
            <asp:TextBox ID="txtDependencyTables" runat="server" TextMode="MultiLine" height="200px" Width ="100%"></asp:TextBox>
        </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnFilterFile" runat="server" Text="过滤文件" 
                    onclick="btnFilterFile_Click" />
                <asp:TextBox ID="txtSlxtPath" runat="server">E:\Mywork\2011\ERP3.0.0\ERP2.5.6\Map\Slxt\</asp:TextBox>
                <asp:TextBox ID="txtCbglPath" runat="server" 
                   >E:\Mywork\2011\ERP3.0.0\ERP2.5.6\Map\Cbgl\</asp:TextBox>
                <asp:Button ID="txtAnalysisXml" runat="server" onclick="txtAnalysisXml_Click" 
                    style="width: 69px" Text="分析Xml" />
                <asp:TextBox ID="txtMapPath" runat="server" >E:\Mywork\2011\ERP3.0.0\ERP2.5.6\Map\</asp:TextBox>
                <asp:Button ID="btnAnalysisMapXml" runat="server" onclick="Button2_Click" 
                    Text="分析MapXml" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="id" DataSourceID="SqlDataSource1" Width="100%" 
                    AllowPaging="True">
                    <Columns>
                        <asp:BoundField DataField="id" HeaderText="id" ReadOnly="True" 
                            SortExpression="id" />
                        <asp:BoundField DataField="TableName" HeaderText="TableName" 
                            SortExpression="TableName" />
                        <asp:CheckBoxField DataField="NeedModify" HeaderText="NeedModify" 
                            SortExpression="NeedModify" />
                        <asp:BoundField DataField="FilePath" HeaderText="FilePath" 
                            SortExpression="FilePath" />
                        <asp:BoundField DataField="textdata" HeaderText="textdata" SortExpression="textdata" />
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" />
                   <%-- ConnectionString="<%$ ConnectionStrings:SQLDependencyTestConnectionString %>" 
                    
                    SelectCommand="SELECT DependencySqlFile.id, DependencySqlFile.TableName, DependencySqlFile.NeedModify, TbSQL.FilePath, TbSQL.textdata FROM DependencySqlFile INNER JOIN TbSQL ON DependencySqlFile.TbSQLID = TbSQL.id ORDER BY TbSQL.FilePath">
                </asp:SqlDataSource>--%>
            </td>
        </tr>
        <tr>
        <td>
            <asp:TextBox ID="txtSource" runat="server">SELECT s_Opportunity.OppGUID,ProjName,Status,Gfyx,UserName,s_Opportunity.CreatedOn,LastDate,s_Opportunity.ProjGUID FROM dbo.s_Opportunity LEFT JOIN s_Opp2Cst ON s_Opp2Cst.OppGUID=s_Opportunity.OppGUID AND cstNum=1 LEFT JOIN myUser ON myUser.UserGUID=s_Opportunity.UserGUID LEFT JOIN p_Project ON p_Project.ProjGUID=s_Opportunity.ProjGUID WHERE (1=1) AND (2=2) ORDER BY ProjCode,s_Opportunity.CreatedOn</asp:TextBox>
            <asp:TextBox ID="txtMacth"
                runat="server">s_Opp2Cst</asp:TextBox><asp:Button ID="Button1" runat="server" 
                Text="Button" onclick="Button1_Click" />
        
        </td>
        </tr>
        <tr>
        <td>
            <asp:TextBox ID="txtSql" runat="server"></asp:TextBox>
            <asp:Button ID="btnGetContainDBObject" runat="server" Text="Button" 
                onclick="btnGetContainDBObject_Click" />
        
            <asp:TextBox ID="txtContainDBObject"
                runat="server" Width="200px"></asp:TextBox>
            <asp:TextBox ID="txtNeedCache" runat="server"></asp:TextBox>
        
        </td>
        </tr>
         <tr>
        <td>
            <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox><asp:Button ID="btnInsertUserActionStringCache"
                runat="server" Text="Button" 
                onclick="btnInsertUserActionStringCache_Click" />
        
        </td>
        </tr>
    </table>
    </div>
    </form>
</body>
</html>
