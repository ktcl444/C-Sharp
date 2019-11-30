<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
<script language="javascript">
    function Test() {
        var table = document.getElementById("table1");
        var row = table.rows[0];
        var td = row.cells[0];
        td.style.border = "0px";

        var i = 0.0000000001;
        alert(i.toString());
    }

    function Test2() {
        var table = document.getElementById("table1");
        var row = table.rows[0];
        var td = row.cells[0];
        td.style.borderStyle = "";
        td.style.borderWidth = "1px";
        
        
//        		td.style.borderBottomStyle="";
//        		td.style.borderBottomWidth = "1px";
//        		td.style.borderLeftStyle="";
//        		td.style.borderLeftWidth = "1px";
//        		td.style.borderRightStyle="";
//        		td.style.borderRightWidth = "1px";
//        		td.style.borderStyle="";
//        		td.style.borderWidth = "1px";
//        		td.style.borderTopStyle="";
//        		td.style.borderTopWidth = "1px";
//        		td.style.border = "1px";

    }

    function Test3() {
        if (confirm("该操作将覆盖标书文档，是否继续？")) {
            txtTest.value = true;

        }
        else {
            txtTest.value = false;
        }


    }

    function Test5() 
    {
        alert(GetModeByState("Add"));
        alert(GetModeByState("Edit"));
        alert(GetModeByState("Look"));
    }

    function GetModeByState(sState)
    { 
        switch (sState.toUpperCase())
        {
        case "ADD":
        return "1";
        case "EDIT":
         return "2";
        case "LOOK":
         return "3";
         }
    }

    function Test4() {
        var i = -0.044;
        var j = 0.05;
//        var k = i + parseFloat(j);
        //        k = int(k * 10000) / 10000
        alert(calcDoubleFix(i, j, '+'));
        alert(calcDoubleFix(i, j, '-'));
        alert(calcDoubleFix(i, j, '*'));
        alert(calcDoubleFix(i, j, '/'));  
//        SetText(k);
    }

    function Test6()
    {
        var strUrl = location.href + "?serviceNode=1";
        alert(strUrl.replace(/serviceNode=(.)+$/i, ""));
    }

    function changLinkByServiceNode(xmlName, serviceNode)
    {
        var strUrl = location.href;
        if (xmlName != undefined && xmlName.length != 0)
        {
            strUrl = strUrl.indexOf("?") < 0 ? strUrl + "?" : strUrl;
            strUrl = strUrl.replace(/xml=(.)+$/i, "") + "xml=" + xmlName + ".xml";
        }
        if (serviceNode != undefined && serviceNode.length != 0)
        {
            strUrl = strUrl.indexOf("?") < 0 ? strUrl + "?" : strUrl + "&";
            strUrl = strUrl.replace(/serviceNode=(.)+$/i, "") + "serviceNode=" + serviceNode;
        }
        return strUrl;
    }

    function SetText(k) {
        document.all("Textbox1").value = k;
    }
