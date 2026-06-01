@echo off
echo ========================================
echo     SAMA HESAB ERP - Build Script
echo     سامانه جامع سما حساب
echo ========================================
echo.

:: Check .NET SDK
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] .NET SDK not found. Please install .NET 9 SDK.
    pause
    exit /b 1
)

echo [1/4] Restoring NuGet packages...
dotnet restore "%~dp0SamaHesab.sln" --no-cache
if %errorlevel% neq 0 goto :error

echo [2/4] Building solution (Debug)...
dotnet build "%~dp0SamaHesab.sln" --configuration Debug --no-restore
if %errorlevel% neq 0 goto :error

echo [3/4] Building solution (Release)...
dotnet build "%~dp0SamaHesab.sln" --configuration Release --no-restore
if %errorlevel% neq 0 goto :error

echo [4/4] Publishing WPF application...
dotnet publish "%~dp0src\SamaHesab.WPF\SamaHesab.WPF.csproj" ^
    --configuration Release ^
    --runtime win-x64 ^
    --self-contained false ^
    --output "%~dp0publish" ^
    /p:PublishSingleFile=false
if %errorlevel% neq 0 goto :error

echo.
echo ========================================
echo [SUCCESS] Build completed successfully!
echo Output: %~dp0publish
echo ========================================
pause
exit /b 0

:error
echo.
echo ========================================
echo [FAILED] Build failed with error %errorlevel%
echo ========================================
pause
exit /b %errorlevel%
