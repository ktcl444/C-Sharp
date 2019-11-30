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
    '>> ʹ�õ���ģʽ >> �����ܵı�֤����Ķ����ٱ䶯�����Ǳ�Ƶ�����ʵ� 
    Public Class MyCache
        Private Sub New()
        End Sub

        '>> ��̬������ֻ֤�ᱻ����һ��
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
        '>>ReadOnly ��ֻ֤�ܸ�һ��ֵ  
        Private Shared ReadOnly _cache As System.Web.Caching.Cache
        Private Shared _syncConfigs As Hashtable            '���з�������ͬ��������Ϣ����Ϊ����������
        Private Shared _syncServices As Hashtable           '��Ҫͬ����WebServiceʵ���б���Ϊ����������
        Private Shared _syncConfigLocal As CacheSyncConfig  '��ǰ��������ͬ��������Ϣ
        Private Shared syncObject As Object = New Object
#End Region

#Region "Property"
        Public Shared ReadOnly Property SyncConfigLocal() As CacheSyncConfig
            Get
                InitSyncConfigs()
                Return _syncConfigLocal
            End Get
        End Property

        '����ͬ������
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

        '��ʼ��ͬ��������Ϣ
        Private Shared Sub InitSyncConfigs()

            _syncConfigs = [Get]("SyncCacheConfig")

            If _syncConfigs Is Nothing Then
                SyncLock syncObject

                    '��ȡ������Ϣ
                    _syncConfigs = CacheSyncHelper.GetAllSyncConfig()
                    If _syncConfigs Is Nothing Then
                        _syncConfigLocal = Nothing
                        _syncServices = Nothing
                        Return
                    End If

                    _syncConfigLocal = _syncConfigs(CacheSyncHelper.GetSyncConfigKey())
                    _cache.Insert("SyncCacheConfig", _syncConfigs)

                    'ʵ��������
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

        'ͬ������(���建�棬����Cache��TempFiles)
        Private Shared Sub SyncCache(ByVal param As String, ByVal cacheOperateType As CacheOperateType)

            '1��ִ��ͬ��ǰ�������ȳ�ʼ��������Ϣ������ʵ��
            InitSyncConfigs()

            If _syncConfigLocal Is Nothing OrElse _syncConfigLocal.IsLoadBalance = 0 Then
                Return
            End If
            If _syncServices Is Nothing OrElse _syncServices.Count = 0 Then
                Return
            End If

            '2������Ŀ���������ִ��ͬ������
            Dim syncResult As String
            Dim syncConfig As CacheSyncConfig
            Dim syncService As SyncCacheService.SyncCacheService

            Dim enumerator As IDictionaryEnumerator = _syncServices.GetEnumerator()
            While enumerator.MoveNext()
                syncConfig = _syncConfigs(enumerator.Key)                                   'Ŀ���������������Ϣ
                syncService = CType(enumerator.Value, SyncCacheService.SyncCacheService)    'Ŀ��������ķ���ʵ��

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

                    '�ɹ����ӵ�Ŀ�������������Ŀ���������ˢ�»���ʱ�����쳣�ĳ�����
                    If syncResult <> String.Empty Then
                        Dim s() As String = syncResult.Split("|"c)
                        CacheSyncHelper.ModifySyncStatus(syncConfig.ServerName, syncConfig.SyncServiceUrl, _syncConfigLocal.ServerName, param, s(0), s(1), cacheOperateType.ToString())
                    End If

                Catch ex As Exception
                	Mysoft.Map.Data.MyDB.LogException(ex)
                    '�޷�����ws�����쳣�ĳ����������������Ŀ�������IIS����
                    CacheSyncHelper.ModifySyncStatus(syncConfig.ServerName, syncConfig.SyncServiceUrl, _syncConfigLocal.ServerName, param, ex.Message, ex.StackTrace, cacheOperateType.ToString())
                End Try
            End While
        End Sub

