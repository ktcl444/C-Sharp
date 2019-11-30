Imports Mysoft.Map.Data
Imports System.Xml
Imports System.Web
Imports System.Collections.Specialized
Imports System.Data
Imports System.Data.SqlClient
Imports Mysoft.Map.Caching

Namespace Application.Security

    Public Class User

#Region "�����û�����"
        ''' <summary>
        ''' ��ʼ�������û�����
        ''' </summary>
        ''' <remarks>
        ''' ����ArrayList�洢�����û�,�����뻺��,ʧЧʱ��Ϊ7��
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
        ''' ��ӳ����û��������û�������
        ''' </summary>
        ''' <param name="userGUID">�û�GUID</param>
        ''' <remarks></remarks>
        Public Shared Sub AddAdminUserCache(ByVal userGUID As String)
            Dim adminUsersList As ArrayList = GetAdminUsersCache()
            '�������в����������ṩ���û�GUID,�򽫸��û�GUID��ӵ������û�������
            If Not adminUsersList Is Nothing AndAlso Not adminUsersList.Contains(userGUID.ToLower) Then
                adminUsersList.Add(userGUID.ToLower)
            End If
        End Sub

        ''' <summary>
        ''' ���û��ӳ����û��������Ƴ�
        ''' </summary>
        ''' <param name="userGUID">�û�GUID</param>
        ''' <remarks></remarks>
        Public Shared Sub RemoveAdminUserCache(ByVal userGUID As String)
            Dim adminUsersList As ArrayList = GetAdminUsersCache()
            '�������а��������ṩ���û�GUID,�򽫸��û�GUID�ӳ����û��������Ƴ�
            If Not adminUsersList Is Nothing AndAlso adminUsersList.Contains(userGUID.ToLower) Then
                adminUsersList.Remove(userGUID.ToLower)
            End If
        End Sub

        ''' <summary>
        ''' ��ó����û�����
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAdminUsersCache() As ArrayList
            Dim adminUsers As Object = MyCache.[Get]("UserIsAdmin")
            Dim adminUsersList As ArrayList
            '���û�ж�Ӧ������,������ݿ��л�ȡ,�����뻺����
            If adminUsers Is Nothing Then
                InitAdminUsersCache()
                adminUsers = MyCache.[Get]("UserIsAdmin")
            End If
            adminUsersList = CType(adminUsers, ArrayList)
            Return adminUsersList
        End Function
