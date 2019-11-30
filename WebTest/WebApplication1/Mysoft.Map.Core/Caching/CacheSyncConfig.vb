Namespace Caching

    Public Class CacheSyncConfig

        Private _serverName As String
        Private _syncServiceUrl As String
        Private _ipLimit As String
        Private _syncStatus As Int16
        Private _isLoadBalance As Int16

        Property ServerName() As String
            Get
                Return _serverName
            End Get
            Set(ByVal Value As String)
                _serverName = Value
            End Set
        End Property

        Property SyncServiceUrl() As String
            Get
                Return _syncServiceUrl
            End Get
            Set(ByVal Value As String)
                _syncServiceUrl = Value
            End Set
        End Property

        Property IPLimit() As String
            Get
                Return _ipLimit
            End Get
            Set(ByVal Value As String)
                _ipLimit = Value
            End Set
        End Property

        Property SyncStatus() As Int16
            Get
                Return _syncStatus
            End Get
            Set(ByVal Value As Int16)
                _syncStatus = Value
            End Set
        End Property

        Property IsLoadBalance() As Int16
            Get
                Return _isLoadBalance
            End Get
            Set(ByVal Value As Int16)
                _isLoadBalance = Value
            End Set
        End Property



        Sub New(ByVal serverName As String, ByVal syncServiceUrl As String, _
                ByVal ipLimit As String, ByVal syncStatus As Int16, ByVal isLoadBalance As Int16)
            Me._serverName = serverName
            Me._syncServiceUrl = syncServiceUrl
            Me._ipLimit = ipLimit
            Me._syncStatus = syncStatus
            Me._isLoadBalance = isLoadBalance
        End Sub
    End Class
End Namespace
