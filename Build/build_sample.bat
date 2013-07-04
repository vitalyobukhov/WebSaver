:: sample screensaver build script


@ECHO OFF
@SETLOCAL


:: set vars
SET root=%~dp0..

:: projects
SET scrgen=ScrGen
SET sample=sample

:: paths
SET scrgen_exe="%root%\Binaries\%scrgen%.exe"
SET build_scrgen="%root%\Build\build_scrgen.bat"
SET build_scr="%root%\Build\build_screensaver.bat"
SET con="%root%\Build\%sample%"
SET cap="%root%\Build\%sample%\caption.txt"
SET ico="%root%\Build\%sample%\icon.ico"
SET scr_out="%root%\Build\sample.scr"


:: start build

:: check paths exist
SET ERROR=0
CALL:checkexist %build_scrgen%
CALL:checkexist %build_scr%
CALL:checkexist %con%
CALL:checkexist %cap% 
CALL:checkexist %ico%
IF %ERROR% NEQ 0 EXIT /B

:: build scrgen if not exist
IF NOT EXIST %scrgen_exe% CALL %build_scrgen%

:: exec scrgen
CALL %build_scr% %con% %cap% %ico% %scr_out%
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