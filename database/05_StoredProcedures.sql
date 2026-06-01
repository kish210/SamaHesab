-- =============================================================================
-- SAMA HESAB ERP - Stored Procedures
-- =============================================================================
USE SamaHesab;
GO

-- =============================================
-- SP: Generate Next Document Number
-- =============================================
CREATE OR ALTER PROCEDURE Cfg.sp_GetNextNumber
    @CompanyId  INT,
    @ModuleCode NVARCHAR(20),
    @DocType    NVARCHAR(50),
    @Result     NVARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentNumber INT, @Padding INT, @Prefix NVARCHAR(10), @Suffix NVARCHAR(10);

    BEGIN TRANSACTION;
    BEGIN TRY
        SELECT @CurrentNumber = CurrentNumber, @Padding = Padding,
               @Prefix = ISNULL(Prefix,''), @Suffix = ISNULL(Suffix,'')
        FROM Cfg.NumberingTemplates WITH (UPDLOCK, ROWLOCK)
        WHERE CompanyId = @CompanyId AND ModuleCode = @ModuleCode AND DocType = @DocType;

        IF @CurrentNumber IS NULL
        BEGIN
            INSERT INTO Cfg.NumberingTemplates(CompanyId, ModuleCode, DocType, StartNumber, CurrentNumber, Padding)
            VALUES (@CompanyId, @ModuleCode, @DocType, 1, 1, 6);
            SET @CurrentNumber = 1;
            SET @Padding = 6;
            SET @Prefix = '';
            SET @Suffix = '';
        END
        ELSE
        BEGIN
            SET @CurrentNumber = @CurrentNumber + 1;
            UPDATE Cfg.NumberingTemplates
            SET CurrentNumber = @CurrentNumber
            WHERE CompanyId = @CompanyId AND ModuleCode = @ModuleCode AND DocType = @DocType;
        END

        SET @Result = @Prefix + RIGHT(REPLICATE('0', @Padding) + CAST(@CurrentNumber AS NVARCHAR), @Padding) + @Suffix;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- =============================================
-- SP: Post Voucher (قطعی کردن سند)
-- =============================================
CREATE OR ALTER PROCEDURE Acc.sp_PostVoucher
    @VoucherId      INT,
    @UserId         INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TotalDebit DECIMAL(18,2), @TotalCredit DECIMAL(18,2);

    SELECT @TotalDebit = SUM(Debit), @TotalCredit = SUM(Credit)
    FROM Acc.VoucherItems WHERE VoucherId = @VoucherId;

    IF ABS(@TotalDebit - @TotalCredit) > 0.01
        THROW 50001, N'سند تراز نیست. مجموع بدهکار و بستانکار باید برابر باشد.', 1;

    UPDATE Acc.Vouchers
    SET Status = 2, ApprovedByUserId = @UserId, ApprovedAt = GETDATE(), UpdatedAt = GETDATE()
    WHERE Id = @VoucherId AND Status = 1;

    IF @@ROWCOUNT = 0
        THROW 50002, N'سند قابل قطعی شدن نیست.', 1;
END;
GO

-- =============================================
-- SP: Reverse Voucher (برگشت سند)
-- =============================================
CREATE OR ALTER PROCEDURE Acc.sp_ReverseVoucher
    @VoucherId      INT,
    @UserId         INT,
    @NewVoucherId   INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CompanyId INT, @BranchId INT, @FiscalYearId INT, @VoucherTypeId INT,
            @VoucherDate NVARCHAR(10), @Description NVARCHAR(1000), @CurrencyId INT, @ExchangeRate DECIMAL(18,4);

    SELECT @CompanyId=CompanyId, @BranchId=BranchId, @FiscalYearId=FiscalYearId,
           @VoucherTypeId=VoucherTypeId, @VoucherDate=VoucherDate, @Description=Description,
           @CurrencyId=CurrencyId, @ExchangeRate=ExchangeRate
    FROM Acc.Vouchers WHERE Id = @VoucherId;

    DECLARE @NewNumber NVARCHAR(20);
    EXEC Cfg.sp_GetNextNumber @CompanyId, N'ACC', N'VOUCHER', @NewNumber OUTPUT;

    INSERT INTO Acc.Vouchers(CompanyId, BranchId, FiscalYearId, VoucherNumber, VoucherDate,
        VoucherTypeId, Status, Description, CurrencyId, ExchangeRate, CreatedByUserId, ReversedFromId)
    VALUES(@CompanyId, @BranchId, @FiscalYearId, @NewNumber, @VoucherDate,
        @VoucherTypeId, 1, N'برگشت: ' + ISNULL(@Description,''), @CurrencyId, @ExchangeRate, @UserId, @VoucherId);

    SET @NewVoucherId = SCOPE_IDENTITY();

    INSERT INTO Acc.VoucherItems(VoucherId, RowNumber, AccountId, Description, Debit, Credit, CurrencyId, ExchangeRate)
    SELECT @NewVoucherId, RowNumber, AccountId, Description, Credit, Debit, CurrencyId, ExchangeRate
    FROM Acc.VoucherItems WHERE VoucherId = @VoucherId;

    UPDATE Acc.Vouchers SET IsReversed = 1, UpdatedAt = GETDATE() WHERE Id = @VoucherId;
