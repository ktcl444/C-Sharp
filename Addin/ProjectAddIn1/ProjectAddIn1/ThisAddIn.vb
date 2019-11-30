Imports System.Windows.Forms

Public Class ThisAddIn


    Private _commandBar As Office.CommandBar
    Private _submitButton As Office.CommandBarButton
    Private _importButton As Office.CommandBarButton
    Private _exportButton As Office.CommandBarButton
    Private _editButton As Office.CommandBarButton

    Private Sub ThisAddIn_Startup(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Startup
        Try

            AddToolbar()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "添加工具栏按钮错误", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '添加工具栏
    Private Sub AddToolbar()
        Try
            _commandBar = Application.CommandBars("明源")
        Catch ex As ArgumentException
            ' The toolbar named MysoftBar does not exist, so create it.
        End Try

        If (_commandBar Is Nothing) Then
            ' Add a command bar named ImportBar to the toolbar area.
            Dim barPosition As Integer = 1
            Dim isTemporary As Boolean = True
            _commandBar = Application.CommandBars.Add("明源", barPosition, , isTemporary)
        End If

        Try
            ' Add a button named ImportProject and an event handler.
            ' It is not necessary to use TryCast() here.
            _submitButton = _commandBar.Controls.Add(Type:=Office.MsoControlType.msoControlButton)
            _submitButton.Style = Office.MsoButtonStyle.msoButtonCaption
            _submitButton.Caption = "提交"
            _submitButton.Tag = "Submit"
            _submitButton.TooltipText = "提交任务到服务器。"
            AddHandler _submitButton.Click, AddressOf SubmitButtonClick

            _importButton = _commandBar.Controls.Add(Type:=Office.MsoControlType.msoControlButton)
            _importButton.Style = Office.MsoButtonStyle.msoButtonCaption
            _importButton.Caption = "导入"
            _importButton.Tag = "Import"
            _importButton.TooltipText = "导入任务。"
            AddHandler _importButton.Click, AddressOf ImportButtonClick

            _exportButton = _commandBar.Controls.Add(Type:=Office.MsoControlType.msoControlButton)
            _exportButton.Style = Office.MsoButtonStyle.msoButtonCaption
            _exportButton.Caption = "导出"
            _exportButton.Tag = "Export"
            _exportButton.TooltipText = "导出任务。"
            AddHandler _exportButton.Click, AddressOf ExportButtonClick

            _editButton = _commandBar.Controls.Add(Type:=Office.MsoControlType.msoControlButton)
            _editButton.Style = Office.MsoButtonStyle.msoButtonCaption
            _editButton.Caption = "编制工作项"
            _editButton.Tag = "EditItem"
            _editButton.TooltipText = "编制工作项。"
            AddHandler _editButton.Click, AddressOf EditItemButtonClick

            _commandBar.Visible = True

        Catch ex As Exception
            MessageBox.Show(ex.Message, "添加工具栏按钮错误", _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub SubmitButtonClick(ByVal ctrl As Office.CommandBarButton, ByRef cancel As Boolean)
        MessageBox.Show("SubmitButtonClick")
    End Sub

    Private Sub ImportButtonClick(ByVal ctrl As Office.CommandBarButton, ByRef cancel As Boolean)
        MessageBox.Show("ImportButtonClick")
    End Sub

    Private Sub ExportButtonClick(ByVal ctrl As Office.CommandBarButton, ByRef cancel As Boolean)
        MessageBox.Show("ExportButtonClick")
    End Sub

    Private Sub EditItemButtonClick(ByVal ctrl As Office.CommandBarButton, ByRef cancel As Boolean)
        MessageBox.Show("EditItemButtonClick")
    End Sub

    Private Sub ThisAddIn_Shutdown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shutdown

    End Sub

End Class
