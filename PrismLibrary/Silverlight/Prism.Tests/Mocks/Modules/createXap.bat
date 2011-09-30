@echo Dynamically creating a XAP file named RemoteModules.xap. This file is included in the Composite.Silverlight.Tests project as a file link (even if it's not shown in Visual Studio) as an embedded resource. 

SET Configuration=%1

pushd "%~dp0"

set compositeDll="..\..\..\Prism\Bin\%Configuration%\Microsoft.Practices.Prism.dll"
if not exist %compositeDll% echo ERROR. Could not find %compositeDll%
if not exist %compositeDll% goto Error

set cscBin="%WINDIR%\Microsoft.NET\Framework\v3.5\Csc.exe"
if not exist %cscBin% set cscBin="%FrameworkDir%\%Framework35Version%\Csc.exe"
if not exist %cscBin% echo ERROR. Could not find %WINDIR%\Microsoft.NET\Framework\v3.5\Csc.exe
if not exist %cscBin% goto Error

REM Try for Silverlight 2 assemblies
set silverlightDll="%ProgramFiles%\Microsoft SDKs\Silverlight\v2.0\Reference Assemblies\mscorlib.dll"
if not exist %silverlightDll% echo Could not find %silverlightDll%. Trying for Silverlight 3 assemblies.
 
REM Trying for Silverlight 3 assemblies
if not exist %silverlightDll% set silverlightDll="%ProgramFiles%\Reference Assemblies\Microsoft\Framework\Silverlight\v3.0\mscorlib.dll"
if not exist %silverlightDll% echo Could not find %silverlightDll%. Trying for Silverlight 4 assemblies.

REM Trying for Silverlight 4 assemblies
if not exist %silverlightDll% set silverlightDll="%ProgramFiles%\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\mscorlib.dll"
if not exist %silverlightDll% echo ERROR. Could not find %silverlightDll%
if not exist %silverlightDll% goto Error

REM Build 2 DLLs
%cscBin% /noconfig /nowarn:1701,1702 /nostdlib+ /errorreport:prompt /warn:4 /define:DEBUG;TRACE;SILVERLIGHT /reference:%silverlightDll% /reference:%compositeDll% /target:library RemoteModuleA.cs
%cscBin% /noconfig /nowarn:1701,1702 /nostdlib+ /errorreport:prompt /warn:4 /define:DEBUG;TRACE;SILVERLIGHT /reference:%silverlightDll% /reference:%compositeDll% /target:library RemoteModuleB.cs

if exist "%CD%\RemoteModules.temp.zip" del "%CD%\RemoteModules.temp.zip"

REM Add the 2 DLLs and the manifest to a Zip file
cscript /nologo AddToZip.vbs "%CD%\RemoteModules.temp.zip" "%CD%\AppManifest.xaml"
cscript /nologo AddToZip.vbs "%CD%\RemoteModules.temp.zip" "%CD%\RemoteModuleA.dll"
cscript /nologo AddToZip.vbs "%CD%\RemoteModules.temp.zip" "%CD%\RemoteModuleB.dll"

REM Rename Zip file to XAP
move RemoteModules.temp.zip RemoteModules.xap

REM delete temporary files
del RemoteModuleA.dll
del RemoteModuleB.dll
goto End


:Error
echo -------
echo WARNING
echo -------
echo There was an error creating a mock XAP file from createXap.bat. The test XapModuleTypeLoaderFixture.ShouldLoadDownloadedAssemblies will probably fail because of this reason.
echo .

REM create an blank file to avoid compilation errors for this error only (because the project needs to have the RemoteModules.xap file as a resource).
if not exist RemoteModules.xap type NUL > RemoteModules.xap


:End
popd