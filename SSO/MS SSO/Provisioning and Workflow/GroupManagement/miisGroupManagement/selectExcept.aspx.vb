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
' Enables to add exceptions to the clause defined
' <Summary/>
Public Class SelectException
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.miisConnectionString = New System.Data.SqlClient.SqlConnection
        Me.miisGroupManagementConnectionString = New System.Data.SqlClient.SqlConnection

    End Sub
    Protected WithEvents btnCancel As System.Web.UI.WebControls.Button
    Protected WithEvents btnSave As System.Web.UI.WebControls.Button
    Protected WithEvents lbSelect As System.Web.UI.WebControls.ListBox
    Protected WithEvents btnSrchGroups As System.Web.UI.WebControls.Button
    Protected WithEvents miisConnectionString As System.Data.SqlClient.SqlConnection
    Protected WithEvents TextBox1 As System.Web.UI.WebControls.TextBox
    Protected WithEvents miisGroupManagementConnectionString As _
                         System.Data.SqlClient.SqlConnection
    Protected WithEvents lblError As System.Web.UI.WebControls.Label
    Protected WithEvents lblDisplay As System.Web.UI.WebControls.Label

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init _
        (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                         Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Protected WithEvents control As _
                        System.Web.UI.HtmlControls.HtmlInputHidden
    Protected WithEvents pnlSelect As _
                                    System.Web.UI.WebControls.Panel
    Protected WithEvents ddlSearchAttribute As _
                                System.Web.UI.WebControls.DropDownList
    Protected WithEvents lblSearch As System.Web.UI.WebControls.Label
    Protected WithEvents ddlSearchOperation As _
                                System.Web.UI.WebControls.DropDownList
    Protected WithEvents txtSearchValue As _
                                    System.Web.UI.WebControls.TextBox

    ' Declarations
    Dim recordsAffected As Integer = 0

    ' Values of the Search Operations drop down list
    Const OPERATOR_EQUALS As String = "Equals"
    Const OPERATOR_STARTSWITH As String = "Starts with"
    Const OPERATOR_ENDSWITH As String = "Ends with"
    Const OPERATOR_CONTAINS As String = "Contains"
    Const OPERATOR_DOESNOTCONTAIN As String = "Does not contain"
    Const OPERATOR_ISPRESENT As String = "Is present"
    Const OPERATOR_ISNOTPRESENT As String = "Is not present"

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
            control.Value = Request.QueryString("ExceptionType").ToString()

            ' Get the connection string from the global.asax file which 
            ' gets them from the XML file
            Me.miisConnectionString.ConnectionString = _
                    ConfigurationSettings.AppSettings("miisConnectionString")
            Me.miisGroupManagementConnectionString.ConnectionString = _
                                        ConfigurationSettings.AppSettings _
                                      ("miisGroupManagementConnectionString")

            If Not IsPostBack Then

                ddlSearchAttribute.Items.Clear()
                ddlSearchAttribute.Items.Add("")

                miisConnectionString.Open()

                ' Selects the column names from the metaverse 
                ' and populates the ddlSearchAttribute dropdown box
                Dim columnSelectQuery As String = _
                    "SET ROWCOUNT 0 SELECT syscolumns.name, " _
                    + " syscolumns.type FROM syscolumns WITH (NOLOCK) " + _
                    "WHERE id IN (SELECT sysobjects.id FROM sysobjects " + _
                    "WHERE name = 'mms_metaverse') ORDER BY syscolumns.name"
                Dim columnReader As SqlDataReader = Nothing
                Dim sqlColumnColumn(2) As Object
                Dim columnCommand As SqlCommand = _
                    New SqlCommand(columnSelectQuery, miisConnectionString)
                columnReader = columnCommand.ExecuteReader()

                While (columnReader.Read())

                    columnReader.GetValues(sqlColumnColumn)

                    ' check if clause in not null or ""
                    If Not columnReader.IsDBNull(0) _
                      AndAlso Not sqlColumnColumn(0).Equals("") Then
                        Dim newItem As ListItem = Nothing
                        ' Populate the dropdown box with the 
                        ' column names from the MV
                        newItem = New ListItem(sqlColumnColumn(0).ToString(). _
                            Trim(), sqlColumnColumn(0).ToString().Trim() _
                                  + "|" + sqlColumnColumn(1).ToString().Trim())
                        ddlSearchAttribute.Items.Add(newItem)
                    End If

                End While

                miisConnectionString.Close()

            End If

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error loading page: " + exception.Message
        Finally
            If Not miisConnectionString Is Nothing Then
                If Not (miisConnectionString.State = ConnectionState.Closed) Then
                    miisConnectionString.Close()
                End If
            End If
        End Try
    End Sub

    ' <summary>
    ' Executes a search of the MV using the specified search criteria 
    ' and fills the lbSelect listbox
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns>  

    Private Sub btnSrchGroups_Click _
            (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                    Handles btnSrchGroups.Click
        Try

            ' Gets the selected value and splits it into an array
            Dim selectedValue As Array = Split _
                                    (ddlSearchAttribute.SelectedValue, "|")

            ' Selects the first 20 rows from the MV that 
            ' meets the search criteria
            Dim SelectQuery As String = "SET ROWCOUNT 20 " _
                           + "SELECT mms_metaverse.uid, " _
                           + " mms_metaverse." + "[" + selectedValue(0) + "]" + "," _
                           + " mms_metaverse.object_id, " _
                           + " mms_metaverse.displayName " _
                           + " FROM mms_metaverse WITH (NOLOCK) "
            Dim srchValueOper As String = txtSearchValue.Text
            Dim sqlSearchClause As String = String.Empty
            Dim sqlSearchSuffix As String = " AND mms_metaverse.object_type " _
                                       + " = 'person' " _
                                       + "ORDER BY mms_metaverse.displayName"

            ' Escape "'" character because it is a special character in SQL
            srchValueOper = Replace(srchValueOper, "'", "''")

            ' Build the SQL statement based on the specified criteria
            Select Case ddlSearchOperation.SelectedValue.ToLower

                Case OPERATOR_EQUALS.ToLower
                    sqlSearchClause = _
                        " where " + "[" + selectedValue(0) + "]" + " = '" _
                        + srchValueOper + "'"

                Case OPERATOR_STARTSWITH.ToLower
                    sqlSearchClause = _
                        " where " + "[" + selectedValue(0) + "]" + " like '" _
                        + srchValueOper + "%'"

                Case OPERATOR_ENDSWITH.ToLower
                    sqlSearchClause = _
                        " where " + "[" + selectedValue(0) + "]" + " like '%" _
                        + srchValueOper + "'"

                Case OPERATOR_CONTAINS.ToLower
                    sqlSearchClause = _
                        " where " + "[" + selectedValue(0) + "]" + " like '%" _
                        + srchValueOper + "%'"

                Case OPERATOR_DOESNOTCONTAIN.ToLower
                    sqlSearchClause = _
                        " where " + "[" + selectedValue(0) + "]" + " not like '%" _
                        + srchValueOper + "%'"

                Case OPERATOR_ISPRESENT.ToLower
                    sqlSearchClause = _
                        " where (" + "[" + selectedValue(0) + "]" + " is not null and " _
                        + "[" + selectedValue(0) + "]" + " <> '')"

                Case OPERATOR_ISNOTPRESENT.ToLower
                    sqlSearchClause = _
                        " where (" + "[" + selectedValue(0) + "]" + " is null or " _
                        + "[" + selectedValue(0) + "]" + " = '')"

            End Select

            ' Compiles the final SQL query
            SelectQuery = SelectQuery + sqlSearchClause + sqlSearchSuffix

            lbSelect.Items.Clear()

            ' Selects the values from the metaverse 
            ' and populates the lbSelect listbox
            If Not selectedValue(0) = "" And Not _
                 ddlSearchOperation.SelectedValue = "" _
                   And (Not txtSearchValue.Text = "" Or _
                        txtSearchValue.Enabled = False) Then

                miisConnectionString.Open()

                Dim groupReader As SqlDataReader = Nothing
                Dim sqlColumn(3) As Object
                Dim command As SqlCommand = _
                       New SqlCommand(SelectQuery, miisConnectionString)

                groupReader = command.ExecuteReader()

                While (groupReader.Read())

                    groupReader.GetValues(sqlColumn)

                    If Not groupReader.IsDBNull(2) AndAlso Not _
                                 sqlColumn(2).Equals("") Then
                        Dim newItem As ListItem = Nothing

                        ' Fill the listbox
                        ' The listBox dispays the UID - 
                        ' dispayName as the text and 
                        ' the <object-id> as the value
                        newItem = New ListItem(sqlColumn(0).ToString().Trim() _
                         + " - " + sqlColumn(3).ToString().Trim(), _
                           "{" + sqlColumn(2).ToString().Trim() + "}")
                        lbSelect.Items.Add(newItem)
                    End If

                End While

                miisConnectionString.Close()

            End If

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error searching: " + exception.Message
        Finally
            If Not miisConnectionString Is Nothing Then
                If Not (miisConnectionString.State = ConnectionState.Closed) Then
                    miisConnectionString.Close()
                End If
            End If
        End Try

    End Sub

    ' <summary>
    ' Saves include/exclude data to the database depending on what was selected
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub btnSave_Click _
            (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                         Handles btnSave.Click

        Try

            If lbSelect.SelectedValue = "" Then

                ' Show popup message if there are no objects selected 
                ' and the user tries to save the data
                Dim strMessage As String = _
                  "You must select a user from the list in ordert to add."
                'Display alert window to the user
                ThrowAlert(strMessage)

                Exit Sub
            End If

            ' Get the UID of the group that we are working with
            Dim groupObjectUID As String = _
                         Request.QueryString("groupObjectUID").ToString()
            Dim groupDisplayName As String = _
                         Request.QueryString("groupDisplayName").ToString()
            Dim selectionValue As ListItem = Nothing

            miisGroupManagementConnectionString.Open()

            ' Add each selected item to the exceptionDefinitions table
            For Each selectionValue In lbSelect.Items

                If selectionValue.Selected Then

                    Dim insertWhereStatement As String = _
                        "INSERT INTO exceptionDefinitions " _
                        + "(objectUID, exceptType, mvObjectUID) " _
                        + " VALUES ('" + groupObjectUID + "', " _
                        + "'" + control.Value + "', '" + _
                        selectionValue.Value + "')"
                    Dim insertWhereCommand As SqlCommand = New _
                        SqlCommand(insertWhereStatement, _
                            miisGroupManagementConnectionString)
                    recordsAffected = insertWhereCommand.ExecuteNonQuery()

                    EventLog.WriteEntry("miisGroupManagement", _
                        Environment.UserDomainName.ToLower + "\" + _
                        Environment.UserName.ToLower _
                        + "added thefollowing exception for group '" _
                        + groupDisplayName + "' with object UID '" _
                        + groupObjectUID + "':" + vbCrLf + vbCrLf + _
                        control.Value.ToUpper + " - '" + selectionValue.Text _
                        + "'", EventLogEntryType.Information)

                End If

            Next

            miisGroupManagementConnectionString.Close()

            ' Transfer back to the buildExcept page in this window
            Server.Transfer("buildExcept.aspx", True)

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error saving: " + exception.Message
        Finally
            If Not miisGroupManagementConnectionString Is Nothing Then
                If Not (miisGroupManagementConnectionString.State = ConnectionState.Closed) Then
                    miisGroupManagementConnectionString.Close()
                End If
            End If
        End Try

    End Sub

    ' <summary>
    ' Close the window without saving the data when "Cancel" button clicked
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub btnCancel_Click _
            (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                   Handles btnCancel.Click

        Try

            ' Close the window without saving the data
            Dim strCloseScript As String = "<script>self.close()"
            strCloseScript += "</script>"
            RegisterClientScriptBlock("key7", strCloseScript)

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error btnCancel_Click: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Executes when a new value is selected from the ddlSearchAttribute
    ' dropdown Builds the contents of and enables the ddlSearchOperation
    ' dropdown</summary>
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
                If selectedValue(1) = 35 Then

                Else
                    ddlSearchOperation.Items.Add(OPERATOR_EQUALS)

                End If

                ddlSearchOperation.Items.Add(OPERATOR_STARTSWITH)
                ddlSearchOperation.Items.Add(OPERATOR_ENDSWITH)
                ddlSearchOperation.Items.Add(OPERATOR_CONTAINS)
                ddlSearchOperation.Items.Add(OPERATOR_DOESNOTCONTAIN)
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
            lblError.Text = _
                "Error srchAttrib_SelectedIndexChanged: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Executes when a new value is selected from the ddlSearchOperation 
    ' dropdown.Enables the txtSearchValue depending on the selected value
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub ddlSearchOperation_SelectedIndexChanged _
              (ByVal sender As System.Object, ByVal e As System.EventArgs) _
              Handles ddlSearchOperation.SelectedIndexChanged

        Try

            ' There is no need to enable the txtSearchValue box if the 
            ' ddlSearchOperation is "Is not present" or "is present"
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
            lblError.Text = _
                "Error srchOper_SelectedIndexChanged: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Hides specific controls upon any exception or error.
    ' </summary>

    Private Sub HideControls()

        lbSelect.Visible = False
        pnlSelect.Visible = False

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
