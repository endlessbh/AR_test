@Title 删除HighlightingSystem库
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo 请确认是否删除HighlightingSystem库?
)else (
	if "%one%"=="Y" (
		goto :excute
	)
	if "%one%"=="y" (
		goto :excute
	)else (
		exit
	)
)

set /p c=请输入指令(Y/N):
set one=%c%
goto :condition

:excute

set currentDirectory=%cd%

pushd %cd%

% ---assets根目录--- %

cd ./../../../

rmdir /q /s %cd%\HighlightingSystemDemo
del /f /q /s HighlightingSystemDemo.meta
del /f /q /s /ah HighlightingSystemDemo.meta

cd ./Plugins

rmdir /q /s %cd%\HighlightingSystem
del /f /q HighlightingSystem.meta
del /f /q /ah HighlightingSystem.meta


exit