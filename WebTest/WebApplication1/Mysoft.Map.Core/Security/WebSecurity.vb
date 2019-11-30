Imports System.Web
Imports System.Xml
Imports System.Configuration
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.IO
Imports Mysoft.Map.Data

Namespace Security

    ''' <summary>
    ''' Web 安全相关(SQL注入、文件上传) 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class WebSecurity

        Public Shared Sub CheckUpFile()
            Dim context As HttpContext = HttpContext.Current
            Dim request As HttpRequest = context.Request

            Dim contentType As String = request.ContentType

            ' 当前请求参数的类型
            If (Not contentType.StartsWith("multipart/form-data", StringComparison.CurrentCultureIgnoreCase)) Then Return

            ' HttpContext 显式实现的一个接口
            Dim provider As IServiceProvider = HttpContext.Current
            Dim worker As HttpWorkerRequest = CType(provider.GetService(GetType(HttpWorkerRequest)), HttpWorkerRequest)

            ' 可能是上传文件，判断是否有内容
            ' 如果没有内容，直接返回
            If Not worker.HasEntityBody() Then Return

            ' 允许上传的文件
            Dim strUpfileType As String = ConfigurationManager.AppSettings("UpFileType")
            If String.IsNullOrEmpty(strUpfileType) Then
                context.Response.Redirect("/ErrPage.aspx?errmessage=对不起，不支持上传此类扩展名的附件。", True)
                Return
            End If

            strUpfileType = "," + strUpfileType + ","

            ' 检查文件名
            Dim strFileName, strFileExt, strFileType As String
            For i As Integer = 0 To request.Files.Count - 1
                strFileName = request.Files(i).FileName
                strFileExt = Path.GetExtension(strFileName).ToLower()

                ' 堵 IIS 本身的漏洞
                If strFileName.IndexOf(";", StringComparison.CurrentCultureIgnoreCase) >= 0 Then
                    ' 记录上传信息
                    Dim strMessage As String
                    strMessage = String.Format("{0} 用户尝试上传文件 {1}，由于属于非允许的上传类型，上传被中止。", _
                                               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), strFileName)
                    LogRiskInfo(context, "文件上传风险", strMessage)

                    context.Response.Redirect("/ErrPage.aspx?errmessage=对不起，不支持上传此类扩展名的附件。", True)
                    Return
                End If

                ' 无扩展名，可能没有选择文件上传
                If String.IsNullOrEmpty(strFileExt) Then Continue For

                ' 截去“.”
                strFileExt = strFileExt.Substring(1)
                strFileType = "," + strFileExt + ","

                If strUpfileType.IndexOf(strFileType, StringComparison.CurrentCultureIgnoreCase) < 0 Then
                    ' 记录上传信息
                    Dim strMessage As String
                    strMessage = String.Format("{0} 用户尝试上传文件 {1}，由于属于非允许的上传类型，上传被中止。", _
                                               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), strFileName)
                    LogRiskInfo(context, "文件上传风险", strMessage)

                    context.Response.Redirect("/ErrPage.aspx?errmessage=对不起，不支持上传此类扩展名的附件。", True)
                    Return
                End If
            Next
        End Sub

        Public Shared Sub CheckSession()
            ' 是否启用页面 Session 检查
            Dim blnEnableCheck As Boolean
            If Not Boolean.TryParse(ConfigurationManager.AppSettings("CheckSession"), blnEnableCheck) Then blnEnableCheck = False
            If Not blnEnableCheck Then Return

            Dim context As HttpContext = HttpContext.Current
            Dim strUrl As String = context.Request.Path

            ' 只在访问 .aspx 页面时检查 Session
            If Not strUrl.EndsWith(".aspx", StringComparison.CurrentCultureIgnoreCase) Then Return

            ' 检查 Session
            If CheckLoginBySession() Then Return


            ' 取 web.config 中的配置信息(PageWithoutCheckSession)
            Dim strPages As String = ConfigurationManager.AppSettings("PageWithoutCheckSession")
            ' 默认登录页面可以访问
            If String.IsNullOrEmpty(strPages) Then strPages = "/Default.aspx,/Default_Login.aspx,/ErrPage.aspx"

            ' 检查至根目录的路径，多个路径用“,”分隔
            ' 如果是文件，配置 /xxx.aspx，如果是目录，配置 /xxx/，注意带上后面的“/”。示例：
            '  <add key="PageWithoutCheckSession" value="/Default.aspx,/xxx/" />
            Dim arrPages() As String = strPages.Split(",")
            Dim blnCheck As Boolean = True
            For Each p As String In arrPages
                If p.EndsWith("/") Then
                    ' 排除目录
                    If strUrl.StartsWith(p, StringComparison.CurrentCultureIgnoreCase) Then
                        blnCheck = False
                        Exit For
                    End If
                Else
                    ' 排除单个文件
                    If String.Compare(strUrl, p, True) = 0 Then
                        blnCheck = False
                        Exit For
                    End If
                End If
            Next
            If Not blnCheck Then Return

            ' 重定向到登录页面
            context.Response.Redirect(String.Format("/Default.aspx?Page={0}", HttpUtility.UrlEncode(context.Request.RawUrl)), True)
        End Sub

        Public Shared Sub CheckSQLRejection()
            Dim context As HttpContext = HttpContext.Current
            Dim strUrl As String = context.Request.Path

            ' 只检查对 .aspx 的访问
            If Not strUrl.EndsWith(".aspx", StringComparison.CurrentCultureIgnoreCase) Then Return

            ' 检查 Session 是否存在，如果不存在，可能是调用时机不对，请在 Application.AcquireRequestState 事件中调用
            If context.Session Is Nothing Then Return

            ' 检查高风险 SQL 关键字，如果检测到风险，直接转到错误页面
            If Not CheckHighRiskSQLKeywords(context) Then
                Dim _message As String
                _message = "访问的页面中，参数部分出现了高风险SQL关键字，访问被中止。" & vbCrLf
                _message &= GetRequestData(context)
                LogRiskInfo(context, "SQL注入风险", _message)

                ' 中止访问，页面跳转
                context.Response.Redirect("/ErrPage.aspx", True)
            End If

            ' 检查注入扫描工具 SQL 关键字，如果没有检测到风险，直接跳过
            If CheckScanToolSQLKeywords(context) Then Return

            ' Session信息
            Dim strSessionID As String = context.Session.SessionID
            Dim strUserGUID As String = context.Session("UserGUID")
            If String.IsNullOrEmpty(strUserGUID) Then strUserGUID = "00000000-0000-0000-0000-000000000000"

            ' 访问 Cache
            Dim strCacheKey As String = String.Format("VisitInfo_{0}", strSessionID)
            Dim intCountSeconds As Integer = 60     ' 计数周期(秒)
            Dim intCountLimit As Integer            ' 计数次数上限
            If Not Integer.TryParse(ConfigurationManager.AppSettings("ToolScanUpperLimit"), intCountLimit) Then intCountLimit = 60

            '
            Dim visitInfo As VisitInfo
            If context.Cache(strCacheKey) Is Nothing Then
                visitInfo = New VisitInfo()
                visitInfo.SessionID = strSessionID
                visitInfo.UserGUID = strUserGUID
                visitInfo.VisitCount = 1
                visitInfo.RemoteAddr = context.Request.ServerVariables("REMOTE_ADDR")
                visitInfo.StartTime = Date.Now

                ' 未生成缓存或缓存过期，则插入一分钟绝对时间过期的缓存
                context.Cache.Insert(strCacheKey, visitInfo, Nothing, DateTime.Now.AddSeconds(intCountSeconds), System.Web.Caching.Cache.NoSlidingExpiration)
                Return
            Else
                ' 已有缓存，如果访问次数小于 30，缓存计数加一
                visitInfo = context.Cache(strCacheKey)
                Dim intVisitCount As Integer = visitInfo.VisitCount
                If intVisitCount < intCountLimit Then
                    visitInfo.VisitCount = intVisitCount + 1
                    Return
                End If
            End If

            ' 单位时间内访问 .aspx 页面次数超出，记录异常信息，禁用当前登录用户，清除用户 Session
            Dim strMessage As String
            strMessage = String.Format("同一会话在短时间内({0}～{1})，参数中出现注入扫描工具SQL关键字的次数超限({2}秒内限制{3}次)，访问被中止。", _
                                       visitInfo.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), _
                                       Date.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), _
                                       intCountSeconds, intCountLimit)
            '同时记录 Post 信息和 Cookie 信息
            strMessage &= vbCrLf & GetRequestData(context)

            LogRiskInfo(context, "SQL注入风险", strMessage)

            ' 禁用当前用户
            Dim strSql As String = String.Format("UPDATE myUser SET IsDisabeld=1 WHERE UserGUID='{0}'", strUserGUID)
            MyDB.ExecSQL(strSql)

            ' 清除 Session
            context.Session.Clear()

            ' 中止访问，转到错误页面
            context.Response.Redirect("/ErrPage.aspx", True)
        End Sub

        ' 检查高风险 SQL 关键字，结果返回 True 表示未检测到风险(包含未启用情况)
        Private Shared Function CheckHighRiskSQLKeywords(ByVal context As HttpContext) As Boolean
            Dim blnEnableCheck As Boolean
            If Not Boolean.TryParse(ConfigurationManager.AppSettings("CheckSQLKeywords"), blnEnableCheck) Then blnEnableCheck = False

            If Not blnEnableCheck Then Return True

            Return CheckSQLKeywords(context, "HighRiskSQLKeywords")
        End Function

        ' 检查注入扫描工具 SQL 关键字，结果返回 True 表示未检测到风险(包含未启用情况)
        Private Shared Function CheckScanToolSQLKeywords(ByVal context As HttpContext) As Boolean
            Dim blnEnableCheck As Boolean
            If Not Boolean.TryParse(ConfigurationManager.AppSettings("CheckSQLScanTool"), blnEnableCheck) Then blnEnableCheck = False

            If Not blnEnableCheck Then Return True

            Return CheckSQLKeywords(context, "ScanToolSQLKeywords")
        End Function


        Private Shared Function CheckSQLKeywords(ByVal context As HttpContext, ByVal key As String) As Boolean
            Dim strKeywords As String = ConfigurationManager.AppSettings(key)
            If String.IsNullOrEmpty(strKeywords) Then Return True

            Dim arrKeywords() As String = strKeywords.Split(",")

            Dim strPattern As String = ""
            For i As Integer = 0 To arrKeywords.Count - 1
                If Not String.IsNullOrEmpty(arrKeywords(i)) Then
                    If strPattern.Length > 0 Then strPattern &= "|"
                    strPattern &= "\b" & arrKeywords(i).Replace(".", "\.").Replace("*", "\w+?") & "(\s|--|/\*|\(|[[]|'|"")"
                End If
            Next
            If String.IsNullOrEmpty(strPattern) Then Return True

            Dim re As New Regex(strPattern, RegexOptions.Singleline Or RegexOptions.IgnoreCase)

            ' 检查 QueryString
            For i As Integer = 0 To context.Request.QueryString.Count - 1
                If re.IsMatch(context.Request.QueryString(i)) Then Return False
            Next

            ' 检查 Form
            For i As Integer = 0 To context.Request.Form.Count - 1
                If re.IsMatch(context.Request.Form(i)) Then Return False
            Next

            ' 如果不是文件上传，需要检查 InputStream。文件上传，InputStream 中除了文件内容，其它都可以通过 Request.Form 检查到。
            If (Not context.Request.ContentType.StartsWith("multipart/form-data", StringComparison.CurrentCultureIgnoreCase)) Then
                ' 检查 InputStream，需要排除 AppForm 的保存页面 /ApplicationMap.aspx
                If context.Request.InputStream.Length > 0 Then
                    Dim strUrl As String = context.Request.Path
                    Dim arrNoCheckPage() As String = New String() {"/ApplicationMap.aspx"}
                    Dim blnCheckPost As Boolean = True
                    For i As Integer = 0 To arrNoCheckPage.Count - 1
                        If String.Compare(strUrl, arrNoCheckPage(i), True) = 0 Then
                            blnCheckPost = False
                            Exit For
                        End If
                    Next
                    If blnCheckPost Then
                        Dim strPostData As String = ""
                        Dim bytes(context.Request.InputStream.Length - 1) As Byte
                        context.Request.InputStream.Read(bytes, 0, bytes.Length)
                        context.Request.InputStream.Position = 0
                        strPostData = Encoding.UTF8.GetString(bytes)

                        If re.IsMatch(strPostData) Then Return False
                    End If
                End If
            End If

            ' 检查 Cookies
            For i As Integer = 0 To context.Request.Cookies.Count - 1
                If re.IsMatch(context.Request.Cookies(i).Value) Then Return False
            Next

            Return True
        End Function

        Private Shared Function GetRequestData(ByVal context As HttpContext) As String
            ' QueryString 信息已通过 URL 记录，因此这里只提供 Post 和 Cookies 信息
            Dim strData As String = ""

            If context.Request.ContentType.StartsWith("multipart/form-data", StringComparison.CurrentCultureIgnoreCase) Then
                ' 上传文件，不记录文件内容，只记录 Request.Form 信息
                If context.Request.Form.Count > 0 Then
                    strData &= vbCrLf & "Form数据：" & vbCrLf
                    For i As Integer = 0 To context.Request.Form.Count - 1
                        strData &= String.Format("{0} = {1}{2}", context.Request.Form.AllKeys(i), context.Request.Form(i), vbCrLf)
                    Next
                End If
            Else
                ' 非文件上传，记录 InputStream，其中包含 Request.Form 的信息
                If context.Request.InputStream.Length > 0 Then
                    Dim bytes(context.Request.InputStream.Length - 1) As Byte
                    context.Request.InputStream.Read(bytes, 0, bytes.Length)
                    context.Request.InputStream.Position = 0
                    strData &= "POST数据：" & vbCrLf & Encoding.UTF8.GetString(bytes)
                End If

            End If

            If context.Request.Cookies.Count > 0 Then
                strData &= vbCrLf & vbCrLf & "Cookies数据：" & vbCrLf
                For i As Integer = 0 To context.Request.Cookies.Count - 1
                    strData &= String.Format("{0} = {1}{2}", context.Request.Cookies(i).Name, context.Request.Cookies(i).Value, vbCrLf)
                Next
            End If

            Return strData
        End Function

        Private Shared Sub LogRiskInfo(ByVal context As HttpContext, ByVal [type] As String, ByVal [message] As String)
            ' Session信息
            Dim strSessionID As String
            Dim strUserName As String
            If context.Session Is Nothing Then
                strSessionID = ""
                strUserName = ""
            Else
                strSessionID = context.Session.SessionID
                strUserName = context.Session("UserName")
                If String.IsNullOrEmpty(strUserName) Then strUserName = ""
            End If

            ' 数据库中对应字段长度限制 nvarchar(2000)
            If message.Length > 2000 Then message = message.Substring(0, 1997) & "..."

            ' 记录信息
            Dim strSql As String
            strSql = String.Format("INSERT INTO myExceptionLog(LogId,[Date],[Message],ExceptionMassage,[Type],[Source]" & _
                                 ",[Host],Url,[User],[Exception]) " & _
                                 "VALUES(newid(),GETDATE(),N'{0}',N'{1}',N'{2}',N'{2}',N'{3}',N'{4}',N'{5}',N'{0}')", _
                                 [type].Replace("'", "''"), _
                                 [message].Replace("'", "''"), _
                                 "Global.asax.vb", _
                                 Environment.MachineName, _
                                 context.Request.Url.ToString.Replace("'", "''"), _
                                 strUserName.Replace("'", "''"))
            MyDB.ExecSQL(strSql)
        End Sub

        Private Class VisitInfo
            Public SessionID As String
            Public UserGUID As String
            Public VisitCount As Integer
            Public RemoteAddr As String
            Public StartTime As Date

        End Class

        ''通过校验会话状态判断是否登录
        Private Shared Function CheckLoginBySession() As Boolean
            If Not HttpContext.Current.Session Is Nothing AndAlso Not HttpContext.Current.Session("UserGUID") Is Nothing AndAlso HttpContext.Current.Session("UserGUID") <> "" AndAlso Not HttpContext.Current.Session("MySessionState") Is Nothing AndAlso HttpContext.Current.Session("MySessionState") <> "" Then
                Return True
            End If
            Return False
        End Function
    End Class

End Namespace
