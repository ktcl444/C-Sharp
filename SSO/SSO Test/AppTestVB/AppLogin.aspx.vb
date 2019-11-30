Imports AppLauncherDataVB
Imports System.Web.Security

Public Class AppLogin
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub

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
    VerifyToken()
  End Sub

  Private Sub VerifyToken()
    Dim app As New Apps
    Dim al As AppToken

    Try
      al = app.VerifyLoginToken( _
        Request.QueryString("Token").ToString())

      If al.LoginID.Trim() = "" Then
        ' Not a valid login
        ' Redirect them to default page 
        ' This will put them at the login page
        Response.Redirect("default.aspx")
      Else
        ' Create a Forms Authentication Cookie
        ' Set Forms authentication variables
        FormsAuthentication.Initialize()
        FormsAuthentication.SetAuthCookie( _
          al.LoginID.ToString(), False)

        ' Set the Application Token Object
        Application("AppToken") = al

        ' Redirect to Default page
        Response.Redirect("default.aspx")
      End If

    Catch
      ' Redirect them to the login page via the Default page
      Response.Redirect("default.aspx")
    End Try
  End Sub
End Class
