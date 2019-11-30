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
Imports ContosoUtilties.Constants

' <summary>
' Implements IMVSynchronization interface to
' provide rules extension to the Extranet Directory Management Agent
' </summary>

Public Class ExtranetActiveDirectoryExtension
    Implements IMASynchronization

    ' Declaration of variables
    Dim extUPNSuffix As String
    Dim extMailDomain As String
    Dim caDN As String
    Dim caSubject As String

    ' <summary>
    ' Initializes the rules extension object.
    ' </summary>
    ' <param name="csEntry">Connector Space Entry</param>
    ' <returns></returns>

    Public Sub Initialize() Implements IMASynchronization.Initialize


        ' Instantiate the config as a new XmlDocument 
        ' Get Directory Extensions and load the XmlDocument
        Dim config As XmlDocument = New XmlDocument
        Dim dir As String = Utils.ExtensionsDirectory()
        config.Load(dir + SCENARIO_XML_CONFIG)

        ' Load the values of the Intranet attributes from the config file
        ' and assign it to the Global variables respectively
        Dim rNode As XmlNode = _
            config.SelectSingleNode( _
                "provisioning/account-provisioning/extranet")

        ' Extranet Email domain
        Dim strADMailNode As XmlNode = _
                rNode.SelectSingleNode("ext-mail-domain")
        extMailDomain = strADMailNode.InnerText

        ' Extranet UPN suffix
        Dim strADUPNnodeEx As XmlNode = _
                rNode.SelectSingleNode("ext-upn-suffix")
        extUPNSuffix = strADUPNnodeEx.InnerText

        ' Extranet CA-dn
        Dim strCaDnNode As XmlNode = _
                rNode.SelectSingleNode("issuing-CA-dn")
        caDN = strCaDnNode.InnerText

        ' Extranet Subject prefix
        Dim strCASubjectNode As XmlNode = _
                rNode.SelectSingleNode("CA-subject-prefix")
        caSubject = strCASubjectNode.InnerText

    End Sub

    ' <summary>
    ' Called when the rules extension object 
    ' is no longer needed.
    ' </summary>
    ' <returns></returns>

    Public Sub Terminate() Implements IMASynchronization.Terminate
        ' TODO: Add termination code here
    End Sub

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
        ByVal csEntry As Microsoft.MetadirectoryServices.CSEntry, _
        ByRef mvObjectClass As String) _
        As Boolean Implements _
        Microsoft.MetadirectoryServices.IMASynchronization.ShouldProjectToMV
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
        ByVal csEntry As Microsoft.MetadirectoryServices.CSEntry) _
        As Boolean Implements _
        Microsoft.MetadirectoryServices.IMASynchronization. _
                                                FilterForDisconnection
        ' TODO: Add stay-disconnector code here
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
        ByVal joinRuleName As String, _
        ByVal csEntry As Microsoft.MetadirectoryServices.CSEntry, _
        ByRef values As Microsoft.MetadirectoryServices.ValueCollection) _
        Implements Microsoft.MetadirectoryServices. _
                                    IMASynchronization.MapAttributesForJoin
        ' TODO: Add join mapping code here
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
        ByVal joinResolution As String, _
        ByVal csEntry As Microsoft.MetadirectoryServices.CSEntry, _
        ByVal rgmvEntry() As Microsoft.MetadirectoryServices.MVEntry, _
        ByRef mvEntry As Integer, _
        ByRef mvObjectClass As String) _
        As Boolean Implements _
        Microsoft.MetadirectoryServices.IMASynchronization.ResolveJoinSearch
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
        ByVal csEntry As Microsoft.MetadirectoryServices.CSEntry, _
        ByVal mvEntry As Microsoft.MetadirectoryServices.MVEntry) _
        Implements Microsoft.MetadirectoryServices. _
                                    IMASynchronization.MapAttributesForImport
        ' TODO: Add Import Attribute flow code here
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
        ByVal mvEntry As Microsoft.MetadirectoryServices.MVEntry, _
        ByVal csEntry As Microsoft.MetadirectoryServices.CSEntry) _
        Implements Microsoft.MetadirectoryServices. _
                                IMASynchronization.MapAttributesForExport

        ' For all the possible values of flowRuleName
        Select Case flowRuleName

            Case "EafUserPrincipalName"

                ' If selected flowRuleName is EafUserPrincipalName, then
                ' check for its Metaverse entry
                If mvEntry("userPrincipalName").IsPresent Then

                    ' building UPN with the value of the userPrincipalName
                    ' in the metaverse.  This means it changes in the 
                    ' infrastructure AD
                    Dim strUPN As String = mvEntry("userPrincipalName").Value
                    Dim iPos As Integer = strUPN.IndexOf("@") + 1
                    Dim strUPNPrefix As String = strUPN.Substring(0, iPos)
                    csEntry("userPrincipalName").Value = strUPNPrefix & extUPNSuffix
                ElseIf mvEntry("samAccountName").IsPresent Then

                    ' Building the UserPrincipalName from the samAccountName
                    csEntry("userPrincipalName").Value = _
                        mvEntry("samAccountName").Value & "@" & extUPNSuffix
                End If

            Case "altSecurityIdentities"

                ' Building the alsSecurityIdentities value based 
                ' on a XML Value and metaverse SamAccountName
                ' X509:<I>DC=com,DC=contoso,DC=corp,CN=Contoso Root 
                ' CA<S>DC=com,DC=contoso,DC=corp,DC=na,OU=Employees,
                ' CN=UK0277946,E=Jsmith@contoso.com
                csEntry("altSecurityIdentities").Value = _
                    "X509:<I>" & caDN & "<S>" & caSubject & ",CN=" & _
                    mvEntry("samAccountName").Value & ",E=" & _
                    mvEntry("samAccountName").Value & extMailDomain

            Case "EAFemployeeStatus"

                Dim currentValue As Long = 0

                ' get current value of userAccountControl
                If csEntry("userAccountControl").IsPresent Then
                    currentValue = _
                        csEntry("userAccountControl").IntegerValue()
                Else
                    currentValue = UF_NORMAL_ACCOUNT
                End If

                'If condition included to avoid "employeeStaus not present" error
                If Not mvEntry("employeeStatus").IsPresent Then
                    ' Make sure the account is not disabled
                    csEntry("userAccountControl").IntegerValue = _
                        (currentValue Or UF_NORMAL_ACCOUNT) And _
                        (Not UF_ACCOUNTDISABLE)
                    Exit Select
                End If

                ' Set userAccountControl to reflect employeeStatus
                Select Case mvEntry("employeeStatus").Value
                    Case EMPLOYEE_ACTIVE

                        ' Make sure the account is not disabled
                        csEntry("userAccountControl").IntegerValue = _
                            (currentValue Or UF_NORMAL_ACCOUNT) And _
                            (Not UF_ACCOUNTDISABLE)
                    Case EMPLOYEE_LEAVE, EMPLOYEE_RETIRED, _
                        EMPLOYEE_DISABLED

                        ' ensure the account is set to disabled and 
                        ' maintain current settings
                        csEntry("userAccountControl").IntegerValue = _
                            currentValue Or UF_ACCOUNTDISABLE
                    Case Else

                        ' employeeStatus must be active, leave, 
                        ' retired or disabled to be valid
                        ' any other case is an error condition for this object. 

                        ' Throw an exception to abort this object's synchronization. 
                        Throw New UnexpectedDataException( _
                            "Illegal employeeStatus= " + _
                            mvEntry("employeeStatus").Value.ToString + _
                            " for employeeId " + _
                            mvEntry("employeeId").Value.ToString)
                End Select

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

                            ' Convert our date to a LargeInteger
                            oLargeInteger = oDate.ToFileTime()
                            csEntry("accountExpires").IntegerValue = oLargeInteger
                    End Select
                    'if employeeStatus is not present
                Else
                    'TODO
                End If

            Case Else
                    Throw New EntryPointNotImplementedException
        End Select

    End Sub

    ' <summary>
    ' This method is called when a connector space entry is deprovisioned.
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
