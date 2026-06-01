using SamaHesab.Domain.Common;
namespace SamaHesab.Domain.Entities.HRM;
public class Department : AuditableEntity
{
    public int? BranchId { get; private set; }
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public int? ParentId { get; private set; }
    public int? ManagerId { get; private set; }
    public bool IsActive { get; private set; } = true;
    private Department() { }
    public static Department Create(int companyId, string code, string name, int? branchId = null)
        => new() { CompanyId = companyId, Code = code, Name = name, BranchId = branchId };
}
