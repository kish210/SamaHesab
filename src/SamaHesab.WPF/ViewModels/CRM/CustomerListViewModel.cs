using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Domain.Interfaces.Repositories;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;

namespace SamaHesab.WPF.ViewModels.CRM;

public partial class CustomerListViewModel : BaseViewModel
{
    private readonly ICurrentUserService _currentUser;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private decimal _totalBalance;

    public ObservableCollection<CustomerListItem> Customers { get; } = new();

    public CustomerListViewModel(ICurrentUserService currentUser, IDialogService d, INavigationService n)
        : base(d, n) { _currentUser = currentUser; }

    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Load from repository
            Customers.Clear();
            TotalCount = 0;
            await Task.CompletedTask;
        }, "در حال بارگذاری مشتریان...");
    }

    [RelayCommand] private async Task SearchAsync() => await LoadAsync();
    [RelayCommand] private void NewCustomer() => _navigationService.NavigateTo("NewCustomer");
    [RelayCommand] private void EditCustomer() { }
    [RelayCommand] private async Task SendSmsAsync() => await _dialogService.ShowInfoAsync("ارسال پیامک...");
    [RelayCommand] private async Task ExportAsync() => await _dialogService.ShowInfoAsync("خروجی اکسل...");
}

public record CustomerListItem(int Id, string Code, string Name, string Mobile, decimal Balance, string PriceLevel, bool IsActive);
