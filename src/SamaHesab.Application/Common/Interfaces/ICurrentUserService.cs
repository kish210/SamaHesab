namespace SamaHesab.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    int? CompanyId { get; }
    int? BranchId { get; }
    string? Username { get; }
    string? FullName { get; }
    bool IsAuthenticated { get; }
    bool HasPermission(string moduleCode, string featureCode, string action);
    IEnumerable<string> GetRoles();
}
