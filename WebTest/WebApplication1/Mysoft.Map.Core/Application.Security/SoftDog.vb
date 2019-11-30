Imports System.Web
Imports System.Configuration
Imports Mysoft.Map.Utility
Imports System.Reflection

Namespace Application.Security

    ' ��װ�����������
    Public Class SoftDogFactory

        Shared Function CreateSoftDogObject()
            Dim SoftDogType As String = ConfigurationManager.AppSettings("SoftDogType")
            Dim MyDog As ISoftDog

            If SoftDogType.ToLower = "aladdin" Then
                MyDog = New AladdinDog

            ElseIf SoftDogType.ToLower = "rainbow" Then
                MyDog = New RainbowDog

            End If

            Return MyDog

        End Function

    End Class

    ' ��װ������ӿ�
    Public Interface ISoftDog
        ReadOnly Property IsZydog() As Boolean
        ReadOnly Property DogPurpose() As Types.DogPurpose
        ReadOnly Property OverdueTimeUTC() As DateTime
        ReadOnly Property Owner() As String
        ReadOnly Property Counter() As Integer
        Property Crc32ReadPos() As Integer
        ReadOnly Property LicenseCrc32() As Integer
        ReadOnly Property DogId() As String

        Sub MinusCounter()

    End Interface

    'Public Class SoftDog
    '    Public IsZydog As Boolean
    '    Public Owner As String
    '    Public Counter As Int16
    '    Public LicenseCrc32 As Integer
    '    Public Crc32ReadPos As Integer = 0

    '    Public Sub New()

    '    End Sub

    '    ' ���ܣ����ù�ʹ�ô����� 1
    '    Public Sub MinusCounter()

    'End Class

    ' ��װ Aladdin �������
    Friend Class VendorCode

        '�˴��� VendorCode ��������Ȼ�ɱ��������������� VendorCode ֱ��д�ڶ����Ĵ�����
        '����Ĵ����ѱ����������ɱ����������Ի��ƽ���

        'The Base64 encoded vendor code for a demoma key.
        Protected Const vendorCodeString As String = _
        "wNI+N26edGX/Mx7s6RvbPP6RSVCNmUuhhROpFY+ky/Jx2mtGR3pXO06a0/iPhkXSkih9ZP5jprE8BRXY" + _
        "XLKliHWnL6of2i/7lIxq2h3ZwSag53Kki5Hk1OPGECPI1Ofkv/gYZstJo8cfjWujpwqJwEHtMS3jg8GA" + _
        "H6xp16JXQ3vnLGzOWgXqqGquVQ4h1M9kDF38wJKdhbgu2ozJLOEH04u7aEAIwt2svDFF+G0ZNS5/1dYD" + _
        "fHmvZ4yz+TGlfBCStTF3MccESv+Gg/7MdD2buXB2cM3pgtvPwfB+oey6iaRhiY9Wt6Vw6PkNtMQng2qB" + _
        "XJLS8EU+ztnsg77bF9llgA92bGPhVE2xkh50mr6FhC5DRvseEm1zpxEDWD+jrxvhgp3eeVQGOqTHOTSm" + _
        "R4LEsTK38MLT49Bcy0nAORaX1qW6cAVkgknammAKAersqibF4LVu48CCyKmeMr79MpksgOONHOIrlsvy" + _
        "47onbwTAoTlQUpSK+7EU0U/XqGvFwtGJ2XKCQsGgwItUoHyM3QAnHsQzCw7v121ZX2cuUtMCTvxpbtt6" + _
        "6+K8kwe1ge/c3QBtn/AxaIv4IS7hBOTb1TPfrWvixZFth1dRoFbZy5dYS6vEbWMHCF9nlR/RYccLpFMP" + _
        "3DLEWKggAvZKUDNO5Hg5HdNWEtbPGT9JQ8WHKmeAT3I9h7juGhG+/L9fu6SxfN6o8yEeMh+4Kf4yjmGg" + _
        "M/Al+AN3n/HwHw=="

        Sub New()
            MyBase.New()
        End Sub

        'Returns the vendor code as a byte array.
        Shared ReadOnly Property Code() As String
            Get
                'Dim vCode() As Byte = Text.ASCIIEncoding.Default.GetBytes(vendorCodeString)
                Return vendorCodeString
            End Get
        End Property

    End Class

    Friend Class AladdinDog
        Implements ISoftDog

        Private _IsZydog As Boolean
        Private _DogPurpose As Types.DogPurpose
        Private _OverdueTimeUTC As DateTime
        Private _Owner As String
        Private _Counter As Integer
        Private _Crc32ReadPos As Integer = 0
        Private _LicenseCrc32 As Integer
        Private _DogId As String

        Sub New()
            ' ע�⣺
            ' 1����ʵ����ʱû�е��� getDoginfo() ����������Ϊ������ʱ���ܻḳֵ Crc32ReadPos ���ԡ�
            ' 2�����ʧЧʱ��Ϊ 2099-12-31 ��ʾ��ʧЧ      
            ' Aladdin ��Ŀǰֻ��Ϊ��Ʒ��ʹ��
            Me._IsZydog = False
            Me._DogPurpose = Types.DogPurpose.None
            Me._OverdueTimeUTC = DateTime.Parse("1970-01-01")
            Me._Owner = ""
            Me._Counter = 0
            Me._Crc32ReadPos = 0
            Me._LicenseCrc32 = 0
        End Sub

        ' ================ ����
        ReadOnly Property IsZydog() As Boolean Implements ISoftDog.IsZydog
            Get
                'If Me._DogPurpose = Types.DogPurpose.ZY Then
                '    Me._IsZydog = True
                'Else
                '    Me._IsZydog = False
                'End If

                'Return Me._IsZydog

                Return False        ' Aladdin ��ֻ��Ϊ��Ʒ��ʹ��
            End Get
        End Property
        ReadOnly Property DogPurpose() As Types.DogPurpose Implements ISoftDog.DogPurpose
            Get
                If Me._DogPurpose = Types.DogPurpose.None Then
                    getDoginfo()

                End If

                Return Me._DogPurpose
            End Get
        End Property
        ReadOnly Property OverdueTimeUTC() As DateTime Implements ISoftDog.OverdueTimeUTC
            Get
                If Me._OverdueTimeUTC = DateTime.Parse("1970-01-01") Then
                    getDoginfo()

                End If

                Return Me._OverdueTimeUTC
            End Get
        End Property
        ReadOnly Property Owner() As String Implements ISoftDog.Owner
            Get
                Return Me._Owner
            End Get
        End Property
        ReadOnly Property Counter() As Integer Implements ISoftDog.Counter
            Get
                Return Me._Counter
            End Get
        End Property
        Property Crc32ReadPos() As Integer Implements ISoftDog.Crc32ReadPos
            Get
                Return Me._Crc32ReadPos
            End Get
            Set(ByVal Value As Integer)
                Me._Crc32ReadPos = Value
            End Set
        End Property
        ReadOnly Property LicenseCrc32() As Integer Implements ISoftDog.LicenseCrc32
            Get
                If _LicenseCrc32 = 0 Then
                    getDoginfo()
                End If

                Return _LicenseCrc32
            End Get
        End Property

        ReadOnly Property DogId() As String Implements ISoftDog.DogId
            Get
                Return Me._DogId
            End Get
        End Property

        ' ================ ����
        Sub MinusCounter() Implements ISoftDog.MinusCounter

        End Sub

        ' ================ ˽�к���
        ' ���ܣ���ȡ�������Ϣ�����������Ϣ��ֵ�������ڲ�����
        Private Sub getDoginfo()
            Dim strDLLName As String
            If IntPtr.Size < 8 Then
                ' 32 λϵͳ
                strDLLName = ConfigurationManager.AppSettings("Hasp32")

                'У����ļ��Ƿ񱻸Ķ�
                If GeneralBase.Crc32IntegerFile(HttpContext.Current.Server.MapPath(strDLLName)) <> &H5CB526D1 Then
                    Throw New Exception("δ�ҵ���������")
                End If
            Else
                ' 64 λϵͳ
                strDLLName = ConfigurationManager.AppSettings("Hasp64")

                'У����ļ��Ƿ񱻸Ķ�
                If GeneralBase.Crc32IntegerFile(HttpContext.Current.Server.MapPath(strDLLName)) <> &HB67EF00 Then
                    Throw New Exception("δ�ҵ���������")
                End If
            End If

            ' 1������ DLL
            Dim info As New AppDomainSetup
            Dim dom As AppDomain
            dom = AppDomain.CreateDomain("myDomain", Nothing, info)
            Dim asm As Reflection.Assembly
            asm = Reflection.Assembly.LoadFrom(HttpContext.Current.Server.MapPath(strDLLName))

            ' 2����ʼ��
            Dim feature As Object
            Dim pInfo As Reflection.PropertyInfo
            Dim objType As Type = asm.GetType("Aladdin.HASP.HaspFeature", True, True)

            pInfo = objType.GetProperty("ProgNumDefault")
            feature = pInfo.GetValue(objType, Nothing)

            feature.SetOptions(&H4000, &H0)

            Dim hasp As Object
            hasp = asm.CreateInstance("Aladdin.HASP.Hasp", True, Reflection.BindingFlags.Default, Nothing, New Object() {feature}, Nothing, Nothing)

            ' 3����¼
            Dim status As Object

            'Dim vendorCodeString As String
            'vendorCodeString = "jE/H8cWLtSeVIyRQTtS/S1J9kgRZQ7WtI0/FetR4T8d+Wl36m3gGTVqcsGY/XRF6NGFA3p8LcG5jg5q/"
            'vendorCodeString += "TKegXKNEGa/atu0bL7WRFUgzay93paF4uPnX3/LRuwN3lplq42NfbNMoEDE9wNXgV6YvhN/l8vKVLA2G"
            'vendorCodeString += "3mdC5nQZ6pk86qyfdPPxBGSETrMiRBcX+wNC7kG3O2wxu6reLwFy0I6rU+uByXs1+Q83Tm2GMraKoqD/"
            'vendorCodeString += "b82FpU5X6aF/szr7bRv+CeOc8HyOVkUBA5TFkLn+hYyD8YZK2e6UBhhdPOxZPvOjrBlpGjhauBecTTWE"
            'vendorCodeString += "xTfBF8EJQWn9aht0vxKPc2mTJe0aCHns09w/G9LbhVM0lxi9naqYy0rtAsKA01zvx65K6ZR6OQOgvqQK"
            'vendorCodeString += "vWvYes2AmydXEcrBxs7N+NlUmjOtjGJcQXftwlNost1sBszQY52fngbAHTTXX1mYI9ieyqtNZ9wcE+L4"
            'vendorCodeString += "euH5wWQ6JcBaa8RaOtCuqimhJc7Okak5i9c+fGQZNNbt7sTrf3w0ZzUihXPqH+CnuUCQpJvDNtSwCF8i"
            'vendorCodeString += "3ti5KZ/3nga2INOiYGeUWkhZVkeXNGe01/UW3dpe7Nl1G/sHqYBkX1UNV9orEghGduZS6huUpxHQ1iyL"
            'vendorCodeString += "QIoHKAn5RnxGE04eCzY5wkFk/52fJBsgacmciQkbA9EigxCtFklGUYGZk4Xbn+pEMQKRFs6BR74ygKj4"
            'vendorCodeString += "QNZt6ixF2iXv96xxKC5y4GDZc4eAPlxPvw32+18f59cOloSDoBs8mjQTfj0C/E/8ejGwEq3VuyU5w/5+"
            'vendorCodeString += "edftXwzowzzPOXDvAwSUlae4Ort6vMsXzCORhh2LvbPFgC+KO9iFcw71CQpLQCaSxW84Iiarmq5Az5ki"
            'vendorCodeString += "iE6Wbp9E+Qdrg0gZhELN8ioB7y/HYZE8eFX1E9dZfsOjBDC/B1kgeEmuhyA="

            '�����ַ���˳�򣬷�ֹ DLL й¶�˴���
            Dim _code1, _code2, _code3 As String
            _code3 = "QIoHKAn5RnxGE04eCzY5wkFk/52fJBsgacmciQkbA9EigxCtFklGUYGZk4Xbn+pEMQKRFs6BR74ygKj4"
            _code1 = "jE/H8cWLtSeVIyRQTtS/S1J9kgRZQ7WtI0/FetR4T8d+Wl36m3gGTVqcsGY/XRF6NGFA3p8LcG5jg5q/"
            _code2 = "xTfBF8EJQWn9aht0vxKPc2mTJe0aCHns09w/G9LbhVM0lxi9naqYy0rtAsKA01zvx65K6ZR6OQOgvqQK"
            _code1 += "TKegXKNEGa/atu0bL7WRFUgzay93paF4uPnX3/LRuwN3lplq42NfbNMoEDE9wNXgV6YvhN/l8vKVLA2G"
            _code3 += "QNZt6ixF2iXv96xxKC5y4GDZc4eAPlxPvw32+18f59cOloSDoBs8mjQTfj0C/E/8ejGwEq3VuyU5w/5+"
            _code1 += "3mdC5nQZ6pk86qyfdPPxBGSETrMiRBcX+wNC7kG3O2wxu6reLwFy0I6rU+uByXs1+Q83Tm2GMraKoqD/"
            _code2 += "vWvYes2AmydXEcrBxs7N+NlUmjOtjGJcQXftwlNost1sBszQY52fngbAHTTXX1mYI9ieyqtNZ9wcE+L4"
            _code1 += "b82FpU5X6aF/szr7bRv+CeOc8HyOVkUBA5TFkLn+hYyD8YZK2e6UBhhdPOxZPvOjrBlpGjhauBecTTWE"
            _code3 += "edftXwzowzzPOXDvAwSUlae4Ort6vMsXzCORhh2LvbPFgC+KO9iFcw71CQpLQCaSxW84Iiarmq5Az5ki"
            _code2 += "euH5wWQ6JcBaa8RaOtCuqimhJc7Okak5i9c+fGQZNNbt7sTrf3w0ZzUihXPqH+CnuUCQpJvDNtSwCF8i"
            _code2 += "3ti5KZ/3nga2INOiYGeUWkhZVkeXNGe01/UW3dpe7Nl1G/sHqYBkX1UNV9orEghGduZS6huUpxHQ1iyL"
            _code3 += "iE6Wbp9E+Qdrg0gZhELN8ioB7y/HYZE8eFX1E9dZfsOjBDC/B1kgeEmuhyA="

            status = hasp.Login(_code1 & _code2 & _code3)

            If status <> 0 Then
                hasp.Logout()       ' �˳�
                hasp.Dispose()
                AppDomain.Unload(dom)
                Throw New System.Exception("δ��⵽�������")
            End If

            ' 4����ȡ������������
            Dim file As Object
            file = hasp.GetFile(&HFFF0)
            If (Not file.IsLoggedIn()) Then
                hasp.Logout()
                hasp.Dispose()
                AppDomain.Unload(dom)
                Throw New System.Exception("�޷���������ж�ȡ��Ϣ��")
            End If

            ' 5����ȡ�������;
            Dim intPurpose As Integer
            file.FilePos = 0
            status = file.Read(intPurpose)

            If (0 <> status) Then
                hasp.Logout()
                hasp.Dispose()
                AppDomain.Unload(dom)
                Throw New System.Exception("�޷���������ж�ȡ��Ϣ��")
            End If
            Me._DogPurpose = intPurpose

            ' 6����ȡʧЧʱ��
            If Me._DogPurpose = Types.DogPurpose.Time Then
                Dim intTime As Integer
                file.FilePos = 4
                status = file.Read(intTime)

                If (0 <> status) Then
                    hasp.Logout()
                    hasp.Dispose()
                    AppDomain.Unload(dom)
                    Throw New System.Exception("�޷���������ж�ȡ��Ϣ��")
                End If
                Me._OverdueTimeUTC = Date.FromFileTimeUtc(intTime * 864000000000)
                If DateDiff(DateInterval.Day, Me._OverdueTimeUTC, Date.Today) > 0 Then
                    hasp.Logout()   ' �˳�
                    hasp.Dispose()
                    AppDomain.Unload(dom)
                    Throw New System.Exception("�����ʧЧ��ʧЧ���ڣ�" & Me._OverdueTimeUTC.ToShortDateString & "��")
                End If
            Else
                Me._OverdueTimeUTC = DateTime.Parse("2099-12-31")
            End If

            ' 7����ȡ License �ļ����������
            Dim intLicenseCrc32 As Integer
            If Me._DogPurpose = Types.DogPurpose.Normal Then      ' ��ͨ��Ʒ��
                file.FilePos = Me.Crc32ReadPos
            ElseIf Me._DogPurpose = Types.DogPurpose.Time Then   ' ��ʧЧʱ��Ĳ�Ʒ��
                file.FilePos = (16 + 28) - Me.Crc32ReadPos  ' ����ͨ��Ʒ����λ�øպ��෴
            End If
            status = file.Read(intLicenseCrc32)

            If (0 <> status) Then
                hasp.Logout()
                hasp.Dispose()
                AppDomain.Unload(dom)
                Throw New System.Exception("�޷���������ж�ȡ��Ϣ��")
            End If

            Me._LicenseCrc32 = intLicenseCrc32


            'Dim status As HaspStatus
            Dim haspinfo As String = Nothing
            status = hasp.GetSessionInfo("<haspformat format=""keyinfo""/>", haspinfo)

            If (0 <> status) Then
                hasp.Logout()
                hasp.Dispose()
                AppDomain.Unload(dom)
                Throw New System.Exception("�޷���������ж�ȡ��Ϣ��")
            End If

            Dim xmlDoc As System.Xml.XmlDocument
            xmlDoc = New System.Xml.XmlDocument
            Try
                xmlDoc.LoadXml(haspinfo)
            Catch ex As System.Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Throw New System.Exception("�޷���������ж�ȡ��Ϣ��")
            End Try

            Dim xmlN As System.Xml.XmlNode
            xmlN = xmlDoc.SelectSingleNode("/hasp_info/keyspec/hasp/haspid")
            If xmlN Is Nothing Then
                Throw New System.Exception("�޷���������ж�ȡ��Ϣ��")
            End If

            Me._DogId = xmlN.InnerText


            ' 5���˳�
            hasp.Logout()
            hasp.Dispose()
            AppDomain.Unload(dom)
        End Sub

    End Class

    ' ��װ Rainbow �������

    '�����ж�̬������ӿڵ���(RC)
    Friend Class Dog
        Public DogBytes As Integer = 0
        Public DogAddr As Integer = 1
        Public DogCascade As Integer = 0
        Public NewCascade As Integer = 0
        Public DogPassword As Integer = 352476381
        Public NewPassword As Integer = 0
        Public DogResult As Integer = 0
        Public CurrentNu As Integer = 0
        Public DogData(199) As Byte
        Public RetCode As Integer = 0

        Private Delegate Function dCheck_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByRef RetCode As Integer) As Integer

        Private Delegate Function dRead_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogAddr As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef RetCode As Integer) As Integer

        Private Delegate Function dWrite_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogAddr As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef RetCode As Integer) As Integer

        Private Delegate Function dConvert_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef DogResult As Integer, ByRef RetCode As Integer) As Integer

        Private Delegate Function dDisableShare_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByRef RetCode As Integer) As Integer

        Private Delegate Function dSetPassword_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal NewPassword As Integer, ByRef RetCode32 As Integer) As Integer

        Private Delegate Function dSetCascade_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal NewCascade As Integer, ByRef RetCode As Integer) As Integer

        Private Delegate Function dGetCurrentNo_Dog(ByVal DogCascade As Integer, ByRef CurrentNu As Integer, ByRef RetCode As Integer) As Integer

        Private entCheck_Dog As dCheck_Dog
        Private entRead_Dog As dRead_Dog
        Private entWrite_Dog As dWrite_Dog
        Private entConvert_Dog As dConvert_Dog
        Private entDisableShare_Dog As dDisableShare_Dog
        Private entSetPassword_Dog As dSetPassword_Dog
        Private entSetCascade_Dog As dSetCascade_Dog
        Private entGetCurrentNo_Dog As dGetCurrentNo_Dog


        Public Sub New()
            'IntPtr.Size = 4   32 λϵͳ��ʹ�� htbdog.dll
            If IntPtr.Size = 4 Then
                entCheck_Dog = New dCheck_Dog(AddressOf Check_Dog32)
                entRead_Dog = New dRead_Dog(AddressOf Read_Dog32)
                entWrite_Dog = New dWrite_Dog(AddressOf Write_Dog32)
                entConvert_Dog = New dConvert_Dog(AddressOf Convert_Dog32)
                entDisableShare_Dog = New dDisableShare_Dog(AddressOf DisableShare_Dog32)
                entSetPassword_Dog = New dSetPassword_Dog(AddressOf SetPassword_Dog32)
                entSetCascade_Dog = New dSetCascade_Dog(AddressOf SetCascade_Dog32)
                entGetCurrentNo_Dog = New dGetCurrentNo_Dog(AddressOf GetCurrentNo_Dog32)
            Else
                entCheck_Dog = New dCheck_Dog(AddressOf Check_Dog64)
                entRead_Dog = New dRead_Dog(AddressOf Read_Dog64)
                entWrite_Dog = New dWrite_Dog(AddressOf Write_Dog64)
                entConvert_Dog = New dConvert_Dog(AddressOf Convert_Dog64)
                entDisableShare_Dog = New dDisableShare_Dog(AddressOf DisableShare_Dog64)
                entSetPassword_Dog = New dSetPassword_Dog(AddressOf SetPassword_Dog64)
                entSetCascade_Dog = New dSetCascade_Dog(AddressOf SetCascade_Dog64)
                entGetCurrentNo_Dog = New dGetCurrentNo_Dog(AddressOf GetCurrentNo_Dog64)
            End If
        End Sub
