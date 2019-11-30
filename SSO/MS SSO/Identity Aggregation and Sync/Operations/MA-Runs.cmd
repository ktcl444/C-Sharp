@echo off
rem
rem Copyright (c) Microsoft Corporation.  All rights reserved.
rem

setlocal
set zworkdir=%~dp0
pushd %zworkdir%


cscript runMA.vbs /m:"Intranet Directory MA" /p:"Delta Import (Stage Only)"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Intranet Directory MA" /p:"Delta Synchronization"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Lotus Notes MA" /p:"Delta Import (Stage Only)"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Lotus Notes MA" /p:"Delta Synchronization"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Extranet Directory MA" /p:"Delta Import (Stage Only)"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Extranet Directory MA" /p:"Delta Synchronization"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Sun ONE Directory MA" /p:"Delta Import (Stage Only)"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Sun ONE Directory MA" /p:"Delta Synchronization"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Intranet Directory MA" /p:"Export"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Extranet Directory MA" /p:"Export"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Lotus Notes MA" /p:"Export"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

cscript runMA.vbs /m:"Sun ONE Directory MA" /p:"Export"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

:exit_script
popd
endlocal

