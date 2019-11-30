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
Imports System.Data
Imports System.Drawing
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Configuration
Imports AppDevASPLib.Contoso.Identity.Template
Imports AppDevASPLib.Contoso.Web.UI.Localization

Namespace Contoso.Web.UI.WebControls

    ' <summary>
    '   Summary description for LogonControl.
    ' </summary>
    Public Class LogonControl
        Inherits LocalizedUserControl

        Protected WithEvents btnLogon As System.Web.UI.WebControls.Button
        Protected txtPassword As System.Web.UI.WebControls.TextBox
        Protected txtUsername As System.Web.UI.WebControls.TextBox
        Protected txtDomainName As System.Web.UI.WebControls.TextBox
        Protected lblDomainNameHeader As System.Web.UI.WebControls.Label
        Protected lblUserNameHeader As System.Web.UI.WebControls.Label
        Protected lblPasswordHeader As System.Web.UI.WebControls.Label
        Protected lblDomainNameHelp As System.Web.UI.WebControls.Label
        Protected lblError As System.Web.UI.WebControls.Label

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.LocalizeControls()

        End Sub 'Page_Load


        Private Sub btnLogon_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogon.Click

            ' Path to your LDAP directory server.
            Dim adPath As String = ConfigurationSettings.AppSettings("LDAPBindingString")
            Dim adAuth As New LdapAuthentication(adPath)

            Try

                If True = adAuth.IsAuthenticated(txtDomainName.Text, txtUsername.Text, txtPassword.Text) Then
                    FormsAuthentication.RedirectFromLoginPage(txtUsername.Text + "@" + txtDomainName.Text, False)
                Else
                    lblError.Text = Me.GetResourceString("msgInvalidUPN")
                End If

            Catch ex As Exception
                lblError.Text = ex.Message
            End Try

        End Sub 'btnLogon_Click 

#Region "Web Form Designer generated code"

        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            '
            ' CODEGEN: This call is required by the ASP.NET Web Form Designer.
            '
            InitializeComponent()
            MyBase.OnInit(e)

        End Sub 'OnInit

        ' <summary>
        '	Required method for Designer support - do not modify
        '	the contents of this method with the code editor.
        ' </summary>
        Private Sub InitializeComponent()

        End Sub 'InitializeComponent

#End Region

    End Class 'LogonControl
End Namespace 'Contoso.Web.UI.WebControls
