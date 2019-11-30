'##############################################################################
'
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
Imports System.Web
Imports System.Web.Security
Imports System.Security.Principal
Imports System.Runtime.InteropServices
Imports Microsoft.Interop.Security.AzRoles
Imports AppDevASPLib.Contoso.Web.UI.Localization

Namespace Contoso.Identity.Template

    ' <summary>
    '   Helper class for common Authorization Manager (AzMan) functions.
    ' </summary>
    Public Class AzMan

        Public Const AZ_AZSTORE_FLAG_ACCESS_CHECK As Integer = 0 'The authorization store is opened for use by the Update method and the AccessCheck method
        Public Const AZ_AZSTORE_FLAG_CREATE As Integer = 1 'The system attempts to create the policy store specified by the bstrPolicyURL parameter
        Public Const AZ_AZSTORE_FLAG_MANAGE_STORE_ONLY As Integer = 2 'An existing store is opened for management purposes. Run-time routines such as AccessCheck cannot be performed
        Public Const AZ_AZSTORE_FLAG_BATCH_UPDATE As Integer = 4 'The provider is notified that many objects will be modified or created. The provider then optimizes submission of the changes for better performance

        ' <summary>
        '   Prepare AzMan when using trusted subsystem model
        ' </summary>
        Public Sub Initialize(ByVal azPolicyURL As String, ByVal applicationName As String)

            Try
                If IdentityUtil.IsImpersonating() = True Then
                    Return
                End If

                Dim store As New AzAuthorizationStoreClass
                store.Initialize(AZ_AZSTORE_FLAG_ACCESS_CHECK, azPolicyURL, Nothing)

                Dim app As IAzApplication = store.OpenApplication(applicationName, Nothing)
                HttpContext.Current.Application.Add("AzManApp", app)
            Catch ex As Exception
                Throw New Exception(LocalizationUtils.GetGlobalString("msgUnableToInitAzMan"), ex)
            End Try

        End Sub 'Initialize

        ' <summary>
        '   Allow access if using impersonation/delegation model
        '   and check access when using trusted subsystem model
        ' </summary>
        ' <param name="objectName"></param>
        ' <param name="scope"></param>
        ' <param name="operation"></param>
        ' <returns></returns>
        Public Function IsAllowedAccess(ByVal objectName As String, ByVal scope As String, ByVal operation As Int32) As [Boolean]

            If IdentityUtil.IsImpersonating() = True Then
                Return True
            End If

            'Get the application
            Dim app As IAzApplication = CType(HttpContext.Current.Application.Get("AzManApp"), IAzApplication)
            If app Is Nothing Then
                Throw New Exception("Unable to get AzMan application")
            End If

            Dim token As HandleRef

            If TypeOf HttpContext.Current.User.Identity Is FormsIdentity Then
                ' For ASP.NET FormsAuthentication in the Extranet, 
				' we need to do a Windows logon to creat a token
                Dim formsUser As New WindowsIdentity(HttpContext.Current.User.Identity.Name)
                token = New HandleRef(Me, formsUser.Token)
            Else
                ' For Windows Integrated Authentication in the Intranet scenario 
				' as well as the Client Certificate and Passport Authentication 
				' in the Extranet scenario we can ask IIS for the token of the 
				' browser user
                token = New HandleRef(Me, CType(HttpContext.Current.User.Identity, WindowsIdentity).Token)
            End If

            'Create Client Context
            Dim clientContext As IAzClientContext = app.InitializeClientContextFromToken(Convert.ToUInt64(token.Handle.ToInt64), 0)

            Dim scopes() As Object = {scope}
            Dim operations() As Object = {operation}
            Dim results As Object() = CType(clientContext.AccessCheck(objectName, scopes, operations, Nothing, Nothing, Nothing, Nothing, Nothing), Object())

            Dim isAuthorized As Boolean = True
            Dim result As Integer

            For Each result In results
                If result <> 0 Then ' zero = no error
                    isAuthorized = False
                    Exit For
                End If
            Next result

            Return isAuthorized

        End Function 'IsAllowedAccess 
    End Class 'AzMan
End Namespace 'Contoso.Identity.Template
