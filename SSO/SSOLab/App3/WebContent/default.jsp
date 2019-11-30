<%@ page language="java" contentType="text/html; charset=utf-8"
	pageEncoding="utf-8"%>
<%
	if (request.getSession().getAttribute("userSession") == null) {
		response.sendRedirect("ssoController.jsp");
	}
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=GB18030">
<title>网上办公系统</title>
</head>
<body>

<form id="form1">
<div>
<h1>网上办公系统</h1>
<div>
<%
	if (request.getSession().getAttribute("userSession") != null) {
%> 当前登录用户： <%=request.getSession().getAttribute("userSession")
								.toString()%>
<%
	}
%>
</div>
</div>
</form>
</body>
</html>