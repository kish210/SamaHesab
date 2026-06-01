# 📦 Build Folder - Release Executables

## محتویات

```
build/
├── SamaHesab.exe           ← Standalone executable
├── SamaHesab-Setup.exe     ← Installer (آئندہ)
├── BUILD_INSTRUCTIONS.md   ← تفصیلی راہنمائی
├── BUILD.ps1               ← خودکار build script
└── README.md               ← یہ فائل
```

---

## 🚀 فوری شروعات

### آپ کے مشین پر:

```powershell
cd D:\duc\sama-hesab

# Build script چلائیں
.\build\BUILD.ps1

# یا خود سے:
cd src\SamaHesab.WPF
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
Copy-Item "bin\Release\net9.0-windows\win-x64\publish\SamaHesab.WPF.exe" "..\..\build\SamaHesab.exe"
```

---

## 📋 Version Info

| فائل | Version | Size | تاریخ |
|------|---------|------|--------|
| SamaHesab.exe | 1.0.0 | TBD | TBD |
| SamaHesab-Setup.exe | 1.0.0 | TBD | TBD |

---

## 💻 System Requirements

### برائے SamaHesab.exe (Self-Contained)
```
✅ Windows 10 / 11
✅ 500 MB free space
✅ SQL Server / Docker database
✅ کوئی اضافی installation نہیں
```

### برائے SamaHesab-Setup.exe (Installer)
```
✅ Windows 10 / 11
✅ 500 MB free space
✅ SQL Server 2019+ یا Docker
```

---

## 🔗 Release Links

- GitHub Releases: [SamaHesab Releases](https://github.com/kish210/SamaHesab/releases)
- Docker Setup: [DOCKER_README.md](../DOCKER_README.md)
- Build Instructions: [BUILD_INSTRUCTIONS.md](./BUILD_INSTRUCTIONS.md)

---

## 📝 Build History

### v1.0.0 (پہلی بار)
```
✅ WPF UI مکمل
✅ Docker support
✅ تمام 14 modules
✅ Production ready
```

---

## 🐛 مسائل؟

### EXE کام نہیں کر رہی؟
```
1. Docker database چلتی ہے؟
   docker-compose ps

2. Connection string صحیح ہے؟
   appsettings.json چیک کریں

3. SQL Server/Docker سے متصل ہو؟
   Test connection
```

---

**آخری update: 2026-06-01** 📅
