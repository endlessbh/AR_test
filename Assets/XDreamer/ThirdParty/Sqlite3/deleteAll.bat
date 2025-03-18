@Title 删除Sqlite3库
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo 请确认是否删除Sqlite3库?
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

cd ./../../../Plugins

del /f /q /s Mono.Data.Sqlite.dll
del /f /q /s Mono.Data.Sqlite.dll.meta
del /f /q /s /ah Mono.Data.Sqlite.dll.meta

del /f /q /s Android\libsqlite3.so
del /f /q /s Android\libsqlite3.so.meta
del /f /q /s /ah Android\libsqlite3.so.meta

del /f /q /s x86\sqlite3.dll
del /f /q /s x86\sqlite3.dll.meta
del /f /q /s /ah x86\sqlite3.dll.meta

del /f /q /s x86_64\sqlite3.dll
del /f /q /s x86_64\sqlite3.dll.meta
del /f /q /s /ah x86_64\sqlite3.dll.meta

;不再移除当前目录 del /f /q /s %currentDirectory%.meta
;不再移除当前目录 del /f /q /s /ah %currentDirectory%.meta
;不再移除当前目录 rmdir /q /s %currentDirectory%

exit