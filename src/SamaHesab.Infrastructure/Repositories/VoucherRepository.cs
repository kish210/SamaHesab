using Microsoft.EntityFrameworkCore;
using SamaHesab.Domain.Entities.Accounting;
using SamaHesab.Domain.Interfaces.Repositories;
using SamaHesab.Infrastructure.Data;

namespace SamaHesab.Infrastructure.Repositories;

public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
{
    public VoucherRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Voucher>> GetByDateRangeAsync(int companyId, int fiscalYearId,
        string fromDate, string toDate, CancellationToken ct = default)
        => await _dbSet
            .Where(v => v.CompanyId == companyId
                && v.FiscalYearId == fiscalYearId
                && string.Compare(v.VoucherDate, fromDate) >= 0
                && string.Compare(v.VoucherDate, toDate) <= 0)
            .OrderByDescending(v => v.VoucherDate)
            .ToListAsync(ct);

    public async Task<Voucher?> GetWithItemsAsync(int id, CancellationToken ct = default)
        => await _dbSet
            .Include(v => v.Items)
                .ThenInclude(i => i.Account)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<string> GetNextNumberAsync(int companyId, int fiscalYearId, CancellationToken ct = default)
    {
        var count = await _dbSet.CountAsync(v => v.CompanyId == companyId && v.FiscalYearId == fiscalYearId, ct);
        return $"{(count + 1):D8}";
    }
}

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Account>> GetByCompanyAsync(int companyId, CancellationToken ct = default)
        => await _dbSet.Where(a => a.CompanyId == companyId).OrderBy(a => a.Code).ToListAsync(ct);

    public async Task<Account?> GetByCodeAsync(int companyId, string code, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(a => a.CompanyId == companyId && a.Code == code, ct);

    public async Task<IReadOnlyList<Account>> GetChildrenAsync(int parentId, CancellationToken ct = default)
        => await _dbSet.Where(a => a.ParentId == parentId).OrderBy(a => a.Code).ToListAsync(ct);

    public async Task<IReadOnlyList<Account>> GetLeafAccountsAsync(int companyId, CancellationToken ct = default)
        => await _dbSet.Where(a => a.CompanyId == companyId && a.IsLeaf && a.IsActive)
            .OrderBy(a => a.Code).ToListAsync(ct);

    public async Task<bool> HasTransactionsAsync(int accountId, CancellationToken ct = default)
        => await _context.VoucherItems.AnyAsync(vi => vi.AccountId == accountId, ct);

    public async Task<decimal> GetBalanceAsync(int accountId, int? fiscalYearId = null,
        string? toDate = null, CancellationToken ct = default)
    {
        var query = _context.VoucherItems
            .Include(vi => vi.Voucher)
            .Where(vi => vi.AccountId == accountId && vi.Voucher!.Status >= Domain.Enums.VoucherStatus.Posted);

        if (fiscalYearId.HasValue)
            query = query.Where(vi => vi.Voucher!.FiscalYearId == fiscalYearId.Value);

        if (!string.IsNullOrEmpty(toDate))
            query = query.Where(vi => string.Compare(vi.Voucher!.VoucherDate, toDate) <= 0);

        var debit = await query.SumAsync(vi => vi.Debit, ct);
        var credit = await query.SumAsync(vi => vi.Credit, ct);

        var account = await GetByIdAsync(accountId, ct);
        return account?.Nature == Domain.Enums.AccountNature.Debit
            ? debit - credit
            : credit - debit;
    }
}

public class ChequeRepository : GenericRepository<Cheque>, IChequeRepository
{
    public ChequeRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Cheque>> GetByStatusAsync(int companyId, string status, CancellationToken ct = default)
        => await _dbSet.Where(c => c.CompanyId == companyId).ToListAsync(ct);

    public async Task<IReadOnlyList<Cheque>> GetDueTodayAsync(int companyId, string todayDate, CancellationToken ct = default)
        => await _dbSet.Where(c => c.CompanyId == companyId && c.DueDate == todayDate).ToListAsync(ct);

    public async Task<IReadOnlyList<Cheque>> GetOverdueAsync(int companyId, string todayDate, CancellationToken ct = default)
        => await _dbSet.Where(c => c.CompanyId == companyId
            && string.Compare(c.DueDate, todayDate) < 0).ToListAsync(ct);
}
