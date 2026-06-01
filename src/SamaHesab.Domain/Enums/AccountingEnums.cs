namespace SamaHesab.Domain.Enums;

public enum VoucherStatus
{
    Draft = 1,       // پیش‌نویس
    Posted = 2,      // قطعی
    Permanent = 3    // دائمی
}

public enum AccountLevel
{
    Group = 1,       // گروه
    General = 2,     // کل
    Subsidiary = 3,  // معین
    Detail = 4       // تفصیلی
}

public enum AccountNature
{
    Debit,   // بدهکار
    Credit   // بستانکار
}

public enum ChequeType
{
    Received,  // دریافتی
    Paid       // پرداختی
}

public enum ChequeStatus
{
    InProcess,    // در جریان
    Cleared,      // وصول شده
    Returned,     // برگشت خورده
    Transferred,  // واگذار شده
    Cancelled     // ابطال شده
}

public enum PaymentType
{
    Cash,         // نقدی
    Card,         // کارتخوان
    Cheque,       // چک
    Transfer,     // حواله
    Installment   // اقساط
}
