namespace SamaHesab.Application.Common.Interfaces;

public interface ISmsService
{
    Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default);
    Task<bool> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default);
    Task<bool> SendTemplateAsync(string mobile, string templateCode, Dictionary<string, string> parameters, CancellationToken ct = default);
    Task<decimal> GetCreditAsync(CancellationToken ct = default);
}

public interface ISmsProvider
{
    string ProviderCode { get; }
    string ProviderName { get; }
    Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default);
    Task<bool> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default);
    Task<decimal> GetCreditAsync(CancellationToken ct = default);
}
