:: examples build script


@ECHO OFF
@SETLOCAL


:: set vars
SET root=%~dp0..

:: projects
SET scrgen=ScrGen
SET build=Build
SET examples=Examples

:: paths
SET scrgen_exe="%root%\Binaries\%scrgen%.exe"
SET build_scrgen="%root%\%build%\build_scrgen.bat"
SET build_scr="%root%\%build%\build_screensaver.bat"
SET examples_dir="%root%\%build%\%examples%"

:: start build

:: check paths exist
SET ERROR=0
CALL:checkexist %build_scrgen%
CALL:checkexist %build_scr%
CALL:checkexist %examples_dir%
IF %ERROR% NEQ 0 EXIT /B

:: build scrgen if not exist
IF NOT EXIST %scrgen_exe% CALL %build_scrgen%

:: exec scrgen
FOR /F %%D IN ('DIR %examples_dir% /A:D /B') DO CALL %build_scr% "%root%\%build%\%examples%\%%D" "%root%\%build%\%examples%\%%D.SCR"
IF %ERRORLEVEL% NEQ 0 EXIT /B

:: end build
GOTO:EOF


:: delete file if exists
:trydelete
	IF EXIST %1 DEL /Q %1
GOTO:EOF

:: check file exists with error
:checkexist
	IF NOT EXIST %1 (
		SET ERROR=1
		@ECHO File does not exist %1
	)
GOTO:EOF