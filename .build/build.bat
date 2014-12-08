@ECHO OFF
::-----------------------------------------------------------------------------
:: Build Bootstrapper: build.bat
:: This batch file is used to bootstrap MS Build project, build.proj.
::
::-----------------------------------------------------------------------------
::
:: Set DOTNETDIR
:: Use only one of the lines below based on your system configuration.
:: If you have a 64-bit Windows OS, use Framework64, otherwise use Framework.
:: This path is used to access the MS Build executable file.
:: NOTE: Requires Microsoft .NET 4.0 or 4.5
SET DOTNETDIR=%windir%\Microsoft.NET\Framework64\v4.0.30319\
::SET DOTNETDIR=%windir%\Microsoft.NET\Framework\v4.0.30319\
::
::-----------------------------------------------------------------------------
::
SET MSBUILDEXE=%DOTNETDIR%msbuild.exe
SET MSBUILDPROJ=%CD%\build.proj
::
::-----------------------------------------------------------------------------
::
:: Set BUILDCONFIG
:: Choices are: Debug or Release
SET BUILDCONFIG=%1
::
IF "%~1"=="" SET BUILDCONFIG=Release
::
::-----------------------------------------------------------------------------
::
:: Set SHOULDRUNTESTS
:: Choices are: true or false
SET SHOULDRUNTESTS=%2
::
IF "%~2"=="" SET SHOULDRUNTESTS=false
::
::-----------------------------------------------------------------------------
::
:: Get the absolute path of the source directory.
::
SET ROOTDIR=%3
::
PUSHD ..
IF "%~3"=="" SET ROOTDIR=%CD%\
POPD
::
::-----------------------------------------------------------------------------
::
SET MSBUILDCOMMAND=%MSBUILDEXE% %MSBUILDPROJ% /p:Configuration=%BUILDCONFIG%;RootDir=%ROOTDIR%;ShouldRunTests=%SHOULDRUNTESTS%
::
::-----------------------------------------------------------------------------
::
ECHO.
ECHO Preparing to build.
ECHO.
ECHO BUILDCONFIG......: %BUILDCONFIG%
ECHO ROOTDIR..........: %ROOTDIR%
ECHO SHOULDRUNTESTS...: %SHOULDRUNTESTS%
ECHO MSBUILDEXE.......: %MSBUILDEXE%
ECHO MSBUILDPROJ......: %MSBUILDPROJ%
ECHO.
ECHO %MSBUILDCOMMAND%
ECHO.
::
:: Execute the command:
%MSBUILDCOMMAND%
