Imports System.Text
Imports System.Xml
Imports System.Web
Imports System.Collections.Specialized
Imports Mysoft.Map.Data
Imports System.Data.SqlClient
Imports System.Windows.Forms
Imports Mysoft.Map.Application.Types

Namespace Application.Security

    ' ����Ȩ��
    Public Class DataRights
        ' ============================ ϵͳ����Ȩ�޽���ר�� =====================================
        ' ���ܣ������û�������˾�������¼���˾���˱��ʽ
        Public Shared Function GetUserCompanyDeepExp(ByVal a_UserGUID As String, ByVal a_KeyName As String) As String
            If a_UserGUID = "" Then Return "1=2"

            Dim strObjects As String
            strObjects = GetUserCompanyDeepString(a_UserGUID)

            If strObjects = "" Then
                Return "1=2"
            Else
                Return a_KeyName & " IN (" & strObjects & ")"
            End If
        End Function

        ' ���ܣ������û�������˾�������¼���˾�ַ���
        ' ���أ�'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx','xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx'����ַ���
        Public Shared Function GetUserCompanyDeepString(ByVal a_UserGUID As String) As String
            Dim dtBU As DataTable
            Dim strSQL As String
            Dim i As Integer
            Dim sbBU As New StringBuilder
            If a_UserGUID Is Nothing Then a_UserGUID = System.Guid.NewGuid().ToString()
            strSQL = "SELECT BUGUID FROM myBusinessUnit WHERE charindex((select HierarchyCode+'.' from myBusinessUnit" & _
                                                                        " where BUGUID=(select BUGUID from myUser" & _
                                                                        " where UserGUID='" & a_UserGUID.Replace("'", "''") & "'))" & _
                                                                        ",HierarchyCode+'.')=1"
            dtBU = MyDB.GetDataTable(strSQL)
            If dtBU.Rows.Count > 0 Then
                For i = 0 To dtBU.Rows.Count - 1
                    If i = 0 Then
                        sbBU.Append("'" & dtBU.Rows(i)("BUGUID").ToString & "'")
                    Else
                        sbBU.Append(",'" & dtBU.Rows(i)("BUGUID").ToString & "'")
                    End If
                Next
            End If

            Return sbBU.ToString
        End Function

        ' ============================ Public Method =====================================

        ' ���ܣ��ж��û��Ƿ��и�����Ȩ��
        ' ������a_UserGUID          -- �û� GUID������
        '       a_ObjectID          -- ��Դ����Ψһ��ʶ id������
        '       a_strObjectGUID     -- �������ͣ�����
        Public Shared Function IsHaveDataRights(ByVal a_UserGUID As String, ByVal a_ObjectID As String, ByVal a_strObjectGUID As String) As Boolean
            Dim bReturn As Boolean
            Dim dtDataRights As DataTable
            Dim i As Integer

            bReturn = False
            a_strObjectGUID = a_strObjectGUID.ToLower
            dtDataRights = GetDataRightsDT(DataRightsDTType.Basic, a_UserGUID, a_ObjectID)
            If dtDataRights.Rows.Count > 0 Then
                For i = 0 To dtDataRights.Rows.Count - 1
                    If dtDataRights.Rows(i)("_guid").ToString.ToLower = a_strObjectGUID Then
                        bReturn = True
                        Exit For
                    End If
                Next
            End If

            Return bReturn
        End Function

        ' ���ܣ���ȡ�û�����Ȩ�޹��˱��ʽ
        ' ������a_UserGUID          -- �û� GUID������
        '       a_ObjectID          -- ��Դ����Ψһ��ʶ id�������Ӧ /bin/DataRights.xml �е� object.id ����
        '       a_KeyName           -- ��Դ����������ֶΣ������Ӧ _guid �ֶ�
        '       a_SourceType        -- ������Դ����ѡ����Ӧ _sourcetype �ֶ�
        '       a_Application       -- Ӧ��ϵͳ����ѡ
        Public Shared Function GetDataRightsExp(ByVal a_UserGUID As String, ByVal a_ObjectID As String, ByVal a_KeyName As String) As String
            Return GetDataRightsExp(a_UserGUID, a_ObjectID, a_KeyName, Nothing)
        End Function

        Public Shared Function GetDataRightsExp(ByVal a_UserGUID As String, ByVal a_ObjectID As String, ByVal a_KeyName As String, ByVal a_SourceType As String) As String
            Return GetDataRightsExp(a_UserGUID, a_ObjectID, a_KeyName, a_SourceType, Nothing)
        End Function

        Public Shared Function GetDataRightsExp(ByVal a_UserGUID As String, ByVal a_ObjectID As String, ByVal a_KeyName As String, ByVal a_SourceType As String, ByVal a_Application As String) As String
            If a_UserGUID = "" Then Return "1=2"

            Dim strObjects As String
            strObjects = GetDataRightsString(a_UserGUID, a_ObjectID, a_SourceType, a_Application)

            If strObjects = "" Then
                Return "1=2"
            Else
                Return a_KeyName & " IN (" & strObjects & ")"
            End If
        End Function


        ' ���ܣ���ȡ�û�����Ȩ���ַ���
        ' ���أ�'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx','xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx'����ַ���
        ' ������a_UserGUID          -- �û� GUID������
        '       a_ObjectID          -- ��Դ����Ψһ��ʶ id�������Ӧ /bin/DataRights.xml �е� object.id ����
        '       a_SourceType        -- ������Դ����ѡ����Ӧ _sourcetype �ֶ�
        '       a_Application       -- Ӧ��ϵͳ����ѡ
        ' ע�⣺���ڹ�����Ŀ��ʵ����Ŀ�� ProjGUID ��ͬ�����ԣ�������ڷ��� ProjGUID �޷���ʵ��ӳ����������©����
        Public Shared Function GetDataRightsString(ByVal a_UserGUID As String, ByVal a_ObjectID As String) As String
            Return GetDataRightsString(a_UserGUID, a_ObjectID, Nothing)
        End Function

        Public Shared Function GetDataRightsString(ByVal a_UserGUID As String, ByVal a_ObjectID As String, ByVal a_SourceType As String) As String
            Return GetDataRightsString(a_UserGUID, a_ObjectID, a_SourceType, Nothing)
        End Function

        Public Shared Function GetDataRightsString(ByVal a_UserGUID As String, ByVal a_ObjectID As String, ByVal a_SourceType As String, ByVal a_Application As String) As String
            Dim dtDataRights As DataTable
            Dim sbReturn As New StringBuilder
            Dim i As Integer

            dtDataRights = GetDataRightsDT(DataRightsDTType.Basic, a_UserGUID, a_ObjectID, a_SourceType, a_Application)
            If dtDataRights.Rows.Count > 0 Then
                For i = 0 To dtDataRights.Rows.Count - 1
                    If i = 0 Then
                        sbReturn.Append("'" & dtDataRights.Rows(i)("_guid").ToString & "'")
                    Else
                        sbReturn.Append(",'" & dtDataRights.Rows(i)("_guid").ToString & "'")
                    End If
                Next
            End If

            Return sbReturn.ToString

        End Function


        ' ���ܣ���ȡ��Ȩ�޵����ݼ�
        ' ������a_DataRightsDTType		���� �������͡�
        ' 	    a_UserGUID		        ���� �û�GUID��
        ' 	    a_ObjectID		        ���� ��Ȩ����Ψһ��ʶID������磺project�����
        ' 	    a_SourceType		    ���� �������ͣ�����ʶ��ǰ������ʲô���磺��˾����Ŀ�ȡ�������ֶ�Ϊ�գ����ص����ݼ�����������Ȩ�޵����������������͵����ݡ�
        ' 	    a_Application		    ���� ����ϵͳ����Ҫ����ȡ��Ŀ��Ȩʱ���Ƿ�ϵͳ���˵�������Ŀ��
        ' ���أ�DataTable������������������Դ�ж�����ֶ��⣬���������Ƿ���Ȩ���ֶ�_isallowopr������Ȩ�޵Ľڵ�Ϊ1������Ľڵ�Ϊ0��
        ' ע�����������Ĵ���������� Nothing ��ʾ��������ʱ��������û�û��Ȩ�ޣ���������Ϊ 0 �Ľ������
        Public Shared Function GetDataRightsDT(ByVal a_DataRightsDTType As DataRightsDTType, ByVal a_UserGUID As String, ByVal a_ObjectID As String) As DataTable
            Return GetDataRightsDT(a_DataRightsDTType, a_UserGUID, a_ObjectID, Nothing, Nothing)
        End Function

        Public Shared Function GetDataRightsDT(ByVal a_DataRightsDTType As DataRightsDTType, ByVal a_UserGUID As String, ByVal a_ObjectID As String, _
                                                        ByVal a_SourceType As String) As DataTable
            Return GetDataRightsDT(a_DataRightsDTType, a_UserGUID, a_ObjectID, a_SourceType, Nothing)
        End Function

        Public Shared Function GetDataRightsDT(ByVal a_DataRightsDTType As DataRightsDTType, ByVal a_UserGUID As String, ByVal a_ObjectID As String, _
                                        ByVal a_SourceType As String, ByVal a_Application As String) As DataTable
            Dim SqlConn As New SqlConnection(MyDB.GetSqlConnectionString)
            Dim ds As New DataSet

            Try
                '��������	    
                Dim SqlComm As SqlCommand
                Dim SqlAdpt As SqlDataAdapter
                Dim SqlParams As SqlParameterCollection

                SqlComm = New SqlCommand("usp_myGetDataRightsDT", SqlConn)
                SqlComm.CommandType = CommandType.StoredProcedure

                SqlParams = SqlComm.Parameters
                SqlParams.Add(New SqlParameter("@chvDataRightsDTType", SqlDbType.NVarChar, 50))
                SqlParams.Add(New SqlParameter("@chvUserGUID", SqlDbType.NVarChar, 40))
                SqlParams.Add(New SqlParameter("@chvObjectID", SqlDbType.NVarChar, 100))
                SqlParams.Add(New SqlParameter("@chvSourceType", SqlDbType.NVarChar, 100))
                SqlParams.Add(New SqlParameter("@chvApplication", SqlDbType.NVarChar, 100))
                SqlParams.Add(New SqlParameter("@chvErrorInfo", SqlDbType.NVarChar, 100))

                SqlParams("@chvDataRightsDTType").Value = a_DataRightsDTType
                SqlParams("@chvUserGUID").Value = a_UserGUID
                SqlParams("@chvObjectID").Value = a_ObjectID
                SqlParams("@chvSourceType").Value = DBNull.Value
                SqlParams("@chvApplication").Value = DBNull.Value
                SqlParams("@chvErrorInfo").Value = ""

                SqlParams("@chvErrorInfo").Direction = ParameterDirection.Output

                '��ȡ����
                SqlConn.Open()
                SqlAdpt = New SqlDataAdapter
                SqlAdpt.SelectCommand = SqlComm
                SqlAdpt.Fill(ds)

                ds.Tables(ds.Tables.Count - 1).TableName = "DataRights"

                Return ds.Tables(ds.Tables.Count - 1)

            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return Nothing
            Finally
                '�ر�����
                SqlConn.Close()
            End Try
        End Function

