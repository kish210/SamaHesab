# 📦 Sama Hesab — Build Artifacts

This folder contains ready-to-run distributables of **سما حساب (Sama Hesab)**.

| File | Size | Description |
|------|------|-------------|
| `SamaHesab_Setup.msi` | ~7.7 MB | **Installer** — double-click to install. Creates Start Menu + Desktop shortcuts and an uninstall entry. |
| `SamaHesab.exe` | ~29 MB | **Portable app** — single-file, run directly without installing. |

## Requirements

Both builds are **framework-dependent**, so the target machine needs the
**.NET 9 Desktop Runtime (x64)**:

- Download: https://dotnet.microsoft.com/download/dotnet/9.0 → *.NET Desktop Runtime 9.x*

> A fully self-contained build (no runtime needed, ~163 MB) can be produced with
> `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true`.
> It is too large for the git repo — publish it to GitHub **Releases** instead.

## Two editions

| Edition | Connection string (`appsettings.json`) |
|---------|----------------------------------------|
| **Standalone** (single PC, SQL Express) | `Server=.\SQLEXPRESS;Database=SamaHesab;Trusted_Connection=True;TrustServerCertificate=True;` |
| **Networked** (shared SQL Server) | `Server=SERVER_NAME;Database=SamaHesab;User Id=sa;Password=***;TrustServerCertificate=True;` |

The database is created from the scripts in `/database` (or via `docker-compose up -d`).

## Default login

```
Username: admin
Password: admin123
```

## Rebuilding

```powershell
# Portable single-file exe
dotnet publish src/SamaHesab.WPF -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o build/app
Copy-Item build/app/SamaHesab.WPF.exe build/SamaHesab.exe -Force

# MSI installer (requires WiX v5: dotnet tool install --global wix --version 5.0.2)
wix build installer/Package.wxs -o build/SamaHesab_Setup.msi
```
