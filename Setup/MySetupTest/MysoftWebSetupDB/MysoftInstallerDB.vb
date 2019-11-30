Imports System.ComponentModel
Imports System.Configuration.Install
Imports System
Imports System.IO
Imports System.Security.Permissions
Imports System.Security
Imports System.Xml
Imports Microsoft.Win32
Imports System.Diagnostics
Imports Microsoft.Win32.Security
Imports MysoftWebSetupDB.MySoft.IISManage
Imports System.Collections.Specialized
Imports System.Management
Imports System.DirectoryServices

<RunInstaller(True)> Public Class MysoftInstallerDB
    Inherits System.Configuration.Install.Installer

#Region " 组件设计器生成的代码 "

    Public Sub New()
        MyBase.New()

        '该调用是组件设计器所必需的。
        InitializeComponent()

        '在 InitializeComponent() 调用之后添加任何初始化

    End Sub

    'Installer 重写 dispose 以清理组件列表。
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    '组件设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是组件设计器所必需的
    '可以使用组件设计器来修改此过程。
    '不要使用代码编辑器来修改它。
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container
    End Sub

#End Region

#Region "私有变量"

    Private strWebName As String = String.Empty
    Private strWebPort As String = String.Empty
    Private strMachineName As String = String.Empty
    Private strVDirectory As String = String.Empty
    Private strWebDir As String = String.Empty
    Private strAppFriendlyName As String = String.Empty
    Private strSourceDir As String = String.Empty
    Private strAppPool10 As String = "Mysoft ASP.NET 1.0"
    Private strAppPool20 As String = "Mysoft ASP.NET 2.0"
    Private appIsolated As Integer = 2

    Private strTargetSite As String = String.Empty
    Private strTargetAppPool As String = String.Empty
    Private strTargetVDir As String = String.Empty

