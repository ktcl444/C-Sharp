
Imports Microsoft.MetadirectoryServices

Public Class MAExtensionObject
    Implements IMASynchronization

    Public Sub Initialize() Implements IMASynchronization.Initialize
        ' TODO: Add initialization code here
    End Sub

    Public Sub Terminate() Implements IMASynchronization.Terminate
        ' TODO: Add termination code here
    End Sub

    Public Function ShouldProjectToMV(ByVal csentry As CSEntry, ByRef MVObjectType As String) As Boolean Implements IMASynchronization.ShouldProjectToMV
        'Initialize ObjectClass to Nothing
        MVObjectType = Nothing

        'If the company attribute is set to fabrikam then project
        If csentry("CompanyName").IsPresent Then
            If csentry("CompanyName").Value.ToLower = "fabrikam" AndAlso csentry.ObjectType = "Person" Then
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
        ' TODO: write your import attribute flow code
        Select Case FlowRuleName
            Case "IAFsamAccountname"
                'will flow the sAMAccountname attribute for fabrikam users.
                'Fabrikam Users will flow shortName attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("ShortName").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("sAMAccountname").Value = csentry("ShortName").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + csentry.ToString)
                End If

            Case "IAFMail"
                    'will flow the Mail attribute for fabrikam users.
                    'Fabrikam Users will flow InternetAddress attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("InternetAddress").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("Mail").Value = csentry("InternetAddress").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for User " + csentry.ToString)
                End If

            Case "IAFemployeeID"
                'will flow the employeeID attribute for fabrikam users.
                'Fabrikam Users will flow employeeID attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("employeeID").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("employeeID").Value = csentry("employeeID").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for User " + csentry.ToString)
                End If

            Case "IAFdisplayName"
                'will flow the displayName attribute for fabrikam users.
                'Fabrikam Users will flow displayName attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("FirstName").IsPresent AndAlso csentry("LastName").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("displayName").Value = csentry("lastName").Value & ", " & csentry("firstName").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or shortName attribute" + mventry.ToString)
                End If


            Case "IAFLastName"
                'will flow the LastName attribute for fabrikam users.
                'Fabrikam Users will flow sn attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("LastName").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("sn").Value = csentry("LastName").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "IAFFirstName"
                'will flow the FirstName attribute for fabrikam users.
                'Fabrikam Users will flow givenName attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("FirstName").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("givenName").Value = csentry("FirstName").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company for user " + mventry.ToString)
                End If

            Case "IAFtelephoneNumber"
                'will flow the TelephoneNumber attribute for fabrikam users.
                'Fabrikam Users will flow TelephoneNumber attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("OfficeNumber").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("TelephoneNumber").Value = csentry("OfficeNumber").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "IAFfacsimileTelephoneNumber"
                'will flow the OfficeFaxPhoneNumber attribute for fabrikam users.
                'Fabrikam Users will flow OfficeFAXPhoneNumber attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("OfficeFAXPhoneNumber").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("facsimileTelephoneNumber").Value = csentry("OfficeFAXPhoneNumber").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "IAFtitle"
                'will flow the Title attribute for fabrikam users.
                'Fabrikam Users will flow Title attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("Title").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("title").Value = csentry("Title").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user " + mventry.ToString)
                End If

            Case "IAFdepartment"
                'will flow the Department attribute for fabrikam users.
                'Fabrikam Users will flow Department attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("Department").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("Department").Value = csentry("Department").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + mventry.ToString)
                End If

            Case "IAFcompany"
                'will flow the CompanyName attribute for fabrikam users.
                'Fabrikam Users will flow CompanyName attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                        mventry("company").Value = csentry("CompanyName").Value
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + mventry.ToString)
                End If

            Case "IAFco"
                'will flow the officeCountry attribute for fabrikam users."
                'Fabrikam Users will flow officeCountry attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("officeCountry").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("co").Value = csentry("OfficeCountry").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + mventry.ToString)
                End If

            Case "IAFl"
                    'will flow the Officecity attribute for fabrikam users.
                    'Fabrikam Users will OfficeCity attribute from Notes
                If csentry("CompanyName").IsPresent Then
                    If csentry("OfficeCity").IsPresent Then
                        If csentry("CompanyName").Value.ToLower = "fabrikam" Then
                            mventry("l").Value = csentry("OfficeCity").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company attribute for user" + mventry.ToString)
                End If

            Case Else
                    ' TODO: remove the following statement and add your default script here
                    Throw New EntryPointNotImplementedException

        End Select
    End Sub

    Public Sub MapAttributesForExport(ByVal FlowRuleName As String, ByVal mventry As MVEntry, ByVal csentry As CSEntry) Implements IMASynchronization.MapAttributesForExport
        Select Case FlowRuleName
            Case "EAFShortName"
                'will flow the shortName attribute for Contoso users.
                'Contoso Users will flow shortName attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("sAMAccountName").IsPresent Then
                        If mventry("company").Value.ToLower = "contoso" Then
                            csentry("shortName").Value = mventry("sAMAccountName").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or Mail attribute" + mventry.ToString)
                End If

            Case "EAFemployeeID"
                'will flow the employeeID attribute for Contoso users.
                'Contoso Users will flow employeeID attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("employeeID").IsPresent Then
                        If mventry("company").Value.ToLower = "contoso" Then
                            csentry("employeeID").Value = mventry("employeeID").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or Mail attribute" + mventry.ToString)
                End If

            Case "EAFMailAddress"
                'will flow the mail attribute for Contoso users.
                'Contoso Users will flow mail attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("mail").IsPresent Then
                        If mventry("company").Value.ToLower = "contoso" Then
                            csentry("MailAddress").Value = mventry("mail").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or Mail attribute" + mventry.ToString)
                End If

            Case "EAFfirstName"
                'will flow the FirstName attribute for Contoso users.
                'Contoso Users will flow firstName attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("givenName").IsPresent Then
                        If mventry("company").Value.ToLower = "contoso" Then
                            csentry("firstName").Value = mventry("givenName").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or GivenName attribute" + mventry.ToString)
                End If

            Case "EAFLastName"
                'will flow the Last attribute for Contoso users.
                'Contoso Users will flow Last attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("sn").IsPresent Then
                        If mventry("company").Value.ToLower = "contoso" Then
                            csentry("LastName").Value = mventry("sn").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or SN attribute" + mventry.ToString)
                End If

            Case "EAFTitle"
                'will flow the title attribute for Contoso users.
                'Contoso Users will flow title attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("title").IsPresent Then
                        If mventry("company").Value.ToLower = "contoso" Then
                            csentry("Title").Value = mventry("title").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or title attribute" + mventry.ToString)
                End If

            Case "EAFDepartment"
                'will flow the Department attribute for Contoso users.
                'Contoso Users will flow Department attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("department").IsPresent Then
                        If mventry("company").Value.ToLower = "contoso" Then
                            csentry("department").Value = mventry("department").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or title attribute" + mventry.ToString)
                End If

            Case "EAFfacsimileTelephoneNumber"
                'will flow the facsimileTelephoneNumber attribute for Contoso users.
                'Contoso Users will flow OfficeFAXPhoneNumber attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("facsimileTelephoneNumber").IsPresent Then
                        If mventry("company").Value.ToLower = "contoso" Then
                            csentry("OfficeFaxPhoneNumber").Value = mventry("facsimileTelephoneNumber").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or facsimileTelephoneNumber attribute" + mventry.ToString)
                End If

            Case "EAFl"
                'will flow the l attribute for Contoso users.
                'Contoso Users will flow OfficeCity attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("l").IsPresent Then
                        If mventry("company").Value.ToLower = "contoso" Then
                            csentry("OfficeCity").Value = mventry("l").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or facsimileTelephoneNumber attribute" + mventry.ToString)
                End If

            Case "EAFco"
                'will flow the co attribute for Contoso users.
                'Contoso Users will flow OfficeCountry attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("co").IsPresent Then
                        If mventry("company").Value.ToLower = "contoso" Then
                            csentry("OfficeCountry").Value = mventry("co").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or facsimileTelephoneNumber attribute" + mventry.ToString)
                End If

            Case "EAFtelephoneNumber"
                'will flow the OfficeNumber attribute for Contoso users.
                'Contoso Users will flow OfficeNumber attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("TelephoneNumber").IsPresent Then
                        If mventry("company").Value.ToLower = "contoso" Then
                            csentry("OfficeNumber").Value = mventry("TelephoneNumber").Value
                        End If
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or TelephoneNumber attribute" + mventry.ToString)
                End If

            Case "EAFcompany"
                'will flow the company attribute for Contoso users.
                'Contoso Users will flow company attribute to Notes
                If mventry("company").IsPresent Then
                    If mventry("company").Value.ToLower = "contoso" Then
                        csentry("companyName").Value = mventry("company").Value
                    End If
                Else
                    Throw New UnexpectedDataException("Missing Company or TelephoneNumber attribute" + mventry.ToString)
                End If

            Case Else
                Throw New EntryPointNotImplementedException

        End Select
    End Sub

    Public Function Deprovision(ByVal csentry As CSEntry) As DeprovisionAction Implements IMASynchronization.Deprovision
        ' TODO: Remove this throw statement if you implement this method
        Throw New EntryPointNotImplementedException
    End Function
End Class
