<HTML>
	<HEAD>
		<title>Contoso Sample App</title><%
  'Copyright (c) Microsoft Corporation. All rights reserved.

	Response.Expires = -1

%>
	</HEAD>
	<body BGCOLOR="white" TOPMARGIN="10">
		<H1 align="center"><IMG alt="" src="Contoso.gif"></H1>
		<H1 align="center">Sales And Contacts Application</H1>
		<H4 align="left">&nbsp;</H4>
		<H4 align="left">This is a sample application provided with the Extranet Access 
			Management&nbsp;paper that is part of the Identity and Access Management 
			solution series available from the <A href="http://www.microsoft.com/idm">Microsoft 
				IdM web site</A>.&nbsp; 
			<!-- Display header. --></H4>
		<table border="0">
			<tr>
				<td></td>
			</tr>
			<br>
		</table>
		<%

	'
	' Get the name of the current Windows user that represents this security context
	'
	Response.Write( "<br><br>The current Windows security context for this page is: " ) 

	UserName = Request.ServerVariables("LOGON_USER")

	If StrComp( UserName, "" ) = 0 Then
		Response.Write( " IIS Anonymous User" )
	Else
		Response.Write( " " & UserName )
		Response.Write( "You have been authenticated as a Contoso employee!")
	End If

	Response.Write( "<br>" )

%>
	</body>
</HTML>
