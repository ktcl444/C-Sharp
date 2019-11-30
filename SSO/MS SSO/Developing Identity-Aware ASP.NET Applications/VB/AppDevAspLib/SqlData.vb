'##############################################################################
' Microsoft Solutions for Security
' Copyright (c) 2004 Microsoft Corporation. All rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
' THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
' AND INFORMATION REMAINS WITH THE USER.
'##############################################################################

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration

Namespace Contoso.Identity.Template

    ' <summary>
    '   This is a simple class to wrap data access provided to minimize
    '   external dependencies. We recommend you use the DataAccess 
    '   Application Block (DAAB) from Patterns & Practices that can be
    '   found at:
    '   http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnbda/html/daab-rm.asp
    ' </summary>
    Public Class SqlData

        Private _error As String = Nothing

        ' <summary>
        '   Public default constructor of the SqlData helper class
        ' </summary>
        Public Sub New()

            _error = Nothing

        End Sub 'New

        ' <summary>
        '   Returns the last error message from a database operation
        ' </summary>
        Public ReadOnly Property [Error]() As String
            Get
                Return _error
            End Get
        End Property

        ' <summary>
        '   Connects to the database, executes the specified command and
        '   returns a dataset. The DAAB includes more variants on this
        '   and implements them in a more robust way
        ' </summary>
        ' <param name="command"></param>
        ' <returns></returns>
        Public Function GetDataset(ByVal command As String) As DataSet

            Dim cn As New SqlConnection

            Try
                Dim cnString As String = ConfigurationSettings.AppSettings("SQLConnectionString")

                cn.ConnectionString = cnString
                cn.Open()

                Dim cmd As SqlCommand = cn.CreateCommand()
                cmd.CommandText = command

                Dim a As New SqlDataAdapter
                a.SelectCommand = cmd

                Dim ds As New DataSet
                a.Fill(ds)
                Return ds
            Catch ex As Exception
                _error = ex.Message
                Return Nothing
            Finally
                If cn.State = ConnectionState.Open Then
                    cn.Close()
                End If
            End Try

        End Function 'GetDataset
    End Class 'SqlData 
End Namespace 'Contoso.Identity.Template