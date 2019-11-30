
' 封装应用程序安全校验方法
Imports System.Xml
Imports System.Text
Imports System.Web
Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports Mysoft.Map.Data
Imports Mysoft.Map.Utility
Imports Mysoft.Map.Security

Namespace Application.Security

    ' 注：该类属于业务策略层
    Public Class VerifyApplication
        Private Shared _MAXCOUNT As Integer = 999999
        Private Shared PUBLIC_KEY As String = "<RSAKeyValue><Modulus>17k9JTxyvTjvbpLHhkuzYvYucQBVjVul9m9EhrlU8SXr6B0TC+xqV9YNj2nNidGNbtUHtzTLq1pdd9i7XR+2Nq/4YekSxbQDJtkAhKIwdhMkW4iycTd4M2MOJhXWi636L+gyW478js3ul3lqgO3d0zJa2k0NalAgBv0AwwfH4K0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
        ' 用户类型
        ' modified by chenyong 2009-08-14
        Private Enum UserKind
            ErpUser = 0
            NormalUser = 1
            Sale = 2
        End Enum

        ' ===================== 公共方法

        '功能：打开登陆页面时校验
        '说明：模块是否被授权使用、公司数、用户数、项目数等
        '      如果自用狗需要减次数
        Public Shared Sub VerifyOpenLoginWindow()
            Try
                VerifyRuntime(False)
            Catch ex As Exception
                MyDB.LogException("授权校验失败", ex)
                Throw ex
            End Try
        End Sub

        ' 功能：随机校验
        ' 说明：模块是否被授权使用、公司数、用户数、项目数等
        Public Shared Sub VerifyRandom()
            VerifyLoginData()               ' 校验登录时生成的加密信息，校验机率 100%

            Dim RndCount As Integer = 200   ' 1/几率，如：RndCount = 200 表示 200 次中有 1 次，几率为 0.005。

            Randomize()
            If Int(Rnd() * RndCount) = 0 Then
                Try
                    VerifyRuntime(False)
                Catch ex As Exception
                    MyDB.LogException("授权校验失败", ex)
                    Throw ex
                End Try

                ' 重置用户数信息  2006.04.28 zt
                'If Int(Rnd() * 10) = 0 Then  modified by chenyong 2009-08-14 授权体系调整需求中不再需要此方法
                '    SetApplicationUserCount()

                'End If
            End If

        End Sub

        ' 功能：校验登录是否合法
        ' 说明：校验登录时生成的加密信息是否合法，如果不合法，说明登录被模拟了
        Private Shared Sub VerifyLoginData()
            Dim strUserGUID As String = HttpContext.Current.Session("UserGUID")
            Dim strLoginData As String = HttpContext.Current.Session("LoginData")
            Dim dtmLoginDate As DateTime
            If Not DateTime.TryParse(HttpContext.Current.Session("LoginDate"), dtmLoginDate) Then dtmLoginDate = Nothing

            If String.IsNullOrEmpty(strUserGUID) Or String.IsNullOrEmpty(strLoginData) Or dtmLoginDate = Nothing Then
                Dim ex As New Exception("非法登录，请与系统管理员联系！")
                MyDB.LogException("登录信息非法", ex)
                Throw ex
            End If

            '限制 LoginData 有效时间为 12 小时
            Dim t As TimeSpan = TimeSpan.FromTicks(DateTime.Now.Ticks - dtmLoginDate.Ticks)
            If t.TotalHours > 12 Then
                Dim ex As New Exception("非法登录，请与系统管理员联系！")
                MyDB.LogException("登录信息非法", ex)
                Throw ex
            End If

            Dim strInput As String
            strInput = HttpContext.Current.Session.SessionID & "|" & strUserGUID & "|" & dtmLoginDate.ToString("yyyyMMddHHmmss")

            ' 变换后的 MD5 算法，请确保代码混淆后不能被反编译
            Dim md5 As New System.Security.Cryptography.MD5CryptoServiceProvider()
            Dim bs As Byte() = System.Text.Encoding.UTF8.GetBytes(strInput)
            For i As Integer = 0 To bs.Length - 1
                bs(i) = bs(i) Xor &HF0
            Next
            bs = md5.ComputeHash(bs)
            Dim s As New System.Text.StringBuilder()
            Dim b As Byte
            For i As Integer = bs.Length - 1 To 0 Step -1
                b = bs(i) Xor (((i + 3) ^ 2 + &HA3) And &HFF)
                s.Append(b.ToString("X2"))
            Next

            If Not String.Compare(s.ToString(), strLoginData, True) = 0 Then
                Dim ex As New Exception("非法登录，请与系统管理员联系！")
                MyDB.LogException("登录信息非法", ex)
                Throw ex
            End If
        End Sub

        ' 功能：系统校验，如果未通过直接抛出错误
        ' 参数：isLogin     －－是否登陆。为了防止随意调用减少自用狗次数，该参数设置为必填
        ' 说明：软件狗是否正确安装、文件合法性、dll 版本、数据库版本、公司数、用户数、项目数等
        Public Shared Sub VerifyRuntime(ByVal isLogin As Boolean)
            ' 实例化授权对象从 License 文件中读取信息
            Dim MyLicense As New License(True)

            Dim MyDog As ISoftDog

            MyDog = SoftDogFactory.CreateSoftDogObject
            MyDog.Crc32ReadPos = MyLicense.Crc32ReadPos
            If MyDog.IsZydog Then
                If isLogin Then
                    ' 0、登陆时减自用狗使用次数
                    MyDog.MinusCounter()

                End If

            Else
                ' 1、授权文件合法性校验，自用狗除外
                verifyLicenseFile(MyDog.LicenseCrc32, MyDog.DogId)

            End If

            ' 2、dll 版本校验
            verifyDllVersion(MyLicense.GetLicenseXmlDocument)

            ' 3、Db 版本校验
            verifyDBVersion(MyLicense.GetLicenseXmlDocument)

            ' 4、公司数校验
            verifyCompanyCount(MyLicense.CompanyCount)

            ' 5、项目数校验
            verifyProjectCount(MyLicense.ProjectCount)

            ' 6、普通用户数校验
            ' modified by chenyong 2009-08-14 普通用户数校验
            verifyNormalUserCount(MyLicense.NormalUserCount)

            ' 7、销售系统用户数校验
            'modified by chenyong 2011-03-10 销售系统用户数校验
            verifySaleUserCount(MyLicense.SaleUserCount)

            ' 8、ERP用户数校验
            ' modified by chenyong 2009-08-14 ERP用户数校验
            verifyErpUserCount(MyLicense.ErpUserCount)
        End Sub

        ' 功能：普通用户数校验
        ' 修改：陈勇 2009-08-14
        Private Shared Sub verifyNormalUserCount(ByVal normalUserCount As Integer)
            If normalUserCount >= 0 Then
                If GetNormalUserCount(Nothing) > normalUserCount Then
                    Throw New System.Exception("对不起，数据库中的普通用户总数超出允许范围！")
                    Exit Sub
                End If
            End If
        End Sub

        ' 功能：销楼系统用户数校验
        ' 修改：陈勇 2011-03-10
        Private Shared Sub verifySaleUserCount(ByVal saleUserCount As Integer)
            If saleUserCount >= 0 Then
                If GetSaleUserCount(Nothing) > saleUserCount Then
                    Throw New System.Exception("对不起，数据库中的销售系统用户总数超出允许范围！")
                    Exit Sub
                End If
            End If
        End Sub

        ' 功能：ERP用户数校验
        ' 修改：陈勇 2009-08-14
        Private Shared Sub verifyErpUserCount(ByVal normalUserCount As Integer)
            If normalUserCount >= 0 Then
                If GetErpUserCount(Nothing) > normalUserCount Then
                    Throw New System.Exception("对不起，数据库中的ERP用户总数超出允许范围！")
                    Exit Sub
                End If
            End If
        End Sub

        ' 功能：获取授权应用系统代码字符串
        ' 返回：01,0101,0102,02,0201
        ' 注意：包含 License 文件中不存在的系统
        Public Shared Function GetLicenseApplicationString() As String
            Dim MyLicense As New License
            Dim DT As DataTable
            Dim SQL As String
            Dim i As Integer
            Dim AllowApplication As New StringBuilder

            SQL = " SELECT Application FROM myApplication WHERE isDisabled='0' AND " & _
                  " Application IN ('" & MyLicense.GetLicenseApplicationString.ToString.Replace(",", "','") & "')"
            DT = MyDB.GetDataTable(SQL)
            If DT.Rows.Count > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    AllowApplication.Append("," & DT.Rows(i)("Application"))
                Next
            End If

            If AllowApplication.ToString = "" Then
                Return ""
            Else
                Return Right(AllowApplication.ToString, AllowApplication.ToString.Length - 1)
            End If
        End Function

        ' 功能：获取系统下授权功能代码字符串
        ' 参数：application     －系统代码
        '       level           －级别。如果 -1 表示所有级别
        ' 返回：010102,01010201,01010202
        ' 注意：包含 License 文件中不存在的功能
        Public Shared Function GetLicenseFunctionString(ByVal application As String) As String
            Return GetLicenseFunctionString(application, -1)
        End Function

        Public Shared Function GetLicenseFunctionString(ByVal application As String, ByVal level As Integer) As String
            Dim MyLicense As New License

            Return MyLicense.GetLicenseFunctionString(application, level)
        End Function

        ' 功能：校验功能授权情况，如果未通过直接抛出错误
        ' 注意：License 文件中不存在的功能校验也无法通过
        Public Shared Sub VerifyLicenseFunction(ByVal functionCode As String)
            If Not CheckLicenseObject(functionCode, "功能") Then
                Throw New System.Exception("对不起，""" & MyDB.GetDataItemString("SELECT FunctionName FROM myFunction WHERE FunctionCode='" & functionCode & "'") & _
                    """功能未被授权使用！")

            End If

        End Sub

        ' 功能：校验功能是否授权
        Public Shared Function CheckLicenseFunction(ByVal functionCode As String) As Boolean
            Return CheckLicenseObject(functionCode, "功能")

        End Function

        ' 功能：校验对象是否授权
        ' 参数：licenseObject      －检验对象代码
        '       licenseObjectType      －校验对象类型（系统/功能）
        Public Shared Function CheckLicenseObject(ByVal licenseObject As String, ByVal licenseObjectType As String) As Boolean
            Dim MyLicense As New License

            Return MyLicense.CheckLicenseObject(licenseObject, licenseObjectType)

        End Function

        ' 功能：获取受业务影响需要隐藏的对象字符串
        ' 参数：objectType          －对象类型。枚举：1－功能；2－功能+'.'+动作；3－业务参数。
        '       additionalParam     －附加参数。该参数内容如下：
        '       1、当 objectType=1或2时，该参数为系统代码或空，只过滤该系统下的功能和动作。目的是防止返回的字符串过长，加系统过滤。
        '       注：如果 additionalParam 参数为空不做附加参数过滤处理
        ' 返回：1、当 objectType=1 时，返回功能代码串，如：01010101,01010102
        '       2、当 objectType=2 时，返回功能+'.'+动作代码串，如：01010101.01,01010102.02
        Public Shared Function GetNotInFilterString(ByVal objectType As String, ByVal additionalParam As String) As String
            Dim SqlConn As SqlClient.SqlConnection
            Dim SqlComm As SqlClient.SqlCommand
            Dim SqlParams As SqlParameterCollection

            'Dim LicenseApplication As New StringBuilder         ' 授权而且启用的系统
            Dim DT As DataTable
            Dim i As Integer

            'DT = MyDB.GetDataTable("SELECT Application FROM myApplication WHERE IsDisabled=0 AND Application IN ('" & GetLicenseApplicationString.Replace(",", "','") & "')")
            'If DT.Rows.Count > 0 Then
            '    For i = 0 To DT.Rows.Count - 1
            '        If LicenseApplication.ToString = "" Then
            '            LicenseApplication.Append(DT.Rows(i)("Application"))
            '        Else
            '            LicenseApplication.Append("," & DT.Rows(i)("Application"))
            '        End If
            '    Next
            'End If

            SqlConn = New SqlClient.SqlConnection(MyDB.GetSqlConnectionString)
            SqlComm = New SqlClient.SqlCommand("usp_p_GetNotInFilterString", SqlConn)
            SqlComm.CommandType = CommandType.StoredProcedure

            SqlParams = SqlComm.Parameters
            SqlParams.Add(New SqlParameter("@chvObjectType", SqlDbType.NVarChar, 8))
            SqlParams.Add(New SqlParameter("@chvAdditionalParam", SqlDbType.NVarChar, 50))
            SqlParams.Add(New SqlParameter("@chvLicenseApplication", SqlDbType.NVarChar, 500))
            SqlParams.Add(New SqlParameter("@chvReturnString", SqlDbType.NVarChar, 1000))

            SqlParams("@chvObjectType").Value = objectType
            SqlParams("@chvAdditionalParam").Value = additionalParam
            SqlParams("@chvLicenseApplication").Value = GetLicenseApplicationString()
            SqlParams("@chvReturnString").Value = ""

            SqlParams("@chvReturnString").Direction = ParameterDirection.Output

            SqlConn.Open()
            SqlComm.ExecuteNonQuery()
            SqlConn.Close()

            If SqlParams("@chvReturnString").Value Is DBNull.Value Then
                Return ""
            Else
                Return SqlParams("@chvReturnString").Value
            End If

        End Function

        ' 功能：获取已授权产品的 HTML 字符串，About 界面用
        ' 显示格式如：销售管理系统 SP1 V5.0.405.0
        ''普及版合并高级、普通用户 2008.03.13 yuzy ，子系统不需要显示授权用户数
        Public Shared Function GetLicenseApplicationHTML() As String
            Dim MyLicense As New License
            Dim SubApplicationNodeList As XmlNodeList
            Dim i As Integer
            Dim LicenseProductInfo As New StringBuilder
            Dim FileProperties As FileVersionInfo
            Dim ProductName As String
            'Dim UserCount As String

            SubApplicationNodeList = MyLicense.GetLicenseXmlDocument.SelectNodes("/product/system/subsystem[@enabled=1]")
            For Each item As XmlNode In SubApplicationNodeList
                If item.Attributes("dllname").Value <> "" Then
                    Try
                        Dim filePath As String = HttpContext.Current.Server.MapPath("/bin/") & item.Attributes("dllname").Value
                        If IO.File.Exists(filePath) Then
                            FileProperties = FileVersionInfo.GetVersionInfo(filePath)

                            ' 如果 0201 是合同管理，将产品名称中的“成本”替换成“合同”   2008.03.18 zt
                            ProductName = FileProperties.ProductName
                            If item.Attributes("code").Value = "0201" Then
                                If item.Attributes("name").Value.IndexOf("合同") >= 0 Then
                                    ProductName = ProductName.Replace("成本", "合同")
                                End If
                            End If

                            '2009-9-19 huyl 如果当前是0206，则将“全面预算”改为“费用管理”
                            If item.Attributes("code").Value = "0206" Then
                                ProductName = ProductName.Replace("全面预算", "费用管理")

                            End If

                            LicenseProductInfo.Append(ProductName & " (" & FileProperties.ProductVersion & ") " & ChrW(13) & ChrW(10))
                        End If
                      
                    Catch ex As System.IO.FileNotFoundException
                        Mysoft.Map.Data.MyDB.LogException(ex)
                        ' 如果 dll 不存在就不显示

                    End Try
                End If

            Next
            Return LicenseProductInfo.ToString

        End Function

        '功能：获取数据库中普通用户数 
        '说明：若参数userGUID有值,则排除该userGUID不计算
        '修改：陈勇 2009-08-14
        Public Shared Function GetNormalUserCount(ByVal userGUID As String) As Integer
            Return GetDBUserCount(UserKind.NormalUser, userGUID)
        End Function

        '功能：获取数据库中销售系统用户数 
        '说明：若参数userGUID有值,则排除该userGUID不计算
        '修改：陈勇 2011-03-10
        Public Shared Function GetSaleUserCount(ByVal userGUID As String) As Integer
            Return GetDBUserCount(UserKind.Sale, userGUID)
        End Function

        '功能：获取数据库中Erp用户数
        '说明：若参数userGUID有值,则排除该userGUID不计算
        '修改：陈勇 2009-08-14
        Public Shared Function GetErpUserCount(ByVal userGUID As String) As Integer
            Return GetDBUserCount(UserKind.ErpUser, userGUID)
        End Function

        '功能：获取数据库中用户数
        '说明：参数userKind为用户类型，若参数userGUID有值,则排除该userGUID不计算
        '修改：陈勇 2009-08-14
        Private Shared Function GetDBUserCount(ByVal userType As Integer, ByVal userGUID As String) As Integer
            Dim _strSQL As String = "SELECT COUNT(*) FROM myUser WHERE UserCode<>'Admin' AND (IsDisabeld=0 OR IsDisabeld IS null) AND UserKind = " & userType.ToString()

            If Not String.IsNullOrEmpty(userGUID) Then
                _strSQL = _strSQL & " and UserGUID<>'" & userGUID & "'"
            End If

            Return MyDB.GetDataItemInt(_strSQL)
        End Function


        ' 功能：获取授权公司数
        Public Shared Function GetLicenseCompanyCount() As Integer
            Dim MyLicense As New License

            Return MyLicense.CompanyCount

        End Function


        ' 功能：获取授权用户信息
        ' 说明：About 界面用
        ' 修改：陈勇 2009-08-14
        Public Shared Function GetLicenseUserHTML() As String
            Dim MyLicense As New License
            Dim strHTML As New Text.StringBuilder

            strHTML.Append("（")

            If MyLicense.ErpUserCount = _MAXCOUNT Then
                strHTML.Append("ERP用户数：无限 &nbsp;&nbsp;")
            Else
                strHTML.Append("ERP用户数：" & MyLicense.ErpUserCount.ToString() & "&nbsp;&nbsp;")
            End If

            If MyLicense.NormalUserCount = _MAXCOUNT Then
                strHTML.Append("普通用户数：无限 &nbsp;&nbsp;")
            Else
                strHTML.Append("普通用户数：" & MyLicense.NormalUserCount.ToString() & "&nbsp;&nbsp;")
            End If

            'modified by chenyong 2011-03-10 销售系统用户调整
            If MyLicense.SaleUserCount = _MAXCOUNT Then
                strHTML.Append("销售系统用户数：无限 ")
            Else
                strHTML.Append("销售系统用户数：" & MyLicense.SaleUserCount.ToString())
            End If

            strHTML.Append("）")

            Return strHTML.ToString()
        End Function


        ' 功能：获取授权项目数
        Public Shared Function GetLicenseProjectCount() As Integer
            Dim MyLicense As New License

            Return MyLicense.ProjectCount

        End Function

        ' ========================== 私有方法

        ' 功能：授权文件合法性校验
        Private Shared Sub verifyLicenseFile(ByVal licenseCrc32 As Integer, ByVal dogId As String)
            Dim strLicenseFile As String
            strLicenseFile = HttpContext.Current.Server.MapPath("/bin/License.xml")

            '校验函数中不应抛出异常，给出明确的错误信息等，避免被跟踪         -- Lionel  2009.05.14
            If GeneralBase.Crc32IntegerFile(strLicenseFile) <> licenseCrc32 Then
                'Throw New System.Exception("非法的授权文件（软件狗信息与授权文件不匹配）！")
                Throw New System.Exception("非法的授权文件！")
            End If

            '验证授权文件签名
            Dim xmlDoc As XmlDocument
            Dim xmlNData As XmlNode
            Dim strHashData As String
            Dim xmlNSigData As XmlNode
            Dim strData As String
            Dim xmlAttr As XmlAttribute
            Dim xmlLicense As XmlDocument

            xmlDoc = New XmlDocument
            Try
                xmlDoc.Load(strLicenseFile)
                '验证签名
                xmlNData = xmlDoc.DocumentElement.SelectSingleNode("data")
                strData = xmlNData.InnerXml
                'modified by kongy 2009-08-19 改为调用MySoft.Map.Security.RSAUtil执行签名验证
                If Not RSAUtil.GetHash(strData, strHashData) Then
                    Throw New System.Exception("非法的授权文件！")
                End If

                xmlNSigData = xmlDoc.DocumentElement.SelectSingleNode("signature")
                If Not RSAUtil.SignatureDeformatterByHash(PUBLIC_KEY, strHashData, xmlNSigData.InnerText) Then
                    Throw New System.Exception("非法的授权文件！")
                End If

                xmlLicense = New XmlDocument
                xmlLicense.LoadXml(strData)

                xmlAttr = xmlLicense.DocumentElement.Attributes.GetNamedItem("dogid")
                If String.Compare(xmlAttr.Value, dogId) <> 0 Then
                    Throw New System.Exception("非法的授权文件！")
                End If

                '校验 Application 中保存的授权文档对象
                If Not HttpContext.Current.Application("_licenseXmlDocument") Is Nothing Then
                    xmlLicense = CType(HttpContext.Current.Application("_licenseXmlDocument"), XmlDocument)

                    If String.Compare(strData, xmlLicense.DocumentElement.OuterXml) <> 0 Then
                        Throw New Exception("非法的授权文件！")
                    End If
                End If

            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Throw New System.Exception("非法的授权文件！")
            End Try
        End Sub

        ' 功能：校验 dll 版本信息
        Private Shared Sub verifyDllVersion(ByVal licenseXmlDocument As XmlDocument)
            Dim SubApplicationNodeList As XmlNodeList
            Dim i As Integer
            Dim ErrMessage As String

            SubApplicationNodeList = licenseXmlDocument.SelectNodes("/product/system/subsystem")
            For i = 0 To SubApplicationNodeList.Count - 1
                ' 校验子系统版本号。如果没有指定 dll 或 版本号就不校验
                If SubApplicationNodeList.Item(i).Attributes("dllname").Value <> "" And SubApplicationNodeList.Item(i).Attributes("version").Value <> "" Then
                    ErrMessage = compareToDllVersion(SubApplicationNodeList.Item(i).Attributes("dllname").Value, SubApplicationNodeList.Item(i).Attributes("version").Value)
                    If ErrMessage <> "" Then
                        Throw New System.Exception(ErrMessage)

                    End If

                End If

            Next

        End Sub

        ' 功能：检查子系统真实版本号
        ' 参数：dllName     －－ dll 文件名，如：slxt.dll
        '       licenseVersion      －－ 授权版本号
        ' 返回：
        ' 说明：用授权文件中的版本与实际版本比对，只校验主.副版本号
        Private Shared Function compareToDllVersion(ByVal dllName As String, ByVal licenseVersion As String) As String
            Try
                Dim FileProperties As FileVersionInfo
                Dim RealVersion As String
                Dim licenseVersionArray() As String = licenseVersion.Split(".")
                Dim FileMajorPart, FileMinorPart As String

                FileMajorPart = licenseVersionArray(0)
                If licenseVersionArray.Length > 1 Then
                    FileMinorPart = licenseVersionArray(1)
                Else
                    FileMinorPart = "x"
                End If

                Dim filePath As String = HttpContext.Current.Server.MapPath("/bin/") & dllName
                If IO.File.Exists(filePath) Then
                    FileProperties = FileVersionInfo.GetVersionInfo(filePath)
                    RealVersion = FileProperties.FileMajorPart & "." & FileProperties.FileMinorPart
                    If FileMajorPart = FileProperties.FileMajorPart.ToString And FileMinorPart = FileProperties.FileMinorPart.ToString Then
                        Return ""
                    Else
                        Return dllName & " 的授权版本号与真实版本号不一致！"
                    End If
                End If

            Catch ex As System.io.FileNotFoundException
            	Mysoft.Map.Data.MyDB.LogException(ex)
                ' 如果 dll 文件不存在就不校验
                Return ""

            End Try

        End Function

        ' 功能：校验 DB 版本信息
        Private Shared Sub verifyDBVersion(ByVal licenseXmlDocument As XmlDocument)
            Dim SubApplicationNodeList As XmlNodeList
            Dim i As Integer
            Dim ErrMessage As String

            SubApplicationNodeList = licenseXmlDocument.SelectNodes("/product/system/subsystem")
            For i = 0 To SubApplicationNodeList.Count - 1
                ErrMessage = compareToDBVersion(SubApplicationNodeList.Item(i).Attributes("code").Value)
                If ErrMessage <> "" Then
                    Throw New System.Exception(ErrMessage)

                End If

            Next

        End Sub

        ' 功能：校验公司数
        Private Shared Sub verifyCompanyCount(ByVal companyCount As Integer)
            If companyCount > 0 Then
                If MyDB.GetDataItemInt("SELECT COUNT(*) FROM myBusinessUnit WHERE IsEndCompany = 1") > companyCount Then
                    Throw New System.Exception("对不起，数据库中的公司数超出允许范围！")
                    Exit Sub
                End If

            End If

        End Sub


        ' 功能：校验项目数
        Private Shared Sub verifyProjectCount(ByVal projectCount As Integer)
            If projectCount > 0 Then
                If MyDB.GetDataItemInt("SELECT COUNT(*) FROM p_Project WHERE Level=2 And IsGt=0") > projectCount Then
                    Throw New System.Exception("对不起，数据库中的项目数超出允许范围！")
                    Exit Sub
                End If

            End If

        End Sub

        ' 功能：检查数据库版本
        ' 参数：dllName     －－ dll 文件名，如：slxt.dll
        '       licenseVersion      －－ 授权版本号
        ' 返回：
        ' 说明：实际版本与数据库中记录的版本比对，全校验（包括主.副.内部.修订号）
        Private Shared Function compareToDBVersion(ByVal application As String) As String
            Dim MyDB As New MyDB
            Dim ApplicationName, DBVersion, dllName As String
            Dim FileProperties As FileVersionInfo
            Dim RealVersion As String

            dllName = MyDB.GetDataItemString("SELECT DllName FROM myApplication WHERE Application='" & application & "'")
            Try
                ApplicationName = MyDB.GetDataItemString("SELECT ApplicationName FROM myApplication WHERE Application='" & application & "'")
                DBVersion = MyDB.GetDataItemString("SELECT Version FROM myApplication WHERE Application='" & application & "'")
                Dim filePath As String = HttpContext.Current.Server.MapPath("/bin/") & dllName
                If IO.File.Exists(filePath) Then
                    FileProperties = FileVersionInfo.GetVersionInfo(filePath)
                    RealVersion = FileProperties.FileVersion.ToString
                    If DBVersion <> "" Then
                        If DBVersion <> RealVersion Then
                            Return "『" & ApplicationName & "』当前使用的数据库版本不正确！"
                        Else
                            Return ""
                        End If

                    End If
                End If

            Catch ex As System.io.FileNotFoundException
                Mysoft.Map.Data.MyDB.LogException(ex)

            End Try
            Return String.Empty
        End Function


        '定义授权的场景，用于校验子系统用户数是否超标
        Public Enum SetRightType
            AddRightsToStation = 0       '为岗位授权
            AddUsersToStation = 1        '为岗位添加成员
            AddStationsToUser = 2        '为成员分配岗位
        End Enum

        ' 功能：给用户授权前，校验各子系统的授权用户数是否超标
        ' 2007.01.17 yuzy
        '参数：     a_enmSetRightType       授权场景，使用“SetRightType”枚举值
        '           a_strStationGUIDList    岗位GUID
        '           a_strUserGUIDList       用户GUID（为岗位授权时该参数为空字符串）
        '           a_strFunccode           授权功能代码（仅在为岗位授权时使用）
        '返回：     true    通过，不超标   ；   false   不通过，超标
        Public Shared Function CheckApplicationUserCount(ByVal a_enmSetRightType As SetRightType, ByVal a_strStationGUIDList As String, ByVal a_strUserGUIDList As String, ByVal a_strFunccode As String) As Boolean
            Dim AdminCount, DestUsersCount As Integer
            Dim SQL, strDestUsersClause, strDestFuncClause As String

            ' 1、计算超级用户数（排除高级用户数）
            AdminCount = MyDB.GetDataItemInt("SELECT COUNT(*) FROM myUser WHERE IsAdmin=1 AND (IsDisabeld=0 OR IsDisabeld IS null) AND (IsAdvanceUser=0 OR IsAdvanceUser is NULL)")

            ' 2、计算要赋予权限的用户数：即当前岗位的成员数（排除超级用户、禁用用户、高级用户数）
            Select Case a_enmSetRightType
                Case SetRightType.AddRightsToStation
                    SQL = " SELECT COUNT(DISTINCT(convert(varchar(40),su.UserGUID))) " & _
                          " FROM myStationUser su  " & _
                          " LEFT JOIN myUser u ON u.UserGUID=su.UserGUID " & _
                          " WHERE u.IsAdmin=0 " & _
                          " AND (IsDisabeld=0 OR IsDisabeld IS null)" & _
                          " AND (IsAdvanceUser=0 OR IsAdvanceUser is null)" & _
                          " AND su.StationGUID in ('" & a_strStationGUIDList.Replace(",", "','") & "')"
                    DestUsersCount = MyDB.GetDataItemInt(SQL)

                    '拼接授权用户的子句
                    strDestUsersClause = "(select distinct(su.UserGUID) from myStationUser su where su.StationGUID in ('" & a_strStationGUIDList.Replace(",", "','") & "'))"
                    '拼接授权功能代码的字句
                    strDestFuncClause = "('" & a_strFunccode.ToString.Replace(",", "','") & "') "

                    Exit Select

                Case SetRightType.AddStationsToUser, SetRightType.AddUsersToStation
                    SQL = " SELECT COUNT(*) " & _
                          " FROM myUser " & _
                          " WHERE IsAdmin=0 " & _
                          " AND (IsDisabeld=0 OR IsDisabeld IS null)" & _
                          " AND (IsAdvanceUser=0 OR IsAdvanceUser is null)" & _
                          " AND UserGUID in ('" & a_strUserGUIDList.ToString.Replace(",", "','") & "')"
                    DestUsersCount = MyDB.GetDataItemInt(SQL)

                    strDestUsersClause = "('" & a_strUserGUIDList.ToString.Replace(",", "','") & "')"
                    strDestFuncClause = "(select ObjectType from myStationRights where StationGUID in ('" & a_strStationGUIDList.Replace(",", "','") & "')) "

                    Exit Select

            End Select


            ' 3、与授权用户数比较（排除高级用户数）
            SQL = " SELECT COUNT(*) " & _
                " FROM (select temp.Application,COUNT(*) AS UserCount " & _
                "       from (select v_ur.Application " & _
                "	          from myUserRights v_ur " & _
                "	          inner join myUser u on v_ur.UserGUID=u.UserGUID " & _
                "	          where u.IsAdmin=0 " & _
                "             and (IsDisabeld=0 or IsDisabeld is null)" & _
                "             and (IsAdvanceUser=0 or IsAdvanceUser is null)" & _
                "             and v_ur.UserGUID not in " & strDestUsersClause & _
                "	          group by v_ur.Application,v_ur.UserGUID) as temp " & _
                "       group by temp.Application) AS temp2 " & _
                " RIGHT JOIN myApplication a ON a.Application=temp2.Application " & _
                " WHERE a.Level=1" & _
                " AND a.Application IN (select Application " & _
                "                       from myFunction " & _
                "                       where FunctionCode IN " & strDestFuncClause & _
                "                       group by Application)" & _
                " AND (CASE WHEN temp2.UserCount IS NULL THEN " & DestUsersCount.ToString & "+" & AdminCount.ToString & " ELSE temp2.UserCount+" & DestUsersCount.ToString & "+" & AdminCount.ToString & " END)>isnull(a.LicenseUserCount,0)"

            If MyDB.GetDataItemInt(SQL.ToString) > 0 Then
                Return False
            Else
                Return True
            End If

        End Function

        ' =============================== Friend Method ==================================

        ' 功能：返回受增值模块影响的没有授权的功能动作点过滤表达式
        ' 返回：<功能代码1>.<动作代码1>,<功能代码2>.<动作代码2>
        Public Shared Function GetNotInFilterStringByIncrement() As String
            Dim SQL As String
            Dim DT As DataTable
            Dim FilterExp As New StringBuilder
            Dim i As Integer

            ' 1、增值模块影响
            SQL = "SELECT LicenseObjectType,LicenseObject,ROPriProperty,ROSecProperty,ROType FROM myLicenseRelationShip" & _
                  " WHERE ROType='动作点' OR ROType='桌面部件' ORDER BY LicenseObject"
            DT = MyDB.GetDataTable(SQL)
            If DT.Rows.Count > 0 Then
                ' 系统或功能被禁用与没有授权同等处理  史海峰、向前    zt 2007.3.13
                Dim dtApplication As DataTable
                Dim bolIsDisabled As Boolean        '  是否被禁用
                Dim j As Integer

                ' 禁用模块对动作点的影响不处理  史海峰、孙海芳  zt 2007.3.22
                dtApplication = MyDB.GetDataTable("SELECT Application AS Code FROM myApplication WHERE IsDisabled=1")
                bolIsDisabled = False
                For i = 0 To DT.Rows.Count - 1
                    bolIsDisabled = False

                    ' 判断是否已被禁用
                    For j = 0 To dtApplication.Rows.Count - 1
                        If DT.Rows(i)("LicenseObject").ToString.ToLower = dtApplication.Rows(j)("Code").ToString.ToLower Then
                            bolIsDisabled = True
                            Exit For
                        End If
                    Next

                    ' 判断系统/功能权限判断是否授权
                    If bolIsDisabled Or Not VerifyApplication.CheckLicenseObject(DT.Rows(i)("LicenseObject").ToString, DT.Rows(i)("LicenseObjectType").ToString) Then
                        If DT.Rows(i)("ROType").ToString = "动作点" Then
                            ' 1.1、如果是动作点
                            FilterExp.Append("," & DT.Rows(i)("ROPriProperty").ToString & "." & DT.Rows(i)("ROSecProperty").ToString)
                        ElseIf DT.Rows(i)("ROType").ToString = "桌面部件" Then
                            ' 1.2、如果是桌面部件
                            FilterExp.Append(",0000029902." & DT.Rows(i)("ROPriProperty").ToString)
                        End If
                    End If
                Next
            End If

            ' 2、桌面部件受系统影响
            SQL = "SELECT WPCode FROM myWebPart WHERE Application NOT IN ('" & VerifyApplication.GetLicenseApplicationString.Replace(",", "','") & "')"
            DT = MyDB.GetDataTable(SQL)
            If DT.Rows.Count > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    FilterExp.Append(",0000029902." & DT.Rows(i)("WPCode").ToString)
                Next
            End If

            Return FilterExp.ToString
        End Function

        '功能：获取用户是否有该功能的使用权限，根据如下条件判断：
        '1、模块是否被授权
        '2、功能模块及模块所在系统是否启用
        '3、该模块是否被业务影响需要隐藏
        ' 参数1：application    —— 功能所属系统的代码
        ' 参数2：functionCode   —— 功能代码
        '说明：不区分普通和虚拟模块
        Public Shared Function CheckFunctionAllowUse(ByVal application As String, ByVal functionCode As String) As Boolean
            '1、模块是否被授权
            If GetLicenseFunctionString(application).IndexOf(functionCode) < 0 Then Return False

            '2.功能模块是否已启用 且 功能所在系统是否已启用
            Dim strSql As String
            strSql = "SELECT top 1 (0) From myApplication A LEFT JOIN myFunction F ON A.Application=F.Application " & _
                     " WHERE A.isDisabled=0 AND F.isDisabled=0 AND F.FunctionCode='" & functionCode & "'"
            If MyDB.GetDataTable(strSql).Rows.Count = 0 Then Return False

            '3、该模块是否受业务影响需要隐藏
            If GetNotInFilterString(1, application).IndexOf(functionCode) >= 0 Then Return False

            Return True

        End Function

#Region "注释代码"
        ' 功能：获取授权总用户数
        'Public Shared Function GetLicenseUserCount() As Integer
        '    Dim MyLicense As New License

        '    Return MyLicense.UserCount

        'End Function

        ' 功能：获取授权高级用户信息
        ' 说明：About 界面用
        'Public Shared Function GetLicenseAdvanceUserHTML() As String
        '    Dim MyLicense As New License

        '    '普及版合并高级、普通用户 2008.03.13 yuzy
        '    If MyLicense.AdvanceUserCount = _MAXCOUNT Then
        '        Return "（授权用户数：无限）"
        '    Else
        '        Return "（授权用户数：" & MyLicense.AdvanceUserCount.ToString & "）"
        '    End If

        'End Function

        ' 功能：获取授权高级用户数
        'Public Shared Function GetLicenseAdvanceUserCount() As Integer
        '    Dim MyLicense As New License

        '    Return MyLicense.AdvanceUserCount.ToString

        'End Function

        ' 功能：总用户数校验
        'Private Shared Sub verifyUserCount(ByVal userCount As Integer)
        '    If userCount > 0 Then
        '        If MyDB.GetDataItemInt("SELECT COUNT(*) FROM myUser WHERE (IsDisabeld=0 OR IsDisabeld IS null)") > userCount Then
        '            Throw New System.Exception("对不起，数据库中的总用户数超出允许范围！！")
        '            Exit Sub
        '        End If

        '    End If

        'End Sub

        ' 功能：高级户数校验
        '       普及版合并高级、普通用户。admin为赠送用户，不计入授权用户数。  2008.03.13 yuzy
        'Private Shared Sub verifyAdvanceUserCount(ByVal userCount As Integer)
        '    If userCount > 0 Then
        '        If MyDB.GetDataItemInt("SELECT COUNT(*) FROM myUser WHERE UserCode<>'Admin' AND (IsDisabeld=0 OR IsDisabeld IS null) AND IsAdvanceUser = 1") > userCount Then
        '            'Throw New System.Exception("对不起，数据库中的高级用户数超出允许范围！！")
        '            Throw New System.Exception("对不起，数据库中的用户数超出允许范围！！")
        '            Exit Sub
        '        End If

        '    End If

        'End Sub

        ' 功能：子系统用户数校验
        'Private Shared Sub verifyApplicationUserCount()
        '    Dim DT As DataTable

        '    DT = MyDB.GetDataTable("SELECT ApplicationName FROM myApplication WHERE ActualUserCount>LicenseUserCount AND Level=1")
        '    If DT.Rows.Count > 0 Then
        '        Dim i As Integer
        '        Dim ErrorMessage As String

        '        ErrorMessage = "对不起，"
        '        For i = 0 To DT.Rows.Count - 1
        '            If i = 0 Then
        '                ErrorMessage &= "[" & DT.Rows(i)("ApplicationName") & "]"
        '            Else
        '                ErrorMessage &= "[" & DT.Rows(i)("ApplicationName") & "]"
        '            End If
        '        Next
        '        ErrorMessage &= " 的用户数超出授权范围！"

        '        Throw New System.Exception(ErrorMessage)

        '    End If

        'End Sub

        ' 功能：向 myApplication 表中填写 授权用户数 和 实际用户数
        ' 2006.04.27 zt
        'Public Shared Sub SetApplicationUserCount()
        '    Dim SQL As New StringBuilder
        '    Dim DTActualUser As DataTable           ' 授权用户数
        '    Dim AdminCount As Integer               ' 超级用户的数量
        '    Dim MyLicense As New License

        '    ' 2、超级用户数
        '    AdminCount = MyDB.GetDataItemInt("SELECT COUNT(*) FROM myUser WHERE IsAdmin=1 AND (IsDisabeld=0 OR IsDisabeld IS null) AND (IsAdvanceUser=0 OR IsAdvanceUser is NULL)")

        '    ' 3、系统用户数记录集
        '    SQL.Append(" SELECT CASE WHEN temp2.UserCount IS NULL THEN " & AdminCount & " ELSE temp2.UserCount+" & AdminCount & " END AS UserCount,a.Application" & _
        '          " FROM (select temp.Application,COUNT(*) AS UserCount " & _
        '          "       from (select ur.Application " & _
        '          "	            from myUserRights ur " & _
        '          "	            inner join myUser u on ur.UserGUID=u.UserGUID " & _
        '          "	            where u.IsAdmin=0 " & _
        '          "             and (IsDisabeld=0 or IsDisabeld is null)" & _
        '          "             and (IsAdvanceUser=0 or IsAdvanceUser is null)" & _
        '          "	            group by ur.Application,ur.UserGUID) as temp " & _
        '          "       group by temp.Application) AS temp2 " & _
        '          " RIGHT JOIN myApplication a ON a.Application=temp2.Application " & _
        '          " WHERE a.Level=1")
        '    DTActualUser = MyDB.GetDataTable(SQL.ToString)

        '    ' 4、拼写 update 语句
        '    If DTActualUser.Rows.Count > 0 Then
        '        Dim i As Integer

        '        SQL = New StringBuilder

        '        For i = 0 To DTActualUser.Rows.Count - 1
        '            If i <> 0 Then
        '                SQL.Append(";")
        '            End If

        '            ' 取总用户数和子系统用户数中最小的作为授权用户数
        '            SQL.Append("UPDATE myApplication SET LicenseUserCount=" & _
        '                        IIf(MyLicense.UserCount < MyLicense.ApplicationUserCount(DTActualUser.Rows(i)("Application")), MyLicense.UserCount, MyLicense.ApplicationUserCount(DTActualUser.Rows(i)("Application"))) & _
        '                        ",ActualUserCount=" & DTActualUser.Rows(i)("UserCount").ToString & _
        '                        " WHERE Application='" & DTActualUser.Rows(i)("Application") & "'")
        '        Next

        '        MyDB.ExecSQL(SQL.ToString)
        '    End If

        'End Sub
#End Region

    End Class

End Namespace
