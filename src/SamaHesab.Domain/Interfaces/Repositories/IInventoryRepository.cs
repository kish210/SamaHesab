using SamaHesab.Domain.Entities.Inventory;

namespace SamaHesab.Domain.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByCodeAsync(int companyId, string code, CancellationToken ct = default);
    Task<Product?> GetByBarcodeAsync(int companyId, string barcode, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> SearchAsync(int companyId, string query, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetByGroupAsync(int groupId, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetLowStockAsync(int companyId, int? warehouseId = null, CancellationToken ct = default);
}

public interface IWarehouseRepository : IRepository<Warehouse>
{
    Task<IReadOnlyList<Warehouse>> GetByCompanyAsync(int companyId, CancellationToken ct = default);
    Task<Warehouse?> GetDefaultAsync(int companyId, CancellationToken ct = default);
}

public interface IStockItemRepository : IRepository<StockItem>
{
    Task<StockItem?> GetByProductAndWarehouseAsync(int productId, int warehouseId, CancellationToken ct = default);
    Task<IReadOnlyList<StockItem>> GetByProductAsync(int productId, CancellationToken ct = default);
    Task<IReadOnlyList<StockItem>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default);
    Task<decimal> GetTotalQuantityAsync(int productId, CancellationToken ct = default);
}
