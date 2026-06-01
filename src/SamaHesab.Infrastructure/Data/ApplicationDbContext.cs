using Microsoft.EntityFrameworkCore;
using SamaHesab.Domain.Entities.Accounting;
using SamaHesab.Domain.Entities.Inventory;
using SamaHesab.Domain.Entities.Settings;
using SamaHesab.Domain.Entities.CRM;
using SamaHesab.Domain.Entities.Sales;

namespace SamaHesab.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // Accounting
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<VoucherItem> VoucherItems { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Cheque> Cheques { get; set; }

    // Inventory
    public DbSet<Product> Products { get; set; }
    public DbSet<StockItem> StockItems { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }

    // CRM
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    // Sales
    public DbSet<SalesInvoice> SalesInvoices { get; set; }
    public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Fluent configurations would go here
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Handle domain events, audit, etc.
        return await base.SaveChangesAsync(cancellationToken);
    }
}