END;
GO

-- =============================================
-- SP: Process Stock Entry (ورود انبار)
-- =============================================
CREATE OR ALTER PROCEDURE Inv.sp_ProcessStockEntry
    @ProductId      INT,
    @WarehouseId    INT,
    @Quantity       DECIMAL(18,4),
    @UnitCost       DECIMAL(18,4),
    @DocumentType   NVARCHAR(50),
    @DocumentNumber NVARCHAR(20),
    @DocumentDate   NVARCHAR(10),
    @BatchId        INT = NULL,
    @SerialId       INT = NULL,
    @RelatedDocId   INT = NULL,
    @UserId         INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CompanyId INT, @CurrentQty DECIMAL(18,4), @CurrentCost DECIMAL(18,4),
            @NewQty DECIMAL(18,4), @NewAvgCost DECIMAL(18,4);

    SELECT @CompanyId = CompanyId FROM Inv.Warehouses WHERE Id = @WarehouseId;

    -- Get current stock
    SELECT @CurrentQty = ISNULL(Quantity, 0), @CurrentCost = ISNULL(AverageCost, 0)
    FROM Inv.StockItems WHERE ProductId = @ProductId AND WarehouseId = @WarehouseId;

    SET @NewQty = ISNULL(@CurrentQty, 0) + @Quantity;

    -- Weighted Average Cost
    IF ISNULL(@CurrentQty, 0) <= 0
        SET @NewAvgCost = @UnitCost
    ELSE
        SET @NewAvgCost = (ISNULL(@CurrentQty,0) * ISNULL(@CurrentCost,0) + @Quantity * @UnitCost) / @NewQty;

    -- Upsert stock item
    MERGE Inv.StockItems AS target
    USING (SELECT @ProductId AS ProductId, @WarehouseId AS WarehouseId) AS source
        ON target.ProductId = source.ProductId AND target.WarehouseId = source.WarehouseId
    WHEN MATCHED THEN
        UPDATE SET Quantity = @NewQty, AverageCost = @NewAvgCost, LastCost = @UnitCost, LastUpdated = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (ProductId, WarehouseId, Quantity, AverageCost, LastCost, LastUpdated)
        VALUES (@ProductId, @WarehouseId, @NewQty, @NewAvgCost, @UnitCost, GETDATE());

    -- Transaction log
    INSERT INTO Inv.StockTransactions(CompanyId, TransactionType, DocumentNumber, DocumentDate,
        ProductId, WarehouseId, BatchId, SerialId, Quantity, UnitCost, TotalCost,
        BalanceQty, BalanceCost, RelatedDocType, RelatedDocId, CreatedByUserId)
    VALUES(@CompanyId, @DocumentType, @DocumentNumber, @DocumentDate,
        @ProductId, @WarehouseId, @BatchId, @SerialId, @Quantity, @UnitCost, @Quantity * @UnitCost,
        @NewQty, @NewAvgCost, @DocumentType, @RelatedDocId, @UserId);
END;
GO

