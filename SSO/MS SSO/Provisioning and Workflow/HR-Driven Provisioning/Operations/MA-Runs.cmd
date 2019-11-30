@echo off
rem
rem Copyright (c) Microsoft Corporation.  All rights reserved.
rem

setlocal
set zworkdir=%~dp0
pushd %zworkdir%

cscript runMA.vbs /m:"SAP HR" /p:"Delta Import"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Intranet Active Directory" /p:"Export"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Intranet Active Directory" /p:"Delta Import"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Lotus Notes" /p:"Export"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Extranet Active Directory" /p:"Export"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Sun ONE Directory" /p:"Export"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)


:exit_script
popd
endlocal

