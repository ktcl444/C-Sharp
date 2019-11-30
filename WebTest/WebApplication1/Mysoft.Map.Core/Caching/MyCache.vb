Imports System
Imports System.Collections
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.Caching
Imports System.IO
Imports System.Net.Security
Imports System.Net

'>> Added by liubao 2007/9/16 
Namespace Caching
    '>> 使用单例模式 >> 尽可能的保证缓存的对象少变动并且是被频繁访问的 
    Public Class MyCache
        Private Sub New()
        End Sub

        '>> 静态方法保证只会被调用一次
        Shared Sub New()
            Dim context As HttpContext = HttpContext.Current
            If Not context Is Nothing Then
                _cache = context.Cache
            Else
                _cache = HttpRuntime.Cache
            End If
        End Sub

#Region "Fields"
        Public Shared ReadOnly DayFactor As Integer = 86400
        Public Shared ReadOnly HourFactor As Integer = 3600
        Public Shared ReadOnly MinuteFactor As Integer = 60
        '>>ReadOnly 保证只能赋一次值  
        Private Shared ReadOnly _cache As System.Web.Caching.Cache
        Private Shared _syncConfigs As Hashtable            '所有服务器的同步配置信息，键为服务器名称
        Private Shared _syncServices As Hashtable           '需要同步的WebService实例列表，键为服务器名称
        Private Shared _syncConfigLocal As CacheSyncConfig  '当前服务器的同步配置信息
        Private Shared syncObject As Object = New Object
#End Region

#Region "Property"
        Public Shared ReadOnly Property SyncConfigLocal() As CacheSyncConfig
            Get
                InitSyncConfigs()
                Return _syncConfigLocal
            End Get
        End Property

        '缓存同步类型
        Private Enum CacheOperateType
            Insert
            Remove
            Clear
            Max
            RemoveByPattern
            ClearAllDeskTempFiles
            ClearAllFuncTempFile
            ClearAllFuncTempFileByCode
            ClearAllSysTempFile
            ClearDeskTempFile
            ClearTempFiles
            ClearTempFilesByPattern
        End Enum
#End Region

