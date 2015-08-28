@echo off

cd /d "%~p0"

set msbuild="%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set project=.\HearthCap\HearthCap.csproj
set msbuildparams=/p:SolutionDir="%~dp0\" /p:Configuration=Release /t:ReBuild /nologo

rmdir /s /q .\HearthCap\bin\Release
%msbuild% %project% %msbuildparams%