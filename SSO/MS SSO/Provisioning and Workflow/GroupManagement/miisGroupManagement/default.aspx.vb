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

' <summary>
' Represents the Default.aspx page, to accomodate all the code behind
' methods involved to perform various fuinctionalities at the start page
' </summary>
Public Class DefaultClass
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.sqldaGroupDefinitions = New System.Data.SqlClient.SqlDataAdapter
        Me.SqlDeleteCommand1 = New System.Data.SqlClient.SqlCommand
        Me.miisGroupManagementConnectionString = New System.Data.SqlClient.SqlConnection
        Me.SqlInsertCommand1 = New System.Data.SqlClient.SqlCommand
        Me.SqlSelectCommand1 = New System.Data.SqlClient.SqlCommand
        Me.SqlUpdateCommand1 = New System.Data.SqlClient.SqlCommand
        Me.dsGroupDefinitions1 = New gp.dsGroupDefinitions
        CType(Me.dsGroupDefinitions1, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'sqldaGroupDefinitions
        '
        Me.sqldaGroupDefinitions.DeleteCommand = Me.SqlDeleteCommand1
        Me.sqldaGroupDefinitions.InsertCommand = Me.SqlInsertCommand1
        Me.sqldaGroupDefinitions.SelectCommand = Me.SqlSelectCommand1
        Me.sqldaGroupDefinitions.TableMappings.AddRange(New System.Data.Common.DataTableMapping() {New System.Data.Common.DataTableMapping("Table", "groupDefinitions", New System.Data.Common.DataColumnMapping() {New System.Data.Common.DataColumnMapping("objectUID", "objectUID"), New System.Data.Common.DataColumnMapping("objectType", "objectType"), New System.Data.Common.DataColumnMapping("displayName", "displayName"), New System.Data.Common.DataColumnMapping("clauseLink", "clauseLink"), New System.Data.Common.DataColumnMapping("enabledFlag", "enabledFlag"), New System.Data.Common.DataColumnMapping("maxExcept", "maxExcept"), New System.Data.Common.DataColumnMapping("groupType", "groupType"), New System.Data.Common.DataColumnMapping("description", "description"), New System.Data.Common.DataColumnMapping("mailNickName", "mailNickName"), New System.Data.Common.DataColumnMapping("preserveMembers", "preserveMembers")})})
        Me.sqldaGroupDefinitions.UpdateCommand = Me.SqlUpdateCommand1
        '
        'SqlDeleteCommand1
        '
        Me.SqlDeleteCommand1.CommandText = "DELETE FROM groupDefinitions WHERE (objectUID = @Original_objectUID) AND (clauseL" & _
        "ink = @Original_clauseLink OR @Original_clauseLink IS NULL AND clauseLink IS NUL" & _
        "L) AND (description = @Original_description OR @Original_description IS NULL AND" & _
        " description IS NULL) AND (displayName = @Original_displayName OR @Original_disp" & _
        "layName IS NULL AND displayName IS NULL) AND (enabledFlag = @Original_enabledFla" & _
        "g OR @Original_enabledFlag IS NULL AND enabledFlag IS NULL) AND (groupType = @Or" & _
        "iginal_groupType OR @Original_groupType IS NULL AND groupType IS NULL) AND (mail" & _
        "NickName = @Original_mailNickName OR @Original_mailNickName IS NULL AND mailNick" & _
        "Name IS NULL) AND (maxExcept = @Original_maxExcept OR @Original_maxExcept IS NUL" & _
        "L AND maxExcept IS NULL) AND (objectType = @Original_objectType OR @Original_obj" & _
        "ectType IS NULL AND objectType IS NULL) AND (preserveMembers = @Original_preserv" & _
        "eMembers OR @Original_preserveMembers IS NULL AND preserveMembers IS NULL)"
        Me.SqlDeleteCommand1.Connection = Me.miisGroupManagementConnectionString
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_objectUID", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "objectUID", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_clauseLink", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "clauseLink", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_description", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "description", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_displayName", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "displayName", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_enabledFlag", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "enabledFlag", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_groupType", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "groupType", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_mailNickName", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "mailNickName", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_maxExcept", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "maxExcept", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_objectType", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "objectType", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_preserveMembers", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "preserveMembers", System.Data.DataRowVersion.Original, Nothing))
        '
        'miisGroupManagementConnectionString
        '
        Me.miisGroupManagementConnectionString.ConnectionString = "workstation id=""localhost"";packet size=4096;integrated security=SSPI;data source=" & _
        "localhost;persist security info=False;initial catalog=miisGroupManagement"
        '
        'SqlInsertCommand1
        '
        Me.SqlInsertCommand1.CommandText = "INSERT INTO groupDefinitions(objectUID, objectType, displayName, clauseLink, enab" & _
        "ledFlag, maxExcept, groupType, description, mailNickName, preserveMembers) VALUE" & _
        "S (@objectUID, @objectType, @displayName, @clauseLink, @enabledFlag, @maxExcept," & _
        " @groupType, @description, @mailNickName, @preserveMembers); SELECT objectUID, o" & _
        "bjectType, displayName, clauseLink, enabledFlag, maxExcept, groupType, descripti" & _
        "on, mailNickName, preserveMembers FROM groupDefinitions WHERE (objectUID = @obje" & _
        "ctUID)"
        Me.SqlInsertCommand1.Connection = Me.miisGroupManagementConnectionString
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@objectUID", System.Data.SqlDbType.VarChar, 256, "objectUID"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@objectType", System.Data.SqlDbType.VarChar, 64, "objectType"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@displayName", System.Data.SqlDbType.VarChar, 256, "displayName"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@clauseLink", System.Data.SqlDbType.VarChar, 64, "clauseLink"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@enabledFlag", System.Data.SqlDbType.VarChar, 64, "enabledFlag"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@maxExcept", System.Data.SqlDbType.VarChar, 64, "maxExcept"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@groupType", System.Data.SqlDbType.Int, 4, "groupType"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@description", System.Data.SqlDbType.VarChar, 256, "description"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@mailNickName", System.Data.SqlDbType.VarChar, 256, "mailNickName"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@preserveMembers", System.Data.SqlDbType.Int, 4, "preserveMembers"))
        '
        'SqlSelectCommand1
        '
        Me.SqlSelectCommand1.CommandText = "SELECT objectUID, objectType, displayName, clauseLink, enabledFlag, maxExcept, gr" & _
        "oupType, description, mailNickName, preserveMembers FROM groupDefinitions"
        Me.SqlSelectCommand1.Connection = Me.miisGroupManagementConnectionString
        '
        'SqlUpdateCommand1
        '
        Me.SqlUpdateCommand1.CommandText = "UPDATE groupDefinitions SET objectUID = @objectUID, objectType = @objectType, dis" & _
        "playName = @displayName, clauseLink = @clauseLink, enabledFlag = @enabledFlag, m" & _
        "axExcept = @maxExcept, groupType = @groupType, description = @description, mailN" & _
        "ickName = @mailNickName, preserveMembers = @preserveMembers WHERE (objectUID = @" & _
        "Original_objectUID) AND (clauseLink = @Original_clauseLink OR @Original_clauseLi" & _
        "nk IS NULL AND clauseLink IS NULL) AND (description = @Original_description OR @" & _
        "Original_description IS NULL AND description IS NULL) AND (displayName = @Origin" & _
        "al_displayName OR @Original_displayName IS NULL AND displayName IS NULL) AND (en" & _
        "abledFlag = @Original_enabledFlag OR @Original_enabledFlag IS NULL AND enabledFl" & _
        "ag IS NULL) AND (groupType = @Original_groupType OR @Original_groupType IS NULL " & _
        "AND groupType IS NULL) AND (mailNickName = @Original_mailNickName OR @Original_m" & _
        "ailNickName IS NULL AND mailNickName IS NULL) AND (maxExcept = @Original_maxExce" & _
        "pt OR @Original_maxExcept IS NULL AND maxExcept IS NULL) AND (objectType = @Orig" & _
        "inal_objectType OR @Original_objectType IS NULL AND objectType IS NULL) AND (pre" & _
        "serveMembers = @Original_preserveMembers OR @Original_preserveMembers IS NULL AN" & _
        "D preserveMembers IS NULL); SELECT objectUID, objectType, displayName, clauseLin" & _
        "k, enabledFlag, maxExcept, groupType, description, mailNickName, preserveMembers" & _
        " FROM groupDefinitions WHERE (objectUID = @objectUID)"
        Me.SqlUpdateCommand1.Connection = Me.miisGroupManagementConnectionString
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@objectUID", System.Data.SqlDbType.VarChar, 256, "objectUID"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@objectType", System.Data.SqlDbType.VarChar, 64, "objectType"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@displayName", System.Data.SqlDbType.VarChar, 256, "displayName"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@clauseLink", System.Data.SqlDbType.VarChar, 64, "clauseLink"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@enabledFlag", System.Data.SqlDbType.VarChar, 64, "enabledFlag"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@maxExcept", System.Data.SqlDbType.VarChar, 64, "maxExcept"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@groupType", System.Data.SqlDbType.Int, 4, "groupType"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@description", System.Data.SqlDbType.VarChar, 256, "description"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@mailNickName", System.Data.SqlDbType.VarChar, 256, "mailNickName"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@preserveMembers", System.Data.SqlDbType.Int, 4, "preserveMembers"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_objectUID", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "objectUID", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_clauseLink", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "clauseLink", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_description", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "description", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_displayName", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "displayName", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_enabledFlag", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "enabledFlag", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_groupType", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "groupType", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_mailNickName", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "mailNickName", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_maxExcept", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "maxExcept", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_objectType", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "objectType", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_preserveMembers", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "preserveMembers", System.Data.DataRowVersion.Original, Nothing))
        '
        'dsGroupDefinitions1
        '
        Me.dsGroupDefinitions1.DataSetName = "dsGroupDefinitions"
        Me.dsGroupDefinitions1.Locale = New System.Globalization.CultureInfo("en-US")
        CType(Me.dsGroupDefinitions1, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Protected WithEvents sqldaGroupDefinitions As System.Data.SqlClient.SqlDataAdapter
    Protected WithEvents dgGroupDefinitions As System.Web.UI.WebControls.DataGrid
    Protected WithEvents dsGroupDefinitions1 As gp.dsGroupDefinitions
    Protected WithEvents btnAddRow As System.Web.UI.WebControls.Button
    Protected WithEvents btnSrchGroups As System.Web.UI.WebControls.Button
    Protected WithEvents logo As System.Web.UI.WebControls.Image
    Protected WithEvents frmDefault As System.Web.UI.HtmlControls.HtmlForm
    Protected WithEvents lblError As System.Web.UI.WebControls.Label
    Protected WithEvents miisGroupManagementConnectionString As System.Data.SqlClient.SqlConnection
    Protected WithEvents Label3 As System.Web.UI.WebControls.Label
    Protected WithEvents cbIncludeGroupAuto As System.Web.UI.WebControls.CheckBox
    Protected WithEvents SqlSelectCommand1 As System.Data.SqlClient.SqlCommand
    Protected WithEvents SqlInsertCommand1 As System.Data.SqlClient.SqlCommand
    Protected WithEvents SqlUpdateCommand1 As System.Data.SqlClient.SqlCommand
    Protected WithEvents SqlDeleteCommand1 As System.Data.SqlClient.SqlCommand
    Protected WithEvents btnDefineAttributeDefs As System.Web.UI.WebControls.Button
    Protected WithEvents lblSearch As System.Web.UI.WebControls.Label
    Protected WithEvents txtSearchValue As System.Web.UI.WebControls.TextBox
    Protected WithEvents ddlSearchAttribute As System.Web.UI.WebControls.DropDownList
    Protected WithEvents ddlSearchOperation As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lblPageTitle As System.Web.UI.WebControls.Label

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    ' Group types in Active Directory
    Const ADS_GROUP_TYPE_GLOBAL_GROUP = &H2
    Const ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP = &H4
    Const ADS_GROUP_TYPE_UNIVERSAL_GROUP = &H8
    Const ADS_GROUP_TYPE_SECURITY_ENABLED = &H80000000

    'Constants for message exceptions
    Const MSG_NULL_REFERENCE = "Object reference not set to an instance of an object."
    Const MSG_TIMEOUT = "Your session has timed out or there is an object" + _
                        " reference error, please reconnect."

    Const GROUPNAME_LENGTH_MAX = 25
    Const GROUPNAME_LENGTH_TRUNCATE = 22

    'Constants for group types
    Const SEC_GROUP_UNIV = "Sec Group - Univ"
    Const DIST_GROUP_UNIV = "Dist Group - Univ"
    Const SEC_GROUP_GLOBAL = "Sec Group - Global"
    Const DIST_GROUP_GLOBAL = "Dist Group - Global"
    Const SEC_GROUP_DOMLOCAL = "Sec Group - DomLocal"
    Const DIST_GROUP_DOMLOCAL = "Dist Group - DomLocal"
    ' Declarations
    Dim recordsAffected As Integer = 0

    ' <summary>
    ' Gets executed at the page load, initially and 
    ' during post backs as well</summary>
    ' <param name="sender">Sender object</param>
	' <param name="e">Event Arguments</param>    
    ' <returns></returns> 
    
    Private Sub Page_Load(ByVal sender As System.Object, _
                        ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            ' Sets the connection string from the xml file 
            ' that is defined in the global.asax file
            Me.miisGroupManagementConnectionString.ConnectionString _
                              = ConfigurationSettings.AppSettings _
                                    ("miisGroupManagementConnectionString")

            ' This allows the group search to be submitted 
            ' when the enter button is pressed
            Page.RegisterHiddenField("__EVENTTARGET", "btnSrchGroups")

            ' This value is changed from the system generated
            ' update(Command) so that the where clause includes 
            ' only the primary key (objectUID).Also the clause 
            ' is not set because it is handled outside the datagrid
            Me.SqlUpdateCommand1.CommandText = "UPDATE groupDefinitions " + _
                "SET objectUID = @objectUID, objectType = @objectType, " + _
                "displayName = @displayName, enabledFlag = @enabledFlag, " + _
                "maxExcept = @maxExcept, groupType = @groupType, " + _
                "description = @description WHERE " + _
                "(objectUID = @" & "Original_objectUID); " + _
                "SELECT objectUID, objectType, displayName, clauseLink, " + _
                "enabledFlag, maxExcept, groupType, description, " + _
                "mailNickName, preserveMembers" & _
                " FROM groupDefinitions WHERE (objectUID = @objectUID)"

            If IsPostBack Then

                ' Used to cache the datagrid
                dsGroupDefinitions1 = CType(Session("DsGroups"), _
                                                    dsGroupDefinitions)
            Else
                dgGroupDefinitions.DataBind()
                Session("DsGroups") = dsGroupDefinitions1
            End If

        Catch exception As Exception
            HideControls()
            If exception.Message = MSG_NULL_REFERENCE Then
                ' Most likely cause for the error is that the 
                ' SQL connection has timed out
                lblError.Text = "Page_Load:" + MSG_TIMEOUT
            Else
                lblError.Text = "Error Page_Load: " + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Puts the datagrid into edit mode for the respective row selected
    ' </summary>
    ' <param name="source">Source object</param>
	' <param name="e">Event Arguments</param>    
    ' <returns></returns> 
    
    Private Sub dgGroupDefinitions_EditCommand(ByVal source As Object, _
        ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
            Handles dgGroupDefinitions.EditCommand

        Try

            ' Get the imageurl that represents if this autoGroup or just a group
            Dim autoImage As System.Web.UI.WebControls.Image = CType(e.Item. _
                    FindControl("autoImage"), System.Web.UI.WebControls.Image)

            ' If an update is trying to happen to a group that was automaticaly 
            ' generated by an attribute definition, disallow.
            If autoImage.ImageUrl.ToLower.EndsWith("groupauto.gif") Then
                Dim strMessage As String = "This group definition was automatically" + _
                " generated based on an attribute based group definition.  Editting " + _
                "this group definition is not allowed because these groups are " + _
                "deleted and re-added when the groupPopulator.exe is run with the" + _
                " /r switch."
                Dim strScript As String = "<script language=JavaScript>"
                strScript += "alert(""" & strMessage & """);"
                strScript += Chr(60) & "/script>"

                If Not (Page.IsStartupScriptRegistered("clientScript")) Then
                    Page.RegisterStartupScript("alertScript", strScript)
                End If
                Exit Sub
            Else

                ' Unhide the following columns
                ' Column[3] - description
                dgGroupDefinitions.Columns(3).Visible = True

                ' Hide the following columns
                ' Column[11] - delete
                dgGroupDefinitions.Columns(11).Visible = False

                ' Put the datagrid into edit mode for the 
                ' row in which the edit button was clicked
                dgGroupDefinitions.EditItemIndex = e.Item.ItemIndex
                dgGroupDefinitions.DataBind()

            End If

        Catch exception As Exception
            HideControls()
            'Check for null reference message to display cistomized error
            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "dgGroupDefinitions_EditCommand: " + MSG_TIMEOUT
            Else
                lblError.Text = "Error dgGroupDefinitions_EditCommand: " _
                                    + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Updates the datagrid and removes the datagrid from edit 
    ' mode when update is clicked
    ' </summary>
    ' <param name="source">Source object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub dgGroupDefinitions_UpdateCommand(ByVal source As Object, _
        ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
                                   Handles dgGroupDefinitions.UpdateCommand

        Try

            Dim objectUID As String = String.Empty
            Dim displayName As String = String.Empty
            Dim description As String = String.Empty
            Dim enabledFlag As String = String.Empty
            Dim maxExcept As String = String.Empty
            Dim groupType As String = String.Empty

            ' Gets the value of the key field of the row being updated
            Dim key As String = dgGroupDefinitions.DataKeys _
                                            (e.Item.ItemIndex).ToString()

            'Data Grid column references

            ' Column[0] - objectType
            ' Column[1] - objectUID
            ' Column[2] - displayName
            ' Column[3] - description
            ' Column[4] - groupType
            ' Column[5] - enabledFlag
            ' Column[6] - mail
            ' Column[7] - clauseButton
            ' Column[8] - exceptionsButton
            ' Column[9] - maxExcept
            ' Column[10] - edit/update/cancel
            ' Column[11] - delete

            Dim textBox As TextBox = Nothing
            Dim dropDownList As DropDownList = Nothing

            ' set to the objectUID
            objectUID = e.Item.Cells(1).Text.ToString

            ' set to the group displayName
            textBox = CType(e.Item.Cells(2).Controls(0), TextBox)
            displayName = textBox.Text

            ' Strip out characters of the displayname that are 
            ' not allowed on groups and replace with an '_'
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
            displayName = Replace(displayName, "@", "_")

            ' If an update is trying to happen without a group name being set
            ' create a popup message that alerts to the condition
            If displayName = Nothing Then
                Dim strMessage As String = "Group Name cannot be blank."

                'Display alert window to the user
                ThrowAlert(strMessage)

                Exit Sub

                ' if group name is populated
            Else
                miisGroupManagementConnectionString.Open()

                ' select rows from the groupDefinitions table that
                ' has the same name and if returned then alert 
                ' that the name is not unique in the table
                Dim displayNameForSQL As String = Replace _
                                            (displayName, "'", "''")
                Dim displayNameReader As SqlDataReader = Nothing
                Dim sqlDisplayNameColumn(2) As Object
                Dim displayNameQuery As String = "SELECT objectUID, " + _
                    "displayName FROM groupDefinitions WHERE " + _
                            "displayName = '" + displayNameForSQL + _
                                 "' and objectUID <> '" + objectUID + "'"
                Dim displayNameCommand As SqlCommand = New SqlCommand _
                     (displayNameQuery, miisGroupManagementConnectionString)
                displayNameReader = displayNameCommand.ExecuteReader()

                While (displayNameReader.Read())
                    displayNameReader.GetValues(sqlDisplayNameColumn)

                    If Not displayNameReader.IsDBNull(1) AndAlso Not _
                                    sqlDisplayNameColumn(0).Equals("") Then

                        ' if the group name already exists in the table
                        ' create a popup message that alerts to the condition
                        Dim strMessage As String = "Group Name already exists."

                        ThrowAlert(strMessage)

                        If Not (displayNameReader Is Nothing) Then
                            displayNameReader.Close()
                        End If

                        Exit Sub

                    End If
                End While

                If Not (displayNameReader Is Nothing) Then
                    displayNameReader.Close()
                End If
                'Close the sql connection
                miisGroupManagementConnectionString.Close()

            End If

            ' set to the description (description)
            textBox = _
                CType(e.Item.FindControl("descriptionLabelEdit"), TextBox)
            description = textBox.Text
            'validate the field description to avoid sql injection
            If Not description = "" AndAlso (ValidateStringData(description) = False) Then
                Dim strMessage As String = "Invalid characters in the description."
                strMessage += "The description cannnot contain any of the following characters. "
                strMessage += "~!@#$%^&*()<>=_+./\:;\`[]|"
                ThrowAlert(strMessage)
                Exit Sub
            End If

            ' set to the groupType (groupType)
            dropDownList = _
                CType(e.Item.FindControl("groupTypeDropDownList"), _
                                DropDownList)

            groupType = dropDownList.SelectedValue

            Dim groupTypeNum As String = String.Empty

            ' Each group type is created by setting the groupType 
            ' attritute to the correct value as follows
            Select Case groupType.ToLower
                Case SEC_GROUP_UNIV.ToLower
                    groupTypeNum = ADS_GROUP_TYPE_UNIVERSAL_GROUP Or _
                                            ADS_GROUP_TYPE_SECURITY_ENABLED
                Case DIST_GROUP_UNIV.ToLower
                    groupTypeNum = ADS_GROUP_TYPE_UNIVERSAL_GROUP
                Case SEC_GROUP_GLOBAL.ToLower
                    groupTypeNum = ADS_GROUP_TYPE_GLOBAL_GROUP Or _
                                            ADS_GROUP_TYPE_SECURITY_ENABLED
                Case DIST_GROUP_GLOBAL.ToLower
                    groupTypeNum = ADS_GROUP_TYPE_GLOBAL_GROUP
                Case SEC_GROUP_DOMLOCAL.ToLower
                    groupTypeNum = ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP Or _
                                            ADS_GROUP_TYPE_SECURITY_ENABLED
                Case DIST_GROUP_DOMLOCAL.ToLower
                    groupTypeNum = ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP
                Case Else
                    groupTypeNum = ADS_GROUP_TYPE_UNIVERSAL_GROUP Or _
                                            ADS_GROUP_TYPE_SECURITY_ENABLED
            End Select

            Dim groupDefRow As dsGroupDefinitions.groupDefinitionsRow
            groupDefRow = _
                dsGroupDefinitions1.groupDefinitions.FindByobjectUID(key)

            ' Updates the dataset table
            groupDefRow.displayName = displayName
            groupDefRow.description = description
            groupDefRow.groupType = groupTypeNum

            ' Assign to enabledFlag
            Dim enabledImageEdit As ImageButton = CType _
                    (e.Item.FindControl("enabledImageEdit"), ImageButton)
            If enabledImageEdit.ImageUrl = "./images/buttonCheck.gif" Then
                enabledFlag = "enabled"
            Else
                enabledFlag = "disabled"
            End If

            groupDefRow.enabledFlag = enabledFlag

            ' Calls a SQL statement to update the database from the dataset
            sqldaGroupDefinitions.Update(dsGroupDefinitions1)

            EventLog.WriteEntry("miisGroupManagement", Environment. _
                UserDomainName.ToLower + "\" + Environment.UserName.ToLower _
                + " updated the following properties for group '" + _
                displayName + "' with object UID '" + objectUID + _
                "':" + vbCrLf + vbCrLf + "Description - '" + description + _
                "'" + vbCrLf + "enabledFlag - '" + enabledFlag + "'" + _
                vbCrLf + "groupType - '" + groupType + "'", _
                EventLogEntryType.Information)

            miisGroupManagementConnectionString.Open()

            ' Load the queries to be executed into string variables
            Dim copyGroupToDeltaStatement As String = _
                "INSERT INTO groupDefinitions_delta (objectUID, " + _
                "groupAutoUID, objectType, displayName, description, " + _
                "clauseLink, enabledFlag, maxExcept, preserveMembers, " + _
                "groupType, mailNickName) SELECT objectUID, groupAutoUID, " + _
                "objectType, displayName, description, clauseLink, " + _
                "enabledFlag, maxExcept, preserveMembers, groupType, " + _
                "mailNickName FROM groupDefinitions WHERE objectUID " + _
                "= '" + objectUID + "'"
            Dim copyGroupToDeltaCommand As SqlCommand = _
                    New SqlCommand(copyGroupToDeltaStatement, _
                            miisGroupManagementConnectionString)
            Dim rowsAffected As Integer = copyGroupToDeltaCommand. _
                                                        ExecuteNonQuery()

            If rowsAffected = 0 Then
                Throw New Exception("There was an error adding the " _
                                + objectUID + " group to the delta table.")
            End If

            'Frame the query to update delta table 'groupDefiniftions_delta'
            Dim updateGroupInDeltaStatement As String = _
                "UPDATE groupDefinitions_delta SET changeTime = " + _
                "'" + Date.Now + "', changeType = 'Modify' WHERE " + _
                "objectUID = '" + objectUID + "'  and changeType is NULL"
            Dim updateGroupInDeltaCommand As SqlCommand = _
                New SqlCommand(updateGroupInDeltaStatement, _
                            miisGroupManagementConnectionString)
            rowsAffected = updateGroupInDeltaCommand.ExecuteNonQuery()

            If rowsAffected = 0 Then
                Throw New Exception("There was an error updating the " _
                            + objectUID + " group after it was added " + _
                                                      "to the delta table.")
            End If

            miisGroupManagementConnectionString.Close()

            ' Removes the DataGrid from edit mode
            dgGroupDefinitions.EditItemIndex = -1

            ' changes the state of the columns back to non-editable mode
            dgGroupDefinitions.Columns(3).Visible = False
            dgGroupDefinitions.Columns(11).Visible = True

            ' Refreshes the grid
            dgGroupDefinitions.DataBind()

        Catch exception As Exception
            HideControls()
            'Check for null reference message to display customized error
            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "dgGroupDefinitions_UpdateCommand: " + MSG_TIMEOUT
            Else
                lblError.Text = "Error dgGroupDefinitions_UpdateCommand: " _
                                                    + exception.Message
            End If
            'Ensure that the database connection is closed before method exit
        Finally
            If Not miisGroupManagementConnectionString Is Nothing Then
                If Not (miisGroupManagementConnectionString.State = ConnectionState.Closed) Then
                    miisGroupManagementConnectionString.Close()
                End If
            End If
        End Try

    End Sub

    ' <summary>
    ' Removes the datagrid from edit mode without saving changes
    ' </summary>
    ' <param name="source">Source object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub dgGroupDefinitions_CancelCommand(ByVal source As Object, _
            ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
                                    Handles dgGroupDefinitions.CancelCommand

        Try

            miisGroupManagementConnectionString.Open()

            ' select row from the groupDefinitions table that
            ' has the same objectUID and check to ensure that 
            ' the displayName is set to something otherwise
            ' delete the row
            Dim displayNameReader As SqlDataReader = Nothing
            Dim sqlDisplayNameColumn(2) As Object
            Dim displayNameQuery As String = "SELECT objectUID, " + _
                "displayName FROM groupDefinitions WHERE " + _
                "objectUID = '" + e.Item.Cells(1).Text.ToString + "'"
            Dim displayNameCommand As SqlCommand = New SqlCommand _
                 (displayNameQuery, miisGroupManagementConnectionString)
            displayNameReader = displayNameCommand.ExecuteReader()

            While (displayNameReader.Read())
                displayNameReader.GetValues(sqlDisplayNameColumn)

                If displayNameReader.IsDBNull(1) Or _
                                sqlDisplayNameColumn(1).Equals("") Then

                    If Not (displayNameReader Is Nothing) Then
                        displayNameReader.Close()
                    End If

                    'Close the sql connection
                    miisGroupManagementConnectionString.Close()

                    dgGroupDefinitions_DeleteCommand(source, e)

                    Exit While

                End If
            End While

            If Not (displayNameReader Is Nothing) Then
                displayNameReader.Close()
            End If

            'Close the sql connection
            miisGroupManagementConnectionString.Close()

            ' Enable and disable columns in the datagrid based on the mode 
            dgGroupDefinitions.Columns(3).Visible = False
            dgGroupDefinitions.Columns(11).Visible = True

            ' Removes the particular DataGrid row from edit mode
            dgGroupDefinitions.EditItemIndex = -1
            dgGroupDefinitions.DataBind()

        Catch exception As Exception
            HideControls()

            ''Check for null reference message to display customized error
            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "dgGroupDefinitions_CancelCommand: " + _
                                        MSG_TIMEOUT
            Else
                lblError.Text = "Error dgGroupDefinitions_CancelCommand: " _
                                      + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Controls miscellaneous behaviors such as the popup when the delete button
    ' is pressed and selecting the value of the dropdown in edit mode
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub dgGroupDefinitions_ItemDataBound(ByVal sender As Object, _
            ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) _
                                    Handles dgGroupDefinitions.ItemDataBound

        Try
            Dim btn As Button = Nothing

            ' Trigger the confirm delete popup when the delete
            ' button is clicked
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = _
                                            ListItemType.AlternatingItem Then
                btn = CType(e.Item.Cells(11).FindControl("btnDelete"), Button)
                btn.Attributes.Add("onclick", "return confirm_delete();")
            End If

            ' Change the value of the grouptype dropdownlist to 
            ' show the currently set value
            If e.Item.ItemType = ListItemType.EditItem Then
                Dim drvGroupType As DataRowView = CType(e.Item.DataItem, _
                                                                DataRowView)
                Dim currentGroupTypeNum As String = CType(drvGroupType _
                                                        ("groupType"), String)
                Dim currentGroupType As String = String.Empty

                ' For each type of the group in AD, group types are assigned
                Select Case currentGroupTypeNum
                    Case ADS_GROUP_TYPE_UNIVERSAL_GROUP Or _
                                            ADS_GROUP_TYPE_SECURITY_ENABLED
                        currentGroupType = SEC_GROUP_UNIV
                    Case ADS_GROUP_TYPE_UNIVERSAL_GROUP
                        currentGroupType = DIST_GROUP_UNIV
                    Case ADS_GROUP_TYPE_GLOBAL_GROUP Or _
                                            ADS_GROUP_TYPE_SECURITY_ENABLED
                        currentGroupType = SEC_GROUP_GLOBAL
                    Case ADS_GROUP_TYPE_GLOBAL_GROUP
                        currentGroupType = DIST_GROUP_GLOBAL
                    Case ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP Or _
                                            ADS_GROUP_TYPE_SECURITY_ENABLED
                        currentGroupType = SEC_GROUP_DOMLOCAL
                    Case ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP
                        currentGroupType = DIST_GROUP_DOMLOCAL

                    Case Else
                        Throw New Exception _
                                    ("The selected groupType was invalid.")
                End Select

                ' Assign the current group type as the selected index 
                ' of the dropdownlist 
                Dim ddlGroupType As DropDownList = Nothing
                ddlGroupType = CType(e.Item.FindControl _
                                    ("groupTypeDropDownList"), DropDownList)
                ddlGroupType.SelectedIndex = ddlGroupType.Items. _
                       IndexOf(ddlGroupType.Items.FindByText(currentGroupType))
            End If

        Catch exception As Exception
            HideControls()
            'Check for null reference message to display customized error
            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "dgGroupDefinitions_ItemDataBound: " + _
                                                            MSG_TIMEOUT
            Else
                lblError.Text = "Error dgGroupDefinitions_ItemDataBound: " _
                                                            + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Deletes the row from the datagrid and the data base and all associated
    ' records from the other SQL tables such as the clause and exceptions
    ' </summary>
    ' <param name="source">Source object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub dgGroupDefinitions_DeleteCommand(ByVal source As Object, _
            ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
                                  Handles dgGroupDefinitions.DeleteCommand

        Try

            Me.SqlDeleteCommand1.CommandText = "DELETE FROM groupDefinitions WHERE (objectUID = @Original_objectUID) AND (clauseL" & _
            "ink = @Original_clauseLink OR @Original_clauseLink IS NULL AND clauseLink IS NUL" & _
            "L) AND (description = @Original_description OR @Original_description IS NULL AND" & _
            " description IS NULL) AND (displayName = @Original_displayName OR @Original_disp" & _
            "layName IS NULL AND displayName IS NULL) AND (enabledFlag = @Original_enabledFla" & _
            "g OR @Original_enabledFlag IS NULL AND enabledFlag IS NULL) AND (groupType = @Or" & _
            "iginal_groupType OR @Original_groupType IS NULL AND groupType IS NULL) AND" & _
            " (maxExcept = @Original_maxExcept OR @Original_maxExcept IS NUL" & _
            "L AND maxExcept IS NULL) AND (objectType = @Original_objectType OR @Original_obj" & _
            "ectType IS NULL AND objectType IS NULL) AND (preserveMembers = @Original_preserv" & _
            "eMembers OR @Original_preserveMembers IS NULL AND preserveMembers IS NULL)"

            ' Get the imageurl that represents if this autoGroup or just a group
            Dim autoImage As System.Web.UI.WebControls.Image = CType(e.Item. _
                    FindControl("autoImage"), System.Web.UI.WebControls.Image)

            ' If a delete is trying to happen to a group that was automaticaly 
            ' generated by an attribute definition, disallow.
            If Not autoImage Is Nothing AndAlso autoImage.ImageUrl.ToLower.EndsWith("groupauto.gif") Then
                Dim strMessage As String = "This group definition was automatically" + _
                " generated based on an attribute based group definition.  Editting " + _
                "this group definition is not allowed because these groups are " + _
                "deleted and re-added when the groupPopulator.exe is run with the" + _
                " /r switch."
                Dim strScript As String = "<script language=JavaScript>"
                strScript += "alert(""" & strMessage & """);"
                strScript += Chr(60) & "/script>"

                If Not (Page.IsStartupScriptRegistered("clientScript")) Then
                    Page.RegisterStartupScript("alertScript", strScript)
                End If
                Exit Sub
            Else

                ' set the objectUID (UID)
                Dim objectUID As String = String.Empty
                objectUID = e.Item.Cells(1).Text.ToString

                Dim displayName As String = String.Empty
                displayName = e.Item.Cells(2).Text.ToString

                miisGroupManagementConnectionString.Open()

                ' Ensure that before you delete the group, 
                ' there are no other groups that are dependant on it
                ' for the where clause
                Dim clauseLinkReader As SqlDataReader = Nothing
                Dim sqlClauseLinkColumn(2) As Object
                Dim clauseLinkQuery As String = "select displayName " + _
                        "from groupDefinitions where objectUID <> " + _
                        "'" + objectUID + "' and clauseLink = '" + objectUID + "'"
                Dim clauseLinkCommand As SqlCommand = New SqlCommand _
                            (clauseLinkQuery, miisGroupManagementConnectionString)
                clauseLinkReader = clauseLinkCommand.ExecuteReader()

                While (clauseLinkReader.Read())
                    clauseLinkReader.GetValues(sqlClauseLinkColumn)

                    If Not clauseLinkReader.IsDBNull(0) AndAlso Not _
                                        sqlClauseLinkColumn(0).Equals("") Then

                        ' if there is a group that uses the clause,
                        ' create a popup message that alerts to the condition
                        Dim strMessage As String = "One or more groups are " + _
                            "dependant on this group for the clause.Please " + _
                            "remove the dependancy before deleting this group."

                        'Display alert window to the user
                        ThrowAlert(strMessage)

                        If Not (clauseLinkReader Is Nothing) Then
                            clauseLinkReader.Close()
                        End If

                        Exit Sub

                    End If
                End While

                If Not (clauseLinkReader Is Nothing) Then
                    clauseLinkReader.Close()
                End If

                ' Delete all the exceptions that are associated 
                ' with the group from the exceptions table
                Dim deleteExcludeStatement As String = "delete " + _
                        "from exceptionDefinitions where " + _
                            "exceptionDefinitions.objectUID = '" + objectUID + "'"
                Dim deleteExcludeCommand As SqlCommand = New SqlCommand _
                    (deleteExcludeStatement, miisGroupManagementConnectionString)
                recordsAffected = deleteExcludeCommand.ExecuteNonQuery()

                ' Delete all the clauses that are associated 
                ' with the group from the clause table
                Dim deleteWhereStatement As String = "delete from " + _
                    "clauseDefinitions where clauseDefinitions.objectUID = " + _
                                                            "'" + objectUID + "'"
                Dim deleteWhereCommand As SqlCommand = New SqlCommand _
                        (deleteWhereStatement, miisGroupManagementConnectionString)
                recordsAffected = deleteWhereCommand.ExecuteNonQuery()

                miisGroupManagementConnectionString.Close()

                Dim index As Integer = 0

                ' Get the value of the row that is going to be deleted and set
                ' it to the index.
                ' Note : 
                ' Currently there are 10 rows per page allowed,so that multiplying
                ' the page number by 10 and adding the value of the index on that
                ' page to get the value of the row that will be deleted.  
                ' For example if we are on page 3 which has the page index of
                ' (2 And we)are deleting the third row, then we need to set the 
                ' index to 3+(2*10) to get 23
                If Not dgGroupDefinitions.CurrentPageIndex = 0 Then
                    index = e.Item.ItemIndex + (dgGroupDefinitions. _
                                                        CurrentPageIndex * 10)
                Else
                    index = e.Item.ItemIndex
                End If

                miisGroupManagementConnectionString.Open()

                ' Load the variables with queries and commands that needs to be run
                Dim copyGroupToDeltaStatement As String = _
                    "INSERT INTO groupDefinitions_delta (objectUID, " + _
                        "groupAutoUID, objectType, displayName, description, " + _
                        "clauseLink, enabledFlag, maxExcept, preserveMembers, " + _
                        "groupType, mailNickName) SELECT objectUID, " + _
                        "groupAutoUID, objectType, displayName, description, " + _
                        "clauseLink, enabledFlag, maxExcept, preserveMembers, " + _
                        "groupType, mailNickName FROM groupDefinitions WHERE " + _
                        "objectUID = '" + objectUID + "'"
                Dim copyGroupToDeltaCommand As SqlCommand = New _
                                SqlCommand(copyGroupToDeltaStatement, _
                                        miisGroupManagementConnectionString)
                Dim rowsAffected As Integer = copyGroupToDeltaCommand. _
                                                            ExecuteNonQuery()

                If rowsAffected = 0 Then
                    lblError.Text = "There was an error adding the " + _
                                      objectUID + " group to the delta table."
                End If

                Dim updateGroupInDeltaStatement As String = _
                        "UPDATE groupDefinitions_delta set changeTime = " + _
                        "'" + Date.Now + "', changeType = 'Delete' WHERE " + _
                        "objectUID = '" + objectUID + "'  and changeType is null"
                Dim updateGroupInDeltaCommand As SqlCommand = _
                            New SqlCommand(updateGroupInDeltaStatement, _
                                        miisGroupManagementConnectionString)
                rowsAffected = updateGroupInDeltaCommand.ExecuteNonQuery()

                If rowsAffected = 0 Then
                    lblError.Text = "There was an error updating the " + _
                        objectUID + " group after it was added to the delta table."
                End If

                miisGroupManagementConnectionString.Close()

                ' Delete the row from the data grid
                dsGroupDefinitions1.groupDefinitions.Rows(index).Delete()
                sqldaGroupDefinitions.Update(dsGroupDefinitions1)

                ' Bind back to the first page in case the row that is
                ' deleted is the only row on that page
                dgGroupDefinitions.CurrentPageIndex = 0
                dgGroupDefinitions.DataBind()

                EventLog.WriteEntry("miisGroupManagement", _
                    Environment.UserDomainName.ToLower + "\" + _
                    Environment.UserName.ToLower + " deleted the group named '" _
                    + displayName + "' with object UID '" + objectUID + "'.", _
                    EventLogEntryType.Information)

            End If

        Catch exception As Exception
            HideControls()

            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "dgGroupDefinitions_DeleteCommand: " + _
                                                            MSG_TIMEOUT
            Else
                lblError.Text = "Error dgGroupDefinitions_DeleteCommand: " _
                                                          + exception.Message
            End If
            'Ensure that the database connection is closed
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
    ' Opens the buildClause windows and the buildException window,
    ' when the button is clicked
    ' </summary>
    ' <param name="source">Source object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub dgGroupDefinitions_ItemCommand1(ByVal source As Object, _
            ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
                                   Handles dgGroupDefinitions.ItemCommand

        Try

            Dim groupObjectUID As String = String.Empty
            Dim groupDisplayName As String = String.Empty

            ' If the '...' button is clicked under mail, 
            ' then open the buildMail window
            If (e.CommandName = "mailEnabledEdit") Then

                ' get the objectUID and the displayName to 
                ' pass into the new window
                groupObjectUID = e.Item.Cells(1).Text.ToString
                groupDisplayName = e.Item.Cells(2).Text.ToString

                If groupDisplayName = "" Then
                    Dim textBox As TextBox
                    textBox = CType(e.Item.Cells(2).Controls(0), TextBox)
                    groupDisplayName = textBox.Text
                End If

                groupDisplayName = Replace(groupDisplayName, "'", "\'")

                ' Open the buildMail window
                Dim strScript As String = _
                    "<script>window.open('buildMail.aspx?groupObjectUID=" + _
                    groupObjectUID + "&groupDisplayName=" + groupDisplayName _
                    + "', 'buildMail', 'width=415,height=315,left=50,top=100')"
                strScript += "</script>"
                RegisterClientScriptBlock("key3", strScript)
            End If

            ' If the '...' button is clicked under clause, 
            ' then open the buildClause window
            If (e.CommandName = "clauseImageEdit") Then

                ' get the objectUID and the displayName to 
                ' pass into the new window
                groupObjectUID = e.Item.Cells(1).Text.ToString
                groupDisplayName = e.Item.Cells(2).Text.ToString

                If groupDisplayName = "" Then
                    Dim textBox As TextBox = Nothing
                    textBox = CType(e.Item.Cells(2).Controls(0), TextBox)
                    groupDisplayName = textBox.Text
                End If

                groupDisplayName = Replace(groupDisplayName, "'", "\'")

                ' Open the buildClause window
                Dim strScript As String = "<script>window.open" + _
                    "('buildClause.aspx?groupObjectUID=" + groupObjectUID _
                    + "&groupDisplayName=" + groupDisplayName + _
                    "&SQLClause=', 'buildClause', 'width=615,height=315, " + _
                                                            "left=75,top=125')"
                strScript += "</script>"
                RegisterClientScriptBlock("key2", strScript)
            End If

            ' If the '...' button is clicked under exceptions, 
            ' then open the buildExcept window
            If (e.CommandName = "exceptionsImageEdit") Then

                ' get the objectUID and the displayName to 
                ' pass into the new window
                groupObjectUID = e.Item.Cells(1).Text.ToString
                groupDisplayName = e.Item.Cells(2).Text.ToString

                If groupDisplayName = "" Then
                    Dim textBox As TextBox = Nothing
                    textBox = CType(e.Item.Cells(2).Controls(0), TextBox)
                    groupDisplayName = textBox.Text
                End If

                groupDisplayName = Replace(groupDisplayName, "'", "\'")

                ' Open the buildExcept window
                Dim strScript As String = "<script>window.open" + _
                    "('buildExcept.aspx?groupObjectUID=" + groupObjectUID + _
                    "&groupDisplayName=" + groupDisplayName + _
                    "','buildExcept', 'width=615,height=315,left=100,top=150')"
                strScript += "</script>"
                RegisterClientScriptBlock("key1", strScript)
            End If

            ' If the checkButton is clicked under clause, then toggle it
            If (e.CommandName = "enabledImageEdit") Then

                Dim enabledImageEdit As ImageButton = CType _
                        (e.Item.FindControl("enabledImageEdit"), ImageButton)

                If enabledImageEdit.ImageUrl = "./images/buttonCheck.gif" Then
                    enabledImageEdit.ImageUrl = "./images/button.gif"
                Else
                    enabledImageEdit.ImageUrl = "./images/buttonCheck.gif"
                End If

            End If

        Catch exception As Exception
            HideControls()

            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "dgGroupDefinitions_ItemCommand1: " + _
                                                            MSG_TIMEOUT
            Else
                lblError.Text = "Error dgGroupDefinitions_ItemCommand1: " _
                                                        + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Adds a new row to the Data grid and the database and sets the row in
    ' edit mode for editing
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub btnAddRow_Click(ByVal sender As Object, _
                    ByVal e As System.EventArgs) Handles btnAddRow.Click

        Try

            lblError.Visible = False

            dgGroupDefinitions.Visible = True

            ' Add a new row to the datagrid
            Dim dataRow As DataRow = _
                dsGroupDefinitions1.groupDefinitions.NewRow

            ' Generate a new GUID for the new group
            Dim genObjectUID As Guid = System.Guid.NewGuid()
            Dim stringObjectUID As String = _
                                    "{" + genObjectUID.ToString.ToUpper + "}"

            ' Set the default values for the new group
            dataRow("objectUID") = stringObjectUID
            dataRow("objectType") = "group"
            dataRow("displayName") = ""
            dataRow("clauseLink") = stringObjectUID
            dataRow("groupType") = ADS_GROUP_TYPE_UNIVERSAL_GROUP Or _
                                            ADS_GROUP_TYPE_SECURITY_ENABLED
            dataRow("enabledFlag") = "enabled"
            dataRow("maxExcept") = "10"
            dataRow("preserveMembers") = "0"
            dataRow("description") = ""
            dataRow("mailNickName") = ""

            ' insert the new group row into the table
            dsGroupDefinitions1.groupDefinitions.Rows.InsertAt(dataRow, 0)

            ' manage the cache
            Session("DsGroups") = dsGroupDefinitions1

            ' update the datagrid
            sqldaGroupDefinitions.Update(dsGroupDefinitions1)

            miisGroupManagementConnectionString.Open()

            ' Load the queries to the local variables
            Dim copyGroupToDeltaStatement As String = _
                    "INSERT INTO groupDefinitions_delta (objectUID, " + _
                    "groupAutoUID, objectType, displayName, description, " + _
                    "clauseLink, enabledFlag, maxExcept, preserveMembers, " + _
                    "groupType, mailNickName) SELECT objectUID, " + _
                    "groupAutoUID, objectType, displayName, description, " + _
                    "clauseLink, enabledFlag, maxExcept, preserveMembers, " + _
                    "groupType, mailNickName FROM groupDefinitions WHERE " + _
                    "objectUID = '" + stringObjectUID + "'"
            Dim copyGroupToDeltaCommand As SqlCommand = _
                    New SqlCommand(copyGroupToDeltaStatement, _
                                miisGroupManagementConnectionString)
            Dim rowsAffected As Integer = copyGroupToDeltaCommand. _
                                                            ExecuteNonQuery()

            If rowsAffected = 0 Then
                lblError.Text = "There was an error adding the " + _
                                stringObjectUID + " group to the delta table."
            End If

            Dim updateGroupInDeltaStatement As String = _
                "UPDATE groupDefinitions_delta SET changeTime = '" _
                + Date.Now + "', changeType = 'Add' WHERE objectUID = '" _
                + stringObjectUID + "'  and changeType is NULL"
            Dim updateGroupInDeltaCommand As SqlCommand = _
                        New SqlCommand(updateGroupInDeltaStatement, _
                                        miisGroupManagementConnectionString)
            rowsAffected = updateGroupInDeltaCommand.ExecuteNonQuery()

            If rowsAffected = 0 Then
                lblError.Text = "There was an error updating the " + _
                        stringObjectUID + " group after it was added " + _
                                                        "to the delta table."
            End If

            miisGroupManagementConnectionString.Close()

            ' Set the datagrid in edit mode and unhide/hide 
            ' the columns for edit mode
            dgGroupDefinitions.Columns(3).Visible = True
            dgGroupDefinitions.Columns(11).Visible = False
            dgGroupDefinitions.EditItemIndex = 0
            dgGroupDefinitions.CurrentPageIndex = 0
            dgGroupDefinitions.DataBind()

        Catch exception As Exception
            HideControls()

            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "btnAddRow_Click: Your session has " + _
                    "timed out or there is an object reference error, " + _
                                                       "please reconnect."
            Else
                lblError.Text = "Error btnAddRow_Click: " + exception.Message
            End If
            'Ensure that the database connection is closed
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
    ' Enables the ddlSearchOperation box if a ddlSearchAttribute is selected
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub ddlSearchAttribute_SelectedIndexChanged _
                (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles ddlSearchAttribute.SelectedIndexChanged

        Try

            ' Declarations
            Dim newItem As ListItem
            newItem = New ListItem("", "")
            ddlSearchOperation.Items.Add(newItem)

            ddlSearchOperation.Items.Clear()
            txtSearchValue.Text = String.Empty
            txtSearchValue.Enabled = False
            txtSearchValue.BackColor = System.Drawing.Color.LightGray

            ' For each unique selection criterion, perform the 
            ' corresponding operations
            Select Case ddlSearchAttribute.SelectedValue.ToLower
                Case "all", ""
                    ddlSearchOperation.Enabled = False
                    ddlSearchOperation.BackColor = _
                            System.Drawing.Color.LightGray

                Case "clause", "exception"
                    newItem = New ListItem("", "")
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem("Is present", "is present")
                    ddlSearchOperation.Items.Add(newItem)
                    If ddlSearchAttribute.SelectedValue.ToLower = "clause" Then
                        newItem = New ListItem("Is not present", _
                                                        "is not present")
                        ddlSearchOperation.Items.Add(newItem)
                    End If

                    ' Enable the ddlSearchOperation box if a 
                    ' ddlSearchAttribute is selected 
                    ddlSearchOperation.Enabled = True
                    ddlSearchOperation.BackColor = System.Drawing.Color.White

                Case "groupdefinitions.enabledflag"
                    newItem = New ListItem("", "")
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem("Is enabled", "is enabled")
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem("Is not enabled", "is not enabled")
                    ddlSearchOperation.Items.Add(newItem)

                    ' enable the ddlSearchOperation box if a 
                    ' ddlSearchAttribute is selected 
                    ddlSearchOperation.Enabled = True
                    ddlSearchOperation.BackColor = System.Drawing.Color.White

                Case "groupdefinitions.grouptype"
                    newItem = New ListItem("", "")
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem(SEC_GROUP_UNIV, _
                                    ADS_GROUP_TYPE_UNIVERSAL_GROUP Or _
                                              ADS_GROUP_TYPE_SECURITY_ENABLED)
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem(DIST_GROUP_UNIV, _
                                               ADS_GROUP_TYPE_UNIVERSAL_GROUP)
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem(SEC_GROUP_GLOBAL, _
                                     ADS_GROUP_TYPE_GLOBAL_GROUP Or _
                                            ADS_GROUP_TYPE_SECURITY_ENABLED)
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem(DIST_GROUP_GLOBAL, _
                                                ADS_GROUP_TYPE_GLOBAL_GROUP)
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem(SEC_GROUP_DOMLOCAL, _
                                    ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP Or _
                                            ADS_GROUP_TYPE_SECURITY_ENABLED)
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem(DIST_GROUP_DOMLOCAL, _
                                          ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP)
                    ddlSearchOperation.Items.Add(newItem)

                    ' enable the ddlSearchOperation box if a 
                    ' ddlSearchAttribute is selected 
                    ddlSearchOperation.Enabled = True
                    ddlSearchOperation.BackColor = System.Drawing.Color.White

                    ' Otherwise, enter the values like Equals, Starts with,
                    ' Ends with, Contains etc. into the dropdownlist
                Case Else
                    newItem = New ListItem("", "")
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem("Equals", "equals")
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem("Starts with", "starts with")
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem("Ends with", "ends with")
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem("Contains", "contains")
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem("Does not contain", _
                                                    "does not contain")
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem("Is present", "is present")
                    ddlSearchOperation.Items.Add(newItem)
                    newItem = New ListItem("Is not present", "is not present")
                    ddlSearchOperation.Items.Add(newItem)

                    ' enable the ddlSearchOperation box if a 
                    ' ddlSearchAttribute is selected 
                    ddlSearchOperation.Enabled = True
                    ddlSearchOperation.BackColor = System.Drawing.Color.White

            End Select

        Catch exception As Exception
            HideControls()

            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "srchAttrib_SelectedIndexChanged: " + _
                                                          MSG_TIMEOUT
            Else
                lblError.Text = "Error srchAttrib_SelectedIndexChanged: " _
                                                        + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Enables the ddlSearchOperation box if a ddlSearchAttribute is selected
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub ddlSearchOperation_SelectedIndexChanged(ByVal sender As System.Object, _
                ByVal e As System.EventArgs) _
                Handles ddlSearchOperation.SelectedIndexChanged

        Try

            ' enable the ddlSearchOperation box if a 
            ' ddlSearchAttribute is selected
            Select Case ddlSearchOperation.SelectedValue.ToLower

                ' if the selected value equals 'is present' or 
                ' is not present' then there is no
                ' need to enable the txtSearchValue box
            Case "is present", "is not present", "is enabled", _
                            "is not enabled", "is enabled", "is not enabled", ""
                    txtSearchValue.Text = ""
                    txtSearchValue.Enabled = False
                    txtSearchValue.BackColor = System.Drawing.Color.LightGray

                Case Else
                    If Not ddlSearchAttribute.SelectedValue.ToLower = _
                                            "groupdefinitions.grouptype" Then
                        txtSearchValue.Enabled = True
                        txtSearchValue.BackColor = System.Drawing.Color.White
                    End If

            End Select

        Catch exception As Exception
            HideControls()

            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "srchOper_SelectedIndexChanged: " + _
                                                          MSG_TIMEOUT
            Else
                lblError.Text = "Error srchOper_SelectedIndexChanged: " _
                                                        + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Performs the search operation and populates the datagrid based on 
    ' the search criteria
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub btnSrchGroups_Click(ByVal sender As System.Object, _
                    ByVal e As System.EventArgs) Handles btnSrchGroups.Click

        Try

            ' Removes the DataGrid row from edit mode
            dgGroupDefinitions.EditItemIndex = -1

            dgGroupDefinitions.Visible = True

            ' changes the state of the columns back to non-editable mode
            dgGroupDefinitions.Columns(3).Visible = False
            dgGroupDefinitions.Columns(11).Visible = True

            lblError.Visible = False
            dgGroupDefinitions.Visible = True
            btnAddRow.Visible = True

            ' Search as long as ddlSearchAttribute, ddlSearchOperation, 
            ' and txtSearchValue are not empty
            If Not ddlSearchAttribute.SelectedValue = _
                "" AndAlso (Not ddlSearchOperation. _
                SelectedValue = "" Or ddlSearchOperation.Enabled = False) _
                AndAlso (Not txtSearchValue.Text = "" Or _
                txtSearchValue.Enabled = False) Then

                ' Store the clause that fills the datagrid
                Dim sqlSearchCommand As String = _
                            Me.SqlSelectCommand1.CommandText

                Dim sqlFromCommand As String = String.Empty

                Select Case ddlSearchAttribute.SelectedValue.ToLower

                    ' Incase of ddlSearchAttribute's values as Clause
                    ' and Exception, execute the following query
                Case "clause", "exception"
                        If ddlSearchAttribute.SelectedValue.ToLower = _
                            "clause" Then
                            sqlSearchCommand = "SELECT " + _
                            "groupDefinitions.objectUID, " + _
                            "groupDefinitions.objectType, " + _
                            "groupDefinitions.displayName, " + _
                            "groupDefinitions.clauseLink, " + _
                            "groupDefinitions.enabledFlag, " + _
                            "groupDefinitions.maxExcept, " + _
                            "groupDefinitions.groupType, " + _
                            "groupDefinitions.description, " + _
                            "groupDefinitions.mailNickName, " + _
                            "groupDefinitions.preserveMembers "
                            sqlFromCommand = " FROM groupDefinitions, " + _
                                                       "clauseDefinitions "
                        Else
                            sqlSearchCommand = "SELECT DISTINCT " + _
                                "(groupDefinitions.objectUID), " + _
                                "groupDefinitions.objectType, " + _
                                "groupDefinitions.displayName, " + _
                                "groupDefinitions.clauseLink, " + _
                                "groupDefinitions.enabledFlag, " + _
                                "groupDefinitions.maxExcept, " + _
                                "groupDefinitions.groupType, " + _
                                "groupDefinitions.description, " + _
                                "groupDefinitions.mailNickName, " + _
                                "groupDefinitions.preserveMembers "

                            sqlFromCommand = " FROM groupDefinitions, " + _
                                                   "exceptionDefinitions "
                        End If
                End Select

                Dim sqlWhereClause As String = String.Empty

                If cbIncludeGroupAuto.Checked = True Then
                    sqlWhereClause = " WHERE (objectType = 'group' or " + _
                                                "objectType = 'groupAuto') "
                Else
                    sqlWhereClause = " WHERE (objectType = 'group') "
                End If

                Dim srchValueOper As String = txtSearchValue.Text
                Dim sqlSearchClause As String = String.Empty

                ' Need to skip the "'" character,because it is a 
                ' special character in SQL
                srchValueOper = Replace(srchValueOper, "'", "''")

                ' Setup the search statement depending on the 
                ' value of the ddlSearchOperation
                Select Case ddlSearchOperation.SelectedValue.ToLower
                    Case "equals"
                        sqlSearchClause = _
                            " and (" + ddlSearchAttribute.SelectedValue _
                                           + " = '" + srchValueOper + "')"

                    Case "starts with"
                        sqlSearchClause = _
                            " and (" + ddlSearchAttribute.SelectedValue _
                                       + " like '" + srchValueOper + "%')"

                    Case "ends with"
                        sqlSearchClause = _
                            " and (" + ddlSearchAttribute.SelectedValue _
                                        + " like '%" + srchValueOper + "')"

                    Case "contains"
                        sqlSearchClause = _
                            " and (" + ddlSearchAttribute.SelectedValue _
                                       + " like '%" + srchValueOper + "%')"

                    Case "does not contain"
                        sqlSearchClause = _
                            " and (" + ddlSearchAttribute.SelectedValue _
                                   + " not like '%" + srchValueOper + "%')"

                    Case "is present" ', "is enabled"
                        sqlSearchClause = _
                            " and (" + ddlSearchAttribute.SelectedValue + _
                            " is not null and " + _
                            ddlSearchAttribute.SelectedValue + " <> '')"

                        ' If the ddlSearchAttribute is clause or exception,
                        ' then assign the corresponding search clauses
                        If ddlSearchAttribute.SelectedValue.ToLower = _
                            "clause" Then
                            sqlSearchClause = _
                                " and ((groupDefinitions.clauseLink " + _
                                "= clauseDefinitions.objectUID) and " + _
                                "(clauseDefinitions.clause is not null and " + _
                                "clauseDefinitions.clause <> ''))"
                        ElseIf ddlSearchAttribute.SelectedValue.ToLower = _
                                                            "exception" Then
                            sqlSearchClause = _
                                " and ((groupDefinitions.objectUID = " + _
                            "exceptionDefinitions.objectUID) and " + _
                            "(exceptionDefinitions.mvObjectUID is not null" + _
                            " and exceptionDefinitions.mvObjectUID <> ''))"
                        End If

                    Case "is not present" ', "is not enabled"
                        sqlSearchClause = _
                            " and (" + ddlSearchAttribute.SelectedValue + _
                            " is null or " + _
                            ddlSearchAttribute.SelectedValue + " = '')"

                        If ddlSearchAttribute.SelectedValue.ToLower = _
                                                    "clause" Then
                            sqlSearchClause = _
                                " and ((groupDefinitions.clauseLink " + _
                            "= clauseDefinitions.objectUID) and " + _
                            "(clauseDefinitions.clause is null or " + _
                            "clauseDefinitions.clause = ''))"
                        ElseIf ddlSearchAttribute.SelectedValue.ToLower = _
                                                           "exception" Then
                            sqlSearchClause = _
                            " and ((groupDefinitions.objectUID " + _
                            "= exceptionDefinitions.objectUID) and " + _
                            "(exceptionDefinitions.mvObjectUID is null or " + _
                            "exceptionDefinitions.mvObjectUID = ''))"
                        End If

                    Case "is enabled"
                        sqlSearchClause = " and (" + _
                                ddlSearchAttribute.SelectedValue _
                                                    + " = 'enabled')"

                    Case "is not enabled"
                        sqlSearchClause = " and (" + _
                            ddlSearchAttribute.SelectedValue + _
                                                    " = 'disabled')"

                    Case (ADS_GROUP_TYPE_UNIVERSAL_GROUP Or _
                        ADS_GROUP_TYPE_SECURITY_ENABLED), _
                            (ADS_GROUP_TYPE_GLOBAL_GROUP Or _
                                ADS_GROUP_TYPE_SECURITY_ENABLED), _
                                    (ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP _
                                        Or ADS_GROUP_TYPE_SECURITY_ENABLED), _
                                        ADS_GROUP_TYPE_UNIVERSAL_GROUP, _
                                        ADS_GROUP_TYPE_GLOBAL_GROUP, _
                                        ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP
                        sqlSearchClause = " and (" + _
                            ddlSearchAttribute.SelectedValue _
                            + " = '" + ddlSearchOperation.SelectedValue + "')"

                End Select

                Dim sqlClauseSuffix As String = " order by displayName "

                ' build the serach statement
                Me.SqlSelectCommand1.CommandText = sqlSearchCommand _
                                + sqlFromCommand + sqlWhereClause + _
                                        sqlSearchClause + sqlClauseSuffix

                ' clear the datagrid
                dsGroupDefinitions1.Clear()

                ' refill datagrid with new query
                sqldaGroupDefinitions.Fill(dsGroupDefinitions1)
                dgGroupDefinitions.CurrentPageIndex = 0
                dgGroupDefinitions.DataBind()

            End If

        Catch exception As Exception
            HideControls()

            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "btnSrchGroups_Click: " + MSG_TIMEOUT
            Else
                lblError.Text = _
                    "Error btnSrchGroups_Click: " + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Updates the datagrid and removes the datagrid from edit 
    ' mode when update is clicked
    ' </summary>
    ' <param name="source">Source object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub dgGroupDefinitions_PageIndexChanged(ByVal source As Object, _
        ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) _
            Handles dgGroupDefinitions.PageIndexChanged

        Try

            ' Change the page number when the pageIndex is clicked
            dgGroupDefinitions.CurrentPageIndex = e.NewPageIndex
            dgGroupDefinitions.DataBind()

        Catch exception As Exception
            HideControls()

            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "dgGroupDefinitions_PageIndexChanged: " + _
                                                    MSG_TIMEOUT
            Else
                lblError.Text = _
                    "Error dgGroupDefinitions_PageIndexChanged: " _
                                                 + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Converts the groupType into a friendly group type name</summary>
    ' <param name="groupTypeNum">Value representing the group type</param>
    ' <returns>String</returns>

    Function GetGroupTypeName(ByVal groupTypeNum As String) As String

        ' Based on the grouptypeNum, get the corresponding group type
        Select Case groupTypeNum
            Case ADS_GROUP_TYPE_UNIVERSAL_GROUP Or _
                                ADS_GROUP_TYPE_SECURITY_ENABLED
                Return SEC_GROUP_UNIV
            Case ADS_GROUP_TYPE_UNIVERSAL_GROUP
                Return DIST_GROUP_UNIV
            Case ADS_GROUP_TYPE_GLOBAL_GROUP Or _
                                ADS_GROUP_TYPE_SECURITY_ENABLED
                Return SEC_GROUP_GLOBAL
            Case ADS_GROUP_TYPE_GLOBAL_GROUP
                Return DIST_GROUP_GLOBAL
            Case ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP Or _
                                ADS_GROUP_TYPE_SECURITY_ENABLED
                Return SEC_GROUP_DOMLOCAL
            Case ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP
                Return DIST_GROUP_DOMLOCAL
            Case Else
                Return "Invalid"
        End Select
    End Function

    ' <summary>
    ' Determine the status of the checkbox that displays if a group 
    ' is mail enabled</summary>
    ' <param name="mailNickName">The name of the mailing group</param>
    ' <returns>String</returns>

    Function GetMailNickNameStatus(ByVal mailNickName As String) As String
        Select Case mailNickName
            Case ""
                Return "./images/blank.gif"
            Case Else
                Return "./images/check.gif"
        End Select
    End Function

    ' <summary>
    ' Determine the status of the checkbox that displays if a group 
    ' is mail enabled</summary>
    ' <param name="mailNickName">The name of the mailing group</param>
    ' <returns>String</returns>

    Function GetMailNickNameStatusEdit(ByVal mailNickName As String) As String

        Return "./images/buttonEdit.gif"

    End Function

    ' <summary>
    ' Determine the status of the checkbox that displays if a group is 
    ' mail enabled by the return type of the status flag</summary>
    ' <param name="enabledFlag">Flag indicating the mail enabled 
    ' status</param>
    ' <returns>String</returns>

    Function GetenabledFlagStatus(ByVal enabledFlag As String) As String
        Select Case enabledFlag.ToLower
            Case "enabled"
                Return "./images/check.gif"
            Case Else
                Return "./images/blank.gif"
        End Select
    End Function

    ' <summary>
    ' Determine the status of the checkbox that displays if a group 
    ' is mail enabled</summary>
    ' <param name="enabledFlag">Flag indicating the mail enabled 
    ' status</param>
    ' <returns>String</returns>

    Function GetenabledFlagStatusEdit(ByVal enabledFlag As String) As String
        Select Case enabledFlag.ToLower
            Case "enabled"
                Return "./images/buttonCheck.gif"
            Case Else
                Return "./images/button.gif"
        End Select
    End Function

    ' <summary>
    ' Determines the status of the checkbox control that displays 
    ' if the group has a clause set
    ' </summary>
    ' <param name="objectUID">Group name</param>
    ' <returns>String</returns>

    Function GetClauseStatus(ByVal objectUID As String) As String

        Dim clauseLink As String = String.Empty

        Try
            miisGroupManagementConnectionString.Open()

            ' select rows from the groupDefinitions table that 
            ' have the same name.If any name is returned then alert 
            ' that the name is not unique in the table
            Dim clauseLinkReader As SqlDataReader = Nothing
            Dim sqlClauseLinkColumn(1) As Object
            Dim clauseLinkQuery As String = "select clauseLink from " + _
                 "groupDefinitions where objectUID = '" + objectUID + "'"
            Dim clauseLinkCommand As SqlCommand = New SqlCommand _
                 (clauseLinkQuery, miisGroupManagementConnectionString)
            clauseLinkReader = clauseLinkCommand.ExecuteReader()

            While (clauseLinkReader.Read())
                clauseLinkReader.GetValues(sqlClauseLinkColumn)

                If Not clauseLinkReader.IsDBNull(0) AndAlso Not _
                                    sqlClauseLinkColumn(0).Equals("") Then
                    clauseLink = sqlClauseLinkColumn(0)

                Else
                    If Not (clauseLinkReader Is Nothing) Then
                        clauseLinkReader.Close()
                    End If

                    miisGroupManagementConnectionString.Close()
                    Return "./images/blank.gif"

                End If

            End While

            If Not (clauseLinkReader Is Nothing) Then
                clauseLinkReader.Close()
            End If

            ' select rows from the groupDefinitions table that 
            ' have the same name.If any name is returned then alert 
            ' that the name is not unique in the table
            Dim clauseReader As SqlDataReader = Nothing
            Dim sqlClauseColumn(1) As Object
            Dim clauseQuery As String = "select clause from " + _
                 "clauseDefinitions where objectUID = '" + clauseLink + "'"
            Dim clauseCommand As SqlCommand = New SqlCommand _
                        (clauseQuery, miisGroupManagementConnectionString)
            clauseReader = clauseCommand.ExecuteReader()

            While (clauseReader.Read())
                clauseReader.GetValues(sqlClauseColumn)

                If Not clauseReader.IsDBNull(0) AndAlso Not _
                                        sqlClauseColumn(0).Equals("") Then
                    If Not (clauseReader Is Nothing) Then
                        clauseReader.Close()
                    End If

                    miisGroupManagementConnectionString.Close()
                    Return "./images/check.gif"

                Else
                    If Not (clauseReader Is Nothing) Then
                        clauseReader.Close()
                    End If

                    miisGroupManagementConnectionString.Close()
                    Return "./images/blank.gif"

                End If

            End While

            If Not (clauseReader Is Nothing) Then
                clauseReader.Close()
            End If

            miisGroupManagementConnectionString.Close()
            Return "./images/blank.gif"

        Catch exception As Exception
            lblError.Text = exception.Message

        End Try

    End Function

    ' <summary>
    ' Determines the status of the checkbox control which
    ' indicates whether the group has a clause set
    ' </summary>
    ' <param name="objectUID">Group name</param>
    ' <returns>String</returns>

    Function GetClauseStatusEdit(ByVal objectUID As String) As String

        Return "./images/buttonEdit.gif"

    End Function

    ' <summary>
    ' Determines the status of the checkbox control which
    ' indicates whether a group is mail enabled
    ' </summary>
    ' <param name="objectUID">Group name</param>
    ' <returns>String</returns>

    Function GetExceptionStatus(ByVal objectUID As String) As String
        miisGroupManagementConnectionString.Open()

        ' select rows from the groupDefinitions table that 
        ' has the same name.If any name is returned then alert 
        ' that the name is not unique in the table
        Dim exceptionReader As SqlDataReader = Nothing
        Dim sqlexceptionColumn(1) As Object
        Dim exceptionQuery As String = "select objectUID from " + _
            "exceptionDefinitions where objectUID = '" + objectUID + "' " + _
                "and (exceptType = 'include' or exceptType = 'exclude')"
        Dim exceptionCommand As SqlCommand = New SqlCommand _
                    (exceptionQuery, miisGroupManagementConnectionString)
        exceptionReader = exceptionCommand.ExecuteReader()

        If exceptionReader.HasRows Then
            If Not (exceptionReader Is Nothing) Then
                exceptionReader.Close()
            End If

            miisGroupManagementConnectionString.Close()
            Return "./images/check.gif"
        Else
            If Not (exceptionReader Is Nothing) Then
                exceptionReader.Close()
            End If
            miisGroupManagementConnectionString.Close()
            Return "./images/blank.gif"
        End If

    End Function

    ' <summary>
    ' Determines the status of the checkbox control which 
    ' indicates whether a group is mail enabled
    ' </summary>
    ' <param name="objectUID">Group name</param>
    ' <returns>String</returns>

    Function GetExceptionStatusEdit(ByVal objectUID As String) As String

        Return "./images/buttonEdit.gif"

    End Function

    ' <summary>
    ' Determines the status of the checkbox control which indicates 
    ' whether a group is mail enabled
    ' </summary>
    ' <param name="objectUID">Group name</param>
    ' <returns>String</returns>

    Function GetObjectType(ByVal objectType As String) As String
        Select Case objectType.ToLower
            Case "groupauto"
                Return "./images/groupAuto.gif"
            Case Else
                Return "./images/group.gif"
        End Select
    End Function

    ' <summary>
    ' Set the length of the visible description field of a Group
    ' </summary>
    ' <param name="objectUID">Group name</param>
    ' <returns>String</returns>

    Function GetDescription(ByVal description As String) As String
        If description.Length > GROUPNAME_LENGTH_MAX Then
            description = Left(description, GROUPNAME_LENGTH_TRUNCATE) + "..."
            Return description
        Else
            Return description
        End If
    End Function

    ' <summary>
    ' Switches to the page that manages Attribute group definitions
    ' </summary>
    ' <param name="objectUID">Group name</param>
    ' <returns>String</returns>

    Private Sub btnDefineAttributeDefs_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles btnDefineAttributeDefs.Click
        Server.Transfer("manageAttrGroup.aspx", False)
    End Sub

    ' <summary>
    ' Hides the active controls for group definition and
    ' attirbute row addition upon any exception or error.
    ' Sets the error lable Visible property to true.
    ' </summary>

    Private Sub HideControls()

        dgGroupDefinitions.Visible = False
        btnAddRow.Visible = False
        lblError.Visible = True

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

            Dim InvalidChars As String = "~!@#$%^&*()<>=_+./\\:;\`[]|"

            ' Return true or false based on the validation success
            If (input.IndexOfAny(InvalidChars.ToCharArray()) > -1) Then

                Return False

            Else
                Return True
            End If
        End If
    End Function

End Class
