Imports System.Data.SqlClient
Imports Microsoft.Win32
Imports System.Configuration
Imports Mysoft.Map.Security
Imports System.Text
Imports System.Xml
Imports System.Web

Imports System
Imports System.Data
Imports System.Data.OleDb
Imports System.Collections.Specialized
Imports Mysoft.Map.Application.Security
Imports Mysoft.Map.Caching
Imports System.IO
Imports System.Reflection
Imports Mysoft.Map.MyException

Namespace Data

    ''' <summary>
    ''' ��װ���ݿ�����࣬�ṩ��ȡ���������ַ��������ݿ������غ�����
    ''' </summary>
    Public Class MyDB
        Private Shared _DefaultAppName As String = "Default"

        '***************************  Public ���� *****************************

        ''' <summary>
        ''' ��ȡ SQL ���ݿ������ַ�����
        ''' </summary>
        ''' <returns>���ݿ������ַ������磺server=netserver,1433;uid=sa;pwd=12345678;database=dotnet_erp25�����ע���û��ָ�����ݿ�˿ںţ�Ĭ�� 1433�� </returns>
        Public Shared Function GetSqlConnectionString() As String
            Return GetSqlConnectionString(_DefaultAppName)
        End Function

        ''' <summary>
        ''' ��ȡָ����Ӧ�ó�������Ӧ�� SQL ���ݿ������ַ�����
        ''' </summary>
        ''' <param name="appName">Ӧ�ó�����</param>
        ''' <returns>���ݿ������ַ������磺server=netserver,1433;uid=sa;pwd=12345678;database=dotnet_erp25�����ע���û��ָ�����ݿ�˿ںţ�Ĭ�� 1433�� </returns>
        Public Shared Function GetSqlConnectionString(ByVal appName As String) As String
            Dim isHttpContext As Boolean = True
            If HttpContext.Current Is Nothing Then
                isHttpContext = False
            End If
            Dim strConn As String
            If isHttpContext Then
                strConn = HttpContext.Current.Application("ConnStr_" & appName)
            End If
            If Not strConn Is Nothing AndAlso strConn.Length > 0 Then
                Return strConn
            Else
                Dim MyDBConn As New DBConn(appName)

                If MyDBConn.ServerProt = "1433" Then        ' ��Щ����ǽ�����1433�˿ڣ�Ϊ�����ڼ���֮ǰ��ģʽ�����ж�
                    strConn = "server=" & MyDBConn.ServerName & ";uid=" & MyDBConn.UserName & ";pwd=" & MyDBConn.Password & ";database=" & MyDBConn.DBName
                Else
                    strConn = "server=" & MyDBConn.ServerName & "," & MyDBConn.ServerProt & ";uid=" & MyDBConn.UserName & ";pwd=" & MyDBConn.Password & ";database=" & MyDBConn.DBName
                End If
                If isHttpContext Then
                    HttpContext.Current.Application.Lock()
                    HttpContext.Current.Application("ConnStr_" & appName) = strConn
                    HttpContext.Current.Application.UnLock()
                End If

                Return strConn
            End If

        End Function

        ''' <summary>
        ''' ִ��SQL��ѯ������ȡ DataTable ���ͽ���� DataTable ����Ϊ<b>_tb</b>��
        ''' </summary>
        ''' <param name="sql">SQL ����ַ����� </param>
        ''' <returns>DataTable ʵ����DataTable ����Ϊ<b>_tb</b> </returns>
        Public Shared Function GetDataTable(ByVal sql As String) As DataTable
            Return GetDataTable(sql, "_tb", _DefaultAppName)
        End Function

        ''' <summary>
        ''' ִ��SQL��ѯ������ȡ DataTable ���ͽ����ָ�� DataTable ���ơ� 
        ''' </summary>
        ''' <param name="sql">SQL ����ַ�����</param>
        ''' <param name="tableName">DataTable ���ơ� </param>
        ''' <returns>DataTable ʵ����DataTable ����Ϊ<paramref name="tableName"></paramref>��ֵ</returns>
        Public Shared Function GetDataTable(ByVal sql As String, ByVal tableName As String) As DataTable
            Return GetDataTable(sql, tableName, _DefaultAppName)
        End Function

        ''' <summary>
        ''' ��ָ����Ӧ�ó���������ִ��SQL��ѯ������ȡ DataTable ���ͽ���� ָ�� DataTable ���ơ� 
        ''' </summary>
        ''' <param name="sql">SQL ����ַ�����</param>
        ''' <param name="tableName">DataTable ���ơ� </param>
        ''' <param name="appName">Ӧ�ó�������Ĭ�� Default���μ���Ȩ�ļ���/bin/License.xml���� dbconns/dbconn �ڵ�� appname ���ԡ� </param>
        ''' <returns>DataTable ʵ����DataTable ����Ϊ<paramref name="tableName"></paramref>��ֵ</returns>
        Public Shared Function GetDataTable(ByVal sql As String, ByVal tableName As String, ByVal appName As String) As DataTable
            Dim ConnStr As String
            Dim DS As DataSet
            Dim SqlConn As SqlConnection
            Dim SqlComm As SqlDataAdapter
            Try
                ConnStr = GetSqlConnectionString(appName)
                SqlConn = New SqlConnection(ConnStr)
                SqlComm = New SqlDataAdapter(sql, SqlConn)

                DS = New DataSet
                SqlComm.Fill(DS, tableName)

                Return DS.Tables(tableName)

            Catch ex As Exception

                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ
                MyDB.LogException("sql[appname:" + appName + "]:" & sql & ";tableName:" & tableName, ex)
                '------------------------------------------------------------------------------
                Return New DataTable()
            Finally
                SqlConn.Close()
            End Try

        End Function

        ''' <summary>
        ''' ִ�� Transact-SQL ������
        ''' </summary>
        ''' <param name="sql">SQL ����ַ����� </param>
        ''' <returns>���� UPDATE��INSERT �� DELETE ��䣬����ֵΪ��������Ӱ�������<br/>
        ''' �������͵���䣬����ֵΪ 0<br/>
        ''' SQL ִ��ʧ�ܡ�д���ݿ������־ʧ�ܡ�ִ��ʱ�䳬�� 30 �룬����ֵΪ -1��</returns>
        ''' <remarks>������ʹ�ó� INSERT��DELELE��UPDATE �������֮��� SQL ��䣡��Ϊ�����޷�ȷ������ֵ�ĺ��壡<br/>
        ''' <b>�йؼ�¼���ݿ������־��</b><br/>
        ''' ִ�иú���ʱ�Ὣ��� INSERT��UPDATE��DELTE ��ѯ����¼�� mySQLLog ���У���Ҫ�� web.config �ļ���<c> &lt;appSetting&gt;&lt;add key="TraceTable" value="��1,��2" /&gt;&lt;/appSetting&gt;</c> ����Ҫ���ٵı�<br/>  
        ''' ��� SQL ִ��ʧ�ܻὫ������Ϣ��¼�����ݿ������־�С� <br/>
        ''' </remarks>
        Public Shared Function ExecSQL(ByVal sql As String, ByVal recordLog As Boolean) As Integer
            Return ExecSQL(sql, _DefaultAppName, recordLog)
        End Function

        Public Shared Function ExecSQL(ByVal sql As String) As Integer
            Return ExecSQL(sql, _DefaultAppName, True)
        End Function

        ''' <summary>
        ''' ��ָ����Ӧ�ó���������ִ�� Transact-SQL ������
        ''' </summary>
        ''' <param name="sql">SQL ����ַ�����</param>
        ''' <param name="appName">Ӧ�ó��������μ���Ȩ�ļ���/bin/License.xml���� dbconns/dbconn �ڵ�� appname ���ԡ� </param>
        ''' <returns>���� UPDATE��INSERT �� DELETE ��䣬����ֵΪ��������Ӱ�������<br/>
        ''' �������͵���䣬����ֵΪ 0<br/>
        ''' SQL ִ��ʧ�ܡ�д���ݿ������־ʧ�ܡ�ִ��ʱ�䳬�� 30 �룬����ֵΪ -1��
        ''' </returns>
        ''' <remarks>������ʹ�ó� INSERT��DELELE��UPDATE �������֮��� SQL ��䣡��Ϊ�����޷�ȷ������ֵ�ĺ��壡<br/>
        ''' <b>�йؼ�¼���ݿ������־��</b><br/>
        ''' ִ�иú���ʱ�Ὣ��� INSERT��UPDATE��DELTE ��ѯ����¼�� mySQLLog ���У���Ҫ�� web.config �ļ���<c> &lt;appSetting&gt;&lt;add key="TraceTable" value="��1,��2" /&gt;&lt;/appSetting&gt;</c> ����Ҫ���ٵı�<br/>  
        ''' ��� SQL ִ��ʧ�ܻὫ������Ϣ��¼�����ݿ������־�С� <br/>
        ''' </remarks>
        Public Shared Function ExecSQL(ByVal sql As String, ByVal appName As String, ByVal recordLog As Boolean) As Integer
            If String.IsNullOrEmpty(sql) Then Return 0
            Dim SqlConn As SqlConnection
            Dim SqlComm As SqlCommand
            Dim RowsCount As Integer

            SqlConn = New SqlConnection(GetSqlConnectionString(appName))
            SqlComm = New SqlCommand(sql, SqlConn)
            SqlComm.CommandTimeout = 300          '���ڴ��������ı�Ĭ��30�벻��
            Try
                SqlConn.Open()
                RowsCount = SqlComm.ExecuteNonQuery()

                If RowsCount = -1 Then      ' �� SQL Ϊ�� INSERT��UPDATE��DELETE ���ʱ������Ϊ -1
                    RowsCount = 0
                End If

                If recordLog Then
                    If Not HttpContext.Current Is Nothing Then
                        ' ��¼ SQL ��־
                        WriteSQLLog(sql, appName)
                    End If
                End If
                Return RowsCount
            Catch ex As System.Exception
                ' ��¼ SQL ������־
                'WriteSQLLog(sql, appName, ex.Message.ToString)

                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Return -1
            Finally
                SqlConn.Close()
            End Try

        End Function

        ' ���ܣ�ִ��sql���
        ' ������sql���
        ' ���أ�Ӱ���¼��������ֱ���׳��쳣
        Public Shared Function ExecuteSQL(ByVal sql As String) As Integer
            Return ExecuteSQL(sql, _DefaultAppName)
        End Function

        Public Shared Function ExecuteSQL(ByVal sql As String, ByVal appName As String) As Integer
            If String.IsNullOrEmpty(sql) Then Return 0
            Dim SqlConn As SqlConnection
            Dim SqlComm As SqlCommand
            Dim RowsCount As Integer

            SqlConn = New SqlConnection(GetSqlConnectionString(appName))
            SqlComm = New SqlCommand(sql, SqlConn)
            SqlComm.CommandTimeout = 300          '���ڴ��������ı�Ĭ��30�벻��
            Try
                SqlConn.Open()
                RowsCount = SqlComm.ExecuteNonQuery()

                If RowsCount = -1 Then      ' �� SQL Ϊ�� INSERT��UPDATE��DELETE ���ʱ������Ϊ -1
                    RowsCount = 0
                End If

                ' ��¼ SQL ��־
                WriteSQLLog(sql, appName)
                Return RowsCount
            Catch ex As System.Exception
                ' ��¼ SQL ������־
                'WriteSQLLog(sql, appName, ex.Message.ToString)
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Return -1
            Finally
                SqlConn.Close()
            End Try
        End Function

        '���ܣ�ִ��SQL��䣬������IDֵ
        '������sql���
        '���أ�id
        Public Shared Function ExecSQL_Id(ByVal sql As String) As Int64
            Return ExecSQL_Id(sql, _DefaultAppName)
        End Function

        Public Shared Function ExecSQL_Id(ByVal sql As String, ByVal appName As String) As Int64
            Dim SqlConn As SqlConnection
            Dim SqlComm As SqlCommand
            Dim RowsCount As Integer

            SqlConn = New SqlConnection(GetSqlConnectionString(appName))
            SqlComm = New SqlCommand(sql & ";SELECT @@IDENTITY", SqlConn)
            SqlComm.CommandTimeout = 300          '���ڴ��������ı�Ĭ��30�벻��
            Try
                SqlConn.Open()
                RowsCount = Convert.ToInt64(SqlComm.ExecuteScalar())

                ' ��¼ SQL ��־
                WriteSQLLog(sql, appName)
                Return RowsCount
            Catch ex As System.Exception
                ' ��¼ SQL ������־
                'WriteSQLLog(sql, appName, ex.Message.ToString)

                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Return -1
            Finally
                SqlConn.Close()
            End Try
        End Function

        '���ܣ���ȡsql��ѯ�ļ�¼��
        '������sql
        '���أ���¼��
        Public Shared Function GetRowsCount(ByVal sql As String) As Integer
            Return GetRowsCount(sql, _DefaultAppName)
        End Function

        Public Shared Function GetRowsCount(ByVal sql As String, ByVal appName As String) As Integer
            Dim DT As DataTable

            DT = GetDataTable(sql, "temp")
            Return DT.Rows.Count

        End Function

        '���ܣ�ͨ��sql����õ���ֵ���ַ��ͣ�
        '������sql���
        '���أ��ַ��ͣ�NULLΪ�մ�
        Public Shared Function GetDataItemString(ByVal sql As String) As String
            Return GetDataItemString(sql, _DefaultAppName)
        End Function

        Public Shared Function GetDataItemString(ByVal sql As String, ByVal appName As String) As String
            Dim MyConnection As New SqlClient.SqlConnection(GetSqlConnectionString(appName))
            Dim MyCommand As New SqlClient.SqlCommand(sql, MyConnection)
            Try
                MyConnection.Open()

                Dim StrReturn As String
                StrReturn = Convert.ToString(MyCommand.ExecuteScalar())

                Return StrReturn
            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Throw ex
            Finally
                MyConnection.Close()
            End Try
        End Function

        '���ܣ�ͨ��sql����õ���ֵ�����ͣ�
        '������sql���
        '���أ����ͣ�NULLΪ0
        Public Shared Function GetDataItemInt(ByVal sql As String) As Integer
            Return GetDataItemInt(sql, _DefaultAppName)
        End Function

        Public Shared Function GetDataItemInt(ByVal sql As String, ByVal appName As String) As Integer
            Dim MyConnection As New SqlClient.SqlConnection(GetSqlConnectionString(appName))
            Dim MyCommand As New SqlClient.SqlCommand(sql, MyConnection)
            Try
                MyConnection.Open()

                Dim IntReturn As Integer
                Dim ObjReturn As Object = MyCommand.ExecuteScalar()
                If ObjReturn Is DBNull.Value Or ObjReturn Is Nothing Then
                    IntReturn = 0
                Else
                    IntReturn = Convert.ToInt64(ObjReturn)
                End If

                Return IntReturn
            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Throw ex
            Finally
                MyConnection.Close()
            End Try
        End Function

        '���ܣ�ͨ��sql����õ���ֵ�������ͣ�
        '������sql���
        '���أ������ͣ�NULLΪ0
        Public Shared Function GetDataItemDecimal(ByVal sql As String) As Decimal
            Return GetDataItemDecimal(sql, _DefaultAppName)
        End Function

        Public Shared Function GetDataItemDecimal(ByVal sql As String, ByVal appName As String) As Decimal
            Dim MyConnection As New SqlClient.SqlConnection(GetSqlConnectionString(appName))
            Dim MyCommand As New SqlClient.SqlCommand(sql, MyConnection)
            Try
                MyConnection.Open()

                Dim DecReturn As Decimal
                Dim ObjReturn As Object = MyCommand.ExecuteScalar()
                If ObjReturn Is DBNull.Value Or ObjReturn Is Nothing Then
                    DecReturn = 0
                Else
                    DecReturn = Convert.ToDecimal(ObjReturn)
                End If

                Return DecReturn
            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Throw ex
            Finally
                MyConnection.Close()
            End Try
        End Function

        '���ܣ�ͨ��sql����õ���ֵ�������ͣ�
        '������sql���
        '���أ������ͣ�NULLΪ1900-01-01
        Public Shared Function GetDataItemDateTime(ByVal sql As String) As DateTime
            Return GetDataItemDateTime(sql, _DefaultAppName)
        End Function

        Public Shared Function GetDataItemDateTime(ByVal sql As String, ByVal appName As String) As DateTime
            Dim MyConnection As New SqlClient.SqlConnection(GetSqlConnectionString(appName))
            Dim MyCommand As New SqlClient.SqlCommand(sql, MyConnection)
            Try
                MyConnection.Open()

                Dim DTReturn As DateTime
                Dim ObjReturn As Object = MyCommand.ExecuteScalar()
                If ObjReturn Is DBNull.Value Or ObjReturn Is Nothing Then
                    DTReturn = Convert.ToDateTime("1900-01-01")
                Else
                    DTReturn = Convert.ToDateTime(ObjReturn)
                End If

                Return DTReturn
            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Throw ex
            Finally
                MyConnection.Close()
            End Try
        End Function


        Public Shared Function SaveXml(ByVal dataXml As String) As String
            Return SaveXml(dataXml, "NO XMLHTTP")
        End Function

        Public Shared Function SaveXml(ByVal dataXml As String, ByVal invokeType As String) As String
            Return SaveXml(dataXml, invokeType, Guid.NewGuid.ToString)
        End Function

        ' ������invokeType     -- ���ú��������͡�
        '       ��ʱ�û���ֱ�ӱ��� DataXml ���Ե��øú�����ʹ�� XMLHTTP ��ʽͨ�� ApplicationMap.aspx.vb ���õĺ��������������������
        '       ����������һ�������������ж��Ƿ���ͨ�� XMLHTTP ���õ�
        '       keyValue       -- ������¼ʱ��ʹ��֮ǰ������ guid���޸ļ�¼ʱ��ֵ��Ч��
        '���أ�ִ�гɹ����������� GUID �� <xml result="true" ... �ַ�����ִ��ʧ�ܣ����� "" �� <xml result="false" ... �ַ�����
        Public Shared Function SaveXml(ByVal dataXml As String, ByVal invokeType As String, ByVal keyValue As String) As String
            Dim xmlDoc As New XmlDocument
            Dim keyValueOfXML As String
            Dim SQL As String

            '-----------------------------------------------------------------------------------
            'Modified by chenyong 2010-02-25   ��־������Ŀ�ؼ�������Դ������ֲ       
            Dim _AppName As String = "Default"

            Try
                xmlDoc.LoadXml(dataXml)

                If Not xmlDoc.DocumentElement.Attributes.GetNamedItem("keyvalue") Is Nothing Then
                    keyValueOfXML = xmlDoc.DocumentElement.Attributes.GetNamedItem("keyvalue").Value
                Else
                    keyValueOfXML = ""
                End If

                '-----------------------------------------------------------------------------------
                ''Modified by chenyong 2010-02-25   ��־������Ŀ�ؼ�������Դ������ֲ       
                'AppName
                Dim appNameXmlNode As XmlNode = xmlDoc.DocumentElement.Attributes.GetNamedItem("appname")
                If Not appNameXmlNode Is Nothing AndAlso Not String.IsNullOrEmpty(appNameXmlNode.Value) Then
                    _AppName = appNameXmlNode.Value
                End If

                If keyValueOfXML = "" Then      ' ����
                    SQL = getInserSqlByDataXml(dataXml, keyValue)

                Else      ' �޸�
                    keyValue = keyValueOfXML    ' ����ɾ��������ֵ��
                    SQL = getUpdateSqlByDataXml(dataXml)

                End If

                'Modified by chenyong 2010-02-25   ��־������Ŀ�ؼ�������Դ������ֲ       
                If ExecSQL(SQL, _AppName) = -1 Then       ' ʧ��
                    If invokeType = "NO XMLHTTP" Then
                        Return ""
                    Else
                        Return "<xml result=""false"" errormessage=""����ʱ�쳣���������Ա��ϵ��""></xml>"
                    End If

                Else        ' �ɹ�
                    If invokeType = "NO XMLHTTP" Then
                        Return keyValue
                    Else
                        Return "<xml result=""true"" keyvalue=""" & keyValue & """></xml>"
                    End If

                End If

            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ                     
                MyDB.LogException("dataXml:" & dataXml & ";invokeType:" & invokeType, ex)
                '------------------------------------------------------------------------------

                If invokeType = "NO XMLHTTP" Then
                    Return ""
                Else
                    Return "<xml result=""false"" errormessage=""����ʱ�쳣���������Ա��ϵ��""></xml>"
                End If
            End Try

        End Function


        Public Shared Function GetSqlByDataXml(ByVal dataXml As String) As String
            Return SaveXml(dataXml, "NO XMLHTTP")
        End Function

        ' ���ܣ����� DataXml ����һ�� SQL �ַ���
        ' ������invokeType      -- �μ� SaveXml
        ' ���أ����ͨ�� XmlHTTP ��ʽ���ý����ظ�ʽ�磺<xml result="true/false" keyvalue="xxx-xxxxx..." sql="SQL�ַ���"></xml>�����򣬷���һ�� SQL �ַ�����
        Public Shared Function GetSqlByDataXml(ByVal dataXml As String, ByVal invokeType As String) As String
            Dim XmlDoc As New XmlDocument
            Dim KeyValue As String
            Dim SQL As String

            Try
                XmlDoc.LoadXml(dataXml)

                If Not XmlDoc.DocumentElement.Attributes.GetNamedItem("keyvalue") Is Nothing Then
                    KeyValue = XmlDoc.DocumentElement.Attributes.GetNamedItem("keyvalue").Value
                Else
                    KeyValue = ""
                End If

                If KeyValue = "" Then      ' ����
                    KeyValue = Guid.NewGuid.ToString
                    SQL = getInserSqlByDataXml(dataXml, KeyValue)

                Else      ' �޸�
                    SQL = getUpdateSqlByDataXml(dataXml)

                End If

                If invokeType = "NO XMLHTTP" Then
                    Return SQL
                Else
                    Return "<xml result=""true"" keyvalue=""" & KeyValue & """ sql=""" & SQL & """></xml>"
                End If

            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                '  Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ                  
                MyDB.LogException("dataXml:" & dataXml & ";invokeType:" & invokeType, ex)
                '------------------------------------------------------------------------------
                If invokeType = "NO XMLHTTP" Then
                    Return ""
                Else
                    Return "<xml result=""false"" errormessage=""����ʱ�쳣���������Ա��ϵ��""></xml>"
                End If
            End Try

        End Function


        ' ���ܣ���ȡ�û���ĿȨ�޵� SQL ���ʽ��
        ' ������userGUID    -- �û� GUID
        '       tableName   -- ƴд SQL ���ʽ��ʹ�ñ���
        '       buGUID      -- ��˾��ֻ���˸ù�˾�µ���Ŀ
        '       application     -- ����ϵͳ�����ڿͷ�������Ŀ����������¥�Ϳͷ�����Ŀ����ͬһ�ű�ȡֵ
        '       isUserFilter  -- �û����˵���Ŀ�Ƿ���Ч��Ĭ�� true
        ' ˵������ͨ�����Ŀ���˲�����ϵͳ�����û�������������Ŀ�ж��û�������Ŀ��Ȩ
        ' ���أ����� SQL ���ʽ���磺Lead.ProjGUID in ('7AED691F-DC73-4632-949E-110A39B7B5A5','7CE7774C-16B9-4558-A271-0FB1F375BA08')
        ' 2007.7.9  zt
        ' �������޸ģ��ָ�֮ǰע�͵�ǰ����������buGUID Ĭ�ϵ�ǰ��˾��application Ĭ�� 0101��
        ''' <exclude></exclude>
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="userGUID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetProjectFilterExp(ByVal userGUID As String) As String
            Return GetProjectFilterExp(userGUID, Nothing)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="userGUID"></param>
        ''' <param name="tableName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetProjectFilterExp(ByVal userGUID As String, ByVal tableName As String) As String
            Return GetProjectFilterExp(userGUID, tableName, HttpContext.Current.Session("BUGUID"))
        End Function

        ''' <exclude></exclude>
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="userGUID"></param>
        ''' <param name="tableName"></param>
        ''' <param name="buGUID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetProjectFilterExp(ByVal userGUID As String, ByVal tableName As String, ByVal buGUID As String) As String
            Return GetProjectFilterExp(userGUID, tableName, buGUID, "0101")
        End Function

        ''' <exclude></exclude>
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="userGUID"></param>
        ''' <param name="tableName"></param>
        ''' <param name="buGUID"></param>
        ''' <param name="application"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetProjectFilterExp(ByVal userGUID As String, ByVal tableName As String, ByVal buGUID As String, ByVal application As String) As String
            Return GetProjectFilterExp(userGUID, tableName, buGUID, application, True)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="userGUID"></param>
        ''' <param name="tableName"></param>
        ''' <param name="buGUID"></param>
        ''' <param name="application"></param>
        ''' <param name="isUserFilter"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetProjectFilterExp(ByVal userGUID As String, ByVal tableName As String, ByVal buGUID As String, ByVal application As String, ByVal isUserFilter As Boolean) As String
            Dim UserProjects As String

            UserProjects = GetProjectString(userGUID, buGUID, application, isUserFilter)     ' ȡ�û���Ŀ
            If UserProjects = "" Then
                Return "(1=2)"

            Else
                If tableName = "" Or tableName Is Nothing Then
                    Return "ProjGUID in (" & UserProjects & ")"

                Else
                    Return tableName & ".ProjGUID in (" & UserProjects & ")"

                End If

            End If

        End Function

        ' ˵���������û���Ŀ�ַ���
        ' ���أ���Ŀ�ַ������磺'7AED691F-DC73-4632-949E-110A39B7B5A5','7CE7774C-16B9-4558-A271-0FB1F375BA08'
        '       ���û����Ŀ���ؿ��ַ���""
        ' 2007.7.9  zt
        ' �������޸ģ��ָ�֮ǰע�͵�ǰ����������buGUID Ĭ�ϵ�ǰ��˾��application Ĭ�� 0101��
        Public Shared Function GetProjectString(ByVal userGUID As String) As String
            Return GetProjectString(userGUID, HttpContext.Current.Session("BUGUID"))
        End Function

        Public Shared Function GetProjectString(ByVal userGUID As String, ByVal buGUID As String) As String
            Return GetProjectString(userGUID, buGUID, Nothing)
        End Function

        Public Shared Function GetProjectString(ByVal userGUID As String, ByVal buGUID As String, ByVal application As String) As String
            Return GetProjectString(userGUID, buGUID, application, True)
        End Function

        Public Shared Function GetProjectString(ByVal userGUID As String, ByVal buGUID As String, ByVal application As String, ByVal isUserFilter As Boolean) As String
            ' Ӱ�컺��ĵط���
            ' 1����Ȩ��
            ' 2��������ɾ����Ŀ��
            ' 3���û��Լ�������Ŀ��
            ' 4���޸���Ŀ����Щϵͳ���ã�
            ' 5���޸� myApplication ��������ϵͳ���Ƿ�������Ŀ��ϵͳ���ˡ���
            ' 6���޸���Ȩ���� XML ���壨usp_myGetDataRightsXML����

            If userGUID Is Nothing Then
                Return ""
            End If

            ' �ȴ� Cache ��ȡ
            Dim oUserProjectSec As Object
            Dim strUserProjectSec, strPara As String      ' strPara�������ַ������������Ĳ����仯����Ҫˢ�»���
            Dim intTag As Integer
            Dim CACHEEXP As Integer = MyCache.DayFactor * 7            ' �������ʱ��Ϊһ��

            strPara = userGUID & "," & buGUID & "," & application & "," & isUserFilter
            oUserProjectSec = MyCache.Get("ProjectString_" & userGUID)
            If Not oUserProjectSec Is Nothing Then
                strUserProjectSec = oUserProjectSec
                intTag = InStr(strUserProjectSec, "|")

                ' �жϲ����Ƿ�仯
                If strPara = Left(strUserProjectSec, intTag - 1) Then
                    Return Mid(strUserProjectSec, intTag + 1)
                End If
            End If

            ' ��� Cache ��û�У������ȡ
            Dim sbReturn As New StringBuilder
            Dim SqlConn As New SqlConnection(MyDB.GetSqlConnectionString)
            Dim dt As New DataTable

            Try
                '��������	    
                Dim SqlComm As SqlCommand
                Dim SqlAdpt As SqlDataAdapter
                Dim SqlParams As SqlParameterCollection

                SqlComm = New SqlCommand("usp_myProjFilterBaseDT", SqlConn)
                SqlComm.CommandType = CommandType.StoredProcedure

                SqlParams = SqlComm.Parameters
                SqlParams.Add(New SqlParameter("@chvUserGUID", SqlDbType.NVarChar, 40))
                SqlParams.Add(New SqlParameter("@chvApplication", SqlDbType.NVarChar, 100))
                SqlParams.Add(New SqlParameter("@chvBUGUID", SqlDbType.NVarChar, 100))
                SqlParams.Add(New SqlParameter("@blnIsUserFilter", SqlDbType.Bit, 100))

                SqlParams("@chvUserGUID").Value = userGUID
                SqlParams("@chvApplication").Value = application
                SqlParams("@chvBUGUID").Value = buGUID
                SqlParams("@blnIsUserFilter").Value = isUserFilter

                '��ȡ����
                SqlConn.Open()
                SqlAdpt = New SqlDataAdapter
                SqlAdpt.SelectCommand = SqlComm
                SqlAdpt.Fill(dt)

                ' �����û���Ȩ�޵���Ŀ�ַ���
                For Each dr As DataRow In dt.Rows
                    sbReturn.Append(",'" & dr(0).ToString & "'")
                Next

                Dim strReturn As String = ""

                If sbReturn.ToString <> String.Empty Then
                    strReturn = Mid(sbReturn.ToString, 2)
                End If

                MyCache.Insert("ProjectString_" & userGUID, strPara & "|" & strReturn, CACHEEXP)
                Return strReturn

            Catch ex As Exception
                '------------------------------------------------------------------------------
                '   Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ                     
                MyDB.LogException(ex)
                '------------------------------------------------------------------------------
                'MyCache.Insert("ProjectString_" & userGUID, strPara & "|", CACHEEXP)
                MyCache.Remove("ProjectString_" & userGUID)
                Return ""
            Finally
                '�ر�����
                SqlConn.Close()
            End Try

        End Function


        ' ���ܣ������û�������Ŀ�Ŷ��еļ��𣬻�ȡ���¼����˵� SQL ���ʽ��
        ' ������userGUID    -- �û� GUID
        '       tableName   -- ƴд SQL ���ʽ��ʹ�ñ���
        '       buGUID      -- ��˾��ֻ���˸ù�˾�µ���Ŀ
        '       a_UserGUIDFieldname     -- ҵ��������ĸ��ֶμ�¼�û�GUID��Ĭ��UserGUID
        ' ���أ����� SQL ���ʽ���磺Lead.ProjGUID in ('7AED691F-DC73-4632-949E-110A39B7B5A5','7CE7774C-16B9-4558-A271-0FB1F375BA08')��û����ĿȨ�޷���(1=2)
        ' ע�⣺��¥��
        Public Shared Function GetProjectTeamFilterExp(ByVal userGUID As String) As String
            Return GetProjectTeamFilterExp(userGUID, "")
        End Function

        Public Shared Function GetProjectTeamFilterExp(ByVal userGUID As String, ByVal tableName As String) As String
            Return GetProjectTeamFilterExp(userGUID, tableName, Nothing)
        End Function

        Public Shared Function GetProjectTeamFilterExp(ByVal userGUID As String, ByVal tableName As String, ByVal buGUID As String) As String
            Return GetProjectTeamFilterExp(userGUID, tableName, Nothing, "UserGUID")
        End Function

        Public Shared Function GetProjectTeamFilterExp(ByVal userGUID As String, ByVal tableName As String, ByVal buGUID As String, ByVal a_UserGUIDFieldname As String) As String
            ' 2007.1.12 ���۽�������塢��ǰ��
            ' 1��ϵͳ����Ա�����ˣ����� 3=3
            ' 2����Ŀ������ ProjGUID = 'xxx-xxx-'
            ' 3���������� (StationHierarchyCode Like 'xxx.%')
            ' �����û���λ�������û�����ԴҲȡ�����ˣ������⡣Ӧ���ǣ��� Like ȡ�¼����ٵ����Լ�����Դ��2007.2.27 ��ϲ����ǰ
            ' �磺((StationHierarchyCode Like 'xxx.%') OR a_UserGUIDFieldname=UserGUID)
            If userGUID = "" Then
                userGUID = HttpContext.Current.Session("UserGUID").ToString
            End If

            If Mysoft.Map.Application.Security.User.IsAdmin(userGUID) Then      ' ���ϵͳ����Ա zt 2007.1.15
                Return "(3 = 3)"
            End If

            If tableName <> "" Then
                tableName += "."
            End If

            Dim strSQL As String
            Dim dtStation As DataTable
            Dim i, j As Integer

            Dim sbReturn As StringBuilder
            sbReturn = New StringBuilder

            '��ȡ�û����ڵĸ�λ
            strSQL = "SELECT s.StationGUID,s.HierarchyCode,ISNULL(s.IsProjManager,0) AS IsProjManager,s.ProjGUID" & _
                     " FROM myStation AS s" & _
                     " INNER JOIN myStationUser AS su ON s.StationGUID = su.StationGUID" & _
                     " WHERE su.UserGUID = '" & userGUID.Replace("'", "''") & "'"
            dtStation = MyDB.GetDataTable(strSQL)

            If dtStation.Rows.Count > 0 Then
                sbReturn.Append("(")

                ' ȡ���˵���Դ
                sbReturn.Append(tableName & a_UserGUIDFieldname & "='" & userGUID & "'")

                ' ȡ�¼�����Դ
                For i = 0 To dtStation.Rows.Count - 1
                    sbReturn.Append(" OR ")

                    If dtStation.Rows(i).Item("IsProjManager") = 1 Then      '��Ŀ����
                        sbReturn.Append(tableName & "TeamProjGUID = '" & dtStation.Rows(i).Item("ProjGUID").ToString & "'")
                    Else
                        sbReturn.Append(tableName & "StationHierarchyCode LIKE '" & dtStation.Rows(i).Item("HierarchyCode").ToString & ".%'")
                    End If
                Next

                sbReturn.Append(")")
            Else
                Return "1=2"
            End If

            Return sbReturn.ToString
        End Function


        ' ���ܣ���ȡ�����õĹ��˱��ʽ
        ' ������userGUID    -- �û� GUID
        '       tableName   -- ƴд SQL ���ʽ��ʹ�ñ���
        '       application      -- ҵ��Ӧ��ϵͳ���룬�磺0101����¥����0102���ͷ�����0103����Ա�� ��
        ' ˵������ҵ��ϵͳ������˷�ʽ������    zt 2006-03-24
        Public Shared Function GetFilterExpOfReport(ByVal userGUID As String, ByVal tableName As String, ByVal tableAliasName As String, ByVal application As String) As String
            Dim SQL As String
            Dim ReportFilterType As String

            ReportFilterType = GetDataItemString("SELECT ReportFilterType FROM myApplication WHERE Application='" & application & "'")
            Select Case ReportFilterType
                Case "��Ŀ"
                    If GetDataItemInt("SELECT COUNT(*) FROM data_dict WHERE table_name='" & tableName & "' AND field_name='ProjGUID'") > 0 Then    ' �������Ŀ���ֶδ���
                        Return GetProjectFilterExp(userGUID, tableAliasName, Nothing, "0101")
                    Else        ' �������Ŀ���ֶβ�����
                        Return "(4=4)"
                    End If

                Case "������Ŀ"
                    If GetDataItemInt("SELECT COUNT(*) FROM data_dict WHERE table_name='" & tableName & "' AND field_name='ProjGUID'") > 0 Then    ' �������Ŀ���ֶδ���
                        Return GetProjectFilterExp(userGUID, tableAliasName, Nothing, "0102")
                    Else        ' �������Ŀ���ֶβ�����
                        Return "(4=4)"
                    End If

                Case "��˾"
                    If GetDataItemInt("SELECT COUNT(*) FROM data_dict WHERE table_name='" & tableName & "' AND field_name='BUGUID'") > 0 Then     ' �������˾���ֶδ���
                        Return tableAliasName & ".BUGUID in ('" & HttpContext.Current.Session("BUGUID") & "')"
                    Else        ' �������˾���ֶβ�����
                        Return "(4=4)"
                    End If

                Case "������"
                    Return "(4=4)"

                Case Else
                    Return "(4=5)"

            End Select

        End Function


        ' ���ܣ�QueryXml ���� api ��ȡ SQL �����������ʽ��
        ' ������a_XmlNodeCondition      -- queryxml �� condition �ڵ㣬
        '       �磺<condition attribute="xx.ProjGUID" operator="api" datatype="buprojectfilter" value="<��˾GUID>" application="0102"/>
        ' ���أ������������ʽ
        ' ע�⣺�����⹫����ֻ�� AppGrid �ؼ�ƴд����ʱ����
        Public Shared Function getQueryString(ByVal a_Entity As String, ByVal a_XmlNodeCondition As XmlNode) As String
            Dim TableName, FieldName, Attribute, caseCode, value, application As String

            Attribute = a_XmlNodeCondition.Attributes.GetNamedItem("attribute").Value
            caseCode = a_XmlNodeCondition.Attributes.GetNamedItem("datatype").Value
            value = a_XmlNodeCondition.Attributes.GetNamedItem("value").Value
            If Not a_XmlNodeCondition.Attributes.GetNamedItem("application") Is Nothing Then
                application = a_XmlNodeCondition.Attributes.GetNamedItem("application").Value
            End If

            ' ��ȡ����
            TableName = ""
            If Trim(Attribute) <> "" Then
                Dim Attributes() As String = Attribute.Split(".")
                If Attributes.Length > 1 Then
                    TableName = Attributes(0)
                    FieldName = Attributes(1)
                Else
                    TableName = a_Entity
                    FieldName = Attributes(0)
                End If
            End If

            ' ִ�з�֧��������ȡ���˱��ʽ
            Select Case caseCode
                Case "buprojectfilter"          ' ��չ֧�ֹ�˾��Ŀ����  zt 2006.12.31
                    Return GetProjectFilterExp(HttpContext.Current.Session("UserGUID"), TableName, value, application).Replace("ProjGUID", FieldName)

                Case "xmdj"
                    Return GetProjectFilterExp(HttpContext.Current.Session("UserGUID"), Nothing, Nothing, Nothing, False)

                Case "allprojectofslxt"
                    ' ��¥ϵͳ�з��ص�ǰ��˾����ǰ�û���������Ŀ���˱��ʽ
                    ' ����֧��ǰ�˶����ֶΣ���������Ҫ�� ProjGUID �滻��ǰ�˶��������
                    Return GetProjectFilterExp(HttpContext.Current.Session("UserGUID"), TableName, HttpContext.Current.Session("BUGUID"), "0101").Replace("ProjGUID", FieldName)

                Case "allprojectofkfxt"
                    ' �ͷ�ϵͳ�з��ص�ǰ��˾����ǰ�û���������Ŀ���˱��ʽ
                    ' ����֧��ǰ�˶����ֶΣ���������Ҫ�� ProjGUID �滻��ǰ�˶��������
                    Return GetProjectFilterExp(HttpContext.Current.Session("UserGUID"), TableName, HttpContext.Current.Session("BUGUID"), "0102").Replace("ProjGUID", FieldName)

                Case "buildsofyuan"
                    ' ���ط�����¥�����˱��ʽ
                    Dim SQL As String
                    Dim DT As DataTable
                    Dim i As Integer
                    Dim sb As New StringBuilder

                    SQL = "SELECT BldGUID FROM p_Building WHERE CHARINDEX('" & value & ".', ParentCode + '.') = 1"
                    DT = GetDataTable(SQL)
                    If DT.Rows.Count > 0 Then
                        If TableName = "" Then
                            sb.Append(FieldName & " IN (")
                        Else
                            sb.Append(TableName & "." & FieldName & " IN (")
                        End If

                        For i = 0 To DT.Rows.Count - 1
                            If i = 0 Then
                                sb.Append("'" & DT.Rows(i)("BldGUID").ToString & "'")
                            Else
                                sb.Append(",'" & DT.Rows(i)("BldGUID").ToString & "'")
                            End If
                        Next

                        sb.Append(")")

                        Return sb.ToString
                    Else
                        Return "(1=2)"
                    End If

            End Select
        End Function


        '***************************  Private �������������ĵ�һ����ĸСд��������д��ĸ��д *****************************


        ' ���ܣ����� DataXml ���� Insert SQL
        Private Shared Function getInserSqlByDataXml(ByVal dataXml As String, ByVal keyValue As String) As String
            Dim xmlDoc As New XmlDocument
            Dim i As Integer
            Dim sbFields As New StringBuilder
            Dim sbValues As New StringBuilder
            Dim strEntityName As String
            Dim strKeyName As String

            Dim dtFields As DataTable
            Dim strSQL As String

            Dim strFieldName As String
            Dim strFieldType As String

            '-----------------------------------------------------------------------------------
            'Modified by chenyong 2010-02-25   ��־������Ŀ�ؼ�������Դ������ֲ       
            Dim _AppName As String = "Default"

            Try
                xmlDoc.LoadXml(dataXml)
            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ
                MyDB.LogException("dataXml:" & dataXml & ";keyValue:" & keyValue, ex)
                '------------------------------------------------------------------------------
                Return ""
            End Try

            '-----------------------------------------------------------------------------------
            ''Modified by chenyong 2010-02-25   ��־������Ŀ�ؼ�������Դ������ֲ       
            'AppName
            Dim appNameXmlNode As XmlNode = xmlDoc.DocumentElement.Attributes.GetNamedItem("appname")
            If Not appNameXmlNode Is Nothing AndAlso Not String.IsNullOrEmpty(appNameXmlNode.Value) Then
                _AppName = appNameXmlNode.Value
            End If


            strEntityName = xmlDoc.DocumentElement.Name
            strKeyName = xmlDoc.DocumentElement.Attributes.GetNamedItem("keyname").Value

            strSQL = "SELECT c.name AS field, t.name AS datatype FROM syscolumns c, sysobjects o, systypes t " & _
                     "WHERE c.id = o.id AND c.xtype = t.xtype AND o.name = '" & strEntityName.Replace("'", "''") & "'"
            dtFields = GetDataTable(strSQL, "fields", _AppName) 'Modified by chenyong 2010-02-25   ��־������Ŀ�ؼ�������Դ������ֲ  

            sbFields.Append(strKeyName)
            sbValues.Append("N'" & keyValue & "'")

            For i = 0 To xmlDoc.DocumentElement.ChildNodes.Count - 1
                strFieldName = xmlDoc.DocumentElement.ChildNodes(i).Name
                sbFields.Append("," & strFieldName)

                '��ȡ�ֶ�����
                Dim drTemp() As DataRow
                drTemp = dtFields.Select("field='" & strFieldName & "'")
                If drTemp.Length > 0 Then
                    strFieldType = drTemp(0).Item(1).ToString
                Else
                    strFieldType = ""
                End If

                ' money �� smallmoney ���͵����ݣ����ܴ� varchar ��ʽת��
                Select Case strFieldType
                    Case "money", "smallmoney"
                        sbValues.Append(",cast('" & xmlDoc.DocumentElement.ChildNodes(i).InnerText.Replace("'", "''").Replace(",", "") & "' as " & strFieldType & ")")
                    Case "uniqueidentifier", "datetime"
                        If xmlDoc.DocumentElement.ChildNodes(i).InnerText = "" Then
                            sbValues.Append(",NULL")
                        Else
                            sbValues.Append(",'" & xmlDoc.DocumentElement.ChildNodes(i).InnerText.Replace("'", "''") & "'")
                        End If
                    Case Else
                        sbValues.Append(",'" & xmlDoc.DocumentElement.ChildNodes(i).InnerText.Replace("'", "''") & "'")
                End Select
            Next

            Return "INSERT INTO " & strEntityName & " (" & sbFields.ToString & ") VALUES (" & sbValues.ToString & ")"

        End Function


        ' ���ܣ����� DataXml ���� Update SQL
        Private Shared Function getUpdateSqlByDataXml(ByVal dataXml As String) As String
            Dim xmlDoc As New XmlDocument

            Dim strEntityName As String
            Dim strKeyName As String
            Dim strKeyValue As String

            Dim dtFields As DataTable
            Dim strSQL As String

            Dim strFieldName As String
            Dim strFieldType As String
            Dim sb As New StringBuilder
            Dim i As Integer

            '-----------------------------------------------------------------------------------
            'Modified by chenyong 2010-02-25   ��־������Ŀ�ؼ�������Դ������ֲ       
            Dim _AppName As String = "Default"

            Try
                xmlDoc.LoadXml(dataXml)
            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   ��־������Ŀ������ֲ
                MyDB.LogException("dataXml:" & dataXml, ex)
                '------------------------------------------------------------------------------
                Exit Function
            End Try

            '-----------------------------------------------------------------------------------
            ''Modified by chenyong 2010-02-25   ��־������Ŀ�ؼ�������Դ������ֲ       
            'AppName
            Dim appNameXmlNode As XmlNode = xmlDoc.DocumentElement.Attributes.GetNamedItem("appname")
            If Not appNameXmlNode Is Nothing AndAlso Not String.IsNullOrEmpty(appNameXmlNode.Value) Then
                _AppName = appNameXmlNode.Value
            End If


            strEntityName = xmlDoc.DocumentElement.Name
            strKeyName = xmlDoc.DocumentElement.Attributes.GetNamedItem("keyname").Value
            strKeyValue = xmlDoc.DocumentElement.Attributes.GetNamedItem("keyvalue").Value

            strSQL = "SELECT c.name AS field, t.name AS datatype FROM syscolumns c, sysobjects o, systypes t " & _
                     "WHERE c.id = o.id AND c.xtype = t.xtype AND o.name = '" & strEntityName.Replace("'", "''") & "'"
            dtFields = GetDataTable(strSQL, "fields", _AppName) 'Modified by chenyong 2010-02-25   ��־������Ŀ�ؼ�������Դ������ֲ  

            sb.Append("UPDATE " & strEntityName & " SET ")
            For i = 0 To xmlDoc.DocumentElement.ChildNodes.Count - 1
                If i > 0 Then sb.Append(",")
                strFieldName = xmlDoc.DocumentElement.ChildNodes(i).Name

                '��ȡ�ֶ�����
                ' money �� smallmoney ���͵����ݣ����ܴ� varchar ��ʽת��
                Dim drTemp() As DataRow
                drTemp = dtFields.Select("field='" & strFieldName & "'")
                If drTemp.Length > 0 Then
                    strFieldType = drTemp(0).Item(1).ToString
                Else
                    strFieldType = ""
                End If

                sb.Append(strFieldName & "=")
                Select Case strFieldType
                    Case "money", "smallmoney"
                        sb.Append("cast('" & xmlDoc.DocumentElement.ChildNodes(i).InnerText.Replace("'", "''").Replace(",", "") & "' as " & strFieldType & ")")
                    Case "uniqueidentifier", "datetime"
                        If xmlDoc.DocumentElement.ChildNodes(i).InnerText = "" Then
                            sb.Append("NULL")
                        Else
                            sb.Append("'" & xmlDoc.DocumentElement.ChildNodes(i).InnerText.Replace("'", "''") & "'")
                        End If
                    Case Else
                        sb.Append("'" & xmlDoc.DocumentElement.ChildNodes(i).InnerText.Replace("'", "''") & "'")
                End Select
            Next
            sb.Append(" WHERE " & strKeyName & " = '" & strKeyValue.Replace("'", "''") & "'")

            Return sb.ToString

        End Function


        ' ���ܣ��� INSERT��UPDATE��DELETE ִ�� SQL д�������־
        Private Shared Sub WriteSQLLog(ByVal sql As String)
            WriteSQLLog(sql, _DefaultAppName)
        End Sub

        Private Shared Sub WriteSQLLog(ByVal sql As String, ByVal appName As String)
            WriteSQLLog(sql, appName, Nothing)
        End Sub

        Private Shared Sub WriteSQLLog(ByVal sql As String, ByVal appName As String, ByVal errorMessage As String)
            If HttpContext.Current.Application("TraceTable") Is Nothing Then    ' �����һ��д��־
                Dim TraceTable As String

                TraceTable = ConfigurationSettings.AppSettings("TraceTable")    ' ���ڵ�
                If TraceTable Is Nothing Then   ' ����ڵ㲻����
                    HttpContext.Current.Application("TraceTable") = ""
                    Exit Sub
                Else
                    HttpContext.Current.Application("TraceTable") = Trim(TraceTable)
                End If
            End If

            If HttpContext.Current.Application("TraceTable") = "" Then     ' ���û�ж���Ҫ��¼�ı�
                Exit Sub
            End If

            ' �ж� SQL �Ƿ���Ҫ����
            Dim ArrTable() As String
            Dim IsNeedTrace As Boolean
            Dim i As Int16

            ArrTable = HttpContext.Current.Application("TraceTable").Split(",")

            If ArrTable.Length = 0 Then
                Return
            End If

            For i = 0 To ArrTable.Length - 1
                If InStr(sql, ArrTable(i), CompareMethod.Text) > 0 Then
                    IsNeedTrace = True
                    Exit For
                End If
            Next

            ' ƴдִ�� SQL
            If IsNeedTrace Then
                Dim ExecSQL As String
                Dim MyConnection As SqlConnection
                Dim MyCommand As SqlCommand
                Dim IntReturn As Integer
                Dim Cont As HttpContext = HttpContext.Current
                Dim strErrorMessage As String
                If errorMessage Is Nothing Then
                    strErrorMessage = "NULL"
                Else
                    strErrorMessage = errorMessage
                End If

                ExecSQL = " INSERT INTO mySQLLog (LogGUID, ExeDateTime, ExeUser, ExeIP, ExePage, ExeSQL, ErrorMessage)" & _
                          " VALUES ('" & Guid.NewGuid.ToString & "', getdate() , '" & Cont.Session("username").ToString.Replace("'", "''") & "','" & Cont.Request.ServerVariables("REMOTE_ADDR") & "','" & Cont.Request.Path.ToString.Replace("'", "''") & "', '" & sql.Replace("'", "''") & "','" & strErrorMessage.Replace("'", "''") & "')"

                MyConnection = New SqlConnection(GetSqlConnectionString(appName))
                MyCommand = New SqlCommand(ExecSQL, MyConnection)

                MyCommand.Connection.Open()
                MyCommand.CommandTimeout = 300          '���ڴ��������ı�Ĭ��30�벻��
                IntReturn = MyCommand.ExecuteNonQuery()
                MyConnection.Close()

            End If
        End Sub

#Region "�쳣��־��¼�ӿڷ���(�²� 2012/7/31 ��ֲ������)"

        ''' <summary>
        ''' ��¼�쳣��Ϣ
        ''' </summary>
        ''' <param name="ex"> �쳣����</param>
        Public Shared Sub LogException(ByVal ex As Exception)
            AllSceneDoLogException(Nothing, "", ex, HttpContext.Current)
        End Sub

        ''' <summary>
        ''' ��¼�쳣��Ϣ
        ''' </summary>
        ''' <param name="message">��־��Ϣ(���磺XXXģ���쳣)</param>
        ''' <param name="ex">�쳣����</param>
        Public Shared Sub LogException(ByVal message As String, ByVal ex As Exception)
            AllSceneDoLogException(message, "", ex, HttpContext.Current)
        End Sub

        ''' <summary>
        ''' ר�Ÿ�httpModule�м�¼��־��ƵĽӿ�
        ''' </summary>
        ''' <param name="ex">�쳣����</param>
        ''' <param name="context">The context.</param>
        Public Shared Sub LogExceptionInHttpModule(ByVal ex As Exception, ByVal context As HttpContext)
            AllSceneDoLogException(Nothing, "δ����", ex, context)
        End Sub

        ''' <summary>
        ''' ר�Ÿ�httpModule�м�¼��־��ƵĽӿ�
        ''' </summary>
        ''' <param name="message">��־��Ϣ(���磺XXXģ���쳣)</param>
        ''' <param name="ex">�쳣����</param>
        ''' <param name="context">The context.</param>
        Public Shared Sub LogExceptionInHttpModule(ByVal message As String, ByVal ex As Exception, ByVal context As HttpContext)
            AllSceneDoLogException(message, "δ����", ex, context)
        End Sub

        ''' <summary>
        ''' ���ֳ����¼�¼�쳣����־�ӿڣ��ع������������⹫��
        ''' </summary>
        ''' <param name="message">>��־��Ϣ(���磺XXXģ���쳣)</param>
        ''' <param name="preExceptionMessage">�쳣��Ϣǰ׺</param>
        ''' <param name="ex">�쳣����</param>
        ''' <param name="context">http�����Ķ���</param>
        Private Shared Sub AllSceneDoLogException(ByVal message As String, ByVal preExceptionMessage As String, ByVal ex As Exception, ByVal context As HttpContext)

            ''-------------------------------------------
            Dim SqlConn As SqlConnection
            Dim SqlComm As SqlCommand
            Dim Sql As String
            Dim Url As String
            Dim exTypeName As String, excMessage As String
            Const sqlExc As String = "sqlexception"
            Const jsExc As String = "javascriptexception"

            exTypeName = ex.GetType().Name().ToLower()

            Select Case exTypeName
                Case sqlExc
                    excMessage = ExceptionHelper.GetSqlExceptionDesc(ex)
                Case jsExc
                    excMessage = ExceptionHelper.GetJsExceptionDesc(ex)
                    Url = CType(ex, JavaScriptException).Url
                Case Else
                    excMessage = ex.Message
            End Select

            '2011-11-16 by SunF����¼�ͻ���ID���ͻ������ơ��ỰID���������ٶ�λ�쳣����
            Dim strClientIP, strClientHostName, strSessionID As String

            Try
                strSessionID = context.Session.SessionID
                strClientIP = context.Request.UserHostAddress
                'strClientHostName = System.Net.Dns.GetHostByAddress(context.Request.UserHostAddress).HostName
                '���ϴ��������������,����ʹ�����·���ʱ,�ھ�������ֻ�ܷ���ip,�����ܷ���������
                'strClientHostName = context.Request.UserHostName
                strClientHostName = String.Empty
            Catch ex1 As Exception
                'NOTE�����ε���¼��־�����п��ܳ��ֵ��쳣����¼��־���쳣��Ӧ��Ӱ��ͻ����ڴ����ҵ��
            End Try

            ''-------------------------------------------
            ''ʹ�ò�������ѯ������sqlע��   -- url���ֶδ���sqlע��Ŀ����� 
            Sql = "INSERT INTO myExceptionLog (" & _
                                            "LogId " & _
                                            ",Date" & _
                                            ",Message" & _
                                            ",ExceptionMassage" & _
                                            ",Type" & _
                                            ",ExceptionNumber" & _
                                            ",Source" & _
                                            " ,Host" & _
                                            " ,Url" & _
                                            "  ,[User]" & _
                                             "  ,UserGUID" & _
                                            ",ClientIP" & _
                                            ",ClientHostName" & _
                                            ",SessionID" & _
                                            ",Exception)" & _
                                            " VALUES (" & _
                                                        "newid()" & _
                                            ",@Date" & _
                                           " ,@Message" & _
                                            ",@ExceptionMassage" & _
                                            ",@Type" & _
                                           " ,@ExceptionNumber" & _
                                           " ,@Source" & _
                                            ",@Host" & _
                                           " ,@Url" & _
                                            ",@User" & _
                                            ",@UserGUID" & _
                                            ",@ClientIP" & _
                                            ",@ClientHostName" & _
                                            ",@SessionID" & _
                                            ",@Exception" & _
                                        ")"

            SqlConn = New SqlConnection(GetSqlConnectionString(_DefaultAppName))
            SqlComm = New SqlCommand(Sql, SqlConn)

            Dim myExceptionLog As New ExceptionLog

            myExceptionLog.LogDate = DateTime.Now
            SqlComm.Parameters.AddWithValue("@Date", myExceptionLog.LogDate.ToString())

            If message Is Nothing Then
                myExceptionLog.Message = String.Empty
                SqlComm.Parameters.AddWithValue("@Message", DBNull.Value)

            Else
                myExceptionLog.Message = message.Replace("'", "''")
                SqlComm.Parameters.AddWithValue("@Message", message)
            End If

            myExceptionLog.ExceptionMessage = excMessage.Replace("'", "''")
            SqlComm.Parameters.AddWithValue("@ExceptionMassage", excMessage)

            myExceptionLog.Type = ex.GetType().FullName
            SqlComm.Parameters.AddWithValue("@Type", myExceptionLog.Type)

            '�쳣�š� ��exΪSqlException����ʱ����������ԡ�
            If exTypeName = sqlExc Then
                myExceptionLog.ExceptionNumber = CType(ex, SqlException).Number
                SqlComm.Parameters.AddWithValue("@ExceptionNumber", myExceptionLog.ExceptionNumber)
            Else
                myExceptionLog.ExceptionNumber = DBNull.Value.ToString
                SqlComm.Parameters.AddWithValue("@ExceptionNumber", DBNull.Value)
            End If

            If ex.Source Is Nothing Then
                myExceptionLog.Source = DBNull.Value.ToString
                SqlComm.Parameters.AddWithValue("@Source", DBNull.Value)
            Else
                myExceptionLog.Source = ex.Source
                SqlComm.Parameters.AddWithValue("@Source", ex.Source)
            End If

            myExceptionLog.Host = Environment.MachineName
            SqlComm.Parameters.AddWithValue("@Host", Environment.MachineName)


            ''��Ϊ����session�Ļ��������쳣�����˵�ǲ�ȷ���ģ�������Ҫ�ж�Session��Session("username")�Ƿ�Ϊnothing
            myExceptionLog.UserName = If(context IsNot Nothing AndAlso context.Session IsNot Nothing AndAlso context.Session("username") IsNot Nothing, context.Session("username").ToString, DBNull.Value.ToString)
            SqlComm.Parameters.AddWithValue("@User", myExceptionLog.UserName)

            myExceptionLog.UserGUID = If(context.Session IsNot Nothing AndAlso context.Session("UserGUID") IsNot Nothing, context.Session("UserGUID").ToString, DBNull.Value.ToString)
            SqlComm.Parameters.AddWithValue("@UserGUID", myExceptionLog.UserGUID)

            If exTypeName = jsExc Then
                myExceptionLog.Url = Url
                SqlComm.Parameters.AddWithValue("@Url", Url)
            Else
                myExceptionLog.Url = If(context IsNot Nothing, context.Request.Url.ToString(), Url)
                SqlComm.Parameters.AddWithValue("@Url", myExceptionLog.Url)
            End If

            If strClientIP Is Nothing Then
                myExceptionLog.ClientIP = DBNull.Value.ToString
                SqlComm.Parameters.AddWithValue("@ClientIP", DBNull.Value)
            Else
                myExceptionLog.ClientIP = strClientIP
                SqlComm.Parameters.AddWithValue("@ClientIP", strClientIP)
            End If

            If strClientHostName Is Nothing Then
                myExceptionLog.ClientHostName = DBNull.Value.ToString
                SqlComm.Parameters.AddWithValue("@ClientHostName", DBNull.Value)
            Else
                myExceptionLog.ClientHostName = strClientHostName
                SqlComm.Parameters.AddWithValue("@ClientHostName", strClientHostName)
            End If

            If strSessionID Is Nothing Then
                myExceptionLog.SessionID = DBNull.Value.ToString
                SqlComm.Parameters.AddWithValue("@SessionID", DBNull.Value)
            Else
                myExceptionLog.SessionID = strSessionID
                SqlComm.Parameters.AddWithValue("@SessionID", strSessionID)
            End If

            If ex.StackTrace() Is Nothing Then
                myExceptionLog.Exception = DBNull.Value.ToString
                SqlComm.Parameters.AddWithValue("@Exception", DBNull.Value)
            Else
                myExceptionLog.Exception = ExceptionToString(preExceptionMessage, ex)
                SqlComm.Parameters.AddWithValue("@Exception", myExceptionLog.Exception)
            End If

            Try
                SqlConn.Open()
                SqlComm.ExecuteNonQuery()
            Catch exception As Exception
                RecordExcetionLogToTextFile(myExceptionLog)
            Finally
                SqlConn.Close()
            End Try
        End Sub

        <Serializable()> _
 Private Class ExceptionLog
            Public LogDate As DateTime
            Public Message As String
            Public ExceptionMessage As String
            Public Type As String
            Public ExceptionNumber As String
            Public Source As String
            Public Host As String
            Public Url As String
            Public UserGUID As String
            Public UserName As String
            Public Exception As String
            Public ClientHostName As String
            Public ClientIP As String
            Public SessionID As String
        End Class

        Private Shared Function ExceptionToString(ByVal preMessage As String, ByVal ex As Exception) As String
            ''-------------------------------------------
            Dim builder As New StringBuilder(500) ''��ʼֵ���ô�һ�㣬���⻺����������copy&write
            Dim str As String = preMessage
            Dim exception As Exception = ex
            ''-------------------------------------------
            ''��������ڲ��쳣�����
            While exception IsNot Nothing
                builder.AppendFormat("{0}�쳣: {1}", str, exception.GetType().FullName)
                If exception.Message.Length > 0 Then
                    builder.AppendFormat(": {0}", exception.Message.Replace("'", "''"))
                End If
                builder.Append(Environment.NewLine)
                builder.Append(exception.StackTrace)
                builder.Append(Environment.NewLine)
                str = "Ƕ��"
                exception = exception.InnerException
            End While
            Return builder.ToString() & GetCallerStackTrace()
        End Function

        '��ȡ����ջ���ӹ���ʱ������ֲ
        Private Shared Function GetCallerStackTrace() As String
            Dim sbStackTrace As New StringBuilder(500)

            Dim st As New StackTrace

            Dim intFrameCount = st.FrameCount
            Dim i As Integer

            Dim sf As StackFrame
            Dim mb As MethodBase
            For i = 5 To intFrameCount - 1
                sf = st.GetFrame(i)
                mb = sf.GetMethod()
                If mb.DeclaringType.Namespace.ToLower().StartsWith("system") = True Then
                    Exit For
                End If
                sbStackTrace.AppendFormat("{0}   at {1}.{2}()", System.Environment.NewLine, mb.DeclaringType.FullName, mb.Name)
            Next

            Return sbStackTrace.ToString()

        End Function

        '''���쳣��־��¼���ı��ļ���
        Private Shared Sub RecordExcetionLogToTextFile(ByVal myExceptionLog As ExceptionLog)
            If Not HttpContext.Current Is Nothing Then
                Dim strLogPath As String = String.Format("/TempFiles/ExceptionLog_{0}.log", Guid.NewGuid.ToString)
                Mysoft.Map.Utility.GeneralBase.Serialize(Of ExceptionLog)(myExceptionLog, HttpContext.Current.Server.MapPath(strLogPath))
            End If
        End Sub

        '''���쳣��־�ı��ļ����ݼ�¼�����ݿ���
        Public Shared Sub RecordExceptionLogToDB()
            Dim folder As String = "/TempFiles"
            Dim di As DirectoryInfo
            di = New DirectoryInfo(HttpContext.Current.Server.MapPath(folder))
            If di.Exists() Then
                Dim myExptionLogs As New Hashtable
                Dim myExceptionLog As ExceptionLog
                Dim strLogPath As String = String.Empty

                For Each fi As FileInfo In di.GetFiles
                    If fi.Extension.Equals(".log") Then
                        strLogPath = Path.Combine("/TempFiles", fi.Name)
                        myExceptionLog = Mysoft.Map.Utility.GeneralBase.Deserialize(Of ExceptionLog)(HttpContext.Current.Server.MapPath(strLogPath))
                        myExptionLogs.Add(strLogPath, myExceptionLog)
                    End If
                Next

                If Not myExptionLogs Is Nothing AndAlso myExptionLogs.Count > 0 Then
                    RecordExceptionLogToDB(myExptionLogs)
                End If
            End If
        End Sub

        '''���쳣��־�ı��ļ����ݼ�¼�����ݿ���
        Private Shared Sub RecordExceptionLogToDB(ByVal myExptionLogs As Hashtable)
            Dim sqlSB As New StringBuilder
            Dim insertSQL As String = "INSERT INTO myExceptionLog([LogId],[Date],[Message],[ExceptionMassage],[Type],[ExceptionNumber],[Source],[Host],[Url],[User],[Exception],[ClientHostName],[ClientIP],[SessionID],[UserGUID]) VALUES " & _
            "(dbo.seqnewid(),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}');"
            Dim myExceptionLog As ExceptionLog
            For Each logPath As String In myExptionLogs.Keys
                myExceptionLog = myExptionLogs(logPath)
                sqlSB.Append(String.Format(insertSQL, myExceptionLog.LogDate.ToString, myExceptionLog.Message, myExceptionLog.ExceptionMessage, myExceptionLog.Type, myExceptionLog.ExceptionNumber, myExceptionLog.Source, myExceptionLog.Host, myExceptionLog.Url, myExceptionLog.UserName, myExceptionLog.Exception, myExceptionLog.ClientHostName, myExceptionLog.ClientIP, myExceptionLog.SessionID, myExceptionLog.UserGUID))
            Next
            If ExecSQL(sqlSB.ToString) > -1 Then
                DeleteExceptionLog(myExptionLogs)
            End If
        End Sub

        '''ɾ���쳣��־�ı��ļ�
        Private Shared Sub DeleteExceptionLog(ByVal myExptionLogs As Hashtable)
            Dim strLogPath As String = String.Empty

            For Each logPath As String In myExptionLogs.Keys
                Try
                    strLogPath = HttpContext.Current.Server.MapPath(logPath)
                    If File.Exists(strLogPath) Then
                        File.Delete(strLogPath)
                    End If
                Catch appendException1 As Exception
                End Try
            Next
        End Sub

#End Region


        ''' <summary>
        ''' ����appGridTree�ؼ�����ļ���ѡ������
        ''' </summary>
        ''' <param name="intLevel">����</param>
        ''' <returns></returns>
        Public Shared Function GetLeveHtml(ByVal intLevel As Integer) As String
            Dim i As Integer
            Dim strHtml As String = ""

            '���ɡ����������˰�ť����������Ӧ��ť��ִ�г���
            For i = 1 To intLevel
                strHtml = strHtml & "<a href='#' style='text-decoration: none' onclick='appGridTree.showLevel(" & i & ")'>&nbsp;" & i & "&nbsp;</a>"
            Next
            '���ء��������ˡ���ť���ɵ� HTML ���
            Return strHtml
        End Function

        ''' <summary>
        ''' ��ȡ��ǰ���ݿ�汾
        ''' </summary>
        ''' <returns>���ݿ�汾�����磺<c>2000</c> <c>2005</c> <c>7</c></returns>
        Public Shared Function getDBVersion() As String
            Return getDBVersion(_DefaultAppName)
        End Function

        ''' <summary>
        ''' ��ȡָ��Ӧ�ó����������ݿ�汾
        ''' </summary>
        ''' <param name="appName">Ӧ�ó�������Ĭ�� Default���μ���Ȩ�ļ���/bin/License.xml���� dbconns/dbconn �ڵ�� appname ���ԡ�</param>
        ''' <returns></returns>
        Public Shared Function getDBVersion(ByVal appName As String) As String
            If appName.Trim() = "" Then 'modified by chenyong 2010-01-26 ��ֹ���ַ�����������
                appName = "Default"
            End If
            Dim sVer As String = MyCache.Get("DBVersion_" & appName) '�ӻ����л�ȡ modified by chenyong 2010-02-25 �����ݿ�֧�֣�����Ӧ�÷ֿ�
            If String.IsNullOrEmpty(sVer) Then '��������ڣ�������ݿ��ȡ
                Dim sSql As String
                'modified by chenyong 2010-02-25 ֧��2008
                sSql = "SELECT CASE WHEN CONVERT(varchar(10), SERVERPROPERTY('productversion')) LIKE '8.%' THEN '2000' " & _
                        " WHEN CONVERT(varchar(10), SERVERPROPERTY('productversion')) LIKE '9.%' THEN '2005' " & _
                        " WHEN CONVERT(varchar(10), SERVERPROPERTY('productversion')) LIKE '10.%' THEN '2008' " & _
                        " ELSE '7' END AS version"

                sVer = GetDataItemString(sSql, appName)

                '���汾�ű�����������
                MyCache.Insert("DBVersion_" & appName, sVer)
            End If
            Return sVer
        End Function


    End Class


End Namespace
