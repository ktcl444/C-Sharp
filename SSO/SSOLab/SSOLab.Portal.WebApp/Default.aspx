<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SSOLab.Portal.WebApp._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>企业门户系统</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>
            企业门户系统</h1>
        <div>
            当前登录用户：<asp:Label ID="lblUserID" runat="server" />
        </div>
        <div>
            <h1>
                业务系统</h1>
            <div>
                <a href="http://localhost:7773/App1/Default.aspx" target="_blank">人力资源管理系统</a></div>
            <div>
                <a href="http://localhost:7774/App2/Default.aspx" target="_blank">财务管理系统</a></div>
            <div>
                <a href="http://localhost:8080/App3/default.jsp" target="_blank">网上办公系统</a></div>
        </div>
    </div>
    </form>
</body>
</html>
