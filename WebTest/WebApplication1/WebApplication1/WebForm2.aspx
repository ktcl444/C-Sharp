<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="WebApplication1.WebForm2" %>

<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head >
<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE8"/>
    <title></title>
<script language=javascript>
//    function window.onbeforeunload() {
//       
//    }

    function OK()
    {
        //        window.returnValue = "222";
        //        window.close();
//        if (isNaN(9999999999999999999999999))
//        {
//            alert("1");
//        }
//        else
//        {
//            alert("2");
        //        }
        var s = document.getElementById("test").value;
        alert(parseFloat(s));
    }
</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table width="100%" border="1px" cellSpacing=0 cellPadding=0   >
    <tr>
    <td>
    <input type=button value ="返回" onclick ="OK();" />
     <input type=text id="test" />
    </td>
    <td ></td>
    <td></td>
    <td></td>
    <td></td>
    </tr>
    <tr>
    <td rowspan="12"></td>
    <td ></td>
    <td></td>
    <td></td>
    <td></td>
    </tr>
    <tr>
    <td ></td>
    <td></td>
    <td></td>
    <td></td>
    </tr>
    <tr>
    <td  rowspan="3"></td>
    <td rowspan="3"></td>
    <td></td>
    <td></td>
    </tr>
    <tr>
    <td></td>
    <td></td>
    </tr>
    <tr>
    <td></td>
    <td></td>
    </tr>
    <tr>
    <td  rowspan="2"></td>
    <td rowspan="2"></td>
    <td></td>
    <td></td>
    </tr>
    <tr>
    <td></td>
    <td></td>
    </tr>
    <tr>
    <td ></td>
    <td></td>
    <td colspan="2"></td>
    </tr>
    <tr>
    <td ></td>
    <td></td>
    <td></td>
    <td></td>
    </tr>
    <tr>
    <td  rowspan="3"></td>
    <td rowspan="2"></td>
    <td></td>
    <td></td>
    </tr>
    <tr>
    <td></td>
    <td></td>
    </tr>
    <tr>
    <td></td>
    <td></td>
    <td></td>
    </tr>
    </table>
    </div>
    </form>
</body>
</html>
