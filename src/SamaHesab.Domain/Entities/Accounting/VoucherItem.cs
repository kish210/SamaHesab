using SamaHesab.Domain.Common;

namespace SamaHesab.Domain.Entities.Accounting;

public class VoucherItem : BaseEntity
{
    public int VoucherId { get; private set; }
    public int RowNumber { get; private set; }
    public int AccountId { get; private set; }
    public string? Description { get; private set; }
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
    public int? CurrencyId { get; private set; }
    public decimal? ForeignDebit { get; private set; }
    public decimal? ForeignCredit { get; private set; }
    public decimal ExchangeRate { get; private set; } = 1;
    public int? CostCenterId { get; private set; }
    public int? ProjectId { get; private set; }
    public int? CheckId { get; private set; }
    public string? Reference { get; private set; }

    public Account? Account { get; private set; }
    public Voucher? Voucher { get; private set; }

    private VoucherItem() { }

    public static VoucherItem Create(int voucherId, int rowNumber, int accountId,
        decimal debit, decimal credit, string? description = null,
        int? currencyId = null, decimal exchangeRate = 1,
        int? costCenterId = null, int? projectId = null)
    {
        if (debit < 0 || credit < 0)
            throw new ArgumentException("مبلغ بدهکار و بستانکار نمی‌تواند منفی باشد.");
        if (debit > 0 && credit > 0)
            throw new ArgumentException("یک ردیف نمی‌تواند هم بدهکار و هم بستانکار باشد.");

        return new VoucherItem
        {
            VoucherId = voucherId,
            RowNumber = rowNumber,
            AccountId = accountId,
            Debit = debit,
            Credit = credit,
            Description = description,
            CurrencyId = currencyId,
            ExchangeRate = exchangeRate,
            CostCenterId = costCenterId,
            ProjectId = projectId
        };
    }

    public void Update(int accountId, decimal debit, decimal credit, string? description,
        int? costCenterId = null, int? projectId = null)
    {
        if (debit > 0 && credit > 0)
            throw new ArgumentException("یک ردیف نمی‌تواند هم بدهکار و هم بستانکار باشد.");
        AccountId = accountId;
        Debit = debit;
        Credit = credit;
        Description = description;
        CostCenterId = costCenterId;
        ProjectId = projectId;
    }
}
