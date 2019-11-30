Imports System.Web
Imports Mysoft.Map.Data

Namespace Application.Security
    Public Class Logger

        'Added by liubao 2007-11-16
        '增加一个记录用户登陆日志函数
        '以后期望增加一个日志接口 ILogger
        '写日志使用存储过程
        Public Shared Sub WriteLoginLog()
            Dim HC As HttpContext = HttpContext.Current

            'modified by chenyong 2009-09-02 防止myOperLog 表BUGUID字段内出现空字符串值(该字段类型为uniqueindentifier) 。
            Dim sql As New Text.StringBuilder
            sql.Append(" Insert myOperLog(LogGUID,UserName,FunctionName,InDateTime,InTime,HostIP,BUGUID)")
            sql.Append(" VALUES (NewID() ,'" & HC.Session("UserName") & "','用户登陆',getdate()")
            sql.Append(" ,right(convert(varchar(19),getdate(),21),8) ,'" & HC.Request.ServerVariables("REMOTE_ADDR") & "'")

            If Not HC.Session("BUGUID") Is Nothing AndAlso HC.Session("BUGUID").ToString().Trim() <> "" Then
                sql.Append(",'" & HC.Session("BUGUID") & "' ")
            Else
                sql.Append(",null")
            End If

            sql.Append(")")
            Try
                If MyDB.ExecSQL(sql.ToString()) < 0 Then
                    Throw New System.Exception("写用户登陆日志失败")
                End If
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)

            End Try
           

        End Sub

        ' 写日志
        Public Shared Sub WriteLog(ByVal funcID As String)
            If funcID = "" Or funcID Is Nothing Then
                Throw New System.Exception("写日志失败")
            End If

            Try
                Dim HC As HttpContext = HttpContext.Current
                Dim IsWriteNewLog As Boolean = True

                ' 获取离开时间，并更新日志
                If Not HC.Session("LogGUID") Is Nothing And HC.Session("LogGUID") <> "" Then
                    If HC.Session("LogGUID").ToString.Split(";")(1) <> funcID Then      ' 如果切换了模块
                        MyDB.ExecSQL("UPDATE myOperLog SET OutDateTime=getdate(), OutTime=right(convert(varchar(19),getdate(),21),8) WHERE LogGUID='" & HC.Session("LogGUID").ToString.Split(";")(0) & "'")
                        IsWriteNewLog = True
                    Else
                        IsWriteNewLog = False
                    End If
                End If

                ' 插入新的日志
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
                        Throw New System.Exception("写日志失败")
                    End If

                    HC.Session("LogGUID") = LogGUID & ";" & funcID

                End If

            Catch ex As System.Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Throw New System.Exception("写日志失败")

            End Try

        End Sub

        ' 功能：删除日志
        ' 参数：oids			-- oid 字符串，多个 oid 之间用分号(;)分隔
        '       UserDataXml		-- 待扩展，暂时没有用
        ' 说明：函数必须包含这两个参数
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
                    Return "<xml result=""false"" errormessage=""删除失败！""></xml>"
                Else
                    Return "<xml result=""true""></xml>"
                End If

            Catch ex As System.Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return "<xml result=""false"" errormessage=""运行时异常，请与管理员联系！""></xml>"

            End Try
        End Function

        ' 功能：清空日志
        ' 参数：oids			-- oid 字符串，多个 oid 之间用分号(;)分隔
        '       UserDataXml		-- 待扩展，暂时没有用
        ' 说明：函数必须包含这两个参数
        Public Shared Function ClearLogs(ByVal oids As String, ByVal UserDataXml As String) As String
            Try
                If MyDB.ExecSQL("DELETE myOperLog") < 0 Then
                    Return "<xml result=""false"" errormessage=""清空失败！""></xml>"
                Else
                    Return "<xml result=""true""></xml>"
                End If

            Catch ex As System.Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return "<xml result=""false"" errormessage=""运行时异常，请与管理员联系！""></xml>"

            End Try
        End Function

    End Class

End Namespace
