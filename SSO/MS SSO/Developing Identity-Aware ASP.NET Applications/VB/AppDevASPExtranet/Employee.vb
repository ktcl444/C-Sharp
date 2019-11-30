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
Imports AppDevASPLib.Contoso.Identity.Template

Namespace Contoso.AppDevASPSample

    ' <summary>
    '   The Employee class is a sample business object that manages
    '   employee info
    ' </summary>
    Public Class Employee

        Private Const READ_EMPLOYEE_OPERATION_ID As Integer = 61
        Private Const WRITE_EMPLOYEE_OPERATION_ID As Integer = 62
        Private _error As String = Nothing

        ' <summary>
        '   Returns the last error message from the employee business
        '   object
        ' </summary>
        Public ReadOnly Property [Error]() As String
            Get
                Return _error
            End Get
        End Property

        ' <summary>
        '   Authorize against role and get a list of employees
        ' </summary>
        ' <returns>A dataset containing Contoso employees</returns>
        Public Function GetEmployees() As DataSet

            'Do access AzMan check and get data from back-end
            Dim employeeDS As DataSet = Nothing
            Dim authManager As New AzMan

            If authManager.IsAllowedAccess(Me.GetType().Name, "All", READ_EMPLOYEE_OPERATION_ID) Then
                Dim employeeData As New SqlData
                employeeDS = employeeData.GetDataset("SELECT Firstname, Lastname, Title FROM Employees")

                If Not (employeeData.Error Is Nothing) Then
                    _error = employeeData.Error
                End If
            Else
                _error = "msgAccessDenied"
            End If

            Return employeeDS

        End Function 'GetEmployees 
    End Class 'Employee
End Namespace 'Contoso.AppDevASPSample