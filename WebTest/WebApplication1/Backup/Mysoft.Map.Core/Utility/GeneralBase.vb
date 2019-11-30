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

        ' 功能：返回一个随机的代码
        ' 返回：长度为 16 字节的字符串
        Public Shared Function GetRandomCode16() As String
            Return Right(Guid.NewGuid.ToString.Replace("-", ""), 16)
        End Function

        ' 功能：通过反射调用程序集中的函数
        '参数：assembly     -- 程序集名称，如 Mysoft.Map.Core
        '      type         -- 类名称，如 Mysoft.Map.Utility.General
        '      name         -- 函数名称
        '      params       -- 参数数组
        '返回：所调用函数返回的对象，如果调用失败，返回 Nothing
        Public Shared Function Invoke(ByVal [assembly] As String, ByVal [class] As String, ByVal [function] As String, ByVal params() As Object) As Object
            Dim assemblyObj As System.Reflection.Assembly
            Dim typeObj As Type

            assemblyObj = System.Reflection.Assembly.Load([assembly])
            typeObj = assemblyObj.GetType([class], True, True)

            '获取类的方法集合
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
                '获取函数的参数集合
                Dim p() As System.Reflection.ParameterInfo = m.GetParameters()

                '函数名及参数个数相同
                If m.Name = [function] And _
                    ((params Is Nothing And p.Length = 0) OrElse _
                    (Not params Is Nothing AndAlso p.Length = params.Length)) Then

                    Dim bError As Boolean = False

                    If Not params Is Nothing Then
                        For i = 0 To params.Length - 1
                            Try
                                '每个参数都转换为函数所需类型
                                objParams(i) = Convert.ChangeType(params(i), p(i).ParameterType)
                            Catch e As System.Exception
                            	Mysoft.Map.Data.MyDB.LogException(e)
                                bError = True
                                Exit For
                            End Try
                        Next
                    End If

                    '如果参数转换成功
                    If Not bError Then
                        Dim instance As Object = Nothing
                        If Not m.IsStatic Then
                            instance = Activator.CreateInstance(typeObj)
                        End If

                        '调用函数，并返回函数所返回的对象
                        Return m.Invoke(instance, objParams)
                    End If
                End If
            Next
        End Function

        ' 功能：以 Decimal 类型的数值形式返回包含于字符串内的数字
        ' 参数：value    -- 数字字符串
        ' 返回：字符数字性合法返回一个数字，否则返回0
        Public Shared Function StringToDecimal(ByVal value As String) As Decimal
            Dim dec As Decimal

            value = Replace(value, ",", "")
            If IsNumeric(value) = True Then         '判断是否为合法数字性字符
                dec = Convert.ToDecimal(Val(value))
            Else
                dec = 0
            End If

            Return dec

        End Function

        ' 功能：将日期字符串转换成日期类型
        ' 参数：value    -- 日期字符串
        ' 返回：字符日期性合法返回一个格式为yyyy-MM-dd的有效日期，否则返回日期1900-01-01
        Public Shared Function StringToDateTime(ByVal value As String) As DateTime
            Dim dtm As DateTime

            If IsDate(value) = True Then         '判断是否为合法数字性字符
                dtm = Convert.ToDateTime(value)
            Else
                dtm = Convert.ToDateTime("1900-01-01")
            End If

            Return dtm

        End Function

        ' 功能：将日期类型转换成日期字符串
        ' 参数：value        -- 日期
        '       withTime     -- 字符串是否包含时间
        ' 返回：如果日期是1900-01-01返回空串，否则返回格式如yyy-MM-dd或yyy-MM-dd hh:mm:ss的字符串
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

        ' 功能：四舍五入（支持小数点后5位）
        ' 参数：acc     -- 保留小数位数
        ' 说明：目前按照取绝对值后四舍五入的方式处理
        Public Shared Function Round(ByVal value As Decimal, ByVal acc As Integer) As Decimal
            Dim Dec As Decimal
            Dim AbsValue As Decimal

            AbsValue = Math.Abs(value)      ' 取绝对值

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

        ' 功能：检查位状态
        Public Shared Function CheckBit(ByVal val As Integer, ByVal bit As Integer) As Boolean
            Return (bit And val) = bit
        End Function

        '功能：计算文件的CRC32值
        '参数：strFilename -- 需要校验计算的文件名
        '返回：校验值的十六进制代码
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
            '补位
            For i = 0 To 7 - sReturn.Length
                sReturn = "0" & sReturn
            Next

            Return sReturn
        End Function

        '功能：计算文件的CRC32值
        '参数：strFilename -- 需要校验计算的文件名
        '返回：整型
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
            ''补位
            'For i = 0 To 7 - sReturn.Length
            '    sReturn = "0" & sReturn
            'Next

            Return iCrc32Value
        End Function

        '计算表达式，类似 JS 中的 Eval 函数
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
        ''' 检查当前操作系统是32位还是64位
        ''' </summary>
        ''' <returns>"32"、"64"、"UNKNOWN"</returns>
        ''' <remarks>
        ''' 1、为了避免asp.net权限不够，所以没有使用 System.Management 和 WMI的方法进行版本判断。
        ''' 2、使用System.IntPtr.Size只能判断当前DLL运行在32还是64位环境。该方法有一个缺点：当返回"32"时，有可能是在64位操作系统中以32位兼容模式运行。
        ''' 但由于ERP是以AnyCpu方式编译，对于ERP来说，在64位操作系统中一定是以64位方式运行的，所以可以忽略此问题。
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

        '通过完整的url，截取domain名称
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

        ' 功能：往临时变量表中插入关键字的值
        ' 参数：a_UserGUID      -- 用户GUID
        '       a_KeywordName   -- 关键字名称
        '       a_KeywordValue  -- 关键字的值
        ' 返回：0：成功，-1：失败
        Public Shared Function InsertKeywordValue2myTemp(ByVal a_UserGUID As String, ByVal a_KeywordName As String, ByVal a_KeywordValue As String) As Integer
            Dim dtTemp As DataTable
            Dim strSQL As String
            Dim intReturn As Integer

            '查询关键字
            strSQL = "SELECT TempGUID, TempID, TempType, TempValue, CreateOn FROM myTemp" & _
                     " WHERE TempID = '" & a_UserGUID.Replace("'", "''") & "'" & _
                     " AND TempType = '关键字'"
            dtTemp = MyDB.GetDataTable(strSQL)

            Dim xmlDoc As New XmlDocument
            Dim xmlN As XmlNode
            Dim xmlAttr As XmlAttribute
            Dim i As Integer

            If dtTemp.Rows.Count = 0 Then
                '无记录，要插入新记录
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
                         "      '关键字'," & _
                         "      '" & xmlDoc.InnerXml.Replace("'", "''") & "'," & _
                         "      GETDATE())"
                intReturn = MyDB.ExecSQL(strSQL)

                If intReturn = 0 Then Return -1
            Else
                '有记录，需要插入关键字
                Try
                    xmlDoc.LoadXml(dtTemp.Rows(0).Item("TempValue").ToString)

                    If xmlDoc.DocumentElement.Name <> "keywords" Then
                        xmlDoc.LoadXml("<keywords/>")
                    End If
                Catch ex As Exception
                	Mysoft.Map.Data.MyDB.LogException(ex)
                    xmlDoc.LoadXml("<keywords/>")
                End Try

                '查找是否已存在要设置的关键字
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

                '更新
                strSQL = "UPDATE myTemp SET " & _
                         "TempValue = '" & xmlDoc.InnerXml.Replace("'", "''") & "'," & _
                         "CreateOn = GETDATE() " & _
                         "WHERE TempGUID = '" & dtTemp.Rows(0).Item("TempGUID").ToString & "'"
                intReturn = MyDB.ExecSQL(strSQL)

                If intReturn = 0 Then Return -1
            End If

            Return 0
        End Function

        '序列化到文件
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

        '从文件反序列化
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
