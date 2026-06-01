using System.Globalization;
using SamaHesab.Application.Common.Interfaces;

namespace SamaHesab.Infrastructure.Services.Persian;

public class PersianCalendarService : IPersianCalendarService
{
    private static readonly PersianCalendar _pc = new();
    private static readonly string[] _monthNames =
        ["فروردین","اردیبهشت","خرداد","تیر","مرداد","شهریور","مهر","آبان","آذر","دی","بهمن","اسفند"];

    public string ToPersianDate(DateTime date, string format = "yyyy/MM/dd")
    {
        int y = _pc.GetYear(date), m = _pc.GetMonth(date), d = _pc.GetDayOfMonth(date);
        return format.Replace("yyyy", y.ToString("D4"))
                     .Replace("MM", m.ToString("D2"))
                     .Replace("dd", d.ToString("D2"));
    }

    public DateTime ToGregorianDate(string persianDate)
    {
        var parts = persianDate.Split('/');
        if (parts.Length != 3) throw new FormatException("فرمت تاریخ نادرست است. yyyy/MM/dd استفاده کنید.");
        int y = int.Parse(parts[0]), m = int.Parse(parts[1]), d = int.Parse(parts[2]);
        return _pc.ToDateTime(y, m, d, 0, 0, 0, 0);
    }

    public string GetCurrentPersianDate() => ToPersianDate(DateTime.Now);
    public string GetCurrentPersianDateTime()
    {
        var now = DateTime.Now;
        return $"{ToPersianDate(now)} {now:HH:mm:ss}";
    }

    public string GetPersianMonthName(int month) =>
        month >= 1 && month <= 12 ? _monthNames[month - 1] : string.Empty;

    public int GetPersianYear(DateTime date) => _pc.GetYear(date);
    public int GetPersianMonth(DateTime date) => _pc.GetMonth(date);
    public int GetPersianDay(DateTime date) => _pc.GetDayOfMonth(date);

    public string FormatCurrency(decimal amount, bool showToman = false)
    {
        if (showToman)
            return $"{amount / 10:N0} تومان";
        return $"{amount:N0} ریال";
    }

    public string NumberToWords(decimal number)
    {
        if (number == 0) return "صفر";
        if (number < 0) return "منفی " + NumberToWords(-number);

        var parts = new List<string>();
        var n = (long)number;

        if (n >= 1_000_000_000) { parts.Add(NumberToWords(n / 1_000_000_000m) + " میلیارد"); n %= 1_000_000_000; }
        if (n >= 1_000_000)     { parts.Add(NumberToWords(n / 1_000_000m) + " میلیون"); n %= 1_000_000; }
        if (n >= 1_000)         { parts.Add(NumberToWords(n / 1_000m) + " هزار"); n %= 1_000; }
        if (n >= 100)           { parts.Add(Hundreds(n / 100)); n %= 100; }
        if (n > 0)              { parts.Add(BelowHundred(n)); }

        return string.Join(" و ", parts);
    }

    private static string Hundreds(long h) => h switch
    {
        1=>"یکصد",2=>"دویست",3=>"سیصد",4=>"چهارصد",5=>"پانصد",
        6=>"ششصد",7=>"هفتصد",8=>"هشتصد",9=>"نهصد",_=>""
    };

    private static string BelowHundred(long n)
    {
        if (n <= 20) return new[]{"","یک","دو","سه","چهار","پنج","شش","هفت","هشت","نه","ده",
            "یازده","دوازده","سیزده","چهارده","پانزده","شانزده","هفده","هجده","نوزده","بیست"}[n];
        string[] tens = ["","","بیست","سی","چهل","پنجاه","شصت","هفتاد","هشتاد","نود"];
        var result = tens[n / 10];
        if (n % 10 > 0) result += " و " + BelowHundred(n % 10);
        return result;
    }
}
