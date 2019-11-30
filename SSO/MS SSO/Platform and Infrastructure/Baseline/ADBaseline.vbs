'ADBaseline.vbs - v4
'(c) Microsoft, Corporation
'This script is used to configure Contoso Active Directory forests for the Microsoft Identity and Access Management Series
Option Explicit

'Public Variables
Public oFSO : Set oFSO = CreateObject("Scripting.FileSystemObject")
Public vAttributes
Public vData
Public sDataFile
Public sTargetServer
Public sTargetMailServer
Public bintranet

'Public Constants
Const DEFAULT_INTRANET_DATA_FILE	=	"IntranetADData.txt"
Const DEFAULT_EXTRANET_DATA_FILE	=	"ExtranetADData.txt"
Const INTRANET_AD_ROOT				=	"DC=na,DC=corp,DC=contoso,DC=com"
Const EXTRANET_AD_ROOT				=	"DC=perimeter,DC=contoso,DC=com"
Const FSO_FORREADING				=	1
Const UF_ACCOUNTDISABLE				=	&H2
Const ADS_SECURE_AUTHENTICATION		=	&H1
Const ADS_SERVER_BIND				=	&H200
Const ADS_USE_SSL					=	&H2

Dim oARgs : Set oArgs = WScript.Arguments
Dim i

'Begin Script
if (oArgs.count = 0) or (oargs.count < 4) then
		ShowUsage
else
	For i = 0 to oargs.count - 1
		Select Case lcase(oArgs(i))
				
			Case "/t"
				if lcase(oArgs(i + 1)) = "intranet"  then
					bIntranet = True
					sDataFile = DEFAULT_INTRANET_DATA_FILE
				elseif lcase(oArgs(i + 1)) = "extranet" then
					bIntranet = False
					sDataFile = DEFAULT_EXTRANET_DATA_FILE
				else
					wscript.stdout.write "Invalid value for /t: " & oArgs(i + 1)
					ShowUsage
				end if
				
			Case "/s"
				sTargetServer = oArgs(i + 1)
				
			Case "/m"
				sTargetMailServer = oArgs(i + 1)
					
			Case "/f"
				sDataFile = oArgs(i + 1)

		End Select
	Next
	If (sTargetServer = "") or (sDataFile = "") then
		wscript.stdout.writeline "Missing Parameter"
		ShowUsage
	elseif bIntranet then
		if sTargetMailServer = "" then
			wscript.stdout.writeline "Missing Parameter"
			ShowUsage
		end if
	end if
	
end if

Main

'End Script
Set oFSO = nothing

