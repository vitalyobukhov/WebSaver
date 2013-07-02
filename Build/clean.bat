:: solution cleaning script
::
::   /Q  key specifies no-prompt mode


@ECHO OFF
@SETLOCAL


:: confirm clean
SET confirm=0
IF [%~1]==[/Q] (SET confirm=1) ELSE (SET /P confirm="Are you sure? (y/n) ")
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
SET resupd_bin=%root%\%resupd%\bin
SET resupd_obj=%root%\%resupd%\obj
SET scr_bin=%root%\%scr%\bin
SET scr_obj=%root%\%scr%\obj
SET scrgen_bin=%root%\%scrgen%\bin
SET scrgen_obj=%root%\%scrgen%\obj
SET build_scr=%root%\%build%\*.scr
SET bin_exe=%root%\%bin%\*.exe


:: start clean
@ECHO.
@ECHO Clean started.
@ECHO.

@ECHO Cleaning %resupd%...
IF EXIST %resupd_bin% RMDIR /S /Q %resupd_bin%
IF EXIST %resupd_obj% RMDIR /S /Q %resupd_obj%

@ECHO Cleaning %scr%...
IF EXIST %scr_bin% RMDIR /S /Q %scr_bin%
IF EXIST %scr_obj% RMDIR /S /Q %scr_obj%

@ECHO Cleaning %scrgen%...
IF EXIST %scrgen_bin% RMDIR /S /Q %scrgen_bin%
IF EXIST %scrgen_obj% RMDIR /S /Q %scrgen_obj%

@ECHO Cleaning %build%...
IF EXIST %build_scr% DEL /Q %build_scr%

@ECHO Cleaning %bin%...
IF EXIST %bin_exe% DEL /Q %bin_exe%

:: end clean
@ECHO.
@ECHO Clean successfully completed.
GOTO:EOF