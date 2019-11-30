Imports Mysoft.Map.Data
Imports System.Web
Imports Mysoft.Map.Caching

Namespace Security
    Public Class OnlineUser

#Region "变量定义"
        Private Shared _htOnlineUsers As Hashtable
        Private Shared ReadOnly _sessionTimeout As Integer
#End Region

#Region "常量定义"
        Private Const ONLINEUSER_CACHE_KEY As String = "OnlineUsers"
#End Region

#Region "构造函数"
        Shared Sub New()
            _sessionTimeout = HttpContext.Current.Session.Timeout
            Dim onlineUsers As Object = MyCache.Get(ONLINEUSER_CACHE_KEY)
            If onlineUsers Is Nothing Then
                _htOnlineUsers = New Hashtable
                MyCache.Max(ONLINEUSER_CACHE_KEY, _htOnlineUsers)
            Else
                _htOnlineUsers = CType(onlineUsers, Hashtable)
            End If
        End Sub
#End Region

#Region "公共函数"
        ''' <summary>
        ''' 添加当前用户到在线用户列表
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public Shared Sub AddCurrentUser()
            UpdateCurrentUser()
            RemoveTimeoutUser()
        End Sub

        ''' <summary>
        ''' 更新当前用户在线信息
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public Shared Sub UpdateCurrentUser()
            If Not HttpContext.Current.Session Is Nothing AndAlso Not HttpContext.Current.Session("UserGUID") Is Nothing Then
                If _htOnlineUsers.ContainsKey(HttpContext.Current.Session("UserGUID")) Then
                    _htOnlineUsers(HttpContext.Current.Session("UserGUID")) = DateTime.Now
                Else
                    _htOnlineUsers.Add(HttpContext.Current.Session("UserGUID"), DateTime.Now)
                End If
            End If
        End Sub

        ''' <summary>
        ''' 获得在线用户列表
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Public Shared Function GetOnlineUserList() As DataSet
            Try
                Dim drUser As DataRow
                Dim dsUserList As DataSet = CreateDataSetStructure()
                For Each user As DictionaryEntry In _htOnlineUsers
                    drUser = GetUserInfo(user.Key)
                    AddOnlineUser(drUser, dsUserList)
                Next
                Return dsUserList
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' 从在线用户列表中移除注稍用户
        ''' </summary>
        ''' <param name="userGUID">用户GUID</param>
        ''' <remarks>
        ''' </remarks>
        Public Shared Sub RemoveLogoutUser(ByVal userGUID As String)
            If String.IsNullOrEmpty(userGUID) Then Exit Sub
            '从在线用户缓存中移除用户
            _htOnlineUsers.Remove(userGUID)
        End Sub
#End Region

