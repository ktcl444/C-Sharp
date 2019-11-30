Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Text
Imports System.DirectoryServices
Imports System.Reflection


Namespace MySoft.IISManage

    ''' <summary>
    ''' IISWebServer�Ŀ����ࡣIIS��ÿ����վ����һ��WebServer��
    ''' ����Ҫע�⣺ADsPath �����ִ�Сд�ģ���Ϊ���Ǵ�MetaBase.xml�н�����
    ''' </summary>
    Public Class IISWebServer
        Implements IIISWebServer

#Region "˽�е�ʵ���ֶ�"
        Private _machine, _identifier, _adsPath, _port, _defaultDoc, _siteName, _path, _appFriendlyName As String
        Private _appIsolated As Integer
        Private _enableDefaultDoc, _enableDirBrowsing, _autoStart, _accessRead, _accessScript As Boolean
        Private _parent As IISWebService
        Protected _serverEntry, _rootDirEntry As DirectoryEntry
        Private _appPoolId As String = "Mysoft ASP.NET 2.0"
#End Region

#Region "ʵ�����캯��"
        Public Sub New()

        End Sub

        Public Sub New(ByVal machine As String, ByVal identifier As String, ByVal applicationName As String)
            _machine = machine
            _identifier = identifier
            _appFriendlyName = applicationName
            '_port = port
            Init(machine, "", "", "", applicationName, 2, "")
        End Sub
#End Region

