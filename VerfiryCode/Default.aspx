<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script language="javascript">

        function Change()
        {
            var s = document.getElementById("DropDownList1").value;
            document.getElementById("Test").src = path + "?" + Math.random() + "&type=" + s;
        }
        var path = "Validate.aspx";
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        验证码:<asp:TextBox ID="Chkcode" runat="server" Columns="4" MaxLength="4" />
        &nbsp;
        <img alt="看不清,下一张" id="Test" src="Validate.aspx"  onclick="Change(this.src);"  />
    </div>
    <asp:Button ID="Button1" runat="server" Text="验证" OnClick="Button1_Click" />
    <p />
    验证码方案
    <asp:DropDownList ID="DropDownList1" runat="server" 
        onchange="Change()">
    <asp:ListItem Text ="方案1">1</asp:ListItem>
    <asp:ListItem Text ="方案2">2</asp:ListItem>
    <asp:ListItem Text ="方案3">3</asp:ListItem>
    </asp:DropDownList>
    </form>
</body>
</html>
