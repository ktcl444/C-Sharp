
Namespace Application.Types

    ''' <summary>
    ''' ҳ��״̬
    ''' </summary>
    Public Enum PageState As Integer
        ''' <summary>
        ''' ��״̬
        ''' </summary>
        None = 0
        ''' <summary>
        ''' ����
        ''' </summary>
        Add = 1
        ''' <summary>
        ''' �޸�
        ''' </summary>
        Modify = 2       ' 
        ''' <summary>
        ''' �鿴
        ''' </summary>
        Look = 3         ' 
    End Enum

    ''' <summary>
    ''' Form�¼�
    ''' </summary>
    Public Enum FormEventId As Integer
        ''' <summary>
        ''' ���κ��¼�������
        ''' </summary>
        None = 0
        ''' <summary>
        ''' ����
        ''' </summary>
        Save = 1
        ''' <summary>
        ''' ���沢����
        ''' </summary>
        SaveAndClose = 2
        ''' <summary>
        ''' ���沢�ر�
        ''' </summary>
        SaveAndNew = 3
        ''' <summary>
        ''' ������
        ''' </summary>
        Load = 4
    End Enum

    '
    ''' <summary>
    ''' Lookup ģʽ
    ''' </summary>
    Public Enum LookupMode As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        None = 0
        ''' <summary>
        ''' 
        ''' </summary>
        Browse = 1
        ''' <summary>
        ''' 
        ''' </summary>
        ShowColumns = 2
        ''' <summary>
        ''' 
        ''' </summary>
        MultiSelect = 4
    End Enum

    ' ���������
    Friend Enum DogKind As Integer
        Ranbo = 0
        Aladdin = 1
    End Enum

    ' 
    ''' <summary>
    ''' �������;
    ''' </summary>
    Public Enum DogPurpose As Integer
        ''' <summary>
        ''' ��
        ''' </summary>
        None = 0
        ''' <summary>
        ''' ���߹�
        ''' </summary>
        Tools = 1           ' 
        ''' <summary>
        ''' ��ͨ��Ʒ��
        ''' </summary>
        Normal = 2          ' 
        ''' <summary>
        ''' ��ʧЧʱ��Ĳ�Ʒ��
        ''' </summary>
        Time = 3            ' 
        ''' <summary>
        ''' ���ù�
        ''' </summary>
        ZY = 4              ' 
    End Enum

    ' 
    ''' <summary>
    ''' ��֯����
    ''' </summary>
    Public Enum BUType As Integer
        ''' <summary>
        ''' ��
        ''' </summary>
        None = -1
        ''' <summary>
        ''' ��˾
        ''' </summary>
        Company = 0             ' 
        ''' <summary>
        ''' ����
        ''' </summary>
        Department = 1          ' 
        ''' <summary>
        ''' �Ŷ�
        ''' </summary>
        Team = 2                ' 
        ''' <summary>
        ''' ��Ŀ�Ŷ�
        ''' </summary>
        ProjectTeam = 3         ' 
        'Station = 4            ' ��λ
    End Enum

    ' 
    ''' <summary>
    ''' ��λ����
    ''' </summary>
    Public Enum StationType As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        None = -1
        ''' <summary>
        ''' ��λ��λ
        ''' </summary>
        BUStation = 0                   ' 
        ''' <summary>
        ''' �ŶӸ�λ
        ''' </summary>
        TeamStation = 1                 ' 
    End Enum

    '���ܣ�
    ''' <summary>
    ''' ����Ȩ������
    ''' </summary>
    Public Enum DataRightsDTType
        ''' <summary>
        ''' �������û���Ȩ�޵ļ�¼
        ''' </summary>
        Basic = 0           ' 
        ''' <summary>
        ''' ���شӸ��ڵ㵽�û���Ȩ�޽ڵ�ļ�¼���������������ڵ㡪����Ҫ�����Ȩ����Ϊ��״�ṹ���磺��˾��Ŀ��Ȩ
        ''' </summary>
        RootTree = 2        ' 
        ''' <summary>
        ''' �����û�������˾���û���Ȩ�޽ڵ�ļ�¼�������������û�������˾�������Ȩ����û�� _buguid �ֶΣ����ؽ��ͬ Basic
        ''' </summary>
        UserBUDeepTree = 3  ' 
    End Enum

    ''' <summary>
    ''' �û�����
    ''' </summary>
    Public Enum UserKind
        ''' <summary>
        ''' ȫ��
        ''' </summary>
        All = -1
        ''' <summary>
        ''' ERP�û�
        ''' </summary>
        ERP = 0
        ''' <summary>
        ''' ��ͨ�û�
        ''' </summary>
        Normal = 1
        ''' <summary>
        ''' ����ϵͳ�û�
        ''' </summary>
        Sale = 2
    End Enum
End Namespace
