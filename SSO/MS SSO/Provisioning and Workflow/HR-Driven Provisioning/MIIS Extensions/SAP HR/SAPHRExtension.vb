'/////////////////////////////////////////////////////////////////////////////
' Microsoft Solutions for Security
' Copyright (c) 2004 Microsoft Corporation. All rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
' THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
' AND INFORMATION REMAINS WITH THE USER.
'/////////////////////////////////////////////////////////////////////////////

Imports Microsoft.MetadirectoryServices
Imports System.Xml
Imports System.Exception
Imports ContosoUtilties.ContosoUtilities
Imports ContosoUtilties.Constants


' <summary>
' Implements IMVSynchronization interface to
' provide rules extension to the SAP HR Management Agent
' </summary>

Public Class SAPHRExtension
    Implements IMASynchronization

    ' Declaration of variables
    Dim defaultManagerEmail As String
    Dim phoneTable As Hashtable

    ' <summary>
    ' Initializes the rules extension object.
    ' </summary>
    ' <param name="csEntry">Connector Space Entry</param>
    ' <returns></returns>

    Public Sub Initialize() Implements IMASynchronization.Initialize

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

        phoneTable = New Hashtable
        phoneTable.Add("US", "+1")
        phoneTable.Add("GB", "+44")
        phoneTable.Add("UK", "+44")
        phoneTable.Add("us", "+1")
        phoneTable.Add("gb", "+44")
        phoneTable.Add("uk", "+44")
        'List to be extended if more countries are added

    End Sub

    ' <summary>
    ' Called when the rules extension object is no longer needed.
    ' </summary>
    ' <returns></returns>

    Public Sub Terminate() Implements IMASynchronization.Terminate
        ' TODO: Add termination code here
    End Sub

    ' <summary>
    ' Determines if a new connector space object 
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
        ' TODO: Remove this throw statement if you implement this method
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
        ' TODO: Add connector filter code here
        Throw New EntryPointNotImplementedException
    End Function

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
        ' TODO: Add join mapping code here
        Throw New EntryPointNotImplementedException
    End Sub

    ' <summary>
    ' Called when a join rule is configured to use a 
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
        ' TODO: Add join resolution code here
        Throw New EntryPointNotImplementedException
    End Function

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
        ' TODO: write your import attribute flow code
        Dim countryName As String

        ' For each case of flow rule name
        Select Case flowRuleName

            ' In case of display name
        Case "IAFdisplayName"

                ' Check for the presence of middle name
                If csEntry("middleName").IsPresent Then
                    mvEntry("displayName").Value = _
                            csEntry("surname").Value + ", " + _
                                csEntry("givenName").Value + " " + _
                                        csEntry("middleName").Value
                Else
                    mvEntry("displayName").Value = _
                        csEntry("surname").Value + ", " + _
                            csEntry("givenName").Value
                End If

                ' In case of employee status
            Case "IAFemployeeStatus"

                ' Test the current status of employeeStatus
                If mvEntry("employeeStatus").IsPresent Then
                    '
                    Select Case mvEntry("employeeStatus").Value
                        Case EMPLOYEE_LEAVE, EMPLOYEE_RETIRED, _
                                                        EMPLOYEE_ACTIVE

                            ' This is an existing user.
                            ' Accept changes from SAP HR
                            mvEntry("employeeStatus").Value = _
                                        csEntry("employeeStatus").Value
                        Case EMPLOYEE_DISABLED

                            ' This is an existing user. Do not accept 
                            ' any(changes) from SAP HR, because it 
                            ' has been disabled in AD
                    End Select
                Else

                    ' This is a new user. Accept the 
                    ' employeeStatus from SAP HR
                    mvEntry("employeeStatus").Value = _
                                csEntry("employeeStatus").Value
                End If

                ' In case of employees account name
            Case "IAFsAMAccountName"

                ' checking to see if the metaverse attribute sAMAccountName 
                ' is present if it is then do nothing.
                If mvEntry("sAMAccountName").IsPresent Then
                Else

                    ' Generate the sAMAccountName
                    Dim samName As String
                    samName = _
                        New ContosoUtilties.ContosoUtilities(). _
                                    GeneratesAMAccountName( _
                                        csEntry, mvEntry, False, "")

                    ' Set the sAMAccountName Metaverse value from
                    ' the SamName we built
                    mvEntry("sAMAccountName").Value = samName

                End If

            Case "IAFcn"

                ' checking to see if the metaverse attribute sAMAccountName
                ' is present if it is then use that. Otherwise calculate it
                If mvEntry("sAMAccountName").IsPresent Then
                    mvEntry("cn").Value = mvEntry("sAMAccountName").Value
                Else

                    ' Generate sAMAccountName and use it as cn
                    Dim cn As String
                    cn = _
                        New ContosoUtilties.ContosoUtilities(). _
                                            GeneratesAMAccountName( _
                                             csEntry, mvEntry, False, "")

                    'Generate cn Metaverse value from the SamName we built
                    mvEntry("cn").Value = cn
                End If

                ' Checking to see if connector space attribute is manager
            Case "IAFmanagerEmail"
                Dim findResults() As MVEntry

                ' Check whether the manager entry is available in CS entry
                If Not csEntry("manager").IsPresent Then
                    Throw New UnexpectedDataException( _
                        "Manager expected for employee= " + _
                            csEntry("employeeId").Value)
                End If

                ' Find the managers e-mail address based on employeeId, 
                ' since we can not search on reference values
                findResults = Utils.FindMVEntries( _
                    "employeeId", csEntry("manager").Value, 1)
                If findResults.Length = 0 Then
                    mvEntry("managerEmail").Value = defaultManagerEmail
                Else

                    ' Get the e-mail address
                    If findResults(0)("mail").IsPresent Then
                        mvEntry("managerEmail").Value = _
                                    findResults(0)("mail").Value
                    Else
                        mvEntry("managerEmail").Value = defaultManagerEmail
                    End If
                End If
                'Logic to add contry code to the telephone number
            Case "IAFtelephoneNumber"

                    ' checking to see if the metaverse attribute Country
                    ' is present
                    If Not csEntry("Country").IsPresent Then
                        Throw New UnexpectedDataException( _
                                "Country expected for employee=" + _
                                    csEntry("employeeId").Value)
                    Else
                        'Prefix country code and a space
                        countryName = csEntry("Country").Value
                        mvEntry("telephoneNumber").Value = _
                            phoneTable(countryName) + " " + _
                                csEntry("telephoneNumber").Value()
                    End If

                    ' Logic to add contry code to the fax number
            Case "IAFfacsimileTelephoneNumber"

                    ' checking to see if the metaverse attribute Country
                    ' is present
                    If Not csEntry("Country").IsPresent Then
                        Throw New UnexpectedDataException( _
                                "Country expected for employee=" + _
                                    csEntry("employeeId").Value)
                    ElseIf csEntry("facsimileTelephoneNumber").IsPresent Then

                        'Prefix country code and a space
                        countryName = csEntry("Country").Value
                        mvEntry("facsimileTelephoneNumber").Value = _
                            phoneTable(countryName) + " " + _
                                csEntry("facsimileTelephoneNumber").Value()
                    End If
                    'Logic to add country code to the mobile number
            Case "IAFmobile"

                    ' checking to see if the metaverse attribute Country
                    ' is present
                    If Not csEntry("Country").IsPresent Then
                        Throw New UnexpectedDataException( _
                                "Country expected for employee=" + _
                                    csEntry("employeeId").Value)
                    ElseIf csEntry("mobile").IsPresent Then
                        'Prefix country code and a space
                        countryName = csEntry("Country").Value
                        mvEntry("mobile").Value = _
                            phoneTable(countryName) + " " + _
                                csEntry("mobile").Value()
                    End If
                    'Logic to add country code to the pager number
            Case "IAFpager"

                    ' checking to see if the metaverse attribute Country
                    ' is present
                    If Not csEntry("Country").IsPresent Then
                        Throw New UnexpectedDataException( _
                                "Country expected for employee=" + _
                                    csEntry("employeeId").Value)
                    ElseIf csEntry("pager").IsPresent Then
                        'Prefix country code and a space
                        countryName = csEntry("Country").Value
                        mvEntry("pager").Value = _
                            phoneTable(countryName) + " " + _
                                csEntry("pager").Value()
                    End If
            Case Else

                    ' TODO: remove the following statement and 
                    ' add your default script here
                    Throw New EntryPointNotImplementedException
        End Select

    End Sub

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
        ' TODO: Add export attribute flow code here
        Throw New EntryPointNotImplementedException
    End Sub

    ' <summary>
    ' Called when a connector space entry is deprovisioned.
    ' </summary>
    ' <param name="csEntry">connector space entry </param>
    ' <returns>Returns DeprovisionAction values that determines 
    ' which action should be taken on the connector space entry.</returns>

    Public Function Deprovision( _
        ByVal csEntry As CSEntry) _
        As DeprovisionAction Implements IMASynchronization.Deprovision
        ' TODO: Remove this throw statement if you implement this method
        Throw New EntryPointNotImplementedException
    End Function
End Class
