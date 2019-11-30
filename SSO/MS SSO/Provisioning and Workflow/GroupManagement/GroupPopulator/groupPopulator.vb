'/////////////////////////////////////////////////////////////////////////////
' Microsoft Solutions for Security
' Copyright (c) 2004 Microsoft Corporation. All rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
' THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
' AND INFORMATION REMAINS WITH THE USER.
'/////////////////////////////////////////////////////////////////////////////

Imports System.IO
Imports System.Xml
Imports System.Messaging
Imports System.Diagnostics
Imports System.Configuration
Imports System.Data.SqlClient

' <summary>
' Implements all the mandatory methods required by the functionality
' of the group populator exe 
' </summary>

Module GroupPopulator

    ' Initialize the SQL connection variables to be used throughout 
    ' the application from the groupPopulator.exe.config file
    Dim miisConnectionString As String = _
            ConfigurationSettings.AppSettings("miisConnectionString")
    Dim miisGroupMgmtConnectionString As String = _
                        ConfigurationSettings.AppSettings _
                            ("miisGroupMgmtConnectionString")
    Dim sqlCommandTimeout As Integer = _
                        ConfigurationSettings.AppSettings _
                            ("sqlCommandTimeout")

    ' Initialize the names of the SQL tables from 
    ' the groupPopulator.exe.config file
    Dim groupDefinitionsTable As String = _
        ConfigurationSettings.AppSettings("groupDefinitionsTable")
    Dim groupDefinitions_deltaTable As String = _
        ConfigurationSettings.AppSettings("groupDefinitions_deltaTable")
    Dim exceptionDefinitionsTable As String = _
        ConfigurationSettings.AppSettings("exceptionDefinitionsTable")
    Dim clauseDefinitionsTable As String = _
        ConfigurationSettings.AppSettings("clauseDefinitionsTable")
    Dim memberDefinitionsTable As String = _
        ConfigurationSettings.AppSettings("memberDefinitionsTable")
    Dim memberDefinitions_tempTable As String = _
        ConfigurationSettings.AppSettings("memberDefinitions_tempTable")
    Dim stagingDefinitionsTable As String = _
        ConfigurationSettings.AppSettings("stagingDefinitionsTable")
    Dim attributeGroupDefinitionsTable As String = _
        ConfigurationSettings.AppSettings("attributeGroupDefinitionsTable")
    Dim notificationEnabled As String = _
        ConfigurationSettings.AppSettings("notificationEnabled")
    Dim notificationAttribute As String = _
        ConfigurationSettings.AppSettings("notificationAttribute")
    Dim notificationQueue As String = _
        ConfigurationSettings.AppSettings("notificationQueue")

    ' Initialize the rest of the variables
    Dim includeClause As String = String.Empty
    Dim excludeClause As String = String.Empty
    Dim whereClauseLink As String = String.Empty
    Dim whereClause As String = String.Empty
    Dim groupDisplayName As String = String.Empty
    Dim preserveMemberDays As Double = 0
    Dim groupMembersFromTempTable As New ArrayList
    Dim addGroupToDeltaTable As Boolean
    Dim rowsAffected As Integer = 0
    Dim listOfCommentsFromStagingTable As New ArrayList
    Dim listOfRegeneratedAutoGroups As New ArrayList
    Dim memberHash As Hashtable = New Hashtable
    Dim memberList As ArrayList = New ArrayList

    ' Initialization of database connection variables
    Dim miisGroupMgmtConForNonReader As SqlConnection = Nothing
    Dim miisGroupMgmtConnectionOuter As SqlConnection = Nothing
    Dim miisGroupMgmtConnectionInner As SqlConnection = Nothing
    Dim miisConnection As SqlConnection = Nothing

    ' <summary>
    ' Main entry of the program. Evaluates the cmd line arguments
    ' and implements the program flow accordingly.
    ' </summary>
    ' <param name="args()">cmd line arguments</param>
    ' <returns></returns>

    Sub Main(ByVal args() As String)

        Dim processSuccess As Boolean = False

        Try
            Dim arg As String = String.Empty

            ' Test to see if a previous instance of the process is already running and
            ' exit the process if true
            If PrevInstance() Then
                LogEvent("Process Already Running.  Process Terminating with ErrorLevel = 2", _
                    True, True, 1000, EventLogEntryType.Error)
                Environment.Exit(2)

            End If

            ManageSqlConnections("openAll")

            ' Check for presence of success message in the database.
            ' If this flag is not present, then a
            ' prior run failed and may have left the memberDefinitions table
            ' in an incomplete state, which if reprocessed without intervention 
            ' could result in a large number of delta records which will cause
            ' the next Delta Import to run for an unacceptable amount of time.
            ' If this is detected, Group Populator will attempt to restore
            ' the state of the last run by doing the following if the members_temp table
            ' has records to restore:
            ' 
            '    Restore memberDefinitions_Temp -> memberDefinitions

            If Not (SuccessfulLastRun()) Then
                LogEvent("Success flag was not found." + vbCrLf + vbCrLf + _
                    "A prior run of Group Populator has failed and left the system in an unknown " + _
                    "state.  Group Populator will attempt to restore the state of the last run " + _
                    "by doing the following:" + _
                    vbCrLf + vbCrLf + _
                    "1. Restore memberDefinitions_Temp -> memberDefinitions" + vbCrLf + vbCrLf + _
                    "If this is the first time you are running this application, you can" + _
                    " safely ignore this message.", _
                    True, True, 1002, EventLogEntryType.Error)

                RestoreStateOfLastRun()

            End If

            ' If there are no arguments
            If args.Length = 0 Then
                ProcessStaging()
                InitializeTables()
                PopulateGroups()
            Else

                ' Regenerate automatically generated group 
                ' definitions if /r is present
                For int As Integer = 0 To args.Length - 1

                    arg = args(int).ToLower().TrimStart(New Char() _
                                                {"-", "/"}).Substring(0, 1)

                    If arg = "r" Then
                        RegenerateAttributeGroupDefinitions(args(int). _
                                        Substring(3).Split(New Char() {","}))
                    End If

                Next

                ' Populate (calculate) all group membership if /p is present
                For int As Integer = 0 To args.Length - 1

                    arg = args(int).ToLower().TrimStart(New Char() _
                                                {"-", "/"}).Substring(0, 1)

                    If arg = "p" Then
                        ProcessStaging()
                        InitializeTables()
                        PopulateGroups()
                    End If

                Next

                ' Publish usage information
                For int As Integer = 0 To args.Length - 1

                    arg = args(int).ToLower().TrimStart(New Char() _
                                                {"-", "/"}).Substring(0, 1)

                    If arg = "?" Then
                        Console.WriteLine("")
                        Console.WriteLine("Usage:")
                        Console.WriteLine("")
                        Console.WriteLine("groupPopulator.exe " _
                                        + "[/r:groupUniqueID1,groupUniqueID2] [/p]")
                        Console.WriteLine("")
                        Console.WriteLine("/r Regenerate the individual group " _
                            + "definitions that are created by the attribute " _
                            + "based group definition.")
                        Console.WriteLine("/p Recalculate the group memeberships " _
                            + "for all group definitions")
                        Console.WriteLine("   If no parameters are passed, " _
                            + "groupPopulator.exe will run with the /p option.")
                        Console.WriteLine("")

                        Exit Sub

                    End If

                Next

            End If

            processSuccess = True

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1004, EventLogEntryType.Error)

        Finally
            If processSuccess = True Then
                FlagSuccessfulRun()

                ' Close all the SQL connections before exiting the program
                ManageSqlConnections("closeAll")

            Else
                ' Close all the SQL connections before exiting the program
                ManageSqlConnections("closeAll")
                ' Set errorcode to non zero if exception was caught
                Environment.Exit(1)

            End If

        End Try

    End Sub

    ' <summary>
    ' Initializes and closes the SQL connections to be used 
    ' throughout the program.
    ' </summary>
    ' <param name="action">action (open or close) on the 
    ' to SQL connection </param>
    ' <returns></returns>

    Private Sub ManageSqlConnections(ByVal action As String)

        Try
            Select Case action

                Case "openAll"

                    ' Open the SQL connection to the group management database 
                    ' for SQL statements that don't hold open readers
                    miisGroupMgmtConForNonReader = New _
                            SqlConnection(miisGroupMgmtConnectionString)
                    miisGroupMgmtConForNonReader.Open()

                    ' Open the SQL connection to the group management 
                    ' database for the main reader
                    miisGroupMgmtConnectionOuter = New _
                            SqlConnection(miisGroupMgmtConnectionString)
                    miisGroupMgmtConnectionOuter.Open()

                    ' Open the SQL connection to the group management database 
                    ' for readers that are short lived
                    miisGroupMgmtConnectionInner = New _
                            SqlConnection(miisGroupMgmtConnectionString)
                    miisGroupMgmtConnectionInner.Open()

                    ' Open the SQL connection to the 
                    ' MicrosoftIdentityIntegrationServer Database
                    miisConnection = New SqlConnection(miisConnectionString)
                    miisConnection.Open()

                Case "closeAll"

                    ' Close all connection before exiting the program
                    miisGroupMgmtConnectionOuter.Close()
                    miisGroupMgmtConnectionInner.Close()
                    miisGroupMgmtConForNonReader.Close()
                    miisConnection.Close()

            End Select

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1006, EventLogEntryType.Error)

            Throw exception

        End Try

    End Sub

    ' <summary>
    ' Executes time based commands that will perform operations 
    ' such as group preservation (delayed deletion).
    ' </summary>
    ' <returns></returns>

    Private Sub ProcessStaging()

        ' Declarations
        Dim theCurrentTime As Date = Date.Now

        Dim stagingQuery As String = "SELECT sqlCommand, comment FROM " _
                + stagingDefinitionsTable + " WHERE executeDateTime " + _
                "< '" + theCurrentTime + "' and sqlCommand <> ''"
        Dim stagingCommand As SqlCommand = New _
                SqlCommand(stagingQuery, miisGroupMgmtConnectionInner)
        stagingCommand.CommandTimeout = sqlCommandTimeout
        Dim stagingColumn(2) As Object

        ' stagingColumn object will contain the following values:
        ' sqlCommand (0)
        ' comment (1)
        ' Empty (2)

        Dim stagingReader As SqlDataReader = stagingCommand.ExecuteReader()

        Try

            ' For each row value in the staging reader, execute the command
            While (stagingReader.Read())
                stagingReader.GetValues(stagingColumn)
                If Not stagingReader.IsDBNull(0) AndAlso _
                                        Not stagingColumn(0).Equals("") Then

                    Dim executeString As String = _
                                        stagingColumn(0).ToString().Trim()

                    Dim executeCommand As SqlCommand = _
                        New SqlCommand(executeString, _
                            miisGroupMgmtConForNonReader)
                    rowsAffected = executeCommand.ExecuteNonQuery()
                    executeCommand.CommandTimeout = sqlCommandTimeout

                    Dim commentFromStagingTable As String = String.Empty

                    If Not rowsAffected = 0 Then
                        If Not stagingReader.IsDBNull(1) AndAlso Not _
                            stagingColumn(1).Equals("") Then
                            listOfCommentsFromStagingTable. _
                                Add(stagingColumn(1).ToString().Trim())
                            commentFromStagingTable = _
                                stagingColumn(1).ToString().Trim()
                        End If

                        If Not commentFromStagingTable = String.Empty Then
                            ' Add the group to the delta table so that it gets
                            ' re-evaluated for membership changes
                            Dim arrayOfCommentsFromStagingTable As Array _
                                                = Split(commentFromStagingTable, "|")

                            InsertGroupIntoDeltaTable(arrayOfCommentsFromStagingTable(1))
                        End If

                    Else
                        LogEvent("The following SQL command " + _
                            "did not impact any records - " + executeString, _
                            True, True, 1008, EventLogEntryType.Warning)
                    End If

                End If
            End While

            ' Once all the rows are processed, clean up the table by 
            ' deleting all the unwanted entries
            If stagingReader.HasRows Then

                Dim cleanupString As String = "DELETE FROM " + _
                    stagingDefinitionsTable + " WHERE executeDateTime " + _
                    "< '" + theCurrentTime + "' and sqlCommand <> ''"
                Dim cleanupCommand As SqlCommand = _
                    New SqlCommand(cleanupString, _
                        miisGroupMgmtConForNonReader)
                cleanupCommand.CommandTimeout = sqlCommandTimeout
                rowsAffected = cleanupCommand.ExecuteNonQuery()

                If rowsAffected = 0 Then
                    LogEvent("The cleanup of the processed " + _
                        "staging records was not processed correctly.", _
                        True, True, 1010, EventLogEntryType.Warning)
                End If

            End If

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1012, EventLogEntryType.Error)

            Throw exception

        Finally
            'Close the data reader before exiting the method
            If Not (stagingReader Is Nothing) Then
                stagingReader.Close()
            End If

        End Try

    End Sub

    ' <summary>
    ' The primary method that does the work to find the groups and 
    ' calculate the membership
    ' </summary>
    ' <returns></returns>

    Private Sub PopulateGroups()

        Dim groupUID As String = String.Empty
        Dim groupType As String = String.Empty
        Dim description As String = String.Empty

        Dim mainReader As SqlDataReader = Nothing

        ' Select the list of groups and associated values 
        ' from the group management database

        ' If you are going to add columns to the select statement, 
        ' they should be appended to the end of the list so that 
        ' all of the column numbering does not get impacted

        Try
            Dim mainSelectStatement As String = "SELECT objectUID, " + _
                "displayName, clauseLink, enabledFlag, groupType, " + _
                "description, preserveMembers FROM " + groupDefinitionsTable _
                + " WHERE objectType = 'group' OR objectType = 'groupAuto'"
            Dim mainCommand As SqlCommand = New SqlCommand _
                (mainSelectStatement, miisGroupMgmtConnectionOuter)
            mainCommand.CommandTimeout = sqlCommandTimeout
            Dim sqlColumn(7) As Object

            ' sqlColumn object will contain the following values:
            ' objectUID (0)
            ' displayName (1)
            ' clauseLink (2)
            ' enabledFlag (3)
            ' groupType (4)
            ' description (5)
            ' preserveMembers (6)
            ' Empty (7)

            ' Excecute the sql command that lists the groups and 
            ' associated values from the group management database
            mainReader = mainCommand.ExecuteReader()

            ' This while loop will be executed for each group that is returned
            While (mainReader.Read())
                mainReader.GetValues(sqlColumn)

                ' make sure enabledFlag (enabledFlag) in not null or 
                ' "" and that the value is enabled
                If Not mainReader.IsDBNull(3) AndAlso Not sqlColumn(3). _
                            Equals("") AndAlso sqlColumn(3).ToString(). _
                                            Trim().ToLower = "enabled" Then

                    ' make sure the objectUID, displayName, clauseLink, 
                    ' groupType, description and preserveMembers are not null
                    If Not mainReader.IsDBNull(0) AndAlso Not mainReader. _
                        IsDBNull(1) AndAlso Not mainReader.IsDBNull(2) _
                        AndAlso Not mainReader.IsDBNull(4) AndAlso Not _
                        mainReader.IsDBNull(5) _
                        AndAlso Not mainReader.IsDBNull(5) Then

                        ' make sure the objectUID, displayName, clauseLink and 
                        ' groupType are not "" (it is alright if description 
                        ' and preserveMembers are "" but not NULL)
                        If Not sqlColumn(0).Equals("") AndAlso Not _
                            sqlColumn(1).Equals("") AndAlso Not _
                            sqlColumn(2).Equals("") AndAlso Not _
                            sqlColumn(4).Equals("") Then

                            ' clear variables for each group that goes 
                            ' through the while loop
                            includeClause = String.Empty
                            excludeClause = String.Empty
                            groupUID = String.Empty
                            groupDisplayName = String.Empty
                            whereClauseLink = String.Empty
                            whereClause = String.Empty
                            groupType = String.Empty
                            description = String.Empty
                            preserveMemberDays = 0
                            addGroupToDeltaTable = False
                            memberList.Clear()
                            groupMembersFromTempTable.Clear()

                            ' Set the variables equal to the sqlColumn contents
                            groupUID = sqlColumn(0).ToString().Trim()
                            groupDisplayName = sqlColumn(1).ToString().Trim()
                            whereClauseLink = sqlColumn(2).ToString().Trim()
                            groupType = sqlColumn(4).ToString().Trim()
                            description = sqlColumn(5).ToString().Trim()
                            preserveMemberDays = sqlColumn(6).ToString().Trim()

                            ' Call the sub that gets the associated clause for 
                            ' the group that is being worked on.  The value of 
                            ' whereClauseLink will be the same as the groupUID 
                            ' if the group is using a clause that is specific 
                            ' to that group. If they are different then the  
                            ' clause is shared with another group.
                            GetClause(whereClauseLink)

                            ' As long as a whereClause was returned, continue on
                            If Not whereClause = "" Then

                                ' Call the sub that gets the associated 
                                ' exceptions for the group that is being 
                                ' worked on
                                GetExceptions(groupUID)

                                ' Ensure that we only return person objects 
                                ' to be members of the groups
                                Dim whereSuffix As String = _
                                        ") and object_type = 'person' "

                                ' Build the final where clause
                                whereClause = whereClause + excludeClause + _
                                                includeClause + whereSuffix

                                ' Call the sub that calculates the new group 
                                ' membership
                                GetMembers(whereClause, groupDisplayName)

                                ' Call the sub that gets the old group  
                                ' membership from the temp table
                                GetOldMembers(groupUID)

                                Dim memberID As String = String.Empty

                                ' Write the group members
                                For Each memberID In memberList

                                    Dim insertPersonObjectsStatement As _
                                        String = "INSERT INTO " + _
                                        memberDefinitionsTable + _
                                        "(objectUID, objectType, mvObjectUID)" + _
                                        "VALUES('" + groupUID.Trim() + "', " + _
                                        "'member', '{" + memberID + "}')"
                                    Dim insertPersonObjectsCommand As _
                                        SqlCommand = New SqlCommand( _
                                        insertPersonObjectsStatement, _
                                        miisGroupMgmtConForNonReader)
                                    insertPersonObjectsCommand.CommandTimeout = _
                                        sqlCommandTimeout
                                    rowsAffected = _
                                        insertPersonObjectsCommand. _
                                                            ExecuteNonQuery()

                                    If rowsAffected = 0 Then
                                        LogEvent("There was an " + _
                                            "error adding a member to the " + _
                                            groupDisplayName + " group.", _
                                            True, True, 1014, EventLogEntryType.Warning)
                                    End If

                                    ProcessAddedMembers(memberID)

                                Next

                                ' process deleted users
                                ProcessDeletedMembers(groupUID, _
                                                preserveMemberDays)

                                If addGroupToDeltaTable = True Then
                                    InsertGroupIntoDeltaTable(groupUID)
                                End If

                            Else
                                LogEvent("A valid where clause " + _
                                "was not found for one or more groups - " + _
                                groupDisplayName, True, True, 1016, _
                                EventLogEntryType.Warning)
                            End If
                        Else
                            LogEvent("Either the objectUID, " + _
                                "displayName, groupType or clauseLink is " + _
                                "blank for one or more groups", _
                                True, True, 1018, EventLogEntryType.Warning)
                        End If
                    Else
                        LogEvent("Either the objectUID, " + _
                            "displayName, description, groupType or " + _
                                "clauseLink is null for one or more groups", _
                                True, True, 1020, EventLogEntryType.Warning)
                    End If

                End If

            End While

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1020, EventLogEntryType.Error)

            Throw exception

        Finally
            If Not (mainReader Is Nothing) Then
                mainReader.Close()
            End If

        End Try

    End Sub

    ' <summary>
    ' Manipulates tables so that when the program runs, 
    ' it can utilize the clean tables and copies of the information 
    ' as it was in the prior run.
    ' </summary>
    ' <returns></returns>

    Private Sub InitializeTables()

        Try

            ' Delete all person entries out of the primary table
            Dim delPersonsStatement As String = _
                    "DELETE FROM " + groupDefinitionsTable + _
                    " WHERE objectType = 'person'"

            Dim delPersonsCommand As SqlCommand = _
                New SqlCommand(delPersonsStatement, miisGroupMgmtConForNonReader)
            delPersonsCommand.CommandTimeout = sqlCommandTimeout

            rowsAffected = delPersonsCommand.ExecuteNonQuery()

            ' Delete all members from the members temp table
            Dim delTempStatement As String = "DELETE FROM " + _
                                        memberDefinitions_tempTable

            Dim delTempCommand As SqlCommand = _
                    New SqlCommand(delTempStatement, _
                        miisGroupMgmtConForNonReader)
            delTempCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = delTempCommand.ExecuteNonQuery()

            ' Copy all members from the members table to the temp table
            Dim copyMembersStatement As String = "INSERT INTO " + _
                                        memberDefinitions_tempTable + _
                                        " SELECT * FROM " _
                                        + memberDefinitionsTable

            Dim copyMembersCommand As SqlCommand = _
                    New SqlCommand(copyMembersStatement, _
                        miisGroupMgmtConForNonReader)
            copyMembersCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = copyMembersCommand.ExecuteNonQuery()

            ' Delete all members from the members table
            Dim delMembersStatement As String = "DELETE FROM " + _
                                        memberDefinitionsTable + _
                                        " WHERE objectType = 'member'"
            Dim delMembersCommand As SqlCommand = _
                    New SqlCommand(delMembersStatement, _
                        miisGroupMgmtConForNonReader)
            delMembersCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = delMembersCommand.ExecuteNonQuery()

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1022, EventLogEntryType.Error)

            Throw exception

        End Try

    End Sub

    ' <summary>
    ' Restores the memberDefinitions table from the membersDefinitions_temp table
    ' (state from start of last run).
    ' </summary>
    ' <returns></returns>

    Private Sub RestoreStateOfLastRun()

        Try
            ' Delete all members from the members table
            Dim delMembersStatement As String = "DELETE FROM " + _
                                        memberDefinitionsTable

            Dim delMembersCommand As SqlCommand = _
                    New SqlCommand(delMembersStatement, _
                        miisGroupMgmtConForNonReader)
            delMembersCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = delMembersCommand.ExecuteNonQuery()

            ' Copy all members from the members temp table to the members table
            Dim copyMembersStatement As String = "INSERT INTO " + _
                                        memberDefinitionsTable + _
                                        " SELECT * FROM " _
                                        + memberDefinitions_tempTable

            Dim copyMembersCommand As SqlCommand = _
                    New SqlCommand(copyMembersStatement, _
                        miisGroupMgmtConForNonReader)
            copyMembersCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = copyMembersCommand.ExecuteNonQuery()

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1024, EventLogEntryType.Error)

            Throw exception

        End Try

    End Sub

    ' <summary>
    ' Selects the members from the temp table to compare to the 
    ' new members. This will assist in figuring out the changes 
    ' since the last run.
    ' </summary>
    ' <param name="groupUID">ID of the group in comparison</param>
    ' <returns></returns>

    Private Sub GetOldMembers(ByVal groupUID As String)

        Dim grpMembersFromTempTableQuery As String = _
        "SELECT mvObjectUID FROM " + memberDefinitions_tempTable + _
        " WHERE objectUID = '" + groupUID + "'"
        Dim grpMembersFromTempTableCmd As SqlCommand = _
                New SqlCommand(grpMembersFromTempTableQuery, _
                                miisGroupMgmtConnectionInner)
        grpMembersFromTempTableCmd.CommandTimeout = sqlCommandTimeout
        Dim sqlColumnHistory(1) As Object

        ' sqlColumnHistory object will contain the following values:
        ' mvObjectUID (0)
        ' Empty (1)

        Dim grpMembersFromTempTableReader As SqlDataReader = _
                    grpMembersFromTempTableCmd.ExecuteReader()

        Try

            While grpMembersFromTempTableReader.Read()

                ' For each entry in the grpMembersFromTmpTableReader,
                ' add the history column to the groupMembersFromTempTable
                grpMembersFromTempTableReader.GetValues(sqlColumnHistory)
                If Not grpMembersFromTempTableReader.IsDBNull(0) _
                            AndAlso Not sqlColumnHistory(0).Equals("") Then

                    groupMembersFromTempTable.Add(sqlColumnHistory(0). _
                                                ToString().ToUpper.Trim())

                End If
            End While

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1026, EventLogEntryType.Error)

            Throw exception

        Finally

            If Not (grpMembersFromTempTableReader Is Nothing) Then
                grpMembersFromTempTableReader.Close()
            End If

        End Try

    End Sub

    ' <summary>
    ' Take action on the users that were added to the group since 
    ' the last run.</summary>
    ' <param name="groupUID">ID of the group in comparison</param>
    ' <param name="memberID">ID of the group member</param>
    ' <returns></returns>

    Private Sub ProcessAddedMembers(ByVal memberID As String)

        Try

            ' Check if the member was in the group by comparing the user to
            ' the groupMembersFromTempTable arraylist. If so (if they were in
            ' the groupMembersFromTempTable arraylist) then delete then from 
            ' the groupMembersFromTempTable arraylist. If not then add the 
            ' group that is being worked on to delta table, setting a flag so 
            ' that group gets added once.

            Dim memberWasInOldGroup As Boolean = False

            For int As Integer = 0 To groupMembersFromTempTable.Count - 1
                If memberWasInOldGroup = False AndAlso "{" + memberID + "}" _
                                    = groupMembersFromTempTable(int) Then

                    ' This member was already present so remove them from the
                    ' working list. Left out users in the working list
                    ' will be optionally preserved later
                    groupMembersFromTempTable.RemoveAt(int)

                    memberWasInOldGroup = True

                End If
            Next

            If memberWasInOldGroup = False Then

                ' If there is a new member then add the group to the 
                ' delta table so that it will be evaluated
                If addGroupToDeltaTable = False Then
                    addGroupToDeltaTable = True
                End If

                ' Since they are a new member, add the user to the 
                ' delta table so that it will be evaluated
                Dim insertPersonObjectsStatement As String = "INSERT INTO " _
                + groupDefinitions_deltaTable + " (objectUID, objectType, " _
                + "changeTime, changeType) VALUES ('{" + memberID + "}', " _
                + "'person', '" + Date.Now + "','Add')"
                Dim insertPersonObjectsCommand As SqlCommand = New SqlCommand _
                (insertPersonObjectsStatement, miisGroupMgmtConForNonReader)
                rowsAffected = insertPersonObjectsCommand.ExecuteNonQuery()
                insertPersonObjectsCommand.CommandTimeout = sqlCommandTimeout
                If rowsAffected = 0 Then
                    LogEvent("There was an error adding " _
                    + memberID + " into the delta groupDefinition table.  " _
                    + "This will likely result in that user not being added " _
                    + "to any enabled groups.", True, True, 1028, _
                    EventLogEntryType.Warning)
                End If

                ' Send a notification if notifications are enabled
                If notificationEnabled.ToLower = "true" Then

                    Dim action As String = "Added to"
                    SendNotification(action, groupDisplayName, _
                                                    "{" + memberID + "}")

                End If

            End If

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1030, EventLogEntryType.Error)

            Throw exception

        End Try

    End Sub

    ' <summary>
    ' Any member which was deleted since the last time must
    ' be preserved and/or notified.
    ' </summary>
    ' <param name="groupUID">group id of the deleted member</param>
    ' <param name="preserveMemberDays">Days of membership preservation</param>
    ' <returns></returns>

    Private Sub ProcessDeletedMembers _
            (ByVal groupUID As String, ByVal preserveMemberDays As String)

        Dim validateExceptionStatusReader As SqlDataReader

        Try
            ' After processing the last of the members that are going 
            ' to be in the current group definition, preserve all members
            ' that are left in the groupMembersFromTempTable arraylist

            ' If a user is going to be preserved, they need to be added
            ' back into the group so that they are included
            ' after this run of the groupPopulator is done

            Dim commentFromStagingTable As String = String.Empty

            ' This code block reads the comment information that originated
            ' from the comment column in the stagingDefinitions table.
            ' The comments include the users that have expired as
            ' being an includeAuto member. If a user expires, they should
            ' not be added back into the group again as an includeAuto
            ' member, so we take them out of the list of potential people to
            ' add back into the group.

            ' If there are comments
            If listOfCommentsFromStagingTable.Count > 0 Then

                For Each commentFromStagingTable In listOfCommentsFromStagingTable

                    Dim infoForMemtShldNotIncAutoAgain As Array _
                                        = Split(commentFromStagingTable, "|")
                    Dim memInListOfGrpMemFromTmpTable As Boolean = False

                    If infoForMemtShldNotIncAutoAgain(0) = "includeAuto" Then

                        For int As Integer = 0 To groupMembersFromTempTable.Count - 1

                            If memInListOfGrpMemFromTmpTable = _
                                False AndAlso _
                                infoForMemtShldNotIncAutoAgain(1) = _
                                groupUID.ToUpper AndAlso _
                                infoForMemtShldNotIncAutoAgain(2) = _
                                groupMembersFromTempTable(int) Then

                                ' Notify all of the users that they have been
                                ' removed from the group after their expiration
                                ' date passed
                                Dim action As String = "Expired from"
                                SendNotification(action, groupDisplayName, _
                                            groupMembersFromTempTable(int))

                                groupMembersFromTempTable.RemoveAt(int)
                                memInListOfGrpMemFromTmpTable = True

                            End If

                        Next

                    End If

                Next

            End If

            ' Delete all members from the potential preserve list 
            ' whose time just expired as being an includeAuto

            ' Preserve all the members that were just deleted from
            ' the group by processing the leftover users in the
            ' groupMembersFromTempTable(Array)
            Dim objectsToBeRemoved As New ArrayList
            For int As Integer = 0 To groupMembersFromTempTable.Count - 1

                ' Build a SQL query to see if the user was deleted out of
                ' the group because they were added as a manual exclusion
                Dim validateExceptionStatusQuery As String = _
                                "SELECT objectUID FROM " _
                                + exceptionDefinitionsTable _
                                + " WHERE objectUID = '" _
                                + groupUID + "' AND " _
                                + "exceptType = 'exclude' AND mvObjectUID = " _
                                + "'" + groupMembersFromTempTable(int) + "'"
                Dim validateExceptionStatusCommand As SqlCommand = _
                        New SqlCommand(validateExceptionStatusQuery, _
                                    miisGroupMgmtConnectionInner)
                validateExceptionStatusCommand.CommandTimeout = sqlCommandTimeout

                ' sqlColumnHistory object will contain the following values:
                ' mvObjectUID (0)
                ' Empty (1)

                validateExceptionStatusReader = _
                                validateExceptionStatusCommand.ExecuteReader()

                ' If they are not a manual exclusion
                If Not validateExceptionStatusReader.HasRows Then

                    If preserveMemberDays > 0 Then

                        ' Insert the user into the exception table as 
                        ' an includeAuto
                        Dim insertAutoExceptionStatement As String = _
                            "INSERT INTO " + exceptionDefinitionsTable + _
                            " (objectUID, exceptType, mvObjectUID) VALUES " + _
                            "('" + groupUID.ToUpper + "','includeAuto','" _
                            + groupMembersFromTempTable(int).ToUpper + "')"

                        Dim insertAutoExceptionCommand As SqlCommand = _
                            New SqlCommand(insertAutoExceptionStatement, _
                                miisGroupMgmtConForNonReader)
                        insertAutoExceptionCommand.CommandTimeout = sqlCommandTimeout
                        rowsAffected = _
                            insertAutoExceptionCommand.ExecuteNonQuery()

                        If rowsAffected = 0 Then
                            LogEvent("There was an error adding " + _
                            "user GUID " + groupMembersFromTempTable(int). _
                            ToUpper + " as an includeAuto member of group " + _
                            "GUID " + groupUID.ToUpper + ".", True, True, 1032, _
                            EventLogEntryType.Warning)
                        End If

                        ' Insert a row into the staging table so that they 
                        ' will be cleaned up in preserveMemberDays days
                        Dim insertStagingInfoStatement As String = _
                            "INSERT INTO " + stagingDefinitionsTable + _
                            " (executeDateTime, sqlCommand, comment) " + _
                            "VALUES ('" + Date.Now.AddDays _
                            (preserveMemberDays) + "', " + _
                            "'DELETE FROM " + exceptionDefinitionsTable _
                            + " " + "WHERE objectUID = ''" + _
                            groupUID + "'' AND " + _
                            "exceptType = ''includeAuto''" + _
                            "AND mvObjectUID = ''" + groupMembersFromTempTable(int) + _
                            "''','includeAuto|" + groupUID.ToUpper + "|" + _
                            groupMembersFromTempTable(int).ToUpper + "')"

                        Dim insertStagingInfoCommand As SqlCommand = _
                                New SqlCommand(insertStagingInfoStatement, _
                                    miisGroupMgmtConForNonReader)
                        insertStagingInfoCommand.CommandTimeout = sqlCommandTimeout
                        rowsAffected = _
                                insertStagingInfoCommand.ExecuteNonQuery()

                        If rowsAffected = 0 Then
                            LogEvent("There was an error adding " + _
                            "the cleanup data to the staging table for " + _
                            "user GUID " + _
                            groupMembersFromTempTable(int).ToUpper + _
                            " and group GUID " + _
                            groupUID.ToUpper + ".  This " + _
                            "may result in that user being a perminant " + _
                            "included member of the group.", True, True, 1034, _
                            EventLogEntryType.Warning)
                        End If

                        ' Make sure that they are added back to the group
                        ' and included in this run
                        Dim insertMemBackIntoGrpStatement As String = _
                            "INSERT INTO " + memberDefinitionsTable + _
                            " (objectUID, objectType, mvObjectUID) VALUES " + _
                            "('" + groupUID + "','member','" + _
                            groupMembersFromTempTable(int) + "')"

                        Dim insertMemBackIntoGrpCommand As _
                            SqlCommand = New SqlCommand( _
                                insertMemBackIntoGrpStatement, _
                                miisGroupMgmtConForNonReader)
                        insertMemBackIntoGrpCommand.CommandTimeout = sqlCommandTimeout
                        rowsAffected = insertMemBackIntoGrpCommand.ExecuteNonQuery()

                        If rowsAffected = 0 Then
                            LogEvent("There was an error adding " + _
                            "user GUID " + _
                            groupMembersFromTempTable(int).ToUpper + _
                            " back into group GUID " + groupUID.ToUpper + _
                            " so that they will be a member " + _
                            "durring this cycle.", True, True, 1036, _
                            EventLogEntryType.Warning)
                        End If

                        ' Notify perserved members that they have been
                        ' preserved for preserveMemberDays days
                        Dim action As String = "Preserved for " + _
                                                preserveMemberDays + " days in"
                        SendNotification(action, groupDisplayName, _
                                                groupMembersFromTempTable(int))

                        ' Removes the user from the groupMembersFromTempTable
                        ' (used later for notification) if
                        ' the user became an includeAuto
                        objectsToBeRemoved.Add(groupMembersFromTempTable(int))

                    End If

                End If

                If Not (validateExceptionStatusReader Is Nothing) Then
                    validateExceptionStatusReader.Close()
                End If

            Next

            For Each objectToBeRemoved As String In objectsToBeRemoved
                groupMembersFromTempTable.Remove(objectToBeRemoved)
            Next

            If groupMembersFromTempTable.Count > 0 Then

                addGroupToDeltaTable = True

                If notificationEnabled.ToLower = "true" Then
                    For int As Integer = 0 To groupMembersFromTempTable.Count - 1
                        ' Notifies the rest of the users that they were
                        ' removed from the group
                        Dim action As String = "Removed from"
                        SendNotification(action, groupDisplayName, _
                                            groupMembersFromTempTable(int))
                    Next
                End If

            End If

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1038, EventLogEntryType.Error)

            Throw exception

        Finally
            If Not (validateExceptionStatusReader Is Nothing) Then
                validateExceptionStatusReader.Close()
            End If

        End Try

    End Sub

    ' <summary>
    ' Add group into the delta table for delta imports.
    ' </summary>
    ' <param name="groupUID">ID of the group to be inserted 
    ' into Delta table</param>
    ' <returns></returns>

    Private Sub InsertGroupIntoDeltaTable(ByVal groupUID As String)

        Try
            ' If addGroupToDeltaTable is true, then add group
            ' to delta table because the membership has changed
            Dim copyGroupToDeltaStatement As String = _
                "INSERT INTO " + groupDefinitions_deltaTable + _
                " (objectUID, groupAutoUID, objectType, displayName, " + _
                "description, clauseLink, enabledFlag, maxExcept, " + _
                "preserveMembers, groupType, mailNickName) " + _
                "SELECT objectUID, groupAutoUID, objectType, " + _
                "displayName, description, " + _
                "clauseLink, enabledFlag, maxExcept, preserveMembers, " + _
                "groupType, mailNickName FROM " + groupDefinitionsTable + _
                " WHERE objectUID = '" + groupUID + "'"

            Dim copyGroupToDeltaCommand As SqlCommand = _
                    New SqlCommand(copyGroupToDeltaStatement, _
                        miisGroupMgmtConForNonReader)
            copyGroupToDeltaCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = copyGroupToDeltaCommand.ExecuteNonQuery()

            If rowsAffected = 0 Then
                LogEvent("There was an error adding the " _
                        + groupDisplayName + " group to the delta table.", _
                        True, True, 1040, EventLogEntryType.Warning)
            End If

            ' Execute the update command on groupDefinitions_deltaTable
            Dim updateGroupInDeltaStatement As String = "UPDATE " _
                + groupDefinitions_deltaTable + " SET attributeName = " + _
                "'member', changeTime = '" + Date.Now + "', changeType " + _
                "= 'Modify_Attribute' WHERE " + _
                "objectUID = '" + groupUID + "' " + _
                "and changeType is null"

            Dim updateGroupInDeltaCommand As SqlCommand = _
                    New SqlCommand(updateGroupInDeltaStatement, _
                            miisGroupMgmtConForNonReader)
            updateGroupInDeltaCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = updateGroupInDeltaCommand.ExecuteNonQuery()

            If rowsAffected = 0 Then
                LogEvent("There was an error updating the " + _
                groupDisplayName + _
                " group after it was added to the delta table.", True, True, 1042, _
                EventLogEntryType.Warning)
            End If

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1044, EventLogEntryType.Error)

            Throw exception

        End Try

    End Sub

    ' <summary>
    ' Send email notfication via MSMQ if enabled in the XML config file
    ' </summary>
    ' <param name="action">action leading to notification</param>
    ' <param name="groupDisplayName">Display name of the group</param>
    ' <param name="mvObjectUID">metaverse object id</param>
    ' <returns></returns>

    Private Sub SendNotification(ByVal action As String, _
            ByVal groupDisplayName As String, ByVal mvObjectUID As String)

        ' Open a new reader for the MIIS database
        Dim emailReader As SqlDataReader = Nothing

        Try

            If notificationEnabled.ToLower = "true" Then

                ' Open a private queue named AccountProvisioning 
                ' on the local machine (".")
                Dim queue As MessageQueue = _
                    New MessageQueue(notificationQueue)

                ' Create a new message
                Dim msg As Message = New Message

                ' Give a meaningful label so the receiver quickly can
                ' see what it is without reading the body
                Dim subject As String = action + " group " + groupDisplayName
                Dim email As String = String.Empty

                msg.Label = subject

                ' Select the list of person objects from the metaverse
                ' that match the whereClause created earlier.
                ' To be safe make sure that the queries against
                ' the metaverse are executed with nolock
                Dim emailSelectStatement As String = "SELECT " _
                    + notificationAttribute + " FROM mms_metaverse " + _
                    "WITH (NOLOCK) WHERE object_id = '" + mvObjectUID + "'"
                Dim emailCommand As SqlCommand = New _
                            SqlCommand(emailSelectStatement, miisConnection)
                emailCommand.CommandTimeout = sqlCommandTimeout
                Dim emailColumn(3) As Object

                ' sqlColumn object will contain the following values:
                ' object_id (0)
                ' uid (1)
                ' Empty (2)

                ' Execute sql command
                emailReader = emailCommand.ExecuteReader()

                ' This while loop will be executed for each member
                ' that is returned (includes exceptions)
                While (emailReader.Read())
                    emailReader.GetValues(emailColumn)

                    ' check if object_id and cn are not null or
                    ' "" and add to arraylist
                    If Not emailReader.IsDBNull(0) AndAlso Not _
                                            emailColumn(0).Equals("") Then

                        ' The guid is used to join the object
                        ' to the metaverse object
                        email = emailColumn(0).ToString().ToUpper().Trim()
                    Else
                        LogEvent("User with MV object-id " + _
                            mvObjectUID + " does not have an email address.", _
                            True, True, 1046, EventLogEntryType.Warning)
                        Exit Sub
                    End If

                End While

                ' Write some XML data into the body
                msg.Body = _
                    BuildXMLData(subject, action, groupDisplayName, email)

                ' Send the message which informs MSMQ to commit the
                ' transaction (TransactionType = Single message)
                queue.Send(msg, MessageQueueTransactionType.Single)

            End If

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1048, EventLogEntryType.Error)

            Throw exception

        Finally
            If Not (emailReader Is Nothing) Then
                emailReader.Close()
            End If

        End Try

    End Sub

    ' <summary>
    ' Initializes and closes the SQL connections that are 
    ' used throughout the program.
    ' </summary>
    ' <param name="subject">subject of the mail</param>
    ' <param name="action">action corresponding to notification</param>
    ' <param name="groupDisplayName">display name of the group</param>
    ' <param name="email">notificating email</param>
    ' <returns>String</returns>

    Function BuildXMLData(ByVal subject As String, _
        ByVal action As String, ByVal groupDisplayName As String, _
        ByVal email As String) As String

        Try

            Dim body As StringWriter = New StringWriter
            Dim writer As XmlTextWriter = New XmlTextWriter(body)

            writer.WriteStartDocument()
            writer.WriteStartElement("account")
            writer.WriteAttributeString("subject", subject)
            writer.WriteAttributeString("action", action)
            writer.WriteAttributeString("groupDisplayName", groupDisplayName)
            writer.WriteAttributeString("email", email)
            writer.WriteEndElement()
            writer.WriteEndDocument()

            Return body.ToString()

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1050, EventLogEntryType.Error)

            Throw exception

        End Try

    End Function

    ' <summary>
    ' Gets the associated clause for the group that is being worked upon
    ' worked upon</summary>
    ' <param name="whereClauseLink">clause definition of a group</param>
    ' <returns></returns>

    Private Sub GetClause(ByVal whereClauseLink As String)

        ' Select the list of clauses from the clauseDefinitionsTable
        ' that match the whereClauseLink
        Dim clauseQuery As String = "SELECT clause FROM " _
                        + clauseDefinitionsTable + _
                        " WHERE objectUID = '" + whereClauseLink + "'"

        Dim clauseCommand As SqlCommand = New SqlCommand _
                    (clauseQuery, miisGroupMgmtConnectionInner)
        clauseCommand.CommandTimeout = sqlCommandTimeout
        Dim sqlColumnClause(1) As Object

        ' sqlColumnClause object will contain the following values:
        ' clause (0)
        ' Empty (1)

        ' Execute sql command
        Dim clauseReader As SqlDataReader = clauseCommand.ExecuteReader()

        Try

            ' This while loop will be executed for each clause that is
            ' returned (should be one) If there are multiple clauses
            ' returned it will use the last one that is read, this could
            ' cause support confusion, but might be better than throwing 
            ' an error
            While (clauseReader.Read())
                clauseReader.GetValues(sqlColumnClause)

                ' check if clause is not null or "" and build where statement
                If Not clauseReader.IsDBNull(0) AndAlso Not _
                                        sqlColumnClause(0).Equals("") Then
                    whereClause = "(" + sqlColumnClause(0).ToString().Trim()
                Else
                    ' If there are no values returned
                    LogEvent("A valid where clause was not " + _
                                "found for this group - " + groupDisplayName, True, _
                                True, 1052, EventLogEntryType.Warning)
                End If

            End While

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1054, EventLogEntryType.Error)

            Throw exception

        Finally
            If Not (clauseReader Is Nothing) Then
                clauseReader.Close()
            End If

        End Try

    End Sub

    ' <summary>
    ' Gets the associated exceptions for the group that is worked upon
    ' exceptions are explicit includes and excludes for the group
    ' </summary>
    ' <param name="groupUID">Id of the group whose associated
    ' exceptions are being caught</param>
    ' <returns></returns>

    Private Sub GetExceptions(ByVal groupUID As String)

        ' Select the list of member exceptions from the
        ' exceptionDefinitionsTable that match the groupUID
        Dim exceptionsQuery As String = "SELECT exceptType, " + _
                "mvObjectUID FROM " + exceptionDefinitionsTable + _
                                " WHERE objectUID = '" + groupUID + "'"
        Dim exceptionsCommand As SqlCommand = _
                    New SqlCommand(exceptionsQuery, _
                            miisGroupMgmtConnectionInner)
        exceptionsCommand.CommandTimeout = sqlCommandTimeout
        Dim sqlColumnExceptions(2) As Object

        ' sqlColumnExceptions object will contain the following values:
        ' exceptType (0)
        ' mvObjectUID(1)
        ' Empty (2)

        ' Execute sql command
        Dim exceptionsReader As SqlDataReader = _
                                exceptionsCommand.ExecuteReader()

        Try

            ' This while loop will be executed for each exception
            ' that is returned (both includes and excludes)
            While (exceptionsReader.Read())
                exceptionsReader.GetValues(sqlColumnExceptions)

                ' check if exceptType and mvObjectUID are not
                ' null or "" and build the statements
                If Not exceptionsReader.IsDBNull(0) AndAlso Not _
                    sqlColumnExceptions(0).Equals("") AndAlso Not _
                    exceptionsReader.IsDBNull(1) AndAlso Not _
                    sqlColumnExceptions(1).Equals("") Then
                    Select Case sqlColumnExceptions(0).ToString().Trim().ToLower

                        Case "include", "includeauto"

                            includeClause = includeClause + _
                                " or object_id = '" _
                                + sqlColumnExceptions(1).ToString().Trim() + "' "

                        Case "exclude"

                            excludeClause = excludeClause + _
                                " and object_id <> '" _
                                + sqlColumnExceptions(1).ToString().Trim() + "' "

                        Case Else
                            ' if there is anything but include or exclude
                            ' returned from the mvObjectUID
                            LogEvent("exceptType does not " + _
                                "contain the value include, " + _
                                "includeAuto or exclude.", True, True, 1056, _
                                EventLogEntryType.Warning)
                    End Select
                End If

            End While

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1058, EventLogEntryType.Error)

            Throw exception

        Finally
            If Not (exceptionsReader Is Nothing) Then
                exceptionsReader.Close()
            End If

        End Try

    End Sub

    ' <summary>
    ' Calculates the group membership based on a query to the 
    ' metaverse table</summary>
    ' <param name="whereClause">clause used to check group membership
    ' </param>
    ' <param name="groupDisplayName">Display name of the group</param>
    ' <returns></returns>

    Private Sub GetMembers(ByVal whereClause As String, _
                                ByVal groupDisplayName As String)

        ' Open a new reader for the MIIS database
        Dim memberReader As SqlDataReader = Nothing

        Dim displayname As String = String.Empty
        Dim objectid As String = String.Empty

        ' Select the list of person objects from the metaverse that match
        ' the whereClause created earlier. To be safe make sure that the
        ' queries against the metaverse are executed using - with (nolock)

        Dim memberSelectStatement As String = "SELECT object_id " + _
            "FROM mms_metaverse WITH (NOLOCK) WHERE " + whereClause

        Dim memberCommand As SqlCommand = _
                New SqlCommand(memberSelectStatement, miisConnection)
        memberCommand.CommandTimeout = sqlCommandTimeout
        Dim sqlColumn(2) As Object

        ' sqlColumn object will contain the following values:
        ' object_id (0)
        ' Empty (1)

        Try

            ' Execute sql command
            memberReader = memberCommand.ExecuteReader()

            ' This while loop will be executed for each member
            ' that is returned (including exceptions)
            While (memberReader.Read())
                memberReader.GetValues(sqlColumn)

                ' Check if object_id and cn are not null or ""
                ' and add to arraylist
                If Not memberReader.IsDBNull(0) AndAlso Not _
                    sqlColumn(0).Equals("") Then

                    ' The guid is used to join the object
                    ' to the metaverse object
                    objectid = sqlColumn(0).ToString().ToUpper().Trim()

                    ' Add the person objectid to the arraylist
                    memberList.Add(objectid)

                    ' hash item is being used to control uniqueness, so that
                    ' person objects are only added to the table once
                    If Not memberHash.ContainsKey(objectid) Then

                        ' Add the person objectid to the hashtable
                        memberHash.Add(objectid, "")

                        ' Add the person to the SQL table
                        Dim insertPersonObjectsStatement As String = _
                            "INSERT INTO " + groupDefinitionsTable + _
                            " (objectUID, objectType) " + _
                            "VALUES ('{" + objectid + "}', 'person')"

                        Dim insertPersonObjectsCommand As SqlCommand = _
                                New SqlCommand(insertPersonObjectsStatement, _
                                miisGroupMgmtConForNonReader)
                        insertPersonObjectsCommand.CommandTimeout = sqlCommandTimeout
                        rowsAffected = _
                            insertPersonObjectsCommand.ExecuteNonQuery()

                        If rowsAffected = 0 Then
                            LogEvent("There was an error adding " _
                            + objectid + " into the membership table. " + _
                            "This will likely result in that user not " + _
                            "being added to any enabled groups.", True, True, 1060, _
                            EventLogEntryType.Warning)
                        End If

                    End If

                End If

            End While

        Catch exception As Exception
            LogEvent("Group '" + groupDisplayName + _
                "' does not have a valid whereclause.", True, True, 1062, EventLogEntryType.Warning)

        Finally
            If Not (memberReader Is Nothing) Then
                memberReader.Close()
            End If

        End Try

    End Sub

    ' <summary>
    ' Based on the cmd line arguments, regenerate the groups 
    ' that are passed in</summary>
    ' <param name="groupDefinitions">Definitions of the group which
    ' is being regenerated</param>
    ' <returns></returns>

    Private Sub RegenerateAttributeGroupDefinitions( _
                    ByVal groupDefinitions As String())

        For int As Integer = 0 To groupDefinitions.Length - 1

            ' Open a new reader for the miisGroupManagement database
            Dim attributeGroupReader As SqlDataReader = Nothing

            ' Select the list of person objects from the metaverse
            ' that match the whereClause created earlier
            Dim attributeGroupSelectStatement As String = _
                "SELECT uniqueGroupID, attributeGroupType, attribute, " + _
                "displayName, groupType, mailEnabled, linkAttribute, " + _
                "linkAttributeKey FROM " + attributeGroupDefinitionsTable + _
                " WHERE uniqueGroupID = " + _
                "'" + groupDefinitions(int).ToLower() + "'"

            Dim attributeGroupCommand As SqlCommand = _
                    New SqlCommand(attributeGroupSelectStatement, _
                                    miisGroupMgmtConnectionOuter)
            attributeGroupCommand.CommandTimeout = sqlCommandTimeout

            Dim attributeGroupSQLColumn(8) As Object

            ' sqlColumn object will contain the following values:
            ' uniqueGroupID (0)
            ' attributeGroupType (1)
            ' attribute (2)
            ' displayName (3)
            ' groupType (4)
            ' mailEnabled (5)
            ' linkAttribute (6)
            ' linkAttributeKey (7)
            ' empty (8)

            ' Execute sql command
            attributeGroupReader = attributeGroupCommand.ExecuteReader()

            Try

                If attributeGroupReader.HasRows Then

                    ' This while loop will be executed for each member that
                    ' is returned (including exceptions)
                    While (attributeGroupReader.Read())
                        attributeGroupReader.GetValues(attributeGroupSQLColumn)

                        ' make sure that all required attributes are available
                        If Not attributeGroupReader.IsDBNull(0) AndAlso Not _
                        attributeGroupSQLColumn(0).Equals("") AndAlso Not _
                        attributeGroupReader.IsDBNull(1) AndAlso Not _
                        attributeGroupSQLColumn(1).Equals("") AndAlso Not _
                        attributeGroupReader.IsDBNull(2) AndAlso Not _
                        attributeGroupSQLColumn(2).Equals("") AndAlso Not _
                        attributeGroupReader.IsDBNull(3) AndAlso Not _
                        attributeGroupSQLColumn(3).Equals("") AndAlso Not _
                        attributeGroupReader.IsDBNull(4) AndAlso Not _
                        attributeGroupSQLColumn(4).Equals("") AndAlso Not _
                        attributeGroupReader.IsDBNull(5) AndAlso Not _
                        attributeGroupSQLColumn(5).Equals("") Then

                            Dim uniqueGroupID As String = _
                                attributeGroupSQLColumn(0).ToString().ToLower().Trim()
                            Dim attributeGroupType As String = _
                                attributeGroupSQLColumn(1).ToString().ToLower().Trim()
                            Dim attribute As String = _
                                attributeGroupSQLColumn(2).ToString().ToLower().Trim()
                            Dim displayName As String = _
                                attributeGroupSQLColumn(3).ToString().Trim()
                            Dim groupType As String = _
                                attributeGroupSQLColumn(4).ToString().ToLower().Trim()
                            Dim mailEnabled As Boolean = _
                                attributeGroupSQLColumn(5).ToString().ToLower().Trim()

                            ' For all the attributes group types, 
                            ' execute various activities
                            Select Case attributeGroupType.ToLower().Trim()

                                Case "single"
                                    RegenerateSingleAttributeGroup _
                                    (uniqueGroupID, attribute, displayName, _
                                                    groupType, mailEnabled)

                                Case "linked"
                                    If Not attributeGroupReader.IsDBNull(6) _
                                    AndAlso Not _
                                    attributeGroupSQLColumn(6).Equals("") _
                                    AndAlso Not attributeGroupReader.IsDBNull(7) _
                                    AndAlso Not _
                                    attributeGroupSQLColumn(7).Equals("") Then

                                        Dim linkAttribute As String = _
                                            attributeGroupSQLColumn(6).ToString().ToLower().Trim()
                                        Dim linkAttributeKey As String = _
                                            attributeGroupSQLColumn(7).ToString().ToLower().Trim()

                                        RegenerateLinkedAttributeGroup( _
                                            uniqueGroupID, _
                                            attribute, displayName, _
                                            groupType, mailEnabled, _
                                            linkAttribute, linkAttributeKey)

                                    Else
                                        LogEvent("There was not a " + _
                                            "complete definition " + _
                                            "for group " + groupDefinitions( _
                                            int).ToLower() + " in the " + _
                                            "attrbuteGroupDefinitions table.", True, True, 1064, _
                                            EventLogEntryType.Warning)

                                    End If

                            End Select

                        Else
                            LogEvent("There was not a complete " + _
                                "definition for group " + _
                                groupDefinitions(int).ToLower() + _
                                " in the attrbuteGroupDefinitions table.", True, True, 1066, _
                                EventLogEntryType.Warning)

                        End If

                    End While

                Else
                    LogEvent("There was not an attribute group definition " _
                    + "with the uniqueID of " + groupDefinitions(int).ToLower() _
                    + ".  If there are any auto groups definitions that were generated " _
                    + "with that ID, they will be deleted.", True, True, 1068, _
                    EventLogEntryType.Warning)

                    DeleteAttributeGroupDefinitions(groupDefinitions(int).ToLower())

                End If


                InsertAttributeGroupDeltaDeletes()

                ' Handle the exceptions
            Catch exception As Exception
                LogEvent("There was an unhandled error " + _
                    "during the regeneration of groups. This may " + _
                    "result in the incorrect number of auto generated groups or members." _
                    + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                    + exception.StackTrace, True, True, 1070, EventLogEntryType.Warning)

                Throw exception

            Finally
                If Not (attributeGroupReader Is Nothing) Then
                    attributeGroupReader.Close()
                End If

            End Try

        Next

    End Sub

    ' <summary>
    ' Single attribute groups can be defined in XML config works as follows:
    ' 1. A distinct list of a single attribute (such as location, company or
    '     department) is created
    ' 2. A group definition is created for objects that have same value of 
    '    that attribute (such as 'Employees in department X' or 'Employees 
    '    at location Y')</summary>
    ' <param name="uniqueGroupID">unique Id of the group</param>
    ' <param name="attribute">name of the attribute of a group</param>
    ' <param name="displayName">display name of a group</param>
    ' <param name="groupType">type of the group containing the attribute
    ' </param>
    ' <param name="mailEnabled">boolean value indicating the status of 
    ' mail enabled groups</param>
    ' <returns></returns>

    Private Sub RegenerateSingleAttributeGroup _
        (ByVal uniqueGroupID As String, _
        ByVal attribute As String, ByVal displayName As String, _
        ByVal groupType As Integer, ByVal mailEnabled As Boolean)

        Dim distinctListReader As SqlDataReader = Nothing

        Try

            ' Delete previously autocreated groups of this kind
            DeleteAttributeGroupDefinitions(uniqueGroupID)

            Dim attributeValue As String = String.Empty
            Dim attributeBasedDisplayName As String = String.Empty
            Dim whereClause As String = attribute + _
                                    " = '[[singleAttributeValue]]'"

            ' Query to create a distinct list of the single attribute values
            Dim distinctListStatement As String = "SELECT DISTINCT " _
                + attribute + " FROM mms_metaverse WITH (NOLOCK) " + _
                                        "WHERE object_type='person'"
            Dim distinctListCommand As SqlCommand = _
                        New SqlCommand(distinctListStatement, miisConnection)
            distinctListCommand.CommandTimeout = sqlCommandTimeout
            Dim sqlColumn(1) As Object

            ' sqlColumn object will contain the following values:
            ' attributeValue (0)
            ' Empty (1)

            ' Create a distinct list of attribute values
            ' for the specified object type
            distinctListReader = distinctListCommand.ExecuteReader()

            '  Create a group definition for each group
            While (distinctListReader.Read())

                distinctListReader.GetValues(sqlColumn)

                If Not distinctListReader.IsDBNull(0) AndAlso Not _
                                        sqlColumn(0).Equals("") Then

                    attributeValue = sqlColumn(0).ToString().Trim()
                    attributeBasedDisplayName = Replace(displayName, _
                                    "{attributeValue}", attributeValue)
                    attributeBasedDisplayName = Replace _
                                    (attributeBasedDisplayName, "'", "''")
                    InsertAttributeGroupDefinition(attributeValue, _
                            uniqueGroupID, attributeBasedDisplayName, _
                            whereClause.Replace("[[singleAttributeValue]]", _
                            attributeValue), groupType, mailEnabled)

                End If

            End While

        Catch exception As Exception
            LogEvent("Error in RegenerateSingleAttributeGroup" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1072, EventLogEntryType.Error)

            Throw exception

        Finally
            If Not (distinctListReader Is Nothing) Then
                distinctListReader.Close()
            End If

        End Try

    End Sub

    ' <summary>
    ' Linked attribute groups can be defined in XML config works as follows:
    ' 1. A select distinct is made on a sigle attribute (such as the manager
    '    attribute of person objects)
    ' 2. The single attribute is a key (guid) to another MV entry. That MV 
    '    entry (the manager object) is read and a single attribute from the
    '    linked entry (such as displayName) can be used in the group name
    '    
    '    A typical example would be to create groups for direct reports
    ' </summary>
    ' <param name="uniqueGroupID">unique Id of the group</param>
    ' <param name="attrbuteGroupType">Group type of the linked attribute
    ' </param>
    ' <param name="attribute">name of the attribute of a group</param>
    ' <param name="displayName">display name of a group</param>
    ' <param name="groupType">type of the group containing the attribute
    ' </param>
    ' <param name="mailEnabled">boolean value indicating the status of 
    ' mail enabled groups</param>
    ' <param name="linkAttribute">linked attribute of the group</param>
    ' <param name="linkAttributeKey">linked attribute key of the
    ' group</param>
    ' <returns></returns>

    Private Sub RegenerateLinkedAttributeGroup _
        (ByVal uniqueGroupID As String, _
        ByVal attribute As String, ByVal displayName As String, _
        ByVal groupType As Integer, ByVal mailEnabled As Boolean, _
        ByVal linkAttribute As String, ByVal linkAttributeKey As String)

        Dim distinctListReader As SqlDataReader = Nothing

        Try

            ' Delete previously autocreated groups of this kind
            DeleteAttributeGroupDefinitions(uniqueGroupID)

            ' Declarations
            Dim attributeValue As String = String.Empty
            Dim attributeBasedDisplayName As String = String.Empty
            Dim linkAttributeValue As String = String.Empty
            Dim linkAttributeKeyValue As String = String.Empty

            Dim whereClause = String.Format("object_id IN " + _
                    "(SELECT object_id FROM mms_mv_link WHERE " + _
                    "attribute_name='{0}' AND reference_id = " + _
                    "(SELECT object_id FROM mms_metaverse WHERE {1} = " + _
                    "'((linkAttributeKeyValue))'))", linkAttribute, _
                    linkAttributeKey)

            ' Query to create a distinct list of the single attribute values
            Dim distinctListStatement = "SELECT " + attribute + ", " + _
                linkAttributeKey + _
                " FROM MicrosoftIdentityIntegrationServer " + _
                "..mms_metaverse WHERE object_id IN (SELECT DISTINCT " + _
                "reference_id FROM MicrosoftIdentityIntegrationServer " + _
                "..mms_mv_link WHERE attribute_name='" + linkAttribute + "')"

            Dim distinctListCommand As SqlCommand = _
                    New SqlCommand(distinctListStatement, miisConnection)
            distinctListCommand.CommandTimeout = sqlCommandTimeout
            Dim sqlColumn(2) As Object

            ' sqlColumn object will contain the following values:
            ' attribute (0)
            ' linkAttributeKey (1)
            ' Empty (2)

            ' Create a distinct list of attribute values for
            ' the specified object type
            distinctListReader = distinctListCommand.ExecuteReader()

            ' Create group definition for each group
            While (distinctListReader.Read())

                distinctListReader.GetValues(sqlColumn)

                If Not distinctListReader.IsDBNull(0) _
                    AndAlso Not sqlColumn(0).Equals("") AndAlso Not _
                    distinctListReader.IsDBNull(1) AndAlso Not _
                    sqlColumn(1).Equals("") Then

                    attributeValue = sqlColumn(0).ToString().Trim()
                    linkAttributeKeyValue = sqlColumn(1).ToString().Trim()

                    attributeBasedDisplayName = Replace(displayName, _
                                    "{attributeValue}", attributeValue)

                    attributeBasedDisplayName = Replace _
                                    (attributeBasedDisplayName, "'", "''")

                    InsertAttributeGroupDefinition(attributeValue, _
                            uniqueGroupID, attributeBasedDisplayName, _
                            whereClause.Replace("((linkAttributeKeyValue))", _
                            linkAttributeKeyValue), groupType, mailEnabled)

                End If

            End While

        Catch exception As Exception
            LogEvent("Error in regenerateLinkedAttributeGroup" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1074, EventLogEntryType.Error)

            Throw exception

        Finally
            If Not (distinctListReader Is Nothing) Then
                distinctListReader.Close()
            End If

        End Try

    End Sub

    ' <summary>
    ' Delete the groups that were created previously
    ' so that it can be regenerated
    ' </summary>
    ' <param name="uniqueGroupID">unique id of the group</param>
    ' <returns></returns>

    Private Sub DeleteAttributeGroupDefinitions( _
                ByVal uniqueGroupID As String)

        ' Open a new reader for the MIIS database
        Dim deleteAutoReader As SqlDataReader = Nothing

        ' check if the displayname and objectid are cleared 
        ' for this group

        Dim listOfRegeneratedAutoGrpTmp As New ArrayList

        Try

            Dim deleteAutoStatement As String = "SELECT objectUID FROM " + _
                    groupDefinitionsTable + _
                    " WHERE objectType = 'groupAuto' " + _
                    "AND groupAutoUID = '" + uniqueGroupID + "'"

            Dim deleteAutoCommand As SqlCommand = New SqlCommand _
                    (deleteAutoStatement, miisGroupMgmtConnectionInner)
            deleteAutoCommand.CommandTimeout = sqlCommandTimeout
            Dim sqlColumn(1) As Object

            ' sqlColumn object will contain the following values:
            ' objectUID (0)
            ' Empty (1)

            ' Execute sql command
            deleteAutoReader = deleteAutoCommand.ExecuteReader()

            While (deleteAutoReader.Read())
                deleteAutoReader.GetValues(sqlColumn)

                ' check if object_id is not null or ""
                ' and add to arraylist
                If Not deleteAutoReader.IsDBNull(0) AndAlso Not _
                                            sqlColumn(0).Equals("") Then

                    ' The guid is used to join the object to the 
                    ' metaverse object
                    listOfRegeneratedAutoGrpTmp.Add(sqlColumn(0). _
                                                    ToString().Trim())

                End If

            End While

            For int As Integer = 0 To listOfRegeneratedAutoGrpTmp.Count - 1

                Dim listOfRegAutoGrpTmpString As String = _
                                    listOfRegeneratedAutoGrpTmp(int)

                ' Execute the delete query against the groupDefinitionsTable
                Dim delAutoGroupsStatement As String = "DELETE FROM " + _
                        groupDefinitionsTable + _
                        " WHERE objectType = 'groupAuto' " + _
                        "AND objectUID = " + _
                        "'" + listOfRegAutoGrpTmpString + "'"

                Dim delAutoGroupsCommand As SqlCommand = _
                            New SqlCommand(delAutoGroupsStatement, _
                            miisGroupMgmtConForNonReader)
                delAutoGroupsCommand.CommandTimeout = sqlCommandTimeout
                rowsAffected = delAutoGroupsCommand.ExecuteNonQuery()

                ' Execute the delete query against the clauseDefinitionsTable
                Dim delAutoGroupsClauseStatement As String = _
                    "DELETE FROM " + clauseDefinitionsTable + _
                    " WHERE clauseType = 'clauseAuto' " + _
                    "AND objectUID = " + _
                    "'" + listOfRegAutoGrpTmpString + "'"

                Dim delAutoGroupsClauseCommand As SqlCommand = _
                        New SqlCommand(delAutoGroupsClauseStatement, _
                                miisGroupMgmtConForNonReader)
                delAutoGroupsClauseCommand.CommandTimeout = sqlCommandTimeout
                rowsAffected = delAutoGroupsClauseCommand.ExecuteNonQuery()

                listOfRegeneratedAutoGroups.Add _
                                    (listOfRegAutoGrpTmpString)

            Next

        Catch exception As Exception
            LogEvent("Error in deleteAttributeGroupDefinitions" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1076, EventLogEntryType.Error)

            Throw exception

        Finally
            If Not (deleteAutoReader Is Nothing) Then
                deleteAutoReader.Close()
            End If

        End Try

    End Sub

    ' <summary>
    ' Insert the group definitions into the groupDefinitions table
    ' </summary>
    ' <param name="UID">Id of the group</param>
    ' <param name="uniqueGroupID">unique id of the group</param>
    ' <param name="displayName">display name of the group</param>
    ' <param name="whereClause">clause definition of the group</param>
    ' <param name="groupType">type of the group containing the attribute
    ' </param>
    ' <param name="mailEnabled">string value indicating the status of 
    ' mail enabled groups</param>
    ' <returns></returns>

    Private Sub InsertAttributeGroupDefinition _
            (ByVal UID As String, ByVal uniqueGroupID As String, _
            ByVal displayName As String, ByVal whereClause As String, _
            ByVal groupType As String, ByVal mailEnabled As String)

        Dim attributeGroupUniqueID As String = uniqueGroupID + "-" + UID

        Try
            attributeGroupUniqueID = Replace(attributeGroupUniqueID, "'", "''")

            ' Strip out characters of the displayname that are not allowed
            ' on groups and replace with an '_' character. This is a 
            ' limitation of the sAMAccountName in AD, but since displayName 
            ' flows into(sAMAccountName, it is enforced)
            displayName = Replace(displayName, "/", "_")
            displayName = Replace(displayName, "\", "_")
            displayName = Replace(displayName, "[", "_")
            displayName = Replace(displayName, "]", "_")
            displayName = Replace(displayName, ":", "_")
            displayName = Replace(displayName, ";", "_")
            displayName = Replace(displayName, "|", "_")
            displayName = Replace(displayName, "=", "_")
            displayName = Replace(displayName, ",", "_")
            displayName = Replace(displayName, "+", "_")
            displayName = Replace(displayName, "*", "_")
            displayName = Replace(displayName, "?", "_")
            displayName = Replace(displayName, "<", "_")
            displayName = Replace(displayName, ">", "_")

            Dim mailNickName As String = String.Empty

            If mailEnabled.ToLower = "true" Then
                mailNickName = Replace(displayName, "-", "")
                mailNickName = Replace(mailNickName, " ", "")
            End If

            ' Execute the insert command over groupDefinitions table
            Dim insertAutoGroupStatement As String = "INSERT INTO " _
                + groupDefinitionsTable + " (objectUID, groupAutoUID, " + _
                "objectType, displayName, description, clauseLink, " + _
                "enabledFlag, maxExcept, preserveMembers, groupType, " + _
                "mailNickName) VALUES ('" + attributeGroupUniqueID + "', '" _
                + uniqueGroupID + "', 'groupAuto', " + _
                "'" + displayName + "', " + "'Attribute Based Group', '" _
                + attributeGroupUniqueID + "', " + "'enabled', '10', '0', '" _
                + groupType + " ', '" + mailNickName + "')"

            Dim insertAutoGroupCommand As SqlCommand = _
                    New SqlCommand(insertAutoGroupStatement, _
                            miisGroupMgmtConForNonReader)
            insertAutoGroupCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = insertAutoGroupCommand.ExecuteNonQuery()

            If rowsAffected = 0 Then
                LogEvent("Unable to create group definition for " _
                    + displayName, True, True, 1078, EventLogEntryType.Warning)
            End If

            whereClause = Replace(whereClause, "'", "''")

            ' Insert the new clause for this group
            Dim insertAutoGroupClauseStatement As String = "INSERT INTO " _
                + clauseDefinitionsTable + " (objectUID, clauseAutoUID, " + _
                "clauseType, clause) VALUES ('" + attributeGroupUniqueID + _
                "', '" + uniqueGroupID + "', 'clauseAuto', " + _
                "'" + whereClause + "')"

            Dim insertAutoGroupClauseCommand As SqlCommand = _
                    New SqlCommand(insertAutoGroupClauseStatement, _
                                miisGroupMgmtConForNonReader)
            insertAutoGroupClauseCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = insertAutoGroupClauseCommand.ExecuteNonQuery()

            If rowsAffected = 0 Then
                LogEvent("Unable to create group clause " + _
                    "definition for " + displayName, True, True, 1080, _
                    EventLogEntryType.Warning)
            End If

            Dim changeType As String = "Add"
            Dim groupWasPresentLastRun As String = "no"

            For int As Integer = 0 To listOfRegeneratedAutoGroups.Count - 1
                If groupWasPresentLastRun = "no" AndAlso _
                                attributeGroupUniqueID.ToLower = _
                                LCase(listOfRegeneratedAutoGroups(int)) Then
                    changeType = "Modify"
                    listOfRegeneratedAutoGroups.RemoveAt(int)
                    groupWasPresentLastRun = "yes"
                End If
            Next

            ' Execute the insert command over the groupDefinitions_deltaTable
            Dim copyGroupToDeltaStatement As String = "INSERT INTO " _
                    + groupDefinitions_deltaTable + " (objectUID, " + _
                    "groupAutoUID, objectType, displayName, description, " + _
                    "clauseLink, enabledFlag, maxExcept, preserveMembers, " + _
                    "groupType, mailNickName) SELECT objectUID, " + _
                    "groupAutoUID, objectType, displayName, description, " + _
                    "clauseLink, enabledFlag, maxExcept, preserveMembers, " + _
                    "groupType, mailNickName FROM " + groupDefinitionsTable _
                    + " WHERE objectUID = '" + attributeGroupUniqueID + "'"

            Dim copyGroupToDeltaCommand As SqlCommand = _
                    New SqlCommand(copyGroupToDeltaStatement, _
                            miisGroupMgmtConForNonReader)
            copyGroupToDeltaCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = copyGroupToDeltaCommand.ExecuteNonQuery()

            If rowsAffected = 0 Then
                LogEvent("There was an error adding the " + _
                    groupDisplayName + " group to the delta table.", True, True, 1082, _
                    EventLogEntryType.Warning)
            End If

            ' Update the groupDefinitions_deltaTable with the new change time
            ' and corresponding changetype for a particular object ID
            Dim updateGroupInDeltaStatement As String = "UPDATE " + _
                    groupDefinitions_deltaTable + " SET changeTime = '" + _
                    Date.Now + "', changeType = '" + changeType + _
                    "' WHERE objectUID = '" + attributeGroupUniqueID + _
                    "' AND changeType IS NULL"

            Dim updateGroupInDeltaCommand As SqlCommand = _
                        New SqlCommand(updateGroupInDeltaStatement, _
                            miisGroupMgmtConForNonReader)
            updateGroupInDeltaCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = updateGroupInDeltaCommand.ExecuteNonQuery()

            If rowsAffected = 0 Then
                LogEvent("There was an error updating the " _
                    + groupDisplayName + _
                    " group after it was added to the delta table.", True, True, 1084, _
                    EventLogEntryType.Warning)
            End If

        Catch exception As Exception
            LogEvent("Error in insertAttributeGroupDefinition" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1086, EventLogEntryType.Error)

            Throw exception

        End Try

    End Sub

    ' <summary>
    ' Copy the newly added groups to the delta table
    ' </summary>
    ' <returns></returns>

    Private Sub InsertAttributeGroupDeltaDeletes()

        Try

            For int As Integer = 0 To listOfRegeneratedAutoGroups.Count - 1

                ' Insert into the groupDefinitions_deltaTable the values like,
                ' object ID, change time and change type
                Dim copyGroupToDeltaStatement As String = "INSERT INTO " _
                    + groupDefinitions_deltaTable + _
                    " (objectUID, changeTime, " + _
                    "changeType) VALUES " + _
                    "('" + listOfRegeneratedAutoGroups(int) + _
                    "', '" & Date.Now & "', 'Delete')"

                Dim copyGroupToDeltaCommand As SqlCommand = _
                        New SqlCommand(copyGroupToDeltaStatement, _
                            miisGroupMgmtConForNonReader)
                rowsAffected = copyGroupToDeltaCommand.ExecuteNonQuery()
                copyGroupToDeltaCommand.CommandTimeout = sqlCommandTimeout
                If rowsAffected = 0 Then
                    LogEvent("There was an error adding the " + _
                            groupDisplayName + " group to the delta table.", True, True, 1088, _
                            EventLogEntryType.Warning)
                End If

            Next

        Catch exception As Exception
            LogEvent("Error in insertAttributeGroupDeltaDeletes" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1090, EventLogEntryType.Error)

            Throw exception

        End Try

    End Sub

    ' <summary>
    ' Determine if there is already and instance of the application running
    ' </summary>
    ' <returns>Boolean</returns>
    Function PrevInstance() As Boolean
        If UBound(Diagnostics.Process.GetProcessesByName _
        (System.Reflection.Assembly.GetExecutingAssembly.GetName.Name)) > 0 Then
            Return True

        Else
            Return False

        End If

    End Function

    ' <summary>
    ' Check to see if there is a flag that states that the last run was successful
    ' </summary>
    ' <returns>Boolean</returns>
    Function SuccessfulLastRun() As Boolean

        Dim stagingSuccessQuery As String = "SELECT executeDateTime, comment FROM " _
                + stagingDefinitionsTable + " WHERE sqlCommand = '' and " _
                + "comment = 'successfulRun'"
        Dim stagingSuccessCommand As SqlCommand = New _
                SqlCommand(stagingSuccessQuery, miisGroupMgmtConnectionInner)
        stagingSuccessCommand.CommandTimeout = sqlCommandTimeout
        Dim stagingSuccessColumn(2) As Object

        ' stagingColumn object will contain the following values:
        ' executeDateTime (0)
        ' comment (1)
        ' Empty (2)

        Dim stagingSuccessReader As SqlDataReader = stagingSuccessCommand.ExecuteReader()

        Try

            ' Check to see if the flag is set that shows it was ran successfully last time
            While (stagingSuccessReader.Read())
                stagingSuccessReader.GetValues(stagingSuccessColumn)
                If Not stagingSuccessReader.IsDBNull(0) AndAlso _
                                        Not stagingSuccessColumn(0).Equals("") Then

                    Dim successTimeString As String = _
                                        stagingSuccessColumn(0).ToString().Trim()

                    LogEvent("The groupPopulator process ran sucessfully last time at " + _
                        successTimeString + " so the process will proceed normally.", _
                        False, True, 1092, _
                        EventLogEntryType.Information)
                End If
            End While

            ' Once all the rows are processed, clean up the table by 
            ' deleting all the flag
            If stagingSuccessReader.HasRows Then

                Dim cleanupString As String = "DELETE FROM " + _
                    stagingDefinitionsTable + " WHERE sqlCommand = '' and comment = 'successfulRun'"
                Dim cleanupCommand As SqlCommand = _
                    New SqlCommand(cleanupString, _
                        miisGroupMgmtConForNonReader)
                cleanupCommand.CommandTimeout = sqlCommandTimeout
                rowsAffected = cleanupCommand.ExecuteNonQuery()

                If rowsAffected = 0 Then
                    Dim errorText As String = "Unable to delete the flag that " + _
                                            "shows if the process ran successfully last time."
                    LogEvent(errorText, True, True, 1094, EventLogEntryType.Warning)
                    Throw New Exception(errorText)

                End If

                'Close the data reader before method exit.
                If Not (stagingSuccessReader Is Nothing) Then
                    stagingSuccessReader.Close()
                End If

                Return True

            Else
                Return False

            End If

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1096, EventLogEntryType.Error)

            Throw exception

        Finally
            'Close the data reader before method exit.
            If Not (stagingSuccessReader Is Nothing) Then
                stagingSuccessReader.Close()
            End If

        End Try

    End Function

    ' <summary>
    ' Set a flag that states that the last run was successful
    ' </summary>
    ' <returns></returns>
    Private Sub FlagSuccessfulRun()

        Try

            Dim setSuccessFlagString As String = "INSERT INTO " + _
                stagingDefinitionsTable + "(executeDateTime, sqlCommand, comment)" + _
                "VALUES('" + Date.Now + "', " + "'', 'successfulRun')"
            Dim setSuccessFlagCommand As SqlCommand = _
                New SqlCommand(setSuccessFlagString, _
                    miisGroupMgmtConForNonReader)
            setSuccessFlagCommand.CommandTimeout = sqlCommandTimeout
            rowsAffected = setSuccessFlagCommand.ExecuteNonQuery()

            If rowsAffected = 0 Then
                Dim errorText As String = "Unable to set the flag that " + _
                        "shows if the process ran successfully last time."
                LogEvent(errorText, True, True, 1098, EventLogEntryType.Error)
                Throw New Exception(errorText)

            End If

        Catch exception As Exception
            LogEvent("Error: Exception occured" _
                + vbCrLf + "Message: " + exception.Message + vbCrLf + "StackTrace: " _
                + exception.StackTrace, True, True, 1100, EventLogEntryType.Error)

            Throw exception

        End Try

    End Sub

    ' <summary>
    ' Log information to either the eventlog or console or both
    ' </summary>
    ' <returns></returns>
    Private Sub LogEvent(ByVal errorString As String, ByVal logToConsole As Boolean, _
        ByVal logEventToEventLog As Boolean, ByVal eventID As Integer, _
        ByVal eventType As System.Diagnostics.EventLogEntryType)

        If logToConsole = True Then
            Console.WriteLine(errorString)
        End If

        If logEventToEventLog = True Then
            EventLog.WriteEntry("MIIS_GroupPopulator", errorString, _
                eventType, eventID)
        End If

    End Sub

End Module
