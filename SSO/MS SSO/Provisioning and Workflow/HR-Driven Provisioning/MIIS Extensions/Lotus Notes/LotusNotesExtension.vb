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
Imports System.Exception

' <summary>
' Implements IMVSynchronization interface to
' provide rules extension to the Lotus Notes Management Agent
' </summary>

Public Class LotusNotesExtension
    Implements IMASynchronization

    ' Declaration for Group types in Active Directory
    Const ADS_GROUP_TYPE_GLOBAL_GROUP = &H2
    Const ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP = &H4
    Const ADS_GROUP_TYPE_UNIVERSAL_GROUP = &H8
    Const ADS_GROUP_TYPE_SECURITY_ENABLED = &H80000000

    ' Declaration for Group types in Lotus Notes
    Const LN_GROUP_TYPE_MULTIPURPOSE = 0
    Const LN_GROUP_TYPE_MAIL_ONLY = 1
    Const LN_GROUP_TYPE_ACL_ONLY = 2

    ' <summary> 
    ' Initializes the rules extension object.
    ' </summary>
    ' <returns></returns>

    Public Sub Initialize() Implements IMASynchronization.Initialize
        ' TODO: Add initialization code here
    End Sub

    ' <summary>
    ' Is called when the rules extension object 
    ' is no longer needed.
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
        ByVal csEntry As CSEntry, ByRef mvObjectType As String) _
        As Boolean Implements IMASynchronization.ShouldProjectToMV
        ' TODO: Add code here
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
    ' Is called when a join rule is configured to use a 
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
        Throw New EntryPointNotImplementedException

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

        ' Checking for the Attributes of the flowRuleName
        ' If EAFshortName, check for the presence of employee account
        Select Case flowRuleName
            Case "EAFshortName"
                If mvEntry("sAMAccountName").IsPresent Then
                    csEntry("shortName").Value = _
                        mvEntry("sAMAccountName").Value()
                Else
                    Throw New UnexpectedDataException( _
                        "sAMAccountName expected for employee= " + _
                        mvEntry("employeeId").Value)
                End If

                ' If EAFinternetAddress, check the Metaverse Entry as company 
                ' or delete connector space entry of InternetAddress  
            Case "EAFinternetAddress"
                If mvEntry("company").Value.ToLower = "fabrikam" Then
                    If mvEntry("mail").IsPresent Then

                        ' This is a real Notes user, only set the 
                        ' Internet Address for them
                        csEntry("InternetAddress").Value = _
                                mvEntry("mail").Value.ToLower
                    End If
                Else
                    csEntry("InternetAddress").Delete()
                End If

                ' If EAFMailAddress, check the Metaverse Entry as company
                ' or delete connector space entry of MailAddress
            Case "EAFMailAddress"
                If mvEntry("company").Value.ToLower = "contoso" Then
                    If mvEntry("mail").IsPresent Then

                        ' Map the mail address of the MV entry to CS entry
                        csEntry("MailAddress").Value = _
                                mvEntry("mail").Value.ToLower
                    End If
                Else
                    csEntry("MailAddress").Delete()

                End If

                ' If EAFgroupType, check for Metaverse Entry of groupType
                ' and connector space entry of groupType
            Case "EAFgroupType"

                ' Check for the existence of group type
                If mvEntry("groupType").IsPresent Then

                    ' For all possibel cases of group type
                    Select Case mvEntry("groupType").Value
                        Case (ADS_GROUP_TYPE_GLOBAL_GROUP Or _
                                ADS_GROUP_TYPE_SECURITY_ENABLED), _
                                (ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP Or _
                                ADS_GROUP_TYPE_SECURITY_ENABLED), _
                                (ADS_GROUP_TYPE_UNIVERSAL_GROUP Or _
                                ADS_GROUP_TYPE_SECURITY_ENABLED)
                            If mvEntry("mailNickName").IsPresent Then

                                ' Set the Lotus Notes groupType to Multi-purpose
                                csEntry("groupType").Value = _
                                        LN_GROUP_TYPE_MULTIPURPOSE
                            Else

                                ' Set the Lotus Notes groupType to ACL only
                                csEntry("groupType").Value = _
                                            LN_GROUP_TYPE_ACL_ONLY
                            End If
                        Case ADS_GROUP_TYPE_GLOBAL_GROUP, _
                                ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP, _
                                ADS_GROUP_TYPE_UNIVERSAL_GROUP

                            ' Set the Lotus Notes groupType to Mail only
                            csEntry("groupType").Value = _
                                        LN_GROUP_TYPE_MAIL_ONLY
                    End Select
                End If
            Case Else
                Throw New EntryPointNotImplementedException

        End Select

    End Sub

    ' <summary>
    ' Is called when a connector space entry is deprovisioned.
    ' </summary>
    ' <param name="csEntry">connector space entry </param>
    ' <returns>Returns DeprovisionAction values that determines 
    ' which action should be taken on the connector space entry.</returns>

    Public Function Deprovision(ByVal csEntry As CSEntry) _
        As DeprovisionAction Implements IMASynchronization.Deprovision
        ' TODO: Remove this throw statement if you implement this method
        Throw New EntryPointNotImplementedException
    End Function
End Class
