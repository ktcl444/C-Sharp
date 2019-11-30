Imports Microsoft.VisualBasic
Imports System
Imports Microsoft.Win32
Imports System.Diagnostics
Imports System.DirectoryServices

Namespace MySoft.IISManage

    ''' <summary>
    ''' IISWebService�Ŀ����ࡣ
    ''' WWW�������һ��IISWebService��������������WebServer����վ����
    ''' </summary>
    Public Class IISWebService

#Region "��̬�ķ���"

        ''' <summary>
        ''' ��ȡ������IIS�汾
        ''' </summary>
        ''' <param name="machine"></param>
        ''' <returns></returns>
        Public Shared Function GetIISVersion(ByVal machine As String) As IISVersionEnum
            Dim path As String = "IIS://" & machine.ToUpper() & "/W3SVC/INFO"
            Dim entry As DirectoryEntry = Nothing
            Try
                entry = New DirectoryEntry(path)
            Catch
                Return IISVersionEnum.Unknown
            End Try
            Dim num As Integer = 5
            Try
                num = CInt(Fix(entry.Properties("MajorIISVersionNumber").Value))
            Catch
                num = GetIISVersion()
                'return IISVersionEnum.IIS5;
            End Try
            Select Case num
                Case 6
                    Return IISVersionEnum.IIS6

                Case 7
                    Return IISVersionEnum.IIS7
            End Select
            Return IISVersionEnum.IIS6
        End Function
        Private Shared Function GetIISVersion() As Integer
            Dim pregkey As RegistryKey

            pregkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\InetStp", True)

            Return Convert.ToInt16(pregkey.GetValue("MajorVersion"))
        End Function
        ''' <summary>
        ''' �ж�һ��ָ��·���Ƿ����
        ''' ������DirectoryEntry.Exists(_adsPath)��������ڷ����쳣��������false��
        ''' ����ֻ���Լ�д���������жϡ���
        ''' </summary>
        ''' <param name="adsPath"></param>
        ''' <returns></returns>
        Public Shared Function IsPathExists(ByVal adsPath As String) As Boolean
            Dim de As New DirectoryEntry(adsPath)
            Try
                Dim name As String = de.Name ' if this succeeds return true
                Return True
            Catch
                Return False
            End Try
        End Function
        ''' <summary> 
        ''' ��ȡ��վ�ı�ʶ�� 
        ''' </summary> 
        ''' <param name="portNumber">�˿ں�</param> 
        ''' <returns></returns> 
        Public Shared Function GetIdentifier(ByVal machine As String, ByVal portNumber As String) As String
            Dim iisservice As New DirectoryEntry(String.Format("IIS://{0}/W3SVC", machine))
            For Each e As DirectoryEntry In iisservice.Children
                If e.SchemaClassName = "IIsWebServer" Then
                    For Each [property] As Object In e.Properties("ServerBindings")
                        If [property].Equals(":" & portNumber & ":") Then
                            Return e.Name
                        End If
                    Next [property]
                End If
            Next e

            Return ""
        End Function
        'ע��ASP.NET Framework
        Public Shared Sub RegisterAspNet(ByVal vDirPath As String, ByVal aspNetVersion As String)
            If (Not String.IsNullOrEmpty(aspNetVersion)) Then
                Dim fileName As String = Environment.GetEnvironmentVariable("windir") & "\Microsoft.NET\Framework\" & aspNetVersion & "\aspnet_regiis.exe"
                Dim startInfo As New ProcessStartInfo(fileName)
                '����Ŀ¼·��
                Dim path As String = vDirPath.ToUpper()
                Dim index As Integer = path.IndexOf("W3SVC")
                path = path.Remove(0, index)
                '����aspnet_iis.exe����,ˢ�½̱�ӳ��
                startInfo.Arguments = "-s " & path
                startInfo.WindowStyle = ProcessWindowStyle.Hidden
                startInfo.UseShellExecute = False
                startInfo.CreateNoWindow = True
                startInfo.RedirectStandardOutput = True
                startInfo.RedirectStandardError = True
                Dim process As New Process()
                process.StartInfo = startInfo
                process.Start()
                process.WaitForExit()
                Dim errors As String = process.StandardError.ReadToEnd()
                If errors <> String.Empty Then
                    Throw New Exception(errors)
                End If
            End If
        End Sub

#End Region

#Region "˽�е�ʵ���ֶ�"
        Private _machine, _adsPath As String
        Private _svcEntry As DirectoryEntry
#End Region

#Region "ʵ�����캯��"

        ''' <summary>
        ''' ���캯��
        ''' </summary>
        ''' <param name="machine">IISWebService���ڷ������ļ����������IP����������ʹ��"localhost"</param>
        ''' <returns></returns>
        Public Sub New(ByVal machine As String)
            If machine.ToString() = "" Then
                _machine = "localhost"
            Else
                _machine = machine
            End If

            Init()
        End Sub
#End Region

#Region "������ʵ������"

        ''' <summary>
        ''' �����������IP�������������ʹ��localhost��
        ''' </summary>
        Public ReadOnly Property Machine() As String
            Get
                Return _machine
            End Get
        End Property

        ''' <summary>
        ''' ��ǰIISWebService��DirectoryEntry����
        ''' </summary>
        Public ReadOnly Property DirectoryEntry() As DirectoryEntry
            Get
                Return _svcEntry
            End Get
        End Property

        ''' <summary>
        ''' �Ӽ�IISWebServer����
        ''' </summary>
        Public ReadOnly Property WebServers() As IISWebServerCollection
            Get
                Return GetWebServers()
            End Get
        End Property

#End Region

#Region "˽�еķ���"

        ''' <summary>
        ''' ��ʼ��
        ''' </summary>
        Private Sub Init()
            _adsPath = String.Format("IIS://{0}/W3SVC", _machine)
            _svcEntry = New DirectoryEntry(_adsPath)
        End Sub


        ''' <summary>
        ''' ��ȡ�Ӽ�IISWebServer����
        ''' </summary>
        ''' <returns></returns>
        Private Function GetWebServers() As IISWebServerCollection
            Dim webServers As New IISWebServerCollection()
            Dim webServer As IISWebServer

            For Each de As DirectoryEntry In _svcEntry.Children
                If de.SchemaClassName = "IIsWebServer" Then
                    webServer = New IISWebServer(_machine, de.Properties("ServerBindings")(0).ToString().Replace(":", ""), "")
                    webServers.Add(webServer)
                End If
            Next de

            Return webServers
        End Function

#End Region

    End Class

End Namespace
