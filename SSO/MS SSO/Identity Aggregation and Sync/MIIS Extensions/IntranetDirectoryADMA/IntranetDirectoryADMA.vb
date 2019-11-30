
Imports Microsoft.MetadirectoryServices
Imports System.Xml

Public Class MAExtensionObject
    Implements IMASynchronization

    Public Sub Initialize() Implements IMASynchronization.Initialize
        ' TODO: Add termination code here
    End Sub

    Public Sub Terminate() Implements IMASynchronization.Terminate
        ' TODO: Add termination code here
    End Sub

    Public Function ShouldProjectToMV(ByVal csentry As CSEntry, ByRef MVObjectType As String) As Boolean Implements IMASynchronization.ShouldProjectToMV
        'Initialize ObjectClass to Nothing
        MVObjectType = Nothing

        'If the company attribute is set to Contoso then project
        If csentry("company").IsPresent Then
            If csentry("company").Value.ToLower = "contoso" AndAlso csentry.ObjectType = "user" Then
                MVObjectType = "Person"
                Return True
            End If
        Else
            Throw New UnexpectedDataException("Missing Company attribute for user" + csentry.ToString)
        End If
    End Function

    Public Function FilterForDisconnection(ByVal csentry As CSEntry) As Boolean Implements IMASynchronization.FilterForDisconnection
        ' TODO: Add connector filter code here
        Throw New EntryPointNotImplementedException
    End Function

    Public Sub MapAttributesForJoin(ByVal FlowRuleName As String, ByVal csentry As CSEntry, ByRef values As ValueCollection) Implements IMASynchronization.MapAttributesForJoin
        ' TODO: Add join mapping code here
        Throw New EntryPointNotImplementedException
    End Sub

    Public Function ResolveJoinSearch(ByVal joinCriteriaName As String, ByVal csentry As CSEntry, ByVal rgmventry() As MVEntry, ByRef imventry As Integer, ByRef MVObjectType As String) As Boolean Implements IMASynchronization.ResolveJoinSearch
        ' TODO: Add join resolution code here
        Throw New EntryPointNotImplementedException
    End Function


    Public Sub MapAttributesForImport(ByVal FlowRuleName As String, ByVal csentry As CSEntry, ByVal mventry As MVEntry) Implements IMASynchronization.MapAttributesForImport

        Select Case FlowRuleName

            Case "IAFemployeeID"
                'will flow the employeeID attribute for Contoso users.
                'Contoso Users will flow employeeID attribute from AD
                If csentry("company").IsPresent AndAlso csentry("employeeID").IsPresent Then
                    If csentry("company").Value.ToLower = "contoso" Then
                        mventry("employeeID").Value = csentry("employeeID").Value
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or employeeID attribute for user" + csentry.ToString)
                End If

            Case "IAFMail"
                'will flow the mail attribute for Contoso users.
                'Contoso Users will flow mail attribute from AD
                If csentry("company").IsPresent AndAlso csentry("mail").IsPresent Then
                    If csentry("company").Value.ToLower = "contoso" Then
                        mventry("mail").Value = csentry("mail").Value
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or Mail attribute for user" + csentry.ToString)
                End If

            Case "IAFdisplayName"
                'will flow the displayName attribute for Contoso users.
                'Contoso Users will flow displayName attribute from AD.
                If csentry("company").IsPresent Then
                    If csentry("displayName").IsPresent Then
                        If csentry("company").Value.ToLower = "contoso" Then
                            mventry("displayName").Value = csentry("displayName").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for User " + csentry.ToString)
                End If

            Case "IAFsAMAccountName"
                'will flow the mail attribute for Contoso users.
                'Contoso Users will flow mail attribute from AD
                If csentry("company").IsPresent Then
                    If csentry("company").Value.ToLower = "contoso" Then
                        mventry("samAccountname").Value = csentry("samAccountname").Value
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + csentry.ToString)
                End If

            Case "IAFsn"
                'will flow the SN attribute for Contoso users.
                'Contoso Users will flow SN attribute from AD
                If csentry("Company").IsPresent AndAlso csentry("sn").IsPresent Then
                    If csentry("Company").Value.ToLower = "contoso" Then
                        mventry("sn").Value = csentry("sn").Value
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or SN attribute" + csentry.ToString)
                End If

            Case "IAFgivenName"
                'will flow the FirstName attribute for Contoso users.
                'Contoso Users will flow FirstName attribute from AD
                If csentry("Company").IsPresent AndAlso csentry("givenName").IsPresent Then
                    If csentry("Company").Value.ToLower = "contoso" Then
                        mventry("givenName").Value = csentry("givenName").Value
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or givenName attribute for user " + csentry.ToString)
                End If

            Case "IAFtelephoneNumber"
                'will flow the TelephoneNumber attribute for Contoso users.
                'Contoso Users will flow TelephoneNumber attribute from AD
                If csentry("Company").IsPresent Then
                    If csentry("TelephoneNumber").IsPresent Then
                        If csentry("Company").Value.ToLower = "contoso" Then
                            mventry("TelephoneNumber").Value = csentry("TelephoneNumber").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + csentry.ToString)
                End If

            Case "IAFfacsimileTelephoneNumber"
                'will flow the FaxNumber attribute for Contoso users.
                'Contoso Users will flow FaxNumber attribute from AD
                If csentry("Company").IsPresent Then
                    If csentry("facsimileTelephoneNumber").IsPresent Then
                        If csentry("Company").Value.ToLower = "contoso" Then
                            mventry("facsimileTelephoneNumber").Value = csentry("facsimileTelephoneNumber").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + csentry.ToString)
                End If

            Case "IAFtitle"
                'will flow the Title attribute for Contoso users.
                'Contoso Users will flow Title attribute from AD
                If csentry("Company").IsPresent Then
                    If csentry("Title").IsPresent Then
                        If csentry("Company").Value.ToLower = "contoso" Then
                            mventry("title").Value = csentry("Title").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + csentry.ToString)
                End If

            Case "IAFdepartment"
                'will flow the Department attribute for Contoso users.
                'Contoso Users will flow Department attribute from AD
                If csentry("Company").IsPresent Then
                    If csentry("Department").IsPresent Then
                        If csentry("Company").Value.ToLower = "contoso" Then
                            mventry("Department").Value = csentry("Department").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + csentry.ToString)
                End If

            Case "IAFcompany"
                'will flow the company attribute for Contoso users.
                'Contoso Users will flow company attribute from AD
                If csentry("company").IsPresent Then
                    If csentry("company").Value.ToLower = "contoso" Then
                        mventry("company").Value = csentry("company").Value
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + csentry.ToString)
                End If

            Case "IAFco"
                'will flow the co attribute for Contoso users.
                'Contoso Users will flow co attribute from AD
                If csentry("company").IsPresent Then
                    If csentry("co").IsPresent Then
                        If csentry("company").Value.ToLower = "contoso" Then
                            mventry("co").Value = csentry("co").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + csentry.ToString)
                End If

            Case "IAFc"
                'will flow the c attribute for Contoso users.
                'Contoso Users will flow c attribute from AD
                If csentry("company").IsPresent Then
                    If csentry("c").IsPresent Then
                        If csentry("company").Value.ToLower = "contoso" Then
                            mventry("c").Value = csentry("c").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + csentry.ToString)
                End If

            Case "IAFl"
                'will flow the l attribute for Contoso users.
                'Contoso Users will flow l attribute from AD
                If csentry("company").IsPresent Then
                    If csentry("l").IsPresent Then
                        If csentry("company").Value.ToLower = "contoso" Then
                            mventry("l").Value = csentry("l").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + csentry.ToString)
                End If

            Case Else
                Throw New EntryPointNotImplementedException
        End Select
    End Sub

    Public Sub MapAttributesForExport(ByVal FlowRuleName As String, ByVal mventry As MVEntry, ByVal csentry As CSEntry) Implements IMASynchronization.MapAttributesForExport
        Select Case FlowRuleName
            Case "EAFMail"
                'will flow the mail attribute for fabrikam users.
                'Fabrikam Users will flow mail attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("mail").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("mail").Value = mventry("mail").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "EAFtargetAddress"
                'will flow the mail attribute for fabrikam users.
                'Fabrikam Users will flow targetAddress attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("mail").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("targetAddress").Value = mventry("mail").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "EAFsAMAccountName"
                'will flow the mailNickName attribute for fabrikam users.
                'Fabrikam Users will flow shortName attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("sAMAccountName").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("sAMAccountName").Value = mventry("samAccountName").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "EAFmailNickname"
                'will flow the mailNickName attribute for fabrikam users.
                'Fabrikam Users will flow shortName attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("sAMAccountName").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("mailNickName").Value = mventry("sAMAccountName").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "EAFemployeeID"
                'will flow the employeeID attribute for fabrikam users.
                'Fabrikam Users will flow employeeID attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("employeeID").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("employeeID").Value = mventry("employeeID").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute from user " + mventry.ToString)
                End If

            Case "EAFdisplayName"
                'will flow the displayName attribute for fabrikam users.
                'Fabrikam Users will flow displayName attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("displayName").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("displayName").Value = mventry("displayName").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute from user " + mventry.ToString)
                End If

            Case "EAFgivenName"
                'will flow the givenName attribute for Fabrikam users.
                'Fabrikam Users will flow givenName attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("givenName").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("givenName").Value = mventry("givenName").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "EAFsn"
                'will flow the sn attribute for fabrikam users.
                'fabrikam Users will flow sn attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("sn").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("sn").Value = mventry("sn").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "EAFtitle"
                'will flow the title attribute for Contoso users.
                'Contoso Users will flow title attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("title").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("title").Value = mventry("title").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + mventry.ToString)
                End If

            Case "EAFdepartment"
                'will flow the Department attribute for Fabrikam users.
                'Fabrikam Users will flow Department attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("department").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("department").Value = mventry("department").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "EAFfacsimileTelephoneNumber"
                'will flow the facsimileTelephoneNumber attribute for Fabrikam users.
                'Fabrikam Users populate facsimileTelephoneNumber attribute in metaverse              If mventry("company").IsPresent Then
                If mventry("company").IsPresent Then
                    If mventry("facsimileTelephoneNumber").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("facsimileTelephoneNumber").Value = mventry("facsimileTelephoneNumber").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for User" + mventry.ToString)
                End If

            Case "EAFtelephoneNumber"
                'will flow the TelephoneNumber attribute for Fabrikam users.
                'Fabrikam Users populate TelephoneNumber attribute in metaverse
                If mventry("company").IsPresent Then
                    If mventry("TelephoneNumber").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("TelephoneNumber").Value = mventry("TelephoneNumber").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "EAFc"
                'will flow the c attribute for Fabrikam users.
                'Fabrikam Users will populate c attribute in metaverse
                If mventry("company").IsPresent Then
                    If mventry("c").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("c").Value = mventry("c").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "EAFco"
                'will flow the co attribute for Fabrikam users.
                'Fabrikam Users will populate co attribute in metaverse
                If mventry("company").IsPresent Then
                    If mventry("co").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("co").Value = mventry("co").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "EAFl"
                'will flow the l attribute for Fabrikam users.
                'Fabrikam Users will flow l attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("l").IsPresent Then
                        If mventry("company").Value.ToLower = "fabrikam" Then
                            csentry("l").Value = mventry("l").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If


            Case "EAFcompany"
                'will flow the company attribute for Fabrikam users.
                'Fabrikam Users will flow company attribute from Notes
                If mventry("company").IsPresent Then
                    If mventry("company").Value.ToLower = "fabrikam" Then
                        csentry("company").Value = mventry("company").Value
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case Else
                Throw New EntryPointNotImplementedException
        End Select

    End Sub

    Public Function Deprovision(ByVal csentry As CSEntry) As DeprovisionAction Implements IMASynchronization.Deprovision
        ' TODO: Add deprovisioning code here
        Throw New EntryPointNotImplementedException

    End Function
End Class
