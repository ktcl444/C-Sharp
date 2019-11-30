Imports System.Web.Configuration
Imports System.Configuration
Imports System.Xml

Namespace Application
    Public Class WebConfig
        '读取Web.config中AppSettings节点下的Key值
        Public Shared Function ReadAppSettings(ByVal key As String) As String
            Dim item As String = "appSettings"
            Dim sRtn As String = String.Empty
            Dim config As Configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath)
            Dim appSection As AppSettingsSection = DirectCast(config.GetSection(item), AppSettingsSection)
            If Not appSection.Settings(key) Is Nothing Then
                sRtn = appSection.Settings(key).Value.ToString()
            End If

            Return sRtn
        End Function

        ''设置Web.config中AppSettings节点下的Key值
        'Public Shared Function WriteAppSettings(ByVal key As String, ByVal value As String)
        '    Dim item As String = "appSettings"

        '    Dim config As Configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath)
        '    Dim appSection As AppSettingsSection = DirectCast(config.GetSection(item), AppSettingsSection)

        '    If appSection.Settings(key) Is Nothing Then
        '        appSection.Settings.Add(key, value)
        '        config.Save()
        '    Else
        '        'appSection.Settings.Remove(key)
        '        'appSection.Settings.Add(key, value)
        '        appSection.Settings(key).Value = value
        '        config.Save()
        '    End If
        'End Function

        '设置Web.config中AppSettings节点下的Key值
        Public Shared Function WriteAppSettings(ByVal key As String, ByVal value As String)
            Dim strFileName = System.Web.HttpContext.Current.Server.MapPath("\") + "web.config"
            Dim doc As New XmlDocument()
            doc.Load(strFileName)
            Dim appSection As XmlElement = doc.DocumentElement.SelectSingleNode("/configuration/appSettings")
            Dim item As XmlElement = appSection.SelectSingleNode("add[@key=""" + key + """]")
            If item Is Nothing Then
                Dim e As XmlElement = doc.CreateElement("add")
                e.SetAttribute("key", key)
                e.SetAttribute("value", value)
                appSection.AppendChild(e)
            Else
                item.SetAttribute("value", value)
            End If
            doc.Save(strFileName)
        End Function

    End Class
End Namespace
