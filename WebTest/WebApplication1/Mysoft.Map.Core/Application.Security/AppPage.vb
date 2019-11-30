Imports System.Web
Imports System.Resources
Imports Mysoft.Map.Application.Types
Imports System.Xml
Imports System.Web.UI.Design
Imports Mysoft.Map.WebControls.Designer
Imports Mysoft.Map.Security
Imports Mysoft.Map.Data
Imports Mysoft.Map.Application.UpdatePack

Namespace Application.Security

    Public Class AppPage
        Inherits System.Web.UI.Page

        Protected _formEvent As FormEventId

        Protected Sub New()
            Me._formEvent = FormEventId.None
        End Sub

        Private _AccessUserKind As UserKind = UserKind.All

        Protected Property AccessUserKind() As UserKind
            Get
                Return _AccessUserKind
            End Get
            Set(ByVal value As UserKind)
                _AccessUserKind = value
            End Set
        End Property

        Private Sub CheckAccess()
            Dim sUserKind As String = Session("UserKind")

            If Me.AccessUserKind = UserKind.ERP AndAlso sUserKind = "1" Then
                '页面只允许ERP用户访问时，普通用户不能访问
                Throw New Exception("普通用户无数据权限！")

            ElseIf Me.AccessUserKind = UserKind.Normal AndAlso sUserKind = "0" Then
                '页面只允许普通用户访问时，ERP用户不能访问
                Throw New Exception("ERP用户无数据权限！")
            End If
        End Sub

        'modified by chenyong 2009-09-11 Repeater 在OnInit事件取Session("UserGUID") 取不到原来集成登录在OnLoad中处理
        Protected Overrides Sub OnPreInit(ByVal e As System.EventArgs)
            'modified by kongy 2009-08-19 集成登录修改
            Dim userCode As String = MyLogin.IntegrateLogin()
            Dim isLogin As Boolean = MyLogin.CheckLoginBySession()
            '若已登录，并且登录用户与集成访问的用户相同（或集成访问用户为空）时，只校验功能代码
            If isLogin AndAlso (Session("UserCode") = userCode OrElse String.IsNullOrEmpty(userCode)) Then
                'added by kongy 2009-09-14 根据传递的参数，改变公司会话状态
                SetCompanyByPageParameter()
                CheckFunctionCode()
            Else
                '若未登录，或登录用户与集成访问用户不同，则进行集成登录操作
                CheckIntegrateLogin(userCode)
            End If
            MyBase.OnPreInit(e)
        End Sub

        '通过页面传递参数设置公司会话状态
        Private Sub SetCompanyByPageParameter()
            If Not Request.QueryString("MyCurrentCompany") Is Nothing AndAlso Request.QueryString("MyCurrentCompany").ToString <> "" Then
                MyLogin.SetCompanySession(Request.QueryString("MyCurrentCompany").ToString)
            End If
        End Sub

        '更新当前在线用户信息
        Private Sub UpdateCurrentUser()
            OnlineUser.UpdateCurrentUser()
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            Me.UpdateCurrentUser()
            Me.CheckSecurity()

            MyBase.OnLoad(e)

            Me.ConfigurePage()
            Me.ConfigureForm()
            Me.ConfigureMenus()
            Me.ConfigureNav()
            'Me.WriteLog()
        End Sub


        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.Render(writer)
            ' 运行提示
            writer.Write("<script language=""javascript"">" & vbCrLf)
            writer.Write("var iLeft = (document.body.clientWidth-200)/2;" & vbCrLf)
            writer.Write("var iTop = (document.body.clientHeight-47)/2;" & vbCrLf)
            writer.Write("document.writeln(""<div id='__divRuning' style='POSITION: absolute; Z-INDEX: 108; Left:""+iLeft.toString()+""px; top:""+iTop.toString() + ""px; width:223px; height:47px; BACKGROUND-COLOR: white; border:#9E9E9B 1px solid; font:bold 14px; color:#FF5A00; text-align:center; padding-top:16px; DISPLAY:none;'>处理中，请稍候...</div>"");" & vbCrLf)
            ' Session 状态
            writer.Write("document.writeln(""<input id='___MYSESSIONSTATE' value='" & Session("MySessionState") & "' style='display:none;'>"");" & vbCrLf)
            ' 后缀参数
            Dim QueryString As String
            For Each QueryString In Me.Context.Request.QueryString
                writer.Write("document.writeln(""<input type=\""hidden\"" id=\""__" & QueryString & "\"" name=\""__" & QueryString & "\"" value=\""" & HttpUtility.HtmlEncode(Me.Context.Request.QueryString(QueryString).Replace(ChrW(13) & ChrW(10), "").Replace("\", "\\")) & "\"">"");" & vbCrLf)
            Next
            writer.Write("</script>" & vbCrLf)
        End Sub

        Public Property State() As PageState
            Get
                If ViewState("_state") = Nothing Then
                    If Not Request.QueryString("mode") Is Nothing Then
                        ViewState("_state") = CType(Integer.Parse(Request.QueryString("mode")), PageState)

                    Else
                        ViewState("_state") = PageState.None

                    End If

                End If

                Return ViewState("_state")
            End Get

            Set(ByVal Value As PageState)
                ViewState("_state") = Value
            End Set

        End Property
        Public Property FormEvent() As FormEventId
            Get
                If Not Request.Form("__EVENTTARGET") Is Nothing And Not Request.Form("__EVENTARGUMENT") Is Nothing Then
                    If Request.Form("__EVENTTARGET").ToLower = "__submit" And IsNumeric(Request.Form("__EVENTARGUMENT")) Then
                        Me._formEvent = CType(Integer.Parse(Request.Form("__EVENTARGUMENT")), FormEventId)

                    End If

                End If

                Return Me._formEvent
            End Get

            Set(ByVal Value As FormEventId)
                Me._formEvent = Value
            End Set

        End Property

        ''' <summary>
        ''' 检测安全性
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CheckSecurity()
            ' 校验会话状态是否改变（重登陆、切换公司或用户都会导致会话状态改变）
            ' Invoke 的会话状态校验见 ApplicationMap.aspx.vb
            If ViewState("mysessionstate") Is Nothing Then           ' 如果第一次打开页面
                ' 把会话状态信息保存在页面上
                ViewState("mysessionstate") = Session("MySessionState")
            Else        ' 如果回调
                ' 校验会话状态
                If ViewState("mysessionstate") <> Session("MySessionState") Then       ' 如果会话状态改变
                    Throw New Exception("页面会话状态改变")
                End If
            End If
            CheckRights()
            CheckAccess()
        End Sub

        ''' <summary>
        ''' 检测function权限
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CheckFunctionCode()
            Dim functionCode As String = GetFunctionCode()
            If Not String.IsNullOrEmpty(functionCode) Then
                Dim scope As String = GetFunctionUseScope(functionCode)
                If scope = "1" AndAlso IIf(HttpContext.Current.Session("IsEndCompany") = "1", True, False) Then
                    RedirectSelectCompany("非末级公司")
                Else
                    If scope = "2" AndAlso Not IIf(HttpContext.Current.Session("IsEndCompany") = "1", True, False) Then
                        RedirectSelectCompany("末级公司")
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' 跳转到选择公司页面
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RedirectSelectCompany(ByVal description As String)
            Dim sFile As String = "/Security/SelectCurrentCompany.aspx"
            Dim url As String = "/FrameTemp0.aspx"
            url = url & "?title=" & "选择公司"
            url = url & "&height=" & ""
            url = url & "&filename=" & sFile
            url = url & "&param=" & "Description=" & Server.UrlEncode(description)
            url = url & "&isLogin=" & "true"
            'modified by kongy 2009-09-15 选择公司后,根据需要调用Menu.aspx的refreshByBU方法刷新页面
            Dim script As String = "<script>var returnValue = window.showModalDialog('" & url & "','','dialogWidth:360px; dialogHeight=500px; status:no; help:no; resizable:yes;');try{top.frames('menu').refreshByBU(returnValue); } catch(e){window.location = window.location;}</script>"
            ClientScript.RegisterStartupScript(Me.GetType(), "选择公司", script)
        End Sub

        ''' <summary>
        ''' 获得function使用范围
        ''' </summary>
        ''' <param name="functionCode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetFunctionUseScope(ByVal functionCode As String) As String
            Dim useScope As String = String.Empty
            Dim strSQL As New System.Text.StringBuilder(1000)
            strSQL.Append(" SELECT UseScope")
            strSQL.Append(" FROM myFunction ")
            strSQL.Append(" WHERE functionCode='" & functionCode & "'")
            Dim dtScope As DataTable = MyDB.GetDataTable(strSQL.ToString())
            If Not dtScope Is Nothing AndAlso dtScope.Rows.Count > 0 Then
                '如果为空，则认为是UserScope为0（共用）
                If DBNull.Value.Equals(dtScope.Rows(0).Item("UseScope")) Then
                    useScope = "0"
                Else
                    useScope = dtScope.Rows(0).Item("UseScope")
                End If
            End If
            Return useScope
        End Function

        ''' <summary>
        ''' 检测集成登录
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CheckIntegrateLogin(ByVal userCode As String)
            If String.IsNullOrEmpty(userCode) Then
                Dim loginUrl As String = "/Default.aspx"
                Dim redirectUrl As String = loginUrl
                Dim queryString As String = String.Empty

                If (Not Request.QueryString("MyCurrentCompany") Is Nothing) Then
                    If Request.QueryString("MyCurrentCompany").ToString <> "" Then
                        queryString = "MyCurrentCompany=" & Request.QueryString("MyCurrentCompany").ToString
                    End If
                End If

                If (Not Request.QueryString("usercode") Is Nothing) Then
                    queryString = queryString & GetLinkSign(queryString) & "usercode=" & Request.QueryString("usercode")
                ElseIf (Not Request.Form("usercode") Is Nothing) Then
                    queryString = queryString & GetLinkSign(queryString) & "usercode=" & Request.Form("usercode")
                End If

                queryString = queryString & GetLinkSign(queryString) & "Page=" & Server.UrlEncode(Request.Url.ToString)

                Response.Redirect(redirectUrl & "?" & queryString)
            Else
                'modified by chenbo on 2011/3/3
                '原来的逻辑没有判断SetLoginState返回值而直接调用CheckFunctionCode
                'CheckFunctionCode的实际功能只是选择公司，名不副实
                'SetLoginState的返回值实际上也是是否需要选择公司，也是名不副实
                '因此本次修改为判断返回值后再执行选择公司部分
                If MyLogin.SetLoginState(userCode) Then
                    CheckFunctionCode()
                End If
            End If
        End Sub

        ''' <summary>
        ''' 获取连接符号
        ''' </summary>
        ''' <param name="queryString">页面参数</param>
        ''' <returns>页面参数不为空时返回"&",否则返回空</returns>
        ''' <remarks></remarks>
        Private Function GetLinkSign(ByVal queryString As String) As String
            Dim linkSign As String = String.Empty
            If Not (queryString.Length = 0) Then
                linkSign = "&"
            End If
            Return linkSign
        End Function

        ''' <summary>
        ''' 获得Xml中page节点
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetPageXmlNode() As XmlNode
            Dim PathOfXml As String
            Dim XmlDoc As XmlDocument
            Dim XmlN As XmlNode

            If Not Me.Context.Request.QueryString("Xml") Is Nothing Then
                PathOfXml = Me.Context.Request.QueryString("Xml")
            Else
                PathOfXml = Me.Context.Request.Path.Replace(".aspx", ".xml")
            End If

            If IO.File.Exists(Me.Context.Server.MapPath(PathOfXml)) Then
                XmlDoc = Mysoft.Map.Caching.Cache.GetXmlDocument(PathOfXml)
                If XmlDoc Is Nothing Then
                    Throw New Exception("文件读取错误")
                End If
                XmlN = XmlDoc.SelectSingleNode("/page")
            End If
            Return XmlN
        End Function

        ''' <summary>
        ''' 获得functionCode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetFunctionCode() As String
            Dim functionCode As String = String.Empty
            Dim pageXmlNode As XmlNode = GetPageXmlNode()
            If Not pageXmlNode Is Nothing Then
                functionCode = GetFunctionCodeByXml(pageXmlNode)
            End If
            Return functionCode
        End Function

        ''' <summary>
        ''' 通过Xml节点获得functionCode
        ''' </summary>
        ''' <param name="pageXmlNode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetFunctionCodeByXml(ByVal pageXmlNode As XmlNode) As String
            Dim functionCode As String = String.Empty
            If Not pageXmlNode.Attributes.GetNamedItem("funcid") Is Nothing Then       ' 如果设置 funcid 属性
                functionCode = pageXmlNode.Attributes.GetNamedItem("funcid").InnerText.Replace(";", ",").Split(",")(0)
                If Not Request.QueryString("funcid") Is Nothing Then            ' 如果有后缀 funcid
                    If InStr(("," & pageXmlNode.Attributes.GetNamedItem("funcid").InnerText.Replace(";", ",") & ","), ("," & Context.Request.QueryString("funcid") & ",")) > 0 Then      ' 如果后缀funcid在xml中定义了
                        functionCode = Context.Request.QueryString("funcid")
                    Else      ' 如果后缀funcid没有在xml中定义了
                        Throw New Exception("功能未配置!")
                    End If
                End If
            End If
            Return functionCode
        End Function

        ''' <summary>
        ''' 通过Xml节点获得actionID
        ''' </summary>
        ''' <param name="pageXmlNode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetActionIDByXml(ByVal pageXmlNode As XmlNode) As String
            Dim actionID As String = String.Empty
            If pageXmlNode.Attributes.GetNamedItem("actionid") Is Nothing Then
                actionID = "00"
            Else
                actionID = pageXmlNode.Attributes.GetNamedItem("actionid").InnerText
            End If
            Return actionID
        End Function

        ''' <summary>
        ''' 检查权限
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CheckRights()
            Dim functionCode As String = GetFunctionCode()

            Dim actionID As String = String.Empty
            Dim pageXmlNode As XmlNode = GetPageXmlNode()
            If Not pageXmlNode Is Nothing Then
                actionID = GetActionIDByXml(pageXmlNode)
            End If

            If Not String.IsNullOrEmpty(functionCode) And Not String.IsNullOrEmpty(actionID) Then
                If Not HttpContext.Current.Session("UserKind") Is Nothing Then
                    'modified by chenyong 2009-08-15 授权体系调整 
                    'modified by chenyong 2011-03-10 销售系统用户调整
                    If HttpContext.Current.Session("UserKind") = "1" Then '普通用户

                        '普通用户：funcid前4位不为0206、0000、1001不能访问，前2位不为20不能访问
                        Dim aryAllowCode As String() = New String() {"0206", "0000", "1001", "20"}

                        If Not CheckFunctionCode(functionCode, aryAllowCode) Then
                            Throw New Exception("""普通用户""无此功能模块权限！")
                        End If

                    ElseIf HttpContext.Current.Session("UserKind") = "2" Then '销售系统用户

                        '销售用户：funcid前4位不为0101、0000、1001不能访问，不为01030401、01030302、01030301不能访问
                        Dim aryAllowCode As String() = New String() {"0101", "0000", "1001", "01030401", "01030302", "01030301"}

                        If Not CheckFunctionCode(functionCode, aryAllowCode) Then
                            Throw New Exception("""销售系统用户""无此功能模块权限！")
                        End If
                    End If
                End If


                Mysoft.Map.Application.Security.User.LoadUserRight(Context.Session("UserGUID"), functionCode, actionID)
            End If
        End Sub

        Private Function CheckFunctionCode(ByVal strFuncCode As String, ByVal aryAllowFunCode As String()) As Boolean '0301

            For Each strCode As String In aryAllowFunCode
                If String.Compare(strFuncCode, 0, strCode, 0, strCode.Length) = 0 Then
                    Return True
                End If
            Next

            Return False
        End Function

        Protected Overridable Sub ConfigurePage()
        End Sub

        Protected Overridable Sub ConfigureForm()
        End Sub

        Protected Overridable Sub ConfigureMenus()
        End Sub

        Protected Overridable Sub ConfigureNav()
        End Sub

        '' 支持三种写日志方法：
        '' 1、如果页面是 xml 配置的，<cnt:AppLog runat="server"/>
        '' 2、如果页面不是 xml 配置的非公共模块，<cnt:AppLog runat="server" funcid="01010101"/>
        '' 3、如果页面不是 xml 配置的公共模块，AppLog.WriteLog("01010101")
        'Protected Sub WriteLog()
        '    If Not appLog Is Nothing Then
        '        Mysoft.Map.Application.Security.Logger.WriteLog(appLog.Funcid)
        '    End If
        'End Sub

    End Class

End Namespace

Namespace WebControls.Designer
    Public Class SimpleDesigner
        Inherits ControlDesigner
        Public Overloads Overrides Function GetDesignTimeHtml() As String
            Return MyBase.GetDesignTimeHtml()
        End Function
    End Class
End Namespace
