@Title 删除全部第三方库
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo 请确认是否删除全部第三方库?
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

for /f "delims=" %%i in ('dir /b /a-d /s "*.bat"') do (
if not %0=="%%i" (
if not %cd%\==%%~dpi (
start /D %%~dpi deleteAll.bat Y
)
)
)

exit