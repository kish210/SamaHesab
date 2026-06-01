using SamaHesab.WPF.ViewModels.Settings;
using System.Windows.Controls;
namespace SamaHesab.WPF.Views.Settings;
public partial class UserManagementView : UserControl
{
    public UserManagementView(UserManagementViewModel viewModel) { InitializeComponent(); DataContext = viewModel; Loaded += async (_, _) => await viewModel.LoadAsync(); }
}
