<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppDateTimeTest.aspx.cs" Inherits="Map.Web.Test.AppDateTimeTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery.js" type="text/javascript"></script>
    <script src="../Scripts/Common.js" type="text/javascript"></script>
    <script type="text/javascript">
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
    <table id="AppDateTime">
            <tr>
                <td class="Title">
                    AppDateTime测试
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divAppDateTime" runat="server">
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div>
                        <input id="Button7" type="button" value="VisibleTrue" onclick="SetVisible('True')" />
                        <input id="Button8" type="button" value="VisibleFalse" onclick="SetVisible('False')" />
                        <input id="Button9" type="button" value="CheckedTrue" onclick="SetValue('1')" />
                        <input id="Button10" type="button" value="CheckedFalse" onclick="SetValue('0')" />
                        <input id="Button11" type="button" value="DisabledTrue" onclick="SetDisabled('True')" />
                        <input id="Button12" type="button" value="DisabledFalse" onclick="SetDisabled('False')" />
                        <input id="Button1" type="button" value="RequireTrue" onclick="SetRequired('True')" />
                        <input id="Button2" type="button" value="RequireFalse" onclick="SetRequired('False')" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
