:: scrgen build script

@ECHO OFF
@SETLOCAL


:: set vars
IF [%~1]==[] (SET config=Release) ELSE (SET config=%~1)
SET root=%~dp0..

:: projects
SET resupd=ResUpd
SET scr=Scr
SET scrgen=ScrGen
SET build=Build
SET binaries=Binaries

:: paths
SET msbuild64=%windir%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe
SET msbuild32=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
SET msbuild=msbuild32
SET solution=%root%\WebSaver.sln
SET resupd_exe=%root%\%resupd%\bin\%config%\%resupd%.exe
SET scrgen_in=%root%\%scrgen%\bin\%config%\%scrgen%.exe
SET scr_exe=%root%\%scr%\bin\%config%\%scr%.exe
SET scrgen_out=%root%\%build%\%scrgen%.exe
SET bin=%root%\%binaries%
SET scrgen_out_bin=%bin%\%scrgen%.exe

:: resource args
SET res_type=42
SET res_name=0


:: start build
@ECHO.
@ECHO %scrgen% build started.
@ECHO.

:: set msbuild path
IF EXIST %msbuild64% (
	SET msbuild=%msbuild64%
) ELSE IF EXIST %msbuild32% (
	SET msbuild=%msbuild32%
)
:: check msbuild path
SET ERROR=0
CALL:checkexist %msbuild%
IF %ERROR% NEQ 0 EXIT /B

:: build solution
@ECHO Building solution...
%msbuild% %solution% /p:Configuration=%config% /nologo /verbosity:minimal
IF %ERRORLEVEL% NEQ 0 EXIT /B
@ECHO Solution successfully built.
@ECHO.

:: check paths exist
SET ERROR=0
CALL:checkexist %resupd_exe%
CALL:checkexist %scrgen_in% 
CALL:checkexist %scr_exe%
IF %ERROR% NEQ 0 EXIT /B

:: delete old scrgen
CALL:trydelete %scrgen_out%
CALL:trydelete %scrgen_out_bin%

:: inject screensaver into scrgen
@ECHO Starting %resupd%...
@%resupd_exe% /I %scrgen_in% %scrgen_out% %res_type% %res_name% %scr_exe%
IF %ERRORLEVEL% NEQ 0 EXIT /B
@ECHO %resupd% successfully finished.
@ECHO.

:: copy binary
@ECHO Copying %scrgen%...
IF NOT EXIST %bin% (@MD %bin%)
@COPY /Y %scrgen_out% %scrgen_out_bin%
IF %ERRORLEVEL% NEQ 0 EXIT /B
@ECHO %scrgen% successfully copied.
ECHO.

:: end build
@ECHO %scrgen% build successfully completed.
GOTO:EOF


:: delete file if exists
:trydelete
	IF EXIST %~1 DEL /Q %~1
GOTO:EOF

:: check file exists with error
:checkexist
	IF NOT EXIST %~1 (
		SET ERROR=1
		@ECHO File does not exist %~1
	)
GOTO:EOF