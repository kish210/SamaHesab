-- =============================================================================
-- SAMA HESAB ERP - Seed Data (داده‌های پایه)
-- =============================================================================
USE SamaHesab;
GO

-- Voucher Types
INSERT INTO Acc.VoucherTypes(Code, Name, Prefix) VALUES
(N'OPEN',   N'افتتاحیه',        N'AF'),
(N'CLOSE',  N'اختتامیه',        N'AK'),
(N'SALE',   N'فروش',            N'FR'),
(N'PUR',    N'خرید',            N'KH'),
(N'CASH',   N'صندوق',           N'SN'),
(N'BANK',   N'بانک',            N'BN'),
(N'CHQ',    N'چک',              N'CK'),
(N'ADJ',    N'تعدیل',           N'TA'),
(N'GEN',    N'عمومی',           N'AG'),
(N'PAY',    N'پرداخت',          N'PR'),
(N'RCV',    N'دریافت',          N'DR'),
(N'SAL',    N'حقوق و دستمزد',   N'HD');

-- Stock Transaction Types
INSERT INTO Inv.StockTransactionTypes(Code, Name, Effect) VALUES
(N'خرید',           N'خرید کالا',               1),
(N'فروش',           N'فروش کالا',              -1),
(N'برگشت-خرید',    N'برگشت از خرید',           -1),
(N'برگشت-فروش',    N'برگشت از فروش',            1),
(N'انتقال-خروج',   N'انتقال بین انبار - خروج', -1),
(N'انتقال-ورود',   N'انتقال بین انبار - ورود',  1),
(N'تعدیل-افزایش',  N'تعدیل موجودی - افزایش',   1),
(N'تعدیل-کاهش',    N'تعدیل موجودی - کاهش',    -1),
(N'ضایعات',         N'ضایعات',                  -1),
(N'تولید-ورود',    N'ورود تولید',               1),
(N'تولید-خروج',    N'خروج مواد تولید',          -1);

-- Default Units
INSERT INTO Cfg.Units(Code, Name, Abbreviation) VALUES
(N'عدد',    N'عدد',     N'عدد'),
(N'کیلوگرم',N'کیلوگرم', N'کگ'),
(N'گرم',    N'گرم',     N'گر'),
(N'لیتر',   N'لیتر',    N'لت'),
(N'متر',    N'متر',     N'مت'),
(N'جفت',    N'جفت',     N'جف'),
(N'بسته',   N'بسته',    N'بس'),
(N'کارتن',  N'کارتن',   N'کر'),
(N'دوجین',  N'دوجین',   N'دج'),
(N'روز',    N'روز',     N'رز'),
(N'ساعت',   N'ساعت',    N'سع');

-- Default Currencies
INSERT INTO Cfg.Currencies(Code, Name, Symbol, ExchangeRate, IsBase) VALUES
(N'IRR', N'ریال ایران',     N'﷼',  1,         1),
(N'IRT', N'تومان ایران',    N'ت',  10,        0),
(N'USD', N'دلار آمریکا',    N'$',  600000,    0),
(N'EUR', N'یورو',           N'€',  650000,    0),
(N'AED', N'درهم امارات',    N'د',  163000,    0);

-- Default Security Roles
INSERT INTO Sec.Roles(Code, Name, Description, IsSystem) VALUES
(N'ADMIN',      N'مدیر سیستم',      N'دسترسی کامل به تمام بخش‌ها',            1),
(N'MANAGER',    N'مدیر',            N'مدیریت عملیات و گزارش‌ها',              0),
(N'ACCOUNTANT', N'حسابدار',         N'مدیریت سیستم حسابداری',                0),
(N'WAREHOUSE',  N'مسئول انبار',     N'مدیریت انبار و موجودی',                 0),
(N'SALES',      N'فروشنده',         N'ثبت فاکتور فروش و مشتریان',             0),
(N'PURCHASE',   N'مسئول خرید',      N'ثبت فاکتور خرید',                       0),
(N'CASHIER',    N'صندوقدار',        N'عملیات صندوق و POS',                    0),
(N'HR',         N'مسئول منابع انسانی', N'مدیریت کارکنان و حقوق',              0),
(N'VIEWER',     N'مشاهده‌کننده',    N'فقط مشاهده گزارش‌ها',                  0);

