@Title 删除HoloLens库
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo 请确认是否删除HoloLens库?
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

rmdir /q /s %cd%\HoloToolkit
del /f /q /s HoloToolkit.meta
del /f /q /s /ah HoloToolkit.meta

rmdir /q /s %cd%\HoloToolkit-Examples
del /f /q /s HoloToolkit-Examples.meta
del /f /q /s /ah HoloToolkit-Examples.meta

rmdir /q /s %cd%\HoloToolkit-Preview
del /f /q /s HoloToolkit-Preview.meta
del /f /q /s /ah HoloToolkit-Preview.meta

exit