-- =============================================================================
-- SAMA HESAB ERP - Views
-- =============================================================================
USE SamaHesab;
GO

-- =============================================
-- View: Account Balance (مانده حساب)
-- =============================================
CREATE OR ALTER VIEW Acc.vw_AccountBalance AS
SELECT
    a.Id AS AccountId,
    a.CompanyId,
    a.Code,
    a.Name,
    a.Level,
    a.Nature,
    a.ParentId,
    COALESCE(SUM(vi.Debit), 0)  AS TotalDebit,
    COALESCE(SUM(vi.Credit), 0) AS TotalCredit,
    CASE a.Nature
        WHEN N'بدهکار' THEN COALESCE(SUM(vi.Debit), 0) - COALESCE(SUM(vi.Credit), 0)
        ELSE COALESCE(SUM(vi.Credit), 0) - COALESCE(SUM(vi.Debit), 0)
    END AS Balance
FROM Acc.Accounts a
LEFT JOIN Acc.VoucherItems vi ON vi.AccountId = a.Id
LEFT JOIN Acc.Vouchers v ON v.Id = vi.VoucherId AND v.Status >= 2
GROUP BY a.Id, a.CompanyId, a.Code, a.Name, a.Level, a.Nature, a.ParentId;
GO

-- =============================================
-- View: Trial Balance (تراز آزمایشی)
-- =============================================
CREATE OR ALTER VIEW Acc.vw_TrialBalance AS
SELECT
    a.CompanyId,
    a.Code,
    a.Name,
    a.Level,
    a.Nature,
    COALESCE(SUM(vi.Debit), 0)  AS TotalDebit,
    COALESCE(SUM(vi.Credit), 0) AS TotalCredit,
    CASE WHEN COALESCE(SUM(vi.Debit), 0) >= COALESCE(SUM(vi.Credit), 0)
         THEN COALESCE(SUM(vi.Debit), 0) - COALESCE(SUM(vi.Credit), 0)
         ELSE 0 END AS DebitBalance,
    CASE WHEN COALESCE(SUM(vi.Credit), 0) > COALESCE(SUM(vi.Debit), 0)
         THEN COALESCE(SUM(vi.Credit), 0) - COALESCE(SUM(vi.Debit), 0)
         ELSE 0 END AS CreditBalance
FROM Acc.Accounts a
LEFT JOIN Acc.VoucherItems vi ON vi.AccountId = a.Id
LEFT JOIN Acc.Vouchers v ON v.Id = vi.VoucherId AND v.Status >= 2
GROUP BY a.CompanyId, a.Code, a.Name, a.Level, a.Nature;
GO

-- =============================================
-- View: Customer Balance (مانده مشتریان)
-- =============================================
CREATE OR ALTER VIEW Crm.vw_CustomerBalance AS
SELECT
    c.Id AS CustomerId,
    c.CompanyId,
    c.Code,
    ISNULL(c.FirstName + N' ' + c.LastName, c.CompanyName) AS FullName,
    c.Mobile,
    c.CreditLimit,
    COALESCE(si.TotalSales, 0) AS TotalSales,
    COALESCE(sp.TotalPaid, 0) AS TotalPaid,
    COALESCE(si.TotalSales, 0) - COALESCE(sp.TotalPaid, 0) AS Balance
FROM Crm.Customers c
LEFT JOIN (
    SELECT CustomerId, SUM(GrandTotal) AS TotalSales
    FROM Sal.SalesInvoices
    WHERE InvoiceType = N'فروش' AND StatusCode = N'قطعی'
    GROUP BY CustomerId
) si ON si.CustomerId = c.Id
LEFT JOIN (
    SELECT si2.CustomerId, SUM(sp2.Amount) AS TotalPaid
    FROM Sal.SalesPayments sp2
    JOIN Sal.SalesInvoices si2 ON si2.Id = sp2.InvoiceId
    GROUP BY si2.CustomerId
) sp ON sp.CustomerId = c.Id;
GO

-- =============================================
-- View: Supplier Balance (مانده تأمین‌کنندگان)
-- =============================================
CREATE OR ALTER VIEW Crm.vw_SupplierBalance AS
SELECT
    s.Id AS SupplierId,
    s.CompanyId,
    s.Code,
    ISNULL(s.FirstName + N' ' + s.LastName, s.CompanyName) AS FullName,
    s.Mobile,
    COALESCE(pi.TotalPurchases, 0) AS TotalPurchases,
    COALESCE(pp.TotalPaid, 0) AS TotalPaid,
    COALESCE(pi.TotalPurchases, 0) - COALESCE(pp.TotalPaid, 0) AS Balance
FROM Crm.Suppliers s
LEFT JOIN (
    SELECT SupplierId, SUM(GrandTotal) AS TotalPurchases
    FROM Pur.PurchaseInvoices
    WHERE InvoiceType = N'خرید' AND StatusCode = N'قطعی'
    GROUP BY SupplierId
) pi ON pi.SupplierId = s.Id
LEFT JOIN (
    SELECT pi2.SupplierId, SUM(pp2.Amount) AS TotalPaid
    FROM Pur.PurchasePayments pp2
    JOIN Pur.PurchaseInvoices pi2 ON pi2.Id = pp2.InvoiceId
    GROUP BY pi2.SupplierId
) pp ON pp.SupplierId = s.Id;
GO

