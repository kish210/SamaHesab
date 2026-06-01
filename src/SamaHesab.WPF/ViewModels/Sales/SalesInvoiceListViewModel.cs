using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;

namespace SamaHesab.WPF.ViewModels.Sales;

public partial class SalesInvoiceListViewModel : BaseViewModel
{
    private readonly ICurrentUserService _currentUser;
    private readonly IPersianCalendarService _calendar;

    [ObservableProperty] private string _fromDate = string.Empty;
    [ObservableProperty] private string _toDate = string.Empty;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private string _selectedStatus = "همه";
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private decimal _totalAmount;

    public ObservableCollection<SalesInvoiceListItem> Invoices { get; } = new();
    public List<string> StatusList { get; } = new() { "همه", "پیش‌نویس", "قطعی", "لغو شده" };

    public SalesInvoiceListViewModel(ICurrentUserService currentUser, IPersianCalendarService calendar,
        IDialogService dialogService, INavigationService navigationService)
        : base(dialogService, navigationService)
    {
        _currentUser = currentUser;
        _calendar = calendar;
    }

    public override async Task LoadAsync()
    {
        var persianCal = new System.Globalization.PersianCalendar();
        var now = DateTime.Now;
        FromDate = $"{persianCal.GetYear(now)}/{persianCal.GetMonth(now):D2}/01";
        ToDate = _calendar.GetCurrentPersianDate();
        await SearchAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Load from repository
            Invoices.Clear();
            TotalCount = 0;
            TotalAmount = 0;
            await Task.CompletedTask;
        });
    }

    [RelayCommand] private void NewInvoice() => _navigationService.NavigateTo("NewSalesInvoice");
    [RelayCommand] private async Task PrintAsync() => await _dialogService.ShowInfoAsync("در حال چاپ...");
    [RelayCommand] private async Task ExportExcelAsync() => await _dialogService.ShowInfoAsync("در حال آماده‌سازی اکسل...");
}

public record SalesInvoiceListItem(int Id, string Number, string Date, string CustomerName,
    decimal Total, decimal Paid, decimal Remain, string Status);
