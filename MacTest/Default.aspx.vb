Imports System.Runtime.InteropServices
Imports System.Web


Partial Class _Default
    Inherits System.Web.UI.Page

    <DllImport("Iphlpapi.dll")> _
    Private Shared Function SendARP(ByVal dest As Int32, ByVal host As Int32, ByRef mac As Int64, ByRef length As Int32) As Integer
    End Function
    <DllImport("Ws2_32.dll")> _
    Private Shared Function inet_addr(ByVal ip As String) As Int32
    End Function

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Dim userip As String = Request.UserHostAddress
            Dim strClientIP As String = Request.UserHostAddress.ToString().Trim()
            Dim ldest As Int32 = inet_addr(strClientIP) '目的地的ip
            Dim macinfo As New Int64()
            Dim len As Int32 = 6
            Dim res As Integer = SendARP(ldest, 0, macinfo, len)
            Dim mac_src As String = macinfo.ToString("X")
            If mac_src = "0" Then
                If userip = "127.0.0.1" Then
                    Response.Write("正在访问Localhost!")
                Else
                    Response.Write("欢迎来自IP为" & userip & "的朋友！" & "<br>")
                End If
                Return
            End If
            Do While mac_src.Length < 12
                mac_src = mac_src.Insert(0, "0")
            Loop
            Dim mac_dest As String = ""
            For i As Integer = 0 To 10
                If 0 = (i Mod 2) Then
                    If i = 10 Then
                        mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2))
                    Else
                        mac_dest = "-" & mac_dest.Insert(0, mac_src.Substring(i, 2))
                    End If
                End If
            Next i
            Response.Write("欢迎来自IP为" & userip & "<br>" & ",MAC地址为" & mac_dest & "的朋友！" & "<br>")
        Catch err As Exception
            Response.Write(err.Message)
        End Try
    End Sub

End Class
