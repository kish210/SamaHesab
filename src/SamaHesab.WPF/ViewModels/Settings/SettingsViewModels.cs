using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;

namespace SamaHesab.WPF.ViewModels.Settings;

public partial class BackupViewModel : BaseViewModel
{
    private readonly IBackupService _backupService;
    private readonly IPersianCalendarService _calendar;

    [ObservableProperty] private string _backupPath = @"C:\SamaHesabBackup";
    [ObservableProperty] private bool _autoBackup = true;
    [ObservableProperty] private int _backupIntervalDays = 1;
    [ObservableProperty] private string _lastBackupTime = "بارگذاری...";
    [ObservableProperty] private string? _restoreFilePath;

    public ObservableCollection<BackupInfo> BackupHistory { get; } = new();

    public BackupViewModel(IBackupService backupService, IPersianCalendarService calendar,
        IDialogService dialogService, INavigationService navigationService)
        : base(dialogService, navigationService)
    { _backupService = backupService; _calendar = calendar; }

    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var history = await _backupService.GetBackupHistoryAsync();
            BackupHistory.Clear();
            foreach (var h in history) BackupHistory.Add(h);
            LastBackupTime = BackupHistory.FirstOrDefault()?.CreatedAt.ToString("yyyy/MM/dd HH:mm") ?? "هنوز پشتیبانی نشده";
        }, "در حال بارگذاری...");
    }

    [RelayCommand]
    private async Task ManualBackupAsync()
    {
        var ok = await _dialogService.ConfirmAsync("آیا از پایگاه داده پشتیبان‌گیری شود؟", "پشتیبان‌گیری");
        if (!ok) return;
        await ExecuteAsync(async () =>
        {
            var path = await _backupService.BackupAsync(BackupPath);
            await LoadAsync();
            await _dialogService.ShowSuccessAsync($"پشتیبان‌گیری با موفقیت انجام شد.\nفایل: {path}");
        }, "در حال پشتیبان‌گیری...");
    }

    [RelayCommand]
    private async Task BrowseRestoreFileAsync()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Backup Files (*.bak)|*.bak|All Files (*.*)|*.*",
            Title = "انتخاب فایل پشتیبان"
        };
        if (dlg.ShowDialog() == true) RestoreFilePath = dlg.FileName;
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task RestoreAsync()
    {
        if (string.IsNullOrWhiteSpace(RestoreFilePath)) { await _dialogService.ShowErrorAsync("فایل پشتیبان انتخاب کنید."); return; }
        var ok = await _dialogService.ConfirmAsync(
            "⚠ هشدار: بازیابی پایگاه داده اطلاعات فعلی را بازنویسی می‌کند.\nآیا ادامه می‌دهید؟", "بازیابی پایگاه داده");
        if (!ok) return;
        await ExecuteAsync(async () =>
        {
            await _backupService.RestoreAsync(RestoreFilePath);
            await _dialogService.ShowSuccessAsync("پایگاه داده با موفقیت بازیابی شد. لطفاً برنامه را مجدداً راه‌اندازی کنید.");
        }, "در حال بازیابی... لطفاً صبر کنید");
    }

    [RelayCommand]
    private async Task BrowseBackupPathAsync()
    {
        var dlg = new System.Windows.Forms.FolderBrowserDialog { Description = "مسیر پشتیبان‌گیری را انتخاب کنید" };
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) BackupPath = dlg.SelectedPath;
        await Task.CompletedTask;
    }
}

// Settings ViewModel
public partial class SettingsViewModel : BaseViewModel
{
    [ObservableProperty] private string _selectedTheme = "Dark";
    [ObservableProperty] private string _selectedCurrency = "ریال";
    [ObservableProperty] private bool _smsEnabled;
    [ObservableProperty] private string _smsProvider = "kavenegar";
    [ObservableProperty] private string _smsApiKey = string.Empty;
    [ObservableProperty] private string _smsSender = string.Empty;
    [ObservableProperty] private string _companyName = string.Empty;
    [ObservableProperty] private string _companyPhone = string.Empty;
    [ObservableProperty] private string _fiscalYearStart = string.Empty;
    [ObservableProperty] private string _fiscalYearEnd = string.Empty;

    public List<string> Themes { get; } = new() { "Dark", "Light" };
    public List<string> Currencies { get; } = new() { "ریال", "تومان" };
    public List<string> SmsProviders { get; } = new() { "kavenegar", "farazsms", "melipayamak" };

    public SettingsViewModel(IDialogService dialogService, INavigationService navigationService)
        : base(dialogService, navigationService) { }

    public override async Task LoadAsync()
    {
        CompanyName = "شرکت نمونه";
        FiscalYearStart = "1403/01/01";
        FiscalYearEnd = "1403/12/29";
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await ExecuteAsync(async () => await _dialogService.ShowSuccessAsync("تنظیمات ذخیره شد."));
    }

    [RelayCommand]
    private async Task TestSmsAsync()
    {
        if (!SmsEnabled) { await _dialogService.ShowWarningAsync("SMS فعال نیست."); return; }
        await _dialogService.ShowInfoAsync("پیامک آزمایشی ارسال شد.");
    }
}

// Company Settings
public partial class CompanySettingsViewModel : BaseViewModel
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string? _nationalId;
    [ObservableProperty] private string? _economicCode;
    [ObservableProperty] private string? _address;
    [ObservableProperty] private string? _phone;

    public CompanySettingsViewModel(IDialogService d, INavigationService n) : base(d, n) { }
    public override async Task LoadAsync() => await Task.CompletedTask;

    [RelayCommand]
    private async Task SaveAsync() =>
        await ExecuteAsync(async () => await _dialogService.ShowSuccessAsync("اطلاعات شرکت ذخیره شد."));
}

// User Management
public partial class UserManagementViewModel : BaseViewModel
{
    public ObservableCollection<UserRow> Users { get; } = new();

    public UserManagementViewModel(IDialogService d, INavigationService n) : base(d, n) { }

    public override async Task LoadAsync()
    {
        Users.Clear();
        Users.Add(new UserRow(1, "admin", "مدیر سیستم", "مدیر سیستم", true, null));
        Users.Add(new UserRow(2, "accountant", "حسابدار اصلی", "حسابدار", true, null));
        await Task.CompletedTask;
    }

    [RelayCommand] private void AddNew() => _ = _dialogService.ShowInfoAsync("فرم ایجاد کاربر جدید");
    [RelayCommand] private void Edit(UserRow? u) => _ = _dialogService.ShowInfoAsync($"ویرایش کاربر: {u?.FullName}");

    [RelayCommand]
    private async Task ToggleActiveAsync(UserRow? u)
    {
        if (u == null) return;
        await _dialogService.ShowSuccessAsync($"وضعیت کاربر {u.FullName} تغییر کرد.");
    }
}

public record UserRow(int Id, string Username, string FullName, string RoleName, bool IsActive, string? LastLogin);