-- =============================================
-- SP: Process Stock Exit (خروج انبار)
-- =============================================
CREATE OR ALTER PROCEDURE Inv.sp_ProcessStockExit
    @ProductId      INT,
    @WarehouseId    INT,
    @Quantity       DECIMAL(18,4),
    @DocumentType   NVARCHAR(50),
    @DocumentNumber NVARCHAR(20),
    @DocumentDate   NVARCHAR(10),
    @BatchId        INT = NULL,
    @SerialId       INT = NULL,
    @RelatedDocId   INT = NULL,
    @UserId         INT = NULL,
    @UnitCost       DECIMAL(18,4) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CompanyId INT, @CurrentQty DECIMAL(18,4), @CurrentCost DECIMAL(18,4), @NewQty DECIMAL(18,4);

    SELECT @CompanyId = CompanyId FROM Inv.Warehouses WHERE Id = @WarehouseId;

    SELECT @CurrentQty = ISNULL(Quantity, 0), @CurrentCost = ISNULL(AverageCost, 0)
    FROM Inv.StockItems WHERE ProductId = @ProductId AND WarehouseId = @WarehouseId;

    IF ISNULL(@CurrentQty, 0) < @Quantity
        THROW 50003, N'موجودی کافی در انبار وجود ندارد.', 1;

    SET @UnitCost = ISNULL(@CurrentCost, 0);
    SET @NewQty = @CurrentQty - @Quantity;

    UPDATE Inv.StockItems
    SET Quantity = @NewQty, LastUpdated = GETDATE()
    WHERE ProductId = @ProductId AND WarehouseId = @WarehouseId;

    INSERT INTO Inv.StockTransactions(CompanyId, TransactionType, DocumentNumber, DocumentDate,
        ProductId, WarehouseId, BatchId, SerialId, Quantity, UnitCost, TotalCost,
        BalanceQty, BalanceCost, RelatedDocType, RelatedDocId, CreatedByUserId)
    VALUES(@CompanyId, @DocumentType, @DocumentNumber, @DocumentDate,
        @ProductId, @WarehouseId, @BatchId, @SerialId, -@Quantity, @UnitCost, -@Quantity * @UnitCost,
        @NewQty, @CurrentCost, @DocumentType, @RelatedDocId, @UserId);
END;
GO

