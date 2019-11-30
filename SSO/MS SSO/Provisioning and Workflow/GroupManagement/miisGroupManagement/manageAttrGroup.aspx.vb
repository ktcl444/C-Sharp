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
' Contains the functionality for managing the attributes that defines a group
' <Summary/>
Public Class AddAttributeGroup
    Inherits System.Web.UI.Page

    ' Group types in Active Directory
    Const ADS_GROUP_TYPE_GLOBAL_GROUP = &H2
    Const ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP = &H4
    Const ADS_GROUP_TYPE_UNIVERSAL_GROUP = &H8
    Protected WithEvents lblPageTitle As System.Web.UI.WebControls.Label
    Const ADS_GROUP_TYPE_SECURITY_ENABLED = &H80000000

    Const MSG_NULL_REFERENCE = "Object reference not set to an instance of an object."
    Const MSG_TIMEOUT = "Your session has timed out or there is an object" + _
                        " reference error, please reconnect."

    'Constants for group types
    Const SEC_GROUP_UNIV = "Sec Group - Univ"
    Const DIST_GROUP_UNIV = "Dist Group - Univ"
    Const SEC_GROUP_GLOBAL = "Sec Group - Global"
    Const DIST_GROUP_GLOBAL = "Dist Group - Global"
    Const SEC_GROUP_DOMLOCAL = "Sec Group - DomLocal"
    Protected WithEvents lblPageHeading As System.Web.UI.WebControls.Label
    Const DIST_GROUP_DOMLOCAL = "Dist Group - DomLocal"

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.sqldaAttributeGroupDefinitions = New System.Data.SqlClient.SqlDataAdapter
        Me.SqlDeleteCommand1 = New System.Data.SqlClient.SqlCommand
        Me.miisGroupManagementConnectionString = New System.Data.SqlClient.SqlConnection
        Me.SqlInsertCommand1 = New System.Data.SqlClient.SqlCommand
        Me.SqlSelectCommand1 = New System.Data.SqlClient.SqlCommand
        Me.SqlUpdateCommand1 = New System.Data.SqlClient.SqlCommand
        Me.dsAttributeGroupDefinitions1 = New gp.dsAttributeGroupDefinitions
        Me.miisConnectionString = New System.Data.SqlClient.SqlConnection
        CType(Me.dsAttributeGroupDefinitions1, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'sqldaAttributeGroupDefinitions
        '
        Me.sqldaAttributeGroupDefinitions.DeleteCommand = Me.SqlDeleteCommand1
        Me.sqldaAttributeGroupDefinitions.InsertCommand = Me.SqlInsertCommand1
        Me.sqldaAttributeGroupDefinitions.SelectCommand = Me.SqlSelectCommand1
        Me.sqldaAttributeGroupDefinitions.TableMappings.AddRange(New System.Data.Common.DataTableMapping() {New System.Data.Common.DataTableMapping("Table", "attributeGroupDefinitions", New System.Data.Common.DataColumnMapping() {New System.Data.Common.DataColumnMapping("objectUID", "objectUID"), New System.Data.Common.DataColumnMapping("uniqueGroupID", "uniqueGroupID"), New System.Data.Common.DataColumnMapping("displayName", "displayName"), New System.Data.Common.DataColumnMapping("attributeGroupType", "attributeGroupType"), New System.Data.Common.DataColumnMapping("attribute", "attribute"), New System.Data.Common.DataColumnMapping("linkAttribute", "linkAttribute"), New System.Data.Common.DataColumnMapping("linkAttributeKey", "linkAttributeKey"), New System.Data.Common.DataColumnMapping("groupType", "groupType"), New System.Data.Common.DataColumnMapping("mailEnabled", "mailEnabled")})})
        Me.sqldaAttributeGroupDefinitions.UpdateCommand = Me.SqlUpdateCommand1
        '
        'SqlDeleteCommand1
        '
        Me.SqlDeleteCommand1.CommandText = "DELETE FROM attributeGroupDefinitions WHERE (objectUID = @Original_objectUID) AND" & _
        " (attribute = @Original_attribute OR @Original_attribute IS NULL AND attribute I" & _
        "S NULL) AND (attributeGroupType = @Original_attributeGroupType OR @Original_attr" & _
        "ibuteGroupType IS NULL AND attributeGroupType IS NULL) AND (displayName = @Origi" & _
        "nal_displayName OR @Original_displayName IS NULL AND displayName IS NULL) AND (g" & _
        "roupType = @Original_groupType OR @Original_groupType IS NULL AND groupType IS N" & _
        "ULL) AND (linkAttribute = @Original_linkAttribute OR @Original_linkAttribute IS " & _
        "NULL AND linkAttribute IS NULL) AND (linkAttributeKey = @Original_linkAttributeK" & _
        "ey OR @Original_linkAttributeKey IS NULL AND linkAttributeKey IS NULL) AND (mail" & _
        "Enabled = @Original_mailEnabled OR @Original_mailEnabled IS NULL AND mailEnabled" & _
        " IS NULL) AND (uniqueGroupID = @Original_uniqueGroupID OR @Original_uniqueGroupI" & _
        "D IS NULL AND uniqueGroupID IS NULL)"
        Me.SqlDeleteCommand1.Connection = Me.miisGroupManagementConnectionString
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_objectUID", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "objectUID", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_attribute", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "attribute", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_attributeGroupType", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "attributeGroupType", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_displayName", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "displayName", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_groupType", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "groupType", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_linkAttribute", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "linkAttribute", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_linkAttributeKey", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "linkAttributeKey", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_mailEnabled", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "mailEnabled", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_uniqueGroupID", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "uniqueGroupID", System.Data.DataRowVersion.Original, Nothing))
        '
        'miisGroupManagementConnectionString
        '
        Me.miisGroupManagementConnectionString.ConnectionString = "workstation id=""localhost"";packet size=4096;integrated security=SSPI;data source=" & _
        "localhost;persist security info=False;initial catalog=miisGroupManagement"
        '
        'SqlInsertCommand1
        '
        Me.SqlInsertCommand1.CommandText = "INSERT INTO attributeGroupDefinitions(objectUID, uniqueGroupID, displayName, attr" & _
        "ibuteGroupType, attribute, linkAttribute, linkAttributeKey, groupType, mailEnabl" & _
        "ed) VALUES (@objectUID, @uniqueGroupID, @displayName, @attributeGroupType, @attr" & _
        "ibute, @linkAttribute, @linkAttributeKey, @groupType, @mailEnabled); SELECT obje" & _
        "ctUID, uniqueGroupID, displayName, attributeGroupType, attribute, linkAttribute," & _
        " linkAttributeKey, groupType, mailEnabled FROM attributeGroupDefinitions WHERE (" & _
        "objectUID = @objectUID)"
        Me.SqlInsertCommand1.Connection = Me.miisGroupManagementConnectionString
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@objectUID", System.Data.SqlDbType.VarChar, 64, "objectUID"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@uniqueGroupID", System.Data.SqlDbType.VarChar, 256, "uniqueGroupID"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@displayName", System.Data.SqlDbType.VarChar, 256, "displayName"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@attributeGroupType", System.Data.SqlDbType.VarChar, 256, "attributeGroupType"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@attribute", System.Data.SqlDbType.VarChar, 256, "attribute"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@linkAttribute", System.Data.SqlDbType.VarChar, 256, "linkAttribute"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@linkAttributeKey", System.Data.SqlDbType.VarChar, 256, "linkAttributeKey"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@groupType", System.Data.SqlDbType.Int, 4, "groupType"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@mailEnabled", System.Data.SqlDbType.VarChar, 256, "mailEnabled"))
        '
        'SqlSelectCommand1
        '
        Me.SqlSelectCommand1.CommandText = "SELECT objectUID, uniqueGroupID, displayName, attributeGroupType, attribute, link" & _
        "Attribute, linkAttributeKey, groupType, mailEnabled FROM attributeGroupDefinitio" & _
        "ns"
        Me.SqlSelectCommand1.Connection = Me.miisGroupManagementConnectionString
        '
        'SqlUpdateCommand1
        '
        Me.SqlUpdateCommand1.CommandText = "UPDATE attributeGroupDefinitions SET objectUID = @objectUID, uniqueGroupID = @uni" & _
        "queGroupID, displayName = @displayName, attributeGroupType = @attributeGroupType" & _
        ", attribute = @attribute, linkAttribute = @linkAttribute, linkAttributeKey = @li" & _
        "nkAttributeKey, groupType = @groupType, mailEnabled = @mailEnabled WHERE (object" & _
        "UID = @Original_objectUID) AND (attribute = @Original_attribute OR @Original_att" & _
        "ribute IS NULL AND attribute IS NULL) AND (attributeGroupType = @Original_attrib" & _
        "uteGroupType OR @Original_attributeGroupType IS NULL AND attributeGroupType IS N" & _
        "ULL) AND (displayName = @Original_displayName OR @Original_displayName IS NULL A" & _
        "ND displayName IS NULL) AND (groupType = @Original_groupType OR @Original_groupT" & _
        "ype IS NULL AND groupType IS NULL) AND (linkAttribute = @Original_linkAttribute " & _
        "OR @Original_linkAttribute IS NULL AND linkAttribute IS NULL) AND (linkAttribute" & _
        "Key = @Original_linkAttributeKey OR @Original_linkAttributeKey IS NULL AND linkA" & _
        "ttributeKey IS NULL) AND (mailEnabled = @Original_mailEnabled OR @Original_mailE" & _
        "nabled IS NULL AND mailEnabled IS NULL) AND (uniqueGroupID = @Original_uniqueGro" & _
        "upID OR @Original_uniqueGroupID IS NULL AND uniqueGroupID IS NULL); SELECT objec" & _
        "tUID, uniqueGroupID, displayName, attributeGroupType, attribute, linkAttribute, " & _
        "linkAttributeKey, groupType, mailEnabled FROM attributeGroupDefinitions WHERE (o" & _
        "bjectUID = @objectUID)"
        Me.SqlUpdateCommand1.Connection = Me.miisGroupManagementConnectionString
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@objectUID", System.Data.SqlDbType.VarChar, 64, "objectUID"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@uniqueGroupID", System.Data.SqlDbType.VarChar, 256, "uniqueGroupID"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@displayName", System.Data.SqlDbType.VarChar, 256, "displayName"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@attributeGroupType", System.Data.SqlDbType.VarChar, 256, "attributeGroupType"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@attribute", System.Data.SqlDbType.VarChar, 256, "attribute"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@linkAttribute", System.Data.SqlDbType.VarChar, 256, "linkAttribute"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@linkAttributeKey", System.Data.SqlDbType.VarChar, 256, "linkAttributeKey"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@groupType", System.Data.SqlDbType.Int, 4, "groupType"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@mailEnabled", System.Data.SqlDbType.VarChar, 256, "mailEnabled"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_objectUID", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "objectUID", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_attribute", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "attribute", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_attributeGroupType", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "attributeGroupType", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_displayName", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "displayName", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_groupType", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "groupType", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_linkAttribute", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "linkAttribute", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_linkAttributeKey", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "linkAttributeKey", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_mailEnabled", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "mailEnabled", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_uniqueGroupID", System.Data.SqlDbType.VarChar, 256, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "uniqueGroupID", System.Data.DataRowVersion.Original, Nothing))
        '
        'dsAttributeGroupDefinitions1
        '
        Me.dsAttributeGroupDefinitions1.DataSetName = "dsAttributeGroupDefinitions"
        Me.dsAttributeGroupDefinitions1.Locale = New System.Globalization.CultureInfo("en-US")
        CType(Me.dsAttributeGroupDefinitions1, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Protected WithEvents lblError As System.Web.UI.WebControls.Label
    Protected WithEvents logo As System.Web.UI.WebControls.Image
    Protected WithEvents miisGroupManagementConnectionString As System.Data.SqlClient.SqlConnection
    Protected WithEvents sqldaAttributeGroupDefinitions As System.Data.SqlClient.SqlDataAdapter
    Protected WithEvents dsAttributeGroupDefinitions1 As gp.dsAttributeGroupDefinitions
    Protected WithEvents dgAttributeGroupDefinitions As System.Web.UI.WebControls.DataGrid
    Protected WithEvents btnAddAttributeRow As System.Web.UI.WebControls.Button
    Protected WithEvents SqlSelectCommand1 As System.Data.SqlClient.SqlCommand
    Protected WithEvents SqlInsertCommand1 As System.Data.SqlClient.SqlCommand
    Protected WithEvents SqlUpdateCommand1 As System.Data.SqlClient.SqlCommand
    Protected WithEvents SqlDeleteCommand1 As System.Data.SqlClient.SqlCommand
    Protected WithEvents btnDefineGroups As System.Web.UI.WebControls.Button
    Protected WithEvents miisConnectionString As System.Data.SqlClient.SqlConnection

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

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

            ' Sets the connection string from the xml file that 
            ' is defined in the global.asax file
            Me.miisConnectionString.ConnectionString = _
                    ConfigurationSettings.AppSettings("miisConnectionString")
            Me.miisGroupManagementConnectionString.ConnectionString _
                                    = ConfigurationSettings.AppSettings _
                                    ("miisGroupManagementConnectionString")

            ' This allows the group search to be submitted 
            ' when the enter button is pressed
            Page.RegisterHiddenField("__EVENTTARGET", "btnSrchGroups")

            ' This value is changed from the system generated 
            ' update command so that the where clause only
            ' includes the primary key (objectUID). Also the clause 
            ' is not set because it is handled outside of the datagrid
            Me.SqlUpdateCommand1.CommandText = "UPDATE " + _
                "attributeGroupDefinitions SET objectUID = @objectUID, " + _
                "uniqueGroupID = @uni" & "queGroupID, displayName " + _
                "= @displayName, attributeGroupType = @attributeGroupType" & _
                ", attribute = @attribute, linkAttribute = @linkAttribute" + _
                ", linkAttributeKey = @li" & "nkAttributeKey, " + _
                "groupType = @groupType, mailEnabled = @mailEnabled WHERE " + _
                "(object" & "UID = @Original_objectUID); SELECT objec" & _
                "tUID, uniqueGroupID, displayName, attributeGroupType, " + _
                "attribute, linkAttribute, " & "linkAttributeKey, " + _
                "groupType, mailEnabled FROM attributeGroupDefinitions " + _
                "WHERE (o" & "bjectUID = @objectUID)"

            If IsPostBack Then
                ' Used to cache the datagrid
                dsAttributeGroupDefinitions1 = CType(Session _
                        ("dsAttributeGroups"), dsAttributeGroupDefinitions)
            Else
                dgAttributeGroupDefinitions.DataBind()
                Session("dsAttributeGroups") = dsAttributeGroupDefinitions1

                ' fill the datagrid
                sqldaAttributeGroupDefinitions.Fill _
                                        (dsAttributeGroupDefinitions1)
                dgAttributeGroupDefinitions.CurrentPageIndex = 0
                dgAttributeGroupDefinitions.DataBind()

            End If

        Catch exception As Exception
            HideControls()

            If exception.Message = MSG_NULL_REFERENCE Then
                ' Most likely cause for this error is that the 
                ' SQL connection has timed out
                lblError.Text = "Page_Load: Your session has " + _
                        "timed out or there is another object reference " + _
                            "error, please reconnect."
            Else
                lblError.Text = "Error Page_Load: " + exception.Message
            End If

        End Try
    End Sub


    ' <summary>
    ' Sets the datagrid in edit mode for the selected row
    ' </summary>
    ' <param name="source">Source object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns> 

    Private Sub dgAttributeGroupDefinitions_EditCommand _
            (ByVal source As Object, _
            ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
            Handles dgAttributeGroupDefinitions.EditCommand

        Try

            ' Hide the following columns
            ' Column[10] - delete
            dgAttributeGroupDefinitions.Columns(11).Visible = False

            ' Put the datagrid into edit mode for the row
            ' that the edit button was clicked
            dgAttributeGroupDefinitions.EditItemIndex = e.Item.ItemIndex
            dgAttributeGroupDefinitions.DataBind()

            dgAttributeGroupDefinitions.SelectedIndex = _
                                dgAttributeGroupDefinitions.EditItemIndex

            Dim dropDownList As DropDownList = Nothing

            dropDownList = CType(dgAttributeGroupDefinitions.SelectedItem. _
                                    Cells(4).Controls(1), DropDownList)

            Dim selectedValue As String = dropDownList.SelectedValue.ToLower

            ' If the selected value is linked...
            If selectedValue.ToLower = "linked" Then
                dgAttributeGroupDefinitions.Columns(6).Visible = True
                dgAttributeGroupDefinitions.Columns(7).Visible = True
            Else
                dgAttributeGroupDefinitions.Columns(6).Visible = False
                dgAttributeGroupDefinitions.Columns(7).Visible = False
            End If

            dgAttributeGroupDefinitions.SelectedIndex = -1

        Catch exception As Exception
            HideControls()

            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = _
                "dgAttributeGroupDefinitions_EditCommand: " + MSG_TIMEOUT
            Else
                lblError.Text = "Error dgAttributeGroupDefinitions_ " + _
                                       "EditCommand: " + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Updates the datagrid and removes the datagrid from edit mode
    ' when "Update" button is clicked
    ' </summary>
    ' <param name="source">Source object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub dgAttributeGroupDefinitions_UpdateCommand _
        (ByVal source As Object, _
        ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
        Handles dgAttributeGroupDefinitions.UpdateCommand

        Try

            'Declarations
            Dim objectUID As String = String.Empty
            Dim uniqueGroupID As String = String.Empty
            Dim attributeGroupType As String = String.Empty
            Dim attribute As String = String.Empty
            Dim linkAttribute As String = String.Empty
            Dim linkAttributeKey As String = String.Empty
            Dim displayName As String = String.Empty
            Dim groupType As String = String.Empty
            Dim mailEnabledFlag As String = String.Empty

            ' Gets the value of the key field of the row being updated
            Dim key As String = dgAttributeGroupDefinitions. _
                                DataKeys(e.Item.ItemIndex).ToString()

            'Data Grid column reference

            ' Column[0] - objectType
            ' Column[1] - objectUID
            ' Column[2] - uniqueGroupID
            ' Column[3] - displayName
            ' Column[4] - attributeGroupType
            ' Column[5] - attribute
            ' Column[6] - linkAttribute
            ' Column[7] - linkAttributeKey
            ' Column[8] - groupType
            ' Column[9] - mail
            ' Column[10] - edit/update/cancel
            ' Column[11] - delete

            Dim textBox As TextBox = Nothing
            Dim dropDownList As DropDownList = Nothing

            ' set to the objectUID
            objectUID = e.Item.Cells(1).Text.ToString
            objectUID = objectUID.Trim()
            If objectUID Is Nothing Then

                ' validate the uniqueID not to be null
                Dim strMessage As String = _
                            "uniqueGroupID can not be null."
                'Display alert window to the user
                ThrowAlert(strMessage)
                Exit Sub
            End If
            ' set to the uniqueGroupID
            textBox = _
                CType(e.Item.FindControl("uniqueGroupIDLabelEdit"), TextBox)
            uniqueGroupID = textBox.Text

            'Valid uniqueID string to avoid sql injection
            If (ValidateStringData(uniqueGroupID) = False) Then
                Dim strMessage As String = "Invalid character present in group ID"
                ThrowAlert(strMessage)
                Exit Sub
            End If
            ' set to the displayName
            textBox = _
                CType(e.Item.FindControl("displayNameLabelEdit"), TextBox)
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

            ' If an update is trying to happen without a group name being 
            ' set create a popup message that alerts to the condition
            If displayName = Nothing Then
                Dim strMessage As String = "Group Name cannot be blank."

                'Display alert window to the user
                ThrowAlert(strMessage)

                Exit Sub

                ' if group name is populated
            Else
                miisGroupManagementConnectionString.Open()

                ' select rows from the groupDefinitions table that 
                ' have the same name and if any name is returned then 
                ' alert that the name is not unique in the table
                Dim uniqueGroupIDForSQL As String = Replace _
                                            (uniqueGroupID, "'", "''")
                Dim uniqueGroupIDReader As SqlDataReader = Nothing
                Dim sqlUniqueGroupIDColumn(1) As Object
                Dim uniqueGroupIDQuery As String = "SELECT " + _
                    "uniqueGroupID FROM attributeGroupDefinitions " + _
                        "WHERE uniqueGroupID = '" + uniqueGroupIDForSQL + "' " + _
                            "AND objectUID <> '" + objectUID + "'"
                Dim uniqueGroupIDCommand As SqlCommand = _
                            New SqlCommand(uniqueGroupIDQuery, _
                                    miisGroupManagementConnectionString)
                uniqueGroupIDReader = uniqueGroupIDCommand.ExecuteReader()

                While (uniqueGroupIDReader.Read())
                    uniqueGroupIDReader.GetValues(sqlUniqueGroupIDColumn)

                    If Not uniqueGroupIDReader.IsDBNull(0) Then

                        ' if the group name already exists in the table
                        ' create a popup message that alerts to the condition
                        Dim strMessage As String = _
                                    "uniqueGroupID already exists."
                        'Display alert window to the user
                        ThrowAlert(strMessage)
                        If Not (uniqueGroupIDReader Is Nothing) Then
                            uniqueGroupIDReader.Close()
                        End If

                        Exit Sub

                    End If
                End While

                If Not (uniqueGroupIDReader Is Nothing) Then
                    uniqueGroupIDReader.Close()
                End If

                miisGroupManagementConnectionString.Close()

            End If

            ' set to the attributeGroupType
            dropDownList = CType(e.Item.FindControl _
                    ("attributeGroupTypeDropDownListEdit"), _
                        DropDownList)
            attributeGroupType = dropDownList.SelectedValue

            ' set to the attribute
            dropDownList = _
                CType(e.Item.FindControl("attributeDropDownListEdit"), _
                         DropDownList)
            If Not dropDownList.SelectedValue = "" Then
                attribute = "[" + dropDownList.SelectedValue + "]"
            End If

            ' set to the linkAttribute
            dropDownList = _
                CType(e.Item.FindControl("linkAttributeDropDownListEdit"), _
                         DropDownList)
            If Not dropDownList.SelectedValue = "" Then
                linkAttribute = dropDownList.SelectedValue
            End If

            ' set to the linkAttributeKey
            dropDownList = _
                CType(e.Item.FindControl _
                    ("linkAttributeKeyDropDownListEdit"), DropDownList)
            If Not dropDownList.SelectedValue = "" Then
                linkAttributeKey = "[" + dropDownList.SelectedValue + "]"
            End If

            ' set to the groupType
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

            ' set to the enabledFlag
            Dim mailEnabledImageEdit As ImageButton = CType _
                        (e.Item.FindControl("mailEnabledEdit"), ImageButton)
            If mailEnabledImageEdit.ImageUrl = _
                        "./images/buttonCheck.gif" Then
                mailEnabledFlag = "true"
            Else
                mailEnabledFlag = "false"
            End If

            Dim row As dsAttributeGroupDefinitions.attributeGroupDefinitionsRow
            row = dsAttributeGroupDefinitions1.attributeGroupDefinitions. _
                    FindByobjectUID(key)

            ' Updates the dataset table
            row.uniqueGroupID = uniqueGroupID
            row.displayName = displayName
            row.attributeGroupType = attributeGroupType
            row.attribute = attribute
            row.linkAttribute = linkAttribute
            row.linkAttributeKey = linkAttributeKey
            row.groupType = groupTypeNum
            row.mailEnabled = mailEnabledFlag

            ' Calls a SQL statement to update the database from the dataset
            sqldaAttributeGroupDefinitions.Update(dsAttributeGroupDefinitions1)

            EventLog.WriteEntry("miisGroupManagement", _
                    Environment.UserDomainName.ToLower _
                    + "\" + Environment.UserName.ToLower _
                    + " updated the following properties for " _
                    + "attribute based group definition '" + displayName _
                    + "' with uniqueID '" + uniqueGroupID + "':" + vbCrLf + _
                    vbCrLf + "mailEnabledFlag - '" + mailEnabledFlag + "'" _
                    + vbCrLf + "groupType - '" + groupType + "'", _
                    EventLogEntryType.Information)

            ' Removes the DataGrid row from edit mode
            dgAttributeGroupDefinitions.EditItemIndex = -1

            ' Removes the DataGrid columns from edit mode
            dgAttributeGroupDefinitions.Columns(6).Visible = False
            dgAttributeGroupDefinitions.Columns(7).Visible = False
            dgAttributeGroupDefinitions.Columns(11).Visible = True

            ' Refreshes the grid
            dgAttributeGroupDefinitions.DataBind()

        Catch exception As Exception
            HideControls()

            'Check for null reference message to display cistomized error
            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "dgAttributeGroupDefinitions_UpdateCommand: " _
                                                        + MSG_TIMEOUT
            Else
                lblError.Text = _
                "Error dgAttributeGroupDefinitions_UpdateCommand: " _
                                                        + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Removes the datagrid from Edit mode without saving any changes
    ' </summary>
    ' <param name="source">Source object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub dgAttributeGroupDefinitions_CancelCommand _
            (ByVal source As Object, _
             ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
                           Handles dgAttributeGroupDefinitions.CancelCommand

        Try

            miisGroupManagementConnectionString.Open()

            ' select row from the groupDefinitions table that
            ' has the same objectUID and check to ensure that 
            ' the uniqueGroupID is set to something otherwise
            ' delete the row
            Dim uniqueGroupIDReader As SqlDataReader = Nothing
            Dim sqluniqueGroupIDColumn(2) As Object
            Dim uniqueGroupIDQuery As String = "SELECT objectUID, " + _
                "uniqueGroupID FROM attributeGroupDefinitions WHERE " + _
                "objectUID = '" + e.Item.Cells(1).Text.ToString + "'"
            Dim uniqueGroupIDCommand As SqlCommand = New SqlCommand _
                 (uniqueGroupIDQuery, miisGroupManagementConnectionString)
            uniqueGroupIDReader = uniqueGroupIDCommand.ExecuteReader()

            While (uniqueGroupIDReader.Read())
                uniqueGroupIDReader.GetValues(sqluniqueGroupIDColumn)

                If uniqueGroupIDReader.IsDBNull(1) Or _
                                sqluniqueGroupIDColumn(1).Equals("") Then

                    If Not (uniqueGroupIDReader Is Nothing) Then
                        uniqueGroupIDReader.Close()
                    End If

                    'Close the sql connection
                    miisGroupManagementConnectionString.Close()

                    dgAttributeGroupDefinitions_DeleteCommand(source, e)

                    Exit While

                End If
            End While

            If Not (uniqueGroupIDReader Is Nothing) Then
                uniqueGroupIDReader.Close()
            End If

            'Close the sql connection
            miisGroupManagementConnectionString.Close()

            ' Removes the Datagrid columns from edit mode
            dgAttributeGroupDefinitions.Columns(6).Visible = False
            dgAttributeGroupDefinitions.Columns(7).Visible = False
            dgAttributeGroupDefinitions.Columns(11).Visible = True

            ' Removes the Datagrid rows from edit mode
            dgAttributeGroupDefinitions.EditItemIndex = -1
            dgAttributeGroupDefinitions.DataBind()

        Catch exception As Exception
            HideControls()

            'Check for null reference message to display cistomized error
            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = _
                        "dgAttributeGroupDefinitions_CancelCommand: " _
                                                + MSG_TIMEOUT
            Else
                lblError.Text = _
                        "Error dgAttributeGroupDefinitions_CancelCommand: " _
                                                + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Adds a new row to the Datagrid and hence to the SQL Database and 
    ' then opens the row in edit mode for editing
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub btnAddAttributeRow_Click _
            (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                            Handles btnAddAttributeRow.Click

        Try

            lblError.Visible = False

            ' Add a new row to the datagrid
            Dim dataRow As DataRow = _
                dsAttributeGroupDefinitions1.attributeGroupDefinitions.NewRow

            ' Generate a new GUID for the new group
            Dim genObjectUID As Guid = System.Guid.NewGuid()
            Dim stringObjectUID As String = _
                                    "{" + genObjectUID.ToString.ToUpper + "}"

            ' Set the default values for the new group
            dataRow("objectUID") = stringObjectUID
            dataRow("uniqueGroupID") = ""
            dataRow("attributeGroupType") = "single"
            dataRow("attribute") = ""
            dataRow("linkAttribute") = ""
            dataRow("linkAttributeKey") = ""
            dataRow("displayName") = "{attributeValue}"
            dataRow("groupType") = ADS_GROUP_TYPE_UNIVERSAL_GROUP Or _
                              ADS_GROUP_TYPE_SECURITY_ENABLED
            dataRow("mailEnabled") = "true"

            ' insert the new group row into the table
            dsAttributeGroupDefinitions1.attributeGroupDefinitions. _
                                                    Rows.InsertAt(dataRow, 0)
            ' manage the cache
            Session("dsAttributeGroups") = dsAttributeGroupDefinitions1

            ' update the datagrid
            sqldaAttributeGroupDefinitions.Update _
                                        (dsAttributeGroupDefinitions1)

            ' put the datagrid in edit mode and unhide/hide 
            ' the columns for edit mode
            dgAttributeGroupDefinitions.Columns(11).Visible = False

            dgAttributeGroupDefinitions.EditItemIndex = 0
            dgAttributeGroupDefinitions.CurrentPageIndex = 0
            dgAttributeGroupDefinitions.DataBind()

        Catch exception As Exception
            HideControls()

            'Check for null reference message to display cistomized error
            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "btnAddRow_Click:" + MSG_TIMEOUT
            Else
                lblError.Text = "Error btnAddRow_Click: " + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Deletes the selected row from the datagrid and hence from the 
    ' SQL Database</summary>
    ' <param name="source">Source object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub dgAttributeGroupDefinitions_DeleteCommand _
             (ByVal source As Object, _
              ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
                           Handles dgAttributeGroupDefinitions.DeleteCommand

        Try

            Dim index As Integer = 0

            ' Get the value of the row that is going to be deleted
            ' and set it to the index.
            ' Note:
            ' Currently there are 10 rows per page allowed so that 
            ' multiplying the page number by 10 and adding the value 
            ' of the index on that page to get the value of the row 
            ' that's going to deleted. For example if we are on page 3 
            ' which has the page index of 2 and we are deleting the third 
            ' row,then we need to set the index to 3+(2*10) to get 23
            If Not dgAttributeGroupDefinitions.CurrentPageIndex = 0 Then
                index = e.Item.ItemIndex + _
                        (dgAttributeGroupDefinitions.CurrentPageIndex * 10)
            Else
                index = e.Item.ItemIndex
            End If

            ' Delete the row
            dsAttributeGroupDefinitions1.attributeGroupDefinitions. _
                                                        Rows(index).Delete()
            sqldaAttributeGroupDefinitions. _
                    Update(dsAttributeGroupDefinitions1)

            ' Bind back to the first page in case the row that is  
            ' deleted is the only row on that page
            dgAttributeGroupDefinitions.CurrentPageIndex = 0
            dgAttributeGroupDefinitions.DataBind()

        Catch exception As Exception
            HideControls()

            'Check for null reference message to display cistomized error
            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "dgAttributeGroupDefinitions_DeleteCommand: " _
                                                        + MSG_TIMEOUT
            Else
                lblError.Text = _
                    "Error dgAttributeGroupDefinitions_DeleteCommand: " _
                                                        + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Controls behaviors of the data grid such as the popup when 
    ' the delete button is pressed and selecting the value 
    ' of the dropdown in edit mode
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub dgAttributeGroupDefinitions_ItemDataBound _
            (ByVal sender As Object, _
            ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) _
                            Handles dgAttributeGroupDefinitions.ItemDataBound

        Try
            Dim button As Button = Nothing

            ' This triggers the confirm delete popup 
            ' when the delete button is clicked
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType _
                                        = ListItemType.AlternatingItem Then
                button = _
                    CType(e.Item.Cells(11).FindControl("btnDelete"), Button)
                button.Attributes.Add("onclick", "return confirm_delete();")
            End If

            ' This changes the value of the grouptype dropdownlist 
            ' to show the currently set value
            If e.Item.ItemType = ListItemType.EditItem Then
                Dim drvGroupType As DataRowView = _
                    CType(e.Item.DataItem, DataRowView)
                Dim currentGroupTypeNum As String = _
                    CType(drvGroupType("groupType"), String)
                Dim currentGroupType As String = String.Empty

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
                        Throw New Exception("The selected groupType" + _
                                                            " was invalid.")
                End Select

                Dim ddlGroupType As DropDownList = Nothing
                ddlGroupType = CType(e.Item.FindControl _
                                    ("groupTypeDropDownList"), DropDownList)
                ddlGroupType.SelectedIndex = ddlGroupType.Items.IndexOf _
                            (ddlGroupType.Items.FindByText(currentGroupType))
            End If

            ' This changes the value of the attributeGroupType dropdownlist
            ' to show the currently set value
            If e.Item.ItemType = ListItemType.EditItem Then
                Dim datarowView As DataRowView = CType _
                                            (e.Item.DataItem, DataRowView)
                Dim currentValue As String = _
                        CType(datarowView("attributeGroupType"), String)

                Dim dropDownList As DropDownList
                dropDownList = CType(e.Item.FindControl _
                        ("attributeGroupTypeDropDownListEdit"), DropDownList)
                dropDownList.SelectedIndex = _
                    dropDownList.Items.IndexOf _
                        (dropDownList.Items.FindByText(currentValue.tolower))
            End If

            ' Populates the attribute dropDownList with the column 
            ' values from the metaverse database
            If e.Item.ItemType = ListItemType.EditItem Then

                Dim dropDownList As DropDownList = Nothing

                ' set to the attribute
                dropDownList = CType(e.Item.FindControl _
                                ("attributeDropDownListEdit"), DropDownList)

                miisConnectionString.Open()

                ' Selects the column names from the metaverse and 
                ' populates the srchAttrib dropdown box
                Dim columnSelectQuery As String = _
                            "SET ROWCOUNT 0 SELECT syscolumns.name " _
                            + " FROM syscolumns WITH (NOLOCK) WHERE id IN " _
                            + "(SELECT sysobjects.id FROM sysobjects " _
                            + "WHERE name = 'mms_metaverse') " _
                            + "ORDER BY syscolumns.name"
                Dim columnReader As SqlDataReader = Nothing
                Dim sqlColumnColumn(2) As Object
                Dim columnCommand As SqlCommand = _
                    New SqlCommand(columnSelectQuery, miisConnectionString)
                columnReader = columnCommand.ExecuteReader()

                While (columnReader.Read())

                    columnReader.GetValues(sqlColumnColumn)

                    ' Check if the clause in not null or ""
                    If Not columnReader.IsDBNull(0) AndAlso Not _
                            sqlColumnColumn(0).Equals("") Then
                        Dim NewItem As ListItem = Nothing

                        ' Populate the dropdown box with the 
                        ' column names from the MV
                        NewItem = New ListItem(sqlColumnColumn(0). _
                            ToString().Trim().ToLower, sqlColumnColumn(0). _
                                                  ToString().Trim().ToLower)
                        dropDownList.Items.Add(NewItem)
                    End If

                End While

                miisConnectionString.Close()

            End If

            ' This changes the value of the attribute dropdownlist 
            ' to show the currently set value
            If e.Item.ItemType = ListItemType.EditItem Then
                Dim datarowView As DataRowView = CType _
                                        (e.Item.DataItem, DataRowView)
                Dim currentValue As String = CType _
                                    (datarowView("attribute"), String)

                currentValue = currentValue.TrimStart("[")
                currentValue = currentValue.TrimEnd("]")

                Dim dropDownList As DropDownList = Nothing
                dropDownList = CType(e.Item.FindControl _
                                ("attributeDropDownListEdit"), DropDownList)
                dropDownList.SelectedIndex = dropDownList.Items.IndexOf _
                                (dropDownList.Items.FindByText(currentValue))
            End If

            ' Populates the linkAttribute dropDownList with the column
            ' values from the metaverse database
            If e.Item.ItemType = ListItemType.EditItem Then

                Dim dropDownList As DropDownList = Nothing

                ' set to the linkAttribute
                dropDownList = CType(e.Item.FindControl _
                            ("linkAttributeDropDownListEdit"), DropDownList)

                miisConnectionString.Open()

                ' Selects the column names from the metaverse and 
                ' populates the srchAttrib dropdown box
                Dim columnSelectQuery As String = _
                    "SET ROWCOUNT 0 SELECT DISTINCT attribute_name " _
                    + "FROM mms_mv_link WITH (NOLOCK)"
                Dim columnReader As SqlDataReader = Nothing
                Dim sqlColumnColumn(2) As Object
                Dim columnCommand As SqlCommand = _
                    New SqlCommand(columnSelectQuery, miisConnectionString)
                columnReader = columnCommand.ExecuteReader()

                While (columnReader.Read())

                    columnReader.GetValues(sqlColumnColumn)

                    ' Check if the clause in not null or ""
                    If Not columnReader.IsDBNull(0) AndAlso Not _
                            sqlColumnColumn(0).Equals("") Then
                        Dim NewItem As ListItem = Nothing
                        ' Populate the dropdown box with the column 
                        ' names from the MV
                        NewItem = New ListItem(sqlColumnColumn(0). _
                            ToString().Trim().ToLower, sqlColumnColumn(0). _
                                                  ToString().Trim().ToLower)
                        dropDownList.Items.Add(NewItem)
                    End If

                End While

                miisConnectionString.Close()

            End If

            ' This changes the value of the linkAttribute dropdownlist 
            ' to show the currently set value
            If e.Item.ItemType = ListItemType.EditItem Then
                Dim datarowView As DataRowView = CType _
                                            (e.Item.DataItem, DataRowView)
                Dim currentValue As String = CType _
                                    (datarowView("linkAttribute"), String)

                currentValue = currentValue.TrimStart("[")
                currentValue = currentValue.TrimEnd("]")

                Dim dropDownList As DropDownList = Nothing
                dropDownList = CType(e.Item.FindControl _
                            ("linkAttributeDropDownListEdit"), DropDownList)
                dropDownList.SelectedIndex = dropDownList.Items. _
                        IndexOf(dropDownList.Items.FindByText(currentValue))
            End If

            ' Populates the linkAttributeKey dropDownList with the 
            ' column values from the metaverse database
            If e.Item.ItemType = ListItemType.EditItem Then

                Dim dropDownList As DropDownList = Nothing

                ' set to the linkAttribute
                dropDownList = CType(e.Item.FindControl _
                        ("linkAttributeKeyDropDownListEdit"), DropDownList)

                miisConnectionString.Open()

                ' Selects the column names from the metaverse and 
                ' populates the srchAttrib dropdown box
                Dim columnSelectQuery As String = _
                    "SET ROWCOUNT 0 SELECT syscolumns.name FROM syscolumns " _
                    + " WITH (NOLOCK) WHERE id IN (SELECT sysobjects.id " _
                    + "FROM sysobjects WHERE name = 'mms_metaverse') " _
                    + "ORDER BY syscolumns.name"
                Dim columnReader As SqlDataReader = Nothing
                Dim sqlColumnColumn(2) As Object
                Dim columnCommand As SqlCommand = _
                    New SqlCommand(columnSelectQuery, miisConnectionString)
                columnReader = columnCommand.ExecuteReader()

                While (columnReader.Read())

                    columnReader.GetValues(sqlColumnColumn)

                    ' Check if the clause in not null or ""
                    If Not columnReader.IsDBNull(0) AndAlso Not _
                            sqlColumnColumn(0).Equals("") Then
                        Dim NewItem As ListItem = Nothing

                        ' Populate the dropdown box with the 
                        ' column names from the MV
                        NewItem = New ListItem(sqlColumnColumn(0). _
                            ToString().Trim().ToLower, sqlColumnColumn(0). _
                                                    ToString().Trim().ToLower)
                        dropDownList.Items.Add(NewItem)
                    End If

                End While

                miisConnectionString.Close()

            End If

            ' This changes the value of the linkAttributeKey dropdownlist 
            ' to show the currently set value
            If e.Item.ItemType = ListItemType.EditItem Then
                Dim datarowView As DataRowView = CType _
                                            (e.Item.DataItem, DataRowView)
                Dim currentValue As String = CType _
                                (datarowView("linkAttributeKey"), String)

                currentValue = currentValue.TrimStart("[")
                currentValue = currentValue.TrimEnd("]")

                Dim dropDownList As DropDownList = Nothing
                dropDownList = CType(e.Item.FindControl _
                        ("linkAttributeKeyDropDownListEdit"), DropDownList)
                dropDownList.SelectedIndex = dropDownList.Items. _
                        IndexOf(dropDownList.Items.FindByText(currentValue))
            End If

        Catch exception As Exception
            HideControls()

            'Check for null reference message to display cistomized error
            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = _
                    "dgAttributeGroupDefinitions_ItemDataBound: " + MSG_TIMEOUT
            Else
                lblError.Text = _
                    "Error dgAttributeGroupDefinitions_ItemDataBound: " _
                                + exception.Message
            End If
            'Ensure that the database connection is closed before method exit
        Finally
            If Not miisConnectionString Is Nothing Then
                If Not (miisConnectionString.State = ConnectionState.Closed) Then
                    miisConnectionString.Close()
                End If
            End If
        End Try

    End Sub

    ' <summary>
    ' Implements the check box toggling for attribute 'Mail Enabled' selection
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub dgAttributeGroupDefinitions_ItemCommand1 _
            (ByVal source As Object, _
             ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
                              Handles dgAttributeGroupDefinitions.ItemCommand

        Try

            ' If the checkButton is clicked under clause, then toggle it
            If (e.CommandName = "mailEnabledEdit") Then

                Dim mailEnabledImageEdit As ImageButton = _
                    CType(e.Item.FindControl("mailEnabledEdit"), ImageButton)

                If mailEnabledImageEdit.ImageUrl = _
                                        "./images/buttonCheck.gif" Then
                    mailEnabledImageEdit.ImageUrl = "./images/button.gif"
                Else
                    mailEnabledImageEdit.ImageUrl = "./images/buttonCheck.gif"
                End If

            End If

        Catch exception As Exception
            HideControls()
            'Check for null reference message to display cistomized error
            If exception.Message = MSG_NULL_REFERENCE Then
                lblError.Text = "dgAttributeGroupDefinitions_ItemCommand1: " _
                                                        + MSG_TIMEOUT
            Else
                lblError.Text = _
                        "Error dgAttributeGroupDefinitions_ItemCommand1: " _
                                                         + exception.Message
            End If

        End Try

    End Sub

    ' <summary>
    ' Converts the groupType into a friendly Group name
    ' </summary>
    ' <param name="groupTypeNum">Group type name</param>
    ' <returns>String</returns>

    Function GetGroupTypeName(ByVal groupTypeNum As String) As String
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
    ' Determines the status of the checkbox control which
    ' indicates whether a group is mail enabled
    ' </summary>
    ' <param name="mailEnabled">Indicates the group whose
    ' status needs to be checked</param>
    ' <returns>String</returns>

    Function GetMailEnabledStatus(ByVal mailEnabled As String) As String
        Select Case mailEnabled.ToLower
            Case "true"
                Return "./images/check.gif"
            Case Else
                Return "./images/blank.gif"
        End Select
    End Function


    ' <summary>
    ' Determines the status of the checkbox control which 
    ' indicates whether a group is mail enabled (on edit)
    ' </summary>
    ' <param name="mailEnabled">Indicates the group whose
    ' status needs to be checked</param>
    ' <returns>String</returns>

    Function GetMailEnabledStatusEdit(ByVal mailEnabled As String) As String
        Select Case mailEnabled.ToLower
            Case "true"
                Return "./images/buttonCheck.gif"
            Case Else
                Return "./images/button.gif"
        End Select
    End Function

    ' <summary>
    ' Truncates the display name field to fit the length of visibility
    ' </summary>
    ' <param name="displayName">Display Name of the Group</param>
    ' <returns>String</returns>

    Function GetDisplayName(ByVal displayName As String) As String
        If displayName.Length > 23 Then
            displayName = Left(displayName, 20) + "..."
            Return displayName
        Else
            Return displayName
        End If

    End Function

    ' <summary>
    ' Trims the attribute field so that it does not display the brackets ([])
    ' </summary>
    ' <param name="attribute">Atribute for the Group Definition</param>
    ' <returns>String</returns>

    Function GetAttribute(ByVal attribute As String) As String
        attribute = attribute.TrimStart("[")
        attribute = attribute.TrimEnd("]")

        Return attribute
        
    End Function

    ' <summary>
    ' Trims the linkAttribute field so that it does not display the brackets ([])
    ' </summary>
    ' <param name="linkAttribute">linkAtribute for the Group Definition</param>
    ' <returns>String</returns>

    Function GetLinkAttribute(ByVal linkAttribute As String) As String
        linkAttribute = linkAttribute.TrimStart("[")
        linkAttribute = linkAttribute.TrimEnd("]")

        Return linkAttribute

    End Function

    ' <summary>
    ' Trims the linkAttributeKey field so that it does not display the brackets ([])
    ' </summary>
    ' <param name="linkAttributeKey">linkAtribute for the Group Definition</param>
    ' <returns>String</returns>

    Function GetLinkAttributeKey(ByVal linkAttributeKey As String) As String
        linkAttributeKey = linkAttributeKey.TrimStart("[")
        linkAttributeKey = linkAttributeKey.TrimEnd("]")

        Return linkAttributeKey

    End Function

    ' <summary>
    ' Determines which columns to be shown based on the 
    ' selection of the defType dropdown list
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Sub SelectionChange(ByVal sender As Object, ByVal e As EventArgs)

        Try
            dgAttributeGroupDefinitions.SelectedIndex = _
                dgAttributeGroupDefinitions.EditItemIndex

            Dim dropDownList As DropDownList = Nothing

            dropDownList = CType(dgAttributeGroupDefinitions.SelectedItem. _
                    Cells(4).Controls(1), DropDownList)

            Dim selectedValue As String = dropDownList.SelectedValue.ToLower

            ' If the selected value is linked then...
            If selectedValue.ToLower = "linked" Then
                dgAttributeGroupDefinitions.Columns(6).Visible = True
                dgAttributeGroupDefinitions.Columns(7).Visible = True
            Else
                dgAttributeGroupDefinitions.Columns(6).Visible = False
                dgAttributeGroupDefinitions.Columns(7).Visible = False
            End If

            dgAttributeGroupDefinitions.SelectedIndex = -1

        Catch exception As Exception
            lblError.Text = "Error: " + exception.Message
        End Try

    End Sub

    ' <summary>
    ' Loads the default.aspx page
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param>
    ' <returns></returns> 

    Private Sub btnDefineGroups_Click _
        (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                         Handles btnDefineGroups.Click
        Server.Transfer("default.aspx", False)
    End Sub

    ' <summary>
    ' Hides the active controls for group definition and
    ' attirbute row addition upon any exception or error.
    ' Sets the error lable Visible property to true.
    ' </summary>

    Private Sub HideControls()

        dgAttributeGroupDefinitions.Visible = False
        btnAddAttributeRow.Visible = False
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