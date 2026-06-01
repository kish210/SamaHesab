using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.WPF.Services;

namespace SamaHesab.WPF.ViewModels.Shell;

public partial class MainShellViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;
    private readonly ICurrentUserService _currentUser;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private string _currentPageTitle = "داشبورد";

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _companyName = "سما حساب";

    [ObservableProperty]
    private string _currentDate = string.Empty;

    [ObservableProperty]
    private string _themeIcon = "☀";

    [ObservableProperty]
    private bool _isSidebarExpanded = true;

    [ObservableProperty]
    private string _activeMenu = "Dashboard";

    [ObservableProperty]
    private int _pendingCheques;

    [ObservableProperty]
    private int _lowStockCount;

    private readonly Dictionary<string, (string Title, Func<object> Factory)> _views;

    public MainShellViewModel(
        INavigationService navigationService,
        IThemeService themeService,
        ICurrentUserService currentUser,
        IDialogService dialogService,
        IPersianCalendarService persianCalendar)
    {
        _navigationService = navigationService;
        _themeService = themeService;
        _currentUser = currentUser;
        _dialogService = dialogService;

        UserName = currentUser.FullName ?? "کاربر";
        CurrentDate = persianCalendar.GetCurrentPersianDate();

        _navigationService.Navigated += OnNavigated;

        _views = new Dictionary<string, (string, Func<object>)>
        {
            ["Dashboard"]           = ("داشبورد",                  () => App.GetService<Views.Dashboard.DashboardView>()),
            ["Vouchers"]            = ("اسناد حسابداری",           () => App.GetService<Views.Accounting.VoucherListView>()),
            ["Accounts"]            = ("دفتر حساب‌ها",             () => App.GetService<Views.Accounting.AccountListView>()),
            ["Cheques"]             = ("مدیریت چک",                 () => App.GetService<Views.Accounting.ChequeListView>()),
            ["Products"]            = ("مدیریت کالا",              () => App.GetService<Views.Inventory.ProductListView>()),
            ["SalesInvoices"]       = ("فاکتور فروش",              () => App.GetService<Views.Sales.SalesInvoiceListView>()),
            ["NewSalesInvoice"]     = ("فاکتور فروش جدید",         () => App.GetService<Views.Sales.SalesInvoiceEditView>()),
            ["PurchaseInvoices"]    = ("فاکتور خرید",              () => App.GetService<Views.Purchase.PurchaseInvoiceListView>()),
            ["POS"]                 = ("صندوق فروش",               () => App.GetService<Views.POS.PosView>()),
            ["Customers"]           = ("مدیریت مشتریان",           () => App.GetService<Views.CRM.CustomerListView>()),
            ["Suppliers"]           = ("مدیریت تأمین‌کنندگان",     () => App.GetService<Views.CRM.CustomerListView>()),
            ["Employees"]           = ("مدیریت کارکنان",           () => App.GetService<Views.HRM.EmployeeListView>()),
            ["CompanySettings"]     = ("تنظیمات شرکت",             () => App.GetService<Views.Settings.CompanySettingsView>()),
            ["UserManagement"]      = ("مدیریت کاربران",           () => App.GetService<Views.Settings.UserManagementView>()),
            ["Backup"]              = ("پشتیبان‌گیری",              () => App.GetService<Views.Settings.BackupView>()),
        };

        // Navigate to dashboard on startup
        NavigateTo("Dashboard");
    }

    private void OnNavigated(object? sender, NavigationEventArgs e)
    {
        if (_views.TryGetValue(e.ViewName, out var viewDef))
        {
            CurrentPageTitle = viewDef.Title;
            ActiveMenu = e.ViewName;
            CurrentView = viewDef.Factory();
        }
    }

    [RelayCommand]
    private void NavigateTo(string viewName)
    {
        _navigationService.NavigateTo(viewName);
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        _themeService.ToggleTheme();
        ThemeIcon = _themeService.CurrentTheme == "Dark" ? "☀" : "🌙";
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsSidebarExpanded = !IsSidebarExpanded;
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await _dialogService.ConfirmAsync("آیا می‌خواهید از سیستم خارج شوید؟", "خروج");
        if (!confirm) return;

        App.GetService<ICurrentUserService>();
        ((CurrentUserService)_currentUser).Clear();

        var loginWindow = App.GetService<Views.Shell.LoginWindow>();
        loginWindow.Show();

        foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
        {
            if (w is Views.Shell.MainShellWindow) { w.Close(); break; }
        }
    }

    [RelayCommand]
    private void OpenNewSalesInvoice() => NavigateTo("NewSalesInvoice");

    [RelayCommand]
    private void OpenPOS() => NavigateTo("POS");
}
