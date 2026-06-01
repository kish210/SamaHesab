using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Domain.Interfaces.Repositories;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;

// ─── Accounting ───────────────────────────────────────────────────────────────
namespace SamaHesab.WPF.ViewModels.Accounting
{
    public partial class VoucherListViewModel : BaseViewModel
    {
        private readonly IVoucherRepository _repo;
        private readonly ICurrentUserService _user;
        [ObservableProperty] private string _fromDate = string.Empty;
        [ObservableProperty] private string _toDate = string.Empty;
        [ObservableProperty] private string _searchText = string.Empty;

        public ObservableCollection<Domain.Entities.Accounting.Voucher> Vouchers { get; } = new();

        public VoucherListViewModel(IVoucherRepository repo, ICurrentUserService user,
            IDialogService d, INavigationService n) : base(d, n)
        { _repo = repo; _user = user; }

        public override async Task LoadAsync()
        {
            var all = await _repo.GetByDateRangeAsync(
                _user.CompanyId ?? 1, 1, "1403/01/01", "1403/12/29");
            Vouchers.Clear();
            foreach (var v in all) Vouchers.Add(v);
        }

        [RelayCommand] private void AddNew() => _navigationService.NavigateTo("VoucherEdit");
        [RelayCommand] private void Edit(Domain.Entities.Accounting.Voucher? v) => _navigationService.NavigateTo("VoucherEdit");
    }

    public partial class BankAccountViewModel : BaseViewModel
    {
        public BankAccountViewModel(IDialogService d, INavigationService n) : base(d, n) { }
        public override Task LoadAsync() => Task.CompletedTask;
    }

    public partial class ChequeListViewModel : BaseViewModel
    {
        private readonly IChequeRepository _repo;
        private readonly ICurrentUserService _user;
        [ObservableProperty] private string _selectedStatus = "همه";

        public ObservableCollection<Domain.Entities.Accounting.Cheque> Cheques { get; } = new();
        public List<string> Statuses { get; } = new()
            { "همه", "در جریان", "وصول شده", "برگشت خورده", "واگذار شده", "ابطال شده" };

        public ChequeListViewModel(IChequeRepository repo, ICurrentUserService user,
            IDialogService d, INavigationService n) : base(d, n)
        { _repo = repo; _user = user; }

        public override async Task LoadAsync()
        {
            var all = await _repo.FindAsync(c => c.CompanyId == (_user.CompanyId ?? 1));
            Cheques.Clear();
            foreach (var c in all) Cheques.Add(c);
        }

        [RelayCommand] private async Task AddAsync() => await _dialogService.ShowInfoAsync("فرم ثبت چک جدید");

        [RelayCommand]
        private async Task ClearChequeAsync(Domain.Entities.Accounting.Cheque? c) =>
            await _dialogService.ShowSuccessAsync("چک وصول شد.");

        [RelayCommand]
        private async Task ReturnChequeAsync(Domain.Entities.Accounting.Cheque? c) =>
            await _dialogService.ShowWarningAsync("چک برگشت خورد.");
    }
}

// ─── Inventory ────────────────────────────────────────────────────────────────
namespace SamaHesab.WPF.ViewModels.Inventory
{
    public partial class ProductListViewModel : BaseViewModel
    {
        private readonly IProductRepository _repo;
        private readonly ICurrentUserService _user;
        [ObservableProperty] private string _searchText = string.Empty;

        public ObservableCollection<Domain.Entities.Inventory.Product> Products { get; } = new();

        public ProductListViewModel(IProductRepository repo, ICurrentUserService user,
            IDialogService d, INavigationService n) : base(d, n)
        { _repo = repo; _user = user; }

        public override async Task LoadAsync()
        {
            var all = await _repo.FindAsync(p =>
                p.CompanyId == (_user.CompanyId ?? 1) && p.IsActive);
            Products.Clear();
            foreach (var p in all.OrderBy(x => x.Code)) Products.Add(p);
        }

        [RelayCommand] private void AddNew() => _navigationService.NavigateTo("ProductEdit");
        [RelayCommand] private void Edit(Domain.Entities.Inventory.Product? p) => _navigationService.NavigateTo("ProductEdit");