#Region "Ӳ��������������32λϵͳ��"

        Declare Function Check_Dog32 Lib "htbdog.dll" Alias "Check_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByRef RetCode As Integer) As Integer

        Declare Function Read_Dog32 Lib "htbdog.dll" Alias "Read_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogAddr As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef RetCode As Integer) As Integer

        Declare Function Write_Dog32 Lib "htbdog.dll" Alias "Write_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogAddr As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef RetCode As Integer) As Integer

        Declare Function Convert_Dog32 Lib "htbdog.dll" Alias "Convert_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef DogResult As Integer, ByRef RetCode As Integer) As Integer

        Declare Function DisableShare_Dog32 Lib "htbdog.dll" Alias "DisableShare_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByRef RetCode As Integer) As Integer

        Declare Function SetPassword_Dog32 Lib "htbdog.dll" Alias "SetPassword_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal NewPassword As Integer, ByRef RetCode32 As Integer) As Integer

        Declare Function SetCascade_Dog32 Lib "htbdog.dll" Alias "SetCascade_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal NewCascade As Integer, ByRef RetCode As Integer) As Integer

        Declare Function GetCurrentNo_Dog32 Lib "htbdog.dll" Alias "GetCurrentNo_Dog" (ByVal DogCascade As Integer, ByRef CurrentNu As Integer, ByRef RetCode As Integer) As Integer