#Region "    �ɰ�GetDataRightsDT"

        'Public Shared Function GetDataRightsDT(ByVal a_DataRightsDTType As DataRightsDTType, ByVal a_UserGUID As String, ByVal a_ObjectID As String, _
        '                                                ByVal a_SourceType As String, ByVal a_Application As String) As DataTable
        '    Dim dtAllObject, dtUO, dtReturn As DataTable
        '    Dim dvAllObject As DataView
        '    Dim drTemp, arrDr() As DataRow
        '    Dim strDataType, strCodeSec, strCodeAll, strSQL, strFilt, strUserBUDeepFilt As String
        '    Dim i, j, intTopLevel As Integer
        '    Dim blnIsHasBUGUID, blnIsHasIsShare, blnIsOutput, blnIsAllowOpr As Boolean

        '    '������Ȩ������
        '    Dim xmlDoc As XmlDocument
        '    xmlDoc = GetDataRightsXml()
        '    If xmlDoc Is Nothing Then Return Nothing

        '    Dim xmlObject As XmlNode
        '    xmlObject = xmlDoc.SelectSingleNode("/objects/object[@id='" & a_ObjectID & "']")

        '    If xmlObject Is Nothing Then Return Nothing

        '    '��ȡ����Դ��Ŀǰֻ֧�� SQL
        '    strDataType = xmlObject.SelectSingleNode("datatype").InnerText
        '    Select Case strDataType.ToUpper
        '        Case "SQL"
        '            strSQL = xmlObject.SelectSingleNode("datasource").InnerText
        '            dtAllObject = MyDB.GetDataTable(strSQL)

        '        Case Else
        '            Return Nothing
        '    End Select

        '    blnIsHasBUGUID = dtAllObject.Columns.Contains("_buguid")         '�Ƿ��幫˾GUID����
        '    blnIsHasIsShare = dtAllObject.Columns.Contains("_isshare")       '�Ƿ����Ƿ�����Ŀ������֧�ֹ�����Ŀ�����ã�

        '    dtAllObject.Columns.Add(New DataColumn("_isallowopr", GetType(String)))
        '    dtAllObject.Columns.Add(New DataColumn("IsShow", GetType(String)))

        '    dvAllObject = New DataView(dtAllObject)

        '    If User.IsAdmin(a_UserGUID) Then
        '        '����Ա��ӵ�����е�����Ȩ��
        '        For i = 0 To dvAllObject.Count - 1
        '            dvAllObject(i)("_isallowopr") = "1"
        '            dvAllObject(i)("IsShow") = "1"
        '        Next
        '    Else

        '        '��ͨ�û���ֻ�������е�����Ȩ��
        '        strSQL = "SELECT UserObjectGUID,UserGUID,StationGUID,ObjectType,ObjectGUID,TableName,BUGUID" & _
        '                  " FROM myUserObject" & _
        '                  " WHERE ObjectType = '" & a_ObjectID.Replace("'", "''") & "'" & _
        '                  " AND UserGUID = '" & a_UserGUID.Replace("'", "''") & "'"
        '        dtUO = MyDB.GetDataTable(strSQL)

        '        'intTopLevel = 99               '��ʱ������LevelTree�����

        '        ' ��ֵ IsShow �� _isallowopr �ֶ�
        '        For i = 0 To dtUO.Rows.Count - 1
        '            strFilt = "_guid='" & dtUO.Rows(i).Item("ObjectGUID").ToString & "'"
        '            If blnIsHasBUGUID Then
        '                strFilt &= " AND _buguid='" & dtUO.Rows(i).Item("buguid").ToString & "'"
        '            End If
        '            arrDr = dtAllObject.Select(strFilt)

        '            If arrDr.Length > 0 Then            'myUserObject���ܴ�����������
        '                drTemp = arrDr(0)
        '                strCodeSec = drTemp("_hierarchycode").ToString
        '                'intTopLevel = IIf(CInt(drTemp("_level")) < intTopLevel, CInt(drTemp("_level")), intTopLevel)

        '                '�������¼�
        '                dvAllObject.RowFilter = "_hierarchycode + '.' LIKE '" & strCodeSec & ".%'"
        '                For j = 0 To dvAllObject.Count - 1
        '                    dvAllObject(j)("IsShow") = "1"
        '                    dvAllObject(j)("_isallowopr") = "1"
        '                Next

        '                '�ϼ�
        '                dvAllObject.RowFilter = "'" & strCodeSec & ".' LIKE _hierarchycode + '.%'" & _
        '                                        " AND _hierarchycode + '.' <> '" & strCodeSec & ".'"
        '                For j = 0 To dvAllObject.Count - 1
        '                    dvAllObject(j)("IsShow") = "1"
        '                Next

        '            End If

        '        Next

        '    End If


        '    '������������
        '    dtReturn = dtAllObject.Clone

        '    strFilt = "IsShow = '1'"

        '    If User.IsAdmin(a_UserGUID) Then
        '        '����ǹ���Ա��ֻ���û�������˾�� Deep Ȩ�ޣ�������DataRightsDTType��
        '        strUserBUDeepFilt = GetUserCompanyDeepString(a_UserGUID)
        '        If strUserBUDeepFilt <> "" Then
        '            strFilt &= " AND (_buguid=" & strUserBUDeepFilt.Replace("','", "' OR _buguid='") & ")"
        '        End If
        '    Else
        '        Select Case a_DataRightsDTType
        '            Case DataRightsDTType.Basic
        '                strFilt &= " AND _isallowopr = '1'"
        '                'Case DataRightsDTType.LevelTree                '��ʱ������LevelTree�����
        '                '    strFilt &= " AND _level >= " & intTopLevel
        '            Case DataRightsDTType.UserBUDeepTree
        '                strUserBUDeepFilt = GetUserCompanyDeepString(a_UserGUID)
        '                If strUserBUDeepFilt <> "" Then
        '                    strFilt &= " AND (_buguid=" & strUserBUDeepFilt.Replace("','", "' OR _buguid='") & ")"
        '                End If
        '        End Select
        '    End If

        '    If a_Application <> "" AndAlso a_Application <> "0102" AndAlso blnIsHasIsShare Then     '��������Ŀ���⣨����ǿͷ�ϵͳ(0102)����Ҫ���˵��������ݣ�
        '        strFilt &= " AND _isshare=0"
        '    End If


        '    dvAllObject.RowFilter = strFilt

        '    For i = 0 To dvAllObject.Count - 1
        '        blnIsOutput = True
        '        blnIsAllowOpr = True

        '        '������������
        '        If a_SourceType <> "" AndAlso dvAllObject(i)("_sourcetype").ToString <> a_SourceType Then
        '            strFilt = "_sourcetype='" & a_SourceType.Replace("'", "''") & "'" & _
        '                      " AND _hierarchycode+'.' like '" & dvAllObject(i)("_hierarchycode").ToString & ".%'"

        '            If dtAllObject.Select(strFilt).Length > 0 Then
        '                blnIsAllowOpr = False          '����ýڵ�����ӽڵ㣬����ɾ����ֻ����Ϊ���ɲ���
        '            Else
        '                blnIsOutput = False
        '            End If
        '        End If

        '        '���
        '        If blnIsOutput Then
        '            drTemp = dtReturn.NewRow()
        '            For j = 0 To dtReturn.Columns.Count - 1
        '                drTemp(j) = dvAllObject(i)(j)
        '            Next

        '            If IsDBNull(drTemp("_isallowopr")) OrElse Not blnIsAllowOpr Then
        '                drTemp("_isallowopr") = "0"
        '            End If

        '            dtReturn.Rows.Add(drTemp)
        '        End If

        '    Next

        '    Return dtReturn

        'End Function

