# 🐳 SamaHesab Docker - آسان و سریع

## بدون پیچیدگی - فقط کامپیوتر و تست!

### 1️⃣ **شروع:**

```bash
docker-compose up -d
```

### ✅ **آماده است:**

```
Server:     localhost,1433
Username:   sa
Password:   SamaHesab@2024
Database:   SamaHesab (خالی - برای ایجاد در WPF)
```

### 2️⃣ **اتصال از WPF:**

```
appsettings.json:
Server=localhost,1433;Database=SamaHesab;User Id=sa;Password=SamaHesab@2024;TrustServerCertificate=True;
```

### 3️⃣ **اجرای Application:**

```
F5 در Visual Studio
↓
EF Core Migrations خودکار جدول‌ها می‌سازد
↓
آماده تست!
```

---

## 📋 دستورات سریع

```bash
# شروع
docker-compose up -d

# توقف
docker-compose down

# Restart
docker-compose down
docker-compose up -d

# حذف کامل (ڈیٹا شامل)
docker-compose down -v

# وضعیت
docker-compose ps

# Logs
docker-compose logs -f
```

---

## 🎯 دو نسخه:

### نسخه 1: Standalone (SQL Express)
```
آماده: فقط executable و SQL Express Local DB
فایل: SamaHesab.exe
```

### نسخه 2: Network (MSSQL Enterprise)
```
آماده: Server + SQL Server 2019/2022
فایل: appsettings.json تغییر server
```

---

## ✨ فوری کار کنید!

- ✅ بدون SQL Server Local نصب
- ✅ یک دستور = Database آماده
- ✅ Migrations خودکار
- ✅ آسان تست

**تمام!** 🚀
