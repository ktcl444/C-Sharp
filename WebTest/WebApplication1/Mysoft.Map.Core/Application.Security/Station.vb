Imports Mysoft.Map.Data
Imports System.Xml
Imports System.Web
Imports System.Text
Imports System.Collections.Specialized
Imports System.Data.SqlClient
Imports Mysoft.Map.Application.Types

Namespace Application.Security
    Public Class Station


        '���ܣ���ȡ��Ӧ��ϵͳ-����ģ��-�����㡱������DataTable
        '������userGUID        -- ��ǰ������Ȩ���û�
        Public Shared Function GetStationActionRightsTreeDT(ByVal a_strStationGUID As String, ByVal a_strUserGUID As String, ByVal a_strApplicationList As String) As DataTable
            Dim strSQL As StringBuilder = New StringBuilder(4000)
            Dim dtActionTree As DataTable
            Dim strTemp, arrApplicationList(), strFilterLicensedFunc, strFilterFuncHidden, strFilterFuncActHidden, strFilterFuncActIncre As String
            If a_strStationGUID Is Nothing Then a_strStationGUID = Guid.NewGuid.ToString
            If a_strUserGUID Is Nothing Then a_strUserGUID = Guid.NewGuid.ToString
            arrApplicationList = a_strApplicationList.Split(",")
            For i As Integer = 0 To arrApplicationList.Length - 1
                '��ȡ��ɵĹ���ģ������ַ���(,�ָ�)
                strTemp = VerifyApplication.GetLicenseFunctionString(arrApplicationList(i))
                strFilterLicensedFunc &= IIf(strTemp = "", "", IIf(i = 0, "", ",") & strTemp)

                '��ȡ��ҵ��Ӱ����Ҫ���صĶ���Ĺ����ַ���(,�ָ�)
                '��ʽ�� 1�����ܣ�2������.����
                strTemp = VerifyApplication.GetNotInFilterString(1, arrApplicationList(i))
                strFilterFuncHidden &= IIf(strTemp = "", "", IIf(i = 0, "", ",") & strTemp)
                strTemp = VerifyApplication.GetNotInFilterString(2, arrApplicationList(i))
                strFilterFuncActHidden &= IIf(strTemp = "", "", IIf(i = 0, "", ",") & strTemp)

            Next

            '��ȡ����ֵģ��Ӱ���û����Ȩ�Ĺ��ܶ���������ַ���(,�ָ�)
            '��ʽ�����ܴ���.��������
            strFilterFuncActIncre = VerifyApplication.GetNotInFilterStringByIncrement()


            If User.IsAdmin(a_strUserGUID) Then      ' ����ǳ����û�
                strSQL.Append(" SELECT Application,'' AS FunctionCode,'' AS ActionCode,Application AS [Code],ApplicationName AS [Name],1 as [Level],IsLastStage,HierarchyCode,ParentHierarchyCode,'0' AS IsSelected,'1' AS IsAllowOpr,'����' AS FunctionType ")
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
                strSQL.Append("') AND FunctionType='����'))")
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
                strSQL.Append("') AND FC.FunctionType='����'))")
                strSQL.Append(vbCrLf)
                strSQL.Append(" ORDER BY Application,[Code]")

            Else        ' ������ǳ����û�
                strSQL.Append(" SELECT Application,'' AS FunctionCode,'' AS ActionCode,Application AS [Code],ApplicationName AS [Name],1 as [Level],IsLastStage,HierarchyCode,ParentHierarchyCode,'0' AS IsSelected,'1' AS IsAllowOpr,'����' AS FunctionType ")
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
                strSQL.Append("') AND FunctionType='����'))")
                strSQL.Append(vbCrLf)
                strSQL.Append(" UNION")
                strSQL.Append(vbCrLf)
                strSQL.Append(" SELECT Act.Application,Act.ObjectType AS FunctionCode,Act.ActionCode,FC.Application+'.'+FC.HierarchyCode+'.__'+Act.ActionCode AS [Code],Act.ActionName AS [Name],FC.Level+3 as [Level],'1' AS IsLastStage")
                strSQL.Append(vbCrLf)
                strSQL.Append(",Act.HierarchyCode,Act.ParentHierarchyCode")

                '��ǰѡ������жϹ���
                '1 �Ǳ��� �ж��Ƿ��и�λȨ��
                '2 ����   �жϹ���β��Ϊ"1999"[�������],�Ƿ���"ĳȫ��"Ȩ��,���б����"00"[�鿴]Ȩ��,��Ӧ�����е�"01"[�鿴ȫ��],��Ҫ����ת�崦��
                strSQL.Append(" ,CASE WHEN RR.ActionCode IS NOT NULL THEN 1 ")
                strSQL.Append("  When exists(select * from myStationRights sr1 ")
                strSQL.Append("   where sr1.ObjectType=Act.Application+'1999' ")
                strSQL.Append("   and sr1.ActionCode = case when Act.ActionCode='00' then '01' else Act.ActionCode end")
                strSQL.Append("   and FC.FunctionType='����' and sr1.StationGUID='")
                strSQL.Append(a_strStationGUID.Replace("'", "''"))
                strSQL.Append("') then 1")
                strSQL.Append(" ELSE 0 END IsSelected")
                'strSQL.Append(",CASE WHEN RR.ActionCode IS NULL THEN 0 ELSE 1 END IsSelected")
                strSQL.Append(vbCrLf)

                '��ǰ�ɲ�������жϹ���
                '1 �Ǳ��� �ж��Ƿ����û�Ȩ��
                '2 ����   �жϹ���β��Ϊ"1999"[�������],�Ƿ���"ĳȫ��"Ȩ��,���б����"00"[�鿴]Ȩ��,��Ӧ�����е�"01"[�鿴ȫ��],��Ҫ����ת�崦��
                strSQL.Append(" ,CASE WHEN UR.StationGUID IS NOT NULL THEN 1 ")
                strSQL.Append(" WHEN exists(select * from myUserRights ur1 ")
                strSQL.Append("   where ur1.ObjectType=Act.Application+'1999' ")
                strSQL.Append("   and ur1.ActionCode = case when Act.ActionCode='00' then '01' else Act.ActionCode end")
                strSQL.Append("   and FC.FunctionType='����' and ur1.UserGUID='")
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
                strSQL.Append("') AND FC.FunctionType='����'))")
                strSQL.Append(vbCrLf)
                strSQL.Append(" ORDER BY Application,[Code]")
            End If
            dtActionTree = MyDB.GetDataTable(strSQL.ToString())

            Return dtActionTree

        End Function


        '���ܣ���ȡ����Ȩ�޶��������DataTable
        '������userGUID        -- ��ǰ������Ȩ���û�
        'ע�⣺ ��Ҫ�����������ӡ���ǰ�û�Ȩ��+��ǰ��λ������˾�����ˣ������ݵ�ǰ��λ������Ȩ�޳�ʼ��
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

            '��ǰ�û�������Ȩ����
            dtObjectTree = DataRights.GetDataRightsDT(DataRightsDTType.RootTree, a_UserGUID, a_ObjectID, a_SourceType, a_Application)
            If dtObjectTree Is Nothing Then Return dtObjectTree

            '����IsSelected��
            dtObjectTree.Columns.Add(New DataColumn("IsSelected", GetType(String)))
            If dtObjectTree.Rows.Count = 0 Then Return dtObjectTree

            blnIsHasBUGUID = dtObjectTree.Columns.Contains("_buguid")
            blnIsHasIsShare = dtObjectTree.Columns.Contains("_isshare")

            'Ŀ���λ������Ȩ�޼���
            'strSQL = "SELECT * FROM myStationObject WHERE StationGUID='" & a_StationGUID & "' AND ObjectType='" & a_ObjectID & "'"
            strSQL.Remove(0, strSQL.Length)
            '�Ӹ�λ����Ȩ�޶����л�ȡ
            strSQL.Append(" SELECT StationObjectGUID,StationGUID, ObjectType, ObjectGUID, EffectScope, TableName, BUGUID ")
            strSQL.Append(" FROM myStationObject AS so ")
            strSQL.Append(" WHERE StationGUID = '%StationGUID%' AND ObjectType = '%ObjectType%' ")

            ''�ӣ���֯�ݶԣ���֯�ݹ������л�ȡȫ���Ӽ���˾�벿��
            'strSQL.Append(" union ")
            'strSQL.Append(" Select newid(),'%StationGUID%', '%ObjectType%',SubOrgGuid,1 ")
            'strSQL.Append(" ,Case SubOrgType When 0 Then '��˾' When 1 Then '����' When 3 Then '�Ŷ�' Else '' End  ")
            'strSQL.Append(" ,OrgGuid ")
            'strSQL.Append(" from MyOrgToOrg oo ")
            'strSQL.Append(" Where OrgGuid In ")
            'strSQL.Append(" ( ")
            'strSQL.Append(" SELECT ObjectGUID ")
            'strSQL.Append(" FROM myStationObject AS so ")
            'strSQL.Append(" WHERE StationGUID = '%StationGUID%' AND ObjectType = '%ObjectType%' ")
            'strSQL.Append(" ) ")

            ''����֯�ݶԣ���֯�ݹ������л�ȡȫ���Ӽ���Ŀ
            'strSQL.Append(" union ")
            'strSQL.Append(" Select  newid(),'%StationGUID%', '%ObjectType%',projguid ,1,'��Ŀ',buprojguid  ")
            'strSQL.Append(" from p_buproject2project oo ")
            'strSQL.Append(" Where buprojGuid In ")
            'strSQL.Append(" ( ")
            'strSQL.Append(" SELECT ObjectGUID ")
            'strSQL.Append(" FROM myStationObject AS so ")
            'strSQL.Append(" WHERE StationGUID = '%StationGUID%' AND   ObjectType = '%ObjectType%' ")
            'strSQL.Append(" ) ")

            strSQL.Replace("%StationGUID%", a_StationGUID) '��λ����
            strSQL.Replace("%ObjectType%", a_ObjectID) 'ObjectID����

            dtStationObject = MyDB.GetDataTable(strSQL.ToString())

            'Ŀ���λ��������˾���������µ�����Ȩ����(ֻ���ڵ�ǰĿ���λ��������˾���������µ�����Ȩ��)
            If blnIsHasBUGUID Then

                Dim dtBU As DataTable
                strHierarchyCode = MyDB.GetDataItemString("SELECT BU.HierarchyCode FROM myBusinessUnit BU INNER JOIN myStation S ON BU.BUGUID=S.CompanyGUID AND S.StationGUID='" & a_StationGUID & "'")

                strSQL.Remove(0, strSQL.Length)
                strSQL.Append("SELECT BUGUID FROM myBusinessUnit WHERE charindex('" & strHierarchyCode & ".',HierarchyCode+'.')=1")
                dtBU = MyDB.GetDataTable(strSQL.ToString())

                '�ӣ���֯�ݶԣ���֯�ݹ������л�ȡ��ǰ��λ���ڹ�˾��ȫ���Ӽ��ڵ㣨������˾�����ţ�
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

                '���ݵ�ǰ��λ������Ȩ�޳�ʼ��
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

                '��DataView�����DataTable
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
