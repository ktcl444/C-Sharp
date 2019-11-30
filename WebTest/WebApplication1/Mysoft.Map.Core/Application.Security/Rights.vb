Imports System.Text
Imports System.Xml
Imports System.Web
Imports System.Collections.Specialized
Imports Mysoft.Map.Data
Imports System.Data.SqlClient
Imports System.Windows.Forms
Imports Mysoft.Map.Application.Types

Namespace Application.Security

    ' 数据权限
    Public Class DataRights
        ' ============================ 系统管理权限解释专用 =====================================
        ' 功能：返回用户所属公司及所有下级公司过滤表达式
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

        ' 功能：返回用户所属公司及所有下级公司字符串
        ' 返回：'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx','xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx'或空字符串
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

        ' 功能：判断用户是否有该数据权限
        ' 参数：a_UserGUID          -- 用户 GUID，必填
        '       a_ObjectID          -- 资源对象唯一标识 id，必填
        '       a_strObjectGUID     -- 对象类型，必填
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

        ' 功能：获取用户数据权限过滤表达式
        ' 参数：a_UserGUID          -- 用户 GUID，必填
        '       a_ObjectID          -- 资源对象唯一标识 id，必填。对应 /bin/DataRights.xml 中的 object.id 属性
        '       a_KeyName           -- 资源对象的主键字段，必填。对应 _guid 字段
        '       a_SourceType        -- 数据来源，可选。对应 _sourcetype 字段
        '       a_Application       -- 应用系统，可选
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


        ' 功能：获取用户数据权限字符串
        ' 返回：'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx','xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx'或空字符串
        ' 参数：a_UserGUID          -- 用户 GUID，必填
        '       a_ObjectID          -- 资源对象唯一标识 id，必填。对应 /bin/DataRights.xml 中的 object.id 属性
        '       a_SourceType        -- 数据来源，可选。对应 _sourcetype 字段
        '       a_Application       -- 应用系统，可选
        ' 注意：由于共享项目和实际项目的 ProjGUID 相同，所以，这里存在返回 ProjGUID 无法真实反映，有这样的漏洞。
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


        ' 功能：获取有权限的数据集
        ' 参数：a_DataRightsDTType		—— 返回类型。
        ' 	    a_UserGUID		        —— 用户GUID。
        ' 	    a_ObjectID		        —— 授权对象唯一标识ID，必填，如：project。必填。
        ' 	    a_SourceType		    —— 数据类型，用于识别当前数据是什么，如：公司、项目等。如果该字段为空，返回的数据集包含所有有权限的数据类型数据类型的数据。
        ' 	    a_Application		    —— 所属系统。主要用于取项目授权时，是否按系统过滤掉共享项目。
        ' 返回：DataTable，除包含所有在数据源中定义的字段外，还增加了是否有权限字段_isallowopr——有权限的节点为1，补齐的节点为0。
        ' 注意对以下情况的处理：如果返回 Nothing 表示函数运行时错误。如果用户没有权限，返回行数为 0 的结果集。
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
                '建立连接	    
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

                '获取数据
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
                '关闭连接
                SqlConn.Close()
            End Try
        End Function

