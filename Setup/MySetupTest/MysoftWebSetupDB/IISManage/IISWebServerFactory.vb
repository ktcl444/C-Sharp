Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace MySoft.IISManage
    Public Class IISWebServerFactory
        Public Shared Function GetIISWebServer(ByVal machineName As String, ByVal identigier As String, ByVal applicationName As String) As IIISWebServer
            Dim iisVersion As IISVersionEnum = IISWebService.GetIISVersion(machineName)
            If iisVersion = IISVersionEnum.IIS6 Then
                Return New IISWebServer(machineName, identigier, applicationName)
            ElseIf iisVersion = IISVersionEnum.IIS7 Then
                Return New IIS7WebServer(identigier, applicationName)
            End If
            Return Nothing
        End Function
    End Class
End Namespace
