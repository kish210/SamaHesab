using SamaHesab.Domain.Common;

namespace SamaHesab.Domain.Entities.Settings;

public class Company : BaseEntity
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? NameEn { get; private set; }
    public string? NationalId { get; private set; }
    public string? EconomicCode { get; private set; }
    public string? RegisterNumber { get; private set; }
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public string? Fax { get; private set; }
    public string? Email { get; private set; }
    public string? Website { get; private set; }
    public byte[]? Logo { get; private set; }
    public string FiscalYearStart { get; private set; } = "1403/01/01";
    public string FiscalYearEnd { get; private set; } = "1403/12/29";
    public string Currency { get; private set; } = "ریال";
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; private set; }

    private Company() { }

    public static Company Create(string code, string name, string fiscalYearStart, string fiscalYearEnd)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("کد شرکت الزامی است.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("نام شرکت الزامی است.");

        return new Company
        {
            Code = code,
            Name = name,
            FiscalYearStart = fiscalYearStart,
            FiscalYearEnd = fiscalYearEnd
        };
    }

    public void Update(string name, string? nameEn, string? nationalId, string? economicCode,
        string? registerNumber, string? address, string? phone, string? fax, string? email, string? website)
    {
        Name = name;
        NameEn = nameEn;
        NationalId = nationalId;
        EconomicCode = economicCode;
        RegisterNumber = registerNumber;
        Address = address;
        Phone = phone;
        Fax = fax;
        Email = email;
        Website = website;
        UpdatedAt = DateTime.Now;
    }

    public void SetLogo(byte[] logo) { Logo = logo; UpdatedAt = DateTime.Now; }
    public void SetFiscalYear(string start, string end) { FiscalYearStart = start; FiscalYearEnd = end; UpdatedAt = DateTime.Now; }
    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.Now; }
}
