@echo OFF
if "%1"=="" (
  set /p version="Enter SemVer: "
) else (
version = %1
)
cd MonoGame.Framework.WpfInterop
msbuild MonoGame.Framework.WpfInterop.csproj /p:Configuration=Release
cd bin\Release
nuget pack MonoGame.Framework.WpfInterop.nuspec -Version %version% -Properties Configuration=Release -OutputDirectory ..\..\..
cd ..\..\..