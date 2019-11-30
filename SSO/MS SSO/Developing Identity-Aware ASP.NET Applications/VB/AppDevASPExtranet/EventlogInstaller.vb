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
Imports System.Configuration.Install

Namespace Contoso.AppDevASPSample

    ' <summary>
    '   Creates an Event source when installing and deletes it
    '   on uninstall
    ' </summary>
    <RunInstaller(True)> _
    Public Class EventlogInstaller
        Inherits Installer

        Private Const SOURCE_NAME As String = "AppDevASPExtranet"

        ' <summary>
        ' Overrides Install and creates an event source 
        ' </summary>
        ' <param name="savedState"></param>
        Public Overrides Sub Install(ByVal savedState As IDictionary)

            MyBase.Install(savedState)

            If Not System.Diagnostics.EventLog.SourceExists(SOURCE_NAME) Then
                System.Diagnostics.EventLog.CreateEventSource(SOURCE_NAME, "Application")
            End If

        End Sub 'Install


        ' <summary>
        '   Overrides Uninstall and deletes an event source 
        ' </summary>
        ' <param name="savedState"></param>
        Public Overrides Sub Uninstall(ByVal savedState As IDictionary)

            MyBase.Uninstall(savedState)

            If System.Diagnostics.EventLog.SourceExists(SOURCE_NAME) Then
                System.Diagnostics.EventLog.DeleteEventSource(SOURCE_NAME, "Application")
            End If

        End Sub 'Uninstall
    End Class 'EventlogInstaller
End Namespace 'Contoso.AppDevASPSample