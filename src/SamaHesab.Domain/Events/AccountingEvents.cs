using SamaHesab.Domain.Common;

namespace SamaHesab.Domain.Events;

public sealed class VoucherPostedEvent : DomainEvent
{
    public int VoucherId { get; }
    public int CompanyId { get; }
    public int UserId { get; }

    public VoucherPostedEvent(int voucherId, int companyId, int userId)
    {
        VoucherId = voucherId;
        CompanyId = companyId;
        UserId = userId;
    }
}

public sealed class SalesInvoicePostedEvent : DomainEvent
{
    public int InvoiceId { get; }
    public int CompanyId { get; }
    public int CustomerId { get; }
    public decimal Amount { get; }
    public int UserId { get; }

    public SalesInvoicePostedEvent(int invoiceId, int companyId, int customerId, decimal amount, int userId)
    {
        InvoiceId = invoiceId;
        CompanyId = companyId;
        CustomerId = customerId;
        Amount = amount;
        UserId = userId;
    }
}

public sealed class StockAlertEvent : DomainEvent
{
    public int ProductId { get; }
    public int WarehouseId { get; }
    public decimal CurrentStock { get; }
    public decimal MinStock { get; }

    public StockAlertEvent(int productId, int warehouseId, decimal currentStock, decimal minStock)
    {
        ProductId = productId;
        WarehouseId = warehouseId;
        CurrentStock = currentStock;
        MinStock = minStock;
    }
}
