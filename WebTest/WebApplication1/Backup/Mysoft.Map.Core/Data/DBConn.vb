Imports Microsoft.Win32
Imports System.Configuration
Imports Mysoft.Map.Security
Imports System.Web
Imports Mysoft.Map.Application.Security

Namespace Data
    ''' <summary>
    ''' �ṩ��ȡע����е����ݿ�������Ϣ�����Ժͷ�����
    ''' </summary>
    Public Class DBConn
        Private _servername As String
        Private _dbname As String
        Private _username As String
        Private _password As String
        Private _serverprot As String
        Private _DefaultAppName As String = "Default"

        ''' <summary>
        ''' ��ʼ�� <see cref="DBConn" /> �����ʵ����
        ''' </summary>
        Sub New()
            SetProperty(_DefaultAppName)
        End Sub

        ''' <summary>
        ''' ��ʼ�� <see cref="DBConn" /> �����ʵ����
        ''' </summary>
        ''' <param name="appName">Ӧ�ó�������Ĭ�� <b>Default</b>���μ���Ȩ�ļ���/bin/License.xml���� dbconns/dbconn �ڵ�� appname ���ԡ� </param>
        Sub New(ByVal appName As String)
            SetProperty(appName)
        End Sub

        ''' <summary>
        ''' ��ȡ���ݿ��������������
        ''' </summary>
        ''' <value>���ݿ��������������</value>
        Public ReadOnly Property ServerName() As String
            Get
                Return Me._servername
            End Get
        End Property

        ''' <summary>
        ''' ��ȡ�˿ںţ�Ĭ�� 1433��
        ''' </summary>
        ''' <value>�˿ں�.</value>
        Public ReadOnly Property ServerProt() As String
            Get
                Return Me._serverprot
            End Get
        End Property

        ''' <summary>
        ''' ��ȡ���ݿ����ơ�
        ''' </summary>
        ''' <value>���ݿ����ơ�</value>
        Public ReadOnly Property DBName() As String
            Get
                Return Me._dbname
            End Get
        End Property

        ''' <summary>
        ''' ��ȡ���ݿ��½�û���
        ''' </summary>
        ''' <value>���ݿ��½�û���</value>
        Public ReadOnly Property UserName() As String
            Get
                Return Me._username
            End Get
        End Property

        ''' <summary>
        ''' ��ȡ���ݿ��������롣
        ''' </summary>
        ''' <value>���ݿ��������롣��ԭ�ģ�</value>
        Public ReadOnly Property Password() As String
            Get
                Return Me._password
            End Get
        End Property

        ' ˽�к���
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
                '64λ����ϵͳ���������ݿ�ע�������pb���ģ���64λ����ϵͳ��ֻ����32λ��ʽ���У���дע���ֻ��д�뵽��Software\Wow6432Node\
                RegKey = Reg.LocalMachine.OpenSubKey("Software\Wow6432Node\mysoft\" & AppValue, False)

            Else
                '32λ����ϵͳ
                RegKey = Reg.LocalMachine.OpenSubKey("Software\mysoft\" & AppValue, False)
            End If

            If Not RegKey Is Nothing Then
                Password = RegKey.GetValue("SaPassword", "")           ' ��ע����ж�ȡ����
                Password = Cryptography.DeCode(Password)               ' ���н��ܷ��任

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
                Throw New System.Exception("δ����ע�����Ϣ")

            End If
        End Sub

    End Class

End Namespace
