
' ��װӦ�ó���ȫУ�鷽��
Imports System.Xml
Imports System.Text
Imports System.Web
Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports Mysoft.Map.Data
Imports Mysoft.Map.Utility
Imports Mysoft.Map.Security

Namespace Application.Security

    ' ע����������ҵ����Բ�
    Public Class VerifyApplication
        Private Shared _MAXCOUNT As Integer = 999999
        Private Shared PUBLIC_KEY As String = "<RSAKeyValue><Modulus>17k9JTxyvTjvbpLHhkuzYvYucQBVjVul9m9EhrlU8SXr6B0TC+xqV9YNj2nNidGNbtUHtzTLq1pdd9i7XR+2Nq/4YekSxbQDJtkAhKIwdhMkW4iycTd4M2MOJhXWi636L+gyW478js3ul3lqgO3d0zJa2k0NalAgBv0AwwfH4K0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
        ' �û�����
        ' modified by chenyong 2009-08-14
        Private Enum UserKind
            ErpUser = 0
            NormalUser = 1
            Sale = 2
        End Enum

        ' ===================== ��������

        '���ܣ��򿪵�½ҳ��ʱУ��
        '˵����ģ���Ƿ���Ȩʹ�á���˾�����û�������Ŀ����
        '      ������ù���Ҫ������
        Public Shared Sub VerifyOpenLoginWindow()
            Try
                VerifyRuntime(False)
            Catch ex As Exception
                MyDB.LogException("��ȨУ��ʧ��", ex)
                Throw ex
            End Try
        End Sub

        ' ���ܣ����У��
        ' ˵����ģ���Ƿ���Ȩʹ�á���˾�����û�������Ŀ����
        Public Shared Sub VerifyRandom()
            VerifyLoginData()               ' У���¼ʱ���ɵļ�����Ϣ��У����� 100%

            Dim RndCount As Integer = 200   ' 1/���ʣ��磺RndCount = 200 ��ʾ 200 ������ 1 �Σ�����Ϊ 0.005��

            Randomize()
            If Int(Rnd() * RndCount) = 0 Then
                Try
                    VerifyRuntime(False)
                Catch ex As Exception
                    MyDB.LogException("��ȨУ��ʧ��", ex)
                    Throw ex
                End Try

                ' �����û�����Ϣ  2006.04.28 zt
                'If Int(Rnd() * 10) = 0 Then  modified by chenyong 2009-08-14 ��Ȩ��ϵ���������в�����Ҫ�˷���
                '    SetApplicationUserCount()

                'End If
            End If

        End Sub

        ' ���ܣ�У���¼�Ƿ�Ϸ�
        ' ˵����У���¼ʱ���ɵļ�����Ϣ�Ƿ�Ϸ���������Ϸ���˵����¼��ģ����
        Private Shared Sub VerifyLoginData()
            Dim strUserGUID As String = HttpContext.Current.Session("UserGUID")
            Dim strLoginData As String = HttpContext.Current.Session("LoginData")
            Dim dtmLoginDate As DateTime
            If Not DateTime.TryParse(HttpContext.Current.Session("LoginDate"), dtmLoginDate) Then dtmLoginDate = Nothing

            If String.IsNullOrEmpty(strUserGUID) Or String.IsNullOrEmpty(strLoginData) Or dtmLoginDate = Nothing Then
                Dim ex As New Exception("�Ƿ���¼������ϵͳ����Ա��ϵ��")
                MyDB.LogException("��¼��Ϣ�Ƿ�", ex)
                Throw ex
            End If

            '���� LoginData ��Чʱ��Ϊ 12 Сʱ
            Dim t As TimeSpan = TimeSpan.FromTicks(DateTime.Now.Ticks - dtmLoginDate.Ticks)
            If t.TotalHours > 12 Then
                Dim ex As New Exception("�Ƿ���¼������ϵͳ����Ա��ϵ��")
                MyDB.LogException("��¼��Ϣ�Ƿ�", ex)
                Throw ex
            End If

            Dim strInput As String
            strInput = HttpContext.Current.Session.SessionID & "|" & strUserGUID & "|" & dtmLoginDate.ToString("yyyyMMddHHmmss")

            ' �任��� MD5 �㷨����ȷ������������ܱ�������
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
                Dim ex As New Exception("�Ƿ���¼������ϵͳ����Ա��ϵ��")
                MyDB.LogException("��¼��Ϣ�Ƿ�", ex)
                Throw ex
            End If
        End Sub

        ' ���ܣ�ϵͳУ�飬���δͨ��ֱ���׳�����
        ' ������isLogin     �����Ƿ��½��Ϊ�˷�ֹ������ü������ù��������ò�������Ϊ����
        ' ˵����������Ƿ���ȷ��װ���ļ��Ϸ��ԡ�dll �汾�����ݿ�汾����˾�����û�������Ŀ����
        Public Shared Sub VerifyRuntime(ByVal isLogin As Boolean)
            ' ʵ������Ȩ����� License �ļ��ж�ȡ��Ϣ
            Dim MyLicense As New License(True)

            Dim MyDog As ISoftDog

            MyDog = SoftDogFactory.CreateSoftDogObject
            MyDog.Crc32ReadPos = MyLicense.Crc32ReadPos
            If MyDog.IsZydog Then
                If isLogin Then
                    ' 0����½ʱ�����ù�ʹ�ô���
                    MyDog.MinusCounter()

                End If

            Else
                ' 1����Ȩ�ļ��Ϸ���У�飬���ù�����
                verifyLicenseFile(MyDog.LicenseCrc32, MyDog.DogId)

            End If

            ' 2��dll �汾У��
            verifyDllVersion(MyLicense.GetLicenseXmlDocument)

            ' 3��Db �汾У��
            verifyDBVersion(MyLicense.GetLicenseXmlDocument)

            ' 4����˾��У��
            verifyCompanyCount(MyLicense.CompanyCount)

            ' 5����Ŀ��У��
            verifyProjectCount(MyLicense.ProjectCount)

            ' 6����ͨ�û���У��
            ' modified by chenyong 2009-08-14 ��ͨ�û���У��
            verifyNormalUserCount(MyLicense.NormalUserCount)

            ' 7������ϵͳ�û���У��
            'modified by chenyong 2011-03-10 ����ϵͳ�û���У��
            verifySaleUserCount(MyLicense.SaleUserCount)

            ' 8��ERP�û���У��
            ' modified by chenyong 2009-08-14 ERP�û���У��
            verifyErpUserCount(MyLicense.ErpUserCount)
        End Sub

        ' ���ܣ���ͨ�û���У��
        ' �޸ģ����� 2009-08-14
        Private Shared Sub verifyNormalUserCount(ByVal normalUserCount As Integer)
            If normalUserCount >= 0 Then
                If GetNormalUserCount(Nothing) > normalUserCount Then
                    Throw New System.Exception("�Բ������ݿ��е���ͨ�û�������������Χ��")
                    Exit Sub
                End If
            End If
        End Sub

        ' ���ܣ���¥ϵͳ�û���У��
        ' �޸ģ����� 2011-03-10
        Private Shared Sub verifySaleUserCount(ByVal saleUserCount As Integer)
            If saleUserCount >= 0 Then
                If GetSaleUserCount(Nothing) > saleUserCount Then
                    Throw New System.Exception("�Բ������ݿ��е�����ϵͳ�û�������������Χ��")
                    Exit Sub
                End If
            End If
        End Sub

        ' ���ܣ�ERP�û���У��
        ' �޸ģ����� 2009-08-14
        Private Shared Sub verifyErpUserCount(ByVal normalUserCount As Integer)
            If normalUserCount >= 0 Then
                If GetErpUserCount(Nothing) > normalUserCount Then
                    Throw New System.Exception("�Բ������ݿ��е�ERP�û�������������Χ��")
                    Exit Sub
                End If
            End If
        End Sub

        ' ���ܣ���ȡ��ȨӦ��ϵͳ�����ַ���
        ' ���أ�01,0101,0102,02,0201
        ' ע�⣺���� License �ļ��в����ڵ�ϵͳ
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

        ' ���ܣ���ȡϵͳ����Ȩ���ܴ����ַ���
        ' ������application     ��ϵͳ����
        '       level           ��������� -1 ��ʾ���м���
        ' ���أ�010102,01010201,01010202
        ' ע�⣺���� License �ļ��в����ڵĹ���
        Public Shared Function GetLicenseFunctionString(ByVal application As String) As String
            Return GetLicenseFunctionString(application, -1)
        End Function

        Public Shared Function GetLicenseFunctionString(ByVal application As String, ByVal level As Integer) As String
            Dim MyLicense As New License

            Return MyLicense.GetLicenseFunctionString(application, level)
        End Function

        ' ���ܣ�У�鹦����Ȩ��������δͨ��ֱ���׳�����
        ' ע�⣺License �ļ��в����ڵĹ���У��Ҳ�޷�ͨ��
        Public Shared Sub VerifyLicenseFunction(ByVal functionCode As String)
            If Not CheckLicenseObject(functionCode, "����") Then
                Throw New System.Exception("�Բ���""" & MyDB.GetDataItemString("SELECT FunctionName FROM myFunction WHERE FunctionCode='" & functionCode & "'") & _
                    """����δ����Ȩʹ�ã�")

            End If

        End Sub

        ' ���ܣ�У�鹦���Ƿ���Ȩ
        Public Shared Function CheckLicenseFunction(ByVal functionCode As String) As Boolean
            Return CheckLicenseObject(functionCode, "����")

        End Function

        ' ���ܣ�У������Ƿ���Ȩ
        ' ������licenseObject      ������������
        '       licenseObjectType      ��У��������ͣ�ϵͳ/���ܣ�
        Public Shared Function CheckLicenseObject(ByVal licenseObject As String, ByVal licenseObjectType As String) As Boolean
            Dim MyLicense As New License

            Return MyLicense.CheckLicenseObject(licenseObject, licenseObjectType)

        End Function

        ' ���ܣ���ȡ��ҵ��Ӱ����Ҫ���صĶ����ַ���
        ' ������objectType          ���������͡�ö�٣�1�����ܣ�2������+'.'+������3��ҵ�������
        '       additionalParam     �����Ӳ������ò����������£�
        '       1���� objectType=1��2ʱ���ò���Ϊϵͳ�����գ�ֻ���˸�ϵͳ�µĹ��ܺͶ�����Ŀ���Ƿ�ֹ���ص��ַ�����������ϵͳ���ˡ�
        '       ע����� additionalParam ����Ϊ�ղ������Ӳ������˴���
        ' ���أ�1���� objectType=1 ʱ�����ع��ܴ��봮���磺01010101,01010102
        '       2���� objectType=2 ʱ�����ع���+'.'+�������봮���磺01010101.01,01010102.02
        Public Shared Function GetNotInFilterString(ByVal objectType As String, ByVal additionalParam As String) As String
            Dim SqlConn As SqlClient.SqlConnection
            Dim SqlComm As SqlClient.SqlCommand
            Dim SqlParams As SqlParameterCollection

            'Dim LicenseApplication As New StringBuilder         ' ��Ȩ�������õ�ϵͳ
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

        ' ���ܣ���ȡ����Ȩ��Ʒ�� HTML �ַ�����About ������
        ' ��ʾ��ʽ�磺���۹���ϵͳ SP1 V5.0.405.0
        ''�ռ���ϲ��߼�����ͨ�û� 2008.03.13 yuzy ����ϵͳ����Ҫ��ʾ��Ȩ�û���
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

                            ' ��� 0201 �Ǻ�ͬ��������Ʒ�����еġ��ɱ����滻�ɡ���ͬ��   2008.03.18 zt
                            ProductName = FileProperties.ProductName
                            If item.Attributes("code").Value = "0201" Then
                                If item.Attributes("name").Value.IndexOf("��ͬ") >= 0 Then
                                    ProductName = ProductName.Replace("�ɱ�", "��ͬ")
                                End If
                            End If

                            '2009-9-19 huyl �����ǰ��0206���򽫡�ȫ��Ԥ�㡱��Ϊ�����ù���
                            If item.Attributes("code").Value = "0206" Then
                                ProductName = ProductName.Replace("ȫ��Ԥ��", "���ù���")

                            End If

                            LicenseProductInfo.Append(ProductName & " (" & FileProperties.ProductVersion & ") " & ChrW(13) & ChrW(10))
                        End If
                      
                    Catch ex As System.IO.FileNotFoundException
                        Mysoft.Map.Data.MyDB.LogException(ex)
                        ' ��� dll �����ھͲ���ʾ

                    End Try
                End If

            Next
            Return LicenseProductInfo.ToString

        End Function

        '���ܣ���ȡ���ݿ�����ͨ�û��� 
        '˵����������userGUID��ֵ,���ų���userGUID������
        '�޸ģ����� 2009-08-14
        Public Shared Function GetNormalUserCount(ByVal userGUID As String) As Integer
            Return GetDBUserCount(UserKind.NormalUser, userGUID)
        End Function

        '���ܣ���ȡ���ݿ�������ϵͳ�û��� 
        '˵����������userGUID��ֵ,���ų���userGUID������
        '�޸ģ����� 2011-03-10
        Public Shared Function GetSaleUserCount(ByVal userGUID As String) As Integer
            Return GetDBUserCount(UserKind.Sale, userGUID)
        End Function

        '���ܣ���ȡ���ݿ���Erp�û���
        '˵����������userGUID��ֵ,���ų���userGUID������
        '�޸ģ����� 2009-08-14
        Public Shared Function GetErpUserCount(ByVal userGUID As String) As Integer
            Return GetDBUserCount(UserKind.ErpUser, userGUID)
        End Function

        '���ܣ���ȡ���ݿ����û���
        '˵��������userKindΪ�û����ͣ�������userGUID��ֵ,���ų���userGUID������
        '�޸ģ����� 2009-08-14
        Private Shared Function GetDBUserCount(ByVal userType As Integer, ByVal userGUID As String) As Integer
            Dim _strSQL As String = "SELECT COUNT(*) FROM myUser WHERE UserCode<>'Admin' AND (IsDisabeld=0 OR IsDisabeld IS null) AND UserKind = " & userType.ToString()

            If Not String.IsNullOrEmpty(userGUID) Then
                _strSQL = _strSQL & " and UserGUID<>'" & userGUID & "'"
            End If

            Return MyDB.GetDataItemInt(_strSQL)
        End Function


        ' ���ܣ���ȡ��Ȩ��˾��
        Public Shared Function GetLicenseCompanyCount() As Integer
            Dim MyLicense As New License

            Return MyLicense.CompanyCount

        End Function


        ' ���ܣ���ȡ��Ȩ�û���Ϣ
        ' ˵����About ������
        ' �޸ģ����� 2009-08-14
        Public Shared Function GetLicenseUserHTML() As String
            Dim MyLicense As New License
            Dim strHTML As New Text.StringBuilder

            strHTML.Append("��")

            If MyLicense.ErpUserCount = _MAXCOUNT Then
                strHTML.Append("ERP�û��������� &nbsp;&nbsp;")
            Else
                strHTML.Append("ERP�û�����" & MyLicense.ErpUserCount.ToString() & "&nbsp;&nbsp;")
            End If

            If MyLicense.NormalUserCount = _MAXCOUNT Then
                strHTML.Append("��ͨ�û��������� &nbsp;&nbsp;")
            Else
                strHTML.Append("��ͨ�û�����" & MyLicense.NormalUserCount.ToString() & "&nbsp;&nbsp;")
            End If

            'modified by chenyong 2011-03-10 ����ϵͳ�û�����
            If MyLicense.SaleUserCount = _MAXCOUNT Then
                strHTML.Append("����ϵͳ�û��������� ")
            Else
                strHTML.Append("����ϵͳ�û�����" & MyLicense.SaleUserCount.ToString())
            End If

            strHTML.Append("��")

            Return strHTML.ToString()
        End Function


        ' ���ܣ���ȡ��Ȩ��Ŀ��
        Public Shared Function GetLicenseProjectCount() As Integer
            Dim MyLicense As New License

            Return MyLicense.ProjectCount

        End Function

        ' ========================== ˽�з���

        ' ���ܣ���Ȩ�ļ��Ϸ���У��
        Private Shared Sub verifyLicenseFile(ByVal licenseCrc32 As Integer, ByVal dogId As String)
            Dim strLicenseFile As String
            strLicenseFile = HttpContext.Current.Server.MapPath("/bin/License.xml")

            'У�麯���в�Ӧ�׳��쳣��������ȷ�Ĵ�����Ϣ�ȣ����ⱻ����         -- Lionel  2009.05.14
            If GeneralBase.Crc32IntegerFile(strLicenseFile) <> licenseCrc32 Then
                'Throw New System.Exception("�Ƿ�����Ȩ�ļ����������Ϣ����Ȩ�ļ���ƥ�䣩��")
                Throw New System.Exception("�Ƿ�����Ȩ�ļ���")
            End If

            '��֤��Ȩ�ļ�ǩ��
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
                '��֤ǩ��
                xmlNData = xmlDoc.DocumentElement.SelectSingleNode("data")
                strData = xmlNData.InnerXml
                'modified by kongy 2009-08-19 ��Ϊ����MySoft.Map.Security.RSAUtilִ��ǩ����֤
                If Not RSAUtil.GetHash(strData, strHashData) Then
                    Throw New System.Exception("�Ƿ�����Ȩ�ļ���")
                End If

                xmlNSigData = xmlDoc.DocumentElement.SelectSingleNode("signature")
                If Not RSAUtil.SignatureDeformatterByHash(PUBLIC_KEY, strHashData, xmlNSigData.InnerText) Then
                    Throw New System.Exception("�Ƿ�����Ȩ�ļ���")
                End If

                xmlLicense = New XmlDocument
                xmlLicense.LoadXml(strData)

                xmlAttr = xmlLicense.DocumentElement.Attributes.GetNamedItem("dogid")
                If String.Compare(xmlAttr.Value, dogId) <> 0 Then
                    Throw New System.Exception("�Ƿ�����Ȩ�ļ���")
                End If

                'У�� Application �б������Ȩ�ĵ�����
                If Not HttpContext.Current.Application("_licenseXmlDocument") Is Nothing Then
                    xmlLicense = CType(HttpContext.Current.Application("_licenseXmlDocument"), XmlDocument)

                    If String.Compare(strData, xmlLicense.DocumentElement.OuterXml) <> 0 Then
                        Throw New Exception("�Ƿ�����Ȩ�ļ���")
                    End If
                End If

            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Throw New System.Exception("�Ƿ�����Ȩ�ļ���")
            End Try
        End Sub

        ' ���ܣ�У�� dll �汾��Ϣ
        Private Shared Sub verifyDllVersion(ByVal licenseXmlDocument As XmlDocument)
            Dim SubApplicationNodeList As XmlNodeList
            Dim i As Integer
            Dim ErrMessage As String

            SubApplicationNodeList = licenseXmlDocument.SelectNodes("/product/system/subsystem")
            For i = 0 To SubApplicationNodeList.Count - 1
                ' У����ϵͳ�汾�š����û��ָ�� dll �� �汾�žͲ�У��
                If SubApplicationNodeList.Item(i).Attributes("dllname").Value <> "" And SubApplicationNodeList.Item(i).Attributes("version").Value <> "" Then
                    ErrMessage = compareToDllVersion(SubApplicationNodeList.Item(i).Attributes("dllname").Value, SubApplicationNodeList.Item(i).Attributes("version").Value)
                    If ErrMessage <> "" Then
                        Throw New System.Exception(ErrMessage)

                    End If

                End If

            Next

        End Sub

        ' ���ܣ������ϵͳ��ʵ�汾��
        ' ������dllName     ���� dll �ļ������磺slxt.dll
        '       licenseVersion      ���� ��Ȩ�汾��
        ' ���أ�
        ' ˵��������Ȩ�ļ��еİ汾��ʵ�ʰ汾�ȶԣ�ֻУ����.���汾��
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
                        Return dllName & " ����Ȩ�汾������ʵ�汾�Ų�һ�£�"
                    End If
                End If

            Catch ex As System.io.FileNotFoundException
            	Mysoft.Map.Data.MyDB.LogException(ex)
                ' ��� dll �ļ������ھͲ�У��
                Return ""

            End Try

        End Function

        ' ���ܣ�У�� DB �汾��Ϣ
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

        ' ���ܣ�У�鹫˾��
        Private Shared Sub verifyCompanyCount(ByVal companyCount As Integer)
            If companyCount > 0 Then
                If MyDB.GetDataItemInt("SELECT COUNT(*) FROM myBusinessUnit WHERE IsEndCompany = 1") > companyCount Then
                    Throw New System.Exception("�Բ������ݿ��еĹ�˾����������Χ��")
                    Exit Sub
                End If

            End If

        End Sub


        ' ���ܣ�У����Ŀ��
        Private Shared Sub verifyProjectCount(ByVal projectCount As Integer)
            If projectCount > 0 Then
                If MyDB.GetDataItemInt("SELECT COUNT(*) FROM p_Project WHERE Level=2 And IsGt=0") > projectCount Then
                    Throw New System.Exception("�Բ������ݿ��е���Ŀ����������Χ��")
                    Exit Sub
                End If

            End If

        End Sub

        ' ���ܣ�������ݿ�汾
        ' ������dllName     ���� dll �ļ������磺slxt.dll
        '       licenseVersion      ���� ��Ȩ�汾��
        ' ���أ�
        ' ˵����ʵ�ʰ汾�����ݿ��м�¼�İ汾�ȶԣ�ȫУ�飨������.��.�ڲ�.�޶��ţ�
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
                            Return "��" & ApplicationName & "����ǰʹ�õ����ݿ�汾����ȷ��"
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


        '������Ȩ�ĳ���������У����ϵͳ�û����Ƿ񳬱�
        Public Enum SetRightType
            AddRightsToStation = 0       'Ϊ��λ��Ȩ
            AddUsersToStation = 1        'Ϊ��λ��ӳ�Ա
            AddStationsToUser = 2        'Ϊ��Ա�����λ
        End Enum

        ' ���ܣ����û���Ȩǰ��У�����ϵͳ����Ȩ�û����Ƿ񳬱�
        ' 2007.01.17 yuzy
        '������     a_enmSetRightType       ��Ȩ������ʹ�á�SetRightType��ö��ֵ
        '           a_strStationGUIDList    ��λGUID
        '           a_strUserGUIDList       �û�GUID��Ϊ��λ��Ȩʱ�ò���Ϊ���ַ�����
        '           a_strFunccode           ��Ȩ���ܴ��루����Ϊ��λ��Ȩʱʹ�ã�
        '���أ�     true    ͨ����������   ��   false   ��ͨ��������
        Public Shared Function CheckApplicationUserCount(ByVal a_enmSetRightType As SetRightType, ByVal a_strStationGUIDList As String, ByVal a_strUserGUIDList As String, ByVal a_strFunccode As String) As Boolean
            Dim AdminCount, DestUsersCount As Integer
            Dim SQL, strDestUsersClause, strDestFuncClause As String

            ' 1�����㳬���û������ų��߼��û�����
            AdminCount = MyDB.GetDataItemInt("SELECT COUNT(*) FROM myUser WHERE IsAdmin=1 AND (IsDisabeld=0 OR IsDisabeld IS null) AND (IsAdvanceUser=0 OR IsAdvanceUser is NULL)")

            ' 2������Ҫ����Ȩ�޵��û���������ǰ��λ�ĳ�Ա�����ų������û��������û����߼��û�����
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

                    'ƴ����Ȩ�û����Ӿ�
                    strDestUsersClause = "(select distinct(su.UserGUID) from myStationUser su where su.StationGUID in ('" & a_strStationGUIDList.Replace(",", "','") & "'))"
                    'ƴ����Ȩ���ܴ�����־�
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


            ' 3������Ȩ�û����Ƚϣ��ų��߼��û�����
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

        ' ���ܣ���������ֵģ��Ӱ���û����Ȩ�Ĺ��ܶ�������˱��ʽ
        ' ���أ�<���ܴ���1>.<��������1>,<���ܴ���2>.<��������2>
        Public Shared Function GetNotInFilterStringByIncrement() As String
            Dim SQL As String
            Dim DT As DataTable
            Dim FilterExp As New StringBuilder
            Dim i As Integer

            ' 1����ֵģ��Ӱ��
            SQL = "SELECT LicenseObjectType,LicenseObject,ROPriProperty,ROSecProperty,ROType FROM myLicenseRelationShip" & _
                  " WHERE ROType='������' OR ROType='���沿��' ORDER BY LicenseObject"
            DT = MyDB.GetDataTable(SQL)
            If DT.Rows.Count > 0 Then
                ' ϵͳ���ܱ�������û����Ȩͬ�ȴ���  ʷ���塢��ǰ    zt 2007.3.13
                Dim dtApplication As DataTable
                Dim bolIsDisabled As Boolean        '  �Ƿ񱻽���
                Dim j As Integer

                ' ����ģ��Զ������Ӱ�첻����  ʷ���塢�ﺣ��  zt 2007.3.22
                dtApplication = MyDB.GetDataTable("SELECT Application AS Code FROM myApplication WHERE IsDisabled=1")
                bolIsDisabled = False
                For i = 0 To DT.Rows.Count - 1
                    bolIsDisabled = False

                    ' �ж��Ƿ��ѱ�����
                    For j = 0 To dtApplication.Rows.Count - 1
                        If DT.Rows(i)("LicenseObject").ToString.ToLower = dtApplication.Rows(j)("Code").ToString.ToLower Then
                            bolIsDisabled = True
                            Exit For
                        End If
                    Next

                    ' �ж�ϵͳ/����Ȩ���ж��Ƿ���Ȩ
                    If bolIsDisabled Or Not VerifyApplication.CheckLicenseObject(DT.Rows(i)("LicenseObject").ToString, DT.Rows(i)("LicenseObjectType").ToString) Then
                        If DT.Rows(i)("ROType").ToString = "������" Then
                            ' 1.1������Ƕ�����
                            FilterExp.Append("," & DT.Rows(i)("ROPriProperty").ToString & "." & DT.Rows(i)("ROSecProperty").ToString)
                        ElseIf DT.Rows(i)("ROType").ToString = "���沿��" Then
                            ' 1.2����������沿��
                            FilterExp.Append(",0000029902." & DT.Rows(i)("ROPriProperty").ToString)
                        End If
                    End If
                Next
            End If

            ' 2�����沿����ϵͳӰ��
            SQL = "SELECT WPCode FROM myWebPart WHERE Application NOT IN ('" & VerifyApplication.GetLicenseApplicationString.Replace(",", "','") & "')"
            DT = MyDB.GetDataTable(SQL)
            If DT.Rows.Count > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    FilterExp.Append(",0000029902." & DT.Rows(i)("WPCode").ToString)
                Next
            End If

            Return FilterExp.ToString
        End Function

        '���ܣ���ȡ�û��Ƿ��иù��ܵ�ʹ��Ȩ�ޣ��������������жϣ�
        '1��ģ���Ƿ���Ȩ
        '2������ģ�鼰ģ������ϵͳ�Ƿ�����
        '3����ģ���Ƿ�ҵ��Ӱ����Ҫ����
        ' ����1��application    ���� ��������ϵͳ�Ĵ���
        ' ����2��functionCode   ���� ���ܴ���
        '˵������������ͨ������ģ��
        Public Shared Function CheckFunctionAllowUse(ByVal application As String, ByVal functionCode As String) As Boolean
            '1��ģ���Ƿ���Ȩ
            If GetLicenseFunctionString(application).IndexOf(functionCode) < 0 Then Return False

            '2.����ģ���Ƿ������� �� ��������ϵͳ�Ƿ�������
            Dim strSql As String
            strSql = "SELECT top 1 (0) From myApplication A LEFT JOIN myFunction F ON A.Application=F.Application " & _
                     " WHERE A.isDisabled=0 AND F.isDisabled=0 AND F.FunctionCode='" & functionCode & "'"
            If MyDB.GetDataTable(strSql).Rows.Count = 0 Then Return False

            '3����ģ���Ƿ���ҵ��Ӱ����Ҫ����
            If GetNotInFilterString(1, application).IndexOf(functionCode) >= 0 Then Return False

            Return True

        End Function

#Region "ע�ʹ���"
        ' ���ܣ���ȡ��Ȩ���û���
        'Public Shared Function GetLicenseUserCount() As Integer
        '    Dim MyLicense As New License

        '    Return MyLicense.UserCount

        'End Function

        ' ���ܣ���ȡ��Ȩ�߼��û���Ϣ
        ' ˵����About ������
        'Public Shared Function GetLicenseAdvanceUserHTML() As String
        '    Dim MyLicense As New License

        '    '�ռ���ϲ��߼�����ͨ�û� 2008.03.13 yuzy
        '    If MyLicense.AdvanceUserCount = _MAXCOUNT Then
        '        Return "����Ȩ�û��������ޣ�"
        '    Else
        '        Return "����Ȩ�û�����" & MyLicense.AdvanceUserCount.ToString & "��"
        '    End If

        'End Function

        ' ���ܣ���ȡ��Ȩ�߼��û���
        'Public Shared Function GetLicenseAdvanceUserCount() As Integer
        '    Dim MyLicense As New License

        '    Return MyLicense.AdvanceUserCount.ToString

        'End Function

        ' ���ܣ����û���У��
        'Private Shared Sub verifyUserCount(ByVal userCount As Integer)
        '    If userCount > 0 Then
        '        If MyDB.GetDataItemInt("SELECT COUNT(*) FROM myUser WHERE (IsDisabeld=0 OR IsDisabeld IS null)") > userCount Then
        '            Throw New System.Exception("�Բ������ݿ��е����û�����������Χ����")
        '            Exit Sub
        '        End If

        '    End If

        'End Sub

        ' ���ܣ��߼�����У��
        '       �ռ���ϲ��߼�����ͨ�û���adminΪ�����û�����������Ȩ�û�����  2008.03.13 yuzy
        'Private Shared Sub verifyAdvanceUserCount(ByVal userCount As Integer)
        '    If userCount > 0 Then
        '        If MyDB.GetDataItemInt("SELECT COUNT(*) FROM myUser WHERE UserCode<>'Admin' AND (IsDisabeld=0 OR IsDisabeld IS null) AND IsAdvanceUser = 1") > userCount Then
        '            'Throw New System.Exception("�Բ������ݿ��еĸ߼��û�����������Χ����")
        '            Throw New System.Exception("�Բ������ݿ��е��û�����������Χ����")
        '            Exit Sub
        '        End If

        '    End If

        'End Sub

        ' ���ܣ���ϵͳ�û���У��
        'Private Shared Sub verifyApplicationUserCount()
        '    Dim DT As DataTable

        '    DT = MyDB.GetDataTable("SELECT ApplicationName FROM myApplication WHERE ActualUserCount>LicenseUserCount AND Level=1")
        '    If DT.Rows.Count > 0 Then
        '        Dim i As Integer
        '        Dim ErrorMessage As String

        '        ErrorMessage = "�Բ���"
        '        For i = 0 To DT.Rows.Count - 1
        '            If i = 0 Then
        '                ErrorMessage &= "[" & DT.Rows(i)("ApplicationName") & "]"
        '            Else
        '                ErrorMessage &= "[" & DT.Rows(i)("ApplicationName") & "]"
        '            End If
        '        Next
        '        ErrorMessage &= " ���û���������Ȩ��Χ��"

        '        Throw New System.Exception(ErrorMessage)

        '    End If

        'End Sub

        ' ���ܣ��� myApplication ������д ��Ȩ�û��� �� ʵ���û���
        ' 2006.04.27 zt
        'Public Shared Sub SetApplicationUserCount()
        '    Dim SQL As New StringBuilder
        '    Dim DTActualUser As DataTable           ' ��Ȩ�û���
        '    Dim AdminCount As Integer               ' �����û�������
        '    Dim MyLicense As New License

        '    ' 2�������û���
        '    AdminCount = MyDB.GetDataItemInt("SELECT COUNT(*) FROM myUser WHERE IsAdmin=1 AND (IsDisabeld=0 OR IsDisabeld IS null) AND (IsAdvanceUser=0 OR IsAdvanceUser is NULL)")

        '    ' 3��ϵͳ�û�����¼��
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

        '    ' 4��ƴд update ���
        '    If DTActualUser.Rows.Count > 0 Then
        '        Dim i As Integer

        '        SQL = New StringBuilder

        '        For i = 0 To DTActualUser.Rows.Count - 1
        '            If i <> 0 Then
        '                SQL.Append(";")
        '            End If

        '            ' ȡ���û�������ϵͳ�û�������С����Ϊ��Ȩ�û���
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
