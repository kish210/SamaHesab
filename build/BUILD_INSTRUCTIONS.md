# 🏗️ SamaHesab - Build Instructions

## ⚙️ پیش‌نیازها

- Visual Studio 2022 (یا بعد میں)
- .NET 9 SDK
- Windows 10/11

---

## 🚀 Method 1: Visual Studio میں Build کریں (آسان)

### گام 1: Solution کھولیں
```
D:\duc\sama-hesab\SamaHesab.sln
```

### گام 2: Build Configuration تبدیل کریں
```
1. Top میں "Debug" dropdown کلیک کریں
2. "Release" منتخب کریں
3. Platform: x64
```

### گام 3: Build کریں
```
Build → Build Solution (Ctrl+Shift+B)
```

### گام 4: EXE تلاش کریں
```
📍 Location:
src\SamaHesab.WPF\bin\Release\net9.0-windows\SamaHesab.WPF.exe

✅ یہ executable استعمال کریں
```

---

## 🔧 Method 2: PowerShell سے Build کریں (بہتر)

### گام 1: PowerShell کھولیں
```powershell
cd D:\duc\sama-hesab
```

### گام 2: Build چلائیں
```powershell
cd src\SamaHesab.WPF

# Self-contained executable (سب کچھ شامل)
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Output:
# bin\Release\net9.0-windows\win-x64\publish\SamaHesab.WPF.exe
```

### گام 3: Copy to Build Folder
```powershell
# PowerShell میں:
Copy-Item "bin\Release\net9.0-windows\win-x64\publish\SamaHesab.WPF.exe" "..\..\build\SamaHesab.exe"
```

---

## 📦 Installer بنائیں (WiX)

### Install WiX Toolset
```powershell
# PowerShell Admin میں:
dotnet tool install --global wix

# یا download کریں:
# https://wixtoolset.org/releases/
```

### Build Installer
```powershell
wix extension add WixToolset.UI.wixext
wix build installer.wxs -o SamaHesab-Installer.exe
```

---

## 📋 Build Output Files

### Method 1: Simple EXE
```
✅ SamaHesab.WPF.exe (50-80 MB)
- .NET runtime ضروری
```

### Method 2: Self-Contained
```
✅ SamaHesab.WPF.exe (150-200 MB)
- مکمل خود مختار
- کوئی .NET نصب نہیں چاہیے
- Recommended!
```

### Method 3: Installer
```
✅ SamaHesab-Setup.exe (120-150 MB)
- احترافی installation
- Desktop shortcut
- Uninstall support
```

---

## ✅ Verification

### EXE آزمائیں:
```powershell
D:\duc\sama-hesab\build\SamaHesab.exe
```

### Expected:
```
✅ Application شروع ہوتی ہے
✅ Login screen دکھائی دے
✅ Docker database سے متصل ہو
```

---

## 🎯 Final Steps

```bash
# 1. Build کریں
cd D:\duc\sama-hesab\src\SamaHesab.WPF
dotnet publish -c Release -r win-x64 --self-contained

# 2. Copy کریں
Copy-Item "bin\Release\net9.0-windows\win-x64\publish\SamaHesab.WPF.exe" "..\..\build\SamaHesab.exe"

# 3. Git میں شامل کریں
cd D:\duc\sama-hesab
git add build/
git commit -m "Add release executables"
git push

# 4. Release بنائیں (GitHub)
# GitHub → Releases → Create new release
# Files attach کریں
```

---

## 🐳 Docker Database Testing

```bash
# Start database
docker-compose up -d

# Run EXE
build\SamaHesab.exe
```

---

## 🔗 Helpful Links

- [Visual Studio Download](https://visualstudio.microsoft.com/downloads/)
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [WiX Toolset](https://wixtoolset.org/)

---

**اپنے machine پر یہ steps چلائیں اور EXEs GitHub میں ڈالیں!** 🚀
