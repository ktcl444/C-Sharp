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
Imports ContosoUtilties.ContosoUtilities
Imports ContosoUtilties.Constants
Imports System.Exception

' <summary>
' Implements IMVSynchronization interface to
' provide rules extension to the Contractor Workflow Management Agent
' </summary>

Public Class SelfServiceProvisioningMA
    Implements IMASynchronization

    ' Constants for Contractor ID
    Const MAX_CONTRACTOR_ID = 20000

    Dim defaultManagerEmail As String
    Dim contractorPrefix As String

    ' <summary>
    ' Initializes the rules extension object.
    ' </summary>
    ' <returns></returns>

    Public Sub Initialize() Implements IMASynchronization.Initialize



        ' Load the XML from the corresponding location
        Dim config As XmlDocument = New XmlDocument
        Dim extnDirectory As String = Utils.ExtensionsDirectory()
        config.Load(extnDirectory + SCENARIO_XML_CONFIG)

        ' Get information about Intranet
        Dim rNode As XmlNode = _
                config.SelectSingleNode("provisioning/general-definitions")
        Dim node As XmlNode = rNode.SelectSingleNode("defaultManagerEmail")

        ' Assign the email of the manager to a variable
        defaultManagerEmail = node.InnerText

        node = rNode.SelectSingleNode("contractorPrefix")
        contractorPrefix = node.InnerText

    End Sub

    ' <summary>
    ' Called when the rules extension
    ' object is no longer needed.
    ' </summary>
    ' <returns></returns>

    Public Sub Terminate() Implements IMASynchronization.Terminate
        ' TODO: Add termination code here
    End Sub

    ' <summary>
    ' Determines if
    ' a new connector space object should be projected to a
    ' new metaverse object when the connector space object
    ' does not join to an existing metaverse object.
    ' </summary>
    ' <param name="csEntry">connector space entry
    ' corresponding to contractor management agent</param>
    ' <param name="mvObjectType">Object Type corresponding 
    ' to the Metaverse entry</param>
    ' <returns>Returns true if the connector space entry 
    ' should be projected. </returns>

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
    ' <param name="flowRuleName">Flow rule name</param>
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

        ' For all the possible values of flowRuleName
        Select Case flowRuleName

            ' In case of display name
        Case "IAFdisplayName"
                If csEntry("FIRST_NAME").IsPresent And _
                           csEntry("LAST_NAME").IsPresent Then
                    mvEntry("displayName").Value = _
                        csEntry("LAST_NAME").Value + ", " + _
                             csEntry("FIRST_NAME").Value
                End If

                ' In case of employee account name
            Case "IAFsAMAccountName"

                ' checking to see if the metaverse attribute sAMAccountName 
                ' is presentif it is then don't do anything.
                If mvEntry("sAMAccountName").IsPresent Then
                Else

                    ' Generate the sAMAccountName
                    Dim samName As String
                    samName = New ContosoUtilties.ContosoUtilities(). _
                        GeneratesAMAccountName( _
                            csEntry, mvEntry, True, contractorPrefix)

                    ' Set the sAMAccountName Metaverse value from the SamName we built
                    mvEntry("sAMAccountName").Value = samName

                End If

            Case "IAFcn"

                ' checking to see if the metaverse attribute sAMAccountName is 
                ' present if it is then use that. Otherwise calculate it
                If mvEntry("sAMAccountName").IsPresent Then
                    mvEntry("contractor").Value = mvEntry("sAMAccountName").Value
                Else

                    ' Generate sAMAccountName and use it as contractor
                    Dim cn As String
                    cn = New ContosoUtilties.ContosoUtilities(). _
                            GeneratesAMAccountName(csEntry, mvEntry, _
                                            True, contractorPrefix)

                    'Generate contractor Metaverse value from the SamName we built
                    mvEntry("cn").Value = cn
                End If

                ' In case of employee status
            Case "IAFemployeeStatus"
                mvEntry("employeeStatus").Value = EMPLOYEE_ACTIVE

            Case "IAFmanagerEmail"
                Dim findResults() As MVEntry

                ' Check for the absence of the managers ID
                If Not csEntry("MANAGER_ID").IsPresent Then
                    Throw New UnexpectedDataException( _
                        "MANAGER_ID expected for contractor= " _
                              + csEntry("CONTRACTOR_ID").Value)
                End If

                ' Find the managers e-mail address based on employeeId, 
                ' since we can not search on reference values
                findResults = Utils.FindMVEntries( _
                        "employeeId", csEntry("MANAGER_ID").Value, 1)
                If findResults.Length = 0 Then
                    mvEntry("managerEmail").Value = defaultManagerEmail
                Else

                    ' Get the e-mail address
                    If findResults(0)("mail").IsPresent Then
                        mvEntry("managerEmail").Value = findResults(0)("mail").Value
                    Else
                        mvEntry("managerEmail").Value = defaultManagerEmail
                    End If
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

        Select Case flowRuleName

            Case "EAFUpdateCount"

                ' Check for the existence of the Update count in CS Entry
                If csEntry("UPDATE_COUNT").IsPresent Then
                    If (csEntry("UPDATE_COUNT").IntegerValue = MAX_CONTRACTOR_ID) Then
                        csEntry("UPDATE_COUNT").IntegerValue = 0
                    Else

                        ' Increment the value of the count by 1
                        csEntry("UPDATE_COUNT").IntegerValue = _
                                csEntry("UPDATE_COUNT").IntegerValue + 1
                    End If
                End If
            Case Else
                Throw New EntryPointNotImplementedException
        End Select

    End Sub

    ' <summary>
    ' Called when a connector space entry is deprovisioned.
    ' </summary>
    ' <param name="csEntry">connector space entry </param>
    ' <returns>Returns DeprovisionAction values that determines 
    ' which action should be taken on the connector space entry.</returns>

    Public Function Deprovision( _
        ByVal csEntry As CSEntry) As DeprovisionAction _
        Implements IMASynchronization.Deprovision
        ' TODO: Remove this throw statement if you implement this method
        Throw New EntryPointNotImplementedException
    End Function

End Class
