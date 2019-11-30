<%@ PAGE Language="C#" AutoEventWireup="true" CodeBehind="AllanTest.aspx.cs" Inherits="Map.Web.Test.AllanTest" %>

<%@ REGISTER Assembly="Mysoft.Map.WebControls" Namespace="Mysoft.Map.WebControls" TagPrefix="cnt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>无标题页</title>
<meta HTTP-EQUIV="Content-Type" CONTENT="text/html; CHARSET=gb2312">
    <script src="../Scripts/jquery.js" type="text/javascript"></script>

    <link href="../App_Themes/Default/SimpleControl.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <table width="100%">
    <colgroup>
    <col width="30%"/><col nowrap/>
    </colgroup>
        <tr>
            <td >
                <CNT:APPDATETTIEM ID="AppDate1" runat="server">
                </CNT:APPDATETTIEM>
            </td>
            <td >
                <input type="button" onclick="javascript:AppDate1._EnableTime=true" value="有时间" />
                <input type="button" onclick="javascript:AppDate1._EnableTime=false" value="无时间" />
                <input type=text Id="date1" />
            <input type="button" onclick="javascript:AppDate1.SetValue(date1.value);" value="SetValue" />
            <input type="button" onclick="javascript:alert(AppDate1.GetValue());" value="GetValue" />
            </td>
        </tr>
        <tr>
            <td>
                <CNT:APPNUMBER ID="AppNumber1" runat="server" Aac="4">
                </CNT:APPNUMBER>
            </td>
            <td >
            <input type=text Id="number1" />
            <input type="button" onclick="javascript:AppNumber1.SetValue(number1.value);" value="SetValue" />
            <input type="button" onclick="javascript:alert(AppNumber1.GetValue());" value="GetValue" />
            <input type="button" onclick="javascript:alert(AppNumber1.Validation());" value="Validation" />
                &nbsp;</td>
        </tr>
        <tr>
            <td >
                <CNT:APPNUMBER ID="AppNumber2" runat="server" Aac="2">
                </CNT:APPNUMBER>
            </td>
            <td >
            <input type=text Id="number2" />
            <input type="button" onclick="javascript:AppNumber2.SetValue(number2.value);" value="SetValue" />
            <input type="button" onclick="javascript:alert(AppNumber2.GetValue());" value="GetValue" />
                &nbsp;</td>
        </tr>
    </table>
    <select >
    <option >ddddd</option>
    <option >ddddd</option>
    <option >ddddd</option>
    <option >ddddd</option>
    <option >ddddd</option>
    </select>
    </form>
</body>
</html>
