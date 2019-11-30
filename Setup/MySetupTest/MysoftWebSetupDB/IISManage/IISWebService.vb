Imports Microsoft.VisualBasic
Imports System
Imports Microsoft.Win32
Imports System.Diagnostics
Imports System.DirectoryServices

Namespace MySoft.IISManage

    ''' <summary>
    ''' IISWebService的控制类。
    ''' WWW服务就是一个IISWebService，它下面包含多个WebServer（网站）。
    ''' </summary>
    Public Class IISWebService

#Region "静态的方法"

        ''' <summary>
        ''' 获取服务器IIS版本
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
        ''' 判断一个指定路径是否存在
        ''' （由于DirectoryEntry.Exists(_adsPath)如果不存在返回异常，而不是false。
        ''' 所以只能自己写个函数来判断。）
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
        ''' 获取网站的标识符 
        ''' </summary> 
        ''' <param name="portNumber">端口号</param> 
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
        '注册ASP.NET Framework
        Public Shared Sub RegisterAspNet(ByVal vDirPath As String, ByVal aspNetVersion As String)
            If (Not String.IsNullOrEmpty(aspNetVersion)) Then
                Dim fileName As String = Environment.GetEnvironmentVariable("windir") & "\Microsoft.NET\Framework\" & aspNetVersion & "\aspnet_regiis.exe"
                Dim startInfo As New ProcessStartInfo(fileName)
                '处理目录路径
                Dim path As String = vDirPath.ToUpper()
                Dim index As Integer = path.IndexOf("W3SVC")
                path = path.Remove(0, index)
                '启动aspnet_iis.exe程序,刷新教本映射
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

#Region "私有的实例字段"
        Private _machine, _adsPath As String
        Private _svcEntry As DirectoryEntry
#End Region

#Region "实例构造函数"

        ''' <summary>
        ''' 构造函数
        ''' </summary>
        ''' <param name="machine">IISWebService所在服务器的计算机名或者IP。本机可以使用"localhost"</param>
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

#Region "公共的实例属性"

        ''' <summary>
        ''' 计算机名或者IP。如果本机可以使用localhost。
        ''' </summary>
        Public ReadOnly Property Machine() As String
            Get
                Return _machine
            End Get
        End Property

        ''' <summary>
        ''' 当前IISWebService的DirectoryEntry对象。
        ''' </summary>
        Public ReadOnly Property DirectoryEntry() As DirectoryEntry
            Get
                Return _svcEntry
            End Get
        End Property

        ''' <summary>
        ''' 子集IISWebServer集合
        ''' </summary>
        Public ReadOnly Property WebServers() As IISWebServerCollection
            Get
                Return GetWebServers()
            End Get
        End Property

#End Region

#Region "私有的方法"

        ''' <summary>
        ''' 初始化
        ''' </summary>
        Private Sub Init()
            _adsPath = String.Format("IIS://{0}/W3SVC", _machine)
            _svcEntry = New DirectoryEntry(_adsPath)
        End Sub


        ''' <summary>
        ''' 获取子集IISWebServer集合
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
