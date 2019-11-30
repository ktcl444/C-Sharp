Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Linq
Imports System.Text


Public Class MyDataCopyController
    Public Sub New(ByVal connectionString As String)
        Me.ConnectionString = connectionString
        _mappingController = New MappingController(Me)
        BatchSize = 10000
        DataCopyTimeout = 60 * 10
    End Sub

    Public Sub New(ByVal connection As SqlConnection, ByVal transaction As SqlTransaction)
        Me.Connection = connection
        Me.Transaction = transaction
        _mappingController = New MappingController(Me)
        BatchSize = 10000
        DataCopyTimeout = 60 * 10
    End Sub

    Protected Overrides Sub Finalize()
        Try
            If Not String.IsNullOrEmpty(Me.ConnectionString) Then
                If Transaction IsNot Nothing Then
                    Transaction.Rollback()
                End If
                If Connection IsNot Nothing AndAlso Connection.State <> ConnectionState.Closed Then
                    Connection.Close()
                End If
            End If
        Catch appendException1 As Exception
        	Mysoft.Map.Data.MyDB.LogException(appendException1)

        End Try
    End Sub

#Region "public inner class"
    <System.Diagnostics.DebuggerStepThrough()> _
    Public Class PretreatingEventArgs
        Inherits EventArgs
        Private privateTableName As String
        Public Property TableName() As String
            Get
                Return privateTableName
            End Get
            Friend Set(ByVal value As String)
                privateTableName = value
            End Set
        End Property
        Private privateConnection As SqlConnection
        Public Property Connection() As SqlConnection
            Get
                Return privateConnection
            End Get
            Friend Set(ByVal value As SqlConnection)
                privateConnection = value
            End Set
        End Property
        Private privateTransaction As SqlTransaction
        Public Property Transaction() As SqlTransaction
            Get
                Return privateTransaction
            End Get
            Friend Set(ByVal value As SqlTransaction)
                privateTransaction = value
            End Set
        End Property
        Private privateCancel As Boolean
        Public Property Cancel() As Boolean
            Get
                Return privateCancel
            End Get
            Set(ByVal value As Boolean)
                privateCancel = value
            End Set
        End Property
        Private privateTag As List(Of Object)
        Public Property Tag() As List(Of Object)
            Get
                Return privateTag
            End Get
            Set(ByVal value As List(Of Object))
                privateTag = value
            End Set
        End Property
    End Class

    <System.Diagnostics.DebuggerStepThrough()> _
    Public Class PretreatedEventArgs
        Inherits EventArgs
        Private privateTableName As String
        Public Property TableName() As String
            Get
                Return privateTableName
            End Get

            Friend Set(ByVal value As String)
                privateTableName = value
            End Set
        End Property
        Private privateRowsCopied As Long
        Public Property RowsCopied() As Long
            Get
                Return privateRowsCopied
            End Get
            Friend Set(ByVal value As Long)
                privateRowsCopied = value
            End Set
        End Property
        Private privateConnection As SqlConnection
        Public Property Connection() As SqlConnection
            Get
                Return privateConnection
            End Get
            Friend Set(ByVal value As SqlConnection)
                privateConnection = value
            End Set
        End Property
        Private privateTransaction As SqlTransaction
        Public Property Transaction() As SqlTransaction
            Get
                Return privateTransaction
            End Get
            Friend Set(ByVal value As SqlTransaction)
                privateTransaction = value
            End Set
        End Property
        Private privateCancel As Boolean
        Public Property Cancel() As Boolean
            Get
                Return privateCancel
            End Get
            Set(ByVal value As Boolean)
                privateCancel = value
            End Set
        End Property
        Private privateTag As List(Of Object)
        Public Property Tag() As List(Of Object)
            Get
                Return privateTag
            End Get
            Set(ByVal value As List(Of Object))
                privateTag = value
            End Set
        End Property
    End Class

    <System.Diagnostics.DebuggerStepThrough()> _
    Public Class DataUpdatingEventArgs
        Inherits EventArgs
        Private privateTempTableName As String
        Public Property TempTableName() As String
            Get
                Return privateTempTableName
            End Get
            Friend Set(ByVal value As String)
                privateTempTableName = value
            End Set
        End Property
        Private privateTargetTableName As String
        Public Property TargetTableName() As String
            Get
                Return privateTargetTableName
            End Get
            Friend Set(ByVal value As String)
                privateTargetTableName = value
            End Set
        End Property
        Private privateConnection As SqlConnection
        Public Property Connection() As SqlConnection
            Get
                Return privateConnection
            End Get
            Friend Set(ByVal value As SqlConnection)
                privateConnection = value
            End Set
        End Property
        Private privateTransaction As SqlTransaction
        Public Property Transaction() As SqlTransaction
            Get
                Return privateTransaction
            End Get
            Friend Set(ByVal value As SqlTransaction)
                privateTransaction = value
            End Set
        End Property
        Private privateCancel As Boolean
        Public Property Cancel() As Boolean
            Get
                Return privateCancel
            End Get
            Set(ByVal value As Boolean)
                privateCancel = value
            End Set
        End Property
        Private privateTag As List(Of Object)
        Public Property Tag() As List(Of Object)
            Get
                Return privateTag
            End Get
            Set(ByVal value As List(Of Object))
                privateTag = value
            End Set
        End Property
    End Class

    <System.Diagnostics.DebuggerStepThrough()> _
    Public Class DataUpdatedEventArgs
        Inherits EventArgs
        Private privateTempTableName As String
        Public Property TempTableName() As String
            Get
                Return privateTempTableName
            End Get
            Friend Set(ByVal value As String)
                privateTempTableName = value
            End Set
        End Property
        Private privateTargetTableName As String
        Public Property TargetTableName() As String
            Get
                Return privateTargetTableName
            End Get
            Friend Set(ByVal value As String)
                privateTargetTableName = value
            End Set
        End Property
        Private privateRowsUpdated As Long
        Public Property RowsUpdated() As Long
            Get
                Return privateRowsUpdated
            End Get
            Friend Set(ByVal value As Long)
                privateRowsUpdated = value
            End Set
        End Property
        Private privateConnection As SqlConnection
        Public Property Connection() As SqlConnection
            Get
                Return privateConnection
            End Get
            Friend Set(ByVal value As SqlConnection)
                privateConnection = value
            End Set
        End Property
        Private privateTransaction As SqlTransaction
        Public Property Transaction() As SqlTransaction
            Get
                Return privateTransaction
            End Get
            Friend Set(ByVal value As SqlTransaction)
                privateTransaction = value
            End Set
        End Property
        Private privateCancel As Boolean
        Public Property Cancel() As Boolean
            Get
                Return privateCancel
            End Get
            Set(ByVal value As Boolean)
                privateCancel = value
            End Set
        End Property
        Private privateTag As List(Of Object)
        Public Property Tag() As List(Of Object)
            Get
                Return privateTag
            End Get
            Set(ByVal value As List(Of Object))
                privateTag = value
            End Set
        End Property
    End Class

    <System.Diagnostics.DebuggerStepThrough()> _
    Public Class CopyToTargetEventArgs
        Inherits EventArgs
        Private privateTempTableName As String
        Public Property TempTableName() As String
            Get
                Return privateTempTableName
            End Get
            Friend Set(ByVal value As String)
                privateTempTableName = value
            End Set
        End Property
        Private privateTargetTableName As String
        Public Property TargetTableName() As String
            Get
                Return privateTargetTableName
            End Get
            Friend Set(ByVal value As String)
                privateTargetTableName = value
            End Set
        End Property
        Private privateConnection As SqlConnection
        Public Property Connection() As SqlConnection
            Get
                Return privateConnection
            End Get
            Friend Set(ByVal value As SqlConnection)
                privateConnection = value
            End Set
        End Property
        Private privateTransaction As SqlTransaction
        Public Property Transaction() As SqlTransaction
            Get
                Return privateTransaction
            End Get
            Friend Set(ByVal value As SqlTransaction)
                privateTransaction = value
            End Set
        End Property
        Private privateRowsCopied As Long
        Public Property RowsCopied() As Long
            Get
                Return privateRowsCopied
            End Get
            Friend Set(ByVal value As Long)
                privateRowsCopied = value
            End Set
        End Property
        Private privateCancel As Boolean
        Public Property Cancel() As Boolean
            Get
                Return privateCancel
            End Get
            Set(ByVal value As Boolean)
                privateCancel = value
            End Set
        End Property
        Private privateTag As List(Of Object)
        Public Property Tag() As List(Of Object)
            Get
                Return privateTag
            End Get
            Set(ByVal value As List(Of Object))
                privateTag = value
            End Set
        End Property
    End Class

    Private Class MappingController
        Private ReadOnly _myDataCopyController As MyDataCopyController
        Public Sub New(ByVal myDataCopyController As MyDataCopyController)
            _myDataCopyController = myDataCopyController
        End Sub

        Friend Sub CreateColumnMappings(ByVal bulkCopy As SqlBulkCopy, ByVal insertMode As Boolean)
            If insertMode Then
                Dim dtSchema As DataTable = GetTableSchema()
                Dim columnName As String
                For Each column As DataColumn In _myDataCopyController.DataSource.Columns
                    If _myDataCopyController.InsertColumnMappings IsNot Nothing AndAlso (Not _myDataCopyController.InsertColumnMappings.ContainsKey(column.ColumnName.ToUpper())) Then
                        If IsAllowColDbType(GetColDbType(column.ColumnName, dtSchema)) Then
                            columnName = GetRealName(column.ColumnName, dtSchema)
                            If columnName <> "" Then
                                bulkCopy.ColumnMappings.Add(column.ColumnName, GetRealName(column.ColumnName, dtSchema))
                            End If
                        End If
                    End If
                Next column
                If _myDataCopyController.InsertColumnMappings IsNot Nothing Then
                    For Each columnMapping As KeyValuePair(Of String, String) In _myDataCopyController.InsertColumnMappings
                        columnName = GetRealName(columnMapping.Value, dtSchema)
                        If columnName <> "" Then
                            bulkCopy.ColumnMappings.Add(columnMapping.Key, columnName)
                        End If
                    Next columnMapping
                End If
            Else
                For Each column As DataColumn In _myDataCopyController.DataSource.Columns
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName)
                Next column
            End If
        End Sub

        Private Function GetRealName(ByVal name As String, ByVal dtSchema As DataTable) As String
            dtSchema.DefaultView.RowFilter = String.Format("name = '{0}'", name)
            Dim dtView As DataTable = dtSchema.DefaultView.ToTable()
            If dtView.Rows.Count > 0 Then
                Return dtSchema.DefaultView.ToTable().Rows(0)(0).ToString()
            End If
            If _myDataCopyController.UserTemporaryTable OrElse _myDataCopyController.CurrentCopyTypeEnum = MyDataCopyTypeEnum.UserTemporaryTable Then
                Return name
            Else
                Return ""
            End If
        End Function

        Private Function GetColDbType(ByVal name As String, ByVal dtSchema As DataTable) As String
            dtSchema.DefaultView.RowFilter = String.Format("name = '{0}'", name)
            Dim dtView As DataTable = dtSchema.DefaultView.ToTable()
            If dtView.Rows.Count > 0 Then
                Return dtSchema.DefaultView.ToTable().Rows(0)(1).ToString()
            End If
            Return ""
        End Function

        Friend Function IsAllowColDbType(ByVal strDbType As String) As Boolean
            Return strDbType.ToUpper <> "TIMESTAMP"
        End Function

        Friend Function GetTableSchema() As DataTable
            Dim dtSchema As New DataTable()
            Try
                Dim sql As String = String.Format("SELECT  c.name,tp.name FROM sys.tables t,sys.columns c,sys.types tp WHERE t.object_id = c.object_id AND c.user_type_id = tp.user_type_id AND t.name='{0}'", _myDataCopyController.TargetTableName)

                Dim adapter As New SqlDataAdapter(sql, _myDataCopyController.Connection)
                adapter.SelectCommand.Transaction = _myDataCopyController.Transaction
                adapter.Fill(dtSchema)
            Catch exception As Exception
            	Mysoft.Map.Data.MyDB.LogException(exception)
                _myDataCopyController.LastErrorMessage = exception.Message
                Return Nothing
            End Try

            Return dtSchema
        End Function
    End Class

    Public Enum MyDataCopyTypeEnum
        UserTemporaryTable
        DirectlyCopy
    End Enum

    Public Enum MyDataUpdateTypeEnum
        AllMappingColumnsData
        ExcludeNullData
    End Enum
