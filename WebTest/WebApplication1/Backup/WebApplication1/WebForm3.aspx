<%@ Page Language="C#"  AutoEventWireup="true" CodeBehind="WebForm3.aspx.cs" Inherits="WebApplication1.WebForm3" %>

<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head >
 <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE8"/>
    <title></title>
    <script language =javascript>
        window.onload = function() {
//            debugger;
//            alert(document.charset);
//            alert(document.language);

        }
        function ChangeLabel() {
            alert(document.getElementById("Label1").innerText);
            document.getElementById("Label1").innerText = "Test";
            document.getElementById("TextBox2").innerText = "hhggg";
            document.getElementById("HiddenField1").Value = "Changed";
            document.getElementById("Input1").Value = "Changed";
            var o = document.getElementById("Select1");
            var option = document.createElement('option');

            option.text = '4';
            option.value = "4";
            //oS.insertBefore(option);

            o.options.add(option);
        }
    function ShowLabel() {
        alert(document.getElementById("Label1").value);
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <input type=button onclick ="ChangeLabel();" value = "ChangeLabel" />
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click"/>
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    <input id="Input1" type="hidden" runat=server value ="Init" />
    <asp:HiddenField ID="HiddenField1" Value ="Init"
        runat="server" />
        <select id="Select1" runat =server >
        <option value="1">1</option>
        <option value="2">2</option>
        <option value="3">3</option>
        </select>
        <asp:DropDownList ID="DropDownList1" runat="server">
        </asp:DropDownList>
        <asp:textbox ID="TextBox2" runat="server"></asp:TextBox>
        
        <input type=text runat=server id="InputText" />
        <asp:Button c ID="Button2" runat="server" Text="Button" 
            onclick="Button2_Click" />
        <textarea runat =server id="TextArea" ></textarea> 
    </div>
    </form>
</body>
</html>
