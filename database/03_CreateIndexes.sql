-- =============================================================================
-- SAMA HESAB ERP - Indexes
-- =============================================================================
USE SamaHesab;
GO

-- Cfg Indexes
CREATE INDEX IX_Branches_CompanyId ON Cfg.Branches(CompanyId);
CREATE INDEX IX_FiscalYears_CompanyId ON Cfg.FiscalYears(CompanyId);

-- Sec Indexes
CREATE INDEX IX_Users_CompanyId ON Sec.Users(CompanyId);
CREATE INDEX IX_Users_Username ON Sec.Users(Username);
CREATE INDEX IX_AuditLogs_UserId ON Sec.AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_CreatedAt ON Sec.AuditLogs(CreatedAt DESC);
CREATE INDEX IX_AuditLogs_TableName ON Sec.AuditLogs(TableName, RecordId);

-- Acc Indexes
CREATE INDEX IX_Accounts_CompanyId ON Acc.Accounts(CompanyId);
CREATE INDEX IX_Accounts_ParentId ON Acc.Accounts(ParentId);
CREATE INDEX IX_Accounts_Code ON Acc.Accounts(CompanyId, Code);
CREATE INDEX IX_Vouchers_CompanyId ON Acc.Vouchers(CompanyId, FiscalYearId);
CREATE INDEX IX_Vouchers_Date ON Acc.Vouchers(VoucherDate DESC);
CREATE INDEX IX_Vouchers_Status ON Acc.Vouchers(Status);
CREATE INDEX IX_VoucherItems_VoucherId ON Acc.VoucherItems(VoucherId);
CREATE INDEX IX_VoucherItems_AccountId ON Acc.VoucherItems(AccountId);
CREATE INDEX IX_Cheques_CompanyId ON Acc.Cheques(CompanyId);
CREATE INDEX IX_Cheques_DueDate ON Acc.Cheques(DueDate);
CREATE INDEX IX_Cheques_Status ON Acc.Cheques(Status);

-- Inv Indexes
CREATE INDEX IX_Products_CompanyId ON Inv.Products(CompanyId);
CREATE INDEX IX_Products_Code ON Inv.Products(CompanyId, Code);
CREATE INDEX IX_Products_Barcode ON Inv.Products(Barcode) WHERE Barcode IS NOT NULL;
CREATE INDEX IX_Products_GroupId ON Inv.Products(GroupId);
CREATE INDEX IX_StockItems_ProductId ON Inv.StockItems(ProductId);
CREATE INDEX IX_StockItems_WarehouseId ON Inv.StockItems(WarehouseId);
CREATE INDEX IX_StockTransactions_ProductId ON Inv.StockTransactions(ProductId);
CREATE INDEX IX_StockTransactions_WarehouseId ON Inv.StockTransactions(WarehouseId);
CREATE INDEX IX_StockTransactions_Date ON Inv.StockTransactions(DocumentDate DESC);
CREATE INDEX IX_StockTransactions_RelatedDoc ON Inv.StockTransactions(RelatedDocType, RelatedDocId);
CREATE INDEX IX_Serials_ProductId ON Inv.Serials(ProductId);
CREATE INDEX IX_Serials_Status ON Inv.Serials(Status);

-- Crm Indexes
CREATE INDEX IX_Customers_CompanyId ON Crm.Customers(CompanyId);
CREATE INDEX IX_Customers_Code ON Crm.Customers(CompanyId, Code);
CREATE INDEX IX_Customers_Mobile ON Crm.Customers(Mobile) WHERE Mobile IS NOT NULL;
CREATE INDEX IX_Customers_NationalCode ON Crm.Customers(NationalCode) WHERE NationalCode IS NOT NULL;
CREATE INDEX IX_Suppliers_CompanyId ON Crm.Suppliers(CompanyId);

-- Sal Indexes
CREATE INDEX IX_SalesInvoices_CompanyId ON Sal.SalesInvoices(CompanyId, FiscalYearId);
CREATE INDEX IX_SalesInvoices_CustomerId ON Sal.SalesInvoices(CustomerId);
CREATE INDEX IX_SalesInvoices_Date ON Sal.SalesInvoices(InvoiceDate DESC);
CREATE INDEX IX_SalesInvoices_Status ON Sal.SalesInvoices(StatusCode);
CREATE INDEX IX_SalesInvoiceItems_InvoiceId ON Sal.SalesInvoiceItems(InvoiceId);
CREATE INDEX IX_SalesInvoiceItems_ProductId ON Sal.SalesInvoiceItems(ProductId);

-- Pur Indexes
CREATE INDEX IX_PurchaseInvoices_CompanyId ON Pur.PurchaseInvoices(CompanyId, FiscalYearId);
CREATE INDEX IX_PurchaseInvoices_SupplierId ON Pur.PurchaseInvoices(SupplierId);
CREATE INDEX IX_PurchaseInvoices_Date ON Pur.PurchaseInvoices(InvoiceDate DESC);

-- Hrm Indexes
CREATE INDEX IX_Employees_CompanyId ON Hrm.Employees(CompanyId);
CREATE INDEX IX_Employees_NationalCode ON Hrm.Employees(CompanyId, NationalCode);
CREATE INDEX IX_AttendanceRecords_EmployeeId ON Hrm.AttendanceRecords(EmployeeId);
CREATE INDEX IX_AttendanceRecords_WorkDate ON Hrm.AttendanceRecords(WorkDate DESC);
CREATE INDEX IX_SalarySlips_EmployeeId ON Hrm.SalarySlips(EmployeeId);
GO