        [RelayCommand]
        private async Task DeleteAsync(Domain.Entities.Inventory.Product? p)
        {
            if (p == null) return;
            var ok = await _dialogService.ConfirmAsync($"کالا '{p.Name}' حذف شود؟");
            if (ok) { _repo.Remove(p); Products.Remove(p); }
        }

        partial void OnSearchTextChanged(string v) => _ = LoadAsync();
    }

    public partial class WarehouseViewModel : BaseViewModel
    {
        private readonly IWarehouseRepository _repo;
        private readonly ICurrentUserService _user;

        public ObservableCollection<Domain.Entities.Inventory.Warehouse> Warehouses { get; } = new();

        public WarehouseViewModel(IWarehouseRepository repo, ICurrentUserService user,
            IDialogService d, INavigationService n) : base(d, n)
        { _repo = repo; _user = user; }

        public override async Task LoadAsync()
        {
            var all = await _repo.GetByCompanyAsync(_user.CompanyId ?? 1);
            Warehouses.Clear();
            foreach (var w in all) Warehouses.Add(w);
        }

        [RelayCommand]
        private async Task AddAsync() => await _dialogService.ShowInfoAsync("فرم ایجاد انبار جدید");
    }

    public partial class StockAdjustViewModel : BaseViewModel
    {
        private readonly IProductRepository _productRepo;
        private readonly ICurrentUserService _user;
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private int _selectedWarehouseId;

        public ObservableCollection<StockAdjustRow> Rows { get; } = new();

        public StockAdjustViewModel(IProductRepository productRepo, ICurrentUserService user,
            IDialogService d, INavigationService n) : base(d, n)
        { _productRepo = productRepo; _user = user; }

        public override Task LoadAsync() => Task.CompletedTask;

        [RelayCommand]
        private async Task SaveAsync() =>
            await _dialogService.ShowSuccessAsync("تعدیل موجودی ذخیره شد.");
    }

    public record StockAdjustRow(int ProductId, string ProductCode, string ProductName,
        decimal CurrentQty, decimal NewQty, string? Notes);
}

// ─── Sales ────────────────────────────────────────────────────────────────────
namespace SamaHesab.WPF.ViewModels.Sales
{
    public partial class SalesInvoiceListViewModel : BaseViewModel
    {
        private readonly ICurrentUserService _user;
        [ObservableProperty] private string _fromDate = string.Empty;
        [ObservableProperty] private string _toDate = string.Empty;
        [ObservableProperty] private string _searchText = string.Empty;

        public ObservableCollection<Domain.Entities.Sales.SalesInvoice> Invoices { get; } = new();

        public SalesInvoiceListViewModel(ICurrentUserService user,
            IDialogService d, INavigationService n) : base(d, n)
        { _user = user; }

        public override Task LoadAsync() => Task.CompletedTask;

        [RelayCommand] private void NewInvoice() => _navigationService.NavigateTo("SalesInvoice");
    }
}

// ─── CRM ─────────────────────────────────────────────────────────────────────
namespace SamaHesab.WPF.ViewModels.CRM
{
    public partial class CustomerListViewModel : BaseViewModel
    {
        private readonly ICurrentUserService _user;
        [ObservableProperty] private string _searchText = string.Empty;

        public ObservableCollection<Domain.Entities.CRM.Customer> Customers { get; } = new();

        public CustomerListViewModel(ICurrentUserService user,
            IDialogService d, INavigationService n) : base(d, n)
        { _user = user; }

        public override Task LoadAsync() => Task.CompletedTask;

        [RelayCommand] private void AddNew() => _navigationService.NavigateTo("CustomerEdit");
        [RelayCommand] private void Edit(Domain.Entities.CRM.Customer? c) => _navigationService.NavigateTo("CustomerEdit");
    }

    public partial class SupplierListViewModel : BaseViewModel
    {
        private readonly ICurrentUserService _user;
        [ObservableProperty] private string _searchText = string.Empty;

        public ObservableCollection<Domain.Entities.CRM.Supplier> Suppliers { get; } = new();

        public SupplierListViewModel(ICurrentUserService user,
            IDialogService d, INavigationService n) : base(d, n)
        { _user = user; }

        public override Task LoadAsync() => Task.CompletedTask;

        [RelayCommand] private void AddNew() => _navigationService.NavigateTo("SupplierEdit");
    }
}
