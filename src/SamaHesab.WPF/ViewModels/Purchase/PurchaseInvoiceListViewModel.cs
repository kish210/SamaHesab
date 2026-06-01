using CommunityToolkit.Mvvm.ComponentModel;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
using System.Collections.ObjectModel;
namespace SamaHesab.WPF.ViewModels.Purchase;
public partial class PurchaseInvoiceListViewModel : BaseViewModel
{
    [ObservableProperty] private string _fromDate = string.Empty;
    [ObservableProperty] private string _toDate = string.Empty;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private decimal _totalAmount;
    public ObservableCollection<PurchaseInvoiceListItem> Invoices { get; } = new();
    public PurchaseInvoiceListViewModel(IPersianCalendarService calendar, IDialogService d, INavigationService n) : base(d, n) { }
    public override async Task LoadAsync() { await Task.CompletedTask; }
}
public record PurchaseInvoiceListItem(int Id, string Number, string Date, string SupplierName, decimal Total, string Status);
