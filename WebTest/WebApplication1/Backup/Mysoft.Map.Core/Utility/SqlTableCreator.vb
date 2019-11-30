Imports Microsoft.VisualBasic
Imports System
Imports System.Data

Public Class SqlTableCreator
#Region "Static Methods"
    <System.Diagnostics.DebuggerStepThrough()> _
    Public Shared Function GetCreateFromDataTableSQL(ByVal tableName As String, ByVal table As DataTable) As String
        Dim sql As String = "CREATE TABLE [" & tableName & "] (" & Constants.vbLf
        ' columns
        For Each column As DataColumn In table.Columns
            Dim strColDbType As String = SQLGetType(column)
            If strColDbType <> "" Then
                sql &= "[" & column.ColumnName & "] " & SQLGetType(column) & "," & Constants.vbLf
            End If
        Next column
        sql = sql.TrimEnd(New Char() {","c, ControlChars.Lf}) + Constants.vbLf
        ' primary keys
        If table.PrimaryKey.Length > 0 Then
            sql &= "CONSTRAINT [PK_" & tableName & "] PRIMARY KEY CLUSTERED ("
            For Each column As DataColumn In table.PrimaryKey
                sql &= "[" & column.ColumnName & "],"
            Next column
            sql = sql.TrimEnd(New Char() {","c}) & "))" & Constants.vbLf
        End If

        'if not ends with ")"
        If (table.PrimaryKey.Length = 0) AndAlso ((Not sql.EndsWith(")"))) Then
            sql &= ")"
        End If

        Return sql
    End Function

    Public Shared Function SQLGetType(ByVal type As Object, ByVal columnSize As Integer, ByVal numericPrecision As Integer, ByVal numericScale As Integer) As String
        Select Case type.ToString()
            Case "System.String"
                Return "VARCHAR(" & (If((columnSize = -1), "512", If((columnSize > 8000), "MAX", columnSize.ToString()))) & ")"

            Case "System.Decimal"
                If numericScale > 0 Then
                    Return "MONEY"
                ElseIf numericPrecision > 10 Then
                    Return "BIGINT"
                Else
                    Return "INT"
                End If

            Case "System.Double", "System.Single"
                Return "MONEY"

            Case "System.Int64"
                Return "BIGINT"

            Case "System.Int16", "System.Int32"
                Return "INT"

            Case "System.DateTime"
                Return "DATETIME"

            Case "System.Boolean"
                Return "BIT"

            Case "System.Byte"
                Return "TINYINT"

            Case "System.Guid"
                Return "UNIQUEIDENTIFIER"

            Case "System.Byte[]"
                'TIMESTAMP 时间戳类型不处理
                Return ""
            Case Else
                Throw New Exception(type.ToString() & " not implemented.")
        End Select
    End Function

    ' Overload based on DataColumn from DataTable type
    Public Shared Function SQLGetType(ByVal column As DataColumn) As String
        Return SQLGetType(column.DataType, column.MaxLength, 10, 2)
    End Function
#End Region
End Class
