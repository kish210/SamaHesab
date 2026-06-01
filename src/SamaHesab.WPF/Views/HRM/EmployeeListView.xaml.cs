using SamaHesab.WPF.ViewModels.HRM;
using System.Windows.Controls;
namespace SamaHesab.WPF.Views.HRM;
public partial class EmployeeListView : UserControl
{
    public EmployeeListView(EmployeeListViewModel viewModel) { InitializeComponent(); DataContext = viewModel; Loaded += async (_, _) => await viewModel.LoadAsync(); }
}
