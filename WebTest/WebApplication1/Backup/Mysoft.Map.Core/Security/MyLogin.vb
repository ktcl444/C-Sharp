Imports Mysoft.Map.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.UI
Imports System.Web.SessionState.HttpSessionState
Imports Mysoft.Map.Utility
Imports Mysoft.Map.Application.Security
Imports Mysoft.Map.Security
Imports Mysoft.Map.Application.UpdatePack

Namespace Security
    ''add 2009.8.14 kongy 登录处理类
    Public Class MyLogin

#Region "公共函数"

        ''普通登录验证
        ''参数 userCode 用户代码
        ''参数 password 用户密码(密文)
        ''返回值 是否验证通过
        Public Shared Function NormalLogin(ByVal userCode As String, ByVal password As String) As Boolean
            Dim myUser As MyUser = GetMyUser(userCode)

            If myUser.UserGuid Is Nothing Then
                Throw New LoginException("用户代码或密码不正确！")
            End If

            '实施密码错误策略(判断账户是否被锁定)
            If MyPasswordPolicy.Instance.EnabledUserLock AndAlso MyPasswordPolicy.GetLockState(userCode) = LockState.Locked Then
                Throw New LoginException("已经达到最大密码错误次数，帐号已被锁定，不允许登录！")
            End If

            ' 验证密码(不区分大小写)
            If String.Compare(myUser.Password, password, True) <> 0 Then

                '实施密码错误策略(该方法累计错误次数，并根据累计错误次数自动加锁)
                MyPasswordPolicy.AddWrongTimes(userCode)

                Throw New LoginException("用户代码或密码不正确！")
            End If

            '判断帐户禁用状态
            If myUser.IsDisabeld = "1" Then
                Throw New LoginException("此账号已被禁用，不允许登录！")
            End If

            Return True
        End Function

        ''集成登录验证
        ''返回值 验证通过返回用户代码，验证不通过返回空字符串
        Public Shared Function IntegrateLogin() As String
            Dim userCode As String = String.Empty

            Select Case GetConfigurationValue("LOGON_MODE")
                Case "ad"
                    userCode = GetUserCodeByAD()
                Case "cookie"
                    userCode = GetUserCodeByCookie()
                Case Else
            End Select

            '通过AD或Cookie不能登录时再尝试其它登录方式
            If String.IsNullOrEmpty(userCode) Then
                userCode = getOtherLoginUser() '通过URL或PostData获取用户代码
            End If

            Return userCode
        End Function

        ''设置登录会话状态状态
        ''参数 userCode 用户代码
        ''可选参数 isSaveCookie 是否需要保存cookie 默认为false
        ''返回值 是否需要选择公司
        Public Shared Function SetLoginState(ByVal userCode As String, Optional ByVal isSaveCookie As Boolean = False) As Boolean
            Dim myUser As MyUser = GetMyUser(userCode)

            If myUser.UserGuid Is Nothing OrElse myUser.IsDisabeld = "1" Then
                Throw New LoginException("非本系统用户或用户被禁用")
            End If

            '登录前校验
            VerifyLogin(userCode)

            SetUserSession(myUser)

            SetLoginCookie(userCode, isSaveCookie)

            '生成登录加密信息
            SetLoginData()

            Return SetCompanySession(myUser)

        End Function

        '生成 Session 前的校验，防止跳过校验直接登录
        Private Shared Sub VerifyLogin(ByVal userCode As String)
            '系统校验
            Try
                VerifyApplication.VerifyRuntime(True)
            Catch ex As Exception
                MyDB.LogException("授权校验失败", ex)

                Throw New LoginException(ex.Message)
            End Try

            '是否培训版本
            Dim blnTrainingVersion As Boolean = False
            If Not Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings("TrainingVersion"), blnTrainingVersion) Then blnTrainingVersion = False
            If blnTrainingVersion Then Exit Sub

            '是否自用狗
            Dim MyDog As ISoftDog = SoftDogFactory.CreateSoftDogObject
            If Not MyDog.IsZydog Then Exit Sub

            '在线用户数校验，自用狗非培训版本限制同时5个在线用户
            Dim dsOnlineUser As DataSet
            dsOnlineUser = OnlineUser.GetOnlineUserList()

            Dim drUser As DataRow() = dsOnlineUser.Tables(0).Select("userCode='" & userCode.Replace("'", "''") & "'")
            If drUser.Count = 0 And dsOnlineUser.Tables(0).Rows.Count >= 5 Then
                Dim ex As New LoginException("内部使用版本，同时在线用户数不允许超过5个！")
                MyDB.LogException("培训版本用户数超出", ex)
                Throw ex
            End If
        End Sub

        ' 设置登录时的加密信息，用于登录后检查登录是否合法(不是模拟的登录)
        Private Shared Sub SetLoginData()
            Dim strUserGUID As String = HttpContext.Current.Session("UserGUID")
            Dim dtmLoginDate As DateTime = DateTime.Now

            Dim strInput As String
            strInput = HttpContext.Current.Session.SessionID & "|" & strUserGUID & "|" & dtmLoginDate.ToString("yyyyMMddHHmmss")

            ' 变换后的 MD5 算法，请确保代码混淆后不能被反编译
            Dim md5 As New System.Security.Cryptography.MD5CryptoServiceProvider()
            Dim bs As Byte() = System.Text.Encoding.UTF8.GetBytes(strInput)
            For i As Integer = 0 To bs.Length - 1
                bs(i) = bs(i) Xor &HF0
            Next
            bs = md5.ComputeHash(bs)
            Dim s As New System.Text.StringBuilder()
            Dim b As Byte
            For i As Integer = bs.Length - 1 To 0 Step -1
                b = bs(i) Xor (((i + 3) ^ 2 + &HA3) And &HFF)
                s.Append(b.ToString("X2"))
            Next

            HttpContext.Current.Session("LoginDate") = dtmLoginDate
            HttpContext.Current.Session("LoginData") = s.ToString()
        End Sub

        ''通过校验会话状态判断是否登录
        Public Shared Function CheckLoginBySession() As Boolean
            If Not HttpContext.Current.Session("UserGUID") Is Nothing AndAlso HttpContext.Current.Session("UserGUID") <> "" AndAlso Not HttpContext.Current.Session("MySessionState") Is Nothing AndAlso HttpContext.Current.Session("MySessionState") <> "" Then
                Return True
            End If
            Return False
        End Function

        ''注销
        Public Shared Sub LoggingOut()
            '把当前用户从在线用户列表中删除（如果同一用户多处登录，可能存在问题）
            OnlineUser.RemoveLogoutUser(HttpContext.Current.Session("UserGUID"))

            HttpContext.Current.Session.RemoveAll()
            If GetConfigurationValue("LOGON_MODE") = "cookie" Then
                ClearLoginCookie()
            End If
        End Sub

        Public Shared Function CheckERPLogin() As Boolean
            Return GetConfigurationValue("SiteType").ToUpper() = "ERP"
        End Function

        Public Shared Function CheckEKPLogin() As Boolean
            Return GetConfigurationValue("SiteType").ToUpper() = "EKP"
        End Function

        Public Shared Function CheckDSSLogin() As Boolean
            Return GetConfigurationValue("SiteType").ToUpper() = "DSS"
        End Function

        Public Shared Function ReadSiteName() As String
            Return GetConfigurationValue("SiteName")
        End Function

        Public Shared Function ReadSiteMainPage() As String
            Return GetConfigurationValue("SiteMainPage")
        End Function

        ''通过公司参数判断权限后再设置公司会话状态
        Public Shared Sub SetCompanySession(ByVal buGUID As String)
            Dim companyDataTable As DataTable = GetAuthorizedCompany(buGUID)
            If Not companyDataTable Is Nothing AndAlso companyDataTable.Rows.Count > 0 Then
                SetCompanySession(buGUID, companyDataTable.Rows(0).Item("BUName").ToString, IIf(companyDataTable.Rows(0).Item("IsEndCompany"), "1", "0"))
            End If
        End Sub
