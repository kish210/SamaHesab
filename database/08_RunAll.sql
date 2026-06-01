-- =============================================================================
-- SAMA HESAB ERP - Run All Scripts in Order
-- اجرای تمام اسکریپت‌ها به ترتیب
-- =============================================================================

:r 01_CreateDatabase.sql
:r 02_CreateTables.sql
:r 03_CreateIndexes.sql
:r 04_CreateViews.sql
:r 05_StoredProcedures.sql
:r 06_SeedData.sql
:r 07_DefaultChartOfAccounts.sql

PRINT N'تمام اسکریپت‌های پایگاه داده با موفقیت اجرا شدند.';
PRINT N'نرم‌افزار سما حساب آماده استفاده است.';
GO
