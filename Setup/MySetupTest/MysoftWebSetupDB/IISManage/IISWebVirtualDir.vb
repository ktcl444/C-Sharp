Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports System.DirectoryServices
Imports System.Diagnostics

Namespace MySoft.IISManage

    ''' <summary>
    ''' IISWebVirtualDir虚拟目录类。
    ''' 必须要注意：ADsPath 是区分大小写的，因为它是从MetaBase.xml中解析。
    ''' </summary>
    Public Class IISWebVirtualDir

#Region "私有的实例字段"
        Private _machine, _identifier, _adsPath, _port, _appPoolId, _defaultDoc, _path, _name, _dir, _appFriendlyName, _aspNetVersion As String
        Private _appIsolated As Integer
        Private _enableDefaultDoc, _enableDirBrowsing, _accessRead, _accessScript, _isApplication As Boolean
        Private _parent As IISWebVirtualDir
        Private _dirEntry As DirectoryEntry
#End Region

#Region "实例构造函数"
        Public Sub New(ByVal machine As String, ByVal port As String, ByVal dir As String)
            Me.New(machine, port, dir, String.Empty)
        End Sub
        Public Sub New(ByVal machine As String, ByVal port As String, ByVal dir As String, ByVal identifier As String)
            _machine = machine
            _port = port
            _dir = dir
            _identifier = identifier
            Init()
        End Sub
#End Region