#Region "Private Method"

        '初始化同步配置信息
        Private Shared Sub InitSyncConfigs()

            _syncConfigs = [Get]("SyncCacheConfig")

            If _syncConfigs Is Nothing Then
                SyncLock syncObject

                    '获取配置信息
                    _syncConfigs = CacheSyncHelper.GetAllSyncConfig()
                    If _syncConfigs Is Nothing Then
                        _syncConfigLocal = Nothing
                        _syncServices = Nothing
                        Return
                    End If

                    _syncConfigLocal = _syncConfigs(CacheSyncHelper.GetSyncConfigKey())
                    _cache.Insert("SyncCacheConfig", _syncConfigs)

                    '实例化服务
                    _syncServices = New Hashtable
                    For Each config As CacheSyncConfig In _syncConfigs.Values
                        If CacheSyncHelper.GetSyncConfigKey(config.ServerName, config.SyncServiceUrl) <> CacheSyncHelper.GetSyncConfigKey() AndAlso config.IsLoadBalance = 1 Then
                            _syncServices.Add(CacheSyncHelper.GetSyncConfigKey(config.ServerName, config.SyncServiceUrl), New Mysoft.Map.SyncCacheService.SyncCacheService(CacheSyncHelper.GetSyncConfigServiceUrl(config.SyncServiceUrl)))
                        End If
                    Next
                End SyncLock
            End If
        End Sub

        Private Shared Function IngoreValidate() As Boolean
            Return True
        End Function

        '同步缓存(广义缓存，包括Cache和TempFiles)
        Private Shared Sub SyncCache(ByVal param As String, ByVal cacheOperateType As CacheOperateType)

            '1）执行同步前，必须先初始化配置信息、服务实例
            InitSyncConfigs()

            If _syncConfigLocal Is Nothing OrElse _syncConfigLocal.IsLoadBalance = 0 Then
                Return
            End If
            If _syncServices Is Nothing OrElse _syncServices.Count = 0 Then
                Return
            End If

            '2）遍历目标服务器，执行同步操作
            Dim syncResult As String
            Dim syncConfig As CacheSyncConfig
            Dim syncService As SyncCacheService.SyncCacheService

            Dim enumerator As IDictionaryEnumerator = _syncServices.GetEnumerator()
            While enumerator.MoveNext()
                syncConfig = _syncConfigs(enumerator.Key)                                   '目标服务器的配置信息
                syncService = CType(enumerator.Value, SyncCacheService.SyncCacheService)    '目标服务器的服务实例

                ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf IngoreValidate)

                Try
                    Select Case cacheOperateType
                        'cache
                        Case cacheOperateType.Clear
                            syncResult = syncService.Clear()
                        Case cacheOperateType.Insert
                            syncResult = syncService.Remove(param)
                        Case cacheOperateType.Max
                            syncResult = syncService.Remove(param)
                        Case cacheOperateType.Remove
                            syncResult = syncService.Remove(param)
                        Case cacheOperateType.RemoveByPattern
                            syncResult = syncService.RemoveByPattern(param)
                            'tempFiles
                        Case cacheOperateType.ClearAllDeskTempFiles
                            syncResult = syncService.ClearAllDeskTempFiles()
                        Case cacheOperateType.ClearAllFuncTempFile
                            syncResult = syncService.ClearAllFuncTempFile()
                        Case cacheOperateType.ClearAllFuncTempFileByCode
                            syncResult = syncService.ClearAllFuncTempFileByCode(param)
                        Case cacheOperateType.ClearAllSysTempFile
                            syncResult = syncService.ClearAllSysTempFile()
                        Case cacheOperateType.ClearDeskTempFile
                            syncResult = syncService.ClearDeskTempFile(param)
                        Case cacheOperateType.ClearTempFiles
                            syncResult = syncService.ClearTempFiles(param)
                        Case cacheOperateType.ClearTempFilesByPattern
                            syncResult = syncService.ClearTempFilesByPattern(param)
                        Case Else
                            syncResult = String.Empty
                    End Select

                    '成功连接到目标服务器，但在目标服务器中刷新缓存时出现异常的场景。
                    If syncResult <> String.Empty Then
                        Dim s() As String = syncResult.Split("|"c)
                        CacheSyncHelper.ModifySyncStatus(syncConfig.ServerName, syncConfig.SyncServiceUrl, _syncConfigLocal.ServerName, param, s(0), s(1), cacheOperateType.ToString())
                    End If

                Catch ex As Exception
                	Mysoft.Map.Data.MyDB.LogException(ex)
                    '无法调用ws导致异常的场景，例如网络错误、目标服务器IIS错误。
                    CacheSyncHelper.ModifySyncStatus(syncConfig.ServerName, syncConfig.SyncServiceUrl, _syncConfigLocal.ServerName, param, ex.Message, ex.StackTrace, cacheOperateType.ToString())
                End Try
            End While
        End Sub

