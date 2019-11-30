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

Imports System.Diagnostics
Imports System.Data.SqlClient

' <Summary>
' Enables the specification of exceptions for the defined group
' <Summary/>

Public Class BuildExcept
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "
    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.miisGroupManagementConnectionString = New System.Data.SqlClient.SqlConnection
        Me.miisConnectionString = New System.Data.SqlClient.SqlConnection

    End Sub
    Protected WithEvents miisConnectionString As System.Data.SqlClient.SqlConnection
    Protected WithEvents miisGroupManagementConnectionString As System.Data.SqlClient.SqlConnection

    Protected WithEvents lbInclude As System.Web.UI.WebControls.ListBox
    Protected WithEvents lblError As System.Web.UI.WebControls.Label
    Protected WithEvents btnAddExclude As System.Web.UI.WebControls.Button
    Protected WithEvents lbExclude As System.Web.UI.WebControls.ListBox
    Protected WithEvents btnAddInclude As System.Web.UI.WebControls.Button
    Protected WithEvents btnDeleteInclude As System.Web.UI.WebControls.Button
    Protected WithEvents btnDeleteExclude As System.Web.UI.WebControls.Button
    Protected WithEvents btnClose As System.Web.UI.WebControls.Button
    Protected WithEvents lblPreserveMember As System.Web.UI.WebControls.Label
    Protected WithEvents pnlInclude As System.Web.UI.WebControls.Panel
    Protected WithEvents txtPreserveMember As System.Web.UI.WebControls.TextBox
    Protected WithEvents pnlExclude As System.Web.UI.WebControls.Panel

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    ' Declarations
    Dim recordsAffected As Integer = 0
    Dim groupObjectUID As String = String.Empty
    Dim groupDisplayName As String = String.Empty
    Dim objectUIDToName As String = String.Empty
    
    ' <summary>
    ' Executes when the page loads (originally and postback)
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub Page_Load _
        (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                        Handles MyBase.Load

        Try

            ' Get the values that are being passed in on the URL.
            ' These values are used to know which group is being worked upon
            groupObjectUID = Request.QueryString _
                                        ("groupObjectUID").ToString()
            groupDisplayName = Request.QueryString _
                                        ("groupDisplayName").ToString()

            ' Get the connection string from the global.asax file 
            ' which gets them from the XML file
            Me.miisConnectionString.ConnectionString = _
                    ConfigurationSettings.AppSettings("miisConnectionString")
            Me.miisGroupManagementConnectionString.ConnectionString _
                    = ConfigurationSettings.AppSettings _
                        ("miisGroupManagementConnectionString")

            If Not IsPostBack Then

                miisGroupManagementConnectionString.Open()
                miisConnectionString.Open()

                ' call the FillInclude method to fill the lbInclude 
                ' listbox with the person objects 
                FillInclude()

                ' call the FillExclude method to fill the lbExclude 
                ' listbox with the person objects 
                FillExclude()

                ' selects the maxExcept from the groupDefinitions and 
                ' displays the value on the screen
                Dim maxNumCount As Int32 = 0
                Const MAX_COUNT As Int32 = 999999
                Dim countExceptionsReader As SqlDataReader = Nothing
                Dim sqlCountExceptionsColumn(2) As Object
                Dim countExceptionsQuery As String = "SELECT maxExcept, " + _
                    "preserveMembers FROM groupDefinitions WHERE " + _
                        "objectUID = '" + groupObjectUID + "'"
                Dim countExceptionsCommand As SqlCommand = _
                            New SqlCommand(countExceptionsQuery, _
                                    miisGroupManagementConnectionString)
                countExceptionsReader = _
                        countExceptionsCommand.ExecuteReader()

                While (countExceptionsReader.Read())

                    countExceptionsReader.GetValues(sqlCountExceptionsColumn)

                    ' 
                    If Not countExceptionsReader.IsDBNull(0) AndAlso Not _
                                sqlCountExceptionsColumn(0).Equals("") Then
                        ' set the max number of users for the group threshold
                        maxNumCount = sqlCountExceptionsColumn(0). _
                                                            ToString().Trim()
                    Else
                        maxNumCount = MAX_COUNT
                    End If

                    ' 
                    If Not countExceptionsReader.IsDBNull(1) AndAlso Not _
                                  sqlCountExceptionsColumn(1).Equals("") Then
                        ' set the preserveMember values
                        txtPreserveMember.Text = _
                                sqlCountExceptionsColumn(1).ToString().Trim()
                    End If

                End While

                ' if the number of exceptions is greater than the max 
                ' exceptions then fire the warning - workflow could be 
                ' inserted here so that to get approval for 
                ' the exception if desired
                If (lbInclude.Items.Count + lbExclude.Items.Count) _
                                                  > maxNumCount Then
                    btnAddInclude.Attributes.Add("onclick", _
                                          "return confirm_exceptions();")
                    btnAddExclude.Attributes.Add("onclick", _
                                          "return confirm_exceptions();")
                End If

                If Not (countExceptionsReader Is Nothing) Then
                    countExceptionsReader.Close()
                End If
                'Close the sql connection before method exit
                miisConnectionString.Close()
                miisGroupManagementConnectionString.Close()

            End If

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error Page_Load: " + exception.Message
            'Ensure that the sql connections are closed before method exit
        Finally
            If Not miisGroupManagementConnectionString Is Nothing Then
                If Not (miisGroupManagementConnectionString.State = ConnectionState.Closed) Then
                    miisGroupManagementConnectionString.Close()
                End If
            End If
            If Not miisConnectionString Is Nothing Then
                If Not (miisConnectionString.State = ConnectionState.Closed) Then
                    miisConnectionString.Close()
                End If
            End If

        End Try

    End Sub

    ' <summary>
    ' Fills the lbInclude listbox with the person objects 
    ' that should be manually included in the group
    ' </summary>
    ' <returns></returns> 

    Private Sub FillInclude()

        Try

            ' Select the users from the exceptionDefinitions 
            ' (exceptions) table that are included
            Dim inclusionReader As SqlDataReader = Nothing
            Dim sqlInclusionColumn(1) As Object
            Dim inclusionQuery As String = "SELECT mvObjectUID " + _
                    "FROM exceptionDefinitions WHERE objectUID = '" + _
                            groupObjectUID + "' AND exceptType = 'include'"
            Dim inclusionCommand As SqlCommand = _
                            New SqlCommand(inclusionQuery, _
                                    miisGroupManagementConnectionString)
            inclusionReader = inclusionCommand.ExecuteReader()

            lbInclude.Items.Clear()

            While (inclusionReader.Read())

                inclusionReader.GetValues(sqlInclusionColumn)

                ' Check for clause being not null or ""
                If Not inclusionReader.IsDBNull(0) AndAlso Not _
                                    sqlInclusionColumn(0).Equals("") Then

                    ' Call the getMemberName sub to get the friendly 
                    ' name for the users from the MV
                    getMemberName(sqlInclusionColumn(0).ToString().Trim())
                    Dim newItem As ListItem

                    ' Fill the lbInclude listbox with the friendly name 
                    ' as the text and the <object-id> as the value
                    newItem = New ListItem(objectUIDToName, _
                                    sqlInclusionColumn(0).ToString().Trim())
                    lbInclude.Items.Add(newItem)
                End If

            End While

            If Not (inclusionReader Is Nothing) Then
                inclusionReader.Close()
            End If

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error FillInclude: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Fills the lbExclude listbox with the person objects 
    ' that should be manually excluded from the group 
    ' </summary>
    ' <returns></returns> 

    Private Sub FillExclude()

        Try

            ' Select the users from the exceptionDefinitions 
            ' (exceptions) table that are included users
            Dim exclusionReader As SqlDataReader = Nothing
            Dim sqlExclusionColumn(1) As Object
            Dim exclusionQuery As String = "SELECT mvObjectUID " + _
                    "FROM exceptionDefinitions WHERE objectUID = " + _
                    "'" + groupObjectUID + "' AND exceptType = 'exclude'"
            Dim exclusionCommand As SqlCommand = _
                        New SqlCommand(exclusionQuery, _
                                miisGroupManagementConnectionString)
            exclusionReader = exclusionCommand.ExecuteReader()

            lbExclude.Items.Clear()

            While (exclusionReader.Read())

                exclusionReader.GetValues(sqlExclusionColumn)

                ' check for clause being not null or ""
                If Not exclusionReader.IsDBNull(0) AndAlso Not _
                                sqlExclusionColumn(0).Equals("") Then

                    ' Call the getMemberName sub to get the friendly 
                    ' name for the users from the MV
                    getMemberName(sqlExclusionColumn(0).ToString().Trim())
                    Dim newItem As ListItem

                    ' Fill the lbInclude listbox with the friendly name as 
                    ' the text and the <object-id> as the value
                    newItem = New ListItem(objectUIDToName, _
                                sqlExclusionColumn(0).ToString().Trim())
                    lbExclude.Items.Add(newItem)
                End If

            End While

            If Not (exclusionReader Is Nothing) Then
                exclusionReader.Close()
            End If

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error FillExclude: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Takes the user id of the person and gets the friendly 
    ' name from the metaverse</summary>
    ' <param name="userObjectUID">UserID of the user</param>
    ' <returns></returns> 

    Private Sub getMemberName(ByVal userObjectUID As String)

        Try

            ' Select the uid from the metaverse for the person
            ' being worked upon passed in from the other sub
            Dim memberReader As SqlDataReader = Nothing
            Dim sqlMemberColumn(1) As Object
            Dim memberQuery As String = "SELECT " + _
                "displayName FROM mms_metaverse WITH (NOLOCK) " + _
                    "WHERE mms_metaverse.object_id = '" + userObjectUID + "'"
            Dim inclusionCommand As SqlCommand = New _
                            SqlCommand(memberQuery, miisConnectionString)
            memberReader = inclusionCommand.ExecuteReader()

            While (memberReader.Read())

                objectUIDToName = ""

                memberReader.GetValues(sqlMemberColumn)

                ' Check for clause being not null or ""
                If Not memberReader.IsDBNull(0) AndAlso Not _
                                    sqlMemberColumn(0).Equals("") Then

                    ' Set objectUIDToName to the uid from the metaverse
                    objectUIDToName = sqlMemberColumn(0).ToString().Trim()
                Else

                    ' if the user is no longer in the MV, 
                    ' then the <object-id> is displayed
                    objectUIDToName = userObjectUID
                End If

            End While
            'Check and close the sql data reader
            If Not (memberReader Is Nothing) Then
                memberReader.Close()
            End If

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error getMemberName: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Change the page to the selectExcept.aspx page within the window
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub btnAddInclude_Click _
            (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                    Handles btnAddInclude.Click

        Try

            ' Based on the button clicked, prompt a message
            If btnClose.Text = "Save" Then
                Dim strMessage As String = "You must first save the " + _
                    "settings for the number of days to preserve " + _
                    "group membership before adding or removing exceptions."

                'Display alert window to the user
                ThrowAlert(strMessage)

                Exit Sub
            End If

            ' Server.Transfer will keep all variables in tact when the new
            ' page is loaded. This is useful to pass data back and forth
            ' in that and to add an inclusion
            Server.Transfer("selectExcept.aspx?ExceptionType=" + _
                        "include&groupObjectUID=" + groupObjectUID + _
                            "&groupDisplayName=" + groupDisplayName, True)

        Catch exception As Exception
            HideControls()
            lblError.Text = _
                "Error btnAddInclude_Click: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Change the page to the selectExcept.aspx page within the window
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub btnAddExclude_Click _
            (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                              Handles btnAddExclude.Click

        Try

            ' Based on the button clicked, prompt a message
            If btnClose.Text = "Save" Then
                Dim strMessage As String = "You must first save the " + _
                    "settings for the number of days to preserve group " + _
                    "membership before adding or removing exceptions."

                'Display alert window to the user
                ThrowAlert(strMessage)

                Exit Sub
            End If

            ' This is to add an exclusion
            Server.Transfer("selectExcept.aspx?ExceptionType=" + _
                     "exclude&groupObjectUID=" + groupObjectUID + _
                            "&groupDisplayName=" + groupDisplayName, True)

        Catch exception As Exception
            HideControls()
            lblError.Text = _
                "Error btnAddExclude_Click: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Deletes the selected user from the inclusion list for that group
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub btnDeleteInclude_Click _
        (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                            Handles btnDeleteInclude.Click

        Try

            ' Based on the button clicked, prompt a message
            If btnClose.Text = "Save" Then
                Dim strMessage As String = "You must first save " + _
                    "the settings for the number of days to preserve " + _
                    "group membership before adding or removing exceptions."

                'Display alert window to the user
                ThrowAlert(strMessage)

                Exit Sub
            End If

            ' Open database connections
            miisGroupManagementConnectionString.Open()
            miisConnectionString.Open()

            Dim selectionValue As ListItem

            ' Deletes the selected user from the inclusion list for that group
            For Each selectionValue In lbInclude.Items

                If selectionValue.Selected Then

                    Dim deleteIncludeStatement As String = "DELETE FROM " + _
                            "exceptionDefinitions WHERE " _
                                + "exceptionDefinitions.objectUID = '" _
                                + groupObjectUID + "' AND " _
                                + "exceptionDefinitions.exceptType = " _
                                + "'include' AND " _
                                + "exceptionDefinitions.mvObjectUID = '" _
                                + selectionValue.Value + "'"
                    Dim deleteIncludeCommand As SqlCommand = _
                            New SqlCommand(deleteIncludeStatement, _
                                    miisGroupManagementConnectionString)
                    recordsAffected = deleteIncludeCommand.ExecuteNonQuery()

                    EventLog.WriteEntry("miisGroupManagement", _
                            Environment.UserDomainName.ToLower + "\" + _
                            Environment.UserName.ToLower + " removed " + _
                            "the following exception for group " + _
                            "'" + groupDisplayName + "' with object UID '" _
                            + groupObjectUID + "':" + vbCrLf + vbCrLf + _
                            "INCLUDE - '" + selectionValue.Text + "'", _
                            EventLogEntryType.Information)

                End If

            Next

            ' call the FillInclude sub to refresh the list of users
            FillInclude()

            miisConnectionString.Close()
            miisGroupManagementConnectionString.Close()

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error btnDeleteInclude_Click: " + _
                                              exception.Message
        Finally
            If Not miisGroupManagementConnectionString Is Nothing Then
                If Not (miisGroupManagementConnectionString.State = ConnectionState.Closed) Then
                    miisGroupManagementConnectionString.Close()
                End If
            End If
            If Not miisConnectionString Is Nothing Then
                If Not (miisConnectionString.State = ConnectionState.Closed) Then
                    miisConnectionString.Close()
                End If
            End If
        End Try

    End Sub

    ' <summary>
    ' Deletes the selected user from the exclusion list for that group
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub btnDeleteExclude_Click _
        (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                            Handles btnDeleteExclude.Click

        Try

            ' Based on the button clicked, prompt a message
            If btnClose.Text = "Save" Then
                Dim strMessage As String = "You must first " + _
                    "save the settings for the number of days to " + _
                        "preserve group membership before adding or " + _
                                                    "removing exceptions."
                'Display alert window to the user
                ThrowAlert(strMessage)

                Exit Sub
            End If

            miisGroupManagementConnectionString.Open()
            miisConnectionString.Open()

            Dim selectionValue As ListItem

            ' Deletes the selected user from the exclusion list for this group
            For Each selectionValue In lbExclude.Items

                If selectionValue.Selected Then

                    Dim deleteExcludeStatement As String = _
                        "DELETE FROM exceptionDefinitions WHERE " + _
                        "exceptionDefinitions.objectUID " + _
                        "= '" + groupObjectUID + "' AND " + _
                        "exceptionDefinitions.exceptType = 'exclude' " + _
                        "AND exceptionDefinitions.mvObjectUID = " + _
                        "'" + selectionValue.Value + "'"
                    Dim deleteExcludeCommand As SqlCommand = _
                            New SqlCommand(deleteExcludeStatement, _
                                    miisGroupManagementConnectionString)
                    recordsAffected = deleteExcludeCommand.ExecuteNonQuery()

                    ' Log the result
                    EventLog.WriteEntry("miisGroupManagement", _
                        Environment.UserDomainName.ToLower + "\" + _
                            Environment.UserName.ToLower + " removed the " + _
                            "following exception for group " + _
                            "'" + groupDisplayName + "' with object UID '" _
                            + groupObjectUID + "':" + vbCrLf + vbCrLf + _
                            "EXCLUDE - '" + selectionValue.Text + "'", _
                            EventLogEntryType.Information)

                End If

            Next

            ' call the FillExclude sub to refresh the list of users
            FillExclude()

            'close the sql connections before method exit
            miisConnectionString.Close()
            miisGroupManagementConnectionString.Close()

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error btnDeleteExclude_Click: " + _
                                                        exception.Message
        Finally
            If Not miisGroupManagementConnectionString Is Nothing Then
                If Not (miisGroupManagementConnectionString.State = ConnectionState.Closed) Then
                    miisGroupManagementConnectionString.Close()
                End If
            End If
            If Not miisConnectionString Is Nothing Then
                If Not (miisConnectionString.State = ConnectionState.Closed) Then
                    miisConnectionString.Close()
                End If
            End If
        End Try

    End Sub

    ' <summary>
    ' Closes the window without updation when the "Close" button is clicked
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub btnClose_Click _
            (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                    Handles btnClose.Click

        Try

            Dim groupObjectUID As String = Request.QueryString _
                                            ("groupObjectUID").ToString()

            If btnClose.Text = "Save" Then

                miisGroupManagementConnectionString.Open()

                ' Set the preserve member value

                Dim updatePreserveMemberStatement As String = _
                    "UPDATE groupDefinitions SET preserveMembers = " + _
                    "'" + txtPreserveMember.Text + "' WHERE objectUID " + _
                    "= '" + groupObjectUID + "'"
                Dim updatePreserveMemberCommand As SqlCommand = _
                        New SqlCommand(updatePreserveMemberStatement, _
                                    miisGroupManagementConnectionString)
                recordsAffected = _
                        updatePreserveMemberCommand.ExecuteNonQuery()

                ' Set the group to be evaluated at the next delta run
                Dim copyGroupToDeltaStatement As String = _
                    "INSERT INTO groupDefinitions_delta (objectUID, " + _
                        "groupAutoUID, objectType, displayName, " + _
                        "description, clauseLink, enabledFlag, " + _
                        "maxExcept, preserveMembers, groupType, " + _
                        "mailNickName) SELECT objectUID, groupAutoUID, " + _
                        "objectType, displayName, description, " + _
                        "clauseLink, enabledFlag, maxExcept, " + _
                        "preserveMembers, groupType, mailNickName FROM " + _
                        "groupDefinitions WHERE objectUID = " + _
                        "'" + groupObjectUID + "'"
                Dim copyGroupToDeltaCommand As SqlCommand = _
                        New SqlCommand(copyGroupToDeltaStatement, _
                                    miisGroupManagementConnectionString)
                Dim rowsAffected As Integer = copyGroupToDeltaCommand. _
                                                        ExecuteNonQuery()

                'Check for error after the sql query execution
                If rowsAffected = 0 Then
                    Throw New Exception("There was an error adding the " _
                            + groupObjectUID + " group to the delta table.")
                End If

                'Frame query to update delta table 'groupDefinitions_delta'
                Dim updateGroupInDeltaStatement As String = _
                    "UPDATE groupDefinitions_delta SET changeTime " + _
                        "= '" + Date.Now + "', changeType = 'Modify' " + _
                        "WHERE objectUID = '" + groupObjectUID + "'  " + _
                        "AND changeType IS NULL"
                Dim updateGroupInDeltaCommand As SqlCommand = _
                        New SqlCommand(updateGroupInDeltaStatement, _
                                    miisGroupManagementConnectionString)
                rowsAffected = updateGroupInDeltaCommand.ExecuteNonQuery()

                'Check for error after the sql query execution
                If rowsAffected = 0 Then
                    Throw New Exception("There was an error updating the " _
                        + groupObjectUID + " group after it was added to " + _
                        "the delta table.")
                End If

                miisGroupManagementConnectionString.Close()
            End If

            ' Closes the window without updation 
            ' when the close button is clicked
            Dim strCloseScript As String = "<script>self.close()"
            strCloseScript += "</" + "script>"
            RegisterClientScriptBlock("key3", strCloseScript)

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error btnClose_Click: " + exception.Message
        Finally
            If Not miisGroupManagementConnectionString Is Nothing Then
                If Not (miisGroupManagementConnectionString.State = ConnectionState.Closed) Then
                    miisGroupManagementConnectionString.Close()
                End If
            End If
        End Try


    End Sub

    ' <summary>
    ' Assign the text of btnClose as "Save" when text is 
    ' changed in txtPreserveMember
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub txtPreserveMember_TextChanged _
                (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles txtPreserveMember.TextChanged
        'Validate the input,
        Try
            If Not (txtPreserveMember.Text = "") Then
                Dim days As Integer = Convert.ToInt32(txtPreserveMember.Text)
                If (days < 0) Then

                    Dim strMessage As String = "value can not be negative"

                    'Display alert window to the user
                    ThrowAlert(strMessage)

                    Exit Sub
                Else
                    btnClose.Text = "Save"
                End If
            Else
                btnClose.Text = "Close"
            End If
        Catch exception As Exception
            HideControls()
            lblError.Text = "Error:" + exception.Message
        End Try
    End Sub

    ' <summary>
    ' Hides specific controls upon any exception or error.
    ' </summary>

    Private Sub HideControls()

        lbInclude.Visible = False
        lbExclude.Visible = False
        pnlInclude.Visible = False
        pnlExclude.Visible = False

    End Sub

    ' <summary>
    ' Shows an alert window with the error message passed in to the user
    ' </summary>
    ' <param name="strMessage">message to display in the alert window</param>

    Private Sub ThrowAlert(ByVal strMessage As String)
        Dim strScript As String = _
                            "<script language=JavaScript>"
        strScript += "alert(""" & strMessage & """);"
        strScript += Chr(60) & "/script>"

        If Not (Page.IsStartupScriptRegistered("clientScript")) Then
            Page.RegisterStartupScript("alertScript", strScript)
        End If
    End Sub

End Class