#Region "公共的实例属性"

        ''' <summary>
        '父级虚拟目录对象
        ''' </summary>
        Public ReadOnly Property Parent() As IISWebVirtualDir
            Get
                Return _parent
            End Get
        End Property

        '		/// <summary>
        '		//MachineName属性定义访问机器的名字，可以是IP或计算名
        '		/// </summary>
        '		public string MachineName
        '		{
        '			get{ return _machine;}
        '			set{ _machine = value;}
        '		}

        ''' <summary>
        ' 虚拟目录名称
        ''' </summary>
        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        ''' <summary>
        ' 物理路径
        ''' </summary>
        Public Property Path() As String
            Get
                Return _path
            End Get
            Set(ByVal value As String)
                _path = value
            End Set
        End Property


        '		/// <summary>
        '		/// 网站的端口号
        '		/// </summary>
        '		public string Port
        '		{
        '			get{ return _port; }
        '			set{ _port = value; }
        '		}

        '		/// <summary>
        '		/// 网站的标识符
        '		/// </summary>
        '		public string Identifier
        '		{
        '			get{ return _identifier; }
        '			set{ _identifier = value; }
        '		}


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
        ''' 是否应用程序
        ''' </summary>
        Public Property IsApplication() As Boolean
            Get
                Return _isApplication
            End Get
            Set(ByVal value As Boolean)
                _isApplication = value
            End Set
        End Property

        ''' <summary>
        ''' 虚拟目录的DirectoryEntry对象(Root)
        ''' </summary>
        Public ReadOnly Property DirectoryEntry() As DirectoryEntry
            Get
                Return _dirEntry
            End Get
        End Property

        '虚拟目录子集
        Public ReadOnly Property WebVirtualDirs() As IISWebVirtualDirCollection
            Get
                Return GetVirDirs()
            End Get
        End Property

        'ASP.NET版本
        Public Property AspNetVersion() As String
            Get
                Return _aspNetVersion
            End Get
            Set(ByVal value As String)
                _aspNetVersion = value
            End Set
        End Property
#End Region

#Region "公共的实例方法"

        '创建一个虚拟目录
        Public Function Create() As Boolean
            '父级节点是否存在
            Dim parenADsPath As String = _adsPath.Substring(0, _adsPath.LastIndexOf("/"))
            If (Not IISWebService.IsPathExists(parenADsPath)) Then
                Throw (New Exception(String.Format("父级目录{0}不存在，创建失败！", parenADsPath)))
            End If

            '要创建的目录是否已经存在
            Dim parentEntry As New DirectoryEntry(parenADsPath)
            Dim blnExistEntry As Boolean = False
            For Each de As DirectoryEntry In parentEntry.Children
                If de.SchemaClassName = "IIsWebVirtualDir" AndAlso de.Name = _name Then
                    _dirEntry = de
                    blnExistEntry = True
                    Exit For
                    'throw(new Exception(string.Format("名称为“{0}”的虚拟目录已经存在，创建失败！",_name)));
                End If
            Next de
            If (Not blnExistEntry) Then
                _dirEntry = parentEntry.Children.Add(_name, "IIsWebVirtualDir")
            End If

            '是否定义为应用程序
            If Me._isApplication Then
                '为虚拟目录创建应用程序定义(不同IIS版本参数不同)
                Dim iisVersion As IISVersionEnum = IISWebService.GetIISVersion(_machine)

                If iisVersion = IISVersionEnum.IIS5 OrElse iisVersion = IISVersionEnum.IIS6 OrElse iisVersion = IISVersionEnum.IIS7 Then
                    '进程内运行(0)、进程外运行(1)、进程外缓冲池中运行(2)
                    _dirEntry.Invoke("AppCreate2", Convert.ToInt32(Me._appIsolated))
                Else
                    '进程内运行(TRUE) 、进程外运行(FALSE) 
                    _dirEntry.Invoke("AppCreate", Convert.ToInt32(Me._appIsolated))
                End If
            End If

            '然后更新数据
            Update()
            If (Not blnExistEntry) Then
                parentEntry.CommitChanges()
            End If

            IISWebService.RegisterAspNet(_dirEntry.Path, _aspNetVersion)
            Return True
        End Function


        '删除一个虚拟目录
        Public Sub Delete()
            Dim parentEntry As DirectoryEntry = Me._parent.DirectoryEntry


            If IISWebService.IsPathExists(Me._adsPath) Then
                Dim dirEntry As New DirectoryEntry(Me._adsPath)

                '方法一：使用根节点的删除方法
                Dim paras(1) As Object
                paras(0) = "IIsWebVirtualDir" '表示操作的是虚拟目录
                paras(1) = _dir
                parentEntry.Invoke("Delete", paras)
                parentEntry.CommitChanges()
                '				
            Else
                Throw New Exception(String.Format("目录 http://{0}:{1}/{2} 不存在，删除失败！", Me._machine, Me._port, Me._dir))
            End If
        End Sub

        '更新虚拟目录配置
        Public Sub Update()
            _dirEntry.Properties("AppPoolId")(0) = _appPoolId
            _dirEntry.Properties("AccessScript")(0) = _accessScript
            _dirEntry.Properties("AccessRead")(0) = _accessRead
            _dirEntry.Properties("DefaultDoc")(0) = _defaultDoc
            _dirEntry.Properties("EnableDefaultDoc")(0) = _enableDefaultDoc
            _dirEntry.Properties("EnableDirBrowsing")(0) = _enableDirBrowsing
            _dirEntry.Properties("AppFriendlyName")(0) = _appFriendlyName
            _dirEntry.Properties("Path")(0) = _path
            _dirEntry.CommitChanges()
        End Sub
#End Region

#Region "私有方法"
        '初始化
        Private Sub Init()
            Try
                If String.IsNullOrEmpty(_identifier) Then
                    Me._identifier = IISWebService.GetIdentifier(Me._machine, Me._port)
                End If

                If _dir = "Root" Then
                    '如果是根节点Root，需要特殊处理。但如果虚拟目录为名就为Root呢？
                    Me._adsPath = String.Format("IIS://{0}/W3SVC/{1}/Root", _machine, _identifier)
                Else
                    Me._adsPath = String.Format("IIS://{0}/W3SVC/{1}/Root/{2}", _machine, _identifier, _dir)
                End If

                '获取父级节点
                If IISWebService.IsPathExists(_adsPath.Substring(0, _adsPath.LastIndexOf("/"))) Then
                    If _dir.IndexOf("/") > 0 Then
                        Me._parent = New IISWebVirtualDir(_machine, _port, _dir.Substring(0, _dir.LastIndexOf("/")))
                    ElseIf _dir = "Root" Then
                        Me._parent = Nothing
                    Else
                        Me._parent = New IISWebVirtualDir(_machine, _port, "Root")
                    End If
                Else
                    Throw New Exception(String.Format("http://{0}:{1}/{2}父级路径不存在！", Me._machine, Me._port, _dir))
                End If

                '获取配置信息
                Dim arrDir() As String = _dir.Split(New Char() {"/"c})
                Me._name = arrDir(arrDir.Length - 1)

                If IISWebService.IsPathExists(_adsPath) Then
                    _dirEntry = New DirectoryEntry(_adsPath)

                    Me._appPoolId = CStr(_dirEntry.Properties("AppPoolId")(0))
                    Me._accessScript = CBool(_dirEntry.Properties("AccessScript")(0))
                    Me._accessRead = CBool(_dirEntry.Properties("AccessRead")(0))
                    Me._defaultDoc = CStr(_dirEntry.Properties("DefaultDoc")(0))
                    Me._enableDefaultDoc = CBool(_dirEntry.Properties("EnableDefaultDoc")(0))
                    Me._enableDirBrowsing = CBool(_dirEntry.Properties("EnableDirBrowsing")(0))
                    Me._appFriendlyName = CStr(_dirEntry.Properties("AppFriendlyName")(0))
                    Me._appIsolated = CInt(Fix(_dirEntry.Properties("AppIsolated")(0)))
                    Me._isApplication = If((_appFriendlyName = ""), False, True)

                    Me._path = CStr(_dirEntry.Properties("Path")(0))
                Else
                    Me._appPoolId = "DefaultAppPool"
                    Me._accessScript = True
                    Me._accessRead = True
                    Me._defaultDoc = "Default.htm,Default.asp,index.htm,iisstart.htm,Default.aspx,index.aspx"
                    Me._enableDefaultDoc = True
                    Me._enableDirBrowsing = False
                    Me._appFriendlyName = "默认应用程序"
                    Me._appIsolated = 2

                    Me._path = ""
                End If

            Catch e As Exception
                Throw New Exception("无法连接到 http://" & _machine & ":" & _port & "/" & _dir & " ！", e)
            End Try
        End Sub


        '获取当前目录下所有的直接下级虚拟目录
        Private Function GetVirDirs() As IISWebVirtualDirCollection
            Dim webVirDirs As New IISWebVirtualDirCollection()
            Dim webVirDir As IISWebVirtualDir

            For Each de As DirectoryEntry In _dirEntry.Children
                If de.SchemaClassName = "IIsWebVirtualDir" Then
                    webVirDir = New IISWebVirtualDir(_machine, _port, de.Name)
                    webVirDirs.Add(webVirDir)
                End If
            Next de

            Return webVirDirs
        End Function


#End Region

    End Class

End Namespace


