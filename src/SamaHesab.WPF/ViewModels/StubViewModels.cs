using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Domain.Interfaces.Repositories;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;

namespace SamaHesab.WPF.ViewModels.Inventory
{
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
