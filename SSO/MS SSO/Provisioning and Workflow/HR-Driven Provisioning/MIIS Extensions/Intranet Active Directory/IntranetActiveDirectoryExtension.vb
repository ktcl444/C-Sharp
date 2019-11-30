'///////////////////////////////////////////////////////////////////////////////
' Microsoft Solutions for Security
' Copyright (c) 2004 Microsoft Corporation. All rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
' THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
' AND INFORMATION REMAINS WITH THE USER.
'///////////////////////////////////////////////////////////////////////////////

Imports Microsoft.MetadirectoryServices
Imports System.Xml
Imports System.Exception
Imports ContosoUtilties.Constants



' <summary>
' Implements IMVSynchronization interface to
' provide rules extension to the Intranet Directory Management Agent
' </summary>


Public Class IntranetActiveDirectoryExtension
    Implements IMASynchronization

    'Global declarations
    Dim upnValue As String

    ' <summary>
    ' Maps attributes from a metaverse entry 
    ' to a connector space entry during export operations.
    ' </summary>
    ' <param name="flowRuleName">Flow Rule Name </param>
    ' <param name="mvEntry">Metaverse Entry </param>
    ' <param name="csEntry">Connector Space Entry </param>
    ' <returns></returns>

    Public Sub MapAttributesForExport( _
        ByVal flowRuleName As String, _
            ByVal mvEntry As MVEntry, _
                ByVal csEntry As CSEntry) _
        Implements IMASynchronization.MapAttributesForExport

        ' For each value of the FlowRuleName,
        ' cases are being implemented
        Select Case flowRuleName.ToString

            ' Obtain the status of the employee
        Case "EAFemployeeStatus"

                Dim currentValue As Long = 0

                ' get current value of userAccountControl
                If csEntry("userAccountControl").IsPresent Then
                    currentValue = csEntry("userAccountControl").IntegerValue
                Else
                    currentValue = UF_NORMAL_ACCOUNT
                End If

                'If condition included to avoid "employeeStaus not present" error
                If Not mvEntry("employeeStatus").IsPresent Then
                    ' Make sure the account is not disabled
                    csEntry("userAccountControl").IntegerValue = _
                        (currentValue Or UF_NORMAL_ACCOUNT) And (Not UF_ACCOUNTDISABLE)
                    Exit Select
                End If

                ' Set userAccountControl to reflect employeeStatus
                Select Case mvEntry("employeeStatus").Value

                    Case EMPLOYEE_ACTIVE

                        ' Make sure the account is not disabled
                        csEntry("userAccountControl").IntegerValue = _
                            (currentValue Or UF_NORMAL_ACCOUNT) And (Not UF_ACCOUNTDISABLE)
                    Case EMPLOYEE_LEAVE, EMPLOYEE_RETIRED, EMPLOYEE_DISABLED

                        'ensure the account is set to disabled and maintain current settings
                        csEntry("userAccountControl").IntegerValue = currentValue Or UF_ACCOUNTDISABLE
                    Case Else

                        ' employeeStatus must be active, leave, retired or disabled to be valid
                        ' any other case is an error condition for this object. 

                        ' Throw an exception to abort this object's synchronization. 
                        Throw New UnexpectedDataException _
                                ("Illegal employeeStatus= " + mvEntry("employeeStatus").Value.ToString _
                                + " for employeeId " + mvEntry("employeeId").Value.ToString)
                End Select

                ' Case of employee expiration
            Case "EAFaccountExpires"
                ' If condition included to avoid "employeeStatus not present" error
                If mvEntry("employeeStatus").IsPresent Then

                    ' Set accountExpire to reflect employeeStatus
                    Select Case mvEntry("employeeStatus").Value

                        Case EMPLOYEE_RETIRED

                            ' Set the account to expire in 1 month
                            Dim oDate As Date
                            Dim oLargeInteger As Long

                            oDate = DateAdd(DateInterval.Month, 1, Today())
                            '
                            ' Convert our date to an LargeInteger...
                            '
                            oLargeInteger = oDate.ToFileTime()
                            csEntry("accountExpires").IntegerValue = oLargeInteger
                    End Select
                    'if employeeStatus is not present
                Else
                    'TODO:
                End If

                ' Export the User ID to the Connector Space.
            Case "EAFuserPrincipalName"

                    ' Flow it straight through to connector space
                    csEntry("userPrincipalName").Value = mvEntry("samAccountName").Value + "@" + upnValue

        End Select

    End Sub

    ' <summary>
    ' Maps attributes from a connector space 
    ' entry to a metaverse entry during import operations.
    ' </summary>
    ' <param name="flowRuleName">Flow Rule Name</param>
    ' <param name="csEntry">Connector Space Entry</param>
    ' <param name="mvEntry">Metaverse Entry </param>
    ' <returns></returns>
    Public Sub MapAttributesForImport( _
        ByVal flowRuleName As String, _
        ByVal csEntry As CSEntry, _
        ByVal mvEntry As MVEntry) _
        Implements IMASynchronization.MapAttributesForImport

        ' For each value of the flowRuleName,
        ' cases are being implemented
        Select Case flowRuleName.ToString

            ' Obtain the status of the employee
        Case "IAFemployeeStatus"

                Dim currentValue As Long = 0
                Dim employeeStatus As Integer

                ' Check the availability of the employee in connectorspace 
                ' and corresponding status in metaverse
                If mvEntry("employeeStatus").IsPresent And _
                    csEntry("userAccountControl").IsPresent Then

                    ' This is an existing account in Intranet Active Directory
                    '
                    ' Check if there is a difference between
                    ' employeeStatus and userAccountControl

                    currentValue = csEntry("userAccountControl").IntegerValue

                    If mvEntry("employeeStatus").Value = EMPLOYEE_DISABLED _
                                     And ((currentValue And UF_NORMAL_ACCOUNT) _
                                                            = UF_NORMAL_ACCOUNT) Then

                        ' This is an account which has just been re-enabled.
                        ' Accept the change
                        mvEntry("employeeStatus").Value = EMPLOYEE_ACTIVE
                    End If

                    ' Test if UF_NORMAL_ACCOUNT and UD_DISABLEDACCOUNT are set
                    employeeStatus = mvEntry("employeeStatus").Value
                    If ((employeeStatus = EMPLOYEE_ACTIVE) And _
                        ((currentValue And UF_NORMAL_ACCOUNT) = UF_NORMAL_ACCOUNT) _
                                            And ((currentValue And UF_ACCOUNTDISABLE) _
                                                                = UF_ACCOUNTDISABLE)) Then
                        '
                        ' This is an account which has just been disabled. Accept the change
                        mvEntry("employeeStatus").Value = EMPLOYEE_DISABLED
                    End If
                End If

            Case "IAFsAMAccountName"

                ' Flow it straight through to metaverse
                mvEntry("sAMAccountName").Value = csEntry("sAMAccountName").Value
        End Select

    End Sub

    ' <summary>
    ' Generates a list of values based
    ' on the CSEntry attribute values that will
    ' be used to search the metaverse.
    ' </summary>
    ' <param name="flowRuleName">Flow rule name </param>
    ' <param name="csEntry">Connector Space Entry corresponding to 
    ' attribute mapping</param>
    ' <param name="values">Value corresponding to a Collection</param>
    ' <returns></returns>

    Public Sub MapAttributesForJoin( _
        ByVal flowRuleName As String, _
        ByVal csEntry As CSEntry, _
        ByRef values As ValueCollection) _
        Implements IMASynchronization.MapAttributesForJoin

        Throw New EntryPointNotImplementedException

    End Sub

    ' <summary>
    ' Join rule is configured to use a 
    ' rules extension to resolve conflicts
    ' </summary>
    ' <param name="joinCriteriaName">Join criteria Name to 
    ' resolve join search</param>
    ' <param name="csEntry">Connector Space Entry</param>
    ' <param name="rgmvEntry">Metaverse entries that match the 
    ' join criteria </param>
    ' <param name="imvEntry">Index of MV entry to which CS entry 
    ' will be joined</param>
    ' <param name="mvObjectType">Name of the metaverse class</param>
    ' <returns>Returns true if the join operation can be resolved.</returns>

    Public Function ResolveJoinSearch( _
        ByVal joinCriteriaName As String, _
        ByVal csEntry As CSEntry, _
        ByVal rgmvEntry() As MVEntry, _
        ByRef imvEntry As Integer, _
        ByRef mvObjectType As String) _
        As Boolean Implements IMASynchronization.ResolveJoinSearch

        Throw New EntryPointNotImplementedException
    End Function

    ' <summary>
    ' This method is called to determine if a new connector space object 
    ' should be projected to a new metaverse object when the connector
    ' space object does not join to an existing metaverse object.
    ' </summary>
    ' <param name="csEntry">Connector Space Entry</param>
    ' <param name="mvObjectClass">Metaverse Object Type</param>
    ' <returns>Returns true if the connector space entry 
    ' should be projected.</returns>

    Public Function ShouldProjectToMV( _
        ByVal csEntry As CSEntry, _
        ByRef mvObjectType As String) _
        As Boolean Implements IMASynchronization.ShouldProjectToMV

        Throw New EntryPointNotImplementedException
    End Function

    ' <summary>
    ' This method is called when a connector space entry is deprovisioned.
    ' </summary>
    ' <param name="csEntry">connector space entry </param>
    ' <returns>Returns DeprovisionAction values that determines 
    ' which action should be taken on the connector space entry.</returns>

    Public Function Deprovision( _
        ByVal csEntry As CSEntry) _
        As DeprovisionAction Implements IMASynchronization.Deprovision

        Throw New EntryPointNotImplementedException
    End Function

    ' <summary>
    ' Determines if a connector CSEntry object will be 
    ' disconnected. A connector space or CSEntry object is disconnected
    ' when a delta export matches a filter or if the filter rules are 
    ' changed and the new filter criteria for disconnecting an object are met.
    ' </summary>
    ' <param name="csEntry">Connector Space Entry corresponding
    ' to Disconnection</param>
    ' <returns>Returns true if the object will be disconnected</returns>

    Public Function FilterForDisconnection( _
        ByVal csEntry As CSEntry) _
        As Boolean Implements IMASynchronization.FilterForDisconnection

        Throw New EntryPointNotImplementedException
    End Function

    ' <summary>
    ' Initializes the rules extension object.
    ' </summary>
    ' <param name="csEntry">Connector Space Entry</param>
    ' <returns></returns>

    Public Sub Initialize() Implements IMASynchronization.Initialize

        ' Load the XML file from a specific directory
        Dim config As XmlDocument = New XmlDocument
        Dim directory As String = Utils.ExtensionsDirectory()
        config.Load(directory + SCENARIO_XML_CONFIG)

        ' Get information about Intranet
        Dim rNode As XmlNode = config.SelectSingleNode("provisioning/user-definitions")
        Dim node As XmlNode = rNode.SelectSingleNode("upn")
        upnValue = node.InnerText

    End Sub

    ' <summary>
    ' This method is called when the rules extension object 
    ' is no longer needed.
    ' </summary>
    ' <returns></returns>

    Public Sub Terminate() Implements IMASynchronization.Terminate
        ' TODO: Add termination code here
    End Sub

End Class