#End Region

#Region "public event"

    Public Event PretreatingEventHandler As EventHandler(Of PretreatingEventArgs)
    Public Event PretreatedEventHandler As EventHandler(Of PretreatedEventArgs)
    Public Event DataUpdatingEventHandler As EventHandler(Of DataUpdatingEventArgs)
    Public Event DataUpdatedEventHandler As EventHandler(Of DataUpdatedEventArgs)
    Public Event CopyToTargetEventHandler As EventHandler(Of CopyToTargetEventArgs)

#End Region

#Region "protected property"

    Private privateBatchSize As Integer
    Public Property BatchSize() As Integer
        Get
            Return privateBatchSize
        End Get
        Set(ByVal value As Integer)
            privateBatchSize = value
        End Set
    End Property

    Private privateDataCopyTimeout As Integer
    Public Property DataCopyTimeout() As Integer
        Get
            Return privateDataCopyTimeout
        End Get
        Set(ByVal value As Integer)
            privateDataCopyTimeout = value
        End Set
    End Property

    ''' <summary>
    ''' Key:数据源的字段名称
    ''' Value:目标表的字段名称
    ''' </summary>
    Public Overridable ReadOnly Property InsertColumnMappings() As Dictionary(Of String, String)
        Get
            If _insertColumnMappings Is Nothing Then
                _insertColumnMappings = New Dictionary(Of String, String)()
            End If
            Return _insertColumnMappings
        End Get
    End Property

    ''' <summary>
    ''' Key:数据源的字段名称
    ''' Value:目标表的字段名称
    ''' </summary>
    Public Overridable ReadOnly Property UpdateColumnMappings() As Dictionary(Of String, String)
        Get
            If _updateColumnMappings Is Nothing Then
                _updateColumnMappings = New Dictionary(Of String, String)()
            End If
            Return _updateColumnMappings
        End Get
    End Property

    ''' <summary>
    ''' Key:数据源的字段名称
    ''' Value:目标表的字段名称
    ''' </summary>
    Public Overridable ReadOnly Property TableJoinKeys() As Dictionary(Of String, String)
        Get
            If _tableJoinKeys Is Nothing Then
                _tableJoinKeys = New Dictionary(Of String, String)()
            End If
            Return _tableJoinKeys
        End Get
    End Property

    ''' <summary>
    ''' 有序Guid字段名称
    ''' </summary>
    Public Overridable ReadOnly Property SeqGuidColumns() As List(Of String)
        Get
            If _seqGuidColumns Is Nothing Then
                _seqGuidColumns = New List(Of String)()
            End If
            Return _seqGuidColumns
        End Get
    End Property

    Private privateCopyToTargetSql As String
    Public Overridable Property CopyToTargetSql() As String
        Get
            If String.IsNullOrEmpty(privateCopyToTargetSql) Then
                Dim dtSchema As DataTable = _mappingController.GetTableSchema()
                Dim strColumns As String = String.Empty
                For Each drRow As DataRow In dtSchema.Rows
                    If _mappingController.IsAllowColDbType(drRow(1).ToString()) Then
                        strColumns &= drRow(0) & ","
                    End If
                Next
                If Not String.IsNullOrEmpty(strColumns) Then
                    '目标表是物理表,需要明确指定字段(排除时间戳类型),临时表此时已经排除了时间戳类型的字段
                    Return String.Format("Insert Into {0} ({1}) Select * from {2}", TargetTableName, strColumns.Substring(0, strColumns.Length - 1), TempTableName)
                Else
                    '目标表为临时表
                    Return String.Format("Insert Into {0} Select * from {1}", TargetTableName, TempTableName)
                End If
            Else
                Return privateCopyToTargetSql
            End If
        End Get
        Set(ByVal value As String)
            privateCopyToTargetSql = value
        End Set
    End Property

    Private privateUpdateToTargetSql As String
    Public Overridable Property UpdateToTargetSql() As String
        Get
            If String.IsNullOrEmpty(privateUpdateToTargetSql) = True Then
                If UpdateColumnMappings Is Nothing OrElse UpdateColumnMappings.Count = 0 Then
                    Throw New Exception("UpdateColumnMappings")
                End If
                If TableJoinKeys Is Nothing OrElse TableJoinKeys.Count = 0 Then
                    Throw New Exception("TableJoinKeys")
                End If
                If String.IsNullOrEmpty(TargetTableName) Then
                    Throw New Exception("TargetTableName")
                End If

                Dim sbSql As New StringBuilder()
                sbSql.AppendLine("UPDATE  targetTable")
                sbSql.AppendLine("SET")
                For Each columnMapping As KeyValuePair(Of String, String) In UpdateColumnMappings
                    If _myDataUpdateTypeEnum = MyDataUpdateTypeEnum.AllMappingColumnsData Then
                        sbSql.AppendLine(String.Format("    targetTable.[{1}] = sourceTable.[{0}] ,", columnMapping.Key, columnMapping.Value))
                    ElseIf _myDataUpdateTypeEnum = MyDataUpdateTypeEnum.ExcludeNullData Then
                        sbSql.AppendLine(String.Format("    targetTable.[{1}] =(Case When sourceTable.[{0}] Is Null Then targetTable.[{1}] Else sourceTable.[{0}] End) ,", columnMapping.Key, columnMapping.Value))
                    End If
                Next columnMapping
                sbSql.Remove(sbSql.Length - 3, 3)
                sbSql.AppendLine("FROM")
                sbSql.AppendLine(String.Format("  [{0}] AS sourceTable ,", TempTableName))
                sbSql.AppendLine(String.Format("  [{0}] AS targetTable", TargetTableName))
                sbSql.AppendLine("WHERE")
                For Each joinKey As KeyValuePair(Of String, String) In TableJoinKeys
                    sbSql.AppendLine(String.Format("    targetTable.[{1}] = sourceTable.[{0}] AND", joinKey.Key, joinKey.Value))
                Next joinKey
                sbSql.AppendLine("    1 = 1")
                Return sbSql.ToString()
            Else
                Return privateUpdateToTargetSql
            End If
        End Get
        Set(ByVal value As String)
            privateUpdateToTargetSql = value
        End Set
    End Property

    Private privateCreateTableSql As String
    Public Overridable Property CreateTableSql() As String
        Get
            Dim strTemp As String
            If String.IsNullOrEmpty(privateCreateTableSql) Then
                Dim dtTemp As New DataTable()
                Dim dtDv As New DataTable()
                Dim adapter As New SqlDataAdapter(String.Format("SET FMTONLY ON;SELECT * FROM {0};SET FMTONLY OFF;", TargetTableName), Connection)
                adapter.SelectCommand.Transaction = Transaction
                adapter.Fill(dtTemp)
                Dim sql As String = SqlTableCreator.GetCreateFromDataTableSQL(TempTableName, dtTemp)
                adapter = New SqlDataAdapter("SELECT  'ALTER TABLE {0} ADD  DEFAULT ' + definition + ' FOR [' + c.Name + '];' AS stat" & ControlChars.CrLf & "FROM    sys.default_constraints dv ," & ControlChars.CrLf & "        sys.columns c" & ControlChars.CrLf & "WHERE   dv.parent_object_id = OBJECT_ID('" & TargetTableName & "')" & ControlChars.CrLf & "        AND dv.parent_object_id = c.object_id" & ControlChars.CrLf & "        AND dv.parent_column_id=c.column_id", Connection)
                adapter.SelectCommand.Transaction = Transaction
                adapter.Fill(dtDv)
                strTemp = dtDv.Rows.Cast(Of DataRow)().Aggregate(sql, Function(current, row) current + row("stat"))
            Else
                strTemp = privateCreateTableSql
            End If
            For Each seqGuidColumn As String In SeqGuidColumns
                strTemp += "Alter TABLE {0} ADD [" & seqGuidColumn.ToUpper & "] UNIQUEIDENTIFIER DEFAULT (newsequentialid());"
            Next
            Return strTemp
        End Get
        Set(ByVal value As String)
            privateCreateTableSql = value
        End Set
    End Property

    Private privateDataSource As DataTable
    Public Property DataSource() As DataTable
        Get
            Return privateDataSource
        End Get
        Set(ByVal value As DataTable)
            privateDataSource = value
        End Set
    End Property

    Private privateTargetTableName As String
    Public Property TargetTableName() As String
        Get
            Return privateTargetTableName
        End Get
        Set(ByVal value As String)
            privateTargetTableName = value
        End Set
    End Property

#End Region

#Region "protected method"

    Protected Function OpenConnection() As Boolean
        Try
            If Not String.IsNullOrEmpty(ConnectionString) Then
                Connection = New SqlConnection(ConnectionString)
                Connection.Open()
                Transaction = Connection.BeginTransaction()
            End If
        Catch exception As Exception
        	Mysoft.Map.Data.MyDB.LogException(exception)
            LastErrorMessage = exception.Message
            Return False
        End Try

        Return True
    End Function
    Protected Function CloseConnection() As Boolean
        Try
            If Not String.IsNullOrEmpty(Me.ConnectionString) Then
                If Connection IsNot Nothing AndAlso Connection.State <> ConnectionState.Closed Then
                    Transaction.Commit()
                    Connection.Close()
                End If
            End If
        Catch exception As Exception
        	Mysoft.Map.Data.MyDB.LogException(exception)
            LastErrorMessage = exception.Message
            Return False
        End Try
        Return True
    End Function

#End Region

#Region "public property"

    Private privateTransaction As SqlTransaction
    Public Property Transaction() As SqlTransaction
        Get
            Return privateTransaction
        End Get
        Private Set(ByVal value As SqlTransaction)
            privateTransaction = value
        End Set
    End Property
    Private privateConnection As SqlConnection
    Public Property Connection() As SqlConnection
        Get
            Return privateConnection
        End Get
        Private Set(ByVal value As SqlConnection)
            privateConnection = value
        End Set
    End Property
    Private privateConnectionString As String
    Public Property ConnectionString() As String
        Get
            Return privateConnectionString
        End Get
        Private Set(ByVal value As String)
            privateConnectionString = value
        End Set
    End Property
    Private privateLastErrorMessage As String
    Public Property LastErrorMessage() As String
        Get
            Return privateLastErrorMessage
        End Get
        Private Set(ByVal value As String)
            privateLastErrorMessage = value
        End Set
    End Property
    Private privateUserTemporaryTable As Boolean
    Public Property UserTemporaryTable() As Boolean
        Get
            Return privateUserTemporaryTable
        End Get
        Set(ByVal value As Boolean)
            privateUserTemporaryTable = value
        End Set
    End Property
    Public ReadOnly Property TempTableName() As String
        Get
            If String.IsNullOrEmpty(_tableName) Then
                _tableName = String.Format("#{0}", Guid.NewGuid().ToString().Replace("-", ""))
            End If
            Return _tableName
        End Get
    End Property
    Public ReadOnly Property CurrentTargetName() As String
        Get
            Return If(_myDataCopyTypeEnum = MyDataCopyTypeEnum.UserTemporaryTable, TempTableName, TargetTableName)
        End Get
    End Property
    Private privateTag As List(Of Object)
    Public ReadOnly Property Tag() As List(Of Object)
        Get
            If privateTag Is Nothing Then
                privateTag = New List(Of Object)
            End If
            Return privateTag
        End Get
    End Property
    Private _myDataCopyTypeEnum As MyDataCopyTypeEnum
    Public ReadOnly Property CurrentCopyTypeEnum() As MyDataCopyTypeEnum
        Get
            Return _myDataCopyTypeEnum
        End Get
    End Property
    Private _myDataUpdateTypeEnum As MyDataUpdateTypeEnum
    Public ReadOnly Property CurrentUpdateTypeEnum() As MyDataUpdateTypeEnum
        Get
            Return _myDataUpdateTypeEnum
        End Get
    End Property
#End Region

#Region "private variable"

    Private _rowsCopied As Long
    Private _rowsUpdated As Long
    Private _tableName As String
    Private _insertMode As Boolean
    Private ReadOnly _mappingController As MappingController
    Private _seqGuidColumns As List(Of String)
    Private _tableJoinKeys As Dictionary(Of String, String)
    Private _updateColumnMappings As Dictionary(Of String, String)
    Private _insertColumnMappings As Dictionary(Of String, String)

#End Region

#Region "data update"

    Public Function DataUpdate(ByVal myDataUpdateTypeEnum As MyDataUpdateTypeEnum) As Boolean
        _myDataCopyTypeEnum = MyDataCopyTypeEnum.UserTemporaryTable
        _myDataUpdateTypeEnum = myDataUpdateTypeEnum
        _insertMode = False

        If (Not OpenConnection()) Then
            Return False
        End If

        If (Not CreateTempTable(DataSource)) Then
            Return False
        End If

        If FirePretreatingEvent() Then
            Return False
        End If

        If (Not SubDataCopy()) Then
            Return False
        End If

        If FirePretreatedEvent(_rowsCopied) Then
            Return False
        End If

        If FireDataUpdatingEvent() Then
            Return False
        End If

        If (Not SubDataUpdate()) Then
            Return False
        End If

        If FireDataUpdatedEvent(_rowsUpdated) Then
            Return False
        End If

        If (Not CloseConnection()) Then
            Return False
        End If
        Return True
    End Function

    Private Function SubDataUpdate() As Boolean
        Try
            Dim command As New SqlCommand(String.Format(UpdateToTargetSql, TempTableName, TargetTableName), Connection) With {.Transaction = Transaction}
            _rowsUpdated = command.ExecuteNonQuery()
        Catch exception As Exception
        	Mysoft.Map.Data.MyDB.LogException(exception)
            LastErrorMessage = exception.Message
            Return False
        End Try
        Return True
    End Function

    Private Function CreateTempTable(ByVal dataSource As DataTable) As Boolean
        Dim sql As String = SqlTableCreator.GetCreateFromDataTableSQL(TempTableName, dataSource)

        Try
            Dim command As New SqlCommand(sql, Connection) With {.Transaction = Transaction}
            command.ExecuteNonQuery()
        Catch exception As Exception
        	Mysoft.Map.Data.MyDB.LogException(exception)
            LastErrorMessage = exception.Message
            Return False
        End Try
        Return True
    End Function

    Private Function FireDataUpdatingEvent() As Boolean
        Dim e1 As DataUpdatingEventArgs = New DataUpdatingEventArgs With {.TargetTableName = TargetTableName, .TempTableName = TempTableName, .Connection = Connection, .Transaction = Transaction, .Cancel = False, .Tag = Tag}
        RaiseEvent DataUpdatingEventHandler(Me, e1)
        Return e1.Cancel
    End Function

    Private Function FireDataUpdatedEvent(ByVal rowsUpdated As Long) As Boolean
        Dim e1 As DataUpdatedEventArgs = New DataUpdatedEventArgs With {.TargetTableName = TargetTableName, .TempTableName = TempTableName, .Connection = Connection, .Transaction = Transaction, .RowsUpdated = rowsUpdated, .Cancel = False, .Tag = Tag}
        RaiseEvent DataUpdatedEventHandler(Me, e1)
        Return e1.Cancel
    End Function

#End Region

#Region "data copy"

    Public Function DataCopy(ByVal myDataCopyTypeEnum As MyDataCopyTypeEnum) As Boolean
        _myDataCopyTypeEnum = myDataCopyTypeEnum
        _insertMode = True

        If (Not OpenConnection()) Then
            Return False
        End If

        If myDataCopyTypeEnum = myDataCopyTypeEnum.UserTemporaryTable OrElse UserTemporaryTable = True Then
            If (Not CreateTempTable(String.Format(CreateTableSql, CurrentTargetName))) Then
                Return False
            End If
        End If

        If FirePretreatingEvent() Then
            Return False
        End If

        If (Not SubDataCopy()) Then
            Return False
        End If

        If myDataCopyTypeEnum = myDataCopyTypeEnum.UserTemporaryTable Then
            If FirePretreatedEvent(_rowsCopied) Then
                Return False
            End If

            If (Not CopyToTargetTable(String.Format(CopyToTargetSql, TempTableName), _rowsCopied)) Then
                Return False
            End If
        End If

        If FireCopyToTargetEvent(_rowsCopied) Then
            Return False
        End If

        If (Not CloseConnection()) Then
            Return False
        End If
        Return True
    End Function

    Private Function FirePretreatingEvent() As Boolean
        Dim e1 As PretreatingEventArgs = New PretreatingEventArgs With {.TableName = CurrentTargetName, .Connection = Connection, .Transaction = Transaction, .Cancel = False, .Tag = Tag}
        RaiseEvent PretreatingEventHandler(Me, e1)
        Return e1.Cancel
    End Function

    Private Function FirePretreatedEvent(ByVal rowsCopied As Long) As Boolean
        Dim e1 As PretreatedEventArgs = New PretreatedEventArgs With {.TableName = CurrentTargetName, .Connection = Connection, .Transaction = Transaction, .RowsCopied = rowsCopied, .Cancel = False, .Tag = Tag}
        RaiseEvent PretreatedEventHandler(Me, e1)
        Return e1.Cancel
    End Function

    Private Function FireCopyToTargetEvent(ByVal rowsCopied As Long) As Boolean
        Dim e1 As CopyToTargetEventArgs = New CopyToTargetEventArgs With {.TempTableName = CurrentTargetName, .TargetTableName = TargetTableName, .Connection = Connection, .Transaction = Transaction, .RowsCopied = rowsCopied, .Tag = Tag, .Cancel = False}
        RaiseEvent CopyToTargetEventHandler(Me, e1)
        Return e1.Cancel
    End Function

    Private Function CopyToTargetTable(ByVal sql As String, ByVal rowsCopied As Long) As Boolean
        Try
            Dim command As New SqlCommand(sql, Connection) With {.Transaction = Transaction, .CommandTimeout = DataCopyTimeout}
            command.ExecuteNonQuery()
        Catch exception As Exception
        	Mysoft.Map.Data.MyDB.LogException(exception)
            LastErrorMessage = exception.Message
            Return False
        End Try
        Return True
    End Function

    Private Function SubDataCopy() As Boolean
        Try
            _rowsCopied = 0
            Using bulkCopy As New SqlBulkCopy(Connection, SqlBulkCopyOptions.FireTriggers Or SqlBulkCopyOptions.CheckConstraints Or SqlBulkCopyOptions.KeepNulls, Transaction)
                bulkCopy.BatchSize = BatchSize
                bulkCopy.BulkCopyTimeout = DataCopyTimeout
                bulkCopy.DestinationTableName = CurrentTargetName
                bulkCopy.NotifyAfter = 1
                AddHandler bulkCopy.SqlRowsCopied, AddressOf AnonymousMethod1
                _mappingController.CreateColumnMappings(bulkCopy, _insertMode)
                bulkCopy.WriteToServer(DataSource)
            End Using
        Catch exception As Exception
        	Mysoft.Map.Data.MyDB.LogException(exception)
            LastErrorMessage = exception.Message
            Return False
        End Try
        Return True
    End Function
    Private Sub AnonymousMethod1(ByVal sender As Object, ByVal e As SqlRowsCopiedEventArgs)
        _rowsCopied = e.RowsCopied
    End Sub

    Private Function CreateTempTable(ByVal sql As String) As Boolean
        Try
            Dim command As New SqlCommand(sql, Connection) With {.Transaction = Transaction}
            command.ExecuteNonQuery()
        Catch exception As Exception
        	Mysoft.Map.Data.MyDB.LogException(exception)
            LastErrorMessage = exception.Message
            Return False
        End Try
        Return True
    End Function

#End Region
End Class
