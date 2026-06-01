using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace SamaHesab.WPF.ViewModels.Shell;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private int _selectedCompanyId;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private string _loginButtonText = "ورود به سیستم";
    [ObservableProperty] private bool _isNotLoading = true;

    public ObservableCollection<CompanyItem> Companies { get; } = new();

    private readonly IDialogService _dialogService;

    public LoginViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        LoadCompanies();
    }

    private void LoadCompanies()
    {
        Companies.Clear();
        // In production load from DB
        Companies.Add(new CompanyItem(1, "شرکت اول", "DEFAULT"));
        SelectedCompanyId = 1;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username))
        { HasError = true; ErrorMessage = "نام کاربری را وارد کنید."; return; }
        if (string.IsNullOrWhiteSpace(Password))
        { HasError = true; ErrorMessage = "رمز عبور را وارد کنید."; return; }

        IsLoading = true; IsNotLoading = false; LoginButtonText = "در حال ورود..."; HasError = false;

        await Task.Delay(600); // Simulate auth

        try
        {
            // Verify credentials (simplified – check against DB in production)
            bool isValid = (Username == "admin" && Password == "admin123")
                        || (Username == "admin" && Password == "1234");

            if (!isValid)
            {
                HasError = true; ErrorMessage = "نام کاربری یا رمز عبور اشتباه است.";
                return;
            }

            // Set current user
            var roles = new List<string> { "ADMIN" };
            var perms = new List<string>();
            CurrentUserService.SetUser(new CurrentUser(1, SelectedCompanyId, 1, Username,
                Username == "admin" ? "مدیر سیستم" : Username, roles, perms));

            // Open Main Window
            var mainWindow = App.GetService<Views.Shell.MainWindow>();
            mainWindow.Show();

            // Close login
            foreach (Window w in Application.Current.Windows)
                if (w is Views.Shell.LoginWindow) { w.Close(); break; }
        }
        finally
        {
            IsLoading = false; IsNotLoading = true; LoginButtonText = "ورود به سیستم";
        }
    }
}

public record CompanyItem(int Id, string Name, string Code);
