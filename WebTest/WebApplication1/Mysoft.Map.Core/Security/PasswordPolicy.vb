Imports System
Imports System.Text
Imports System.Web
Imports System.Web.Caching
Imports Mysoft.Map.Data

Namespace Security
    ''' <summary>
    ''' 长度策略单元（内容长度）
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class PolicyLengthUnit
        ''' <summary>
        ''' 校验字符长度是否符合策略，true符合，false不符合。
        ''' </summary>
        ''' <param name="verifyString">要校验的字符串</param>
        ''' <param name="length">策略长度</param>
        ''' <returns>true符合，false不符合。</returns>
        ''' <remarks>参数length必须大于0。</remarks>
        Shared Function Verify(ByVal verifyString As String, ByVal length As Int32) As Boolean
            If length < 1 Then
                Throw New ArgumentOutOfRangeException("length", "长度必须大于0")
            End If
            If verifyString.Length >= length Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    ''' <summary>
    ''' 数字(0-9)策略单元（内容中包含数字）
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class PolicyNumberUnit
        Private Shared _Reg As New RegularExpressions.Regex("[0-9]+")
        ''' <summary>
        ''' 校验字符内容是否符合策略，true符合，false不符合。
        ''' </summary>
        ''' <param name="verifyString">要校验的字符串</param>
        ''' <returns>true符合，false不符合。</returns>
        ''' <remarks></remarks>
        Shared Function Verify(ByVal verifyString As String) As Boolean
            If _Reg.IsMatch(verifyString) Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    ''' <summary>
    ''' 字母策略单元（内容中包含字母）
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class PolicyLetterUnit
        Private Shared _Reg As New RegularExpressions.Regex("[A-Za-z]+")
        ''' <summary>
        ''' 校验字符内容是否符合策略，true符合，false不符合。
        ''' </summary>
        ''' <param name="verifyString">要校验的字符串</param>
        ''' <returns>true符合，false不符合。</returns>
        ''' <remarks></remarks>
        Shared Function Verify(ByVal verifyString As String) As Boolean
            If _Reg.IsMatch(verifyString) Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    ''' <summary>
    ''' 小写字母策略单元（内容中包含小写字母）
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class PolicyLowercaseUnit
        Private Shared _Reg As New RegularExpressions.Regex("[a-z]+")
        ''' <summary>
        ''' 校验字符内容是否符合策略，true符合，false不符合。
        ''' </summary>
        ''' <param name="verifyString">要校验的字符串</param>
        ''' <returns>true符合，false不符合。</returns>
        ''' <remarks></remarks>
        Shared Function Verify(ByVal verifyString As String) As Boolean
            If _Reg.IsMatch(verifyString) Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    ''' <summary>
    ''' 大写字母策略单元（内容中包含大写字母）
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class PolicyCapitalUnit
        Private Shared _Reg As New RegularExpressions.Regex("[A-Z]+")
        ''' <summary>
        ''' 校验字符内容是否符合策略，true符合，false不符合。
        ''' </summary>
        ''' <param name="verifyString">要校验的字符串</param>
        ''' <returns>true符合，false不符合。</returns>
        ''' <remarks></remarks>
        Shared Function Verify(ByVal verifyString As String) As Boolean
            If _Reg.IsMatch(verifyString) Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    ''' <summary>
    ''' 特殊字符策略单元（内容中包含特殊字符）
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class PolicySpecialCharacterUnit
        Private Shared _Reg As New RegularExpressions.Regex("[^A-Za-z0-9]+")
        ''' <summary>
        ''' 校验字符内容是否符合策略，true符合，false不符合。
        ''' </summary>
        ''' <param name="verifyString">要校验的字符串</param>
        ''' <returns>true符合，false不符合。</returns>
        ''' <remarks></remarks>
        Shared Function Verify(ByVal verifyString As String) As Boolean
            If _Reg.IsMatch(verifyString) Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    ''' <summary>
    ''' 过期策略单元
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class PolicyExpirationUnit
        ''' <summary>
        ''' 校验过期（以策略有效期[单位天]，衡量某一时间点是否过期）是否符合策略，true未过期（符合），false过期（不符合）。
        ''' </summary>
        ''' <param name="baseDateTime">基线时间点</param>
        ''' <param name="expirationDays">有效天数（策略）</param>
        ''' <returns>true未过期，false过期。</returns>
        ''' <remarks></remarks>
        Shared Function Verify(ByVal baseDateTime As DateTime, ByVal expirationDays As Int32) As Boolean
            Dim strNow As String = MyDB.GetDataItemString("SELECT CONVERT(varchar(20),GetDate(),120) ")
            If Date.Compare(baseDateTime.AddDays(expirationDays).Date, DateTime.Parse(strNow).Date) <= 0 Then
                Return False '过期
            Else
                Return True
            End If
        End Function
    End Class

    ''' <summary>
    ''' 提醒（警告）策略单元
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class PolicyAlarmUnit
        ''' <summary>
        ''' 校验提醒\警告（以策略有效期[单位天]，过期前多少天提示\警告，衡量某一时间点当前是否提示\警告）是否符合策略，true不需要提醒（符合），false需要提醒（不符合）。
        ''' </summary>
        ''' <param name="baseDateTime"></param>
        ''' <param name="expirationDays"></param>
        ''' <param name="alarmDays"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Shared Function Verify(ByVal baseDateTime As DateTime, ByVal expirationDays As Int32, ByVal alarmDays As Int32) As Boolean
            Dim strNow As String = MyDB.GetDataItemString("SELECT CONVERT(varchar(20),GetDate(),120) ")
            If Date.Compare(baseDateTime.AddDays(expirationDays - alarmDays).Date, DateTime.Parse(strNow).Date) <= 0 Then
                Return False '提醒
            Else
                Return True
            End If
        End Function
    End Class

    ''' <summary>
    ''' 锁状态。0、未锁定；1、锁定
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum LockState
        Unlock = 0
        Locked = 1
    End Enum

    ''' <summary>
    ''' 密码复杂度策略类型。0、包含数字和字母；1、包含数字和大小写字母；2、包含数字、大小写字母和特殊字符。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum PasswordPolicyComplexType
        NumberAndLetter = 0
        NumberAndLowercaseAndCapital = 1
        NumberAndLowercaseAndCapitalAndSpecialCharacter = 2
    End Enum

    ''' <summary>
    ''' 密码策略类
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MyPasswordPolicy
        ''' <summary>
        ''' 是否启用了复杂性策略
        ''' </summary>
        ''' <remarks></remarks>
        Public EnabledComplex As Boolean = False
        ''' <summary>
        ''' 是否启用了过期策略
        ''' </summary>
        ''' <remarks></remarks>
        Public EnabledExpiration As Boolean = False
        ''' <summary>
        ''' 是否启用了密码修改策略
        ''' </summary>
        ''' <remarks></remarks>
        Public EnabledPasswordCompare As Boolean = False
        ''' <summary>
        ''' 是否启用了首次登陆必须修改密码策略
        ''' </summary>
        ''' <remarks></remarks>
        Public EnabledFirstLogin As Boolean = False
        ''' <summary>
        ''' 是否启用了用户锁定策略
        ''' </summary>
        ''' <remarks></remarks>
        Public EnabledUserLock As Boolean = False
        ''' <summary>
        ''' 密码最小长度限制
        ''' </summary>
        ''' <remarks></remarks>
        Public MinLength As Int32
        ''' <summary>
        ''' 密码有效天数
        ''' </summary>
        ''' <remarks></remarks>
        Public ExpirationDays As Int32
        ''' <summary>
        ''' 密码到期前提前几天提示
        ''' </summary>
        ''' <remarks></remarks>
        Public AlarmDays As Int32
        ''' <summary>
        ''' 最大允许的登陆密码输入错误数
        ''' </summary>
        ''' <remarks></remarks>
        Public MaxWrongTimes As Int32
        ''' <summary>
        ''' 密码复杂度策略类型
        ''' </summary>
        ''' <remarks></remarks>
        Public ComplexType As PasswordPolicyComplexType = PasswordPolicyComplexType.NumberAndLetter
        ''' <summary>
        ''' 策略的最后一次修改时间，只读。
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly ModifyDateTime As DateTime
        ''' <summary>
        ''' 密码复杂度策略描述，只读。
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property ComplexDescription() As String
            Get
                Select Case Me.ComplexType
                    Case 0
                        Return "数字、字母"
                        Exit Select
                    Case 1
                        Return "数字、大写字母、小写字母"
                        Exit Select
                    Case 2
                        Return "数字、大写字母、小写字母、特殊字符（如：!、@、#、$）"
                        Exit Select
                    Case Else
                        Return "未定义的复杂度类型"
                End Select
            End Get
        End Property

        Private Sub New()
            Dim strSql As String = "SELECT TOP 1 " & _
                                    "MustComplicated," & _
                                    "MustLimitAvailableDay," & _
                                    "MustDifferentPWD," & _
                                    "MustChangePWDFirstLogin," & _
                                    "MustLimitWrongLogin," & _
                                    "MinLength," & _
                                    "AvailableDay," & _
                                    "NotifyDay," & _
                                    "MaxWrongLoginCount," & _
                                    "RequireContent," & _
                                    "ISNULL(CONVERT(varchar(20),ModifyTime,120),'') as ModifyTime " & _
                                    "FROM myPasswordPolicy"
            Dim dt As DataTable = MyDB.GetDataTable(strSql)
            If dt.Rows.Count > 0 Then
                Me.EnabledComplex = IIf(dt.Rows(0)("MustComplicated").ToString = "1", True, False)
                Me.EnabledExpiration = IIf(dt.Rows(0)("MustLimitAvailableDay").ToString = "1", True, False)
                Me.EnabledPasswordCompare = IIf(dt.Rows(0)("MustDifferentPWD").ToString = "1", True, False)
                Me.EnabledFirstLogin = IIf(dt.Rows(0)("MustChangePWDFirstLogin").ToString = "1", True, False)
                Me.EnabledUserLock = IIf(dt.Rows(0)("MustLimitWrongLogin").ToString = "1", True, False)
                Me.MinLength = CInt(dt.Rows(0)("MinLength"))
                Me.ExpirationDays = CInt(dt.Rows(0)("AvailableDay"))
                Me.AlarmDays = CInt(dt.Rows(0)("NotifyDay"))
                Me.MaxWrongTimes = CInt(dt.Rows(0)("MaxWrongLoginCount"))
                Me.ComplexType = CInt(dt.Rows(0)("RequireContent"))
                Me.ModifyDateTime = IIf(dt.Rows(0)("ModifyTime").ToString = "", Nothing, DateTime.Parse(dt.Rows(0)("ModifyTime").ToString))
            End If
        End Sub

        ''' <summary>
        ''' 插入或覆盖当前策略。
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Update()
            Dim strSql As New StringBuilder()
            strSql.Append("DELETE FROM myPasswordPolicy;")
            strSql.Append("INSERT INTO myPasswordPolicy")
            strSql.Append("(")
            strSql.Append(" MustComplicated,")
            strSql.Append(" MustLimitAvailableDay,")
            strSql.Append(" MustDifferentPWD,")
            strSql.Append(" MustChangePWDFirstLogin,")
            strSql.Append(" MustLimitWrongLogin,")
            strSql.Append(" MinLength,")
            strSql.Append(" AvailableDay,")
            strSql.Append(" NotifyDay,")
            strSql.Append(" MaxWrongLoginCount,")
            strSql.Append(" RequireContent,")
            strSql.Append(" ModifyTime")
            strSql.Append(") ")
            strSql.Append("VALUES")
            strSql.Append("(")
            strSql.Append(IIf(Me.EnabledComplex, "1", "0") & ",")
            strSql.Append(IIf(Me.EnabledExpiration, "1", "0") & ",")
            strSql.Append(IIf(Me.EnabledPasswordCompare, "1", "0") & ",")
            strSql.Append(IIf(Me.EnabledFirstLogin, "1", "0") & ",")
            strSql.Append(IIf(Me.EnabledUserLock, "1", "0") & ",")
            strSql.Append(CStr(Me.MinLength) & ",")
            strSql.Append(CStr(Me.ExpirationDays) & ",")
            strSql.Append(CStr(Me.AlarmDays) & ",")
            strSql.Append(CStr(Me.MaxWrongTimes) & ",")
            strSql.Append(CInt(Me.ComplexType).ToString & ",")
            strSql.Append("getDate()")
            strSql.Append(");")
            If MyDB.ExecSQL(strSql.ToString) = -1 Then
                Me._Instance = New MyPasswordPolicy()
                Throw New Exception("操作数据库失败！")
            End If
            Me._Instance = New MyPasswordPolicy()
        End Sub

        Private Shared _Instance As MyPasswordPolicy
        Private Shared ReadOnly _LockHelper = New Object()

        '线程安全
        ''' <summary>
        ''' 获取系统当前密码策略的实例（线程安全），只读。
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Instance() As MyPasswordPolicy
            Get
                If _Instance Is Nothing Then
                    SyncLock _LockHelper
                        If _Instance Is Nothing Then
                            _Instance = New MyPasswordPolicy()
                        End If
                    End SyncLock
                End If
                Return _Instance
            End Get
        End Property

        ''' <summary>
        ''' 锁定用户。
        ''' </summary>
        ''' <param name="userCode">用户代码</param>
        ''' <remarks>不校验用户的存在性</remarks>
        Public Shared Sub LockUser(ByVal userCode As String)
            If MyDB.ExecSQL(String.Format("UPDATE myUser SET IsLocked = 1 WHERE UserCode = '{0}'", userCode)) = -1 Then
                Throw New Exception("操作数据库失败！")
            End If

        End Sub

        ''' <summary>
        ''' 单个用户解锁
        ''' </summary>
        ''' <param name="userName">用户代码</param>
        ''' <remarks>不校验用户的存在性true</remarks>
        Public Shared Sub UnlockUser(ByVal userCode As String)
            If MyDB.ExecSQL(String.Format("UPDATE myUser SET IsLocked = 0 WHERE UserCode = '{0}'", userCode)) = -1 Then
                Throw New Exception("操作数据库失败！")
            End If
            '清除该用户缓存计数
            If Not Mysoft.Map.Caching.MyCache.Get("UserLockHashTable") Is Nothing AndAlso CType(HttpRuntime.Cache.Item("UserLockHashTable"), Hashtable).ContainsKey(userCode) Then
                CType(Mysoft.Map.Caching.MyCache.Get("UserLockHashTable"), Hashtable).Remove(userCode)
            End If
        End Sub

        ''' <summary>
        ''' 批量用户解锁
        ''' </summary>
        ''' <param name="userCode">用户代码数组</param>
        ''' <remarks>不校验用户的存在性true</remarks>
        Public Shared Sub UnlockUser(ByVal userCode() As String)
            If MyDB.ExecSQL(String.Format("UPDATE myUser SET IsLocked = 0 WHERE UserCode in ('{0}')", Join(userCode, "','"))) = -1 Then
                Throw New Exception("操作数据库失败！")
            End If
            '清除该用户缓存计数
            If Not Mysoft.Map.Caching.MyCache.Get("UserLockHashTable") Is Nothing Then
                Dim hashLock As Hashtable = CType(Mysoft.Map.Caching.MyCache.Get("UserLockHashTable"), Hashtable)
                For i As Integer = 0 To userCode.Count - 1
                    If hashLock.ContainsKey(userCode(i)) Then
                        hashLock.Remove(userCode(i))
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' 获取用户帐户的锁状态
        ''' </summary>
        ''' <param name="userCode">用户代码</param>
        ''' <returns>返回用户锁状态</returns>
        ''' <remarks></remarks>
        Public Shared Function GetLockState(ByVal userCode As String) As LockState
            Return MyDB.GetDataItemInt(String.Format("SELECT ISNULL(IsLocked,0) FROM myUser WHERE UserCode ='{0}'", userCode))
        End Function

        ''' <summary>
        ''' 向缓存中写入用户登陆错误次数，返回当前最新的错误次数。
        ''' </summary>
        ''' <param name="userCode">用户名代码</param>
        ''' <returns>返回当前最新的错误次数。</returns>
        ''' <remarks></remarks>
        Public Shared Function AddWrongTimes(ByVal userCode As String) As Int32
            '启用锁定策略
            If MyPasswordPolicy.Instance.EnabledUserLock Then
                Dim lockHelper As New Object()
                '第一次使用或缓存过期时,开辟一个缓存放入哈稀表
                If Mysoft.Map.Caching.MyCache.Get("UserLockHashTable") Is Nothing Then
                    SyncLock _LockHelper
                        If Mysoft.Map.Caching.MyCache.Get("UserLockHashTable") Is Nothing Then
                            Mysoft.Map.Caching.MyCache.Insert("UserLockHashTable", New Hashtable, DateTime.Parse(Now.AddDays(1).Date.ToShortDateString + " 00:00:00"), Cache.NoSlidingExpiration)
                        End If
                    End SyncLock
                End If
                Dim hashLock As Hashtable = CType(Mysoft.Map.Caching.MyCache.Get("UserLockHashTable"), Hashtable)
                '写入错误数
                If hashLock.ContainsKey(userCode) Then
                    hashLock.Item(userCode) = CInt(hashLock.Item(userCode)) + 1
                Else
                    hashLock.Add(userCode, 1)
                End If
                '达到最大次数设置则锁定用户
                If CInt(hashLock.Item(userCode)) >= MyPasswordPolicy.Instance.MaxWrongTimes Then
                    MyPasswordPolicy.LockUser(userCode)
                End If
                Return CInt(hashLock.Item(userCode))
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' 校验密码是否符合当前的系统密码策略，true符合，false不符合。
        ''' </summary>
        ''' <param name="password">明文密码</param>
        ''' <returns>true符合，false不符合。</returns>
        ''' <remarks></remarks>
        Public Shared Function VerifyComplex(ByVal password As String) As Boolean
            '启用复杂性策略
            If MyPasswordPolicy.Instance.EnabledComplex Then
                '长度要求校验
                If Not PolicyLengthUnit.Verify(password, MyPasswordPolicy.Instance.MinLength) Then
                    Return False
                End If
                '复杂度校验
                Select Case MyPasswordPolicy.Instance.ComplexType
                    Case PasswordPolicyComplexType.NumberAndLetter
                        Return PolicyNumberUnit.Verify(password) AndAlso PolicyLetterUnit.Verify(password)
                        Exit Select
                    Case PasswordPolicyComplexType.NumberAndLowercaseAndCapital
                        Return PolicyNumberUnit.Verify(password) AndAlso PolicyLowercaseUnit.Verify(password) AndAlso PolicyCapitalUnit.Verify(password)
                        Exit Select
                    Case PasswordPolicyComplexType.NumberAndLowercaseAndCapitalAndSpecialCharacter
                        Return PolicyNumberUnit.Verify(password) AndAlso PolicyLowercaseUnit.Verify(password) AndAlso PolicyCapitalUnit.Verify(password) AndAlso PolicySpecialCharacterUnit.Verify(password)
                        Exit Select
                    Case Else
                        Throw New NotSupportedException("不支持该复杂性策略类型""" & MyPasswordPolicy.Instance.ComplexType.ToString & """！")
                End Select
            End If

            Return True
        End Function

        ''' <summary>
        ''' 校验用户密码在当前的系统密码策略下是否已过期，true未过期，false过期。
        ''' </summary>
        ''' <param name="userCode">用户代码</param>
        ''' <returns>true未过期，false过期</returns>
        ''' <remarks></remarks>
        Public Shared Function VerifyExpiration(ByVal userCode As String) As Boolean
            '启用有效期策略
            If MyPasswordPolicy.Instance.EnabledExpiration Then
                '获取上次密码修改时间
                Dim strModifyTime As String = MyDB.GetDataItemString(String.Format("SELECT ISNULL(CONVERT(varchar(20),PSWModifyTime,120),'') FROM myUser WHERE UserCode='{0}'", userCode))
                If strModifyTime = "" Then
                    Return False
                End If
                Return PolicyExpirationUnit.Verify(DateTime.Parse(strModifyTime), MyPasswordPolicy.Instance.ExpirationDays)
            End If

            Return True
        End Function

        ''' <summary>
        ''' 校验用户密码修改在当前的系统密码策略下是否符合，true符合，false不符合。
        ''' </summary>
        ''' <param name="oldPassword">旧密码</param>
        ''' <param name="newPassword">新密码</param>
        ''' <returns>true符合，false不符合。</returns>
        ''' <remarks></remarks>
        Public Shared Function VerifyPwdChanged(ByVal oldPassword As String, ByVal newPassword As String) As Boolean
            '启用了有效期和密码比较策略
            If MyPasswordPolicy.Instance.EnabledExpiration AndAlso MyPasswordPolicy.Instance.EnabledPasswordCompare Then
                Return IIf(String.Compare(oldPassword, newPassword, False) = 0, False, True)
            End If

            Return True
        End Function

        ''' <summary>
        ''' 校验用户在当前的系统密码提示策略下是否需要提示修改密码，true不需要提醒，false需要提醒。
        ''' </summary>
        ''' <param name="userCode">用户代码</param>
        ''' <returns>true不需要提醒，false需要提醒。</returns>
        ''' <remarks></remarks>
        Public Shared Function VerifyAlarm(ByVal userCode As String) As Boolean
            '启用了有效期策略
            If MyPasswordPolicy.Instance.EnabledExpiration Then
                '获取上次密码修改时间
                Dim strModifyTime As String = MyDB.GetDataItemString(String.Format("SELECT ISNULL(CONVERT(varchar(20),PSWModifyTime,120),'') FROM myUser WHERE UserCode='{0}'", userCode))
                If strModifyTime = "" Then
                    Return False
                End If
                Return PolicyAlarmUnit.Verify(DateTime.Parse(strModifyTime), MyPasswordPolicy.Instance.ExpirationDays, MyPasswordPolicy.Instance.AlarmDays)
            End If

            Return True
        End Function

        ''' <summary>
        ''' 校验用户在当前的系统密码首次登陆策略下是否需要修改密码，true不需要修改密码，false需要修改密码。
        ''' </summary>
        ''' <param name="userCode">用户代码</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function VerifyFirstLogin(ByVal userCode As String) As Boolean
            '启用了首次登陆修改密码策略
            If MyPasswordPolicy.Instance.EnabledFirstLogin Then
                Return IIf(MyDB.GetDataItemInt(String.Format("SELECT ISNULL(IsUserChangePWD,0) FROM myUser WHERE UserCode='{0}'", userCode)) = 1, True, False)
            End If

            Return True
        End Function
    End Class
End Namespace

