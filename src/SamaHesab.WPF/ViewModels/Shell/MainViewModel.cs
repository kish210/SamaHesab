using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Dashboard;
using SamaHesab.WPF.ViewModels.Accounting;
using SamaHesab.WPF.ViewModels.Inventory;
using SamaHesab.WPF.ViewModels.Sales;
using SamaHesab.WPF.ViewModels.Purchase;
using SamaHesab.WPF.ViewModels.POS;
using SamaHesab.WPF.ViewModels.CRM;
using SamaHesab.WPF.ViewModels.HRM;
using SamaHesab.WPF.ViewModels.Reports;
using SamaHesab.WPF.ViewModels.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Threading;

namespace SamaHesab.WPF.ViewModels.Shell;

public partial class MainViewModel : BaseViewModel
{
    private readonly IServiceProvider _services;
    private readonly IPersianCalendarService _calendar;
    private readonly ICurrentUserService _currentUser;
    private readonly DispatcherTimer _clockTimer;

    [ObservableProperty] private BaseViewModel? _currentPage;
    [ObservableProperty] private string _activeMenu = "Dashboard";
    [ObservableProperty] private string _currentPageTitle = "داشبورد";
    [ObservableProperty] private string _currentUserName = string.Empty;
    [ObservableProperty] private string _currentUserRole = string.Empty;
    [ObservableProperty] private string _companyName = "سما حساب";
    [ObservableProperty] private string _todayPersianDate = string.Empty;
    [ObservableProperty] private string _statusMessage = "آماده";
    [ObservableProperty] private bool _isDarkTheme = true;

    private readonly Dictionary<string, (string Title, Func<BaseViewModel> Factory)> _pages;

    public MainViewModel(
        IServiceProvider services,
        IPersianCalendarService calendar,
        ICurrentUserService currentUser,
        IDialogService dialogService,
        INavigationService navigationService)
        : base(dialogService, navigationService)
    {
        _services = services;
        _calendar = calendar;
        _currentUser = currentUser;

        _pages = new Dictionary<string, (string, Func<BaseViewModel>)>
        {
            ["Dashboard"]       = ("داشبورد",            () => _services.GetRequiredService<DashboardViewModel>()),
            ["Vouchers"]        = ("اسناد حسابداری",      () => _services.GetRequiredService<VoucherListViewModel>()),
            ["VoucherEdit"]     = ("ثبت سند",             () => _services.GetRequiredService<VoucherEditViewModel>()),
            ["ChartOfAccounts"] = ("نمودار حساب‌ها",      () => _services.GetRequiredService<ChartOfAccountsViewModel>()),
            ["Cheques"]         = ("مدیریت چک",           () => _services.GetRequiredService<ChequeListViewModel>()),
            ["BankAccounts"]    = ("حساب‌های بانکی",      () => _services.GetRequiredService<BankAccountViewModel>()),
            ["Products"]        = ("مدیریت کالا",         () => _services.GetRequiredService<ProductListViewModel>()),
            ["ProductEdit"]     = ("ویرایش کالا",         () => _services.GetRequiredService<ProductEditViewModel>()),
            ["Warehouses"]      = ("انبارها",              () => _services.GetRequiredService<WarehouseViewModel>()),
            ["StockAdjust"]     = ("تعدیل موجودی",        () => _services.GetRequiredService<StockAdjustViewModel>()),
            ["SalesInvoice"]    = ("فاکتور فروش",         () => _services.GetRequiredService<SalesInvoiceEditViewModel>()),
            ["SalesInvoiceList"]= ("لیست فروش",           () => _services.GetRequiredService<SalesInvoiceListViewModel>()),
            ["PurchaseInvoice"] = ("فاکتور خرید",         () => _services.GetRequiredService<PurchaseInvoiceEditViewModel>()),
            ["POS"]             = ("صندوق فروش",          () => _services.GetRequiredService<PosViewModel>()),
            ["Customers"]       = ("مشتریان",             () => _services.GetRequiredService<CustomerListViewModel>()),
            ["CustomerEdit"]    = ("ویرایش مشتری",        () => _services.GetRequiredService<CustomerEditViewModel>()),
            ["Suppliers"]       = ("تأمین‌کنندگان",       () => _services.GetRequiredService<SupplierListViewModel>()),
            ["Employees"]       = ("کارکنان",             () => _services.GetRequiredService<EmployeeListViewModel>()),
            ["EmployeeEdit"]    = ("پرونده کارمند",       () => _services.GetRequiredService<EmployeeEditViewModel>()),
            ["Salary"]          = ("حقوق و دستمزد",       () => _services.GetRequiredService<SalaryViewModel>()),
            ["Attendance"]      = ("حضور و غیاب",         () => _services.GetRequiredService<AttendanceViewModel>()),
            ["Reports"]         = ("گزارش‌ها",            () => _services.GetRequiredService<ReportsViewModel>()),
            ["Settings"]        = ("تنظیمات",             () => _services.GetRequiredService<SettingsViewModel>()),
            ["Backup"]          = ("پشتیبان‌گیری",         () => _services.GetRequiredService<BackupViewModel>()),
        };

        // Clock timer
        _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
        _clockTimer.Tick += (_, _) => TodayPersianDate = _calendar.GetCurrentPersianDate();
        _clockTimer.Start();

        // Navigation service
        navigationService.Navigated += OnNavigationRequested;
    }

    public override async Task LoadAsync()
    {
        CurrentUserName = _currentUser.FullName ?? "کاربر";
        CurrentUserRole = string.Join(", ", _currentUser.GetRoles());
        TodayPersianDate = _calendar.GetCurrentPersianDate();
        await NavigateToAsync("Dashboard");
    }

    private void OnNavigationRequested(object? sender, NavigationEventArgs e) =>
        System.Windows.Application.Current.Dispatcher.InvokeAsync(() => NavigateToAsync(e.ViewName));

    [RelayCommand]
    private async Task NavigateAsync(string page) => await NavigateToAsync(page);

    private async Task NavigateToAsync(string page)
    {
        if (!_pages.TryGetValue(page, out var entry)) return;
        ActiveMenu = page;
        CurrentPageTitle = entry.Title;
        var vm = entry.Factory();
        CurrentPage = vm;
        await vm.LoadAsync();
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
        var themePath = IsDarkTheme
            ? "Assets/Themes/Dark.xaml"
            : "Assets/Themes/Light.xaml";

        var app = System.Windows.Application.Current;
        var existing = app.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme") == true
                              || d.Source?.OriginalString.Contains("Dark") == true
                              || d.Source?.OriginalString.Contains("Light") == true);

        if (existing != null) app.Resources.MergedDictionaries.Remove(existing);

        app.Resources.MergedDictionaries.Add(new System.Windows.ResourceDictionary
        {
            Source = new Uri(themePath, UriKind.Relative)
        });
    }

    [RelayCommand] private async Task UserProfileAsync() => await _dialogService.ShowInfoAsync($"کاربر: {CurrentUserName}");

    [RelayCommand]
    private void Logout()
    {
        var result = System.Windows.MessageBox.Show(
            "آیا می‌خواهید از سیستم خارج شوید؟", "خروج",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);
        if (result != System.Windows.MessageBoxResult.Yes) return;

        _clockTimer.Stop();
        System.Windows.Application.Current.Shutdown();
    }
}
