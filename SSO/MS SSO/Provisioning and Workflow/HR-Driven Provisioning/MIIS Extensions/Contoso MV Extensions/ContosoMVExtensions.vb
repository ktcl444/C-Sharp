' ///////////////////////////////////////////////////////////////////////////////
' Microsoft Solutions for Security
' Copyright (c) 2004 Microsoft Corporation. All rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
' THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
' AND INFORMATION REMAINS WITH THE USER.
' ///////////////////////////////////////////////////////////////////////////////

Imports System.directoryservices
Imports Microsoft.MetadirectoryServices
Imports Microsoft.MetadirectoryServices.logging
Imports System.Xml
Imports ActiveDs
Imports System
Imports System.IO
Imports System.Messaging
Imports PasswordGenerator.PasswordGenerator
Imports System.Exception
Imports ContosoUtilties.ContosoUtilities
Imports ContosoUtilties.Constants

' <summary>
' Implements IMVSynchronization interface to
' provide rules extension for the metaverse.
' </summary>

Public Class Contoso_MV_Extensions
    Implements IMVSynchronization

    ' Expiry time in months
    Const TEMPCONT_EXPIRY_TIME = 3

    ' These variables are initialized based on a xml configuration file
    ' The values are read during the Initialize() method of the Rules Extension
    Const INTRANET_INDEX = 0
    Const EXTRANET_INDEX = 1

    Dim employeesContainer() As String = {"", ""}
    Dim disabledContainer() As String = {"", ""}
    Dim contactsContainer() As String = {"", ""}
    Dim groupsContainer() As String = {"", ""}
    Dim homeMdb() As String = {"", ""}
    Dim fabrikamSMTPdomain As String
    Dim sunOneRootContainer As String
    Dim intADMAName As String
    Dim extADMAName As String
    Dim strLNMAName As String
    Dim strSOMAName As String
    Dim ssprovMAName As String
    Dim strLNDNCertifier As String
    Dim strLNNAB As String
    Dim strLNMailServer As String
    Dim minPasswordLength As Integer
    Dim isFirstRun As Boolean = False
    Dim idFileHomeDir As String
    Dim defaultManagerEmail As String


    ' <summary>
    ' Evaluates connected objects in
    ' response to a change to a metaverse object.
    ' </summary>
    ' <param name="mvEntry">Corresponding Entry from the Metaverse</param>
    ' <returns></returns>

    Public Sub Provision( _
        ByVal mvEntry As MVEntry) _
        Implements IMVSynchronization.Provision

        ' Declarations
        Dim company As String
        Dim employeeType As String
        Dim csEntry As CSEntry
        Dim password As String

        Dim entryObjectType As String

       
        ' Test if this is the first run. If so just return without
        ' doing(anything)
        If isFirstRun Then
            Return
        End If

        entryObjectType = mvEntry.ObjectType.ToLower()

        ' For each entryObjectType, corresponding operations
        ' should be performed
        Select Case entryObjectType

            Case "group"

                CreateADGroup(mvEntry, csEntry, INTRANET_INDEX, intADMAName)
                CreateLNGroup(mvEntry, csEntry, strLNMAName)

            Case "person"

                ' Test for existence of employeeType
                If Not mvEntry("employeeType").IsPresent Then
                    Throw New UnexpectedDataException _
                            ("employeeType expected for employee= " _
                                                + mvEntry("employeeId").Value)
                End If

                ' Generate a random password for usage in AD, LN and iPlanet
                ' Add extra characters to comply with complex password
                password = New PasswordGenerator.PasswordGenerator(). _
                                                    Generate(minPasswordLength)
                password = password + "&IdM2O"

                ' Assign the status of the employee to a variable
                employeeType = mvEntry("employeeType").Value

                ' For each value of the employee, seperate cases
                ' are implemented
                Select Case employeeType

                    ' In case of Full Time Employee
                Case EMPLOYEE_TYPE_FTE

                        ' This is a full time employee
                        ' Based on the value of "company" determine where and
                        ' how the user should be created
                        If Not mvEntry("company").IsPresent Then
                            Throw New UnexpectedDataException _
                                    ("company expected for employee= " _
                                                 + mvEntry("employeeId").Value)
                        End If

                        company = mvEntry("company").Value

                        ' To Check the company of the employee
                        Select Case company.ToLower

                            ' For contoso employee
                        Case "contoso"

                                ' Create the Intranet User and give the user 
                                ' a Exchange 2003 mailbox
                                CreateADUser(mvEntry, csEntry, INTRANET_INDEX, _
                                                        intADMAName, True, password)

                                ' Test to see if this is a Sales employee. 
                                ' If so create an Extranet account
                                If Not mvEntry("department").IsPresent Then
                                    Throw New UnexpectedDataException _
                                            ("department expected for employee= " _
                                                       + mvEntry("employeeId").Value)

                                End If

                                ' If the department is sales...
                                If mvEntry("department").Value.ToLower = _
                                                                       "sales" Then

                                    ' Create the Extranet User
                                    CreateADUser(mvEntry, csEntry, EXTRANET_INDEX, _
                                                           extADMAName, False, password)
                                Else
                                    ' If not, check whether there is an extranet user
                                    Dim activeDirectoryMA As ConnectedMA
                                    activeDirectoryMA = mvEntry.ConnectedMAs(extADMAName)
                                    If activeDirectoryMA.Connectors.Count > 0 Then
                                        'If there is an extranet user, deprovision it
                                        csEntry = activeDirectoryMA.Connectors.ByIndex(0)
                                        csEntry.Deprovision()
                                    End If
                                End If

                                ' Create a Lotus Notes contact
                                CreateLNContact(mvEntry, csEntry, strLNMAName)

                                ' For fabrikam employee
                            Case "fabrikam"

                                    ' Create the Intranet User and don't give the user a 
                                    ' Exchange 2003 mailbox, since they are using Lotus Notes
                                    CreateADUser(mvEntry, csEntry, INTRANET_INDEX, _
                                                                 intADMAName, False, password)

                                    ' Create the Lotus Notes mailbox
                                    CreateLNMailbox(mvEntry, csEntry, strLNMAName, password)

                                    ' Create the SunOne User
                                    CreateSunOneUser(mvEntry, csEntry, strSOMAName, password)

                                    ' For other possible values
                            Case Else
                                    ' 
                                    ' Unknown company
                                    ' Throw an exception to abort this object's synchronization. 
                                    Throw New UnexpectedDataException _
                                            ("Unknown company= " + company.ToString + _
                                                            " for full time employee= " _
                                                                + mvEntry("employeeId").Value)
                        End Select

                        ' In case of temporary and contractor employee types
                    Case EMPLOYEE_TYPE_TEMPORARY, EMPLOYEE_TYPE_CONTRACTOR

                        ' This is a contractor or temporary employee.
                        ' Only create the Intranet account
                        If Not mvEntry("company").IsPresent Then
                            Throw New UnexpectedDataException _
                                            ("company expected for employee= " _
                                                           + mvEntry("employeeId").Value)
                        End If
                        company = mvEntry("company").Value

                        ' To Check the company of the employee
                        Select Case company.ToLower
                            Case "contoso"

                                ' Create the Intranet User and give him/her an
                                ' Exchange 2003 mailbox
                                CreateADUser(mvEntry, csEntry, INTRANET_INDEX, _
                                                            intADMAName, True, password)

                            Case "fabrikam"
                                ' Create the Intranet User and don't
                                ' give him/her an Exchange 2003 mailbox, 
                                ' since they are using Lotus Notes
                                CreateADUser(mvEntry, csEntry, INTRANET_INDEX, _
                                                        intADMAName, False, password)

                                ' Create the Lotus Notes mailbox
                                CreateLNMailbox(mvEntry, csEntry, strLNMAName, password)

                            Case Else

                                ' Unknown company
                                ' Throw an exception to abort this 
                                ' object's synchronization. 
                                Throw New UnexpectedDataException _
                                        ("Unknown company= " + company.ToString + _
                                                " for contractor or temporary= " _
                                                        + mvEntry("employeeId").Value)
                        End Select

                        ' In case of partners and employees
                    Case EMPLOYEE_TYPE_PARTNER, EMPLOYEE_TYPE_CUSTOMER

                        ' This is a partner or customer. Do nothing, since this is
                        ' handled by the Extranet application
                    Case Else

                        ' This is an unknown employee type
                        ' Throw an exception to abort this object's synchronization. 
                        Throw New UnexpectedDataException("Unknown employeeType= " + _
                                        employeeType.ToString + " for employee= " _
                                                + mvEntry("employeeId").Value)

                End Select
            Case Else
                ' We are not handling other object types
        End Select

    End Sub
    ' <summary>
    ' Creates a Group in the Active Directory
    ' with the given criterion
    ' </summary>
    ' <param name="mventry">Metaverse entry</param>
    ' <param name="cventry">connector space entry</param>
    ' <param name="index">Index</param>
    ' <param name="strADMAName">AD Management Agent Name</param>
    ' <returns></returns>

    Private Sub CreateADGroup( _
    ByVal mvEntry As mvEntry, _
    ByRef csEntry As CSEntry, _
    ByVal index As Integer, _
    ByVal strADMAName As String)

        ' Declarations
        Dim activeDirectoryMA As ConnectedMA
        Dim refValue As ReferenceValue
        Dim strRefValue As String

        ' Use the AD MA pointed to by strADMAName
        activeDirectoryMA = mvEntry.ConnectedMAs(strADMAName)

        ' This is new group
        If activeDirectoryMA.Connectors.Count = 0 Then

            ' Check for the absence of the group in Metaverse
            If Not mvEntry("displayName").IsPresent Then
                Throw New UnexpectedDataException("displayName expected for group= " _
                                                        + mvEntry("uid").Value)
            End If
            ' Aggregate all the required values to form a group
            strRefValue = "CN=" + mvEntry("displayName").Value
            refValue = activeDirectoryMA.EscapeDNComponent(strRefValue). _
                                                Concat(groupsContainer(index))

            ' Create a Group in Active Directory
            csEntry = activeDirectoryMA.Connectors.StartNewConnector("group")
            csEntry.DN = refValue
            csEntry.CommitNewConnector()

        End If

    End Sub
    ' <summary>
    ' Creates a Group in the Lotus Notes
    ' with the given criterion
    ' </summary>
    ' <param name="mventry">Metaverse entry</param>
    ' <param name="cventry">connector space entry</param>
    ' <param name="strLNMAName">Lotus Notes MA Name</param>
    ' <returns></returns>

    Private Sub CreateLNGroup( _
    ByVal mvEntry As MVEntry, _
    ByRef csEntry As CSEntry, _
    ByVal strLNMAName As String)

        ' Declarations
        Dim lotusNotesMA As ConnectedMA
        Dim refValue As ReferenceValue
        Dim strRefValue As String
       
        ' Use the Lotus Notes MA pointed to by strLNMAName
        lotusNotesMA = mvEntry.ConnectedMAs(strLNMAName)

        ' This is new group
        If lotusNotesMA.Connectors.Count = 0 Then

            ' Check for the absence of the group in Metaverse
            If Not mvEntry("displayName").IsPresent Then
                Throw New UnexpectedDataException _
                            ("displayName expected for group= " _
                                                + mvEntry("uid").Value)
            End If

            ' Aggregation of all the required parameters to create a group.
            strRefValue = mvEntry("displayName").Value
            refValue = lotusNotesMA.EscapeDNComponent(strRefValue).Concat(strLNNAB)

            ' Create a group in Lotus Notes
            csEntry = lotusNotesMA.Connectors.StartNewConnector("group")
            csEntry.DN = refValue
            csEntry.CommitNewConnector()

        End If

    End Sub
    ' <summary>
    ' Creates an user in the Active Directory
    ' with the given attributes
    ' </summary>
    ' <param name="mventry">Metaverse entry</param>
    ' <param name="cventry">connector space entry</param>
    ' <param name="index">Index</param>
    ' <param name="strADMAName">AD Management Name</param>
    ' <param name="isMailBoxNeeded">Mailbox Enabled</param>
    ' <param name="password">Password of the user account</param>
    ' <returns></returns>

    Private Sub CreateADUser( _
    ByVal mvEntry As MVEntry, _
    ByRef csEntry As CSEntry, _
    ByVal index As Integer, _
    ByVal strADMAName As String, _
    ByVal isMailBoxNeeded As Boolean, _
    ByVal password As String)

        ' Declarations
        Dim container As String
        Dim refValue As ReferenceValue
        Dim strRefValue As String
        Dim employeeStatus As Integer
        Dim numADConnectors As Integer
        Dim nickName As String
        Dim myConnector As CSEntry
        Dim activeDirectoryMA As ConnectedMA
        Dim smtpAddress As String

        ' Use the AD MA pointed to by strADMAName
        activeDirectoryMA = mvEntry.ConnectedMAs(strADMAName)

        ' Based on the value of "employeeStatus" determine the container in AD
        ' Test for existence of employeeStatus
        If Not mvEntry("employeeStatus").IsPresent Then
            Throw New UnexpectedDataException _
                        ("employeeStatus expected for employee= " _
                                                + mvEntry("employeeId").Value)
        End If

        ' Assign the status of the employee to a variable
        employeeStatus = mvEntry("employeeStatus").Value

        ' For various cases of the employee status
        Select Case employeeStatus
            Case EMPLOYEE_ACTIVE
                container = employeesContainer(index)
            Case EMPLOYEE_LEAVE, EMPLOYEE_RETIRED, EMPLOYEE_DISABLED
                container = disabledContainer(index)
            Case Else

                ' employeeStatus must be active, leave, 
                ' retired or disabled to be valid
                ' any other case is an error condition for this object. 
                ' Throw an exception to abort this object's synchronization. 
                Throw New UnexpectedDataException("Illegal employeeStatus= " _
                        + employeeStatus.ToString + " for employeeId " + _
                            mvEntry("employeeId").Value.ToString)
        End Select

        ' Based on the value of "cn" determine the RDN in AD
        ' Test for existence of employeeStatus
        If Not mvEntry("cn").IsPresent Then
            Throw New UnexpectedDataException _
                                ("cn expected for employee= " _
                                             + mvEntry("employeeId").Value)
        End If

        strRefValue = "CN=" & mvEntry("cn").Value

        ' Now construct the DN based on RDN and Container
        refValue = activeDirectoryMA.EscapeDNComponent(strRefValue).Concat(container)


        ' If there is no connector present, add a new AD connector
        numADConnectors = activeDirectoryMA.Connectors.Count
        If numADConnectors = 0 Then

            ' See if we need to create a mailbox for this user
            If isMailBoxNeeded Then

                ' Create User and Mailbox-Enable it
                ' Use location of mailbox from xml configuration file
                ' Set the Exchange alias to the sAMAccountName

                ' Test for existence of sAMAccountName
                If Not mvEntry("sAMAccountName").IsPresent Then
                    Throw New UnexpectedDataException _
                            ("sAMAccountName expected for employee= " _
                                             + mvEntry("employeeId").Value)
                End If
                nickName = mvEntry("sAMAccountName").Value

                ' Create a mailbox with the supplied parameters
                csEntry = ExchangeUtils.CreateMailbox _
                            (activeDirectoryMA, refValue, nickName, homeMdb(index))
            Else

                ' See if this is an Intranet or Extranet User. The Extranet does
                ' not have Exchange 2003 installed
                If index = INTRANET_INDEX Then

                    ' Create a mail-enabled user
                    ' Set the Exchange alias to the sAMAccountName

                    ' Test for existence of sAMAccountName
                    If Not mvEntry("sAMAccountName").IsPresent Then
                        Throw New UnexpectedDataException _
                                    ("sAMAccountName expected for employee= " _
                                                      + mvEntry("employeeId").Value)
                    End If

                    ' Manipulate the nickname and mailing address
                    nickName = mvEntry("sAMAccountName").Value
                    smtpAddress = nickName + fabrikamSMTPdomain

                    ' Create a mail enabled user, with the given set of values
                    csEntry = ExchangeUtils.CreateMailEnabledUser _
                                    (activeDirectoryMA, refValue, nickName, smtpAddress)
                    csEntry.DN = refValue
                Else

                    ' This is an Extranet user
                    csEntry = activeDirectoryMA.Connectors.StartNewConnector("user")
                    csEntry.DN = refValue
                End If
            End If

            ' Set Initial values
            SetInitialValues(csEntry, mvEntry, password)

            ' Save the entry
            csEntry.CommitNewConnector()

            'If the number of connections equal to 1
        ElseIf numADConnectors = 1 Then

            ' check if the connector has a different DN and rename if necessary
            ' First get the connector
            myConnector = activeDirectoryMA.Connectors.ByIndex(0)

            ' MIIS will rename/move if different, if not nothing will happen
            myConnector.DN = refValue

        Else
            Throw New UnexpectedDataException _
                    ("multiple AD connectors:" + _
                        numADConnectors.ToString + " for employeeId " _
                                             + mvEntry("employeeId").Value)
        End If

    End Sub
    ' <summary>
    ' Creates an user in the Lotus Notes
    ' with the given attributes
    ' </summary>
    ' <param name="mventry">Metaverse entry</param>
    ' <param name="cventry">connector space entry</param>
    ' <param name="strLNMAName">Lotus Notes Management Name</param>
    ' <param name="password">Password of the user account</param>
    ' <returns></returns>

    Private Sub CreateLNMailbox( _
    ByVal mvEntry As MVEntry, _
    ByRef csEntry As CSEntry, _
    ByVal strLNMAName As String, _
    ByVal password As String)

        ' Declarations
        Dim refValue As String
        Dim numLNConnectors As Integer
        Dim lotusNotesMA As ConnectedMA

        ' Load the MA of the Lotus Notes
        lotusNotesMA = mvEntry.ConnectedMAs(strLNMAName)
        numLNConnectors = lotusNotesMA.Connectors.Count

        If numLNConnectors = 0 Then

            ' Create an entry in CS for the particular employee
            csEntry = lotusNotesMA.Connectors.StartNewConnector("person")

            ' Test for existence of sn
            If Not mvEntry("sn").IsPresent Then
                Throw New UnexpectedDataException _
                        ("sn expected for employee= " + _
                                        mvEntry("employeeId").Value)
            End If
            refValue = mvEntry("sn").Value

            ' Test for the existence of Middle Name
            If mvEntry("middleName").IsPresent Then
                refValue = mvEntry("middleName").Value + " " + refValue
            End If

            ' Test for the existence of Given Name
            If mvEntry("givenName").IsPresent Then
                refValue = mvEntry("givenName").Value + " " + refValue
            End If

            ' Add sAMAccountName to avoid duplicate entries for similar names
            refValue = refValue + " (" + mvEntry("sAMAccountName").Value + ")"
            refValue = refValue + "/" + strLNDNCertifier

            ' Set the property values to provision the object.
            csEntry.DN = lotusNotesMA.EscapeDNComponent("CN=" _
                            + refValue).Concat(strLNNAB)

            csEntry("LastName").Value = mvEntry("sn").Value
            csEntry("_MMS_Certifier").Value = strLNDNCertifier
            csEntry("_MMS_IDRegType").IntegerValue = 1  ' US User
            csEntry("_MMS_IDStoreType").IntegerValue = 2  ' ID File as a file

            ' The next two properties must have a value for a user with an
            ' identification file.
            ' Test for existence of employeeStatus
            If Not mvEntry("cn").IsPresent Then
                Throw New UnexpectedDataException _
                            ("cn expected for employee= " + _
                                                mvEntry("employeeId").Value)
            End If

            ' Assign attributes of the account like ID and Password
            csEntry("_MMS_IDPath").Value = idFileHomeDir + "\" + _
                                            mvEntry("sAMAccountName").Value + ".id"
            csEntry("_MMS_Password").Value = password
            csEntry("HTTPPassword").Value = password

            ' The next two properties must have a value for a user to access
            ' mail through the Lotus Notes client.
            csEntry("MailServer").Value = strLNMailServer
            csEntry("MailFile").Value = "mail\" + mvEntry("sAMAccountName").Value

            '  Finish creating the new connector.
            csEntry.CommitNewConnector()

        End If

    End Sub
    ' <summary>
    ' Creates a contact for user
    ' in Lotus Notes with the given attributes
    ' </summary>
    ' <param name="mventry">Metaverse entry</param>
    ' <param name="cventry">connector space entry</param>
    ' <param name="strLNMAName">Lotus Notes Management Name</param>
    ' <returns></returns>

    Private Sub CreateLNContact( _
    ByVal mvEntry As MVEntry, _
    ByRef csEntry As CSEntry, _
    ByVal strLNMAName As String)

        ' Declarations
        Dim refValue As String
        Dim numLNConnectors As Integer
        Dim lotusNotesMA As ConnectedMA

        ' Load the MA of the Lotus Notes
        lotusNotesMA = mvEntry.ConnectedMAs(strLNMAName)
        numLNConnectors = lotusNotesMA.Connectors.Count

        If numLNConnectors = 0 Then

            ' Create an entry in CS for the particular employee
            csEntry = lotusNotesMA.Connectors.StartNewConnector("person")

            ' Test for existence of sn
            If Not mvEntry("sn").IsPresent Then
                Throw New UnexpectedDataException _
                            ("sn expected for employee= " + _
                                            mvEntry("employeeId").Value)
            End If
            refValue = mvEntry("sn").Value

            ' Test for existence of Middle Name
            If mvEntry("middleName").IsPresent Then
                refValue = mvEntry("middleName").Value + " " + refValue
            End If

            ' Test for existence of Given Name
            If mvEntry("givenName").IsPresent Then
                refValue = mvEntry("givenName").Value + " " + refValue
            End If

            ' Add sAMAccountName to avoid duplicate entries
            refValue = refValue + " (" + mvEntry("sAMAccountName").Value + ")"

            ' Set the property values to provision the object.
            csEntry.DN = lotusNotesMA.EscapeDNComponent(refValue).Concat(strLNNAB)
            csEntry("_MMS_IDRegType").IntegerValue = 0  ' Contact

            ' Set the Mail System type
            csEntry("MailSystem").Value = "Other Internet Mail"

            '  Finish creating the new connector.
            csEntry.CommitNewConnector()

        End If

    End Sub
    ' <summary>
    ' Creates an user in Sun One Directory 
    ' with the given attributes
    ' </summary>
    ' <param name="mventry">Metaverse entry</param>
    ' <param name="cventry">connector space entry</param>
    ' <param name="strSOMAName">Sun One Management Agent Name</param>
    ' <param name="password">Password of the user</param>
    ' <returns></returns>

    Private Sub CreateSunOneUser(ByVal mvEntry As MVEntry, _
            ByRef csEntry As CSEntry, ByVal strSOMAName As String, _
                                                ByVal password As String)

        ' SunOne MA Provisioning Code
        ' Declarations
        Dim sunOneMA As ConnectedMA
        Dim refValue As ReferenceValue
        Dim numSOConnectors As Integer

        ' Load the MA for the Sun One Directory
        sunOneMA = mvEntry.ConnectedMAs(strSOMAName)

        ' Check for the absence of Employee ID
        If Not mvEntry("employeeId").IsPresent Then
            Throw New UnexpectedDataException("employeeId is expected")
        End If

        'building DN value based on EmployeeID attribute value
        refValue = sunOneMA.EscapeDNComponent _
                    ("CN=" & mvEntry.Item("employeeID").Value). _
                                                Concat(sunOneRootContainer)

        ' Count the number of connectors
        numSOConnectors = sunOneMA.Connectors.Count

        If numSOConnectors = 0 Then

            'Creating a iNETOrgPerson objecttype
            csEntry = sunOneMA.Connectors.StartNewConnector("iNetOrgPerson")
            csEntry.DN = refValue

            'setting initial password
            SetIPlanetPW(csEntry, password)

            ' Committ
            csEntry.CommitNewConnector()

        ElseIf numSOConnectors = 1 Then

            ' Dont rename Sun ONE

        Else
            Throw New UnexpectedDataException _
                        ("multiple Iplanet connectors:" + _
                                                numSOConnectors.ToString)
        End If

    End Sub
    ' <summary>
    ' Set values on a new provisioned Connector Space entry
    ' </summary>
    ' <param name="mventry">Metaverse entry</param>
    ' <param name="cventry">connector space entry</param>
    ' <param name="password">Password of the user</param>
    ' <returns></returns>

    Private Sub SetInitialValues( _
    ByRef csEntry As CSEntry, _
    ByVal mvEntry As MVEntry, _
    ByVal password As String)

        ' Declarations
        Dim oDate As Date
        Dim oLargeInteger As Long

        ' Set the password
        csEntry("unicodepwd").Values.Add(password)

        ' Set AD Account TTL to reflect employeeType
        Select Case mvEntry("employeeType").Value
            Case EMPLOYEE_TYPE_TEMPORARY

                ' Set the account to expire in TEMPCONT_EXPIRY_TIME months
                oDate = DateAdd(DateInterval.Month, TEMPCONT_EXPIRY_TIME, Today())

                ' Convert our date to an IADsLargeInteger...
                oLargeInteger = oDate.ToFileTime()
                csEntry("accountExpires").IntegerValue = oLargeInteger

            Case EMPLOYEE_TYPE_CONTRACTOR

                ' Read the departure date from the MV
                oDate = DateValue(mvEntry("employeeDepartureDate").Value)

                ' Convert our date to an IADsLargeInteger...
                oLargeInteger = oDate.ToFileTime()
                csEntry("accountExpires").IntegerValue = oLargeInteger

        End Select

        ' Send notification to the manager
        Sendnotification(mvEntry, password)

    End Sub
    ' <summary>
    ' Setting inital password For iPlanet Directory MA
    ' </summary>
    ' <param name="mventry">Metaverse entry</param>
    ' <param name="cventry">connector space entry</param>
    ' <param name="password">Password of the user</param>
    ' <returns></returns>

    Private Sub SetIPlanetPW(ByRef csEntry As CSEntry, _
                        ByVal pw As String)

        ' Declarations
        Dim password() As Byte

        password = New System.Text.UTF8Encoding(False, False).GetBytes(pw)
        ReDim Preserve password(UBound(password) + 2)

        ' Add the password as an entry to CS
        csEntry("userPassword").Values.Add(password)

    End Sub
    ' <summary>
    ' Called when a connector
    ' space entry is disconnected during an import operation.
    ' This method determines if the metaverse entry connected
    ' to the disconnecting connector space entry should be deleted.
    ' </summary>
    ' <param name="mventry">Metaverse entry</param>
    ' <param name="cventry">connector space entry</param>
    ' <returns>boolean</returns>

    Public Function ShouldDeleteFromMV( _
        ByVal csEntry As CSEntry, _
        ByVal mvEntry As MVEntry) _
        As Boolean Implements IMVSynchronization.ShouldDeleteFromMV

        If csEntry.MA.Name = ssprovMAName Then
            ' Accept the delete
            Return True
        Else
            Return False
        End If
    End Function
    ' <summary>
    ' Sends notifications to the Manager with the 
    ' password and employee ID of the provisoned user.
    ' </summary>
    ' <param name="mventry">Metaverse entry</param>
    ' <param name="userPassword">Password that is generated</param>
    ' <returns></returns>

    Private Sub Sendnotification(ByVal mvEntry As MVEntry, _
                                    ByVal userPassword As String)

        Dim userName As String

        ' Open a private queue named AccountProvisioning 
        ' on the local machine (".")
        Dim queue As MessageQueue = New MessageQueue _
                                (".\private$\AccountProvisioning")

        ' Create a new message
        Dim msg As Message = New Message

        ' Give a meaningful label so the receiver quickly can see
        ' what it is without reading the body
        msg.Label = "AccountProvisioning"

        userName = mvEntry("givenName").Value + " " + mvEntry("sn").Value

        ' Check whether the manager entry is available in CS entry
        ' and write some XML data into the body
        If Not mvEntry("managerEmail").IsPresent Then
            ' Instantiate the config as a new XmlDocument 
            ' Get Directory Extensions and load the XmlDocument
            Dim config As XmlDocument = New XmlDocument
            Dim directory As String = Utils.ExtensionsDirectory()
            config.Load(directory + SCENARIO_XML_CONFIG)

            ' Get information about Intranet
            Dim rNode As XmlNode = _
                config.SelectSingleNode("provisioning/general-definitions")
            Dim node As XmlNode = _
                rNode.SelectSingleNode("defaultManagerEmail")
            ' Get the value from the XML Node
            defaultManagerEmail = node.InnerText

            msg.Body = BuildXMLData(defaultManagerEmail, _
                                userName, userPassword, mvEntry("employeeId").Value, mvEntry("userPrincipalName").Value)
        Else

            msg.Body = BuildXMLData(mvEntry("managerEmail").Value, _
                               userName, userPassword, mvEntry("employeeId").Value, mvEntry("userPrincipalName").Value)
        End If

        ' Send the message and have tell MSMQ to commit the 
        ' transaction (TransactionType = Single message)
        queue.Send(msg, MessageQueueTransactionType.Single)

    End Sub
    ' <summary>
    ' Build a XML with all the attributes like,
    ' employeeID, password, and manager-email etc
    ' </summary>
    ' <param name="managerEmail">Manager's Email ID</param>
    ' <param name="employeeId">Employee ID</param>
    ' <param name="userPassword">Password</param>
    ' <param name="uPN">User Prinicipal Name</param>
    ' <returns>String</returns>

    Function BuildXMLData(ByVal managerEmail As String, _
                ByVal employeeName As String, ByVal userPassword As String, _
                 ByVal employeeId As String, ByVal uPN As String) As String

        ' Create an XML and write the specified values into the same

        Dim body As StringWriter = New StringWriter
        Dim writer As XmlTextWriter = New XmlTextWriter(body)

        writer.WriteStartDocument()
        writer.WriteStartElement("account")
        writer.WriteAttributeString("employeeId", employeeId)
        writer.WriteAttributeString("employeeName", employeeName)
        writer.WriteAttributeString("userPassword", userPassword)
        writer.WriteAttributeString("managerSmtp", managerEmail)
        writer.WriteAttributeString("userPrincipalName", uPN)
        writer.WriteEndElement()
        writer.WriteEndDocument()

        Return body.ToString()

    End Function

    ' <summary>
    ' Initializes the rules extension object.
    ' </summary>
    ' <returns></returns>

    Public Sub Initialize() Implements IMVSynchronization.Initialize


        ' Declarations
        Dim config As XmlDocument = New XmlDocument
        Dim directory As String = Utils.ExtensionsDirectory()

        ' Load the MV extensions XML
        config.Load(directory + SCENARIO_XML_CONFIG)

        ' Get information about Intranet
        ' Map the corresponding Intranet-Container node to a variable
        Dim rNode As XmlNode = config.SelectSingleNode _
                    ("provisioning/account-provisioning/intranet-container")
        Dim node As XmlNode = rNode.SelectSingleNode("root")
        Dim rootContainer As String = node.InnerText

        ' Get the intranet Organizational Unit 'employees' in a Container
        node = rNode.SelectSingleNode("employees")
        employeesContainer(INTRANET_INDEX) = _
                                node.InnerText + "," + rootContainer

        ' Get the intranet Organizational Unit 'diabsled' in a Container
        node = rNode.SelectSingleNode("disabled")
        disabledContainer(INTRANET_INDEX) = _
                                node.InnerText + "," + rootContainer

        ' Get the intranet Organizational Unit 'contacts' in a Container
        node = rNode.SelectSingleNode("contacts")
        contactsContainer(INTRANET_INDEX) = _
                                node.InnerText + "," + rootContainer

        ' Get the intranet Organizational Unit 'groups' in a Container
        node = rNode.SelectSingleNode("groups")
        groupsContainer(INTRANET_INDEX) = _
                                node.InnerText + "," + rootContainer

        ' Get the homeMDB value
        node = rNode.SelectSingleNode("homeMDB")
        homeMdb(INTRANET_INDEX) = node.InnerText

        ' Get the fabrikam SMTP domain
        node = rNode.SelectSingleNode("fabrikamSMTPdomain")
        fabrikamSMTPdomain = node.InnerText

        ' Get the lotus notes certifier value
        node = rNode.SelectSingleNode("ln-certifier")
        strLNDNCertifier = node.InnerText

        ' Get the Lotus Notes NAB
        node = rNode.SelectSingleNode("ln-nab")
        strLNNAB = node.InnerText

        ' Get the Lotus Notes Mail Server
        node = rNode.SelectSingleNode("ln-mailserver")
        strLNMailServer = node.InnerText

        ' Get the Lotus Notes ID Files Home Directory
        node = rNode.SelectSingleNode("ln-idfilehomedir")
        idFileHomeDir = node.InnerText

        ' Get information about Extranet
        rNode = config.SelectSingleNode _
                    ("provisioning/account-provisioning/extranet-container")
        node = rNode.SelectSingleNode("root")
        rootContainer = node.InnerText

        ' Get the extranet OrganizationalUnit 'employees' in a Container
        node = rNode.SelectSingleNode("employees")
        employeesContainer(EXTRANET_INDEX) = _
                        node.InnerText + "," + rootContainer

        ' Get the extranet OrganizationalUnit 'groups' in a Container
        node = rNode.SelectSingleNode("groups")
        groupsContainer(EXTRANET_INDEX) = _
                        node.InnerText + "," + rootContainer

        ' Get the extranet OrganizationalUnit 'disabled' in a Container
        node = rNode.SelectSingleNode("disabled")
        disabledContainer(EXTRANET_INDEX) = _
                        node.InnerText + "," + rootContainer

        ' Get information about SunOne
        rNode = config.SelectSingleNode _
                        ("provisioning/account-provisioning/sunone-container")
        node = rNode.SelectSingleNode("root")
        sunOneRootContainer = node.InnerText

        ' Get information about Management Agents
        rNode = config.SelectSingleNode("provisioning/ma-definitions")

        ' Intranet Active Directory Management Agent
        node = rNode.SelectSingleNode("int-ad-ma")
        intADMAName = node.InnerText

        ' Extranet Active Directory Management Agent
        node = rNode.SelectSingleNode("ext-ad-ma")
        extADMAName = node.InnerText

        ' Lotus Notes Management Agent
        node = rNode.SelectSingleNode("ln-ma")
        strLNMAName = node.InnerText

        ' Sun One Management Agent
        node = rNode.SelectSingleNode("so-ma")
        strSOMAName = node.InnerText

        node = rNode.SelectSingleNode("ssprov-ma")
        ssprovMAName = node.InnerText


        ' Get information about Users
        rNode = config.SelectSingleNode("provisioning/user-definitions")
        node = rNode.SelectSingleNode("min-password-length")
        minPasswordLength = node.InnerText

        ' Get information about first run
        rNode = config.SelectSingleNode("provisioning/run-definitions")
        node = rNode.SelectSingleNode("first")
        If (node.InnerText.ToLower = "true") Then
            isFirstRun = True
        End If

    End Sub
    ' <summary>
    ' Called when the rules extension object
    ' is no longer needed. This method is used to free resources
    ' owned by the rules extension.
    ' </summary>
    ' <returns></returns> 

    Public Sub Terminate() Implements IMVSynchronization.Terminate
    End Sub

End Class
