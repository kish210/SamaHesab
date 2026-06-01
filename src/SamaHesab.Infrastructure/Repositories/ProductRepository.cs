using Microsoft.EntityFrameworkCore;
using SamaHesab.Domain.Entities.Inventory;
using SamaHesab.Domain.Interfaces.Repositories;
using SamaHesab.Infrastructure.Data;

namespace SamaHesab.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Product?> GetByCodeAsync(int companyId, string code, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(p => p.CompanyId == companyId && p.Code == code, ct);

    public async Task<Product?> GetByBarcodeAsync(int companyId, string barcode, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(
            p => p.CompanyId == companyId && (p.Barcode == barcode || p.Barcode2 == barcode), ct);

    public async Task<IReadOnlyList<Product>> SearchAsync(int companyId, string query, CancellationToken ct = default)
    {
        var q = query.ToLower();
        return await _dbSet
            .Where(p => p.CompanyId == companyId && (
                p.Code.Contains(query) ||
                p.Name.Contains(query) ||
                (p.Barcode != null && p.Barcode.Contains(query)) ||
                (p.NameEn != null && p.NameEn.ToLower().Contains(q))))
            .Take(50)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Product>> GetByGroupAsync(int groupId, CancellationToken ct = default)
        => await _dbSet.Where(p => p.GroupId == groupId).ToListAsync(ct);

    public async Task<IReadOnlyList<Product>> GetLowStockAsync(int companyId, int? warehouseId = null, CancellationToken ct = default)
    {
        var query = _context.StockItems
            .Include(si => si.Product)
            .Where(si => si.Product.CompanyId == companyId && si.Quantity <= si.Product.MinStock);

        if (warehouseId.HasValue)
            query = query.Where(si => si.WarehouseId == warehouseId.Value);

        var stockItems = await query.ToListAsync(ct);
        return stockItems.Select(si => si.Product).Distinct().ToList();
    }
}

public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Warehouse>> GetByCompanyAsync(int companyId, CancellationToken ct = default)
        => await _dbSet.Where(w => w.CompanyId == companyId).ToListAsync(ct);

    public async Task<Warehouse?> GetDefaultAsync(int companyId, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(w => w.CompanyId == companyId && w.IsDefault, ct);
}

public class StockItemRepository : GenericRepository<StockItem>, IStockItemRepository
{
    public StockItemRepository(ApplicationDbContext context) : base(context) { }

    public async Task<StockItem?> GetByProductAndWarehouseAsync(int productId, int warehouseId, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(si => si.ProductId == productId && si.WarehouseId == warehouseId, ct);

    public async Task<IReadOnlyList<StockItem>> GetByProductAsync(int productId, CancellationToken ct = default)
        => await _dbSet.Include(si => si.Warehouse).Where(si => si.ProductId == productId).ToListAsync(ct);

    public async Task<IReadOnlyList<StockItem>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default)
        => await _dbSet.Include(si => si.Product).Where(si => si.WarehouseId == warehouseId).ToListAsync(ct);

    public async Task<decimal> GetTotalQuantityAsync(int productId, CancellationToken ct = default)
        => await _dbSet.Where(si => si.ProductId == productId).SumAsync(si => si.Quantity, ct);
}
