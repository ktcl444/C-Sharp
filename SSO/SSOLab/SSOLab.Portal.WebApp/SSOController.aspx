<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SSOController.aspx.cs"
    Inherits="SSOLab.Portal.WebApp.SSOController" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="visibility: hidden">
        <asp:TextBox ID="sso_signinurl" runat="server" />
        <asp:TextBox ID="sso_signouturl" runat="server" />
        <asp:TextBox ID="sso_userinfo" runat="server" />
        <asp:TextBox ID="isSubmit" runat="server" Text="0" />
        <asp:Button ID="btnSubmit" runat="server" Text="自动提交" />
    </div>
    </form>

    <script type="text/javascript" src="http://localhost:7771/SSOSite/SSOContext.aspx?app=portal">
    </script>

    <script type="text/javascript">
        
        if (document.getElementById("isSubmit").value != "1") {            
            document.getElementById("sso_signinurl").value = ssoContext.signInUrl;
            document.getElementById("sso_signouturl").value = ssoContext.signOutUrl;
            document.getElementById("sso_userinfo").value = ssoContext.userInfo;
            document.getElementById("isSubmit").value = "1";
            document.getElementById("form1").submit();
        }
                  
    </script>

</body>
</html>
