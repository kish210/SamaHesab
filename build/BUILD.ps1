# SamaHesab Build Script (PowerShell)
# یہ script WPF application کو build کرتا ہے

param(
    [string]$Configuration = "Release",
    [string]$Platform = "x64"
)

$ErrorActionPreference = "Stop"

Write-Host "════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "🔨 SamaHesab Build Script" -ForegroundColor Cyan
Write-Host "════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

$rootDir = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$wfprojDir = Join-Path $rootDir "src\SamaHesab.WPF"

Write-Host "📍 Project Directory: $wfprojDir" -ForegroundColor Yellow

# Check if .NET CLI is available
$dotnetPath = (Get-Command dotnet -ErrorAction SilentlyContinue).Path
if (-not $dotnetPath) {
    Write-Host "❌ .NET CLI not found in PATH" -ForegroundColor Red
    Write-Host "   Please install .NET 9 SDK: https://dotnet.microsoft.com/download/dotnet/9.0" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ .NET CLI found" -ForegroundColor Green

# Check if Visual Studio solution exists
$slnFile = Join-Path $rootDir "SamaHesab.sln"
if (-not (Test-Path $slnFile)) {
    Write-Host "❌ Solution file not found: $slnFile" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Solution found" -ForegroundColor Green
Write-Host ""

# Build
Write-Host "⏳ Building SamaHesab.WPF ($Configuration)..." -ForegroundColor Yellow
Write-Host ""

try {
    Push-Location $wfprojDir

    # Restore packages
    Write-Host "📦 Restoring packages..." -ForegroundColor Yellow
    & dotnet restore

    # Publish
    Write-Host ""
    Write-Host "🔨 Publishing executable..." -ForegroundColor Yellow
    & dotnet publish -c $Configuration -r "win-$Platform" --self-contained `
        -p:PublishSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:DebugType=embedded `
        -p:DebugSymbols=false

    # Copy to build folder
    $publishDir = "bin\$Configuration\net9.0-windows\win-$Platform\publish"
    $exePath = Join-Path $publishDir "SamaHesab.WPF.exe"

    if (Test-Path $exePath) {
        $buildFolder = Join-Path $rootDir "build"
        $targetExe = Join-Path $buildFolder "SamaHesab.exe"

        Write-Host ""
        Write-Host "📋 Copying executable..." -ForegroundColor Yellow
        Copy-Item $exePath $targetExe -Force

        $fileSize = (Get-Item $targetExe).Length / 1MB
        Write-Host ""
        Write-Host "════════════════════════════════════════════════════════" -ForegroundColor Green
        Write-Host "✅ Build Successful!" -ForegroundColor Green
        Write-Host "════════════════════════════════════════════════════════" -ForegroundColor Green
        Write-Host ""
        Write-Host "📦 Output:" -ForegroundColor Green
        Write-Host "   $targetExe" -ForegroundColor Cyan
        Write-Host "   Size: $([Math]::Round($fileSize, 2)) MB" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "🚀 Next Steps:" -ForegroundColor Green
        Write-Host "   1. Test the executable:" -ForegroundColor White
        Write-Host "      & '.\build\SamaHesab.exe'" -ForegroundColor Yellow
        Write-Host "   2. Push to GitHub:" -ForegroundColor White
        Write-Host "      git add build/SamaHesab.exe" -ForegroundColor Yellow
        Write-Host "      git commit -m 'Add release build'" -ForegroundColor Yellow
        Write-Host "      git push" -ForegroundColor Yellow
        Write-Host ""
    } else {
        Write-Host "❌ Executable not found at: $exePath" -ForegroundColor Red
        exit 1
    }

} catch {
    Write-Host "❌ Build failed: $_" -ForegroundColor Red
    exit 1
} finally {
    Pop-Location
}
