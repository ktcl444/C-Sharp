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
Imports System.Xml
Imports System.Data
Imports System.Drawing
Imports System.Configuration
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports AppDevASPLib.Contoso.Identity.Template
Imports AppDevASPLib.Contoso.Web.UI.Localization

Namespace Contoso.Web.UI.WebControls

    ' <summary>
    '	Summary description for IdentityViewerControl.
    ' </summary>
    Public Class IdentityViewerControl
        Inherits LocalizedUserControl

        Protected lblIdentityModelHeader As System.Web.UI.WebControls.Label
        Protected lblIdentityModel As System.Web.UI.WebControls.Label
        Protected lblChangeIdentityModel As System.Web.UI.WebControls.Label
        Protected lblAuthNModeHeader As System.Web.UI.WebControls.Label
        Protected lblAuthNMode As System.Web.UI.WebControls.Label
        Protected lblHttpContextHeader As System.Web.UI.WebControls.Label
        Protected lblHttpContext As System.Web.UI.WebControls.Label
        Protected lblWindowsIdentityHeader As System.Web.UI.WebControls.Label
        Protected lblWindowsIdentity As System.Web.UI.WebControls.Label
        Protected lblThreadHeader As System.Web.UI.WebControls.Label
        Protected lblThread As System.Web.UI.WebControls.Label

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.LocalizeControls()
            ShowIdentityInfo(lblIdentityModel, lblAuthNMode, lblThread, lblWindowsIdentity, lblHttpContext)

        End Sub 'Page_Load

        Private Sub ShowIdentityInfo(ByVal identityModel As Label, ByVal authnMode As Label, ByVal threadUser As Label, ByVal windowsIdentityUser As Label, ByVal httpContextUser As Label)

            Dim changeIdentityModel As String

            authnMode.Text = System.Threading.Thread.CurrentPrincipal.Identity.AuthenticationType
            threadUser.Text = System.Threading.Thread.CurrentPrincipal.Identity.Name
            windowsIdentityUser.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name
            httpContextUser.Text = HttpContext.Current.User.Identity.Name
            identityModel.ForeColor = System.Drawing.Color.Blue

            If IdentityUtil.IsImpersonating() Then
                identityModel.Text = Me.GetResourceString("strImpersonateDelegate")
                If IdentityUtil.AuthenticationMode() = "Forms" Then
                    changeIdentityModel = Me.GetResourceString("strFormsChangeToTrusted")
                Else
                    changeIdentityModel = Me.GetResourceString("strFormsChangeToImpersonateDelegate")
                End If
            Else
                identityModel.Text = Me.GetResourceString("strTrustedSubsystem")
                If IdentityUtil.AuthenticationMode() = "Forms" Then
                    changeIdentityModel = Me.GetResourceString("strFormsChangeToImpersonateDelegate")
                Else
                    changeIdentityModel = Me.GetResourceString("strNonFormsChangeToImpersonateDelegate")
                End If
            End If

            changeIdentityModel = changeIdentityModel.Replace("[PhysicalApplicationPath]", HttpContext.Current.Request.PhysicalApplicationPath)
            lblChangeIdentityModel.Text = changeIdentityModel

        End Sub 'ShowIdentityInfo 


#Region "Web Form Designer generated code"

        Protected Overrides Sub OnInit(ByVal e As EventArgs)

            ' CODEGEN: This call is required by the ASP.NET Web Form Designer.
            InitializeComponent()
            MyBase.OnInit(e)

        End Sub 'OnInit


        ' <summary>
        '   Required method for Designer support - do not modify
        '   the contents of this method with the code editor.
        ' </summary>
        Private Sub InitializeComponent()

        End Sub 'InitializeComponent

#End Region
    End Class 'IdentityViewerControl
End Namespace 'Contoso.Web.UI.WebControls
