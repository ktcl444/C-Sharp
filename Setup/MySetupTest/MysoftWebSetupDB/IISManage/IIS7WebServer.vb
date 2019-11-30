Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Microsoft.Web.Administration
Imports System.DirectoryServices
Imports System.Diagnostics

Namespace MySoft.IISManage
    Friend Class IIS7WebServer
        Implements IIISWebServer

#Region "���캯��"

        Public Sub New(ByVal identifier As String, ByVal appFriendlyName As String)
            _iisManager = New ServerManager()
            CreateAppPool(AppPoolId)
            Me.AppFriendlyName = appFriendlyName
            For Each site As Site In _iisManager.Sites
                If site.Id = Convert.ToInt32(identifier) Then
                    _site = site
                    Exit For
                End If
            Next site
        End Sub

#End Region

#Region "˽������"
        Private _machine, _port, _siteName, _path, _appFriendlyName As String
        Private _appIsolated As Integer
        Private _site As Site
        Private _iisManager As ServerManager
        Private _appPool As ApplicationPool
        Private _appPoolId As String = "Mysoft ASP.NET 2.0"
#End Region

#Region "��������"
        ''' <summary>
        ''' �����������IP�������������ʹ��localhost��
        ''' </summary>
        Public ReadOnly Property Machine() As String
            Get
                Return _machine
            End Get
        End Property


        ''' <summary>
        ''' ��վ����
        ''' </summary>
        Public Property SiteName() As String
            Get
                Return _siteName
            End Get
            Set(ByVal value As String)
                _siteName = value
            End Set
        End Property

        ''' <summary>
        ''' �˿ں�
        ''' </summary>
        Public Property Port() As String
            Get
                Return _port
            End Get
            Set(ByVal value As String)
                _port = value
            End Set
        End Property

        ''' <summary>
        ''' ����·��
        ''' </summary>
        Public Property Path() As String
            Get
                Return _path
            End Get
            Set(ByVal value As String)
                _path = value
            End Set
        End Property

        ''' <summary>
        ''' Ӧ�ó����Id
        ''' </summary>
        Public Property AppPoolId() As String
            Get
                Return _appPoolId
            End Get
            Set(ByVal value As String)
                _appPoolId = value
            End Set
        End Property

        ''' <summary>
        ''' Web Ӧ�ó�������
        ''' </summary>
        Public Property AppFriendlyName() As String
            Get
                Return _appFriendlyName
            End Get
            Set(ByVal value As String)
                _appFriendlyName = value
            End Set
        End Property

        ''' <summary>
        ''' Web Ӧ�ó���Ľ�������
        ''' IIS5�����ϰ汾��ʹ��AppCreate2()���� ���������� (0)������������ (1)�����̳���(2)	
        ''' IIS5���°汾��ʹ��AppCreate()���������� (1) �������� (0)	
        ''' </summary>
        Public Property AppIsolated() As Integer
            Get
                Return _appIsolated
            End Get
            Set(ByVal value As Integer)
                _appIsolated = value
            End Set
        End Property
#End Region

#Region "��������"

#Region "IIISWebServer ��Ա"
        Public Sub SetSiteProperties() Implements IIISWebServer.SetSiteProperties
            For Each app As Application In _site.Applications
                app.ApplicationPoolName = _appPoolId
            Next app
            SetMime(_site.Name)
            _iisManager.CommitChanges()
        End Sub

        ''' <summary>
        ''' �½���վ
        ''' </summary>
        Public Function Create() As Boolean Implements IIISWebServer.Create
            If _port.ToString() = "" OrElse _siteName.ToString() = "" OrElse _path.ToString() = "" Then
                Throw (New Exception("��վ���ơ��˿ںš�����·��������Ϊ�գ�"))
            End If

            CreateAppPool(_appPoolId)
            CreateSite(_siteName, _path, Convert.ToInt32(_port))
            SetMime(_siteName)
            Return True
        End Function

        Public Sub Delete() Implements IIISWebServer.Delete
            If _site IsNot Nothing AndAlso _iisManager.Sites.Contains(_site) Then
                _iisManager.Sites.Remove(_site)
            End If
        End Sub

        Public Function Update() As Boolean Implements IIISWebServer.Update
            _iisManager.CommitChanges()
            Return True
        End Function

        Public Sub Start() Implements IIISWebServer.Start
            If _site IsNot Nothing AndAlso _site.State <> ObjectState.Started Then
                _site.Start()
            End If
        End Sub

        Public Sub Pause() Implements IIISWebServer.Pause
            Throw New NotImplementedException()
        End Sub

        Public Sub [Stop]() Implements IIISWebServer.Stop
            _site.Stop()
        End Sub

        Public Sub [Continue]() Implements IIISWebServer.Continue
            Throw New NotImplementedException()
        End Sub

        Public Sub Dispose() Implements IIISWebServer.Dispose
            _iisManager.Dispose()
        End Sub

        Public Function GetStatus() As IISServerState Implements IIISWebServer.GetStatus
            Dim siteState As ObjectState
            siteState = _site.State

            Dim e As IISServerState
            Select Case siteState
                Case ObjectState.Started
                    e = IISServerState.Started
                Case ObjectState.Starting
                    e = IISServerState.Starting
                Case ObjectState.Stopped
                    e = IISServerState.Stopped
                Case ObjectState.Stopping
                    e = IISServerState.Stopping
                Case Else
                    e = IISServerState.Unkonwn
            End Select
            Return e

        End Function

        Public Sub Init(ByVal strMachine As String, ByVal strWebName As String, ByVal strWebPort As String, ByVal strWebDir As String, ByVal strAppFriendlyName As String, ByVal appIsolated As Integer, ByVal strAppPool As String) Implements IIISWebServer.Init
            Me._port = (If(String.IsNullOrEmpty(strWebPort), "", strWebPort))
            Me._machine = (If(String.IsNullOrEmpty(strMachine), "", strMachine))
            Me._appPoolId = (If(String.IsNullOrEmpty(strAppPool), "DefaultAppPool", strAppPool))
            Me._appFriendlyName = (If(String.IsNullOrEmpty(strAppFriendlyName), "Ĭ��Ӧ�ó���", strAppFriendlyName))
            Me._appIsolated = appIsolated
            Me._path = (If(String.IsNullOrEmpty(strWebDir), "", strWebDir))
            Me._siteName = (If(String.IsNullOrEmpty(strWebName), "", strWebName))
            '_iisManager = New ServerManager()
        End Sub

        Public Function CheckPortRepeated() As Boolean Implements IIISWebServer.CheckPortRepeated
            For Each site As Site In _iisManager.Sites
                For Each bd As Binding In site.Bindings
                    If bd.EndPoint.Port = Convert.ToInt32(_port) Then
                        Return True
                    End If
                Next bd
            Next site
            Return False
        End Function
