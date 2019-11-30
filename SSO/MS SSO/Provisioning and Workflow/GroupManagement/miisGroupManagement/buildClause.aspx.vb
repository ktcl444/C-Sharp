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

Imports System.Data.SqlClient
Imports System.Diagnostics

' <summary>
' Represents the BuildClause.aspx file, which includes all the methods 
' involving activities of building a clause for a particular group
' </summary>

Public Class BuildClause
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.miisGroupManagementConnectionString = New System.Data.SqlClient.SqlConnection
        Me.miisConnectionString = New System.Data.SqlClient.SqlConnection

    End Sub
    Protected WithEvents btnSave As System.Web.UI.WebControls.Button
    Protected WithEvents tbSQLClause As System.Web.UI.WebControls.TextBox
    Protected WithEvents btnCancel As System.Web.UI.WebControls.Button
    Protected WithEvents btnAppend As System.Web.UI.WebControls.Button
    Protected WithEvents btnReplace As System.Web.UI.WebControls.Button
    Protected WithEvents cbClause As System.Web.UI.WebControls.CheckBox
    Protected WithEvents lbGroupSelect As System.Web.UI.WebControls.ListBox
    Protected WithEvents btnSelect As System.Web.UI.WebControls.Button
    Protected WithEvents btnSelectCancel As System.Web.UI.WebControls.Button
    Protected WithEvents miisGroupManagementConnectionString As System.Data.SqlClient.SqlConnection
    Protected WithEvents miisConnectionString As System.Data.SqlClient.SqlConnection
    Protected WithEvents lblError As System.Web.UI.WebControls.Label
    Protected WithEvents btnPreview As System.Web.UI.WebControls.Button
    Protected WithEvents ddlSearchAttribute As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lblSearch As System.Web.UI.WebControls.Label
    Protected WithEvents ddlSearchOperation As System.Web.UI.WebControls.DropDownList
    Protected WithEvents txtSearchValue As System.Web.UI.WebControls.TextBox
    Protected WithEvents rdbAndOr As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents pnlGroupSelect As System.Web.UI.WebControls.Panel
    Protected WithEvents pnlSQLClause As System.Web.UI.WebControls.Panel

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

    ' Values to be added to the ddl search operations
    Const OPERATOR_LIKE As String = "Like"
    Const OPERATOR_EQUALS As String = "Equals"
    Const OPERATOR_STARTS_WITH As String = "Starts with"
    Const OPERATOR_ENDS_WITH As String = "Ends with"
    Const OPERATOR_CONTAINS As String = "Contains"
    Const OPERATOR_DOESNOT_CONTAIN As String = "Does not contain"
    Const OPERATOR_ISPRESENT As String = "Is present"
    Const OPERATOR_ISNOTPRESENT As String = "Is not present"


    ' <summary>
    ' Gets executed at the page load, initially and during post
    ' backs as well</summary>
    ' <param name="sender">Sender object</param>
	' <param name="e">Event Arguments</param>    
    ' <returns></returns> 
    
    Private Sub Page_Load(ByVal sender As System.Object, _
                        ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            ' Get the connection string from the global.asax file 
            ' which gets them from the XML file
            Me.miisConnectionString.ConnectionString = _
                    ConfigurationSettings.AppSettings("miisConnectionString")
            Me.miisGroupManagementConnectionString.ConnectionString = _
                                     ConfigurationSettings.AppSettings _
                                    ("miisGroupManagementConnectionString")

            If Not IsPostBack Then

                Dim clauseCounter As Integer = 0

                ' Get the values that are being passed in on the URL.
                ' These values are used to know which group is worked upon
                Dim groupObjectUID As String = Request.QueryString _
                                           ("groupObjectUID").ToString()
                Dim groupDisplayName As String = Request.QueryString _
                                           ("groupDisplayName").ToString()
                Dim sqlClause As String = String.Empty
                sqlClause = Request.QueryString("sqlClause").ToString()

                ' If the displayname is present, change the text to 
                ' show the group being worked on
                If Not groupDisplayName = "" Then
                    lblSearch.Text = "Specify Clause Criteria for '" _
                                               + groupDisplayName + "'"
                End If

                If Not sqlClause = "" Then
                    tbSQLClause.Text = sqlClause
                    miisConnectionString.Open()

                    ' Call the CbClauseIsNotChecked method to change
                    ' settings based on if the clause is shared or not
                    CbClauseIsNotChecked()
                    miisConnectionString.Close()
                Else

                    Dim usesOtherClauseDefinitions As Boolean = False

                    miisGroupManagementConnectionString.Open()

                    ' Select the clauseLink (clauseLink) from the 
                    ' groupDefinitions table in order to check and see 
                    ' if the group is using its own clause or another
                    Dim objectUIDReader As SqlDataReader = Nothing
                    Dim sqlObjectUIDColumn(2) As Object
                    Dim objectUIDQuery As String = "SELECT objectUID, " + _
                        "clauseLink FROM groupDefinitions WHERE objectUID " + _
                                            "= '" + groupObjectUID + "'"
                    Dim objectUIDCommand As SqlCommand = New SqlCommand _
                        (objectUIDQuery, miisGroupManagementConnectionString)
                    objectUIDReader = objectUIDCommand.ExecuteReader()

                    While (objectUIDReader.Read())
                        objectUIDReader.GetValues(sqlObjectUIDColumn)

                        ' Make sure objectUID and clauseLink are not null
                        If Not objectUIDReader.IsDBNull(0) And Not _
                                            objectUIDReader.IsDBNull(1) Then

                            ' if the group is not using its own clause 
                            ' (sharing another groups)
                            If Not sqlObjectUIDColumn(0).ToString().Trim() _
                                = sqlObjectUIDColumn(1).ToString().Trim() Then

                                If Not (objectUIDReader Is Nothing) Then
                                    objectUIDReader.Close()
                                End If

                                cbClause.Checked = True

                                ' call the CbClauseIsChecked sub
                                CbClauseIsChecked(sqlObjectUIDColumn(1). _
                                                            ToString().Trim())
                                usesOtherClauseDefinitions = True
                                Exit While
                            End If
                        End If
                    End While

                    If Not (objectUIDReader Is Nothing) Then
                        objectUIDReader.Close()
                    End If

                    ' if it uses its own clause then select it from the 
                    ' clauseDefinitions table for display
                    If usesOtherClauseDefinitions = False Then
                        Dim groupReader As SqlDataReader = Nothing
                        Dim sqlColumn(1) As Object
                        Dim selectQuery As String = "SELECT clause FROM " + _
                                    "clauseDefinitions WHERE objectUID = " + _
                                                    "'" + groupObjectUID + "'"
                        Dim command As SqlCommand = New SqlCommand _
                            (selectQuery, miisGroupManagementConnectionString)
                        groupReader = command.ExecuteReader()

                        ' If groupReader contains rows...
                        If groupReader.HasRows = True Then
                            usesOtherClauseDefinitions = False
                            While (groupReader.Read())
                                If clauseCounter > 0 Then
                                    tbSQLClause.Text = "Error: multiple " + _
                                    "where clauses were found for this group"
                                    clauseCounter = -1
                                    Exit While
                                End If

                                groupReader.GetValues(sqlColumn)

                                ' make sure clause in not null or ""
                                If Not groupReader.IsDBNull(0) AndAlso _
                                          Not sqlColumn(0).Equals("") Then
                                    tbSQLClause.Text = sqlColumn(0). _
                                                        ToString().Trim()
                                    clauseCounter = 1
                                End If

                            End While

                            If Not (groupReader Is Nothing) Then
                                groupReader.Close()
                            End If

                        End If

                        miisConnectionString.Open()

                        ' Call the CbClauseIsNotChecked method to change
                        ' settings based on if the clause is shared or not
                        CbClauseIsNotChecked()
                        miisConnectionString.Close()

                    End If

                    miisGroupManagementConnectionString.Close()

                End If

                ' depending on whether the group already has a 
                ' clause or not, change the buttons and text
                If tbSQLClause.Text = "" Then
                    btnReplace.Text = "Add"
                    btnReplace.ToolTip = "Add"
                    btnAppend.Visible = False
                    rdbAndOr.Visible = False
                Else
                    btnReplace.Text = "Replace"
                    btnReplace.ToolTip = "Replace"
                    btnAppend.Visible = True
                    rdbAndOr.Visible = True
                End If

                'Else
                '    CbClauseIsNotChecked()

            End If

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error Page_Load: " + exception.Message
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
    ' Executes when the 'Update' button is clicked after the clause 
    ' is entered</summary>
    ' <param name="sender">Sender object</param>
	' <param name="e">Event Arguments</param>    
    ' <returns></returns> 

    Private Sub btnSave_Click(ByVal sender As System.Object, _
                    ByVal e As System.EventArgs) Handles btnSave.Click

        Try

            ' Get the values that are being passed in on the URL.
            ' These values are used to know which group is being worked upon
            Dim groupObjectUID As String = Request.QueryString _
                                                ("groupObjectUID").ToString()
            Dim groupDisplayName As String = Request.QueryString _
                                              ("groupDisplayName").ToString()

            miisGroupManagementConnectionString.Open()

            ' Escape "'" character because it is a special character in SQL
            Dim sqlClause As String = Replace(tbSQLClause.Text, "'", "''")

            ' Delete all of the existing clauses for this group 
            ' so that the new ones can be saved
            Dim deleteWhereStatement As String = "DELETE FROM " + _
                                "clauseDefinitions WHERE objectUID " + _
                                                "= '" + groupObjectUID + "'"
            Dim deleteWhereCommand As SqlCommand = New _
                            SqlCommand(deleteWhereStatement, _
                                    miisGroupManagementConnectionString)
            recordsAffected = deleteWhereCommand.ExecuteNonQuery()

            ' Insert the new clause for this group
            Dim insertWhereStatement As String = "INSERT INTO " + _
                "clauseDefinitions (objectUID, clauseType, clause) " + _
                                "VALUES ('" + groupObjectUID + "', " + _
                                        "'clause', '" + sqlClause + "')"
            Dim insertWhereCommand As SqlCommand = New _
                                SqlCommand(insertWhereStatement, _
                                    miisGroupManagementConnectionString)
            recordsAffected = insertWhereCommand.ExecuteNonQuery()

            ' Update the groupDefinitions table to have this group
            ' pointing to iteself for the clause
            Dim updateWhereStatement As String = _
                "UPDATE groupDefinitions SET clauseLink = " + _
                    "'" + groupObjectUID + "' WHERE objectUID = " + _
                                                "'" + groupObjectUID + "'"
            Dim updateWhereCommand As SqlCommand = New _
                            SqlCommand(updateWhereStatement, _
                                miisGroupManagementConnectionString)
            recordsAffected = updateWhereCommand.ExecuteNonQuery()

            miisGroupManagementConnectionString.Close()

            EventLog.WriteEntry("miisGroupManagement", _
                Environment.UserDomainName.ToLower + "\" + _
                Environment.UserName.ToLower + " updated the follwing " + _
                "for group '" + groupDisplayName + "' " + _
                "with object UID '" + groupObjectUID + "':" + _
                vbCrLf + vbCrLf + "Clause - '" + sqlClause + "'", _
                EventLogEntryType.Information)

            ' Close the window
            Dim strCloseScript As String = "<script>self.close();"
            strCloseScript += "</script>"
            RegisterClientScriptBlock("key3", strCloseScript)

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error btnSave_Click: " + exception.Message
        Finally
            If Not miisGroupManagementConnectionString Is Nothing Then
                If Not (miisGroupManagementConnectionString.State = _
                                                    ConnectionState.Closed) Then
                    miisGroupManagementConnectionString.Close()
                End If
            End If
        End Try

    End Sub

    ' <summary>
    ' Executes when an existing group clause is selected and the 
    ' 'Select' button is clicked 
    ' </summary>
    ' <param name="sender">Sender object</param>
	' <param name="e">Event Arguments</param>    
    ' <returns></returns> 

    Private Sub btnSelect_Click(ByVal sender As System.Object, _
                    ByVal e As System.EventArgs) Handles btnSelect.Click

        Try

            ' Get the values that are being passed in on the URL.These 
            ' values are used to know which group is being worked upon
            Dim groupObjectUID As String = Request.QueryString _
                                            ("groupObjectUID").ToString()
            
            miisGroupManagementConnectionString.Open()

            ' Delete all of the existing clauses for this group
            Dim deleteWhereStatement As String = "DELETE FROM " + _
                "clauseDefinitions WHERE objectUID = '" + groupObjectUID + "'"
            Dim deleteWhereCommand As SqlCommand = New _
                            SqlCommand(deleteWhereStatement, _
                                    miisGroupManagementConnectionString)
            recordsAffected = deleteWhereCommand.ExecuteNonQuery()

            ' Update the groupDefinitions table to have this group 
            ' point to the selected group for its clause
            Dim updateWhereStatement As String = "UPDATE " + _
                "groupDefinitions SET clauseLink = '" + _
                   lbGroupSelect.SelectedValue + "' WHERE objectUID = '" + _
                                                       groupObjectUID + "'"
            Dim updateWhereCommand As SqlCommand = New _
                            SqlCommand(updateWhereStatement, _
                                    miisGroupManagementConnectionString)
            recordsAffected = updateWhereCommand.ExecuteNonQuery()

            miisGroupManagementConnectionString.Close()

            ' Close the window
            Dim strCloseScript As String = "<script>self.close()"
            strCloseScript += "</" + "script>"
            RegisterClientScriptBlock("key3", strCloseScript)

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error btnSelect_Click: " + exception.Message
        Finally
            If Not miisGroupManagementConnectionString Is Nothing Then
                If Not (miisGroupManagementConnectionString.State = _
                                                    ConnectionState.Closed) Then
                    miisGroupManagementConnectionString.Close()
                End If
            End If
        End Try

    End Sub

    ' <summary>
    ' Executes when the 'Cancel' button is clicked and 
    ' closes the currently opened window without updating
    ' </summary>
    ' <param name="sender">Sender object</param>
	' <param name="e">Event Arguments</param>    
    ' <returns></returns> 

    Private Sub btnCancel_Click(ByVal sender As System.Object, _
                    ByVal e As System.EventArgs) Handles btnCancel.Click

        Try

            ' Close the window
            Dim strCloseScript As String = "<script>self.close()"
            strCloseScript += "</" + "script>"
            RegisterClientScriptBlock("key3", strCloseScript)

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error btnCancel_Click: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Executes when a new value is selected from the 
    ' ddlSearchAttribute dropdown list, builds the contents 
    ' and enables the ddlSearchOperation.
    ' </summary>
    ' <param name="sender">Sender object</param>
	' <param name="e">Event Arguments</param>    
    ' <returns></returns> 

    Private Sub ddlSearchAttribute_SelectedIndexChanged _
            (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles ddlSearchAttribute.SelectedIndexChanged

        Try

            If Not ddlSearchAttribute.SelectedValue = "" Then

                ddlSearchOperation.Items.Clear()
                ddlSearchOperation.Items.Add("")

                ddlSearchOperation.Enabled = True
                ddlSearchOperation.BackColor = System.Drawing.Color.White
                ' Split the selectedValue into an array
                Dim selectedValue As Array = Split _
                                (ddlSearchAttribute.SelectedValue, "|")

                ' The value of 35 means that the attribute is a string that
                ' is of type string (non-indexable) in the MV
                ' non-indexable strings in the MV are stored as type ntext 
                ' which cannot use the = command.
                ' These queries must use the 'like' operator instead of '='
                ' Build the dropdown accordingly
                If (selectedValue(1) = 35) Then
                    ddlSearchOperation.Items.Add(OPERATOR_LIKE)
                Else
                    ddlSearchOperation.Items.Add(OPERATOR_EQUALS)
                End If

                ddlSearchOperation.Items.Add(OPERATOR_STARTS_WITH)
                ddlSearchOperation.Items.Add(OPERATOR_ENDS_WITH)
                ddlSearchOperation.Items.Add(OPERATOR_CONTAINS)
                ddlSearchOperation.Items.Add(OPERATOR_DOESNOT_CONTAIN)
                ddlSearchOperation.Items.Add(OPERATOR_ISPRESENT)
                ddlSearchOperation.Items.Add(OPERATOR_ISNOTPRESENT)

            Else
                txtSearchValue.Text = ""
                txtSearchValue.Enabled = False
                txtSearchValue.BackColor = System.Drawing.Color.LightGray

                ddlSearchOperation.Items.Clear()
                ddlSearchOperation.Items.Add("")
                ddlSearchOperation.Enabled = False
                ddlSearchOperation.BackColor = System.Drawing.Color.LightGray

            End If

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error srchAttrib_SelectedIndexChanged: " _
                                                        + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Executes when a new value is selected from the ddlSearchOperation
    ' dropdown list and enables the txtSearchValue depending on 
    ' the selected value</summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>    
    ' <returns></returns> 

    Private Sub ddlSearchOperation_SelectedIndexChanged _
                (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                    Handles ddlSearchOperation.SelectedIndexChanged
        Try

            ' There is no need to enable the txtSearchValue box if the 
            ' ddlSearchOperation is "Is Present" or "Is Not Present"
            Select Case ddlSearchOperation.SelectedValue.ToLower
                Case OPERATOR_ISNOTPRESENT.ToLower, OPERATOR_ISPRESENT.ToLower, ""
                    txtSearchValue.Text = ""
                    txtSearchValue.Enabled = False
                    txtSearchValue.BackColor = System.Drawing.Color.LightGray

                Case Else
                    txtSearchValue.Enabled = True
                    txtSearchValue.BackColor = System.Drawing.Color.White
            End Select

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error srchOper_SelectedIndexChanged: " _
                                                          + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Appends the clause that was built to the tbSQLClause 
    ' textbox using 'and' or 'or' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub btnAppend_Click _
        (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                    Handles btnAppend.Click

        Try

            ' Provide a message if the 'and' or 'or' radio button is 
            ' not selected when the append button is clicked
            If rdbAndOr.SelectedValue = "" Then
                Dim strMessage As String = _
                        "The 'and' or 'or' radio button must be selected."

                'Display alert window to the user
                ThrowAlert(strMessage)

                Exit Sub
            End If

            Dim appendClause As String = String.Empty
            Dim selectedValue As Array = _
                    Split(ddlSearchAttribute.SelectedValue, "|")

            ' Build the where clause depending on the selected 
            ' value on the ddlSearchOperation
            Select Case ddlSearchOperation.SelectedValue.ToLower

                Case OPERATOR_EQUALS.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " = '" _
                                         + txtSearchValue.Text + "'"

                Case OPERATOR_LIKE.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " like '" _
                                         + txtSearchValue.Text + "'"

                Case OPERATOR_STARTS_WITH.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " like '" _
                                         + txtSearchValue.Text + "%'"

                Case OPERATOR_ENDS_WITH.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " like '%" _
                                         + txtSearchValue.Text + "'"

                Case OPERATOR_CONTAINS.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " like '%" _
                                         + txtSearchValue.Text + "%'"

                Case OPERATOR_DOESNOT_CONTAIN.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " not like '%" _
                                         + txtSearchValue.Text + "%'"

                Case OPERATOR_ISPRESENT.ToLower
                    appendClause = "(" + "[" + selectedValue(0) + "]" + _
                        " is not null and " + "[" + selectedValue(0) + "]" + " <> '') "

                Case OPERATOR_ISNOTPRESENT.ToLower
                    appendClause = "(" + "[" + selectedValue(0) + "]" + _
                            " is null or " + "[" + selectedValue(0) + "]" + " = '')"

            End Select

            ' Build the SQL statement
            tbSQLClause.Text = tbSQLClause.Text + " " + _
                                rdbAndOr.SelectedValue + " " + appendClause

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error btnAppend_Click: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Replaces the clause that was built to the tbSQLClause textbox
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub btnReplace_Click _
        (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                Handles btnReplace.Click

        Try

            Dim appendClause As String = String.Empty
            Dim selectedValue As Array = _
                        Split(ddlSearchAttribute.SelectedValue, "|")

            ' Build the where clause depending on the 
            ' selected value on the ddlSearchOperation
            Select Case ddlSearchOperation.SelectedValue.ToLower

                Case OPERATOR_EQUALS.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " = '" _
                                          + txtSearchValue.Text + "'"

                Case OPERATOR_LIKE.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " like '" _
                                          + txtSearchValue.Text + "'"

                Case OPERATOR_STARTS_WITH.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " like '" _
                                          + txtSearchValue.Text + "%'"

                Case OPERATOR_ENDS_WITH.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " like '%" _
                                          + txtSearchValue.Text + "'"

                Case OPERATOR_CONTAINS.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " like '%" _
                                          + txtSearchValue.Text + "%'"

                Case OPERATOR_DOESNOT_CONTAIN.ToLower
                    appendClause = "[" + selectedValue(0) + "]" + " not like '%" _
                                          + txtSearchValue.Text + "%'"

                Case OPERATOR_ISPRESENT.ToLower
                    appendClause = "(" + "[" + selectedValue(0) + "]" + _
                        " is not null and " + "[" + selectedValue(0) + "]" + " <> '') "

                Case OPERATOR_ISNOTPRESENT.ToLower
                    appendClause = "(" + "[" + selectedValue(0) + "]" + _
                            " is null or " + "[" + selectedValue(0) + "]" + " = '')"

            End Select

            ' Build the SQL statement
            tbSQLClause.Text = appendClause

            ' depending on whether the group has a clause or not,
            ' change the buttons and text
            If tbSQLClause.Text = "" Then
                btnReplace.Text = "Add"
                btnReplace.ToolTip = "Add"
                btnAppend.Visible = False
                rdbAndOr.Visible = False
            Else
                btnReplace.Text = "Replace"
                btnReplace.ToolTip = "Replace"
                btnAppend.Visible = True
                rdbAndOr.Visible = True
            End If

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error btnReplace_Click: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Executes when the cbClause checkbox control is checked
    ' or unchecked</summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub cbClause_CheckedChanged _
        (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                        Handles cbClause.CheckedChanged

        Try

            Select Case cbClause.Checked
                Case True
                    miisGroupManagementConnectionString.Open()

                    ' call the CbClauseIsChecked method if 
                    ' the checkbox control is checked
                    CbClauseIsChecked()
                    miisGroupManagementConnectionString.Close()

                Case False
                    miisConnectionString.Open()

                    ' call the CbClauseIsNotChecked method if
                    ' the checkbox control is not checked
                    CbClauseIsNotChecked()
                    miisConnectionString.Close()

            End Select

        Catch exception As Exception
            HideControls()
            lblError.Text = _
                "Error cbClause_CheckedChanged: " + exception.Message
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
    ' Enables the page to select a clause from an existing group
    ' </summary>
    ' <param name="selectedGroup">Selected Group name</param>
    ' <returns></returns> 

    Private Sub CbClauseIsChecked(Optional ByVal selectedGroup As String = "")

        Try
            ' Get the values that are being passed in on the URL.These 
            ' values are used to know which group is being worked upon
            Dim groupDisplayName As String = Request.QueryString _
                                        ("groupDisplayName").ToString()

            lbGroupSelect.Items.Clear()

            ' Since the group is using the clause from another group
            ' set the correct boxes and buttons to be visible
            ddlSearchOperation.Enabled = False
            rdbAndOr.Visible = False
            ddlSearchAttribute.Enabled = False
            txtSearchValue.Enabled = False
            btnAppend.Visible = False
            btnReplace.Visible = False
            tbSQLClause.Visible = False
            pnlSQLClause.Visible = False
            btnSave.Visible = False
            btnCancel.Visible = False
            btnPreview.Visible = False
            ddlSearchOperation.Items.Clear()
            ddlSearchOperation.Items.Add("")
            ddlSearchAttribute.BackColor = System.Drawing.Color.LightGray
            ddlSearchOperation.BackColor = System.Drawing.Color.LightGray
            txtSearchValue.Text = ""
            txtSearchValue.BackColor = System.Drawing.Color.LightGray

            btnSelect.Visible = True
            btnSelectCancel.Visible = True
            lbGroupSelect.Visible = True
            pnlGroupSelect.Visible = True

            ' Build the sql statement that selects the groups from the 
            ' groupDefinitions table with associated clause
            Dim groupReader As SqlDataReader = Nothing
            Dim sqlGroupColumn(1) As Object
            Dim groupSelectQuery As String = "SELECT " + _
                "groupDefinitions.displayName, " + _
                "groupDefinitions.clauseLink " + _
                "FROM groupDefinitions ORDER BY groupDefinitions.displayName"
            Dim command As SqlCommand = New SqlCommand _
                    (groupSelectQuery, miisGroupManagementConnectionString)
            groupReader = command.ExecuteReader()

            While (groupReader.Read())
                groupReader.GetValues(sqlGroupColumn)

                ' make sure that the displayName and the clause 
                ' are not null or ""
                If Not groupReader.IsDBNull(0) And Not _
                        groupReader.IsDBNull(1) AndAlso Not _
                                sqlGroupColumn(0).Equals("") Then

                    Dim NewItem As ListItem

                    ' if the value is not equal to the displayName then add 
                    ' the item to the listox. This is because one needs to 
                    ' choose a different group than what is being worked upon
                    If Not sqlGroupColumn(0).ToString().Trim() = _
                                                    groupDisplayName Then
                        NewItem = New ListItem _
                            (sqlGroupColumn(0).ToString().Trim(), _
                                sqlGroupColumn(1).ToString().Trim())
                        lbGroupSelect.Items.Add(NewItem)
                    End If

                End If

            End While

            If Not selectedGroup = "" Then
                lbGroupSelect.SelectedValue = selectedGroup
            End If

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error CbClauseIsChecked: " + _
                exception.Message + " - Check to make sure that this " + _
                    "groups clause is not pointing to a group that " + _
                        "points to another groups clause."
        End Try

    End Sub

    ' <summary>
    ' Enables the page to build a new clause for this group
    ' </summary>
    ' <returns></returns> 

    Private Sub CbClauseIsNotChecked()

        Try

            ' Since the group is not using the clause from another group
            ' set the correct boxes and buttons to be visible
            ddlSearchAttribute.Enabled = True
            rdbAndOr.Visible = True
            btnAppend.Visible = True
            btnReplace.Visible = True
            tbSQLClause.Visible = True
            pnlSQLClause.Visible = True
            btnSave.Visible = True
            btnCancel.Visible = True
            lblSearch.Visible = True
            btnPreview.Visible = True
            ddlSearchAttribute.BackColor = System.Drawing.Color.White
            ddlSearchOperation.BackColor = System.Drawing.Color.LightGray
            txtSearchValue.BackColor = System.Drawing.Color.LightGray

            btnSelect.Visible = False
            btnSelectCancel.Visible = False
            lbGroupSelect.Visible = False
            pnlGroupSelect.Visible = False

            If tbSQLClause.Text = "" Then
                btnReplace.Text = "Add"
                btnReplace.ToolTip = "Add"
                btnAppend.Visible = False
                rdbAndOr.Visible = False
            Else
                btnReplace.Text = "Replace"
                btnReplace.ToolTip = "Replace"
                btnAppend.Visible = True
                rdbAndOr.Visible = True
            End If

            ddlSearchAttribute.Items.Clear()
            ddlSearchAttribute.Items.Add("")

            ' Build the sql statement that selects the columns from 
            ' MIIS to populate the ddlSearchAttribute dropdown
            Dim columnSelectQuery As String = "SELECT syscolumns.name," + _
                " syscolumns.type FROM syscolumns WITH (NOLOCK) WHERE " + _
                "id IN (SELECT sysobjects.id FROM sysobjects WHERE name " + _
                "= 'mms_metaverse') ORDER BY syscolumns.name"

            Dim columnReader As SqlDataReader = Nothing
            Dim sqlColumnColumn(2) As Object
            Dim columnCommand As SqlCommand = New SqlCommand _
                                (columnSelectQuery, miisConnectionString)
            columnReader = columnCommand.ExecuteReader()

            While (columnReader.Read())

                columnReader.GetValues(sqlColumnColumn)

                ' make sure clause in not null or ""
                If Not columnReader.IsDBNull(0) AndAlso Not _
                                        sqlColumnColumn(0).Equals("") Then
                    Dim NewItem As ListItem
                    ' The value is stored as a combo of the text and the 
                    ' column type such as 'object_id|37'. This is then split
                    ' later in order to retrieve both the column name and type
                    NewItem = New ListItem _
                        (sqlColumnColumn(0).ToString().Trim(), _
                            sqlColumnColumn(0).ToString().Trim() + "|" _
                                + sqlColumnColumn(1).ToString().Trim())
                    ddlSearchAttribute.Items.Add(NewItem)
                End If

            End While

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error cbClauseIsNotChecked: " + _
                                                    exception.Message
        End Try

    End Sub

    ' <summary>
    ' Closes the page if the 'Cancel' button is clicked without 
    ' doing any updation
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub btnSelectCancel_Click _
            (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                           Handles btnSelectCancel.Click

        Try

            ' Closes the buildClause window
            Dim strCloseScript As String = "<script>self.close()"
            strCloseScript += "</" + "script>"
            RegisterClientScriptBlock("key3", strCloseScript)

        Catch exception As Exception
            HideControls()
            lblError.Text = _
                "Error btnSelectCancel_Click: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' A preview window with all the opted options will be displayed
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub btnPreview_Click( _
            ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                             Handles btnPreview.Click
        Try

            If Not tbSQLClause.Text = "" Then
                ' Pass in that this is to add an exclusion
                Server.Transfer("previewClause.aspx?groupDisplayName=" _
                    + Request.QueryString("groupDisplayName").ToString() _
                    + "&groupObjectUID=" + Request.QueryString _
                    ("groupObjectUID").ToString() + "&sqlClause=" + _
                    Server.UrlEncode(tbSQLClause.Text), True)
            End If

        Catch exception As Exception
            HideControls()
            lblError.Text = _
                "Error btnSelectCancel_Click: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Hides specific controls upon any exception or error.
    ' </summary>

    Private Sub HideControls()

        tbSQLClause.Visible = False
        pnlSQLClause.Visible = False
        lbGroupSelect.Visible = False
        pnlGroupSelect.Visible = False

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
