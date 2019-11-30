'/////////////////////////////////////////////////////////////////////////////
' Microsoft Solutions for Security
' Copyright (c) 2004 Microsoft Corporation. All rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
' THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
' AND INFORMATION REMAINS WITH THE USER.
'////////////////////////////////////////////////////////////////////////////

Imports System.Data.SqlClient

' <Summary>
' Enables to have a preview of the resultant of clause definition.
' <Summary/>

Public Class PreviewClause
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    ' This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> _
                        Private Sub InitializeComponent()
        Me.miisConnectionString = New System.Data.SqlClient.SqlConnection
        Me.miisGroupManagementConnectionString = New System.Data.SqlClient.SqlConnection

    End Sub
    Protected WithEvents btnClosePreview As System.Web.UI.WebControls.Button
    Protected WithEvents lblGroupName As System.Web.UI.WebControls.Label
    Protected WithEvents miisConnectionString _
                    As System.Data.SqlClient.SqlConnection
    Protected WithEvents lblError As System.Web.UI.WebControls.Label
    Protected WithEvents lblNote As System.Web.UI.WebControls.Label
    Protected WithEvents miisGroupManagementConnectionString _
                    As System.Data.SqlClient.SqlConnection
    Protected WithEvents tbPreview As System.Web.UI.WebControls.Table
    Protected WithEvents lblTotal As System.Web.UI.WebControls.Label

    ' NOTE: The following placeholder declaration is required 
    ' by the Web Form Designer.
    ' Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init _
        (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                Handles MyBase.Init
        ' CODEGEN: This method call is required by the Web Form Designer
        ' Do not modify it using the code editor.
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
            (ByVal sender As System.Object, _
                    ByVal e As System.EventArgs) Handles MyBase.Load
        Try

            ' Get the values that are being passed in on the URL.These 
            ' values are used to know which group is being worked upon
            Dim sqlClause As String = _
                    Request.QueryString("sqlClause").ToString()
            Dim groupDisplayName As String = _
                    Request.QueryString("groupDisplayName").ToString()
            Dim groupObjectUID As String = _
                    Request.QueryString("groupObjectUID").ToString()

            ' Get the connection string from the global.asax file
            ' which gets them from the XML file
            Me.miisConnectionString.ConnectionString = _
            ConfigurationSettings.AppSettings _
                                        ("miisConnectionString")
            Me.miisGroupManagementConnectionString.ConnectionString = _
            ConfigurationSettings.AppSettings _
                                ("miisGroupManagementConnectionString")

            If Not groupDisplayName = "" Then
                lblGroupName.Text = groupDisplayName + " member preview"
            End If

            Dim excpetionsPresent As Boolean = _
                    GetExceptionStatus(groupObjectUID)

            If excpetionsPresent = True Then
                lblNote.Visible = True
            End If

            miisConnectionString.Open()

            ' Open a new reader for the MIIS database
            Dim memberReader As SqlDataReader = Nothing

            ' Ensure that only person objects is returned
            Dim whereSuffix As String = ") and object_type = 'person' "

            ' Select the list of person objects from the metaverse 
            ' that match the whereClause created earlier
            ' To be safe make sure that the queries against 
            ' the metaverse are executed with nolock
            Dim memberSelectStatement As String = _
                "SELECT displayName, object_id FROM mms_metaverse " _
                + "WITH (NOLOCK) WHERE (" + sqlClause + whereSuffix
            Dim memberCommand As SqlCommand = _
                New SqlCommand(memberSelectStatement, miisConnectionString)

            Dim sqlColumn(2) As Object

            ' sqlColumn object will contain the following values
            ' uid (0)
            ' object_id (1)
            ' Empty (2)

            ' Execute sql command
            memberReader = memberCommand.ExecuteReader()

            Dim total As Integer = 0

            ' This while loop will be executed for each member 
            ' that is returned (includes exceptions)
            While (memberReader.Read())
                memberReader.GetValues(sqlColumn)

                ' Check for object_id and displayName not being null
                ' or "" and add to arraylist
                If Not memberReader.IsDBNull(0) _
                AndAlso Not sqlColumn(0).Equals("") _
                AndAlso Not memberReader.IsDBNull(1) _
                AndAlso Not sqlColumn(1).Equals("") Then

                    Dim memberRow As New TableRow
                    Dim displayNameCell As New TableCell
                    Dim mvObjectUIDCell As New TableCell
                    displayNameCell.Text = sqlColumn(0).ToString().Trim()
                    mvObjectUIDCell.Text = _
                        "{" + sqlColumn(1).ToString().ToUpper.Trim() + "}"
                    memberRow.Cells.Add(displayNameCell)
                    memberRow.Cells.Add(mvObjectUIDCell)
                    tbPreview.Rows.Add(memberRow)

                    total = total + 1

                End If

            End While

            lblTotal.Text = "Total - " + total.ToString

        Catch Exception As Exception
            tbPreview.Visible = False
            lblError.Text = "Error Page_Load: " + Exception.Message
            'Make sure that you close the connection before exit
        Finally
            If Not miisConnectionString Is Nothing Then
                miisConnectionString.Close()
            End If
        End Try
    End Sub

    ' <summary>
    ' To close the preview windows, and load the details to the 
    ' previous parent window
    ' </summary>
    ' <param name="sender">Sender object</param>
    ' <param name="e">Event Arguments</param> 
    ' <returns></returns>

    Private Sub btnClosePreview_Click _
            (ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                            Handles btnClosePreview.Click

        Try
            Dim sqlClause As String = _
                Request.QueryString("sqlClause").ToString()
            Dim groupDisplayName As String = _
                Request.QueryString("groupDisplayName").ToString()
            Dim groupObjectUID As String = _
                Request.QueryString("groupObjectUID").ToString()

            ' Transfer back to the buildExcept page in this window
            Server.Transfer("buildClause.aspx?groupObjectUID=" _
                + groupObjectUID + "&groupDisplayName=" + _
                groupDisplayName + "&sqlClause=" + Server.UrlEncode(sqlClause), True)

        Catch exception As Exception
            tbPreview.Visible = False
            lblError.Text = "Error btnClosePreview_Click: " + _
                                                    exception.Message
        End Try

    End Sub

    ' <summary>
    ' Determines the status of the checkbox control which
    ' indicates whether a group is mail enabled
    ' </summary>
    ' <returns></returns>

    Function GetExceptionStatus _
                (ByVal objectUID As String) As String

        Try
            miisGroupManagementConnectionString.Open()

            ' select rows from the groupDefinitions table that have the 
            ' same(name) and if any name is returned then alert that 
            ' the name is not unique in the table
            Dim exceptionReader As SqlDataReader = Nothing
            Dim exceptionQuery As String = _
            "SELECT objectUID FROM exceptionDefinitions " _
                + "WHERE objectUID = '" + objectUID + "'"
            Dim exceptionCommand As SqlCommand = _
                New SqlCommand(exceptionQuery, _
                        miisGroupManagementConnectionString)
            exceptionReader = exceptionCommand.ExecuteReader()

            ' Check to see if there are any exceptions and
            ' return true if any exceptions are present.
            If exceptionReader.HasRows Then
                If Not (exceptionReader Is Nothing) Then
                    exceptionReader.Close()
                End If

                miisGroupManagementConnectionString.Close()
                Return True

            Else
                If Not (exceptionReader Is Nothing) Then
                    exceptionReader.Close()
                End If

                miisGroupManagementConnectionString.Close()
                Return False

            End If

        Catch exception As Exception
            lblError.Text = "Error : " + exception.Message
        Finally
            If Not miisGroupManagementConnectionString Is Nothing Then
                If Not (miisGroupManagementConnectionString.State = ConnectionState.Closed) Then
                    miisGroupManagementConnectionString.Close()
                End If
            End If
        End Try

    End Function

End Class