#End Region

#Region "私有函数"

        ''获得用户实体
        ''参数 strUserCode 用户代码
        ''返回值 用户实体
        Private Shared Function GetMyUser(ByVal strUserCode As String) As MyUser
            Dim DT As DataTable
            Dim SQL As String

            ' 暂时没有加密码验证
            SQL = " SELECT us.UserGUID,us.UserKind,us.UserName,us.Password,bu.BUGUID,bu.BUName,bu.IsEndCompany,ISNULL(us.IsDisabeld,0) AS IsDisabeld " & _
                  " FROM myUser us,myBusinessUnit bu " & _
                  " WHERE us.BUGUID = bu.BUGUID" & _
                  " AND us.UserCode='" & strUserCode.Replace("'", "''") & "'"
            '" AND (us.IsDisabeld=0 OR us.IsDisabeld IS null)"
            DT = MyDB.GetDataTable(SQL)

            Dim user As New MyUser()
            If DT.Rows.Count = 0 Then
                Return user
            End If
            user.UserCode = strUserCode
            user.UserGuid = DT.Rows(0)("UserGUID").ToString
            user.UserName = DT.Rows(0)("UserName").ToString
            user.BuGuid = DT.Rows(0)("BuGuid").ToString
            user.BuName = DT.Rows(0)("BuName").ToString
            user.Password = DT.Rows(0)("Password").ToString
            user.IsEndCompany = IIf(DT.Rows(0).Item("IsEndCompany"), "1", "0")
            user.UserKind = DT.Rows(0).Item("UserKind").ToString
            user.IsDisabeld = IIf(DT.Rows(0).Item("IsDisabeld"), "1", "0")
            Return user
        End Function

        ''用户实体
        Private Structure MyUser
            Public UserCode As String
            Public UserGuid As String
            Public UserName As String
            Public Password As String
            Public BuGuid As String
            Public BuName As String
            Public IsEndCompany As String
            Public UserKind As String
            Public IsDisabeld As String
        End Structure

        ''设置用户会话状态
        Private Shared Sub SetUserSession(ByVal myUser As MyUser)
            HttpContext.Current.Session("UserGUID") = myUser.UserGuid   ' 用户GUID
            HttpContext.Current.Session("UserName") = myUser.UserName   ' 用户名称
            HttpContext.Current.Session("UserCode") = myUser.UserCode   ' 用户代码 
            HttpContext.Current.Session("Password") = myUser.Password    ' 密码（密文） 
            HttpContext.Current.Session("UserKind") = myUser.UserKind
            HttpContext.Current.Session("MySessionState") = Guid.NewGuid.ToString        ' HttpContext.Current.Session 状态信息
        End Sub

        ''获得AD验证用户代码
        ''返回值 验证通过返回用户代码，验证不通过返回空字符串
        Private Shared Function GetUserCodeByAD() As String
            Dim userCode As String = String.Empty
            If HttpContext.Current.User.Identity.IsAuthenticated Then
                userCode = HttpContext.Current.User.Identity.Name.Split("\")(1)
            End If
            If ConfigurationSettings.AppSettings("IIS_PASS") = "1" Then
                Return userCode
            End If
            Return String.Empty
        End Function

        ''获得cookie验证用户代码
        ''返回值 验证通过返回用户代码，验证不通过返回空字符串
        Private Shared Function GetUserCodeByCookie() As String
            Dim userCode As String = String.Empty
            Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies("Login_User")

            If Not cookie Is Nothing Then
                Dim signTime As DateTime = Convert.ToDateTime(cookie("SignTime").ToString)
                userCode = cookie("UserCode").ToString
                Dim userCodeSign As String = cookie("UserSign").ToString

                If signTime.AddDays(1) > DateTime.Now AndAlso RSAUtil.SignatureDeformatter(userCode + cookie("SignTime").ToString, userCodeSign) Then
                    Return userCode
                End If
            End If

            Return String.Empty
        End Function

        ''设置登录cookie
        ''参数 userCode 用户代码
        ''参数 isSaveCookie 是否保存cookie
        Private Shared Sub SetLoginCookie(ByVal userCode As String, ByVal isSaveCookie As Boolean)
            If isSaveCookie AndAlso ConfigurationSettings.AppSettings("LOGON_MODE").ToLower = "cookie" Then
                Dim cookie As HttpCookie = New HttpCookie("Login_User")
                Dim signTime As DateTime = DateTime.Now
                cookie("UserCode") = userCode
                cookie("SignTime") = signTime.ToString
                Dim strUserSign As String = String.Empty
                If RSAUtil.SignatureFormatter(userCode + signTime.ToString, strUserSign) Then
                    cookie("UserSign") = strUserSign
                    cookie.Domain = GetDomainName()
                    cookie.Path = "/"
                    HttpContext.Current.Response.Cookies.Add(cookie)
                End If
            End If
        End Sub

        ''清除登录cookie
        Private Shared Sub ClearLoginCookie()
            Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies("Login_User")
            If Not cookie Is Nothing Then
                cookie.Domain = GetDomainName()
                cookie.Expires = DateTime.Now.AddYears(-1)
                HttpContext.Current.Response.Cookies.Add(cookie)
            End If
        End Sub

        ''设置公司会话状态
        ''参数 myUser 当前登录用户
        ''返回值 是否需要选择公司
        Private Shared Function SetCompanySession(ByVal myUser As MyUser) As Boolean
            Dim b As Boolean = False
            If Not HttpContext.Current.Request.QueryString("MyCurrentCompany") Is Nothing AndAlso HttpContext.Current.Request.QueryString("MyCurrentCompany").ToString <> "" Then
                b = SetCompanySession(HttpContext.Current.Request.QueryString("MyCurrentCompany").ToString, myUser)
            Else
                If Not HttpContext.Current.Request.Cookies("mycrm_company") Is Nothing AndAlso HttpContext.Current.Request.Cookies("mycrm_company").Value <> "" Then
                    b = SetCompanySession(HttpContext.Current.Request.Cookies("mycrm_company").Value, myUser)
                Else
                    b = SetCurrentCompanyByUserCompany(myUser)
                End If
            End If

            If CheckDSSLogin() Then
                Return False
            Else
                Return b
            End If
        End Function

        ''设置用户所属公司为当前公司
        Private Shared Function SetCurrentCompanyByUserCompany(ByVal myUser As MyUser) As Boolean
            SetCompanySession(myUser.BuGuid, myUser.BuName, myUser.IsEndCompany)
            Dim isSelectCompany As Boolean = (GetAuthorizedCompanyCount(myUser.UserGuid) <> 1)
            If Not isSelectCompany Then
                Logger.WriteLoginLog()
            End If
            Return isSelectCompany
        End Function

        ''通过公司参数判断权限后再设置公司会话状态(登录调用)
        Private Shared Function SetCompanySession(ByVal buGUID As String, ByVal myUser As MyUser) As Boolean
            Dim companyDataTable As DataTable = GetAuthorizedCompany(buGUID)
            If Not companyDataTable Is Nothing AndAlso companyDataTable.Rows.Count > 0 Then
                SetCompanySession(buGUID, companyDataTable.Rows(0).Item("BUName").ToString, IIf(companyDataTable.Rows(0).Item("IsEndCompany"), "1", "0"))
                Logger.WriteLoginLog()
                Return False
            Else
                Return SetCurrentCompanyByUserCompany(myUser)
            End If
        End Function

        ''设置当前公司会话状态
        Private Shared Sub SetCompanySession(ByVal buGUID As String, ByVal buName As String, ByVal isEndCompany As String)
            HttpContext.Current.Session("BUGUID") = buGUID
            HttpContext.Current.Session("BUName") = buName
            HttpContext.Current.Session("IsEndCompany") = isEndCompany
            GeneralBase.InsertKeywordValue2myTemp(HttpContext.Current.Session("UserGUID") & "_" & HttpContext.Current.Session.SessionID, "[当前公司]", buGUID)
        End Sub

        ''如果公司在当前用户的权限内,返回公司信息数据集
        Private Shared Function GetAuthorizedCompany(ByVal buGUID As String) As DataTable
            Dim strSQL As New System.Text.StringBuilder(1000)
            '用户所属公司及下属公司
            strSQL.Append(" SELECT BUGUID, BUName,IsEndCompany ")
            strSQL.Append(" FROM myBusinessUnit ")
            strSQL.Append(" WHERE BUGUID='%BUGUID%' AND IsCompany=1 AND (HierarchyCode LIKE ")
            strSQL.Append("           (SELECT HierarchyCode ")
            strSQL.Append("          FROM myBusinessUnit AS b ")
            strSQL.Append("          WHERE (BUGUID = ")
            strSQL.Append("                    (SELECT TOP 1 BUGUID ")
            strSQL.Append("                   FROM myUser ")
            strSQL.Append("                   WHERE (UserGUID = '%UserGUID%')))) + '%') ")
            strSQL.Append(" UNION ")
            '用户岗位所属公司
            strSQL.Append(" SELECT b.BUGUID, b.BUName,IsEndCompany ")
            strSQL.Append(" FROM myBusinessUnit AS b INNER JOIN ")
            strSQL.Append("       myStation AS s ON b.BUGUID = s.CompanyGUID INNER JOIN ")
            strSQL.Append("       myStationUser AS su ON s.StationGUID = su.StationGUID AND  ")
            strSQL.Append("       su.UserGUID = '%UserGUID%' ")
            strSQL.Append(" WHERE b.IsCompany=1 AND b.BUGUID='%BUGUID%' ")

            strSQL.Replace("%BUGUID%", buGUID.Replace("'", "''"))
            strSQL.Replace("%UserGUID%", HttpContext.Current.Session("UserGUID"))

            Return MyDB.GetDataTable(strSQL.ToString())
        End Function

        ''获取用户拥有权限的公司数目
        Private Shared Function GetAuthorizedCompanyCount(ByVal userGUID As String) As Int16
            Dim dtBU As DataTable, sql As String
            sql = "SELECT b.OrderHierarchyCode FROM myBusinessUnit AS b" & _
                 " RIGHT OUTER JOIN" & _
                 " (" & _
                 "  SELECT ISNULL(s.CompanyGUID, u.BUGUID) AS BUGUID" & _
                 "  FROM myStation AS s" & _
                 "  RIGHT OUTER JOIN myStationUser AS su ON s.StationGUID = su.StationGUID" & _
                 "  RIGHT OUTER JOIN myUser AS u ON su.UserGUID = u.UserGUID" & _
                 "  WHERE u.UserGUID = '" & userGUID & "'" & _
                 " ) AS ub ON b.BUGUID = ub.BUGUID"

            ' 取用户物理所属公司
            sql &= " UNION SELECT b.OrderHierarchyCode FROM myBusinessUnit AS b" & _
                   " INNER JOIN myUser AS u ON u.BUGUID=b.BUGUID" & _
                   "  WHERE u.UserGUID = '" & userGUID & "'"

            sql = "SELECT b1.BUGUID,b1.BUName FROM myBusinessUnit AS b1" & _
                  " INNER JOIN (" & sql & ") AS b2 ON b1.OrderHierarchyCode + '.' LIKE b2.OrderHierarchyCode + '.%'" & _
                  " WHERE b1.BUType = 0"

            dtBU = MyDB.GetDataTable(sql)
            Return dtBU.Rows.Count
        End Function

        ''获得配置值
        Private Shared Function GetConfigurationValue(ByVal settingName As String) As String
            Dim configurationValue As String = String.Empty
            Try
                configurationValue = System.Configuration.ConfigurationSettings.AppSettings(settingName).ToLower
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)

            End Try
            Return configurationValue
        End Function

        Private Shared Function getOtherLoginUser() As String
            '其它登录的方式（URL参数、PostData）
            '优先顺序为URL、PostData
            'usercode为用户代码, password为密码密文, 参数名全小写
            Dim strUserCode As String = String.Empty, strPassWord As String

            Try
                '从URL中获取
                strUserCode = HttpContext.Current.Request.QueryString("usercode")
                strPassWord = HttpContext.Current.Request.QueryString("password")
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                strUserCode = String.Empty
            End Try

            '若URL中没有，则从PostData中获取
            If String.IsNullOrEmpty(strUserCode) Then
                Try
                    '从PostData中获取
                    strUserCode = HttpContext.Current.Request.Form("usercode")
                    strPassWord = HttpContext.Current.Request.Form("password")
                Catch ex As Exception
                	Mysoft.Map.Data.MyDB.LogException(ex)
                    strUserCode = String.Empty
                End Try
            End If

            If Not String.IsNullOrEmpty(strUserCode) Then
                '如果通过验证，则返回此用户号
                If VerifyUserLogin(strUserCode, strPassWord) Then
                    Return strUserCode
                End If
            End If

            '如果不存在正确的用户名与密码，则返回空字符串
            Return String.Empty
        End Function


        Private Shared Function VerifyUserLogin(ByVal strUserCode As String, ByVal strPassWord As String) As Boolean
            '通过普通登录方法验证用户名与密码的正确性
            Try
                Return NormalLogin(strUserCode, strPassWord)
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                '验证失败则返回False
                Return False
            End Try
        End Function

        Private Shared Function GetDomainName() As String
            Dim sRtn As String
            '从会话中获取DomainName
            If Not HttpContext.Current.Session("DomainName") Is Nothing AndAlso HttpContext.Current.Session("DomainName") <> "" Then
                sRtn = HttpContext.Current.Session("DomainName")
            Else
                '会话没有，则从WebConfig中获取
                sRtn = Mysoft.Map.Application.WebConfig.ReadAppSettings("DomainName")

                'WebConfig中没有配置，则通过当前URL获取
                If sRtn = "" Then
                    sRtn = Mysoft.Map.Utility.GeneralBase.GetDomain(HttpContext.Current.Request.Url.ToString())
                End If

                '将DomainName设置至会话中
                HttpContext.Current.Session("DomainName") = sRtn
            End If

            Return sRtn
        End Function
#End Region

        Public Class LoginException
            Inherits System.Exception
            Sub New()
                MyBase.New()
            End Sub

            Sub New(ByVal message As String)
                MyBase.New(message)
            End Sub

            Sub New(ByVal message As String, ByVal InnerException As Exception)
                MyBase.New(message, InnerException)
            End Sub

            Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
                MyBase.New(info, context)
            End Sub
        End Class

    End Class
End Namespace

