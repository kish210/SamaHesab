# 🚀 سما حساب - GitHub Setup Guide

## راه‌اندازی پروژه روی GitHub

### مرحله ۱: ایجاد Repository روی GitHub

```
1. برو به github.com
2. کلیک کن روی "+" → "New repository"
3. نام: SamaHesab
4. توضیح: Enterprise ERP System
5. Public یا Private (به انتخابت)
6. کلیک "Create repository"
```

### مرحله ۲: Push کردن کد به GitHub

```bash
cd D:\duc\sama-hesab

# اگر git مقدازتر initialize نشده:
git init
git add .
git commit -m "Initial commit: Complete ERP system with all modules

Co-Authored-By: Sama Software <info@samanarm.ir>"

git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/SamaHesab.git
git push -u origin main
```

**جایگزین کن:** `YOUR_USERNAME` را با username GitHub خودت

### مرحله ۳: GitHub Actions Builds

✅ **خودکار بناخته می‌شود وقتی:**
- Push کنی به `main` یا `develop`
- Pull Request بسازی

**چک کن:**
```
1. برو به GitHub repository
2. Actions tab کلیک کن
3. ببین build status
```

### مرحله ۴: Deploy کردن

برای ایجاد Release:

```bash
# Tag کردن نسخه
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0
```

سپس GitHub Release درست می‌کنه خودکار.

## 📋 Build Status Badge

Add این رو به README.md:

```markdown
[![Build & Test](https://github.com/YOUR_USERNAME/SamaHesab/workflows/Build%20&%20Test/badge.svg)](https://github.com/YOUR_USERNAME/SamaHesab/actions)
```

## 🔧 Branch Strategy

```
main              ← Production ready (releases)
├── develop       ← Development branch
│   ├── feature/* ← New features
│   └── hotfix/*  ← Urgent fixes
```

### مثال: Feature Branch

```bash
# بسازی feature branch
git checkout -b feature/add-dashboard

# کار رو انجام بده، commit کن
git add .
git commit -m "Add dashboard module"

# Push به GitHub
git push origin feature/add-dashboard

# Create Pull Request روی GitHub
# Merge بشه بعد از review
```

## 📦 GitHub Packages

(اختیاری) برای NuGet packages:

```bash
# در .github/workflows/publish.yml
# خودکار publish کن NuGet packages
```

## 🔐 Secrets Configuration

اگر API keys لازم داشتی:

```
Settings → Secrets and variables → Actions
Add Secret:
  Name: SQL_CONNECTION_STRING
  Value: Server=...
```

بعد use کنش در workflow:
```yaml
env:
  CONNECTION_STRING: ${{ secrets.SQL_CONNECTION_STRING }}
```

## 📊 Git Stats

```bash
# Commits count
git log --oneline | wc -l

# Contributors
git shortlog -sn

# File changes
git diff main...develop --stat
```

## 🎯 GitHub Pages (Optional)

برای documentation:

```
Settings → Pages
Source: main branch /docs folder
```

## ⚠️ Important Notes

1. **SQL Server**: GitHub Actions runs on Windows
   - Automated tests می‌تونن SQL LocalDB استفاده کنند
   - Integration tests نیاز دارند به SQL Server

2. **Build Time**: ~5-10 minutes per build
   - .NET restore + build + test
   - First build slower (caches build)

3. **Branch Protection**:
   ```
   Settings → Branches → Add Rule
   - Require status checks to pass
   - Require pull request reviews
   ```

## 📝 Commit Message Format

```
feat: Add dashboard module
fix: Correct voucher calculation
docs: Update README
refactor: Reorganize repositories
test: Add unit tests for accounting
ci: Update GitHub Actions workflow
```

## 🚀 Quick Commands

```bash
# Clone کردن project
git clone https://github.com/YOUR_USERNAME/SamaHesab.git
cd SamaHesab

# Setup
dotnet restore src/
dotnet build src/

# Create feature
git checkout -b feature/my-feature
# ... کار کن ...
git push origin feature/my-feature

# Delete branch
git push origin --delete feature/my-feature
git branch -d feature/my-feature
```

## 📞 Collaboration

برای collaboration:

```
Settings → Collaborators
Add teammates
```

Permissions:
- Pull (Read)
- Triage (Manage issues/PRs)
- Push (Write code)
- Admin (Full access)

════════════════════════════════════════════════════════════════════════

✨ Your GitHub workflow is configured!
✨ Every push will auto-build with GitHub Actions

════════════════════════════════════════════════════════════════════════
