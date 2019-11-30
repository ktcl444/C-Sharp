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
' Mail status of the group can be opted and generated 
' from the given set of options like mail enabled or disabled
' <Summary/>
Public Class BuildMail
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.miisGroupManagementConnectionString = New System.Data.SqlClient.SqlConnection

    End Sub
    Protected WithEvents btnCancel As System.Web.UI.WebControls.Button
    Protected WithEvents btnSave As System.Web.UI.WebControls.Button
    Protected WithEvents lblError As System.Web.UI.WebControls.Label
    Protected WithEvents miisGroupManagementConnectionString As System.Data.SqlClient.SqlConnection
    Protected WithEvents pnlMailStatus As System.Web.UI.WebControls.Panel
    Protected WithEvents txtDefault As System.Web.UI.WebControls.TextBox
    Protected WithEvents rdblMail As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents txtCustom As System.Web.UI.WebControls.TextBox
    Protected WithEvents lblDisplayName As System.Web.UI.WebControls.Label

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    ' Declaration
    Dim recordsAffected As Integer = 0
    Const GROUPNAME_LENGTH_MAX = 25
    Const GROUPNAME_LENGTH_TRUNCATE = 22

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

            ' Get the connection string from the global.asax file which 
            ' gets them from the XML file 
            ' Me.miisConnectionString.ConnectionString()
            ' = ConfigurationSettings.AppSettings("miisConnectionString")
            Me.miisGroupManagementConnectionString.ConnectionString _
                                = ConfigurationSettings.AppSettings _
                                    ("miisGroupManagementConnectionString")

            If Not IsPostBack Then

                Dim clauseCounter As Integer = 0

                ' Get the values that are being passed in on the URL.
                ' These values are used to know which group is being 
                ' worked upon
                Dim groupObjectUID As String = _
                            Request.QueryString("groupObjectUID").ToString()
                Dim groupDisplayName As String = _
                            Request.QueryString("groupDisplayName").ToString()

                If groupDisplayName.Length > GROUPNAME_LENGTH_MAX Then
                    groupDisplayName = Left(groupDisplayName, _
                                        GROUPNAME_LENGTH_TRUNCATE) + "..."
                End If

                lblDisplayName.Text = "'" + groupDisplayName + "'"

                Dim mailNickName As String = GenerateMailNickName()

                miisGroupManagementConnectionString.Open()

                'Declarations
                Dim mailNickNameReader As SqlDataReader = Nothing
                Dim mailNickNameColumn(1) As Object
                Dim mailNickNameQuery As String = "SELECT " + _
                    "mailNickName FROM groupDefinitions " + _
                            "WHERE objectUID = '" + groupObjectUID + "'"
                Dim mailNickNameCommand As SqlCommand = _
                                New SqlCommand(mailNickNameQuery, _
                                        miisGroupManagementConnectionString)
                mailNickNameReader = mailNickNameCommand.ExecuteReader()

                While (mailNickNameReader.Read())
                    mailNickNameReader.GetValues(mailNickNameColumn)

                    If Not mailNickNameReader.IsDBNull(0) Then

                        ' Based on the value of the mailnickname, determine 
                        ' the visibility of some other controls
                        Select Case mailNickNameColumn(0)

                            Case ""
                                rdblMail.SelectedValue = "none"
                                txtCustom.Visible = False
                                txtDefault.Visible = False

                            Case mailNickName
                                rdblMail.SelectedValue = "default"
                                txtCustom.Visible = False
                                txtDefault.Visible = True
                                txtDefault.Text = mailNickNameColumn(0)

                            Case Else
                                rdblMail.SelectedValue = "custom"
                                txtDefault.Visible = False
                                txtCustom.Visible = True
                                txtCustom.Text = mailNickNameColumn(0)

                        End Select
                    End If
                End While

                If Not (mailNickNameReader Is Nothing) Then
                    mailNickNameReader.Close()
                End If
                'Close connection string before method exit
                miisGroupManagementConnectionString.Close()

            End If

        Catch exception As Exception
            lblError.Text = "Error PageLoad: " + exception.Message

        End Try

    End Sub

    ' <summary>
    ' Saves all the specifications that are being performed
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub btnSave_Click _
            (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                    Handles btnSave.Click
        Try

            ' Get the values that are being passed in on the URL.
            ' These values are used to know which group is being 
            ' worked upon
            Dim groupObjectUID As String = Request.QueryString _
                                            ("groupObjectUID").ToString()
            Dim groupDisplayName As String = Request.QueryString _
                                            ("groupDisplayName").ToString()

            Dim mailNickName As String = String.Empty

            ' For each selected value of the rdblMail, mailnickname
            ' is assigned to a variable
            Select Case rdblMail.SelectedValue

                Case "default"
                    mailNickName = txtDefault.Text

                Case "custom"
                    If txtCustom.Text = "" Then
                        Dim strMessage As String = _
                               "Custom mail alias cannot be blank."

                        'Display alert window to the user
                        ThrowAlert(strMessage)

                        Exit Sub
                    Else
                        mailNickName = txtCustom.Text
                        If (ValidateStringData(mailNickName) = False) Then
                            Dim strMessage As String = _
                                    "Invalid characters in custom alias."
                            ThrowAlert(strMessage)
                            Exit Sub
                        End If
                    End If

                Case Else
                    mailNickName = ""

            End Select

            miisGroupManagementConnectionString.Open()

            ' Escape "'" character because it is a 
            ' special character in SQL
            mailNickName = Replace(mailNickName, "'", "''")

            ' Update the groupDefinitions table to have this 
            ' group point to itself for the clause
            Dim updateMailNickName As String = "UPDATE " + _
                        "groupDefinitions SET mailNickName " + _
                            "= '" + mailNickName + "' WHERE objectUID " + _
                                "= '" + groupObjectUID + "'"
            Dim updateMailNickNameCommand As SqlCommand = _
                            New SqlCommand(updateMailNickName, _
                                    miisGroupManagementConnectionString)
            recordsAffected = updateMailNickNameCommand.ExecuteNonQuery()

            ' Frame the insert query string and then execute it
            Dim copyGroupToDeltaStatement As String = "INSERT INTO " + _
                    "groupDefinitions_delta (objectUID, groupAutoUID, " + _
                    "objectType, displayName, description, clauseLink, " + _
                    "enabledFlag, maxExcept, preserveMembers, groupType, " + _
                    "mailNickName) SELECT objectUID, groupAutoUID, " + _
                    "objectType, displayName, description, clauseLink, " + _
                    "enabledFlag, maxExcept, preserveMembers, groupType, " + _
                    "mailNickName FROM groupDefinitions WHERE objectUID " + _
                    "= '" + groupObjectUID + "'"
            'Create a sql command with the above framed query
            Dim copyGroupToDeltaCommand As SqlCommand = _
                    New SqlCommand(copyGroupToDeltaStatement, _
                                miisGroupManagementConnectionString)
            'Execute query
            Dim rowsAffected As Integer = _
                                copyGroupToDeltaCommand.ExecuteNonQuery()
            'Check for error
            If rowsAffected = 0 Then
                Throw New Exception("There was an error adding the " _
                          + groupObjectUID + " group to the delta table.")
            End If

            'Frame query to update groupDefinition_delta table and execute it
            Dim updateGroupInDeltaStatement As String = "UPDATE " + _
                "groupDefinitions_delta SET attributeName = " + _
                    "'mailNickName', changeTime = '" + Date.Now + "', " + _
                    "changeType = 'Modify_Attribute' WHERE objectUID " + _
                    "= '" + groupObjectUID + "'  AND changeType IS NULL"
            Dim updateGroupInDeltaCommand As SqlCommand = _
                    New SqlCommand(updateGroupInDeltaStatement, _
                                miisGroupManagementConnectionString)
            rowsAffected = updateGroupInDeltaCommand.ExecuteNonQuery()

            'Check for any error in execution
            If rowsAffected = 0 Then
                Throw New Exception("There was an error updating the " _
                    + groupObjectUID + " group after it was added to " + _
                            "the delta table.")
            End If
            'Close database connection string
            miisGroupManagementConnectionString.Close()

            ' Log the resultant into the event log
            EventLog.WriteEntry("miisGroupManagement", _
                Environment.UserDomainName.ToLower + "\" + _
                    Environment.UserName.ToLower + " updated the " + _
                    "follwing for group '" + groupDisplayName + "' with " + _
                    "object UID '" + groupObjectUID + "':" + vbCrLf + _
                    vbCrLf + "mailNickName - '" + mailNickName + "'", _
                    EventLogEntryType.Information)

            ' Close the window
            Dim strCloseScript As String = "<script>self.close();"
            strCloseScript += "</script>"
            RegisterClientScriptBlock("key3", strCloseScript)

        Catch exception As Exception
            HideControls()
            lblError.Text = "Error btnSave_Click: " + exception.Message

            'Ensure that the database connection object is closed before exit
        Finally
            If Not miisGroupManagementConnectionString Is Nothing Then
                If Not (miisGroupManagementConnectionString.State = ConnectionState.Closed) Then
                    miisGroupManagementConnectionString.Close()
                End If
            End If
        End Try
    End Sub

    ' <summary>
    ' Close the current window and go back to the parent window
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub btnCancel_Click _
        (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                              Handles btnCancel.Click
        Try

            ' Closes the buildClause window
            Dim strCloseScript As String = "<script>self.close();</script>"
            RegisterClientScriptBlock("key1", strCloseScript)

        Catch exception As Exception
            HideControls()
            lblError.Text = _
                "Error btnSelectCancel_Click: " + exception.Message
        End Try
    End Sub

    ' <summary>
    ' Based on the selected radio button from the radio button 
    ' list,corresponding controls are made visible</summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub rdblMail_SelectedIndexChanged _
                (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles rdblMail.SelectedIndexChanged
        Select Case rdblMail.SelectedValue
            'radio button selection none
        Case "none"
                txtCustom.Visible = False
                txtDefault.Visible = False
                'radio button selection default
            Case "default"
                txtCustom.Visible = False
                txtDefault.Visible = True
                Dim mailNickName As String = GenerateMailNickName()
                txtDefault.Text = mailNickName
                'radio button selection custom
            Case "custom"
                txtDefault.Visible = False
                txtCustom.Visible = True

        End Select
    End Sub

    ' <summary>
    ' Generates a mail nick name, by replacing all the 
    ' unaccepted characters
    ' </summary>
    ' <returns></returns> 

    Function GenerateMailNickName()

        Try

            ' Get the values that are being passed in on the URL.
            ' These values are used to know which group is being 
            ' worked upon
            Dim groupDisplayName As String = Request.QueryString _
                                            ("groupDisplayName").ToString()

            ' Strip out characters of the groupDisplayName that are not 
            ' allowed on groups and replace with an '_'
            groupDisplayName = Replace(groupDisplayName, "/", "_")
            groupDisplayName = Replace(groupDisplayName, "\", "_")
            groupDisplayName = Replace(groupDisplayName, "[", "_")
            groupDisplayName = Replace(groupDisplayName, "]", "_")
            groupDisplayName = Replace(groupDisplayName, ":", "_")
            groupDisplayName = Replace(groupDisplayName, ";", "_")
            groupDisplayName = Replace(groupDisplayName, "|", "_")
            groupDisplayName = Replace(groupDisplayName, "=", "_")
            groupDisplayName = Replace(groupDisplayName, ",", "_")
            groupDisplayName = Replace(groupDisplayName, "+", "_")
            groupDisplayName = Replace(groupDisplayName, "*", "_")
            groupDisplayName = Replace(groupDisplayName, "?", "_")
            groupDisplayName = Replace(groupDisplayName, "<", "_")
            groupDisplayName = Replace(groupDisplayName, ">", "_")

            ' Strip out characters of the groupDisplayName to set 
            ' the default format for the mailNickName
            groupDisplayName = Replace(groupDisplayName, " ", "")
            groupDisplayName = Replace(groupDisplayName, "-", "")

            Dim mailNickName As String = groupDisplayName

            'Return generated mail nick name
            Return mailNickName

        Catch exception As Exception
            lblError.Text = "Error in Generating Nick Name: " + _
                                    exception.Message

        End Try

    End Function

    ' <summary>
    ' Hides specific controls upon any exception or error.
    ' </summary>

    Private Sub HideControls()
        txtCustom.Visible = False
        txtDefault.Visible = False
        pnlMailStatus.Visible = False
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
    ' <summary>
    ' Validate the input data for empty string and 
    ' invalid characters. 
    ' </summary>
    ' <param name="input"></param>		
    ' <returns>Returns true if the string is valid</returns>
    Private Function ValidateStringData(ByVal input As String) As Boolean

        'Empty string is considered to be invalid

        If (input.Trim() = "") Then
            Return False
        Else

            ' This string shows the set of invalid characters and is used
            ' when validating the string inputs on the webpages. The goal
            ' is to have a simple defense implementation against SQL 
            ' injection.

            Dim InvalidChars As String = "~!@#$%^&*()<>-=_+,.?/\\:;\`[]|"""

            ' Return true or false based on the validation success
            If (input.IndexOfAny(InvalidChars.ToCharArray()) > -1) Then

                Return False

            Else
                Return True
            End If
        End If
    End Function
End Class