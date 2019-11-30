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
    ''' 封装数据库操作类，提供读取数据链接字符串、数据库操作相关函数。
    ''' </summary>
    Public Class MyDB
        Private Shared _DefaultAppName As String = "Default"

        '***************************  Public 函数 *****************************

        ''' <summary>
        ''' 获取 SQL 数据库连接字符串。
        ''' </summary>
        ''' <returns>数据库连接字符串，如：server=netserver,1433;uid=sa;pwd=12345678;database=dotnet_erp25。如果注册表没有指定数据库端口号，默认 1433。 </returns>
        Public Shared Function GetSqlConnectionString() As String
            Return GetSqlConnectionString(_DefaultAppName)
        End Function

        ''' <summary>
        ''' 获取指定的应用程序名对应的 SQL 数据库连接字符串。
        ''' </summary>
        ''' <param name="appName">应用程序名</param>
        ''' <returns>数据库连接字符串，如：server=netserver,1433;uid=sa;pwd=12345678;database=dotnet_erp25。如果注册表没有指定数据库端口号，默认 1433。 </returns>
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

                If MyDBConn.ServerProt = "1433" Then        ' 有些防火墙会禁用1433端口，为了用于兼容之前的模式做此判断
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
        ''' 执行SQL查询，并获取 DataTable 类型结果。 DataTable 名称为<b>_tb</b>。
        ''' </summary>
        ''' <param name="sql">SQL 语句字符串。 </param>
        ''' <returns>DataTable 实例，DataTable 名称为<b>_tb</b> </returns>
        Public Shared Function GetDataTable(ByVal sql As String) As DataTable
            Return GetDataTable(sql, "_tb", _DefaultAppName)
        End Function

        ''' <summary>
        ''' 执行SQL查询，并获取 DataTable 类型结果。指定 DataTable 名称。 
        ''' </summary>
        ''' <param name="sql">SQL 语句字符串。</param>
        ''' <param name="tableName">DataTable 名称。 </param>
        ''' <returns>DataTable 实例，DataTable 名称为<paramref name="tableName"></paramref>的值</returns>
        Public Shared Function GetDataTable(ByVal sql As String, ByVal tableName As String) As DataTable
            Return GetDataTable(sql, tableName, _DefaultAppName)
        End Function

        ''' <summary>
        ''' 在指定的应用程序连接上执行SQL查询，并获取 DataTable 类型结果。 指定 DataTable 名称。 
        ''' </summary>
        ''' <param name="sql">SQL 语句字符串。</param>
        ''' <param name="tableName">DataTable 名称。 </param>
        ''' <param name="appName">应用程序名，默认 Default。参见授权文件（/bin/License.xml）中 dbconns/dbconn 节点的 appname 属性。 </param>
        ''' <returns>DataTable 实例，DataTable 名称为<paramref name="tableName"></paramref>的值</returns>
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
                ' Modified by chenyong 2010-02-25   日志管理项目代码移植
                MyDB.LogException("sql[appname:" + appName + "]:" & sql & ";tableName:" & tableName, ex)
                '------------------------------------------------------------------------------
                Return New DataTable()
            Finally
                SqlConn.Close()
            End Try

        End Function

        ''' <summary>
        ''' 执行 Transact-SQL 语句命令。
        ''' </summary>
        ''' <param name="sql">SQL 语句字符串。 </param>
        ''' <returns>对于 UPDATE、INSERT 和 DELETE 语句，返回值为该命令所影响的行数<br/>
        ''' 其他类型的语句，返回值为 0<br/>
        ''' SQL 执行失败、写数据库操作日志失败、执行时间超过 30 秒，返回值为 -1。</returns>
        ''' <remarks>不建议使用除 INSERT、DELELE、UPDATE 语句命令之外的 SQL 语句！因为可能无法确定返回值的含义！<br/>
        ''' <b>有关记录数据库操作日志。</b><br/>
        ''' 执行该函数时会将表的 INSERT、UPDATE、DELTE 查询语句记录在 mySQLLog 表中，需要在 web.config 文件的<c> &lt;appSetting&gt;&lt;add key="TraceTable" value="表1,表2" /&gt;&lt;/appSetting&gt;</c> 配置要跟踪的表。<br/>  
        ''' 如果 SQL 执行失败会将错误信息记录在数据库操作日志中。 <br/>
        ''' </remarks>
        Public Shared Function ExecSQL(ByVal sql As String, ByVal recordLog As Boolean) As Integer
            Return ExecSQL(sql, _DefaultAppName, recordLog)
        End Function

        Public Shared Function ExecSQL(ByVal sql As String) As Integer
            Return ExecSQL(sql, _DefaultAppName, True)
        End Function

        ''' <summary>
        ''' 在指定的应用程序连接上执行 Transact-SQL 语句命令。
        ''' </summary>
        ''' <param name="sql">SQL 语句字符串。</param>
        ''' <param name="appName">应用程序名。参见授权文件（/bin/License.xml）中 dbconns/dbconn 节点的 appname 属性。 </param>
        ''' <returns>对于 UPDATE、INSERT 和 DELETE 语句，返回值为该命令所影响的行数<br/>
        ''' 其他类型的语句，返回值为 0<br/>
        ''' SQL 执行失败、写数据库操作日志失败、执行时间超过 30 秒，返回值为 -1。
        ''' </returns>
        ''' <remarks>不建议使用除 INSERT、DELELE、UPDATE 语句命令之外的 SQL 语句！因为可能无法确定返回值的含义！<br/>
        ''' <b>有关记录数据库操作日志。</b><br/>
        ''' 执行该函数时会将表的 INSERT、UPDATE、DELTE 查询语句记录在 mySQLLog 表中，需要在 web.config 文件的<c> &lt;appSetting&gt;&lt;add key="TraceTable" value="表1,表2" /&gt;&lt;/appSetting&gt;</c> 配置要跟踪的表。<br/>  
        ''' 如果 SQL 执行失败会将错误信息记录在数据库操作日志中。 <br/>
        ''' </remarks>
        Public Shared Function ExecSQL(ByVal sql As String, ByVal appName As String, ByVal recordLog As Boolean) As Integer
            If String.IsNullOrEmpty(sql) Then Return 0
            Dim SqlConn As SqlConnection
            Dim SqlComm As SqlCommand
            Dim RowsCount As Integer

            SqlConn = New SqlConnection(GetSqlConnectionString(appName))
            SqlComm = New SqlCommand(sql, SqlConn)
            SqlComm.CommandTimeout = 300          '对于大数据量的表，默认30秒不够
            Try
                SqlConn.Open()
                RowsCount = SqlComm.ExecuteNonQuery()

                If RowsCount = -1 Then      ' 当 SQL 为非 INSERT、UPDATE、DELETE 语句时，返回为 -1
                    RowsCount = 0
                End If

                If recordLog Then
                    If Not HttpContext.Current Is Nothing Then
                        ' 记录 SQL 日志
                        WriteSQLLog(sql, appName)
                    End If
                End If
                Return RowsCount
            Catch ex As System.Exception
                ' 记录 SQL 错误日志
                'WriteSQLLog(sql, appName, ex.Message.ToString)

                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   日志管理项目代码移植                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Return -1
            Finally
                SqlConn.Close()
            End Try

        End Function

        ' 功能：执行sql语句
        ' 参数：sql语句
        ' 返回：影响记录数。否则直接抛出异常
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
            SqlComm.CommandTimeout = 300          '对于大数据量的表，默认30秒不够
            Try
                SqlConn.Open()
                RowsCount = SqlComm.ExecuteNonQuery()

                If RowsCount = -1 Then      ' 当 SQL 为非 INSERT、UPDATE、DELETE 语句时，返回为 -1
                    RowsCount = 0
                End If

                ' 记录 SQL 日志
                WriteSQLLog(sql, appName)
                Return RowsCount
            Catch ex As System.Exception
                ' 记录 SQL 错误日志
                'WriteSQLLog(sql, appName, ex.Message.ToString)
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   日志管理项目代码移植                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Return -1
            Finally
                SqlConn.Close()
            End Try
        End Function

        '功能：执行SQL语句，并返回ID值
        '参数：sql语句
        '返回：id
        Public Shared Function ExecSQL_Id(ByVal sql As String) As Int64
            Return ExecSQL_Id(sql, _DefaultAppName)
        End Function

        Public Shared Function ExecSQL_Id(ByVal sql As String, ByVal appName As String) As Int64
            Dim SqlConn As SqlConnection
            Dim SqlComm As SqlCommand
            Dim RowsCount As Integer

            SqlConn = New SqlConnection(GetSqlConnectionString(appName))
            SqlComm = New SqlCommand(sql & ";SELECT @@IDENTITY", SqlConn)
            SqlComm.CommandTimeout = 300          '对于大数据量的表，默认30秒不够
            Try
                SqlConn.Open()
                RowsCount = Convert.ToInt64(SqlComm.ExecuteScalar())

                ' 记录 SQL 日志
                WriteSQLLog(sql, appName)
                Return RowsCount
            Catch ex As System.Exception
                ' 记录 SQL 错误日志
                'WriteSQLLog(sql, appName, ex.Message.ToString)

                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   日志管理项目代码移植                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Return -1
            Finally
                SqlConn.Close()
            End Try
        End Function

        '功能：获取sql查询的记录数
        '参数：sql
        '返回：记录数
        Public Shared Function GetRowsCount(ByVal sql As String) As Integer
            Return GetRowsCount(sql, _DefaultAppName)
        End Function

        Public Shared Function GetRowsCount(ByVal sql As String, ByVal appName As String) As Integer
            Dim DT As DataTable

            DT = GetDataTable(sql, "temp")
            Return DT.Rows.Count

        End Function

        '功能：通过sql语句获得单个值（字符型）
        '参数：sql语句
        '返回：字符型，NULL为空串
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
                ' Modified by chenyong 2010-02-25   日志管理项目代码移植                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Throw ex
            Finally
                MyConnection.Close()
            End Try
        End Function

        '功能：通过sql语句获得单个值（整型）
        '参数：sql语句
        '返回：整型，NULL为0
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
                ' Modified by chenyong 2010-02-25   日志管理项目代码移植                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Throw ex
            Finally
                MyConnection.Close()
            End Try
        End Function

        '功能：通过sql语句获得单个值（货币型）
        '参数：sql语句
        '返回：货币型，NULL为0
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
                ' Modified by chenyong 2010-02-25   日志管理项目代码移植                 
                MyDB.LogException("sql[appname:" + appName + "]:" & sql, ex)
                '------------------------------------------------------------------------------
                Throw ex
            Finally
                MyConnection.Close()
            End Try
        End Function

        '功能：通过sql语句获得单个值（日期型）
        '参数：sql语句
        '返回：日期型，NULL为1900-01-01
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
                ' Modified by chenyong 2010-02-25   日志管理项目代码移植                 
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

        ' 参数：invokeType     -- 调用函数的类型。
        '       有时用户想直接保存 DataXml 可以调用该函数，使用 XMLHTTP 方式通过 ApplicationMap.aspx.vb 调用的函数必须包含两个参数，
        '       所以增加了一个辅助参数，判断是否是通过 XMLHTTP 调用的
        '       keyValue       -- 新增记录时，使用之前产生的 guid。修改记录时该值无效。
        '返回：执行成功，返回主键 GUID 或 <xml result="true" ... 字符串；执行失败，返回 "" 或 <xml result="false" ... 字符串。
        Public Shared Function SaveXml(ByVal dataXml As String, ByVal invokeType As String, ByVal keyValue As String) As String
            Dim xmlDoc As New XmlDocument
            Dim keyValueOfXML As String
            Dim SQL As String

            '-----------------------------------------------------------------------------------
            'Modified by chenyong 2010-02-25   日志管理项目控件多数据源代码移植       
            Dim _AppName As String = "Default"

            Try
                xmlDoc.LoadXml(dataXml)

                If Not xmlDoc.DocumentElement.Attributes.GetNamedItem("keyvalue") Is Nothing Then
                    keyValueOfXML = xmlDoc.DocumentElement.Attributes.GetNamedItem("keyvalue").Value
                Else
                    keyValueOfXML = ""
                End If

                '-----------------------------------------------------------------------------------
                ''Modified by chenyong 2010-02-25   日志管理项目控件多数据源代码移植       
                'AppName
                Dim appNameXmlNode As XmlNode = xmlDoc.DocumentElement.Attributes.GetNamedItem("appname")
                If Not appNameXmlNode Is Nothing AndAlso Not String.IsNullOrEmpty(appNameXmlNode.Value) Then
                    _AppName = appNameXmlNode.Value
                End If

                If keyValueOfXML = "" Then      ' 新增
                    SQL = getInserSqlByDataXml(dataXml, keyValue)

                Else      ' 修改
                    keyValue = keyValueOfXML    ' 不可删除，返回值用
                    SQL = getUpdateSqlByDataXml(dataXml)

                End If

                'Modified by chenyong 2010-02-25   日志管理项目控件多数据源代码移植       
                If ExecSQL(SQL, _AppName) = -1 Then       ' 失败
                    If invokeType = "NO XMLHTTP" Then
                        Return ""
                    Else
                        Return "<xml result=""false"" errormessage=""运行时异常，请与管理员联系！""></xml>"
                    End If

                Else        ' 成功
                    If invokeType = "NO XMLHTTP" Then
                        Return keyValue
                    Else
                        Return "<xml result=""true"" keyvalue=""" & keyValue & """></xml>"
                    End If

                End If

            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   日志管理项目代码移植                     
                MyDB.LogException("dataXml:" & dataXml & ";invokeType:" & invokeType, ex)
                '------------------------------------------------------------------------------

                If invokeType = "NO XMLHTTP" Then
                    Return ""
                Else
                    Return "<xml result=""false"" errormessage=""运行时异常，请与管理员联系！""></xml>"
                End If
            End Try

        End Function


        Public Shared Function GetSqlByDataXml(ByVal dataXml As String) As String
            Return SaveXml(dataXml, "NO XMLHTTP")
        End Function

        ' 功能：根据 DataXml 返回一个 SQL 字符串
        ' 参数：invokeType      -- 参见 SaveXml
        ' 返回：如果通过 XmlHTTP 方式调用将返回格式如：<xml result="true/false" keyvalue="xxx-xxxxx..." sql="SQL字符串"></xml>。否则，返回一个 SQL 字符串。
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

                If KeyValue = "" Then      ' 新增
                    KeyValue = Guid.NewGuid.ToString
                    SQL = getInserSqlByDataXml(dataXml, KeyValue)

                Else      ' 修改
                    SQL = getUpdateSqlByDataXml(dataXml)

                End If

                If invokeType = "NO XMLHTTP" Then
                    Return SQL
                Else
                    Return "<xml result=""true"" keyvalue=""" & KeyValue & """ sql=""" & SQL & """></xml>"
                End If

            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                '  Modified by chenyong 2010-02-25   日志管理项目代码移植                  
                MyDB.LogException("dataXml:" & dataXml & ";invokeType:" & invokeType, ex)
                '------------------------------------------------------------------------------
                If invokeType = "NO XMLHTTP" Then
                    Return ""
                Else
                    Return "<xml result=""false"" errormessage=""运行时异常，请与管理员联系！""></xml>"
                End If
            End Try

        End Function


        ' 功能：获取用户项目权限的 SQL 表达式。
        ' 参数：userGUID    -- 用户 GUID
        '       tableName   -- 拼写 SQL 表达式所使用表名
        '       buGUID      -- 公司，只过滤该公司下的项目
        '       application     -- 所属系统，由于客服存在项目共享，所以售楼和客服的项目不从同一张表取值
        '       isUserFilter  -- 用户过滤的项目是否有效，默认 true
        ' 说明：打通后的项目过滤不区分系统，在用户管理－》分配项目中对用户进行项目授权
        ' 返回：返回 SQL 表达式，如：Lead.ProjGUID in ('7AED691F-DC73-4632-949E-110A39B7B5A5','7CE7774C-16B9-4558-A271-0FB1F375BA08')
        ' 2007.7.9  zt
        ' 兼容性修改，恢复之前注释的前三个函数。buGUID 默认当前公司，application 默认 0101。
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

            UserProjects = GetProjectString(userGUID, buGUID, application, isUserFilter)     ' 取用户项目
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

        ' 说明：返回用户项目字符串
        ' 返回：项目字符串，如：'7AED691F-DC73-4632-949E-110A39B7B5A5','7CE7774C-16B9-4558-A271-0FB1F375BA08'
        '       如果没有项目返回空字符串""
        ' 2007.7.9  zt
        ' 兼容性修改，恢复之前注释的前三个函数。buGUID 默认当前公司，application 默认 0101。
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
            ' 影响缓存的地方：
            ' 1、授权；
            ' 2、新增、删除项目；
            ' 3、用户自己过滤项目；
            ' 4、修改项目在哪些系统可用；
            ' 5、修改 myApplication 表中设置系统“是否启用项目子系统过滤”；
            ' 6、修改授权对象 XML 定义（usp_myGetDataRightsXML）。

            If userGUID Is Nothing Then
                Return ""
            End If

            ' 先从 Cache 中取
            Dim oUserProjectSec As Object
            Dim strUserProjectSec, strPara As String      ' strPara：参数字符串，如果传入的参数变化，需要刷新缓存
            Dim intTag As Integer
            Dim CACHEEXP As Integer = MyCache.DayFactor * 7            ' 缓存过期时间为一周

            strPara = userGUID & "," & buGUID & "," & application & "," & isUserFilter
            oUserProjectSec = MyCache.Get("ProjectString_" & userGUID)
            If Not oUserProjectSec Is Nothing Then
                strUserProjectSec = oUserProjectSec
                intTag = InStr(strUserProjectSec, "|")

                ' 判断参数是否变化
                If strPara = Left(strUserProjectSec, intTag - 1) Then
                    Return Mid(strUserProjectSec, intTag + 1)
                End If
            End If

            ' 如果 Cache 中没有，计算获取
            Dim sbReturn As New StringBuilder
            Dim SqlConn As New SqlConnection(MyDB.GetSqlConnectionString)
            Dim dt As New DataTable

            Try
                '建立连接	    
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

                '获取数据
                SqlConn.Open()
                SqlAdpt = New SqlDataAdapter
                SqlAdpt.SelectCommand = SqlComm
                SqlAdpt.Fill(dt)

                ' 返回用户有权限的项目字符串
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
                '   Modified by chenyong 2010-02-25   日志管理项目代码移植                     
                MyDB.LogException(ex)
                '------------------------------------------------------------------------------
                'MyCache.Insert("ProjectString_" & userGUID, strPara & "|", CACHEEXP)
                MyCache.Remove("ProjectString_" & userGUID)
                Return ""
            Finally
                '关闭连接
                SqlConn.Close()
            End Try

        End Function


        ' 功能：根据用户所在项目团队中的级别，获取上下级过滤的 SQL 表达式。
        ' 参数：userGUID    -- 用户 GUID
        '       tableName   -- 拼写 SQL 表达式所使用表名
        '       buGUID      -- 公司，只过滤该公司下的项目
        '       a_UserGUIDFieldname     -- 业务表中用哪个字段记录用户GUID，默认UserGUID
        ' 返回：返回 SQL 表达式，如：Lead.ProjGUID in ('7AED691F-DC73-4632-949E-110A39B7B5A5','7CE7774C-16B9-4558-A271-0FB1F375BA08')。没有项目权限返回(1=2)
        ' 注意：售楼用
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
            ' 2007.1.12 讨论结果（海峰、向前）
            ' 1、系统管理员不过滤，返回 3=3
            ' 2、项目经理返回 ProjGUID = 'xxx-xxx-'
            ' 3、其他返回 (StationHierarchyCode Like 'xxx.%')
            ' 这样用户岗位的其它用户的资源也取出来了，有问题。应该是：用 Like 取下级，再叠加自己的资源。2007.2.27 国喜，向前
            ' 如：((StationHierarchyCode Like 'xxx.%') OR a_UserGUIDFieldname=UserGUID)
            If userGUID = "" Then
                userGUID = HttpContext.Current.Session("UserGUID").ToString
            End If

            If Mysoft.Map.Application.Security.User.IsAdmin(userGUID) Then      ' 如果系统管理员 zt 2007.1.15
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

            '获取用户所在的岗位
            strSQL = "SELECT s.StationGUID,s.HierarchyCode,ISNULL(s.IsProjManager,0) AS IsProjManager,s.ProjGUID" & _
                     " FROM myStation AS s" & _
                     " INNER JOIN myStationUser AS su ON s.StationGUID = su.StationGUID" & _
                     " WHERE su.UserGUID = '" & userGUID.Replace("'", "''") & "'"
            dtStation = MyDB.GetDataTable(strSQL)

            If dtStation.Rows.Count > 0 Then
                sbReturn.Append("(")

                ' 取本人的资源
                sbReturn.Append(tableName & a_UserGUIDFieldname & "='" & userGUID & "'")

                ' 取下级的资源
                For i = 0 To dtStation.Rows.Count - 1
                    sbReturn.Append(" OR ")

                    If dtStation.Rows(i).Item("IsProjManager") = 1 Then      '项目经理
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


        ' 功能：获取报表用的过滤表达式
        ' 参数：userGUID    -- 用户 GUID
        '       tableName   -- 拼写 SQL 表达式所使用表名
        '       application      -- 业务应用系统编码，如：0101（售楼）、0102（客服）、0103（会员） 等
        ' 说明：各业务系统报表过滤方式可配置    zt 2006-03-24
        Public Shared Function GetFilterExpOfReport(ByVal userGUID As String, ByVal tableName As String, ByVal tableAliasName As String, ByVal application As String) As String
            Dim SQL As String
            Dim ReportFilterType As String

            ReportFilterType = GetDataItemString("SELECT ReportFilterType FROM myApplication WHERE Application='" & application & "'")
            Select Case ReportFilterType
                Case "项目"
                    If GetDataItemInt("SELECT COUNT(*) FROM data_dict WHERE table_name='" & tableName & "' AND field_name='ProjGUID'") > 0 Then    ' 如果“项目”字段存在
                        Return GetProjectFilterExp(userGUID, tableAliasName, Nothing, "0101")
                    Else        ' 如果“项目”字段不存在
                        Return "(4=4)"
                    End If

                Case "虚拟项目"
                    If GetDataItemInt("SELECT COUNT(*) FROM data_dict WHERE table_name='" & tableName & "' AND field_name='ProjGUID'") > 0 Then    ' 如果“项目”字段存在
                        Return GetProjectFilterExp(userGUID, tableAliasName, Nothing, "0102")
                    Else        ' 如果“项目”字段不存在
                        Return "(4=4)"
                    End If

                Case "公司"
                    If GetDataItemInt("SELECT COUNT(*) FROM data_dict WHERE table_name='" & tableName & "' AND field_name='BUGUID'") > 0 Then     ' 如果“公司”字段存在
                        Return tableAliasName & ".BUGUID in ('" & HttpContext.Current.Session("BUGUID") & "')"
                    Else        ' 如果“公司”字段不存在
                        Return "(4=4)"
                    End If

                Case "不过滤"
                    Return "(4=4)"

                Case Else
                    Return "(4=5)"

            End Select

        End Function


        ' 功能：QueryXml 中用 api 获取 SQL 过滤条件表达式。
        ' 参数：a_XmlNodeCondition      -- queryxml 的 condition 节点，
        '       如：<condition attribute="xx.ProjGUID" operator="api" datatype="buprojectfilter" value="<公司GUID>" application="0102"/>
        ' 返回：过滤条件表达式
        ' 注意：不对外公开，只供 AppGrid 控件拼写条件时调用
        Public Shared Function getQueryString(ByVal a_Entity As String, ByVal a_XmlNodeCondition As XmlNode) As String
            Dim TableName, FieldName, Attribute, caseCode, value, application As String

            Attribute = a_XmlNodeCondition.Attributes.GetNamedItem("attribute").Value
            caseCode = a_XmlNodeCondition.Attributes.GetNamedItem("datatype").Value
            value = a_XmlNodeCondition.Attributes.GetNamedItem("value").Value
            If Not a_XmlNodeCondition.Attributes.GetNamedItem("application") Is Nothing Then
                application = a_XmlNodeCondition.Attributes.GetNamedItem("application").Value
            End If

            ' 获取表名
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

            ' 执行分支函数，获取过滤表达式
            Select Case caseCode
                Case "buprojectfilter"          ' 扩展支持公司项目过滤  zt 2006.12.31
                    Return GetProjectFilterExp(HttpContext.Current.Session("UserGUID"), TableName, value, application).Replace("ProjGUID", FieldName)

                Case "xmdj"
                    Return GetProjectFilterExp(HttpContext.Current.Session("UserGUID"), Nothing, Nothing, Nothing, False)

                Case "allprojectofslxt"
                    ' 售楼系统中返回当前公司、当前用户的所有项目过滤表达式
                    ' 由于支持前端定义字段，所以这里要将 ProjGUID 替换成前端定义的内容
                    Return GetProjectFilterExp(HttpContext.Current.Session("UserGUID"), TableName, HttpContext.Current.Session("BUGUID"), "0101").Replace("ProjGUID", FieldName)

                Case "allprojectofkfxt"
                    ' 客服系统中返回当前公司、当前用户的所有项目过滤表达式
                    ' 由于支持前端定义字段，所以这里要将 ProjGUID 替换成前端定义的内容
                    Return GetProjectFilterExp(HttpContext.Current.Session("UserGUID"), TableName, HttpContext.Current.Session("BUGUID"), "0102").Replace("ProjGUID", FieldName)

                Case "buildsofyuan"
                    ' 返回分区下楼栋过滤表达式
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


        '***************************  Private 函数，函数名的第一个字母小写，其他首写字母大写 *****************************


        ' 功能：根据 DataXml 生成 Insert SQL
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
            'Modified by chenyong 2010-02-25   日志管理项目控件多数据源代码移植       
            Dim _AppName As String = "Default"

            Try
                xmlDoc.LoadXml(dataXml)
            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   日志管理项目代码移植
                MyDB.LogException("dataXml:" & dataXml & ";keyValue:" & keyValue, ex)
                '------------------------------------------------------------------------------
                Return ""
            End Try

            '-----------------------------------------------------------------------------------
            ''Modified by chenyong 2010-02-25   日志管理项目控件多数据源代码移植       
            'AppName
            Dim appNameXmlNode As XmlNode = xmlDoc.DocumentElement.Attributes.GetNamedItem("appname")
            If Not appNameXmlNode Is Nothing AndAlso Not String.IsNullOrEmpty(appNameXmlNode.Value) Then
                _AppName = appNameXmlNode.Value
            End If


            strEntityName = xmlDoc.DocumentElement.Name
            strKeyName = xmlDoc.DocumentElement.Attributes.GetNamedItem("keyname").Value

            strSQL = "SELECT c.name AS field, t.name AS datatype FROM syscolumns c, sysobjects o, systypes t " & _
                     "WHERE c.id = o.id AND c.xtype = t.xtype AND o.name = '" & strEntityName.Replace("'", "''") & "'"
            dtFields = GetDataTable(strSQL, "fields", _AppName) 'Modified by chenyong 2010-02-25   日志管理项目控件多数据源代码移植  

            sbFields.Append(strKeyName)
            sbValues.Append("N'" & keyValue & "'")

            For i = 0 To xmlDoc.DocumentElement.ChildNodes.Count - 1
                strFieldName = xmlDoc.DocumentElement.ChildNodes(i).Name
                sbFields.Append("," & strFieldName)

                '获取字段类型
                Dim drTemp() As DataRow
                drTemp = dtFields.Select("field='" & strFieldName & "'")
                If drTemp.Length > 0 Then
                    strFieldType = drTemp(0).Item(1).ToString
                Else
                    strFieldType = ""
                End If

                ' money 和 smallmoney 类型的数据，不能从 varchar 隐式转换
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


        ' 功能：根据 DataXml 生成 Update SQL
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
            'Modified by chenyong 2010-02-25   日志管理项目控件多数据源代码移植       
            Dim _AppName As String = "Default"

            Try
                xmlDoc.LoadXml(dataXml)
            Catch ex As System.Exception
                '------------------------------------------------------------------------------
                ' Modified by chenyong 2010-02-25   日志管理项目代码移植
                MyDB.LogException("dataXml:" & dataXml, ex)
                '------------------------------------------------------------------------------
                Exit Function
            End Try

            '-----------------------------------------------------------------------------------
            ''Modified by chenyong 2010-02-25   日志管理项目控件多数据源代码移植       
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
            dtFields = GetDataTable(strSQL, "fields", _AppName) 'Modified by chenyong 2010-02-25   日志管理项目控件多数据源代码移植  

            sb.Append("UPDATE " & strEntityName & " SET ")
            For i = 0 To xmlDoc.DocumentElement.ChildNodes.Count - 1
                If i > 0 Then sb.Append(",")
                strFieldName = xmlDoc.DocumentElement.ChildNodes(i).Name

                '获取字段类型
                ' money 和 smallmoney 类型的数据，不能从 varchar 隐式转换
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


        ' 功能：将 INSERT、UPDATE、DELETE 执行 SQL 写入跟踪日志
        Private Shared Sub WriteSQLLog(ByVal sql As String)
            WriteSQLLog(sql, _DefaultAppName)
        End Sub

        Private Shared Sub WriteSQLLog(ByVal sql As String, ByVal appName As String)
            WriteSQLLog(sql, appName, Nothing)
        End Sub

        Private Shared Sub WriteSQLLog(ByVal sql As String, ByVal appName As String, ByVal errorMessage As String)
            If HttpContext.Current.Application("TraceTable") Is Nothing Then    ' 如果第一次写日志
                Dim TraceTable As String

                TraceTable = ConfigurationSettings.AppSettings("TraceTable")    ' 读节点
                If TraceTable Is Nothing Then   ' 如果节点不存在
                    HttpContext.Current.Application("TraceTable") = ""
                    Exit Sub
                Else
                    HttpContext.Current.Application("TraceTable") = Trim(TraceTable)
                End If
            End If

            If HttpContext.Current.Application("TraceTable") = "" Then     ' 如果没有定义要记录的表
                Exit Sub
            End If

            ' 判断 SQL 是否需要跟踪
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

            ' 拼写执行 SQL
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
                MyCommand.CommandTimeout = 300          '对于大数据量的表，默认30秒不够
                IntReturn = MyCommand.ExecuteNonQuery()
                MyConnection.Close()

            End If
        End Sub

#Region "异常日志记录接口方法(陈波 2012/7/31 移植、调整)"

        ''' <summary>
        ''' 记录异常信息
        ''' </summary>
        ''' <param name="ex"> 异常对象</param>
        Public Shared Sub LogException(ByVal ex As Exception)
            AllSceneDoLogException(Nothing, "", ex, HttpContext.Current)
        End Sub

        ''' <summary>
        ''' 记录异常信息
        ''' </summary>
        ''' <param name="message">日志消息(比如：XXX模块异常)</param>
        ''' <param name="ex">异常对象</param>
        Public Shared Sub LogException(ByVal message As String, ByVal ex As Exception)
            AllSceneDoLogException(message, "", ex, HttpContext.Current)
        End Sub

        ''' <summary>
        ''' 专门给httpModule中记录日志设计的接口
        ''' </summary>
        ''' <param name="ex">异常对象</param>
        ''' <param name="context">The context.</param>
        Public Shared Sub LogExceptionInHttpModule(ByVal ex As Exception, ByVal context As HttpContext)
            AllSceneDoLogException(Nothing, "未处理", ex, context)
        End Sub

        ''' <summary>
        ''' 专门给httpModule中记录日志设计的接口
        ''' </summary>
        ''' <param name="message">日志消息(比如：XXX模块异常)</param>
        ''' <param name="ex">异常对象</param>
        ''' <param name="context">The context.</param>
        Public Shared Sub LogExceptionInHttpModule(ByVal message As String, ByVal ex As Exception, ByVal context As HttpContext)
            AllSceneDoLogException(message, "未处理", ex, context)
        End Sub

        ''' <summary>
        ''' 各种场景下记录异常的日志接口，重构得来，不对外公开
        ''' </summary>
        ''' <param name="message">>日志消息(比如：XXX模块异常)</param>
        ''' <param name="preExceptionMessage">异常消息前缀</param>
        ''' <param name="ex">异常对象</param>
        ''' <param name="context">http上下文对象</param>
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

            '2011-11-16 by SunF：记录客户端ID、客户端名称、会话ID，帮助快速定位异常问题
            Dim strClientIP, strClientHostName, strSessionID As String

            Try
                strSessionID = context.Session.SessionID
                strClientIP = context.Request.UserHostAddress
                'strClientHostName = System.Net.Dns.GetHostByAddress(context.Request.UserHostAddress).HostName
                '以上代码存在性能问题,但是使用以下方法时,在局域网中只能返回ip,而不能返回主机名
                'strClientHostName = context.Request.UserHostName
                strClientHostName = String.Empty
            Catch ex1 As Exception
                'NOTE：屏蔽掉记录日志过程中可能出现的异常，记录日志的异常不应该影响客户正在处理的业务
            End Try

            ''-------------------------------------------
            ''使用参数化查询，避免sql注入   -- url等字段存在sql注入的可能性 
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

            '异常号。 当ex为SqlException类型时，有这个属性。
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


            ''因为调用session的环境对于异常组件来说是不确定的，所以需要判断Session和Session("username")是否不为nothing
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
            Dim builder As New StringBuilder(500) ''初始值设置大一点，避免缓存区填满后copy&write
            Dim str As String = preMessage
            Dim exception As Exception = ex
            ''-------------------------------------------
            ''处理存在内部异常的情况
            While exception IsNot Nothing
                builder.AppendFormat("{0}异常: {1}", str, exception.GetType().FullName)
                If exception.Message.Length > 0 Then
                    builder.AppendFormat(": {0}", exception.Message.Replace("'", "''"))
                End If
                builder.Append(Environment.NewLine)
                builder.Append(exception.StackTrace)
                builder.Append(Environment.NewLine)
                str = "嵌套"
                exception = exception.InnerException
            End While
            Return builder.ToString() & GetCallerStackTrace()
        End Function

        '获取调用栈。从广州时代中移植
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

        '''将异常日志记录到文本文件中
        Private Shared Sub RecordExcetionLogToTextFile(ByVal myExceptionLog As ExceptionLog)
            If Not HttpContext.Current Is Nothing Then
                Dim strLogPath As String = String.Format("/TempFiles/ExceptionLog_{0}.log", Guid.NewGuid.ToString)
                Mysoft.Map.Utility.GeneralBase.Serialize(Of ExceptionLog)(myExceptionLog, HttpContext.Current.Server.MapPath(strLogPath))
            End If
        End Sub

        '''将异常日志文本文件内容记录到数据库中
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

        '''将异常日志文本文件内容记录到数据库中
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

        '''删除异常日志文本文件
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
        ''' 生成appGridTree控件上面的级别选择内容
        ''' </summary>
        ''' <param name="intLevel">级别</param>
        ''' <returns></returns>
        Public Shared Function GetLeveHtml(ByVal intLevel As Integer) As String
            Dim i As Integer
            Dim strHtml As String = ""

            '生成“级数”过滤按钮，并设置相应按钮的执行程序
            For i = 1 To intLevel
                strHtml = strHtml & "<a href='#' style='text-decoration: none' onclick='appGridTree.showLevel(" & i & ")'>&nbsp;" & i & "&nbsp;</a>"
            Next
            '返回“级数过滤”按钮生成的 HTML 语句
            Return strHtml
        End Function

        ''' <summary>
        ''' 获取当前数据库版本
        ''' </summary>
        ''' <returns>数据库版本，例如：<c>2000</c> <c>2005</c> <c>7</c></returns>
        Public Shared Function getDBVersion() As String
            Return getDBVersion(_DefaultAppName)
        End Function

        ''' <summary>
        ''' 获取指定应用程序名的数据库版本
        ''' </summary>
        ''' <param name="appName">应用程序名，默认 Default。参见授权文件（/bin/License.xml）中 dbconns/dbconn 节点的 appname 属性。</param>
        ''' <returns></returns>
        Public Shared Function getDBVersion(ByVal appName As String) As String
            If appName.Trim() = "" Then 'modified by chenyong 2010-01-26 防止空字符串传入后出错
                appName = "Default"
            End If
            Dim sVer As String = MyCache.Get("DBVersion_" & appName) '从缓存中获取 modified by chenyong 2010-02-25 多数据库支持，缓存应该分开
            If String.IsNullOrEmpty(sVer) Then '如果不存在，则从数据库获取
                Dim sSql As String
                'modified by chenyong 2010-02-25 支持2008
                sSql = "SELECT CASE WHEN CONVERT(varchar(10), SERVERPROPERTY('productversion')) LIKE '8.%' THEN '2000' " & _
                        " WHEN CONVERT(varchar(10), SERVERPROPERTY('productversion')) LIKE '9.%' THEN '2005' " & _
                        " WHEN CONVERT(varchar(10), SERVERPROPERTY('productversion')) LIKE '10.%' THEN '2008' " & _
                        " ELSE '7' END AS version"

                sVer = GetDataItemString(sSql, appName)

                '将版本号保存至缓存中
                MyCache.Insert("DBVersion_" & appName, sVer)
            End If
            Return sVer
        End Function


    End Class


End Namespace
