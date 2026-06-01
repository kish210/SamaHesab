using System.Linq.Expressions;

namespace SamaHesab.Domain.Interfaces.Repositories;

public interface IProductRepository : IRepository<Entities.Inventory.Product>
{
    Task<Entities.Inventory.Product?> GetByCodeAsync(int companyId, string code, CancellationToken ct = default);
    Task<Entities.Inventory.Product?> GetByBarcodeAsync(int companyId, string barcode, CancellationToken ct = default);
    Task<List<Entities.Inventory.Product>> SearchAsync(int companyId, string searchText, CancellationToken ct = default);
    Task<List<Entities.Inventory.Product>> GetByGroupAsync(int groupId, CancellationToken ct = default);
    Task<List<Entities.Inventory.Product>> GetLowStockAsync(int companyId, CancellationToken ct = default);
}

public interface IWarehouseRepository : IRepository<Entities.Inventory.Warehouse>
{
    Task<List<Entities.Inventory.Warehouse>> GetByCompanyAsync(int companyId, CancellationToken ct = default);
    Task<Entities.Inventory.Warehouse?> GetDefaultAsync(int companyId, CancellationToken ct = default);
}

public interface IStockItemRepository : IRepository<Entities.Inventory.StockItem>
{
    Task<Entities.Inventory.StockItem?> GetByProductAndWarehouseAsync(
        int productId, int warehouseId, CancellationToken ct = default);
    Task<List<Entities.Inventory.StockItem>> GetByProductAsync(int productId, CancellationToken ct = default);
    Task<List<Entities.Inventory.StockItem>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default);
    Task<decimal> GetTotalQuantityAsync(int productId, CancellationToken ct = default);
}
