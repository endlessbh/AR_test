@Title 删除EasyAR库
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo 请确认是否删除EasyAR库?
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

rmdir /q /s %cd%\EasyARUnity.bundle
del /f /q /s EasyARUnity.bundle.meta
del /f /q /s /ah EasyARUnity.bundle.meta

del /f /q /s Android\libs\EasyAR.jar
del /f /q /s Android\libs\EasyAR.jar.meta
del /f /q /s /ah Android\libs\EasyAR.jar.meta

del /f /q /s Android\libs\armeabi-v7a\libEasyAR.so
del /f /q /s Android\libs\armeabi-v7a\libEasyAR.so.meta
del /f /q /s /ah Android\libs\armeabi-v7a\libEasyAR.so.meta

del /f /q /s Android\libs\armeabi-v7a\libEasyARUnity.so
del /f /q /s Android\libs\armeabi-v7a\libEasyARUnity.so.meta
del /f /q /s /ah Android\libs\armeabi-v7a\libEasyARUnity.so.meta

del /f /q /s iOS\EasyARAppController.mm
del /f /q /s iOS\EasyARAppController.mm.meta
del /f /q /s /ah iOS\EasyARAppController.mm.meta

del /f /q /s iOS\libEasyARUnity.a
del /f /q /s iOS\libEasyARUnity.a.meta
del /f /q /s /ah iOS\libEasyARUnity.a.meta

del /f /q /s x86\EasyARUnity.dll
del /f /q /s x86\EasyARUnity.dll.meta
del /f /q /s /ah x86\EasyARUnity.dll.meta

del /f /q /s x86_64\EasyARUnity.dll
del /f /q /s x86_64\EasyARUnity.dll.meta
del /f /q /s /ah x86_64\EasyARUnity.dll.meta

rmdir /q /s %cd%\EasyAR.bundle
del /f /q /s EasyAR.bundle.meta
del /f /q /s /ah EasyAR.bundle.meta

del /f /q /s Android\libs\EasyAR.jar
del /f /q /s Android\libs\EasyAR.jar.meta
del /f /q /s /ah Android\libs\EasyAR.jar.meta

del /f /q /s Android\libs\armeabi-v7a\libEasyAR.so
del /f /q /s Android\libs\armeabi-v7a\libEasyAR.so.meta
del /f /q /s /ah Android\libs\armeabi-v7a\libEasyAR.so.meta

del /f /q /s Android\libs\arm64-v8a\libEasyAR.so
del /f /q /s Android\libs\arm64-v8a\libEasyAR.so.meta
del /f /q /s /ah Android\libs\arm64-v8a\libEasyAR.so.meta

rmdir /q /s %cd%\iOS\easyar.framework
del /f /q /s iOS\easyar.framework.meta
del /f /q /s /ah iOS\easyar.framework.meta

del /f /q /s x64\bin\EasyAR.dll
del /f /q /s x64\bin\EasyAR.dll.meta
del /f /q /s /ah x86\bin\EasyAR.dll.meta

del /f /q /s x86\bin\EasyAR.dll
del /f /q /s x86\bin\EasyAR.dll.meta
del /f /q /s /ah x86\bin\EasyAR.dll.meta

cd ./../

rmdir /q /s %cd%\EasyAR
del /f /q EasyAR.meta
del /f /q /ah EasyAR.meta


rmdir /q /s %cd%\Samples
del /f /q Samples.meta
del /f /q /ah Samples.meta

cd StreamingAssets

rmdir /q /s %cd%\Coloring3D
del /f /q /s Coloring3D.meta
del /f /q /s /ah Coloring3D.meta

del /f /q /s argame00.jpg
del /f /q /s argame00.jpg.meta
del /f /q /s /ah argame00.jpg.meta

del /f /q /s idback.etd
del /f /q /s idback.etd.meta
del /f /q /s /ah idback.etd.meta

del /f /q /s idback.jpg
del /f /q /s idback.jpg.meta
del /f /q /s /ah idback.jpg.meta

del /f /q /s namecard.etd
del /f /q /s namecard.etd.meta
del /f /q /s /ah namecard.etd.meta

del /f /q /s namecard.jpg
del /f /q /s namecard.jpg.meta
del /f /q /s /ah namecard.jpg.meta

;不再移除当前目录 del /f /q /s %currentDirectory%.meta
;不再移除当前目录 del /f /q /s /ah %currentDirectory%.meta
;不再移除当前目录 rmdir /q /s %currentDirectory%

exit