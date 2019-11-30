Imports Mysoft.Map.Data
Imports System.Xml
Imports System.Web
Imports System.Text
Imports System.Collections.Specialized
Imports System.Data.SqlClient
Imports Mysoft.Map.Application.Types

Namespace Application.Security
    Public Class Station


        '功能：获取“应用系统-功能模块-动作点”的树形DataTable
        '参数：userGUID        -- 当前操作授权的用户
        Public Shared Function GetStationActionRightsTreeDT(ByVal a_strStationGUID As String, ByVal a_strUserGUID As String, ByVal a_strApplicationList As String) As DataTable
            Dim strSQL As StringBuilder = New StringBuilder(4000)
            Dim dtActionTree As DataTable
            Dim strTemp, arrApplicationList(), strFilterLicensedFunc, strFilterFuncHidden, strFilterFuncActHidden, strFilterFuncActIncre As String
            If a_strStationGUID Is Nothing Then a_strStationGUID = Guid.NewGuid.ToString
            If a_strUserGUID Is Nothing Then a_strUserGUID = Guid.NewGuid.ToString
            arrApplicationList = a_strApplicationList.Split(",")
            For i As Integer = 0 To arrApplicationList.Length - 1
                '获取许可的功能模块代码字符串(,分割)
                strTemp = VerifyApplication.GetLicenseFunctionString(arrApplicationList(i))
                strFilterLicensedFunc &= IIf(strTemp = "", "", IIf(i = 0, "", ",") & strTemp)

                '获取受业务影响需要隐藏的对象的过滤字符串(,分割)
                '格式： 1－功能；2－功能.动作
                strTemp = VerifyApplication.GetNotInFilterString(1, arrApplicationList(i))
                strFilterFuncHidden &= IIf(strTemp = "", "", IIf(i = 0, "", ",") & strTemp)
                strTemp = VerifyApplication.GetNotInFilterString(2, arrApplicationList(i))
                strFilterFuncActHidden &= IIf(strTemp = "", "", IIf(i = 0, "", ",") & strTemp)

            Next

            '获取受增值模块影响的没有授权的功能动作点过滤字符串(,分割)
            '格式：功能代码.动作代码
            strFilterFuncActIncre = VerifyApplication.GetNotInFilterStringByIncrement()


            If User.IsAdmin(a_strUserGUID) Then      ' 如果是超级用户
                strSQL.Append(" SELECT Application,'' AS FunctionCode,'' AS ActionCode,Application AS [Code],ApplicationName AS [Name],1 as [Level],IsLastStage,HierarchyCode,ParentHierarchyCode,'0' AS IsSelected,'1' AS IsAllowOpr,'功能' AS FunctionType ")
                strSQL.Append(vbCrLf)
                strSQL.Append(" FROM myApplication")
                strSQL.Append(vbCrLf)
                strSQL.Append(" WHERE Application IN ('")
                strSQL.Append(a_strApplicationList.Replace(",", "','"))
                strSQL.Append("')")
                strSQL.Append(vbCrLf)
                strSQL.Append(" UNION")
                strSQL.Append(vbCrLf)
                strSQL.Append(" SELECT Application,FunctionCode,'' AS ActionCode,Application+'.'+HierarchyCode AS [Code],FunctionName AS [Name],Level+2 as [Level],IsLastStage,HierarchyCode,ParentHierarchyCode,'0' AS IsSelected,'1' AS IsAllowOpr,FunctionType ")
                strSQL.Append(vbCrLf)
                strSQL.Append(" FROM myFunction")
                strSQL.Append(vbCrLf)
                strSQL.Append(" WHERE IsDisabled=0 AND IsDummy=0")
                strSQL.Append(vbCrLf)
                strSQL.Append(" AND FunctionCode NOT IN ('")
                strSQL.Append(strFilterFuncHidden.Replace(",", "','"))
                strSQL.Append("')")
                strSQL.Append(vbCrLf)
                strSQL.Append(" AND (FunctionCode IN ('")
                strSQL.Append(strFilterLicensedFunc.Replace(",", "','"))
                strSQL.Append("') OR (Application in ('")
                strSQL.Append(a_strApplicationList.Replace(",", "','"))
                strSQL.Append("') AND FunctionType='报表'))")
                strSQL.Append(vbCrLf)
                strSQL.Append(" UNION")
                strSQL.Append(vbCrLf)
                strSQL.Append(" SELECT Act.Application,Act.ObjectType AS FunctionCode,Act.ActionCode,FC.Application+'.'+FC.HierarchyCode+'.__'+Act.ActionCode AS [Code],Act.ActionName AS [Name],FC.Level+3 as [Level],'1' AS IsLastStage")
                strSQL.Append(vbCrLf)
                strSQL.Append(",Act.HierarchyCode,Act.ParentHierarchyCode,CASE WHEN RR.ActionCode IS NULL THEN 0 ELSE 1 END IsSelected ,'1' AS IsAllowOpr,FC.FunctionType")
                strSQL.Append(vbCrLf)
                strSQL.Append(" FROM e_myAction Act")
                strSQL.Append(vbCrLf)
                strSQL.Append(" INNER JOIN myFunction FC ON (Act.ObjectType=FC.FunctionCode AND FC.IsDummy=0 AND FC.IsDisabled=0)")
                strSQL.Append(vbCrLf)
                strSQL.Append(" LEFT JOIN myStationRights RR ON (Act.ObjectType=RR.ObjectType AND Act.ActionCode=RR.ActionCode AND RR.StationGUID='")
                strSQL.Append(a_strStationGUID.Replace("'", "''"))
                strSQL.Append("')")
                strSQL.Append(vbCrLf)
                strSQL.Append(" WHERE (Act.ObjectType+'.'+Act.ActionCode) NOT IN ('")
                strSQL.Append(strFilterFuncActIncre.Replace(",", "','"))
                strSQL.Append("')")
                strSQL.Append(vbCrLf)
                strSQL.Append(" AND (Act.ObjectType+'.'+Act.ActionCode) NOT IN ('")
                strSQL.Append(strFilterFuncActHidden.Replace(",", "','"))
                strSQL.Append("')")
                strSQL.Append(vbCrLf)
                strSQL.Append(" AND (Act.ObjectType IN ('")
                strSQL.Append(strFilterLicensedFunc.Replace(",", "','"))
                strSQL.Append("') OR (FC.Application in ('")
                strSQL.Append(a_strApplicationList.Replace(",", "','"))
                strSQL.Append("') AND FC.FunctionType='报表'))")
                strSQL.Append(vbCrLf)
                strSQL.Append(" ORDER BY Application,[Code]")

            Else        ' 如果不是超级用户
                strSQL.Append(" SELECT Application,'' AS FunctionCode,'' AS ActionCode,Application AS [Code],ApplicationName AS [Name],1 as [Level],IsLastStage,HierarchyCode,ParentHierarchyCode,'0' AS IsSelected,'1' AS IsAllowOpr,'功能' AS FunctionType ")
                strSQL.Append(vbCrLf)
                strSQL.Append(" FROM myApplication")
                strSQL.Append(vbCrLf)
                strSQL.Append(" WHERE Application IN ('")
                strSQL.Append(a_strApplicationList.Replace(",", "','"))
                strSQL.Append("')")
                strSQL.Append(vbCrLf)
                strSQL.Append(" UNION")
                strSQL.Append(vbCrLf)
                strSQL.Append(" SELECT Application,FunctionCode,'' AS ActionCode,Application+'.'+HierarchyCode AS [Code],FunctionName AS [Name],Level+2 as [Level],IsLastStage,HierarchyCode,ParentHierarchyCode,'0' AS IsSelected,'1' AS IsAllowOpr,FunctionType ")
                strSQL.Append(vbCrLf)
                strSQL.Append(" FROM myFunction")
                strSQL.Append(vbCrLf)
                strSQL.Append(" WHERE IsDisabled=0 AND IsDummy=0")
                strSQL.Append(vbCrLf)
                strSQL.Append(" AND FunctionCode NOT IN ('")
                strSQL.Append(strFilterFuncHidden.Replace(",", "','"))
                strSQL.Append("')")
                strSQL.Append(vbCrLf)
                strSQL.Append(" AND (FunctionCode IN ('")
                strSQL.Append(strFilterLicensedFunc.Replace(",", "','"))
                strSQL.Append("') OR (Application in ('")
                strSQL.Append(a_strApplicationList.Replace(",", "','"))
                strSQL.Append("') AND FunctionType='报表'))")
                strSQL.Append(vbCrLf)
                strSQL.Append(" UNION")
                strSQL.Append(vbCrLf)
                strSQL.Append(" SELECT Act.Application,Act.ObjectType AS FunctionCode,Act.ActionCode,FC.Application+'.'+FC.HierarchyCode+'.__'+Act.ActionCode AS [Code],Act.ActionName AS [Name],FC.Level+3 as [Level],'1' AS IsLastStage")
                strSQL.Append(vbCrLf)
                strSQL.Append(",Act.HierarchyCode,Act.ParentHierarchyCode")

                '当前选中项的判断规则
                '1 非报表 判断是否有岗位权限
                '2 报表   判断功能尾号为"1999"[报表管理],是否有"某全部"权限,其中报表的"00"[查看]权限,对应功能中的"01"[查看全部],需要进行转义处理
                strSQL.Append(" ,CASE WHEN RR.ActionCode IS NOT NULL THEN 1 ")
                strSQL.Append("  When exists(select * from myStationRights sr1 ")
                strSQL.Append("   where sr1.ObjectType=Act.Application+'1999' ")
                strSQL.Append("   and sr1.ActionCode = case when Act.ActionCode='00' then '01' else Act.ActionCode end")
                strSQL.Append("   and FC.FunctionType='报表' and sr1.StationGUID='")
                strSQL.Append(a_strStationGUID.Replace("'", "''"))
                strSQL.Append("') then 1")
                strSQL.Append(" ELSE 0 END IsSelected")
                'strSQL.Append(",CASE WHEN RR.ActionCode IS NULL THEN 0 ELSE 1 END IsSelected")
                strSQL.Append(vbCrLf)

                '当前可操作项的判断规则
                '1 非报表 判断是否有用户权限
                '2 报表   判断功能尾号为"1999"[报表管理],是否有"某全部"权限,其中报表的"00"[查看]权限,对应功能中的"01"[查看全部],需要进行转义处理
                strSQL.Append(" ,CASE WHEN UR.StationGUID IS NOT NULL THEN 1 ")
                strSQL.Append(" WHEN exists(select * from myUserRights ur1 ")
                strSQL.Append("   where ur1.ObjectType=Act.Application+'1999' ")
                strSQL.Append("   and ur1.ActionCode = case when Act.ActionCode='00' then '01' else Act.ActionCode end")
                strSQL.Append("   and FC.FunctionType='报表' and ur1.UserGUID='")
                strSQL.Append(a_strUserGUID)
                strSQL.Append("')  then 1")
                strSQL.Append(" ELSE 0 END IsAllowOpr")
                'strSQL.Append(",CASE WHEN UR.StationGUID IS NULL THEN 0 ELSE 1 END IsAllowOpr")

                strSQL.Append(",FC.FunctionType ")
                strSQL.Append(vbCrLf)
                strSQL.Append(" FROM e_myAction Act")
                strSQL.Append(vbCrLf)
                strSQL.Append(" INNER JOIN myFunction FC ON (Act.ObjectType=FC.FunctionCode AND FC.IsDummy=0 AND FC.IsDisabled=0)")
                strSQL.Append(vbCrLf)
                strSQL.Append(" LEFT JOIN myUserRights UR ON (Act.ObjectType=UR.ObjectType AND Act.ActionCode=UR.ActionCode AND UR.UserGUID='")
                strSQL.Append(a_strUserGUID)
                strSQL.Append("')")
                strSQL.Append(vbCrLf)
                strSQL.Append(" LEFT JOIN myStationRights RR ON (Act.ObjectType=RR.ObjectType AND Act.ActionCode=RR.ActionCode AND RR.StationGUID='")
                strSQL.Append(a_strStationGUID.Replace("'", "''"))
                strSQL.Append("')")
                strSQL.Append(vbCrLf)
                strSQL.Append(" WHERE (Act.ObjectType+'.'+Act.ActionCode) NOT IN ('")
                strSQL.Append(strFilterFuncActIncre.Replace(",", "','"))
                strSQL.Append("')")
                strSQL.Append(vbCrLf)
                strSQL.Append(" AND (Act.ObjectType+'.'+Act.ActionCode) NOT IN ('")
                strSQL.Append(strFilterFuncActHidden.Replace(",", "','"))
                strSQL.Append("')")
                strSQL.Append(vbCrLf)
                strSQL.Append(" AND (Act.ObjectType IN ('")
                strSQL.Append(strFilterLicensedFunc.Replace(",", "','"))
                strSQL.Append("') OR (FC.Application in ('")
                strSQL.Append(a_strApplicationList.Replace(",", "','"))
                strSQL.Append("') AND FC.FunctionType='报表'))")
                strSQL.Append(vbCrLf)
                strSQL.Append(" ORDER BY Application,[Code]")
            End If
            dtActionTree = MyDB.GetDataTable(strSQL.ToString())

            Return dtActionTree

        End Function


        '功能：获取数据权限对象的树形DataTable
        '参数：userGUID        -- 当前操作授权的用户
        '注意： 需要补齐树，叠加“当前用户权限+当前岗位所属公司”过滤，并根据当前岗位的已有权限初始化
        Public Shared Function GetStationDataRightsTreeDT(ByVal a_StationGUID As String, ByVal a_UserGUID As String, ByVal a_ObjectID As String) As DataTable
            Return GetStationDataRightsTreeDT(a_StationGUID, a_UserGUID, a_ObjectID, Nothing, Nothing)
        End Function

        Public Shared Function GetStationDataRightsTreeDT(ByVal a_StationGUID As String, ByVal a_UserGUID As String, ByVal a_ObjectID As String, ByVal a_SourceType As String) As DataTable
            Return GetStationDataRightsTreeDT(a_StationGUID, a_UserGUID, a_ObjectID, a_SourceType, Nothing)
        End Function

        Public Shared Function GetStationDataRightsTreeDT(ByVal a_StationGUID As String, ByVal a_UserGUID As String, ByVal a_ObjectID As String, ByVal a_SourceType As String, ByVal a_Application As String) As DataTable

            Dim dtTemp, dtObjectTree, dtUserObject, dtStationObject, dtReturn As DataTable
            Dim strSQL As New StringBuilder(1000)

            Dim strHierarchyCode, strBUFilt, strRowFilt As String
            Dim drTemp As DataRow
            Dim drObjects() As DataRow
            Dim i As Integer
            Dim blnIsHasBUGUID, blnIsHasIsShare As Boolean

            '当前用户的数据权限树
            dtObjectTree = DataRights.GetDataRightsDT(DataRightsDTType.RootTree, a_UserGUID, a_ObjectID, a_SourceType, a_Application)
            If dtObjectTree Is Nothing Then Return dtObjectTree

            '增加IsSelected列
            dtObjectTree.Columns.Add(New DataColumn("IsSelected", GetType(String)))
            If dtObjectTree.Rows.Count = 0 Then Return dtObjectTree

            blnIsHasBUGUID = dtObjectTree.Columns.Contains("_buguid")
            blnIsHasIsShare = dtObjectTree.Columns.Contains("_isshare")

            '目标岗位的已有权限集合
            'strSQL = "SELECT * FROM myStationObject WHERE StationGUID='" & a_StationGUID & "' AND ObjectType='" & a_ObjectID & "'"
            strSQL.Remove(0, strSQL.Length)
            '从岗位数据权限对象中获取
            strSQL.Append(" SELECT StationObjectGUID,StationGUID, ObjectType, ObjectGUID, EffectScope, TableName, BUGUID ")
            strSQL.Append(" FROM myStationObject AS so ")
            strSQL.Append(" WHERE StationGUID = '%StationGUID%' AND ObjectType = '%ObjectType%' ")

            ''从［组织］对［组织］关联表中获取全部子级公司与部门
            'strSQL.Append(" union ")
            'strSQL.Append(" Select newid(),'%StationGUID%', '%ObjectType%',SubOrgGuid,1 ")
            'strSQL.Append(" ,Case SubOrgType When 0 Then '公司' When 1 Then '部门' When 3 Then '团队' Else '' End  ")
            'strSQL.Append(" ,OrgGuid ")
            'strSQL.Append(" from MyOrgToOrg oo ")
            'strSQL.Append(" Where OrgGuid In ")
            'strSQL.Append(" ( ")
            'strSQL.Append(" SELECT ObjectGUID ")
            'strSQL.Append(" FROM myStationObject AS so ")
            'strSQL.Append(" WHERE StationGUID = '%StationGUID%' AND ObjectType = '%ObjectType%' ")
            'strSQL.Append(" ) ")

            ''［组织］对［组织］关联表中获取全部子级项目
            'strSQL.Append(" union ")
            'strSQL.Append(" Select  newid(),'%StationGUID%', '%ObjectType%',projguid ,1,'项目',buprojguid  ")
            'strSQL.Append(" from p_buproject2project oo ")
            'strSQL.Append(" Where buprojGuid In ")
            'strSQL.Append(" ( ")
            'strSQL.Append(" SELECT ObjectGUID ")
            'strSQL.Append(" FROM myStationObject AS so ")
            'strSQL.Append(" WHERE StationGUID = '%StationGUID%' AND   ObjectType = '%ObjectType%' ")
            'strSQL.Append(" ) ")

            strSQL.Replace("%StationGUID%", a_StationGUID) '岗位参数
            strSQL.Replace("%ObjectType%", a_ObjectID) 'ObjectID参数

            dtStationObject = MyDB.GetDataTable(strSQL.ToString())

            '目标岗位的所属公司本级及以下的数据权限树(只能授当前目标岗位所属物理公司本级及以下的数据权限)
            If blnIsHasBUGUID Then

                Dim dtBU As DataTable
                strHierarchyCode = MyDB.GetDataItemString("SELECT BU.HierarchyCode FROM myBusinessUnit BU INNER JOIN myStation S ON BU.BUGUID=S.CompanyGUID AND S.StationGUID='" & a_StationGUID & "'")

                strSQL.Remove(0, strSQL.Length)
                strSQL.Append("SELECT BUGUID FROM myBusinessUnit WHERE charindex('" & strHierarchyCode & ".',HierarchyCode+'.')=1")
                dtBU = MyDB.GetDataTable(strSQL.ToString())

                '从［组织］对［组织］关联表中获取当前岗位所在公司的全部子级节点（下属公司及部门）
                'strSQL.Remove(0, strSQL.Length)
                'strSQL.Append(" Select SubOrgGuid BUGUID from MyOrgToOrg ")
                'strSQL.Append(" Where OrgGuid IN (select CompanyGUID from myStation S Where StationGUID='")
                'strSQL.Append(a_StationGUID)
                'strSQL.Append("') ")
                'dtBU = MyDB.GetDataTable(strSQL.ToString())

                Dim sbBUFilter As New StringBuilder(100)
                If dtBU.Rows.Count > 0 Then
                    sbBUFilter.Append("_buguid in (")
                    For i = 0 To dtBU.Rows.Count - 1
                        If i = 0 Then
                            sbBUFilter.Append("'" & dtBU.Rows(i)("BUGUID").ToString & "'")
                        Else
                            sbBUFilter.Append(",'" & dtBU.Rows(i)("BUGUID").ToString & "'")
                        End If
                    Next
                    sbBUFilter.Append(")")
                Else
                    Return Nothing
                End If


                strBUFilt = sbBUFilter.ToString()
            Else
                strBUFilt = "1=1"
            End If
            drObjects = dtObjectTree.Select(strBUFilt, "_hierarchycode ASC")

            dtReturn = dtObjectTree.Clone()

            For Each dr As DataRow In drObjects

                '根据当前岗位的已有权限初始化
                strRowFilt = "StationGUID='" & a_StationGUID & _
                           "' and ObjectType='" & a_ObjectID & _
                           "' and ObjectGUID='" & dr("_guid").ToString & _
                           "' and TableName='" & dr("_sourcetype").ToString & _
                           "' "
                If blnIsHasBUGUID Then
                    strRowFilt &= " and BUGUID='" & dr("_buguid").ToString & "'"
                End If

                If dtStationObject.Select(strRowFilt).Length > 0 Then
                    dr("IsSelected") = "1"
                Else
                    dr("IsSelected") = "0"
                End If

                '把DataView输出到DataTable
                drTemp = dtReturn.NewRow
                drTemp.Item("_guid") = dr("_guid").ToString
                drTemp.Item("_name") = dr("_name").ToString
                drTemp.Item("_hierarchycode") = dr("_hierarchycode").ToString
                drTemp.Item("_sourcetype") = dr("_sourcetype").ToString
                drTemp.Item("_level") = dr("_level").ToString
                drTemp.Item("_isallowopr") = dr("_isallowopr").ToString
                drTemp.Item("IsSelected") = dr("IsSelected").ToString

                If blnIsHasBUGUID Then
                    drTemp.Item("_buguid") = dr("_buguid").ToString
                End If

                If blnIsHasIsShare Then
                    drTemp.Item("_isshare") = dr("_isshare").ToString
                End If

                dtReturn.Rows.Add(drTemp)
            Next

            Return dtReturn

        End Function



    End Class

End Namespace