'**********************************************************
'Subroutine:	Main
'
'Main entry point for script that queries XML configuraiton file, generates recordset of all users and
'executes corresponding function based on company name
'
'Paramters:		None
'Return:		None
'**********************************************************
Public Sub Main

	'Declare local variables and references
	Dim oMgrList : Set oMgrList = CreateObject("Scripting.Dictionary")
	Dim oDSO : Set oDSO = GetObject("LDAP:")
	Dim oRoot, oRootOU, oTargetOU, oUser, oGroup, oInputFile
	Dim sAltCred, sLDAPName
	Dim vKeys, vItems, vMembers
	Dim lControlFlags, lnewflag
	Dim i, n
    Dim AD_ROOT
	
	'Check to see if target is Intranet or Extranet 
	If bIntranet then
		wscript.stdout.writeline "Configuring Intranet Environment Objects."
		wscript.stdout.writeline "Opening target input file: " & sDataFile
		Set oInputFile = oFSO.OpenTextFile(sDataFile,FSO_FORREADING)
		sLDAPName = "LDAP://" & sTargetServer & "/" & INTRANET_AD_ROOT
		AD_ROOT = INTRANET_AD_ROOT
	else
		wscript.stdout.writeline "Configuring Extranet Environment Objects."
		wscript.stdout.writeline "Opening target input file: " & sDataFile
		Set oInputFile = oFSO.OpenTextFile(sDataFile,FSO_FORREADING)
		sLDAPName = "LDAP://" & sTargetServer & "/" & EXTRANET_AD_ROOT
		AD_ROOT = EXTRANET_AD_ROOT
	end if
	
	'Read first row of Data file which defiens all obejct attributes
	wscript.stdout.writeline "Extracting list of attributes."
	vAttributes = split(oInputFile.Readline,";")

	'Bind to LDAP server.
	wscript.stdout.writeline "Binding to Root: " & sLDAPName
	wscript.stdout.writeline "Importing objects."
	
	'Connecting to target namespace
	'***** BEGIN NOTE *****
	'If you are connecting to a DC outside of your current domain, you may need to call OpenDSobject in order
	'to pass alternate credentials.  Refer to the sample below.  If this is required, uncomment the preceding 
	'line below, and remove the line: Set oRoot = GetObject(sLDAPName)
	'To pass alternate credentials, uncomment and replace the corresponding params with the User ID and Password
	'for the target account you will use.
	'Set oRoot = oDSO.openDSObject(sLDAPName, <USERID>, <PASSWORD>, ADS_SECURE_AUTHENTICATION)
	'***** END NOTE *****
	Set oRoot = GetObject(sLDAPName)
	'Create PArent OU structure.  The parent Ou is the first data row in the file.
	vData = split(oInputFile.Readline,";")
	wscript.stdout.writeline "Creating " & vData(0) & ": " & vData(3) & "..."
	On Error Resume Next
	
	'Delete the OU if it already exists
	Dim objCont 'As IADsDeleteOps
	Set  objCont = Getobject ("LDAP: //OU=" & vData(3) & ", " & AD_ROOT)
    objCont.DeleteObject (0)
    Set objCont = Nothing
    if err.number = 424 then
		err.Clear
	end if
	
	Set oRootOU = oRoot.Create("organizationalUnit", "OU=" & vData(1))
	oRootOU.put "name", vData(3)
	oRootOU.SetInfo

	if err.number = 0 then
		Set oRoot = oRootOU
		wscript.stdout.write "Completed Successfully" & vbcrlf
	else
		wscript.stdout.writeline "Error encountered creating OU: " & err.number & " - " & err.Description
		wscript.quit
	end if
	
	'Loop through the remainding rows in the file which correspond to user objects.
	While not oInputFile.AtEndofStream
		
		'On Error Resume Next
		
		'Read corresponding row which defines OU obejcts.
		vData = split(oInputFile.Readline,";")
		
		Select Case lcase(vdata(0))
		
			Case "organizationalunit"
						
				'Create OU objects using the data defined in the input file.
				wscript.stdout.write "Creating " & vData(0) & ": " & vData(3) & "..."
				On Error Resume Next
				Set oRootOU = oRoot.Create("organizationalUnit", "OU=" & vData(1))
				oRootOU.put "name", vData(3)
				oRootOU.SetInfo
				if err.number = 0 then
					wscript.stdout.write "Completed Successfully" & vbcrlf
				else
					wscript.stdout.write "Error encountered creating OU: " & err.number & " - " & err.Description
					wscript.quit
				end if

			Case "user"

				wscript.stdout.write "Creating " & vData(0) & ": " & vData(5) & "..."

				'Create a new user object using data defiend in input file.
				Set oUser = oRootOU.create(vData(0), "CN=" & vData(5))
				
				'enumerate through all items in the array to add the corresponding user data values to the respective attributes.
				'Some of the items in the array are ignored because we handle them explicitly, hence we use a select 
				'case statement to skip ignored attributes.
				For i = 0 to ubound(vData) - 1

					Select case i

						Case 0
							'Do Nothing - Entry for object class.
						Case 1
							'Do nothing - Entry for OU objects.
						Case 4
							'Do nothing - Entry for homeMDBPAth.
						Case 18
							'Add the Manager information into a local dictionary object so they can be linked after
							'all user objects are created.
							oMgrList.add vData(2), vData(18)
						Case 22
							'Do Nothing for extranet, becuase this field is for groups
							if bIntranet then oUser.Put vAttributes(i), vData(i)

						Case 23
							'Do Nothing for extranet, becuase this field is for groups
							if bIntranet then oUser.Put vAttributes(i), vData(i)
														
						Case Else
						
							'Add the correspoding data value to the respective attribute based on the array index.					
							If vData(i) <> "" then
								oUser.Put vAttributes(i), vData(i)
							end if

					End Select
				
				next
				
				'Saving new user object
				oUser.SetInfo
				if err.number <> 0 then
					wscript.stdout.write "Error encountered performing SetInfo: " & err.number & " - " & err.Description
					wscript.quit
				end if
				
				'Set User password
				oUser.SetPassword "Idmv2_P@assw0rd"
				if err.number <> 0 then
					wscript.stdout.write "Error encountered setting password: " & err.number & " - " & err.Description
					wscript.quit
				end if
				
				'Customize account control flags
				lControlFlags = oUser.Get("UserAccountControl")
				lnewFlag = lControlFlags xOr UF_ACCOUNTDISABLE
				oUser.Put "userAccountControl", lnewFlag
				
				'check to determine which target environment the user will be created in, and perform additional
				'functions.
				if bintranet then
					'If creating Intranet users, perform Mail and certificate functions which are unique to INtranet users.
					if lcase(oUser.company) = "contoso" then
						'User object is a Contoso user so a Mailbox will be created.
						If Not(MailboxEnableUser(oUser.adsPath, vData(4))) then
							wscript.stdout.write "Error encountered creating mailbox: " & err.number & " - " & err.Description
							wscript.quit
						end if
					else
						'User object is a Fabrikam user for a Mail Recipient will be created.
						If Not(MailEnableUser(oUser.adsPath, vData(21))) then
							wscript.stdout.write "Error encountered creating mailbox: " & err.number & " - " & err.Description
							wscript.quit
						end if
					end if
				else
					'Setting external certificate for extranet users.
					'X509:<I>DC=com,DC=contoso,DC=corp,CN=Issuing CA II<S>DC=com,DC=contoso,DC=corp,DC=na,OU=ContosoCorp,OU=Employees,CN=aalberts,E=aalberts@contoso.com
					sAltCred = "X509:<I>DC=com,DC=contoso,DC=corp,CN=Issuing CA II<S>DC=com,DC=contoso,DC=corp,DC=na,OU=ContosoCorp,OU=Employees," & oUser.name & ",E=" & oUser.cn & "@contoso.com"
					oUser.put "altSecurityIdentities", sAltCred
				end if
			
				'Save all Object changes
				oUser.SetInfo
				if err.number = 0 then
					'Object created successfully
					wscript.stdout.write "Completed Successfully." & vbcrlf
				else
					'Error encountered saving object changes.
					wscript.stdout.write "Error encountered saving object: " & err.number & " - " & err.Description
					wscript.quit
				end if
				
			case "group"
			
				'on error resume next
				wscript.stdout.write "Creating " & vData(0) & ": " & vData(5) & "..."
				'Create a new user object using data defiend in input file.
				Set oGroup = oRootOU.create(vData(0), "CN=" & vData(5))
				
				'Populate Distinguished Name
				oGroup.put vAttributes(2), vData(2)
				
				'Populate DisplayName
				oGroup.put vAttributes(11), vData(11)
				
				'Populate sAMAccountName
				oGroup.put vAttributes(16), vData(16)
				
				'Populate groupType
				oGroup.put vAttributes(23), vData(23)
				
				'Save the group object
				oGroup.Setinfo
				if err.number <>0 then
					'Error encountered saving object changes.
					wscript.stdout.write "Error encountered saving object: " & err.number & " - " & err.Description & vbcrlf
				else
					'Add Group Memberships
					if vData(22) <> "" then
						if instr(1, vData(22), "!") then
							vMembers = split(vData(22),"!")
							For i = 0 to ubound(vMembers) - 1
								oGroup.add "LDAP://" & vMember(i)
							Next
						else
							oGroup.add "LDAP://" & vData(22)
						end if
					end if
					
					'Save all Object changes
					oGroup.SetInfo
					if err.number = 0 then
						'Object created successfully
						wscript.stdout.write "Completed Successfully." & vbcrlf
					else
						'Error encountered saving object changes.
						wscript.stdout.write "Error encountered saving object: " & err.number & " - " & err.Description
						wscript.quit
					end if
				end if
				
				on error goto 0								
							
		End Select

	Wend
	
	'Linking user objects with corresponding user objects that are defined as the users manager.  This needs
	'to be done after all user objects are created.
	wscript.stdout.write "Linking Managers..."
	
	'Set the keys and items from the dictionary object to separate arrays
	vKeys = oMgrList.Keys
	vItems = oMgrList.Items
	
	Set oUser = nothing
	
	'Loop through the manager dictionary object.
	For i = 0 to oMgrList.Count - 1
	
		'Set the link the user with their corresponding manager.
		if vItems(i) <> "" then
			Set oUser = GetObject("LDAP://" & vKeys(i))
			oUser.put "manager", vItems(i)
			oUSer.Setinfo
		end if
	
	Next
	
	wscript.stdout.write "Completed Successully." & vbcrlf
	
	wscript.stdout.writeline "Done"

	'Done.
	Set oRoot = Nothing
	Set oRootOU = Nothing
	Set oUser = Nothing
	Set oInputFile = Nothing
	Set OFSO = Nothing

