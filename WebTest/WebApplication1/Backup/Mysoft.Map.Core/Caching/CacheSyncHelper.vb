Imports System
Imports System.Web
Imports System.Collections
Imports Mysoft.Map.Data
Imports System.Text.RegularExpressions
Imports System.Security.Policy
Imports System.Net

Namespace Caching

    Public Class CacheSyncHelper
        '获得缓存同步配置键
        Public Shared Function GetSyncConfigKey() As String
            Return GetServerName() + "," + GetServerUrlPort()
        End Function

        '获得服务器端口号
        Public Shared Function GetServerUrlPort() As String
            Return HttpContext.Current.Request.Url.Port.ToString
        End Function

        '获得服务器端口号
        '参数：服务器Url
        Public Shared Function GetServerUrlPort(ByVal serviceUrl As String) As String
            Dim urls As String() = GetTrimUrl(serviceUrl).Split(":")
            If Not urls Is Nothing AndAlso urls.Length > 2 Then
                Return urls(2)
            End If
            Return "80"
        End Function

        ''获得缓存同步配置键
        ''参数 服务器名称,服务器地址
        Public Shared Function GetSyncConfigKey(ByVal serverName As String, ByVal serviceUrl As String) As String
            Return serverName.ToLower + "," + GetServerUrlPort(serviceUrl)
        End Function

        ''获得缓存同步配置服务器WebService覅之
        ''参数 服务器地址
        Public Shared Function GetSyncConfigServiceUrl(ByVal serviceUrl As String) As String
            Return GetTrimUrl(serviceUrl) + "/WebService/SyncCacheService.asmx"
        End Function

        ''获得截断后的地址
        ''参数 原始地址
        Private Shared Function GetTrimUrl(ByVal baseUrl As String) As String
            Return GetTrimUrl(baseUrl, "/")
        End Function

        ''获得截断后的地址
        ''参数 原始地址,阶段字符串
        Private Shared Function GetTrimUrl(ByVal baseUrl As String, ByVal trimString As String) As String
            Return baseUrl.TrimEnd(trimString)
        End Function

        '从数据库获得缓存同步配置信息
        Public Shared Function GetAllSyncConfig() As Hashtable
            Dim DT As DataTable
            Dim SQL As String
            Dim DR As DataRow
            Dim syncConfigs As Hashtable
            Dim config As CacheSyncConfig

            SQL = "SELECT ServerName,SyncServiceUrl,IPLimit,SyncStatus,IsLoadBalance FROM myCacheSyncConfig"
            DT = MyDB.GetDataTable(SQL)

            If DT.Rows.Count > 0 Then
                syncConfigs = New Hashtable
                For Each DR In DT.Rows
                    config = New CacheSyncConfig(Convert.ToString(DR("ServerName")), Convert.ToString(DR("SyncServiceUrl")), Convert.ToString(DR("IPLimit")), _
                     Convert.ToInt16(DR("SyncStatus")), Convert.ToInt16(DR("IsLoadBalance")))

                    syncConfigs.Add(GetSyncConfigKey(config.ServerName, config.SyncServiceUrl), config)
                Next
            End If

            Return syncConfigs
        End Function

        ''修改同步状态为失败，并记录同步失败日志
        Public Shared Sub ModifySyncStatus(ByVal serverName As String, ByVal syncServiceUrl As String, ByVal sourceServer As String, ByVal cacheKey As String, ByVal message As String, ByVal stackTrace As String, ByVal operateType As String)
            If CheckSyncStatusSuccess(serverName, syncServiceUrl) Then
                Dim SQL As String
                SQL = "update myCacheSyncConfig set SyncStatus='0' where ServerName ='" + serverName + "' and SyncServiceUrl = '" & syncServiceUrl & "'" & vbCrLf
                SQL &= "INSERT INTO myCacheSyncLog(LogGUID,FailTime,DestServer,SourceServer,CacheKey,SyncServiceURL,ExceptionMessage,ExceptionStackTrace,CacheOperateType) " & _
                       "VALUES(newid(),getdate(),'" + serverName + "','" + sourceServer + "','" + cacheKey + "','" + syncServiceUrl + "','" + message + "','" + stackTrace + "','" + operateType + "')"
                MyDB.ExecuteSQL(SQL)
            End If
        End Sub


        ''检测同步状态是否成功
        Public Shared Function CheckSyncStatusSuccess(ByVal serverName As String, ByVal serviceUrl As String) As Boolean
            Dim DT As DataTable
            Dim SQL As String
            Dim DR As DataRow
            SQL = "SELECT 1 FROM myCacheSyncConfig where SyncStatus=1 and ServerName ='" + serverName + "' and SyncServiceUrl = '" & serviceUrl & "'"

            If MyDB.GetDataItemInt(SQL) = 1 Then
                Return True
            End If

            Return False
        End Function

        ''登录后检测当前服务缓存同步状态
        Public Shared Sub CheckCacheSyncStatusAfterLogin()
            Dim serverName As String = GetServerName()
            Dim serviceUrl As String = GetServerUrl()
            If CheckClearCacheAfterLogin(serverName, serviceUrl) Then
                Dim SQL As String
                MyCache.ClearLocal()
                SQL = "update myCacheSyncConfig set SyncStatus='1' where ServerName ='" + serverName + "' and SyncServiceUrl = '" & serviceUrl & "'"
                MyDB.ExecuteSQL(SQL)
            End If
        End Sub

        ''检测是否需要清空缓存
        Public Shared Function CheckClearCacheAfterLogin(ByVal serverName As String, ByVal serviceUrl As String) As Boolean
            Dim SQL As String
            SQL = "SELECT 1 FROM myCacheSyncConfig where IsLoadBalance=1 and SyncStatus=0 and ServerName ='" + serverName + "' and SyncServiceUrl = '" & serviceUrl & "'"

            If MyDB.GetDataItemInt(SQL) = 1 Then
                Return True
            End If

            Return False
        End Function

        '获得访问地址
        Public Shared Function GetServerUrl() As String
            Return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)
        End Function

        '获得IP检测结果
        Public Shared Function GetCheckIPResult() As String
            Dim returnValue As String = String.Empty
            returnValue = CheckIP()
            If returnValue.Length = 0 Then
                Return returnValue
            Else
                Return returnValue & "|"
            End If
        End Function

        '检查IP是否合法
        Public Shared Function CheckIP() As String
            '客户端的IP
            Dim clientIP As String = CacheSyncHelper.GetIP(HttpContext.Current.Request.UserHostAddress)

            '本主机的同步配置信息
            Dim config As CacheSyncConfig = MyCache.SyncConfigLocal

            If clientIP = String.Empty Then
                Return "IP校验不通过：客户端IP为空！"
            End If

            If config Is Nothing Then
                Return "IP校验不通过：服务器" & CacheSyncHelper.GetServerName() & "）的缓存同步配置信息为空！"
            End If

            If config.IPLimit = String.Empty Then
                Return "IP校验不通过：服务器" & CacheSyncHelper.GetServerName() & "）的IPLimit设置为空！"
            End If

            If config.IPLimit.IndexOf(clientIP) = -1 Then
                Return "IP校验不通过：服务器" & CacheSyncHelper.GetServerName() & "不允许" & clientIP & "同步！"
            End If

            Return String.Empty
        End Function

        '获取请求的客户端IP
        Public Shared Function GetIP(ByVal host As String) As String
            If host = "localhost" OrElse host = "." OrElse host = "(local)" Then
                Return "127.0.0.1"
            End If

            Dim regex As New Regex("\d+\.\d+\.\d+\.\d+")
            If regex.IsMatch(host) Then
                Return host
            Else
                Dim h As IPHostEntry = Dns.GetHostByName(host)
                Dim ips As IPAddress() = h.AddressList
                Return ips(0).ToString()
            End If
        End Function

        ''检测同步设置是否重复
        Public Shared Function CheckSyncConfigRepeated(ByVal serverName As String, ByVal serviceUrl As String) As Boolean
            Return CheckSyncConfigRepeated(serverName, serviceUrl, String.Empty)
        End Function

        ''检测同步设置是否重复
        Public Shared Function CheckSyncConfigRepeated(ByVal serverName As String, ByVal serviceUrl As String, ByVal configGUID As String) As Boolean
            Dim DT As DataTable
            Dim SQL As String
            Dim DR As DataRow
            SQL = "SELECT COUNT(*) FROM myCacheSyncConfig where  ServerName ='" + serverName + "' and SyncServiceUrl = '" & serviceUrl & "'"
            If configGUID.Length > 0 Then
                SQL += " and CacheSyncConfigGUID <> '" + configGUID + "'"
            End If
            If MyDB.GetDataItemInt(SQL) > 0 Then
                Return True
            End If
            Return False
        End Function

        '检测配置是否正确
        Public Shared Function CheckConfig(ByVal serverName As String) As String
            Dim strResult As String = String.Empty
            Dim rightServerName As String = GetServerName()
            If serverName <> rightServerName Then
                strResult = "服务器名称不正确：" & serverName & "应该为 " & rightServerName & "！"
            End If
            Return strResult
        End Function

        '获取机器名。（该值可以从“我的电脑->属性->计算机名->更改->计算机名”获取）
        Public Shared Function GetServerName() As String
            Return HttpContext.Current.Server.MachineName.ToLower
        End Function

    End Class
End Namespace

