<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebApplication1.Index" %>

<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head >
<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE8"/>
    <title></title>
    <script type="text/javascript"  language="javascript" >

        function ShowModal() {
            window.showModalDialog("WebForm2.aspx", "", "dialogWidth:" + 500 + "px;dialogHeight:" + 340 + "px; status:0; help:0; resizable:0; center:1;");
//           var s =  window.showModalDialog("WebForm2.aspx", "", "dialogWidth:" + 500 + "px;dialogHeight:" + 340 + "px; status:0; help:0; resizable:0; center:1;");
            //           document.getElementById("Text1").value = s;
          //  alert(window.test);
            //window.attachEvent("onbeforeunload", ConfirmClose);
        }

        function ConfirmClose() {
            window.event.returnValue = "111";
        }

        window.onload = function() {
        var s = calcDoubleFix2(89.1, 95.45, "*", 2);
        alert(s);
            var className;
            var val = undefined;
            className = (val) ? "num ro2" : "num";
            window.test = "222";
            // alert(className);
            //Test(this);
            //document.getElementById("Text1").onclick = function() { Test(this); };
        }
        var _var1 = 20;
        function Test(oThis) {
//            var a = oThis._var1;
//            var b = 2;
//            var c = 3;
//            alert(bal(a + "+"+  b + "+" + c));
            // TestFrame.location.replace("WebForm1.aspx");
            setInterval(code, 3000);
        }

        window.onunload = function() {
            //var s = GetDataByXMLHTTP('', 'Index_onunload');
            //alert("Index_onunload");

            //event.returnValue = "-----------------------------------------------------------------------\n确定后，将不保存当前记录，直接关闭当前页面。\n-----------------------------------------------------------------------";
   
        }

        window.onbeforeunload = function() {
            //alert("Index_onbeforeunload");
//            TestFrame.location.href = "about:blank";
            //TestFrame.location.replace("WebForm1.aspx");
            //TestFrame.location.href="WebForm1.aspx";

        
            //alert("Index_onbeforeunload");
            //var s = GetDataByXMLHTTP('', 'Index_onbeforeunload');
            event.returnValue = "-----------------------------------------------------------------------\n确定后，将不保存当前记录，直接关闭当前页面。\n-----------------------------------------------------------------------";
        }

        function bal(s) {
            var evalString = returnEvalString(s);

            return eval(evalString);
        }

        function returnEvalString(s) {
           //return "calcDoubleFix(calcDoubleFix(1,2,'+'),3,'+')";
            return GetDataByXMLHTTP('', 'returnEvalString', escape(s).replace(/\+/g,"pp"));
        }

        //通过XMLHTTP通道不刷新页面从后台取数
        //参数：sFile					-	打开文件
        //		strBusinessType			-	业务对象
        //		strBusinessInfo			-	业务信息
        //		strPostStream			-	大容量业务参数
        function GetDataByXMLHTTP(strFile, strBusinessType, strBusinessInfo, strPostStream) {
            if (strFile == undefined || strFile == "") strFile = "/Pub_XmlHttp.aspx"
            var rdNum = Math.random();
            var oHTTP = new ActiveXObject("Msxml2.XMLHTTP");
            var sUrl = strFile + "?p_businessType=" + escape(strBusinessType) + "&p_businessInfo=" + escape(strBusinessInfo) + "&rdnum=" + rdNum;

            if (strPostStream == undefined) {
                oHTTP.open("GET", sUrl, false);
                oHTTP.send();
            }
            else {
                oHTTP.open("POST", sUrl, false);
                oHTTP.send(strPostStream);
            }

            var bSuccess = handleXMLErr(oHTTP.responseXML);
            if (bSuccess) {
                if (oHTTP.responseText == "服务器超时") window.navigate("/ErrPage.aspx?errid=001");
                return oHTTP.responseText;
            }
            else {
                alert("操作失败，请关闭重试！");
                return "-1";
            }
        }

        var ERROR_STOP = 0;
        var ERROR_NONE = 1;
        var ERROR_CONTINUE = 2;
        function handleXMLErr(xml, bContinue) {
            if (bContinue == null) bContinue = false;
            if (xml.parseError.errorCode != 0) {
                //alert("XML Parser Error: " + xml.parseError.reason);
                alert("XML解析错误: " + xml.parseError.reason);
                if (!bContinue) {
                    return ERROR_STOP;
                }
                else {
                    return ERROR_CONTINUE;
                }
            }
            var node = xml.selectSingleNode("/error");
            if (node) {
                if (!bContinue) {
                    //openStdDlg("/_common/error/dlg_error.aspx?hresult=" + node.selectSingleNode("number").text, null, 400, 200);
                    alert("发生运行时错误！");
                    return ERROR_STOP;
                }
                else {
                    return ERROR_CONTINUE;
                }
            }
            return ERROR_NONE;
        }

        //Add by kongy 2010-1-27
        //解决浮点运算精度丢失问题
        //参数：    m:第一个运算值
        //          n:第二个运算值
        //          op:运算符(仅支持“+”，“-”，“*”，“/”)
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


        //Add by kongy 2010-1-27
        //解决浮点运算精度丢失问题
        //参数：    num1:第一个运算值
        //          num2:第二个运算值
        //operation：运算符，只支持加减乘除
        //round：小数位，不能超过6位
        //产品开发部 - 张旭灿 2011-08-16 18:13:01 
        //operation：运算符，只支持加减乘除
        //round：小数位，不能超过6位
        function calcDoubleFix2(num1, num2, operation, round) {
            var fltNum1 = (num1.toString().replace(/,/g, "")); //去掉千分位中的","
            var fltNum2 = (num2.toString().replace(/,/g, ""));
            var intNum1Digit = 1, intNum2Digit = 1, intDigit = 1;   //需要放大多少位

            var blnNumber = !isNaN(fltNum1) && fltNum1 != "" && !isNaN(fltNum2) && fltNum2 != ""; //判断是否是数值，包括字符窜的数值
            if (blnNumber) {
                var returnValue;
                if (fltNum1.indexOf(".") > 0) {
                    intNum1Digit = Math.pow(10, fltNum1.length - fltNum1.indexOf(".") - 1); //数1需要放大多少小数位
                }
                if (fltNum2.indexOf(".") > 0) {
                    intNum2Digit = Math.pow(10, fltNum2.length - fltNum2.indexOf(".") - 1); //数2需要放大多少小数位
                }
                if (operation == '*') {
                    intDigit = intNum1Digit * intNum2Digit;
                    num1 = Math.round(parseFloat(fltNum1) * intNum1Digit);   //放大成整数进行运算
                    num2 = Math.round(parseFloat(fltNum2) * intNum2Digit);
                }
                else {
                    intDigit = Math.max(intNum1Digit, intNum2Digit);
                    num1 = Math.round(parseFloat(fltNum1) * intDigit);   //放大成整数进行运算
                    num2 = Math.round(parseFloat(fltNum2) * intDigit);
                }

                switch (operation) {
                    case '+':
                        returnValue = (num1 + num2) / intDigit;
                        break;
                    case '-':
                        returnValue = (num1 - num2) / intDigit;
                        break;
                    case '*':
                        returnValue = (num1 * num2) / intDigit;
                        break;
                    case '/':
                        returnValue = num1 / num2;
                        break;
                    default:
                        throw new Error("calcDoubleFix只支持加减乘除运算");
                }
                //大余6位小数位会以指数形式显示
                if (round && round < 6) {
                    var pow = Math.pow(10, round);
                    return Math.round(returnValue * pow) / pow;
                }
                return Math.round(returnValue * 1000000) / 1000000;
            }
            else {
                switch (operation) {
                    case '+':
                        return num1 + num2;
                    default:
                        throw new Error("calcDoubleFix不支持字符串的乘除减运算");
                }
            }
        }


    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label><input onclick="Test()" value="30" id="Text1"
            type="text" value="3879" />
            <input type="button" name="test" onclick ="ShowModal();"  value="显示模式窗口"/>
    </div>
    </form>
</body>
</html>
