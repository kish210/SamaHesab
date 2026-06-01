using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SamaHesab.Application.Common.Interfaces;

namespace SamaHesab.Infrastructure.Services.Sms;

// ─── Provider Abstraction ─────────────────────────────────────────────────────
public class SmsService : ISmsService
{
    private readonly ISmsProvider _provider;
    private readonly ILogger<SmsService> _logger;

    public SmsService(ISmsProvider provider, ILogger<SmsService> logger)
    { _provider = provider; _logger = logger; }

    public async Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default)
    {
        try { return await _provider.SendAsync(mobile, message, ct); }
        catch (Exception ex) { _logger.LogError(ex, "خطا در ارسال SMS به {Mobile}", mobile); return false; }
    }

    public async Task<bool> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default)
    {
        try { return await _provider.SendBulkAsync(mobiles, message, ct); }
        catch (Exception ex) { _logger.LogError(ex, "خطا در ارسال انبوه SMS"); return false; }
    }

    public async Task<bool> SendTemplateAsync(string mobile, string templateCode,
        Dictionary<string, string> parameters, CancellationToken ct = default)
    {
        var message = BuildFromTemplate(templateCode, parameters);
        return await SendAsync(mobile, message, ct);
    }

    public async Task<decimal> GetCreditAsync(CancellationToken ct = default) =>
        await _provider.GetCreditAsync(ct);

    private static string BuildFromTemplate(string templateCode, Dictionary<string, string> parameters)
    {
        // Templates loaded from DB in real implementation
        var templates = new Dictionary<string, string>
        {
            ["BIRTHDAY"] = "مشتری گرامی {Name}، سالروز تولدتان مبارک. سما حساب",
            ["INVOICE_SENT"] = "فاکتور شماره {InvoiceNo} به مبلغ {Amount} ریال ثبت شد.",
            ["CHEQUE_DUE"] = "چک شماره {ChequeNo} به مبلغ {Amount} ریال فردا سررسید می‌شود.",
            ["PAYMENT_RCV"] = "مبلغ {Amount} ریال دریافت شد. باقیمانده: {Remain} ریال",
            ["LOW_STOCK"] = "موجودی کالا {ProductName} به حداقل رسیده است.",
        };

        if (!templates.TryGetValue(templateCode, out var template)) return string.Empty;
        foreach (var (key, val) in parameters)
            template = template.Replace("{" + key + "}", val);
        return template;
    }
}

// ─── Kavenegar Provider ───────────────────────────────────────────────────────
public class KavenegarProvider : ISmsProvider
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _sender;

    public string ProviderCode => "kavenegar";
    public string ProviderName => "کاوه‌نگار";

    public KavenegarProvider(HttpClient http, string apiKey, string sender)
    { _http = http; _apiKey = apiKey; _sender = sender; }

    public async Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default)
    {
        var url = $"https://api.kavenegar.com/v1/{_apiKey}/sms/send.json";
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["receptor"] = mobile,
            ["message"]  = message,
            ["sender"]   = _sender
        });
        var response = await _http.PostAsync(url, form, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default)
    {
        var url = $"https://api.kavenegar.com/v1/{_apiKey}/sms/sendarray.json";
        var receptors = string.Join(",", mobiles);
        var messages = string.Join(",", mobiles.Select(_ => message));
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["receptor"] = receptors,
            ["message"]  = messages,
            ["sender"]   = _sender
        });
        var response = await _http.PostAsync(url, form, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<decimal> GetCreditAsync(CancellationToken ct = default)
    {
        var url = $"https://api.kavenegar.com/v1/{_apiKey}/account/info.json";
        var response = await _http.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode) return 0;
        // Parse response JSON for remaincredit
        return 0;
    }
}

// ─── FarazSMS Provider ────────────────────────────────────────────────────────
public class FarazSmsProvider : ISmsProvider
{
    private readonly HttpClient _http;
    private readonly string _username;
    private readonly string _password;
    private readonly string _sender;

    public string ProviderCode => "farazsms";
    public string ProviderName => "فراز اس‌ام‌اس";

    public FarazSmsProvider(HttpClient http, string username, string password, string sender)
    { _http = http; _username = username; _password = password; _sender = sender; }

    public async Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default)
    {
        var url = "https://ippanel.com/api/select";
        var payload = new
        {
            op = "send", uname = _username, pass = _password,
            from = _sender, message, to = new[] { mobile }
        };
        var response = await _http.PostAsJsonAsync(url, payload, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default)
    {
        var url = "https://ippanel.com/api/select";
        var payload = new
        {
            op = "send", uname = _username, pass = _password,
            from = _sender, message, to = mobiles.ToArray()
        };
        var response = await _http.PostAsJsonAsync(url, payload, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<decimal> GetCreditAsync(CancellationToken ct = default)
    {
        await Task.CompletedTask; return 0;
    }
}

// ─── MeliPayamak Provider ─────────────────────────────────────────────────────
public class MeliPayamakProvider : ISmsProvider
{
    private readonly HttpClient _http;
    private readonly string _username;
    private readonly string _password;
    private readonly string _from;

    public string ProviderCode => "melipayamak";
    public string ProviderName => "ملی پیامک";

    public MeliPayamakProvider(HttpClient http, string username, string password, string from)
    { _http = http; _username = username; _password = password; _from = from; }

    public async Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default)
    {
        var url = "https://rest.payamak-panel.com/api/SendSMS/SendSMS";
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["username"] = _username, ["password"] = _password,
            ["to"] = mobile, ["from"] = _from,
            ["text"] = message, ["isFlash"] = "false"
        });
        var response = await _http.PostAsync(url, form, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default)
    {
        foreach (var mobile in mobiles)
            await SendAsync(mobile, message, ct);
        return true;
    }

    public async Task<decimal> GetCreditAsync(CancellationToken ct = default)
    {
        await Task.CompletedTask; return 0;
    }
}

// ─── Null / Mock Provider (for when SMS is disabled) ─────────────────────────
public class NullSmsProvider : ISmsProvider
{
    public string ProviderCode => "null";
    public string ProviderName => "غیرفعال";
    public Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default) => Task.FromResult(false);
    public Task<bool> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default) => Task.FromResult(false);
    public Task<decimal> GetCreditAsync(CancellationToken ct = default) => Task.FromResult(0m);
}
