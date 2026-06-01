# 🐳 Docker Quick Reference

## شروع سریع

```bash
cd D:\duc\sama-hesab
docker-compose up -d
```

---

## 📊 اطلاعات اتصال

| کلید | مقدار |
|------|-------|
| **Server** | `localhost,1433` |
| **Database** | `SamaHesab` |
| **Username** | `sa` |
| **Password** | `SamaHesab@2024` |

---

## 🔌 Connection String

```
Server=localhost,1433;Database=SamaHesab;User Id=sa;Password=SamaHesab@2024;TrustServerCertificate=True;
```

---

## 📋 دستورات اساسی

| دستور | توضیح |
|-------|-------|
| `docker-compose up -d` | شروع database |
| `docker-compose down` | متوقف کردن |
| `docker-compose ps` | وضعیت container |
| `docker-compose logs -f` | مشاهده logs |
| `docker-compose down -v` | حذف کامل (ڈیٹا شامل) |

---

## 🔧 دستورات پیشرفته

### اتصال مستقیم:
```bash
docker exec -it samahesab-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "SamaHesab@2024"
```

### اجرای Query:
```bash
docker exec samahesab-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "SamaHesab@2024" -Q "SELECT COUNT(*) FROM Companies"
```

### مشاهده Logs دقیق:
```bash
docker-compose logs -f mssql-server --tail=50
```

### بازسازی Database:
```bash
docker-compose down -v
docker-compose up -d --build
```

---

## 🎯 استفاده در WPF

### گام 1: شروع Docker
```bash
docker-compose up -d
```

### گام 2: منتظر بمانید
```
⏳ تا زمان آماده شدن Database (30 ثانیه)
```

### گام 3: اتصال WPF
```
appsettings.json → "Docker" connection string استفاده کنید
```

### گام 4: Run Application
```
F5 یا Debug
```

---

## ⏱️ وضعیت Container

```bash
docker-compose ps
```

**Output:**
```
NAME                STATUS              PORTS
samahesab-db        Up X minutes        0.0.0.0:1433->1433/tcp
```

---

## 🔍 تشخیص مشکلات

### Container شروع نشد:
```bash
docker-compose logs
```

### Password اشتباه:
```bash
# تجدید:
docker-compose down -v
docker-compose up -d --build
```

### Port اشغال است:
```bash
# استفاده از پورت دیگر در docker-compose.yml
ports:
  - "1434:1433"
```

---

## 📈 مانیتورینگ

### مصرف Resource:
```bash
docker stats samahesab-db
```

### Health Check:
```bash
docker-compose ps
```

### Logs Real-time:
```bash
docker-compose logs -f
```

---

## 🔒 نکات امنیتی

- ✅ Password محدود به local development
- ✅ TrustServerCertificate=True برای local testing
- ⚠️ Production میں secure password استفاده کریں

---

## 🧹 تمیزی

### حذف Container:
```bash
docker-compose down
```

### حذف Container + Data:
```bash
docker-compose down -v
```

### حذف Image:
```bash
docker rmi samahesab-db:latest
```

---

## ✅ تست Connection

### PowerShell:
```powershell
$connString = "Server=localhost,1433;Database=SamaHesab;User Id=sa;Password=SamaHesab@2024;TrustServerCertificate=True;"
$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $connString
$connection.Open()

if ($connection.State -eq 'Open') {
    Write-Host "✅ Success" -ForegroundColor Green
} else {
    Write-Host "❌ Failed" -ForegroundColor Red
}
```

### Docker CLI:
```bash
docker exec samahesab-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "SamaHesab@2024" -Q "SELECT @@VERSION"
```

---

## 🚀 Production Setup

### تغییر Password:

**1. Dockerfile:**
```dockerfile
ENV SA_PASSWORD=YourSecurePassword@2024
```

**2. docker-compose.yml:**
```yaml
environment:
  SA_PASSWORD: "YourSecurePassword@2024"
```

**3. appsettings.json:**
```json
"Docker": "Server=localhost,1433;Database=SamaHesab;User Id=sa;Password=YourSecurePassword@2024;..."
```

---

## 📚 اضافی

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [SQL Server on Docker](https://hub.docker.com/_/microsoft-mssql-server)
- مرجع مکمل: `DOCKER_SETUP.md`

---

**Happy Docker Development! 🐳🚀**
