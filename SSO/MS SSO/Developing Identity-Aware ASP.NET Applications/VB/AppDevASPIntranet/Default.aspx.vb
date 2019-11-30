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
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Web
Imports System.Web.SessionState
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Configuration
Imports AppDevASPLib.Contoso.Web.UI.Localization

Namespace Contoso.AppDevASPSample

    ' <summary>
    '   The default page of the intranet application. The page is made localizable by
    '   deriving from LocalizedPage
    ' </summary>
    Public Class _Default
        Inherits LocalizedPage

        Protected EmployeeGrid As System.Web.UI.WebControls.DataGrid
        Protected lblPageHeader As System.Web.UI.WebControls.Label
        Protected lblError As System.Web.UI.WebControls.Label

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.LocalizeControls()

            EmployeeGrid.Columns(0).HeaderText = Me.GetResourceString("strColumnHeaderFirstName")
            EmployeeGrid.Columns(1).HeaderText = Me.GetResourceString("strColumnHeaderLastName")
            EmployeeGrid.Columns(2).HeaderText = Me.GetResourceString("strColumnHeaderTitle")

            'Get and display employee data
            Dim emp As New Employee
            EmployeeGrid.DataSource = emp.GetEmployees()
            EmployeeGrid.DataBind()

            If Not (emp.Error Is Nothing) Then
                If emp.Error.StartsWith("msg") Then
                    lblError.Text = Me.GetResourceString(emp.Error)
                Else
                    lblError.Text = emp.Error
                End If
            End If

        End Sub 'Page_Load 
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
    End Class '_Default 
End Namespace 'Contoso.AppDevASPSample