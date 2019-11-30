Imports Mysoft.Map.Data
Imports System.Xml

Namespace Application.Security
    Public Class Application

        '功能：校验应用系统是否启用
        '参数：appName（应用系统代码），0101-售楼系统, 0102-客服系统 , 0103-会员系统
        '返回：True,false
        Public Shared Function IsAllowApplication(ByVal strAppCode As String) As Boolean
            ' 首先判断是否授权
            If Not VerifyApplication.CheckLicenseObject(strAppCode, "系统") Then
                Return False
            End If

            Dim strSQL As String = "SELECT TOP 1 Application  FROM myApplication WHERE Application='" & strAppCode & "' AND IsDisabled=0"
            If MyDB.GetRowsCount(strSQL) > 0 Then
                Return True
            Else
                Return False
            End If

        End Function

    End Class

End Namespace
