@Title 删除SteamVR库
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo 请确认是否删除SteamVR库?
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

cd ./../../../

rmdir /q /s %cd%\SteamVR
del /f /q SteamVR.meta
del /f /q /ah SteamVR.meta

;不再移除当前目录 del /f /q /s %currentDirectory%.meta
;不再移除当前目录 del /f /q /s /ah %currentDirectory%.meta
;不再移除当前目录 rmdir /q /s %currentDirectory%

exit