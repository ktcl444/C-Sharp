option explicit
on error resume next

'=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
'SCRIPT:        runMA.vbs
'DATE:          2003-02-05
'=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
'= Copyright (C) 2003 Microsoft Corporation. All rights reserved.
'=
'******************************************************************************
'* Function: DisplayUsage
'*
'* Purpose:  Displays the usage of the script and exits ths script
'*
'******************************************************************************
Sub DisplayUsage()
        WScript.Echo ""
        WScript.Echo "Usage: runMa </m:ma-name> </p:profile-name>"
        WScript.Echo "                [/s:mms-server-name]"
        WScript.Echo "                [/u:user-name]"
        WScript.Echo "                [/a:password]"
        WScript.Echo "                [/v] Switch on Verbose mode"
        WScript.Echo "                [/?] Show the Usage of the script"
        WScript.Echo ""
        WScript.Echo "Example 1: runMa /m:adma1 /p:fullimport"
        WScript.Echo "Example 2: runMa /m:adma1 /p:fullimport /u:domain\user /a:mysecret /v"
        WScript.Quit (-1)
End Sub

'******************************************************************************
' Script Main Execution Starts Here
'******************************************************************************
'--Used Variables--------------------------
dim s
dim runResult
dim rescode
dim managementagentName
dim profile
dim verbosemode
dim wmiLocator
dim wmiService
dim managementagent
dim server
dim username
dim password
'-----------------------------------------

rescode = ParamExists("/?")
if rescode = true then call DisplayUsage
verbosemode = ParamExists("/v")

managementagentName = ParamValue("/m")
if managementagentName = "" then call DisplayUsage

profile = ParamValue("/p")
if profile = "" then call DisplayUsage

if verbosemode then wscript.echo "%Info: Management Agent and Profile is <"& managementagentName &":"& profile &">"
if verbosemode then wscript.Echo "%Info: Getting WMI Locator object"

set wmiLocator = CreateObject("WbemScripting.SWbemLocator")
if err.number <> 0 then
        wscript.echo "%Error: Cannot get WMI Locator object"
        wscript.quit(-1)
end if

server = ParamValue("/s")
password = ParamValue("/a")
username = ParamValue("/u")

if server = "" then server = "." ' connect to WMI on local machine

if verbosemode then 
        wscript.Echo "%Info: Connecting to MMS WMI Service on <" & server &">"
        if username <> "" then wscript.Echo "%Info: Accessing MMS WMI Service as <"& username &">"
end if

if username = "" then
        set wmiService = wmiLocator.ConnectServer(server, "root/MicrosoftIdentityIntegrationServer")
else
        set wmiService = wmiLocator.ConnectServer(server, "root/MicrosoftIdentityIntegrationServer", username, password)
end if

if err.number <> 0 then
        wscript.echo "%Error: Cannot connect to MMS WMI Service <" & err.Description & ">"
        wscript.quit(-1)
end if

if verbosemode then wscript.Echo "%Info: Getting MMS Management Agent via WMI"

Set managementagent = wmiService.Get( "MIIS_ManagementAgent.Name='" & managementagentName & "'")
if err.number <> 0 then
        wscript.echo "%Error: Cannot get Management Agent with specified WMI Service <" & err.Description & ">"
        wscript.quit(-1)
end if

wscript.echo "%Info: Starting Management Agent with Profile <"& managementagent.name &":"& profile &">"
runResult = managementagent.Execute(profile)
if err.number <> 0 then
        wscript.Echo "%Error: Running MA <"& err.Description & ">. Make sure the correct profile name is specified."
        wscript.quit(-1)
end if

wscript.Echo "%Info: Finish Running Management Agent"
wscript.Echo "%Result: <" & CStr(runResult) & ">"
wscript.quit(0)

'******************************************************************************
'* Function: ParamValue
'*
'* Purpose:  Parses the command line for an argument and 
'*           returns the value of the argument to the caller
'*           Argument and value must be seperated by a colon
'*
'* Arguments:
'*  [in]     parametername      name of the paramenter
'*
'* Returns:   
'*           STRING      Parameter found in commandline
'*           ""         Parameter NOT found in commandline
'*
'******************************************************************************
Function ParamValue(ParameterName)

        Dim i                   '* Counter
        Dim Arguments           '* Arguments from the command-line command
        Dim NumberofArguments   '* Number of arguments from the command-line command
        Dim ArgumentArray       '* Array in which to store the arguments from the command-line
        Dim TemporaryString     '* Utility string

        '* Initialize Return Value to e the Empty String
        ParamValue = ""

        '* If no ParameterName is passed into the function exit
        if ParameterName = "" then exit function

        '* Check if Parameter is in the Arguments and return the value
        Set Arguments = WScript.Arguments
        NumberofArguments = Arguments.Count - 1

        For i=0 to NumberofArguments
                TemporaryString = Arguments(i)
                ArgumentArray = Split(TemporaryString,":",-1,vbTextCompare)

                If ArgumentArray(0) = ParameterName Then
                      ParamValue = ArgumentArray(1)
                      exit function
                End If
        Next
end Function

'******************************************************************************
'* Function: ParamExists
'*
'* Purpose:  Parses the command line for an argument and 
'*           returns the true if argument is present
'*
'* Arguments:
'*  [in]     parametername      name of the paramenter
'*
'* Returns:   
'*           true       Parameter found in commandline
'*           false      Parameter NOT found in commandline
'*
'******************************************************************************

Function ParamExists(ParameterName)

        Dim i                   '* Counter
        Dim Arguments           '* Arguments from the command-line command
        Dim NumberofArguments   '* Number of arguments from the command-line command
        Dim ArgumentArray       '* Array in which to store the arguments from the command-line
        Dim TemporaryString     '* Utility string

        '* Initialize Return Value to e the Empty String
        ParamExists = false

        '* If no ParameterName is passed into the function exit
        if ParameterName = "" then exit function

        '* Check if Parameter is in the Arguments and return the value
        Set Arguments = WScript.Arguments
        NumberofArguments = Arguments.Count - 1

        For i=0 to NumberofArguments
                TemporaryString = Arguments(i)
                If TemporaryString = ParameterName Then
                      ParamExists = true
                      exit function
                End If
        Next
end Function