#End Region

#Region "Ӳ��������������64λϵͳ��"
        Declare Function Check_Dog64 Lib "htbdog64.dll" Alias "Check_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByRef RetCode As Integer) As Integer

        Declare Function Read_Dog64 Lib "htbdog64.dll" Alias "Read_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogAddr As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef RetCode As Integer) As Integer

        Declare Function Write_Dog64 Lib "htbdog64.dll" Alias "Write_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogAddr As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef RetCode As Integer) As Integer

        Declare Function Convert_Dog64 Lib "htbdog64.dll" Alias "Convert_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef DogResult As Integer, ByRef RetCode As Integer) As Integer

        Declare Function DisableShare_Dog64 Lib "htbdog64.dll" Alias "DisableShare_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByRef RetCode As Integer) As Integer

        Declare Function SetPassword_Dog64 Lib "htbdog64.dll" Alias "SetPassword_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal NewPassword As Integer, ByRef RetCode64 As Integer) As Integer

        Declare Function SetCascade_Dog64 Lib "htbdog64.dll" Alias "SetCascade_Dog" (ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal NewCascade As Integer, ByRef RetCode As Integer) As Integer

        Declare Function GetCurrentNo_Dog64 Lib "htbdog64.dll" Alias "GetCurrentNo_Dog" (ByVal DogCascade As Integer, ByRef CurrentNu As Integer, ByRef RetCode As Integer) As Integer
