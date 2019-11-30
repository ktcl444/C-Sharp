Imports System
Imports System.Xml

Namespace MyException
    Public Class JavaScriptException
        Inherits Exception

        Public Sub New(ByVal strXml As String)
            Dim MyXmlDOM As New XmlDocument
            MyXmlDOM.LoadXml(strXml)

            ' 异常类型号
            If MyXmlDOM.SelectSingleNode("/xml/number") IsNot Nothing AndAlso MyXmlDOM.SelectSingleNode("/xml/number").Attributes.GetNamedItem("value") IsNot Nothing Then
                _Number = MyXmlDOM.SelectSingleNode("/xml/number").Attributes.GetNamedItem("value").InnerText
            End If

            ' 异常类型名称
            If MyXmlDOM.SelectSingleNode("/xml/name") IsNot Nothing AndAlso MyXmlDOM.SelectSingleNode("/xml/name").Attributes.GetNamedItem("value") IsNot Nothing Then
                _Name = MyXmlDOM.SelectSingleNode("/xml/name").Attributes.GetNamedItem("value").InnerText
            End If

            ' 异常的描述信息
            If MyXmlDOM.SelectSingleNode("/xml/description") IsNot Nothing AndAlso MyXmlDOM.SelectSingleNode("/xml/description").Attributes.GetNamedItem("value") IsNot Nothing Then
                _Description = MyXmlDOM.SelectSingleNode("/xml/description").Attributes.GetNamedItem("value").InnerText
            End If

            ' 异常信息
            If MyXmlDOM.SelectSingleNode("/xml/message") IsNot Nothing AndAlso MyXmlDOM.SelectSingleNode("/xml/message").Attributes.GetNamedItem("value") IsNot Nothing Then
                _Message = MyXmlDOM.SelectSingleNode("/xml/message").Attributes.GetNamedItem("value").InnerText
            End If

            ' 异常错误所产生的行号
            If MyXmlDOM.SelectSingleNode("/xml/lineNumber") IsNot Nothing AndAlso MyXmlDOM.SelectSingleNode("/xml/lineNumber").Attributes.GetNamedItem("value") IsNot Nothing Then
                _LineNumber = MyXmlDOM.SelectSingleNode("/xml/lineNumber").Attributes.GetNamedItem("value").InnerText
            End If

            ' 异常错误所产生的URL
            If MyXmlDOM.SelectSingleNode("/xml/url") IsNot Nothing AndAlso MyXmlDOM.SelectSingleNode("/xml/url").Attributes.GetNamedItem("value") IsNot Nothing Then
                _Url = MyXmlDOM.SelectSingleNode("/xml/url").Attributes.GetNamedItem("value").InnerText
            End If

            ' 堆栈信息
            If MyXmlDOM.SelectSingleNode("/xml/stackTrace") IsNot Nothing AndAlso MyXmlDOM.SelectSingleNode("/xml/stackTrace").Attributes.GetNamedItem("value") IsNot Nothing Then
                _StackTrace = MyXmlDOM.SelectSingleNode("/xml/stackTrace").Attributes.GetNamedItem("value").InnerText
            End If

        End Sub


        ''' <summary>
        ''' 异常类型号
        ''' </summary>
        Private _Number As String

        Public ReadOnly Property Number() As String
            Get
                Return _Number
            End Get
        End Property

        ''' <summary>
        ''' 异常类型名称
        ''' </summary>
        Private _Name As String
        Public ReadOnly Property Name() As String
            Get
                Return _Name
            End Get
        End Property

        ''' <summary>
        ''' 异常的描述信息
        ''' </summary>
        Private _Description As String
        Public ReadOnly Property Description() As String
            Get
                Return _Description
            End Get
        End Property

        ''' <summary>
        ''' 异常信息
        ''' </summary>
        Private _Message As String
        Public Overrides ReadOnly Property Message() As String
            Get
                Return _Message
            End Get
        End Property

        ''' <summary>
        ''' 异常错误所产生的行号
        ''' </summary>
        Private _LineNumber As String
        Public ReadOnly Property LineNumber() As String
            Get
                Return _LineNumber
            End Get
        End Property

        ''' <summary>
        ''' 异常错误所产生的URL
        ''' </summary>
        Private _Url As String
        Public ReadOnly Property Url() As String
            Get
                Return _Url
            End Get
        End Property

        ''' <summary>
        ''' 堆栈信息
        ''' </summary>
        Private _StackTrace As String
        Public Overrides ReadOnly Property StackTrace() As String
            Get
                Return _StackTrace
            End Get
        End Property

    End Class

End Namespace