-- =============================================
-- SP: Finalize Sales Invoice
-- =============================================
CREATE OR ALTER PROCEDURE Sal.sp_FinalizeSalesInvoice
    @InvoiceId  INT,
    @UserId     INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @CompanyId INT, @BranchId INT, @WarehouseId INT, @InvoiceNumber NVARCHAR(20),
                @InvoiceDate NVARCHAR(10), @GrandTotal DECIMAL(18,2), @CustomerId INT;

        SELECT @CompanyId=CompanyId, @BranchId=BranchId, @WarehouseId=WarehouseId,
               @InvoiceNumber=InvoiceNumber, @InvoiceDate=InvoiceDate,
               @GrandTotal=GrandTotal, @CustomerId=CustomerId
        FROM Sal.SalesInvoices WHERE Id = @InvoiceId AND StatusCode = N'پیش‌نویس';

        IF @CompanyId IS NULL
            THROW 50010, N'فاکتور یافت نشد یا قابل قطعی شدن نیست.', 1;

        -- Process stock exit for each item
        DECLARE @ProductId INT, @Quantity DECIMAL(18,4), @BatchId INT, @SerialId INT, @UnitCost DECIMAL(18,4);

        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT ProductId, Quantity, BatchId, SerialId FROM Sal.SalesInvoiceItems WHERE InvoiceId = @InvoiceId;

        OPEN cur;
        FETCH NEXT FROM cur INTO @ProductId, @Quantity, @BatchId, @SerialId;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            EXEC Inv.sp_ProcessStockExit
                @ProductId, @WarehouseId, @Quantity,
                N'فروش', @InvoiceNumber, @InvoiceDate,
                @BatchId, @SerialId, @InvoiceId, @UserId, @UnitCost OUTPUT;

            -- Update cost price on invoice item
            UPDATE Sal.SalesInvoiceItems SET CostPrice = @UnitCost,
                Profit = NetAmount - (Quantity * @UnitCost)
            WHERE InvoiceId = @InvoiceId AND ProductId = @ProductId;

            FETCH NEXT FROM cur INTO @ProductId, @Quantity, @BatchId, @SerialId;
        END;
        CLOSE cur; DEALLOCATE cur;

        -- Create accounting voucher
        DECLARE @VoucherId INT, @VoucherNumber NVARCHAR(20);
        EXEC Cfg.sp_GetNextNumber @CompanyId, N'ACC', N'VOUCHER', @VoucherNumber OUTPUT;

        DECLARE @FiscalYearId INT;
        SELECT TOP 1 @FiscalYearId = Id FROM Cfg.FiscalYears
        WHERE CompanyId = @CompanyId AND IsActive = 1 AND IsClosed = 0;

        DECLARE @VoucherTypeId INT;
        SELECT @VoucherTypeId = Id FROM Acc.VoucherTypes WHERE Code = N'SALE';

        INSERT INTO Acc.Vouchers(CompanyId, BranchId, FiscalYearId, VoucherNumber, VoucherDate,
            VoucherTypeId, Status, Description, TotalDebit, TotalCredit, CreatedByUserId)
        VALUES(@CompanyId, @BranchId, @FiscalYearId, @VoucherNumber, @InvoiceDate,
            @VoucherTypeId, 2, N'فاکتور فروش شماره ' + @InvoiceNumber, @GrandTotal, @GrandTotal, @UserId);

        SET @VoucherId = SCOPE_IDENTITY();

        -- Dr: Accounts Receivable / Cash, Cr: Sales
        DECLARE @CustomerAccountId INT, @SalesAccountId INT;
        SELECT @CustomerAccountId = AccountId FROM Crm.Customers WHERE Id = @CustomerId;
        SELECT @SalesAccountId = CAST([Value] AS INT) FROM Cfg.AppSettings
        WHERE CompanyId = @CompanyId AND [Key] = N'DefaultSalesAccountId';

        INSERT INTO Acc.VoucherItems(VoucherId, RowNumber, AccountId, Description, Debit, Credit)
        VALUES
            (@VoucherId, 1, @CustomerAccountId, N'فروش به مشتری - فاکتور ' + @InvoiceNumber, @GrandTotal, 0),
            (@VoucherId, 2, @SalesAccountId, N'درآمد فروش - فاکتور ' + @InvoiceNumber, 0, @GrandTotal);

        UPDATE Sal.SalesInvoices
        SET StatusCode = N'قطعی', VoucherId = @VoucherId, UpdatedAt = GETDATE()
        WHERE Id = @InvoiceId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- =============================================
-- SP: Get General Ledger (دفتر کل)
-- =============================================
CREATE OR ALTER PROCEDURE Acc.sp_GetGeneralLedger
    @CompanyId      INT,
    @AccountId      INT,
    @FiscalYearId   INT,
    @FromDate       NVARCHAR(10) = NULL,
    @ToDate         NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @OpeningDebit DECIMAL(18,2) = 0, @OpeningCredit DECIMAL(18,2) = 0;

    -- Opening balance before from date
    IF @FromDate IS NOT NULL
    BEGIN
        SELECT @OpeningDebit = ISNULL(SUM(vi.Debit),0), @OpeningCredit = ISNULL(SUM(vi.Credit),0)
        FROM Acc.VoucherItems vi
        JOIN Acc.Vouchers v ON v.Id = vi.VoucherId
        WHERE vi.AccountId = @AccountId
          AND v.CompanyId = @CompanyId
          AND v.FiscalYearId = @FiscalYearId
          AND v.Status >= 2
          AND v.VoucherDate < @FromDate;
    END

    SELECT
        v.VoucherDate AS [تاریخ],
        v.VoucherNumber AS [شماره سند],
        vt.Name AS [نوع سند],
        vi.Description AS [شرح],
        vi.Debit AS [بدهکار],
        vi.Credit AS [بستانکار],
        SUM(vi.Debit - vi.Credit) OVER (
            PARTITION BY vi.AccountId
            ORDER BY v.VoucherDate, v.VoucherNumber, vi.RowNumber
            ROWS UNBOUNDED PRECEDING
        ) + @OpeningDebit - @OpeningCredit AS [مانده],
        @OpeningDebit AS OpeningDebit,
        @OpeningCredit AS OpeningCredit
    FROM Acc.VoucherItems vi
    JOIN Acc.Vouchers v ON v.Id = vi.VoucherId
    JOIN Acc.VoucherTypes vt ON vt.Id = v.VoucherTypeId
    WHERE vi.AccountId = @AccountId
      AND v.CompanyId = @CompanyId
      AND v.FiscalYearId = @FiscalYearId
      AND v.Status >= 2
      AND (@FromDate IS NULL OR v.VoucherDate >= @FromDate)
      AND (@ToDate IS NULL OR v.VoucherDate <= @ToDate)
    ORDER BY v.VoucherDate, v.VoucherNumber, vi.RowNumber;
