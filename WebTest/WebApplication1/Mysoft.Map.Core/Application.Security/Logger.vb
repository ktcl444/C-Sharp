Imports System.Web
Imports Mysoft.Map.Data

Namespace Application.Security
    Public Class Logger

        'Added by liubao 2007-11-16
        '����һ����¼�û���½��־����
        '�Ժ���������һ����־�ӿ� ILogger
        'д��־ʹ�ô洢����
        Public Shared Sub WriteLoginLog()
            Dim HC As HttpContext = HttpContext.Current

            'modified by chenyong 2009-09-02 ��ֹmyOperLog ��BUGUID�ֶ��ڳ��ֿ��ַ���ֵ(���ֶ�����Ϊuniqueindentifier) ��
            Dim sql As New Text.StringBuilder
            sql.Append(" Insert myOperLog(LogGUID,UserName,FunctionName,InDateTime,InTime,HostIP,BUGUID)")
            sql.Append(" VALUES (NewID() ,'" & HC.Session("UserName") & "','�û���½',getdate()")
            sql.Append(" ,right(convert(varchar(19),getdate(),21),8) ,'" & HC.Request.ServerVariables("REMOTE_ADDR") & "'")

            If Not HC.Session("BUGUID") Is Nothing AndAlso HC.Session("BUGUID").ToString().Trim() <> "" Then
                sql.Append(",'" & HC.Session("BUGUID") & "' ")
            Else
                sql.Append(",null")
            End If

            sql.Append(")")
            Try
                If MyDB.ExecSQL(sql.ToString()) < 0 Then
                    Throw New System.Exception("д�û���½��־ʧ��")
                End If
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)

            End Try
           

        End Sub

        ' д��־
        Public Shared Sub WriteLog(ByVal funcID As String)
            If funcID = "" Or funcID Is Nothing Then
                Throw New System.Exception("д��־ʧ��")
            End If

            Try
                Dim HC As HttpContext = HttpContext.Current
                Dim IsWriteNewLog As Boolean = True

                ' ��ȡ�뿪ʱ�䣬��������־
                If Not HC.Session("LogGUID") Is Nothing And HC.Session("LogGUID") <> "" Then
                    If HC.Session("LogGUID").ToString.Split(";")(1) <> funcID Then      ' ����л���ģ��
                        MyDB.ExecSQL("UPDATE myOperLog SET OutDateTime=getdate(), OutTime=right(convert(varchar(19),getdate(),21),8) WHERE LogGUID='" & HC.Session("LogGUID").ToString.Split(";")(0) & "'")
                        IsWriteNewLog = True
                    Else
                        IsWriteNewLog = False
                    End If
                End If

                ' �����µ���־
                If IsWriteNewLog Then
                    Dim SQL As String
                    Dim LogGUID As String
                    Dim AppAndFuncName(), Application, FunctionName As String
                    Dim BUGUID As String

                    LogGUID = Guid.NewGuid.ToString
                    AppAndFuncName = MyDB.GetDataItemString("SELECT Application+';'+FunctionName FROM myFunction WHERE FunctionCode='" & funcID & "'").Split(";")
                    Application = AppAndFuncName(0)
                    FunctionName = AppAndFuncName(1)
                    If Not HC.Session("BUGUID") Is Nothing And HC.Session("BUGUID") <> "" Then
                        BUGUID = HC.Session("BUGUID")
                    Else
                        BUGUID = MyDB.GetDataItemString("SELECT BUGUID FROM myUser WHERE UserGUID='" & HC.Session("UserGUID") & "'")
                    End If

                    SQL = " INSERT myOperLog(LogGUID,UserName,Application,FunctionCode,FunctionName,InDateTime,InTime,OutDateTime,OutTime,HostIP,BUGUID)" & _
                  " VALUES" & _
                  " ('" & LogGUID & "'" & _
                  " ,'" & HC.Session("UserName") & "'" & _
                  " ,'" & Application & "'" & _
                  " ,'" & funcID & "'" & _
                  " ,'" & FunctionName & "'" & _
                  " ,getdate()" & _
                  " ,right(convert(varchar(19),getdate(),21),8)" & _
                  " ,DATEADD(ss,30,getdate())" & _
                  " ,right(convert(varchar(19),getdate(),21),8)" & _
                  " ,'" & HC.Request.ServerVariables("REMOTE_ADDR") & "'" & _
                  " ,'" & BUGUID & "')"
                    If MyDB.ExecSQL(SQL) < 0 Then
                        Throw New System.Exception("д��־ʧ��")
                    End If

                    HC.Session("LogGUID") = LogGUID & ";" & funcID

                End If

            Catch ex As System.Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Throw New System.Exception("д��־ʧ��")

            End Try

        End Sub

        ' ���ܣ�ɾ����־
        ' ������oids			-- oid �ַ�������� oid ֮���÷ֺ�(;)�ָ�
        '       UserDataXml		-- ����չ����ʱû����
        ' ˵�������������������������
        Public Shared Function DeleteLogs(ByVal oids As String, ByVal UserDataXml As String) As String
            Try
                Dim SQL, WHEREIN As String
                Dim ArrOIDS() As String = oids.Split(";")
                Dim i As Int16

                ArrOIDS = oids.Split(";")
                For i = 0 To ArrOIDS.Length - 1
                    If i = 0 Then
                        WHEREIN += "'" & ArrOIDS(i) & "'"
                    Else
                        WHEREIN += ",'" & ArrOIDS(i) & "'"
                    End If
                Next

                SQL = "DELETE myOperLog WHERE LogGUID IN (" & WHEREIN & ")"
                If MyDB.ExecSQL(SQL) < 0 Then
                    Return "<xml result=""false"" errormessage=""ɾ��ʧ�ܣ�""></xml>"
                Else
                    Return "<xml result=""true""></xml>"
                End If

            Catch ex As System.Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return "<xml result=""false"" errormessage=""����ʱ�쳣���������Ա��ϵ��""></xml>"

            End Try
        End Function

        ' ���ܣ������־
        ' ������oids			-- oid �ַ�������� oid ֮���÷ֺ�(;)�ָ�
        '       UserDataXml		-- ����չ����ʱû����
        ' ˵�������������������������
        Public Shared Function ClearLogs(ByVal oids As String, ByVal UserDataXml As String) As String
            Try
                If MyDB.ExecSQL("DELETE myOperLog") < 0 Then
                    Return "<xml result=""false"" errormessage=""���ʧ�ܣ�""></xml>"
                Else
                    Return "<xml result=""true""></xml>"
                End If

            Catch ex As System.Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return "<xml result=""false"" errormessage=""����ʱ�쳣���������Ա��ϵ��""></xml>"

            End Try
        End Function

    End Class

End Namespace
