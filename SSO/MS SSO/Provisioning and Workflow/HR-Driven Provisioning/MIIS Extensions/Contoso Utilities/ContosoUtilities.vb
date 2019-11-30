'/////////////////////////////////////////////////////////////////////////////
' Microsoft Solutions for Security
' Copyright (c) 2004 Microsoft Corporation. All rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
' THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
' AND INFORMATION REMAINS WITH THE USER.
'/////////////////////////////////////////////////////////////////////////////

Imports Microsoft.MetadirectoryServices
Public Class Constants
    ' Constants for employeeStatus
    Public Const EMPLOYEE_ACTIVE = 3
    Public Const EMPLOYEE_LEAVE = 1
    Public Const EMPLOYEE_RETIRED = 2
    Public Const EMPLOYEE_DISABLED = 0

    ' Constants for employeeType
    Public Const EMPLOYEE_TYPE_FTE = "A"
    Public Const EMPLOYEE_TYPE_CONTRACTOR = "C"
    Public Const EMPLOYEE_TYPE_TEMPORARY = "T"
    Public Const EMPLOYEE_TYPE_PARTNER = "P"
    Public Const EMPLOYEE_TYPE_CUSTOMER = "M"

    ' Constant to load the XML file 
    Public Const SCENARIO_XML_CONFIG = "\ContosoExtensions.xml"

    ' Declaration of Constants for the Active Directory 
    ' "userAccountControl" bitmask. Taken from LMaccess.h
    Public Const UF_SCRIPT = &H1
    Public Const UF_ACCOUNTDISABLE = &H2
    Public Const UF_HOMEDIR_REQUIRED = &H8
    Public Const UF_LOCKOUT = &H10
    Public Const UF_PASSWD_NOTREQD = &H20
    Public Const UF_PASSWD_CANT_CHANGE = &H40
    Public Const UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = &H80
    Public Const UF_TEMP_DUPLICATE_ACCOUNT = &H100
    Public Const UF_NORMAL_ACCOUNT = &H200
    Public Const UF_DONT_EXPIRE_PASSWD = &H10000
    Public Const UF_MNS_LOGON_ACCOUNT = &H20000
    Public Const UF_SMARTCARD_REQUIRED = &H40000
    Public Const UF_TRUSTED_FOR_DELEGATION = &H80000
    Public Const UF_NOT_DELEGATED = &H100000
    Public Const UF_USE_DES_KEY_ONLY = &H200000
    Public Const UF_DONT_REQUIRE_PREAUTH = &H400000
    Public Const UF_PASSWORD_EXPIRED = &H800000
    Public Const UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = &H1000000
