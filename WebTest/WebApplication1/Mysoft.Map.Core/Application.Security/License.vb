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

        ' ���ܣ�ʵ������Ȩ����
        ' ������isLoadLicenseFile     ���Ƿ�� License.xml �ж�ȡ��Ϣ������� Application ������ȡ
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
                Throw New System.Exception("��ȡ��Ȩ�ļ�ʧ�ܣ�")
            End Try
        End Sub

        ' ============================ ���� ===============================

        ' ���ܣ�У����Ȩ
        Public Function CheckLicenseObject(ByVal objectCode As String, ByVal objectType As String) As Boolean
            '2009-9-21 huyl ���Ӷ�һ��ϵͳ���ж�
            Dim node As XmlNode
            Select Case objectType
                Case "һ��ϵͳ" 'У�����Ϊһ��ϵͳ
                    node = _licenseXmlDocument.SelectSingleNode("/product/system[@code=""" & objectCode & """]")
                    Exit Select

                Case "ϵͳ" ' У�����Ϊ��ϵͳ
                    node = _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & objectCode & """]")
                    Exit Select

                Case Else 'У�����Ϊ��ϵͳ����ģ��
                    node = _licenseXmlDocument.SelectSingleNode("/product/system/subsystem/function[@code=""" & objectCode & """]")
                    Exit Select

            End Select

            If node Is Nothing Then '��ǰ�ڵ㲻���ڣ�����False
                Return False
            ElseIf node.Attributes("enabled") Is Nothing Then '��ǰ�ڵ㲻����;enabled���ԣ�����False
                Return False
            ElseIf node.Attributes("enabled").Value = "1" Then '��ǰ�ڵ��enabled����ֵΪ1������True
                Return True
            Else '��ǰ�ڵ��enabled����ֵ��Ϊ1������False
                Return False
            End If

        End Function

        ' ���ܣ���ȡ��ȨӦ��ϵͳ����
        ' ���أ�01,0101,0102,02,0201
        ' ע�⣺���û����Ȩ���ؿ��ַ���
        Public Function GetLicenseApplicationString() As String
            Dim ApplicationNodeList As XmlNodeList
            Dim ApplicationInLicense As New StringBuilder         ' ������ License �ļ�������ϵͳ�б�
            Dim LicenseApplication As New StringBuilder           ' ����Ȩ��ϵͳ�б�
            Dim i As Integer

            ' 1��ƴд��Ȩϵͳ
            ' 1.1��ϵͳ
            ApplicationNodeList = _licenseXmlDocument.SelectNodes("/product/system")
            For i = 0 To ApplicationNodeList.Count - 1
                ApplicationInLicense.Append("," & ApplicationNodeList.Item(i).Attributes("code").Value)

                If Not ApplicationNodeList.Item(i).Attributes("enabled") Is Nothing Then
                    If ApplicationNodeList.Item(i).Attributes("enabled").Value = "1" Then
                        LicenseApplication.Append("," & ApplicationNodeList.Item(i).Attributes("code").Value)

                    End If

                End If

            Next

            ' 1.2����ϵͳ
            ApplicationNodeList = _licenseXmlDocument.SelectNodes("/product/system/subsystem")
            For i = 0 To ApplicationNodeList.Count - 1
                ApplicationInLicense.Append("," & ApplicationNodeList.Item(i).Attributes("code").Value)

                If Not ApplicationNodeList.Item(i).Attributes("enabled") Is Nothing Then
                    If ApplicationNodeList.Item(i).Attributes("enabled").Value = "1" Then
                        LicenseApplication.Append("," & ApplicationNodeList.Item(i).Attributes("code").Value)

                    End If

                End If

            Next

            ' 2��ƴд��Ȩ�ļ��в����ڵ�ϵͳ
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

        ' ���ܣ���ȡ��Ȩ���ܴ���
        ' ������application     ��ϵͳ����
        '       level           �����������1��ʾ���м���
        ' ���أ�01010201,01010202
        ' ˵�����ų����⹦�ܺͱ���
        ' ע�⣺���û����Ȩ���ؿ��ַ���
        Public Function GetLicenseFunctionString(ByVal application As String) As String
            Return GetLicenseFunctionString(application, -1)

        End Function

        Public Function GetLicenseFunctionString(ByVal application As String, ByVal level As Integer) As String
            Dim FunctionNodeList As XmlNodeList
            Dim FunctionInLicense As New StringBuilder
            Dim LicenseFunction As New StringBuilder
            Dim i As Integer

            ' 1����ȡ��Ȩ�Ĺ���
            FunctionNodeList = _licenseXmlDocument.SelectNodes("/product/system/subsystem[@code=""" & application & """]/function")
            For i = 0 To FunctionNodeList.Count - 1
                FunctionInLicense.Append("," & FunctionNodeList.Item(i).Attributes("code").Value)

                If Not FunctionNodeList.Item(i).Attributes("enabled") Is Nothing Then
                    If FunctionNodeList.Item(i).Attributes("enabled").Value = "1" And IIf(level = -1, True, FunctionNodeList.Item(i).Attributes("level").Value = level) Then
                        LicenseFunction.Append("," & FunctionNodeList.Item(i).Attributes("code").Value)

                    End If

                End If

            Next

            ' 2����ȡ��Ȩ�ļ��в������Ĺ���
            Dim DT As DataTable
            Dim SQL As String

            SQL = "SELECT FunctionCode FROM myFunction WHERE FunctionCode NOT IN ('" & FunctionInLicense.ToString.Replace(",", "','") & "') " & _
                  " AND Application = '" & application & "' AND FunctionType='����'"
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

        ' ���ܣ���ȡ License �ļ� XmlDocument
        Public Function GetLicenseXmlDocument() As XmlDocument
            Return Me._licenseXmlDocument
        End Function

        ' ============================ ���� ===============================

        ' ���ܣ����ݿ����ӵ�ע�������
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

        ' ���ܣ���Ȩ��˾��
        ' ˵����������� 0 ��ʾ������
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
                            Throw New System.Exception("License �ļ��й�˾����Ȩ����")

                        End If

                    End If

                End If

                Return _CompanyCount

            End Get

        End Property

        ' ���ܣ�����License�ļ������õ��û���
        ' �޸ģ����� 2011-03-10 ����ϵͳ�û�����
        ' ˵�������������ȡֵΪ��������/=��/<0����ʾ�����û���
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

        ' ���ܣ�����License�ļ������õ���ͨ�û���
        ' �޸ģ����� 2009-08-14
        ' ˵����NormalUserCount ���ԡ�������/=��/<0����ʾ�����û���
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

        ' ���ܣ�����License�ļ������õ�����ϵͳ�û���
        ' �޸ģ����� 2011-03-10 ����ϵͳ�û�����
        ' ˵����SaleUserCount ���ԡ�������/=��/<0����ʾ�����û���
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

        ' ���ܣ�����License�ļ������õ�Erp�û���
        ' �޸ģ����� 2009-08-14
        ' ˵����ErpUserCount ���ԡ�������/=��/<0����ʾ�����û���
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


        ' ���ܣ���Ȩ��Ŀ��
        ' ˵����������� 0 ��ʾ������
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
                            Throw New System.Exception("License �ļ�����Ŀ����Ȩ����")

                        End If

                    End If

                End If

                Return _ProjectCount

            End Get

        End Property

        ' ���ܣ�License �ļ���������������д洢����ʼλ�ã����� Aladdin��
        ' ˵����Ĭ��λ���� 0
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

        ' ���ܣ���Ȩģʽ
        ' ���أ����� 0 ������
        ' ˵������ ERP1.0~ERP2.5 SP1��LicenseMode = 1��֮��汾 LicenseMode = 2��
        '       ��� licensemode �ڵ㲻���ڣ�Ĭ�� 1��
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

#Region "ע�͵Ĵ���"
        ' ���ܣ�����Ȩ�û���
        ' ˵����usercount ���ԡ�������/=��/<=0����ʾ�����û���
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
        '                    Throw New System.Exception("License �ļ������û�����Ȩ����")

        '                End If

        '            End If

        '        End If

        '        Return _UserCount
        '    End Get
        'End Property



        ' ���ܣ���ϵͳ��Ȩ�û���
        ' ˵���������ϵͳ�ڵ㡰�����ڡ��û���Ϊ0��usercount ���ԡ�������/=��/<=0����ʾ�����û���
        '       Ϊ����֮ǰ����ͳһ��û����Ȩ����ϵͳ�û�������Ϊ���ޡ�
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
        '            If _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]") Is Nothing Then     ' �ڵ㲻����Ĭ��Ϊ0
        '                _UserCount = 0

        '            Else
        '                If CheckLicenseObject(application, "ϵͳ") Then
        '                    If _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]").Attributes("usercount") Is Nothing Then
        '                        _UserCount = _maxcount
        '                    ElseIf _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]").Attributes("usercount").Value = "" Then
        '                        _UserCount = _maxcount
        '                    ElseIf _licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]").Attributes("usercount").Value = "-1" Then
        '                        _UserCount = _maxcount
        '                    Else
        '                        _UserCount = Convert.ToInt32(_licenseXmlDocument.SelectSingleNode("/product/system/subsystem[@code=""" & application & """]").Attributes("usercount").Value)
        '                        If _UserCount < 0 Then
        '                            Throw New System.Exception("License �ļ�����ϵͳ�û�����Ȩ����")
        '                        End If
        '                    End If
        '                Else        ' �����ϵͳû����Ȩ���û���Ĭ������
        '                    _UserCount = _maxcount
        '                End If
        '            End If
        '        End If

        '        Return _UserCount
        '    End Get

        'End Property

        ' ���ܣ��߼���Ȩ�û���
        ' ˵���������ϵͳ�ڵ㡰�����ڡ��û���Ϊ0��usercount ���ԡ�������/=��/<=0����ʾ�����û���
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
        '                    'Throw New System.Exception("License �ļ��и߼��û�����Ȩ����")
        '                    Throw New System.Exception("License �ļ����û�����Ȩ����")     '�ռ���ϲ��߼�����ͨ�û� 2008.03.13 yuzy

        '                End If

        '            End If

        '        End If

        '        Return _UserCount
        '    End Get

        'End Property
#End Region

    End Class

End Namespace

