Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports System.DirectoryServices
Imports System.Diagnostics

Namespace MySoft.IISManage

    ''' <summary>
    ''' IISWebVirtualDir����Ŀ¼�ࡣ
    ''' ����Ҫע�⣺ADsPath �����ִ�Сд�ģ���Ϊ���Ǵ�MetaBase.xml�н�����
    ''' </summary>
    Public Class IISWebVirtualDir

#Region "˽�е�ʵ���ֶ�"
        Private _machine, _identifier, _adsPath, _port, _appPoolId, _defaultDoc, _path, _name, _dir, _appFriendlyName, _aspNetVersion As String
        Private _appIsolated As Integer
        Private _enableDefaultDoc, _enableDirBrowsing, _accessRead, _accessScript, _isApplication As Boolean
        Private _parent As IISWebVirtualDir
        Private _dirEntry As DirectoryEntry
#End Region

#Region "ʵ�����캯��"
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

#Region "������ʵ������"

        ''' <summary>
        '��������Ŀ¼����
        ''' </summary>
        Public ReadOnly Property Parent() As IISWebVirtualDir
            Get
                Return _parent
            End Get
        End Property

        '		/// <summary>
        '		//MachineName���Զ�����ʻ��������֣�������IP�������
        '		/// </summary>
        '		public string MachineName
        '		{
        '			get{ return _machine;}
        '			set{ _machine = value;}
        '		}

        ''' <summary>
        ' ����Ŀ¼����
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
        ' ����·��
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
        '		/// ��վ�Ķ˿ں�
        '		/// </summary>
        '		public string Port
        '		{
        '			get{ return _port; }
        '			set{ _port = value; }
        '		}

        '		/// <summary>
        '		/// ��վ�ı�ʶ��
        '		/// </summary>
        '		public string Identifier
        '		{
        '			get{ return _identifier; }
        '			set{ _identifier = value; }
        '		}


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
        ''' �Ƿ�Ӧ�ó���
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
        ''' ����Ŀ¼��DirectoryEntry����(Root)
        ''' </summary>
        Public ReadOnly Property DirectoryEntry() As DirectoryEntry
            Get
                Return _dirEntry
            End Get
        End Property

        '����Ŀ¼�Ӽ�
        Public ReadOnly Property WebVirtualDirs() As IISWebVirtualDirCollection
            Get
                Return GetVirDirs()
            End Get
        End Property

        'ASP.NET�汾
        Public Property AspNetVersion() As String
            Get
                Return _aspNetVersion
            End Get
            Set(ByVal value As String)
                _aspNetVersion = value
            End Set
        End Property
#End Region

#Region "������ʵ������"

        '����һ������Ŀ¼
        Public Function Create() As Boolean
            '�����ڵ��Ƿ����
            Dim parenADsPath As String = _adsPath.Substring(0, _adsPath.LastIndexOf("/"))
            If (Not IISWebService.IsPathExists(parenADsPath)) Then
                Throw (New Exception(String.Format("����Ŀ¼{0}�����ڣ�����ʧ�ܣ�", parenADsPath)))
            End If

            'Ҫ������Ŀ¼�Ƿ��Ѿ�����
            Dim parentEntry As New DirectoryEntry(parenADsPath)
            Dim blnExistEntry As Boolean = False
            For Each de As DirectoryEntry In parentEntry.Children
                If de.SchemaClassName = "IIsWebVirtualDir" AndAlso de.Name = _name Then
                    _dirEntry = de
                    blnExistEntry = True
                    Exit For
                    'throw(new Exception(string.Format("����Ϊ��{0}��������Ŀ¼�Ѿ����ڣ�����ʧ�ܣ�",_name)));
                End If
            Next de
            If (Not blnExistEntry) Then
                _dirEntry = parentEntry.Children.Add(_name, "IIsWebVirtualDir")
            End If

            '�Ƿ���ΪӦ�ó���
            If Me._isApplication Then
                'Ϊ����Ŀ¼����Ӧ�ó�����(��ͬIIS�汾������ͬ)
                Dim iisVersion As IISVersionEnum = IISWebService.GetIISVersion(_machine)

                If iisVersion = IISVersionEnum.IIS5 OrElse iisVersion = IISVersionEnum.IIS6 OrElse iisVersion = IISVersionEnum.IIS7 Then
                    '����������(0)������������(1)�������⻺���������(2)
                    _dirEntry.Invoke("AppCreate2", Convert.ToInt32(Me._appIsolated))
                Else
                    '����������(TRUE) ������������(FALSE) 
                    _dirEntry.Invoke("AppCreate", Convert.ToInt32(Me._appIsolated))
                End If
            End If

            'Ȼ���������
            Update()
            If (Not blnExistEntry) Then
                parentEntry.CommitChanges()
            End If

            IISWebService.RegisterAspNet(_dirEntry.Path, _aspNetVersion)
            Return True
        End Function


        'ɾ��һ������Ŀ¼
        Public Sub Delete()
            Dim parentEntry As DirectoryEntry = Me._parent.DirectoryEntry


            If IISWebService.IsPathExists(Me._adsPath) Then
                Dim dirEntry As New DirectoryEntry(Me._adsPath)

                '����һ��ʹ�ø��ڵ��ɾ������
                Dim paras(1) As Object
                paras(0) = "IIsWebVirtualDir" '��ʾ������������Ŀ¼
                paras(1) = _dir
                parentEntry.Invoke("Delete", paras)
                parentEntry.CommitChanges()
                '				
            Else
                Throw New Exception(String.Format("Ŀ¼ http://{0}:{1}/{2} �����ڣ�ɾ��ʧ�ܣ�", Me._machine, Me._port, Me._dir))
            End If
        End Sub

        '��������Ŀ¼����
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

#Region "˽�з���"
        '��ʼ��
        Private Sub Init()
            Try
                If String.IsNullOrEmpty(_identifier) Then
                    Me._identifier = IISWebService.GetIdentifier(Me._machine, Me._port)
                End If

                If _dir = "Root" Then
                    '����Ǹ��ڵ�Root����Ҫ���⴦�����������Ŀ¼Ϊ����ΪRoot�أ�
                    Me._adsPath = String.Format("IIS://{0}/W3SVC/{1}/Root", _machine, _identifier)
                Else
                    Me._adsPath = String.Format("IIS://{0}/W3SVC/{1}/Root/{2}", _machine, _identifier, _dir)
                End If

                '��ȡ�����ڵ�
                If IISWebService.IsPathExists(_adsPath.Substring(0, _adsPath.LastIndexOf("/"))) Then
                    If _dir.IndexOf("/") > 0 Then
                        Me._parent = New IISWebVirtualDir(_machine, _port, _dir.Substring(0, _dir.LastIndexOf("/")))
                    ElseIf _dir = "Root" Then
                        Me._parent = Nothing
                    Else
                        Me._parent = New IISWebVirtualDir(_machine, _port, "Root")
                    End If
                Else
                    Throw New Exception(String.Format("http://{0}:{1}/{2}����·�������ڣ�", Me._machine, Me._port, _dir))
                End If

                '��ȡ������Ϣ
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
                    Me._appFriendlyName = "Ĭ��Ӧ�ó���"
                    Me._appIsolated = 2

                    Me._path = ""
                End If

            Catch e As Exception
                Throw New Exception("�޷����ӵ� http://" & _machine & ":" & _port & "/" & _dir & " ��", e)
            End Try
        End Sub


        '��ȡ��ǰĿ¼�����е�ֱ���¼�����Ŀ¼
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