#End Region


        ' ���ܣ��ж��û��Ƿ񳬼��û�
        Public Shared Function IsAdmin(ByVal userGUID As String) As Boolean
            '�ӻ����л�ȡ�����û��б�
            Dim adminUsersList As ArrayList = User.GetAdminUsersCache()
            '��������û��б��а��������ṩ���û�GUID,�򷵻�true
            If Not adminUsersList Is Nothing AndAlso adminUsersList.Contains(userGUID.ToLower) Then
                Return True
            Else
                Return False
            End If
        End Function

        ' ���ܣ��жϵ�ǰ�û��Ƿ�ϵͳ
        ' ���أ�����û����ڷ��� true������Ѿ���ɾ������ false
        Public Shared Function IsSystemUser(ByVal userGUID As String) As Boolean
            If userGUID Is Nothing Then userGUID = Guid.NewGuid.ToString
            If MyDB.GetDataItemInt("SELECT count(*) FROM myUser WHERE UserGUID='" & userGUID.Replace("'", "''") & "'") = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        ' ���ܣ���ȡ�û�Ȩ�޵�
        Public Shared Function GetUserRights(ByVal userGUID As String, ByVal functionCode As String) As NameValueCollection
            Dim SQL As String
            Dim DT As DataTable
            Dim i As Integer
            Dim ReturnValue As New NameValueCollection
            If userGUID Is Nothing Then userGUID = Guid.NewGuid.ToString
            If functionCode Is Nothing Then functionCode = ""
            If IsAdmin(userGUID) Then   '����ǳ����û���Ĭ��ӵ������Ȩ��
                SQL = "SELECT DISTINCT ActionCode FROM myAction WHERE ObjectType='" & functionCode.Replace("'", "''") & "'"
                DT = MyDB.GetDataTable(SQL)
                If DT.Rows.Count > 0 Then
                    For i = 0 To DT.Rows.Count - 1
                        ReturnValue.Add(DT.Rows(i).Item("ActionCode").ToString, "1")
                    Next
                End If
            Else        '��ͨ�û�
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

        ' ���ܣ�����û�������û��Ȩ��ֱ���׳�����
        Public Shared Sub LoadUserRight(ByVal userGUID As String, ByVal functionCode As String, ByVal actionCode As String)
            ' ���Ȩ��
            If userGUID Is Nothing Then userGUID = Guid.NewGuid.ToString
            If functionCode Is Nothing Then functionCode = ""
            If actionCode Is Nothing Then actionCode = ""
            If Not CheckUserRight(userGUID, functionCode, actionCode) Then
                Throw New System.Exception("�Բ�������""" & MyDB.GetDataItemString("SELECT FunctionName FROM myFunction WHERE FunctionCode='" & functionCode.Replace("'", "''") & "'") & _
                                    """ģ���""" & MyDB.GetDataItemString("SELECT ActionName FROM myAction WHERE ActionCode='" & actionCode.Replace("'", "''") & "' AND ObjectType='" & functionCode.Replace("'", "''") & "'") & """Ȩ�ޣ�")
            End If
        End Sub

        ' ���ܣ��ж��û��Ƿ��иö������Ȩ��
        Public Shared Function CheckUserRight(ByVal userGUID As String, ByVal functionCode As String, ByVal actionCode As String) As Boolean
            ' ���У��
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
        ' ���ܣ������û�GUID�͹���ģ���ŷ��� ,ActionCode,ActionCode,ActionCode,...,�ַ���
        ' ˵������ÿ�û����棬����ʹ�� hashtable��ÿ�����ܵ��Ӧһ����¼���������ö���(,)�ָ�
        Public Shared Function GetUserActionRights(ByVal userGUID As String, ByVal functionCode As String) As String
            Dim ACTION_RIGHTS As String = "ActionString_"
            Dim userActionRightsObject As Object
            Dim userActionRightsHashTable As Hashtable
            Dim actionString As String
            Dim CACHEEXP As Integer = MyCache.DayFactor * 7                                 ' �������ʱ��Ϊһ��

            userActionRightsObject = MyCache.Get(ACTION_RIGHTS & userGUID.ToLower)          ' ��ȡ�û��������
            If Not userActionRightsObject Is Nothing Then                                   ' ������ڸ��û���Ȩ�޻���
                userActionRightsHashTable = CType(userActionRightsObject, Hashtable)
                If userActionRightsHashTable.Contains(functionCode) Then                    ' ��������ù��ܻ���
                    actionString = CType(userActionRightsHashTable(functionCode), String)
                Else                                                                        ' ����������ù��ܻ���
                    ' �� hashtable �����ӹ��ܻ���
                    actionString = GetUserActionRightString(userGUID, functionCode)
                    userActionRightsHashTable.Add(functionCode, actionString)               ' HashTable����������,�޸ĵ����ݻ�ֱ�Ӹ��µ�������,����Ҫ��ʽ�������
                End If
            Else                                                                            ' ����������û��������
                ' �� hashtable �����ӹ��ܻ���
                userActionRightsHashTable = New Hashtable
                actionString = GetUserActionRightString(userGUID, functionCode)
                userActionRightsHashTable.Add(functionCode, actionString)
                ' ����û�����
                MyCache.Insert(ACTION_RIGHTS & userGUID.ToLower, userActionRightsHashTable, CACHEEXP)
            End If

            Return actionString
        End Function

        ''' <summary>
        ''' �����ݿ��ȡ�û�����Ȩ���ַ���
        ''' </summary>
        ''' <param name="userGUID">�û�GUID</param>
        ''' <param name="functionCode">���ܴ���</param>
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
                Return "����usp_myGetUserActionRights�洢����ʧ�ܣ�"
            Finally
                SqlConn.Close()
            End Try

            Return sb.ToString()
        End Function

        ' ���ܣ���ȡ�û��� BUGUID �ַ������磺'7AED691F-DC73-4632-949E-110A39B7B5A5'��
        ' ˵�����û��� BUGUID ��һ�����û������ĵ�λ��
        '       ��������û�������λ�ǹ�˾��ô���ص�ǰ��λ������û������ĵ�λ��һ�����ŷ��ظò��������Ĺ�˾��
        Public Shared Function GetUserBUString(ByVal userGUID As String) As String
            If userGUID Is Nothing Then userGUID = Guid.NewGuid.ToString
            Return MyDB.GetDataItemString("SELECT BUGUID FROM myUser WHERE UserGUID = '" & userGUID.Replace("'", "''") & "'")
        End Function

        ' ���ܣ���ȡ�û������µ� BUGUID �ַ������磺'7AED691F-DC73-4632-949E-110A39B7B5A5','7CE7774C-16B9-4558-A271-0FB1F375BA08'��
        ' ˵�������������ż���ĵ�λ��
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
