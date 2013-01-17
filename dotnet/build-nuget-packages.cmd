@echo off
call "%VS100COMNTOOLS%vsvars32.bat"
mkdir log\
mkdir lib\net40\

msbuild.exe /ToolsVersion:4.0 "src\project\project.csproj" /p:configuration=Release
utilities\nuget.exe pack project.nuspec
pause