Imports System.Web
Imports System.Xml
Imports System.IO
Imports System.Text

Namespace Caching

    Public Class Cache

        '从缓存中获取 XML
        Public Shared Function GetXmlDocument(ByVal path As String) As XmlDocument
            path = HttpContext.Current.Server.MapPath(path)

            Dim xmlDoc As XmlDocument
            Try
                xmlDoc = HttpRuntime.Cache(path)

                If xmlDoc Is Nothing Then
                    xmlDoc = New XmlDocument
                    xmlDoc.Load(path)

                    Dim xslt As New Xsl.XslTransform()
                    Dim xsltDom As New XmlDocument()
                    xsltDom.LoadXml("<xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">  <xsl:output method=""xml"" indent=""no"" encoding=""gb2312""/>  <xsl:template match=""/|comment()|processing-instruction()"">    <xsl:copy>      <xsl:apply-templates/>    </xsl:copy>  </xsl:template>  <xsl:template match=""*"">    <xsl:element name=""{local-name()}"">      <xsl:apply-templates select=""@*|node()""/>    </xsl:element>  </xsl:template>  <xsl:template match=""@*"">    <xsl:attribute name=""{local-name()}"">      <xsl:value-of select="".""/>    </xsl:attribute>  </xsl:template></xsl:stylesheet>")

                    xslt.Load(xsltDom)

                    Dim output As New StringBuilder(2000)
                    Dim writer As New StringWriter(output)

                    xslt.Transform(xmlDoc, Nothing, writer, Nothing)

                    xmlDoc = New XmlDocument
                    xmlDoc.LoadXml(output.ToString())

                    'HttpRuntime.Cache.Insert(path, xmlDoc, New System.Web.Caching.CacheDependency(path))
                    MyCache.Insert(path, xmlDoc, New System.Web.Caching.CacheDependency(path))

                End If
            Catch ex As Exception
            	Mysoft.Map.Data.MyDB.LogException(ex)
                Return Nothing
            End Try

            '直接返回缓存内容，修改 XML 会影响到缓存，所以需要复制一份返回
            Return xmlDoc.Clone()
        End Function

    End Class

End Namespace
