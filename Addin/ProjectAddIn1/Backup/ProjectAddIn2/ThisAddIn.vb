Imports System.Windows.Forms
Imports System.IO
Imports System.Xml
Imports System.Net

Public Class ThisAddIn


    Private _commandBar As Office.CommandBar
    Private _submitButton As Office.CommandBarButton
    Private _importButton As Office.CommandBarButton
    Private _exportButton As Office.CommandBarButton
    Private _editButton As Office.CommandBarButton

#Region "属性定义"

    Private _errorCode As Integer = -1
    Private _currentUser As LoginInfo

    'modified by chenyong 2009-07-13
    Private Property ErrorCode() As Integer
        Get
            Return _errorCode
        End Get
        Set(ByVal value As Integer)
            _errorCode = value
        End Set
    End Property

    'modified by chenyong 2009-07-13
    Private Structure LoginInfo
        Friend UserName As String
        Friend PassWord As String
        Friend Domain As String
    End Structure

    'modified by chenyong 2009-07-13
    Private Property CurrentUser() As LoginInfo
        Get
            Return _currentUser
        End Get
        Set(ByVal value As LoginInfo)
            _currentUser = value
        End Set
    End Property

#End Region

    Private Sub ThisAddIn_Startup(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Startup
        Try

            AddToolbar()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "添加工具栏按钮错误", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '添加工具栏
    Private Sub AddToolbar()
        Try
            _commandBar = Application.CommandBars("明源")
        Catch ex As ArgumentException
            ' The toolbar named MysoftBar does not exist, so create it.
        End Try

        If (_commandBar Is Nothing) Then
            ' Add a command bar named ImportBar to the toolbar area.
            Const barPosition As Integer = 1
            Const isTemporary As Boolean = True
            _commandBar = Application.CommandBars.Add("明源", barPosition, , isTemporary)
        End If

        Try
            ' Add a button named ImportProject and an event handler.
            ' It is not necessary to use TryCast() here.
            _submitButton = _commandBar.Controls.Add(Type:=Office.MsoControlType.msoControlButton)
            _submitButton.Style = Office.MsoButtonStyle.msoButtonCaption
            _submitButton.Caption = "提交1"
            _submitButton.Tag = "Submit"
            _submitButton.TooltipText = "提交任务到服务器。"
            AddHandler _submitButton.Click, AddressOf SubmitButtonClick

            _importButton = _commandBar.Controls.Add(Type:=Office.MsoControlType.msoControlButton)
            _importButton.Style = Office.MsoButtonStyle.msoButtonCaption
            _importButton.Caption = "导入1"
            _importButton.Tag = "Import"
            _importButton.TooltipText = "导入任务。"
            AddHandler _importButton.Click, AddressOf ImportButtonClick

            _exportButton = _commandBar.Controls.Add(Type:=Office.MsoControlType.msoControlButton)
            _exportButton.Style = Office.MsoButtonStyle.msoButtonCaption
            _exportButton.Caption = "导出1"
            _exportButton.Tag = "Export"
            _exportButton.TooltipText = "导出任务。"
            AddHandler _exportButton.Click, AddressOf ExportButtonClick

            _editButton = _commandBar.Controls.Add(Type:=Office.MsoControlType.msoControlButton)
            _editButton.Style = Office.MsoButtonStyle.msoButtonCaption
            _editButton.Caption = "编制工作项1"
            _editButton.Tag = "EditItem"
            _editButton.TooltipText = "编制工作项。"
            AddHandler _editButton.Click, AddressOf EditItemButtonClick

            _commandBar.Visible = True

        Catch ex As Exception
            MessageBox.Show(ex.Message, "添加工具栏按钮错误", _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub SubmitButtonClick(ByVal ctrl As Office.CommandBarButton, ByRef cancel As Boolean)
        MessageBox.Show("SubmitButtonClick")

        Dim strDataFile, strInfoFile As String
        Dim strTempFile As String

        strDataFile = Application.ActiveProject.Author       '临时借用字段

        'modified by chenyong 2011-05-10 解决更改项目信息后,Project软件截断Author属性值,导致提交提前退出的问题.
        If Not Path.HasExtension(strDataFile) Then
            strDataFile = String.Format("{0}\{1}.xml", Path.GetDirectoryName(strDataFile), Application.ActiveProject.Name)
            strInfoFile = String.Format("{0}\{1}.info", Path.GetDirectoryName(strDataFile), Application.ActiveProject.Name)
        Else
            strInfoFile = Path.GetDirectoryName(strDataFile) & "\" & Path.GetFileNameWithoutExtension(strDataFile) + ".info"
        End If

        If Not File.Exists(strDataFile) Then Exit Sub
        If Not File.Exists(strInfoFile) Then Exit Sub

        '保存文件
        File.Delete(strDataFile)        '避免弹出覆盖提示
        Application.FileSaveAs(Name:=strDataFile, FormatID:="MSProject.XML")

        '保存临时文件
        strTempFile = Path.GetDirectoryName(strDataFile) & "\" & Path.GetFileNameWithoutExtension(strDataFile) + ".tmp"
        Application.ActiveProject.Author = ""
        If File.Exists(strTempFile) Then File.Delete(strTempFile)
        Application.FileSaveAs(Name:=strTempFile, FormatID:="MSProject.XML")
        Application.ActiveProject.Author = strDataFile

        '上传文件
        UploadProject(strTempFile, strInfoFile)

        '删除临时文件
        File.Delete(strTempFile)
    End Sub

    Private Sub ImportButtonClick(ByVal ctrl As Office.CommandBarButton, ByRef cancel As Boolean)
        MessageBox.Show("ImportButtonClick")


        '先检查是否为在线编辑的 Project 项目

        If Application.ActiveProject Is Nothing Then Exit Sub

        Dim strDataFile, strInfoFile As String

        strDataFile = Application.ActiveProject.Author
        If Not File.Exists(strDataFile) Then Exit Sub

        strInfoFile = Path.GetDirectoryName(strDataFile) & "\" & Path.GetFileNameWithoutExtension(strDataFile) + ".info"
        If Not File.Exists(strInfoFile) Then Exit Sub

        '
        Dim ofd As New Windows.Forms.OpenFileDialog
        ofd.Filter = "项目 (*.mpp)|*.mpp|XML 格式(*.xml)|*.xml"
        ofd.FilterIndex = 1
        If Not ofd.ShowDialog = DialogResult.OK Then Exit Sub

        If String.Compare(Path.GetExtension(ofd.FileName), ".mpp", True) <> 0 And _
           String.Compare(Path.GetExtension(ofd.FileName), ".xml", True) <> 0 Then
            MsgBox("文件格式不正确！", MsgBoxStyle.OkOnly, "错误")
            Exit Sub
        End If

        '

        '保存文件
        If File.Exists(strDataFile) Then File.Delete(strDataFile)
        Application.FileSaveAs(Name:=strDataFile, FormatID:="MSProject.XML")
        'Application.FileCloseEx(MSProject.PjSaveType.pjDoNotSave, False, False)
        Application.FileClose(MSProject.PjSaveType.pjDoNotSave, False)

        Try
            If String.Compare(Path.GetExtension(ofd.FileName), ".mpp", True) = 0 Then
                'Application.FileOpenEx(Name:=ofd.FileName, ReadOnly:=False, FormatID:="MSProject.MPP")
                Application.FileOpen(Name:=ofd.FileName, ReadOnly:=False, FormatID:="MSProject.MPP")
            ElseIf String.Compare(Path.GetExtension(ofd.FileName), ".xml", True) = 0 Then
                'Application.FileOpenEx(Name:=ofd.FileName, ReadOnly:=False, Merge:=0, FormatID:="MSProject.XML")
                Application.FileOpen(Name:=ofd.FileName, ReadOnly:=False, Merge:=0, FormatID:="MSProject.XML")
            End If

            Application.ActiveProject.Author = strDataFile
            Application.ActiveProject.Name = Path.GetFileName(strDataFile)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ExportButtonClick(ByVal ctrl As Office.CommandBarButton, ByRef cancel As Boolean)
        MessageBox.Show("ExportButtonClick")


        Dim sfd As New Windows.Forms.SaveFileDialog
        sfd.Filter = "项目 (*.mpp)|*.mpp|XML 格式(*.xml)|*.xml"
        sfd.FilterIndex = 1

        If Not sfd.ShowDialog = DialogResult.OK Then Exit Sub

        Application.ActiveProject.Author = ""
        If String.Compare(Path.GetExtension(sfd.FileName), ".mpp", True) = 0 Then
            Application.FileSaveAs(Name:=sfd.FileName, FormatID:="MSProject.MPP")
        ElseIf String.Compare(Path.GetExtension(sfd.FileName), ".xml", True) = 0 Then
            Application.FileSaveAs(Name:=sfd.FileName, FormatID:="MSProject.XML")
        End If
    End Sub

    Private Sub EditItemButtonClick(ByVal ctrl As Office.CommandBarButton, ByRef cancel As Boolean)
        MessageBox.Show("EditItemButtonClick")

        Try
            Dim fieldName() As String = {"文本25", "文本26", "文本27", "文本28", "文本29", "文本30"}

            For i = 0 To fieldName.Length - 1
                Dim isColumnExist As Boolean = False
                Try
                    isColumnExist = Application.SelectTaskField(0, fieldName(i))
                Catch ex As Exception
                End Try

                If Not isColumnExist Then
                    Application.TableEdit(Name:="项", TaskTable:=True, NewName:="", FieldName:="", NewFieldName:=fieldName(i), Title:="", Width:=10, Align:=2, ShowInMenu:=True, LockFirstColumn:=True, DateFormat:=255, RowHeight:=1, ColumnPosition:=-1, AlignTitle:=1)
                    Application.TableApply(Name:="项")
                End If

            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message, "编制工作项错误", _
                         MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '上传计划
    Private Sub UploadProject(ByVal datafile As String, ByVal infofile As String)
        If Not File.Exists(datafile) Then Return
        If Not File.Exists(infofile) Then Return

        Dim xmlInfo As New XmlDocument
        Dim xmlNUrl As XmlNode
        Dim strUrl, strReturn As String
        Dim xmlNData As XmlNode

        Try
            xmlInfo.Load(infofile)
        Catch ex As Exception
            Return
        End Try

        xmlNUrl = xmlInfo.DocumentElement.SelectSingleNode("Url")
        If xmlNUrl Is Nothing Then Return

        strUrl = xmlNUrl.InnerText
        xmlInfo.DocumentElement.RemoveChild(xmlNUrl)

        xmlNData = xmlInfo.CreateElement("Data")
        xmlNData.InnerText = GetFileContent(datafile)
        xmlInfo.DocumentElement.AppendChild(xmlNData)

        Dim s As String
        Dim bytes() As Byte
        bytes = System.Text.Encoding.Default.GetBytes(xmlInfo.OuterXml)
        s = Convert.ToBase64String(bytes)

        strReturn = PostURL(strUrl, s)

        If strReturn <> "" Then
            Dim strInfoList As String() = strReturn.Split("|")
            If strInfoList.Length = 2 Then
                If strInfoList(0).ToUpper() = "OPENURL" Then
                    '打开计划系统中的错误提示页面 Modified by xq at:2010.12.03
                    OpenUrlInIE(strInfoList(1))
                Else
                    If strInfoList(0).ToUpper() = "NO" Then
                        MsgBox(strInfoList(1), MsgBoxStyle.Critical, "错误")
                    Else
                        MsgBox(strInfoList(1), MsgBoxStyle.Information, "信息")
                    End If
                End If
            Else
                MsgBox("提交失败，请关闭重试！", MsgBoxStyle.Critical, "错误")
            End If
        Else
            MsgBox("提交失败，请关闭重试！", MsgBoxStyle.Critical, "错误")
        End If

        Return
    End Sub

    '将文件内容读为 Base64 编码
    Private Function GetFileContent(ByVal filename As String) As String
        If Not File.Exists(filename) Then Return ""

        Dim fs As New FileStream(filename, FileMode.Open, FileAccess.Read)
        If fs.Length = 0 Then Return ""

        Dim bytes() As Byte
        ReDim bytes(fs.Length - 1)
        fs.Read(bytes, 0, fs.Length)
        fs.Close()

        Return Convert.ToBase64String(bytes)
    End Function

    Private Function PostURL(ByVal url As String, ByVal data As String) As String
        'Return PostURL(url, data, Nothing)

        Dim result As String = PostURL(url, data, Nothing)

        'modified by chenyong 2009-07-13
        If HttpStatusCode.Unauthorized = ErrorCode AndAlso String.IsNullOrEmpty(result) Then
            ErrorCode = -1

            '取默认证书登陆
            result = PostURL(url, data, Nothing, CredentialCache.DefaultCredentials)
            If HttpStatusCode.Unauthorized = ErrorCode Then '权限错误
                ErrorCode = -1
                If String.IsNullOrEmpty(CurrentUser.UserName) Then
                    '取用户信息

                    Dim fm As New UserInfoDialog
                    fm.ShowDialog()

                    If fm.DialogResult = Windows.Forms.DialogResult.OK Then
                        Dim lg As LoginInfo = New LoginInfo()
                        lg.UserName = fm.txtUserName.Text.Trim()
                        lg.PassWord = fm.txtPassWord.Text.Trim()
                        lg.Domain = fm.txtDomain.Text.Trim()

                        CurrentUser = lg
                    End If
                End If

                If Not String.IsNullOrEmpty(CurrentUser.UserName) Then
                    '系统当前用户证书未通过验证，根据用户输入的信息生成证书登陆
                    Dim ntc As NetworkCredential = New NetworkCredential()
                    ntc.UserName = CurrentUser.UserName
                    ntc.Password = CurrentUser.PassWord
                    ntc.Domain = CurrentUser.Domain

                    result = PostURL(url, data, Nothing, ntc)

                End If
            End If
        End If
        Return result
    End Function

    'modified by chenyong 2009-07-13
    Private Function PostURL(ByVal url As String, ByVal data As String, ByVal cookies As CookieContainer, ByVal credentials As ICredentials) As String
        Dim httpRequest As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)

        If Not (cookies Is Nothing) Then
            httpRequest.CookieContainer = cookies
        End If

        ' 指定 Post 方式
        httpRequest.ContentType = "application/x-www-form-urlencoded"
        httpRequest.Method = "POST"

        '证书 modified by chenyong 2009-07-13
        If Not credentials Is Nothing Then
            httpRequest.Credentials = credentials
        End If

        ' 往服务器发送数据
        WriteRequest(httpRequest, data)

        Return GetHtml(httpRequest, cookies, credentials)
    End Function

    Private Function PostURL(ByVal url As String, ByVal data As String, ByVal cookies As CookieContainer) As String
        Dim request As WebRequest = WebRequest.Create(url)
        Dim httpRequest As HttpWebRequest = CType(request, HttpWebRequest)
        If Not (cookies Is Nothing) Then
            httpRequest.CookieContainer = cookies
        End If

        ' 指定 Post 方式
        httpRequest.Method = "POST"
        httpRequest.ContentType = "application/x-www-form-urlencoded"

        ' 往服务器发送数据
        WriteRequest(httpRequest, data)

        ' 返回回应内容
        'Return GetHtml(httpRequest)

        'modified by chenyong 2009-07-13
        Dim result As String = GetHtml(httpRequest)

        If HttpStatusCode.Unauthorized = ErrorCode AndAlso String.IsNullOrEmpty(result) Then
            ErrorCode = -1

            '取默认证书登陆
            result = PostURL(url, data, Nothing, CredentialCache.DefaultCredentials)
            If HttpStatusCode.Unauthorized = ErrorCode Then '权限错误
                ErrorCode = -1
                If String.IsNullOrEmpty(CurrentUser.UserName) Then
                    '取用户信息

                    'Me.Hide()
                    Dim fm As New UserInfoDialog
                    fm.ShowDialog()

                    If fm.DialogResult = Windows.Forms.DialogResult.OK Then
                        Dim lg As LoginInfo = New LoginInfo()
                        lg.UserName = fm.txtUserName.Text.Trim()
                        lg.PassWord = fm.txtPassWord.Text.Trim()
                        lg.Domain = fm.txtDomain.Text.Trim()

                        CurrentUser = lg
                    End If
                End If

                If Not String.IsNullOrEmpty(CurrentUser.UserName) Then
                    '系统当前用户证书未通过验证，根据用户输入的信息生成证书登陆
                    Dim ntc As NetworkCredential = New NetworkCredential()
                    ntc.UserName = CurrentUser.UserName
                    ntc.Password = CurrentUser.PassWord
                    ntc.Domain = CurrentUser.Domain

                    result = PostURL(url, data, cookies, ntc)
                End If
            End If
        End If
        Return result
    End Function

    Private Sub WriteRequest(ByVal request As WebRequest, ByVal data As String)
        Dim bytes() As Byte
        bytes = System.Text.Encoding.ASCII.GetBytes(data)
        request.ContentLength = bytes.Length

        If bytes.Length > 0 Then
            Dim outputStream As Stream = request.GetRequestStream()
            outputStream.Write(bytes, 0, bytes.Length)
            outputStream.Close()
        End If
    End Sub

    Private Function GetHtml(ByVal request As WebRequest) As String
        Return GetHtml(request, Nothing)
    End Function

    Private Function GetHtml(ByVal request As WebRequest, ByVal cookies As CookieContainer) As String
        Dim result As String = ""

        Try
            Dim httpRequest As HttpWebRequest = CType(request, HttpWebRequest)
            If Not (Cookies Is Nothing) Then
                httpRequest.CookieContainer = Cookies
            End If
            Dim response As WebResponse = httpRequest.GetResponse()
            Dim responseStream As Stream = response.GetResponseStream()
            Dim sr As StreamReader = New StreamReader(responseStream, System.Text.Encoding.Default)
            result = sr.ReadToEnd()

            sr.Close()
            responseStream.Close()
            response.Close()
        Catch ex As WebException
            'modified by chenyong 2009-07-13
            If Not ex.Response Is Nothing AndAlso HttpStatusCode.Unauthorized = DirectCast(ex.Response, HttpWebResponse).StatusCode Then
                ErrorCode = HttpStatusCode.Unauthorized
            End If
            Return ""
        End Try

        Return result
    End Function

    'modified by chenyong 2009-07-13
    Private Function GetHtml(ByVal request As WebRequest, ByVal cookies As CookieContainer, ByVal credential As ICredentials) As String
        Dim result As String = ""

        Try
            Dim httpRequest As HttpWebRequest = CType(request, HttpWebRequest)
            If Not (cookies Is Nothing) Then
                httpRequest.CookieContainer = cookies
            End If

            'modified by chenyong 2009-07-13
            If Not credential Is Nothing Then
                httpRequest.Credentials = credential
            End If

            Dim response As WebResponse = httpRequest.GetResponse()
            Dim responseStream As Stream = response.GetResponseStream()
            Dim sr As StreamReader = New StreamReader(responseStream, System.Text.Encoding.Default)
            result = sr.ReadToEnd()

            sr.Close()
            responseStream.Close()
            response.Close()
        Catch ex As WebException
            'modified by chenyong 2009-07-13
            If Not ex.Response Is Nothing AndAlso HttpStatusCode.Unauthorized = CType(ex.Response, HttpWebResponse).StatusCode Then
                ErrorCode = HttpStatusCode.Unauthorized
            End If
            Return ""
        End Try

        Return result
    End Function

    '在IE中打开通过参数传入的URL地址
    Private Sub OpenUrlInIE(ByVal strUrl As String)

        If strUrl Is String.Empty Then Return

        Dim wbCtl As WebBrowser = New WebBrowser()
        '给WeBrowser控件的DocumentCompleted事件上绑定一个定义过程
        AddHandler wbCtl.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf wbCtlDocumentCompleted)
        '初始化WebBrowser控件，触发绑定的过程打开IE窗口
        wbCtl.Visible = False

        Try
            wbCtl.Url = New Uri(strUrl)
        Catch ex As Exception
            Return
        End Try

        Return
    End Sub

    'WebBrowser控件初始化时触发的过程，以打开IE窗口
    Private Sub wbCtlDocumentCompleted(ByVal sender As Object, ByVal e As WebBrowserDocumentCompletedEventArgs)
        Dim wbCtl As WebBrowser = CType(sender, WebBrowser)
        Dim docCtl As HtmlDocument = wbCtl.Document

        Dim strUrl As String = wbCtl.Url.ToString()

        '增加一个参数，告诉页面本次是真正的调用，需要加载数据等（因为用WebBrowser的这种方式打开窗口时，页面会被调用两次）
        strUrl &= IIf(strUrl.IndexOf("?") > 0, "&", "?") & "IsLoading=1"

        docCtl.Window.Open(strUrl, "_blank", "width=800, height=552,toolbar =no, menubar=no, scrollbars=no, resizable=no, location=no, status=no", True)

        wbCtl.Dispose()
    End Sub

    Private Sub ThisAddIn_Shutdown(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Shutdown

    End Sub

End Class
