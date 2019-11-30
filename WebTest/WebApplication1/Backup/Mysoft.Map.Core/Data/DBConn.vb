Imports Microsoft.Win32
Imports System.Configuration
Imports Mysoft.Map.Security
Imports System.Web
Imports Mysoft.Map.Application.Security

Namespace Data
    ''' <summary>
    ''' 提供获取注册表中的数据库连接信息的属性和方法。
    ''' </summary>
    Public Class DBConn
        Private _servername As String
        Private _dbname As String
        Private _username As String
        Private _password As String
        Private _serverprot As String
        Private _DefaultAppName As String = "Default"

        ''' <summary>
        ''' 初始化 <see cref="DBConn" /> 类的新实例。
        ''' </summary>
        Sub New()
            SetProperty(_DefaultAppName)
        End Sub

        ''' <summary>
        ''' 初始化 <see cref="DBConn" /> 类的新实例。
        ''' </summary>
        ''' <param name="appName">应用程序名，默认 <b>Default</b>。参见授权文件（/bin/License.xml）中 dbconns/dbconn 节点的 appname 属性。 </param>
        Sub New(ByVal appName As String)
            SetProperty(appName)
        End Sub

        ''' <summary>
        ''' 读取数据库服务器机器名。
        ''' </summary>
        ''' <value>数据库服务器机器名。</value>
        Public ReadOnly Property ServerName() As String
            Get
                Return Me._servername
            End Get
        End Property

        ''' <summary>
        ''' 读取端口号，默认 1433。
        ''' </summary>
        ''' <value>端口号.</value>
        Public ReadOnly Property ServerProt() As String
            Get
                Return Me._serverprot
            End Get
        End Property

        ''' <summary>
        ''' 读取数据库名称。
        ''' </summary>
        ''' <value>数据库名称。</value>
        Public ReadOnly Property DBName() As String
            Get
                Return Me._dbname
            End Get
        End Property

        ''' <summary>
        ''' 读取数据库登陆用户。
        ''' </summary>
        ''' <value>数据库登陆用户。</value>
        Public ReadOnly Property UserName() As String
            Get
                Return Me._username
            End Get
        End Property

        ''' <summary>
        ''' 读取数据库连接密码。
        ''' </summary>
        ''' <value>数据库连接密码。（原文）</value>
        Public ReadOnly Property Password() As String
            Get
                Return Me._password
            End Get
        End Property

        ' 私有函数
        Private Sub SetProperty(ByVal appName As String)
            Dim Reg As Registry
            Dim RegKey As RegistryKey
            Dim Password As String = ""
            Dim ConnStr As String = ""
            Dim Cryptography As Cryptography
            Dim AppValue As String
            Dim MyLicense As New License

            AppValue = MyLicense.RegName(appName)

            If (Mysoft.Map.Utility.GeneralBase.Detect3264() = "64") Then
                '64位操作系统。由于数据库注册程序是pb做的，在64位操作系统中只能以32位方式运行，它写注册表只能写入到：Software\Wow6432Node\
                RegKey = Reg.LocalMachine.OpenSubKey("Software\Wow6432Node\mysoft\" & AppValue, False)

            Else
                '32位操作系统
                RegKey = Reg.LocalMachine.OpenSubKey("Software\mysoft\" & AppValue, False)
            End If

            If Not RegKey Is Nothing Then
                Password = RegKey.GetValue("SaPassword", "")           ' 从注册表中读取密码
                Password = Cryptography.DeCode(Password)               ' 进行解密反变换

                Me._dbname = RegKey.GetValue("DBName", "")
                Me._servername = RegKey.GetValue("ServerName", "")
                If RegKey.GetValue("ServerProt", "") = "" Then
                    Me._serverprot = "1433"
                Else
                    Me._serverprot = RegKey.GetValue("ServerProt", "")
                End If
                Me._username = RegKey.GetValue("UserName", "")
                Me._password = Password

            Else
                Throw New System.Exception("未配置注册表信息")

            End If
        End Sub

    End Class

End Namespace
