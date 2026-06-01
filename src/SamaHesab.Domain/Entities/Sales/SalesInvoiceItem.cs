using SamaHesab.Domain.Common;

namespace SamaHesab.Domain.Entities.Sales;

public class SalesInvoiceItem : BaseEntity
{
    public int InvoiceId { get; private set; }
    public int RowNumber { get; private set; }
    public int ProductId { get; private set; }
    public int? BatchId { get; private set; }
    public int? SerialId { get; private set; }
    public string? Description { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountPct { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxPct { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal? CostPrice { get; private set; }
    public decimal? Profit { get; private set; }

    private SalesInvoiceItem() { }

    public static SalesInvoiceItem Create(int invoiceId, int rowNumber, int productId,
        decimal quantity, decimal unitPrice, decimal discountPct = 0, decimal taxPct = 0,
        string? description = null, int? batchId = null, int? serialId = null)
    {
        if (quantity <= 0) throw new ArgumentException("مقدار باید بزرگتر از صفر باشد.");
        if (unitPrice < 0) throw new ArgumentException("قیمت واحد نمی‌تواند منفی باشد.");

        var item = new SalesInvoiceItem
        {
            InvoiceId = invoiceId,
            RowNumber = rowNumber,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            DiscountPct = discountPct,
            TaxPct = taxPct,
            Description = description,
            BatchId = batchId,
            SerialId = serialId
        };
        item.Calculate();
        return item;
    }

    private void Calculate()
    {
        var subtotal = Quantity * UnitPrice;
        DiscountAmount = subtotal * DiscountPct / 100;
        var afterDiscount = subtotal - DiscountAmount;
        TaxAmount = afterDiscount * TaxPct / 100;
        NetAmount = afterDiscount + TaxAmount;
    }

    public void UpdateQuantity(decimal quantity)
    {
        Quantity = quantity;
        Calculate();
    }

    public void UpdatePrice(decimal unitPrice, decimal discountPct, decimal taxPct)
    {
        UnitPrice = unitPrice;
        DiscountPct = discountPct;
        TaxPct = taxPct;
        Calculate();
    }

    public void SetCostAndProfit(decimal costPrice)
    {
        CostPrice = costPrice;
        Profit = NetAmount - (Quantity * costPrice);
    }
}
