Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Text

Namespace MySoft.IISManage
    ''' <summary>
    ''' IISWebServerCollection 
    ''' </summary>
    Public Class IISWebServerCollection
        Inherits CollectionBase

#Region "公共的实例属性"
        ''' <summary>
        ''' 通过序号进行索引
        ''' </summary>
        Default Public ReadOnly Property Item(ByVal Index As Integer) As IISWebServer
            Get
                Return CType(Me.List(Index), IISWebServer)

            End Get
        End Property


        ''' <summary>
        ''' 通过网站名进行索引
        ''' </summary>
        Default Public ReadOnly Property Item(ByVal sitName As String) As IISWebServer
            Get
                sitName = sitName.ToLower().Trim()
                Dim list As IISWebServer
                For i As Integer = 0 To Me.List.Count - 1
                    list = CType(Me.List(i), IISWebServer)
                    If list.SiteName.ToLower().Trim() = sitName Then
                        Return list
                    End If
                Next i
                Return Nothing
            End Get
        End Property
#End Region

#Region "公共的实例方法"
        ''' <summary>
        ''' 添加集合元素
        ''' </summary>
        Public Sub Add(ByVal webServer As IISWebServer)
            Me.List.Add(webServer)
        End Sub


        ''' <summary>
        ''' 批量增加集合元素
        ''' </summary>
        ''' <param name="WebServers"></param>
        Public Sub AddRange(ByVal webServers() As IISWebServer)
            For i As Integer = 0 To webServers.Length - 1
                Me.List.Add(webServers(i))
            Next i
        End Sub


        ''' <summary>
        ''' 删除集合元素
        ''' </summary>
        Public Sub Remove(ByVal webServer As IISWebServer)
            Me.List.Remove(webServer)
        End Sub


        ''' <summary>
        ''' 是否包含指定名称的网站
        ''' </summary>
        Public Function Contains(ByVal webServer As IISWebServer) As Boolean
            Return Me.List.Contains(webServer)
        End Function

        ''' <summary>
        ''' 是否包含指定名称的网站
        ''' </summary>
        ''' <param name="ServerComment"></param>
        ''' <returns></returns>
        Public Function Contains(ByVal sitName As String) As Boolean
            sitName = sitName.ToLower().Trim()
            For i As Integer = 0 To Me.List.Count - 1
                Dim server As IISWebServer = Me(i)
                If server.SiteName.ToLower().Trim() = sitName Then
                    Return True
                End If
            Next i
            Return False
        End Function


#End Region

    End Class
End Namespace
