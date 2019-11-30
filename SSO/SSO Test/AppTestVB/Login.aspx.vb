Imports AppLauncherDataVB
Imports System.Web.Security
Imports System.Configuration

Public Class Login
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub
  Protected WithEvents btnSignIn As System.Web.UI.WebControls.Button
  Protected WithEvents txtLogin As System.Web.UI.WebControls.TextBox
  Protected WithEvents lblLogin As System.Web.UI.WebControls.Label

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
    'Put user code to initialize the page here
  End Sub

  Private Sub btnSignIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSignIn.Click
    VerifyUser(txtLogin.Text)
  End Sub

  Private Sub VerifyUser(ByVal LoginID As String)
    Dim al As New AppToken
    Dim app As New AppUserRoles

    Try
      If app.IsLoginValid(LoginID, _
        Convert.ToInt32(ConfigurationSettings. _
        AppSettings("eSecurityAppID"))) Then

        ' Retrieve Login Key
        al.LoginKey = app.GetLoginKey(LoginID, _
          Convert.ToInt32(ConfigurationSettings. _
          AppSettings("eSecurityAppID")))

        ' Load up AppToken Object
        al.LoginID = LoginID
        al.AppKey = Convert.ToInt32(ConfigurationSettings. _
              AppSettings("eSecurityAppID"))
        al.AppName = ConfigurationSettings. _
            AppSettings("eSecurityAppName")

        ' Set the Application Token Object
        Application("AppToken") = al

        ' Create a Forms Authentication Cookie
        ' Set Forms authentication variables
        FormsAuthentication.RedirectFromLoginPage( _
          al.LoginID.ToString(), False)
      Else
        ' Not a valid login
        ' Redirect them to the login page via the Default page
        Response.Redirect("default.aspx")
      End If

    Catch
      ' Redirect them to the login page via the Default page
      Response.Redirect("default.aspx")
    End Try
  End Sub
End Class
