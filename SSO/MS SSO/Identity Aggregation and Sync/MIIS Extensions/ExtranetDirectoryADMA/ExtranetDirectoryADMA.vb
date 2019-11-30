
Imports Microsoft.MetadirectoryServices
Imports System.xml



Public Class MAExtensionObject
    Implements IMASynchronization
    Dim extUPNSuffix As String
    Dim extMailDomain As String
    Dim caDN As String
    Dim caSubject As String



    Public Sub Initialize() Implements IMASynchronization.Initialize
        Const SCENARIO_XML_CONFIG = "\IDMGTExtranet.xml"

        Dim config As XmlDocument = New XmlDocument
        Dim dir As String = Utils.ExtensionsDirectory()
        config.Load(dir + SCENARIO_XML_CONFIG)

        Dim rnode As XmlNode = config.SelectSingleNode("rules-extension-properties/account-Sync/Extranet")

        Dim ADMailnode As XmlNode = rnode.SelectSingleNode("ExtMailDomain")
        extMailDomain = ADMailnode.InnerText

        Dim ADUPNnodeEx As XmlNode = rnode.SelectSingleNode("Ext-upn-suffix")
        extUPNsuffix = ADUPNnodeEx.InnerText

        Dim CaDnNode As XmlNode = rnode.SelectSingleNode("issuing-CA-dn")
        caDN = CaDnNode.InnerText

        Dim CASubjectNode As XmlNode = rnode.SelectSingleNode("CA-subject-prefix")
        caSubject = CASubjectNode.InnerText
    End Sub

    Public Sub Terminate() Implements IMASynchronization.Terminate
        ' TODO: Add termination code here
    End Sub


    Public Function ShouldProjectToMV(ByVal csentry As Microsoft.MetadirectoryServices.CSEntry, ByRef MVObjectClass As String) As Boolean Implements Microsoft.MetadirectoryServices.IMASynchronization.ShouldProjectToMV
        ' TODO: Remove this throw statement if you implement this method
        Throw New EntryPointNotImplementedException
    End Function

    Public Function FilterForDisconnection(ByVal csentry As Microsoft.MetadirectoryServices.CSEntry) As Boolean Implements Microsoft.MetadirectoryServices.IMASynchronization.FilterForDisconnection
        ' TODO: Add stay-disconnector code here
        Throw New EntryPointNotImplementedException
    End Function

    Public Sub MapAttributesForJoin(ByVal JoinRuleName As String, ByVal csentry As Microsoft.MetadirectoryServices.CSEntry, ByRef values As Microsoft.MetadirectoryServices.ValueCollection) Implements Microsoft.MetadirectoryServices.IMASynchronization.MapAttributesForJoin
        ' TODO: Add join mapping code here
        Throw New EntryPointNotImplementedException
    End Sub

    Public Function ResolveJoinSearch(ByVal JoinResolution As String, ByVal csentry As Microsoft.MetadirectoryServices.CSEntry, ByVal rgmventry() As Microsoft.MetadirectoryServices.MVEntry, ByRef MVEntry As Integer, ByRef MVObjectClass As String) As Boolean Implements Microsoft.MetadirectoryServices.IMASynchronization.ResolveJoinSearch
        ' TODO: Add join resolution code here
        Throw New EntryPointNotImplementedException
    End Function

    Public Sub MapAttributesForImport(ByVal FlowRuleName As String, ByVal csentry As Microsoft.MetadirectoryServices.CSEntry, ByVal mventry As Microsoft.MetadirectoryServices.MVEntry) Implements Microsoft.MetadirectoryServices.IMASynchronization.MapAttributesForImport
        ' TODO: Add Import Attribute flow code here
        Throw New EntryPointNotImplementedException
    End Sub

    Public Sub MapAttributesForExport(ByVal FlowRuleName As String, ByVal mventry As Microsoft.MetadirectoryServices.MVEntry, ByVal csentry As Microsoft.MetadirectoryServices.CSEntry) Implements Microsoft.MetadirectoryServices.IMASynchronization.MapAttributesForExport

        Select Case FlowRuleName

            Case "EafUserPrincipalName"

                If mventry("userPrincipalName").IsPresent Then
                    'building UPN with the value of the userPrincipalName
                    'in the metaverse.  This means it changes in the 
                    'infrastructure AD
                    Dim UPN As String = mventry("userPrincipalName").Value
                    Dim Pos As Integer = UPN.IndexOf("@") + 1
                    Dim UPNPrefix As String = UPN.Substring(0, Pos)
                    csentry("userPrincipalName").Value = UPNPrefix & extUPNsuffix
                ElseIf mventry("samAccountName").IsPresent Then
                    'Building the UserPrincipalName from the samAccountName
                    csentry("userPrincipalName").Value = mventry("samAccountName").Value & "@" & ExtUPNsuffix
                End If

            Case "altSecurityIdentities"

                Dim RDN As String
                'Building the alsSecurityIdentities value based on a XML Value and metaverse SamAccountName
                ''X509:<I>DC=com,DC=contoso,DC=corp,CN=Contoso Root CA<S>DC=com,DC=contoso,DC=corp,DC=na,OU=Employees,CN=UK0277946,E=Jsmith@contoso.com
                csentry("altSecurityIdentities").Value = "X509:<I>" & caDN & "<S>" & caSubject & "," & RDN & ",E=" & mventry("samAccountName").Value & extMailDomain


            Case Else
                Throw New EntryPointNotImplementedException
        End Select
    End Sub

    Public Function Deprovision(ByVal csentry As CSEntry) As DeprovisionAction Implements IMASynchronization.Deprovision
        ' TODO: Remove this throw statement if you implement this method
        Throw New EntryPointNotImplementedException
    End Function

End Class
