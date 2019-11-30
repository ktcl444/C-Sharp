'ScenarioExchangeBaseline.vbs
'(c) Microsoft, Corporation
'This script is used to configure the Exchange environment for the Microsoft Identity and Access Management Series
'This file will also update the corresponding target scenario Intranet data file that is used by ADBaseline.vbs.
Option Explicit

'Public Constants
Const	CONTOSO_FIRST_STORAGE_GROUP		= "First Storage Group"
Const	CONTOSO_SECOND_STORAGE_GROUP	= "Second Storage Group"
Const	FIRST_MAILBOX_STORE_SG1			= "First Mailbox Store (SG1)"
Const	SECOND_MAILBOX_STORE_SG1		= "Second Mailbox Store (SG1)"
Const	FIRST_MAILBOX_STORE_SG2			= "First Mailbox Store (SG2)"
Const	SECOND_MAILBOX_STORE_SG2		= "Second Mailbox Store (SG2)"

'Public Variables
Public oFSO : Set oFSO = CreateObject("Scripting.FileSystemObject")
Public oExchangeServer : Set oExchangeServer = CreateObject("CDOEXM.ExchangeServer")
Public sDataFile
Public sTargetServer

Dim oARgs : Set oArgs = WScript.Arguments
Dim i

'Set our default file, if no file is specified.
sDataFile = "IntranetADData.txt"

'Begin Script
'Get the command line parameters
if (oArgs.count = 0) then
		ShowUsage
else
	For i = 0 to oargs.count - 1
		Select Case lcase(oArgs(i))
				
			Case "/s"
				sTargetServer = oArgs(i + 1)

		End Select
	Next
end if

Main

wscript.stdout.writeline "Done"
Set oExchangeServer = Nothing
Set oFSO = Nothing

'**********************************************************
'Subroutine:	Main
'
'Main entry point for script that connects to target Exchange Server and calls respective functions
'to create Mailbox Stores and Storage Groups.
'
'Paramters:		None
'Return:		None
'**********************************************************
Sub Main

	'Dim local variables.
	Dim oTargetFile
	Dim vFileStream
	
	on error resume next

	wscript.stdout.writeline "Configuring Extranet Exchange Environemnt Objects."
	wscript.stdout.writeline "Connecting to server: " & sTargetServer & "..."

	'Connect to target Exchange Server specified by user.
	oExchangeServer.DataSource.Open sTargetServer
	if err.number = 0 then
		wscript.stdout.write "Connected Succesfully" & vbcrlf
	else
		wscript.stdout.write "Error encountered: " & err.number & " - " & err.Description 
		exit sub
	end if
	
	'Create First Storage Group.
	wscript.stdout.write "Creating Storage Group: " & CONTOSO_FIRST_STORAGE_GROUP & "..."
	If (CreateStorageGroup(CONTOSO_FIRST_STORAGE_GROUP))then
		wscript.stdout.write "Completed Succesfully" & vbcrlf
	else
		wscript.stdout.write "Error encountered: " & err.number & " - " & err.Description 
		exit sub
	end if
	
	'Create Second Storage Group.
	wscript.stdout.write "Creating Storage Group: " & CONTOSO_SECOND_STORAGE_GROUP & "..."
	If (CreateStorageGroup(CONTOSO_SECOND_STORAGE_GROUP))then
		wscript.stdout.write "Completed Succesfully" & vbcrlf
	else
		wscript.stdout.write "Error encountered: " & err.number & " - " & err.Description 
		exit sub
	end if
	
	'Create Mailbox Store in First Storage Group
	wscript.stdout.write "Creating Mailbox Store: " & FIRST_MAILBOX_STORE_SG1 & "..."
	If (CreateMailboxStore(FIRST_MAILBOX_STORE_SG1, CONTOSO_FIRST_STORAGE_GROUP))then
		wscript.stdout.write "Completed Succesfully" & vbcrlf
	else
		wscript.stdout.write "Error encountered: " & err.number & " - " & err.Description 
		exit sub
	end if
	
	'Create Second Mailbox Store in First Storage Group
	wscript.stdout.write "Creating Mailbox Store: " & SECOND_MAILBOX_STORE_SG1 & "..."
	If (CreateMailboxStore(SECOND_MAILBOX_STORE_SG1, CONTOSO_FIRST_STORAGE_GROUP))then
		wscript.stdout.write "Completed Succesfully" & vbcrlf
	else
		wscript.stdout.write "Error encountered: " & err.number & " - " & err.Description 
		exit sub
	end if
	
	'Create Mailbox Store in Second Storage Group
	wscript.stdout.write "Creating Mailbox Store: " & FIRST_MAILBOX_STORE_SG2 & "..."
	If (CreateMailboxStore(FIRST_MAILBOX_STORE_SG2, CONTOSO_SECOND_STORAGE_GROUP))then
		wscript.stdout.write "Completed Succesfully" & vbcrlf
	else
		wscript.stdout.write "Error encountered: " & err.number & " - " & err.Description 
		exit sub
	end if

	'Create Second Mailbox Store in Second Storage Group
	wscript.stdout.write "Creating Mailbox Store: " & SECOND_MAILBOX_STORE_SG2 & "..."
	If (CreateMailboxStore(SECOND_MAILBOX_STORE_SG2, CONTOSO_SECOND_STORAGE_GROUP))then
		wscript.stdout.write "Completed Succesfully" & vbcrlf
	else
		wscript.stdout.write "Error encountered: " & err.number & " - " & err.Description 
		exit sub
	end if
	
	'Modify target Intranet data file to reference specified Exchange Server.  We do this
	'so all target user attributes have correct Exchange server information in order to
	'create the specified mailboxes.
	