#End Region

    ''' <summary>
    ''' 安装
    ''' </summary>
    ''' <param name="stateSaver"></param>
    Public Overrides Sub Install(ByVal stateSaver As System.Collections.IDictionary)

        MyBase.Install(stateSaver)

        Me.InitParameters()
        Me.SetWebProperties()
        Me.CopyFile()
        test2()
    End Sub
    Public Overrides Sub Commit(ByVal savedState As System.Collections.IDictionary)
        MyBase.Commit(savedState)
    End Sub
    Protected Overrides Sub OnCommitting(ByVal savedState As System.Collections.IDictionary)
        MyBase.OnCommitting(savedState)
    End Sub
    Private Sub test2()
        Dim iden As String = IISWebService.GetIdentifier("localhost", "8054")
        Dim strPath As String = String.Format("IIS://localhost/W3SVC/{0}/ROOT", iden)

        Dim root As New DirectoryEntry(strPath)
        Dim serverEntry As DirectoryEntry
        Try
            serverEntry = root.Children.Find("UpFiles", "IIsWebDirectory") '如果UpFiles节点不存在，则会报DirectoryNotFoundException异常
        Catch ee As System.IO.DirectoryNotFoundException
            serverEntry = Nothing
        End Try
        If serverEntry Is Nothing Then
            serverEntry = root.Children.Add("UpFiles", "IIsWebDirectory")
        End If
        serverEntry.Properties("AccessRead")(0) = False
        serverEntry.CommitChanges()
    End Sub
    Private Sub Test1()
        Dim scope As New ManagementScope("\\.\root\MicrosoftIISV2")
        scope.Connect()
        Dim mc As New ManagementClass(scope, New ManagementPath("IIsWebDirectorySetting"), Nothing)
        Dim oWebDir As ManagementObject = mc.CreateInstance()
        Dim indet As Int32 = IISWebService.GetIdentifier("localhost", "8055")
        oWebDir.Properties("Name").Value = "W3SVC/" & indet & "/ROOT"
        oWebDir.Properties("AccessFlags").Value = 512
        oWebDir.Put()
    End Sub
    Private Sub Test()
        System.Diagnostics.Debugger.Launch()
        Dim scope As New ManagementScope("\\.\root\MicrosoftIISV2")
        scope.Connect()
        Dim mc As New ManagementClass(scope, New ManagementPath("IIsWebDirectorySetting"), Nothing)
        Dim oWebDir As ManagementObject = mc.CreateInstance()
        Dim indet As Int32 = IISWebService.GetIdentifier("localhost", "8057")
        oWebDir.Properties("Name").Value = "W3SVC/" & indet & "/ROOT/UpFiles"
        'oWebDir.Properties("AccessFlags").Value = 1
        oWebDir.Properties("AccessRead").Value = False
        oWebDir.Put()
        Dim strPath As String = String.Format("IIS://localhost/W3SVC/{0}/ROOT/UpFiles", indet)
        Dim root As New DirectoryEntry(strPath)
        root.Properties("AccessRead")(0) = False
        root.CommitChanges()
    End Sub
    Private Sub SetWebProperties()
        Dim isrv As IIISWebServer = IISWebServerFactory.GetIISWebServer(strMachineName, GetIdentifierByTargetSite(strTargetSite), strTargetVDir)
        isrv.SetSiteProperties()
    End Sub
    Private Function GetIdentifierByTargetSite(ByVal targetSite As String) As String
        Dim targetSiteLength, tarGetSiteIdentifierLength As Integer
        'targetSiteLength = targetSite.Length
        'tarGetSiteIdentifierLength = targetSiteLength - targetSite.LastIndexOf("/") - 1
        Return targetSite.Substring(targetSite.LastIndexOf("/") + 1, targetSite.Length - targetSite.LastIndexOf("/") - 1)
        'Return targetSite.Substring(targetSiteLength - tarGetSiteIdentifierLength + 1, targetSiteLength)
    End Function

    ''' <summary>
    ''' 复制64位DLL
    ''' </summary>
    Private Sub CopyFile()
        Dim systemFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.System)
        Dim sourceDir As String = strSourceDir.Substring(0, strSourceDir.LastIndexOf("\")) & "\SUPPORT\system32\"
        Dim targetDir As String = systemFolder & "\"

        Dim fileName As String = "hasp_net_windows.dll"
        CopyFile(fileName, sourceDir, targetDir)

        fileName = "hasp_net_windows_x64.dll"
        CopyFile(fileName, sourceDir, targetDir)

        fileName = "hasp_windows_demo.dll"
        CopyFile(fileName, sourceDir, targetDir)

        fileName = "hasp_windows_x64_demo.dll"
        CopyFile(fileName, sourceDir, targetDir)
    End Sub
    Private Sub CopyFile(ByVal fileName As String, ByVal sourceDir As String, ByVal targetDir As String)
        If File.Exists(sourceDir & fileName) AndAlso Directory.Exists(targetDir) Then
            File.Copy(sourceDir & fileName, targetDir & fileName, True)
        End If
    End Sub
    ''' <summary>
    ''' 初始化参数
    ''' </summary>
    Private Sub InitParameters()

        strMachineName = "localhost"
        Me.strTargetAppPool = Me.Context.Parameters("strTargetAppPool")
        Me.strTargetSite = Me.Context.Parameters("strTargetSite")
        Me.strTargetVDir = Me.Context.Parameters("strTargetVDir")
        Me.strSourceDir = Me.Context.Parameters("strSourceDir")
        'Dim a As StringDictionary = CType(Context.Parameters, StringDictionary)

        'Dim myEnumerator As IEnumerator = a.GetEnumerator()
        'Dim de As DictionaryEntry

        'Dim b As String = ""
        'For Each de In a
        '    b += de.Key + "" + de.Value
        'Next de

        'strWebPort = Me.Context.Parameters("strWebPort")
        'strWebDir = Me.Context.Parameters("targetdir")
        'Dim strVWebDir = Me.Context.Parameters("targetvdir")
        'strSourceDir = Me.Context.Parameters("strSourceDir")
        'If String.IsNullOrEmpty(strAppFriendlyName) Then
        '    strAppFriendlyName = "默认应用程序"
        'End If
    End Sub
    Protected Overrides Sub OnBeforeInstall(ByVal savedState As System.Collections.IDictionary)
        MyBase.OnBeforeInstall(savedState)
        'MsgBox("开始安装，beforeInstall")
    End Sub

    Protected Overrides Sub OnAfterInstall(ByVal savedState As System.Collections.IDictionary)
        MyBase.OnAfterInstall(savedState)
        Dim strLogicDir As String = Context.Parameters("targetdir")

        '目录授权
        Try
            Dim xmlDoc As New XmlDocument
            Dim xmlNList As XmlNodeList
            Dim xmlN As XmlNode
            Dim i As Integer
            Dim strXmlPath As String
            Dim strPath As String

            '获取程序执行路径
            '加载 XML 文件
            xmlDoc.Load(strLogicDir & "\bin\SetPermission.xml")

            Dim strType As String
            Dim blnIsFile As Boolean
            Dim strUserName As String
            Dim intAccessType As Integer
            Dim strTemp As String

            xmlNList = xmlDoc.SelectNodes("/xml/path")
            For i = 0 To xmlNList.Count - 1
                xmlN = xmlDoc.DocumentElement.ChildNodes(i)

                strPath = getNodeText(xmlN, "name")
                strType = getNodeText(xmlN, "type")

                '授权对象是否为文件
                strTemp = getNodeText(xmlN, "isfile")
                If strTemp = "" Then
                    blnIsFile = False
                Else
                    blnIsFile = Boolean.Parse(strTemp)
                End If

                strUserName = getNodeText(xmlN, "username")

                ''授权类型
                'strTemp = getNodeText(xmlN, "accesstype")
                'intAccessType = Integer.Parse(strTemp)

                Select Case strType.ToLower
                    Case "absolute"         '绝对路径
                        strPath = Path.GetFullPath(strPath)

                    Case "windows"          '相对 Windows 目录的路径
                        strPath = Path.GetFullPath(Environment.GetEnvironmentVariable("windir") & "\" & strPath)

                    Case "system"
                        strPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.System) & "\" & strPath)

                    Case "programfiles"
                        strPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\" & strPath)

                    Case "relative"         '相对于传入参数的路径
                        strPath = strLogicDir & "\" & strPath
                        strPath = Path.GetFullPath(strPath)
                    Case Else
                        strPath = ""
                End Select

                If strPath <> "" Then
                    '设置文件/文件夹权限
                    '默认设置所有权限
                    If blnIsFile Then
                        '去掉只读属性
                        Dim objFI As IO.FileInfo
                        objFI = New IO.FileInfo(strPath)
                        If objFI.Exists Then
                            objFI.Attributes = objFI.Attributes And Not IO.FileAttributes.ReadOnly
                        End If

                        MySec.SetFileSec(strPath, strUserName, FileAccessType.GENERIC_ALL)
                    Else
                        If Not Directory.Exists(strPath) Then
                            Directory.CreateDirectory(strPath)
                        End If
                        MySec.SetFolderSec(strPath, strUserName, DirectoryAccessType.FILE_ALL_ACCESS)
                    End If
                End If
            Next
        Catch ex As Exception
        End Try

        'MsgBox("结束安装，afterInstall，安装路径：" & strLogicDir)

        'ERP设计变更，某客户可能会先购买CRM系统，后期又增购POM系统，此时客户只需要更新License即可,因此对于ERP系统不需要删除各业务系统(2006.9.8)
        '根据License.xml 配置文件，删除无效的文件目录
        'Dim docXML As New System.Xml.XmlDocument
        'docXML.Load(strLogicDir & "\bin\License.xml")
        'Dim nodeXML As System.Xml.XmlNode

        'Dim blHaveSL As Boolean = True         ' 是否启用售楼
        'nodeXML = docXML.SelectSingleNode("//system/subsystem[@code=""0101""]")
        'If nodeXML Is Nothing OrElse nodeXML.Attributes("enabled").Value <> "1" Then
        'blHaveSL = False
        'End If

        'Dim blHaveKf As Boolean = True         ' 是否启用客服
        'nodeXML = docXML.SelectSingleNode("//system/subsystem[@code=""0102""]")
        'If nodeXML Is Nothing OrElse nodeXML.Attributes("enabled").Value <> "1" Then
        'blHaveKf = False
        'End If

        'Dim blHaveHy As Boolean = True         ' 是否启用会员
        'nodeXML = docXML.SelectSingleNode("//system/subsystem[@code=""0103""]")
        'If nodeXML Is Nothing OrElse nodeXML.Attributes("enabled").Value <> "1" Then
        'blHaveHy = False
        'End If

        'Dim blHaveZl As Boolean = True         ' 是否启用租赁
        'nodeXML = docXML.SelectSingleNode("//system/subsystem[@code=""0104""]")
        'If nodeXML Is Nothing OrElse nodeXML.Attributes("enabled").Value <> "1" Then
        '    blHaveZl = False
        'End If

        'Dim blHaveCb As Boolean = True         ' 是否启用成本
        'nodeXML = docXML.SelectSingleNode("//system/subsystem[@code=""0201""]")
        'If nodeXML Is Nothing OrElse nodeXML.Attributes("enabled").Value <> "1" Then
        '    blHaveCb = False
        'End If

        'Dim blHaveXmJd As Boolean = True       ' 是否启用项目进度
        'nodeXML = docXML.SelectSingleNode("//system/subsystem[@code=""0202""]")
        'If nodeXML Is Nothing OrElse nodeXML.Attributes("enabled").Value <> "1" Then
        '    blHaveXmJd = False
        'End If

        'Dim blHaveMc As Boolean = True         ' 是否启用管理驾驶舱
        'nodeXML = docXML.SelectSingleNode("//system/subsystem[@code=""0301""]")
        'If nodeXML Is Nothing OrElse nodeXML.Attributes("enabled").Value <> "1" Then
        '    blHaveMc = False
        'End If

        'Dim blHaveSlpad As Boolean = True      ' 是否启用销售PDA
        'nodeXML = docXML.SelectSingleNode("//system/subsystem/function[@code=""010115""]")
        'If nodeXML Is Nothing OrElse nodeXML.Attributes("enabled").Value <> "1" Then
        '    blHaveSlpad = False
        'End If

        'Dim strPath As String = ""

        ''无销售且无客服且无会员,删除Slxt目录
        'If blHaveSL = False AndAlso blHaveKf = False AndAlso blHaveHy = False Then
        '    Try
        '        strPath = strLogicDir & "\Slxt"
        '        Directory.Delete(strPath)
        '    Catch ex As Exception

        '    End Try
        'End If

        ''无客服,删除Kfxt目录
        'If blHaveKf = False Then
        '    Try
        '        strPath = strLogicDir & "\Kfxt"
        '        Directory.Delete(strPath)
        '    Catch ex As Exception

        '    End Try
        'End If

        ''无会员,删除Hyxt目录
        'If blHaveHy = False Then
        '    Try
        '        strPath = strLogicDir & "\Hyxt"
        '        Directory.Delete(strPath)
        '    Catch ex As Exception

        '    End Try
        'End If

        ''无租赁,删除Zlxt目录
        'If blHaveZl = False Then
        '    Try
        '        strPath = strLogicDir & "\Zlxt"
        '        Directory.Delete(strPath)
        '    Catch ex As Exception

        '    End Try
        'End If

        ''无成本且无项目进度,删除Cbgl和Xmjd目录
        'If blHaveCb = False AndAlso blHaveXmJd = False Then
        '    Try
        '        strPath = strLogicDir & "\Cbgl"
        '        Directory.Delete(strPath)

        '        strPath = strLogicDir & "\Xmjd"
        '        Directory.Delete(strPath)

        '    Catch ex As Exception

        '    End Try
        'End If

        ''无销售PDA,删除SlPda目录
        'If blHaveHy = False Then
        '    Try
        '        strPath = strLogicDir & "\SlPda"
        '        Directory.Delete(strPath)
        '    Catch ex As Exception

        '    End Try
        'End If

        ''无无管理驾驶舱,删除POMMC 和 MC 目录
        'If blHaveHy = False Then
        '    Try
        '        strPath = strLogicDir & "\POMMC"
        '        Directory.Delete(strPath)

        '        strPath = strLogicDir & "\MC"
        '        Directory.Delete(strPath)

        '    Catch ex As Exception

        '    End Try
        'End If

        '更新安装注册表中的键值为1（成功）---.net需要有很高的权限才能更改注册表,所以在此屏蔽.
        'Dim Reg As Registry
        'Dim RegKey As RegistryKey
        'Dim strRegName As String = "Setup_ERP10"

        'RegKey = Reg.LocalMachine.OpenSubKey("Software\mysoft\" & strRegName, True)
        'If Not RegKey Is Nothing Then
        '    Dim i As Integer
        '    For i = 0 To RegKey.GetValueNames.Length - 1
        '        RegKey.SetValue(RegKey.GetValueNames(i), "1")
        '    Next
        'End If

    End Sub

    Protected Overrides Sub OnCommitted(ByVal savedState As System.Collections.IDictionary)
        MyBase.OnCommitted(savedState)
        'MsgBox("提交，onCommitted")
    End Sub


    Protected Overrides Sub OnAfterUninstall(ByVal savedState As System.Collections.IDictionary)
        '更新安装注册表中的键值为1（成功）
        Dim Reg As Registry
        Dim RegKey As RegistryKey
        Dim strRegName As String = "Setup_ERP20"

        RegKey = Reg.LocalMachine.OpenSubKey("Software\mysoft\" & strRegName, True)
        If Not RegKey Is Nothing Then
            Dim i As Integer
            For i = 0 To RegKey.GetSubKeyNames.Length - 1
                RegKey.SetValue(RegKey.GetSubKeyNames(i), "0")
            Next
        End If

    End Sub

    Private Function getNodeText(ByVal xmlNodeRoot As XmlNode, ByVal nodeName As String) As String
        Dim xmlN As XmlNode

        If xmlNodeRoot Is Nothing Then Return ""

        xmlN = xmlNodeRoot.SelectSingleNode(nodeName)
        If xmlN Is Nothing Then Return ""

        Return xmlN.InnerText
    End Function

End Class