#Region "    旧版GetDataRightsDT"

        'Public Shared Function GetDataRightsDT(ByVal a_DataRightsDTType As DataRightsDTType, ByVal a_UserGUID As String, ByVal a_ObjectID As String, _
        '                                                ByVal a_SourceType As String, ByVal a_Application As String) As DataTable
        '    Dim dtAllObject, dtUO, dtReturn As DataTable
        '    Dim dvAllObject As DataView
        '    Dim drTemp, arrDr() As DataRow
        '    Dim strDataType, strCodeSec, strCodeAll, strSQL, strFilt, strUserBUDeepFilt As String
        '    Dim i, j, intTopLevel As Integer
        '    Dim blnIsHasBUGUID, blnIsHasIsShare, blnIsOutput, blnIsAllowOpr As Boolean

        '    '加载授权对象定义
        '    Dim xmlDoc As XmlDocument
        '    xmlDoc = GetDataRightsXml()
        '    If xmlDoc Is Nothing Then Return Nothing

        '    Dim xmlObject As XmlNode
        '    xmlObject = xmlDoc.SelectSingleNode("/objects/object[@id='" & a_ObjectID & "']")

        '    If xmlObject Is Nothing Then Return Nothing

        '    '获取数据源，目前只支持 SQL
        '    strDataType = xmlObject.SelectSingleNode("datatype").InnerText
        '    Select Case strDataType.ToUpper
        '        Case "SQL"
        '            strSQL = xmlObject.SelectSingleNode("datasource").InnerText
        '            dtAllObject = MyDB.GetDataTable(strSQL)

        '        Case Else
        '            Return Nothing
        '    End Select

        '    blnIsHasBUGUID = dtAllObject.Columns.Contains("_buguid")         '是否定义公司GUID列名
        '    blnIsHasIsShare = dtAllObject.Columns.Contains("_isshare")       '是否定义是否共享项目（特殊支持共享项目过滤用）

        '    dtAllObject.Columns.Add(New DataColumn("_isallowopr", GetType(String)))
        '    dtAllObject.Columns.Add(New DataColumn("IsShow", GetType(String)))

        '    dvAllObject = New DataView(dtAllObject)

        '    If User.IsAdmin(a_UserGUID) Then
        '        '管理员，拥有所有的数据权限
        '        For i = 0 To dvAllObject.Count - 1
        '            dvAllObject(i)("_isallowopr") = "1"
        '            dvAllObject(i)("IsShow") = "1"
        '        Next
        '    Else

        '        '普通用户，只能授已有的数据权限
        '        strSQL = "SELECT UserObjectGUID,UserGUID,StationGUID,ObjectType,ObjectGUID,TableName,BUGUID" & _
        '                  " FROM myUserObject" & _
        '                  " WHERE ObjectType = '" & a_ObjectID.Replace("'", "''") & "'" & _
        '                  " AND UserGUID = '" & a_UserGUID.Replace("'", "''") & "'"
        '        dtUO = MyDB.GetDataTable(strSQL)

        '        'intTopLevel = 99               '暂时不考虑LevelTree的情况

        '        ' 赋值 IsShow 和 _isallowopr 字段
        '        For i = 0 To dtUO.Rows.Count - 1
        '            strFilt = "_guid='" & dtUO.Rows(i).Item("ObjectGUID").ToString & "'"
        '            If blnIsHasBUGUID Then
        '                strFilt &= " AND _buguid='" & dtUO.Rows(i).Item("buguid").ToString & "'"
        '            End If
        '            arrDr = dtAllObject.Select(strFilt)

        '            If arrDr.Length > 0 Then            'myUserObject可能存在垃圾数据
        '                drTemp = arrDr(0)
        '                strCodeSec = drTemp("_hierarchycode").ToString
        '                'intTopLevel = IIf(CInt(drTemp("_level")) < intTopLevel, CInt(drTemp("_level")), intTopLevel)

        '                '本级及下级
        '                dvAllObject.RowFilter = "_hierarchycode + '.' LIKE '" & strCodeSec & ".%'"
        '                For j = 0 To dvAllObject.Count - 1
        '                    dvAllObject(j)("IsShow") = "1"
        '                    dvAllObject(j)("_isallowopr") = "1"
        '                Next

        '                '上级
        '                dvAllObject.RowFilter = "'" & strCodeSec & ".' LIKE _hierarchycode + '.%'" & _
        '                                        " AND _hierarchycode + '.' <> '" & strCodeSec & ".'"
        '                For j = 0 To dvAllObject.Count - 1
        '                    dvAllObject(j)("IsShow") = "1"
        '                Next

        '            End If

        '        Next

        '    End If


        '    '输出到结果集中
        '    dtReturn = dtAllObject.Clone

        '    strFilt = "IsShow = '1'"

        '    If User.IsAdmin(a_UserGUID) Then
        '        '如果是管理员，只出用户所属公司的 Deep 权限，不考虑DataRightsDTType。
        '        strUserBUDeepFilt = GetUserCompanyDeepString(a_UserGUID)
        '        If strUserBUDeepFilt <> "" Then
        '            strFilt &= " AND (_buguid=" & strUserBUDeepFilt.Replace("','", "' OR _buguid='") & ")"
        '        End If
        '    Else
        '        Select Case a_DataRightsDTType
        '            Case DataRightsDTType.Basic
        '                strFilt &= " AND _isallowopr = '1'"
        '                'Case DataRightsDTType.LevelTree                '暂时不考虑LevelTree的情况
        '                '    strFilt &= " AND _level >= " & intTopLevel
        '            Case DataRightsDTType.UserBUDeepTree
        '                strUserBUDeepFilt = GetUserCompanyDeepString(a_UserGUID)
        '                If strUserBUDeepFilt <> "" Then
        '                    strFilt &= " AND (_buguid=" & strUserBUDeepFilt.Replace("','", "' OR _buguid='") & ")"
        '                End If
        '        End Select
        '    End If

        '    If a_Application <> "" AndAlso a_Application <> "0102" AndAlso blnIsHasIsShare Then     '处理共享项目问题（如果非客服系统(0102)，需要过滤掉共享数据）
        '        strFilt &= " AND _isshare=0"
        '    End If


        '    dvAllObject.RowFilter = strFilt

        '    For i = 0 To dvAllObject.Count - 1
        '        blnIsOutput = True
        '        blnIsAllowOpr = True

        '        '过滤数据类型
        '        If a_SourceType <> "" AndAlso dvAllObject(i)("_sourcetype").ToString <> a_SourceType Then
        '            strFilt = "_sourcetype='" & a_SourceType.Replace("'", "''") & "'" & _
        '                      " AND _hierarchycode+'.' like '" & dvAllObject(i)("_hierarchycode").ToString & ".%'"

        '            If dtAllObject.Select(strFilt).Length > 0 Then
        '                blnIsAllowOpr = False          '如果该节点存在子节点，则不能删除，只能置为不可操作
        '            Else
        '                blnIsOutput = False
        '            End If
        '        End If

        '        '输出
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


        '功能：把DataRights.xml中的定义转换为Datatable格式
        Public Shared Function TransDataRightsXML2DT() As DataTable

            Dim xmlDoc As XmlDocument
            xmlDoc = GetDataRightsXml()
            If xmlDoc Is Nothing Then Return Nothing

            Dim xmlObjects As XmlNodeList
            xmlObjects = xmlDoc.SelectNodes("/objects/object[@id]")

            If xmlObjects.Count = 0 Then Return Nothing

            Dim dtObjects As New DataTable("Objects")
            Try
                '添加列
                dtObjects.Columns.Add(New DataColumn("id", GetType(String)))
                dtObjects.Columns.Add(New DataColumn("objectname", GetType(String)))
                dtObjects.Columns.Add(New DataColumn("datatype", GetType(String)))
                dtObjects.Columns.Add(New DataColumn("datasource", GetType(String)))
                dtObjects.Columns.Add(New DataColumn("memo", GetType(String)))

                '添加行
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

        ' 功能：获取数据授权对象定义
        ' 返回：XmlDocument 类型
        Public Shared Function GetDataRightsXml() As XmlDocument

            Dim key As String = "DataRights.xml"
            Dim xmlDoc As XmlDocument
            Dim SqlConn As New SqlConnection(MyDB.GetSqlConnectionString)

            Try
                xmlDoc = Mysoft.Map.Caching.MyCache.Get(key)

                If xmlDoc Is Nothing Then

                    '建立连接	    
                    Dim SqlComm As SqlCommand
                    Dim SqlAdpt As SqlDataAdapter
                    Dim SqlParams As SqlParameterCollection

                    SqlComm = New SqlCommand("usp_myGetDataRightsXML", SqlConn)
                    SqlComm.CommandType = CommandType.StoredProcedure

                    SqlParams = SqlComm.Parameters
                    SqlParams.Add(New SqlParameter("@chvXML", SqlDbType.NVarChar, 4000))

                    SqlParams("@chvXML").Direction = ParameterDirection.Output

                    '获取数据
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
                '关闭连接
                SqlConn.Close()
            End Try

        End Function


        ' ============================ Private Method =====================================

        '' 功能：获取按用户所属公司 Deep 数据权限字符串（系统管理模块专用）
        '' 返回：'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx','xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx'或空字符串
        '' 参数：a_UserGUID          -- 用户 GUID，必填
        ''       a_ObjectID          -- 资源对象唯一标识 id，必填。对应 /bin/DataRights.xml 中的 object.id 属性
        ''       a_SourceType        -- 数据来源，可选。对应 _sourcetype 字段
        ''       a_Application       -- 应用系统，可选
        '' 注意：由于共享项目和实际项目的 ProjGUID 相同，所以，这里存在返回 ProjGUID 无法真实反映，有这样的漏洞。
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
