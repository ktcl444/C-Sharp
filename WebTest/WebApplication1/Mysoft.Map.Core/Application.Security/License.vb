Imports System.Web
Imports System.Data
Imports System.Xml
Imports System.Text
Imports System.Security.Cryptography
Imports Mysoft.Map.Data

Namespace Application.Security
    Public Class License
        'Private _usercount As Integer
        'Private _projectcount As Integer
        'Private _companycount As Integer
        Private _licenseXmlDocument As XmlDocument
        Private _maxcount As Integer = 999999
        Private _licensemode As Integer = 0

        Public Sub New()
            Me.New(False)

        End Sub

        ' 功能：实例化授权对象
        ' 参数：isLoadLicenseFile     －是否从 License.xml 中读取信息，否则从 Application 对象中取
        Public Sub New(ByVal isLoadLicenseFile As Boolean)
            Dim xmlDoc As XmlDocument
            Dim xmlNData As XmlNode

            Try
                If HttpContext.Current.Application("_licenseXmlDocument") Is Nothing Or isLoadLicenseFile Then
                    xmlDoc = New XmlDocument
                    xmlDoc.Load(HttpContext.Current.Server.MapPath("/bin/") & "License.xml")

                    xmlNData = xmlDoc.DocumentElement.SelectSingleNode("data")

                    _licenseXmlDocument = New XmlDocument
                    _licenseXmlDocument.LoadXml(xmlNData.InnerXml)

                    HttpContext.Current.Application("_licenseXmlDocument") = _licenseXmlDocument
                Else
                    _licenseXmlDocument = CType(HttpContext.Current.Application("_licenseXmlDocument"), XmlDocument)
                End If
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Throw New System.Exception("读取授权文件失败！")
            End Try
        End Sub

        ' ============================ 方法 ===============================

        ' 功能：校验授权
        Public Function CheckLicenseObject(ByVal objectCode As String, ByVal objectType As String) As Boolean
            '2009-9-21 huyl 增加对一级系统的判断
            Dim node As XmlNode
            Select Case objectType
                Case "一级系统" '校验对象为一级系统
                    node = _licenseXmlDocument.SelectSingleNode("/product/system[@code=""" & objectCode & """]")
                    Exit Select

                Case "系统" ' 校验对象为子系统
                    node = _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & objectCode & """]")
                    Exit Select

                Case Else '校验对象为子系统功能模块
                    node = _licenseXmlDocument.SelectSingleNode("/product/system/subsystem/function[@code=""" & objectCode & """]")
                    Exit Select

            End Select

            If node Is Nothing Then '当前节点不存在，返回False
                Return False
            ElseIf node.Attributes("enabled") Is Nothing Then '当前节点不存在;enabled属性，返回False
                Return False
            ElseIf node.Attributes("enabled").Value = "1" Then '当前节点的enabled属性值为1，返回True
                Return True
            Else '当前节点的enabled属性值不为1，返回False
                Return False
            End If

        End Function

        ' 功能：获取授权应用系统代码
        ' 返回：01,0101,0102,02,0201
        ' 注意：如果没有授权返回空字符串
        Public Function GetLicenseApplicationString() As String
            Dim ApplicationNodeList As XmlNodeList
            Dim ApplicationInLicense As New StringBuilder         ' 包含在 License 文件中所有系统列表
            Dim LicenseApplication As New StringBuilder           ' 被授权的系统列表
            Dim i As Integer

            ' 1、拼写授权系统
            ' 1.1、系统
            ApplicationNodeList = _licenseXmlDocument.SelectNodes("/product/system")
            For i = 0 To ApplicationNodeList.Count - 1
                ApplicationInLicense.Append("," & ApplicationNodeList.Item(i).Attributes("code").Value)

                If Not ApplicationNodeList.Item(i).Attributes("enabled") Is Nothing Then
                    If ApplicationNodeList.Item(i).Attributes("enabled").Value = "1" Then
                        LicenseApplication.Append("," & ApplicationNodeList.Item(i).Attributes("code").Value)

                    End If

                End If

            Next

            ' 1.2、子系统
            ApplicationNodeList = _licenseXmlDocument.SelectNodes("/product/system/subsystem")
            For i = 0 To ApplicationNodeList.Count - 1
                ApplicationInLicense.Append("," & ApplicationNodeList.Item(i).Attributes("code").Value)

                If Not ApplicationNodeList.Item(i).Attributes("enabled") Is Nothing Then
                    If ApplicationNodeList.Item(i).Attributes("enabled").Value = "1" Then
                        LicenseApplication.Append("," & ApplicationNodeList.Item(i).Attributes("code").Value)

                    End If

                End If

            Next

            ' 2、拼写授权文件中不存在的系统
            Dim DT As DataTable
            Dim SQL As String

            SQL = "SELECT Application FROM myApplication WHERE Application NOT IN ('" & ApplicationInLicense.ToString.Replace(",", "','") & "')"
            DT = MyDB.GetDataTable(SQL)
            If DT.Rows.Count > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    LicenseApplication.Append("," & DT.Rows(i)("Application"))

                Next

            End If

            If LicenseApplication.ToString = "" Then
                Return ""

            Else
                Return Right(LicenseApplication.ToString, LicenseApplication.ToString.Length - 1)

            End If
        End Function

        ' 功能：获取授权功能代码
        ' 参数：application     －系统代码
        '       level           －级别。如果－1表示所有级别
        ' 返回：01010201,01010202
        ' 说明：排除虚拟功能和报表
        ' 注意：如果没有授权返回空字符串
        Public Function GetLicenseFunctionString(ByVal application As String) As String
            Return GetLicenseFunctionString(application, -1)

        End Function

        Public Function GetLicenseFunctionString(ByVal application As String, ByVal level As Integer) As String
            Dim FunctionNodeList As XmlNodeList
            Dim FunctionInLicense As New StringBuilder
            Dim LicenseFunction As New StringBuilder
            Dim i As Integer

            ' 1、获取授权的功能
            FunctionNodeList = _licenseXmlDocument.SelectNodes("/product/system/subsystem[@code=""" & application & """]/function")
            For i = 0 To FunctionNodeList.Count - 1
                FunctionInLicense.Append("," & FunctionNodeList.Item(i).Attributes("code").Value)

                If Not FunctionNodeList.Item(i).Attributes("enabled") Is Nothing Then
                    If FunctionNodeList.Item(i).Attributes("enabled").Value = "1" And IIf(level = -1, True, FunctionNodeList.Item(i).Attributes("level").Value = level) Then
                        LicenseFunction.Append("," & FunctionNodeList.Item(i).Attributes("code").Value)

                    End If

                End If

            Next

            ' 2、获取授权文件中不包含的功能
            Dim DT As DataTable
            Dim SQL As String

            SQL = "SELECT FunctionCode FROM myFunction WHERE FunctionCode NOT IN ('" & FunctionInLicense.ToString.Replace(",", "','") & "') " & _
                  " AND Application = '" & application & "' AND FunctionType='功能'"
            DT = MyDB.GetDataTable(SQL)
            If DT.Rows.Count > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    LicenseFunction.Append("," & DT.Rows(i)("FunctionCode"))

                Next

            End If

            If LicenseFunction.ToString = "" Then
                Return ""

            Else
                Return Right(LicenseFunction.ToString, LicenseFunction.ToString.Length - 1)

            End If

        End Function

        ' 功能：获取 License 文件 XmlDocument
        Public Function GetLicenseXmlDocument() As XmlDocument
            Return Me._licenseXmlDocument
        End Function

        ' ============================ 属性 ===============================

        ' 功能：数据库连接的注册表名称
        Public ReadOnly Property RegName(ByVal appName As String) As String
            Get
                Dim DBConnNode As XmlNode = _licenseXmlDocument.SelectSingleNode("/product/dbconns/dbconn[@appname=""" & appName & """]")

                If DBConnNode Is Nothing Then
                    Return ""

                Else
                    Return DBConnNode.Attributes("regname").Value

                End If

            End Get

        End Property

        ' 功能：授权公司数
        ' 说明：如果返回 0 表示无限制
        Public ReadOnly Property CompanyCount() As Integer
            Get
                Dim _CompanyCount As Integer

                If LicenseMode = 1 Then
                    If _licenseXmlDocument.SelectSingleNode("/product").Attributes("companycount").Value = "" Then
                        _CompanyCount = _maxcount

                    Else
                        _CompanyCount = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product").Attributes("companycount").Value)
                        If _CompanyCount <= 0 Then
                            _CompanyCount = _maxcount

                        End If

                    End If

                Else
                    If _licenseXmlDocument.SelectSingleNode("/product").Attributes("companycount") Is Nothing Then
                        _CompanyCount = _maxcount

                    ElseIf _licenseXmlDocument.SelectSingleNode("/product").Attributes("companycount").Value = "" Then
                        _CompanyCount = _maxcount

                    ElseIf _licenseXmlDocument.SelectSingleNode("/product").Attributes("companycount").Value = "-1" Then
                        _CompanyCount = _maxcount

                    Else
                        _CompanyCount = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product").Attributes("companycount").Value)
                        If _CompanyCount < 0 Then
                            Throw New System.Exception("License 文件中公司数授权错误！")

                        End If

                    End If

                End If

                Return _CompanyCount

            End Get

        End Property

        ' 功能：解释License文件中配置的用户数
        ' 修改：陈勇 2011-03-10 销售系统用户调整
        ' 说明：传入的属性取值为“不存在/=空/<0”表示无限用户数
        Private Function GetUserCount(ByVal strAttrKeyName As String) As Integer
            Dim _userCount As Integer
            Dim _attrNormalCnt As Xml.XmlAttribute = _licenseXmlDocument.SelectSingleNode("/product").Attributes(strAttrKeyName)

            If LicenseMode = 3 Then
                If _attrNormalCnt Is Nothing OrElse _attrNormalCnt.Value = "" Then
                    _userCount = _maxcount
                Else
                    _userCount = Convert.ToInt32(_attrNormalCnt.Value)

                    If _userCount < 0 Then
                        _userCount = _maxcount
                    End If
                End If
            End If

            Return _userCount
        End Function

        ' 功能：解释License文件中配置的普通用户数
        ' 修改：陈勇 2009-08-14
        ' 说明：NormalUserCount 属性“不存在/=空/<0”表示无限用户数
        Public ReadOnly Property NormalUserCount() As Integer
            Get
                Return GetUserCount("normalusercount")

                'Dim _normalUserCount As Integer
                'Dim _attrNormalCnt As Xml.XmlAttribute = _licenseXmlDocument.SelectSingleNode("/product").Attributes("normalusercount")

                'If LicenseMode = 3 Then
                '    If _attrNormalCnt Is Nothing OrElse _attrNormalCnt.Value = "" Then
                '        _normalUserCount = _maxcount
                '    Else
                '        _normalUserCount = Convert.ToInt32(_attrNormalCnt.Value)

                '        If _normalUserCount < 0 Then
                '            _normalUserCount = _maxcount
                '        End If
                '    End If
                'End If

                'Return _normalUserCount
            End Get
        End Property

        ' 功能：解释License文件中配置的销售系统用户数
        ' 修改：陈勇 2011-03-10 销售系统用户调整
        ' 说明：SaleUserCount 属性“不存在/=空/<0”表示无限用户数
        Public ReadOnly Property SaleUserCount() As Integer
            Get
                Return GetUserCount("slusercount")
                'Dim _saleUserCount As Integer
                'Dim _attrNormalCnt As Xml.XmlAttribute = _licenseXmlDocument.SelectSingleNode("/product").Attributes("sluserCount")

                'If LicenseMode = 3 Then
                '    If _attrNormalCnt Is Nothing OrElse _attrNormalCnt.Value = "" Then
                '        _saleUserCount = _maxcount
                '    Else
                '        _saleUserCount = Convert.ToInt32(_attrNormalCnt.Value)

                '        If _saleUserCount < 0 Then
                '            _saleUserCount = _maxcount
                '        End If
                '    End If
                'End If

                'Return _saleUserCount
            End Get
        End Property

        ' 功能：解释License文件中配置的Erp用户数
        ' 修改：陈勇 2009-08-14
        ' 说明：ErpUserCount 属性“不存在/=空/<0”表示无限用户数
        Public ReadOnly Property ErpUserCount() As Integer
            Get
                Return GetUserCount("erpusercount")
                'Dim _erpUserCount As Integer
                'Dim _attrErpCnt As Xml.XmlAttribute = _licenseXmlDocument.SelectSingleNode("/product").Attributes("erpusercount")

                'If LicenseMode = 3 Then
                '    If _attrErpCnt Is Nothing OrElse _attrErpCnt.Value = "" Then
                '        _erpUserCount = _maxcount
                '    Else
                '        _erpUserCount = Convert.ToInt32(_attrErpCnt.Value)

                '        If _erpUserCount < 0 Then
                '            _erpUserCount = _maxcount
                '        End If
                '    End If
                'End If

                'Return _erpUserCount
            End Get
        End Property


        ' 功能：授权项目数
        ' 说明：如果返回 0 表示无限制
        Public ReadOnly Property ProjectCount() As Integer
            Get
                Dim _ProjectCount As Integer

                If LicenseMode = 1 Then
                    If _licenseXmlDocument.SelectSingleNode("/product").Attributes("projectcount").Value = "" Then
                        _ProjectCount = _maxcount

                    Else
                        _ProjectCount = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product").Attributes("projectcount").Value)
                        If _ProjectCount <= 0 Then
                            _ProjectCount = _maxcount

                        End If

                    End If

                Else
                    If _licenseXmlDocument.SelectSingleNode("/product").Attributes("projectcount") Is Nothing Then
                        _ProjectCount = _maxcount

                    ElseIf _licenseXmlDocument.SelectSingleNode("/product").Attributes("projectcount").Value = "" Then
                        _ProjectCount = _maxcount

                    ElseIf _licenseXmlDocument.SelectSingleNode("/product").Attributes("projectcount").Value = "-1" Then
                        _ProjectCount = _maxcount

                    Else
                        _ProjectCount = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product").Attributes("projectcount").Value)
                        If _ProjectCount < 0 Then
                            Throw New System.Exception("License 文件中项目数授权错误！")

                        End If

                    End If

                End If

                Return _ProjectCount

            End Get

        End Property

        ' 功能：License 文件冗余码在软件狗中存储的起始位置（仅对 Aladdin）
        ' 说明：默认位置是 0
        Public ReadOnly Property Crc32ReadPos() As Integer
            Get
                Dim _Crc32ReadPos As Integer

                If _licenseXmlDocument.SelectSingleNode("/product").Attributes("dogpos") Is Nothing Then
                    _Crc32ReadPos = 0

                Else
                    _Crc32ReadPos = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product").Attributes("dogpos").Value)

                End If

                Return _Crc32ReadPos

            End Get

        End Property

        ' 功能：授权模式
        ' 返回：大于 0 的整数
        ' 说明：从 ERP1.0~ERP2.5 SP1，LicenseMode = 1；之后版本 LicenseMode = 2。
        '       如果 licensemode 节点不存在，默认 1。
        Public ReadOnly Property LicenseMode() As Integer
            Get
                If _licensemode = 0 Then
                    If _licenseXmlDocument.SelectSingleNode("/product").Attributes("licensemode") Is Nothing Then
                        _licensemode = 1

                    Else
                        _licensemode = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product").Attributes("licensemode").Value)

                    End If

                End If

                Return _licensemode

            End Get

        End Property

#Region "注释的代码"
        ' 功能：总授权用户数
        ' 说明：usercount 属性“不存在/=空/<=0”表示无限用户数
        'Public ReadOnly Property UserCount() As Integer
        '    Get
        '        Dim _UserCount As Integer

        '        If LicenseMode = 1 Then
        '            If _licenseXmlDocument.SelectSingleNode("/product").Attributes("usercount") Is Nothing Then
        '                _UserCount = _maxcount
        '            Else
        '                If _licenseXmlDocument.SelectSingleNode("/product").Attributes("usercount").Value = "" Then
        '                    _UserCount = _maxcount

        '                Else
        '                    _UserCount = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product").Attributes("usercount").Value)
        '                    If _UserCount <= 0 Then
        '                        _UserCount = _maxcount

        '                    End If

        '                End If
        '            End If

        '        Else
        '            If _licenseXmlDocument.SelectSingleNode("/product").Attributes("usercount") Is Nothing Then
        '                _UserCount = _maxcount

        '            ElseIf _licenseXmlDocument.SelectSingleNode("/product").Attributes("usercount").Value = "" Then
        '                _UserCount = _maxcount

        '            ElseIf _licenseXmlDocument.SelectSingleNode("/product").Attributes("usercount").Value = "-1" Then
        '                _UserCount = _maxcount

        '            Else
        '                _UserCount = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product").Attributes("usercount").Value)
        '                If _UserCount < 0 Then
        '                    Throw New System.Exception("License 文件中总用户数授权错误！")

        '                End If

        '            End If

        '        End If

        '        Return _UserCount
        '    End Get
        'End Property



        ' 功能：子系统授权用户数
        ' 说明：如果子系统节点“不存在”用户数为0；usercount 属性“不存在/=空/<=0”表示无限用户数
        '       为了与之前保持统一，没有授权的子系统用户数解释为无限。
        'Public ReadOnly Property ApplicationUserCount(ByVal application As String) As Integer
        '    Get
        '        Dim _UserCount As Integer

        '        If LicenseMode = 1 Then
        '            If _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]") Is Nothing Then
        '                _UserCount = 0

        '            Else
        '                If _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]").Attributes("usercount") Is Nothing Then
        '                    _UserCount = _maxcount

        '                Else
        '                    If _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]").Attributes("usercount").Value = "" Then
        '                        _UserCount = _maxcount

        '                    Else
        '                        _UserCount = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]").Attributes("usercount").Value)
        '                        If _UserCount <= 0 Then
        '                            _UserCount = _maxcount

        '                        End If

        '                    End If

        '                End If

        '            End If

        '        Else
        '            If _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]") Is Nothing Then     ' 节点不存在默认为0
        '                _UserCount = 0

        '            Else
        '                If CheckLicenseObject(application, "系统") Then
        '                    If _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]").Attributes("usercount") Is Nothing Then
        '                        _UserCount = _maxcount
        '                    ElseIf _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]").Attributes("usercount").Value = "" Then
        '                        _UserCount = _maxcount
        '                    ElseIf _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]").Attributes("usercount").Value = "-1" Then
        '                        _UserCount = _maxcount
        '                    Else
        '                        _UserCount = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]").Attributes("usercount").Value)
        '                        If _UserCount < 0 Then
        '                            Throw New System.Exception("License 文件中子系统用户数授权错误！")
        '                        End If
        '                    End If
        '                Else        ' 如果子系统没有授权，用户数默认无限
        '                    _UserCount = _maxcount
        '                End If
        '            End If
        '        End If

        '        Return _UserCount
        '    End Get

        'End Property

        ' 功能：高级授权用户数
        ' 说明：如果子系统节点“不存在”用户数为0；usercount 属性“不存在/=空/<=0”表示无限用户数
        'Public ReadOnly Property AdvanceUserCount() As Integer
        '    Get
        '        Dim _UserCount As Integer

        '        If LicenseMode = 1 Then
        '            _UserCount = 0

        '        Else
        '            If _licenseXmlDocument.SelectSingleNode("/product").Attributes("advanceusercount") Is Nothing Then
        '                _UserCount = _maxcount

        '            ElseIf _licenseXmlDocument.SelectSingleNode("/product").Attributes("advanceusercount").Value = "" Then
        '                _UserCount = _maxcount

        '            ElseIf _licenseXmlDocument.SelectSingleNode("/product").Attributes("advanceusercount").Value = "-1" Then
        '                _UserCount = _maxcount

        '            Else
        '                _UserCount = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product").Attributes("advanceusercount").Value)
        '                If _UserCount < 0 Then
        '                    'Throw New System.Exception("License 文件中高级用户数授权错误！")
        '                    Throw New System.Exception("License 文件中用户数授权错误！")     '普及版合并高级、普通用户 2008.03.13 yuzy

        '                End If

        '            End If

        '        End If

        '        Return _UserCount
        '    End Get

        'End Property
#End Region

    End Class

End Namespace

