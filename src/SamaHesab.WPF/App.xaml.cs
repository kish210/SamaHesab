using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Windows;
using MediatR;
using SamaHesab.Application.Common.Behaviors;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Infrastructure;
using SamaHesab.Infrastructure.Data;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
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
using SamaHesab.WPF.Views.Shell;

namespace SamaHesab.WPF;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // ─── Serilog ──────────────────────────────────────────────────────────
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("logs/samaHesab-.txt", rollingInterval: RollingInterval.Day,
                          retainedFileCountLimit: 30, fileSizeLimitBytes: 10_000_000)
            .CreateLogger();

        // ─── Host ─────────────────────────────────────────────────────────────
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                cfg.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                   .AddJsonFile("appsettings.json", optional: false)
                   .AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true)
                   .AddEnvironmentVariables();
            })
            .ConfigureServices((ctx, services) =>
            {
                // Infrastructure
                services.AddInfrastructure(ctx.Configuration);

                // MediatR + Pipelines
                services.AddMediatR(cfg => {
                    cfg.RegisterServicesFromAssembly(typeof(Application.Accounting.Commands.CreateVoucherCommand).Assembly);
                    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                });

                // WPF Services
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<ICurrentUserService, CurrentUserService>();

                // ViewModels
                services.AddTransient<MainViewModel>();
                services.AddTransient<LoginViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<VoucherListViewModel>();
                services.AddTransient<VoucherEditViewModel>();
                services.AddTransient<ChartOfAccountsViewModel>();
                services.AddTransient<ChequeListViewModel>();
                services.AddTransient<BankAccountViewModel>();
                services.AddTransient<ProductListViewModel>();
                services.AddTransient<ProductEditViewModel>();
                services.AddTransient<WarehouseViewModel>();
                services.AddTransient<StockAdjustViewModel>();
                services.AddTransient<SalesInvoiceListViewModel>();
                services.AddTransient<SalesInvoiceEditViewModel>();
                services.AddTransient<PurchaseInvoiceEditViewModel>();
                services.AddTransient<PosViewModel>();
                services.AddTransient<CustomerListViewModel>();
                services.AddTransient<CustomerEditViewModel>();
                services.AddTransient<SupplierListViewModel>();
                services.AddTransient<EmployeeListViewModel>();
                services.AddTransient<EmployeeEditViewModel>();
                services.AddTransient<SalaryViewModel>();
                services.AddTransient<AttendanceViewModel>();
                services.AddTransient<ReportsViewModel>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<CompanySettingsViewModel>();
                services.AddTransient<BackupViewModel>();
                services.AddTransient<UserManagementViewModel>();

                // Windows
                services.AddTransient<LoginWindow>();
                services.AddTransient<MainWindow>();
            })
            .Build();

        await _host.StartAsync();

        // ─── Migrate Database ─────────────────────────────────────────────────
        try
        {
            using var scope = _host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "خطا در اجرای migration پایگاه داده");
        }

        // ─── Show Login ───────────────────────────────────────────────────────
        var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    public static T GetService<T>() where T : notnull =>
        Current is App app && app._host != null
            ? app._host.Services.GetRequiredService<T>()
            : throw new InvalidOperationException("Host not initialized.");
}
