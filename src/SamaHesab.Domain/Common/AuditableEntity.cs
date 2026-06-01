namespace SamaHesab.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public int CompanyId { get; protected set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; protected set; }
    public int? CreatedByUserId { get; protected set; }
    public int? UpdatedByUserId { get; protected set; }

    public void SetAudit(int? userId)
    {
        UpdatedAt = DateTime.Now;
        UpdatedByUserId = userId;
    }
}
