using SamaHesab.WPF.ViewModels.CRM;
using System.Windows.Controls;
namespace SamaHesab.WPF.Views.CRM;
public partial class CustomerListView : UserControl
{
    public CustomerListView(CustomerListViewModel viewModel) { InitializeComponent(); DataContext = viewModel; Loaded += async (_, _) => await viewModel.LoadAsync(); }
}
