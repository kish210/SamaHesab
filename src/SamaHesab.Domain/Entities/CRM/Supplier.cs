using SamaHesab.Domain.Common;

namespace SamaHesab.Domain.Entities.CRM;

public class Supplier : AuditableEntity
{
    public string Code { get; private set; } = default!;
    public string SupplierType { get; private set; } = "حقیقی";
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? CompanyName { get; private set; }
    public string? NationalCode { get; private set; }
    public string? EconomicCode { get; private set; }
    public int? AccountId { get; private set; }
    public string? Phone { get; private set; }
    public string? Mobile { get; private set; }
    public string? Email { get; private set; }
    public string? Province { get; private set; }
    public string? City { get; private set; }
    public string? Address { get; private set; }
    public string? PostalCode { get; private set; }
    public decimal CreditLimit { get; private set; }
    public int CreditDays { get; private set; }
    public decimal Balance { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Notes { get; private set; }

    private Supplier() { }

    public static Supplier Create(int companyId, string code, string supplierType,
        string? firstName = null, string? lastName = null, string? companyName = null)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("کد تأمین‌کننده الزامی است.");

        return new Supplier
        {
            CompanyId = companyId,
            Code = code,
            SupplierType = supplierType,
            FirstName = firstName,
            LastName = lastName,
            CompanyName = companyName
        };
    }

    public string FullName => SupplierType == "حقوقی"
        ? CompanyName ?? ""
        : $"{FirstName} {LastName}".Trim();

    public void UpdateContactInfo(string? phone, string? mobile, string? email,
        string? province, string? city, string? address)
    {
        Phone = phone;
        Mobile = mobile;
        Email = email;
        Province = province;
        City = city;
        Address = address;
        UpdatedAt = DateTime.Now;
    }

    public void UpdateCreditTerms(decimal creditLimit, int creditDays)
    {
        CreditLimit = creditLimit;
        CreditDays = creditDays;
        UpdatedAt = DateTime.Now;
    }

    public void UpdateBalance(decimal amount) { Balance = amount; UpdatedAt = DateTime.Now; }
    public void SetAccountId(int accountId) { AccountId = accountId; UpdatedAt = DateTime.Now; }
    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.Now; }
}
