# سامانه جامع سما حساب
## راهنمای نصب و راه‌اندازی

### پیش‌نیازها
- Windows 10/11 x64
- .NET 9 SDK
- Microsoft SQL Server 2019/2022 (Express یا بالاتر)
- Visual Studio 2022 یا JetBrains Rider
- Inno Setup 6.x (برای ساخت نصاب)

### مراحل راه‌اندازی

#### ۱. پایگاه داده
```sql
-- در SQL Server Management Studio اجرا کنید:
-- 1. database/01_CreateDatabase.sql
-- 2. database/02_CreateTables.sql
-- 3. database/03_CreateIndexes.sql
-- 4. database/04_CreateViews.sql
-- 5. database/05_StoredProcedures.sql
-- 6. database/06_SeedData.sql
```

#### ۲. تنظیم رشته اتصال
فایل `src/SamaHesab.WPF/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SamaHesab;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

#### ۳. نصب پکیج‌ها و Build
```bash
cd src
dotnet restore
dotnet build --configuration Release
```

#### ۴. اجرا
```bash
dotnet run --project SamaHesab.WPF
```

#### ۵. ورود اولیه
- نام کاربری: `admin`
- رمز عبور: `admin123`

### ساخت Installer
```bash
# Install Inno Setup, then:
iscc installer/SamaHesab_Setup.iss
```

### ماژول‌های پیاده‌سازی شده
- ✅ حسابداری (اسناد، نمودار حساب‌ها، چک، بانک)
- ✅ انبار (کالا، انبارها، موجودی، تعدیل)
- ✅ فروش (فاکتور، برگشت، پیش‌فاکتور)
- ✅ خرید (فاکتور، برگشت)
- ✅ POS (صندوق لمسی، بارکد، پرداخت)
- ✅ مشتریان و تأمین‌کنندگان
- ✅ منابع انسانی (پرونده، حقوق، حضور)
- ✅ گزارش‌ها (PDF، Excel)
- ✅ پشتیبان‌گیری خودکار
- ✅ مدیریت کاربران و دسترسی‌ها
- ✅ SMS (کاوه‌نگار، فراز، ملی پیامک)
- ✅ داشبورد حرفه‌ای
- ✅ تم تاریک/روشن
- ✅ تقویم شمسی
- ✅ مدیریت چندشعبه
