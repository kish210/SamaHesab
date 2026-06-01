using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;

namespace SamaHesab.WPF.ViewModels.HRM;

public partial class EmployeeListViewModel : BaseViewModel
{
    private readonly ICurrentUserService _currentUser;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private int _activeCount;

    public ObservableCollection<EmployeeListItem> Employees { get; } = new();

    public EmployeeListViewModel(ICurrentUserService currentUser, IDialogService d, INavigationService n)
        : base(d, n) { _currentUser = currentUser; }

    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            Employees.Clear();
            TotalCount = 0;
            ActiveCount = 0;
            await Task.CompletedTask;
        });
    }

    [RelayCommand] private async Task SearchAsync() => await LoadAsync();
    [RelayCommand] private void NewEmployee() { }
    [RelayCommand] private async Task PrintAsync() => await _dialogService.ShowInfoAsync("چاپ لیست کارکنان...");
}

public record EmployeeListItem(int Id, string Code, string FullName, string NationalCode, string Mobile,
    string Department, string Position, string HireDate, decimal BaseSalary, bool IsActive);
