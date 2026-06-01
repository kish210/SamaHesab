# 🐳 Docker Setup Guide - SamaHesab ERP

## نصب و راه‌اندازی دیتابیس با Docker

---

## 📋 پیش‌نیازها

### Windows:
- [Docker Desktop for Windows](https://www.docker.com/products/docker-desktop)
- Windows 10/11 (Pro, Enterprise, or Education)
- 4GB RAM minimum for Docker
- WSL 2 (Windows Subsystem for Linux 2)

### Mac:
- [Docker Desktop for Mac](https://www.docker.com/products/docker-desktop)

### Linux:
```bash
sudo apt-get install docker.io docker-compose
```

---

## 🚀 شروع سریع (Quick Start)

### 1️⃣ **شروع دیتابیس:**

```bash
cd D:\duc\sama-hesab

# Build and run the container
docker-compose up -d
```

### 2️⃣ **صبر کنید تا Database آماده شود:**

```bash
# Check status
docker-compose ps

# View logs
docker-compose logs -f
```

### ✅ **وقتی این پیام دیدید, آماده است:**
```
✅ Database initialization completed!
🎉 SamaHesab Database is Ready!
```

### 3️⃣ **اتصال از WPF:**

```
Server=localhost,1433
Database=SamaHesab
User Id=sa
Password=SamaHesab@2024
TrustServerCertificate=True
```

---

## 📝 دستورات مفید

### شروع دیتابیس:
```bash
docker-compose up -d
```

### متوقف کردن:
```bash
docker-compose down
```

### مشاهده Logs:
```bash
docker-compose logs -f mssql-server
```

### دسترسی به SQL Server:
```bash
docker exec -it samahesab-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "SamaHesab@2024"
```

### پاک کردن همه (ڈیٹا شامل):
```bash
docker-compose down -v
```

### تجدید:
```bash
docker-compose down
docker-compose up -d --build
```

---

## 🔧 اتصال از SQL Server Management Studio

### Server Address:
```
localhost,1433
```

### Authentication:
```
SQL Server Authentication
Username: sa
Password: SamaHesab@2024
```

### Database:
```
SamaHesab
```

---

## 🔌 اتصال از WPF Application

### appsettings.json (Docker):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=SamaHesab;User Id=sa;Password=SamaHesab@2024;TrustServerCertificate=True;Encrypt=false;"
  }
}
```

### appsettings.json (Local SQL Server):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=SamaHesab;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## 🐛 مشکلات و حل‌ها

### ❌ پیام: "Cannot connect to Docker daemon"

**حل:**
```bash
# Start Docker Desktop (Windows/Mac)
# یا شروع Docker service (Linux):
sudo systemctl start docker
```

---

### ❌ پیام: "Port 1433 is already in use"

**حل 1:** استفاده از پورت متفاوت

```yaml
# docker-compose.yml
ports:
  - "1434:1433"  # استفاده از 1434 بجای 1433
```

سپس connection string:
```
Server=localhost,1434;...
```

**حل 2:** متوقف کردن container قدیمی

```bash
docker-compose down
docker container prune
docker-compose up -d
```

---

### ❌ پیام: "Database initialization failed"

**حل:**

```bash
# مشاهده جزئیات خطا
docker-compose logs mssql-server

# تجدید کامل
docker-compose down -v
docker-compose up -d --build
```

---

### ❌ پیام: "Connection timeout"

**حل:**

```bash
# منتظر بمانید تا SQL Server شروع شود (30 ثانیه)
docker-compose logs -f

# Check if container is running
docker ps | grep samahesab-db
```

---

## 📊 Container Information

### Image Details:
```
Base: mcr.microsoft.com/mssql/server:2022-latest
```

### Container Name:
```
samahesab-db
```

### Default Port:
```
1433
```

### Default SA Password:
```
SamaHesab@2024
```

### Volumes:
```
mssql-data       → Database files
mssql-logs       → Backup/logs
```

---

## 🔄 Database Scripts

تمام scripts به ترتیب اجرا می‌شوند:

| # | Script | هدف |
|---|--------|------|
| 1 | 01_CreateDatabase.sql | ایجاد دیتابیس |
| 2 | 02_CreateTables.sql | ایجاد جداول |
| 3 | 03_CreateIndexes.sql | ایجاد indexes |
| 4 | 04_CreateViews.sql | ایجاد views |
| 5 | 05_StoredProcedures.sql | ایجاد stored procedures |
| 6 | 06_SeedData.sql | داده های نمونه |
| 7 | 07_DefaultChartOfAccounts.sql | چارت حسابها |

---

## ✅ تست کردن اتصال

### PowerShell:

```powershell
# Install SqlServer module (یکبار)
Install-Module SqlServer -Force

# Test connection
$connString = "Server=localhost,1433;Database=SamaHesab;User Id=sa;Password=SamaHesab@2024;TrustServerCertificate=True;"
$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $connString
$connection.Open()

if ($connection.State -eq 'Open') {
    Write-Host "✅ Connection Successful!" -ForegroundColor Green
    $connection.Close()
} else {
    Write-Host "❌ Connection Failed!" -ForegroundColor Red
}
```

### Bash/PowerShell:

```bash
docker exec samahesab-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "SamaHesab@2024" -Q "SELECT @@VERSION"
```

---

## 🔒 Security Notes

⚠️ **توجه:** این password برای development است.

### برای Production:

1. تغییر password در Dockerfile:
```dockerfile
ENV SA_PASSWORD=YourSecurePassword@2024
```

2. تغییر در docker-compose.yml:
```yaml
environment:
  SA_PASSWORD: "YourSecurePassword@2024"
```

3. استفاده از .env file:
```bash
# .env
SA_PASSWORD=YourSecurePassword@2024
```

4. استفاده در docker-compose.yml:
```yaml
environment:
  SA_PASSWORD: ${SA_PASSWORD}
```

---

## 📈 Performance Tips

### Docker Resources:

1. **CPU Limit:**
```yaml
deploy:
  resources:
    limits:
      cpus: '2'
    reservations:
      cpus: '1'
```

2. **Memory Limit:**
```yaml
mem_limit: 2g
memswap_limit: 2g
```

3. **مثال:**
```yaml
services:
  mssql-server:
    # ... other config ...
    deploy:
      resources:
        limits:
          cpus: '2'
          memory: 2G
        reservations:
          cpus: '1'
          memory: 1G
```

---

## 🔗 Networking

### اتصال بین Containers:

اگر WPF app درون Docker است:
```
Server=mssql-server,1433
```

اگر WPF app بیرون Docker است:
```
Server=localhost,1433
```

---

## 📚 مراجع

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)
- [SQL Server on Docker](https://hub.docker.com/_/microsoft-mssql-server)
- [Microsoft SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-2022)

---

## 🎉 تمام!

دیتابیس شما در Docker آماده تست است!

```bash
docker-compose up -d
```

**Happy Testing! 🚀**
