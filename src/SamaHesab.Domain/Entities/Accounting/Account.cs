using SamaHesab.Domain.Common;
using SamaHesab.Domain.Enums;

namespace SamaHesab.Domain.Entities.Accounting;

public class Account : AuditableEntity
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public AccountLevel Level { get; private set; }
    public int? ParentId { get; private set; }
    public AccountNature Nature { get; private set; }
    public string? AccountType { get; private set; }
    public bool IsLeaf { get; private set; }
    public bool IsSystem { get; private set; }
    public int? CurrencyId { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Account? Parent { get; private set; }
    public ICollection<Account> Children { get; private set; } = new List<Account>();
    public ICollection<VoucherItem> VoucherItems { get; private set; } = new List<VoucherItem>();

    private Account() { }

    public static Account Create(int companyId, string code, string name, AccountLevel level,
        AccountNature nature, string accountType, int? parentId = null, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("کد حساب الزامی است.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("نام حساب الزامی است.");

        return new Account
        {
            CompanyId = companyId,
            Code = code,
            Name = name,
            Level = level,
            Nature = nature,
            AccountType = accountType,
            ParentId = parentId,
            Description = description,
            IsLeaf = level >= AccountLevel.Subsidiary
        };
    }

    public void Update(string name, string? description, AccountNature nature)
    {
        Name = name;
        Description = description;
        Nature = nature;
        UpdatedAt = DateTime.Now;
    }

    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.Now; }
    public void MakeSystem() => IsSystem = true;
}
