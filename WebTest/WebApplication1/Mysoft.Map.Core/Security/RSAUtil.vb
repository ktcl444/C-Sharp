Imports System.Security.Cryptography
Imports System.Text
Imports Mysoft.Map.Application
Imports System.Collections.Generic
Imports System.Web

Namespace Security
    ''add 2009.8.14 kongy RSA 签名验证类
    Public Class RSAUtil
        Public Const privateKey As String = "<RSAKeyValue><Modulus>mE8PFoNc6IoMFam4zCquflCkquFB823eNc6lob2yRqSHx8LFMCpttcF2zEZ5I3uSiOGBP8aijTjD8JSk7TvU+EtZfP04hL+nARAGyQM1u+FiabsBOQlkDbOq59dkThmjSPdHw0x/igRdFHe1Uwru4nRbKs1OWczzUsJWWDLV6+0=</Modulus><Exponent>AQAB</Exponent><P>1wBBFlOVkA8O8txRISj9ql8wiuAz3jGTKCaGWnojWqjIhw9W84mPbosrzDS59ZaboVJ2198zRl6jwThR0imR8Q==</P><Q>tVpZRyb1MfLPZsRG0eSV9J05+28DX2FcZWHUCzsVBgJXlBiBaRfgi2eR32a9yNbSZXpqUIr5nbdhXuEP2Yr9vQ==</Q><DP>HvtmZbU9xDinSs/80O57P2XgNOMCFm7Gae7DRZ58IcBYxT2spgOYq7Faal7evUkqvCCKB6meVfGlX16iS8q5wQ==</DP><DQ>FhlGW8dBha6i21D7mEQUidRG5n6mmI7SpYAASMYQT8UlSuSZkGbac+JRAjoQ0lJrHPaH0fy9YhygfuFJ/yZSuQ==</DQ><InverseQ>t9Wr+33I+/wMUuEwzF1MjBZ+LfCIK7HqSxR3kv1oxr1sjq3uQXn+feJQQISNyKM44sNJxWa8TodVbtjwidsmRg==</InverseQ><D>HuqVXnWFy3ISJ+eOqmrThrJp6oHU+EvJ+lQbDOzLnklRgnwHuNIz+NvveGGpv0kbIovbx41Te6UVKOWTYNBvVzLH3EqernsNJTGvJrBn5lwsBhS1OWYc6ajGBL4Z0yf7v5eHJxJPnCuXEE1XxksOUArZAhvlcTRHj7Wp+ETPwcE=</D></RSAKeyValue>"
        Public Const publicKey As String = "<RSAKeyValue><Modulus>mE8PFoNc6IoMFam4zCquflCkquFB823eNc6lob2yRqSHx8LFMCpttcF2zEZ5I3uSiOGBP8aijTjD8JSk7TvU+EtZfP04hL+nARAGyQM1u+FiabsBOQlkDbOq59dkThmjSPdHw0x/igRdFHe1Uwru4nRbKs1OWczzUsJWWDLV6+0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
        '获取 HASH 描述表
        Public Shared Function GetHash(ByVal strSource As String, ByRef strHashData As String) As Boolean
            Try
                '从字符串中取得hash描述
                Dim buffer As Byte()
                Dim hashdata As Byte()
                Dim md5 As HashAlgorithm = HashAlgorithm.Create("MD5")
                buffer = System.Text.Encoding.Default.GetBytes(strSource)
                hashdata = md5.ComputeHash(buffer)

                strHashData = Convert.ToBase64String(hashdata)
                Return True
            Catch ex As System.Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Throw ex
            End Try
        End Function

#Region "RSA 签名"
        'RSA 签名，参数strSignature为需要签名的数据，strSignatureData为签名后返回结果
        Public Shared Function SignatureFormatter(ByVal strSignature As String, ByRef strSignatureData As String) As Boolean
            Dim strPrivateKey As String = GetCustomPrivateKey()
            '如果有自定义私钥，则以自定义私钥进行签名
            If strPrivateKey <> "" Then
                Return SignatureFormatter(strPrivateKey, strSignature, strSignatureData)
            Else
                '如果没有自定义私钥，则以默认私钥进行签名
                Return SignatureFormatter(privateKey, strSignature, strSignatureData)
            End If
        End Function

        '通过指定私钥进行RSA 签名，参数strPrivateKey为私钥，strSignature为需要签名的数据，strSignatureData为签名后返回结果
        Public Shared Function SignatureFormatter(ByVal strPrivateKey As String, ByVal strSignature As String, ByRef strSignatureData As String) As Boolean
            Dim strHashByteSignature As String = String.Empty
            If Not GetHash(strSignature, strHashByteSignature) Then
                Return False
            End If
            Return SignatureFormatterByHash(strPrivateKey, strHashByteSignature, strSignatureData)
        End Function


        'RSA 签名，通过Hash串计算得到签名
        Public Shared Function SignatureFormatterByHash(ByVal strHashByteSignature As String, ByRef strSignatureData As String) As Boolean
            '通过默认私钥生成签名
            Return SignatureFormatter(privateKey, strHashByteSignature, strSignatureData)
        End Function

        'RSA 签名，指定私钥，通过Hash串计算得到签名
        Public Shared Function SignatureFormatterByHash(ByVal strPrivateKey As String, ByVal strHashByteSignature As String, ByRef strSignatureData As String) As Boolean
            Try
                Dim hashbytesignature As Byte()
                Dim encryptedsignaturedata As Byte()

                hashbytesignature = Convert.FromBase64String(strHashByteSignature)

                RSACryptoServiceProvider.UseMachineKeyStore = True
                Dim rsa As RSACryptoServiceProvider = New RSACryptoServiceProvider()
                rsa.FromXmlString(strPrivateKey)

                Dim rsaformatter As RSAPKCS1SignatureFormatter = New RSAPKCS1SignatureFormatter(rsa)
                '设置签名的算法为MD5
                rsaformatter.SetHashAlgorithm("MD5")
                '执行签名
                encryptedsignaturedata = rsaformatter.CreateSignature(hashbytesignature)

                strSignatureData = Convert.ToBase64String(encryptedsignaturedata)

                Return True
            Catch ex As System.Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Throw ex
            End Try
        End Function
