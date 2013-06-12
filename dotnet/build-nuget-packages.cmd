@echo off
echo *** Make sure you have updated the assembly and nuspec to match the release version! ***
pause

call "%VS100COMNTOOLS%vsvars32.bat"
mkdir log
mkdir lib\net40
mkdir nupkg_archive

msbuild.exe /ToolsVersion:4.0 "src\project\project.csproj" /p:configuration=Release
.nuget\nuget.exe pack project.nuspec

echo *** Ready to upload to nuget.org ***
pause

for %%f in (*.nupkg) do (
	.nuget\nuget.exe push %%f
)

copy *.nupkg nupkg_archive\
del *.nupkg