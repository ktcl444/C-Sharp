Imports System.Xml
Imports System.Xml.Xsl
Imports System.Xml.XPath
Imports System.Text
Imports System.IO
Imports Mysoft.Map.Data
Imports Mysoft.Map.Application.Security
Imports System.Configuration

Public Class SysNav
    Inherits Mysoft.Map.Application.Controls.AppPage

#Region " Web 窗体设计器生成的代码 "


    '该调用是 Web 窗体设计器所必需的。
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    '注意: 以下占位符声明是 Web 窗体设计器所必需的。
    '不要删除或移动它。
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: 此方法调用是 Web 窗体设计器所必需的
        '不要使用代码编辑器修改它。
        InitializeComponent()
    End Sub

#End Region

    'modified by chenyong 2009-09-01 
    Private Enum UserKind
        ErpUser = 0
        NormalUser = 1
    End Enum

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '在此处放置初始化页的用户代码
        If Not IsPostBack Then
            Content.Text = "<script language=""javascript""> var __xml=""" & getUserNavXml() & """;</script>"
        End If

        Dim strSrc As String = ConfigurationSettings.AppSettings("MainPage_SiteImage")
        Dim strUrl As String = ConfigurationSettings.AppSettings("MainPage_SiteUrl")
        aSite.HRef = strUrl
        imgSite1.Src = strSrc


        'ImgSite.Src = strSrc
        'ImgSite.Value = strUrl

        '重新定位导航
        txtNav.Value = Trim(Request.QueryString("nav"))
    End Sub

    '获取个人导航菜单
    Private Function getUserNavXml() As String

        Dim fileName As String = "/TempFiles/SysNav_" & Session("UserGUID") & ".xml"
        Dim filePath As String = Server.MapPath(fileName)
        Dim s As New StringBuilder
        Dim userType As UserKind

        '用户类型赋值 
        If (Session("UserKind") <> Nothing AndAlso Session("UserKind") = "1") Then
            userType = UserKind.NormalUser
        Else
            userType = UserKind.ErpUser
        End If


        If Not File.Exists(filePath) OrElse Mysoft.Map.Caching.MyCache.Get("SysNav") Is Nothing Then
            Dim xmlDoc As XmlDocument = getSysNavXml()

            Dim isAdmin As Boolean = Mysoft.Map.Application.Security.User.IsAdmin(Session("UserGUID"))
            Dim isShow As String

            'modified by chenyong 2009-08-28 只有ERP用户且是超级用户时才显示
            If isAdmin And userType = UserKind.ErpUser Then        ' 如果超级用户不受功能权限约束，显示所有系统
                isShow = "true"
            Else
                If Application("ISSHOWFUNC_NOSEC") Is Nothing Then
                    Application("ISSHOWFUNC_NOSEC") = ConfigurationSettings.AppSettings("ISSHOWFUNC_NOSEC").ToLower
                End If
                isShow = Application("ISSHOWFUNC_NOSEC")       ' 是否在导航中显示没有权限的系统
            End If

            RemoveRoleNode(xmlDoc, userType, isAdmin, isShow) '限制用户权限菜单

            'txtFlag.Value 
            Dim intReturn As Integer = MyDB.GetRowsCount("SELECT myBusinessUnit.IsEndCompany FROM myUser,myBusinessUnit WHERE myUser.BUGUID=myBusinessUnit.BUGUID AND myBusinessUnit.IsEndCompany='0' AND myUser.UserGUID='" & Session("UserGUID") & "'")

            Dim attr As XmlAttribute
            attr = xmlDoc.CreateAttribute("companycount")
            attr.Value = intReturn
            xmlDoc.DocumentElement.Attributes.SetNamedItem(attr)

            xmlDoc.Save(filePath)
        End If

        Return fileName
    End Function

    'modified by chneyong 2009-09-01 移除权限外的节点
    Private Sub RemoveRoleNode(ByVal xmlDoc As XmlDocument, ByVal userType As UserKind, ByVal isAdmin As Boolean, ByVal isShow As Boolean)
        Dim userApplication As New ArrayList

        If xmlDoc Is Nothing Then
            Throw New Exception("获取XML配置出错，或未配置。")
            Exit Sub
        End If

        '处理ERP节点删除
        If userType = UserKind.ErpUser Then
            'ERP的超级用户直接退出
            If isAdmin Then Exit Sub

            '非ERP的超级用户，且配置成显示无权限菜单，则直接退出
            If isShow Then
                Exit Sub
            Else
                userApplication = GetUserApplication(UserKind.ErpUser, False)
                RemoveNode(xmlDoc, userApplication)
            End If
        Else '处理普通用户节点
            If isAdmin OrElse isShow Then
                userApplication = GetUserApplication(UserKind.NormalUser, True)
                RemoveNode(xmlDoc, userApplication)
            Else
                If Not isShow Then
                    userApplication = GetUserApplication(UserKind.NormalUser, False)

                    RemoveNode(xmlDoc, userApplication)
                End If
            End If
        End If
    End Sub
    'modified by chenyong 2009-09-01 移除XML节点
    Private Sub RemoveNode(ByVal xmlDoc As XmlDocument, ByVal application As ArrayList)
        Dim nodes As XmlNodeList = xmlDoc.SelectNodes("//sys")
        Dim strApplication As String = ""

        For Each n As XmlNode In nodes
            If Not n.Attributes.GetNamedItem("Application") Is Nothing Then
                strApplication = n.Attributes.GetNamedItem("Application").Value

                For Each strApp As String In application
                    If strApplication = strApp Then
                        n.Attributes.GetNamedItem("allow").Value = "1"
                        n.ParentNode.Attributes.GetNamedItem("allow").Value = "1"
                        Exit For
                    End If
                Next
            End If
        Next

        For Each n As XmlNode In nodes
            If n.Attributes("allow").Value = "0" Then
                n.ParentNode.RemoveChild(n)
            End If
        Next
    End Sub


    'modified by chenyong 2009-09-01 取用户的权限，不包括ERP的超级用户的权限。
    Private Function GetUserApplication(ByVal userType As UserKind, ByVal isAdmin As Boolean) As ArrayList
        Dim sql As New System.Text.StringBuilder
        Dim dtTemp As DataTable = Nothing

        Dim result As New ArrayList
        If (Not isAdmin) Then '只有非超级用户才去数据库取权限
            sql.Append("SELECT Distinct f.Application FROM myUserRights ur INNER JOIN myFunction f ON f.FunctionCode=ur.ObjectType ")
            sql.Append(" WHERE  ur.UserGUID='" & Session("UserGUID") & "'")

            'modified by chenyong 2009-08-28 授权体系调整，UserKind=1时即普通用户登陆则只显示“费用管理”、“系统设置”、“工作流管理”
            If userType = UserKind.NormalUser Then
                sql.Append(" AND f.Application in('0206','0000','1001') ")
            End If

            sql.Append(" ORDER BY f.Application")

            dtTemp = MyDB.GetDataTable(sql.ToString)
            If (Not dtTemp Is Nothing) AndAlso (Not dtTemp.Rows Is Nothing) Then
                For Each dr As DataRow In dtTemp.Rows
                    result.Add(dr.Item("Application").ToString)
                Next
            End If
        Else
            If userType = UserKind.NormalUser Then '普通用户的超级权限
                result.Add("0000")
                result.Add("1001")
                result.Add("0206")
            End If
        End If

        Return result
    End Function




    '获取系统导航菜单（全集）
    Private Function getSysNavXml() As XmlDocument

        Dim cache As System.Web.Caching.Cache = HttpRuntime.Cache
        Dim key As String = "SysNav"
        Dim xmlDoc As New XmlDocument

        xmlDoc = cache(key)

        If xmlDoc Is Nothing Then
            '不显示EKP系统，Application为20（胡亚林　2009-2-12）
            Dim sql As New System.Text.StringBuilder
            sql.Append(" SELECT ApplicationName,Application ,ApplicationIcon,IsSelectCompany,HierarchyCode,")
            sql.Append("ParentHierarchyCode,'application' AS type,'FuncNav.aspx' as url,SelectBULevel")
            sql.Append(" FROM myApplication WHERE (not Application  LIKE '20%') AND (not Application  LIKE '30%') AND  isDisabled='0' AND ")
            sql.Append(" Application IN ('" & VerifyApplication.GetLicenseApplicationString.Replace(",", "','") & "')")

            sql.Append(" ORDER BY HierarchyCode")

            Dim dtTemp As DataTable = MyDB.GetDataTable(sql.ToString)

            Dim xmlNode(2) As XmlElement

            xmlDoc = New XmlDocument

            xmlDoc.LoadXml("<nav></nav>")

            Dim root As XmlNode = xmlDoc.DocumentElement
            Dim attr As XmlNode
            Dim n As Integer

            n = dtTemp.Rows.Count
            For I As Integer = 0 To n - 1
                With dtTemp.Rows(I)
                    If dtTemp.Rows(I)("ParentHierarchyCode").ToString = "" Then

                        xmlNode(0) = xmlDoc.CreateElement("sys")
                        attr = xmlDoc.CreateAttribute("text")
                        attr.Value = .Item("ApplicationName").ToString
                        xmlNode(0).Attributes.SetNamedItem(attr)

                        attr = xmlDoc.CreateAttribute("allow")
                        attr.Value = "0"  '默认都没有权限
                        xmlNode(0).Attributes.SetNamedItem(attr)

                        root.AppendChild(xmlNode(0))
                    Else

                        xmlNode(1) = xmlDoc.CreateElement("sys")

                        attr = xmlDoc.CreateAttribute("text")
                        attr.Value = .Item("ApplicationName").ToString
                        xmlNode(1).Attributes.SetNamedItem(attr)

                        attr = xmlDoc.CreateAttribute("littleimg")
                        attr.Value = IIf(.Item("ApplicationIcon").ToString = "", "/images/nav_sys_sales.gif", .Item("ApplicationIcon").ToString)
                        xmlNode(1).Attributes.SetNamedItem(attr)

                        attr = xmlDoc.CreateAttribute("url")
                        attr.Value = .Item("url").ToString
                        xmlNode(1).Attributes.SetNamedItem(attr)

                        attr = xmlDoc.CreateAttribute("Application")
                        attr.Value = .Item("Application").ToString
                        xmlNode(1).Attributes.SetNamedItem(attr)

                        attr = xmlDoc.CreateAttribute("IsSelectCompany")
                        attr.Value = .Item("IsSelectCompany").ToString
                        xmlNode(1).Attributes.SetNamedItem(attr)

                        attr = xmlDoc.CreateAttribute("type")
                        attr.Value = .Item("type").ToString
                        xmlNode(1).Attributes.SetNamedItem(attr)

                        '选择公司级别
                        '0 - 不选公司
                        '1 - 只允许选择末级
                        '2 - 允许选择任何级别公司
                        attr = xmlDoc.CreateAttribute("SelectBULevel")
                        attr.Value = .Item("SelectBULevel").ToString
                        xmlNode(1).Attributes.SetNamedItem(attr)

                        attr = xmlDoc.CreateAttribute("allow")
                        attr.Value = "0"  '默认都没有权限
                        xmlNode(1).Attributes.SetNamedItem(attr)

                        xmlNode(0).AppendChild(xmlNode(1))
                    End If
                End With
            Next

            '删除在TempFiles目录缓存的所有与导航相关的临时文件
            Mysoft.Map.Caching.MyCache.ClearAllSysTempFile()
            Mysoft.Map.Caching.MyCache.ClearAllFuncTempFile()

            cache.Insert(key, xmlDoc, New System.Web.Caching.CacheDependency(HttpContext.Current.Server.MapPath("\bin\License.xml")))
        End If

        Return xmlDoc.Clone
    End Function

End Class