#End Region

#Region "RSA 验证签名"

        '验证 RSA 签名，参数strDeformatter为原数据，strDeformatterData为签名数据，签名匹配返回True，签名不匹配返回False
        Public Shared Function SignatureDeformatter(ByVal strDeformatter As String, ByVal strDeformatterData As String) As Boolean
            Dim strPublicKey As String = GetCustomPublicKey()
            '如果有自定义公钥，则以自定义公钥进行验证签名
            If strPublicKey <> "" Then
                Return SignatureDeformatter(strPublicKey, strDeformatter, strDeformatterData)
            Else
                '如果没有自定义公钥，则以默认公钥进行验证签名
                Return SignatureDeformatter(publicKey, strDeformatter, strDeformatterData)
            End If
        End Function

        '通过指定公钥验证 RSA 签名，参数strPublicKey为指定公钥，strDeformatter为原数据，strDeformatterData为签名数据，签名匹配返回True，签名不匹配返回False
        Public Shared Function SignatureDeformatter(ByVal strPublicKey As String, ByVal strDeformatter As String, ByVal strDeformatterData As String) As Boolean
            Dim strHashByteDeformatter As String = String.Empty
            If Not GetHash(strDeformatter, strHashByteDeformatter) Then
                Return False
            End If
            Return SignatureDeformatterByHash(strPublicKey, strHashByteDeformatter, strDeformatterData)
        End Function

        'RSA 验证签名，通过Hash串验证
        Public Shared Function SignatureDeformatterByHash(ByVal strHashByteDeformatter As String, ByVal strDeformatterData As String) As Boolean
            Return SignatureDeformatterByHash(publicKey, strHashByteDeformatter, strDeformatterData)
        End Function


        'RSA 验证签名，通过Hash串验证,指定公钥
        Public Shared Function SignatureDeformatterByHash(ByVal strPublicKey As String, ByVal strHashByteDeformatter As String, ByVal strDeformatterData As String) As Boolean
            Try
                Dim deformatterdata As Byte()
                Dim hashbytedeformatter As Byte()

                hashbytedeformatter = Convert.FromBase64String(strHashByteDeformatter)
                RSACryptoServiceProvider.UseMachineKeyStore = True
                Dim rsa As RSACryptoServiceProvider = New RSACryptoServiceProvider()
                rsa.FromXmlString(strPublicKey)

                Dim rsadeformatter As RSAPKCS1SignatureDeformatter = New RSAPKCS1SignatureDeformatter(rsa)
                '指定解密的时候hash算法为MD5
                rsadeformatter.SetHashAlgorithm("MD5")

                deformatterdata = Convert.FromBase64String(strDeformatterData)

                If (rsadeformatter.VerifySignature(hashbytedeformatter, deformatterdata)) Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return False
            End Try
        End Function
#End Region

