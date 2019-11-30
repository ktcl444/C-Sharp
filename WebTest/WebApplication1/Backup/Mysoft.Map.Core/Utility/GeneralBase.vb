Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Web
Imports System.CodeDom.Compiler
Imports Microsoft.VisualBasic
Imports System.Reflection
Imports System.Data.SqlClient
Imports Mysoft.Map.Data
Imports Mysoft.Map.Application.Security
Imports Mysoft.Map.Application.Types
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Configuration
Imports System.Runtime.Serialization.Formatters.Binary

Namespace Utility

    Public Class GeneralBase

        ' ���ܣ�����һ������Ĵ���
        ' ���أ�����Ϊ 16 �ֽڵ��ַ���
        Public Shared Function GetRandomCode16() As String
            Return Right(Guid.NewGuid.ToString.Replace("-", ""), 16)
        End Function

        ' ���ܣ�ͨ��������ó����еĺ���
        '������assembly     -- �������ƣ��� Mysoft.Map.Core
        '      type         -- �����ƣ��� Mysoft.Map.Utility.General
        '      name         -- ��������
        '      params       -- ��������
        '���أ������ú������صĶ����������ʧ�ܣ����� Nothing
        Public Shared Function Invoke(ByVal [assembly] As String, ByVal [class] As String, ByVal [function] As String, ByVal params() As Object) As Object
            Dim assemblyObj As System.Reflection.Assembly
            Dim typeObj As Type

            assemblyObj = System.Reflection.Assembly.Load([assembly])
            typeObj = assemblyObj.GetType([class], True, True)

            '��ȡ��ķ�������
            Dim methods() As System.Reflection.MethodInfo = typeObj.GetMethods()
            Dim m As System.Reflection.MethodInfo
            Dim i As Integer

            Dim objParams() As Object
            If Not params Is Nothing Then
                If params.Length > 0 Then
                    ReDim objParams(params.Length - 1)
                End If
            End If

            For Each m In methods
                '��ȡ�����Ĳ�������
                Dim p() As System.Reflection.ParameterInfo = m.GetParameters()

                '������������������ͬ
                If m.Name = [function] And _
                    ((params Is Nothing And p.Length = 0) OrElse _
                    (Not params Is Nothing AndAlso p.Length = params.Length)) Then

                    Dim bError As Boolean = False

                    If Not params Is Nothing Then
                        For i = 0 To params.Length - 1
                            Try
                                'ÿ��������ת��Ϊ������������
                                objParams(i) = Convert.ChangeType(params(i), p(i).ParameterType)
                            Catch e As System.Exception
                            	Mysoft.Map.Data.MyDB.LogException(e)
                                bError = True
                                Exit For
                            End Try
                        Next
                    End If

                    '�������ת���ɹ�
                    If Not bError Then
                        Dim instance As Object = Nothing
                        If Not m.IsStatic Then
                            instance = Activator.CreateInstance(typeObj)
                        End If

                        '���ú����������غ��������صĶ���
                        Return m.Invoke(instance, objParams)
                    End If
                End If
            Next
        End Function

        ' ���ܣ��� Decimal ���͵���ֵ��ʽ���ذ������ַ����ڵ�����
        ' ������value    -- �����ַ���
        ' ���أ��ַ������ԺϷ�����һ�����֣����򷵻�0
        Public Shared Function StringToDecimal(ByVal value As String) As Decimal
            Dim dec As Decimal

            value = Replace(value, ",", "")
            If IsNumeric(value) = True Then         '�ж��Ƿ�Ϊ�Ϸ��������ַ�
                dec = Convert.ToDecimal(Val(value))
            Else
                dec = 0
            End If

            Return dec

        End Function

        ' ���ܣ��������ַ���ת������������
        ' ������value    -- �����ַ���
        ' ���أ��ַ������ԺϷ�����һ����ʽΪyyyy-MM-dd����Ч���ڣ����򷵻�����1900-01-01
        Public Shared Function StringToDateTime(ByVal value As String) As DateTime
            Dim dtm As DateTime

            If IsDate(value) = True Then         '�ж��Ƿ�Ϊ�Ϸ��������ַ�
                dtm = Convert.ToDateTime(value)
            Else
                dtm = Convert.ToDateTime("1900-01-01")
            End If

            Return dtm

        End Function

        ' ���ܣ�����������ת���������ַ���
        ' ������value        -- ����
        '       withTime     -- �ַ����Ƿ����ʱ��
        ' ���أ����������1900-01-01���ؿմ������򷵻ظ�ʽ��yyy-MM-dd��yyy-MM-dd hh:mm:ss���ַ���
        Public Shared Function DateTimeToString(ByVal value As DateTime) As String
            Return DateTimeToString(value, False)
        End Function

        Public Shared Function DateTimeToString(ByVal value As DateTime, ByVal withTime As Boolean) As String
            If value = Nothing Then Return ""

            If value = Convert.ToDateTime("1900-01-01") Then
                Return ""
            Else

                Return IIf(withTime, Format(value, "yyyy-MM-dd hh:mm:ss"), Format(value, "yyyy-MM-dd"))
            End If

        End Function

        ' ���ܣ��������루֧��С�����5λ��
        ' ������acc     -- ����С��λ��
        ' ˵����Ŀǰ����ȡ����ֵ����������ķ�ʽ����
        Public Shared Function Round(ByVal value As Decimal, ByVal acc As Integer) As Decimal
            Dim Dec As Decimal
            Dim AbsValue As Decimal

            AbsValue = Math.Abs(value)      ' ȡ����ֵ

            Select Case acc
                Case 0
                    Dec = Int(AbsValue + 0.5)
                Case 1
                    Dec = Int(AbsValue * 10 + 0.5) / 10
                Case 2
                    Dec = Int(AbsValue * 100 + 0.5) / 100
                Case 3
                    Dec = Int(AbsValue * 1000 + 0.5) / 1000
                Case 4
                    Dec = Int(AbsValue * 10000 + 0.5) / 10000
                Case Else
                    Dec = Int(AbsValue * 100000 + 0.5) / 100000
            End Select

            If value >= 0 Then
                Return Dec
            Else
                Return -Dec
            End If

        End Function

        ' ���ܣ����λ״̬
        Public Shared Function CheckBit(ByVal val As Integer, ByVal bit As Integer) As Boolean
            Return (bit And val) = bit
        End Function

        '���ܣ������ļ���CRC32ֵ
        '������strFilename -- ��ҪУ�������ļ���
        '���أ�У��ֵ��ʮ�����ƴ���
        Public Shared Function Crc32File(ByVal strFilename As String) As String
            Dim Crc32Table(255) As Integer
            Dim iCrc32Value As Integer

            Dim iBytes As Integer, iBits As Integer
            Dim lCrc32 As Integer
            Dim lTempCrc32 As Integer
            Dim sReturn As String
            Dim i As Integer

            If Not IO.File.Exists(strFilename) Then
                Return "00000000"
            End If

            '// Iterate 256 times
            For iBytes = 0 To 255
                '// Initiate lCrc32 to counter variable
                lCrc32 = iBytes

                '// Now iterate through each bit in counter byte
                For iBits = 0 To 7
                    '// Right shift unsigned long 1 bit
                    lTempCrc32 = lCrc32 And &HFFFFFFFE
                    lTempCrc32 = lTempCrc32 \ &H2
                    lTempCrc32 = lTempCrc32 And &H7FFFFFFF

                    '// Now check if temporary is less than zero and then 
                    'mix Crc32 checksum with Seed value
                    If (lCrc32 And &H1) <> 0 Then
                        lCrc32 = lTempCrc32 Xor &HEDB88320
                    Else
                        lCrc32 = lTempCrc32
                    End If
                Next

                '// Put Crc32 checksum value in the holding array
                Crc32Table(iBytes) = lCrc32
            Next

            iCrc32Value = &HFFFFFFFF

            '
            '// Declare following variables
            'Dim bCharValue As Byte, iCounter As Integer, lIndex As Long
            Dim bCharValue As Byte, iCounter As Integer, lIndex As Integer
            Dim lAccValue As Integer, lTableValue As Integer

            Dim fs As New System.IO.FileStream(strFilename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)

            '// Iterate through the string that is to be checksum-computed
            For iCounter = 0 To fs.Length - 1

                '// Get ASCII value for the current character
                'bCharValue = Asc(Mid$(Item, iCounter, 1))
                bCharValue = fs.ReadByte()

                '// Right shift an Unsigned Long 8 bits
                lAccValue = iCrc32Value And &HFFFFFF00
                lAccValue = lAccValue \ &H100
                lAccValue = lAccValue And &HFFFFFF

                '// Now select the right adding value from the 
                'holding table
                lIndex = iCrc32Value And &HFF
                lIndex = lIndex Xor bCharValue
                lTableValue = Crc32Table(lIndex)

                '// Then mix new Crc32 value with previous 
                'accumulated Crc32 value
                iCrc32Value = lAccValue Xor lTableValue
            Next

            fs.Close()

            iCrc32Value = iCrc32Value Xor &HFFFFFFFF

            sReturn = Hex(iCrc32Value)
            '��λ
            For i = 0 To 7 - sReturn.Length
                sReturn = "0" & sReturn
            Next

            Return sReturn
        End Function

        '���ܣ������ļ���CRC32ֵ
        '������strFilename -- ��ҪУ�������ļ���
        '���أ�����
        Public Shared Function Crc32IntegerFile(ByVal strFilename As String) As Integer
            Dim Crc32Table(255) As Integer
            Dim iCrc32Value As Integer

            Dim iBytes As Integer, iBits As Integer
            Dim lCrc32 As Integer
            Dim lTempCrc32 As Integer

            If Not IO.File.Exists(strFilename) Then
                Return 0
            End If

            '// Iterate 256 times
            For iBytes = 0 To 255
                '// Initiate lCrc32 to counter variable
                lCrc32 = iBytes

                '// Now iterate through each bit in counter byte
                For iBits = 0 To 7
                    '// Right shift unsigned long 1 bit
                    lTempCrc32 = lCrc32 And &HFFFFFFFE
                    lTempCrc32 = lTempCrc32 \ &H2
                    lTempCrc32 = lTempCrc32 And &H7FFFFFFF

                    '// Now check if temporary is less than zero and then 
                    'mix Crc32 checksum with Seed value
                    If (lCrc32 And &H1) <> 0 Then
                        lCrc32 = lTempCrc32 Xor &HEDB88320
                    Else
                        lCrc32 = lTempCrc32
                    End If
                Next

                '// Put Crc32 checksum value in the holding array
                Crc32Table(iBytes) = lCrc32
            Next

            iCrc32Value = &HFFFFFFFF

            '
            '// Declare following variables
            'Dim bCharValue As Byte, iCounter As Integer, lIndex As Long
            Dim bCharValue As Byte, iCounter As Integer, lIndex As Integer
            Dim lAccValue As Integer, lTableValue As Integer

            Dim fs As New System.IO.FileStream(strFilename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)

            '// Iterate through the string that is to be checksum-computed
            For iCounter = 0 To fs.Length - 1

                '// Get ASCII value for the current character
                'bCharValue = Asc(Mid$(Item, iCounter, 1))
                bCharValue = fs.ReadByte()

                '// Right shift an Unsigned Long 8 bits
                lAccValue = iCrc32Value And &HFFFFFF00
                lAccValue = lAccValue \ &H100
                lAccValue = lAccValue And &HFFFFFF

                '// Now select the right adding value from the 
                'holding table
                lIndex = iCrc32Value And &HFF
                lIndex = lIndex Xor bCharValue
                lTableValue = Crc32Table(lIndex)

                '// Then mix new Crc32 value with previous 
                'accumulated Crc32 value
                iCrc32Value = lAccValue Xor lTableValue
            Next

            fs.Close()

            iCrc32Value = iCrc32Value Xor &HFFFFFFFF

            'sReturn = Hex(iCrc32Value)
            ''��λ
            'For i = 0 To 7 - sReturn.Length
            '    sReturn = "0" & sReturn
            'Next

            Return iCrc32Value
        End Function

        '������ʽ������ JS �е� Eval ����
        Public Shared Function Eval(ByVal expression As String) As Object
            Dim comp As CodeDomProvider = New VBCodeProvider
            Dim cp As CompilerParameters = New CompilerParameters
            Dim mi As MethodInfo

            Dim codeBuilder As StringBuilder = New StringBuilder
            codeBuilder.Append("Imports System" & vbCrLf)
            codeBuilder.Append("Imports System.Math" & vbCrLf)
            codeBuilder.Append("Imports Microsoft.VisualBasic" & vbCrLf)
            codeBuilder.Append("" & vbCrLf)
            codeBuilder.Append("Public Module Mode" & vbCrLf)
            codeBuilder.Append("   Public Function Func() As Object" & vbCrLf)
            codeBuilder.Append("        Return " & expression & vbCrLf)
            codeBuilder.Append("   End Function" & vbCrLf)
            codeBuilder.Append("End Module" & vbCrLf)

            cp.ReferencedAssemblies.Add("System.dll")
            cp.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll")
            cp.GenerateExecutable = False
            cp.GenerateInMemory = True

            Dim code As String = codeBuilder.ToString
            Dim cr As CompilerResults = comp.CreateCompiler.CompileAssemblyFromSource(cp, code)

            If cr.Errors.HasErrors Then
                Return Nothing
            Else
                Dim a As [Assembly] = cr.CompiledAssembly
                Dim t As Type = a.GetType("Mode")
                Dim CS(0) As Object
                mi = t.GetMethod("Func", BindingFlags.Static Or BindingFlags.Public)
                Return mi.Invoke("", CS(0))
            End If
        End Function


        Public Shared Function EscapeUnicode(ByVal s As String) As String
            Dim sb As New StringBuilder()
            Dim ba As Byte() = System.Text.Encoding.Unicode.GetBytes(s)
            For i As Integer = 0 To ba.Length - 1 Step 2
                '
                '''/ BE SURE 2's
                sb.Append("%")
                If ba(i + 1) <> 0 Then
                    sb.Append("u" & ba(i + 1).ToString("X2"))
                End If
                sb.Append(ba(i).ToString("X2"))
            Next
            Return sb.ToString()

        End Function

        ''' <summary>
        ''' ��鵱ǰ����ϵͳ��32λ����64λ
        ''' </summary>
        ''' <returns>"32"��"64"��"UNKNOWN"</returns>
        ''' <remarks>
        ''' 1��Ϊ�˱���asp.netȨ�޲���������û��ʹ�� System.Management �� WMI�ķ������а汾�жϡ�
        ''' 2��ʹ��System.IntPtr.Sizeֻ���жϵ�ǰDLL������32����64λ�������÷�����һ��ȱ�㣺������"32"ʱ���п�������64λ����ϵͳ����32λ����ģʽ���С�
        ''' ������ERP����AnyCpu��ʽ���룬����ERP��˵����64λ����ϵͳ��һ������64λ��ʽ���еģ����Կ��Ժ��Դ����⡣
        ''' </remarks>
        Public Shared Function Detect3264()
            If (System.IntPtr.Size = 4) Then
                Return "32"
            ElseIf (System.IntPtr.Size = 8) Then
                Return "64"
            Else
                Return "UNKNOWN"
            End If
        End Function

        'ͨ��������url����ȡdomain����
        Public Shared Function GetDomain(ByVal strHtmlPagePath As String) As String
            Dim p As String = "http://[^\.]*\.(?<domain>[^/]*)"
            Dim reg As New Regex(p, RegexOptions.IgnoreCase)
            Dim m As Match = reg.Match(strHtmlPagePath)
            Dim returnValue As String = String.Empty
            returnValue = m.Groups("domain").Value
            If (returnValue.IndexOf(":") > -1) Then
                returnValue = returnValue.Substring(0, returnValue.IndexOf(":"))
            End If
            Return returnValue
        End Function

        ' ���ܣ�����ʱ�������в���ؼ��ֵ�ֵ
        ' ������a_UserGUID      -- �û�GUID
        '       a_KeywordName   -- �ؼ�������
        '       a_KeywordValue  -- �ؼ��ֵ�ֵ
        ' ���أ�0���ɹ���-1��ʧ��
        Public Shared Function InsertKeywordValue2myTemp(ByVal a_UserGUID As String, ByVal a_KeywordName As String, ByVal a_KeywordValue As String) As Integer
            Dim dtTemp As DataTable
            Dim strSQL As String
            Dim intReturn As Integer

            '��ѯ�ؼ���
            strSQL = "SELECT TempGUID, TempID, TempType, TempValue, CreateOn FROM myTemp" & _
                     " WHERE TempID = '" & a_UserGUID.Replace("'", "''") & "'" & _
                     " AND TempType = '�ؼ���'"
            dtTemp = MyDB.GetDataTable(strSQL)

            Dim xmlDoc As New XmlDocument
            Dim xmlN As XmlNode
            Dim xmlAttr As XmlAttribute
            Dim i As Integer

            If dtTemp.Rows.Count = 0 Then
                '�޼�¼��Ҫ�����¼�¼
                xmlDoc.LoadXml("<keywords/>")

                xmlN = xmlDoc.CreateElement("keyword")

                xmlAttr = xmlDoc.CreateAttribute("keyname")
                xmlAttr.Value = a_KeywordName
                xmlN.Attributes.Append(xmlAttr)

                xmlAttr = xmlDoc.CreateAttribute("keyvalue")
                xmlAttr.Value = a_KeywordValue
                xmlN.Attributes.Append(xmlAttr)

                xmlDoc.DocumentElement.AppendChild(xmlN)

                strSQL = "INSERT INTO myTemp (" & _
                         "      TempGUID," & _
                         "      TempID," & _
                         "      TempType," & _
                         "      TempValue," & _
                         "      CreateOn)" & _
                         " VALUES (" & _
                         "      NEWID()," & _
                         "      '" & a_UserGUID.Replace("'", "''") & "'," & _
                         "      '�ؼ���'," & _
                         "      '" & xmlDoc.InnerXml.Replace("'", "''") & "'," & _
                         "      GETDATE())"
                intReturn = MyDB.ExecSQL(strSQL)

                If intReturn = 0 Then Return -1
            Else
                '�м�¼����Ҫ����ؼ���
                Try
                    xmlDoc.LoadXml(dtTemp.Rows(0).Item("TempValue").ToString)

                    If xmlDoc.DocumentElement.Name <> "keywords" Then
                        xmlDoc.LoadXml("<keywords/>")
                    End If
                Catch ex As Exception
                	Mysoft.Map.Data.MyDB.LogException(ex)
                    xmlDoc.LoadXml("<keywords/>")
                End Try

                '�����Ƿ��Ѵ���Ҫ���õĹؼ���
                For i = 0 To xmlDoc.DocumentElement.ChildNodes.Count - 1
                    If xmlDoc.DocumentElement.ChildNodes(i).Name = "keyword" AndAlso _
                        xmlDoc.DocumentElement.ChildNodes(i).Attributes.GetNamedItem("keyname").Value.ToLower = a_KeywordName.ToLower Then
                        xmlN = xmlDoc.DocumentElement.ChildNodes(i)
                        Exit For
                    End If
                Next

                If xmlN Is Nothing Then
                    xmlN = xmlDoc.CreateElement("keyword")

                    xmlAttr = xmlDoc.CreateAttribute("keyname")
                    xmlAttr.Value = a_KeywordName
                    xmlN.Attributes.Append(xmlAttr)

                    xmlAttr = xmlDoc.CreateAttribute("keyvalue")
                    xmlAttr.Value = a_KeywordValue
                    xmlN.Attributes.Append(xmlAttr)

                    xmlDoc.DocumentElement.AppendChild(xmlN)
                Else
                    xmlAttr = xmlDoc.CreateAttribute("keyvalue")
                    xmlAttr.Value = a_KeywordValue
                    xmlN.Attributes.SetNamedItem(xmlAttr)
                End If

                '����
                strSQL = "UPDATE myTemp SET " & _
                         "TempValue = '" & xmlDoc.InnerXml.Replace("'", "''") & "'," & _
                         "CreateOn = GETDATE() " & _
                         "WHERE TempGUID = '" & dtTemp.Rows(0).Item("TempGUID").ToString & "'"
                intReturn = MyDB.ExecSQL(strSQL)

                If intReturn = 0 Then Return -1
            End If

            Return 0
        End Function

        '���л����ļ�
        Public Shared Function Serialize(Of T)(ByVal obj As T, ByVal path As String) As Boolean
            Dim fs As FileStream
            Dim bf As BinaryFormatter

            Try
                fs = New FileStream(path, FileMode.Create, FileAccess.Write)
                bf = New BinaryFormatter()
                bf.Serialize(fs, obj)

                Return True
            Catch ex As Exception
                Mysoft.Map.Data.MyDB.LogException(ex)
                Return False
            Finally
                fs.Close()
            End Try
        End Function

        '���ļ������л�
        Public Shared Function Deserialize(Of T)(ByVal path As String) As T
            Dim fs As FileStream
            Dim bf As BinaryFormatter
            Dim obj As T

            Try
                fs = New FileStream(path, FileMode.Open, FileAccess.Read)
                bf = New BinaryFormatter
                obj = CType(bf.Deserialize(fs), T)

                Return obj
            Catch ex As Exception
                Mysoft.Map.Data.MyDB.LogException(ex)
                Return Nothing
            Finally
                fs.Close()
            End Try
        End Function

    End Class

End Namespace
