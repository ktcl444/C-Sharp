Imports System.Web
Imports System.Web.SessionState
Imports AppLauncherDataVB
Imports System.Security.Principal

Public Class Global
  Inherits System.Web.HttpApplication

#Region " Component Designer Generated Code "

  Public Sub New()
    MyBase.New()

    'This call is required by the Component Designer.
    InitializeComponent()

    'Add any initialization after the InitializeComponent() call

  End Sub

  'Required by the Component Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Component Designer
  'It can be modified using the Component Designer.
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    components = New System.ComponentModel.Container
  End Sub

#End Region

  Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
    ' Fires when the application is started
  End Sub

  Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
    ' Fires when the session is started
  End Sub

  Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
    ' Fires at the beginning of each request
  End Sub

  Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
    If Request.IsAuthenticated Then
      RoleBuild()
    End If
  End Sub

  Private Sub RoleBuild()
    Dim usr As AppUserRoles
    Dim al As AppToken
    Dim astrRoles As String()
    Dim gi As GenericIdentity
    Dim gp As GenericPrincipal

    Try
      usr = New AppUserRoles
      al = DirectCast(Application("AppToken"), AppToken)

      astrRoles = usr.GetUserRoles( _
        al.AppKey, al.LoginKey)

      gi = New GenericIdentity(User.Identity.Name)
      gp = New GenericPrincipal(gi, astrRoles)

      Context.User = gp

    Catch ex As Exception
      Throw ex
    End Try
  End Sub

  Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
    ' Fires when an error occurs
  End Sub

  Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
    ' Fires when the session ends
  End Sub

  Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
    ' Fires when the application ends
  End Sub

End Class
