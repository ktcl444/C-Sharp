
Namespace Security

    ' ��װ����/���ܺ���
    '
    Public Class Cryptography

        '���ܣ��ַ�������
        '�����������ܴ�
        '���أ����ܱ任��Ľ��
        Public Shared Function EnCode(ByVal inStr As String) As String
            Dim StrBuff As String
            Dim IntLen, IntCode, IntCode1, IntCode2, IntCode3, i As Integer

            IntLen = Len(Trim(inStr))

            IntCode1 = IntLen Mod 3
            IntCode2 = IntLen Mod 9
            IntCode3 = IntLen Mod 5
            IntCode = IntCode1 + IntCode3

            For i = 1 To IntLen
                StrBuff = StrBuff + Chr(Asc(Mid(inStr, IntLen + 1 - i, 1)) - IntCode)
                If IntCode = IntCode1 + IntCode3 Then
                    IntCode = IntCode2 + IntCode3
                Else
                    IntCode = IntCode1 + IntCode3
                End If
            Next

            Return StrBuff + Space(Len(inStr) - IntLen)

        End Function

        '���ܣ��ַ�������
        '�������������ܴ�
        '���أ������ܱ任��Ľ��
        Public Shared Function DeCode(ByVal inStr As String) As String
            Dim StrBuff As String
            Dim IntLen, IntCode, IntCode1, IntCode2, IntCode3, i As Integer

            StrBuff = ""

            IntLen = Len(Trim(inStr))

            IntCode1 = IntLen Mod 3
            IntCode2 = IntLen Mod 9
            IntCode3 = IntLen Mod 5

            If IntLen / 2 = Int(IntLen / 2) Then
                IntCode = IntCode2 + IntCode3
            Else
                IntCode = IntCode1 + IntCode3
            End If

            For i = 1 To IntLen

                StrBuff = StrBuff + Chr(Asc(Mid(inStr, IntLen + 1 - i, 1)) + IntCode)

                If IntCode = IntCode1 + IntCode3 Then
                    IntCode = IntCode2 + IntCode3
                Else
                    IntCode = IntCode1 + IntCode3
                End If
            Next

            Return StrBuff + Space(Len(inStr) - IntLen)
        End Function

    End Class

End Namespace
