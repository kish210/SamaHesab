using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Domain.Interfaces.Repositories;
using SamaHesab.Infrastructure.Data;
using SamaHesab.Infrastructure.Repositories;
using SamaHesab.Infrastructure.Services.Backup;
using SamaHesab.Infrastructure.Services.Persian;
using SamaHesab.Infrastructure.Services.Reporting;
using SamaHesab.Infrastructure.Services.Sms;

namespace SamaHesab.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
                sqlOptions.UseNodaTime();
            }));

        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IVoucherRepository, VoucherRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IChequeRepository, ChequeRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IStockItemRepository, StockItemRepository>();

        // Services
        services.AddScoped<IPersianCalendarService, PersianCalendarService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IBackupService, BackupService>();

        // SMS
        var smsProvider = configuration["Sms:Provider"] ?? "null";
        services.AddScoped<ISmsProvider>(sp =>
            smsProvider switch
            {
                "kavenegar" => new KavenegarProvider(configuration["Sms:ApiKey"] ?? ""),
                "farazsms" => new FarazSmsProvider(
                    configuration["Sms:Username"] ?? "",
                    configuration["Sms:Password"] ?? ""),
                "melipayamak" => new MeliPayamakProvider(
                    configuration["Sms:Username"] ?? "",
                    configuration["Sms:Password"] ?? ""),
                _ => new NullSmsProvider()
            });
        services.AddScoped<ISmsService, SmsService>();

        return services;
    }
}
