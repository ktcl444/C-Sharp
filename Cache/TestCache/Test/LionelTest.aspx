<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LionelTest.aspx.cs" Inherits="Map.Web.LionelTest" %>

<%@ Register assembly="Mysoft.Map.WebControls" namespace="Mysoft.Map.WebControls" tagprefix="cnt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <cnt:AppTextBox ID="AppTextBox1" runat="server" />
    <cnt:AppHidden ID="AppHidden1" runat="server" />
    <br />
    <br />
    <br />
    <br />
    <textarea id="txtCode" name="txtCode" style="width: 600px; height: 200px" cols="100" rows="10"></textarea>
    <br />
    <input id="btnExec" type="button" value="执行代码" onclick="window.execScript(txtCode.value)" />
    </form>
</body>
</html>
