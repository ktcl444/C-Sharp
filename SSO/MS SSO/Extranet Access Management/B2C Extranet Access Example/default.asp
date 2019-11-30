<%
  'Copyright (c) Microsoft Corporation. All rights reserved.

  Response.Expires = -1
  
  Set oMgr = Server.CreateObject("Passport.Manager.1")


  If oMgr.fromNetworkServer Then
    redirURL = "https://"&Request.ServerVariables("SERVER_NAME")&Request.ServerVariables("SCRIPT_NAME")
    Response.Redirect(redirURL)
  End If

Function GetProperDate (strDate)
    getProperDate = Month(strDate) & "/" & Day(strDate) & "/" & Year(strDate)
End Function



%>

<html>
<head>
	<title>Passport/AD Demo Page</title>
</head>
<body BGCOLOR="White" TOPMARGIN="10" LEFTMARGIN="10">

<!-- Display header. -->
		<table border=0>
		<tr>
		<td><IMG alt="Contoso Pharma" src="logo.gif" align="left"></td>
		</tr><br>
		</table>
<%




   thisURL = Server.URLEncode("https://" & Request.ServerVariables("SERVER_NAME") & Request.ServerVariables("SCRIPT_NAME") )
   signoutURL = Server.URLEncode("https://" & Request.ServerVariables("SERVER_NAME") & "signout.asp")

   TimeWindow = 3600
   
   If oMgr.IsAuthenticated(TimeWindow,FALSE) Then 
 
      '
      ' Display signout since user is logged in
      '
      Response.Write( "<div align='right'>" & oMgr.LogoTag2( TimeWindow ) & "</div>" )
      Response.Write( "<HR>" ) 
    


   '
   ' Display link to refresh if it's just stale (anonymous has to be enabled in IIS to get here)
   '
   ElseIf oMgr.HasTicket Then

      Response.Write( "<br><br>Your ticket is stale. (" & oMgr.TicketAge & "seconds old)<br>" )
      Response.Write( "<A HREF='"  & oMgr.AuthURL2( thisURL, TimeWindow, TRUE ) & "'>Please Sign In to Passport again</A>" )
      
      

   '
   ' User hasn't done anything, make them sign in 
   '
   Else

      Response.Write( "<br> Please click on the Passport scarab to log in to Passport <br>" )
      Response.Write(thisURL)
	Response.Write( oMgr.LogoTag2( thisURL, TimeWindow ) )

   End If


   '
   ' Get the name of the current Windows user that represents this security context
   '
   Response.Write( "<br><br>The current Windows security context for this page is: " ) 

   UserName = Request.ServerVariables("LOGON_USER")

   If StrComp( UserName, "" ) = 0 Then
      Response.Write( " IIS Anonymous User" )
   Else
      Response.Write( " " & UserName )
   End If

   Response.Write( "<br>" )


   '
   ' Make the user refresh tickets every 60 minutes
   '

   If oMgr.IsAuthenticated(TimeWindow,FALSE) Then 


   '
   ' Convert the binary PUID to a hex format for displaying
   '
   strMemberIDHigh = oMgr.Profile("MemberIDHigh")
   strMemberIDLow = oMgr.Profile("MemberIDLow")
   strHex = Hex(strMemberIDHigh)
	
   strPUID = String(8 - Len(strHex), "0") & strHex
   strHex = Hex(strMemberIDLow)
   strPUID = strPUID & String(8 - Len(strHex), "0") & strHex


%>

   <FONT SIZE="2" FACE="ARIAL, HELVETICA">
   <BR><BR><B>You have been authenticated as a Passport user!</B></FONT> <BR><BR>

<%

   '
   ' Show the provisioning link if IIS is not impersonating
   ' (this means the mapping did not happen)
   '
   If StrComp( UserName, "" ) = 0 Then
      Response.Write( "Hey " & oMgr.Profile("FirstName") &" " & oMgr.Profile("LastName") & ". Click " )
      Response.Write( "<A href=../subscribe/subscribe.asp> here</A>" )
      Response.Write( " in order to associate your Passport Unique ID with a user account in AD.<br>" )
   End If

   End If
   
 
%>
   <FONT SIZE="2" FACE="ARIAL, HELVETICA">
   <B>To Enter Trial Click </B><A href=../trial/trial.asp> here</A></FONT> <BR>

</body>
</html>
