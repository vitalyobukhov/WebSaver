:: solution cleaning script
::
::   /Q  key specifies no-prompt mode


@ECHO OFF
@SETLOCAL


:: confirm clean
SET confirm=0
IF [%1]==[/Q] (SET confirm=1) ELSE (SET /P confirm="Are you sure? (y/n): ")
IF NOT %confirm%==y (EXIT /B)


:: set vars
SET root=%~dp0..

:: projects
SET resupd=ResUpd
SET scr=Scr
SET scrgen=ScrGen
SET build=Build
SET bin=Binaries

:: paths
SET resupd_bin="%root%\%resupd%\bin"
SET resupd_obj="%root%\%resupd%\obj"
SET scr_bin="%root%\%scr%\bin"
SET scr_obj="%root%\%scr%\obj"
SET scrgen_bin="%root%\%scrgen%\bin"
SET scrgen_obj="%root%\%scrgen%\obj"
SET build_scr="%root%\%build%\*.scr"
SET bin_exe="%root%\%bin%\*.exe"


:: start clean
@ECHO.
@ECHO Clean started.
@ECHO.

@ECHO Cleaning %resupd%...
CALL:tryrmdir %resupd_bin%
CALL:tryrmdir %resupd_obj%

@ECHO Cleaning %scr%...
CALL:tryrmdir %scr_bin%
CALL:tryrmdir %scr_obj%

@ECHO Cleaning %scrgen%...
CALL:tryrmdir %scrgen_bin%
CALL:tryrmdir %scrgen_obj%

@ECHO Cleaning %build%...
CALL:trydel %build_scr%

@ECHO Cleaning %bin%...
CALL:trydel %bin_exe%

:: end clean
@ECHO.
@ECHO Clean successfully completed.
GOTO:EOF

:tryrmdir
	IF EXIST %1 RMDIR /S /Q %1
GOTO:EOF

:trydel
	IF EXIST %1 DEL /Q %1
GOTO:EOF