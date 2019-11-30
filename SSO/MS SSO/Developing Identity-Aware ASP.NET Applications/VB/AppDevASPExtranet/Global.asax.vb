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
Imports System.Collections
Imports System.ComponentModel
Imports System.Web
Imports System.Web.SessionState
Imports System.Web.Security
Imports System.Security.Principal
Imports System.Configuration
Imports System.Runtime.InteropServices
Imports AppDevASPLib.Contoso.Identity.Template

Namespace Contoso.AppDevASPSample

    ' <summary>
    '   Summary description for Global.
    ' </summary>

    Public Class Global
        Inherits System.Web.HttpApplication

        ' <summary>
        ' Required designer variable.
        ' </summary>
        Private components As System.ComponentModel.IContainer = Nothing
        Private wic As WindowsImpersonationContext

        Public Sub New()

            InitializeComponent()

        End Sub 'New

        Protected Sub Application_Start(ByVal sender As [Object], ByVal e As EventArgs)

            Dim authManager As New AzMan
            authManager.Initialize("msxml://C:\AppDevASPExtranet.xml", "AppDevASPExtranet")

        End Sub 'Application_Start

        Protected Sub Session_Start(ByVal sender As [Object], ByVal e As EventArgs)

        End Sub 'Session_Start

        Protected Sub Application_BeginRequest(ByVal sender As [Object], ByVal e As EventArgs)

        End Sub 'Application_BeginRequest

        Protected Sub Application_EndRequest(ByVal sender As [Object], ByVal e As EventArgs)

            If Not (wic Is Nothing) Then
                wic.Undo()
            End If

        End Sub 'Application_EndRequest

        Protected Sub Application_AuthenticateRequest(ByVal sender As [Object], ByVal e As EventArgs)

            Dim formsCookieName As String = FormsAuthentication.FormsCookieName
            Dim formsCookie As HttpCookie = Request.Cookies(formsCookieName)

            If User Is Nothing OrElse User.Identity Is Nothing Then
                Return
            End If

            If TypeOf User.Identity Is FormsIdentity Then

                ' Extract the forms authentication cookie
                If formsCookie Is Nothing Then
                    ' There is no authentication cookie.
                    Return
                End If

                Dim authTicket As FormsAuthenticationTicket = Nothing

                Try
                    authTicket = FormsAuthentication.Decrypt(formsCookie.Value)
                Catch
                    'TODO: Log exception details (omitted for simplicity)
                    Return
                End Try

                If authTicket Is Nothing Then
                    ' Cookie failed to decrypt.
                    Return
                End If

                ' Create an Identity object with a UPN and impersonate
                ' if configured for custom impersonation
                Dim useImpersonationModel As String = ConfigurationSettings.AppSettings("UseImpersonationModel")

                If useImpersonationModel Is Nothing Then
                    Return 'default to not impersonate
                End If

                If Convert.ToBoolean(useImpersonationModel) = True Then
                    Dim wid As New WindowsIdentity(authTicket.Name)
                    wic = wid.Impersonate()
                End If
                ' Now we are impersonating just as if ASP.NET had been set up for
                ' impersonation
            End If

        End Sub 'Application_AuthenticateRequest

        Protected Sub Application_Error(ByVal sender As [Object], ByVal e As EventArgs)

        End Sub 'Application_Error

        Protected Sub Session_End(ByVal sender As [Object], ByVal e As EventArgs)

        End Sub 'Session_End

        Protected Sub Application_End(ByVal sender As [Object], ByVal e As EventArgs)

        End Sub 'Application_End

#Region "Web Form Designer generated code"

        ' <summary>
        '   Required method for Designer support - do not modify
        '   the contents of this method with the code editor.
        ' </summary>
        Private Sub InitializeComponent()

        End Sub 'InitializeComponent

#End Region
    End Class 'Global
End Namespace 'Contoso.AppDevASPSample
