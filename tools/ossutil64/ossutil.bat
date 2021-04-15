@echo off

echo %1 %2

set CURRENTPATH=%~dp0
echo %CURRENTPATH%

set RUNEXE=%CURRENTPATH%ossutil64.exe
rem echo %RUNEXE%

set CONFIGPATH=%CURRENTPATH%.ossutilconfig
rem echo %configPath%

rem echo Waiting for delete files ...
rem %RUNEXE% rm %2 -r

echo Waiting for upload ...

%RUNEXE% cp -r %1 %2 --config-file %CONFIGPATH% -u

rem pause