#End Region


        Public Function Check_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByRef RetCode As Integer) As Integer
            Return entCheck_Dog.Invoke(DogCascade, DogPassword, RetCode)
        End Function

        Public Function Read_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogAddr As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef RetCode As Integer) As Integer
            Return entRead_Dog(DogCascade, DogPassword, DogAddr, DogBytes, DogData, RetCode)
        End Function


        Public Function Write_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogAddr As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef RetCode As Integer) As Integer
            Return entWrite_Dog(DogCascade, DogPassword, DogAddr, DogBytes, DogData, RetCode)
        End Function

        Public Function Convert_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal DogBytes As Integer, ByVal DogData() As Byte, ByRef DogResult As Integer, ByRef RetCode As Integer) As Integer
            Return entConvert_Dog(DogCascade, DogPassword, DogBytes, DogData, DogResult, RetCode)
        End Function

        Public Function DisableShare_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByRef RetCode As Integer) As Integer
            Return entDisableShare_Dog(DogCascade, DogPassword, RetCode)
        End Function

        Public Function SetPassword_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal NewPassword As Integer, ByRef RetCode As Integer) As Integer
            Return entSetPassword_Dog(DogCascade, DogPassword, NewPassword, RetCode)
        End Function

        Public Function SetCascade_Dog(ByVal DogCascade As Integer, ByVal DogPassword As Integer, ByVal NewCascade As Integer, ByRef RetCode As Integer) As Integer
            Return entSetCascade_Dog(DogCascade, DogPassword, NewCascade, RetCode)
        End Function

        Public Function GetCurrentNo_Dog(ByVal DogCascade As Integer, ByRef CurrentNu As Integer, ByRef RetCode As Integer) As Integer
            Return entGetCurrentNo_Dog(DogCascade, CurrentNu, RetCode)
        End Function


    End Class

    Friend Class RainbowDog
        Implements ISoftDog

        Private _MyDog As Dog
        Private _IsZydog As Boolean
        Private _DogPurpose As Types.DogPurpose
        Private _OverdueTimeUTC As DateTime
        Private _Owner As String
        Private _Counter As Integer
        Private _Crc32ReadPos As Integer

        Public Sub New()
            If IntPtr.Size < 8 Then
                ' 32 λϵͳ������У�� htbdog.dll
                If GeneralBase.Crc32IntegerFile(HttpContext.Current.Server.MapPath("/bin/htbdog.dll")) <> &HCFC2A024 Then
                    Throw New System.Exception("htbdog.dll �汾��Ϣ����")
                End If
            Else
                ' 64 λϵͳ������У�� htbdog64.dll
                If GeneralBase.Crc32IntegerFile(HttpContext.Current.Server.MapPath("/bin/htbdog64.dll")) <> &H1B6AAD93 Then
                    Throw New System.Exception("htbdog64.dll �汾��Ϣ����")
                End If
            End If

            ' ʵ������������󣬼�������
            Me._MyDog = New Dog
            Me.LoadDog()

            ' ��ֵ��ʼ����
            Me._IsZydog = getIsZydog()
            If Me._IsZydog Then
                Me._DogPurpose = Types.DogPurpose.ZY

            Else
                Me._DogPurpose = Types.DogPurpose.Normal

            End If
            Me._OverdueTimeUTC = DateTime.Parse("2099-12-31")
            Me._Counter = Me._MyDog.DogData(113)
            If Me.IsZydog And Me.Counter = 0 Then
                Throw New Exception("�������Ҫ��ֵ")

            End If
            Me._Owner = getOwner()
            Me._Crc32ReadPos = 0        ' ���ù�����¼ License �ļ�������

        End Sub

        ' ================ ����
        ReadOnly Property IsZydog() As Boolean Implements ISoftDog.IsZydog
            Get
                Return Me._IsZydog
            End Get
        End Property
        ReadOnly Property DogPurpose() As Types.DogPurpose Implements ISoftDog.DogPurpose
            Get
                Return Me._DogPurpose
            End Get
        End Property
        ReadOnly Property OverdueTimeUTC() As DateTime Implements ISoftDog.OverdueTimeUTC
            Get
                Return Me._OverdueTimeUTC
            End Get
        End Property
        ReadOnly Property Owner() As String Implements ISoftDog.Owner
            Get
                Return Me._Owner
            End Get
        End Property
        ReadOnly Property Counter() As Integer Implements ISoftDog.Counter
            Get
                Return Me._Counter
            End Get
        End Property
        Property Crc32ReadPos() As Integer Implements ISoftDog.Crc32ReadPos
            Get
                Return Me._Crc32ReadPos
            End Get
            Set(ByVal Value As Integer)
                Me._Crc32ReadPos = Value
            End Set
        End Property
        ReadOnly Property LicenseCrc32() As Integer Implements ISoftDog.LicenseCrc32
            Get
                Return _Crc32ReadPos
            End Get
        End Property
        ReadOnly Property DogId() As String Implements ISoftDog.DogId
            Get
                Return ""
            End Get
        End Property

        ' ================ ����
        Sub MinusCounter() Implements ISoftDog.MinusCounter
            Me._MyDog.DogAddr = 113
            Me._MyDog.DogBytes = 1
            Me._MyDog.DogData(0) = Me.Counter - 1
            Me._MyDog.Write_Dog(Me._MyDog.DogCascade, Me._MyDog.DogPassword, Me._MyDog.DogAddr, Me._MyDog.DogBytes, Me._MyDog.DogData, Me._MyDog.RetCode)
        End Sub

        ' ================ ���к���
        ' ���ܣ�У��������Ƿ���ȷ��װ
        Private Sub LoadDog()
            Me._MyDog.DogAddr = 0
            Me._MyDog.DogBytes = 123        '��鹷ʱֱ�Ӱ���Ҫ������ȫ��ȡ��
            Me._MyDog.Read_Dog(Me._MyDog.DogCascade, Me._MyDog.DogPassword, Me._MyDog.DogAddr, Me._MyDog.DogBytes, Me._MyDog.DogData, Me._MyDog.RetCode)
            If Me._MyDog.RetCode <> 0 Then
                Me._MyDog.DogPassword = 0
                Me._MyDog.Read_Dog(Me._MyDog.DogCascade, Me._MyDog.DogPassword, Me._MyDog.DogAddr, Me._MyDog.DogBytes, Me._MyDog.DogData, Me._MyDog.RetCode)
                If Me._MyDog.RetCode <> 0 Then
                    Throw New System.Exception("δ��⵽�����")
                End If
            End If
        End Sub

        ' ���ܣ����ص�ǰ�Ƿ����ù�
        Private Function getIsZydog() As Boolean
            If InStr(LCase(ReadDog(0, 79)), "dotnet") > 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        ' ���ܣ���ȡ��ǰ������û���Ϣ
        Private Function getOwner() As String
            Return ReadDog(115, 8)
        End Function

        ' ���ܣ����ݲ�����ȡ�������Ϣ
        Private Function ReadDog(ByVal iDogAddr As Integer, ByVal iDogBytes As Integer) As String
            Dim i As Integer
            Dim sb As New System.Text.StringBuilder
            For i = iDogAddr To iDogAddr + iDogBytes - 1
                sb.Append(IIf(Me._MyDog.DogData(i) = 0, "", ChrW(Me._MyDog.DogData(i))))
            Next
            Return sb.ToString
        End Function

    End Class

End Namespace
