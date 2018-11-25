@echo OFF

REM get the version number from file
for /f "tokens=2 delims=()" %%i in ('findstr /r AssemblyFileVersion.* GlobalVersionInfo.cs') do set version=%%i
REM format is "x.y.z.0"m drop the first and last 3 chars to get nuget friendly version
SET version=%version:~1,-3%
echo Using version %version% for nuget package

cd MonoGame.Framework.WpfInterop
msbuild MonoGame.Framework.WpfInterop.csproj /p:Configuration=Release
cd bin\Release
nuget pack MonoGame.Framework.WpfInterop.nuspec -Version %version% -Properties Configuration=Release -OutputDirectory ..\..\..
cd ..\..\..