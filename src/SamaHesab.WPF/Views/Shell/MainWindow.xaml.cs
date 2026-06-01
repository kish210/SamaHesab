using MahApps.Metro.Controls;
using SamaHesab.WPF.ViewModels.Shell;

namespace SamaHesab.WPF.Views.Shell;

public partial class MainWindow : MetroWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.LoadAsync();
    }
}
