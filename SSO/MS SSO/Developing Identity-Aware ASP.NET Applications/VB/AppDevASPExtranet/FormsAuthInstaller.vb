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
Imports System.DirectoryServices
Imports AppDevASPLib.Contoso.Configuration.Install

Namespace Contoso.AppDevASPSample

    ' <summary>
    ' This installer makes sure anonymous access is enabled
    ' for this VDIR.
    ' </summary>
    <RunInstaller(True)> _
    Public Class FormsAuthInstaller
        Inherits Installer

        Private Const MD_AUTH_ANONYMOUS As Int32 = 1

        Public Overrides Sub Install(ByVal savedState As IDictionary)

            MyBase.Install(savedState)

            'Enable anonymous and let ASP.NET do authN
            SetFormsAuthN()

        End Sub 'Install

        'Sets the anonymous bit in IIS metabase for the VDIR
        Private Sub SetFormsAuthN()
            Dim strVDir As String = Nothing
            Dim intServerNum As Int32 = 1

            'Is being run from installUtil, not setup
            If Me.Context.Parameters("VDir") Is Nothing Then
                Return
            End If

            Try
                strVDir = Me.Context.Parameters("VDir")
                intServerNum = IisInstallerUtil.FindServerNum(Me.Context.Parameters("Port"))
            Catch
                Throw New Exception("Unable to read VDir or Port param")
            End Try

            Dim strObjectPath As String = "IIS://" + System.Environment.MachineName + "/W3SVC/" + intServerNum.ToString() + "/ROOT/" + strVDir

            Dim IISVdir As DirectoryEntry = IisInstallerUtil.GetIisObject(strObjectPath)

            Try
                'Change the MD_AUTH_ANONYMOUS bit
                If Convert.ToBoolean(IISVdir.Properties("AuthAnonymous").Value) <> True Then
                    IISVdir.Properties("AuthFlags").Value = Convert.ToInt32(IISVdir.Properties("AuthFlags").Value) Xor MD_AUTH_ANONYMOUS
                    IISVdir.Invoke("SetInfo", Nothing)
                End If
            Catch
                Throw New Exception("Unable to enable/disable anonymous access")
            End Try

        End Sub 'SetFormsAuthN
    End Class 'FormsAuthInstaller 
End Namespace 'Contoso.AppDevASPSample