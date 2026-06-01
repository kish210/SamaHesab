using SamaHesab.WPF.ViewModels.Purchase;
using System.Windows.Controls;
namespace SamaHesab.WPF.Views.Purchase;
public partial class PurchaseInvoiceListView : UserControl
{
    public PurchaseInvoiceListView(PurchaseInvoiceListViewModel viewModel) { InitializeComponent(); DataContext = viewModel; Loaded += async (_, _) => await viewModel.LoadAsync(); }
}
