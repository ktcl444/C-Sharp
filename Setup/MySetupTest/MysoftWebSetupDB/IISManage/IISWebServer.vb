Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Text
Imports System.DirectoryServices
Imports System.Reflection


Namespace MySoft.IISManage

    ''' <summary>
    ''' IISWebServer的控制类。IIS中每个网站都是一个WebServer。
    ''' 必须要注意：ADsPath 是区分大小写的，因为它是从MetaBase.xml中解析。
    ''' </summary>
    Public Class IISWebServer
        Implements IIISWebServer

#Region "私有的实例字段"
        Private _machine, _identifier, _adsPath, _port, _defaultDoc, _siteName, _path, _appFriendlyName As String
        Private _appIsolated As Integer
        Private _enableDefaultDoc, _enableDirBrowsing, _autoStart, _accessRead, _accessScript As Boolean
        Private _parent As IISWebService
        Protected _serverEntry, _rootDirEntry As DirectoryEntry
        Private _appPoolId As String = "Mysoft ASP.NET 2.0"
#End Region

#Region "实例构造函数"
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

#Region "公共的实例属性"

        ''' <summary>
        ''' 父级webservice对象
        ''' </summary>
        Public ReadOnly Property Parent() As IISWebService
            Get
                Return _parent
            End Get
        End Property


        ''' <summary>
        ''' 计算机名或者IP。如果本机可以使用localhost。
        ''' </summary>
        Public ReadOnly Property Machine() As String
            Get
                Return _machine
            End Get
        End Property


        ''' <summary>
        ''' 网站名称
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
        ''' 端口号
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
        ''' 物理路径
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
        ''' 网站标识符
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
        '		/// ADsPath路径
        '		/// </summary>
        '		public string ADsPath
        '		{
        '			get{ return _adsPath; }
        '		}

        ''' <summary>
        ''' 应用程序池Id
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
        ''' 读取权限
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
        ''' 脚本支持
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
        ''' 默认文档
        ''' 格式为："Default.htm,Default.asp,index.htm,Default.aspx"
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
        ''' 使用默认文档
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
        ''' 目录浏览
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
        ''' Web 应用程序名称
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
        ''' Web 应用程序的进程类型
        ''' IIS5及以上版本（使用AppCreate2()）： 进程内运行 (0)、进程外运行 (1)、进程池中(2)	
        ''' IIS5以下版本（使用AppCreate()）：进程内 (1) 、进程外 (0)	
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
        ''' 自启动
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
        ''' 网站的DirectoryEntry对象(Root)
        ''' </summary>
        Public ReadOnly Property DirectoryEntry() As DirectoryEntry
            Get
                Return _rootDirEntry
            End Get
        End Property

        ''' <summary>
        ''' 虚拟目录子集
        ''' </summary>
        Public ReadOnly Property WebVirtualDirs() As IISWebVirtualDirCollection
            Get
                Return GetVirDirs()
            End Get
        End Property

#End Region

#Region "公共的实例方法"
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
        '创建网站
        Public Function Create() As Boolean Implements IIISWebServer.Create
            If _port.ToString() = "" OrElse _siteName.ToString() = "" OrElse _path.ToString() = "" Then
                Throw (New Exception("网站名称、端口号、物理路径不允许为空！"))
            End If
            CreateAppPool(AppPoolId)
            Dim identifier As Integer = 1

            '校验不允许同名或者同端口，同时获取最大的标识符号
            For Each server As IISWebServer In Me._parent.WebServers
                If server.SiteName = _siteName Then
                    Throw (New Exception(String.Format("名称为{0}的网站已经存在，创建失败！", _siteName)))
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

            '创建webserver
            Dim svcEntry As DirectoryEntry = Me._parent.DirectoryEntry
            _serverEntry = svcEntry.Children.Add(_identifier, "IIsWebServer")

            _serverEntry.Properties("ServerComment")(0) = _siteName
            _serverEntry.Properties("ServerBindings").Add(":" & _port & ":")
            _serverEntry.Properties("ServerAutoStart")(0) = _autoStart

            _serverEntry.CommitChanges()
            svcEntry.CommitChanges()

            '创建Root目录
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
        '删除网站
        Public Sub Delete() Implements IIISWebServer.Delete
            Dim svcEntry As DirectoryEntry = Me._parent.DirectoryEntry

            If IISWebService.IsPathExists(Me._adsPath) Then
                Dim serverEntry As New DirectoryEntry(Me._adsPath)

                svcEntry.Children.Remove(serverEntry)
                svcEntry.CommitChanges()
            Else
                Throw New Exception(String.Format("网站 http://{0}:{1} 不存在，删除失败！", Me._machine, Me._port))
            End If
        End Sub
        '更新当前网站配置
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
        '停止当前网站
        Public Sub [Stop]() Implements IIISWebServer.Stop
            '_serverEntry.Invoke("Stop",new object[0]);
            _serverEntry.Invoke("Stop", Nothing)
        End Sub
        '启动当前网站
        Public Sub Start() Implements IIISWebServer.Start
            _serverEntry.Invoke("Start", Nothing)
        End Sub
        '暂停当前网站
        Public Sub Pause() Implements IIISWebServer.Pause
            _serverEntry.Invoke("Pause", Nothing)
        End Sub
        '取消暂停
        Public Sub [Continue]() Implements IIISWebServer.Continue
            _serverEntry.Invoke("Continue", Nothing)
        End Sub
        '销毁ADSI对象
        Public Sub Dispose() Implements IIISWebServer.Dispose
            _rootDirEntry.Dispose()
            _serverEntry.Dispose()
        End Sub
        '获取网站的当前状态
        '1 正在启动 
        '2 已启动 
        '3 正在停止 
        '4 已停止 
        '5 正在暂停 
        '6 已暂停 
        '7 正在继续 
        Public Function GetStatus() As IISServerState Implements IIISWebServer.GetStatus
            'Status实际不是方法，而是属性，所以不能使用下行
            'return (int)_serverEntry.Invoke("Status",null);

            '获取ADSI对象的属性
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
        '初始化
        Public Sub Init(ByVal strMachine As String, ByVal strWebName As String, ByVal strWebPort As String, ByVal strWebDir As String, ByVal strAppFriendlyName As String, ByVal appIsolated As Integer, ByVal strAppPool As String) Implements IIISWebServer.Init
            Try
                Me._port = (If(String.IsNullOrEmpty(strWebPort), "", strWebPort))
                Me._machine = (If(String.IsNullOrEmpty(strMachine), "", strMachine))
                '获取父级节点
                Me._parent = New IISWebService(Me._machine)

                '获取配置信息
                If Me._identifier = "" Then
                    Me._identifier = IISWebService.GetIdentifier(_machine, _port)
                End If

                If Me._identifier <> "" Then
                    Me._adsPath = String.Format("IIS://{0}/W3SVC/{1}", _machine, _identifier)

                    'webserver配置信息
                    _serverEntry = New DirectoryEntry(_adsPath)
                    Me._siteName = CStr(_serverEntry.Properties("ServerComment")(0))
                    Me._port = _serverEntry.Properties("ServerBindings")(0).ToString().Replace(":", "")
                    Me._autoStart = CBool(_serverEntry.Properties("ServerAutoStart")(0))

                    'webserver root配置信息(属性返回object类型，必须进行显示转换。)
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
                    Me._appFriendlyName = (If(String.IsNullOrEmpty(strAppFriendlyName), "默认应用程序", strAppFriendlyName))
                    Me._appIsolated = appIsolated
                    Me._path = (If(String.IsNullOrEmpty(strWebDir), "", strWebDir))
                    Me._siteName = (If(String.IsNullOrEmpty(strWebName), "", strWebName))
                End If

            Catch e As Exception
                Throw New Exception("无法连接到网站 http://" & _machine & ":" & _port & " ！", e)
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

#Region "私有方法"

        ''' <summary>
        ''' 创建应用程序池
        ''' </summary>
        ''' <param name="strAppPoolName">应用程序池名称</param>
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
                    ' 0 = Local System（本地系统）
                    ' 1 = Local Service（本地服务）
                    ' 2 = Network Service（网络服务）
                    ' 3 = Custom Identity（自定义配置）需要设置 WAMUserName 和 WAMUserPass 属性

                    newAppPool.Properties("AppPoolIdentityType")(0) = 2
                    'newAppPool.Properties["WAMUserName"][0] = @"VISTA\1"; //domain\用户，注意：此用户必须在IIS_WPG组中
                    'newAppPool.Properties["WAMUserPass"][0] = "1";
                    '其它属性类似，如设置Web园数目：
                    'newAppPool.Properties["MaxProcesses"][0] = 5;
                    newAppPool.CommitChanges()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        '获取根目录下所有的直接下级虚拟目录
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
        ''' 设置Mime类型
        ''' </summary>
        ''' <param name="entry">虚拟目录</param>
        Private Sub SetMime(ByVal entry As DirectoryEntry)
            SetMime(entry, ".ini", "application/octet-stream")
            SetMime(entry, ".ifd", "application/ifd")
            SetMime(entry, ".dbf", "application/dbf")
        End Sub
        ''' <summary>
        ''' 设置Mime类型
        ''' </summary>
        ''' <param name="entry">虚拟目录</param>
        ''' <param name="extension">扩展名</param>
        ''' <param name="mimeType">Mime类型</param>
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
