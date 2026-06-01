using System.Windows;
using SamaHesab.WPF.ViewModels.Shell;

namespace SamaHesab.WPF.Services;

// ─── Dialog Service ───────────────────────────────────────────────────────────
public interface IDialogService
{
    Task ShowInfoAsync(string message, string title = "اطلاعات");
    Task ShowSuccessAsync(string message, string title = "موفق");
    Task ShowErrorAsync(string message, string title = "خطا");
    Task ShowWarningAsync(string message, string title = "هشدار");
    Task<bool> ConfirmAsync(string message, string title = "تأیید");
    Task<string?> PromptAsync(string message, string title = "ورودی", string defaultValue = "");
}

public class DialogService : IDialogService
{
    public Task ShowInfoAsync(string message, string title = "اطلاعات")
    { MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information); return Task.CompletedTask; }

    public Task ShowSuccessAsync(string message, string title = "موفق")
    { MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information); return Task.CompletedTask; }

    public Task ShowErrorAsync(string message, string title = "خطا")
    { MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error); return Task.CompletedTask; }

    public Task ShowWarningAsync(string message, string title = "هشدار")
    { MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning); return Task.CompletedTask; }

    public Task<bool> ConfirmAsync(string message, string title = "تأیید")
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return Task.FromResult(result == MessageBoxResult.Yes);
    }

    public Task<string?> PromptAsync(string message, string title = "ورودی", string defaultValue = "")
    { return Task.FromResult<string?>(defaultValue); }
}

// ─── Navigation Service ───────────────────────────────────────────────────────
public interface INavigationService
{
    event Action<string> NavigationRequested;
    void NavigateTo(string page);
}

public class NavigationService : INavigationService
{
    public event Action<string> NavigationRequested = default!;
    public void NavigateTo(string page) => NavigationRequested?.Invoke(page);
}

// ─── Current User Service ─────────────────────────────────────────────────────
public class CurrentUserService : SamaHesab.Application.Common.Interfaces.ICurrentUserService
{
    private static CurrentUser? _user;

    public static void SetUser(CurrentUser user) => _user = user;
    public static void ClearUser() => _user = null;

    public int? UserId     => _user?.Id;
    public int? CompanyId  => _user?.CompanyId;
    public int? BranchId   => _user?.BranchId;
    public string? Username  => _user?.Username;
    public string? FullName  => _user?.FullName;
    public bool IsAuthenticated => _user != null;

    public bool HasPermission(string moduleCode, string featureCode, string action)
    {
        if (_user == null) return false;
        if (_user.Roles.Contains("ADMIN")) return true;
        return _user.Permissions.Contains($"{moduleCode}.{featureCode}.{action}");
    }

    public IEnumerable<string> GetRoles() => _user?.Roles ?? Enumerable.Empty<string>();
}

public record CurrentUser(int Id, int CompanyId, int? BranchId, string Username, string FullName,
    List<string> Roles, List<string> Permissions);
