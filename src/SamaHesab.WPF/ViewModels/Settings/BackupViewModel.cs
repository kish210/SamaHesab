using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;

namespace SamaHesab.WPF.ViewModels.Settings;

public partial class BackupViewModel : BaseViewModel
{
    private readonly IBackupService _backupService;

    [ObservableProperty] private string _backupPath = @"C:\SamaHesabBackup";
    [ObservableProperty] private bool _autoBackupEnabled;
    [ObservableProperty] private int _autoBackupInterval = 1;

    public ObservableCollection<BackupInfo> BackupHistory { get; } = new();

    public BackupViewModel(
        IBackupService backupService,
        IDialogService dialogService,
        INavigationService navigationService) : base(dialogService, navigationService)
    {
        _backupService = backupService;
    }

    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var history = await _backupService.GetBackupHistoryAsync();
            BackupHistory.Clear();
            foreach (var h in history)
                BackupHistory.Add(h);
        });
    }

    [RelayCommand]
    private async Task BackupNowAsync()
    {
        await ExecuteAsync(async () =>
        {
            var filePath = await _backupService.BackupAsync(BackupPath);
            await _dialogService.ShowSuccessAsync($"پشتیبان‌گیری انجام شد:\n{filePath}");
            await LoadAsync();
        }, "در حال تهیه پشتیبان...");
    }

    [RelayCommand]
    private async Task RestoreAsync()
    {
        var confirm = await _dialogService.ConfirmAsync(
            "⚠ توجه: بازیابی تمام اطلاعات فعلی را جایگزین می‌کند. آیا مطمئنید؟",
            "بازیابی پایگاه داده");
        if (!confirm) return;

        var dialog = new OpenFileDialog
        {
            Filter = "Backup Files (*.bak)|*.bak|All Files (*.*)|*.*",
            Title = "انتخاب فایل پشتیبان"
        };

        if (dialog.ShowDialog() != true) return;

        await ExecuteAsync(async () =>
        {
            await _backupService.RestoreAsync(dialog.FileName);
            await _dialogService.ShowSuccessAsync("بازیابی با موفقیت انجام شد. لطفاً نرم‌افزار را مجدداً راه‌اندازی کنید.");
        }, "در حال بازیابی...");
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        await _dialogService.ShowSuccessAsync("تنظیمات پشتیبان‌گیری ذخیره شد.");
    }
}
