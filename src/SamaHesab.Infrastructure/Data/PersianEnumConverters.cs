using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SamaHesab.Domain.Enums;

namespace SamaHesab.Infrastructure.Data;

// The SQL scripts store these enums as Persian NVARCHAR values, so EF must
// convert between the C# enum and the exact Persian string used in the database.

public class AccountNatureToPersianConverter : ValueConverter<AccountNature, string>
{
    public AccountNatureToPersianConverter() : base(
        v => v == AccountNature.Credit ? "بستانکار" : "بدهکار",
        s => s == "بستانکار" ? AccountNature.Credit : AccountNature.Debit)
    { }
}

public class ChequeStatusToPersianConverter : ValueConverter<ChequeStatus, string>
{
    public ChequeStatusToPersianConverter() : base(
        v => ToPersian(v),
        s => FromPersian(s))
    { }

    private static string ToPersian(ChequeStatus v) => v switch
    {
        ChequeStatus.Cleared     => "وصول شده",
        ChequeStatus.Returned    => "برگشت خورده",
        ChequeStatus.Transferred => "واگذار شده",
        ChequeStatus.Cancelled   => "ابطال شده",
        _                        => "در جریان"
    };

    private static ChequeStatus FromPersian(string s) => s switch
    {
        "وصول شده"     => ChequeStatus.Cleared,
        "برگشت خورده"  => ChequeStatus.Returned,
        "واگذار شده"   => ChequeStatus.Transferred,
        "ابطال شده"    => ChequeStatus.Cancelled,
        _              => ChequeStatus.InProcess
    };
}

public class ChequeTypeToPersianConverter : ValueConverter<ChequeType, string>
{
    public ChequeTypeToPersianConverter() : base(
        v => v == ChequeType.Paid ? "پرداختی" : "دریافتی",
        s => s == "پرداختی" ? ChequeType.Paid : ChequeType.Received)
    { }
}