#Region "������ʵ������"

        ''' <summary>
        ''' ����webservice����
        ''' </summary>
        Public ReadOnly Property Parent() As IISWebService
            Get
                Return _parent
            End Get
        End Property


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
        ''' ��վ��ʶ��
        ''' </summary>
        Public Property Identifier() As String
            Get
                Return _identifier
            End Get
            Set(ByVal value As String)
                _identifier = value
            End Set
        End Property


        '		/// <summary>
        '		/// ADsPath·��
        '		/// </summary>
        '		public string ADsPath
        '		{
        '			get{ return _adsPath; }
        '		}

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
        ''' ��ȡȨ��
        ''' </summary>
        Public Property AccessRead() As Boolean
            Get
                Return _accessRead
            End Get
            Set(ByVal value As Boolean)
                _accessRead = value
            End Set
        End Property

        ''' <summary>
        ''' �ű�֧��
        ''' </summary>
        Public Property AccessScript() As Boolean
            Get
                Return _accessScript
            End Get
            Set(ByVal value As Boolean)
                _accessScript = value
            End Set
        End Property

        ''' <summary>
        ''' Ĭ���ĵ�
        ''' ��ʽΪ��"Default.htm,Default.asp,index.htm,Default.aspx"
        ''' </summary>
        Public Property DefaultDoc() As String
            Get
                Return _defaultDoc
            End Get
            Set(ByVal value As String)
                _defaultDoc = value
            End Set
        End Property

        ''' <summary>
        ''' ʹ��Ĭ���ĵ�
        ''' </summary>
        Public Property EnableDefaultDoc() As Boolean
            Get
                Return _enableDefaultDoc
            End Get
            Set(ByVal value As Boolean)
                _enableDefaultDoc = value
            End Set
        End Property

        ''' <summary>
        ''' Ŀ¼���
        ''' </summary>
        Public Property EnableDirBrowsing() As Boolean
            Get
                Return _enableDirBrowsing
            End Get
            Set(ByVal value As Boolean)
                _enableDirBrowsing = value
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

        ''' <summary>
        ''' ������
        ''' </summary>
        Public Property AutoStart() As Boolean
            Get
                Return _autoStart
            End Get
            Set(ByVal value As Boolean)
                _autoStart = value
            End Set
        End Property


        ''' <summary>
        ''' ��վ��DirectoryEntry����(Root)
        ''' </summary>
        Public ReadOnly Property DirectoryEntry() As DirectoryEntry
            Get
                Return _rootDirEntry
            End Get
        End Property

        ''' <summary>
        ''' ����Ŀ¼�Ӽ�
        ''' </summary>
        Public ReadOnly Property WebVirtualDirs() As IISWebVirtualDirCollection
            Get
                Return GetVirDirs()
            End Get
        End Property

#End Region

#Region "������ʵ������"
        Public Sub SetSiteProperties() Implements IIISWebServer.SetSiteProperties
            CreateAppPool(AppPoolId)
            _rootDirEntry.Properties("AppPoolId")(0) = AppPoolId
            If Me.AppFriendlyName <> "" Then
                Dim appEntry As DirectoryEntry = New DirectoryEntry(_rootDirEntry.Path & "/" & Me.AppFriendlyName)
                appEntry.Properties("AppPoolId")(0) = AppPoolId
                appEntry.CommitChanges()
            End If
            SetMime(_rootDirEntry)
            _rootDirEntry.CommitChanges()
            IISWebService.RegisterAspNet(_rootDirEntry.Path, "v2.0.50727")
        End Sub
        '������վ
        Public Function Create() As Boolean Implements IIISWebServer.Create
            If _port.ToString() = "" OrElse _siteName.ToString() = "" OrElse _path.ToString() = "" Then
                Throw (New Exception("��վ���ơ��˿ںš�����·��������Ϊ�գ�"))
            End If
            CreateAppPool(AppPoolId)
            Dim identifier As Integer = 1

            'У�鲻����ͬ������ͬ�˿ڣ�ͬʱ��ȡ���ı�ʶ����
            For Each server As IISWebServer In Me._parent.WebServers
                If server.SiteName = _siteName Then
                    Throw (New Exception(String.Format("����Ϊ{0}����վ�Ѿ����ڣ�����ʧ�ܣ�", _siteName)))
                End If
                If server.Port = _port Then
                    Me._parent.DirectoryEntry.Children.Find(server.Identifier, "IIsWebServer").Invoke("Stop", Nothing)
                End If

                If Convert.ToInt32(server.Identifier) > identifier Then
                    identifier = Convert.ToInt32(server.Identifier)
                End If
            Next server
            _identifier = (identifier + 1).ToString()
            _adsPath = String.Format("IIS://{0}/W3SVC/{1}", _machine, _identifier)

            '����webserver
            Dim svcEntry As DirectoryEntry = Me._parent.DirectoryEntry
            _serverEntry = svcEntry.Children.Add(_identifier, "IIsWebServer")

            _serverEntry.Properties("ServerComment")(0) = _siteName
            _serverEntry.Properties("ServerBindings").Add(":" & _port & ":")
            _serverEntry.Properties("ServerAutoStart")(0) = _autoStart

            _serverEntry.CommitChanges()
            svcEntry.CommitChanges()

            '����RootĿ¼
            _rootDirEntry = New DirectoryEntry(_adsPath & "/Root")
            Dim virDir As New IISWebVirtualDir(_machine, _port, "Root", _identifier)

            virDir.Path = Me._path
            virDir.AppFriendlyName = Me._appFriendlyName
            virDir.AppIsolated = Me._appIsolated
            virDir.IsApplication = Not String.IsNullOrEmpty(_appFriendlyName)
            virDir.AppPoolId = _appPoolId
            virDir.AspNetVersion = "v2.0.50727"

            virDir.Create()

            SetMime(virDir.DirectoryEntry)

            Return True
        End Function
        'ɾ����վ
        Public Sub Delete() Implements IIISWebServer.Delete
            Dim svcEntry As DirectoryEntry = Me._parent.DirectoryEntry

            If IISWebService.IsPathExists(Me._adsPath) Then
                Dim serverEntry As New DirectoryEntry(Me._adsPath)

                svcEntry.Children.Remove(serverEntry)
                svcEntry.CommitChanges()
            Else
                Throw New Exception(String.Format("��վ http://{0}:{1} �����ڣ�ɾ��ʧ�ܣ�", Me._machine, Me._port))
            End If
        End Sub
        '���µ�ǰ��վ����
        Public Function Update() As Boolean Implements IIISWebServer.Update
            _serverEntry.Properties("ServerComment")(0) = _siteName
            _serverEntry.Properties("ServerBindings").Add(":" & _port & ":")
            _serverEntry.Properties("ServerAutoStart")(0) = _autoStart

            _rootDirEntry.Properties("AppPoolId")(0) = _appPoolId
            _rootDirEntry.Properties("AccessScript")(0) = _accessScript
            _rootDirEntry.Properties("AccessRead")(0) = _accessRead
            _rootDirEntry.Properties("DefaultDoc")(0) = _defaultDoc
            _rootDirEntry.Properties("EnableDefaultDoc")(0) = _enableDefaultDoc
            _rootDirEntry.Properties("EnableDirBrowsing")(0) = _enableDirBrowsing
            _rootDirEntry.Properties("AppFriendlyName")(0) = _appFriendlyName

            _rootDirEntry.Properties("Path")(0) = _path

            _rootDirEntry.CommitChanges()
            _serverEntry.CommitChanges()

            Return True
        End Function
        'ֹͣ��ǰ��վ
        Public Sub [Stop]() Implements IIISWebServer.Stop
            '_serverEntry.Invoke("Stop",new object[0]);
            _serverEntry.Invoke("Stop", Nothing)
        End Sub
        '������ǰ��վ
        Public Sub Start() Implements IIISWebServer.Start
            _serverEntry.Invoke("Start", Nothing)
        End Sub
        '��ͣ��ǰ��վ
        Public Sub Pause() Implements IIISWebServer.Pause
            _serverEntry.Invoke("Pause", Nothing)
        End Sub
        'ȡ����ͣ
        Public Sub [Continue]() Implements IIISWebServer.Continue
            _serverEntry.Invoke("Continue", Nothing)
        End Sub
        '����ADSI����
        Public Sub Dispose() Implements IIISWebServer.Dispose
            _rootDirEntry.Dispose()
            _serverEntry.Dispose()
        End Sub
        '��ȡ��վ�ĵ�ǰ״̬
        '1 �������� 
        '2 ������ 
        '3 ����ֹͣ 
        '4 ��ֹͣ 
        '5 ������ͣ 
        '6 ����ͣ 
        '7 ���ڼ��� 
        Public Function GetStatus() As IISServerState Implements IIISWebServer.GetStatus
            'Statusʵ�ʲ��Ƿ������������ԣ����Բ���ʹ������
            'return (int)_serverEntry.Invoke("Status",null);

            '��ȡADSI���������
            Dim ads As Object = _serverEntry.NativeObject
            Dim type As Type = ads.GetType() 'typeof(DirectoryEntry.NativeObject);
            Dim status As Integer = CInt(Fix(type.InvokeMember("Status", BindingFlags.GetProperty, Nothing, ads, Nothing)))

            Dim e As IISServerState
            Select Case status
                Case 1
                    e = IISServerState.Starting
                Case 2
                    e = IISServerState.Started
                Case 3
                    e = IISServerState.Stopping
                Case 4
                    e = IISServerState.Stopped
                Case 5
                    e = IISServerState.Pausing
                Case 6
                    e = IISServerState.Paused
                Case 7
                    e = IISServerState.Continuing
                Case Else
                    e = IISServerState.Unkonwn
            End Select
            Return e
        End Function
        '��ʼ��
        Public Sub Init(ByVal strMachine As String, ByVal strWebName As String, ByVal strWebPort As String, ByVal strWebDir As String, ByVal strAppFriendlyName As String, ByVal appIsolated As Integer, ByVal strAppPool As String) Implements IIISWebServer.Init
            Try
                Me._port = (If(String.IsNullOrEmpty(strWebPort), "", strWebPort))
                Me._machine = (If(String.IsNullOrEmpty(strMachine), "", strMachine))
                '��ȡ�����ڵ�
                Me._parent = New IISWebService(Me._machine)

                '��ȡ������Ϣ
                If Me._identifier = "" Then
                    Me._identifier = IISWebService.GetIdentifier(_machine, _port)
                End If

                If Me._identifier <> "" Then
                    Me._adsPath = String.Format("IIS://{0}/W3SVC/{1}", _machine, _identifier)

                    'webserver������Ϣ
                    _serverEntry = New DirectoryEntry(_adsPath)
                    Me._siteName = CStr(_serverEntry.Properties("ServerComment")(0))
                    Me._port = _serverEntry.Properties("ServerBindings")(0).ToString().Replace(":", "")
                    Me._autoStart = CBool(_serverEntry.Properties("ServerAutoStart")(0))

                    'webserver root������Ϣ(���Է���object���ͣ����������ʾת����)
                    If IISWebService.IsPathExists(_adsPath & "/Root") Then
                        _rootDirEntry = New DirectoryEntry(_adsPath & "/Root")
                        'Me._appPoolId = CStr(_rootDirEntry.Properties("AppPoolId")(0))
                        Me._accessScript = CBool(_rootDirEntry.Properties("AccessScript")(0))
                        Me._accessRead = CBool(_rootDirEntry.Properties("AccessRead")(0))
                        Me._defaultDoc = CStr(_rootDirEntry.Properties("DefaultDoc")(0))
                        Me._enableDefaultDoc = CBool(_rootDirEntry.Properties("EnableDefaultDoc")(0))
                        Me._enableDirBrowsing = CBool(_rootDirEntry.Properties("EnableDirBrowsing")(0))
                        'Me._appFriendlyName = CStr(_rootDirEntry.Properties("AppFriendlyName")(0))
                        Me._appIsolated = CInt(Fix(_rootDirEntry.Properties("AppIsolated")(0)))
                        Me._path = CStr(_rootDirEntry.Properties("Path")(0))
                    End If
                Else
                    Me._appPoolId = (If(String.IsNullOrEmpty(strAppPool), "DefaultAppPool", strAppPool))
                    Me._accessScript = True
                    Me._accessRead = True
                    Me._defaultDoc = "Default.htm,Default.asp,index.htm,iisstart.htm,Default.aspx,index.aspx"
                    Me._enableDefaultDoc = True
                    Me._enableDirBrowsing = False
                    Me._appFriendlyName = (If(String.IsNullOrEmpty(strAppFriendlyName), "Ĭ��Ӧ�ó���", strAppFriendlyName))
                    Me._appIsolated = appIsolated
                    Me._path = (If(String.IsNullOrEmpty(strWebDir), "", strWebDir))
                    Me._siteName = (If(String.IsNullOrEmpty(strWebName), "", strWebName))
                End If

            Catch e As Exception
                Throw New Exception("�޷����ӵ���վ http://" & _machine & ":" & _port & " ��", e)
            End Try
        End Sub

        Public Function CheckPortRepeated() As Boolean Implements IIISWebServer.CheckPortRepeated
            For Each server As IISWebServer In Me._parent.WebServers
                If server.Port = _port Then
                    Return True
                End If
            Next server
            Return False
        End Function
#End Region

#Region "˽�з���"

        ''' <summary>
        ''' ����Ӧ�ó����
        ''' </summary>
        ''' <param name="strAppPoolName">Ӧ�ó��������</param>
        Private Sub CreateAppPool(ByVal strAppPoolName As String)
            Try
                Dim blnExistAppPool As Boolean = False
                Dim appPoolRoot As DirectoryEntry = New System.DirectoryServices.DirectoryEntry("IIS://localhost/W3SVC/AppPools")
                For Each appPool As DirectoryEntry In appPoolRoot.Children
                    If appPool.Name = strAppPoolName Then
                        blnExistAppPool = True
                        Exit For
                    End If
                Next appPool
                If (Not blnExistAppPool) Then
                    Dim newAppPool As DirectoryEntry = appPoolRoot.Children.Add(strAppPoolName, "IIsApplicationPool")
                    ' 0 = Local System������ϵͳ��
                    ' 1 = Local Service�����ط���
                    ' 2 = Network Service���������
                    ' 3 = Custom Identity���Զ������ã���Ҫ���� WAMUserName �� WAMUserPass ����

                    newAppPool.Properties("AppPoolIdentityType")(0) = 2
                    'newAppPool.Properties["WAMUserName"][0] = @"VISTA\1"; //domain\�û���ע�⣺���û�������IIS_WPG����
                    'newAppPool.Properties["WAMUserPass"][0] = "1";
                    '�����������ƣ�������Web԰��Ŀ��
                    'newAppPool.Properties["MaxProcesses"][0] = 5;
                    newAppPool.CommitChanges()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        '��ȡ��Ŀ¼�����е�ֱ���¼�����Ŀ¼
        Private Function GetVirDirs() As IISWebVirtualDirCollection
            Dim webVirDirs As New IISWebVirtualDirCollection()
            Dim webVirDir As IISWebVirtualDir

            For Each de As DirectoryEntry In _rootDirEntry.Children
                If de.SchemaClassName = "IIsWebVirtualDir" Then
                    webVirDir = New IISWebVirtualDir(_machine, _port, de.Name)
                    webVirDirs.Add(webVirDir)
                End If
            Next de

            Return webVirDirs
        End Function

        ''' <summary>
        ''' ����Mime����
        ''' </summary>
        ''' <param name="entry">����Ŀ¼</param>
        Private Sub SetMime(ByVal entry As DirectoryEntry)
            SetMime(entry, ".ini", "application/octet-stream")
            SetMime(entry, ".ifd", "application/ifd")
            SetMime(entry, ".dbf", "application/dbf")
        End Sub
        ''' <summary>
        ''' ����Mime����
        ''' </summary>
        ''' <param name="entry">����Ŀ¼</param>
        ''' <param name="extension">��չ��</param>
        ''' <param name="mimeType">Mime����</param>
        Private Sub SetMime(ByVal entry As DirectoryEntry, ByVal extension As String, ByVal mimeType As String)
            If entry Is Nothing Then
                Return
            End If
            Dim mime As PropertyValueCollection = entry.Properties("MimeMap")

            For Each value As Object In mime
                Dim mimeTypeObj As IISOle.IISMimeType = CType(value, IISOle.IISMimeType)
                If extension = mimeTypeObj.Extension Then
                    mime.Remove(value)
                    entry.CommitChanges()
                    Exit For
                End If
            Next value
            Dim newMime As New IISOle.MimeMapClass()
            newMime.Extension = extension
            newMime.MimeType = mimeType

            mime.Add(newMime)
        End Sub
#End Region

    End Class

End Namespace
