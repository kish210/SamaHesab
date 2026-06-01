using SamaHesab.WPF.ViewModels.Purchase;
using System.Windows.Controls;
namespace SamaHesab.WPF.Views.Purchase;
public partial class PurchaseInvoiceEditView : UserControl
{
    public PurchaseInvoiceEditView(PurchaseInvoiceEditViewModel viewModel)
    { InitializeComponent(); DataContext = viewModel; Loaded += async (_, _) => await viewModel.LoadAsync(); }
}