#Region " 加解密 "

        ' RSA 加密(不限长度)
        Public Shared Function Crypt(ByVal s As String, ByVal publicKey As String) As String
            If String.IsNullOrEmpty(s) Then Return ""

            Dim DataToEncrypt() As Byte
            Dim EncryptedData() As Byte

            Try
                DataToEncrypt = System.Text.Encoding.UTF8.GetBytes(s)

                System.Security.Cryptography.RSACryptoServiceProvider.UseMachineKeyStore = True
                Dim RSA = New System.Security.Cryptography.RSACryptoServiceProvider()
                RSA.FromXmlString(publicKey)


                Dim keySize As Integer = RSA.KeySize / 8

                Dim bufferSize As Integer = keySize - 11

                Dim buffer(bufferSize - 1) As Byte

                Dim msInput As New System.IO.MemoryStream(DataToEncrypt)

                Dim msOutput As New System.IO.MemoryStream()

                Dim readLen As Integer = msInput.Read(buffer, 0, bufferSize)

                While (readLen > 0)
                    Dim dataToEnc(readLen - 1) As Byte
                    Array.Copy(buffer, 0, dataToEnc, 0, readLen)

                    Dim encData() As Byte = RSA.Encrypt(dataToEnc, False)
                    msOutput.Write(encData, 0, encData.Length)
                    readLen = msInput.Read(buffer, 0, bufferSize)
                End While

                msInput.Close()
                EncryptedData = msOutput.ToArray()    '得到加密结果
                msOutput.Close()

                RSA.Clear()
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return ""
            End Try

            Return Convert.ToBase64String(EncryptedData)
        End Function

        ' RSA解密(不限长度)
        Public Shared Function Decrypt(ByVal s As String, ByVal privateKey As String) As String
            If String.IsNullOrEmpty(s) Then Return ""

            Dim DataToDecrypt() As Byte
            Dim DecryptedData() As Byte

            Try
                DataToDecrypt = Convert.FromBase64String(s)

                System.Security.Cryptography.RSACryptoServiceProvider.UseMachineKeyStore = True
                Dim RSA As New System.Security.Cryptography.RSACryptoServiceProvider()
                RSA.FromXmlString(privateKey)

                Dim keySize As Integer = RSA.KeySize / 8
                Dim buffer(keySize - 1) As Byte

                Dim msInput As New System.IO.MemoryStream(DataToDecrypt)
                Dim msOutput As New System.IO.MemoryStream()
                Dim readLen As Integer = msInput.Read(buffer, 0, keySize)

                While (readLen > 0)
                    Dim dataToDec(readLen - 1) As Byte
                    Array.Copy(buffer, 0, dataToDec, 0, readLen)

                    Dim decData() As Byte = RSA.Decrypt(dataToDec, False)
                    msOutput.Write(decData, 0, decData.Length)
                    readLen = msInput.Read(buffer, 0, keySize)
                End While

                msInput.Close()

                DecryptedData = msOutput.ToArray()    '得到解密结果

                msOutput.Close()

                RSA.Clear()
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return ""
            End Try

            Return System.Text.Encoding.UTF8.GetString(DecryptedData)
        End Function


        'RSA 加密
        Public Shared Function Crypt(ByVal s As String) As String
            If s Is Nothing OrElse s.Length = 0 Then Return ""

            Dim b() As Byte
            RSACryptoServiceProvider.UseMachineKeyStore = True
            Dim rsaEnc As New RSACryptoServiceProvider

            rsaEnc.FromXmlString(publicKey)

            Try
                b = rsaEnc.Encrypt(Encoding.UTF8.GetBytes(s), False)
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return ""
            End Try

            Return Convert.ToBase64String(b)
        End Function

        'RSA 解密
        Public Shared Function Decrypt(ByVal s As String) As String
            If s Is Nothing OrElse s.Length = 0 Then Return ""

            Dim b() As Byte
            b = Convert.FromBase64String(s)

            RSACryptoServiceProvider.UseMachineKeyStore = True
            Dim rsaDec As New RSACryptoServiceProvider
            rsaDec.FromXmlString(privateKey)

            Try
                b = rsaDec.Decrypt(b, False)
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return ""
            End Try

            Return Encoding.UTF8.GetString(b)
        End Function

#End Region

        '获取RSA密钥对，isCreateNew为true表示创建新密钥，为false时，只提供公钥
        Public Shared Function GetRSAKey(ByVal isCreateNew As Boolean) As IDictionary
            Dim Rtn As New Dictionary(Of String, Object)
            If isCreateNew Then
                RSACryptoServiceProvider.UseMachineKeyStore = True
                Dim rsa As New RSACryptoServiceProvider(1024)
                Rtn.Add("PublicKey", rsa.ToXmlString(False))
                Rtn.Add("PrivateKey", rsa.ToXmlString(True))
            Else
                Rtn.Add("PublicKey", publicKey)
                Rtn.Add("PrivateKey", privateKey)
            End If
            Return Rtn
        End Function

        Public Shared Sub RebuildRSAKey()
            HttpContext.Current.Application("RSAKey") = GetRSAKey(True)
        End Sub

        Public Shared Function GetApplicationPrivateKey() As String
            Dim Rtn As Dictionary(Of String, Object) = CType(HttpContext.Current.Application("RSAKey"), Dictionary(Of String, Object))
            If Not Rtn Is Nothing Then
                Return Rtn("PrivateKey").ToString
            End If
            Return privateKey
        End Function

        Public Shared Function GetApplicationPublicKey() As String
            Dim Rtn As Dictionary(Of String, Object) = CType(HttpContext.Current.Application("RSAKey"), Dictionary(Of String, Object))
            If Not Rtn Is Nothing Then
                Return Rtn("PublicKey").ToString
            End If
            Return publicKey
        End Function

        '获取Web.config中用户自定义的私钥
        Private Shared Function GetCustomPrivateKey() As String
            Return WebConfig.ReadAppSettings("PrivateKey")
        End Function

        '获取Web.config中用户自定义的公钥
        Private Shared Function GetCustomPublicKey() As String
            Return WebConfig.ReadAppSettings("PublicKey")
        End Function
    End Class

End Namespace
