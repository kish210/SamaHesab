using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Domain.Interfaces.Repositories;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;

namespace SamaHesab.WPF.ViewModels.Accounting;

public partial class AccountListViewModel : BaseViewModel
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUser;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private int _totalAccounts;

    public ObservableCollection<AccountItem> Accounts { get; } = new();

    public AccountListViewModel(IAccountRepository accountRepository, ICurrentUserService currentUser,
        IDialogService dialogService, INavigationService navigationService)
        : base(dialogService, navigationService)
    {
        _accountRepository = accountRepository;
        _currentUser = currentUser;
    }

    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var accounts = await _accountRepository.GetByCompanyAsync(_currentUser.CompanyId!.Value);
            Accounts.Clear();
            foreach (var a in accounts)
                Accounts.Add(new AccountItem(a.Id, a.Code, a.Name, (int)a.Level, a.Nature.ToString(), a.IsActive));
            TotalAccounts = Accounts.Count;
        }, "در حال بارگذاری حساب‌ها...");
    }

    [RelayCommand] private async Task SearchAsync() => await LoadAsync();
    [RelayCommand] private void NewAccount() => _navigationService.NavigateTo("AccountEdit");
    [RelayCommand] private async Task DeleteAsync() => await _dialogService.ShowInfoAsync("برای حذف حساب، ابتدا تراکنش‌های آن را بررسی کنید.");
}

public record AccountItem(int Id, string Code, string Name, int Level, string Nature, bool IsActive);
