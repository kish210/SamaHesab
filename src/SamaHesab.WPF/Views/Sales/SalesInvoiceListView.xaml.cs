using SamaHesab.WPF.ViewModels.Sales;
using System.Windows.Controls;
namespace SamaHesab.WPF.Views.Sales;
public partial class SalesInvoiceListView : UserControl
{
    public SalesInvoiceListView(SalesInvoiceListViewModel viewModel) { InitializeComponent(); DataContext = viewModel; Loaded += async (_, _) => await viewModel.LoadAsync(); }
}
