namespace SamaHesab.Domain.Enums;

public enum InvoiceType
{
    Sale,           // فروش
    SaleReturn,     // برگشت از فروش
    Quotation,      // پیش‌فاکتور
    Consignment     // حواله
}

public enum InvoiceStatus
{
    Draft,      // پیش‌نویس
    Confirmed,  // تأیید شده
    Posted,     // قطعی
    Cancelled   // لغو شده
}

public enum PriceLevel
{
    Retail,     // خرده
    Wholesale,  // عمده
    Special,    // ویژه
    Custom      // سفارشی
}

public enum PurchaseInvoiceType
{
    Purchase,        // خرید
    PurchaseReturn   // برگشت از خرید
}
