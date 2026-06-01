namespace SamaHesab.Domain.Enums;

public enum ValuationMethod
{
    WeightedAverage,  // میانگین وزنی
    FIFO              // اول وارد اول خارج
}

public enum StockTransactionEffect
{
    Entry = 1,   // ورود
    Exit = -1,   // خروج
    Neutral = 0  // خنثی
}

public enum ProductType
{
    Product,  // کالا
    Service,  // خدمات
    Bundle    // مجموعه
}

public enum SerialStatus
{
    Available,  // موجود
    Sold,       // فروخته شده
    Defective   // معیوب
}
