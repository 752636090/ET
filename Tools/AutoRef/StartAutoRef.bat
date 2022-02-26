@echo off
::@set WORKSPACE=D:\ET
@set ADJUST=dotnet
@set DllRelativePath=Bin\SyncCodesService.dll
if not defined WORKSPACE (
    cd /d ../../
)
if not defined WORKSPACE (
    @set WORKSPACE=%cd%
)
@%ADJUST% %WORKSPACE%\%DllRelativePath% %WORKSPACE%\Unity