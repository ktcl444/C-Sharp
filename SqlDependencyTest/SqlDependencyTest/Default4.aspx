<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default4.aspx.cs" Inherits="SqlDependencyTest.Default4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
   <table style="width: 100%;">
        <tr>
            <td>
             <label>组织</label>
            </td>
            <td>
                <label>用户</label>
            </td>
        </tr>
        <tr>
            <td>
            
    <label>合同</label>
            </td>
            <td>
                <label>项目</label>
            </td>
        </tr>
         <tr>
        <td colspan="2"> <hr /></td>
        </tr>
        <tr>
            <td>
                 <label>分页SQL - Not in</label>
      <p />
    <asp:textbox ID="Textbox1" runat="server" TextMode="MultiLine"  Width="500px" 
              Height="100px"> SELECT FunctionCode FROM myFunction2 WHERE FunctionCode NOT IN (&#39;&#39;,&#39;020101&#39;,&#39;02010207&#39;,&#39;02010201&#39;,&#39;02010202&#39;,&#39;02010301&#39;,&#39;02010303&#39;,&#39;02010302&#39;,&#39;02010206&#39;,&#39;02010108&#39;,&#39;02010210&#39;,&#39;020102&#39;,&#39;02010204&#39;,&#39;02010205&#39;,&#39;020110&#39;,&#39;02010309&#39;,&#39;02010311&#39;,&#39;02010312&#39;,&#39;02010313&#39;,&#39;02010305&#39;,&#39;020103&#39;,&#39;02010304&#39;,&#39;02010306&#39;,&#39;02010307&#39;,&#39;02010308&#39;,&#39;020104&#39;,&#39;02010401&#39;,&#39;02010402&#39;,&#39;02010403&#39;,&#39;02010404&#39;,&#39;020109&#39;,&#39;02010400&#39;,&#39;02010405&#39;,&#39;02010412&#39;,&#39;02010408&#39;,&#39;02010705&#39;,&#39;02010706&#39;,&#39;020105&#39;,&#39;02010501&#39;,&#39;02010502&#39;,&#39;020113&#39;,&#39;02010504&#39;,&#39;02010505&#39;,&#39;02010506&#39;,&#39;020106&#39;,&#39;02010601&#39;,&#39;02010603&#39;,&#39;02010703&#39;,&#39;02010604&#39;,&#39;02010605&#39;,&#39;02010801&#39;,&#39;020112&#39;,&#39;02010208&#39;,&#39;02010209&#39;,&#39;02010704&#39;,&#39;020119&#39;,&#39;02011999&#39;,&#39;020114&#39;,&#39;02011401&#39;,&#39;02011402&#39;,&#39;02011403&#39;,&#39;02011404&#39;,&#39;02011405&#39;,&#39;020115&#39;,&#39;02011502&#39;) </asp:textbox>
     <p />
        <asp:Button ID="Button1" runat="server" Text="插入缓存" onclick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" Text="获取缓存" onclick="Button2_Click" />
         <asp:TextBox ID="TextBox9" runat="server"></asp:TextBox>
         <asp:Button ID="Button7" runat="server" onclick="Button7_Click" Text="清除缓存" />
      <p />
          <label>依赖SQL</label>
     <p />
    <asp:textbox ID="Textbox10" runat="server" TextMode="MultiLine" Width="500px" 
             Height="200px"> SELECT FunctionGUID
 FROM   dbo.myFunction2
 WHERE  FunctionCode NOT IN ( &#39;02010111&#39;, &#39;02010207&#39;, &#39;02010201&#39;, &#39;02010202&#39;,
                              &#39;02010301111&#39;, &#39;02010303&#39;, &#39;02010302&#39;, &#39;02010206&#39;,
                              &#39;02010108111&#39;, &#39;02010210&#39;, &#39;020102&#39;, &#39;02010204&#39;,
                              &#39;020102051111&#39;, &#39;020110&#39;, &#39;02010309&#39;, &#39;02010311&#39;,
                              &#39;020103121111&#39;, &#39;02010313&#39;, &#39;02010305&#39;, &#39;020103&#39;,
                              &#39;0201030411111&#39;, &#39;02010306&#39;, &#39;02010307&#39;, &#39;02010308&#39;,
                              &#39;0201041111111&#39;, &#39;02010401&#39;, &#39;02010402&#39;, &#39;02010403&#39;,
                              &#39;020104014&#39;, &#39;020109&#39;, &#39;02010400&#39;, &#39;02010405&#39;,
                              &#39;020104112&#39;, &#39;02010408&#39;, &#39;02010705&#39;, &#39;02010706&#39;,
                              &#39;0201051&#39;, &#39;02010501&#39;, &#39;02010502&#39;, &#39;020113&#39;,
                              &#39;02010504111&#39;, &#39;020105051&#39;, &#39;02010506&#39;, &#39;020106&#39;)</asp:textbox>
     <p />
    <asp:textbox ID="Textbox11" runat="server" TextMode="MultiLine" Width="500px"></asp:textbox>
                </td>
            <td><label>分页SQL - row_number</label>
      <p />
    <asp:textbox ID="Textbox7" runat="server" TextMode="MultiLine" Width="500px" 
              Height="100px">WITH _t AS (SELECT ROW_NUMBER() OVER(ORDER BY vcb_ContractGrid.ContractCode DESC,vcb_ContractGrid.ContractGUID) AS _RowNumber, ProjectName as HtProjName,ContractGUID,vcb_ContractGrid.BUGUID,IsJtContract,ApproveState
				,ApproveStateFlag,JsState,MasterContractGUID,IfDdhs,HtCfStateShow,CfMode,HtClass,ContractCode,ContractName,HtAmount,SignDate
				,YfProviderName,YfCorporation ,jbrGUID ,ProcessGUID ,IsLock,(case when IsLock=1 then '已锁定' else '未锁定' end) as ShowLock 
				     FROM vcb_ContractGrid Left Join myWorkflowProcessEntity b ON vcb_ContractGrid.ContractGUID = b.BusinessGUID AND isnull(b.IsHistory,0) = 0  AND ( b.BusinessType='合同审批'  OR b.BusinessType='非合同审批' OR b.BusinessType='非单独执行合同审批')
				     WHERE ((((( JbDeptCode='zb.sz01' or JbDeptCode like 'zb.sz01.%' )) AND ( 88=88 )) AND (1='1'))) AND ( (9=9) ) AND vcb_ContractGrid.BUGUID=('24f41971-3cdf-4e82-8860-90195ab75091')
				 ) SELECT * FROM _t WHERE _RowNumber BETWEEN 21 AND 30 ORDER BY _RowNumber</asp:textbox>
     <p />
        <asp:Button ID="Button5" runat="server" Text="插入缓存" onclick="Button5_Click" />
        <asp:Button ID="Button6" runat="server" Text="获取缓存" onclick="Button6_Click" />
         <asp:TextBox ID="TextBox8" runat="server"></asp:TextBox>
      <p />
    <label>依赖SQL</label>
     <p />
    <asp:textbox ID="Textbox2" runat="server" TextMode="MultiLine" Width="500px">select ContractGUID from dbo.cb_contract</asp:textbox>
     <p />
    <asp:textbox ID="Textbox3" runat="server" TextMode="MultiLine" Width="500px">select BUGUID from dbo.mybusinessunit</asp:textbox></td>
        </tr>
                 <tr>
        <td colspan="2"> <hr /></td>
        </tr>
        <tr>
        <td>
    <label>AppGridTree取数SQL</label>
      <p />
    <asp:textbox ID="Textbox4" runat="server" TextMode="MultiLine" Width="500px" Height="100px">SELECT Distinct a.ProjCode as Code, a.ProjShortName as Name, 
    a.ProjGUID as GUID, a.ParentCode, a.Level, a.LevelCode, a.IfEnd , a.Type FROM ep_project b  Left Join ep_project a 
    On b.LevelCode + '.' like a.LevelCode + '.%' And a.BUGUID=b.BUGUID WHERE b.IfEnd=1 AND b.BUGUID='24f41971-3cdf-4e82-8860-90195ab75091' 
    AND CHARINDEX('0201', b.ApplySys)>0  AND b.ProjGUID NOT IN (select ProjGUID from p_CwjkProject  left join p_CwjkCwzt 
    on p_CwjkProject.CwztGUID = p_CwjkCwzt.CwztGUID  where p_CwjkCwzt.Application ='0201')  
    AND (b.ProjGUID in ('d221755c-938f-47fa-b582-9dbdd81478d4','c3058e26-ff3c-4f0d-92a5-76f34340de3b','37641496-23aa-46cb-9817-027aa120f5c6','37641496-23aa-46cb-9817-027aa120f5c6','37641496-23aa-46cb-9817-027aa120f5c6','baa95b67-fc41-4a78-af1c-014bbe0ac368','4a7b48fe-4793-4538-aacd-3748531c8375','340c0d92-1bc9-464a-b565-77efae6caf53','22a37e0d-103f-4232-b68e-5639bae5e5d5','d5774527-4a2a-4087-b01b-6bf2640b449f','2013254e-e489-4922-8ec9-95f9f954959a','50cc7dda-bf1b-439c-9d70-459e2dbd9998','7f48477c-860e-4a8d-a14f-7844beb6671c','f1147fb3-cc78-4e9f-9e3e-37b2bf99900e','daf0a06f-ca33-47f6-bb11-74c99fed99c5','daf0a06f-ca33-47f6-bb11-74c99fed99c5','29ab88df-83c2-442d-9ca7-6e372e96af97','fee4fff6-ee72-4fce-8e15-06c19fa678ad','086c49c6-decb-4d69-88f8-aded4432aca9') ) 
    ORDER BY a.LevelCode
    </asp:textbox>
      
        </td>
        <td>
              <asp:Button ID="Button3" runat="server" Text="插入缓存" onclick="Button3_Click" />
        <asp:Button ID="Button4" runat="server" Text="获取缓存" onclick="Button4_Click" />
         <asp:TextBox ID="TextBox12" runat="server"></asp:TextBox>
      <p />
    <label>依赖SQL</label>
     <p />
    <asp:textbox ID="Textbox5" runat="server" TextMode="MultiLine" Width="500px">select ContractGUID from dbo.cb_contract</asp:textbox>
     <p />
    <asp:textbox ID="Textbox6" runat="server" TextMode="MultiLine" Width="500px">select ProjGUID from dbo.p_project</asp:textbox>
        </td>
        </tr>
    </table>
 
   
    
    </div>
    </form>
</body>
</html>

