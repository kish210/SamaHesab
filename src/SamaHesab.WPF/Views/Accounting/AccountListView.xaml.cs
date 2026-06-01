using SamaHesab.WPF.ViewModels.Accounting;
using System.Windows.Controls;
namespace SamaHesab.WPF.Views.Accounting;
public partial class AccountListView : UserControl
{
    public AccountListView(AccountListViewModel viewModel) { InitializeComponent(); DataContext = viewModel; Loaded += async (_, _) => await viewModel.LoadAsync(); }
}
