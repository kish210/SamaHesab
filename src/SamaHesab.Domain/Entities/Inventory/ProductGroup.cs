using SamaHesab.Domain.Common;
namespace SamaHesab.Domain.Entities.Inventory;
public class ProductGroup : AuditableEntity
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public int? ParentId { get; private set; }
    public int? AccountId { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ProductGroup? Parent { get; private set; }
    public ICollection<ProductGroup> Children { get; private set; } = new List<ProductGroup>();

    private ProductGroup() { }

    public static ProductGroup Create(int companyId, string code, string name, int? parentId = null)
    {
        return new ProductGroup { CompanyId = companyId, Code = code, Name = name, ParentId = parentId };
    }
}
