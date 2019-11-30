<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppHiddenTest.aspx.cs"
    Inherits="Map.Web.Test.AppHiddenTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery.js" type="text/javascript"></script>
    <script src="../Scripts/Common.js" type="text/javascript"></script>
    <script type="text/javascript">
        function setHiddenValue() {
            section2_Hidden_Ctrl.setValue($("#Text1").val());
        }
        function showHiddenValue() {
            alert(section2_Hidden_Ctrl.getValue());
        }
    </script>

    <style type="text/css">
        .Title
        {
         	font-size:xx-large;
        	font-weight:bolder;
        	color:Red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table id="AppHidden">
            <tr>
                <td class="Title">
                    AppHidden测试
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divAppHidden" runat="server">                    
                        <span><asp:Label ID="Label1" runat="server" Text="HiddenValue"></asp:Label>
                        <input id="Text1" type="text"  value="1234567890<>,!@#$%^&&*()" style="width:500px;"/></span>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div>
                        <input id="Button9" type="button" value="setHiddenValue" onclick="setHiddenValue()" />
                        <input id="Button10" type="button" value="showHiddenValue" onclick="showHiddenValue()" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
