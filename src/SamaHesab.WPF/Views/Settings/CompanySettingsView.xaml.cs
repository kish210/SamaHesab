using SamaHesab.WPF.ViewModels.Settings;
using System.Windows.Controls;
namespace SamaHesab.WPF.Views.Settings;
public partial class CompanySettingsView : UserControl
{
    public CompanySettingsView(CompanySettingsViewModel viewModel) { InitializeComponent(); DataContext = viewModel; Loaded += async (_, _) => await viewModel.LoadAsync(); }
}
