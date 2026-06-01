using SamaHesab.Domain.Common;

namespace SamaHesab.Domain.Entities.Inventory;

public class StockItem : BaseEntity
{
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal AverageCost { get; private set; }
    public decimal? LastCost { get; private set; }
    public DateTime LastUpdated { get; private set; } = DateTime.Now;

    public Product? Product { get; private set; }
    public Warehouse? Warehouse { get; private set; }

    private StockItem() { }

    public static StockItem Create(int productId, int warehouseId)
    {
        return new StockItem { ProductId = productId, WarehouseId = warehouseId };
    }

    public void AddStock(decimal quantity, decimal unitCost)
    {
        if (quantity <= 0) throw new ArgumentException("مقدار ورودی باید بزرگتر از صفر باشد.");

        var newQty = Quantity + quantity;
        AverageCost = Quantity <= 0
            ? unitCost
            : (Quantity * AverageCost + quantity * unitCost) / newQty;
        Quantity = newQty;
        LastCost = unitCost;
        LastUpdated = DateTime.Now;
    }

    public decimal RemoveStock(decimal quantity)
    {
        if (quantity <= 0) throw new ArgumentException("مقدار خروجی باید بزرگتر از صفر باشد.");
        if (Quantity < quantity)
            throw new InvalidOperationException($"موجودی کافی نیست. موجودی فعلی: {Quantity}");

        var cost = AverageCost;
        Quantity -= quantity;
        LastUpdated = DateTime.Now;
        return cost;
    }

    public void Adjust(decimal newQuantity, decimal? newCost = null)
    {
        Quantity = newQuantity;
        if (newCost.HasValue) AverageCost = newCost.Value;
        LastUpdated = DateTime.Now;
    }
}
