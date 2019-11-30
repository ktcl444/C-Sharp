Public Class AppToken
  Private mLoginID As String
  Private mAppName As String
  Private mLoginKey As Integer
  Private mAppKey As Integer

  Public Sub New()
    mLoginID = String.Empty
    mAppName = String.Empty
  End Sub

  Public Property LoginID() As String
    Get
      Return mLoginID
    End Get
    Set(ByVal Value As String)
      mLoginID = Value
    End Set
  End Property

  Public Property AppName() As String
    Get
      Return mAppName
    End Get
    Set(ByVal Value As String)
      mAppName = Value
    End Set
  End Property

  Public Property LoginKey() As Integer
    Get
      Return mLoginKey
    End Get
    Set(ByVal Value As Integer)
      mLoginKey = Value
    End Set
  End Property

  Public Property AppKey() As Integer
    Get
      Return mAppKey
    End Get
    Set(ByVal Value As Integer)
      mAppKey = Value
    End Set
  End Property
End Class
