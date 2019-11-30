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
Imports System.DirectoryServices

Namespace Contoso.Configuration.Install

    ' <summary>
    '   Tools that wrap common IIS configuration code. Used by
    '   KerberosAuthInstaller and FormsAuthInstaller
    ' </summary>
    Public Class IisInstallerUtil

        Private Const MD_SERVER_STATE_STARTED As Int32 = 2

        'Find the server number for the virtual server hosting this VDIR
        Public Shared Function FindServerNum(ByVal port As String) As Int32

            Dim machineName As String = System.Environment.MachineName
            Dim IISObjectPath As String = "IIS://" + machineName + "/W3SVC"
            Dim IISObject As DirectoryEntry
            Dim servers As New ArrayList
            Dim serverNumber As Int32
            Dim nameParts() As String
            Dim name As String
            Dim serverBindings As String
            Dim serverBindingParts() As String
            Dim serverPort As String

            'Iterates through the W3SVC folder to get the name of each child object.
            IISObject = GetIisObject(IISObjectPath)
            Dim IISChildObject As DirectoryEntry

            For Each IISChildObject In IISObject.Children
                nameParts = IISChildObject.Path.Split(New [Char]() {"/"c})
                name = nameParts((nameParts.Length - 1))

                Try
                    'If the name can be converted to an integer (port number),
                    'add it to the Servers collection.
                    serverNumber = Convert.ToInt32(name)
                    servers.Add(serverNumber)
                Catch
                    'If the name cannot be converted to an integer, 
                    'it isn't a port and can be ignored.
                End Try

            Next IISChildObject

            'Iterates through each server, removing each inactive server
            'without the correct port.
            Dim i As Int32

            For i = 0 To servers.Count
                IISObjectPath = "IIS://" + machineName + "/W3SVC/" + servers(i).ToString()
                IISObject = GetIisObject(IISObjectPath)

                'Gets the Port Number of the current IISObject.
                serverBindings = CStr(IISObject.Properties("ServerBindings").Value)
                serverBindingParts = serverBindings.Split(New Char() {":"c})
                serverPort = serverBindingParts(1)

                'Determines if this is our server. IIS can only have one 
                'active port, so if the port is active it is the port where
                'the application is installed.
                If port = serverPort Then
                    If Convert.ToInt32(IISObject.Properties("ServerState").Value) = MD_SERVER_STATE_STARTED Then
                        Return Convert.ToInt32(servers(i))
                    Else
                        'Not the active Port, so remove it from the
                        'collection.
                        servers.RemoveAt(i)
                    End If
                End If
            Next i

            'Checks how many servers are left. If one, we've found it
            'otherwise, report an error.
            Select Case servers.Count
                Case 0
                    Throw New Exception("No Active Servers with the requested port were found. Port=" + port + ". ")
                Case 1
                    Return Convert.ToInt32(servers(0))
                Case Else
                    Throw New Exception("More than one Active servers with the requested port were found. Port=" + port + ". ")
            End Select

        End Function 'FindServerNum

        'Verify if a certain object exists
        Public Shared Function GetIisObject(ByVal strFullObjectPath As String) As DirectoryEntry

            Dim IISObject As DirectoryEntry

            Try
                IISObject = New DirectoryEntry(strFullObjectPath)
                Return IISObject
            Catch ex As Exception
                Throw New Exception("Error opening: " + strFullObjectPath + ". " + ex.Message)
            End Try

        End Function 'GetIisObject
    End Class 'IisInstallerUtil
End Namespace 'Contoso.Configuration.Install