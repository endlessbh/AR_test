@Title 删除Cross Section Shader库
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo 请确认是否删除Cross Section Shader库?
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

ren "Cross Section Shader" CrossSectionShader
ren "Cross Section Shader.meta" CrossSectionShader.meta

rmdir /q /s %cd%\CrossSectionShader
del /f /q /s CrossSectionShader.meta
del /f /q /s /ah CrossSectionShader.meta

exit