#End Region


        '���ܣ���DataRights.xml�еĶ���ת��ΪDatatable��ʽ
        Public Shared Function TransDataRightsXML2DT() As DataTable

            Dim xmlDoc As XmlDocument
            xmlDoc = GetDataRightsXml()
            If xmlDoc Is Nothing Then Return Nothing

            Dim xmlObjects As XmlNodeList
            xmlObjects = xmlDoc.SelectNodes("/objects/object[@id]")

            If xmlObjects.Count = 0 Then Return Nothing

            Dim dtObjects As New DataTable("Objects")
            Try
                '�����
                dtObjects.Columns.Add(New DataColumn("id", GetType(String)))
                dtObjects.Columns.Add(New DataColumn("objectname", GetType(String)))
                dtObjects.Columns.Add(New DataColumn("datatype", GetType(String)))
                dtObjects.Columns.Add(New DataColumn("datasource", GetType(String)))
                dtObjects.Columns.Add(New DataColumn("memo", GetType(String)))

                '�����
                Dim dr As DataRow
                For i As Integer = 0 To xmlObjects.Count - 1
                    dr = dtObjects.NewRow()
                    dr("id") = xmlObjects(i).Attributes.GetNamedItem("id").Value
                    dr("objectname") = xmlObjects(i).SelectSingleNode("./objectname").InnerText
                    dr("datatype") = xmlObjects(i).SelectSingleNode("./datatype").InnerText
                    dr("datasource") = xmlObjects(i).SelectSingleNode("./datasource").InnerText
                    If Not xmlObjects(i).SelectSingleNode("./memo") Is Nothing Then
                        dr("memo") = xmlObjects(i).SelectSingleNode("./memo").InnerText
                    End If
                    dtObjects.Rows.Add(dr)
                Next

                Return dtObjects
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return Nothing
            End Try

        End Function


        ' ============================ Friend Method =====================================

        ' ���ܣ���ȡ������Ȩ������
        ' ���أ�XmlDocument ����
        Public Shared Function GetDataRightsXml() As XmlDocument

            Dim key As String = "DataRights.xml"
            Dim xmlDoc As XmlDocument
            Dim SqlConn As New SqlConnection(MyDB.GetSqlConnectionString)

            Try
                xmlDoc = Mysoft.Map.Caching.MyCache.Get(key)

                If xmlDoc Is Nothing Then

                    '��������	    
                    Dim SqlComm As SqlCommand
                    Dim SqlAdpt As SqlDataAdapter
                    Dim SqlParams As SqlParameterCollection

                    SqlComm = New SqlCommand("usp_myGetDataRightsXML", SqlConn)
                    SqlComm.CommandType = CommandType.StoredProcedure

                    SqlParams = SqlComm.Parameters
                    SqlParams.Add(New SqlParameter("@chvXML", SqlDbType.NVarChar, 4000))

                    SqlParams("@chvXML").Direction = ParameterDirection.Output

                    '��ȡ����
                    SqlConn.Open()
                    SqlComm.ExecuteNonQuery()
                    SqlConn.Close()

                    If SqlParams("@chvXML").Value Is DBNull.Value Then
                        Return Nothing
                    Else
                        xmlDoc = New XmlDocument
                        xmlDoc.LoadXml("<?xml version=""1.0"" encoding=""gb2312"" ?>" & SqlParams("@chvXML").Value)
                        Mysoft.Map.Caching.MyCache.Insert(key, xmlDoc)
                    End If

                End If

                Return xmlDoc

            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return Nothing
            Finally
                '�ر�����
                SqlConn.Close()
            End Try

        End Function


        ' ============================ Private Method =====================================

        '' ���ܣ���ȡ���û�������˾ Deep ����Ȩ���ַ�����ϵͳ����ģ��ר�ã�
        '' ���أ�'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx','xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx'����ַ���
        '' ������a_UserGUID          -- �û� GUID������
        ''       a_ObjectID          -- ��Դ����Ψһ��ʶ id�������Ӧ /bin/DataRights.xml �е� object.id ����
        ''       a_SourceType        -- ������Դ����ѡ����Ӧ _sourcetype �ֶ�
        ''       a_Application       -- Ӧ��ϵͳ����ѡ
        '' ע�⣺���ڹ�����Ŀ��ʵ����Ŀ�� ProjGUID ��ͬ�����ԣ�������ڷ��� ProjGUID �޷���ʵ��ӳ����������©����
        'Private Shared Function GetBUDeepDataRightsString(ByVal a_UserGUID As String, ByVal a_ObjectID As String) As String
        '    Return GetBUDeepDataRightsString(a_UserGUID, a_ObjectID, Nothing)
        'End Function

        'Private Shared Function GetBUDeepDataRightsString(ByVal a_UserGUID As String, ByVal a_ObjectID As String, ByVal a_SourceType As String) As String
        '    Return GetBUDeepDataRightsString(a_UserGUID, a_ObjectID, a_SourceType, Nothing)
        'End Function

        'Private Shared Function GetBUDeepDataRightsString(ByVal a_UserGUID As String, ByVal a_ObjectID As String, ByVal a_SourceType As String, ByVal a_Application As String) As String
        '    Dim dtDataRights As DataTable
        '    Dim sbReturn As New StringBuilder
        '    Dim i As Integer
        '    Dim strBUGUID As String

        '    strBUGUID = MyDB.GetDataItemString("SELECT BUGUID FROM myUser WHERE UserGUID='" & a_UserGUID & "'")
        '    dtDataRights = GetBUDeepDataRightsDT(strBUGUID, a_ObjectID, a_SourceType, a_Application)
        '    If dtDataRights.Rows.Count > 0 Then
        '        For i = 0 To dtDataRights.Rows.Count - 1
        '            If i = 0 Then
        '                sbReturn.Append("'" & dtDataRights.Rows(i)("_guid").ToString & "'")
        '            Else
        '                sbReturn.Append(",'" & dtDataRights.Rows(i)("_guid").ToString & "'")
        '            End If
        '        Next
        '    End If

        '    Return sbReturn.ToString

        'End Function

    End Class

End Namespace
