using SamaHesab.Domain.Entities.Accounting;

namespace SamaHesab.Domain.Interfaces.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<IReadOnlyList<Account>> GetByCompanyAsync(int companyId, CancellationToken ct = default);
    Task<Account?> GetByCodeAsync(int companyId, string code, CancellationToken ct = default);
    Task<IReadOnlyList<Account>> GetChildrenAsync(int parentId, CancellationToken ct = default);
    Task<IReadOnlyList<Account>> GetLeafAccountsAsync(int companyId, CancellationToken ct = default);
    Task<bool> HasTransactionsAsync(int accountId, CancellationToken ct = default);
    Task<decimal> GetBalanceAsync(int accountId, int? fiscalYearId = null, string? toDate = null, CancellationToken ct = default);
}

public interface IVoucherRepository : IRepository<Voucher>
{
    Task<IReadOnlyList<Voucher>> GetByDateRangeAsync(int companyId, int fiscalYearId,
        string fromDate, string toDate, CancellationToken ct = default);
    Task<Voucher?> GetWithItemsAsync(int id, CancellationToken ct = default);
    Task<string> GetNextNumberAsync(int companyId, int fiscalYearId, CancellationToken ct = default);
}

public interface IChequeRepository : IRepository<Cheque>
{
    Task<IReadOnlyList<Cheque>> GetByStatusAsync(int companyId, string status, CancellationToken ct = default);
    Task<IReadOnlyList<Cheque>> GetDueTodayAsync(int companyId, string todayDate, CancellationToken ct = default);
    Task<IReadOnlyList<Cheque>> GetOverdueAsync(int companyId, string todayDate, CancellationToken ct = default);
}