#Region "私有函数"
        ''' <summary>
        ''' 从在线用户列表中移除超时用户
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Private Shared Sub RemoveTimeoutUser()
            Dim visitTime As DateTime
            For Each user As DictionaryEntry In _htOnlineUsers
                visitTime = CType(user.Value, DateTime)
                If CheckTimeout(visitTime) Then
                    _htOnlineUsers.Remove(user.Key)
                End If
            Next
        End Sub

        ''' <summary>
        ''' 检测是否超时
        ''' </summary>
        ''' <param name="visitTime">访问时间</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Private Shared Function CheckTimeout(ByVal visitTime As DateTime) As Boolean
            Dim span As TimeSpan = DateTime.Now.Subtract(visitTime)
            Return span.Minutes >= _sessionTimeout
        End Function

        ''' <summary>
        ''' 获得用户信息
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Private Shared Function GetUserInfo(ByVal userGUID As String) As DataRow
            Dim sql As String = String.Empty
            sql = "SELECT CASE WHEN t_user.MobilePhone is null OR t_user.MobilePhone ='' THEN '' ELSE '(移动电话)' + t_user.MobilePhone + ' ' END + " & _
                    "CASE WHEN t_user.officephone is null OR t_user.officephone ='' THEN '' ELSE '(办公电话)' + t_user.officephone + ' ' END + " & _
                    "CASE WHEN t_user.homephone is null OR t_user.homephone ='' THEN '' ELSE '(家庭电话)' + t_user.homephone + ' ' END  as TelePhone, " & _
                    "t_user.UserGUID,t_user.UserName,t_user.userCode, " & _
                    "t_bu.BUName ," & _
                    "case when t_bu_d.BUName is null OR t_bu_d.BUName ='' THEN '' else t_bu_d.BUName end AS DepartmentName " & _
                    "FROM myUser AS t_user left JOIN myBusinessUnit AS t_bu ON t_user.BUGUID=t_bu.BUGUID " & _
                    "left JOIN myBusinessUnit AS t_bu_d ON t_bu_d.BUGUID=t_user.DepartmentGUID " & _
                    "where t_user.UserGUID = '" & userGUID & "'"
            Dim dtUser As DataTable = MyDB.GetDataTable(sql)
            Dim departmentStation As String = String.Empty
            If Not dtUser Is Nothing AndAlso dtUser.Rows.Count > 0 Then
                Return dtUser.Rows(0)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' 添加在线用户
        ''' </summary>
        ''' <param name="drOnlineUser"></param>
        ''' <param name="dsOnlineUser"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Private Shared Function AddOnlineUser(ByVal drOnlineUser As DataRow, ByVal dsOnlineUser As DataSet)
            Dim drOnlineUserNew As DataRow = dsOnlineUser.Tables(0).NewRow

            drOnlineUserNew("UserGUID") = Convert.ToString(drOnlineUser("UserGUID"))
            drOnlineUserNew("UserName") = Convert.ToString(drOnlineUser("UserName"))
            drOnlineUserNew("UserCode") = Convert.ToString(drOnlineUser("UserCode"))
            drOnlineUserNew("BUName") = Convert.ToString(drOnlineUser("BUName"))
            drOnlineUserNew("DepartmentName") = Convert.ToString(drOnlineUser("DepartmentName"))
            drOnlineUserNew("Telephone") = Convert.ToString(drOnlineUser("Telephone"))

            dsOnlineUser.Tables(0).Rows.Add(drOnlineUserNew)
        End Function

        ''' <summary>
        ''' 创建数据集结构
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Private Shared Function CreateDataSetStructure() As DataSet
            Dim dsOnlineUser As DataSet = New DataSet
            Dim dtOnlineUser As DataTable = New DataTable("OnlineUser")

            Dim dcUserGUID As New DataColumn
            dcUserGUID.DataType = System.Type.GetType("System.String")
            dcUserGUID.ColumnName = "UserGUID"
            dtOnlineUser.Columns.Add(dcUserGUID)

            Dim dcUserName As New DataColumn
            dcUserName.DataType = System.Type.GetType("System.String")
            dcUserName.ColumnName = "UserName"
            dtOnlineUser.Columns.Add(dcUserName)

            Dim dcUserCode As New DataColumn
            dcUserCode.DataType = System.Type.GetType("System.String")
            dcUserCode.ColumnName = "UserCode"
            dtOnlineUser.Columns.Add(dcUserCode)

            Dim dcBUName As New DataColumn
            dcBUName.DataType = System.Type.GetType("System.String")
            dcBUName.ColumnName = "BUName"
            dtOnlineUser.Columns.Add(dcBUName)

            Dim dcDepartmentName As New DataColumn
            dcDepartmentName.DataType = System.Type.GetType("System.String")
            dcDepartmentName.ColumnName = "DepartmentName"
            dtOnlineUser.Columns.Add(dcDepartmentName)

            Dim dcTelephone As New DataColumn
            dcTelephone.DataType = System.Type.GetType("System.String")
            dcTelephone.ColumnName = "Telephone"
            dtOnlineUser.Columns.Add(dcTelephone)

            dsOnlineUser.Tables.Add(dtOnlineUser)

            Return dsOnlineUser
        End Function
#End Region

    End Class

End Namespace