End Sub

'**********************************************************
'Function:	MailBoxEnableUser
'
'This function is given the DN of a target user then calls the CreateMailbox method from the iMailboxStore
'interface to MailBox enable the user in the target Exchange environment.
'
'Paramters:		strTargetUser - String representing the DN of the target user.
'Return:		True - Mailbox Enabled performed successfully
'				False - Mailbox enable failed.
'**********************************************************
Public Function MailBoxEnableUser(ByVal strTargetUser, byval strTargetMBStore)

	'on error resume next

	Const	AD_MODE_READ_WRITE	=	3
	Const	CONTOSO_FIRST_STORAGE_GROUP		= "First Storage Group"
	Const	CONTOSO_SECOND_STORAGE_GROUP	= "Second Storage Group"

	Dim oPerson : Set oPerson = CreateObject("CDO.Person")
	Dim oExServer : Set oExServer = CreateObject("CDOEXM.ExchangeServer")
	Dim oSTGroup : Set oSTGroup = CreateObject("CDOEXM.StorageGroup")
	Dim pMailbox
	Dim oUser
	Dim sUserURL
	Dim sServerMBurl, sIStoreMBurl, sHomeMDBPath
	Dim vGroupList, vMStoreList
	
	'Open connection to local Exchange server we're running script on.
	oExServer.DataSource.Open sTargetMailServer
	if err.number <> 0 then
		wscript.stdout.write "Cannot connect to Exchange Server.  Ensure /m parameter is correct when executing script.  Error: " & err.Number & " - " & err.Description
		MailBoxEnableUser = False
		Exit Function
	end if

	'Open the first storage group in the list of available storage groups.
	vGroupList = oExServer.StorageGroups
	oSTGroup.DataSource.Open vGroupList(0)
	
	'Get the URL for the first Mailbox store in the storage group.
	vMStoreList = oSTGroup.MailboxStoreDBs
	sServerMBurl = vMStoreList(0)
	
	'Extract just the entire ADSI path to the InformationStore
	'sIStoreMBurl = Mid (sServerMBurl, instr(1, sServerMBurl, ",") + 1)
	sIStoreMBurl = Mid (Mid (sServerMBurl, instr(1, sServerMBurl, ",",1) + 1), instr(1, Mid (sServerMBurl, instr(1, sServerMBurl, ",",1) + 1), ",") + 1)

	'Build the user homeMDBPath based on the target Mailbox Store.
	If ucase(right(strTargetMBStore,5)) = "(SG1)" then
		sHomeMDBPath = strTargetMBStore & ",CN=" & CONTOSO_FIRST_STORAGE_GROUP & "," & sIStoreMBurl
	elseif ucase(right(strTargetMBStore,5)) = "(SG2)" then
		sHomeMDBPath = strTargetMBStore & ",CN=" & CONTOSO_SECOND_STORAGE_GROUP & "," & sIStoreMBurl
	else
		wscript.stdout.write "Error unknown Mailbox Store: " & strTargetMBStore
	end if
	
	'Open the USer object
	oPerson.datasource.open strTargetUser, ,AD_MODE_READ_WRITE
	
	'Retrieve the iMailboxStore interface.  This can also be performed through CDOEXM.IMailBoxStore
	Set pMailbox = oPerson.GetInterface("IMailboxStore")
	
	'Call CreateMailbox 
	pMailbox.CreateMailbox sHomeMDBPath        
	
	'Save the USer object
	oPerson.datasource.save
	
	'Check for errors, and return the boolean back to the caller.
	If Err.number = 0 then
		MailBoxEnableUser = True
	else 
		wscript.stdout.write "Error: " & err.Number & " - " & err.Description
		MailBoxEnableUser = False
	end if
	
	on error goto 0
	
	'Clean Up
	Set oExServer = Nothing
	Set oSTGroup = Nothing
	Set oPerson = Nothing