-- Permissions
INSERT INTO Sec.Permissions(ModuleCode, FeatureCode, Name) VALUES
-- Accounting
(N'ACC', N'ACC.VOUCHER',        N'اسناد حسابداری'),
(N'ACC', N'ACC.ACCOUNT',        N'حساب‌ها'),
(N'ACC', N'ACC.BANK',           N'بانک و صندوق'),
(N'ACC', N'ACC.CHEQUE',         N'مدیریت چک'),
(N'ACC', N'ACC.REPORT',         N'گزارش‌های مالی'),
(N'ACC', N'ACC.RECONCILE',      N'مغایرت بانکی'),
-- Inventory
(N'INV', N'INV.PRODUCT',        N'مدیریت کالا'),
(N'INV', N'INV.WAREHOUSE',      N'مدیریت انبار'),
(N'INV', N'INV.STOCK',          N'عملیات انبار'),
(N'INV', N'INV.TRANSFER',       N'انتقال بین انبار'),
(N'INV', N'INV.ADJUST',         N'تعدیل موجودی'),
-- Sales
(N'SAL', N'SAL.INVOICE',        N'فاکتور فروش'),
(N'SAL', N'SAL.RETURN',         N'برگشت فروش'),
(N'SAL', N'SAL.QUOTATION',      N'پیش‌فاکتور'),
(N'SAL', N'SAL.REPORT',         N'گزارش فروش'),
-- Purchase
(N'PUR', N'PUR.INVOICE',        N'فاکتور خرید'),
(N'PUR', N'PUR.RETURN',         N'برگشت خرید'),
(N'PUR', N'PUR.ORDER',          N'سفارش خرید'),
-- POS
(N'POS', N'POS.SALE',           N'فروش صندوق'),
(N'POS', N'POS.SESSION',        N'مدیریت شیفت صندوق'),
-- CRM
(N'CRM', N'CRM.CUSTOMER',       N'مدیریت مشتریان'),
(N'CRM', N'CRM.SUPPLIER',       N'مدیریت تأمین‌کنندگان'),
-- HRM
(N'HRM', N'HRM.EMPLOYEE',       N'مدیریت کارکنان'),
(N'HRM', N'HRM.ATTENDANCE',     N'حضور و غیاب'),
(N'HRM', N'HRM.SALARY',         N'حقوق و دستمزد'),
-- Settings
(N'CFG', N'CFG.COMPANY',        N'تنظیمات شرکت'),
(N'CFG', N'CFG.BRANCH',         N'مدیریت شعب'),
(N'CFG', N'CFG.USER',           N'مدیریت کاربران'),
(N'CFG', N'CFG.BACKUP',         N'پشتیبان‌گیری');

-- Default Chart of Accounts (Iranian Standard)
DECLARE @CompanyId INT = 1; -- Placeholder for first company

-- Level 1 - Account Groups
INSERT INTO Acc.AccountGroups(Code, Name, Nature) VALUES
('1', N'دارایی‌ها',      N'بدهکار'),
('2', N'بدهی‌ها',        N'بستانکار'),
('3', N'سرمایه',         N'بستانکار'),
('4', N'درآمدها',        N'بستانکار'),
('5', N'هزینه‌ها',       N'بدهکار'),
('6', N'حساب‌های انتظامی', N'بدهکار');

-- SMS Templates
INSERT INTO Cfg.SmsTemplates(Code, Name, Template, IsActive) VALUES
(N'BIRTHDAY',       N'تبریک تولد',              N'مشتری گرامی {Name}، سالروز تولدتان مبارک. سما حساب', 1),
(N'INVOICE_SENT',   N'ارسال فاکتور',             N'فاکتور شماره {InvoiceNo} به مبلغ {Amount} ریال ثبت شد.', 1),
(N'CHEQUE_DUE',     N'سررسید چک',               N'چک شماره {ChequeNo} به مبلغ {Amount} ریال فردا سررسید می‌شود.', 1),
(N'PAYMENT_RCV',    N'دریافت پرداخت',            N'مبلغ {Amount} ریال از شما دریافت شد. باقیمانده: {Remain} ریال', 1),
(N'LOW_STOCK',      N'هشدار کمبود موجودی',       N'موجودی کالا {ProductName} به حداقل رسیده است.', 1),
(N'WELCOME',        N'خوش‌آمدگویی مشتری',        N'مشتری گرامی {Name}، به سیستم سما حساب خوش آمدید.', 1);

-- Default App Settings
INSERT INTO Cfg.AppSettings([Key], [Value], [Group], Description) VALUES
(N'Theme',              N'Dark',    N'UI',  N'تم نرم‌افزار'),
(N'Language',           N'fa-IR',   N'UI',  N'زبان'),
(N'CalendarType',       N'Jalali',  N'UI',  N'نوع تقویم'),
(N'CurrencyDisplay',    N'Toman',   N'UI',  N'نمایش ارز'),
(N'PrinterName',        N'',        N'POS', N'نام پرینتر'),
(N'ReceiptWidth',       N'80',      N'POS', N'عرض رسید (mm)'),
(N'AutoBackup',         N'1',       N'SYS', N'پشتیبان خودکار'),
(N'BackupPath',         N'C:\SamaHesabBackup', N'SYS', N'مسیر پشتیبان'),
(N'BackupInterval',     N'1',       N'SYS', N'فاصله پشتیبان (روز)'),
(N'SmsEnabled',         N'0',       N'SMS', N'فعال بودن SMS'),
(N'LoyaltyRate',        N'1',       N'CRM', N'نرخ امتیاز وفاداری (%)');
GO
