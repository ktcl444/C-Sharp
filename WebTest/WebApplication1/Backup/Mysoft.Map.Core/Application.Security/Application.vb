Imports Mysoft.Map.Data
Imports System.Xml

Namespace Application.Security
    Public Class Application

        '���ܣ�У��Ӧ��ϵͳ�Ƿ�����
        '������appName��Ӧ��ϵͳ���룩��0101-��¥ϵͳ, 0102-�ͷ�ϵͳ , 0103-��Աϵͳ
        '���أ�True,false
        Public Shared Function IsAllowApplication(ByVal strAppCode As String) As Boolean
            ' �����ж��Ƿ���Ȩ
            If Not VerifyApplication.CheckLicenseObject(strAppCode, "ϵͳ") Then
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
