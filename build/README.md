# 📦 Sama Hesab — Build Artifacts

Ready-to-run distributables of **سما حساب (Sama Hesab)**. Both are
**self-contained** — they include the .NET 9 runtime, so **no separate
.NET install is required**, and every dependency sits next to the app.

| File | Size | Description |
|------|------|-------------|
| `SamaHesab_Setup.msi` | ~57 MB | **Installer** — double-click → installs to `C:\Program Files\SamaHesab` with all files, plus Start-Menu & Desktop shortcuts and an uninstall entry. |
| `SamaHesab-Portable.zip` | ~68 MB | **Portable** — extract anywhere and run `SamaHesab.exe`. All files live in the same folder. |

## Database (default = Docker)

By default the app connects to the SQL Server defined in `docker-compose.yml`:

```
Server=localhost,1433 ; Database=SamaHesab ; User=sa ; Password=SamaHesab@2024
```

Start it with:

```bash
docker-compose up -d
```

To use a different SQL Server (e.g. a networked instance), click
**«⚙ تنظیمات اتصال به پایگاه داده»** on the login screen and fill in the form
(Server / Database / auth mode / username / password), press **آزمایش اتصال**
to test, then **ذخیره**. Settings are stored in
`%AppData%\SamaHesab\settings.user.json`.

## Default login

```
Username: admin
Password: admin123
```

## Rebuilding

```powershell
# Self-contained folder (all files beside the exe, no runtime needed)
dotnet publish src/SamaHesab.WPF -c Release -r win-x64 --self-contained true -o build/SamaHesab

# MSI installer (WiX v5: dotnet tool install --global wix --version 5.0.2)
wix build installer/Package.wxs -o build/SamaHesab_Setup.msi

# Portable zip
Compress-Archive build/SamaHesab/* build/SamaHesab-Portable.zip
```
