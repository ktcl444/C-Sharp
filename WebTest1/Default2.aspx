<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default2.aspx.cs" Inherits="Default2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script language=javascript>
         window.onload = function() {
             if (window.dialogArguments != undefined && window) {
                 alert("Is Modal Window!")
             }
             else {
                 alert("Not Modal Window!")
             }
             debugger
         }
     </script>
     
     <script>
     var returnValue = window.showModalDialog('/FrameTemp0.aspx?title=选择公司&height=&filename=/Security/SelectCurrentCompany.aspx&param=Description=%b7%c7%c4%a9%bc%b6%b9%ab%cb%be&isLogin=false','','dialogWidth:360px; dialogHeight=500px; status:no; help:no; resizable:yes;');
     try{if (typeof (top) != 'undefined' && typeof (top.all.menu) != 'undefined' && typeof (top.all.menu.refreshByBU) == 'function'){ top.all.menu.refreshByBU(returnValue); } else { window.location = window.location; }}catch(e){}</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    </div>
    </form>
</body>
</html>
