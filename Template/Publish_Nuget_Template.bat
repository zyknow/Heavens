@echo off

color 3

chcp 65001

if not exist nuget.exe (
	echo 下载nuget包
	curl "https://dist.nuget.org/win-x86-commandline/v6.0.0/nuget.exe" > .\nuget.exe
)

rd /s /q Heavens

git clone https://github.com/zyknow/Heavens.git

rd /s /q Heavens\.git

xcopy /s /e /y /q /a /h content\* contentTemp\*

xcopy /s /e /y /q /a /h Heavens\* content\*

:: 删除Nuget旧包
del *.nupkg*

nuget pack heavens.nuspec -NoDefaultExcludes

echo 打包Nuget成功

rd /s /q content Heavens
move contentTemp content

echo 生成Nuget包成功
