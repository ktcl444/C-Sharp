Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration

Public Class AppUserRoles
  Private mConnectString As String

  Public Sub New()
    mConnectString = ConfigurationSettings.AppSettings("eSecurityConnectString")
  End Sub

  Public Function GetLoginKey(ByVal LoginID As String, ByVal AppKey As Integer) As Integer
    Dim ds As New DataSet
    Dim cmd As SqlCommand
    Dim da As SqlDataAdapter
    Dim sql As String
    Dim retValue As Integer

    sql = "SELECT esUsers.iUserID "
    sql &= " FROM esAppsUsers "
    sql &= " INNER JOIN esUsers "
    sql &= " ON esUsers.iUserID = esAppsUsers.iUserID "
    sql &= " WHERE esUsers.sLoginID = @sLoginID "
    sql &= " AND esAppsUsers.iAppID = @iAppID "

    Try
      cmd = New SqlCommand(sql)
      cmd.Connection = New SqlConnection(mConnectString)

      cmd.Parameters.Add(New SqlParameter("@sLoginID", SqlDbType.Char))
      cmd.Parameters("@sLoginID").Value = LoginID

      cmd.Parameters.Add(New SqlParameter("@iAppID", SqlDbType.Int))
      cmd.Parameters("@iAppID").Value = AppKey

      da = New SqlDataAdapter(cmd)

      da.Fill(ds)

      retValue = Convert.ToInt32(ds.Tables(0).Rows(0)("iUserID"))

    Catch ex As Exception
      Throw ex
    End Try

    Return retValue
  End Function

  Public Function IsLoginValid(ByVal LoginID As String, ByVal AppKey As Integer) As Boolean
    Dim ds As New DataSet
    Dim cmd As SqlCommand
    Dim da As SqlDataAdapter
    Dim sql As String
    Dim retValue As Boolean

    sql = "SELECT Count(*) As TotalRows "
    sql &= " FROM esAppsUsers "
    sql &= " INNER JOIN esUsers "
    sql &= " ON esUsers.iUserID = esAppsUsers.iUserID "
    sql &= " WHERE esUsers.sLoginID = @sLoginID "
    sql &= " AND esAppsUsers.iAppID = @iAppID "

    Try
      cmd = New SqlCommand(sql)
      cmd.Connection = New SqlConnection(mConnectString)

      cmd.Parameters.Add(New SqlParameter("@sLoginID", SqlDbType.Char))
      cmd.Parameters("@sLoginID").Value = LoginID

      cmd.Parameters.Add(New SqlParameter("@iAppID", SqlDbType.Int))
      cmd.Parameters("@iAppID").Value = AppKey

      da = New SqlDataAdapter(cmd)
      da.Fill(ds)

      If Convert.ToInt32(ds.Tables(0).Rows(0)("TotalRows")) > 0 Then
        retValue = True
      End If

    Catch ex As Exception
      Throw ex

    End Try

    Return retValue
  End Function

  Public Function GetUserRoles(ByVal AppID As Integer, ByVal UserID As Integer) As String()
    Dim ds As New DataSet
    Dim cmd As SqlCommand
    Dim da As SqlDataAdapter
    Dim sql As String
    Dim aRoles As String()
    Dim dr As DataRow
    Dim i As Integer

    sql = "SELECT sRole "
    sql &= " FROM esAppUserRoles "
    sql &= " INNER JOIN esAppRoles "
    sql &= " ON esAppUserRoles.iAppRoleID = esAppRoles.iAppRoleID "
    sql &= " WHERE esAppUserRoles.iAppID = @iAppID "
    sql &= " AND esAppUserRoles.iUserID = @iUserID "

    Try
      cmd = New SqlCommand(sql)
      cmd.Connection = New SqlConnection(mConnectString)

      cmd.Parameters.Add(New SqlParameter("@iAppID", SqlDbType.Int))
      cmd.Parameters("@iAppID").Value = AppID

      cmd.Parameters.Add(New SqlParameter("@iUserID", SqlDbType.Int))
      cmd.Parameters("@iUserID").Value = UserID

      da = New SqlDataAdapter(cmd)
      da.Fill(ds)

      If ds.Tables(0).Rows.Count > 0 Then
        ReDim aRoles(ds.Tables(0).Rows.Count)
        For Each dr In ds.Tables(0).Rows
          aRoles(i) = dr("sRole").ToString()
          i += 1
        Next
      End If

    Catch ex As Exception
      Throw ex
    End Try

    Return aRoles
  End Function
End Class
