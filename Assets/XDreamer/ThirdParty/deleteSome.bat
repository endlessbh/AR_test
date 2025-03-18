@Title …æ≥˝ƒ≥–©ø‚
@echo off

set lib=EasyAR,GoogleVR,HoloLens,zSpace,HTCVive,SteamVR

set one=%1

:condition

if "%one%"=="" (
	echo «Î»∑»œ «∑Ò…æ≥˝%lib%ø‚?
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

set /p c=«Î ‰»Î÷∏¡Ó(Y/N):
set one=%c%
goto :condition

:excute
for /f "delims=" %%i in ('dir /b /a-d /s "*.bat"') do (
	if not %0=="%%i" (
		if not %cd%\==%%~dpi (		
			for %%l in (%lib%) do (	
				if "%cd%\%%l\"=="%%~dpi" (
					start /D %%~dpi deleteAll.bat Y
				)
			)
		)
	)
)

exit