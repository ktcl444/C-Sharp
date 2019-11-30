
Namespace Application.Types

    ''' <summary>
    ''' 页面状态
    ''' </summary>
    Public Enum PageState As Integer
        ''' <summary>
        ''' 无状态
        ''' </summary>
        None = 0
        ''' <summary>
        ''' 新增
        ''' </summary>
        Add = 1
        ''' <summary>
        ''' 修改
        ''' </summary>
        Modify = 2       ' 
        ''' <summary>
        ''' 查看
        ''' </summary>
        Look = 3         ' 
    End Enum

    ''' <summary>
    ''' Form事件
    ''' </summary>
    Public Enum FormEventId As Integer
        ''' <summary>
        ''' 无任何事件被触发
        ''' </summary>
        None = 0
        ''' <summary>
        ''' 保存
        ''' </summary>
        Save = 1
        ''' <summary>
        ''' 保存并新增
        ''' </summary>
        SaveAndClose = 2
        ''' <summary>
        ''' 保存并关闭
        ''' </summary>
        SaveAndNew = 3
        ''' <summary>
        ''' 读数据
        ''' </summary>
        Load = 4
    End Enum

    '
    ''' <summary>
    ''' Lookup 模式
    ''' </summary>
    Public Enum LookupMode As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        None = 0
        ''' <summary>
        ''' 
        ''' </summary>
        Browse = 1
        ''' <summary>
        ''' 
        ''' </summary>
        ShowColumns = 2
        ''' <summary>
        ''' 
        ''' </summary>
        MultiSelect = 4
    End Enum

    ' 软件狗种类
    Friend Enum DogKind As Integer
        Ranbo = 0
        Aladdin = 1
    End Enum

    ' 
    ''' <summary>
    ''' 软件狗用途
    ''' </summary>
    Public Enum DogPurpose As Integer
        ''' <summary>
        ''' 无
        ''' </summary>
        None = 0
        ''' <summary>
        ''' 工具狗
        ''' </summary>
        Tools = 1           ' 
        ''' <summary>
        ''' 普通产品狗
        ''' </summary>
        Normal = 2          ' 
        ''' <summary>
        ''' 带失效时间的产品狗
        ''' </summary>
        Time = 3            ' 
        ''' <summary>
        ''' 自用狗
        ''' </summary>
        ZY = 4              ' 
    End Enum

    ' 
    ''' <summary>
    ''' 组织类型
    ''' </summary>
    Public Enum BUType As Integer
        ''' <summary>
        ''' 无
        ''' </summary>
        None = -1
        ''' <summary>
        ''' 公司
        ''' </summary>
        Company = 0             ' 
        ''' <summary>
        ''' 部门
        ''' </summary>
        Department = 1          ' 
        ''' <summary>
        ''' 团队
        ''' </summary>
        Team = 2                ' 
        ''' <summary>
        ''' 项目团队
        ''' </summary>
        ProjectTeam = 3         ' 
        'Station = 4            ' 岗位
    End Enum

    ' 
    ''' <summary>
    ''' 岗位类型
    ''' </summary>
    Public Enum StationType As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        None = -1
        ''' <summary>
        ''' 单位岗位
        ''' </summary>
        BUStation = 0                   ' 
        ''' <summary>
        ''' 团队岗位
        ''' </summary>
        TeamStation = 1                 ' 
    End Enum

    '功能：
    ''' <summary>
    ''' 数据权限类型
    ''' </summary>
    Public Enum DataRightsDTType
        ''' <summary>
        ''' 仅返回用户的权限的记录
        ''' </summary>
        Basic = 0           ' 
        ''' <summary>
        ''' 返回从根节点到用户有权限节点的记录，即补齐树到根节点——主要针对授权定义为树状结构，如：公司项目授权
        ''' </summary>
        RootTree = 2        ' 
        ''' <summary>
        ''' 返回用户所属公司到用户有权限节点的记录，即补齐树到用户所属公司，如果授权对象没有 _buguid 字段，返回结果同 Basic
        ''' </summary>
        UserBUDeepTree = 3  ' 
    End Enum

    ''' <summary>
    ''' 用户类型
    ''' </summary>
    Public Enum UserKind
        ''' <summary>
        ''' 全部
        ''' </summary>
        All = -1
        ''' <summary>
        ''' ERP用户
        ''' </summary>
        ERP = 0
        ''' <summary>
        ''' 普通用户
        ''' </summary>
        Normal = 1
        ''' <summary>
        ''' 销售系统用户
        ''' </summary>
        Sale = 2
    End Enum
End Namespace
