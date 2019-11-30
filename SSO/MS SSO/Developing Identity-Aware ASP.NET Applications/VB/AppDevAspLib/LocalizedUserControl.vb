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
Imports System.Resources
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace Contoso.Web.UI.Localization

    ' <summary>
    '   Inherit UserControls from this LocalizedUserControl instead of
    '   inheriting directly from System.Web.UI.UserControl. This class
    '   provide localization of user controls using a resource file
    ' </summary>
    Public Class LocalizedUserControl
        Inherits System.Web.UI.UserControl

        Private _resourceManager As ResourceManager = Nothing
        Private _controlName As String = ""

        ' <summary>
        '   Localizes the page by loading the appropriate resource and then walking through all of the controls.
        ' </summary>
        Protected Sub LocalizeControls()

            'Get the calling assembly object
            Dim thisAssembly As System.Reflection.Assembly = System.Reflection.Assembly.GetCallingAssembly()

            'Strip out the control name from the object's type string.
            'I.E.: MyControl.aspx has a type string of "ASP.MyControl_ascx" which gets striped down to "MyControl"
            _controlName = Me.GetType().ToString()
            _controlName = _controlName.Replace("ASP.", Nothing)
            _controlName = _controlName.Replace("_ascx", Nothing)

            Dim baseName As String = ""
            'Build the base name, it's "assemblyName.formName"
            baseName = thisAssembly.GetName().Name + "." + _controlName

            'Now load the appropriate resource
            _resourceManager = New ResourceManager(baseName, thisAssembly)

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
    End Class 'LocalizedUserControl 
End Namespace 'Contoso.Web.UI.Localization