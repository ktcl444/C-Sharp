'##############################################################################
' Microsoft Solutions for Security
' Copyright (c) 2004 Microsoft Corporation. All rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
' THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
' AND INFORMATION REMAINS WITH THE USER.
'#############################################################################

Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Web
Imports System.Web.SessionState
Imports AppDevASPLib.Contoso.Identity.Template

Namespace Contoso.AppDevASPSample

    ' <summary>
    '   Summary description for Global.
    ' </summary>
    Public Class Global
        Inherits System.Web.HttpApplication

        ' <summary>
        '   Required designer variable.
        ' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        Public Sub New()

            InitializeComponent()

        End Sub 'New

        Protected Sub Application_Start(ByVal sender As [Object], ByVal e As EventArgs)

            Dim authManager As New AzMan
            authManager.Initialize("msxml://C:\AppDevASPIntranet.xml", "AppDevASPIntranet")

        End Sub 'Application_Start

        Protected Sub Session_Start(ByVal sender As [Object], ByVal e As EventArgs)

        End Sub 'Session_Start

        Protected Sub Application_BeginRequest(ByVal sender As [Object], ByVal e As EventArgs)

        End Sub 'Application_BeginRequest

        Protected Sub Application_EndRequest(ByVal sender As [Object], ByVal e As EventArgs)

        End Sub 'Application_EndRequest

        Protected Sub Application_AuthenticateRequest(ByVal sender As [Object], ByVal e As EventArgs)

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
