<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication1.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script language="javascript">
        window.onload = function() {
        var s = "<filter>";
        alert(escape(s));
        }
        function window.onunload() {
          
            //var s = GetDataByXMLHTTP('', '3_onunload');
            //alert("Index_onunload");
        }

        function window.onbeforeunload()
        {
            //alert("Index_onbeforeunload");

            var s = GetDataByXMLHTTP('', '3_onbeforeunload');
        }

        //通过XMLHTTP通道不刷新页面从后台取数
        //参数：sFile					-	打开文件
        //		strBusinessType			-	业务对象
        //		strBusinessInfo			-	业务信息
        //		strPostStream			-	大容量业务参数
        function GetDataByXMLHTTP(strFile, strBusinessType, strBusinessInfo, strPostStream)
        {
            if (strFile == undefined || strFile == "") strFile = "/Pub_XmlHttp.aspx"
            var rdNum = Math.random();
            var oHTTP = new ActiveXObject("Msxml2.XMLHTTP");
            var sUrl = strFile + "?p_businessType=" + escape(strBusinessType) + "&p_businessInfo=" + escape(strBusinessInfo) + "&rdnum=" + rdNum;

            if (strPostStream == undefined)
            {
                oHTTP.open("GET", sUrl, false);
                oHTTP.send();
            }
            else
            {
                oHTTP.open("POST", sUrl, false);
                oHTTP.send(strPostStream);
            }

            var bSuccess = handleXMLErr(oHTTP.responseXML);
            if (bSuccess)
            {
                if (oHTTP.responseText == "服务器超时") window.navigate("/ErrPage.aspx?errid=001");
                return oHTTP.responseText;
            }
            else
            {
                alert("操作失败，请关闭重试！");
                return "-1";
            }
        }

        var ERROR_STOP = 0;
        var ERROR_NONE = 1;
        var ERROR_CONTINUE = 2;
        function handleXMLErr(xml, bContinue)
        {
            if (bContinue == null) bContinue = false;
            if (xml.parseError.errorCode != 0)
            {
                //alert("XML Parser Error: " + xml.parseError.reason);
                alert("XML解析错误: " + xml.parseError.reason);
                if (!bContinue)
                {
                    return ERROR_STOP;
                }
                else
                {
                    return ERROR_CONTINUE;
                }
            }
            var node = xml.selectSingleNode("/error");
            if (node)
            {
                if (!bContinue)
                {
                    //openStdDlg("/_common/error/dlg_error.aspx?hresult=" + node.selectSingleNode("number").text, null, 400, 200);
                    alert("发生运行时错误！");
                    return ERROR_STOP;
                }
                else
                {
                    return ERROR_CONTINUE;
                }
            }
            return ERROR_NONE;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table border="1px" cellSpacing=0 cellPadding=0   >
            <tr>
                <td width="15%">标准角色</td>
                <td width="25%">业务对象</td>
                <td width="10%">操作</td>
                <td width="20%">维度</td>
                <td width="30%">授权</td>
            </tr>
            <tr>
                <td rowspan="12"></td>
           
                 <td>项目(项目相关对象，比如：销售交易、合约规划、合同)</td>
                <td>查询</td>
                <td>项目</td>
                <td></td>
            </tr>
               <tr>
                 <td>合同（合同/非合同）</td>
                <td>发起</td>
                <td>合同类别</td>
                <td></td>
            </tr>
            
               <tr>
                 <td rowspan="3">合同（合同/非合同）</td>
                <td rowspan="3">查询</td>
                <td>组织</td>
                <td></td>
            </tr>
            
               <tr>
                <td>合同类别</td>
                <td></td>
            </tr>
            
               <tr>
                <td><font color="red">项目 </font></td>
                <td><font color="red">同项目“查询”权限 </font></td>
            </tr>
            
               <tr>
                 <td rowspan="2">付款（合同/非合同/日常报销/领借款）</td>
                <td rowspan="2">发起</td>
                <td>付款审批类别</td>
                <td></td>
            </tr>
            
               <tr>
                <td><font color="red">项目 </font></td>
                <td><font color="red">同项目“查询”权限 </font></td>
            </tr>
            
               <tr >
              
                 <td> <font color="red">付款（合同/非合同） </font> </td>
                <td> <font color="red">查询 </font> </td>
                <td colspan="2"> <font color="red">同“合同查询”权限 </font> </td>
               
            </tr>
            
               <tr>
                 <td>付款（日常报销/领借款）</td>
                <td>查询</td>
                <td>组织</td>
                <td></td>
            </tr>
            
               <tr>
                 <td rowspan="3">费用</td>
                <td rowspan="2">发起</td>
                <td>费项</td>
                <td></td>
            </tr>
            
               <tr>
                <td>组织</td>
                <td></td>
            </tr>
            
               <tr>
                <td>查询</td>
                <td>费项</td>
                <td></td>
            </tr>
            
        </table>
        说明：<font color="red">红色</font>字体为关联权限，不需用单独进行设置
    </div>
    </form>
</body>
</html>
