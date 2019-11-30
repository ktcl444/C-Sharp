@echo off
::
:: Copyright (c) Microsoft Corporation.  All rights reserved.
::

setlocal
set zworkdir=%~dp0
pushd %zworkdir%

:: Recalculate Group Membership
echo ----------------------------
echo Running miisGroupPopulator . . .
groupPopulator.exe /r:managers,titles,locations /p
popd

if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

:: Group Management - Import - Sync
echo ----------------------------
cscript runMA.vbs /m:"Group Management" /p:"Delta Import (Stage Only) - Delta Synchronization" //nologo
:: cscript runMA.vbs /m:"Group Management" /p:"Full Import and Full Synchronization" //nologo
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

:: Intranet Active Directory - Export
echo ----------------------------
cscript runMA.vbs /m:"Intranet Active Directory" /p:"Export" //nologo
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

:: Lotus Notes R6 MA - Export
echo ----------------------------
cscript runMA.vbs /m:"Lotus Notes" /p:"Export" //nologo
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)
echo ----------------------------

:: Truncate Delta Table
echo ----------------------------
osql -S localhost -E -n -b -i "TruncateDeltaTable.sql"
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: command file failed) & (goto exit_script)

:exit_script
popd
endlocal
