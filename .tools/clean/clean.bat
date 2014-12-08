@ECHO OFF
::
::-----------------------------------------------------------------------------
::
:: Set ROOTDIR
::
PUSHD ..\..
SET ROOTDIR=%CD%\
POPD
ECHO Cleaning recursively under root directory: %ROOTDIR%
::
::-----------------------------------------------------------------------------
::
:: Delete the "bin" subdirectories.
::
FOR /F "tokens=*" %%D IN ('DIR /B /S /A:D "%ROOTDIR%bin"') DO (
  IF EXIST %%D (
  	ECHO Removing %%D
  	RMDIR /Q /S %%D
  )
)
::
::-----------------------------------------------------------------------------
::
:: Delete the "obj" subdirectories.
::
FOR /F "tokens=*" %%D IN ('DIR /B /S /A:D "%ROOTDIR%obj"') DO (
  IF EXIST %%D (
  	ECHO Removing %%D
  	RMDIR /Q /S %%D
  )
)
