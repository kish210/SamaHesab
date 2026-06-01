using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;

namespace SamaHesab.WPF.ViewModels.Settings;

public partial class CompanySettingsViewModel : BaseViewModel
{
    [ObservableProperty] private string _companyName = string.Empty;
    [ObservableProperty] private string _companyCode = string.Empty;
    [ObservableProperty] private string? _nationalId;
    [ObservableProperty] private string? _economicCode;
    [ObservableProperty] private string? _registerNumber;
    [ObservableProperty] private string? _address;
    [ObservableProperty] private string? _phone;
    [ObservableProperty] private string? _fax;
    [ObservableProperty] private string? _email;
    [ObservableProperty] private string? _website;
    [ObservableProperty] private string _fiscalYearStart = "1403/01/01";
    [ObservableProperty] private string _fiscalYearEnd = "1403/12/29";
    [ObservableProperty] private string _currency = "ریال";

    public CompanySettingsViewModel(IDialogService d, INavigationService n) : base(d, n) { }

    public override async Task LoadAsync()
    {
        // Load from DB
        CompanyName = "شرکت نمونه";
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await ExecuteAsync(async () =>
        {
            await _dialogService.ShowSuccessAsync("تنظیمات شرکت ذخیره شد.");
            await Task.CompletedTask;
        });
    }

    [RelayCommand]
    private void UploadLogo()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
        };
        dialog.ShowDialog();
    }
}
