<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language=javascript>
        function Trim(s) {
            //return s.replace(/^\s+|\s+$/g,"");
            //处理全角空格
            return s.replace(/^[\s　]+|[\s　]+$/g, "");
        }

        function removeQueryString(sURL, sParamName) {
            if (Trim(sURL) == "" || Trim(sParamName) == "")
                return sURL;

            var arrParam;
            var sParamValue;

            if (sURL.indexOf("?") != -1) {
                arrParam = sURL.substring(sURL.indexOf("?") + 1).split("&");
                sURL = sURL.substring(0, sURL.indexOf("?"));
                var obj = new Object();
                for (var i = 0; i < arrParam.length; i++) {
                    if (arrParam[i].indexOf(sParamName + "=") != 0) {
                        obj[i] = arrParam[i];
                    }
                }
                var newQuery = "";
                for (var key in obj) {
                    newQuery = newQuery + obj[key] + "&";
                }
                if (newQuery.length != 0) {
                    newQuery = newQuery.substring(0, newQuery.length - 1);
                    sURL = sURL + "?" + newQuery;
                }
            }

            return sURL;
        }

        function TestRemoveQueryString() {
            var url = "http://test.com?Page=http://222.com?func=2";
            alert(url);
            alert(removeQueryString(url, "func"));

            var url = "http://test.com?Page=222";
            alert(url);
            alert(removeQueryString(url, "Page"));

            var url = "http://test.com?Test=2&Page=222";
            alert(url);
            alert(removeQueryString(url, "Page"));

            var url = "http://test.com?Page=222&Test=2";
            alert(url);
            alert(removeQueryString(url, "Page"));

            var url = "http://test.com?Test=2&Page=222&Test2=3";
            alert(url);
            alert(removeQueryString(url, "Page"));
        }

        function TestSetIframeSrc() {
            if (typeof (document.all.menu) != "undefined") {
                document.all.menu.src = "Default2.aspx";
            }
            else {
                document.location = 'Default3.aspx';
            }
        }

        window.onload = function Test() {


            //            if (typeof (parent) != "undefined") {
            //                alert("0");
            //            }
            //            if (typeof (showPopup) != "function") {
            //                alert("0");
            //            }

            //            if (typeof (top) != "undefined" && typeof (top.frames("menu")) != "undefined" && typeof (top.frames("menu").refreshByBU) != "function") { top.frames("menu").refreshByBU(returnValue); } else { window.location = window.location; }
            //            if (typeof (parent.frames("menu")) != "undefined") {
            //                alert("0");
            //            }
            //            try {
            //                if (parent.frames("menu") != undefined) {
            //                    alert("0");
            //                }
            //                else {
            //                    alert("1");
            //                }
            //            }
            //            catch (e) {
            //                alert(e.message);
            //            }
        }
        function Test() {
            var url = "http://ekp.mysoft.net.cn:1000/DefaultEKP.aspx?RemoveSession=1&page=http%3a%2f%2fforum.mysoft.net.cn%3a1100%2findex.aspx";
            alert(url);
            alert(url.replace("RemoveSession=1", "RemoveSession=0"));
            url = window.location.href;
            alert(url);
            alert(url.replace("RemoveSession=1", "RemoveSession=0"));
            window.location = url.replace("RemoveSession=1", "RemoveSession=0");

        }

        function OpenModal() {
            window.showModalDialog("Default2.aspx");
        }


        function Open() {
            window.open("Default2.aspx");
        }

        function TestWrite() { 
                    var index = 1;
            document.writeln("<input type='button' id='Test' name='Test' value=" +index + " />");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <input type=button onclick ="OpenModal()" value="Test"/>
        <input type=button onclick ="Open()" value="Test2"/>
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Button" />
    
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        <asp:TextBox ID="TextBox1" runat="server" TextMode="MultiLine" Width="837px"></asp:TextBox>
    </div>
    <IFRAME id="menu"  width="100%" height="100"> </IFRAME>
    </form>
</body>
</html>
