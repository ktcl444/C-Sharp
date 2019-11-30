Imports Mysoft.Map.Data
Imports System.Xml
Imports System.Web
Imports System.Collections.Specialized
Imports System.Data
Imports System.Data.SqlClient
Imports Mysoft.Map.Caching

Namespace Application.Security

    Public Class User

#Region "超级用户缓存"
        ''' <summary>
        ''' 初始化超级用户缓存
        ''' </summary>
        ''' <remarks>
        ''' 采用ArrayList存储超级用户,并存入缓存,失效时间为7天
        ''' </remarks>
        Public Shared Sub InitAdminUsersCache()
            Dim sql As String = "SELECT UserGUID FROM myUser WHERE IsAdmin = '1'"
            Dim adminUsersDataTable As DataTable = Data.MyDB.GetDataTable(sql)
            Dim adminUsersList As New ArrayList
            For Each adminUser As DataRow In adminUsersDataTable.Rows
                adminUsersList.Add(adminUser("UserGUID").ToString)
            Next
            MyCache.Insert("UserIsAdmin", adminUsersList, MyCache.DayFactor * 7)
        End Sub

        ''' <summary>
        ''' 添加超级用户到超级用户缓存中
        ''' </summary>
        ''' <param name="userGUID">用户GUID</param>
        ''' <remarks></remarks>
        Public Shared Sub AddAdminUserCache(ByVal userGUID As String)
            Dim adminUsersList As ArrayList = GetAdminUsersCache()
            '当缓存中不包含参数提供的用户GUID,则将该用户GUID添加到超级用户缓存中
            If Not adminUsersList Is Nothing AndAlso Not adminUsersList.Contains(userGUID.ToLower) Then
                adminUsersList.Add(userGUID.ToLower)
            End If
        End Sub

        ''' <summary>
        ''' 将用户从超级用户缓存中移除
        ''' </summary>
        ''' <param name="userGUID">用户GUID</param>
        ''' <remarks></remarks>
        Public Shared Sub RemoveAdminUserCache(ByVal userGUID As String)
            Dim adminUsersList As ArrayList = GetAdminUsersCache()
            '当缓存中包含参数提供的用户GUID,则将该用户GUID从超级用户缓存中移除
            If Not adminUsersList Is Nothing AndAlso adminUsersList.Contains(userGUID.ToLower) Then
                adminUsersList.Remove(userGUID.ToLower)
            End If
        End Sub

        ''' <summary>
        ''' 获得超级用户缓存
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAdminUsersCache() As ArrayList
            Dim adminUsers As Object = MyCache.[Get]("UserIsAdmin")
            Dim adminUsersList As ArrayList
            '如果没有对应缓存项,则从数据库中获取,并存入缓存中
            If adminUsers Is Nothing Then
                InitAdminUsersCache()
                adminUsers = MyCache.[Get]("UserIsAdmin")
            End If
            adminUsersList = CType(adminUsers, ArrayList)
            Return adminUsersList
        End Function
