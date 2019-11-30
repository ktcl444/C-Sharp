@echo off
rem
rem Copyright (c) Microsoft Corporation.  All rights reserved.
rem

setlocal
set zworkdir=%~dp0
pushd %zworkdir%

set sqlCmdFile=GroupPopulatorDB.sql
if NOT exist %sqlCmdFile% (echo Error: specified sql command file does not exist) & (goto exit_script)

osql -S localhost -E -n -b -i %sqlCmdFile%
if {%errorlevel%} NEQ {0} (echo Error[%errorlevel%]: sql command file failed) & (goto exit_script)

:exit_script
popd
endlocal

