Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Diagnostics

Public Class Apps
  Private mConnectString As String

  Public Sub New()
    mConnectString = ConfigurationSettings. _
      AppSettings("eSecurityConnectString")
  End Sub

  Public Function CreateLoginToken(ByVal AppName As String, _
    ByVal LoginID As String, ByVal UserID As Integer, _
    ByVal AppID As Integer) As String
    Dim cmd As New SqlCommand
    Dim param As SqlParameter
    Dim token As String
    Dim sql As String

    ' Generate a new Token
    token = GenerateToken()

    sql = "INSERT INTO esAppToken(sToken, sAppName, "
    sql &= " sLoginID, iUserID, iAppID, dtCreated) "
    sql &= " VALUES(@sToken, @sAppName, @sLoginID, "
    sql &= " @iUserID, @iAppID, @dtCreated)"
    sql = String.Format(sql, token, AppName, _
      Apps.LoginIDNoDomain(LoginID), UserID, AppID, _
      DateTime.Now.ToString())

    param = New SqlParameter("@sToken", SqlDbType.Char)
    param.Value = token
    cmd.Parameters.Add(param)

    param = New SqlParameter("@sAppName", SqlDbType.Char)
    param.Value = AppName
    cmd.Parameters.Add(param)

    param = New SqlParameter("@sLoginID", SqlDbType.Char)
    param.Value = Apps.LoginIDNoDomain(LoginID)
    cmd.Parameters.Add(param)

    param = New SqlParameter("@iUserID", SqlDbType.Int)
    param.Value = UserID
    cmd.Parameters.Add(param)

    param = New SqlParameter("@iAppID", SqlDbType.Int)
    param.Value = AppID
    cmd.Parameters.Add(param)

    param = New SqlParameter("@dtCreated", SqlDbType.DateTime)
    param.Value = DateTime.Now
    cmd.Parameters.Add(param)

    Try
      cmd.CommandType = CommandType.Text
      cmd.CommandText = sql

      cmd.Connection = New SqlConnection(mConnectString)
      cmd.Connection.Open()

      cmd.ExecuteNonQuery()

    Catch ex As Exception
      Throw ex

    Finally
      If cmd.Connection.State <> ConnectionState.Closed Then
        cmd.Connection.Close()
        cmd.Connection.Dispose()
      End If
    End Try

    Return token
  End Function

  Public Function GenerateToken() As String
    Return System.Guid.NewGuid().ToString()
  End Function

  Public Function VerifyLoginToken(ByVal Token As String) As AppToken
    Dim al As New AppToken
    Dim ds As New DataSet
    Dim cmd As SqlCommand
    Dim dr As DataRow
    Dim da As SqlDataAdapter
    Dim sql As String

    sql = "SELECT iAppTokenID, sAppName, sLoginID, "
    sql &= " iAppID, iUserID "
    sql &= " FROM esAppToken"
    sql &= " WHERE sToken = @sToken "

    Try
      cmd = New SqlCommand(sql)
      cmd.Parameters.Add(New _
        SqlParameter("@sToken", SqlDbType.Char))
      cmd.Parameters("@sToken").Value = Token
      cmd.Connection = New SqlConnection(mConnectString)

      da = New SqlDataAdapter(cmd)

      da.Fill(ds)

      If ds.Tables(0).Rows.Count > 0 Then
        dr = ds.Tables(0).Rows(0)

        al.LoginID = dr("sLoginID").ToString()
        al.AppName = dr("sAppName").ToString()
        al.AppKey = Convert.ToInt32(dr("iAppID"))
        al.LoginKey = Convert.ToInt32(dr("iUserID"))

        DeleteToken(Convert.ToInt32(dr("iAppTokenID")))
      End If

    Catch ex As Exception
      Throw ex
    End Try

    Return al
  End Function

  Public Sub DeleteToken(ByVal AppTokenID As Integer)
    Dim cmd As New SqlCommand
    Dim sql As String

    sql = "DELETE FROM esAppToken "
    sql &= " WHERE iAppTokenID = @iAppTokenID"

    Try
      cmd.CommandText = sql
      cmd.Parameters.Add(New _
        SqlParameter("@iAppTokenID", SqlDbType.Int))
      cmd.Parameters("@iAppTokenID").Value = AppTokenID

      cmd.Connection = New SqlConnection(mConnectString)
      cmd.Connection.Open()

      cmd.ExecuteNonQuery()

    Catch ex As Exception
      Throw ex

    Finally
      If cmd.Connection.State <> ConnectionState.Closed Then
        cmd.Connection.Close()
        cmd.Connection.Dispose()
      End If
    End Try
  End Sub

  Public Function GetAppsByLoginID(ByVal LoginID As String) _
    As DataSet
    Dim ds As New DataSet
    Dim cmd As SqlCommand
    Dim da As SqlDataAdapter
    Dim sql As String

    sql = "SELECT esApps.iAppID, esAppsUsers.iUserID, "
    sql &= " esApps.sAppName, esApps.sDesc, esApps.sURL "
    sql &= " FROM esApps"
    sql &= " INNER JOIN esAppsUsers "
    sql &= " ON esApps.iAppID = esAppsUsers.iAppID "
    sql &= " INNER JOIN esUsers "
    sql &= " ON esAppsUsers.iUserID = esUsers.iUserID "
    sql &= " WHERE sLoginID = @sLoginID "
    sql = String.Format(sql, Apps.LoginIDNoDomain(LoginID))

    Try
      cmd = New SqlCommand(sql)
      cmd.Parameters.Add(New _
        SqlParameter("@sLoginID", SqlDbType.Char))
      cmd.Parameters("@sLoginID").Value = _
        Apps.LoginIDNoDomain(LoginID)
      cmd.Connection = New SqlConnection(mConnectString)

      da = New SqlDataAdapter(cmd)
      da.Fill(ds)

      Return ds

    Catch ex As Exception
      Throw ex
    End Try
  End Function

  Public Shared Function LoginIDNoDomain(ByVal LoginID As String) As String
    If LoginID.IndexOf("\") >= 0 Then
      LoginID = LoginID.Substring(LoginID.IndexOf("\") + 1)
    End If

    Return LoginID
  End Function
End Class
