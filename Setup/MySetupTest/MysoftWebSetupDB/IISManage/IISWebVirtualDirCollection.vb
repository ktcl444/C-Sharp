Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Text

Namespace MySoft.IISManage
    ''' <summary>
    ''' IISWebVirtualDirCollection
    ''' </summary>
    Public Class IISWebVirtualDirCollection
        Inherits CollectionBase

#Region "公共的属性"
        ''' <summary>
        ''' 通过序号进行索引
        ''' </summary>
        Default Public ReadOnly Property Item(ByVal Index As Integer) As IISWebVirtualDir
            Get
                Return CType(Me.List(Index), IISWebVirtualDir)
            End Get
        End Property


        ''' <summary>
        ''' 通过虚拟目录名进行索引
        ''' </summary>
        Default Public ReadOnly Property Item(ByVal name As String) As IISWebVirtualDir
            Get
                name = name.ToLower()
                Dim list As IISWebVirtualDir
                For i As Integer = 0 To Me.List.Count - 1
                    list = CType(Me.List(i), IISWebVirtualDir)
                    If list.Name.ToLower() = name Then
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
        Public Sub Add(ByVal webVirtualDir As IISWebVirtualDir)
            Me.List.Add(webVirtualDir)
        End Sub


        ''' <summary>
        ''' 批量增加集合元素
        ''' </summary>
        Public Sub AddRange(ByVal webVirtualDirs() As IISWebVirtualDir)
            For i As Integer = 0 To webVirtualDirs.Length - 1
                Me.List.Add(webVirtualDirs(i))
            Next i
        End Sub


        ''' <summary>
        ''' 删除集合元素
        ''' </summary>
        Public Sub Remove(ByVal webVirtualDir As IISWebVirtualDir)
            Me.List.Remove(webVirtualDir)
        End Sub


        ''' <summary>
        ''' 是否包含指定名称的虚拟目录
        ''' </summary>
        ''' <param name="ServerComment"></param>
        ''' <returns></returns>
        Public Function Contains(ByVal name As String) As Boolean
            name = name.ToLower().Trim()
            For i As Integer = 0 To Me.List.Count - 1
                Dim virDir As IISWebVirtualDir = Me(i)
                If virDir.Name.ToLower().Trim() = name Then
                    Return True
                End If
            Next i
            Return False
        End Function
#End Region

    End Class
End Namespace
