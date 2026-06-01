using SamaHesab.Domain.Common;

namespace SamaHesab.Domain.Entities.Inventory;

public class ProductPriceLevel : BaseEntity
{
    public int ProductId { get; private set; }
    public string LevelCode { get; private set; } = default!;
    public decimal Price { get; private set; }
    public decimal? MinQty { get; private set; }
    public string? ValidFrom { get; private set; }
    public string? ValidTo { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ProductPriceLevel() { }

    public static ProductPriceLevel Create(int productId, string levelCode, decimal price,
        decimal? minQty = null, string? validFrom = null, string? validTo = null)
    {
        return new ProductPriceLevel
        {
            ProductId = productId,
            LevelCode = levelCode,
            Price = price,
            MinQty = minQty,
            ValidFrom = validFrom,
            ValidTo = validTo
        };
    }
}
