<%@ page language="java" contentType="text/html; charset=utf-8"
    pageEncoding="utf-8"%>
<%@ page import="javax.crypto.spec.DESKeySpec" %>
<%@ page import="javax.crypto.SecretKey" %>
<%@ page import="javax.crypto.SecretKeyFactory" %>
<%@ page import="javax.crypto.Cipher" %>
<%@ page import="org.apache.commons.codec.binary.Base64" %>
<%@ page import="org.dom4j.Document" %>
<%@ page import="org.dom4j.DocumentHelper" %>
<%@ page import="org.dom4j.Element" %>

<%	
	if (request.getParameter("isSubmit")!=null && request.getParameter("isSubmit").equals("1")) {		
		String encryptedUserInfo = request.getParameter("sso_userinfo");		
		String decryptedUserInfo = "";		
	    String ssoKey = "XJbbaaAnnQC67829OLkEKwgwiZL30oegpTbptQG0SLQG97665k4O32bb5CQdnffggufXJmBW16nZesssc2AOJl6bO0wiZLiu7k7FTbq27d0CdUG9110ykINvggh5CRjn";
	    //out.println("encryptedUserInfo:"+encryptedUserInfo+"<br />");	
	    
	    try {	    	
	    	Base64 objBase64 = new Base64();   
	        byte[] inputByteArray = objBase64.decode(encryptedUserInfo.getBytes());   
	        
			DESKeySpec objDesKeySpec = new DESKeySpec(ssoKey.substring(ssoKey.length()/2-1, ssoKey.length()/2-1+8).getBytes("ASCII"));
			SecretKeyFactory objKeyFactory = SecretKeyFactory.getInstance("DES");   
			SecretKey objSecretKey = objKeyFactory.generateSecret(objDesKeySpec);   
		
			Cipher objCipher = Cipher.getInstance("DES/ECB/NoPadding");   
			objCipher.init(Cipher.DECRYPT_MODE, objSecretKey);   
			decryptedUserInfo = new String(objCipher.doFinal(inputByteArray), "UTF-8"); 
			out.println("decryptedUserInfo:"+decryptedUserInfo+"<br />");
			
			Document xmlDoc = DocumentHelper.parseText(decryptedUserInfo.trim());			
			Element root = xmlDoc.getRootElement();	
			
			if (root.element("islongin").getText().equals("true"))
            {				
				request.getSession().setAttribute("userSession", root.element("username").getText());
            	response.sendRedirect("default.jsp");
            }
            else
            {            	
            	String returnUrl=  request.getScheme() + "://"
				+ request.getServerName() + ":" + request.getServerPort()
				+ request.getContextPath() + "/default.jsp";
				
            	response.sendRedirect(request.getParameter("sso_signinurl") + "?ReturnUrl=" + response.encodeURL(returnUrl));
            }
		} catch (Exception ex) {			
			out.println("Exception:"+ex.getMessage()+"<br />");
		}	
	}
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=GB18030">
<title></title>
</head>
<body>
    <form id="form1" name="form1" method="post" action="ssoController.jsp">
    <div style="visibility:hidden">
        <input type="text" id="sso_signinurl" name="sso_signinurl" />
        <input type="text" id="sso_signouturl" name="sso_signouturl" />
        <input type="text" id="sso_userinfo" name="sso_userinfo" />
        <input type="text" id="isSubmit" name="isSubmit" value="0" />
        <input type="submit" id="btnSubmit" name="btnSubmit" value="自动提交"/>
        <script type="text/javascript" src="http://localhost:7771/SSOSite/SSOContext.aspx?app=app3">
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
    </div>
    </form> 
</body>
</html>