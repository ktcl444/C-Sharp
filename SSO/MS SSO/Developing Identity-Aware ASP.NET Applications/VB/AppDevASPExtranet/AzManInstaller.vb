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
Imports System.Collections
Imports System.ComponentModel
Imports System.Configuration
Imports System.Configuration.Install
Imports System.Runtime.InteropServices
Imports Microsoft.Interop.Security.AzRoles
Imports AppDevASPLib.Contoso.Identity.Template

Namespace Contoso.AppDevASPSample

    ' <summary>
    '   Creates an AZMan policy file in C:\AppDevASPExtranet.xml for
    '   the Extranet sample. The file AzMan store, application,
    '   operations, role definitions and roles that the application 
    '   will use. 
    '   This could be set up using the AzMan UI, but only the
    '   developer knows what the application is doing and what 
    '   operations, tasks etc that it contains. Therefore the
    '   developer should supply an installer for the AzMan policy
    ' </summary>
    <RunInstaller(True)> _
    Public Class AzManInstaller
        Inherits Installer

        Private Const AZ_POLICY_URL As String = "msxml://C:\AppDevASPExtranet.xml"
        Private Const APPLICATION_NAME As String = "AppDevASPExtranet"
        Private Const READ_EMPLOYEE_OPERATION_ID As Integer = 61
        Private Const WRITE_EMPLOYEE_OPERATION_ID As Integer = 62

        'Overrides the Install event and initializes AzMan stores
        'roles, operations etc.
        Public Overrides Sub Install(ByVal savedState As IDictionary)

            MyBase.Install(savedState)
            Dim store As AzAuthorizationStoreClass = CreateStore()
            Dim app As IAzApplication = CreateApplication(store)
            CreateOperations(app)
            CreateTasks(app)
            CreateRoleDefinitions(app)
            CreateRoles(app)

        End Sub 'Install

        'Create the AzMan store
        Private Function CreateStore() As AzAuthorizationStoreClass

            'Create a new store for app if it doesn't exist
            Dim store As New AzAuthorizationStoreClass

            Try
                store.Initialize(AzMan.AZ_AZSTORE_FLAG_ACCESS_CHECK, AZ_POLICY_URL, Nothing)
            Catch
                store.Initialize(AzMan.AZ_AZSTORE_FLAG_CREATE, AZ_POLICY_URL, Nothing)
                store.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, Nothing)
            End Try

            Return store

        End Function 'CreateStore

        'Create the AzMan application
        Private Function CreateApplication(ByVal store As AzAuthorizationStoreClass) As IAzApplication

            'Create app in store if it doesn't exist
            Dim app As IAzApplication = Nothing

            Try
                app = store.OpenApplication(APPLICATION_NAME, Nothing)
            Catch
                app = store.CreateApplication(APPLICATION_NAME, Nothing)
                app.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, Nothing)
            End Try

            Return app

        End Function 'CreateApplication

        'Create the AzMan operations
        Private Sub CreateOperations(ByVal app As IAzApplication)

            CreateOperation(app, "ReadEmployeeData", READ_EMPLOYEE_OPERATION_ID)
            CreateOperation(app, "WriteEmployeeData", WRITE_EMPLOYEE_OPERATION_ID)

        End Sub 'CreateOperations

        'Create a single AzMan operation
        Private Sub CreateOperation(ByVal app As IAzApplication, ByVal operationName As String, ByVal operationId As Int32)

            Dim op As IAzOperation = Nothing

            Try
                op = app.CreateOperation(operationName, Nothing)
                op.OperationID = operationId
                op.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, Nothing)
            Catch
                'TODO: Do Catch Implementation
            End Try

        End Sub 'CreateOperation

        'Create the AzMan tasks
        Private Sub CreateTasks(ByVal app As IAzApplication)

            CreateTask(app, "View employees", New String() {"ReadEmployeeData"})
            CreateTask(app, "Manage employees", New String() {"ReadEmployeeData", "WriteEmployeeData"})

        End Sub 'CreateTasks

        'Create a single AzMan task
        Private Sub CreateTask(ByVal app As IAzApplication, ByVal taskName As String, ByVal operationNames() As String)

            Dim task As IAzTask = Nothing
            Try
                task = app.CreateTask(taskName, Nothing)
                Dim operationName As String

                For Each operationName In operationNames
                    task.AddOperation(operationName, Nothing)
                Next operationName

                task.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, Nothing)
            Catch
                'TODO: Do Catch Implementation
            End Try

        End Sub 'CreateTask

        'Create the AzMan role definitions
        Private Sub CreateRoleDefinitions(ByVal app As IAzApplication)

            CreateRoleDefinition(app, "Employee user", New String() {"View employees"})
            CreateRoleDefinition(app, "Employee manager", New String() {"Manage employees"})

        End Sub 'CreateRoleDefinitions

        'Create a single AzMan role definition
        Private Sub CreateRoleDefinition(ByVal app As IAzApplication, ByVal roleName As String, ByVal taskNames() As String)

            Dim task As IAzTask = Nothing

            Try
                task = app.CreateTask(roleName, Nothing)
                task.IsRoleDefinition = Convert.ToInt32(True)
                Dim taskName As String

                For Each taskName In taskNames
                    task.AddTask(taskName, Nothing)
                Next taskName

                task.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, Nothing)
            Catch
                'TODO: Do Catch Implementation
            End Try

        End Sub 'CreateRoleDefinition

        'Create the AzMan roles
        Private Sub CreateRoles(ByVal app As IAzApplication)

            Dim scope As IAzScope = app.CreateScope("All", Nothing)
            scope.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, Nothing)
            CreateRole(scope, "Users", New String() {"Employee user"}, Nothing)
            CreateRole(scope, "Managers", New String() {"Employee manager"}, Nothing)

        End Sub 'CreateRoles

        'Create a single AzMan role
        Private Sub CreateRole(ByVal scope As IAzScope, ByVal roleName As String, ByVal taskNames() As String, ByVal userNames() As String)

            Dim role As IAzRole = Nothing

            Try
                role = scope.CreateRole(roleName, Nothing)
                Dim taskName As String

                For Each taskName In taskNames
                    role.AddTask(taskName, Nothing)
                Next taskName

                role.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, Nothing)

                Dim userName As String

                For Each userName In userNames
                    role.AddMemberName(userName, Nothing)
                Next userName

                role.Submit(AzMan.AZ_AZSTORE_FLAG_BATCH_UPDATE, Nothing)
            Catch
                'TODO: Do Catch Implementation
            End Try

        End Sub 'CreateRole 
    End Class 'AzManInstaller
End Namespace 'Contoso.AppDevASPSample
