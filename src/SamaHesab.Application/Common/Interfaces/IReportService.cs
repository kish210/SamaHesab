namespace SamaHesab.Application.Common.Interfaces;

public interface IReportService
{
    Task<byte[]> GeneratePdfAsync(string reportName, object data, CancellationToken ct = default);
    Task<byte[]> GenerateExcelAsync(string reportName, object data, CancellationToken ct = default);
    Task<byte[]> GenerateWordAsync(string reportName, object data, CancellationToken ct = default);
    Task PrintAsync(string reportName, object data, string? printerName = null, CancellationToken ct = default);
}

public interface IPersianCalendarService
{
    string ToPersianDate(DateTime date, string format = "yyyy/MM/dd");
    DateTime ToGregorianDate(string persianDate);
    string GetCurrentPersianDate();
    string GetCurrentPersianDateTime();
    string GetPersianMonthName(int month);
    int GetPersianYear(DateTime date);
    int GetPersianMonth(DateTime date);
    int GetPersianDay(DateTime date);
    string FormatCurrency(decimal amount, bool showToman = false);
    string NumberToWords(decimal number);
}
