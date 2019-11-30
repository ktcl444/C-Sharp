Imports System.Configuration
Imports System.Data.SqlClient
Imports AppLauncherDataVB

Public Class _default
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub
  Protected WithEvents lblMessage As System.Web.UI.WebControls.Label
  Protected WithEvents grdApps As System.Web.UI.WebControls.DataGrid
  Protected WithEvents lblLogin As System.Web.UI.WebControls.Label
  Protected WithEvents Label1 As System.Web.UI.WebControls.Label

  'NOTE: The following placeholder declaration is required by the Web Form Designer.
  'Do not delete or move it.
  Private designerPlaceholderDeclaration As System.Object

  Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
    'CODEGEN: This method call is required by the Web Form Designer
    'Do not modify it using the code editor.
    InitializeComponent()
  End Sub

#End Region

  Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    ' Display the User Name
    ' Without the Domain prefix
    lblLogin.Text = "Applications Available for: " & _
      Apps.LoginIDNoDomain(User.Identity.Name)

    AppLoad()
  End Sub

  Private Sub AppLoad()
    Dim app As New Apps

    Try
      ' Load applications for this user
      grdApps.DataSource = app.GetAppsByLoginID(User.Identity.Name)
      grdApps.DataBind()

    Catch ex As Exception
      lblMessage.Text = ex.Message
    End Try

  End Sub

  Private Sub grdApps_ItemCommand(ByVal source As Object, _
    ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
    Handles grdApps.ItemCommand
    Dim app As New Apps
    Dim boolRedirect As Boolean
    Dim token As String
    Dim lb As LinkButton

    Try
      lb = DirectCast(e.Item.Cells(0).Controls(1), LinkButton)

      ' Create a Token for this user/app
      token = app.CreateLoginToken(lb.Text, _
        User.Identity.Name, _
        Convert.ToInt32(lb.Attributes("UserID")), _
        Convert.ToInt32(lb.Attributes("AppID")))

      boolRedirect = True

    Catch ex As Exception
      boolRedirect = False
      lblMessage.Text = ex.Message

    End Try

    If boolRedirect Then
      ' Redirect to web application 
      ' passing in the generated token
      Response.Redirect(e.CommandArgument.ToString() & _
        "?Token=" & token, False)
    End If
  End Sub
End Class
