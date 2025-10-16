@echo off
REM ========================================
REM Student Enrollment System - Windows Publish Script
REM ========================================
REM This script creates a self-contained Windows deployment
REM that includes the .NET runtime (no installation needed)
REM ========================================

echo.
echo ========================================
echo Student Enrollment System
echo Windows Deployment Publisher
echo ========================================
echo.

REM Change to project root directory
cd /d "%~dp0.."

REM Clean previous builds
echo [1/4] Cleaning previous builds...
if exist "bin\Release" rmdir /s /q "bin\Release"
if exist "DEPLOYMENT_OUTPUT\windows" rmdir /s /q "DEPLOYMENT_OUTPUT\windows"

REM Publish for Windows x64 (self-contained)
echo.
echo [2/4] Publishing for Windows (x64)...
echo This may take a few minutes...
dotnet publish -c Release -r win-x64 --self-contained true -o DEPLOYMENT_OUTPUT\windows\StudentEnrollmentSystem

if errorlevel 1 (
    echo.
    echo ERROR: Publish failed. Please check the error messages above.
    pause
    exit /b 1
)

REM Copy additional files
echo.
echo [3/4] Copying documentation files...
copy /y "ACCOUNTS.txt" "DEPLOYMENT_OUTPUT\windows\StudentEnrollmentSystem\" 2>nul
copy /y "README_FOR_CLIENT.txt" "DEPLOYMENT_OUTPUT\windows\StudentEnrollmentSystem\README.txt" 2>nul

REM Create launch script
echo.
echo [4/4] Creating launcher script...
(
echo @echo off
echo echo Starting Student Enrollment System...
echo echo.
echo echo Opening browser at http://localhost:5000
echo echo Press Ctrl+C to stop the server
echo echo.
echo start http://localhost:5000
echo StudentEnrollmentSystem.exe
) > "DEPLOYMENT_OUTPUT\windows\StudentEnrollmentSystem\START_SERVER.bat"

REM Success message
echo.
echo ========================================
echo SUCCESS!
echo ========================================
echo.
echo Deployment package created at:
echo %CD%\DEPLOYMENT_OUTPUT\windows\StudentEnrollmentSystem
echo.
echo Package size: ~60-80 MB
echo.
echo Next steps:
echo 1. Compress the 'StudentEnrollmentSystem' folder to ZIP
echo 2. Send ZIP file to client
echo 3. Client extracts ZIP and runs START_SERVER.bat
echo.
echo ========================================
pause
