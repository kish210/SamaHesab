using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Domain.Interfaces.Repositories;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;

namespace SamaHesab.WPF.ViewModels.Accounting;

public partial class ChequeListViewModel : BaseViewModel
{
    private readonly IChequeRepository _chequeRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IPersianCalendarService _calendar;

    [ObservableProperty] private string _selectedChequeType = "همه";
    [ObservableProperty] private string _selectedStatus = "همه";
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private decimal _totalAmount;

    public ObservableCollection<ChequeItem> Cheques { get; } = new();
    public List<string> ChequeTypes { get; } = new() { "همه", "دریافتی", "پرداختی" };
    public List<string> StatusList { get; } = new() { "همه", "در جریان", "وصول شده", "برگشت خورده", "واگذار شده", "ابطال شده" };

    public ChequeListViewModel(IChequeRepository chequeRepository, ICurrentUserService currentUser,
        IPersianCalendarService calendar, IDialogService dialogService, INavigationService navigationService)
        : base(dialogService, navigationService)
    {
        _chequeRepository = chequeRepository;
        _currentUser = currentUser;
        _calendar = calendar;
    }

    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var today = _calendar.GetCurrentPersianDate();
            var cheques = await _chequeRepository.GetByStatusAsync(_currentUser.CompanyId!.Value, SelectedStatus == "همه" ? "" : SelectedStatus);
            Cheques.Clear();
            foreach (var c in cheques)
                Cheques.Add(new ChequeItem(c.Id, c.ChequeType.ToString(), c.ChequeNumber, c.BankName, c.Amount, c.DueDate, c.Status.ToString(), c.IssuedBy ?? ""));
            TotalCount = Cheques.Count;
            TotalAmount = Cheques.Sum(c => c.Amount);
        }, "در حال بارگذاری چک‌ها...");
    }

    [RelayCommand] private async Task SearchAsync() => await LoadAsync();
    [RelayCommand] private void NewCheque() { }
    [RelayCommand] private async Task ClearChequeAsync() => await _dialogService.ShowInfoAsync("وصول چک ثبت شد.");
    [RelayCommand] private async Task ReturnChequeAsync() => await _dialogService.ShowInfoAsync("برگشت چک ثبت شد.");
}

public record ChequeItem(int Id, string Type, string Number, string Bank, decimal Amount, string DueDate, string Status, string IssuedBy);
