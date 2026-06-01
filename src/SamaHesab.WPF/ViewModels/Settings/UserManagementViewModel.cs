using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;

namespace SamaHesab.WPF.ViewModels.Settings;

public partial class UserManagementViewModel : BaseViewModel
{
    [ObservableProperty] private int _totalUsers;
    public ObservableCollection<UserItem> Users { get; } = new();

    public UserManagementViewModel(IDialogService d, INavigationService n) : base(d, n) { }

    public override async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            Users.Clear();
            Users.Add(new UserItem(1, "admin", "مدیر سیستم", "مدیر سیستم", true, DateTime.Now));
            TotalUsers = Users.Count;
            await Task.CompletedTask;
        });
    }

    [RelayCommand] private void NewUser() { }
    [RelayCommand] private async Task EditUserAsync() => await _dialogService.ShowInfoAsync("ویرایش کاربر...");
    [RelayCommand] private async Task DeleteUserAsync() => await _dialogService.ConfirmAsync("آیا از حذف کاربر اطمینان دارید؟");
    [RelayCommand] private async Task ResetPasswordAsync() => await _dialogService.ShowInfoAsync("رمز عبور بازنشانی شد.");
}

public record UserItem(int Id, string Username, string FullName, string RoleName, bool IsActive, DateTime? LastLogin);
