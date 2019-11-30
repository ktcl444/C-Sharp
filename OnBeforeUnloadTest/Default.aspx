<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>
<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE8"/>
    <title></title>
    <script language=javascript>
        window.onbeforeunload = function() { 
            event.returnValue = "OnBeforUnloadTest"
        }

        function ShowModal() {
            window.showModalDialog("Default2.aspx", "", "dialogWidth:" + 500 + "px;dialogHeight:" + 340 + "px; status:0; help:0; resizable:0; center:1;");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <input type="button" name="test" onclick ="ShowModal();"  value="显示模式窗口"/>
     <input type="button" value="IE Document Compatibility Mode" onclick="javascript:alert(document.documentMode)"/>
    </div>
    </form>
</body>
</html>