End Function

'**********************************************************
'Function:	MailEnableUser
'
'This function is given the DN of a target user then calls the MailEnable method from the iMailboxStore
'interface to Mail enable the user in the target Mail environment.
'
'Paramters:		strTargetUser - String representing the DN of the target user.
'Return:		True - Mail Enabled performed successfully
'				False - Mail enable failed.
'**********************************************************
Public Function MailEnableUser(ByRef strTargetUser, byval strTargetAddress)

	Const	AD_MODE_READ_WRITE	=	3

	Dim oPerson : Set oPerson = CreateObject("CDO.Person")
	Dim oUser
	Dim oMailEnable
	Dim sUserURL

	'Open the USer object
	oPerson.datasource.open strTargetUser, ,AD_MODE_READ_WRITE
	
	'Retrieve the iMailRecipient interface.  This can also be performed through CDOEXM.iMailRecipient
	Set oMailEnable = oPerson.GetInterface("iMailRecipient")
	
	'Call MailEnable and provide the necessary smtp address for the user
	oMailEnable.MailEnable strTargetAddress 
	
	'Save the user object.
	on error resume next
	oPerson.DataSource.Save
	
	'Check for errors, and return the boolean back to the caller.
	If err.number = 0 then
		MailEnableUser = True
	else
		wscript.stdout.write "Error: " & err.Number & " - " & err.Description	
		MailEnableUser = False
	end if
	
	On Error goto 0 
		
	Set oMailEnable = Nothing
	Set oPerson = Nothing