#End Region


        '>> ������л������
        Public Shared Sub Clear()
            Dim CacheEnum As IDictionaryEnumerator = _cache.GetEnumerator()
            While CacheEnum.MoveNext()
                _cache.Remove(CacheEnum.Key.ToString())
            End While

            'ͬ������
            SyncCache(String.Empty, CacheOperateType.Clear)
        End Sub

        '>> ������л�����󣬲�ͬ��
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

        '>> ͨ��������ʽƥ��key,��ɾ���������
        Public Shared Sub RemoveByPattern(ByVal pattern As String)
            Dim CacheEnum As IDictionaryEnumerator = _cache.GetEnumerator()
            Dim regex As New Regex(pattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline Or RegexOptions.Compiled)
            While CacheEnum.MoveNext()
                If regex.IsMatch(CacheEnum.Key.ToString()) Then
                    _cache.Remove(CacheEnum.Key.ToString())
                End If
            End While

            'ͬ������
            SyncCache(pattern, CacheOperateType.RemoveByPattern)
        End Sub

        '>> ɾ��ֵΪkey�Ļ������
        Public Shared Sub Remove(ByVal key As String)
            _cache.Remove(key)

            'ͬ������
            SyncCache(key, CacheOperateType.Remove)
        End Sub

        '���Թ���ʱ��Ļ���
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

                'ͬ������
                SyncCache(key, CacheOperateType.Insert)
            End If
        End Sub

        '��Թ���ʱ��Ļ���
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

                'ͬ������
                SyncCache(key, CacheOperateType.Insert)
            End If
        End Sub

        'myCache.Insert(strCacheKey, DTTemp, Nothing, DateTime.Now.AddMinutes(20), TimeSpan.Zero)

        '>> ʹ���������ھ����ܶ��ʱ��
        Public Shared Sub Max(ByVal key As String, ByVal obj As Object)
            Max(key, obj, Nothing)
        End Sub
        '>> ʹ���������ھ����ܶ��ʱ��
        Public Shared Sub Max(ByVal key As String, ByVal obj As Object, ByVal dep As CacheDependency)
            If Not obj Is Nothing Then
                _cache.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.AboveNormal, _
                Nothing)

                'ͬ������
                SyncCache(key, CacheOperateType.Max)
            End If
        End Sub

        '>> �õ���ֵΪkey�Ļ������
        Public Shared Function [Get](ByVal key As String) As Object
            Return _cache(key)
        End Function

        '�����������
        Public Shared Sub ClearParamCache(ByVal param As String, ByVal buguid As String)
            '��� AppGrid ��ʹ�õĲ�������
            'Remove("AppGrid_" & param & "_" & buguid)

            ''��� AppForm ��ʹ�õĲ�������
            'Remove("AppForm_" & param & "_" & buguid)

            ''��� Settings.aspx ��ʹ�õĲ�������
            'Remove("s_Mj_" & buguid)

            '���BizParam����
            Remove("Biz_Param_" & buguid.ToUpper & param.ToUpper)

        End Sub

        Public Shared Sub ClearNavFuncCache()
            Remove("SysNav")
            RemoveByPattern("FuncNav_")
        End Sub

        '''����û���ض���Ȩ�޻���
        Public Shared Sub ClearUserActionCache(ByVal strUserGUID As String)
            ' ����û�����Ȩ�޻��棬����μ� User.GetUserActionRights() ����
            RemoveByPattern("ActionString_" & strUserGUID.ToLower)
        End Sub

        '''����û��������Ȩ�޻���
        Public Shared Sub ClearUserDataCache(ByVal strUserGUID As String)
            ClearProjectCache(strUserGUID)
        End Sub

        '''����û���ĿȨ�޻���
        Public Shared Sub ClearProjectCache()
            RemoveByPattern("ProjectString_")
        End Sub

        ' ����û���ĿȨ�޻��棬����μ� GetProjectString() ����
        Public Shared Sub ClearProjectCache(ByVal strUserGUID As String)
            Remove("ProjectString_" & strUserGUID.ToLower)
            Remove("ProjectString_Exp_" & strUserGUID.ToLower)
        End Sub

#Region "TempFiles��ʱ�ļ�����"
        '��ͨ������TempFilesĿ¼�µ���ʱ�ļ�
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

        '������л������ʱ�ļ���TempFilesĿ¼�µ�SysNav_*.xml��
        Public Shared Sub ClearAllSysTempFile()
            ClearTempFilesByPattern("SysNav_*.xml")
        End Sub

        '������л������ʱ�ļ���TempFilesĿ¼�µ�FuncNav_*.xml��
        Public Shared Sub ClearAllFuncTempFile(Optional ByVal appCode As String = "")
            If appCode.Length = 0 Then
                ClearTempFilesByPattern("FuncNav_*.xml")
            Else
                ClearTempFilesByPattern("FuncNav_*_" & appCode & ".xml")
            End If
        End Sub

        '������л������ʱ�ļ���TempFilesĿ¼�µ�DeskTop_*.htm��
        Public Shared Sub ClearAllDeskTempFiles()
            ClearTempFilesByPattern("DeskTop_*")
        End Sub

        '���û�����������ʱ�ļ���TempFilesĿ¼�µ�ϵͳ����xml�����ܵ���xml�����沿��htm��
        Public Shared Sub ClearTempFiles(ByVal userGUID As String)
            If userGUID.Trim.Length <= 0 Then Return
            ClearTempFilesByPattern("*_" & userGUID & "*")
        End Sub

        '���û������������沿��htm
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
