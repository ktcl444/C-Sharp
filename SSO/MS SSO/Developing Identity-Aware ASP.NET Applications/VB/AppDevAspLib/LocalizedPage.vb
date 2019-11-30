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
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace Contoso.Web.UI.Localization

    ' <summary>
    '   Inherit forms fom this LocalizedPage instead of
    '   inheriting directly from System.Web.UI.Page. This class
    '   provide localization of web forms using a resource file
    ' </summary>
    Public Class LocalizedPage
        Inherits System.Web.UI.Page

        Private _resourceManager As System.Resources.ResourceManager = Nothing
        Private _formName As String = ""

        ' <summary>
        '   Localizes the page by loading the appropriate resource and then walking through all of the controls.
        ' </summary>
        Protected Sub LocalizeControls()

            'Get the calling assembly object
            Dim thisAssembly As System.Reflection.Assembly = System.Reflection.Assembly.GetCallingAssembly()

            'Strip out the form name from the object's type string.
            'I.E.: HomePage.aspx has a type string of "ASP.HomePage_aspx" which gets stripped down to "HomePage"
            _formName = Me.GetType().ToString()
            _formName = _formName.Replace("ASP.", Nothing)
            _formName = _formName.Replace("_aspx", Nothing)

            Dim baseName As String = ""

            'Build the base name, it's "assemblyName.formName"
            baseName = thisAssembly.GetName().Name + "." + _formName

            'Now load the appropriate resource
            _resourceManager = New System.Resources.ResourceManager(baseName, thisAssembly)

            'Do the localization
            Dim localizationUtil As New LocalizationUtils
            localizationUtil.ResourceManager = _resourceManager
            localizationUtil.LocalizeControls(Me.Controls)

        End Sub 'LocalizeControls

        ' <summary>
        '   Get a resource string from the resource manager.
        ' </summary>
        ' <param name="genericControl">The control for which the resource string is being sought.</param>
        ' <param name="attributeSuffix">The attribute suffix for the desired resource.</param>
        ' <returns></returns>
        Public Function GetResourceString(ByVal name As String) As String

            Dim resourceString As String = ""
            Try
                resourceString = _resourceManager.GetString(name)
            Catch
                'String not in resource file (not localizable)
                'TODO: Do Catch Implementation
            End Try

            Return resourceString

        End Function 'GetResourceString

        ' <summary>
        '   Page load event handler.  Starts the localization process for the page.
        ' </summary>
        ' <param name="sender"></param>
        ' <param name="e"></param>
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)

        End Sub 'Page_Load

#Region "Web Form Designer generated code"

        Protected Overrides Sub OnInit(ByVal e As EventArgs)

            ' CODEGEN: This call is required by the ASP.NET Web Form Designer.
            InitializeComponent()
            MyBase.OnInit(e)

            ' Here we grab the browser's requested language and apply it to the current thread
            If Not (Request.UserLanguages Is Nothing) AndAlso Request.UserLanguages.Length > 0 Then
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(Request.UserLanguages(0))
            Else
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en")
            End If

            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture

        End Sub 'OnInit

        ' <summary>
        '   Required method for Designer support - do not modify
        '   the contents of this method with the code editor.
        ' </summary>
        Private Sub InitializeComponent()

        End Sub 'InitializeComponent
#End Region
    End Class 'LocalizedPage
End Namespace 'Contoso.Web.UI.Localization