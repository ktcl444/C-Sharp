Imports Microsoft.VisualBasic
Imports System

Namespace MySoft.IISManage

    ''' <summary>
    ''' 服务器IIS版本
    ''' </summary>
    Public Enum IISVersionEnum
        ''' <summary>
        ''' 未知版本
        ''' </summary>
        Unknown
        ''' <summary>
        ''' 版本IIS 4.0
        ''' </summary>
        IIS4
        ''' <summary>
        ''' 版本 IIS 5.0,5.1
        ''' </summary>
        IIS5
        ''' <summary>
        ''' 版本IIS 6.0
        ''' </summary>
        IIS6
        ''' <summary>
        ''' 版本IIS 7.0
        ''' </summary>
        IIS7
    End Enum


    ''' <summary>
    ''' IISWebServer的状态
    ''' </summary>
    Public Enum IISServerState
        ''' <summary>
        ''' 正在启动
        ''' </summary>
        Starting = 1

        ''' <summary>
        ''' 已启动
        ''' </summary>
        Started = 2

        ''' <summary>
        ''' 正在停止
        ''' </summary>
        Stopping = 3

        ''' <summary>
        ''' 已停止
        ''' </summary>
        Stopped = 4

        ''' <summary>
        ''' 正在暂停
        ''' </summary>
        Pausing = 5

        ''' <summary>
        ''' 已暂停
        ''' </summary>
        Paused = 6

        ''' <summary>
        ''' 正在取消暂停
        ''' </summary>
        Continuing = 7

        ''' <summary>
        ''' 未知
        ''' </summary>
        Unkonwn = 8
    End Enum


End Namespace
