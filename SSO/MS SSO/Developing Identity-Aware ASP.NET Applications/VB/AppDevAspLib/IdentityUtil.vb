'##############################################################################
' Microsoft Solutions for Security
' Copyright (c) 2004 Microsoft Corporation. All rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
' THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
' AND INFORMATION REMAINS WITH THE USER.
'##############################################################################

Imports System
Imports System.Xml
Imports System.Web
Imports System.Web.Security
Imports System.Configuration

Namespace Contoso.Identity.Template

    ' <summary>
    '   IdentityInfo helper class that can read the current
    '   authentication mode (Windows, Forms, Passport etc) and
    '   provide information whether we currently are impersonating
    '   the user or not
    ' </summary>
    Public Class IdentityUtil

        ' <summary>
        '	Figure out if we are impersonating the client user or not.
        '	
        '	When using ASP.NET Forms Authentication, the impersonate
        '	attribute of the identity element is always set to false to 
        '	ensure that ASP.NET runs under a priveleged account that can
        '	impersonate users. Contoso chose to use a custom setting to
        '	select model
        '	
        '	When using other authentication mechanisms, such as Windows
        '	Integrated, X.509 certificates and Passport the model is 
        '	directly related to the impersonate attribute of the identity
        '	element.
        ' </summary>
        ' <returns>true if we are impersonating and false on any error</returns>
        Public Shared Function IsImpersonating() As [Boolean]

            Dim configDoc As ConfigXmlDocument = Nothing
            Dim useImpersonationModel As String = Nothing
            Dim identityNode As XmlNode = Nothing
            Dim impersonateAttribute As XmlAttribute = Nothing
            Dim isImpersonatingInPlatform As [Boolean] = False
            Dim isImpersonatingInCode As [Boolean] = False

            Try
                'Check for impersonation in platform
                configDoc = New ConfigXmlDocument
                configDoc.Load(HttpContext.Current.Request.PhysicalApplicationPath + "web.config")
                identityNode = configDoc.SelectSingleNode("configuration/system.web/identity")

                If Not (identityNode Is Nothing) Then
                    impersonateAttribute = identityNode.Attributes("impersonate")
                    isImpersonatingInPlatform = Convert.ToBoolean(impersonateAttribute.Value)
                End If

                'Check for impersonation in code
                useImpersonationModel = ConfigurationSettings.AppSettings("UseImpersonationModel")

                If Not (useImpersonationModel Is Nothing) AndAlso Convert.ToBoolean(useImpersonationModel) = True Then
                    isImpersonatingInCode = True 'default to not impersonate
                End If
                If isImpersonatingInCode OrElse isImpersonatingInPlatform Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                'TODO: Handle configuration exception gracefully
                Return False
            End Try

        End Function 'IsImpersonating

        ' <summary>
        '   Figure out authentication mode from the User.Identity type
        ' </summary>
        ' <returns></returns>
        Public Shared Function AuthenticationMode() As String

            If HttpContext.Current.User Is Nothing Then
                Return "None"
            End If

            If TypeOf HttpContext.Current.User.Identity Is FormsIdentity Then
                Return "Forms"
            ElseIf TypeOf HttpContext.Current.User.Identity Is PassportIdentity Then
                Return "Passport"
            Else
                Return "Windows"
            End If

        End Function 'AuthenticationMode
    End Class 'IdentityUtil
End Namespace 'Contoso.Identity.Template