#End Region

#End Region

#Region "˽�з���"
        ''' <summary>
        ''' ������վ
        ''' </summary>
        ''' <param name="_siteName">����</param>
        ''' <param name="_path">����·��</param>
        ''' <param name="_port">�˿ں�</param>
        Private Sub CreateSite(ByVal _siteName As String, ByVal _path As String, ByVal _port As Integer)
            For Each site As Site In _iisManager.Sites
                If site.Name = _siteName Then
                    Throw (New Exception(String.Format("����Ϊ{0}����վ�Ѿ����ڣ�����ʧ�ܣ�", _siteName)))
                End If
                For Each bd As Binding In site.Bindings
                    If bd.EndPoint.Port = _port Then
                        site.Stop()
                        Exit For
                    End If
                Next bd
            Next site

            _site = _iisManager.Sites.Add(_siteName, _path, _port)
            _site.ApplicationDefaults.ApplicationPoolName = _appPoolId
            _iisManager.CommitChanges()
        End Sub

        ''' <summary>
        ''' ����Ӧ�ó����
        ''' </summary>
        ''' <param name="strAppPoolName">����</param>
        Private Sub CreateAppPool(ByVal strAppPoolName As String)
            Dim blnExistAppPool As Boolean = False
            For Each appPool As ApplicationPool In _iisManager.ApplicationPools
                If appPool.Name = strAppPoolName Then
                    blnExistAppPool = True
                    _appPool = appPool
                    Exit For
                End If
            Next appPool
            If (Not blnExistAppPool) Then
                _appPool = _iisManager.ApplicationPools.Add(strAppPoolName)
            End If
            _appPool.ManagedRuntimeVersion = "v2.0"
            _appPool.ManagedPipelineMode = ManagedPipelineMode.Classic
            _appPool.ProcessModel.IdentityType = ProcessModelIdentityType.NetworkService
            _iisManager.CommitChanges()
        End Sub

        ''' <summary>
        ''' ����Mime����
        ''' </summary>
        ''' <param name="siteName">��վ����</param>
        Private Sub SetMime(ByVal siteName As String)
            If _iisManager IsNot Nothing Then
                Dim config As Configuration = _iisManager.GetWebConfiguration(siteName)
                Dim staticContentSection As ConfigurationSection = config.GetSection("system.webServer/staticContent")
                Dim staticContentCollection As ConfigurationElementCollection = staticContentSection.GetCollection()

                SetMime(staticContentCollection, ".ini", "application/octet-stream")
                SetMime(staticContentCollection, ".ifd", "application/ifd")
                SetMime(staticContentCollection, ".dbf", "application/dbf")
            End If
        End Sub

        ''' <summary>
        ''' ����Mime����
        ''' </summary>
        ''' <param name="staticContentCollection">վ�����ü���</param>
        ''' <param name="extension">��չ��</param>
        ''' <param name="mimeType">Mime����</param>
        Private Sub SetMime(ByVal staticContentCollection As ConfigurationElementCollection, ByVal extension As String, ByVal mimeType As String)
            Dim mimeMapElement As ConfigurationElement = staticContentCollection.CreateElement("mimeMap")
            mimeMapElement("fileExtension") = extension
            mimeMapElement("mimeType") = mimeType
            For Each ce As ConfigurationElement In staticContentCollection
                If ce("fileExtension") <> "" AndAlso ce("fileExtension") = extension Then
                    ce("mimeType") = mimeType
                    Return
                End If
            Next ce
            staticContentCollection.Add(mimeMapElement)
        End Sub
#End Region
    End Class
End Namespace
