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
Imports System.Reflection

Namespace Contoso.Web.UI.Localization

    ' <summary>
    '   Helper class to recursively localize controls. Used by classes
    '   LocalizedPage and LocalizedUserControl to localize all controls
    '   on a page or a user control
    ' </summary>
    Public Class LocalizationUtils

        Private _resourceManager As ResourceManager = Nothing

        Public Property ResourceManager() As ResourceManager
            Get
                Return _resourceManager
            End Get
            Set(ByVal Value As ResourceManager)
                _resourceManager = Value
            End Set
        End Property

        ' <summary>
        '   Get a resource string for a control property from the 
        '   resource manager.
        ' </summary>
        ' <param name="genericControl">The control for which the resource string is being sought.</param>
        ' <param name="attributeSuffix">The attribute suffix for the desired resource.</param>
        ' <returns></returns>
        Private Function GetResourceString(ByVal genericControl As System.Web.UI.Control, ByVal attributeSuffix As String) As String

            Dim theObject As Object = Nothing

            Try
                theObject = _resourceManager.GetObject(genericControl.ID + attributeSuffix)
            Catch
                'TODO: Do Catch Implementation
            End Try

            Return CStr(theObject)

        End Function 'GetResourceString

        ' <summary>
        '   Get an assembly global resource string from an a assembly
        '   global resource manager.
        ' </summary>
        ' <param name="messageId"></param>
        ' <returns></returns>
        Public Shared Function GetGlobalString(ByVal messageId As String) As String

            Dim globalResourceManager As ResourceManager = Nothing

            'Grab the browser's requested language and apply it to the current
            'thread or use "en" if not specified
            If Not (HttpContext.Current.Request.UserLanguages Is Nothing) AndAlso HttpContext.Current.Request.UserLanguages.Length > 0 Then
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(HttpContext.Current.Request.UserLanguages(0))
            Else
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en")
            End If

            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture
            Dim thisAssembly As [Assembly] = [Assembly].GetCallingAssembly()
            globalResourceManager = New ResourceManager(thisAssembly.GetName().Name + "." + thisAssembly.GetName().Name, thisAssembly)
            Dim theObject As Object = Nothing

            Try
                theObject = globalResourceManager.GetObject(messageId)
            Catch
                Return "Can't find the localized message for ID '" + messageId + "'"
            End Try

            Return CStr(theObject)

        End Function 'GetGlobalString

        ' <summary>
        '   Localizes the controls.  Recursively calls itself if a control is found that has child controls.
        ' </summary>
        ' <param name="controls">The collection of controls that will be localized.</param>
        Public Sub LocalizeControls(ByVal controls As System.Web.UI.ControlCollection)

            Dim i As Integer = 0
            Dim genericControl As System.Web.UI.Control

            For Each genericControl In controls

                If genericControl.HasControls() = True Then
                    LocalizeControls(genericControl.Controls)
                End If

                If TypeOf genericControl Is System.Web.UI.WebControls.AdRotator Then

                    Dim theControl As System.Web.UI.WebControls.AdRotator = CType(genericControl, System.Web.UI.WebControls.AdRotator)
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")
                    theControl.AdvertisementFile = GetResourceString(genericControl, ".AdvertisementFile")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.Button Then

                    Dim theControl As System.Web.UI.WebControls.Button = CType(genericControl, System.Web.UI.WebControls.Button)
                    theControl.Text = GetResourceString(genericControl, ".Text")
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.Calendar Then

                    Dim theControl As System.Web.UI.WebControls.Calendar = CType(genericControl, System.Web.UI.WebControls.Calendar)
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.CheckBox Then

                    Dim theControl As System.Web.UI.WebControls.CheckBox = CType(genericControl, System.Web.UI.WebControls.CheckBox)
                    theControl.Text = GetResourceString(genericControl, ".Text")
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.CheckBoxList Then

                    Dim theControl As System.Web.UI.WebControls.CheckBoxList = CType(genericControl, System.Web.UI.WebControls.CheckBoxList)
                    i = 0
                    Dim li As System.Web.UI.WebControls.ListItem

                    For Each li In theControl.Items
                        li.Text = GetResourceString(genericControl, "." + i.ToString() + ".Text")
                        i += 1
                    Next li

                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.DataGrid Then

                    Dim theControl As System.Web.UI.WebControls.DataGrid = CType(genericControl, System.Web.UI.WebControls.DataGrid)
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")
                    Dim dgCol As System.Web.UI.WebControls.DataGridColumn

                    For Each dgCol In theControl.Columns
                        Dim bCol As System.Web.UI.WebControls.BoundColumn = CType(dgCol, System.Web.UI.WebControls.BoundColumn)
                        dgCol.HeaderText = GetResourceString(genericControl, "." + bCol.DataField + ".HeaderText")
                        dgCol.FooterText = GetResourceString(genericControl, "." + bCol.DataField + ".FooterText")
                    Next dgCol

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.DropDownList Then

                    Dim theControl As System.Web.UI.WebControls.DropDownList = CType(genericControl, System.Web.UI.WebControls.DropDownList)
                    i = 0
                    Dim li As System.Web.UI.WebControls.ListItem

                    For Each li In theControl.Items
                        li.Text = GetResourceString(genericControl, "." + i.ToString() + ".Text")
                        i += 1
                    Next li

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.Image Then

                    Dim theControl As System.Web.UI.WebControls.Image = CType(genericControl, System.Web.UI.WebControls.Image)
                    theControl.AlternateText = GetResourceString(genericControl, ".AlternateText")
                    theControl.ImageUrl = GetResourceString(genericControl, ".ImageUrl")
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.ImageButton Then

                    Dim theControl As System.Web.UI.WebControls.ImageButton = CType(genericControl, System.Web.UI.WebControls.ImageButton)
                    theControl.AlternateText = GetResourceString(genericControl, ".AlternateText")
                    theControl.ImageUrl = GetResourceString(genericControl, ".ImageUrl")
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.HyperLink Then

                    Dim theControl As System.Web.UI.WebControls.HyperLink = CType(genericControl, System.Web.UI.WebControls.HyperLink)
                    theControl.Text = GetResourceString(genericControl, ".Text")
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.Label Then

                    Dim theControl As System.Web.UI.WebControls.Label = CType(genericControl, System.Web.UI.WebControls.Label)
                    theControl.Text = GetResourceString(genericControl, ".Text")
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.ListBox Then

                    Dim theControl As System.Web.UI.WebControls.ListBox = CType(genericControl, System.Web.UI.WebControls.ListBox)
                    i = 0
                    Dim li As System.Web.UI.WebControls.ListItem

                    For Each li In theControl.Items
                        li.Text = GetResourceString(genericControl, "." + i.ToString() + ".Text")
                        i += 1
                    Next li

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.Literal Then

                    Dim theControl As System.Web.UI.WebControls.Literal = CType(genericControl, System.Web.UI.WebControls.Literal)
                    theControl.Text = GetResourceString(genericControl, ".Text")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.LinkButton Then

                    Dim theControl As System.Web.UI.WebControls.LinkButton = CType(genericControl, System.Web.UI.WebControls.LinkButton)
                    theControl.Text = GetResourceString(genericControl, ".Text")
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.RadioButton Then
                    Dim theControl As System.Web.UI.WebControls.RadioButton = CType(genericControl, System.Web.UI.WebControls.RadioButton)
                    theControl.Text = GetResourceString(genericControl, ".Text")
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")
                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.RadioButtonList Then

                    Dim theControl As System.Web.UI.WebControls.RadioButtonList = CType(genericControl, System.Web.UI.WebControls.RadioButtonList)
                    i = 0
                    Dim li As System.Web.UI.WebControls.ListItem

                    For Each li In theControl.Items
                        li.Text = GetResourceString(genericControl, "." + i.ToString() + ".Text")
                        i += 1
                    Next li

                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")

                ElseIf TypeOf genericControl Is System.Web.UI.WebControls.TextBox Then

                    Dim theControl As System.Web.UI.WebControls.TextBox = CType(genericControl, System.Web.UI.WebControls.TextBox)
                    theControl.ToolTip = GetResourceString(genericControl, ".ToolTip")

                End If

            Next genericControl

        End Sub 'LocalizeControls
    End Class 'LocalizationUtils 
End Namespace 'Contoso.Web.UI.Localization