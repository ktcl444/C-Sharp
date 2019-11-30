<%@ Language = VBScript %>
<%
     'Copyright (c) Microsoft Corporation. All rights reserved.
%>

<%' Option Explicit %>

<%
''*****************C H A N G E   T H E S E  V A L U E S *********************

'Location of Trial Users OU from the Domain Root
userContainer = "OU=Trial Users,OU=Accounts,"

'The Win2K3 Group in Trial Users OU  
groupDN = "LDAP://CN=Trial User,"

'Note : If your trial application location is different please modify the href at the end of this page. 

''*****************E N D  C H A N G E   T H E S E  V A L U E S ***************

   '
   ' instantiate Passport Manager
   '
	Set objPassportManager = Server.CreateObject("Passport.Manager.1")

	'
	' get whatever information about the user that we can from Passport
	'
	strCurrentUserEmail = objPassportManager.Profile("PreferredEmail")

	strMemberIDHigh = objPassportManager.Profile("MemberIDHigh")
	strMemberIDLow = objPassportManager.Profile("MemberIDLow")
	strHex = Hex(strMemberIDHigh)
	
	'
	' convert the PUID into the form we need
	'
	strPUID = String(8 - Len(strHex), "0") & strHex
	strHex = Hex(strMemberIDLow)
	strPUID = strPUID & String(8 - Len(strHex), "0") & strHex

   ' If the user has entered information in the form, use it else
   ' display the information from the Passport ticket

   If IsEmpty(Request("tnumber")) Then
      strLastName = objPassportManager.Profile("LastName")
      strFirstName = objPassportManager.Profile("FirstName")
   Else
      strTNumber = Request.form("tnumber")
      strLastName = Request("lname")
      strFirstName = Request("fname")
   End If

%>


<HTML>
    <HEAD>
        <TITLE>Form Posting</TITLE>
    </HEAD>

    <BODY BGCOLOR="White" TOPMARGIN="10" LEFTMARGIN="10">
        
		<!-- Display header. -->

		<table border=0>
		<tr>
		<td><IMG alt="Contoso Pharma" src="logo.gif" align="left"></td>
		</tr><br>
		<tr>
		<td><div style="border-top-style: solid; border-top-color: #000050; border-bottom-style:solid; border-bottom-color:#008080">
		<FONT SIZE=2 FACE="Arial" style="COLOR: #008080; FONT-SIZE: larger">Please enter the following information to create an account:</div></td>
		</tr>
		</table>
		<P>Your Legal disclaimer here !!!
		<FORM NAME=Form1 METHOD=Post ACTION="subscribe.asp">

			First Name: <INPUT TYPE=Text NAME=fname VALUE="<%=strFirstName%>"> <P>
			Last Name: <INPUT TYPE=Text NAME=lname VALUE="<%=strLastName%>"> <P>
			Telephone Number: <INPUT TYPE=Text NAME=tnumber VALUE="<%=strTNumber%>"> <P>
			E-Mail Address: <INPUT TYPE=Text NAME=mail VALUE="<%=strCurrentUserEmail%>"> <P>

			<INPUT TYPE=Submit VALUE="Submit">

		</FORM>

		<HR>

	</BODY>
</HTML>

<% 

if Request.form("tnumber") <> "" then

	Err.Clear

   '
	' connect to AD and find the default naming context for the default domain
   '
	set rootDSE = GetObject("LDAP://RootDSE")

	if Err.Number <> 0 then
		Response.Write "GetObject Error = " & Hex(Err.Number) & " (Source: " & Err.Source & ")"
	end if

	Err.Clear

	strDomainDN = rootDSE.Get("defaultNamingContext")

	if Err.Number <> 0 then
		Response.Write "Get Domain DN Error = " & Hex(Err.Number) & " (Source: " & Err.Source & ")"
	end if

	Err.Clear

	strdnsName = rootDSE.Get("dnsHostName")

   FirstDot = Instr( strdnsName, "." )

   strDomainName = Right( strdnsName, Len( strdnsName ) - FirstDot )

	strContainerDN = userContainer & strDomainDN

	' bind to the users container
	Set cont = GetObject ("LDAP://" & strContainerDN )

	if Err.Number <> 0 then
		Response.Write "GetObject Users Container Error = " & Hex(Err.Number) & " (Source: " & Err.Source & ")"
	end if

	Err.Clear

	' create the user account
	Set user = cont.Create("user", "cn=" & strPUID)
	
	if Err.Number <> 0 then
		Response.Write "Create User Error = " & Hex(Err.Number) & " (Source: " & Err.Source & ")"
	end if

	user.put "samAccountName", strPUID
	
	strFullAccountName = strPUID & "@" & strDomainName
	
	user.put "altSecurityIdentities", "Kerberos:" & strFullAccountName

   ' set the UPN of the user to equal the PP e-mail address
   user.put "userPrincipalName", strPUID & "@" & strDomainName
   user.put "givenName", Request.form("fname")(1)
   user.put "sn", Request.form("lname")(1)
   user.put "telephoneNumber", Request.form("tnumber")(1)

   '   WARNING!!!
   ' Before going live with a site based on this sample code,
   ' the following must be changed so that a random password
   ' is assigned to each account. A recommended way to do this
   ' is to use the CAPICOM Utilities.GetRandom function.
   user.put "userPassword", "89sd53ET!23kL"

	' write the property cache
	'user.SetInfo
	' refresh the property cache
	'user.GetInfo

	'clear the account disabled flag	
	'flags = user.get("userAccountControl")
	'flags = flags OR &H00010000
	'flags = flags AND &HFFFFFFFD
	'Response.Write "GetObject" & flags

	user.put "userAccountControl", 66080

	user.SetInfo
	
	'
		' Add New user to Trial User Group
	'
	
	Set tGroup = GetObject(groupDN & strContainerDN )
	if Err.Number <> 0 then
		Response.Write "Group Add Error 1 = " & Hex(Err.Number) & " (Source: " & Err.Source & ")"
	end if
	tGroup.GetInfo
	
	tGroup.Add("LDAP://CN=" & strPUID & "," & strContainerDN)
	
	if Err.Number <> 0 then
		Response.Write "Group Add Error 2 = " & Hex(Err.Number) & " (Source: " & Err.Source & ")"
	end if

	Response.Write "Thank you "
	Response.Write "!<br>"
	Response.Write "You have been enrolled at this Passport-enabled site as: "
	Response.Write Request.form("fname")(1)
	Response.Write " " 
	Response.Write Request.form("lname")(1)

   Response.Write( "<br><br><A href=../trial/trial.asp> Go to Trial</A>" )
 
end if

%>
