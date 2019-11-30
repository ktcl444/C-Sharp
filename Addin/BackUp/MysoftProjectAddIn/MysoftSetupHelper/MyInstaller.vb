Imports System.ComponentModel
Imports System.Configuration.Install
Imports System.IO
Imports Microsoft.Win32
Imports System.Runtime.InteropServices

Public Class MyInstaller

    Public Sub New()
        MyBase.New()

        '组件设计器需要此调用。
        InitializeComponent()

        '调用 InitializeComponent 后添加初始化代码

    End Sub

    Private _installDir As String = String.Empty

    ''' <summary>
    ''' 安装目录地址
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property InstallDir() As String
        Get
            '获取当前插件的安装目录
            If String.IsNullOrEmpty(_InstallDir) Then
                _InstallDir = GetInstallDir()
            End If

            Return _InstallDir
        End Get
    End Property

    ''' <summary>
    ''' 取安装目录地址
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetInstallDir() As String
        Dim strDir As String
        If Not String.IsNullOrEmpty(Context.Parameters("assemblypath")) Then
            strDir = Context.Parameters("assemblypath")
            strDir = strDir.Substring(0, strDir.LastIndexOf("\") + 1)
        Else
            strDir = String.Empty
        End If

        Return strDir
    End Function

    Protected Overrides Sub OnBeforeInstall(ByVal savedState As System.Collections.IDictionary)
        'System.Diagnostics.Debugger.Launch()
        Dim bVstorExists As Boolean = False
        Dim bProjectExits As Boolean = False
        Dim strMsg As String = String.Empty

        bVstorExists = VstorExists()        '判断 Visual Studio 2005 Tools for Office Second Edition Runtime 是否已经安装  
        bProjectExits = ProjectExists()     '判断 Office Project 是否已经安装  

        If bVstorExists AndAlso bProjectExits Then
            MyBase.OnBeforeInstall(savedState)
        Else
            If Not bVstorExists AndAlso Not bProjectExits Then
                strMsg = "必须安装以下软件：""Office Project""、""Visual Studio 2005 Tools for Office Second Edition Runtime""。"
            Else
                If Not bVstorExists Then
                    strMsg = "必须安装""Visual Studio 2005 Tools for Office Second Edition Runtime""。"
                End If

                If Not bProjectExits Then
                    strMsg = "必须安装""Office Project""。"
                End If
            End If

            Throw New Exception(strMsg)
        End If
    End Sub

    Protected Overrides Sub OnAfterInstall(ByVal savedState As System.Collections.IDictionary)
        ' System.Diagnostics.Debugger.Launch()
        Dim runtimeDir As String = RuntimeEnvironment.GetRuntimeDirectory()
        '2.0
        Dim caspolAppPath As String = Path.Combine(runtimeDir, "CasPol.exe")
        '4.0
        Dim caspolV4AppPath As String = String.Empty
        Dim parentDir As DirectoryInfo = New DirectoryInfo(runtimeDir)
        For Each v4Dir As DirectoryInfo In parentDir.Parent.GetDirectories("v4*")
            Dim tmp As String = Path.Combine(v4Dir.FullName, "CasPol.exe")
            If File.Exists(tmp) Then
                caspolV4AppPath = tmp
            End If
        Next
        '获取当前插件的安装目录

        '创建caspol执行脚本文件到安装目录
        Dim renew As Boolean = True
        Dim caspolScriptPath As String = Path.Combine(InstallDir, "caspol.bat")
        If File.Exists(caspolScriptPath) Then
            '如果已存在，则删除，以保证最新。如果删除失败，则不在创建。
            Try
                File.Delete(caspolScriptPath)
            Catch ex As Exception
                renew = False
            End Try
        End If
        If renew Then
            ' throw e
            Dim scriptWriter As New StreamWriter(caspolScriptPath, False, System.Text.Encoding.Default)
            scriptWriter.WriteLine(caspolAppPath & " -polchgprompt off")
            scriptWriter.WriteLine(caspolAppPath & " -u -ag All_Code -url """ & Path.Combine(InstallDir, "MysoftProjectAddIn.dll") & """ FullTrust -n ""MysoftProjectAddIn""")
            scriptWriter.WriteLine(caspolAppPath & " -polchgprompt on")
            '4.0
            If Not String.IsNullOrEmpty(caspolV4AppPath) Then
                scriptWriter.WriteLine(caspolV4AppPath & " -polchgprompt off")
                scriptWriter.WriteLine(caspolV4AppPath & " -u -ag All_Code -url """ & Path.Combine(InstallDir, "MysoftProjectAddIn.dll") & """ FullTrust -n ""MysoftProjectAddIn""")
                scriptWriter.WriteLine(caspolV4AppPath & " -polchgprompt on")
            End If
            scriptWriter.Flush()
            scriptWriter.Close()
        End If

        ExecuteDosCommand(caspolAppPath & " -polchgprompt off", 0)
        ExecuteDosCommand(caspolAppPath & " -u -ag All_Code -url """ & Path.Combine(InstallDir, "MysoftProjectAddIn.dll") & """ FullTrust -n ""MysoftProjectAddIn""", 0)
        ExecuteDosCommand(caspolAppPath & " -polchgprompt on", 0)
        '4.0
        If Not String.IsNullOrEmpty(caspolV4AppPath.Length) Then
            ExecuteDosCommand(caspolV4AppPath & " -polchgprompt off", 0)
            ExecuteDosCommand(caspolV4AppPath & " -u -ag All_Code -url """ & Path.Combine(InstallDir, "MysoftProjectAddIn.dll") & """ FullTrust -n ""MysoftProjectAddIn""", 0)
            ExecuteDosCommand(caspolV4AppPath & " -polchgprompt on", 0)
        End If

        '
        MyBase.OnAfterInstall(savedState)


    End Sub

    Protected Overrides Sub OnCommitting(ByVal savedState As System.Collections.IDictionary)
        'System.Diagnostics.Debugger.Launch()
        If VstorExists() Then
            '判断插件安装是否完整
            Dim addInKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Office\MS Project\Addins\MysoftProjectAddIn", True)
            If Not addInKey Is Nothing Then
                Dim commandLineSafeValue As Object = addInKey.GetValue("CommandLineSafe")
                If commandLineSafeValue Is Nothing Or Not commandLineSafeValue.Equals(1) Then
                    addInKey.SetValue("CommandLineSafe", 1)
                End If
                Dim loadBehaviorValue As Object = addInKey.GetValue("LoadBehavior")
                If loadBehaviorValue Is Nothing Or Not loadBehaviorValue.Equals(3) Then
                    addInKey.SetValue("LoadBehavior", 3)
                End If
            Else
                '修复注册表项
                Dim renew As Boolean = True
                Dim addinFixScriptPath As String = Path.Combine(InstallDir, "addin.reg")
                If File.Exists(addinFixScriptPath) Then
                    '如果已存在，则删除，以保证最新。如果删除失败，则不在创建。
                    Try
                        File.Delete(addinFixScriptPath)
                    Catch ex As Exception
                        renew = False
                    End Try
                End If
                If renew Then
                    ' throw e
                    Dim scriptWriter As New StreamWriter(addinFixScriptPath, False, System.Text.Encoding.Default)
                    scriptWriter.WriteLine("Windows Registry Editor Version 5.00")
                    scriptWriter.WriteLine()
                    scriptWriter.WriteLine("[HKEY_CURRENT_USER\Software\Microsoft\Office\MS Project\Addins\MysoftProjectAddIn]")
                    scriptWriter.WriteLine("""CommandLineSafe""=dword:00000001")
                    scriptWriter.WriteLine("""Description""=""MysoftProjectAddIn -- an addin created with VSTO technology""")
                    scriptWriter.WriteLine("""FriendlyName""=""MysoftProjectAddIn""")
                    scriptWriter.WriteLine("""Manifest""=""" & Path.Combine(InstallDir, "MysoftProjectAddIn.dll.manifest").Replace("\", "\\") & """")
                    scriptWriter.WriteLine("""LoadBehavior""=dword:00000003")
                    scriptWriter.Flush()
                    scriptWriter.Close()
                End If

                'regedit  /s <filename.reg>
                '执行脚本
                ExecuteDosCommand("regedit /s """ & addinFixScriptPath & """", 0)
            End If
            MyBase.OnCommitting(savedState)
        Else
            Throw New Exception("必须安装""Visual Studio 2005 Tools for Office Second Edition Runtime""。")
        End If
    End Sub

    Private Sub MyInstaller_AfterUninstall(ByVal sender As Object, ByVal e As System.Configuration.Install.InstallEventArgs) Handles Me.AfterUninstall
        ExecuteDosCommand("del """ & Path.Combine(InstallDir, "*.bat") & """ -y ", 0)
        ExecuteDosCommand("del """ & Path.Combine(InstallDir, "*.InstallState") & """ -y ", 0)
    End Sub

    ''' <summary>
    ''' 判断 Visual Studio 2005 Tools for Office Second Edition Runtime 是否已经安装
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function VstorExists() As Boolean
        '获取OS类型
        Dim is64Bit As Boolean = (IntPtr.Size > 4)
        Dim vstorInstalled As Boolean = False
        Try
            Dim vstorRegKey As RegistryKey = Nothing
            Dim vstorRegKeyParent As RegistryKey = Nothing
            If is64Bit Then
                vstorRegKeyParent = Registry.LocalMachine.OpenSubKey("SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall", True)
                vstorRegKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Visual Studio 2005 Tools for Office Runtime")
            Else
                vstorRegKeyParent = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", True)
                vstorRegKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Visual Studio 2005 Tools for Office Runtime")
            End If
            If vstorRegKey Is Nothing Then
                vstorInstalled = False
            Else
                Dim uninstallPathValue As String = vstorRegKey.GetValue("UninstallPath")
                If Not uninstallPathValue Is Nothing Then
                    vstorInstalled = File.Exists(uninstallPathValue)
                End If
            End If
            '如果注册表记录存在，而vstor程序不存在，则删除注册表，重新安装
            If Not vstorInstalled And Not vstorRegKeyParent Is Nothing Then
                vstorRegKeyParent.DeleteSubKey("Microsoft Visual Studio 2005 Tools for Office Runtime", False)
            End If
        Catch ex As Exception
            vstorInstalled = False
        End Try
        Return vstorInstalled
    End Function

    ''' <summary>
    ''' 判断 Office Project 是否已经安装
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProjectExists() As Boolean
        '获取OS类型
        Dim is64Bit As Boolean = (IntPtr.Size > 4)
        Dim projInstalled As Boolean = False
        Try
            Dim projRegKey As RegistryKey = Nothing
            If is64Bit Then
                projRegKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\App Paths\WINPROJ.EXE")
            Else
                projRegKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WINPROJ.EXE")
            End If
            If projRegKey Is Nothing Then
                projInstalled = False
            Else
                Dim projPathValue As String = projRegKey.GetValue("Path")
                If Not projPathValue Is Nothing Then
                    projInstalled = File.Exists(Path.Combine(projPathValue, "WINPROJ.EXE"))
                End If
            End If
        Catch ex As Exception
            projInstalled = False
        End Try
        Return projInstalled
    End Function

    ''' <summary>
    ''' 执行DOS命令，返回DOS命令的输出
    ''' </summary>
    ''' <param name="dosCommand">dos命令</param>
    ''' <param name="milliseconds">等待命令执行的时间（单位：毫秒），如果设定为0，则无限等待</param>
    ''' <returns>返回输出，如果发生异常，返回空字符串</returns>
    Private Function ExecuteDosCommand(ByVal dosCommand As String, ByVal milliseconds As Integer) As String
        Dim output As String = ""
        '输出字符串
        If dosCommand <> Nothing AndAlso dosCommand <> "" Then
            Dim process As New Process()
            '创建进程对象
            Dim startInfo As New ProcessStartInfo()
            startInfo.FileName = "cmd.exe"
            '设定需要执行的命令
            startInfo.Arguments = "/C " + dosCommand
            '设定参数，其中的“/C”表示执行完命令后马上退出
            startInfo.UseShellExecute = False
            '不使用系统外壳程序启动
            startInfo.RedirectStandardInput = False
            '不重定向输入
            startInfo.RedirectStandardOutput = True
            '重定向输出
            startInfo.CreateNoWindow = True
            '不创建窗口
            process.StartInfo = startInfo
            Try
                If process.Start() Then
                    '开始进程
                    If milliseconds = 0 Then
                        process.WaitForExit()
                    Else
                        '这里无限等待进程结束
                        process.WaitForExit(milliseconds)
                    End If
                    '这里等待进程结束，等待时间为指定的毫秒
                    '读取进程的输出
                    output = process.StandardOutput.ReadToEnd()
                End If
            Catch
            Finally
                If process IsNot Nothing Then
                    process.Close()
                End If
            End Try
        End If
        Return output
    End Function

End Class
