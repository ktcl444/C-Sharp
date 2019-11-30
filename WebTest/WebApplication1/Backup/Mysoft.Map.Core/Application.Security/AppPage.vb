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
                'ҳ��ֻ����ERP�û�����ʱ����ͨ�û����ܷ���
                Throw New Exception("��ͨ�û�������Ȩ�ޣ�")

            ElseIf Me.AccessUserKind = UserKind.Normal AndAlso sUserKind = "0" Then
                'ҳ��ֻ������ͨ�û�����ʱ��ERP�û����ܷ���
                Throw New Exception("ERP�û�������Ȩ�ޣ�")
            End If
        End Sub

        'modified by chenyong 2009-09-11 Repeater ��OnInit�¼�ȡSession("UserGUID") ȡ����ԭ�����ɵ�¼��OnLoad�д���
        Protected Overrides Sub OnPreInit(ByVal e As System.EventArgs)
            'modified by kongy 2009-08-19 ���ɵ�¼�޸�
            Dim userCode As String = MyLogin.IntegrateLogin()
            Dim isLogin As Boolean = MyLogin.CheckLoginBySession()
            '���ѵ�¼�����ҵ�¼�û��뼯�ɷ��ʵ��û���ͬ���򼯳ɷ����û�Ϊ�գ�ʱ��ֻУ�鹦�ܴ���
            If isLogin AndAlso (Session("UserCode") = userCode OrElse String.IsNullOrEmpty(userCode)) Then
                'added by kongy 2009-09-14 ���ݴ��ݵĲ������ı乫˾�Ự״̬
                SetCompanyByPageParameter()
                CheckFunctionCode()
            Else
                '��δ��¼�����¼�û��뼯�ɷ����û���ͬ������м��ɵ�¼����
                CheckIntegrateLogin(userCode)
            End If
            MyBase.OnPreInit(e)
        End Sub

        'ͨ��ҳ�洫�ݲ������ù�˾�Ự״̬
        Private Sub SetCompanyByPageParameter()
            If Not Request.QueryString("MyCurrentCompany") Is Nothing AndAlso Request.QueryString("MyCurrentCompany").ToString <> "" Then
                MyLogin.SetCompanySession(Request.QueryString("MyCurrentCompany").ToString)
            End If
        End Sub

        '���µ�ǰ�����û���Ϣ
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
            ' ������ʾ
            writer.Write("<script language=""javascript"">" & vbCrLf)
            writer.Write("var iLeft = (document.body.clientWidth-200)/2;" & vbCrLf)
            writer.Write("var iTop = (document.body.clientHeight-47)/2;" & vbCrLf)
            writer.Write("document.writeln(""<div id='__divRuning' style='POSITION: absolute; Z-INDEX: 108; Left:""+iLeft.toString()+""px; top:""+iTop.toString() + ""px; width:223px; height:47px; BACKGROUND-COLOR: white; border:#9E9E9B 1px solid; font:bold 14px; color:#FF5A00; text-align:center; padding-top:16px; DISPLAY:none;'>�����У����Ժ�...</div>"");" & vbCrLf)
            ' Session ״̬
            writer.Write("document.writeln(""<input id='___MYSESSIONSTATE' value='" & Session("MySessionState") & "' style='display:none;'>"");" & vbCrLf)
            ' ��׺����
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
        ''' ��ⰲȫ��
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CheckSecurity()
            ' У��Ự״̬�Ƿ�ı䣨�ص�½���л���˾���û����ᵼ�»Ự״̬�ı䣩
            ' Invoke �ĻỰ״̬У��� ApplicationMap.aspx.vb
            If ViewState("mysessionstate") Is Nothing Then           ' �����һ�δ�ҳ��
                ' �ѻỰ״̬��Ϣ������ҳ����
                ViewState("mysessionstate") = Session("MySessionState")
            Else        ' ����ص�
                ' У��Ự״̬
                If ViewState("mysessionstate") <> Session("MySessionState") Then       ' ����Ự״̬�ı�
                    Throw New Exception("ҳ��Ự״̬�ı�")
                End If
            End If
            CheckRights()
            CheckAccess()
        End Sub

        ''' <summary>
        ''' ���functionȨ��
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CheckFunctionCode()
            Dim functionCode As String = GetFunctionCode()
            If Not String.IsNullOrEmpty(functionCode) Then
                Dim scope As String = GetFunctionUseScope(functionCode)
                If scope = "1" AndAlso IIf(HttpContext.Current.Session("IsEndCompany") = "1", True, False) Then
                    RedirectSelectCompany("��ĩ����˾")
                Else
                    If scope = "2" AndAlso Not IIf(HttpContext.Current.Session("IsEndCompany") = "1", True, False) Then
                        RedirectSelectCompany("ĩ����˾")
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' ��ת��ѡ��˾ҳ��
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RedirectSelectCompany(ByVal description As String)
            Dim sFile As String = "/Security/SelectCurrentCompany.aspx"
            Dim url As String = "/FrameTemp0.aspx"
            url = url & "?title=" & "ѡ��˾"
            url = url & "&height=" & ""
            url = url & "&filename=" & sFile
            url = url & "&param=" & "Description=" & Server.UrlEncode(description)
            url = url & "&isLogin=" & "true"
            'modified by kongy 2009-09-15 ѡ��˾��,������Ҫ����Menu.aspx��refreshByBU����ˢ��ҳ��
            Dim script As String = "<script>var returnValue = window.showModalDialog('" & url & "','','dialogWidth:360px; dialogHeight=500px; status:no; help:no; resizable:yes;');try{top.frames('menu').refreshByBU(returnValue); } catch(e){window.location = window.location;}</script>"
            ClientScript.RegisterStartupScript(Me.GetType(), "ѡ��˾", script)
        End Sub

        ''' <summary>
        ''' ���functionʹ�÷�Χ
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
                '���Ϊ�գ�����Ϊ��UserScopeΪ0�����ã�
                If DBNull.Value.Equals(dtScope.Rows(0).Item("UseScope")) Then
                    useScope = "0"
                Else
                    useScope = dtScope.Rows(0).Item("UseScope")
                End If
            End If
            Return useScope
        End Function

        ''' <summary>
        ''' ��⼯�ɵ�¼
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
                'ԭ�����߼�û���ж�SetLoginState����ֵ��ֱ�ӵ���CheckFunctionCode
                'CheckFunctionCode��ʵ�ʹ���ֻ��ѡ��˾��������ʵ
                'SetLoginState�ķ���ֵʵ����Ҳ���Ƿ���Ҫѡ��˾��Ҳ��������ʵ
                '��˱����޸�Ϊ�жϷ���ֵ����ִ��ѡ��˾����
                If MyLogin.SetLoginState(userCode) Then
                    CheckFunctionCode()
                End If
            End If
        End Sub

        ''' <summary>
        ''' ��ȡ���ӷ���
        ''' </summary>
        ''' <param name="queryString">ҳ�����</param>
        ''' <returns>ҳ�������Ϊ��ʱ����"&",���򷵻ؿ�</returns>
        ''' <remarks></remarks>
        Private Function GetLinkSign(ByVal queryString As String) As String
            Dim linkSign As String = String.Empty
            If Not (queryString.Length = 0) Then
                linkSign = "&"
            End If
            Return linkSign
        End Function

        ''' <summary>
        ''' ���Xml��page�ڵ�
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
                    Throw New Exception("�ļ���ȡ����")
                End If
                XmlN = XmlDoc.SelectSingleNode("/page")
            End If
            Return XmlN
        End Function

        ''' <summary>
        ''' ���functionCode
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
        ''' ͨ��Xml�ڵ���functionCode
        ''' </summary>
        ''' <param name="pageXmlNode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetFunctionCodeByXml(ByVal pageXmlNode As XmlNode) As String
            Dim functionCode As String = String.Empty
            If Not pageXmlNode.Attributes.GetNamedItem("funcid") Is Nothing Then       ' ������� funcid ����
                functionCode = pageXmlNode.Attributes.GetNamedItem("funcid").InnerText.Replace(";", ",").Split(",")(0)
                If Not Request.QueryString("funcid") Is Nothing Then            ' ����к�׺ funcid
                    If InStr(("," & pageXmlNode.Attributes.GetNamedItem("funcid").InnerText.Replace(";", ",") & ","), ("," & Context.Request.QueryString("funcid") & ",")) > 0 Then      ' �����׺funcid��xml�ж�����
                        functionCode = Context.Request.QueryString("funcid")
                    Else      ' �����׺funcidû����xml�ж�����
                        Throw New Exception("����δ����!")
                    End If
                End If
            End If
            Return functionCode
        End Function

        ''' <summary>
        ''' ͨ��Xml�ڵ���actionID
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
        ''' ���Ȩ��
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
                    'modified by chenyong 2009-08-15 ��Ȩ��ϵ���� 
                    'modified by chenyong 2011-03-10 ����ϵͳ�û�����
                    If HttpContext.Current.Session("UserKind") = "1" Then '��ͨ�û�

                        '��ͨ�û���funcidǰ4λ��Ϊ0206��0000��1001���ܷ��ʣ�ǰ2λ��Ϊ20���ܷ���
                        Dim aryAllowCode As String() = New String() {"0206", "0000", "1001", "20"}

                        If Not CheckFunctionCode(functionCode, aryAllowCode) Then
                            Throw New Exception("""��ͨ�û�""�޴˹���ģ��Ȩ�ޣ�")
                        End If

                    ElseIf HttpContext.Current.Session("UserKind") = "2" Then '����ϵͳ�û�

                        '�����û���funcidǰ4λ��Ϊ0101��0000��1001���ܷ��ʣ���Ϊ01030401��01030302��01030301���ܷ���
                        Dim aryAllowCode As String() = New String() {"0101", "0000", "1001", "01030401", "01030302", "01030301"}

                        If Not CheckFunctionCode(functionCode, aryAllowCode) Then
                            Throw New Exception("""����ϵͳ�û�""�޴˹���ģ��Ȩ�ޣ�")
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

        '' ֧������д��־������
        '' 1�����ҳ���� xml ���õģ�<cnt:AppLog runat="server"/>
        '' 2�����ҳ�治�� xml ���õķǹ���ģ�飬<cnt:AppLog runat="server" funcid="01010101"/>
        '' 3�����ҳ�治�� xml ���õĹ���ģ�飬AppLog.WriteLog("01010101")
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
