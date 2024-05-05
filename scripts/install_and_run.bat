@echo off



if "%~1"=="" (
    echo Usage: %0 pathToGame
    exit /b 1
)

set "projectDllPath=bin\Debug\net46\QMarket.dll"
set "gameDllPath=%~1\BepInEx\plugins\QMarket.dll"
set "gameExePath=%~1\Supermarket Simulator.exe"



echo Stopping the game if it's already running...
taskkill /f /im "Supermarket Simulator.exe" >nul 2>&1
timeout 2

echo Copying file from %projectDllPath% to %gameDllPath%...
copy "%projectDllPath%" "%gameDllPath%"
echo File copied successfully.

echo Starting the game...
start /min "" "%gameExePath%"