-- =============================================
-- View: Stock Summary (موجودی انبار)
-- =============================================
CREATE OR ALTER VIEW Inv.vw_StockSummary AS
SELECT
    p.Id AS ProductId,
    p.CompanyId,
    p.Code AS ProductCode,
    p.Barcode,
    p.Name AS ProductName,
    pg.Name AS GroupName,
    u.Name AS UnitName,
    w.Id AS WarehouseId,
    w.Name AS WarehouseName,
    COALESCE(si.Quantity, 0) AS Quantity,
    COALESCE(si.AverageCost, 0) AS AverageCost,
    COALESCE(si.Quantity, 0) * COALESCE(si.AverageCost, 0) AS TotalValue,
    p.MinStock,
    p.MaxStock,
    p.SalePrice,
    p.PurchasePrice,
    CASE
        WHEN COALESCE(si.Quantity, 0) <= 0 THEN N'اتمام موجودی'
        WHEN COALESCE(si.Quantity, 0) <= p.MinStock THEN N'کمتر از حداقل'
        ELSE N'موجود'
    END AS StockStatus
FROM Inv.Products p
CROSS JOIN Inv.Warehouses w
LEFT JOIN Inv.StockItems si ON si.ProductId = p.Id AND si.WarehouseId = w.Id
LEFT JOIN Inv.ProductGroups pg ON pg.Id = p.GroupId
LEFT JOIN Cfg.Units u ON u.Id = p.UnitId
WHERE p.IsActive = 1 AND w.IsActive = 1 AND p.CompanyId = w.CompanyId;
GO

-- =============================================
-- View: Cheques Due (چک‌های سررسید)
-- =============================================
CREATE OR ALTER VIEW Acc.vw_ChequesDue AS
SELECT
    ch.Id,
    ch.CompanyId,
    ch.ChequeType,
    ch.ChequeNumber,
    ch.BankName,
    ch.Amount,
    ch.DueDate,
    ch.IssuedBy,
    ch.Status,
    CASE
        WHEN ch.DueDate < FORMAT(GETDATE(), 'yyyy/MM/dd') AND ch.Status = N'در جریان' THEN N'سررسید گذشته'
        WHEN ch.DueDate = FORMAT(GETDATE(), 'yyyy/MM/dd') AND ch.Status = N'در جریان' THEN N'سررسید امروز'
        ELSE ch.Status
    END AS DisplayStatus
FROM Acc.Cheques ch;
GO

-- =============================================
-- View: Sales Summary by Product
-- =============================================
CREATE OR ALTER VIEW Sal.vw_SalesByProduct AS
SELECT
    p.Id AS ProductId,
    p.CompanyId,
    p.Code AS ProductCode,
    p.Name AS ProductName,
    SUM(sii.Quantity) AS TotalQtySold,
    SUM(sii.NetAmount) AS TotalSalesAmount,
    SUM(sii.Quantity * sii.CostPrice) AS TotalCost,
    SUM(sii.NetAmount) - SUM(sii.Quantity * ISNULL(sii.CostPrice, 0)) AS GrossProfit,
    COUNT(DISTINCT si.Id) AS InvoiceCount
FROM Sal.SalesInvoiceItems sii
JOIN Sal.SalesInvoices si ON si.Id = sii.InvoiceId AND si.StatusCode = N'قطعی' AND si.InvoiceType = N'فروش'
JOIN Inv.Products p ON p.Id = sii.ProductId
GROUP BY p.Id, p.CompanyId, p.Code, p.Name;
GO

-- =============================================
-- View: Employee Summary
-- =============================================
CREATE OR ALTER VIEW Hrm.vw_EmployeeSummary AS
SELECT
    e.Id AS EmployeeId,
    e.CompanyId,
    e.Code,
    e.FirstName + N' ' + e.LastName AS FullName,
    e.NationalCode,
    e.Mobile,
    d.Name AS DepartmentName,
    pos.Name AS PositionName,
    e.HireDate,
    e.ContractType,
    e.BaseSalary,
    e.IsActive,
    COALESCE(ss.LastNetSalary, 0) AS LastNetSalary
FROM Hrm.Employees e
LEFT JOIN Hrm.Departments d ON d.Id = e.DepartmentId
LEFT JOIN Hrm.Positions pos ON pos.Id = e.PositionId
LEFT JOIN (
    SELECT EmployeeId, NetSalary AS LastNetSalary,
           ROW_NUMBER() OVER (PARTITION BY EmployeeId ORDER BY PeriodYear DESC, PeriodMonth DESC) AS rn
    FROM Hrm.SalarySlips
) ss ON ss.EmployeeId = e.Id AND ss.rn = 1;
GO