END;
GO

-- =============================================
-- SP: Balance Sheet (ترازنامه)
-- =============================================
CREATE OR ALTER PROCEDURE Acc.sp_GetBalanceSheet
    @CompanyId    INT,
    @FiscalYearId INT,
    @AsOfDate     NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        a.Code,
        a.Name,
        a.Level,
        a.AccountType,
        SUM(vi.Debit) AS TotalDebit,
        SUM(vi.Credit) AS TotalCredit,
        CASE a.Nature
            WHEN N'بدهکار' THEN SUM(vi.Debit) - SUM(vi.Credit)
            ELSE SUM(vi.Credit) - SUM(vi.Debit)
        END AS Balance,
        a.Nature
    FROM Acc.Accounts a
    JOIN Acc.VoucherItems vi ON vi.AccountId = a.Id
    JOIN Acc.Vouchers v ON v.Id = vi.VoucherId
    WHERE a.CompanyId = @CompanyId
      AND v.FiscalYearId = @FiscalYearId
      AND v.Status >= 2
      AND a.AccountType IN (N'دارایی', N'بدهی', N'سرمایه')
      AND (@AsOfDate IS NULL OR v.VoucherDate <= @AsOfDate)
    GROUP BY a.Code, a.Name, a.Level, a.AccountType, a.Nature
    ORDER BY a.Code;
END;
GO

-- =============================================
-- SP: Profit and Loss (سود و زیان)
-- =============================================
CREATE OR ALTER PROCEDURE Acc.sp_GetProfitLoss
    @CompanyId    INT,
    @FiscalYearId INT,
    @FromDate     NVARCHAR(10) = NULL,
    @ToDate       NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        a.Code,
        a.Name,
        a.AccountType,
        a.Nature,
        SUM(CASE a.Nature WHEN N'بستانکار' THEN vi.Credit - vi.Debit ELSE vi.Debit - vi.Credit END) AS Amount
    FROM Acc.Accounts a
    JOIN Acc.VoucherItems vi ON vi.AccountId = a.Id
    JOIN Acc.Vouchers v ON v.Id = vi.VoucherId
    WHERE a.CompanyId = @CompanyId
      AND v.FiscalYearId = @FiscalYearId
      AND v.Status >= 2
      AND a.AccountType IN (N'درآمد', N'هزینه')
      AND (@FromDate IS NULL OR v.VoucherDate >= @FromDate)
      AND (@ToDate IS NULL OR v.VoucherDate <= @ToDate)
    GROUP BY a.Code, a.Name, a.AccountType, a.Nature
    ORDER BY a.AccountType DESC, a.Code;
END;
GO

-- =============================================
-- SP: Backup Database
-- =============================================
CREATE OR ALTER PROCEDURE Cfg.sp_BackupDatabase
    @BackupPath NVARCHAR(500),
    @UserId     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @FileName NVARCHAR(500), @Sql NVARCHAR(1000);
    DECLARE @DateStr NVARCHAR(20) = REPLACE(REPLACE(REPLACE(CONVERT(NVARCHAR,GETDATE(),120),' ','_'),':','-'),'-','');

    SET @FileName = @BackupPath + N'\SamaHesab_' + @DateStr + N'.bak';
    SET @Sql = N'BACKUP DATABASE SamaHesab TO DISK = ''' + @FileName + N''' WITH COMPRESSION, STATS = 10';

    EXEC sp_executesql @Sql;

    INSERT INTO Cfg.BackupHistory(BackupType, FilePath, Status, CreatedByUserId)
    VALUES(N'دستی', @FileName, N'موفق', @UserId);
END;
GO
