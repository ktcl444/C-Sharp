Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace MySoft.IISManage
    Public Interface IIISWebServer
        '�½���վ
        Function Create() As Boolean

        'ɾ����վ
        Sub Delete()

        '��������
        Function Update() As Boolean

        '����
        Sub Start()

        '��ͣ
        Sub Pause()

        'ֹͣ
        Sub [Stop]()

        'ȡ����ͣ
        Sub [Continue]()

        '����
        Sub Dispose()

        '��õ�ǰ״̬
        Function GetStatus() As IISServerState

        '���˿��Ƿ��ظ�
        Function CheckPortRepeated() As Boolean

        '������վ����
        Sub SetSiteProperties()

        '��ʼ��
        Sub Init(ByVal strMachine As String, ByVal strWebName As String, ByVal strWebPort As String, ByVal strWebDir As String, ByVal strAppFriendlyName As String, ByVal appIsolated As Integer, ByVal strAppPool As String)
    End Interface
End Namespace
