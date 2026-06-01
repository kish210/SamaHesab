using SamaHesab.Domain.Common;

namespace SamaHesab.Domain.Entities.CRM;

public class CustomerContact : BaseEntity
{
    public int CustomerId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Role { get; private set; }
    public string? Mobile { get; private set; }
    public string? Email { get; private set; }
    public bool IsDefault { get; private set; }

    private CustomerContact() { }

    public static CustomerContact Create(int customerId, string name, string? role = null,
        string? mobile = null, string? email = null, bool isDefault = false)
    {
        return new CustomerContact
        {
            CustomerId = customerId,
            Name = name,
            Role = role,
            Mobile = mobile,
            Email = email,
            IsDefault = isDefault
        };
    }
}
