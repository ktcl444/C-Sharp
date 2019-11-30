Imports System.Text
Imports System.Data.SqlClient
Imports Mysoft.Map.MyException

Public Class ExceptionHelper

    '''拼接SqlException中的Sql错误信息
    Public Shared Function GetSqlExceptionDesc(ByVal ex As SqlException) As String
        Dim sb As New StringBuilder()
        For Each er As SqlError In ex.Errors
            If er.Message <> "语句已终止。" Then
                If er.Procedure <> "" Then sb.AppendLine("存储过程：" & er.Procedure)
                sb.AppendLine("错误信息：" & er.Message)
                sb.AppendLine("行号：" & er.LineNumber)
                sb.AppendLine("")
            End If
        Next
        Return sb.ToString()
    End Function

    '''拼接JavascriptException中的js错误信息
    Public Shared Function GetJsExceptionDesc(ByVal ex As JavaScriptException) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("异常信息：" & ex.Message)
        sb.AppendLine("异常类型名称：" & ex.Name)
        sb.AppendLine("异常类型号：" & ex.Number)
        sb.AppendLine("异常描述信息：" & ex.Description)
        sb.AppendLine("异常行号：" & ex.LineNumber)
        sb.AppendLine("异常URL：" & ex.Url)
        sb.AppendLine("异常调用栈：" & ex.StackTrace)
        Return sb.ToString()
    End Function

End Class