End Sub

'**********************************************************
'Function:	CreateMailboxStore
'
'This function is given a mailbox store name and the parent storage group in which to create
'the mailbox store.
'
'Paramters:		strMailboxStoreName - String representing the name of the mailbox store to create.
'				strTargetStorageGroup - Strng representing the name of the Storage group to create
'				the mailbox store in.
'
'Return:		True - Mail Box Store created and mounted successfully
'				False - Mail Box Store creation failed.
'**********************************************************
Public Function CreateMailboxStore(byval strMailboxStoreName, byval strTargetStorageGroup)

	On Error Resume Next

	'Create our private variables and object references
	Dim oExMailboxStore : Set oExMailboxStore = CreateObject("CDOEXM.MailboxStoreDB")
	Dim oExStorageGroup : Set oExStorageGroup = CreateObject("CDOEXM.StorageGroup")
	Dim o
	Dim sTargetSGURL, sMBStoreURL
	
	CreateMailboxStore = False
	
	'Enumerate through the list of mailbox stores and obtain the URL for the storage
	'group provided.
	For each o in oExchangeServer.storageGroups
		oExStorageGroup.DataSource.Open o
		If LCase(oExStorageGroup.name) = LCase(strTargetStorageGroup) Then
			sTargetSGURL = o
		end if
	next
	
	'Build the Exchange URL to the Mailbox Store
	sMBStoreURL = "LDAP://" & oExchangeServer.DirectoryServer & "/CN=" & strMailboxStoreName & "," & sTargetSGURL
	
	'Set our new mailbox store name
	oExMailboxStore.name = strMailboxStoreName

	'Save the new mailbox store
	oExMailboxStore.DataSource.SaveTo sMBStoreURL
	if err.number = 0 then
		wscript.stdout.write "Created..."
	elseif err.number = "-2147019886" then
		'Object already exists, we don't error.
		wscript.stdout.write "object already exists..."
		CreateStorageGroup = True
	else
		wscript.stdout.write "Error: " & err.number & " - " & err.Description
		Exit Function
	end if
	
	wscript.stdout.write "Mounting..."
	
	'Mount the new Mailbox Store.
	oExMailboxStore.Mount
	if err.number = 0 then
		CreateMailboxStore = True
	else
		wscript.stdout.write "Error: " & err.number & " - " & err.Description
		exit function
	end if
	
	Set oExMailboxStore = Nothing
	Set oExStorageGroup = Nothing

End Function

'**********************************************************
'Function:	CreateStorageGroup
'
'This function creates an Exchagne storage group.
'
'Paramters:		sstrTargetStorageGroup - Strng representing the name of the Storage group to create
'
'Return:		True - Storage Group created successfully
'				False - Storage Group creation failed.
'**********************************************************
Public Function CreateStorageGroup(byval strStorageGroupName)

	on error resume next

	'Create our private variables and object references
	Dim oExStorageGroup : Set oExStorageGroup = CreateObject("CDOEXM.StorageGroup")
	Dim sDefaultSG, sBaseURL, sNewSGURL

	CreateStorageGroup = False

	'Set our Storage Group Name
	oExStorageGroup.Name = strStorageGroupName

	'Obtain the defautl storage Group name from URL.  In this case, were using the first 
	'Storage Group in the collection.
	'Becuase of a bug in CDOEXM we can't call it an individual record in the StorageGroups
	'collection implicitly.  We need to call it through a reference.
	sDefaultSG = oExchangeServer.StorageGroups

	'Build the URL to the StorageGroup using the first storage group.      
	sBaseURL = Mid(sDefaultSG(0), InStr(2, sDefaultSG(0), "cn",1))

	'Build the URL to the new storage groups.
	sNewSGURL = "LDAP://" & oExchangeServer.DirectoryServer & "/CN=" & strStorageGroupName & "," & sBaseURL
	
	'Save the new storag group.
	oExStorageGroup.DataSource.SaveTo sNewSGURL
	if err.number = 0 then
		CreateStorageGroup = True
	elseif err.number = "-2147019886" then
		'Object already exists
		wscript.stdout.write "object already exists..."
		CreateStorageGroup = True
	else
		wscript.stdout.write "Error: " & err.number & " - " & err.Description
		Exit Function
	end if

	Set oExStorageGroup = Nothing

End Function


'**********************************************************
'Function:	ShowUsage
'
'Display script usage.
'
'**********************************************************
Public Function ShowUsage
	
	wscript.stdout.writeline "SCRIPT: ExchangeBaseline.vbs"
	wscript.stdout.writeline "This script is used to create the AD object baseline for the Idm Series scenarios."
	wscript.stdout.writeline 
	wscript.stdout.writeline "Parameters:"
	wscript.stdout.writeline "/s [Server Name] - specifies the target Exchange server to bind to."
	wscript.stdout.writeline 
	wscript.stdout.writeline "ScenarioExchangeBaseline.vbs /s [Server Name]"
	wscript.quit
	
End Function