//    function window.onbeforeunload()
//    {
//        alert("Default_onbeforeunload");
//    }
//    function window.onunload()
//    {
//        var s = GetDataByXMLHTTP('', 'test2');
//        alert("onunload");
//    }

    function window.onunload()
    {
        //var s = GetDataByXMLHTTP('', 'Default_onunload');
        //alert("Default_onunload");
    }

    function window.onbeforeunload()
    {
       // alert("Default_onbeforeunload");
//        var oldDate = new Date();
//        var s = "";
//        for (var i = 0; i < 2000000; i++)
//        {
//            s += i;
//        }
//        var newDate = new Date();
       // alert(newDate.getSeconds() - oldDate.getSeconds());
       // var s = GetDataByXMLHTTP('', 'Default_onbeforeunload');
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

    function calcDoubleFix(m, n, op) {
        var a = (m.toString(10));
        var b = (n.toString(10));
        var x = 1;
        var y = 1;
        var c = 1;
        var returnValue;
        if (a.indexOf(".") > 0) {
            x = Math.pow(10, a.length - a.indexOf(".") - 1);
        }
        if (b.indexOf(".") > 0) {
            y = Math.pow(10, b.length - b.indexOf(".") - 1);
        }
        switch (op) {
            case '+':
                c = Math.max(x, y);
                m = Math.round(m * c);
                n = Math.round(n * c);
                returnValue = (m + n) / c;
                break;
            case '-':
                c = Math.max(x, y);
                m = Math.round(m * c);
                n = Math.round(n * c);
                returnValue = (m - n) / c;
                break;
            case '*':
                c = x * y
                m = Math.round(m * x);
                n = Math.round(n * y);
                returnValue = (m * n) / c;
                break;
            case '/':
                c = Math.max(x, y);
                m = Math.round(m * c);
                n = Math.round(n * c);
                c = 1;
                returnValue = m / n;
                break;
        }
        return returnValue;
    }

    function TestNull()
    {
        if ("" == undefined)
        {
            alert("OK");
        }
    }

    function TestTextBoxNull()
    {
        if (TextBox1.value)
        {
            alert("有值");
        }
        else
        {
            alert("没有值");
        }
    }

    function TestRadion() {
        alert(sex.value);
    }
    window.onload = function() {
    //alert(form1.Textbox1.value);
}

function TestOption() {
    var optionValue = form1.optionValue.value;
    alert(optionValue);
    var select1 = form1.select1;
    for (var i = 0; i < select1.options.length; i++) {
        if (select1.options[i].text == optionValue) {
            select1.options[i].selected = true;
            break;
        }
    }
}


</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <table id="table1" width="100%" border="1px">
            <tr>
                <td >
                    &nbsp;<asp:Button ID="Button3" runat="server" Text="AddCookie" /></td>
                <td>
                    &nbsp;<asp:Button ID="Button4" runat="server" Text="ClearCookie" /></td>
            </tr>
            <tr>
                <td>
                <asp:textbox id="Textbox1"  runat="server"></asp:textbox>
                    &nbsp;<asp:textbox id="txtTest" style="DISPLAY: none" runat="server"></asp:textbox></td>
                    
                <td>
                    &nbsp;</td>
                    <td>
                     
                    </td>
            </tr>
            <tr>
            <td colspan =2><span style="width:200px" >2222</span></td>
            </tr>
            <tr>
                <td colspan=3>
                 <iframe src=WebForm1.aspx scrolling=auto height=50px width=100%></iframe>
                </td>
            </tr>
        </table>
         <input id="Button1" type="button" value="button" onclick ="Test();"/></p>
         <input id="Button2" type="button" value="button" onclick ="Test2();"/></p>
            <input id="Button5" type="button" value="Test" onclick ="Test3();"/></p>
            <input value="Button" type="button" onclick="Test4();"/>
            <input value="TestGetModeByState" type="button" onclick="Test5();"/>
             <input value="TestReplace" type="button" onclick="Test6();"/>
              <input value="TestClose" type="button" onclick="Test7();"/>
                <input value="TestNull" type="button" onclick="TestNull();"/>
                
                 <input value="TestTextBoxNull" type="button" onclick="TestTextBoxNull();"/>
                 <lable><input type="radio" name="Sex" value="男" 
        checked="checked" />男</lable>
                 <lable><input type="radio" name="Sex" value="女" />女</lable>
         <input id="Button6" type="button" value="button" onclick ="TestRadion();"/>
         <select id="select1">
            <option value="1" selected >1</option>
            <option value="2">2</option>
            <option value="3">3</option>
         </select>
         <input id="optionValue" type=text />
       <input id="Button7" type="button" value="TestOption" onclick ="TestOption();"/>
          <asp:Button ID="Button8" runat="server" Text="TestReg" />
           <asp:textbox id="txtCacheKey"  runat="server"></asp:textbox>
    </div>
    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
    <asp:Button ID="Button9" runat="server" onclick="Button9_Click" Text="Button" />
    <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
    <p>
        &nbsp;</p>
    <asp:TextBox ID="txtDocName" runat="server"></asp:TextBox>
    <asp:TextBox ID="txtExpName" runat="server"></asp:TextBox>
    <asp:Button ID="btnCheckExp" runat="server" onclick="btnCheckExp_Click" 
        Text="Button" />
    <asp:TextBox ID="txtNewDocName" runat="server"></asp:TextBox>
    <p>
        <asp:Button ID="btnTestCreateDirectory" runat="server" 
            onclick="btnTestCreateDirectory_Click" Text="测试生成文件夹" />
        <asp:Button ID="btnTestDeleteDirectory" runat="server" 
            onclick="btnTestDeleteDirectory_Click" Text="测试删除文件夹" />
        <asp:TextBox ID="txtSessionID" runat="server"></asp:TextBox>
    </p>
    <p>
   
        <asp:TextBox ID="txtFilePath" runat="server">c:\test.xlsx</asp:TextBox>
        <asp:TextBox ID="txtNewFilePath" runat="server">c:\test_new.xlsx</asp:TextBox>
    <asp:Button ID="btnExcelPermission" runat="server" onclick="btnExcelPermission_Click" 
        Text="Button" />
        <asp:TextBox ID="txtUserName" runat="server">kongy@mysoft.com.cn</asp:TextBox>
        <asp:TextBox ID="txtExcelPermissionResult" runat="server"></asp:TextBox>
    <p>
   
        <asp:TextBox ID="TextBox4" runat="server">mysoft.com.cn</asp:TextBox>
        <asp:TextBox ID="TextBox5" runat="server">kongy</asp:TextBox>
        <asp:TextBox ID="TextBox6" runat="server">ky19830816</asp:TextBox>
         <asp:TextBox ID="TextBox7" runat="server">3</asp:TextBox>
          <asp:TextBox ID="TextBox8" runat="server">0</asp:TextBox>
    <asp:Button ID="btnExcelPermission0" runat="server"  
        Text="Button" onclick="btnExcelPermission0_Click" />
        </p>
    </form>
       
</body>
</html>
