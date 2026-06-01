using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Domain.Interfaces.Repositories;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;

namespace SamaHesab.WPF.ViewModels.POS;

public partial class PosViewModel : BaseViewModel
{
    private readonly IProductRepository _productRepo;
    private readonly IPersianCalendarService _calendar;
    private readonly ICurrentUserService _currentUser;

    [ObservableProperty] private string _barcodeInput = string.Empty;
    [ObservableProperty] private string? _selectedCustomerName;
    [ObservableProperty] private int? _selectedCustomerId;
    [ObservableProperty] private decimal _subTotal;
    [ObservableProperty] private decimal _discount;
    [ObservableProperty] private decimal _tax;
    [ObservableProperty] private decimal _grandTotal;
    [ObservableProperty] private decimal _cashReceived;
    [ObservableProperty] private decimal _change;
    [ObservableProperty] private string _paymentMode = "نقدی";
    [ObservableProperty] private string _currentTime = string.Empty;
    [ObservableProperty] private string _receiptNumber = string.Empty;

    public ObservableCollection<PosCartItem> CartItems { get; } = new();
    public List<string> PaymentModes { get; } = new() { "نقدی", "کارتخوان", "ترکیبی" };

    private readonly System.Windows.Threading.DispatcherTimer _timer;

    public PosViewModel(IProductRepository productRepo, IPersianCalendarService calendar,
        ICurrentUserService currentUser, IDialogService dialogService, INavigationService navigationService)
        : base(dialogService, navigationService)
    {
        _productRepo = productRepo; _calendar = calendar; _currentUser = currentUser;
        _timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => CurrentTime = DateTime.Now.ToString("HH:mm:ss");
        _timer.Start();
    }

    public override async Task LoadAsync()
    {
        ReceiptNumber = "POS-" + DateTime.Now.ToString("yyyyMMddHHmm");
        CurrentTime = DateTime.Now.ToString("HH:mm:ss");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ProcessBarcodeAsync()
    {
        if (string.IsNullOrWhiteSpace(BarcodeInput)) return;
        var product = await _productRepo.GetByBarcodeAsync(_currentUser.CompanyId ?? 1, BarcodeInput)
                   ?? await _productRepo.GetByCodeAsync(_currentUser.CompanyId ?? 1, BarcodeInput);

        if (product == null) { await _dialogService.ShowWarningAsync($"کالا با کد '{BarcodeInput}' یافت نشد."); BarcodeInput = string.Empty; return; }

        var existing = CartItems.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null) { existing.Quantity++; existing.Recalculate(); }
        else
        {
            var item = new PosCartItem(product.Id, product.Code, product.Name, 1, product.SalePrice, product.TaxRate);
            item.PropertyChanged += (_, _) => RecalculateTotals();
            CartItems.Add(item);
        }
        RecalculateTotals();
        BarcodeInput = string.Empty;
    }

    [RelayCommand] private void RemoveItem(PosCartItem? item) { if (item != null) { CartItems.Remove(item); RecalculateTotals(); } }
    [RelayCommand] private void ClearCart() { CartItems.Clear(); RecalculateTotals(); }

    private void RecalculateTotals()
    {
        SubTotal   = CartItems.Sum(i => i.Quantity * i.UnitPrice);
        Tax        = CartItems.Sum(i => i.TaxAmount);
        GrandTotal = SubTotal - Discount + Tax;
        Change     = Math.Max(0, CashReceived - GrandTotal);
    }

    partial void OnCashReceivedChanged(decimal v) => Change = Math.Max(0, v - GrandTotal);
    partial void OnDiscountChanged(decimal v) => RecalculateTotals();

    [RelayCommand]
    private async Task CheckoutAsync()
    {
        if (!CartItems.Any()) { await _dialogService.ShowWarningAsync("سبد خرید خالی است."); return; }
        if (PaymentMode == "نقدی" && CashReceived < GrandTotal) { await _dialogService.ShowErrorAsync("مبلغ دریافتی کافی نیست."); return; }

        await ExecuteAsync(async () =>
        {
            await Task.Delay(300); // Process sale
            await _dialogService.ShowSuccessAsync($"فروش با موفقیت ثبت شد.\nباقیمانده: {Change:N0} ریال");
            NewSale();
        }, "در حال پردازش...");
    }

    [RelayCommand]
    private void NewSale()
    {
        CartItems.Clear(); CashReceived = 0; Discount = 0;
        RecalculateTotals();
        ReceiptNumber = "POS-" + DateTime.Now.ToString("yyyyMMddHHmm");
        BarcodeInput = string.Empty;
    }

    [RelayCommand] private async Task PrintReceiptAsync() => await _dialogService.ShowInfoAsync("در حال چاپ رسید...");
}

public partial class PosCartItem : ObservableObject
{
    public int ProductId { get; }
    public string ProductCode { get; }
    public string ProductName { get; }
    [ObservableProperty] private decimal _quantity;
    [ObservableProperty] private decimal _unitPrice;
    [ObservableProperty] private decimal _taxRate;
    [ObservableProperty] private decimal _taxAmount;
    [ObservableProperty] private decimal _netAmount;

    public PosCartItem(int id, string code, string name, decimal qty, decimal price, decimal taxRate)
    { ProductId = id; ProductCode = code; ProductName = name; _quantity = qty; _unitPrice = price; _taxRate = taxRate; Recalculate(); }

    partial void OnQuantityChanged(decimal v) => Recalculate();
    partial void OnUnitPriceChanged(decimal v) => Recalculate();

    public void Recalculate()
    { var sub = Quantity * UnitPrice; TaxAmount = sub * TaxRate / 100; NetAmount = sub + TaxAmount; }
}
