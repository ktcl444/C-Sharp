<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TestIframeCrossArea._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
  <script language="javascript" type="text/javascript">document.domain = 'mysoft.com';</script> 
    <script language="javascript" type="text/javascript">
    function Test()
    {
//        var fm = window.document.createElement("iframe");
//        fm.id = "w";
//        fm.src = "Default.htm";
//        document.getElementById("box").appendChild(fm);
//        
    }

    function Test2() {
        var ifra;
        if ((ifra = document.getElementById('w')) == null) {
            ifra = document.createElement('iframe');
            ifra.src = 'http://testcroosarea.mysoft.com:800/Default.htm'; ;
            ifra.id = 'w';
            document.body.appendChild(ifra);
        }
    
        setTimeout(function() {
        document.getElementById("w").contentWindow.document.write("小马哥");
        }, 3000);
    }
    </script>

</head>
<body>
    <div id="box">
        <input onclick="Test();" type="button" value="测试" />
        <input onclick="Test2();" type="button" value="测试2" />
         <input runat=server type="button" value="测试3" /></div>
</body>
</html>