#End Region


        ' 功能：判断用户是否超级用户
        Public Shared Function IsAdmin(ByVal userGUID As String) As Boolean
            '从缓存中获取超级用户列表
            Dim adminUsersList As ArrayList = User.GetAdminUsersCache()
            '如果超级用户列表中包含参数提供的用户GUID,则返回true
            If Not adminUsersList Is Nothing AndAlso adminUsersList.Contains(userGUID.ToLower) Then
                Return True
            Else
                Return False
            End If
        End Function

        ' 功能：判断当前用户是否系统
        ' 返回：如果用户存在返回 true，如果已经被删除返回 false
        Public Shared Function IsSystemUser(ByVal userGUID As String) As Boolean
            If userGUID Is Nothing Then userGUID = Guid.NewGuid.ToString
            If MyDB.GetDataItemInt("SELECT count(*) FROM myUser WHERE UserGUID='" & userGUID.Replace("'", "''") & "'") = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        ' 功能：获取用户权限点
        Public Shared Function GetUserRights(ByVal userGUID As String, ByVal functionCode As String) As NameValueCollection
            Dim SQL As String
            Dim DT As DataTable
            Dim i As Integer
            Dim ReturnValue As New NameValueCollection
            If userGUID Is Nothing Then userGUID = Guid.NewGuid.ToString
            If functionCode Is Nothing Then functionCode = ""
            If IsAdmin(userGUID) Then   '如果是超级用户，默认拥有所有权限
                SQL = "SELECT DISTINCT ActionCode FROM myAction WHERE ObjectType='" & functionCode.Replace("'", "''") & "'"
                DT = MyDB.GetDataTable(SQL)
                If DT.Rows.Count > 0 Then
                    For i = 0 To DT.Rows.Count - 1
                        ReturnValue.Add(DT.Rows(i).Item("ActionCode").ToString, "1")
                    Next
                End If
            Else        '普通用户
                'SQL = "SELECT DISTINCT ActionCode FROM myUserRights WHERE UserGUID='" & userGUID.Replace("'", "''") & "' AND ObjectType='" & functionCode.Replace("'", "''") & "'"
                'DT = MyDB.GetDataTable(SQL)
                'If DT.Rows.Count > 0 Then
                '    For i = 0 To DT.Rows.Count - 1
                '        ReturnValue.Add(DT.Rows(i).Item("ActionCode").ToString, "1")
                '    Next
                'End If
                '' modified by liubao 2007-11-15
                Dim myActionRightString As String = GetUserActionRights(userGUID, functionCode).TrimStart(",").TrimEnd(",")
                If Not String.IsNullOrEmpty(myActionRightString) Then
                    Dim myActionRightArray() As String = myActionRightString.Split(",")
                    For i = 0 To myActionRightArray.Length - 1
                        ReturnValue.Add(myActionRightArray(i), "1")
                    Next
                End If
            End If

            Return ReturnValue

        End Function

        ' 功能：检查用户如果如果没有权限直接抛出错误
        Public Shared Sub LoadUserRight(ByVal userGUID As String, ByVal functionCode As String, ByVal actionCode As String)
            ' 检查权限
            If userGUID Is Nothing Then userGUID = Guid.NewGuid.ToString
            If functionCode Is Nothing Then functionCode = ""
            If actionCode Is Nothing Then actionCode = ""
            If Not CheckUserRight(userGUID, functionCode, actionCode) Then
                Throw New System.Exception("对不起，您无""" & MyDB.GetDataItemString("SELECT FunctionName FROM myFunction WHERE FunctionCode='" & functionCode.Replace("'", "''") & "'") & _
                                    """模块的""" & MyDB.GetDataItemString("SELECT ActionName FROM myAction WHERE ActionCode='" & actionCode.Replace("'", "''") & "' AND ObjectType='" & functionCode.Replace("'", "''") & "'") & """权限！")
            End If
        End Sub

        ' 功能：判断用户是否有该动作点的权限
        Public Shared Function CheckUserRight(ByVal userGUID As String, ByVal functionCode As String, ByVal actionCode As String) As Boolean
            ' 随机校验
            VerifyApplication.VerifyRandom()
            Dim strActionString As String = GetUserActionRights(userGUID, functionCode)
            If strActionString.IndexOf("," & actionCode & ",") > -1 Then
                Return True
            Else
                If IsAdmin(userGUID) Then
                    Return True
                End If
                Return False
            End If
        End Function

        ' Added by liubao 2007-11-15
        ' 功能：根据用户GUID和功能模块标号返回 ,ActionCode,ActionCode,ActionCode,...,字符串
        ' 说明：按每用户缓存，缓存使用 hashtable，每个功能点对应一条记录，动作点用逗号(,)分隔
        Public Shared Function GetUserActionRights(ByVal userGUID As String, ByVal functionCode As String) As String
            Dim ACTION_RIGHTS As String = "ActionString_"
            Dim userActionRightsObject As Object
            Dim userActionRightsHashTable As Hashtable
            Dim actionString As String
            Dim CACHEEXP As Integer = MyCache.DayFactor * 7                                 ' 缓存过期时间为一周

            userActionRightsObject = MyCache.Get(ACTION_RIGHTS & userGUID.ToLower)          ' 获取用户缓存对象
            If Not userActionRightsObject Is Nothing Then                                   ' 如果存在该用户的权限缓存
                userActionRightsHashTable = CType(userActionRightsObject, Hashtable)
                If userActionRightsHashTable.Contains(functionCode) Then                    ' 如果包含该功能缓存
                    actionString = CType(userActionRightsHashTable(functionCode), String)
                Else                                                                        ' 如果不包含该功能缓存
                    ' 在 hashtable 中增加功能缓存
                    actionString = GetUserActionRightString(userGUID, functionCode)
                    userActionRightsHashTable.Add(functionCode, actionString)               ' HashTable是引用类型,修改的内容会直接更新到缓存中,不需要显式插入更新
                End If
            Else                                                                            ' 如果不存在用户缓存对象
                ' 在 hashtable 中增加功能缓存
                userActionRightsHashTable = New Hashtable
                actionString = GetUserActionRightString(userGUID, functionCode)
                userActionRightsHashTable.Add(functionCode, actionString)
                ' 添加用户缓存
                MyCache.Insert(ACTION_RIGHTS & userGUID.ToLower, userActionRightsHashTable, CACHEEXP)
            End If

            Return actionString
        End Function

        ''' <summary>
        ''' 从数据库获取用户动作权限字符串
        ''' </summary>
        ''' <param name="userGUID">用户GUID</param>
        ''' <param name="functionCode">功能代码</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetUserActionRightString(ByVal userGUID As String, ByVal functionCode As String) As String
            Dim SqlConn As SqlClient.SqlConnection
            Dim SqlComm As SqlClient.SqlCommand
            Dim SqlParams As SqlParameterCollection
            Dim sqlDateReader As SqlDataReader
            Dim sb As System.Text.StringBuilder = New System.Text.StringBuilder
            sb.Append(",")

            SqlConn = New SqlConnection(MyDB.GetSqlConnectionString)
            SqlComm = New SqlCommand("usp_myGetUserActionRights", SqlConn)
            SqlComm.CommandType = CommandType.StoredProcedure

            SqlParams = SqlComm.Parameters

            SqlParams.Add(New SqlParameter("@chvUserGUID", SqlDbType.VarChar, 40))
            SqlParams.Add(New SqlParameter("@chvFunctionCode", SqlDbType.VarChar, 50))

            SqlParams("@chvUserGUID").Value = userGUID
            SqlParams("@chvFunctionCode").Value = functionCode

            Try
                SqlConn.Open()
                sqlDateReader = SqlComm.ExecuteReader()
                Do While sqlDateReader.Read()
                    sb.Append(sqlDateReader.GetString(0))
                    sb.Append(",")
                Loop
            Catch ex As System.Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return "调用usp_myGetUserActionRights存储过程失败！"
            Finally
                SqlConn.Close()
            End Try

            Return sb.ToString()
        End Function

        ' 功能：获取用户的 BUGUID 字符串，如：'7AED691F-DC73-4632-949E-110A39B7B5A5'。
        ' 说明：用户的 BUGUID 不一定是用户所属的单位，
        '       即，如果用户所属单位是公司那么返回当前单位，如果用户所属的单位是一个部门返回该部门所属的公司。
        Public Shared Function GetUserBUString(ByVal userGUID As String) As String
            If userGUID Is Nothing Then userGUID = Guid.NewGuid.ToString
            Return MyDB.GetDataItemString("SELECT BUGUID FROM myUser WHERE UserGUID = '" & userGUID.Replace("'", "''") & "'")
        End Function

        ' 功能：获取用户即以下的 BUGUID 字符串，如：'7AED691F-DC73-4632-949E-110A39B7B5A5','7CE7774C-16B9-4558-A271-0FB1F375BA08'。
        ' 说明：不包括部门级别的单位。
        Public Shared Function GetUserDeepBUString(ByVal userGUID As String) As String
            Dim SQL As String
            Dim DT As DataTable
            Dim sReturn As String
            Dim i As Integer

            sReturn = ""
            If userGUID Is Nothing Then userGUID = Guid.NewGuid.ToString
            SQL = "SELECT BUGUID FROM myBusinessUnit " & _
                  " WHERE CHARINDEX((select HierarchyCode + '.' from myBusinessUnit bu " & _
                  " left join myUser us on bu.BUGUID=us.BUGUID " & _
                  " where UserGUID='" & userGUID.Replace("'", "''") & "'),HierarchyCode + '.') = 1"
            DT = MyDB.GetDataTable(SQL)
            For i = 0 To DT.Rows.Count - 1
                If i = 0 Then
                    sReturn = "'" & DT.Rows(i)("BUGUID").ToString & "'"
                Else
                    sReturn += ",'" & DT.Rows(i)("BUGUID").ToString & "'"
                End If
            Next

            Return sReturn

        End Function


    End Class

End Namespace
