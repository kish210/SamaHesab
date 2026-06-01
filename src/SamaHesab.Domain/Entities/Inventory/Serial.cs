using SamaHesab.Domain.Common;
using SamaHesab.Domain.Enums;
namespace SamaHesab.Domain.Entities.Inventory;
public class Serial : BaseEntity
{
    public int ProductId { get; private set; }
    public int? WarehouseId { get; private set; }
    public string SerialNumber { get; private set; } = default!;
    public SerialStatus Status { get; private set; } = SerialStatus.Available;
    public decimal? PurchasePrice { get; private set; }
    public string? PurchaseDate { get; private set; }
    public string? SaleDate { get; private set; }
    public string? Notes { get; private set; }

    private Serial() { }

    public static Serial Create(int productId, string serialNumber, int? warehouseId = null,
        decimal? purchasePrice = null, string? purchaseDate = null)
    {
        return new Serial
        {
            ProductId = productId, SerialNumber = serialNumber,
            WarehouseId = warehouseId, PurchasePrice = purchasePrice,
            PurchaseDate = purchaseDate
        };
    }

    public void Sell(string saleDate) { Status = SerialStatus.Sold; SaleDate = saleDate; }
    public void MarkDefective() => Status = SerialStatus.Defective;
}
