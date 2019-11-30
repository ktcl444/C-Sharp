Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace MySoft.IISManage
    Public Interface IIISWebServer
        '新建网站
        Function Create() As Boolean

        '删除网站
        Sub Delete()

        '更新配置
        Function Update() As Boolean

        '运行
        Sub Start()

        '暂停
        Sub Pause()

        '停止
        Sub [Stop]()

        '取消暂停
        Sub [Continue]()

        '销毁
        Sub Dispose()

        '获得当前状态
        Function GetStatus() As IISServerState

        '检测端口是否重复
        Function CheckPortRepeated() As Boolean

        '设置网站属性
        Sub SetSiteProperties()

        '初始化
        Sub Init(ByVal strMachine As String, ByVal strWebName As String, ByVal strWebPort As String, ByVal strWebDir As String, ByVal strAppFriendlyName As String, ByVal appIsolated As Integer, ByVal strAppPool As String)
    End Interface
End Namespace
