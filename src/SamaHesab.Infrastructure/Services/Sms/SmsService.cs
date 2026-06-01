using Microsoft.Extensions.Logging;
using SamaHesab.Application.Common.Interfaces;

namespace SamaHesab.Infrastructure.Services.Sms;

public class SmsService : ISmsService
{
    private readonly IEnumerable<ISmsProvider> _providers;
    private readonly ILogger<SmsService> _logger;
    private ISmsProvider? _activeProvider;

    public SmsService(IEnumerable<ISmsProvider> providers, ILogger<SmsService> logger)
    {
        _providers = providers;
        _logger = logger;
    }

    public void SetProvider(string providerCode)
    {
        _activeProvider = _providers.FirstOrDefault(p => p.ProviderCode == providerCode)
            ?? throw new InvalidOperationException($"ارائه‌دهنده پیامک '{providerCode}' یافت نشد.");
    }

    private ISmsProvider GetProvider()
    {
        if (_activeProvider != null) return _activeProvider;
        var provider = _providers.FirstOrDefault()
            ?? throw new InvalidOperationException("هیچ ارائه‌دهنده پیامکی تنظیم نشده است.");
        return provider;
    }

    public async Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default)
    {
        try
        {
            var provider = GetProvider();
            var result = await provider.SendAsync(mobile, message, ct);
            if (result)
                _logger.LogInformation("پیامک ارسال شد. شماره: {Mobile}", mobile);
            else
                _logger.LogWarning("ارسال پیامک ناموفق. شماره: {Mobile}", mobile);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال پیامک. شماره: {Mobile}", mobile);
            return false;
        }
    }

    public async Task<bool> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default)
    {
        try
        {
            return await GetProvider().SendBulkAsync(mobiles, message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال پیامک انبوه.");
            return false;
        }
    }

    public async Task<bool> SendTemplateAsync(string mobile, string templateCode,
        Dictionary<string, string> parameters, CancellationToken ct = default)
    {
        // Load template from DB and replace parameters
        var template = await GetTemplateAsync(templateCode);
        if (template == null) return false;

        var message = parameters.Aggregate(template,
            (current, param) => current.Replace($"{{{param.Key}}}", param.Value));

        return await SendAsync(mobile, message, ct);
    }

    public async Task<decimal> GetCreditAsync(CancellationToken ct = default)
    {
        try { return await GetProvider().GetCreditAsync(ct); }
        catch { return 0; }
    }

    private Task<string?> GetTemplateAsync(string code)
    {
        // In real implementation, query from database
        var templates = new Dictionary<string, string>
        {
            ["BIRTHDAY"] = "مشتری گرامی {Name}، سالروز تولدتان مبارک. سما حساب",
            ["INVOICE_SENT"] = "فاکتور شماره {InvoiceNo} به مبلغ {Amount} ریال ثبت شد.",
            ["CHEQUE_DUE"] = "چک شماره {ChequeNo} به مبلغ {Amount} ریال فردا سررسید می‌شود.",
            ["PAYMENT_RCV"] = "مبلغ {Amount} ریال از شما دریافت شد. باقیمانده: {Remain} ریال",
            ["LOW_STOCK"] = "موجودی کالا {ProductName} به حداقل رسیده است.",
        };
        return Task.FromResult(templates.GetValueOrDefault(code));
    }
}

// =============================================
// Kavenegar Provider
// =============================================
public class KavenegarSmsProvider : ISmsProvider
{
    private readonly string _apiKey;
    private readonly string _sender;
    private readonly HttpClient _httpClient;
    private readonly ILogger<KavenegarSmsProvider> _logger;

    public string ProviderCode => "kavenegar";
    public string ProviderName => "کاوه‌نگار";

    public KavenegarSmsProvider(string apiKey, string sender, HttpClient httpClient,
        ILogger<KavenegarSmsProvider> logger)
    {
        _apiKey = apiKey;
        _sender = sender;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default)
    {
        try
        {
            var url = $"https://api.kavenegar.com/v1/{_apiKey}/sms/send.json";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("receptor", mobile),
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("sender", _sender)
            });

            var response = await _httpClient.PostAsync(url, content, ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال پیامک از طریق کاوه‌نگار");
            return false;
        }
    }

    public async Task<bool> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default)
    {
        var tasks = mobiles.Select(m => SendAsync(m, message, ct));
        var results = await Task.WhenAll(tasks);
        return results.All(r => r);
    }

    public async Task<decimal> GetCreditAsync(CancellationToken ct = default)
    {
        try
        {
            var url = $"https://api.kavenegar.com/v1/{_apiKey}/account/info.json";
            var response = await _httpClient.GetAsync(url, ct);
            // Parse response JSON for credit
            return 0;
        }
        catch { return 0; }
    }
}

// =============================================
// FarazSMS Provider
// =============================================
public class FarazSmsSmsProvider : ISmsProvider
{
    private readonly string _username;
    private readonly string _password;
    private readonly string _sender;
    private readonly HttpClient _httpClient;

    public string ProviderCode => "farazsms";
    public string ProviderName => "فراز پیام";

    public FarazSmsSmsProvider(string username, string password, string sender, HttpClient httpClient)
    {
        _username = username;
        _password = password;
        _sender = sender;
        _httpClient = httpClient;
    }

    public async Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default)
    {
        try
        {
            var url = "https://ippanel.com/api/select";
            var payload = new
            {
                op = "send",
                uname = _username,
                pass = _password,
                from = _sender,
                to = new[] { mobile },
                msg = message
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content, ct);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default)
    {
        var tasks = mobiles.Select(m => SendAsync(m, message, ct));
        var results = await Task.WhenAll(tasks);
        return results.All(r => r);
    }

    public Task<decimal> GetCreditAsync(CancellationToken ct = default) => Task.FromResult(0m);
}

// =============================================
// MeliPayamak Provider
// =============================================
public class MeliPayamakSmsProvider : ISmsProvider
{
    private readonly string _username;
    private readonly string _password;
    private readonly string _sender;
    private readonly HttpClient _httpClient;

    public string ProviderCode => "melipayamak";
    public string ProviderName => "ملی پیامک";

    public MeliPayamakSmsProvider(string username, string password, string sender, HttpClient httpClient)
    {
        _username = username;
        _password = password;
        _sender = sender;
        _httpClient = httpClient;
    }

    public async Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default)
    {
        try
        {
            var url = $"https://rest.payamak-panel.com/api/SendSMS/SendSMS";
            var payload = new
            {
                username = _username,
                password = _password,
                to = mobile,
                from = _sender,
                text = message,
                isFlash = false
            };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content, ct);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default)
    {
        var tasks = mobiles.Select(m => SendAsync(m, message, ct));
        var results = await Task.WhenAll(tasks);
        return results.All(r => r);
    }

    public Task<decimal> GetCreditAsync(CancellationToken ct = default) => Task.FromResult(0m);
}
