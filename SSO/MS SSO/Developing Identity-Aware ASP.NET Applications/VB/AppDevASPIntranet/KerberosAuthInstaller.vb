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
Imports System.Reflection
Imports System.Collections
Imports System.ComponentModel
Imports System.Configuration.Install
Imports System.DirectoryServices
Imports AppDevASPLib.Contoso.Configuration.Install

Namespace Contoso.AppDevASPSample

    ' <summary>
    '   This installer makes sure anonymous access is disabled
    '   for this VDIR.
    ' </summary>
    <RunInstaller(True)> _
    Public Class KerberosInstaller
        Inherits Installer

        Private Const MD_AUTH_ANONYMOUS As Int32 = 1
        Private Const MD_AUTH_NT As Int32 = 4

        Public Overrides Sub Install(ByVal savedState As IDictionary)

            MyBase.Install(savedState)

            'Disable anonymous and let IIS do Kerberos authN
            SetSPNEGOAuthN()

        End Sub 'Install

        'Clears the anonymous bit and sets the SPNEGO bit in IIS
        'metabase for the VDIR
        Private Sub SetSPNEGOAuthN()

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
                'Change the MD_AUTH_ANONYMOUS
                If Convert.ToBoolean(IISVdir.Properties("AuthAnonymous").Value) <> False Then
                    IISVdir.Properties("AuthFlags").Value = Convert.ToInt32(IISVdir.Properties("AuthFlags").Value) Xor MD_AUTH_ANONYMOUS
                End If

                'Make sure IIS Integrated is checked when disabling anonymous
                IISVdir.Properties("AuthFlags").Value = Convert.ToInt32(IISVdir.Properties("AuthFlags").Value) Or MD_AUTH_NT
                IISVdir.Invoke("SetInfo", Nothing)
            Catch
                Throw New Exception("Unable to enable/disable anonymous access")
            End Try

        End Sub 'SetSPNEGOAuthN
    End Class 'KerberosInstaller 
End Namespace 'Contoso.AppDevASPSample