:: sample screensaver build script

@ECHO OFF
@SETLOCAL


:: set vars
SET root=%~dp0..

:: projects
SET scrgen=ScrGen
SET content=content
SET sample=Sample

:: paths
SET scrgen_exe=%root%\Build\%scrgen%.exe
SET scrgen_bat=%root%\Build\scrgen.bat
SET scr_bat=%root%\Build\scr.bat
SET con=%root%\Build\Sample\content
SET cap=%root%\Build\Sample\caption.txt
SET ico=%root%\Build\Sample\icon.ico
SET scr_out=%root%\Build\sample.scr


:: start build

:: build scrgen if not exist
IF NOT EXIST %scrgen_exe% CALL %scrgen_bat%

:: check paths exist
SET ERROR=0
CALL:checkexist %scrgen_bat%
CALL:checkexist %scr_bat%
CALL:checkexist %con%
CALL:checkexist %cap% 
CALL:checkexist %ico%
IF %ERROR% NEQ 0 EXIT /B

:: exec scrgen
CALL %scr_bat% %con% %cap% %ico% %scr_out%
IF %ERRORLEVEL% NEQ 0 EXIT /B

:: end build
GOTO:EOF


:: delete file if exists
:trydelete
	IF EXIST %~1 DEL /F %~1
GOTO:EOF

:: check file exists with error
:checkexist
	IF NOT EXIST %~1 (
		SET ERROR=1
		@ECHO File does not exist %~1
	)
GOTO:EOF