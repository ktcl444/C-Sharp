'##############################################################################
' Microsoft Solutions for Security
' Copyright (c) 2004 Microsoft Corporation. All rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
' THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
' AND INFORMATION REMAINS WITH THE USER.
'##############################################################################

Imports System
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports System.DirectoryServices
Imports AppDevASPLib.Contoso.Web.UI.Localization

Namespace Contoso.Identity.Template

    ' <summary>
    '   The LdapAuthentication class provides authentication of
    '   a user against an LDAP directory.
    ' </summary>
    Public Class LdapAuthentication

        Private Const SOURCE_NAME As String = "AppDevASPExtranet"
        Private _path As String
        Private _filterAttribute As String

        ' <summary>
        '   Public default constructor of the LdapAuthentication
        '   helper class
        ' </summary>
        Public Sub New()

        End Sub 'New

        ' <summary>
        '   Public constructor of the LdapAuthentication class that
        '   initializes the LDAP path to be used
        ' </summary>
        Public Sub New(ByVal path As String)

            _path = path

        End Sub 'New

        ' <summary>
        '   Forces authentication by binding to the native AdsObject.
        ' </summary>
        ' <param name="domain"></param>
        ' <param name="username"></param>
        ' <param name="pwd"></param>
        ' <returns>Returns true after a succesful binding and returns
        ' false if any kind of error occurs</returns>
        Public Function IsAuthenticated(ByVal domain As String, ByVal userName As String, ByVal pwd As String) As Boolean

            Dim domainAndUsername As String = domain + "\" + userName

            Try
                Dim entry As New DirectoryEntry(_path, domainAndUsername, pwd, AuthenticationTypes.Secure)

                ' Bind to the native AdsObject to force authentication.
                Dim obj As [Object] = entry.NativeObject
                Dim search As New DirectorySearcher(entry)
                search.Filter = "(userPrincipalName=" + userName + "@" + domain + ")"
                search.PropertiesToLoad.Add("cn")
                Dim result As SearchResult = search.FindOne()

                If result Is Nothing Then
                    Return False
                End If

                ' Audit successful logon here
                '				EventLog.WriteEntry(sourceName, "Logon success for user " + domainAndUsername, EventLogEntryType.SuccessAudit);				
                ' Update the new path to the user in the directory
                _path = result.Path
                _filterAttribute = CStr(result.Properties("cn")(0))
            Catch ex As Exception
                ' Audit logon failure here
                EventLog.WriteEntry(SOURCE_NAME, LocalizationUtils.GetGlobalString("msgLogonFailureEvent") + " " + domainAndUsername, EventLogEntryType.FailureAudit)
                Throw New Exception(LocalizationUtils.GetGlobalString("msgLogonFailure") + " " + ex.Message)
            End Try

            Return True

        End Function 'IsAuthenticated
    End Class 'LdapAuthentication 
End Namespace 'Contoso.Identity.Template