#End Region


        '>> 清除所有缓存对象
        Public Shared Sub Clear()
            Dim CacheEnum As IDictionaryEnumerator = _cache.GetEnumerator()
            While CacheEnum.MoveNext()
                _cache.Remove(CacheEnum.Key.ToString())
            End While

            '同步缓存
            SyncCache(String.Empty, CacheOperateType.Clear)
        End Sub

        '>> 清除所有缓存对象，不同步
        Friend Shared Sub ClearLocal()
            Dim CacheEnum As IDictionaryEnumerator = _cache.GetEnumerator()
            While CacheEnum.MoveNext()
                _cache.Remove(CacheEnum.Key.ToString())
            End While

            Try
                Dim folder As String = HttpContext.Current.Server.MapPath("/TempFiles/")
                For Each fileName As String In IO.Directory.GetFiles(folder)
                    IO.File.Delete(fileName)
                Next
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)

            End Try
        End Sub

        '>> 通过正则表达式匹配key,来删除缓存对象
        Public Shared Sub RemoveByPattern(ByVal pattern As String)
            Dim CacheEnum As IDictionaryEnumerator = _cache.GetEnumerator()
            Dim regex As New Regex(pattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline Or RegexOptions.Compiled)
            While CacheEnum.MoveNext()
                If regex.IsMatch(CacheEnum.Key.ToString()) Then
                    _cache.Remove(CacheEnum.Key.ToString())
                End If
            End While

            '同步缓存
            SyncCache(pattern, CacheOperateType.RemoveByPattern)
        End Sub

        '>> 删键值为key的缓存对象
        Public Shared Sub Remove(ByVal key As String)
            _cache.Remove(key)

            '同步缓存
            SyncCache(key, CacheOperateType.Remove)
        End Sub

        '绝对过期时间的缓存
        Public Shared Sub Insert(ByVal key As String, ByVal obj As Object)
            Insert(key, obj, Nothing, HourFactor * 12)
        End Sub

        Public Shared Sub Insert(ByVal key As String, ByVal obj As Object, ByVal dep As CacheDependency)
            Insert(key, obj, dep, HourFactor * 12)
        End Sub

        Public Shared Sub Insert(ByVal key As String, ByVal obj As Object, ByVal seconds As Integer)
            Insert(key, obj, Nothing, seconds)
        End Sub

        Public Shared Sub Insert(ByVal key As String, ByVal obj As Object, ByVal seconds As Integer, ByVal priority As CacheItemPriority)
            Insert(key, obj, Nothing, seconds, priority)
        End Sub

        Public Shared Sub Insert(ByVal key As String, ByVal obj As Object, ByVal dep As CacheDependency, ByVal seconds As Integer)
            Insert(key, obj, dep, seconds, CacheItemPriority.Normal)
        End Sub

        Public Shared Sub Insert(ByVal key As String, ByVal obj As Object, ByVal dep As CacheDependency, ByVal seconds As Integer, ByVal priority As CacheItemPriority)
            If Not obj Is Nothing Then
                _cache.Insert(key, obj, dep, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero, priority, Nothing)

                '同步缓存
                SyncCache(key, CacheOperateType.Insert)
            End If
        End Sub

        '相对过期时间的缓存
        Public Shared Sub Insert(ByVal key As String, ByVal obj As Object, ByVal dt As DateTime)
            Insert(key, obj, dt, TimeSpan.Zero)
        End Sub

        Public Shared Sub Insert(ByVal key As String, ByVal obj As Object, ByVal dt As DateTime, ByVal ts As TimeSpan)
            Insert(key, obj, dt, ts, Nothing)
        End Sub

        Public Shared Sub Insert(ByVal key As String, ByVal obj As Object, ByVal dt As DateTime, ByVal ts As TimeSpan, ByVal dep As CacheDependency)
            Insert(key, obj, dt, ts, dep, CacheItemPriority.Normal)
        End Sub

        Public Shared Sub Insert(ByVal key As String, ByVal obj As Object, ByVal dt As DateTime, ByVal ts As TimeSpan, ByVal dep As CacheDependency, ByVal priority As CacheItemPriority)
            If Not obj Is Nothing Then
                _cache.Insert(key, obj, dep, dt, ts, priority, Nothing)

                '同步缓存
                SyncCache(key, CacheOperateType.Insert)
            End If
        End Sub

        'myCache.Insert(strCacheKey, DTTemp, Nothing, DateTime.Now.AddMinutes(20), TimeSpan.Zero)

        '>> 使缓存对象存在尽可能多的时间
        Public Shared Sub Max(ByVal key As String, ByVal obj As Object)
            Max(key, obj, Nothing)
        End Sub
        '>> 使缓存对象存在尽可能多的时间
        Public Shared Sub Max(ByVal key As String, ByVal obj As Object, ByVal dep As CacheDependency)
            If Not obj Is Nothing Then
                _cache.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.AboveNormal, _
                Nothing)

                '同步缓存
                SyncCache(key, CacheOperateType.Max)
            End If
        End Sub

        '>> 得到件值为key的缓存对象
        Public Shared Function [Get](ByVal key As String) As Object
            Return _cache(key)
        End Function

        '清除参数缓存
        Public Shared Sub ClearParamCache(ByVal param As String, ByVal buguid As String)
            '清除 AppGrid 中使用的参数缓存
            'Remove("AppGrid_" & param & "_" & buguid)

            ''清除 AppForm 中使用的参数缓存
            'Remove("AppForm_" & param & "_" & buguid)

            ''清除 Settings.aspx 中使用的参数缓存
            'Remove("s_Mj_" & buguid)

            '清除BizParam缓存
            Remove("Biz_Param_" & buguid.ToUpper & param.ToUpper)

        End Sub

        Public Shared Sub ClearNavFuncCache()
            Remove("SysNav")
            RemoveByPattern("FuncNav_")
        End Sub

        '''清空用户相关动作权限缓存
        Public Shared Sub ClearUserActionCache(ByVal strUserGUID As String)
            ' 清除用户动作权限缓存，缓存参见 User.GetUserActionRights() 函数
            RemoveByPattern("ActionString_" & strUserGUID.ToLower)
        End Sub

        '''清空用户相关数据权限缓存
        Public Shared Sub ClearUserDataCache(ByVal strUserGUID As String)
            ClearProjectCache(strUserGUID)
        End Sub

        '''清除用户项目权限缓存
        Public Shared Sub ClearProjectCache()
            RemoveByPattern("ProjectString_")
        End Sub

        ' 清除用户项目权限缓存，缓存参见 GetProjectString() 函数
        Public Shared Sub ClearProjectCache(ByVal strUserGUID As String)
            Remove("ProjectString_" & strUserGUID.ToLower)
            Remove("ProjectString_Exp_" & strUserGUID.ToLower)
        End Sub

#Region "TempFiles临时文件缓存"
        '按通配符清除TempFiles目录下的临时文件
        Public Shared Sub ClearTempFilesByPattern(ByVal pattern As String)
            If pattern.Trim.Length <= 0 Then Return
            Try
                Dim folder As String = HttpContext.Current.Server.MapPath("/TempFiles/")
                For Each fileName As String In IO.Directory.GetFiles(folder, pattern)
                    IO.File.Delete(fileName)
                Next

                SyncCache(pattern, CacheOperateType.ClearTempFilesByPattern)
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)

            End Try
        End Sub

        '清除所有缓存的临时文件（TempFiles目录下的SysNav_*.xml）
        Public Shared Sub ClearAllSysTempFile()
            ClearTempFilesByPattern("SysNav_*.xml")
        End Sub

        '清除所有缓存的临时文件（TempFiles目录下的FuncNav_*.xml）
        Public Shared Sub ClearAllFuncTempFile(Optional ByVal appCode As String = "")
            If appCode.Length = 0 Then
                ClearTempFilesByPattern("FuncNav_*.xml")
            Else
                ClearTempFilesByPattern("FuncNav_*_" & appCode & ".xml")
            End If
        End Sub

        '清除所有缓存的临时文件（TempFiles目录下的DeskTop_*.htm）
        Public Shared Sub ClearAllDeskTempFiles()
            ClearTempFilesByPattern("DeskTop_*")
        End Sub

        '按用户清除缓存的临时文件（TempFiles目录下的系统导航xml、功能导航xml、桌面部件htm）
        Public Shared Sub ClearTempFiles(ByVal userGUID As String)
            If userGUID.Trim.Length <= 0 Then Return
            ClearTempFilesByPattern("*_" & userGUID & "*")
        End Sub

        '按用户清除缓存的桌面部件htm
        Public Shared Sub ClearDeskTempFile(ByVal userGUID As String)
            If userGUID.Trim.Length <= 0 Then Return
            Try
                File.Delete(HttpContext.Current.Server.MapPath("/TempFiles/DeskTop_" & userGUID & ".htm"))
                SyncCache(userGUID, CacheOperateType.ClearDeskTempFile)
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)

            End Try
        End Sub
#End Region

    End Class
End Namespace