End Function

'**********************************************************
'Function:	ShowUsage
'
'Display script usage.
'
'**********************************************************
Public Function ShowUsage
	
	wscript.stdout.writeline "SCRIPT: ScenarioADBaseline.vbs"
	wscript.stdout.writeline "This script is used to create the AD object baseline for the Idm scenario."
	wscript.stdout.writeline
	wscript.stdout.writeline "Parameters:"
	wscript.stdout.writeline "/t [Intranet] - specifies to create the Intranet objects."
	wscript.stdout.writeline "   [Extranet] - specifies to create the Extranet objects."
	wscript.stdout.writeline "/s [Server Name] - specifies the target server to bind to.  If LDAP is"
	wscript.stdout.writeline "                   bound to a port other than 389, you must specify the"
	wscript.stdout.writeline "                   port name after the server. e.g Server:5678"
	wscript.stdout.writeline "/m [Server Name] - INTRANET ONLY. specifies the target Exchange mail"
	wscript.stdout.writeline "                   server to use for mailbox creation."
	wscript.stdout.writeline "/f [Data File] - OPTIONAL. Defines the target data file to load.  If not"
	wscript.stdout.writeline "                 specified, the default is to load the following files"
	wscript.stdout.writeline "                 from the current directory where the script is executed."
	wscript.stdout.writeline "                     INTRANET: IntranetADData.txt"
	wscript.stdout.writeline "                     EXTRANET: ExtranetADData.txt"
	wscript.stdout.writeline
	wscript.stdout.writeline "ScenarioADBaseline.vbs /t [Intranet | Extranet] /s [Server Name] /m [Exchange Server] /f [Data File]"
	wscript.quit
	
End Function