End Class
' <summary>
' Provides some utility methods to assist other modules in
' this application
' </summary>
Public Class ContosoUtilities

    ' Declaration of the Constants
    Const MAX_SAM_GIVENNAME = 4
    Const MAX_SAM_SURNAME = 3
    Const MAX_SAM_LENGTH = 20

    ' <summary>
    ' Cleans up illegal and inconvenient characters
    ' and replaces it with valid characters.
    ' </summary>
    ' <param name="sAMAccountName">sAMAccountName</param>
    ' <returns>String</returns>

    Private Function ReplaceIllegalCharacters( _
        ByVal sAMAccountName As String) As String

        ' Replace illegal and inconvenient characters
        sAMAccountName = sAMAccountName.Replace(" ", "")
        sAMAccountName = sAMAccountName.Replace("'", "")
        sAMAccountName = sAMAccountName.Replace("Å", "Aa")
        sAMAccountName = sAMAccountName.Replace("å", "aa")
        sAMAccountName = sAMAccountName.Replace("Ø", "Oe")
        sAMAccountName = sAMAccountName.Replace("ø", "oe")
        sAMAccountName = sAMAccountName.Replace("Æ", "Ae")
        sAMAccountName = sAMAccountName.Replace("æ", "ae")
        sAMAccountName = sAMAccountName.Replace("/", "_")
        sAMAccountName = sAMAccountName.Replace("\", "_")
        sAMAccountName = sAMAccountName.Replace(";", "_")
        sAMAccountName = sAMAccountName.Replace("]", "_")
        sAMAccountName = sAMAccountName.Replace("|", "_")
        sAMAccountName = sAMAccountName.Replace("[", "_")
        sAMAccountName = sAMAccountName.Replace("=", "_")
        sAMAccountName = sAMAccountName.Replace("+", "_")
        sAMAccountName = sAMAccountName.Replace("*", "_")
        sAMAccountName = sAMAccountName.Replace(",", "_")
        sAMAccountName = sAMAccountName.Replace(">", "_")
        sAMAccountName = sAMAccountName.Replace("?", "_")
        sAMAccountName = sAMAccountName.Replace(":", "_")
        sAMAccountName = sAMAccountName.Replace("<", "_")
        sAMAccountName = sAMAccountName.Replace(".", "_")

        ' Returns the replaced names to the function
        ReplaceIllegalCharacters = sAMAccountName

    End Function

    ' <summary>
    ' Generates Account Name
    ' </summary>
    ' <param name="csEntry">Connector Space Entry</param>
    ' <param name="mvEntry">Metaverse Entry</param>
    ' <param name="isContractor">Contractor called from the 
    ' respective class</param>
    ' <param name="contractorPrefix">Prefix of contractor</param>
    ' <returns>String</returns>

    Public Function GeneratesAMAccountName( _
        ByVal csEntry As CSEntry, _
        ByVal mvEntry As MVEntry, _
        ByVal isContractor As Boolean, _
        ByVal contractorPrefix As String) As String

        ' Declaration of the variables
        Dim givenName As String
        Dim surName As String

        If isContractor Then

            ' Check the presence of the first name and last name
            ' to assign them to respective variables
            If csEntry("FIRST_NAME").IsPresent AndAlso _
                        csEntry("LAST_NAME").IsPresent Then
                givenName = "FIRST_NAME"
                surName = "LAST_NAME"
            Else
                Throw New UnexpectedDataException( _
                    "Both FIRST_NAME and LAST_NAME are needed " + _
                    "to generate the sAMAccountName")
            End If
        Else

            ' Check for the presence of given Name and Surname
            If csEntry("givenName").IsPresent AndAlso _
                            csEntry("surName").IsPresent Then
                givenName = "givenName"
                surName = "surName"
            Else
                Throw New UnexpectedDataException( _
                    "Both givenName and surName are needed to " + _
                    "generate the sAMAccountName")
            End If
        End If

        ' The value for the sAMAccountName attribute should
        ' be unique on every metaverse entry.

        ' Use the following formular to make them unique:
        ' givenName(1..MAX_SAM_GIVENNAME) + 
        ' surname(1..MAX_SAM_SURNAME) + <number>
        Dim samName As String

        ' Setting FirstPart from Connector Space Value
        Dim firstPart As String = csEntry(givenName).Value

        ' Replacing illegal characters from the givenName Value
        firstPart = ReplaceIllegalCharacters(firstPart)

        ' Testing the length of the FirstPartLength which 
        ' is the Length of this attribute
        Dim firstPartLength As Integer = firstPart.Length


        If firstPartLength > MAX_SAM_GIVENNAME Then

            ' Getting the value of the FirstName based on MAX_SAM_GIVENNAME
            firstPart = firstPart.Substring(0, MAX_SAM_GIVENNAME)
        End If

        ' Setting LastPart from Connector Space Value
        Dim lastPart As String = csEntry(surName).Value

        ' Replacing illegal characters from the LastPart Value
        lastPart = ReplaceIllegalCharacters(lastPart)

        ' Testing the length of the LastPart which is the 
        ' Length of this attribute to ensure that it is  
        ' greater than the constant for maximum length
        If lastPart.Length > MAX_SAM_SURNAME Then
            ' Getting the value of the LastName based on MAX_SAM_SURNAME
            lastPart = lastPart.Substring(0, MAX_SAM_SURNAME)
        End If

        ' Concatenating contractorPrefix, FirstName and LastName
        ' Attribute to Build SamName
        If isContractor AndAlso Not contractorPrefix.Length = 0 Then
            samName = contractorPrefix & firstPart & lastPart
        Else
            samName = firstPart & lastPart
        End If

        Dim newSamName As String

        ' Calling the GetCheckedSamName Function to ensure the value 
        ' is unique. Use all lower case for sAMAccountName
        newSamName = GetCheckedSamName(samName.ToLower, mvEntry, csEntry)

        ' If an unique SAMAccoutName could not be created, throw an exception.
        If newSamName.Equals("") Then
            Throw New UnexpectedDataException( _
                "A unique sAMAccountName could not be found")
        End If

        ' Check for maximum length of SAMAccoutName
        If newSamName.Length >= MAX_SAM_LENGTH Then
            Throw New UnexpectedDataException( _
                "sAMAccountName length is " + newSamName.Length + _
                " which is larger than maximum " + MAX_SAM_LENGTH.ToString)
        End If

        ' Returns the New Account Name
        GeneratesAMAccountName = newSamName

    End Function

    ' <summary>
    ' Function for checking SAM Name
    ' </summary>
    ' <param name="samName">Generated User Name</param>
    ' <param name="mvEntry">Metaverse Entry</param>
    ' <param name="csEntry">Connector Space Entry</param>
    ' <returns>String</returns>

    Private Function GetCheckedSamName( _
        ByVal samName As String, _
        ByVal mvEntry As MVEntry, _
        ByVal csEntry As CSEntry) As String

        ' Declarations
        Dim findResultList() As Microsoft.MetadirectoryServices.MVEntry
        Dim checkedSamName As String = samName
        Dim tmpString As String

        Dim isUniq As Boolean = False
        Dim isNumberAttached As Boolean = False

        Dim number As Integer = 1
        Dim numberIndex As Integer = checkedSamName.Length

        Dim mvEntryFound As MVEntry

        ' Create a unique naming attribute by adding a number to
        ' the existing sAMAccountName value.
        Do While Not isUniq

            ' Check if the passed sAMAccountName value exists in the 
            ' metaverse by using the Utils.FindMVEntries method.
            findResultList = Utils.FindMVEntries( _
                    "sAMAccountName", checkedSamName, 1)

            ' If the value does not exist in the metaverse, use the 
            ' passed value as the metaverse value
            If findResultList.Length = 0 Then
                GetCheckedSamName = checkedSamName
                isUniq = True
                Exit Do
            End If

            ' Check that the connector space entry is connected 
            ' to the metaverse entry.
            mvEntryFound = findResultList(0)
            If mvEntryFound Is mvEntry Then
                GetCheckedSamName = checkedSamName
                Exit Do
            End If

            ' If the passed value already exists, rebuilt the SamName  
            ' attribute with new index values to the passed value and  
            ' verify this new value exists. Repeat this step until a 
            ' unique value is created.
            If isNumberAttached Then
                tmpString = checkedSamName.Remove(numberIndex, 1)
                checkedSamName = tmpString + number.ToString
            Else
                isNumberAttached = True
                checkedSamName = checkedSamName + number.ToString
            End If
            number = number + 1
        Loop

    End Function

End Class

