using System.Windows;
using SamaHesab.WPF.ViewModels.Shell;

namespace SamaHesab.WPF.Views.Shell;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _vm;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _vm = viewModel;
        DataContext = viewModel;
        Resources["BoolVis"] = new System.Windows.Controls.BooleanToVisibilityConverter();
        Loaded += (_, _) => TxtUsername?.Focus();
    }

    private void PwdBox_PasswordChanged(object sender, RoutedEventArgs e) =>
        _vm.Password = PwdBox.Password;

    private void CloseButton_Click(object sender, RoutedEventArgs e) =>
        System.Windows.Application.Current.Shutdown();
}
